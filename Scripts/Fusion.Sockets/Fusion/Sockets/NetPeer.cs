#define DEBUG
using System;
using System.Runtime.CompilerServices;
using Fusion.Profiling;

namespace Fusion.Sockets;

public struct NetPeer
{
	private struct ThreadStartArgs
	{
		public unsafe NetPeer* Peer;

		public INetSocket Socket;
	}

	public const int DEFAULT_HEADERS = 144;

	public const int MIN_MTU_BYTES = 432;

	public const int MIN_MTU_BITS = 3456;

	public const int MAX_MTU_BYTES = 1136;

	public const int MAX_MTU_BITS = 9088;

	internal const int FRAG_MAX_COUNT = 127;

	internal const byte FRAG_END_BIT = 128;

	private const int STATE_RUNNING = 0;

	private const int STATE_SHUTDOWN = 2;

	private volatile int _state;

	private NetConfig _config;

	private Timer _recvTimer;

	private unsafe byte* _fragmentBuffer;

	internal NetSocket _socket;

	private NetAddress _address;

	private NetBitBufferStack _sendStack;

	private unsafe NetPeerGroup* _groups;

	private unsafe NetPeerGroupMap* _groupsMap;

	private unsafe int* _groupsAssigned;

	private unsafe NetCommandRefused* _refusedCommand;

	private unsafe NetBitBuffer* _recv;

	private unsafe NetBitBufferBlock* _recvBlock;

	private Timer _delayedClock;

	private NetDelayedPacketList _delayedPackets;

	public NetAddress Address => _address;

	public NetConfig Config => _config;

	public int GroupCount => _config.ConnectionGroups;

	public bool IsShutdown => _state == 2;

	public unsafe static NetConfig* GetConfigPointer(NetPeer* p)
	{
		if (p->_state == 2)
		{
			return null;
		}
		return &p->_config;
	}

	public unsafe static NetPeerGroup* GetGroup(NetPeer* p, int index)
	{
		if (p->_state == 2)
		{
			return null;
		}
		Assert.Check((uint)index < (uint)p->_config.ConnectionGroups);
		return p->_groups + index;
	}

	public unsafe static void Update(NetPeer* p, INetSocket socket, Random rng)
	{
		bool flag = false;
		Update(p, socket, &flag, rng);
	}

	public unsafe static void Update(NetPeer* p, INetSocket socket, bool* work, Random rng)
	{
		if (p->_state != 2)
		{
			if (p->_state != 0)
			{
				Log.Error("Can't call Update on NetPeer which is running or has been running on a thread");
				return;
			}
			RecvInternal(p, socket, work, rng);
			SendInternal(p, socket, work);
		}
	}

	public unsafe static void Recv(NetPeer* p, INetSocket socket, Random rng)
	{
		bool flag = false;
		Recv(p, socket, &flag, rng);
	}

	public unsafe static void Recv(NetPeer* p, INetSocket socket, bool* work, Random rng)
	{
		if (p->_state != 2)
		{
			if (p->_state != 0)
			{
				Log.Error("Can't call Update on NetPeer which is running or has been running on a thread");
			}
			else
			{
				RecvInternal(p, socket, work, rng);
			}
		}
	}

	public unsafe static void RemapAddress(NetPeer* p, NetAddress oldAddress, NetAddress newAddress)
	{
		int num = p->_groupsMap->Remove(oldAddress);
		Assert.Check(num >= 0);
		p->_groupsMap->Insert(newAddress, 0);
	}

	public unsafe static void Send(NetPeer* p, INetSocket socket)
	{
		if (p->_state != 2)
		{
			bool flag = false;
			Send(p, socket, &flag);
		}
	}

	public unsafe static void Send(NetPeer* p, INetSocket socket, bool* work)
	{
		if (p->_state != 2)
		{
			if (p->_state != 0)
			{
				Log.Error("Can't call Update on NetPeer which is running or has been running on a thread");
			}
			else
			{
				SendInternal(p, socket, work);
			}
		}
	}

	public unsafe static NetPeer* Initialize(NetConfig config, INetSocket socket)
	{
		NetPeer* ptr = Native.MallocAndClear<NetPeer>();
		Initialize(ptr, config, socket);
		return ptr;
	}

	public unsafe static void Initialize(NetPeer* p, NetConfig config, INetSocket socket)
	{
		config.MaxConnections = Maths.Clamp(config.MaxConnections, 1, 2048);
		socket.Initialize(config);
		p->_config = config;
		p->_state = 0;
		p->_recvTimer = default;
		p->_fragmentBuffer = (byte*)Native.MallocAndClear(1136);
		p->_refusedCommand = Native.MallocAndClear<NetCommandRefused>();
		p->_delayedClock = Timer.StartNew();
		p->_delayedPackets = default;
		p->_sendStack = NetBitBufferStack.Create(2048);
		p->_recvBlock = NetBitBufferBlock.Create(config.PacketSize);
		p->_socket = socket.Create(config);
		p->_groupsMap = NetPeerGroupMap.Allocate(config.MaxConnections);
		p->_groups = Native.MallocAndClearArray<NetPeerGroup>(config.ConnectionGroups);
		p->_groupsAssigned = Native.MallocAndClearArray<int>(config.ConnectionGroups);
		for (short num = 0; num < config.ConnectionGroups; num++)
		{
			NetPeerGroup.Initialize(num, p->_groups + num, p, config);
		}
		p->_address = socket.Bind(p->_socket, p->_config);
	}

	public unsafe static void Destroy(NetPeer* p, INetSocket socket, INetPeerGroupCallbacks callbacks)
	{
		if (p->_state == 0)
		{
			p->_state = 2;
			DestroySocket(p, socket, callbacks);
		}
	}

	private unsafe static void DestroySocket(NetPeer* p, INetSocket socket, INetPeerGroupCallbacks callbacks)
	{
		if (p != null && p->_socket.IsCreated)
		{
			NetBitBufferStack.Free(p->_sendStack);
			p->_sendStack = default;
			while (p->_delayedPackets.Count > 0)
			{
				NetDelayedPacket.Free(p->_delayedPackets.RemoveHead());
			}
			for (int i = 0; i < p->GroupCount; i++)
			{
				NetPeerGroup.Dispose(p->_groups + i, callbacks);
			}
			if (p->_recv != null)
			{
				NetBitBuffer.Release(p->_recv);
				p->_recv = null;
			}
			if (p->_recvBlock != null)
			{
				NetBitBufferBlock.Dispose(p->_recvBlock);
				p->_recvBlock = null;
			}
			if (p->_groupsMap != null)
			{
				NetPeerGroupMap.Dispose(p->_groupsMap);
				p->_groupsMap = null;
			}
			if (p->_groupsAssigned != null)
			{
				Native.Free(p->_groupsAssigned);
				p->_groupsAssigned = null;
			}
			if (p->_refusedCommand != null)
			{
				Native.Free(p->_refusedCommand);
				p->_refusedCommand = null;
			}
			if (p->_fragmentBuffer != null)
			{
				Native.Free(p->_fragmentBuffer);
				p->_fragmentBuffer = null;
			}
			if (p->_groups != null)
			{
				Native.Free(p->_groups);
				p->_groups = null;
			}
			socket.Destroy(p->_socket);
			p->_socket = default;
			Native.Free(p);
		}
	}

	private unsafe static short FindGroupWithLeastAssignedAddresses(NetPeer* p)
	{
		short result = -1;
		int num = p->_config.ConnectionsPerGroup;
		for (short num2 = 0; num2 < p->_config.ConnectionGroups; num2++)
		{
			if (p->_groupsAssigned[num2] < num)
			{
				result = num2;
				num = p->_groupsAssigned[num2];
			}
		}
		return result;
	}

	private unsafe static void RecvInternal(NetPeer* p, INetSocket socket, bool* work, Random rng, FusionSampler sampler = null)
	{
		p->_recvTimer.Restart();
		RecvDelayed(p, socket, work, rng);
		if (RecvExpired(p))
		{
			return;
		}
		int lengthBytes;
		while (RecvBufferAvailable(p) && (lengthBytes = socket.Receive(p->_socket, &p->_recv->Address, (byte*)p->_recv->Data, p->_config.PacketSize)) > 0)
		{
			*work = true;
			p->_recv->LengthBytes = lengthBytes;
			if (p->_config.Simulation.LossNotifySequencesLength > 0 && p->_recv->PacketType == NetPacketType.NotifyData)
			{
				Assert.Check(p->_config.Simulation.LossNotifySequences != null);
				ushort sequence = ((NetNotifyHeader*)p->_recv->Data)->Sequence;
				bool flag = false;
				for (int i = 0; i < p->_config.Simulation.LossNotifySequencesLength; i++)
				{
					if (p->_config.Simulation.LossNotifySequences[i] == sequence)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					continue;
				}
			}
			NetConfigSimulationOscillator lossOscillator = p->_config.Simulation.LossOscillator;
			if (lossOscillator.Min > 0.0 && lossOscillator.Min <= lossOscillator.Max)
			{
				double curveValue = lossOscillator.GetCurveValue(rng, p->_delayedClock.ElapsedInSeconds);
				if (rng.NextDouble() <= curveValue)
				{
					continue;
				}
			}
			NetConfigSimulationOscillator delayOscillator = p->_config.Simulation.DelayOscillator;
			if (delayOscillator.Min > 0.0 && delayOscillator.Min <= delayOscillator.Max)
			{
				NetDelayedPacket* ptr = NetDelayedPacket.Create(p->_recv->LengthBytes);
				Native.MemCpy(ptr->Data, p->_recv->Data, p->_recv->LengthBytes);
				ptr->Address = p->_recv->Address;
				double curveValue2 = delayOscillator.GetCurveValue(rng, p->_delayedClock.ElapsedInSeconds);
				ptr->DeliveryTime = p->_delayedClock.ElapsedInSeconds + curveValue2;
				if (curveValue2 > 0.0)
				{
					p->_delayedPackets.AddLast(ptr);
					continue;
				}
			}
			RecvBufferPushToGroup(p, socket, rng);
			if (RecvExpired(p))
			{
				break;
			}
		}
	}

	private unsafe static void RecvBufferPushToGroup(NetPeer* p, INetSocket socket, Random rng)
	{
		Assert.Check(p->_recv != null);
		Assert.Check(!p->_recv->Address.Equals(default));
		short num = p->_groupsMap->Find(p->_recv->Address);
		if (num == -1)
		{
			NetCommandHeader data = *(NetCommandHeader*)p->_recv->Data;
			if (data.PacketType != NetPacketType.Command || data.Command != NetCommands.Connect)
			{
				return;
			}
			num = FindGroupWithLeastAssignedAddresses(p);
			if (num == -1)
			{
				*p->_refusedCommand = NetCommandRefused.Create(NetConnectFailedReason.ServerFull);
				socket.Send(p->_socket, &p->_recv->Address, (byte*)p->_refusedCommand, 3);
				return;
			}
			Assert.Check(p->_groupsAssigned[num] >= 0 && p->_groupsAssigned[num] < p->_config.ConnectionsPerGroup);
			if (!p->_groupsMap->Insert(p->_recv->Address, num))
			{
				return;
			}
			p->_groupsAssigned[num]++;
		}
		Assert.Check(num >= 0 && num <= p->_config.ConnectionGroups);
		if (p->_config.Simulation.DuplicateChance > 0.0 && rng.NextDouble() <= p->_config.Simulation.DuplicateChance && p->_recvBlock->TryAcquire(out var ptr))
		{
			ptr->Address = p->_recv->Address;
			ptr->LengthBytes = p->_recv->LengthBytes;
			Native.MemCpy(ptr->Data, p->_recv->Data, p->_recv->LengthBytes);
			NetPeerGroup.PushOnRecvHead(p->_groups + num, ptr);
		}
		NetPeerGroup.PushOnRecvHead(p->_groups + num, p->_recv);
		p->_recv = null;
	}

	private unsafe static void RecvDelayed(NetPeer* p, INetSocket socket, bool* work, Random rng)
	{
		while (p->_delayedPackets.Count > 0 && p->_delayedPackets.Head->DeliveryTime < p->_delayedClock.ElapsedInSeconds && RecvBufferAvailable(p) && !RecvExpired(p))
		{
			*work = true;
			NetDelayedPacket* ptr = p->_delayedPackets.RemoveHead();
			Native.MemCpy(p->_recv->Data, ptr->Data, ptr->DataLength);
			p->_recv->Address = ptr->Address;
			p->_recv->LengthBytes = ptr->DataLength;
			RecvBufferPushToGroup(p, socket, rng);
			Native.Free(ptr);
		}
	}

	private unsafe static void SendInternal(NetPeer* p, INetSocket socket, bool* work)
	{
		SendFromStack(p, socket, work);
		Assert.Check(p->_sendStack.Count == 0);
		for (int i = 0; i < p->_config.ConnectionGroups; i++)
		{
			IntPtr intPtr = NetPeerGroup.PopSendHead(p->_groups + i);
			if (!(intPtr == IntPtr.Zero))
			{
				*work = true;
				p->_sendStack.PushFromHead((NetBitBuffer*)(void*)intPtr);
			}
		}
		SendFromStack(p, socket, work);
	}

	private unsafe static void SendFromStack(NetPeer* p, INetSocket socket, bool* work)
	{
		NetBitBuffer* ptr = null;
		while (p->_sendStack.TryPop(&ptr))
		{
			*work = true;
			Assert.Check(!ptr->Address.Equals(default));
			if (ptr->PacketType == NetPacketType.Command)
			{
				NetCommandHeader* data = (NetCommandHeader*)ptr->Data;
				if (data->Command == NetCommands.Connect)
				{
					short num = p->_groupsMap->Find(ptr->Address);
					if (num == -1)
					{
						if (!p->_groupsMap->Insert(ptr->Address, ptr->Group))
						{
							NetBitBuffer.Release(ptr);
							continue;
						}
						p->_groupsAssigned[ptr->Group]++;
					}
				}
			}
			if (ptr->PacketType != NetPacketType.Unconnected)
			{
				Assert.Check((uint)p->_groupsMap->Find(ptr->Address) < (uint)p->_config.ConnectionGroups);
			}
			if (ptr->Group == -1)
			{
				Assert.Check(ptr->OffsetBits == 0);
				int num2 = p->_groupsMap->Remove(ptr->Address);
				Assert.Check((uint)num2 < (uint)p->_config.ConnectionGroups);
				p->_groupsAssigned[num2]--;
				Assert.Check(p->_groupsAssigned[num2] >= 0);
				NetBitBuffer.Release(ptr);
				continue;
			}
			int num3 = ((ptr->Mtu <= 0) ? 432 : ptr->Mtu);
			int num4 = Maths.BytesRequiredForBits(ptr->OffsetBits);
			if (ptr->PacketType == NetPacketType.NotifyData && num4 > num3)
			{
				NetNotifyHeader netNotifyHeader = default;
				Native.MemCpy(&netNotifyHeader, ptr->Data, 14);
				byte* ptr2 = (byte*)ptr->Data + 14;
				int num5 = num4 - 14;
				byte b = 1;
				while (num5 > 0)
				{
					Assert.Check(b >= 1 && b <= 127, b, num5);
					int num6 = Math.Min(num3 - 14, num5);
					num5 -= num6;
					Assert.Check(num5 >= 0);
					netNotifyHeader.Fragment = b;
					if (num5 == 0)
					{
						netNotifyHeader.Fragment |= 128;
					}
					Native.MemCpy(p->_fragmentBuffer, &netNotifyHeader, 14);
					Native.MemCpy(p->_fragmentBuffer + 14, ptr2, num6);
					ptr2 += num6;
					b++;
					socket.Send(p->_socket, &ptr->Address, p->_fragmentBuffer, num6 + 14, reliable: true);
				}
			}
			else
			{
				socket.Send(p->_socket, &ptr->Address, (byte*)ptr->Data, num4);
			}
			if (ptr->PacketType == NetPacketType.Command)
			{
				NetCommandHeader* data2 = (NetCommandHeader*)ptr->Data;
				if (data2->Command == NetCommands.Refused && p->_groupsMap->Find(ptr->Address) != -1)
				{
					int num7 = p->_groupsMap->Remove(ptr->Address);
					Assert.Check((uint)num7 < (uint)p->_config.ConnectionGroups);
					p->_groupsAssigned[num7]--;
					Assert.Check(p->_groupsAssigned[num7] >= 0);
				}
			}
			NetBitBuffer.Release(ptr);
		}
		Assert.Check(p->_sendStack.Count == 0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static bool RecvBufferAvailable(NetPeer* p)
	{
		if (p->_recv == null)
		{
			p->_recv = p->_recvBlock->TryAcquire();
		}
		if (p->_recv != null)
		{
			p->_recv->Address = default;
		}
		return p->_recv != null;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static bool RecvExpired(NetPeer* p)
	{
		return p->_recvTimer.ElapsedInMilliseconds > p->_config.OperationExpireTime;
	}
}

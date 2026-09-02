#define DEBUG
using System;

namespace Fusion.Sockets;

public struct NetConnection
{
	internal struct StateConnectingData
	{
		public int Attempts;

		public double AttemptTimeout;
	}

	internal struct StateShutdownData
	{
		public double Timeout;

		public int Unmapped;
	}

	internal struct StateDisconnectedData
	{
		public NetDisconnectReason Reason;

		public int CallbackInvoked;

		public int SentDisconnectCommand;
	}

	internal const byte UNIQUE_ID_SIZE = 8;

	internal ulong MapHash;

	internal unsafe NetConnection* MapNext;

	internal NetConnectionMap.EntryState MapState;

	internal NetConnectionId LocalId;

	internal NetConnectionId RemoteId;

	internal NetAddress Address;

	internal NetConnectionStatus Status;

	internal double Rtt;

	internal double SendTime;

	internal double RecvTime;

	internal StateConnectingData StateConnecting;

	internal StateDisconnectedData StateDisconnected;

	internal StateShutdownData StateShutdown;

	internal NetSendEnvelopeRingBuffer NotifySendWindow;

	internal NetSequencer NotifySendSequencer;

	internal double NotifySendTime;

	internal double NotifyRecvAckTime;

	internal int NotifyRecvAckOutdatedCount;

	internal double NotifyRecvTime;

	internal ulong NotifyRecvMask;

	internal ushort NotifyRecvSequence;

	internal int NotifyRecvUnackedCount;

	internal int NotifyRecvFragment;

	internal unsafe byte* NotifyRecvFragmentBuffer;

	internal int NotifyRecvFragmentBufferLength;

	internal int NotifyRecvFragmentSequenceDistance;

	internal unsafe byte* ConnectionToken;

	internal int ConnectionTokenLength;

	internal long UniqueIdHash;

	internal unsafe byte* UniqueId;

	internal int Mtu;

	internal uint Counter;

	internal ReliableBuffer ReliableBuffer;

	internal ReliableList ReliableSendList;

	internal TimerDelta ReliableSendTimer;

	public NetConnectionStats StatsRoundTripTime;

	public NetConnectionStats StatsSentPacketSizes;

	public NetConnectionStats StatsReceivedPacketSizes;

	public bool Active => MapState == NetConnectionMap.EntryState.Used;

	public double RoundTripTime => Rtt;

	public NetAddress RemoteAddress => Address;

	public NetConnectionStatus ConnectionStatus => Status;

	public NetConnectionId LocalConnectionId => LocalId;

	public NetConnectionId RemoteConnectionId => RemoteId;

	internal unsafe static ushort NextNotifySendSequence(NetConnection* c)
	{
		ulong num = c->NotifySendSequencer.Next();
		Assert.Check(num <= 65535);
		return (ushort)num;
	}

	internal unsafe static void Initialize(NetConnection* c, short group, short index, NetConfig* config)
	{
		c->LocalId.Group = group;
		c->LocalId.GroupIndex = index;
		c->NotifySendWindow = NetSendEnvelopeRingBuffer.Create(config->Notify.WindowSize);
		c->NotifySendSequencer = new NetSequencer(config->Notify.SequenceBytes);
		c->NotifyRecvFragmentBuffer = (byte*)Native.MallocAndClear(144272);
		c->NotifyRecvFragmentBufferLength = 0;
		c->NotifyRecvFragment = 0;
		c->StatsRoundTripTime = new NetConnectionStats(128);
		c->StatsSentPacketSizes = new NetConnectionStats(128);
		c->StatsReceivedPacketSizes = new NetConnectionStats(128);
		c->Mtu = Math.Max(432, config->DefaultMtu);
		Reset(c);
	}

	internal unsafe static void Reset(NetConnection* c)
	{
		Assert.Check(c->NotifySendWindow.Count == 0);
		c->LocalId.Generation++;
		if (c->LocalId.Generation == 0)
		{
			c->LocalId.Generation++;
		}
		if (c->ConnectionToken != null)
		{
			Native.Free(c->ConnectionToken);
			c->ConnectionToken = null;
			c->ConnectionTokenLength = 0;
		}
		if (c->UniqueId != null)
		{
			Native.Free(c->UniqueId);
			c->UniqueId = null;
			c->UniqueIdHash = 0L;
		}
		while (c->ReliableSendList.Count > 0)
		{
			Native.Free(c->ReliableSendList.RemoveHead());
		}
		c->ReliableSendList = default;
		c->ReliableSendTimer = TimerDelta.StartNew();
		c->ReliableBuffer.Dispose();
		c->ReliableBuffer = ReliableBuffer.Create();
		c->StatsRoundTripTime.Clear();
		c->StatsSentPacketSizes.Clear();
		c->StatsReceivedPacketSizes.Clear();
		c->Address = default;
		c->NotifySendWindow.Reset();
		c->NotifySendSequencer.Reset();
		c->Status = (NetConnectionStatus)0;
		c->StateShutdown = default;
		c->StateConnecting = default;
		c->StateDisconnected = default;
		c->RemoteId = default;
		c->MapState = NetConnectionMap.EntryState.None;
		c->SendTime = 0.0;
		c->RecvTime = 0.0;
		c->Rtt = 0.0;
		c->NotifySendTime = 0.0;
		c->NotifyRecvTime = 0.0;
		c->NotifyRecvMask = 0uL;
		c->NotifyRecvAckTime = 0.0;
		c->NotifyRecvSequence = 0;
		c->NotifyRecvUnackedCount = 0;
		c->NotifyRecvAckOutdatedCount = 0;
		c->NotifyRecvFragment = 0;
		c->NotifyRecvFragmentBufferLength = 0;
		c->NotifyRecvFragmentSequenceDistance = 0;
	}

	public override string ToString()
	{
		return string.Format("[{0}: {1}={2}, {3}={4}]", "NetConnection", "RemoteAddress", RemoteAddress, "UniqueId", UniqueIdHash);
	}
}

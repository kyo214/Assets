#define DEBUG
using System;

namespace Fusion.Sockets;

public struct NetConnectionMap
{
	public enum EntryState
	{
		None = 0,
		Free = 1,
		Used = 2
	}

	public struct Iterator
	{
		private unsafe NetConnectionMap* _map;

		private int _index;

		private int _count;

		public unsafe NetConnection* Current => IsValid ? (_map->Connections + _index) : null;

		public bool IsValid => _index >= 0 && _index < _count;

		public unsafe Iterator(NetConnectionMap* map)
		{
			_map = map;
			_index = -1;
			_count = (int)_map->UsedCount;
		}

		public unsafe bool Next()
		{
			while (++_index < _count)
			{
				if (_map->Connections[_index].MapState == EntryState.Used)
				{
					return true;
				}
			}
			return false;
		}
	}

	private unsafe NetConnection** Buckets;

	private unsafe NetConnection* FreeHead;

	internal unsafe NetConnection* Connections;

	private short Group;

	private ulong UsedCount;

	private ulong FreeCount;

	private ulong CapacityAllocated;

	internal ulong CapacityUsable;

	public int Count => (int)(UsedCount - FreeCount);

	public int CountUsed => (int)UsedCount;

	public unsafe NetConnection* ConnectionsBuffer => Connections;

	public bool Full => UsedCount == CapacityAllocated;

	public unsafe static void Dispose(NetConnectionMap* map, INetPeerGroupCallbacks callbacks)
	{
		if (map == null)
		{
			return;
		}
		for (int i = 0; i < (int)map->CapacityUsable; i++)
		{
			NetConnection* ptr = map->Connections + i;
			while (ptr->NotifySendWindow.Count > 0)
			{
				NetSendEnvelope envelope = ptr->NotifySendWindow.Peek();
				ptr->NotifySendWindow.Pop();
				callbacks?.OnNotifyDispose(envelope);
			}
			ptr->NotifySendWindow.Dispose();
			if (ptr->NotifyRecvFragmentBuffer != null)
			{
				Native.Free(ptr->NotifyRecvFragmentBuffer);
				ptr->NotifyRecvFragmentBuffer = null;
			}
			if (ptr->ConnectionToken != null)
			{
				Native.Free(ptr->ConnectionToken);
				ptr->ConnectionTokenLength = 0;
			}
			while (ptr->ReliableSendList.Count > 0)
			{
				Native.Free(ptr->ReliableSendList.RemoveHead());
			}
			ptr->ReliableBuffer.Dispose();
			ptr->StatsRoundTripTime.Free();
			ptr->StatsRoundTripTime = default;
			ptr->StatsSentPacketSizes.Free();
			ptr->StatsSentPacketSizes = default;
			ptr->StatsReceivedPacketSizes.Free();
			ptr->StatsReceivedPacketSizes = default;
		}
		Native.Free(map);
	}

	public unsafe static NetConnectionMap* Allocate(int capacity, short groupIndex, in NetConfig* config)
	{
		Assert.Check(capacity >= 0);
		int nextPrime = Primes.GetNextPrime(capacity);
		int num = Native.RoundToMaxAlignment(sizeof(NetConnectionMap));
		int num2 = Native.RoundToMaxAlignment(sizeof(NetConnection*) * nextPrime);
		int num3 = Native.RoundToMaxAlignment(sizeof(NetConnection) * nextPrime);
		byte* ptr = (byte*)Native.MallocAndClear(num + num2 + num3);
		NetConnectionMap* ptr2 = (NetConnectionMap*)ptr;
		ptr2->Buckets = (NetConnection**)(ptr + num);
		ptr2->Connections = (NetConnection*)(ptr + num + num2);
		ptr2->Group = groupIndex;
		ptr2->UsedCount = 0uL;
		ptr2->FreeCount = 0uL;
		ptr2->CapacityAllocated = (ulong)nextPrime;
		ptr2->CapacityUsable = (ulong)capacity;
		for (short num4 = 0; num4 < capacity; num4++)
		{
			NetConnection.Initialize(ptr2->Connections + num4, groupIndex, num4, config);
		}
		return ptr2;
	}

	public unsafe void Remap(NetAddress oldAddress, NetAddress newAddress)
	{
		ulong num = NetAddress.Hash64(oldAddress);
		ulong num2 = NetAddress.Hash64(newAddress);
		ulong num3 = num % CapacityAllocated;
		NetConnection* ptr = Buckets[num3];
		NetConnection* ptr2 = default;
		ulong num4 = num2 % CapacityAllocated;
		while (ptr != null)
		{
			if (ptr->MapHash == num && ptr->Address.Block0 == oldAddress.Block0 && ptr->Address.Block1 == oldAddress.Block1 && ptr->Address.Block2 == oldAddress.Block2)
			{
				Assert.Check(ptr->MapState == EntryState.Used);
				if (ptr2 == null)
				{
					Buckets[num3] = ptr->MapNext;
				}
				else
				{
					ptr2->MapNext = ptr->MapNext;
				}
				ptr->Address = newAddress;
				ptr->MapHash = num2;
				ptr->MapNext = Buckets[num4];
				Buckets[num4] = ptr;
				return;
			}
			ptr2 = ptr;
			ptr = ptr->MapNext;
		}
		Assert.AlwaysFail($"Remap failed from {oldAddress} to {newAddress}");
	}

	public unsafe bool Remove(NetAddress address)
	{
		ulong num = NetAddress.Hash64(address);
		ulong num2 = num % CapacityAllocated;
		NetConnection* ptr = Buckets[num2];
		NetConnection* ptr2 = default;
		while (ptr != null)
		{
			if (ptr->MapHash == num && ptr->Address.Block0 == address.Block0 && ptr->Address.Block1 == address.Block1 && ptr->Address.Block2 == address.Block2)
			{
				if (ptr2 == null)
				{
					Buckets[num2] = ptr->MapNext;
				}
				else
				{
					ptr2->MapNext = ptr->MapNext;
				}
				Assert.Check(ptr->MapState == EntryState.Used);
				NetConnection.Reset(ptr);
				ptr->MapNext = FreeHead;
				ptr->MapState = EntryState.Free;
				FreeHead = ptr;
				FreeCount++;
				return true;
			}
			ptr2 = ptr;
			ptr = ptr->MapNext;
		}
		return false;
	}

	public unsafe NetConnection* Insert(NetAddress address)
	{
		Assert.Check(Find(address) == null);
		Assert.Check(!address.Equals(default));
		ulong num = NetAddress.Hash64(address);
		ulong num2 = num % CapacityAllocated;
		NetConnection* ptr;
		if (FreeHead != null)
		{
			Assert.Check(FreeCount != 0);
			ptr = FreeHead;
			FreeHead = ptr->MapNext;
			FreeCount--;
			Assert.Check(ptr->MapState == EntryState.Free);
		}
		else
		{
			if (UsedCount == CapacityUsable)
			{
				return null;
			}
			ptr = Connections + UsedCount++;
			Assert.Check(ptr->MapState == EntryState.None);
			Assert.Check(ptr->MapNext == null);
		}
		Assert.Check(ptr == Connections + ptr->LocalId.GroupIndex);
		ptr->Address = address;
		ptr->MapHash = num;
		ptr->MapState = EntryState.Used;
		ptr->MapNext = Buckets[num2];
		Buckets[num2] = ptr;
		return ptr;
	}

	public unsafe NetConnection* FindByIndex(int index)
	{
		if (index >= 0 && index < (int)CapacityUsable)
		{
			return Connections + index;
		}
		throw new IndexOutOfRangeException();
	}

	public unsafe bool TryFindByIndex(int index, out NetConnection* connection)
	{
		if (index >= 0 && index < (int)CapacityUsable)
		{
			connection = Connections + index;
			return true;
		}
		connection = null;
		return false;
	}

	public unsafe NetConnection* Find(NetConnectionId id)
	{
		Assert.Check(Group == id.Group);
		NetConnection* ptr = Connections + id.GroupIndex;
		if (ptr->LocalId.Raw == id.Raw)
		{
			return ptr;
		}
		return null;
	}

	public unsafe NetConnection* Find(NetAddress address)
	{
		ulong num = NetAddress.Hash64(address);
		ulong num2 = num % CapacityAllocated;
		for (NetConnection* ptr = Buckets[num2]; ptr != null; ptr = ptr->MapNext)
		{
			if (ptr->MapHash == num && ptr->Address.Block0 == address.Block0 && ptr->Address.Block1 == address.Block1 && ptr->Address.Block2 == address.Block2)
			{
				return ptr;
			}
		}
		return null;
	}
}

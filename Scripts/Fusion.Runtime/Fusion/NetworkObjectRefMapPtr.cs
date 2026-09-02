#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Fusion;

[StructLayout(LayoutKind.Explicit)]
public struct NetworkObjectRefMapPtr
{
	[StructLayout(LayoutKind.Explicit)]
	public struct Entry
	{
		public const int SIZE = 20;

		[FieldOffset(0)]
		public NetworkId Id;

		[FieldOffset(4)]
		public Ptr Ptr;

		[FieldOffset(8)]
		public ushort Next;

		[FieldOffset(12)]
		public int CheckedTick;

		[FieldOffset(16)]
		public int ChangedTick;
	}

	internal struct ObjectEntry
	{
		public NetworkId Id;

		public Ptr Ptr;
	}

	internal unsafe struct Enumerable(NetworkObjectRefMapPtr* map) : IEnumerable<ObjectEntry>, IEnumerable
	{
		private unsafe NetworkObjectRefMapPtr* _map = map;

		public unsafe Enumerator GetEnumerator()
		{
			return new Enumerator(_map);
		}

		IEnumerator<ObjectEntry> IEnumerable<ObjectEntry>.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	internal unsafe struct Enumerator(NetworkObjectRefMapPtr* map) : IEnumerator<ObjectEntry>, IEnumerator, IDisposable
	{
		private unsafe NetworkObjectRefMapPtr* _map = map;

		private int _index = -1;

		public unsafe ObjectEntry Current
		{
			get
			{
				Entry* ptr = (Entry*)((byte*)_map + _map->EntriesOffset);
				return new ObjectEntry
				{
					Id = ptr[_index].Id,
					Ptr = ptr[_index].Ptr
				};
			}
		}

		object IEnumerator.Current => Current;

		public unsafe void Dispose()
		{
			_map = null;
			_index = -1;
		}

		public unsafe bool MoveNext()
		{
			Entry* ptr = (Entry*)((byte*)_map + _map->EntriesOffset);
			while (++_index < _map->Capacity)
			{
				if (ptr[_index].Id != default(NetworkId))
				{
					return true;
				}
			}
			return false;
		}

		public void Reset()
		{
			_index = -1;
		}
	}

	public const int SIZE = 24;

	private const ushort INVALID_ENTRY = 0;

	private const ushort ENTRY_START_INDEX = 1;

	[FieldOffset(0)]
	private int BucketsOffset;

	[FieldOffset(4)]
	private int EntriesOffset;

	[FieldOffset(8)]
	private ushort Free;

	[FieldOffset(12)]
	private int UsedCount;

	[FieldOffset(16)]
	private int FreeCount;

	[FieldOffset(20)]
	private uint Capacity;

	public int Count => UsedCount - FreeCount - 1;

	public unsafe static int ComputeMemoryNeeded(uint capacity)
	{
		Assert.Always(sizeof(NetworkObjectRefMapPtr) == 24, "NetworkObjectRefMapPtr size mismatch");
		Assert.Always(sizeof(Entry) == 20, "Entry size mismatch");
		capacity = Primes.GetNextPrime(capacity);
		int num = Native.RoundToAlignment(sizeof(NetworkObjectRefMapPtr), 4);
		int num2 = CalculateBucketSize(capacity);
		int num3 = CalculateEntrySize(capacity);
		return num + num2 + num3;
	}

	public unsafe static void InitializeMemory(void* memory, uint capacity)
	{
		Assert.Check(memory);
		capacity = Primes.GetNextPrime(capacity);
		((NetworkObjectRefMapPtr*)memory)->Free = 0;
		((NetworkObjectRefMapPtr*)memory)->UsedCount = 1;
		((NetworkObjectRefMapPtr*)memory)->FreeCount = 0;
		((NetworkObjectRefMapPtr*)memory)->Capacity = capacity;
		((NetworkObjectRefMapPtr*)memory)->BucketsOffset = Native.RoundToAlignment(sizeof(NetworkObjectRefMapPtr), 4);
		((NetworkObjectRefMapPtr*)memory)->EntriesOffset = ((NetworkObjectRefMapPtr*)memory)->BucketsOffset + CalculateBucketSize(capacity);
	}

	internal unsafe static void GetIterateBufferStartCount(NetworkObjectRefMapPtr* map, out Entry* entries, out int start, out int count)
	{
		Assert.Check(map);
		Assert.Check(map->Capacity != 0);
		entries = (Entry*)((byte*)map + map->EntriesOffset);
		count = map->UsedCount;
		start = 1;
	}

	public unsafe static bool TryGet(NetworkObjectRefMapPtr* map, NetworkId id, out Ptr ptr)
	{
		Assert.Check(map);
		Assert.Check(map->Capacity != 0);
		Entry* ptr2 = Find(map, id);
		if (ptr2 != null)
		{
			ptr = ptr2->Ptr;
			return true;
		}
		ptr = default;
		return false;
	}

	public unsafe static bool TryGetEntry(NetworkObjectRefMapPtr* map, NetworkId id, out Entry* ptr)
	{
		Assert.Check(map);
		Assert.Check(map->Capacity != 0);
		return (ptr = Find(map, id)) != null;
	}

	public unsafe static bool Contains(NetworkObjectRefMapPtr* map, NetworkId id)
	{
		Assert.Check(map);
		Assert.Check(map->Capacity != 0);
		return Find(map, id) != null;
	}

	public unsafe static bool Remove(NetworkObjectRefMapPtr* map, NetworkId id)
	{
		Assert.Check(map);
		Assert.Check(map->Capacity != 0);
		Entry* ptr = (Entry*)((byte*)map + map->EntriesOffset);
		ushort* ptr2 = (ushort*)((byte*)map + map->BucketsOffset);
		Assert.Check(ptr);
		Assert.Check(ptr2);
		uint num = id.Raw % map->Capacity;
		ushort num2 = ptr2[num];
		ushort num3 = 0;
		while (num2 != 0)
		{
			if (ptr[(int)num2].Id == id)
			{
				if (num3 == 0)
				{
					ptr2[num] = ptr[(int)num2].Next;
				}
				else
				{
					ptr[(int)num3].Next = ptr[(int)num2].Next;
				}
				ptr[(int)num2] = default;
				ptr[(int)num2].Next = map->Free;
				map->Free = num2;
				map->FreeCount++;
				return true;
			}
			num3 = num2;
			num2 = ptr[(int)num2].Next;
		}
		return false;
	}

	public unsafe static bool Add(NetworkObjectRefMapPtr* map, NetworkId id, Ptr ptr)
	{
		Assert.Check(map);
		Assert.Check(map->Capacity != 0);
		if (Find(map, id) != null)
		{
			return false;
		}
		Insert(map, id, ptr);
		return true;
	}

	private unsafe static Entry* Insert(NetworkObjectRefMapPtr* map, NetworkId id, Ptr ptr)
	{
		Assert.Check(map);
		Assert.Check(map->Capacity != 0);
		Entry* ptr2 = (Entry*)((byte*)map + map->EntriesOffset);
		ushort* ptr3 = (ushort*)((byte*)map + map->BucketsOffset);
		Assert.Check(ptr2);
		Assert.Check(ptr3);
		ushort num;
		Entry* ptr4;
		if (map->Free != 0)
		{
			Assert.Check(map->FreeCount > 0);
			num = map->Free;
			ptr4 = ptr2 + (int)num;
			map->Free = ptr4->Next;
			map->FreeCount--;
		}
		else
		{
			if (map->UsedCount == map->Capacity)
			{
				Assert.AlwaysFail("networked object map is full");
			}
			Assert.Check(map->UsedCount < map->Capacity);
			num = (ushort)map->UsedCount++;
			ptr4 = ptr2 + (int)num;
		}
		uint num2 = id.Raw % map->Capacity;
		ptr4->Next = ptr3[num2];
		ptr4->Id = id;
		ptr4->Ptr = ptr;
		ptr3[num2] = num;
		return ptr4;
	}

	private unsafe static Entry* Find(NetworkObjectRefMapPtr* map, NetworkId id)
	{
		Assert.Check(map);
		Assert.Check(map->Capacity != 0);
		Entry* ptr = (Entry*)((byte*)map + map->EntriesOffset);
		ushort* ptr2 = (ushort*)((byte*)map + map->BucketsOffset);
		Assert.Check(ptr);
		Assert.Check(ptr2);
		uint num = id.Raw % map->Capacity;
		for (ushort num2 = ptr2[num]; num2 != 0; num2 = ptr[(int)num2].Next)
		{
			if (ptr[(int)num2].Id == id)
			{
				return ptr + (int)num2;
			}
		}
		return null;
	}

	private static int CalculateBucketSize(uint capacity)
	{
		return Native.RoundToAlignment((int)(2 * capacity), 4);
	}

	private unsafe static int CalculateEntrySize(uint capacity)
	{
		return Native.RoundToAlignment(sizeof(Entry) * (int)capacity, 4);
	}
}

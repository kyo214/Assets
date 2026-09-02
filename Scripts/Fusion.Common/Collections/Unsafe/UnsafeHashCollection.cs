#define DEBUG
using System;
using System.Runtime.CompilerServices;
using Fusion;

namespace Collections.Unsafe;

internal struct UnsafeHashCollection
{
	public enum EntryState
	{
		None = 0,
		Free = 1,
		Used = 2
	}

	public struct Entry
	{
		public const int ALIGNMENT = 8;

		public unsafe Entry* Next;

		public int Hash;

		public EntryState State;
	}

	public unsafe struct Iterator(UnsafeHashCollection* collection)
	{
		private int _index = -1;

		public unsafe Entry* Current = null;

		public unsafe UnsafeHashCollection* Collection = collection;

		public unsafe bool Next()
		{
			while (++_index < Collection->UsedCount)
			{
				Entry* entry = GetEntry(Collection, _index);
				if (entry->State == EntryState.Used)
				{
					Current = entry;
					return true;
				}
			}
			Current = null;
			return false;
		}

		public void Reset()
		{
			_index = -1;
		}
	}

	private static int[] _primeTable = new int[30]
	{
		3, 7, 17, 29, 53, 97, 193, 389, 769, 1543,
		3079, 6151, 12289, 24593, 49157, 98317, 196613, 393241, 786433, 1572869,
		3145739, 6291469, 12582917, 25165843, 50331653, 100663319, 201326611, 402653189, 805306457, 1610612741
	};

	public unsafe Entry** Buckets;

	public unsafe Entry* FreeHead;

	public UnsafeBuffer Entries;

	public int UsedCount;

	public int FreeCount;

	public int KeyOffset;

	public static int GetNextPrime(int value)
	{
		for (int i = 0; i < _primeTable.Length; i++)
		{
			if (_primeTable[i] > value)
			{
				return _primeTable[i];
			}
		}
		throw new InvalidOperationException($"HashCollection can't get larger than {_primeTable[_primeTable.Length - 1]}");
	}

	public unsafe static void Free(UnsafeHashCollection* collection)
	{
		Assert.Check(collection->Entries.Dynamic == 1);
		Native.Free(collection->Buckets);
		Native.Free(collection->Entries.Ptr);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static Entry* GetEntry(UnsafeHashCollection* collection, int index)
	{
		return (Entry*)UnsafeBuffer.Element(collection->Entries.Ptr, index, collection->Entries.Stride);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static T GetKey<T>(UnsafeHashCollection* collection, Entry* entry) where T : unmanaged
	{
		return *(T*)((byte*)entry + collection->KeyOffset);
	}

	public unsafe static Entry* Find<T>(UnsafeHashCollection* collection, T value, int valueHash) where T : unmanaged, IEquatable<T>
	{
		for (Entry* ptr = collection->Buckets[valueHash % collection->Entries.Length]; ptr != null; ptr = ptr->Next)
		{
			if (ptr->Hash == valueHash && value.Equals(*(T*)((byte*)ptr + collection->KeyOffset)))
			{
				return ptr;
			}
		}
		return null;
	}

	public unsafe static bool Remove<T>(UnsafeHashCollection* collection, T value, int valueHash) where T : unmanaged, IEquatable<T>
	{
		int num = valueHash % collection->Entries.Length;
		Entry* ptr = collection->Buckets[valueHash % collection->Entries.Length];
		Entry* ptr2 = default;
		while (ptr != null)
		{
			if (ptr->Hash == valueHash && value.Equals(*(T*)((byte*)ptr + collection->KeyOffset)))
			{
				if (ptr2 == null)
				{
					collection->Buckets[num] = ptr->Next;
				}
				else
				{
					ptr2->Next = ptr->Next;
				}
				Assert.Check(ptr->State == EntryState.Used);
				ptr->Next = collection->FreeHead;
				ptr->State = EntryState.Free;
				collection->FreeHead = ptr;
				collection->FreeCount++;
				return true;
			}
			ptr2 = ptr;
			ptr = ptr->Next;
		}
		return false;
	}

	public unsafe static Entry* Insert<T>(UnsafeHashCollection* collection, T value, int valueHash) where T : unmanaged
	{
		Entry* ptr;
		if (collection->FreeHead != null)
		{
			Assert.Check(collection->FreeCount > 0);
			ptr = collection->FreeHead;
			collection->FreeHead = ptr->Next;
			collection->FreeCount--;
			Assert.Check(ptr->State == EntryState.Free);
		}
		else
		{
			if (collection->UsedCount == collection->Entries.Length)
			{
				Expand(collection);
			}
			ptr = (Entry*)UnsafeBuffer.Element(collection->Entries.Ptr, collection->UsedCount, collection->Entries.Stride);
			collection->UsedCount++;
			Assert.Check(ptr->State == EntryState.None);
		}
		int num = valueHash % collection->Entries.Length;
		ptr->Hash = valueHash;
		ptr->Next = collection->Buckets[num];
		ptr->State = EntryState.Used;
		*(T*)((byte*)ptr + collection->KeyOffset) = value;
		collection->Buckets[num] = ptr;
		return ptr;
	}

	public unsafe static void Clear(UnsafeHashCollection* collection)
	{
		collection->FreeHead = null;
		collection->FreeCount = 0;
		collection->UsedCount = 0;
		int length = collection->Entries.Length;
		Native.MemClear(collection->Buckets, length * sizeof(Entry**));
		UnsafeBuffer.Clear(&collection->Entries);
	}

	private unsafe static void Expand(UnsafeHashCollection* collection)
	{
		Assert.Check(collection->Entries.Dynamic == 1);
		int nextPrime = GetNextPrime(collection->Entries.Length);
		Assert.Check(nextPrime >= collection->Entries.Length);
		Entry** ptr = (Entry**)Native.MallocAndClear(nextPrime * sizeof(Entry**));
		UnsafeBuffer unsafeBuffer = default;
		UnsafeBuffer.InitDynamic(&unsafeBuffer, nextPrime, collection->Entries.Stride);
		UnsafeBuffer.Copy(collection->Entries, 0, unsafeBuffer, 0, collection->Entries.Length);
		collection->FreeHead = null;
		collection->FreeCount = 0;
		for (int num = collection->Entries.Length - 1; num >= 0; num--)
		{
			Entry* ptr2 = (Entry*)((byte*)unsafeBuffer.Ptr + num * unsafeBuffer.Stride);
			if (ptr2->State == EntryState.Used)
			{
				int num2 = ptr2->Hash % nextPrime;
				ptr2->Next = ptr[num2];
				ptr[num2] = ptr2;
			}
			else if (ptr2->State == EntryState.Free)
			{
				ptr2->Next = collection->FreeHead;
				collection->FreeHead = ptr2;
				collection->FreeCount++;
			}
		}
		Native.Free(collection->Buckets);
		UnsafeBuffer.Free(&collection->Entries);
		collection->Buckets = ptr;
		collection->Entries = unsafeBuffer;
	}
}

#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;

namespace Collections.Unsafe;

public struct UnsafeHashSet
{
	public unsafe struct Iterator<T>(UnsafeHashSet* set) : IUnsafeIterator<T>, IEnumerator<T>, IEnumerator, IDisposable, IEnumerable<T>, IEnumerable where T : unmanaged
	{
		private unsafe UnsafeHashCollection.Iterator _iterator = new UnsafeHashCollection.Iterator(&set->_collection);

		private unsafe int _keyOffset = set->_collection.KeyOffset;

		object IEnumerator.Current => Current;

		public unsafe T Current
		{
			get
			{
				if (_iterator.Current == null)
				{
					throw new InvalidOperationException();
				}
				return *(T*)((byte*)_iterator.Current + _keyOffset);
			}
		}

		public bool MoveNext()
		{
			return _iterator.Next();
		}

		public void Reset()
		{
			_iterator.Reset();
		}

		public void Dispose()
		{
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	private UnsafeHashCollection _collection;

	public unsafe static UnsafeHashSet* Allocate<T>(int capacity, bool fixedSize = false) where T : unmanaged, IEquatable<T>
	{
		return Allocate(capacity, sizeof(T), fixedSize);
	}

	public unsafe static UnsafeHashSet* Allocate(int capacity, int valStride, bool fixedSize = false)
	{
		int num = sizeof(UnsafeHashCollection.Entry);
		capacity = UnsafeHashCollection.GetNextPrime(capacity);
		Assert.Check(num == 16);
		int alignment = Native.GetAlignment(valStride);
		int alignment2 = Math.Max(8, alignment);
		valStride = Native.RoundToAlignment(valStride, alignment2);
		num = Native.RoundToAlignment(sizeof(UnsafeHashCollection.Entry), alignment2);
		UnsafeHashSet* ptr2;
		if (fixedSize)
		{
			int num2 = Native.RoundToAlignment(sizeof(UnsafeHashSet), alignment2);
			int num3 = Native.RoundToAlignment(sizeof(UnsafeHashCollection.Entry**) * capacity, alignment2);
			int num4 = (num + valStride) * capacity;
			void* ptr = Native.MallocAndClear(num2 + num3 + num4);
			ptr2 = (UnsafeHashSet*)ptr;
			ptr2->_collection.Buckets = (UnsafeHashCollection.Entry**)((byte*)ptr + num2);
			UnsafeBuffer.InitFixed(&ptr2->_collection.Entries, (byte*)ptr + (num2 + num3), capacity, num + valStride);
		}
		else
		{
			ptr2 = Native.MallocAndClear<UnsafeHashSet>();
			ptr2->_collection.Buckets = (UnsafeHashCollection.Entry**)Native.MallocAndClear(sizeof(UnsafeHashCollection.Entry**) * capacity);
			UnsafeBuffer.InitDynamic(&ptr2->_collection.Entries, capacity, num + valStride);
		}
		ptr2->_collection.FreeCount = 0;
		ptr2->_collection.UsedCount = 0;
		ptr2->_collection.KeyOffset = num;
		return ptr2;
	}

	public unsafe static void Free(UnsafeHashSet* set)
	{
		if (set->_collection.Entries.Dynamic == 1)
		{
			UnsafeHashCollection.Free(&set->_collection);
		}
		Native.Free(set);
	}

	public unsafe static int Capacity(UnsafeHashSet* set)
	{
		return set->_collection.Entries.Length;
	}

	public unsafe static int Count(UnsafeHashSet* set)
	{
		return set->_collection.UsedCount - set->_collection.FreeCount;
	}

	public unsafe static void Clear(UnsafeHashSet* set)
	{
		UnsafeHashCollection.Clear(&set->_collection);
	}

	public unsafe static bool Add<T>(UnsafeHashSet* set, T key) where T : unmanaged, IEquatable<T>
	{
		int hashCode = key.GetHashCode();
		UnsafeHashCollection.Entry* ptr = UnsafeHashCollection.Find(&set->_collection, key, hashCode);
		if (ptr == null)
		{
			UnsafeHashCollection.Insert(&set->_collection, key, hashCode);
			return true;
		}
		return false;
	}

	public unsafe static bool Remove<T>(UnsafeHashSet* set, T key) where T : unmanaged, IEquatable<T>
	{
		return UnsafeHashCollection.Remove(&set->_collection, key, key.GetHashCode());
	}

	public unsafe static bool Contains<T>(UnsafeHashSet* set, T key) where T : unmanaged, IEquatable<T>
	{
		return UnsafeHashCollection.Find(&set->_collection, key, key.GetHashCode()) != null;
	}

	public unsafe static Iterator<T> GetIterator<T>(UnsafeHashSet* set) where T : unmanaged
	{
		return new Iterator<T>(set);
	}

	public unsafe static void And<T>(UnsafeHashSet* set, UnsafeHashSet* other) where T : unmanaged, IEquatable<T>
	{
		for (int num = set->_collection.UsedCount - 1; num >= 0; num--)
		{
			UnsafeHashCollection.Entry* entry = UnsafeHashCollection.GetEntry(&set->_collection, num);
			if (entry->State == UnsafeHashCollection.EntryState.Used)
			{
				T value = *(T*)((byte*)entry + set->_collection.KeyOffset);
				int hashCode = value.GetHashCode();
				if (UnsafeHashCollection.Find(&other->_collection, value, hashCode) == null)
				{
					UnsafeHashCollection.Remove(&set->_collection, value, hashCode);
				}
			}
		}
	}

	public unsafe static void Or<T>(UnsafeHashSet* set, UnsafeHashSet* other) where T : unmanaged, IEquatable<T>
	{
		for (int num = other->_collection.UsedCount - 1; num >= 0; num--)
		{
			UnsafeHashCollection.Entry* entry = UnsafeHashCollection.GetEntry(&other->_collection, num);
			if (entry->State == UnsafeHashCollection.EntryState.Used)
			{
				Add(set, *(T*)((byte*)entry + other->_collection.KeyOffset));
			}
		}
	}

	public unsafe static void Xor<T>(UnsafeHashSet* set, UnsafeHashSet* other) where T : unmanaged, IEquatable<T>
	{
		for (int num = other->_collection.UsedCount - 1; num >= 0; num--)
		{
			UnsafeHashCollection.Entry* entry = UnsafeHashCollection.GetEntry(&other->_collection, num);
			if (entry->State == UnsafeHashCollection.EntryState.Used)
			{
				T value = *(T*)((byte*)entry + other->_collection.KeyOffset);
				int hashCode = value.GetHashCode();
				if (UnsafeHashCollection.Find(&set->_collection, value, hashCode) == null)
				{
					UnsafeHashCollection.Insert(&set->_collection, value, hashCode);
				}
				else
				{
					UnsafeHashCollection.Remove(&set->_collection, value, hashCode);
				}
			}
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;

namespace Collections.Unsafe;

public struct UnsafeOrderedSet
{
	public unsafe struct Iterator<T>(UnsafeOrderedSet* set) : IUnsafeIterator<T>, IEnumerator<T>, IEnumerator, IDisposable, IEnumerable<T>, IEnumerable where T : unmanaged
	{
		private unsafe int _keyOffset = set->_collection.KeyOffset;

		private unsafe UnsafeOrderedCollection.Iterator _iterator = new UnsafeOrderedCollection.Iterator(&set->_collection);

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

		object IEnumerator.Current => Current;

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

	private UnsafeOrderedCollection _collection;

	public unsafe static UnsafeOrderedSet* Allocate<T>(int capacity, bool fixedSize = false) where T : unmanaged, IComparable<T>
	{
		return Allocate(capacity, sizeof(T), fixedSize);
	}

	public unsafe static UnsafeOrderedSet* Allocate(int capacity, int valStride, bool fixedSize = false)
	{
		int stride = sizeof(UnsafeOrderedCollection.Entry);
		int alignment = Native.GetAlignment(valStride);
		int alignment2 = Math.Max(4, alignment);
		valStride = Native.RoundToAlignment(valStride, alignment2);
		stride = Native.RoundToAlignment(stride, alignment2);
		UnsafeOrderedSet* ptr2;
		if (fixedSize)
		{
			int num = Native.RoundToAlignment(sizeof(UnsafeOrderedSet), alignment2);
			int num2 = (stride + valStride) * capacity;
			void* ptr = Native.MallocAndClear(num + num2);
			ptr2 = (UnsafeOrderedSet*)ptr;
			UnsafeBuffer.InitFixed(&ptr2->_collection.Entries, (byte*)ptr + num, capacity, stride + valStride);
		}
		else
		{
			ptr2 = Native.MallocAndClear<UnsafeOrderedSet>();
			UnsafeBuffer.InitDynamic(&ptr2->_collection.Entries, capacity, stride + valStride);
		}
		ptr2->_collection.FreeCount = 0;
		ptr2->_collection.UsedCount = 0;
		ptr2->_collection.KeyOffset = stride;
		return ptr2;
	}

	public unsafe static void Free(UnsafeOrderedSet* set)
	{
		if (set->_collection.Entries.Dynamic == 1)
		{
			UnsafeBuffer.Free(&set->_collection.Entries);
		}
		*set = default;
		Native.Free(set);
	}

	public unsafe static Iterator<T> GetIterator<T>(UnsafeOrderedSet* set) where T : unmanaged
	{
		return new Iterator<T>(set);
	}

	public unsafe static int Count(UnsafeOrderedSet* set)
	{
		return UnsafeOrderedCollection.Count(&set->_collection);
	}

	public unsafe static void Add<T>(UnsafeOrderedSet* set, T item) where T : unmanaged, IComparable<T>
	{
		UnsafeOrderedCollection.Insert(&set->_collection, item);
	}

	public unsafe static void Remove<T>(UnsafeOrderedSet* set, T item) where T : unmanaged, IComparable<T>
	{
		UnsafeOrderedCollection.Remove(&set->_collection, item);
	}

	public unsafe static bool Contains<T>(UnsafeOrderedSet* set, T item) where T : unmanaged, IComparable<T>
	{
		return UnsafeOrderedCollection.Find(&set->_collection, item) != null;
	}
}

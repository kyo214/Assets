#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;

namespace Collections.Unsafe;

public struct UnsafeList
{
	public struct Iterator<T> : IUnsafeIterator<T>, IEnumerator<T>, IEnumerator, IDisposable, IEnumerable<T>, IEnumerable where T : unmanaged
	{
		private unsafe T* _current;

		private int _index;

		private int _count;

		private int _offset;

		private UnsafeBuffer _buffer;

		public unsafe T Current
		{
			get
			{
				if (_current == null)
				{
					throw new InvalidOperationException();
				}
				return *_current;
			}
		}

		object IEnumerator.Current => Current;

		internal unsafe Iterator(UnsafeBuffer buffer, int offset, int count)
		{
			_index = -1;
			_count = count;
			_offset = offset;
			_buffer = buffer;
			_current = null;
		}

		public unsafe bool MoveNext()
		{
			if (++_index < _count)
			{
				_current = (T*)UnsafeBuffer.Element(_buffer.Ptr, (_offset + _index) % _buffer.Length, _buffer.Stride);
				return true;
			}
			_current = null;
			return false;
		}

		public unsafe void Reset()
		{
			_index = -1;
			_current = null;
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

	private const string LIST_FULL = "Fixed size list is full";

	private const string LIST_FIXED_CANT_CHANGE_CAPACITY = "Fixed size list can't change its capacity";

	private const string LIST_INIT_TOO_SMALL = "Pointer length for must be large enough to contain both header and at least 1 item";

	private UnsafeBuffer _items;

	private int _count;

	public unsafe static UnsafeList* Allocate<T>(int capacity, bool fixedSize = false) where T : unmanaged
	{
		return Allocate(capacity, sizeof(T), fixedSize);
	}

	public unsafe static UnsafeList* Allocate(int capacity, int stride, bool fixedSize = false)
	{
		Assert.Check(capacity > 0);
		Assert.Check(stride > 0);
		UnsafeList* ptr2;
		if (fixedSize)
		{
			int alignment = Native.GetAlignment(stride);
			int num = Native.RoundToAlignment(sizeof(UnsafeList), alignment);
			int num2 = stride * capacity;
			void* ptr = Native.MallocAndClear(num + num2);
			ptr2 = (UnsafeList*)ptr;
			UnsafeBuffer.InitFixed(&ptr2->_items, (byte*)ptr + num, capacity, stride);
		}
		else
		{
			ptr2 = Native.MallocAndClear<UnsafeList>();
			UnsafeBuffer.InitDynamic(&ptr2->_items, capacity, stride);
		}
		ptr2->_count = 0;
		return ptr2;
	}

	public unsafe static void Free(UnsafeList* list)
	{
		Native.Free(list);
	}

	public unsafe static int Count(UnsafeList* list)
	{
		Assert.Check(list != null);
		return list->_count;
	}

	public unsafe static void Clear(UnsafeList* list)
	{
		Assert.Check(list != null);
		list->_count = 0;
	}

	public unsafe static int Capacity(UnsafeList* list)
	{
		Assert.Check(list != null);
		return list->_items.Length;
	}

	public unsafe static bool IsFixedSize(UnsafeList* list)
	{
		Assert.Check(list != null);
		return list->_items.Dynamic == 0;
	}

	public unsafe static void SetCapacity(UnsafeList* list, int capacity)
	{
		Assert.Check(list != null);
		if (list->_items.Dynamic == 0)
		{
			throw new InvalidOperationException("Fixed size list can't change its capacity");
		}
		if (capacity == list->_items.Length)
		{
			return;
		}
		if (capacity <= 0)
		{
			list->_count = 0;
			if (list->_items.Ptr != null)
			{
				UnsafeBuffer.Free(&list->_items);
			}
			return;
		}
		UnsafeBuffer unsafeBuffer = default;
		UnsafeBuffer.InitDynamic(&unsafeBuffer, capacity, list->_items.Stride);
		if (list->_count > 0)
		{
			if (list->_count > capacity)
			{
				list->_count = capacity;
			}
			UnsafeBuffer.Copy(list->_items, 0, unsafeBuffer, 0, list->_count);
		}
		if (list->_items.Ptr != null)
		{
			UnsafeBuffer.Free(&list->_items);
		}
		list->_items = unsafeBuffer;
	}

	public unsafe static void Add<T>(UnsafeList* list, T item) where T : unmanaged
	{
		Assert.Check(list != null);
		int count = list->_count;
		UnsafeBuffer items = list->_items;
		if (count < items.Length)
		{
			*(T*)UnsafeBuffer.Element(items.Ptr, count, items.Stride) = item;
			list->_count = count + 1;
			return;
		}
		if (list->_items.Dynamic == 0)
		{
			throw new InvalidOperationException("Fixed size list is full");
		}
		SetCapacity(list, Math.Max(2, items.Length * 2));
		items = list->_items;
		Assert.Check(count < items.Length);
		*(T*)UnsafeBuffer.Element(items.Ptr, count, items.Stride) = item;
		list->_count = count + 1;
	}

	public unsafe static void Set<T>(UnsafeList* list, int index, T item) where T : unmanaged
	{
		Assert.Check(list != null);
		if ((uint)index >= (uint)list->_count)
		{
			throw new IndexOutOfRangeException();
		}
		UnsafeBuffer items = list->_items;
		*(T*)UnsafeBuffer.Element(items.Ptr, index, items.Stride) = item;
	}

	public unsafe static T Get<T>(UnsafeList* list, int index) where T : unmanaged
	{
		Assert.Check(list != null);
		if ((uint)index >= (uint)list->_count)
		{
			throw new IndexOutOfRangeException();
		}
		UnsafeBuffer items = list->_items;
		return *(T*)UnsafeBuffer.Element(items.Ptr, index, items.Stride);
	}

	public unsafe static T* GetPtr<T>(UnsafeList* list, int index) where T : unmanaged
	{
		Assert.Check(list != null);
		if ((uint)index >= (uint)list->_count)
		{
			throw new IndexOutOfRangeException();
		}
		UnsafeBuffer items = list->_items;
		return (T*)UnsafeBuffer.Element(items.Ptr, index, items.Stride);
	}

	public unsafe static void RemoveAt(UnsafeList* list, int index)
	{
		Assert.Check(list != null);
		int count = list->_count;
		if ((uint)index >= (uint)count)
		{
			throw new ArgumentOutOfRangeException();
		}
		count = (list->_count = count - 1);
		if (index < count)
		{
			UnsafeBuffer.Move(list->_items, index + 1, index, count - index);
		}
	}

	public unsafe static void RemoveAtUnordered(UnsafeList* list, int index)
	{
		Assert.Check(list != null);
		int count = list->_count;
		if ((uint)index >= (uint)count)
		{
			throw new ArgumentOutOfRangeException();
		}
		count = (list->_count = count - 1);
		if (index < count)
		{
			UnsafeBuffer.Move(list->_items, count, index, 1);
		}
	}

	public unsafe static int IndexOf<T>(UnsafeList* list, T item) where T : unmanaged, IEquatable<T>
	{
		Assert.Check(list != null);
		int count = list->_count;
		UnsafeBuffer items = list->_items;
		for (int i = 0; i < count; i++)
		{
			T val = *(T*)UnsafeBuffer.Element(items.Ptr, i, items.Stride);
			if (val.Equals(item))
			{
				return i;
			}
		}
		return -1;
	}

	public unsafe static int LastIndexOf<T>(UnsafeList* list, T item) where T : unmanaged, IEquatable<T>
	{
		Assert.Check(list != null);
		int count = list->_count;
		UnsafeBuffer items = list->_items;
		for (int num = count - 1; num >= 0; num--)
		{
			T val = *(T*)UnsafeBuffer.Element(items.Ptr, num, items.Stride);
			if (val.Equals(item))
			{
				return num;
			}
		}
		return -1;
	}

	public unsafe static bool Remove<T>(UnsafeList* list, T item) where T : unmanaged, IEquatable<T>
	{
		Assert.Check(list != null);
		int num = IndexOf(list, item);
		if (num < 0)
		{
			return false;
		}
		RemoveAt(list, num);
		return true;
	}

	public unsafe static bool RemoveUnordered<T>(UnsafeList* list, T item) where T : unmanaged, IEquatable<T>
	{
		Assert.Check(list != null);
		int num = IndexOf(list, item);
		if (num < 0)
		{
			return false;
		}
		RemoveAtUnordered(list, num);
		return true;
	}

	public unsafe static Iterator<T> GetIterator<T>(UnsafeList* list) where T : unmanaged
	{
		return new Iterator<T>(list->_items, 0, list->_count);
	}
}

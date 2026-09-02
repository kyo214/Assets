#define DEBUG
using System;
using Fusion;

namespace Collections.Unsafe;

public struct UnsafeRingBuffer
{
	private UnsafeBuffer _items;

	private int _head;

	private int _tail;

	private int _count;

	private int _overwrite;

	public unsafe static UnsafeRingBuffer* Allocate<T>(int capacity, bool overwrite) where T : unmanaged
	{
		return Allocate(capacity, sizeof(T), overwrite);
	}

	public unsafe static UnsafeRingBuffer* Allocate(int capacity, int stride, bool overwrite)
	{
		Assert.Check(capacity > 0);
		Assert.Check(stride > 0);
		int alignment = Native.GetAlignment(stride);
		int num = Native.RoundToAlignment(sizeof(UnsafeRingBuffer), alignment);
		int num2 = stride * capacity;
		void* ptr = Native.MallocAndClear(num + num2);
		UnsafeRingBuffer* ptr2 = (UnsafeRingBuffer*)ptr;
		UnsafeBuffer.InitFixed(&ptr2->_items, (byte*)ptr + num, capacity, stride);
		ptr2->_count = 0;
		ptr2->_overwrite = (overwrite ? 1 : 0);
		return ptr2;
	}

	public unsafe static void Free(UnsafeRingBuffer* ring)
	{
		Assert.Check(ring != null);
		*ring = default;
		Native.Free(ring);
	}

	public unsafe static int Capacity(UnsafeRingBuffer* ring)
	{
		Assert.Check(ring != null);
		Assert.Check(ring->_items.Ptr != null);
		return ring->_items.Length;
	}

	public unsafe static int Count(UnsafeRingBuffer* ring)
	{
		Assert.Check(ring != null);
		Assert.Check(ring->_items.Ptr != null);
		return ring->_count;
	}

	public unsafe static void Clear(UnsafeRingBuffer* ring)
	{
		Assert.Check(ring != null);
		Assert.Check(ring->_items.Ptr != null);
		ring->_tail = 0;
		ring->_head = 0;
		ring->_count = 0;
	}

	public unsafe static bool IsFull(UnsafeRingBuffer* ring)
	{
		Assert.Check(ring != null);
		Assert.Check(ring->_items.Ptr != null);
		return ring->_count == ring->_items.Length;
	}

	public unsafe static void Set<T>(UnsafeRingBuffer* ring, int index, T value) where T : unmanaged
	{
		if ((uint)index >= (uint)ring->_count)
		{
			throw new IndexOutOfRangeException();
		}
		*(T*)UnsafeBuffer.Element(ring->_items.Ptr, (ring->_tail + index) % ring->_items.Length, ring->_items.Stride) = value;
	}

	public unsafe static T Get<T>(UnsafeRingBuffer* ring, int index) where T : unmanaged
	{
		if ((uint)index >= (uint)ring->_count)
		{
			throw new IndexOutOfRangeException();
		}
		return *(T*)UnsafeBuffer.Element(ring->_items.Ptr, (ring->_tail + index) % ring->_items.Length, ring->_items.Stride);
	}

	public unsafe static T* GetPtr<T>(UnsafeRingBuffer* ring, int index) where T : unmanaged
	{
		if ((uint)index >= (uint)ring->_count)
		{
			throw new IndexOutOfRangeException();
		}
		return (T*)UnsafeBuffer.Element(ring->_items.Ptr, (ring->_tail + index) % ring->_items.Length, ring->_items.Stride);
	}

	public unsafe static bool Push<T>(UnsafeRingBuffer* ring, T item) where T : unmanaged
	{
		if (ring->_count == ring->_items.Length)
		{
			if (ring->_overwrite != 1)
			{
				return false;
			}
			ring->_tail = (ring->_tail + 1) % ring->_items.Length;
			ring->_count--;
		}
		*(T*)UnsafeBuffer.Element(ring->_items.Ptr, ring->_head, ring->_items.Stride) = item;
		ring->_head = (ring->_head + 1) % ring->_items.Length;
		ring->_count++;
		return true;
	}

	public unsafe static bool Pop<T>(UnsafeRingBuffer* ring, out T value) where T : unmanaged
	{
		Assert.Check(ring != null);
		Assert.Check(ring->_items.Ptr != null);
		if (ring->_count == 0)
		{
			value = default;
			return false;
		}
		value = *(T*)UnsafeBuffer.Element(ring->_items.Ptr, ring->_tail, ring->_items.Stride);
		ring->_tail = (ring->_tail + 1) % ring->_items.Length;
		ring->_count--;
		return true;
	}

	public unsafe static UnsafeList.Iterator<T> GetIterator<T>(UnsafeRingBuffer* buffer) where T : unmanaged
	{
		return new UnsafeList.Iterator<T>(buffer->_items, buffer->_tail, buffer->_count);
	}
}

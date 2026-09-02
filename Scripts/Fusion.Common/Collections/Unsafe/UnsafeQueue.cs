#define DEBUG
using System;
using Fusion;

namespace Collections.Unsafe;

public struct UnsafeQueue
{
	private const string QUEUE_EMPTY = "Queue is empty";

	private const string QUEUE_FIXED_SIZE_FULL = "Fixed size queue full";

	private UnsafeBuffer _items;

	private int _count;

	private int _head;

	private int _tail;

	public unsafe static UnsafeQueue* Allocate<T>(int capacity, bool fixedSize = false) where T : unmanaged
	{
		return Allocate(capacity, sizeof(T), fixedSize);
	}

	public unsafe static UnsafeQueue* Allocate(int capacity, int stride, bool fixedSize = false)
	{
		Assert.Check(capacity > 0);
		Assert.Check(stride > 0);
		UnsafeQueue* ptr2;
		if (fixedSize)
		{
			int alignment = Native.GetAlignment(stride);
			int num = Native.RoundToAlignment(sizeof(UnsafeQueue), alignment);
			int num2 = stride * capacity;
			void* ptr = Native.MallocAndClear(num + num2);
			ptr2 = (UnsafeQueue*)ptr;
			UnsafeBuffer.InitFixed(&ptr2->_items, (byte*)ptr + num, capacity, stride);
		}
		else
		{
			ptr2 = Native.MallocAndClear<UnsafeQueue>();
			UnsafeBuffer.InitDynamic(&ptr2->_items, capacity, stride);
		}
		ptr2->_head = 0;
		ptr2->_tail = 0;
		ptr2->_count = 0;
		return ptr2;
	}

	public unsafe static void Free(UnsafeQueue* queue)
	{
		if (queue != null)
		{
			if (queue->_items.Dynamic == 1)
			{
				UnsafeBuffer.Free(&queue->_items);
			}
			*queue = default;
			Native.Free(queue);
		}
	}

	public unsafe static int Capacity(UnsafeQueue* queue)
	{
		Assert.Check(queue != null);
		Assert.Check(queue->_items.Ptr != null);
		return queue->_items.Length;
	}

	public unsafe static int Count(UnsafeQueue* queue)
	{
		Assert.Check(queue != null);
		Assert.Check(queue->_items.Ptr != null);
		return queue->_count;
	}

	public unsafe static void Clear(UnsafeQueue* queue)
	{
		Assert.Check(queue != null);
		Assert.Check(queue->_items.Ptr != null);
		queue->_head = 0;
		queue->_tail = 0;
	}

	public unsafe static bool IsFixedSize(UnsafeQueue* queue)
	{
		Assert.Check(queue != null);
		return queue->_items.Dynamic == 0;
	}

	public unsafe static void Enqueue<T>(UnsafeQueue* queue, T item) where T : unmanaged
	{
		Assert.Check(queue != null);
		Assert.Check(queue->_items.Ptr != null);
		int count = queue->_count;
		UnsafeBuffer items = queue->_items;
		if (count == items.Length)
		{
			if (items.Dynamic != 1)
			{
				throw new InvalidOperationException("Fixed size queue full");
			}
			Expand(queue, items.Length * 2);
			items = queue->_items;
		}
		int tail = queue->_tail;
		*(T*)UnsafeBuffer.Element(items.Ptr, tail, items.Stride) = item;
		queue->_count = count + 1;
		queue->_tail = (tail + 1) % items.Length;
	}

	public unsafe static bool TryEnqueue<T>(UnsafeQueue* queue, T item) where T : unmanaged
	{
		if (queue->_count == queue->_items.Length && queue->_items.Dynamic == 0)
		{
			return false;
		}
		Enqueue(queue, item);
		return true;
	}

	public unsafe static T Dequeue<T>(UnsafeQueue* queue) where T : unmanaged
	{
		Assert.Check(queue != null);
		Assert.Check(queue->_items.Ptr != null);
		int count = queue->_count;
		if (count == 0)
		{
			throw new InvalidOperationException("Queue is empty");
		}
		int head = queue->_head;
		UnsafeBuffer items = queue->_items;
		T result = *(T*)UnsafeBuffer.Element(items.Ptr, head, items.Stride);
		queue->_count = count - 1;
		queue->_head = (head + 1) % items.Length;
		return result;
	}

	public unsafe static bool TryDequeue<T>(UnsafeQueue* queue, out T result) where T : unmanaged
	{
		Assert.Check(queue != null);
		Assert.Check(queue->_items.Ptr != null);
		if (queue->_count == 0)
		{
			result = default;
			return false;
		}
		result = Dequeue<T>(queue);
		return true;
	}

	public unsafe static bool TryPeek<T>(UnsafeQueue* queue, out T result) where T : unmanaged
	{
		Assert.Check(queue != null);
		Assert.Check(queue->_items.Ptr != null);
		if (queue->_count == 0)
		{
			result = default;
			return false;
		}
		result = *PeekPtr<T>(queue);
		return true;
	}

	public unsafe static T Peek<T>(UnsafeQueue* queue) where T : unmanaged
	{
		return *PeekPtr<T>(queue);
	}

	public unsafe static T* PeekPtr<T>(UnsafeQueue* queue) where T : unmanaged
	{
		Assert.Check(queue != null);
		Assert.Check(queue->_items.Ptr != null);
		if (queue->_count == 0)
		{
			throw new InvalidOperationException("Queue is empty");
		}
		UnsafeBuffer items = queue->_items;
		return (T*)UnsafeBuffer.Element(items.Ptr, queue->_head, items.Stride);
	}

	private unsafe static void Expand(UnsafeQueue* queue, int capacity)
	{
		Assert.Check(capacity > 0);
		Assert.Check(queue->_items.Dynamic == 1);
		Assert.Check(queue->_items.Length < capacity);
		UnsafeBuffer unsafeBuffer = default;
		UnsafeBuffer.InitDynamic(&unsafeBuffer, capacity, queue->_items.Stride);
		if (queue->_count > 0)
		{
			if (queue->_head >= queue->_tail)
			{
				UnsafeBuffer.Copy(queue->_items, queue->_head, unsafeBuffer, 0, queue->_items.Length - queue->_head);
				UnsafeBuffer.Copy(queue->_items, 0, unsafeBuffer, queue->_items.Length - queue->_head, queue->_tail);
			}
			else
			{
				UnsafeBuffer.Copy(queue->_items, queue->_head, unsafeBuffer, 0, queue->_count);
			}
		}
		UnsafeBuffer.Free(&queue->_items);
		queue->_items = unsafeBuffer;
		queue->_head = 0;
		queue->_tail = queue->_count % queue->_items.Length;
	}

	public unsafe static UnsafeList.Iterator<T> GetIterator<T>(UnsafeQueue* queue) where T : unmanaged
	{
		return new UnsafeList.Iterator<T>(queue->_items, queue->_head, queue->_count);
	}
}

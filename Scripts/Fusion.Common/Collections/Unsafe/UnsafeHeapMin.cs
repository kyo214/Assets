#define DEBUG
using System;
using System.Runtime.CompilerServices;
using Fusion;

namespace Collections.Unsafe;

public struct UnsafeHeapMin
{
	private const string HEAP_FULL = "Fixed size heap is full";

	private const string HEAP_EMPTY = "Heap is empty";

	private UnsafeBuffer _items;

	private int _count;

	private int _keyStride;

	public unsafe static UnsafeHeapMin* Allocate<K, V>(int capacity, bool fixedSize = false) where K : unmanaged, IComparable<K> where V : unmanaged
	{
		return Allocate(capacity, sizeof(K), sizeof(V), fixedSize);
	}

	public unsafe static UnsafeHeapMin* Allocate(int capacity, int keyStride, int valStride, bool fixedSize = false)
	{
		capacity++;
		int alignment = Native.GetAlignment(keyStride);
		int alignment2 = Native.GetAlignment(valStride);
		int alignment3 = Math.Max(alignment, alignment2);
		keyStride = Native.RoundToAlignment(keyStride, alignment3);
		valStride = Native.RoundToAlignment(valStride, alignment3);
		UnsafeHeapMin* ptr2;
		if (fixedSize)
		{
			int num = Native.RoundToAlignment(sizeof(UnsafeHeapMin), alignment3);
			int num2 = (keyStride + valStride) * capacity;
			void* ptr = Native.MallocAndClear(num + num2);
			ptr2 = (UnsafeHeapMin*)ptr;
			UnsafeBuffer.InitFixed(&ptr2->_items, (byte*)ptr + num, capacity, keyStride + valStride);
		}
		else
		{
			ptr2 = Native.MallocAndClear<UnsafeHeapMin>();
			UnsafeBuffer.InitDynamic(&ptr2->_items, capacity, keyStride + valStride);
		}
		ptr2->_count = 1;
		ptr2->_keyStride = keyStride;
		return ptr2;
	}

	public unsafe static void Free(UnsafeHeapMin* heap)
	{
		if (heap != null)
		{
			if (heap->_items.Dynamic == 1)
			{
				UnsafeBuffer.Free(&heap->_items);
			}
			*heap = default;
			Native.Free(heap);
		}
	}

	public unsafe static int Capacity(UnsafeHeapMin* heap)
	{
		return heap->_items.Length - 1;
	}

	public unsafe static int Count(UnsafeHeapMin* heap)
	{
		return heap->_count - 1;
	}

	public unsafe static void Clear(UnsafeHeapMin* heap)
	{
		heap->_count = 1;
	}

	public unsafe static void Push<K, V>(UnsafeHeapMin* heap, K key, V val) where K : unmanaged, IComparable<K> where V : unmanaged
	{
		if (heap->_count == heap->_items.Length)
		{
			if (heap->_items.Dynamic != 1)
			{
				throw new InvalidOperationException("Fixed size heap is full");
			}
			ExpandHeap(heap);
		}
		int num = heap->_count;
		SetKeyVal(heap, num, key, val);
		while (num != 1)
		{
			int num2 = num / 2;
			K val2 = *(K*)UnsafeBuffer.Element(heap->_items.Ptr, num2, heap->_items.Stride);
			if (val2.CompareTo(key) > 0)
			{
				GetKeyVal<K, V>(heap, num2, out var key2, out var val3);
				SetKeyVal(heap, num, key2, val3);
				SetKeyVal(heap, num2, key, val);
				num = num2;
				continue;
			}
			break;
		}
		heap->_count++;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public unsafe static void Pop<K, V>(UnsafeHeapMin* heap, out K key, out V val) where K : unmanaged, IComparable<K> where V : unmanaged
	{
		if (heap->_count <= 1)
		{
			throw new InvalidOperationException("Heap is empty");
		}
		heap->_count--;
		GetKeyVal<K, V>(heap, 1, out key, out val);
		GetKeyVal<K, V>(heap, heap->_count, out var key2, out var val2);
		SetKeyVal(heap, 1, key2, val2);
		int num = 1;
		int num2 = 1;
		do
		{
			num2 = num;
			if (2 * num2 + 1 <= heap->_count)
			{
				if (Key<K>(heap, num2).CompareTo(Key<K>(heap, 2 * num2)) >= 0)
				{
					num = 2 * num2;
				}
				if (Key<K>(heap, num).CompareTo(Key<K>(heap, 2 * num2 + 1)) >= 0)
				{
					num = 2 * num2 + 1;
				}
			}
			else if (2 * num2 <= heap->_count && Key<K>(heap, num2).CompareTo(Key<K>(heap, 2 * num2)) >= 0)
			{
				num = 2 * num2;
			}
			if (num2 != num)
			{
				GetKeyVal<K, V>(heap, num2, out var key3, out var val3);
				GetKeyVal<K, V>(heap, num, out var key4, out var val4);
				SetKeyVal(heap, num, key3, val3);
				SetKeyVal(heap, num2, key4, val4);
			}
		}
		while (num2 != num);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static K Key<K>(UnsafeHeapMin* heap, int index) where K : unmanaged
	{
		return *(K*)UnsafeBuffer.Element(heap->_items.Ptr, index, heap->_items.Stride);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void GetKeyVal<K, V>(UnsafeHeapMin* heap, int index, out K key, out V val) where K : unmanaged where V : unmanaged
	{
		void* ptr = UnsafeBuffer.Element(heap->_items.Ptr, index, heap->_items.Stride);
		key = *(K*)ptr;
		val = *(V*)((byte*)ptr + heap->_keyStride);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void SetKeyVal<K, V>(UnsafeHeapMin* heap, int index, K key, V val) where K : unmanaged where V : unmanaged
	{
		void* ptr = UnsafeBuffer.Element(heap->_items.Ptr, index, heap->_items.Stride);
		*(K*)ptr = key;
		*(V*)((byte*)ptr + heap->_keyStride) = val;
	}

	private unsafe static void ExpandHeap(UnsafeHeapMin* heap)
	{
		Assert.Check(heap->_items.Dynamic == 1);
		UnsafeBuffer unsafeBuffer = default;
		UnsafeBuffer.InitDynamic(&unsafeBuffer, heap->_items.Length * 2, heap->_items.Stride);
		UnsafeBuffer.Copy(heap->_items, 0, unsafeBuffer, 0, heap->_items.Length);
		UnsafeBuffer.Free(&heap->_items);
		heap->_items = unsafeBuffer;
	}

	public unsafe static UnsafeList.Iterator<T> GetIterator<T>(UnsafeHeapMin* heap) where T : unmanaged
	{
		return new UnsafeList.Iterator<T>(heap->_items, 1, heap->_count - 1);
	}
}

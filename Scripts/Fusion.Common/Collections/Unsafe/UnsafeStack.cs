#define DEBUG
using System;
using Fusion;

namespace Collections.Unsafe;

public struct UnsafeStack
{
	private const string STACK_FULL = "Fixed size stack is full";

	private UnsafeBuffer _items;

	private int _count;

	public unsafe static UnsafeStack* Allocate<T>(int capacity, bool fixedSize = false) where T : unmanaged
	{
		return Allocate(capacity, sizeof(T), fixedSize);
	}

	public unsafe static UnsafeStack* Allocate(int capacity, int stride, bool fixedSize = false)
	{
		Assert.Check(capacity > 0);
		Assert.Check(stride > 0);
		UnsafeStack* ptr2;
		if (fixedSize)
		{
			int alignment = Native.GetAlignment(stride);
			int num = Native.RoundToAlignment(sizeof(UnsafeStack), alignment);
			int num2 = stride * capacity;
			void* ptr = Native.MallocAndClear(num + num2);
			ptr2 = (UnsafeStack*)ptr;
			UnsafeBuffer.InitFixed(&ptr2->_items, (byte*)ptr + num, capacity, stride);
		}
		else
		{
			ptr2 = Native.MallocAndClear<UnsafeStack>();
			UnsafeBuffer.InitDynamic(&ptr2->_items, capacity, stride);
		}
		ptr2->_count = 0;
		return ptr2;
	}

	public unsafe static void Free(UnsafeStack* stack)
	{
		Assert.Check(stack != null);
		if (stack->_items.Dynamic == 1)
		{
			UnsafeBuffer.Free(&stack->_items);
		}
		*stack = default;
		Native.Free(stack);
	}

	public unsafe static int Capacity(UnsafeStack* stack)
	{
		Assert.Check(stack != null);
		Assert.Check(stack->_items.Ptr != null);
		return stack->_items.Length;
	}

	public unsafe static int Count(UnsafeStack* stack)
	{
		Assert.Check(stack != null);
		Assert.Check(stack->_items.Ptr != null);
		return stack->_count;
	}

	public unsafe static void Clear(UnsafeStack* stack)
	{
		Assert.Check(stack != null);
		Assert.Check(stack->_items.Ptr != null);
		stack->_count = 0;
	}

	public unsafe static bool IsFixedSize(UnsafeStack* stack)
	{
		Assert.Check(stack != null);
		return stack->_items.Dynamic == 0;
	}

	public unsafe static void Push<T>(UnsafeStack* stack, T item) where T : unmanaged
	{
		Assert.Check(stack != null);
		UnsafeBuffer items = stack->_items;
		int count = stack->_count;
		if (count >= items.Length)
		{
			if (items.Dynamic != 1)
			{
				throw new InvalidOperationException("Fixed size stack is full");
			}
			Expand(stack);
			items = stack->_items;
			Assert.Check(count < items.Length);
		}
		*(T*)UnsafeBuffer.Element(items.Ptr, count, items.Stride) = item;
		stack->_count = count + 1;
	}

	public unsafe static bool TryPop<T>(UnsafeStack* stack, out T item) where T : unmanaged
	{
		Assert.Check(stack != null);
		void* ptr = Peek(stack);
		if (ptr == null)
		{
			item = default;
			return false;
		}
		stack->_count--;
		item = *(T*)ptr;
		return true;
	}

	public unsafe static bool TryPeek<T>(UnsafeStack* stack, out T item) where T : unmanaged
	{
		void* ptr = Peek(stack);
		if (ptr == null)
		{
			item = default;
			return false;
		}
		item = *(T*)ptr;
		return true;
	}

	private unsafe static void* Peek(UnsafeStack* stack)
	{
		Assert.Check(stack != null);
		int count = stack->_count;
		if (count == 0)
		{
			return null;
		}
		UnsafeBuffer items = stack->_items;
		return UnsafeBuffer.Element(items.Ptr, count - 1, items.Stride);
	}

	private unsafe static void Expand(UnsafeStack* stack)
	{
		UnsafeBuffer unsafeBuffer = default;
		UnsafeBuffer.InitDynamic(&unsafeBuffer, stack->_items.Length * 2, stack->_items.Stride);
		UnsafeBuffer.Copy(stack->_items, 0, unsafeBuffer, 0, stack->_items.Length);
		UnsafeBuffer.Free(&stack->_items);
		stack->_items = unsafeBuffer;
	}

	public unsafe static UnsafeList.Iterator<T> GetIterator<T>(UnsafeStack* stack) where T : unmanaged
	{
		return new UnsafeList.Iterator<T>(stack->_items, 0, stack->_count);
	}
}

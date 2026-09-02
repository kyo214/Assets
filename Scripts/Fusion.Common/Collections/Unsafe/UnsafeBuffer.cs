#define DEBUG
using System;
using System.Runtime.CompilerServices;
using Fusion;

namespace Collections.Unsafe;

internal struct UnsafeBuffer
{
	public unsafe void* Ptr;

	public int Length;

	public int Stride;

	public int Dynamic;

	public unsafe static void Free(UnsafeBuffer* buffer)
	{
		Assert.Check(buffer != null);
		if (buffer->Dynamic == 0)
		{
			throw new InvalidOperationException("Can't free a fixed buffer");
		}
		Assert.Check(buffer->Ptr != null);
		Native.Free(buffer->Ptr);
		*buffer = default;
	}

	public unsafe static void Clear(UnsafeBuffer* buffer)
	{
		Native.MemClear(buffer->Ptr, buffer->Length * buffer->Stride);
	}

	public unsafe static void InitFixed(UnsafeBuffer* buffer, void* ptr, int length, int stride)
	{
		Assert.Check(buffer != null);
		Assert.Check(ptr != null);
		Assert.Check(length > 0);
		Assert.Check(stride > 0);
		Assert.Check(((IntPtr)ptr).ToInt64() % Native.GetAlignment(stride) == 0);
		buffer->Ptr = ptr;
		buffer->Length = length;
		buffer->Stride = stride;
		buffer->Dynamic = 0;
	}

	public unsafe static void InitDynamic<T>(UnsafeBuffer* buffer, int length) where T : unmanaged
	{
		InitDynamic(buffer, length, sizeof(T));
	}

	public unsafe static void InitDynamic(UnsafeBuffer* buffer, int length, int stride)
	{
		Assert.Check(buffer != null);
		Assert.Check(length > 0);
		Assert.Check(stride > 0);
		buffer->Ptr = Native.MallocAndClear(length * stride);
		buffer->Length = length;
		buffer->Stride = stride;
		buffer->Dynamic = 1;
	}

	public unsafe static void Copy(UnsafeBuffer source, int sourceIndex, UnsafeBuffer destination, int destinationIndex, int count)
	{
		Assert.Check(source.Ptr != null);
		Assert.Check(source.Ptr != destination.Ptr);
		Assert.Check(source.Stride == destination.Stride);
		Assert.Check(source.Stride > 0);
		Assert.Check(destination.Ptr != null);
		Native.MemCpy((byte*)destination.Ptr + destinationIndex * source.Stride, (byte*)source.Ptr + sourceIndex * source.Stride, count * source.Stride);
	}

	public unsafe static void Move(UnsafeBuffer source, int fromIndex, int toIndex, int count)
	{
		Assert.Check(source.Ptr != null);
		Native.MemMove((byte*)source.Ptr + toIndex * source.Stride, (byte*)source.Ptr + fromIndex * source.Stride, count * source.Stride);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void* Element(void* bufferPtr, int index, int stride)
	{
		return (byte*)bufferPtr + index * stride;
	}
}

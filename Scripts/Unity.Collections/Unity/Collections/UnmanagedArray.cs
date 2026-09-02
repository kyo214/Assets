using System;

namespace Unity.Collections;

internal unsafe struct UnmanagedArray<T>(int length, AllocatorManager.AllocatorHandle allocator) : IDisposable where T : unmanaged
{
	private unsafe IntPtr m_pointer = (IntPtr)Memory.Unmanaged.Array.Allocate<T>(length, allocator);

	private int m_length = length;

	private AllocatorManager.AllocatorHandle m_allocator = allocator;

	public unsafe ref T this[int index] => ref *(T*)((byte*)(void*)m_pointer + (nint)index * (nint)sizeof(T));

	public unsafe void Dispose()
	{
		Memory.Unmanaged.Free((T*)(void*)m_pointer, Allocator.Persistent);
	}

	public unsafe T* GetUnsafePointer()
	{
		return (T*)(void*)m_pointer;
	}
}

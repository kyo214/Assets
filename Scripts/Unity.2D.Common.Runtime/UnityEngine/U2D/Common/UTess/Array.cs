using System;
using System.Diagnostics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.U2D.Common.UTess;

[DebuggerDisplay("Length = {Length}")]
[DebuggerTypeProxy(typeof(ArrayDebugView<>))]
internal struct Array<T>(int length, int maxSize, Allocator allocMode, NativeArrayOptions options) : IDisposable where T : struct
{
	internal NativeArray<T> m_Array = new NativeArray<T>(length, allocMode, options);

	internal int m_MaxSize = maxSize;

	internal Allocator m_AllocLabel = allocMode;

	internal NativeArrayOptions m_Options = options;

	public T this[int index]
	{
		get
		{
			return m_Array[index];
		}
		set
		{
			ResizeIfRequired(index);
			m_Array[index] = value;
		}
	}

	public bool IsCreated => m_Array.IsCreated;

	public int Length
	{
		get
		{
			if (m_MaxSize == 0)
			{
				return 0;
			}
			return m_Array.Length;
		}
	}

	public int MaxSize => m_MaxSize;

	public unsafe void* UnsafePtr => m_Array.GetUnsafePtr();

	public unsafe void* UnsafeReadOnlyPtr => m_Array.GetUnsafeReadOnlyPtr();

	private void ResizeIfRequired(int index)
	{
		if (index >= m_MaxSize || index < 0)
		{
			throw new IndexOutOfRangeException($"Trying to access beyond allowed size. {index} is out of range of '{m_MaxSize}' MaxSize.");
		}
		if (index >= m_Array.Length)
		{
			int num;
			for (num = Length; num <= index; num *= 2)
			{
			}
			num = ((num > m_MaxSize) ? m_MaxSize : num);
			NativeArray<T> nativeArray = new NativeArray<T>(num, m_AllocLabel, m_Options);
			NativeArray<T>.Copy(m_Array, nativeArray, Length);
			m_Array.Dispose();
			m_Array = nativeArray;
		}
	}

	public void Dispose()
	{
		m_Array.Dispose();
		m_MaxSize = 0;
	}

	public void CopyTo(T[] array)
	{
		m_Array.CopyTo(array);
	}
}

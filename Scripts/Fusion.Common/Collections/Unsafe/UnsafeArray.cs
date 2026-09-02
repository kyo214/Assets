#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Fusion;

namespace Collections.Unsafe;

public struct UnsafeArray
{
	public struct Iterator<T> : IUnsafeIterator<T>, IEnumerator<T>, IEnumerator, IDisposable, IEnumerable<T>, IEnumerable where T : unmanaged
	{
		private unsafe T* _current;

		private int _index;

		private unsafe UnsafeArray* _array;

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

		internal unsafe Iterator(UnsafeArray* array)
		{
			_index = -1;
			_array = array;
			_current = null;
		}

		public unsafe bool MoveNext()
		{
			if (++_index < _array->_length)
			{
				_current = GetPtr<T>(_array, _index);
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

	private const string ARRAY_SIZE_LESS_THAN_ZERO = "Array size can't be less than 0";

	private unsafe void* _buffer;

	private int _length;

	private int _stride;

	private IntPtr _typeHandle;

	public unsafe static UnsafeArray* Allocate<T>(int size) where T : unmanaged
	{
		if (size < 0)
		{
			throw new InvalidOperationException("Array size can't be less than 0");
		}
		int maxAlignment = Native.GetMaxAlignment(sizeof(T), sizeof(UnsafeArray));
		int num = Native.RoundToAlignment(sizeof(UnsafeArray), maxAlignment);
		int num2 = size * sizeof(T);
		void* ptr = Native.MallocAndClear(num + num2);
		UnsafeArray* ptr2 = (UnsafeArray*)ptr;
		ptr2->_buffer = ((size == 0) ? null : ((byte*)ptr + num));
		ptr2->_length = size;
		ptr2->_stride = sizeof(T);
		ptr2->_typeHandle = typeof(T).TypeHandle.Value;
		return ptr2;
	}

	public unsafe static void Free(UnsafeArray* array)
	{
		Assert.Check(array != null);
		Native.Free(array);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void* GetBuffer(UnsafeArray* array)
	{
		Assert.Check(array != null);
		return array->_buffer;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void* GetBuffer<T>(UnsafeArray* array) where T : unmanaged
	{
		Assert.Check(array != null);
		Assert.Check(typeof(T).TypeHandle.Value == array->_typeHandle);
		return array->_buffer;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static int GetStride(UnsafeArray* array)
	{
		Assert.Check(array != null);
		return array->_stride;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static int Length(UnsafeArray* array)
	{
		Assert.Check(array != null);
		return array->_length;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static T* GetPtr<T>(UnsafeArray* array, int index) where T : unmanaged
	{
		Assert.Check(array != null);
		Assert.Check(typeof(T).TypeHandle.Value == array->_typeHandle);
		if ((uint)index >= (uint)array->_length)
		{
			throw new IndexOutOfRangeException(index.ToString());
		}
		return (T*)array->_buffer + index;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static T* GetPtr<T>(UnsafeArray* array, long index) where T : unmanaged
	{
		Assert.Check(array != null);
		Assert.Check(typeof(T).TypeHandle.Value == array->_typeHandle);
		if ((uint)index >= (uint)array->_length)
		{
			throw new IndexOutOfRangeException(index.ToString());
		}
		return (T*)array->_buffer + index;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static T Get<T>(UnsafeArray* array, int index) where T : unmanaged
	{
		Assert.Check(array != null);
		Assert.Check(typeof(T).TypeHandle.Value == array->_typeHandle);
		if ((uint)index >= (uint)array->_length)
		{
			throw new IndexOutOfRangeException(index.ToString());
		}
		return ((T*)array->_buffer)[index];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static T Get<T>(UnsafeArray* array, long index) where T : unmanaged
	{
		Assert.Check(array != null);
		Assert.Check(typeof(T).TypeHandle.Value == array->_typeHandle);
		if ((uint)index >= (uint)array->_length)
		{
			throw new IndexOutOfRangeException(index.ToString());
		}
		return ((T*)array->_buffer)[index];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void Set<T>(UnsafeArray* array, int index, T value) where T : unmanaged
	{
		Assert.Check(array != null);
		Assert.Check(typeof(T).TypeHandle.Value == array->_typeHandle);
		if ((uint)index >= (uint)array->_length)
		{
			throw new IndexOutOfRangeException(index.ToString());
		}
		((T*)array->_buffer)[index] = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void Set<T>(UnsafeArray* array, long index, T value) where T : unmanaged
	{
		Assert.Check(array != null);
		Assert.Check(typeof(T).TypeHandle.Value == array->_typeHandle);
		if ((uint)index >= (uint)array->_length)
		{
			throw new IndexOutOfRangeException(index.ToString());
		}
		((T*)array->_buffer)[index] = value;
	}

	public unsafe static Iterator<T> GetIterator<T>(UnsafeArray* array) where T : unmanaged
	{
		return new Iterator<T>(array);
	}

	public unsafe static void Copy<T>(UnsafeArray* source, int sourceIndex, UnsafeArray* destination, int destinationIndex, int count) where T : unmanaged
	{
		Assert.Check(source != null);
		Assert.Check(destination != null);
		Assert.Check(typeof(T).TypeHandle.Value == source->_typeHandle);
		Assert.Check(typeof(T).TypeHandle.Value == destination->_typeHandle);
		Native.MemCpy((byte*)destination->_buffer + (nint)destinationIndex * (nint)sizeof(T), (byte*)source->_buffer + (nint)sourceIndex * (nint)sizeof(T), count * sizeof(T));
	}

	public unsafe static int IndexOf<T>(UnsafeArray* array, T item) where T : unmanaged, IEquatable<T>
	{
		Assert.Check(array != null);
		Assert.Check(typeof(T).TypeHandle.Value == array->_typeHandle);
		int num = Length(array);
		for (int i = 0; i < num; i++)
		{
			if (Get<T>(array, i).Equals(item))
			{
				return i;
			}
		}
		return -1;
	}

	public unsafe static int LastIndexOf<T>(UnsafeArray* array, T item) where T : unmanaged, IEquatable<T>
	{
		Assert.Check(array != null);
		Assert.Check(typeof(T).TypeHandle.Value == array->_typeHandle);
		for (int num = Length(array) - 1; num >= 0; num--)
		{
			if (Get<T>(array, num).Equals(item))
			{
				return num;
			}
		}
		return -1;
	}

	public unsafe static int FindIndex<T>(UnsafeArray* array, Func<T, bool> predicate) where T : unmanaged
	{
		Assert.Check(array != null);
		Assert.Check(typeof(T).TypeHandle.Value == array->_typeHandle);
		int num = Length(array);
		for (int i = 0; i < num; i++)
		{
			if (predicate(Get<T>(array, i)))
			{
				return i;
			}
		}
		return -1;
	}

	public unsafe static int FindLastIndex<T>(UnsafeArray* array, Func<T, bool> predicate) where T : unmanaged
	{
		Assert.Check(array != null);
		Assert.Check(typeof(T).TypeHandle.Value == array->_typeHandle);
		for (int num = Length(array) - 1; num >= 0; num--)
		{
			if (predicate(Get<T>(array, num)))
			{
				return num;
			}
		}
		return -1;
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe;

[DebuggerTypeProxy(typeof(UnsafeHashSetDebuggerTypeProxy<>))]
[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
public struct UnsafeHashSet<T>(int capacity, AllocatorManager.AllocatorHandle allocator) : INativeDisposable, IDisposable, IEnumerable<T>, IEnumerable where T : unmanaged, IEquatable<T>
{
	[NativeContainerIsAtomicWriteOnly]
	[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct ParallelWriter
	{
		internal UnsafeHashMap<T, bool>.ParallelWriter m_Data;

		public int Capacity => m_Data.Capacity;

		public bool Add(T item)
		{
			return m_Data.TryAdd(item, item: false);
		}
	}

	public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
	{
		internal UnsafeHashMapDataEnumerator m_Enumerator;

		public T Current => m_Enumerator.GetCurrentKey<T>();

		object IEnumerator.Current => Current;

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			return m_Enumerator.MoveNext();
		}

		public void Reset()
		{
			m_Enumerator.Reset();
		}
	}

	internal UnsafeHashMap<T, bool> m_Data = new UnsafeHashMap<T, bool>(capacity, allocator);

	public bool IsEmpty => m_Data.IsEmpty;

	public int Capacity
	{
		get
		{
			return m_Data.Capacity;
		}
		set
		{
			m_Data.Capacity = value;
		}
	}

	public bool IsCreated => m_Data.IsCreated;

	public int Count()
	{
		return m_Data.Count();
	}

	public void Dispose()
	{
		m_Data.Dispose();
	}

	[NotBurstCompatible]
	public JobHandle Dispose(JobHandle inputDeps)
	{
		return m_Data.Dispose(inputDeps);
	}

	public void Clear()
	{
		m_Data.Clear();
	}

	public bool Add(T item)
	{
		return m_Data.TryAdd(item, item: false);
	}

	public bool Remove(T item)
	{
		return m_Data.Remove(item);
	}

	public bool Contains(T item)
	{
		return m_Data.ContainsKey(item);
	}

	public NativeArray<T> ToNativeArray(AllocatorManager.AllocatorHandle allocator)
	{
		return m_Data.GetKeyArray(allocator);
	}

	public ParallelWriter AsParallelWriter()
	{
		return new ParallelWriter
		{
			m_Data = m_Data.AsParallelWriter()
		};
	}

	public unsafe Enumerator GetEnumerator()
	{
		return new Enumerator
		{
			m_Enumerator = new UnsafeHashMapDataEnumerator(m_Data.m_Buffer)
		};
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		throw new NotImplementedException();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		throw new NotImplementedException();
	}
}

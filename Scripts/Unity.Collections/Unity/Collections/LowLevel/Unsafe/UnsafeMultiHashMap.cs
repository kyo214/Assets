using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe;

[DebuggerTypeProxy(typeof(UnsafeMultiHashMapDebuggerTypeProxy<, >))]
[BurstCompatible(GenericTypeArguments = new Type[]
{
	typeof(int),
	typeof(int)
})]
public struct UnsafeMultiHashMap<TKey, TValue> : INativeDisposable, IDisposable, IEnumerable<KeyValue<TKey, TValue>>, IEnumerable where TKey : struct, IEquatable<TKey> where TValue : struct
{
	public struct Enumerator : IEnumerator<TValue>, IEnumerator, IDisposable
	{
		internal UnsafeMultiHashMap<TKey, TValue> hashmap;

		internal TKey key;

		internal bool isFirst;

		private TValue value;

		private NativeMultiHashMapIterator<TKey> iterator;

		public TValue Current => value;

		object IEnumerator.Current => Current;

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			if (isFirst)
			{
				isFirst = false;
				return hashmap.TryGetFirstValue(key, out value, out iterator);
			}
			return hashmap.TryGetNextValue(out value, ref iterator);
		}

		public void Reset()
		{
			isFirst = true;
		}

		public Enumerator GetEnumerator()
		{
			return this;
		}
	}

	[NativeContainerIsAtomicWriteOnly]
	[BurstCompatible(GenericTypeArguments = new Type[]
	{
		typeof(int),
		typeof(int)
	})]
	public struct ParallelWriter
	{
		[NativeDisableUnsafePtrRestriction]
		internal unsafe UnsafeHashMapData* m_Buffer;

		[NativeSetThreadIndex]
		internal int m_ThreadIndex;

		public unsafe int Capacity => m_Buffer->keyCapacity;

		public unsafe void Add(TKey key, TValue item)
		{
			UnsafeHashMapBase<TKey, TValue>.AddAtomicMulti(m_Buffer, key, item, m_ThreadIndex);
		}
	}

	public struct KeyValueEnumerator : IEnumerator<KeyValue<TKey, TValue>>, IEnumerator, IDisposable
	{
		internal UnsafeHashMapDataEnumerator m_Enumerator;

		public KeyValue<TKey, TValue> Current => m_Enumerator.GetCurrent<TKey, TValue>();

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

	[NativeDisableUnsafePtrRestriction]
	internal unsafe UnsafeHashMapData* m_Buffer;

	internal AllocatorManager.AllocatorHandle m_AllocatorLabel;

	public unsafe bool IsEmpty
	{
		get
		{
			if (IsCreated)
			{
				return UnsafeHashMapData.IsEmpty(m_Buffer);
			}
			return true;
		}
	}

	public unsafe int Capacity
	{
		get
		{
			return m_Buffer->keyCapacity;
		}
		set
		{
			UnsafeHashMapData.ReallocateHashMap<TKey, TValue>(m_Buffer, value, UnsafeHashMapData.GetBucketSize(value), m_AllocatorLabel);
		}
	}

	public unsafe bool IsCreated => m_Buffer != null;

	public unsafe UnsafeMultiHashMap(int capacity, AllocatorManager.AllocatorHandle allocator)
	{
		m_AllocatorLabel = allocator;
		UnsafeHashMapData.AllocateHashMap<TKey, TValue>(capacity, capacity * 2, allocator, out m_Buffer);
		Clear();
	}

	public unsafe int Count()
	{
		if (m_Buffer->allocatedIndexLength <= 0)
		{
			return 0;
		}
		return UnsafeHashMapData.GetCount(m_Buffer);
	}

	public unsafe void Clear()
	{
		UnsafeHashMapBase<TKey, TValue>.Clear(m_Buffer);
	}

	public unsafe void Add(TKey key, TValue item)
	{
		UnsafeHashMapBase<TKey, TValue>.TryAdd(m_Buffer, key, item, isMultiHashMap: true, m_AllocatorLabel);
	}

	public unsafe int Remove(TKey key)
	{
		return UnsafeHashMapBase<TKey, TValue>.Remove(m_Buffer, key, isMultiHashMap: true);
	}

	[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
	public unsafe void Remove<TValueEQ>(TKey key, TValueEQ value) where TValueEQ : struct, IEquatable<TValueEQ>
	{
		UnsafeHashMapBase<TKey, TValueEQ>.RemoveKeyValue(m_Buffer, key, value);
	}

	public unsafe void Remove(NativeMultiHashMapIterator<TKey> it)
	{
		UnsafeHashMapBase<TKey, TValue>.Remove(m_Buffer, it);
	}

	public unsafe bool TryGetFirstValue(TKey key, out TValue item, out NativeMultiHashMapIterator<TKey> it)
	{
		return UnsafeHashMapBase<TKey, TValue>.TryGetFirstValueAtomic(m_Buffer, key, out item, out it);
	}

	public unsafe bool TryGetNextValue(out TValue item, ref NativeMultiHashMapIterator<TKey> it)
	{
		return UnsafeHashMapBase<TKey, TValue>.TryGetNextValueAtomic(m_Buffer, out item, ref it);
	}

	public bool ContainsKey(TKey key)
	{
		TValue item;
		NativeMultiHashMapIterator<TKey> it;
		return TryGetFirstValue(key, out item, out it);
	}

	public int CountValuesForKey(TKey key)
	{
		if (!TryGetFirstValue(key, out var item, out var it))
		{
			return 0;
		}
		int num = 1;
		while (TryGetNextValue(out item, ref it))
		{
			num++;
		}
		return num;
	}

	public unsafe bool SetValue(TValue item, NativeMultiHashMapIterator<TKey> it)
	{
		return UnsafeHashMapBase<TKey, TValue>.SetValue(m_Buffer, ref it, ref item);
	}

	public unsafe void Dispose()
	{
		UnsafeHashMapData.DeallocateHashMap(m_Buffer, m_AllocatorLabel);
		m_Buffer = null;
	}

	[NotBurstCompatible]
	public unsafe JobHandle Dispose(JobHandle inputDeps)
	{
		JobHandle result = new UnsafeHashMapDisposeJob
		{
			Data = m_Buffer,
			Allocator = m_AllocatorLabel
		}.Schedule(inputDeps);
		m_Buffer = null;
		return result;
	}

	public unsafe NativeArray<TKey> GetKeyArray(AllocatorManager.AllocatorHandle allocator)
	{
		NativeArray<TKey> result = CollectionHelper.CreateNativeArray<TKey>(Count(), allocator, NativeArrayOptions.UninitializedMemory);
		UnsafeHashMapData.GetKeyArray(m_Buffer, result);
		return result;
	}

	public unsafe NativeArray<TValue> GetValueArray(AllocatorManager.AllocatorHandle allocator)
	{
		NativeArray<TValue> result = CollectionHelper.CreateNativeArray<TValue>(Count(), allocator, NativeArrayOptions.UninitializedMemory);
		UnsafeHashMapData.GetValueArray(m_Buffer, result);
		return result;
	}

	public unsafe NativeKeyValueArrays<TKey, TValue> GetKeyValueArrays(AllocatorManager.AllocatorHandle allocator)
	{
		NativeKeyValueArrays<TKey, TValue> result = new NativeKeyValueArrays<TKey, TValue>(Count(), allocator, NativeArrayOptions.UninitializedMemory);
		UnsafeHashMapData.GetKeyValueArrays(m_Buffer, result);
		return result;
	}

	public Enumerator GetValuesForKey(TKey key)
	{
		return new Enumerator
		{
			hashmap = this,
			key = key,
			isFirst = true
		};
	}

	public unsafe ParallelWriter AsParallelWriter()
	{
		ParallelWriter result = default;
		result.m_ThreadIndex = 0;
		result.m_Buffer = m_Buffer;
		return result;
	}

	public unsafe KeyValueEnumerator GetEnumerator()
	{
		return new KeyValueEnumerator
		{
			m_Enumerator = new UnsafeHashMapDataEnumerator(m_Buffer)
		};
	}

	IEnumerator<KeyValue<TKey, TValue>> IEnumerable<KeyValue<TKey, TValue>>.GetEnumerator()
	{
		throw new NotImplementedException();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		throw new NotImplementedException();
	}
}

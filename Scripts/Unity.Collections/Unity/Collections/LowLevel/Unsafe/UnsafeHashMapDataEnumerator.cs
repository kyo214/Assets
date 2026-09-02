using System;

namespace Unity.Collections.LowLevel.Unsafe;

internal struct UnsafeHashMapDataEnumerator
{
	[NativeDisableUnsafePtrRestriction]
	internal unsafe UnsafeHashMapData* m_Buffer;

	internal int m_Index;

	internal int m_BucketIndex;

	internal int m_NextIndex;

	internal unsafe UnsafeHashMapDataEnumerator(UnsafeHashMapData* data)
	{
		m_Buffer = data;
		m_Index = -1;
		m_BucketIndex = 0;
		m_NextIndex = -1;
	}

	internal unsafe bool MoveNext()
	{
		return UnsafeHashMapData.MoveNext(m_Buffer, ref m_BucketIndex, ref m_NextIndex, out m_Index);
	}

	internal void Reset()
	{
		m_Index = -1;
		m_BucketIndex = 0;
		m_NextIndex = -1;
	}

	internal unsafe KeyValue<TKey, TValue> GetCurrent<TKey, TValue>() where TKey : struct, IEquatable<TKey> where TValue : struct
	{
		return new KeyValue<TKey, TValue>
		{
			m_Buffer = m_Buffer,
			m_Index = m_Index
		};
	}

	internal unsafe TKey GetCurrentKey<TKey>() where TKey : struct, IEquatable<TKey>
	{
		if (m_Index != -1)
		{
			return UnsafeUtility.ReadArrayElement<TKey>(m_Buffer->keys, m_Index);
		}
		return default;
	}
}

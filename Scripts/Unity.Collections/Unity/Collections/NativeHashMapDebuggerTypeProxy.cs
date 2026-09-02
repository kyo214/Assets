using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections;

internal sealed class NativeHashMapDebuggerTypeProxy<TKey, TValue> where TKey : struct, IEquatable<TKey> where TValue : struct
{
	private UnsafeHashMap<TKey, TValue> m_Target;

	public List<Pair<TKey, TValue>> Items
	{
		get
		{
			List<Pair<TKey, TValue>> list = new List<Pair<TKey, TValue>>();
			NativeKeyValueArrays<TKey, TValue> keyValueArrays = m_Target.GetKeyValueArrays(Allocator.Temp);
			try
			{
				for (int i = 0; i < keyValueArrays.Length; i++)
				{
					NativeArray<TKey> keys = keyValueArrays.Keys;
					TKey k = keys[i];
					NativeArray<TValue> values = keyValueArrays.Values;
					list.Add(new Pair<TKey, TValue>(k, values[i]));
				}
				return list;
			}
			finally
			{
				((IDisposable)keyValueArrays/*cast due to constrained. prefix*/).Dispose();
			}
		}
	}

	public NativeHashMapDebuggerTypeProxy(NativeHashMap<TKey, TValue> target)
	{
		m_Target = target.m_HashMapData;
	}
}

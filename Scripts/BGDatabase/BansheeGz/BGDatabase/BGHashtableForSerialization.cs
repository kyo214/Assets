using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGHashtableForSerialization<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
	public List<TKey> keys = new List<TKey>();

	public List<TValue> values = new List<TValue>();

	public void OnBeforeSerialize()
	{
		keys.Clear();
		values.Clear();
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<TKey, TValue> current = enumerator.Current;
				keys.Add(current.Key);
				values.Add(current.Value);
			}
		}
		FromKeys();
	}

	public void OnAfterDeserialize()
	{
		Clear();
		ToKeys();
		if (keys.Count != values.Count)
		{
			throw new BGException($"there are {keys.Count} keys and {values.Count} values after deserialization. Make sure that both key and value types are serializable.");
		}
		for (int i = 0; i < keys.Count; i++)
		{
			Add(keys[i], values[i]);
		}
	}

	protected virtual void FromKeys()
	{
	}

	protected virtual void ToKeys()
	{
	}

	protected virtual void FromValues()
	{
	}

	protected virtual void ToValues()
	{
	}
}

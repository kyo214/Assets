using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGIdList : List<BGId>, ISerializationCallbackReceiver
{
	public List<string> values = new List<string>();

	public BGIdList()
	{
	}

	public BGIdList(IEnumerable<BGId> collection)
		: base(collection)
	{
	}

	public void OnBeforeSerialize()
	{
		values.Clear();
		using Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			BGId current = enumerator.Current;
			values.Add(current.ToString());
		}
	}

	public void OnAfterDeserialize()
	{
		foreach (string value in values)
		{
			Add(new BGId(value));
		}
		values.Clear();
	}
}

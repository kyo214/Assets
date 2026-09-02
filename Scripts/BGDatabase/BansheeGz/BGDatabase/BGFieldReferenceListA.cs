using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public abstract class BGFieldReferenceListA<T> : BGFieldReferenceA<List<T>> where T : MonoBehaviour
{
	public override bool ReadOnly => true;

	public override List<T> this[int entityIndex]
	{
		get
		{
			BGId storedValue = GetStoredValue(entityIndex);
			if (storedValue == BGId.Empty)
			{
				return null;
			}
			List<T> list = null;
			T[] array = Object.FindObjectsOfType<T>();
			foreach (T val in array)
			{
				if (!(IdProvider(val) != storedValue))
				{
					list = list ?? new List<T>();
					list.Add(val);
				}
			}
			return list;
		}
		set
		{
		}
	}

	protected BGFieldReferenceListA(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected BGFieldReferenceListA(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected abstract BGId IdProvider(T component);
}

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.ScriptableObjects;
using UnityEngine;

namespace Doozy.Runtime.UIManager.ScriptableObjects.Internal;

[Serializable]
public abstract class PrefabLinkDatabase<Tdatabase, TprefabLink> : SingletonRuntimeScriptableObject<Tdatabase> where Tdatabase : SingletonRuntimeScriptableObject<Tdatabase> where TprefabLink : PrefabLink
{
	[SerializeField]
	private List<TprefabLink> Database = new List<TprefabLink>();

	public List<TprefabLink> database => Database;

	public abstract string defaultLinkName { get; }

	public abstract string databaseName { get; }

	public List<string> GetAllNames()
	{
		List<string> list = new List<string>();
		list.Add(defaultLinkName);
		list.AddRange(Database.Select((TprefabLink link) => link.prefabName.RemoveWhitespaces().RemoveAllSpecialCharacters()));
		return list;
	}

	public bool Contains(TprefabLink link)
	{
		return Database.RemoveNulls().Contains(link);
	}

	public bool Contains(string prefabName)
	{
		return Database.RemoveNulls().Any((TprefabLink x) => x.prefabName.Equals(prefabName));
	}

	public bool Contains(GameObject prefab)
	{
		return Database.RemoveNulls().Any((TprefabLink x) => x.prefab == prefab);
	}

	public bool Add(TprefabLink link)
	{
		if (link == null)
		{
			return false;
		}
		link.Validate();
		if (Contains(link))
		{
			return false;
		}
		if (!link.hasPrefab)
		{
			Remove(link);
			return false;
		}
		if (!link.hasPrefabName)
		{
			Remove(link);
			return false;
		}
		if (Contains(link.prefab))
		{
			Debug.Log(databaseName + " database already contains a link with the given prefab reference. Link not added.");
			return false;
		}
		if (Contains(link.prefabName))
		{
			Debug.Log(databaseName + " database already contains a link with the given '" + link.prefabName + "' prefabName. Link not added.");
			return false;
		}
		Database.Add(link);
		Save();
		return true;
	}

	public bool Remove(TprefabLink link)
	{
		if (link == null)
		{
			return false;
		}
		if (!Contains(link))
		{
			return false;
		}
		Database.Remove(link);
		return true;
	}

	public bool Remove(string prefabName)
	{
		prefabName = prefabName.RemoveWhitespaces().RemoveAllSpecialCharacters();
		if (string.IsNullOrEmpty(prefabName))
		{
			return false;
		}
		TprefabLink val = null;
		foreach (TprefabLink item in Database)
		{
			if (item.prefabName.Equals(prefabName))
			{
				val = item;
				break;
			}
		}
		if (val == null)
		{
			return false;
		}
		Database.Remove(val);
		return true;
	}

	public bool Remove(GameObject prefab)
	{
		if (prefab == null)
		{
			return false;
		}
		if (!Contains(prefab))
		{
			return false;
		}
		TprefabLink val = null;
		foreach (TprefabLink item in Database)
		{
			if (!(item.prefab != prefab))
			{
				val = item;
				break;
			}
		}
		if (val == null)
		{
			return false;
		}
		Database.Remove(val);
		return true;
	}

	public bool Delete(TprefabLink link)
	{
		if (link == null)
		{
			return false;
		}
		if (!Contains(link))
		{
			return false;
		}
		Database.Remove(link);
		return true;
	}

	public GameObject GetPrefab(string tooltipName)
	{
		if (tooltipName.IsNullOrEmpty())
		{
			return null;
		}
		if (tooltipName.Equals(defaultLinkName))
		{
			return null;
		}
		tooltipName = tooltipName.RemoveWhitespaces().RemoveAllSpecialCharacters();
		foreach (TprefabLink item in Database)
		{
			if (item.prefabName.Equals(tooltipName))
			{
				return item.prefab;
			}
		}
		return null;
	}

	public void RefreshDatabase(bool saveAssets = true, bool refreshAssetDatabase = false)
	{
	}

	public Tdatabase Validate()
	{
		bool flag = false;
		for (int num = Database.Count - 1; num >= 0; num--)
		{
			TprefabLink val = Database[num];
			if (val != null && val.prefab != null && !val.prefabName.IsNullOrEmpty())
			{
				val.Validate();
			}
			else
			{
				Database.RemoveAt(num);
				flag = true;
			}
		}
		Sort();
		if (flag)
		{
			Save();
		}
		return SingletonRuntimeScriptableObject<Tdatabase>.instance;
	}

	public Tdatabase Sort()
	{
		Database.Sort((TprefabLink x, TprefabLink y) => string.Compare(x.prefabName, y.prefabName, StringComparison.Ordinal));
		return SingletonRuntimeScriptableObject<Tdatabase>.instance;
	}

	public Tdatabase Save()
	{
		return SingletonRuntimeScriptableObject<Tdatabase>.instance;
	}

	public Tdatabase SaveAndRefresh()
	{
		return SingletonRuntimeScriptableObject<Tdatabase>.instance;
	}
}

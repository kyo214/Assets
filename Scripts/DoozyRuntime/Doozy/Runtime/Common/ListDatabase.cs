using System;
using System.Collections.Generic;
using System.Linq;

namespace Doozy.Runtime.Common;

[Serializable]
public class ListDatabase<TKey, TValue> : IListDatabase<TKey, TValue>
{
	private readonly Type m_keyType;

	private readonly Type m_valueType;

	public Dictionary<TKey, List<TValue>> Database { get; }

	public Dictionary<TKey, List<TValue>>.KeyCollection Keys => Database.Keys;

	public Dictionary<TKey, List<TValue>>.ValueCollection Values => Database.Values;

	public ListDatabase()
	{
		m_keyType = typeof(TKey);
		m_valueType = typeof(TValue);
		Database = new Dictionary<TKey, List<TValue>>();
	}

	public void Add(TKey key)
	{
		if (!Database.ContainsKey(key))
		{
			Database.Add(key, new List<TValue>());
		}
	}

	public void Add(TKey key, TValue value)
	{
		if (ContainsKey(key))
		{
			if (!ContainsValue(key, value))
			{
				if (Database[key] == null)
				{
					Database[key] = new List<TValue>();
				}
				Database[key].Add(value);
			}
		}
		else
		{
			Database.Add(key, new List<TValue> { value });
		}
	}

	public void Clear()
	{
		Database.Clear();
	}

	public bool ContainsKey(TKey key)
	{
		return Database.ContainsKey(key);
	}

	public bool ContainsValue(TKey key, TValue value)
	{
		if (ContainsKey(key))
		{
			return Database[key].Contains(value);
		}
		return false;
	}

	public bool ContainsValue(TValue value)
	{
		return Database.Keys.Any((TKey key) => Database[key].Contains(value));
	}

	public int CountKeys()
	{
		return Database.Keys.Count;
	}

	public int CountValues(TKey key)
	{
		if (!Database.ContainsKey(key))
		{
			return 0;
		}
		return Database[key].Count;
	}

	public List<TValue> GetValues(TKey key)
	{
		if (key != null && ContainsKey(key) && Database[key] != null)
		{
			return Database[key].ToList();
		}
		return new List<TValue>();
	}

	public List<TKey> GetKeys()
	{
		return Keys.ToList();
	}

	public void Remove(TKey key)
	{
		if (ContainsKey(key))
		{
			Database.Remove(key);
		}
	}

	public void Remove(TKey key, TValue value, bool deleteEmptyKey = true)
	{
		if (ContainsValue(key, value))
		{
			Database[key].Remove(value);
			if (deleteEmptyKey && Database[key].Count == 0)
			{
				Database.Remove(key);
			}
		}
	}

	public void Remove(TValue value, bool deleteEmptyKey = true)
	{
		if (!ContainsValue(value))
		{
			return;
		}
		List<TKey> list = new List<TKey>();
		foreach (TKey item in Database.Keys.Where((TKey key) => Database[key].Contains(value)))
		{
			Database[item].Remove(value);
			if (deleteEmptyKey && Database[item].Count == 0)
			{
				list.Add(item);
			}
		}
		if (!deleteEmptyKey)
		{
			return;
		}
		foreach (TKey item2 in list)
		{
			Database.Remove(item2);
		}
	}

	public void Validate(bool deleteEmptyKeys = true)
	{
		List<TKey> list = new List<TKey>();
		foreach (TKey key in Database.Keys)
		{
			for (int num = Database[key].Count - 1; num >= 0; num--)
			{
				if (Database[key][num] == null)
				{
					Database[key].RemoveAt(num);
				}
			}
			if (deleteEmptyKeys && Database[key].Count == 0)
			{
				list.Add(key);
			}
		}
		if (!deleteEmptyKeys)
		{
			return;
		}
		foreach (TKey item in list)
		{
			Database.Remove(item);
		}
	}
}

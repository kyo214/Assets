using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGStoreMeta : BGArrayStore<BGEntity>
{
	private BGIdDictionary<BGEntity> id2Entity;

	private Dictionary<string, BGEntity> name2Entity;

	public BGEntity this[BGId id]
	{
		get
		{
			if (id2Entity == null)
			{
				InitDictionaryById();
			}
			if (!id2Entity.TryGetValue(id, out var value))
			{
				return null;
			}
			return value;
		}
	}

	public BGEntity this[int index]
	{
		get
		{
			if (index >= base.Count)
			{
				throw new Exception("Index is out of bounds, greater or equal to maxIndex, " + index + ">=" + base.Count);
			}
			return Items[index];
		}
	}

	public BGEntity this[string name]
	{
		get
		{
			if (name2Entity == null)
			{
				InitDictionaryByName();
			}
			if (!name2Entity.TryGetValue(name, out var value))
			{
				return null;
			}
			return value;
		}
	}

	private void InitDictionaryById()
	{
		int count = base.Count;
		id2Entity = new BGIdDictionary<BGEntity>(count);
		for (int i = 0; i < count; i++)
		{
			BGEntity bGEntity = Items[i];
			id2Entity[bGEntity.Id] = bGEntity;
		}
	}

	private void InitDictionaryByName()
	{
		int count = base.Count;
		name2Entity = new Dictionary<string, BGEntity>(count);
		for (int num = count - 1; num >= 0; num--)
		{
			BGEntity bGEntity = Items[num];
			if (!string.IsNullOrEmpty(bGEntity.Name))
			{
				name2Entity[bGEntity.Name] = bGEntity;
			}
		}
	}

	public new void Add(BGEntity entity)
	{
		entity.Index = base.Count;
		base.Add(entity);
		id2Entity?.Add(entity.Id, entity);
	}

	public void Remove(BGEntity entity)
	{
		int index = entity.Index;
		if (index == -1)
		{
			return;
		}
		id2Entity?.Remove(entity.Id);
		if (name2Entity != null)
		{
			string name = entity.Name;
			if (!string.IsNullOrEmpty(name))
			{
				BGEntity bGEntity = BGUtil.Get(name2Entity, name);
				if (bGEntity != null && bGEntity.Id == entity.Id)
				{
					BGEntity bGEntity2 = FindNext(index + 1, name);
					if (bGEntity2 != null)
					{
						name2Entity[name] = bGEntity2;
					}
					else
					{
						name2Entity.Remove(name);
					}
				}
			}
		}
		DeleteAt(index);
		int count = base.Count;
		for (int i = index; i < count; i++)
		{
			Items[i].Index = i;
		}
	}

	public new void Clear()
	{
		base.Clear();
		if (id2Entity != null)
		{
			id2Entity.Clear();
			id2Entity = null;
		}
		if (name2Entity != null)
		{
			name2Entity.Clear();
			name2Entity = null;
		}
	}

	public List<BGEntity> ToList(List<BGEntity> result = null)
	{
		int count = base.Count;
		if (result == null)
		{
			result = new List<BGEntity>(count);
		}
		else
		{
			result.Clear();
		}
		for (int i = 0; i < count; i++)
		{
			result.Add(Items[i]);
		}
		return result;
	}

	public bool ContainsKey(BGId entityId)
	{
		if (id2Entity == null)
		{
			InitDictionaryById();
		}
		return id2Entity.ContainsKey(entityId);
	}

	internal void OnEntityNameChange(int entityIndex, string oldName, string newName)
	{
		if (name2Entity == null || (string.IsNullOrEmpty(oldName) && string.IsNullOrEmpty(newName)) || string.Equals(oldName, newName))
		{
			return;
		}
		BGEntity bGEntity = Items[entityIndex];
		BGId id = bGEntity.Id;
		if (!string.IsNullOrEmpty(oldName))
		{
			BGEntity bGEntity2 = BGUtil.Get(name2Entity, oldName);
			if (bGEntity2 != null && bGEntity2.Id == id)
			{
				BGEntity bGEntity3 = FindNext(entityIndex + 1, oldName);
				if (bGEntity3 != null)
				{
					name2Entity[oldName] = bGEntity3;
				}
				else
				{
					name2Entity.Remove(oldName);
				}
			}
		}
		if (!string.IsNullOrEmpty(newName))
		{
			BGEntity bGEntity4 = BGUtil.Get(name2Entity, newName);
			if (bGEntity4 == null || bGEntity4.Id == id)
			{
				name2Entity[newName] = bGEntity;
			}
			else if (entityIndex < bGEntity4.Index)
			{
				name2Entity[newName] = bGEntity;
			}
		}
	}

	private BGEntity FindNext(int startIndex, string name)
	{
		int count = base.Count;
		for (int i = startIndex; i < count; i++)
		{
			BGEntity bGEntity = Items[i];
			if (!string.IsNullOrEmpty(bGEntity.Name) && string.Equals(bGEntity.Name, name))
			{
				return bGEntity;
			}
		}
		return null;
	}

	internal void InvalidateNameCache()
	{
		name2Entity = null;
	}
}

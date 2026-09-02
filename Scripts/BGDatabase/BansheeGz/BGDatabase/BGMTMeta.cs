using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGMTMeta : BGObjectI
{
	private readonly BGId id;

	private readonly string name;

	private readonly int index;

	protected BGIdDictionary<BGMTField> id2Field;

	protected Dictionary<string, BGMTField> name2Field;

	protected BGMTField[] fields;

	protected List<BGId> entityIds;

	protected BGIdDictionary<int> entityId2Index;

	public BGId Id => id;

	public string Name => name;

	public int Index => index;

	public int CountEntities => entityIds.Count;

	public BGMTRepo Repo { get; internal set; }

	public int CountFields => fields.Length;

	public BGMTEntity? this[int entityIndex]
	{
		get
		{
			if (entityIndex < 0 || entityIds.Count <= entityIndex)
			{
				return null;
			}
			return new BGMTEntity(this, entityIndex);
		}
	}

	public BGMTEntity? this[BGId entityId]
	{
		get
		{
			if (!entityId2Index.TryGetValue(entityId, out var value))
			{
				return null;
			}
			return new BGMTEntity(this, value);
		}
	}

	internal BGMTMeta(BGMetaEntity meta, int index)
	{
		id = meta.Id;
		name = meta.Name;
		this.index = index;
		entityIds = new List<BGId>(meta.CountEntities);
		entityId2Index = new BGIdDictionary<int>(meta.CountEntities);
		meta.ForEachEntity((BGEntity entity) =>
		{
			entityId2Index.Add(entity.Id, entityIds.Count);
			entityIds.Add(entity.Id);
		});
		int i = 0;
		id2Field = new BGIdDictionary<BGMTField>();
		name2Field = new Dictionary<string, BGMTField>();
		List<BGMTField> fieldsList = new List<BGMTField>();
		meta.ForEachField((BGField field) =>
		{
			BGMTField bGMTField = BGMTFieldFactory.Create(this, field);
			if (bGMTField != null)
			{
				fieldsList.Add(bGMTField);
				bGMTField.Index = i++;
				id2Field[bGMTField.Id] = bGMTField;
				name2Field[bGMTField.Name] = bGMTField;
			}
		});
		fields = fieldsList.ToArray();
	}

	protected internal BGMTMeta(BGMTMeta meta)
	{
		id = meta.Id;
		name = meta.Name;
		index = meta.Index;
		entityIds = meta.entityIds;
		entityId2Index = meta.entityId2Index;
		fields = meta.fields;
		id2Field = meta.id2Field;
		name2Field = meta.name2Field;
		if (fields != null)
		{
			for (int i = 0; i < fields.Length; i++)
			{
				fields[i].Meta = this;
			}
		}
	}

	public void ForEachField(Action<BGMTField> action)
	{
		for (int i = 0; i < fields.Length; i++)
		{
			action(fields[i]);
		}
	}

	public BGMTField GetField(int fieldIndex, bool errorIfNotFound = true)
	{
		try
		{
			return fields[fieldIndex];
		}
		catch (Exception ex)
		{
			if (errorIfNotFound)
			{
				throw new BGException("Can not find field with index $, error: $", fieldIndex, ex.Message);
			}
			return null;
		}
	}

	public BGMTField GetField(string fieldName, bool errorIfNotFound = true)
	{
		if (name2Field.TryGetValue(fieldName, out var value))
		{
			return value;
		}
		if (errorIfNotFound)
		{
			throw new BGException("Can not find field with name $", fieldName);
		}
		return null;
	}

	public BGMTField GetField(BGId fieldId, bool errorIfNotFound = true)
	{
		if (id2Field.TryGetValue(fieldId, out var value))
		{
			return value;
		}
		if (errorIfNotFound)
		{
			throw new BGException("Can not find field with id $", fieldId);
		}
		return null;
	}

	public BGMTField<T> GetField<T>(int fieldIndex, bool errorIfNotFound = true)
	{
		try
		{
			BGMTField<T> bGMTField = fields[fieldIndex] as BGMTField<T>;
			if ((bGMTField == null) & errorIfNotFound)
			{
				throw new BGException("Field '$' can not be cast to BGMTField<$>", fields[fieldIndex].Name, typeof(T).FullName);
			}
			return bGMTField;
		}
		catch (Exception ex)
		{
			if (errorIfNotFound)
			{
				throw new BGException("Can not find field with index $, error: $", fieldIndex, ex.Message);
			}
			return null;
		}
	}

	public BGMTField<T> GetField<T>(string fieldName, bool errorIfNotFound = true)
	{
		if (name2Field.TryGetValue(fieldName, out var value))
		{
			BGMTField<T> bGMTField = (BGMTField<T>)value;
			if ((bGMTField == null) & errorIfNotFound)
			{
				throw new BGException("Field '$' can not be cast to BGMTField<$>", value.Name, typeof(T).FullName);
			}
			return bGMTField;
		}
		if (errorIfNotFound)
		{
			throw new BGException("Can not find field with name $", fieldName);
		}
		return null;
	}

	public BGMTField<T> GetField<T>(BGId fieldId, bool errorIfNotFound = true)
	{
		if (id2Field.TryGetValue(fieldId, out var value))
		{
			BGMTField<T> bGMTField = (BGMTField<T>)value;
			if ((bGMTField == null) & errorIfNotFound)
			{
				throw new BGException("Field '$' can not be cast to BGMTField<$>", value.Name, typeof(T).FullName);
			}
			return bGMTField;
		}
		if (errorIfNotFound)
		{
			throw new BGException("Can not find field with id $", fieldId);
		}
		return null;
	}

	internal BGId GetEntityId(int entityIndex)
	{
		if (entityIndex < 0 || entityIds.Count <= entityIndex)
		{
			return BGId.Empty;
		}
		return entityIds[entityIndex];
	}

	public void ForEachEntity(Action<BGMTEntity> action, Predicate<BGMTEntity> filter = null, Comparison<BGMTEntity> sort = null)
	{
		int countEntities = CountEntities;
		if (countEntities == 0)
		{
			return;
		}
		if (sort == null)
		{
			for (int i = 0; i < countEntities; i++)
			{
				BGMTEntity obj = new BGMTEntity(this, i);
				if (filter == null || filter(obj))
				{
					action(obj);
				}
			}
			return;
		}
		List<BGMTEntity> list = new List<BGMTEntity>();
		for (int j = 0; j < countEntities; j++)
		{
			BGMTEntity bGMTEntity = new BGMTEntity(this, j);
			if (filter == null || filter(bGMTEntity))
			{
				list.Add(bGMTEntity);
			}
		}
		list.Sort(sort);
		int count = list.Count;
		for (int k = 0; k < count; k++)
		{
			action(list[k]);
		}
	}

	public BGMTEntity? FindEntity(Predicate<BGMTEntity> filter)
	{
		int countEntities = CountEntities;
		if (countEntities == 0)
		{
			return null;
		}
		for (int i = 0; i < countEntities; i++)
		{
			BGMTEntity bGMTEntity = new BGMTEntity(this, i);
			if (filter(bGMTEntity))
			{
				return bGMTEntity;
			}
		}
		return null;
	}

	public List<BGMTEntity> FindEntities(Predicate<BGMTEntity> filter, List<BGMTEntity> result = null, Comparison<BGMTEntity> sort = null)
	{
		if (result == null)
		{
			result = new List<BGMTEntity>();
		}
		else
		{
			result.Clear();
		}
		int countEntities = CountEntities;
		if (countEntities == 0)
		{
			return result;
		}
		for (int i = 0; i < countEntities; i++)
		{
			BGMTEntity bGMTEntity = new BGMTEntity(this, i);
			if (filter == null || filter(bGMTEntity))
			{
				result.Add(bGMTEntity);
			}
		}
		if (sort != null)
		{
			result.Sort(sort);
		}
		return result;
	}

	protected internal virtual void Set<T>(int fieldIndex, int entityIndex, T value)
	{
		ReadOnlyError();
	}

	protected internal virtual void Delete(int entityIndex)
	{
		ReadOnlyError();
	}

	protected internal virtual bool IsDeleted(int entityIndex)
	{
		ReadOnlyError();
		return false;
	}

	protected internal virtual void ApplyDelete()
	{
		ReadOnlyError();
	}

	protected internal virtual void Dispose()
	{
		id2Field = null;
		name2Field = null;
		fields = null;
		entityIds = null;
		entityId2Index = null;
	}

	public virtual int NewEntities(int numberOfEntities = 1)
	{
		ReadOnlyError();
		return -1;
	}

	private static void ReadOnlyError()
	{
		throw new BGException("You can not change data in read-only transaction. To change the data create write transaction.");
	}
}

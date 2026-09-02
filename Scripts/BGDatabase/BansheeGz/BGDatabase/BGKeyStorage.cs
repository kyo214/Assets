using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

internal class BGKeyStorage
{
	private class KeyComparer : IEqualityComparer<BGKeyStorageKeyI>
	{
		public bool Equals(BGKeyStorageKeyI x, BGKeyStorageKeyI y)
		{
			return x.Equals(y);
		}

		public int GetHashCode(BGKeyStorageKeyI obj)
		{
			return obj.GetHashCode();
		}
	}

	private class EntityComparer : IComparer<BGEntity>
	{
		public int Compare(BGEntity x, BGEntity y)
		{
			return x.Index.CompareTo(y.Index);
		}
	}

	private const int MaxListSize = 16;

	private readonly Dictionary<BGKeyStorageKeyI, object> key2Value = new Dictionary<BGKeyStorageKeyI, object>(new KeyComparer());

	private readonly BGKey dbKey;

	private readonly BGField[] fields;

	private bool dirty;

	public int KeysLength => fields.Length;

	public BGKeyStorage(BGKey dbKey, BGField[] fields)
	{
		if (fields == null || fields.Length == 0)
		{
			throw new Exception("Can not create keys storage: fields are null!");
		}
		this.dbKey = dbKey;
		this.fields = fields;
		AttachListeners();
		Build();
	}

	private void AttachListeners()
	{
		dbKey.OnUnload += Dispose;
		dbKey.Meta.Repo.Events.OnBatchUpdate += BatchListener;
		dbKey.Meta.AnyEntityAdded += EntityAddedListener;
		dbKey.Meta.AnyEntityBeforeDeleted += EntityBeforeDeletedListener;
		dbKey.Meta.EntitiesOrderChanged += EntityOrderChangedListener;
		for (int i = 0; i < fields.Length; i++)
		{
			fields[i].ValueChanged += FieldValueListener;
		}
	}

	private void Dispose(BGObject obj)
	{
		dbKey.OnUnload -= Dispose;
		dbKey.Meta.Repo.Events.OnBatchUpdate -= BatchListener;
		dbKey.Meta.AnyEntityAdded -= EntityAddedListener;
		dbKey.Meta.AnyEntityBeforeDeleted -= EntityBeforeDeletedListener;
		dbKey.Meta.EntitiesOrderChanged -= EntityOrderChangedListener;
		for (int i = 0; i < fields.Length; i++)
		{
			fields[i].ValueChanged -= FieldValueListener;
		}
	}

	private void BatchListener(object sender, BGEventArgsBatch e)
	{
		if (!dirty)
		{
			BGId id = dbKey.Meta.Id;
			if (e.WasEntitiesAdded(id) || e.WasEntitiesDeleted(id) || e.WasEntitiesUpdated(id) || e.WasEntitiesOrderChanged(id))
			{
				dirty = true;
			}
		}
	}

	private void EntityOrderChangedListener(object sender, BGEventArgsEntitiesOrder e)
	{
		dirty = true;
	}

	private void EntityBeforeDeletedListener(object sender, BGEventArgsAnyEntity e)
	{
		if (!dirty && !Remove(e.Entity))
		{
			dirty = true;
		}
	}

	private void EntityAddedListener(object sender, BGEventArgsAnyEntity e)
	{
		if (!dirty)
		{
			BGEntity entity = e.Entity;
			BGKeyStorageKeyI key = GetKey(entity.Index);
			Add(key, entity);
		}
	}

	private void FieldValueListener(object sender, BGEventArgsField e)
	{
		dirty = true;
		if (!(e is BGEventArgsFieldWithValue bGEventArgsFieldWithValue))
		{
			return;
		}
		BGField field = bGEventArgsFieldWithValue.GetField();
		int num = -1;
		for (int i = 0; i < fields.Length; i++)
		{
			BGField objA = fields[i];
			if (object.Equals(objA, field))
			{
				num = i;
				break;
			}
		}
		if (num == -1)
		{
			return;
		}
		object oldValue = bGEventArgsFieldWithValue.GetOldValue();
		BGKeyStorageKeyI key = GetKey(e.Entity.Index);
		BGKeyStorageKeyI key2 = key.Clone();
		switch (KeysLength)
		{
		case 1:
			((BGKeyStorageKey1)key).Value0 = oldValue;
			break;
		case 2:
		{
			BGKeyStorageKey2 bGKeyStorageKey3 = (BGKeyStorageKey2)key;
			switch (num)
			{
			case 0:
				bGKeyStorageKey3.Value0 = oldValue;
				break;
			case 1:
				bGKeyStorageKey3.Value1 = oldValue;
				break;
			}
			break;
		}
		case 3:
		{
			BGKeyStorageKey3 bGKeyStorageKey2 = (BGKeyStorageKey3)key;
			switch (num)
			{
			case 0:
				bGKeyStorageKey2.Value0 = oldValue;
				break;
			case 1:
				bGKeyStorageKey2.Value1 = oldValue;
				break;
			case 2:
				bGKeyStorageKey2.Value2 = oldValue;
				break;
			}
			break;
		}
		case 4:
		{
			BGKeyStorageKey4 bGKeyStorageKey = (BGKeyStorageKey4)key;
			switch (num)
			{
			case 0:
				bGKeyStorageKey.Value0 = oldValue;
				break;
			case 1:
				bGKeyStorageKey.Value1 = oldValue;
				break;
			case 2:
				bGKeyStorageKey.Value2 = oldValue;
				break;
			case 3:
				bGKeyStorageKey.Value3 = oldValue;
				break;
			}
			break;
		}
		default:
		{
			BGKeyStorageKeyN bGKeyStorageKeyN = (BGKeyStorageKeyN)key;
			bGKeyStorageKeyN.Values[num] = oldValue;
			break;
		}
		}
		if (Remove(e.Entity, key))
		{
			Add(key2, e.Entity);
			dirty = false;
		}
	}

	private void CheckDirty()
	{
		if (dirty)
		{
			Build();
		}
	}

	public void MarkDirty()
	{
		dirty = true;
	}

	internal void Build()
	{
		key2Value.Clear();
		dirty = false;
		BGMetaEntity meta = fields[0].Meta;
		switch (KeysLength)
		{
		case 1:
		{
			BGField field9 = fields[0];
			meta.ForEachEntity((BGEntity entity) =>
			{
				Add(new BGKeyStorageKey1(field9.GetValue(entity.Index)), entity);
			});
			return;
		}
		case 2:
		{
			BGField field7 = fields[0];
			BGField field8 = fields[1];
			meta.ForEachEntity((BGEntity entity) =>
			{
				Add(new BGKeyStorageKey2(field7.GetValue(entity.Index), field8.GetValue(entity.Index)), entity);
			});
			return;
		}
		case 3:
		{
			BGField field4 = fields[0];
			BGField field5 = fields[1];
			BGField field6 = fields[2];
			meta.ForEachEntity((BGEntity entity) =>
			{
				Add(new BGKeyStorageKey3(field4.GetValue(entity.Index), field5.GetValue(entity.Index), field6.GetValue(entity.Index)), entity);
			});
			return;
		}
		case 4:
		{
			BGField field0 = fields[0];
			BGField field1 = fields[1];
			BGField field2 = fields[2];
			BGField field3 = fields[3];
			meta.ForEachEntity((BGEntity entity) =>
			{
				Add(new BGKeyStorageKey4(field0.GetValue(entity.Index), field1.GetValue(entity.Index), field2.GetValue(entity.Index), field3.GetValue(entity.Index)), entity);
			});
			return;
		}
		}
		meta.ForEachEntity((BGEntity entity) =>
		{
			object[] array = new object[fields.Length];
			for (int i = 0; i < fields.Length; i++)
			{
				array[i] = fields[i].GetValue(entity.Index);
			}
			Add(new BGKeyStorageKeyN(array), entity);
		});
	}

	private void Add(BGKeyStorageKeyI key, BGEntity entity)
	{
		if (key2Value.TryGetValue(key, out var value))
		{
			if (!(value is BGEntity bGEntity))
			{
				if (!(value is List<BGEntity> list))
				{
					if (value is SortedSet<BGEntity> sortedSet)
					{
						sortedSet.Add(entity);
						return;
					}
					throw new ArgumentOutOfRangeException("value", "value is " + ((value == null) ? "null" : value.GetType().FullName));
				}
				if (list.Count >= 16)
				{
					key2Value[key] = new SortedSet<BGEntity>(list, new EntityComparer()) { entity };
					return;
				}
				int num = -1;
				for (int i = 0; i < list.Count; i++)
				{
					if (entity.Index <= list[i].Index)
					{
						num = i;
						break;
					}
				}
				if (num == -1)
				{
					list.Add(entity);
				}
				else
				{
					list.Insert(num, entity);
				}
			}
			else
			{
				List<BGEntity> value2 = ((bGEntity.Index < entity.Index) ? new List<BGEntity> { bGEntity, entity } : new List<BGEntity> { entity, bGEntity });
				key2Value[key] = value2;
			}
		}
		else
		{
			key2Value[key] = entity;
		}
	}

	private bool Remove(BGEntity entity)
	{
		return Remove(entity, GetKey(entity.Index));
	}

	private bool Remove(BGEntity entity, BGKeyStorageKeyI key)
	{
		if (!key2Value.TryGetValue(key, out var value))
		{
			return false;
		}
		if (!(value is BGEntity objA))
		{
			if (!(value is List<BGEntity> list))
			{
				if (value is SortedSet<BGEntity> sortedSet)
				{
					return sortedSet.Remove(entity);
				}
				throw new ArgumentOutOfRangeException("value", "value is " + ((value == null) ? "null" : value.GetType().FullName));
			}
			return list.Remove(entity);
		}
		if (object.Equals(objA, entity))
		{
			key2Value.Remove(key);
			return true;
		}
		return false;
	}

	public BGEntity GetEntity(params object[] keys)
	{
		CheckDirty();
		BGObjectPool<BGKeyStorageKeyN> pool = BGKeyStorageKeyN.Pool;
		BGKeyStorageKeyN bGKeyStorageKeyN = pool.Get();
		try
		{
			bGKeyStorageKeyN.Values = keys;
			object value;
			return key2Value.TryGetValue(bGKeyStorageKeyN, out value) ? GetFirst(value) : null;
		}
		finally
		{
			pool.Return(bGKeyStorageKeyN);
		}
	}

	public BGEntity GetEntity<T0>(T0 key0)
	{
		CheckDirty();
		BGObjectPool<BGKeyStorageKey1<T0>> pool = BGKeyStorageKey1<T0>.Pool;
		BGKeyStorageKey1<T0> bGKeyStorageKey = pool.Get();
		try
		{
			bGKeyStorageKey.Value0 = key0;
			object value;
			return key2Value.TryGetValue(bGKeyStorageKey, out value) ? GetFirst(value) : null;
		}
		finally
		{
			pool.Return(bGKeyStorageKey);
		}
	}

	public BGEntity GetEntity<T0, T1>(T0 key0, T1 key1)
	{
		CheckDirty();
		BGObjectPool<BGKeyStorageKey2<T0, T1>> pool = BGKeyStorageKey2<T0, T1>.Pool;
		BGKeyStorageKey2<T0, T1> bGKeyStorageKey = pool.Get();
		try
		{
			bGKeyStorageKey.Value0 = key0;
			bGKeyStorageKey.Value1 = key1;
			object value;
			return key2Value.TryGetValue(bGKeyStorageKey, out value) ? GetFirst(value) : null;
		}
		finally
		{
			pool.Return(bGKeyStorageKey);
		}
	}

	public BGEntity GetEntity<T0, T1, T2>(T0 key0, T1 key1, T2 key2)
	{
		CheckDirty();
		BGObjectPool<BGKeyStorageKey3<T0, T1, T2>> pool = BGKeyStorageKey3<T0, T1, T2>.Pool;
		BGKeyStorageKey3<T0, T1, T2> bGKeyStorageKey = pool.Get();
		try
		{
			bGKeyStorageKey.Value0 = key0;
			bGKeyStorageKey.Value1 = key1;
			bGKeyStorageKey.Value2 = key2;
			object value;
			return key2Value.TryGetValue(bGKeyStorageKey, out value) ? GetFirst(value) : null;
		}
		finally
		{
			pool.Return(bGKeyStorageKey);
		}
	}

	public BGEntity GetEntity<T0, T1, T2, T3>(T0 key0, T1 key1, T2 key2, T3 key3)
	{
		CheckDirty();
		BGObjectPool<BGKeyStorageKey4<T0, T1, T2, T3>> pool = BGKeyStorageKey4<T0, T1, T2, T3>.Pool;
		BGKeyStorageKey4<T0, T1, T2, T3> bGKeyStorageKey = pool.Get();
		try
		{
			bGKeyStorageKey.Value0 = key0;
			bGKeyStorageKey.Value1 = key1;
			bGKeyStorageKey.Value2 = key2;
			bGKeyStorageKey.Value3 = key3;
			object value;
			return key2Value.TryGetValue(bGKeyStorageKey, out value) ? GetFirst(value) : null;
		}
		finally
		{
			pool.Return(bGKeyStorageKey);
		}
	}

	public List<BGEntity> GetEntities(params object[] keys)
	{
		return GetEntities<BGEntity>(null, keys);
	}

	public List<T> GetEntities<T>(List<T> result, params object[] keys) where T : BGEntity
	{
		CheckDirty();
		BGObjectPool<BGKeyStorageKeyN> pool = BGKeyStorageKeyN.Pool;
		BGKeyStorageKeyN bGKeyStorageKeyN = pool.Get();
		try
		{
			bGKeyStorageKeyN.Values = keys;
			object value;
			return key2Value.TryGetValue(bGKeyStorageKeyN, out value) ? GetList(result, value) : null;
		}
		finally
		{
			pool.Return(bGKeyStorageKeyN);
		}
	}

	public List<T> GetEntities<T, T0>(List<T> result, T0 key0) where T : BGEntity
	{
		CheckDirty();
		BGObjectPool<BGKeyStorageKey1<T0>> pool = BGKeyStorageKey1<T0>.Pool;
		BGKeyStorageKey1<T0> bGKeyStorageKey = pool.Get();
		try
		{
			bGKeyStorageKey.Value0 = key0;
			object value;
			return key2Value.TryGetValue(bGKeyStorageKey, out value) ? GetList(result, value) : null;
		}
		finally
		{
			pool.Return(bGKeyStorageKey);
		}
	}

	public List<T> GetEntities<T, T0, T1>(List<T> result, T0 key0, T1 key1) where T : BGEntity
	{
		CheckDirty();
		BGObjectPool<BGKeyStorageKey2<T0, T1>> pool = BGKeyStorageKey2<T0, T1>.Pool;
		BGKeyStorageKey2<T0, T1> bGKeyStorageKey = pool.Get();
		try
		{
			bGKeyStorageKey.Value0 = key0;
			bGKeyStorageKey.Value1 = key1;
			object value;
			return key2Value.TryGetValue(bGKeyStorageKey, out value) ? GetList(result, value) : null;
		}
		finally
		{
			pool.Return(bGKeyStorageKey);
		}
	}

	public List<T> GetEntities<T, T0, T1, T2>(List<T> result, T0 key0, T1 key1, T2 key2) where T : BGEntity
	{
		CheckDirty();
		BGObjectPool<BGKeyStorageKey3<T0, T1, T2>> pool = BGKeyStorageKey3<T0, T1, T2>.Pool;
		BGKeyStorageKey3<T0, T1, T2> bGKeyStorageKey = pool.Get();
		try
		{
			bGKeyStorageKey.Value0 = key0;
			bGKeyStorageKey.Value1 = key1;
			bGKeyStorageKey.Value2 = key2;
			object value;
			return key2Value.TryGetValue(bGKeyStorageKey, out value) ? GetList(result, value) : null;
		}
		finally
		{
			pool.Return(bGKeyStorageKey);
		}
	}

	public List<T> GetEntities<T, T0, T1, T2, T3>(List<T> result, T0 key0, T1 key1, T2 key2, T3 key3) where T : BGEntity
	{
		CheckDirty();
		BGObjectPool<BGKeyStorageKey4<T0, T1, T2, T3>> pool = BGKeyStorageKey4<T0, T1, T2, T3>.Pool;
		BGKeyStorageKey4<T0, T1, T2, T3> bGKeyStorageKey = pool.Get();
		try
		{
			bGKeyStorageKey.Value0 = key0;
			bGKeyStorageKey.Value1 = key1;
			bGKeyStorageKey.Value2 = key2;
			bGKeyStorageKey.Value3 = key3;
			object value;
			return key2Value.TryGetValue(bGKeyStorageKey, out value) ? GetList(result, value) : null;
		}
		finally
		{
			pool.Return(bGKeyStorageKey);
		}
	}

	private BGKeyStorageKeyI GetKey(int entityIndex)
	{
		switch (KeysLength)
		{
		case 1:
			return new BGKeyStorageKey1(fields[0].GetValue(entityIndex));
		case 2:
			return new BGKeyStorageKey2(fields[0].GetValue(entityIndex), fields[1].GetValue(entityIndex));
		case 3:
			return new BGKeyStorageKey3(fields[0].GetValue(entityIndex), fields[1].GetValue(entityIndex), fields[2].GetValue(entityIndex));
		case 4:
			return new BGKeyStorageKey4(fields[0].GetValue(entityIndex), fields[1].GetValue(entityIndex), fields[2].GetValue(entityIndex), fields[3].GetValue(entityIndex));
		default:
		{
			object[] array = new object[fields.Length];
			for (int i = 0; i < fields.Length; i++)
			{
				array[i] = fields[i].GetValue(entityIndex);
			}
			return new BGKeyStorageKeyN(array);
		}
		}
	}

	private BGEntity GetFirst(object result)
	{
		if (!(result is BGEntity result2))
		{
			if (!(result is List<BGEntity> list))
			{
				if (result is SortedSet<BGEntity> sortedSet)
				{
					return sortedSet.Min;
				}
				throw new ArgumentOutOfRangeException("result", "result is " + ((result == null) ? "null" : result.GetType().FullName));
			}
			if (list.Count != 0)
			{
				return list[0];
			}
			return null;
		}
		return result2;
	}

	private List<T> GetList<T>(List<T> resultList, object result) where T : BGEntity
	{
		resultList?.Clear();
		if (!(result is BGEntity bGEntity))
		{
			if (!(result is List<BGEntity> list))
			{
				if (!(result is SortedSet<BGEntity> sortedSet))
				{
					throw new ArgumentOutOfRangeException("result", "result is " + ((result == null) ? "null" : result.GetType().FullName));
				}
				if (resultList == null)
				{
					resultList = new List<T>();
				}
				if (resultList is List<BGEntity> list2)
				{
					list2.AddRange(sortedSet);
				}
				else
				{
					foreach (BGEntity item in sortedSet)
					{
						resultList.Add((T)item);
					}
				}
			}
			else
			{
				if (resultList == null)
				{
					resultList = new List<T>();
				}
				if (resultList is List<BGEntity> list3)
				{
					list3.AddRange(list);
				}
				else
				{
					for (int i = 0; i < list.Count; i++)
					{
						resultList.Add((T)list[i]);
					}
				}
			}
		}
		else if (resultList != null)
		{
			resultList.Add((T)bGEntity);
		}
		else
		{
			resultList = new List<T> { (T)bGEntity };
		}
		return resultList;
	}
}

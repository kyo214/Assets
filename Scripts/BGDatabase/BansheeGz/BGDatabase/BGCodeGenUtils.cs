using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public static class BGCodeGenUtils
{
	private class UnloadEventHelper
	{
		private Action unloadAction;

		public UnloadEventHelper(BGObject target, Action unloadAction)
		{
			this.unloadAction = unloadAction;
			target.OnUnload += Unload;
		}

		private void Unload(BGObject obj)
		{
			obj.OnUnload -= Unload;
			unloadAction?.Invoke();
			unloadAction = null;
		}
	}

	public static bool MultiThreadedEnvironment;

	private static readonly List<BGEntity> reusableList = new List<BGEntity>();

	public static T GetMeta<T>(BGId metaId, Action onUnload) where T : BGMetaEntity
	{
		T meta = BGRepo.I.GetMeta<T>(metaId);
		if (meta == null)
		{
			Debug.Log($"[BGDatabase CodeGen addon ERROR]: Can not find a meta with ID={metaId}");
			return null;
		}
		new UnloadEventHelper(meta, onUnload);
		return meta;
	}

	public static T GetField<T>(BGMetaEntity meta, BGId fieldId, Action onUnload) where T : BGField
	{
		T val = (T)meta.GetField(fieldId);
		if (val == null)
		{
			Debug.Log($"[BGDatabase CodeGen addon ERROR]: Can not find a field with ID={fieldId}, meta={meta.Name}");
			return null;
		}
		new UnloadEventHelper(val, onUnload);
		return val;
	}

	public static BGKey GetKey(BGMetaEntity meta, BGId keyId, Action onUnload)
	{
		BGKey key = meta.GetKey(keyId);
		if (key == null)
		{
			Debug.Log($"[BGDatabase CodeGen addon ERROR]: Can not find a key with ID={keyId}, meta={meta.Name}");
			return null;
		}
		new UnloadEventHelper(key, onUnload);
		return key;
	}

	public static BGIndex GetIndex(BGMetaEntity meta, BGId indexId, Action onUnload)
	{
		BGIndex index = meta.GetIndex(indexId);
		if (index == null)
		{
			Debug.Log($"[BGDatabase CodeGen addon ERROR]: Can not find an index with ID={indexId}, meta={meta.Name}");
			return null;
		}
		new UnloadEventHelper(index, onUnload);
		return index;
	}

	public static List<T> GetNested<T>(BGFieldNested fieldNested, int entityIndex) where T : BGEntity
	{
		List<BGEntity> list = fieldNested[entityIndex];
		if (list == null || list.Count == 0)
		{
			return null;
		}
		List<T> list2 = new List<T>(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			list2.Add((T)list[i]);
		}
		return list2;
	}

	public static List<T> GetRelatedInbound<T>(BGAbstractRelationI relation, BGId id) where T : BGEntity
	{
		List<BGEntity> relatedIn = relation.GetRelatedIn(id, MultiThreadedEnvironment ? null : reusableList);
		if (relatedIn.Count == 0)
		{
			return null;
		}
		List<T> list = new List<T>(relatedIn.Count);
		for (int i = 0; i < relatedIn.Count; i++)
		{
			list.Add((T)relatedIn[i]);
		}
		ClearReusableEntityList();
		return list;
	}

	public static void ForEachEntity<T>(BGMetaEntity meta, Action<T> action, Predicate<T> filter = null, Comparison<T> sort = null) where T : BGEntity
	{
		meta.ForEachEntity((BGEntity entity) =>
		{
			action((T)entity);
		}, (filter == null) ? null : ((Predicate<BGEntity>)((BGEntity entity) => filter((T)entity))), (sort == null) ? null : ((Comparison<BGEntity>)((BGEntity e1, BGEntity e2) => sort((T)e1, (T)e2))));
	}

	public static T FindEntity<T>(BGMetaEntity meta, Predicate<T> filter = null) where T : BGEntity
	{
		if (filter == null)
		{
			if (meta.CountEntities != 0)
			{
				return (T)meta.GetEntity(0);
			}
			return null;
		}
		return (T)meta.FindEntity((BGEntity entity) => filter((T)entity));
	}

	public static List<T> FindEntities<T>(BGMetaEntity meta, Predicate<T> filter, List<T> result, Comparison<T> sort) where T : BGEntity
	{
		ClearReusableEntityList();
		List<BGEntity> list = meta.FindEntities((filter == null) ? null : ((Predicate<BGEntity>)((BGEntity e) => filter((T)e))), MultiThreadedEnvironment ? null : reusableList, (sort == null) ? null : ((Comparison<BGEntity>)((BGEntity e1, BGEntity e2) => sort((T)e1, (T)e2))));
		InitList(ref result, list.Count);
		if (list.Count == 0)
		{
			return result;
		}
		for (int num = 0; num < list.Count; num++)
		{
			result.Add((T)list[num]);
		}
		ClearReusableEntityList();
		return result;
	}

	public static void MultipleRelationAdd<T>(BGFieldRelationMultiple relation, int entityIndex, T related) where T : BGEntity
	{
		if (related == null)
		{
			throw new Exception("Can not add a related entity, cause value is null");
		}
		List<BGEntity> list = relation[entityIndex];
		if (list == null)
		{
			list = new List<BGEntity> { related };
		}
		else
		{
			list.Add(related);
		}
		relation[entityIndex] = list;
	}

	public static void MultipleRelationRemove<T>(BGFieldRelationMultiple relation, int entityIndex, T related) where T : BGEntity
	{
		if (related == null)
		{
			throw new Exception("Can not remove a related entity, cause value is null");
		}
		List<BGEntity> list = relation[entityIndex];
		if (list != null)
		{
			list.RemoveAll((BGEntity e) => object.Equals(e, related));
			relation[entityIndex] = ((list.Count == 0) ? null : list);
		}
	}

	public static List<T> MultipleRelationGet<T>(BGField<List<BGEntity>> relation, int entityIndex) where T : BGEntity
	{
		List<BGEntity> list = relation[entityIndex];
		if (list == null || list.Count == 0)
		{
			return null;
		}
		List<T> list2 = new List<T>(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			list2.Add((T)list[i]);
		}
		return list2;
	}

	public static void MultipleRelationSet<T>(BGField<List<BGEntity>> relation, int entityIndex, List<T> value) where T : BGEntity
	{
		List<BGEntity> result = relation[entityIndex];
		if (value != null && value.Count > 0)
		{
			InitList(ref result, value.Count);
			for (int i = 0; i < value.Count; i++)
			{
				result.Add(value[i]);
			}
		}
		else
		{
			result = null;
		}
		relation[entityIndex] = result;
	}

	public static List<T> MultipleViewRelationGet<T>(BGFieldViewRelationMultiple relation, int entityIndex) where T : BGAbstractEntityI
	{
		List<BGEntity> list = relation[entityIndex];
		if (list == null || list.Count == 0)
		{
			return null;
		}
		List<T> list2 = new List<T>(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			list2.Add((T)(object)list[i]);
		}
		return list2;
	}

	public static void MultipleViewRelationSet<T>(BGFieldViewRelationMultiple relation, int entityIndex, List<T> value) where T : BGAbstractEntityI
	{
		List<BGEntity> result = relation[entityIndex];
		if (value != null && value.Count > 0)
		{
			InitList(ref result, value.Count);
			for (int i = 0; i < value.Count; i++)
			{
				result.Add((BGEntity)(object)value[i]);
			}
		}
		else
		{
			result = null;
		}
		relation[entityIndex] = result;
	}

	public static List<T> EnumListGet<T>(BGFieldEnumList enumListField, int entityIndex) where T : Enum
	{
		List<Enum> list = enumListField[entityIndex];
		if (list == null || list.Count == 0)
		{
			return null;
		}
		List<T> list2 = new List<T>(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			list2.Add((T)list[i]);
		}
		return list2;
	}

	public static void EnumListSet<T>(BGFieldEnumList fieldEnumList, int entityIndex, List<T> value) where T : Enum
	{
		List<Enum> list = null;
		if (value != null && value.Count > 0)
		{
			list = new List<Enum>(value.Count);
			for (int i = 0; i < value.Count; i++)
			{
				list.Add(value[i]);
			}
		}
		fieldEnumList[entityIndex] = list;
	}

	private static void InitList<T>(ref List<T> result, int capacity = 0) where T : BGEntity
	{
		if (result != null)
		{
			result.Clear();
		}
		else
		{
			result = new List<T>(capacity);
		}
	}

	private static void ClearReusableEntityList()
	{
		if (!MultiThreadedEnvironment)
		{
			reusableList.Clear();
		}
	}
}

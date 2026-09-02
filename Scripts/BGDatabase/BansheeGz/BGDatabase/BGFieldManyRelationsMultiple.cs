using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "manyTablesRelationMultiple", Folder = "Relation", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerManyRelationsMultiple")]
public class BGFieldManyRelationsMultiple : BGFieldRelationMA<List<BGEntity>, List<BGRowRef>>, BGBinaryBulkLoaderClass, BGFieldRelationMultipleI
{
	[Serializable]
	private struct JsonConfig
	{
		public List<string> ToIds;

		public bool AllowDuplicates;
	}

	public const ushort CodeType = 42;

	private static readonly List<BGRowRef> TempList = new List<BGRowRef>();

	private bool allowDuplicates;

	public override ushort TypeCode => 42;

	public bool AllowDuplicates
	{
		get
		{
			return allowDuplicates;
		}
		set
		{
			if (allowDuplicates == value)
			{
				return;
			}
			if (!value)
			{
				base.Meta.Repo.Events.WithEventsDisabled(() =>
				{
					base.Meta.ForEachEntity((BGEntity entity) =>
					{
						List<BGEntity> list = this[entity.Index];
						if (list != null && list.Count >= 2)
						{
							List<BGEntity> list2 = list.Distinct().ToList();
							if (list.Count != list2.Count)
							{
								this[entity.Index] = list2;
							}
						}
					});
				});
			}
			allowDuplicates = value;
			ReverseCache.AllowDuplicates = value;
			base.events.MetaWasChanged(base.Meta);
		}
	}

	public override List<BGEntity> this[int entityIndex]
	{
		get
		{
			if (entityIndex >= StoreCount)
			{
				ThrowIndexOutOfBoundOnRead(entityIndex);
			}
			List<BGRowRef> list = StoreItems[entityIndex];
			if (list == null || list.Count == 0)
			{
				return null;
			}
			List<BGEntity> list2 = new List<BGEntity>();
			for (int i = 0; i < list.Count; i++)
			{
				BGRowRef bGRowRef = list[i];
				BGEntity entity = bGRowRef.GetEntity(base.Repo);
				if (entity != null)
				{
					list2.Add(entity);
				}
			}
			return list2;
		}
		set
		{
			if (base.events.On)
			{
				List<BGEntity> list = this[entityIndex];
				if (!BGUtil.ListsValuesEqual(value, list))
				{
					BGEntity entity = base.Meta[entityIndex];
					FireBeforeValueChanged(entity, list, value);
					SetEntityList(entityIndex, value);
					FireValueChanged(entity, list, value);
				}
			}
			else
			{
				SetEntityList(entityIndex, value);
			}
		}
	}

	public BGFieldManyRelationsMultiple(BGMetaEntity meta, string name, List<BGMetaEntity> to)
		: base(meta, name, to)
	{
	}

	internal BGFieldManyRelationsMultiple(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	private List<BGRowRef> EnsureList(int entityIndex)
	{
		List<BGRowRef> list = StoreGet(entityIndex);
		if (list != null)
		{
			return list;
		}
		list = new List<BGRowRef>();
		StoreSet(entityIndex, list);
		return list;
	}

	public override void OnEntityDelete(BGEntity entity)
	{
		if (ReverseCache.Enabled)
		{
			List<BGRowRef> list = StoreGet(entity.Index);
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					ReverseCache.RemoveRelated(entity, list[i].EntityId);
				}
			}
		}
		base.OnEntityDelete(entity);
	}

	protected override void OnRemoveRelatedMeta(BGMetaEntity metaEntity)
	{
		base.Meta.ForEachEntity((BGEntity entity) =>
		{
			List<BGEntity> list = this[entity.Index];
			if (list != null && list.Count != 0)
			{
				for (int num = list.Count - 1; num >= 0; num--)
				{
					BGEntity bGEntity = list[num];
					if (bGEntity.MetaId == metaEntity.Id)
					{
						list.RemoveAt(num);
					}
				}
				this[entity.Index] = list;
			}
		});
	}

	public override string ConfigToString()
	{
		List<BGMetaEntity> relatedMetas = RelatedMetas;
		JsonConfig jsonConfig = new JsonConfig
		{
			ToIds = new List<string>(relatedMetas.Count),
			AllowDuplicates = allowDuplicates
		};
		foreach (BGMetaEntity item in relatedMetas)
		{
			jsonConfig.ToIds.Add(item.Id.ToString());
		}
		return JsonUtility.ToJson(jsonConfig);
	}

	public override void ConfigFromString(string config)
	{
		ToIds.Clear();
		if (string.IsNullOrEmpty(config))
		{
			return;
		}
		JsonConfig jsonConfig = JsonUtility.FromJson<JsonConfig>(config);
		allowDuplicates = jsonConfig.AllowDuplicates;
		if (jsonConfig.ToIds == null)
		{
			return;
		}
		foreach (string toId in jsonConfig.ToIds)
		{
			if (BGId.TryParse(toId, out var item))
			{
				ToIds.Add(item);
			}
		}
	}

	public override byte[] ConfigToBytes()
	{
		BGBinaryWriter writer = new BGBinaryWriter(32);
		writer.AddInt(1);
		writer.AddBool(allowDuplicates);
		List<BGMetaEntity> relatedMetas = RelatedMetas;
		writer.AddArray(() =>
		{
			foreach (BGMetaEntity item in relatedMetas)
			{
				writer.AddId(item.Id);
			}
		}, relatedMetas.Count);
		return writer.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		ToIds.Clear();
		BGBinaryReader reader = new BGBinaryReader(config);
		int num = reader.ReadInt();
		if (num == 1)
		{
			allowDuplicates = reader.ReadBool();
			reader.ReadArray(() =>
			{
				ToIds.Add(reader.ReadId());
			});
			return;
		}
		throw new BGException("Unknown version: $", num);
	}

	public override void CopyValue(BGField fromField, BGId fromEntityId, int fromEntityIndex, BGId toEntityId)
	{
		if (fromEntityIndex == -1 || fromField.IsDeleted)
		{
			return;
		}
		int num = base.Meta.FindEntityIndex(toEntityId);
		if (num == -1)
		{
			return;
		}
		BGFieldManyRelationsMultiple bGFieldManyRelationsMultiple = (BGFieldManyRelationsMultiple)fromField;
		List<BGRowRef> list = bGFieldManyRelationsMultiple.StoreGet(fromEntityIndex);
		ReverseSetRelated(num, list);
		if (list != null && list.Count > 0)
		{
			List<BGRowRef> list2 = StoreGet(num);
			if (list2 == null)
			{
				StoreSet(num, new List<BGRowRef>(list));
				return;
			}
			list2.Clear();
			list2.AddRange(list);
		}
		else
		{
			ClearValueNoEvent(num);
		}
	}

	protected override bool AreStoredValuesEqual(List<BGRowRef> myValue, List<BGRowRef> otherValue)
	{
		bool flag = myValue == null || myValue.Count == 0;
		bool flag2 = otherValue == null || otherValue.Count == 0;
		if (flag & flag2)
		{
			return true;
		}
		if (flag | flag2)
		{
			return false;
		}
		if (myValue.Count != otherValue.Count)
		{
			return false;
		}
		for (int i = 0; i < myValue.Count; i++)
		{
			BGRowRef objA = myValue[i];
			BGRowRef objB = otherValue[i];
			if (!object.Equals(objA, objB))
			{
				return false;
			}
		}
		return true;
	}

	public List<BGEntity> GetRelatedEntity(int entityIndex)
	{
		return this[entityIndex];
	}

	public void SetRelatedEntity(int entityIndex, List<BGEntity> entityList)
	{
		this[entityIndex] = entityList;
	}

	private void SetEntityList(int entityIndex, List<BGEntity> value)
	{
		if (value == null || value.Count == 0)
		{
			ClearValue(entityIndex);
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			BGEntity bGEntity = value[i];
			if (bGEntity != null)
			{
				CheckMetaId(bGEntity);
			}
		}
		BGEntity entity = base.Meta[entityIndex];
		List<BGRowRef> list = StoreGet(entityIndex);
		if (list == null)
		{
			list = new List<BGRowRef>(value.Count);
			StoreSet(entityIndex, list);
		}
		else
		{
			if (ReverseCache.Enabled)
			{
				foreach (BGRowRef item in list)
				{
					ReverseCache.RemoveRelated(entity, item.EntityId);
				}
			}
			list.Clear();
		}
		object obj;
		if (!allowDuplicates)
		{
			obj = value.Distinct();
		}
		else
		{
			obj = value;
		}
		IEnumerable<BGEntity> enumerable = (IEnumerable<BGEntity>)obj;
		foreach (BGEntity item2 in enumerable)
		{
			if (item2 != null)
			{
				ReverseCache.AddRelated(entity, item2.Id);
				list.Add(new BGRowRef(item2));
			}
		}
	}

	public override void ClearValue(int entityIndex)
	{
		ReverseRemoveRelated(entityIndex);
		base.ClearValue(entityIndex);
	}

	private void ClearValueNoEventForRelation(int index)
	{
		ReverseRemoveRelated(index);
		ClearValueNoEvent(index);
	}

	public override void Swap(int entityIndex1, int entityIndex2)
	{
		if (ReverseCache.Enabled)
		{
			MarkReverseDirty(entityIndex1);
			MarkReverseDirty(entityIndex2);
		}
		base.Swap(entityIndex1, entityIndex2);
	}

	private void MarkReverseDirty(int index)
	{
		List<BGRowRef> list = StoreGet(index);
		if (list == null || list.Count == 0)
		{
			return;
		}
		foreach (BGRowRef item in list)
		{
			if (!(item == null))
			{
				ReverseCache.MarkDirty(item.EntityId);
			}
		}
	}

	public override void SetStoredValue(int entityIndex, List<BGRowRef> value)
	{
		ReverseSetRelated(entityIndex, value);
		base.SetStoredValue(entityIndex, value);
	}

	private void ReverseRemoveRelated(int entityIndex)
	{
		if (!ReverseCache.Enabled)
		{
			return;
		}
		List<BGRowRef> list = StoreGet(entityIndex);
		if (list != null && list.Count != 0)
		{
			BGEntity entity = base.Meta[entityIndex];
			for (int i = 0; i < list.Count; i++)
			{
				ReverseCache.RemoveRelated(entity, list[i].EntityId);
			}
		}
	}

	private void ReverseSetRelated(int entityIndex, List<BGRowRef> value)
	{
		if (!ReverseCache.Enabled)
		{
			return;
		}
		ReverseRemoveRelated(entityIndex);
		if (value == null || value.Count == 0)
		{
			return;
		}
		BGEntity entity = base.Meta.GetEntity(entityIndex);
		foreach (BGRowRef item in value)
		{
			ReverseCache.AddRelated(entity, item.EntityId);
		}
	}

	protected override void BuildReverseCache()
	{
		StoreForEachKeyValue((int index, List<BGRowRef> valList) =>
		{
			if (valList == null || valList.Count == 0)
			{
				return;
			}
			BGEntity entity = base.Meta[index];
			foreach (BGRowRef val in valList)
			{
				if (!(val == null))
				{
					ReverseRelationCacheValueI reverseRelationCacheValueI = ReverseCache.Ensure(val.EntityId);
					reverseRelationCacheValueI.Add(entity);
				}
			}
		});
	}

	public override void ClearToValue(BGId entityId)
	{
		Exception exception = null;
		if (ReverseCache.Enabled)
		{
			ClearToValue(entityId, ref exception);
		}
		else
		{
			StoreForEachKeyValue((int index, List<BGRowRef> value) =>
			{
				if (value == null || value.RemoveAll((BGRowRef rowRef) => rowRef.EntityId == entityId) == 0)
				{
					return;
				}
				try
				{
					FireStoredValueChanged(base.Meta[index], value, value);
				}
				catch (Exception ex)
				{
					if (exception == null)
					{
						exception = ex;
					}
				}
			});
		}
		if (exception != null)
		{
			throw exception;
		}
	}

	public override void ClearToValue(HashSet<BGId> entityIds)
	{
		Exception exception = null;
		if (ReverseCache.Enabled)
		{
			foreach (BGId entityId in entityIds)
			{
				ClearToValue(entityId, ref exception);
			}
		}
		else
		{
			StoreForEachKeyValue((int index, List<BGRowRef> value) =>
			{
				if (value == null || value.RemoveAll((BGRowRef tuple) => entityIds.Contains(tuple.EntityId)) == 0)
				{
					return;
				}
				try
				{
					FireStoredValueChanged(base.Meta[index], value, value);
				}
				catch (Exception ex)
				{
					if (exception != null)
					{
						exception = ex;
					}
				}
			});
		}
		if (exception != null)
		{
			throw exception;
		}
	}

	private void ClearToValue(BGId entityId, ref Exception exception)
	{
		List<BGEntity> list = ReverseCache.Get(entityId);
		if (list == null)
		{
			return;
		}
		foreach (BGEntity item in list)
		{
			List<BGRowRef> list2 = StoreGet(item.Index);
			if (list2 == null || list2.RemoveAll((BGRowRef tuple) => tuple.EntityId == entityId) == 0)
			{
				continue;
			}
			try
			{
				FireStoredValueChanged(item, list2, list2);
			}
			catch (Exception ex)
			{
				if (exception == null)
				{
					exception = ex;
				}
			}
		}
		ReverseCache.Remove(entityId);
	}

	public override List<BGEntity> GetRelatedIn(BGId entityId, List<BGEntity> result = null)
	{
		ReverseCache.Enable(enabled: true);
		if (ReverseCache.Enabled)
		{
			List<BGEntity> list = ReverseCache.Get(entityId);
			if (result != null)
			{
				result.Clear();
				if (list != null)
				{
					result.AddRange(list);
				}
			}
			else
			{
				result = ((list == null) ? new List<BGEntity>() : new List<BGEntity>(list));
			}
		}
		else
		{
			result = result ?? new List<BGEntity>();
			result.Clear();
			StoreForEachKeyValue((int index, List<BGRowRef> value) =>
			{
				if (!(value?.Find((BGRowRef tuple) => tuple.EntityId == entityId) == null))
				{
					BGEntity bGEntity = base.Meta[index];
					if (bGEntity != null)
					{
						result.Add(bGEntity);
					}
				}
			});
		}
		return result;
	}

	public override List<BGEntity> GetRelatedIn(HashSet<BGId> entityIds, List<BGEntity> result = null)
	{
		if (entityIds == null || entityIds.Count == 0)
		{
			result?.Clear();
			return result;
		}
		ReverseCache.Enable(enabled: true);
		if (ReverseCache.Enabled)
		{
			HashSet<BGEntity> hashSet = new HashSet<BGEntity>();
			foreach (BGId entityId in entityIds)
			{
				List<BGEntity> list = ReverseCache.Get(entityId);
				if (list == null)
				{
					continue;
				}
				foreach (BGEntity item in list)
				{
					hashSet.Add(item);
				}
			}
			if (result == null)
			{
				result = new List<BGEntity>(hashSet);
			}
			else
			{
				result.AddRange(hashSet);
			}
		}
		else
		{
			result = result ?? new List<BGEntity>();
			result.Clear();
			StoreForEachKeyValue((int index, List<BGRowRef> value) =>
			{
				if (value != null)
				{
					bool flag = false;
					for (int i = 0; i < value.Count; i++)
					{
						BGRowRef bGRowRef = value[i];
						if (entityIds.Contains(bGRowRef.EntityId))
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						BGEntity bGEntity = base.Meta[index];
						if (bGEntity != null)
						{
							result.Add(bGEntity);
						}
					}
				}
			});
		}
		return result;
	}

	public override byte[] ToBytes(int entityIndex)
	{
		List<BGRowRef> list = StoreItems[entityIndex];
		if (list == null || list.Count == 0)
		{
			return null;
		}
		byte[] result = new byte[list.Count * 32];
		for (int i = 0; i < list.Count; i++)
		{
			BGRowRef bGRowRef = list[i];
			int num = i * 32;
			bGRowRef.MetaId.ToByteArray(result, num);
			bGRowRef.EntityId.ToByteArray(result, num + 16);
		}
		return result;
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		if (segment.Count == 0)
		{
			ClearValueNoEventForRelation(entityIndex);
			return;
		}
		List<BGRowRef> list = StoreItems[entityIndex] ?? new List<BGRowRef>();
		for (int i = 0; i < segment.Count; i += 32)
		{
			int num = segment.Offset + i;
			BGId metaId = new BGId(segment.Array, num);
			BGId entityId = new BGId(segment.Array, num + 16);
			list.Add(new BGRowRef(metaId, entityId));
		}
		if (ReverseCache.Enabled)
		{
			ReverseSetRelated(entityIndex, list);
		}
		StoreItems[entityIndex] = list;
	}

	public void FromBytes(BGBinaryBulkRequestClass request)
	{
		byte[] array = request.Array;
		BGBinaryBulkRequestClass.CellRequest[] cellRequests = request.CellRequests;
		int num = cellRequests.Length;
		for (int i = 0; i < num; i++)
		{
			BGBinaryBulkRequestClass.CellRequest cellRequest = cellRequests[i];
			int entityIndex = cellRequest.EntityIndex;
			int offset = cellRequest.Offset;
			try
			{
				if (cellRequest.Count % 32 != 0)
				{
					throw new BGException("Can not convert byte array to value. Wrong byte array size $. Should be dividable by $", cellRequest.Count, 32);
				}
				int num2 = cellRequest.Count / 32;
				if (num2 == 0)
				{
					StoreItems[entityIndex] = null;
					continue;
				}
				List<BGRowRef> list = StoreItems[entityIndex];
				if (list == null)
				{
					list = new List<BGRowRef>(num2);
				}
				else
				{
					list.Clear();
					if (list.Capacity < num2)
					{
						list.Capacity = num2;
					}
				}
				StoreItems[entityIndex] = list;
				int num3 = offset + 32 * num2;
				for (int j = offset; j < num3; j += 32)
				{
					BGId metaId = new BGId(array, j);
					BGId entityId = new BGId(array, j + 16);
					list.Add(new BGRowRef(metaId, entityId));
				}
				if (ReverseCache.Enabled)
				{
					ReverseSetRelated(entityIndex, list);
				}
			}
			catch (Exception obj)
			{
				request.OnError?.Invoke(obj);
			}
		}
	}

	public override string ToString(int entityIndex)
	{
		List<BGRowRef> list = StoreItems[entityIndex];
		if (list == null || list.Count == 0)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (BGRowRef item in list)
		{
			if (!(item == null))
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append('|');
				}
				stringBuilder.Append(BGFieldRelationMA<List<BGEntity>, List<BGRowRef>>.RowRefToString(item, base.Repo).Replace("|", ""));
			}
		}
		string result = stringBuilder.ToString();
		stringBuilder.Length = 0;
		return result;
	}

	public override void FromString(int entityIndex, string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			ClearValueNoEventForRelation(entityIndex);
			return;
		}
		string[] array = value.Split(BGField<List<BGEntity>>.AA, StringSplitOptions.RemoveEmptyEntries);
		TempList.Clear();
		string[] array2 = array;
		foreach (string value2 in array2)
		{
			BGRowRef bGRowRef = BGFieldRelationMA<List<BGEntity>, List<BGRowRef>>.StringToRowRef(value2);
			if (bGRowRef != null)
			{
				TempList.Add(bGRowRef);
			}
		}
		if (TempList.Count == 0)
		{
			ClearValueNoEventForRelation(entityIndex);
		}
		else
		{
			if (ReverseCache.Enabled)
			{
				ReverseSetRelated(entityIndex, TempList);
			}
			List<BGRowRef> list = EnsureList(entityIndex);
			list.Clear();
			list.AddRange(TempList);
		}
		TempList.Clear();
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldManyRelationsMultiple(meta, id, name);
	}
}

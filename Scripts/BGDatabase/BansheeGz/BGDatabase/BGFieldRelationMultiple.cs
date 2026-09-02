using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "relationMultiple", Folder = "Relation", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerRelationMultiple")]
public class BGFieldRelationMultiple : BGFieldRelationSA<List<BGEntity>, List<BGId>>, BGBinaryBulkLoaderClass, BGFieldRelationMultipleI
{
	[Serializable]
	private struct JsonConfig
	{
		public string ToId;

		public bool AllowDuplicates;
	}

	public const ushort CodeType = 45;

	private static readonly List<BGId> TempList = new List<BGId>();

	private bool allowDuplicates;

	public override ushort TypeCode => 45;

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
			List<BGId> list = StoreItems[entityIndex];
			if (list == null || list.Count == 0)
			{
				return null;
			}
			List<BGEntity> list2 = new List<BGEntity>();
			for (int i = 0; i < list.Count; i++)
			{
				BGId entityId = list[i];
				BGEntity bGEntity = base.To[entityId];
				if (bGEntity != null)
				{
					list2.Add(bGEntity);
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

	public BGFieldRelationMultiple(BGMetaEntity meta, string name, BGMetaEntity to)
		: base(meta, name, to)
	{
	}

	internal BGFieldRelationMultiple(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	private List<BGId> EnsureList(int entityIndex)
	{
		List<BGId> list = StoreGet(entityIndex);
		if (list != null)
		{
			return list;
		}
		list = new List<BGId>();
		StoreSet(entityIndex, list);
		return list;
	}

	public override void OnEntityDelete(BGEntity entity)
	{
		if (ReverseCache.Enabled)
		{
			List<BGId> list = StoreGet(entity.Index);
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					ReverseCache.RemoveRelated(entity, list[i]);
				}
			}
		}
		base.OnEntityDelete(entity);
	}

	public override string ConfigToString()
	{
		return JsonUtility.ToJson(new JsonConfig
		{
			ToId = toId.ToString(),
			AllowDuplicates = allowDuplicates
		});
	}

	public override void ConfigFromString(string config)
	{
		JsonConfig jsonConfig = JsonUtility.FromJson<JsonConfig>(config);
		toId = new BGId(jsonConfig.ToId);
		allowDuplicates = jsonConfig.AllowDuplicates;
		ReverseCache.AllowDuplicates = allowDuplicates;
	}

	public override byte[] ConfigToBytes()
	{
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter(20);
		bGBinaryWriter.AddInt(2);
		bGBinaryWriter.AddId(toId);
		bGBinaryWriter.AddBool(allowDuplicates);
		return bGBinaryWriter.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		BGBinaryReader bGBinaryReader = new BGBinaryReader(config);
		int num = bGBinaryReader.ReadInt();
		switch (num)
		{
		case 1:
			toId = bGBinaryReader.ReadId();
			break;
		case 2:
			toId = bGBinaryReader.ReadId();
			allowDuplicates = bGBinaryReader.ReadBool();
			ReverseCache.AllowDuplicates = allowDuplicates;
			break;
		default:
			throw new BGException("Unknown version: $", num);
		}
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
		BGFieldRelationMultiple bGFieldRelationMultiple = (BGFieldRelationMultiple)fromField;
		List<BGId> list = bGFieldRelationMultiple.StoreGet(fromEntityIndex);
		ReverseSetRelated(num, list);
		if (list != null && list.Count > 0)
		{
			List<BGId> list2 = StoreGet(num);
			if (list2 == null)
			{
				StoreSet(num, new List<BGId>(list));
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

	protected override bool AreStoredValuesEqual(List<BGId> myValue, List<BGId> otherValue)
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
			BGId bGId = myValue[i];
			BGId bGId2 = otherValue[i];
			if (bGId != bGId2)
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
			if (bGEntity != null && bGEntity.Meta.Id != base.RelatedMeta.Id)
			{
				throw new BGException("Can not assign related entities: one of the entities has wrong meta! Expected $, actual $", base.RelatedMeta.Name, bGEntity.Meta.Name);
			}
		}
		BGEntity entity = base.Meta[entityIndex];
		List<BGId> list = StoreGet(entityIndex);
		if (list == null)
		{
			list = new List<BGId>(value.Count);
			StoreSet(entityIndex, list);
		}
		else
		{
			if (ReverseCache.Enabled)
			{
				foreach (BGId item in list)
				{
					ReverseCache.RemoveRelated(entity, item);
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
				list.Add(item2.Id);
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
		List<BGId> list = StoreGet(index);
		if (list == null || list.Count == 0)
		{
			return;
		}
		foreach (BGId item in list)
		{
			if (!item.IsEmpty)
			{
				ReverseCache.MarkDirty(item);
			}
		}
	}

	public override void SetStoredValue(int entityIndex, List<BGId> value)
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
		List<BGId> list = StoreGet(entityIndex);
		if (list != null && list.Count != 0)
		{
			BGEntity entity = base.Meta[entityIndex];
			for (int i = 0; i < list.Count; i++)
			{
				ReverseCache.RemoveRelated(entity, list[i]);
			}
		}
	}

	private void ReverseSetRelated(int entityIndex, List<BGId> value)
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
		foreach (BGId item in value)
		{
			ReverseCache.AddRelated(entity, item);
		}
	}

	protected override void BuildReverseCache()
	{
		StoreForEachKeyValue((int index, List<BGId> valList) =>
		{
			if (valList == null || valList.Count == 0)
			{
				return;
			}
			BGEntity entity = base.Meta[index];
			foreach (BGId val in valList)
			{
				if (!val.IsEmpty)
				{
					ReverseRelationCacheValueI reverseRelationCacheValueI = ReverseCache.Ensure(val);
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
			StoreForEachKeyValue((int index, List<BGId> value) =>
			{
				if (value == null || value.RemoveAll((BGId id1) => id1 == entityId) == 0)
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
			StoreForEachKeyValue((int index, List<BGId> value) =>
			{
				if (value == null || value.RemoveAll(entityIds.Contains) == 0)
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
			List<BGId> list2 = StoreGet(item.Index);
			if (list2 == null || list2.RemoveAll((BGId id1) => id1 == entityId) == 0)
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
			StoreForEachKeyValue((int index, List<BGId> value) =>
			{
				if (value != null && value.Contains(entityId))
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
				foreach (BGEntity item2 in list)
				{
					hashSet.Add(item2);
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
			StoreForEachKeyValue((int index, List<BGId> value) =>
			{
				if (value != null)
				{
					bool flag = false;
					for (int i = 0; i < value.Count; i++)
					{
						BGId item = value[i];
						if (entityIds.Contains(item))
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
		List<BGId> list = StoreItems[entityIndex];
		if (list == null || list.Count == 0)
		{
			return null;
		}
		List<byte> list2 = new List<byte>();
		for (int i = 0; i < list.Count; i++)
		{
			list2.AddRange(list[i].ToByteArray());
		}
		return list2.ToArray();
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		if (segment.Count == 0)
		{
			ClearValueNoEventForRelation(entityIndex);
			return;
		}
		List<BGId> list = StoreItems[entityIndex] ?? new List<BGId>();
		for (int i = 0; i < segment.Count; i += 16)
		{
			list.Add(new BGId(segment.Array, segment.Offset + i));
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
				if (cellRequest.Count % 16 != 0)
				{
					throw new BGException("Can not convert byte array to value. Wrong byte array size $. Should be dividable by $", cellRequest.Count, 16);
				}
				int num2 = cellRequest.Count / 16;
				if (num2 == 0)
				{
					ClearValueNoEventForRelation(entityIndex);
					continue;
				}
				List<BGId> list = StoreItems[entityIndex];
				if (list == null)
				{
					list = new List<BGId>(num2);
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
				int num3 = offset + 16 * num2;
				for (int j = offset; j < num3; j += 16)
				{
					list.Add(new BGId(array, j));
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
		List<BGId> list = StoreItems[entityIndex];
		if (list == null || list.Count == 0)
		{
			return "";
		}
		BGMetaEntity to = base.To;
		StringBuilder stringBuilder = new StringBuilder();
		foreach (BGId item in list)
		{
			if (stringBuilder.Length != 0)
			{
				stringBuilder.Append('|');
			}
			stringBuilder.Append(IdToString(item, to?[item]));
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
			TempList.Add(BGFieldRelationSA<List<BGEntity>, List<BGId>>.IdFromString(value2));
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
			List<BGId> list = EnsureList(entityIndex);
			list.Clear();
			list.AddRange(TempList);
		}
		TempList.Clear();
	}

	public static string IdToString(BGId entityId, BGEntity entity)
	{
		string text = entityId.ToString();
		string entityName = GetEntityName(entity);
		if (string.IsNullOrEmpty(entityName))
		{
			return text;
		}
		return entityName + "_" + text;
	}

	public static string GetEntityName(BGEntity entity)
	{
		string text = entity?.Name;
		if (text == null)
		{
			return null;
		}
		text = text.Trim();
		int num = text.IndexOf('|');
		if (num >= 0)
		{
			text = text.Replace("|", "");
		}
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		return text;
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldRelationMultiple(meta, id, name);
	}
}

using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "relationSingle", Folder = "Relation", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerRelationSingle")]
public class BGFieldRelationSingle : BGFieldRelationSA<BGEntity, BGId>, BGBinaryBulkLoaderClass, BGFieldRelationSingleI
{
	public const ushort CodeType = 46;

	public override ushort TypeCode => 46;

	public override bool CanBeUsedAsKey => true;

	public override BGEntity this[int entityIndex]
	{
		get
		{
			if (entityIndex >= StoreCount)
			{
				ThrowIndexOutOfBoundOnRead(entityIndex);
			}
			BGId entityId = StoreItems[entityIndex];
			if (!entityId.IsEmpty)
			{
				return base.To[entityId];
			}
			return null;
		}
		set
		{
			if (value != null && value.Meta.Id != base.RelatedMeta.Id)
			{
				throw new BGException("Can not assign related entity: meta is wrong! Expected $, actual $", base.RelatedMeta.Name, value.Meta.Name);
			}
			BGId value2 = value?.Id ?? BGId.Empty;
			ReverseSetRelated(entityIndex, value2);
			if (base.events.On)
			{
				BGEntity bGEntity = this[entityIndex];
				if (!object.Equals(value, bGEntity))
				{
					BGEntity entity = base.Meta[entityIndex];
					FireBeforeValueChanged(entity, bGEntity, value);
					StoreSet(entityIndex, value2);
					FireValueChanged(entity, bGEntity, value);
				}
			}
			else
			{
				StoreSet(entityIndex, value2);
			}
		}
	}

	public BGFieldRelationSingle(BGMetaEntity meta, string name, BGMetaEntity to)
		: base(meta, name, to)
	{
	}

	internal BGFieldRelationSingle(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	public override void OnEntityDelete(BGEntity entity)
	{
		if (ReverseCache.Enabled)
		{
			ReverseCache.RemoveRelated(entity, StoreGet(entity.Index));
		}
		base.OnEntityDelete(entity);
	}

	public BGEntity GetRelatedEntity(int entityIndex)
	{
		return this[entityIndex];
	}

	public void SetRelatedEntity(int entityIndex, BGEntity entity)
	{
		this[entityIndex] = entity;
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

	public override void SetStoredValue(int entityIndex, BGId value)
	{
		ReverseSetRelated(entityIndex, value);
		base.SetStoredValue(entityIndex, value);
	}

	public override void CopyValue(BGField fromField, BGId fromEntityId, int fromEntityIndex, BGId toEntityId)
	{
		if (fromEntityIndex != -1 && !fromField.IsDeleted)
		{
			int num = base.Meta.FindEntityIndex(toEntityId);
			if (num != -1)
			{
				BGFieldRelationSingle bGFieldRelationSingle = (BGFieldRelationSingle)fromField;
				BGId value = bGFieldRelationSingle.StoreGet(fromEntityIndex);
				ReverseSetRelated(num, value);
				StoreSet(num, value);
			}
		}
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
		BGId relatedId = StoreGet(index);
		if (!relatedId.IsEmpty)
		{
			ReverseCache.MarkDirty(relatedId);
		}
	}

	protected override void BuildReverseCache()
	{
		StoreForEachKeyValue((int index, BGId val) =>
		{
			if (!val.IsEmpty)
			{
				ReverseRelationCacheValueI reverseRelationCacheValueI = ReverseCache.Ensure(val);
				reverseRelationCacheValueI.Add(base.Meta[index]);
			}
		});
	}

	private void ReverseSetRelated(int entityIndex, BGId value)
	{
		if (ReverseCache.Enabled)
		{
			BGId storedValue = GetStoredValue(entityIndex);
			if (!(value == storedValue))
			{
				ReverseCache.RemoveRelated(entityIndex, storedValue);
				ReverseCache.AddRelated(entityIndex, value);
			}
		}
	}

	private void ReverseRemoveRelated(int index)
	{
		ReverseCache.RemoveRelated(index, StoreGet(index));
	}

	public override void ClearToValue(BGId id)
	{
		if (ReverseCache.Enabled)
		{
			List<BGEntity> list = ReverseCache.Get(id);
			if (list == null)
			{
				return;
			}
			try
			{
				foreach (BGEntity item in list)
				{
					BGId oldValue = StoreGet(item.Index);
					if (!oldValue.IsEmpty)
					{
						StoreSet(item.Index, BGId.Empty);
						FireStoredValueChanged(item, oldValue, BGId.Empty);
					}
				}
				return;
			}
			finally
			{
				ReverseCache.Remove(id);
			}
		}
		StoreForEachKeyValue((int index, BGId value) =>
		{
			if (!(value != id))
			{
				BGId oldValue2 = StoreGet(index);
				if (!oldValue2.IsEmpty)
				{
					StoreSet(index, BGId.Empty);
					FireStoredValueChanged(base.Meta[index], oldValue2, BGId.Empty);
				}
			}
		});
	}

	public override void ClearToValue(HashSet<BGId> entityIds)
	{
		if (entityIds == null || entityIds.Count == 0)
		{
			return;
		}
		Exception exception = null;
		if (ReverseCache.Enabled)
		{
			foreach (BGId entityId in entityIds)
			{
				List<BGEntity> list = ReverseCache.Get(entityId);
				if (list == null)
				{
					continue;
				}
				foreach (BGEntity item in list)
				{
					BGId oldValue = StoreGet(item.Index);
					if (oldValue.IsEmpty)
					{
						continue;
					}
					StoreSet(item.Index, BGId.Empty);
					try
					{
						FireStoredValueChanged(item, oldValue, BGId.Empty);
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
		}
		else
		{
			StoreForEachKeyValue((int index, BGId value) =>
			{
				if (entityIds.Contains(value))
				{
					BGId oldValue2 = StoreGet(index);
					if (!oldValue2.IsEmpty)
					{
						StoreSet(index, BGId.Empty);
						try
						{
							FireStoredValueChanged(base.Meta[index], oldValue2, BGId.Empty);
						}
						catch (Exception ex2)
						{
							if (exception == null)
							{
								exception = ex2;
							}
						}
					}
				}
			});
		}
		if (exception == null)
		{
			return;
		}
		throw exception;
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
			StoreForEachKeyValue((int index, BGId value) =>
			{
				if (!(value != entityId))
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
				result.Clear();
				result.AddRange(hashSet);
			}
		}
		else
		{
			result = result ?? new List<BGEntity>();
			result.Clear();
			StoreForEachKeyValue((int index, BGId id) =>
			{
				if (!id.IsEmpty && entityIds.Contains(id))
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

	public override byte[] ToBytes(int entityIndex)
	{
		BGId bGId = StoreItems[entityIndex];
		if (!(bGId == BGId.Empty))
		{
			return bGId.ToByteArray();
		}
		return null;
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		if (segment.Count != 16)
		{
			ClearValueNoEventForRelation(entityIndex);
			return;
		}
		BGId bGId = new BGId(segment.Array, segment.Offset);
		if (ReverseCache.Enabled)
		{
			ReverseSetRelated(entityIndex, bGId);
		}
		StoreItems[entityIndex] = bGId;
	}

	public void FromBytes(BGBinaryBulkRequestClass request)
	{
		byte[] array = request.Array;
		BGBinaryBulkRequestClass.CellRequest[] cellRequests = request.CellRequests;
		int num = cellRequests.Length;
		for (int i = 0; i < num; i++)
		{
			BGBinaryBulkRequestClass.CellRequest cellRequest = cellRequests[i];
			try
			{
				if (ReverseCache.Enabled)
				{
					ReverseSetRelated(cellRequest.EntityIndex, new BGId(array, cellRequest.Offset));
				}
				StoreItems[cellRequest.EntityIndex] = new BGId(array, cellRequest.Offset);
			}
			catch (Exception obj)
			{
				request.OnError?.Invoke(obj);
			}
		}
	}

	public override string ToString(int entityIndex)
	{
		BGId bGId = StoreItems[entityIndex];
		if (bGId == BGId.Empty)
		{
			return "";
		}
		return IdToString(bGId, base.To?[bGId]);
	}

	public override void FromString(int entityIndex, string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			ClearValueNoEventForRelation(entityIndex);
			return;
		}
		BGId bGId = BGFieldRelationSA<BGEntity, BGId>.IdFromString(value);
		if (ReverseCache.Enabled)
		{
			ReverseSetRelated(entityIndex, bGId);
		}
		StoreItems[entityIndex] = bGId;
	}

	public static string IdToString(BGId entityId, BGEntity entity)
	{
		string text = entityId.ToString();
		if (entity == null || string.IsNullOrEmpty(entity.Name))
		{
			return text;
		}
		return entity.Name.Trim() + "_" + text;
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldRelationSingle(meta, id, name);
	}
}

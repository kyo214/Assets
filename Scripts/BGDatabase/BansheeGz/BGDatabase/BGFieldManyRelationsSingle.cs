using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "manyTablesRelationSingle", Folder = "Relation", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerManyRelationsSingle")]
public class BGFieldManyRelationsSingle : BGFieldRelationMA<BGEntity, BGRowRef>, BGBinaryBulkLoaderClass, BGFieldRelationSingleI
{
	public const ushort CodeType = 43;

	public override ushort TypeCode => 43;

	public override BGEntity this[int entityIndex]
	{
		get
		{
			if (entityIndex >= StoreCount)
			{
				ThrowIndexOutOfBoundOnRead(entityIndex);
			}
			BGRowRef bGRowRef = StoreItems[entityIndex];
			if (bGRowRef == null)
			{
				return null;
			}
			return bGRowRef.GetEntity(base.Repo);
		}
		set
		{
			if (value == null)
			{
				ClearValue(entityIndex);
				ReverseRemoveRelated(entityIndex);
				return;
			}
			CheckMetaId(value);
			BGRowRef value2 = new BGRowRef(value);
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

	public BGFieldManyRelationsSingle(BGMetaEntity meta, string name, List<BGMetaEntity> to)
		: base(meta, name, to)
	{
	}

	internal BGFieldManyRelationsSingle(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	public override void OnEntityDelete(BGEntity entity)
	{
		if (ReverseCache.Enabled)
		{
			BGRowRef bGRowRef = StoreGet(entity.Index);
			if (bGRowRef != null)
			{
				ReverseCache.RemoveRelated(entity, bGRowRef.EntityId);
			}
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

	public override void SetStoredValue(int entityIndex, BGRowRef value)
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
				BGFieldManyRelationsSingle bGFieldManyRelationsSingle = (BGFieldManyRelationsSingle)fromField;
				BGRowRef value = bGFieldManyRelationsSingle.StoreGet(fromEntityIndex);
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
		BGRowRef bGRowRef = StoreGet(index);
		if (bGRowRef != null)
		{
			ReverseCache.MarkDirty(bGRowRef.EntityId);
		}
	}

	protected override void BuildReverseCache()
	{
		StoreForEachKeyValue((int index, BGRowRef val) =>
		{
			if (!(val == null))
			{
				ReverseRelationCacheValueI reverseRelationCacheValueI = ReverseCache.Ensure(val.EntityId);
				reverseRelationCacheValueI.Add(base.Meta[index]);
			}
		});
	}

	private void ReverseSetRelated(int entityIndex, BGRowRef value)
	{
		if (!ReverseCache.Enabled)
		{
			return;
		}
		BGRowRef storedValue = GetStoredValue(entityIndex);
		if (!object.Equals(value, storedValue))
		{
			if (storedValue != null)
			{
				ReverseCache.RemoveRelated(entityIndex, storedValue.EntityId);
			}
			if (value != null)
			{
				ReverseCache.AddRelated(entityIndex, value.EntityId);
			}
		}
	}

	private void ReverseRemoveRelated(int index)
	{
		if (ReverseCache.Enabled)
		{
			BGRowRef bGRowRef = StoreGet(index);
			if (bGRowRef != null)
			{
				ReverseCache.RemoveRelated(index, bGRowRef.EntityId);
			}
		}
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
					BGRowRef bGRowRef = StoreGet(item.Index);
					if (!(bGRowRef == null))
					{
						StoreSet(item.Index, null);
						FireStoredValueChanged(item, bGRowRef, null);
					}
				}
				return;
			}
			finally
			{
				ReverseCache.Remove(id);
			}
		}
		StoreForEachKeyValue((int index, BGRowRef value) =>
		{
			if (!(value == null) && !(value.EntityId != id))
			{
				StoreSet(index, null);
				FireStoredValueChanged(base.Meta[index], value, null);
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
					BGRowRef bGRowRef = StoreGet(item.Index);
					if (bGRowRef == null)
					{
						continue;
					}
					StoreSet(item.Index, null);
					try
					{
						FireStoredValueChanged(item, bGRowRef, null);
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
			StoreForEachKeyValue((int index, BGRowRef value) =>
			{
				if (value == null || !entityIds.Contains(value.EntityId))
				{
					return;
				}
				BGRowRef oldValue = StoreGet(index);
				StoreSet(index, null);
				try
				{
					FireStoredValueChanged(base.Meta[index], oldValue, null);
				}
				catch (Exception ex2)
				{
					if (exception == null)
					{
						exception = ex2;
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
			StoreForEachKeyValue((int index, BGRowRef value) =>
			{
				if (!(value == null) && !(value.EntityId != entityId))
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
			StoreForEachKeyValue((int index, BGRowRef tuple) =>
			{
				if (!(tuple == null) && entityIds.Contains(tuple.EntityId))
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

	protected override void OnRemoveRelatedMeta(BGMetaEntity metaEntity)
	{
		base.Meta.ForEachEntity((BGEntity entity) =>
		{
			BGEntity bGEntity = this[entity.Index];
			if (bGEntity != null && !(bGEntity.MetaId != metaEntity.Id))
			{
				this[entity.Index] = null;
			}
		});
	}

	public override byte[] ToBytes(int entityIndex)
	{
		BGRowRef bGRowRef = StoreItems[entityIndex];
		if (bGRowRef == null)
		{
			return null;
		}
		return bGRowRef.ToBytes();
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		if (segment.Count != 32)
		{
			ClearValueNoEventForRelation(entityIndex);
			return;
		}
		BGRowRef bGRowRef = new BGRowRef(segment);
		if (ReverseCache.Enabled)
		{
			ReverseSetRelated(entityIndex, bGRowRef);
		}
		StoreItems[entityIndex] = bGRowRef;
	}

	public void FromBytes(BGBinaryBulkRequestClass request)
	{
		byte[] array = request.Array;
		BGBinaryBulkRequestClass.CellRequest[] cellRequests = request.CellRequests;
		int num = cellRequests.Length;
		for (int i = 0; i < num; i++)
		{
			BGBinaryBulkRequestClass.CellRequest cellRequest = cellRequests[i];
			if (cellRequest.Count != 32)
			{
				ClearValueNoEventForRelation(cellRequest.EntityIndex);
				continue;
			}
			BGRowRef bGRowRef = new BGRowRef(array, cellRequest.Offset);
			if (ReverseCache.Enabled)
			{
				ReverseSetRelated(cellRequest.EntityIndex, bGRowRef);
			}
			StoreItems[cellRequest.EntityIndex] = bGRowRef;
		}
	}

	public override string ToString(int entityIndex)
	{
		BGRowRef rowRef = StoreItems[entityIndex];
		return BGFieldRelationMA<BGEntity, BGRowRef>.RowRefToString(rowRef, base.Repo);
	}

	public override void FromString(int entityIndex, string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			ClearValueNoEventForRelation(entityIndex);
			return;
		}
		value = value.Trim();
		BGRowRef bGRowRef = BGFieldRelationMA<BGEntity, BGRowRef>.StringToRowRef(value);
		if (ReverseCache.Enabled)
		{
			ReverseSetRelated(entityIndex, bGRowRef);
		}
		StoreItems[entityIndex] = bGRowRef;
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldManyRelationsSingle(meta, id, name);
	}
}

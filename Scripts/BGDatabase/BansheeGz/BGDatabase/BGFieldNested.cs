using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "nested", Folder = "Relation", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerNested")]
public class BGFieldNested : BGField<List<BGEntity>>, BGRelationI, BGAbstractRelationI, BGFieldWithCustomConfigI
{
	[Serializable]
	private struct JsonConfig
	{
		public string NestedMetaId;

		public string OwnerRelationId;
	}

	public const ushort CodeType = 44;

	protected BGId ownerRelationId;

	protected BGId nestedMetaId;

	public override ushort TypeCode => 44;

	public BGId OwnerRelationId => ownerRelationId;

	public BGFieldRelationSingle OwnerRelation => NestedMeta.GetFieldAs<BGFieldRelationSingle>(ownerRelationId);

	public BGId NestedMetaId => nestedMetaId;

	public BGId ToId => nestedMetaId;

	public BGMetaNested NestedMeta => base.Meta.Repo.GetMeta<BGMetaNested>(nestedMetaId);

	public override bool System
	{
		get
		{
			return base.System;
		}
		set
		{
			base.System = value;
			BGMetaNested nestedMeta = NestedMeta;
			if (nestedMeta != null)
			{
				nestedMeta.System = value;
			}
		}
	}

	public override bool ReadOnly => true;

	public override bool StoredValueIsTheSameAsValueType => false;

	public override bool EmptyContent => true;

	public BGMetaEntity RelatedMeta => NestedMeta;

	public override List<BGEntity> this[BGId id]
	{
		get
		{
			return GetRelatedIn(id);
		}
		set
		{
		}
	}

	public override List<BGEntity> this[int entityIndex]
	{
		get
		{
			return GetRelatedIn(base.Meta[entityIndex].Id);
		}
		set
		{
		}
	}

	public BGFieldNested(BGMetaEntity meta, string name)
		: base(meta, name)
	{
		if (meta.Repo.HasMeta(name))
		{
			Unregister();
			throw new BGException("Meta with name ($) already exists!", name);
		}
		BGMetaNested bGMetaNested = CreateNestedMeta(meta, name);
		nestedMetaId = bGMetaNested.Id;
		ownerRelationId = bGMetaNested.OwnerRelationId;
	}

	protected internal BGFieldNested(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected virtual BGMetaNested CreateNestedMeta(BGMetaEntity meta, string name)
	{
		return new BGMetaNested(meta.Repo, name, meta);
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldNested(meta, id, name);
	}

	public override void Delete()
	{
		Unregister();
		NestedMeta.Delete();
		base.Delete();
	}

	public override void OnEntityDelete(BGEntity entity)
	{
		List<BGEntity> relatedIn = OwnerRelation.GetRelatedIn(entity.Id);
		if (BGUtil.IsEmpty(relatedIn))
		{
			return;
		}
		foreach (BGEntity item in relatedIn)
		{
			item.Delete();
		}
	}

	public List<BGEntity> GetRelatedIn(BGId entityId, List<BGEntity> result = null)
	{
		BGEntity bGEntity = base.Meta[entityId];
		if (bGEntity == null)
		{
			return null;
		}
		return NestedMeta.GetNested(bGEntity, result);
	}

	public List<BGEntity> GetRelatedIn(HashSet<BGId> entityIds, List<BGEntity> result = null)
	{
		if (entityIds == null || entityIds.Count == 0)
		{
			return result;
		}
		BGMetaNested nestedMeta = NestedMeta;
		if (nestedMeta == null)
		{
			return result;
		}
		BGFieldRelationSingle ownerRelation = nestedMeta.OwnerRelation;
		if (ownerRelation == null)
		{
			return result;
		}
		return ownerRelation.GetRelatedIn(entityIds, result);
	}

	public void ClearToValue(BGId id)
	{
	}

	public void ClearToValue(HashSet<BGId> entityIds)
	{
	}

	public override void CopyValue(BGField fromField, BGId fromEntityId, int fromEntityIndex, BGId toEntityId)
	{
	}

	public override void ClearValue(int entityIndex)
	{
	}

	public override void OnDelete()
	{
	}

	public override void ClearValues()
	{
	}

	public override void ForEachValue(Action<int> action)
	{
	}

	public override void Swap(int entityIndex1, int entityIndex2)
	{
	}

	public override void MoveEntitiesValues(int fromIndex, int toIndex, int numberOfValues)
	{
	}

	public override void DuplicateValue(BGId fromEntityId, int fromEntityIndex, BGId toEntityId)
	{
		if (fromEntityIndex == -1 || base.IsDeleted)
		{
			return;
		}
		int num = base.Meta.FindEntityIndex(toEntityId);
		if (num == -1)
		{
			return;
		}
		BGFieldRelationSingle ownerRelation = NestedMeta.OwnerRelation;
		List<BGEntity> relatedIn = ownerRelation.GetRelatedIn(fromEntityId);
		if (relatedIn != null && relatedIn.Count != 0)
		{
			for (int i = 0; i < relatedIn.Count; i++)
			{
				BGEntity bGEntity = relatedIn[i];
				BGEntity bGEntity2 = bGEntity.Duplicate();
				ownerRelation.SetStoredValue(bGEntity2.Index, toEntityId);
			}
		}
	}

	public override bool AreStoredValuesEqual(BGField field, int myEntityIndex, int otherEntityIndex)
	{
		return true;
	}

	public override string ConfigToString()
	{
		return JsonUtility.ToJson(new JsonConfig
		{
			NestedMetaId = nestedMetaId.ToString(),
			OwnerRelationId = ownerRelationId.ToString()
		});
	}

	public override void ConfigFromString(string config)
	{
		JsonConfig jsonConfig = JsonUtility.FromJson<JsonConfig>(config);
		nestedMetaId = new BGId(jsonConfig.NestedMetaId);
		ownerRelationId = new BGId(jsonConfig.OwnerRelationId);
	}

	public override byte[] ConfigToBytes()
	{
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter(36);
		bGBinaryWriter.AddInt(1);
		bGBinaryWriter.AddId(nestedMetaId);
		bGBinaryWriter.AddId(ownerRelationId);
		return bGBinaryWriter.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		BGBinaryReader bGBinaryReader = new BGBinaryReader(config);
		int num = bGBinaryReader.ReadInt();
		if (num == 1)
		{
			nestedMetaId = bGBinaryReader.ReadId();
			ownerRelationId = bGBinaryReader.ReadId();
			return;
		}
		throw new BGException("Unknown version: $", num);
	}

	public override byte[] ToBytes(int entityIndex)
	{
		return Array.Empty<byte>();
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
	}

	public override string ToString(int entityIndex)
	{
		return "";
	}

	public override void FromString(int entityIndex, string value)
	{
	}
}

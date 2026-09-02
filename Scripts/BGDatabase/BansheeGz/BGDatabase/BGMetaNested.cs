using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[MetaDescriptor(Name = "nested", ManagerType = "BansheeGz.BGDatabase.Editor.BGMetaManagerNested", SkipInList = true)]
public class BGMetaNested : BGMetaEntity
{
	[Serializable]
	private struct JsonConfig
	{
		public string OwnerRelationId;
	}

	public const ushort CodeType = 2;

	protected BGId ownerRelationId;

	public BGMetaEntity Owner => OwnerRelation.To;

	public BGId OwnerRelationId => ownerRelationId;

	public BGFieldRelationSingle OwnerRelation => (BGFieldRelationSingle)GetField(ownerRelationId);

	public override ushort TypeCode => 2;

	public BGMetaNested(BGRepo repo, string name, BGMetaEntity owner)
		: base(repo, name)
	{
		if (owner == null)
		{
			Unregister();
			throw new BGException("Owner can not be null");
		}
		ownerRelationId = new BGFieldRelationSingle(this, owner.Name, owner)
		{
			System = true
		}.Id;
	}

	protected internal BGMetaNested(BGRepo repo, BGId id, string name)
		: base(repo, id, name)
	{
	}

	protected override Func<BGRepo, BGId, string, BGMetaEntity> CreateMetaFactory()
	{
		return (BGRepo repo, BGId id, string name) => new BGMetaNested(repo, id, name);
	}

	public override string ConfigToString()
	{
		return JsonUtility.ToJson(new JsonConfig
		{
			OwnerRelationId = ownerRelationId.ToString()
		});
	}

	public override void ConfigFromString(string config)
	{
		ownerRelationId = new BGId(JsonUtility.FromJson<JsonConfig>(config).OwnerRelationId);
	}

	public override byte[] ConfigToBytes()
	{
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter(20);
		bGBinaryWriter.AddInt(1);
		bGBinaryWriter.AddId(ownerRelationId);
		return bGBinaryWriter.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		BGBinaryReader bGBinaryReader = new BGBinaryReader(config);
		int num = bGBinaryReader.ReadInt();
		if (num == 1)
		{
			ownerRelationId = bGBinaryReader.ReadId();
			return;
		}
		throw new BGException("Unknown version: $", num);
	}

	public BGEntity NewEntity(BGEntity owner)
	{
		CheckOwner(owner);
		return NewEntity(new NewEntityContext((BGEntity entity) =>
		{
			entity.Set(ownerRelationId, owner);
		}));
	}

	public BGEntity NewEntity(BGEntity owner, BGId entityId)
	{
		CheckOwner(owner);
		return NewEntity(new NewEntityContext(entityId, (BGEntity entity) =>
		{
			entity.Set(ownerRelationId, owner);
		}));
	}

	public BGEntity NewEntity(BGEntity owner, NewEntityContext context)
	{
		CheckOwner(owner);
		return NewEntity(new NewEntityContext((BGEntity entity) =>
		{
			entity.Set(ownerRelationId, owner);
			context?.Callback?.Invoke(entity);
		}));
	}

	private void CheckOwner(BGEntity owner)
	{
		if (owner == null || owner.MetaId == OwnerRelation.ToId)
		{
			return;
		}
		BGMetaEntity to = OwnerRelation.To;
		if (to != null)
		{
			throw new BGException("Can not create a nested entity, cause the owner entity has a wrong meta, expected [$] actual [$]", to.Name, owner.MetaName);
		}
		throw new BGException("Can not create a nested entity, cause the owner entity has a wrong meta, expected meta ID [$] actual meta ID [$]", OwnerRelation.ToId, owner.MetaId);
	}

	public List<BGEntity> GetNested(BGEntity owner, List<BGEntity> result = null)
	{
		BGFieldRelationSingle ownerRelation = OwnerRelation;
		if (ownerRelation.To.Id != owner.Meta.Id)
		{
			throw new BGException("Error from Nested meta $. Incorrect type of owner ($). The type should be equal to $", Name, owner.Meta.Name, ownerRelation.To.Name);
		}
		result = result ?? new List<BGEntity>();
		result.Clear();
		ownerRelation.GetRelatedIn(owner.Id, result);
		return result;
	}

	public BGEntity GetOwner(BGEntity nestedEntity)
	{
		return nestedEntity?.Get<BGEntity>(ownerRelationId);
	}
}

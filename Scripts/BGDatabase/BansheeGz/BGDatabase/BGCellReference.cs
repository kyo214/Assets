using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGCellReference : BGFieldReference
{
	[SerializeField]
	private string entityId;

	private BGEntity entity;

	public object Value
	{
		get
		{
			return GetField().GetValue(GetEntity().Index);
		}
		set
		{
			GetField().SetValue(GetEntity().Index, value);
		}
	}

	public BGEntity GetEntity()
	{
		if (entity != null && entity.Meta != null && !entity.Meta.IsDeleted && entity.Id == BGId.Parse(entityId))
		{
			return entity;
		}
		BGMetaEntity meta = base.Meta;
		if (meta == null)
		{
			return null;
		}
		entity = meta.GetEntity(BGId.Parse(entityId));
		return entity;
	}

	public void SetEntity(BGEntity entity)
	{
		if (entity == null)
		{
			Reset();
			return;
		}
		BGId metaIdConstraint = MetaIdConstraint;
		if (!metaIdConstraint.IsEmpty && entity.MetaId != metaIdConstraint)
		{
			string text = entity.Meta.Id.ToString();
			BGId bGId = metaIdConstraint;
			throw new Exception("Can not assign entity, cause meta is wrong. Ids mismatch " + text + "!=" + bGId.ToString());
		}
		metaId = entity.MetaId.ToString();
		entityId = entity.Id.ToString();
		this.entity = entity;
	}

	public override void Reset()
	{
		base.Reset();
		entityId = null;
		entity = null;
	}

	protected bool Equals(BGCellReference other)
	{
		if (Equals((BGFieldReference)other))
		{
			return entityId == other.entityId;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((BGCellReference)obj);
	}

	public override int GetHashCode()
	{
		return (base.GetHashCode() * 397) ^ ((entityId != null) ? entityId.GetHashCode() : 0);
	}
}
[Serializable]
public class BGCellReference<T> : BGCellReference
{
	public T ValueCasted
	{
		get
		{
			return (T)GetField().GetValue(GetEntity().Index);
		}
		set
		{
			GetField().SetValue(GetEntity().Index, value);
		}
	}
}

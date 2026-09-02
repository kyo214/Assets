using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGEntityReference : BGMetaReference
{
	public class EntityChangedEventArgs : EventArgs
	{
		public readonly BGEntity OldEntity;

		public readonly BGEntity NewEntity;

		public EntityChangedEventArgs(BGEntity oldEntity, BGEntity newEntity)
		{
			OldEntity = oldEntity;
			NewEntity = newEntity;
		}
	}

	[SerializeField]
	private string entityId;

	private BGEntity entity;

	public event EventHandler<EntityChangedEventArgs> OnEntityChanged;

	public BGEntityReference()
	{
	}

	public BGEntityReference(BGEntity entity)
	{
		SetEntity(entity);
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
		BGEntity oldEntity = ((OnEntityChanged != null) ? GetEntity() : null);
		if (entity == null)
		{
			Reset();
		}
		else
		{
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
		OnEntityChanged?.Invoke(this, new EntityChangedEventArgs(oldEntity, entity));
	}

	public override void Reset()
	{
		base.Reset();
		entityId = null;
		entity = null;
	}

	protected bool Equals(BGEntityReference other)
	{
		if (Equals((BGMetaReference)other))
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
		return Equals((BGEntityReference)obj);
	}

	public override int GetHashCode()
	{
		return (base.GetHashCode() * 397) ^ ((entityId != null) ? entityId.GetHashCode() : 0);
	}
}
[Serializable]
public abstract class BGEntityReference<T> : BGEntityReference where T : BGEntity
{
	public class EntityChangedEventArgs2 : EventArgs
	{
		public readonly T OldEntity;

		public readonly T NewEntity;

		public EntityChangedEventArgs2(T oldEntity, T newEntity)
		{
			OldEntity = oldEntity;
			NewEntity = newEntity;
		}
	}

	public T Entity
	{
		get
		{
			return (T)GetEntity();
		}
		set
		{
			T oldEntity = ((OnEntityChanged2 != null) ? Entity : null);
			SetEntity(value);
			OnEntityChanged2?.Invoke(this, new EntityChangedEventArgs2(oldEntity, value));
		}
	}

	public override BGId MetaIdConstraint => TargetMetaId;

	public abstract BGId TargetMetaId { get; }

	public event EventHandler<EntityChangedEventArgs2> OnEntityChanged2;

	public static implicit operator T(BGEntityReference<T> reference)
	{
		return reference.Entity;
	}
}

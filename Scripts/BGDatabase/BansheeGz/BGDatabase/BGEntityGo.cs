using System;
using UnityEngine;
using UnityEngine.Events;

namespace BansheeGz.BGDatabase;

[AddComponentMenu("BansheeGz/BGEntityGo")]
public class BGEntityGo : MonoBehaviour
{
	[Serializable]
	public class EntityChangedEvent : UnityEvent<BGEntityGo>
	{
	}

	public class EntityChangedEventArgs : EventArgs
	{
		public readonly BGEntityGo Component;

		public readonly BGEntity OldEntity;

		public readonly BGEntity NewEntity;

		public EntityChangedEventArgs(BGEntityGo component, BGEntity oldEntity, BGEntity newEntity)
		{
			Component = component;
			OldEntity = oldEntity;
			NewEntity = newEntity;
		}
	}

	public EntityChangedEvent OnEntityChange = new EntityChangedEvent();

	[HideInInspector]
	[SerializeField]
	private string entityIdString;

	private BGEntity entity;

	[HideInInspector]
	[SerializeField]
	private string metaIdString;

	[HideInInspector]
	[SerializeField]
	private bool initWithFirst;

	private BGId EntityBGId
	{
		get
		{
			string text = entityIdString;
			if (text != null && text.Length == 22)
			{
				return new BGId(entityIdString);
			}
			return BGId.Empty;
		}
	}

	private BGId MetaBGId
	{
		get
		{
			string text = metaIdString;
			if (text != null && text.Length == 22)
			{
				return new BGId(metaIdString);
			}
			return BGId.Empty;
		}
	}

	public BGId EntityId
	{
		get
		{
			if (!initWithFirst)
			{
				return EntityBGId;
			}
			return GetFirst()?.Id ?? BGId.Empty;
		}
		set
		{
			if (!initWithFirst && !(value == EntityBGId) && (!(value != BGId.Empty) || SetUpEntity(value)))
			{
				BGEntity oldEntity = ((OnEntityChange2 != null) ? Entity : null);
				SetEntityId(value);
				EntityChanged();
				OnEntityChange?.Invoke(this);
				OnEntityChange2?.Invoke(this, new EntityChangedEventArgs(this, oldEntity, entity));
			}
		}
	}

	public BGEntity Entity
	{
		get
		{
			if (initWithFirst)
			{
				return GetFirst();
			}
			BGId entityBGId = EntityBGId;
			if ((entity == null || entity.Id != entityBGId || entity.IsDeleted || entity.Meta.IsDeleted) && entityBGId != BGId.Empty)
			{
				SetUpEntity(entityBGId);
			}
			return entity;
		}
		set
		{
			if (!initWithFirst)
			{
				BGEntity oldEntity = ((OnEntityChange2 != null) ? Entity : null);
				entity = value;
				if (entity != null)
				{
					SetEntityId(entity.Id);
					SetMetaId(entity.MetaId);
				}
				else
				{
					SetEntityId(BGId.Empty);
				}
				EntityChanged();
				OnEntityChange?.Invoke(this);
				OnEntityChange2?.Invoke(this, new EntityChangedEventArgs(this, oldEntity, entity));
			}
		}
	}

	public BGId MetaId
	{
		get
		{
			return MetaBGId;
		}
		set
		{
			if (MetaConstraint == null)
			{
				BGId metaBGId = MetaBGId;
				if (!object.Equals(value, metaBGId))
				{
					SetMetaId(value);
					SetEntityId(BGId.Empty);
				}
			}
		}
	}

	public virtual BGMetaEntity Meta
	{
		get
		{
			return MetaConstraint ?? BGRepo.I.GetMeta(MetaBGId);
		}
		set
		{
			if (MetaConstraint != null)
			{
				return;
			}
			if (value == null)
			{
				SetMetaId(BGId.Empty);
				SetEntityId(BGId.Empty);
			}
			else if (!(MetaBGId == value.Id))
			{
				SetMetaId(value.Id);
				SetEntityId(BGId.Empty);
				if (initWithFirst)
				{
					SetUpFirst();
				}
			}
		}
	}

	public virtual BGMetaEntity MetaConstraint => null;

	public bool InitWithFirst
	{
		get
		{
			return initWithFirst;
		}
		set
		{
			if (initWithFirst != value)
			{
				initWithFirst = value;
				SetUpFirst();
			}
		}
	}

	public event EventHandler<EntityChangedEventArgs> OnEntityChange2;

	private void SetEntityId(BGId value)
	{
		if (value == BGId.Empty)
		{
			entityIdString = null;
			entity = null;
		}
		else
		{
			entityIdString = value.ToString();
		}
	}

	private void SetMetaId(BGId metaId)
	{
		metaIdString = ((metaId == BGId.Empty) ? null : metaId.ToString());
	}

	private void SetUpFirst()
	{
		if (initWithFirst)
		{
			BGMetaEntity meta = Meta;
			if (meta != null && meta.CountEntities > 0)
			{
				SetEntityId(meta[0].Id);
			}
		}
	}

	private BGEntity GetFirst()
	{
		BGMetaEntity meta = Meta;
		if (meta == null)
		{
			return null;
		}
		if (meta.CountEntities != 0)
		{
			return meta[0];
		}
		return null;
	}

	public virtual void EntityChanged()
	{
	}

	public virtual void Awake()
	{
	}

	public virtual void Start()
	{
	}

	public virtual void OnDestroy()
	{
	}

	public T Get<T>(string name)
	{
		return Entity.Get<T>(name);
	}

	public void Set<T>(string name, T value)
	{
		Entity.Set(name, value);
	}

	public T Get<T>(BGId id)
	{
		return Entity.Get<T>(id);
	}

	public void Set<T>(BGId id, T value)
	{
		Entity.Set(id, value);
	}

	private bool SetUpEntity(BGId newEntityId)
	{
		entity = Meta?[newEntityId];
		if (entity != null)
		{
			return true;
		}
		entity = BGRepo.I.GetEntity(newEntityId);
		if (entity == null)
		{
			return true;
		}
		BGMetaEntity metaConstraint = MetaConstraint;
		if (metaConstraint != null && entity.MetaId != metaConstraint.Id)
		{
			entity = null;
			return false;
		}
		SetMetaId(entity.MetaId);
		return true;
	}
}

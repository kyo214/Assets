using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[AddComponentMenu("BansheeGz/BGDataBinderRowGo")]
public class BGDataBinderRowGo : BGDataBinderGoA
{
	[SerializeField]
	[HideInInspector]
	private Component targetComponent;

	[SerializeField]
	[HideInInspector]
	private long metaIdKey1;

	[SerializeField]
	[HideInInspector]
	private long metaIdKey2;

	[SerializeField]
	[HideInInspector]
	private long entityIdKey1;

	[SerializeField]
	[HideInInspector]
	private long entityIdKey2;

	[SerializeField]
	[HideInInspector]
	private bool ignoreBaseTypes;

	[SerializeField]
	[HideInInspector]
	private bool liveUpdate;

	private BGMetaEntity meta;

	private BGEntity entity;

	private bool listenersWasAdded;

	public override string Error
	{
		get
		{
			if (targetComponent == null)
			{
				return "Target component is not set";
			}
			BGEntity bGEntity = Entity;
			if (bGEntity == null)
			{
				BGMetaEntity bGMetaEntity = Meta;
				if (bGMetaEntity == null)
				{
					if (metaIdKey1 != 0L || metaIdKey2 != 0L)
					{
						return "Can not find meta with id " + new BGId(metaIdKey1, metaIdKey2).ToString();
					}
					return "Meta is not defined";
				}
				if (entityIdKey1 != 0L || entityIdKey2 != 0L)
				{
					return "Can not find entity with id " + new BGId(entityIdKey1, entityIdKey2).ToString();
				}
				return "Entity is not defined";
			}
			return null;
		}
	}

	public Component TargetComponent
	{
		get
		{
			return targetComponent;
		}
		set
		{
			targetComponent = value;
		}
	}

	public bool IgnoreBaseTypes
	{
		get
		{
			return ignoreBaseTypes;
		}
		set
		{
			ignoreBaseTypes = value;
		}
	}

	public bool LiveUpdate
	{
		get
		{
			return liveUpdate;
		}
		set
		{
			liveUpdate = value;
		}
	}

	public BGId MetaId => Meta?.Id ?? BGId.Empty;

	public BGMetaEntity Meta
	{
		get
		{
			bool flag = false;
			if (meta == null)
			{
				flag = true;
			}
			else if (meta.IsDeleted)
			{
				flag = true;
			}
			else
			{
				meta.Id.ToLongKeys(out var key, out var key2);
				if (metaIdKey1 != key || metaIdKey2 != key2)
				{
					flag = true;
				}
			}
			if (flag)
			{
				meta = BGRepo.I[new BGId(metaIdKey1, metaIdKey2)];
			}
			return meta;
		}
	}

	public BGId EntityId => Entity?.Id ?? BGId.Empty;

	public BGEntity Entity
	{
		get
		{
			if (entity != null && !entity.Meta.IsDeleted)
			{
				entity.Id.ToLongKeys(out var key, out var key2);
				if (entityIdKey1 == key && entityIdKey2 == key2)
				{
					return entity;
				}
			}
			BGMetaEntity bGMetaEntity = Meta;
			if (bGMetaEntity != null)
			{
				entity = bGMetaEntity[new BGId(entityIdKey1, entityIdKey2)];
			}
			return entity;
		}
		set
		{
			if (value == null)
			{
				entity = null;
				entityIdKey1 = 0L;
				entityIdKey2 = 0L;
			}
			else if (!(entity.Id == new BGId(entityIdKey1, entityIdKey2)))
			{
				entity = value;
				entity.Id.ToLongKeys(out entityIdKey1, out entityIdKey2);
				entity.Meta.Id.ToLongKeys(out metaIdKey1, out metaIdKey2);
				meta = entity.Meta;
				Bind();
			}
		}
	}

	protected bool IsLiveUpdateOn
	{
		get
		{
			if (liveUpdate && (Application.isPlaying || BGUtil.TestIsRunning))
			{
				return Error == null;
			}
			return false;
		}
	}

	private bool HasError
	{
		get
		{
			if (Error == null)
			{
				return false;
			}
			LogError(Error);
			return true;
		}
	}

	private void Reset()
	{
		AutoConfig();
	}

	private void AutoConfig()
	{
		Component[] components = GetComponents<Component>();
		List<Component> list = new List<Component>();
		foreach (Component component in components)
		{
			if (!(component == this))
			{
				list.Add(component);
			}
		}
		List<BGMetaEntity> list2 = BGRepo.I.FindMetas();
		for (int j = 0; j < list2.Count; j++)
		{
			BGMetaEntity bGMetaEntity = list2[j];
			foreach (Component component2 in components)
			{
				if (string.Equals(bGMetaEntity.Name, component2.GetType().Name))
				{
					targetComponent = component2;
					bGMetaEntity.Id.ToLongKeys(out metaIdKey1, out metaIdKey2);
					return;
				}
			}
		}
	}

	protected override void FirstBind()
	{
		if (!HasError)
		{
			Bind();
			AddListeners();
		}
	}

	public override void Bind()
	{
		if (HasError)
		{
			return;
		}
		if (!bindedOnce)
		{
			bindedOnce = true;
			FirstBind();
		}
		else
		{
			BGMetaEntity bGMetaEntity = Meta;
			BGEntity bGEntity = Entity;
			BGDBAutoMapRegistry.Instance.GetAutoMappedConfig(bGMetaEntity, targetComponent.GetType(), !ignoreBaseTypes).Bind(bGEntity, targetComponent, detailedLog: false, (string s) =>
			{
				LogError("BGDatabase Error: Can not bind a value using BGDataBinderRowGo binder. " + s);
			});
		}
		FireOnBind();
	}

	public override void ReverseBind()
	{
		if (!HasError)
		{
			BGMetaEntity bGMetaEntity = Meta;
			BGEntity bGEntity = Entity;
			BGDBAutoMapRegistry.Instance.GetAutoMappedConfig(bGMetaEntity, targetComponent.GetType(), !ignoreBaseTypes).ReverseBind(bGEntity, targetComponent, (string s) =>
			{
				LogError("BGDatabase Error: Can not reverse bind a value using BGDataBinderRowGo binder. " + s);
			});
		}
	}

	private void AddListeners()
	{
		if (IsLiveUpdateOn && !listenersWasAdded)
		{
			listenersWasAdded = true;
			BGRepo.OnLoad += OnLoad;
			BGRepo.I.Events.OnBatchUpdate += OnBatch;
			BGEntity bGEntity = Entity;
			bGEntity?.Meta.AddEntityUpdatedListener(bGEntity.Id, EntityIsChanged);
		}
	}

	private void RemoveListeners()
	{
		if (listenersWasAdded)
		{
			BGRepo.OnLoad -= OnLoad;
			BGRepo.I.Events.OnBatchUpdate -= OnBatch;
			BGEntity bGEntity = Entity;
			bGEntity?.Meta.RemoveEntityUpdatedListener(bGEntity.Id, EntityIsChanged);
		}
	}

	private void EntityIsChanged(object sender, BGEventArgsEntityUpdated e)
	{
		Bind();
	}

	private void OnLoad(bool loaded)
	{
		if (loaded)
		{
			Bind();
		}
	}

	private void OnBatch(object sender, BGEventArgsBatch e)
	{
		if (e.WasEntitiesUpdated(MetaId))
		{
			Bind();
		}
	}

	protected override void OnDestroy()
	{
		RemoveListeners();
	}
}

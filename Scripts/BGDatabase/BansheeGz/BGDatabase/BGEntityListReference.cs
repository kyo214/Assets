using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGEntityListReference : BGMetaReference
{
	[SerializeField]
	private List<string> entityIds;

	public List<BGEntity> GetEntities()
	{
		if (entityIds == null || entityIds.Count == 0)
		{
			return null;
		}
		BGMetaEntity meta = base.Meta;
		if (meta == null)
		{
			return null;
		}
		List<BGEntity> list = new List<BGEntity>();
		foreach (string entityId in entityIds)
		{
			BGEntity entity = meta.GetEntity(BGId.Parse(entityId));
			if (entity != null)
			{
				list.Add(entity);
			}
		}
		return list;
	}

	public void SetEntities(List<BGEntity> entities)
	{
		if (entities == null || entities.Count == 0)
		{
			Reset();
			return;
		}
		BGId metaIdConstraint = MetaIdConstraint;
		if (metaIdConstraint.IsEmpty)
		{
			metaIdConstraint = entities[0].MetaId;
		}
		List<string> list = new List<string>();
		foreach (BGEntity entity in entities)
		{
			if (metaIdConstraint != entity.MetaId)
			{
				string text = entity.Meta.Id.ToString();
				BGId bGId = metaIdConstraint;
				throw new Exception("Can not assign entities, cause meta is wrong for one of the entities. Meta ids mismatch " + text + "!=" + bGId.ToString());
			}
			list.Add(entity.Id.ToString());
		}
		entityIds = list;
	}

	public override void Reset()
	{
		base.Reset();
		entityIds = null;
	}
}
[Serializable]
public abstract class BGEntityListReference<T> : BGEntityListReference where T : BGEntity
{
	public List<T> Entities
	{
		get
		{
			List<T> list = new List<T>();
			List<BGEntity> entities = GetEntities();
			if (entities == null || entities.Count == 0)
			{
				return null;
			}
			foreach (BGEntity item in entities)
			{
				list.Add((T)item);
			}
			return list;
		}
		set
		{
			if (value == null || value.Count == 0)
			{
				SetEntities(null);
				return;
			}
			List<BGEntity> list = new List<BGEntity>();
			foreach (T item in value)
			{
				list.Add(item);
			}
			SetEntities(list);
		}
	}

	public override BGId MetaIdConstraint => TargetMetaId;

	public abstract BGId TargetMetaId { get; }

	public static implicit operator List<T>(BGEntityListReference<T> reference)
	{
		return reference.Entities;
	}
}

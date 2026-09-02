using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGMTFieldManyTablesRelationSingle : BGMTFieldCached<BGMTEntity?, BGRowRef>
{
	private readonly List<BGId> relatedMetaIds = new List<BGId>();

	public List<BGId> RelatedMetaIds => relatedMetaIds;

	protected internal override BGMTEntity? this[int entityIndex]
	{
		get
		{
			BGRowRef storedValue = GetStoredValue(entityIndex);
			if (storedValue == null)
			{
				return null;
			}
			return base.Meta.Repo[storedValue.MetaId]?[storedValue.EntityId];
		}
		set
		{
			if (!value.HasValue)
			{
				SetStoredValue(entityIndex, null);
				return;
			}
			BGMTMeta bGMTMeta = base.Meta.Repo[value.Value.Meta.Id];
			if (bGMTMeta == null)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < relatedMetaIds.Count; i++)
			{
				if (!(relatedMetaIds[i] != bGMTMeta.Id))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				throw new BGException("Can not set value: Entity's meta does not match related metas. meta with id=$ and name=$ is not allowed", value.Value.Meta.Id, value.Value.Meta.Name);
			}
			BGId entityId = value.Value.Meta.GetEntityId(value.Value.Index);
			SetStoredValue(entityIndex, new BGRowRef(value.Value.Meta.Id, entityId));
		}
	}

	internal BGMTFieldManyTablesRelationSingle(BGField field)
		: base(field)
	{
		BGFieldManyRelationsSingle bGFieldManyRelationsSingle = (BGFieldManyRelationsSingle)field;
		relatedMetaIds.AddRange(bGFieldManyRelationsSingle.ToIds);
	}

	internal BGMTFieldManyTablesRelationSingle(BGMTMeta meta, BGMTFieldManyTablesRelationSingle otherField)
		: base(meta, (BGMTFieldCached<BGMTEntity?, BGRowRef>)otherField)
	{
		relatedMetaIds.AddRange(otherField.RelatedMetaIds);
	}

	internal override BGMTField DeepClone(BGMTMeta meta)
	{
		return new BGMTFieldManyTablesRelationSingle(meta, this);
	}

	public override void CopyTo(BGField field, BGEntity entity, BGMTEntity fromEntity)
	{
		BGFieldManyRelationsSingle bGFieldManyRelationsSingle = (BGFieldManyRelationsSingle)field;
		bGFieldManyRelationsSingle.SetStoredValue(entity.Index, GetStoredValue(fromEntity.Index));
	}
}

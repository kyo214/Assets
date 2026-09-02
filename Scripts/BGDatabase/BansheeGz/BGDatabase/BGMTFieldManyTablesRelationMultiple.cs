using System.Collections.Generic;
using System.Linq;

namespace BansheeGz.BGDatabase;

public class BGMTFieldManyTablesRelationMultiple : BGMTFieldCached<List<BGMTEntity>, List<BGRowRef>>
{
	private readonly List<BGId> relatedMetaIds = new List<BGId>();

	private bool allowDuplicates;

	public List<BGId> RelatedMetaIds => relatedMetaIds;

	protected internal override List<BGMTEntity> this[int entityIndex]
	{
		get
		{
			List<BGRowRef> storedValue = GetStoredValue(entityIndex);
			if (storedValue == null || storedValue.Count == 0)
			{
				return null;
			}
			List<BGMTEntity> list = null;
			for (int i = 0; i < storedValue.Count; i++)
			{
				BGRowRef bGRowRef = storedValue[i];
				BGMTEntity? bGMTEntity = base.Meta.Repo[bGRowRef.MetaId]?[bGRowRef.EntityId];
				if (bGMTEntity.HasValue)
				{
					list = list ?? new List<BGMTEntity>();
					list.Add(bGMTEntity.Value);
				}
			}
			return list;
		}
		set
		{
			if (value == null || value.Count == 0)
			{
				SetStoredValue(entityIndex, null);
				return;
			}
			List<BGRowRef> list = new List<BGRowRef>();
			for (int i = 0; i < value.Count; i++)
			{
				BGMTEntity bGMTEntity = value[i];
				bool flag = false;
				for (int j = 0; j < relatedMetaIds.Count; j++)
				{
					if (!(relatedMetaIds[j] != bGMTEntity.Meta.Id))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					throw new BGException("Can not set value: Entity's meta does not match related metas. meta with id=$ and name=$ is not allowed", bGMTEntity.Meta.Id, bGMTEntity.Meta.Name);
				}
				list.Add(new BGRowRef(bGMTEntity.Meta.Id, bGMTEntity.Id));
			}
			if (!allowDuplicates)
			{
				list = list.Distinct().ToList();
			}
			SetStoredValue(entityIndex, list);
		}
	}

	internal BGMTFieldManyTablesRelationMultiple(BGField field)
		: base(field)
	{
		BGFieldManyRelationsMultiple bGFieldManyRelationsMultiple = (BGFieldManyRelationsMultiple)field;
		relatedMetaIds.AddRange(bGFieldManyRelationsMultiple.ToIds);
		allowDuplicates = bGFieldManyRelationsMultiple.AllowDuplicates;
	}

	internal BGMTFieldManyTablesRelationMultiple(BGMTMeta meta, BGMTFieldManyTablesRelationMultiple otherField)
		: base(meta, (BGMTFieldCached<List<BGMTEntity>, List<BGRowRef>>)otherField)
	{
		relatedMetaIds.AddRange(otherField.RelatedMetaIds);
		allowDuplicates = otherField.allowDuplicates;
	}

	internal override BGMTField DeepClone(BGMTMeta meta)
	{
		return new BGMTFieldManyTablesRelationMultiple(meta, this);
	}

	public override void CopyTo(BGField field, BGEntity entity, BGMTEntity fromEntity)
	{
		BGFieldManyRelationsMultiple bGFieldManyRelationsMultiple = (BGFieldManyRelationsMultiple)field;
		List<BGRowRef> storedValue = GetStoredValue(fromEntity.Index);
		bGFieldManyRelationsMultiple.SetStoredValue(value: (storedValue != null && storedValue.Count != 0) ? new List<BGRowRef>(storedValue) : null, entityIndex: entity.Index);
	}
}

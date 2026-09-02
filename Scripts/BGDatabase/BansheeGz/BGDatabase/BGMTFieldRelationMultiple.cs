using System.Collections.Generic;
using System.Linq;

namespace BansheeGz.BGDatabase;

public class BGMTFieldRelationMultiple : BGMTFieldCached<List<BGMTEntity>, List<BGId>>
{
	private readonly BGId relatedMetaId;

	private bool allowDuplicates;

	public BGId RelatedMetaId => relatedMetaId;

	private BGMTMeta RelatedMeta
	{
		get
		{
			BGMTMeta bGMTMeta = base.Meta.Repo[relatedMetaId];
			if (bGMTMeta == null)
			{
				throw new BGException("Can not find related meta with id '$'", relatedMetaId);
			}
			return bGMTMeta;
		}
	}

	protected internal override List<BGMTEntity> this[int entityIndex]
	{
		get
		{
			List<BGId> storedValue = GetStoredValue(entityIndex);
			if (storedValue == null || storedValue.Count == 0)
			{
				return null;
			}
			BGMTMeta relatedMeta = RelatedMeta;
			if (relatedMeta == null)
			{
				return null;
			}
			List<BGMTEntity> list = null;
			for (int i = 0; i < storedValue.Count; i++)
			{
				BGId entityId = storedValue[i];
				BGMTEntity? bGMTEntity = relatedMeta[entityId];
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
			BGMTMeta relatedMeta = RelatedMeta;
			if (relatedMeta == null)
			{
				SetStoredValue(entityIndex, null);
				return;
			}
			List<BGId> list = new List<BGId>();
			for (int i = 0; i < value.Count; i++)
			{
				BGMTEntity bGMTEntity = value[i];
				if (relatedMeta.Id != bGMTEntity.Meta.Id)
				{
					throw new BGException("Can not set value: Entity's meta does not match related meta. expected: $, found $", relatedMeta.Name, bGMTEntity.Meta.Name);
				}
				list.Add(bGMTEntity.Id);
			}
			if (!allowDuplicates)
			{
				list = list.Distinct().ToList();
			}
			SetStoredValue(entityIndex, list);
		}
	}

	internal BGMTFieldRelationMultiple(BGField field)
		: base(field)
	{
		BGFieldRelationMultiple bGFieldRelationMultiple = (BGFieldRelationMultiple)field;
		relatedMetaId = bGFieldRelationMultiple.RelatedMeta.Id;
		allowDuplicates = bGFieldRelationMultiple.AllowDuplicates;
	}

	internal BGMTFieldRelationMultiple(BGMTMeta meta, BGMTFieldRelationMultiple otherField)
		: base(meta, (BGMTFieldCached<List<BGMTEntity>, List<BGId>>)otherField)
	{
		relatedMetaId = otherField.relatedMetaId;
		allowDuplicates = otherField.allowDuplicates;
	}

	internal override BGMTField DeepClone(BGMTMeta meta)
	{
		return new BGMTFieldRelationMultiple(meta, this);
	}

	public override void CopyTo(BGField field, BGEntity entity, BGMTEntity fromEntity)
	{
		BGFieldRelationMultiple bGFieldRelationMultiple = (BGFieldRelationMultiple)field;
		List<BGId> storedValue = GetStoredValue(fromEntity.Index);
		bGFieldRelationMultiple.SetStoredValue(value: (storedValue != null && storedValue.Count != 0) ? new List<BGId>(storedValue) : null, entityIndex: entity.Index);
	}
}

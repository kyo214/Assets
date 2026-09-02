using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGMTFieldNested : BGMTField<List<BGMTEntity>>
{
	private readonly BGId relatedMetaId;

	private readonly BGId relationId;

	public BGId RelatedMetaId => relatedMetaId;

	public BGId RelationId => relationId;

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
			List<BGMTEntity> list = null;
			BGMTMeta relatedMeta = RelatedMeta;
			if (!(relatedMeta.GetField(relationId) is BGMTFieldRelationSingle bGMTFieldRelationSingle))
			{
				throw new BGException("Can not find relation field with id '$' at meta '$'", relationId, relatedMeta.Name);
			}
			BGId entityId = base.Meta.GetEntityId(entityIndex);
			if (entityId.IsEmpty)
			{
				return list;
			}
			int countEntities = relatedMeta.CountEntities;
			for (int i = 0; i < countEntities; i++)
			{
				BGId storedValue = bGMTFieldRelationSingle.GetStoredValue(i);
				if (!(storedValue != entityId))
				{
					list = list ?? new List<BGMTEntity>();
					list.Add(new BGMTEntity(relatedMeta, i));
				}
			}
			return list;
		}
		set
		{
		}
	}

	internal BGMTFieldNested(BGField field)
		: base(field.Id, field.Name)
	{
		BGFieldNested bGFieldNested = (BGFieldNested)field;
		relatedMetaId = bGFieldNested.NestedMeta.Id;
		relationId = bGFieldNested.OwnerRelationId;
	}

	internal BGMTFieldNested(BGMTMeta meta, BGMTField<List<BGMTEntity>> otherField)
		: base(meta, otherField)
	{
	}

	internal override BGMTField DeepClone(BGMTMeta meta)
	{
		return new BGMTFieldNested(meta, this);
	}

	internal override void ResizeTo(int newCount)
	{
	}

	internal override void RemoveRange(int from, int count)
	{
	}

	public override void CopyTo(BGField field, BGEntity entity, BGMTEntity fromEntity)
	{
	}
}

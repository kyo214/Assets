namespace BansheeGz.BGDatabase;

public class BGMTFieldRelationSingle : BGMTFieldCached<BGMTEntity?, BGId>
{
	private readonly BGId relatedMetaId;

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

	protected internal override BGMTEntity? this[int entityIndex]
	{
		get
		{
			BGId storedValue = GetStoredValue(entityIndex);
			if (storedValue.IsEmpty)
			{
				return null;
			}
			return RelatedMeta?[storedValue];
		}
		set
		{
			if (!value.HasValue)
			{
				SetStoredValue(entityIndex, BGId.Empty);
				return;
			}
			BGMTMeta relatedMeta = RelatedMeta;
			if (relatedMeta != null)
			{
				if (relatedMeta.Id != value.Value.Meta.Id)
				{
					throw new BGException("Can not set value: Entity's meta does not match related meta. expected: $, found $", relatedMeta.Name, value.Value.Meta.Name);
				}
				BGId entityId = value.Value.Meta.GetEntityId(value.Value.Index);
				SetStoredValue(entityIndex, entityId);
			}
		}
	}

	internal BGMTFieldRelationSingle(BGField field)
		: base(field)
	{
		BGFieldRelationSingle bGFieldRelationSingle = (BGFieldRelationSingle)field;
		relatedMetaId = bGFieldRelationSingle.RelatedMeta.Id;
	}

	internal BGMTFieldRelationSingle(BGMTMeta meta, BGMTFieldRelationSingle otherField)
		: base(meta, (BGMTFieldCached<BGMTEntity?, BGId>)otherField)
	{
		relatedMetaId = otherField.relatedMetaId;
	}

	internal override BGMTField DeepClone(BGMTMeta meta)
	{
		return new BGMTFieldRelationSingle(meta, this);
	}

	public override void CopyTo(BGField field, BGEntity entity, BGMTEntity fromEntity)
	{
		BGFieldRelationSingle bGFieldRelationSingle = (BGFieldRelationSingle)field;
		bGFieldRelationSingle.SetStoredValue(entity.Index, GetStoredValue(fromEntity.Index));
	}
}

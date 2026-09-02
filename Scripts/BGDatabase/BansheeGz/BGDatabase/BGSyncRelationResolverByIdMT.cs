using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public abstract class BGSyncRelationResolverByIdMT : BGSyncRelationResolver
{
	protected readonly BGField relation;

	protected readonly BGRepo backUpRepo;

	protected bool someMetaIsMissing;

	protected BGSyncRelationResolverByIdMT(BGField relation, BGRepo backUpRepo)
	{
		this.relation = relation;
		this.backUpRepo = backUpRepo;
		BGManyTablesRelationI bGManyTablesRelationI = (BGManyTablesRelationI)relation;
		List<BGId> toIds = bGManyTablesRelationI.ToIds;
		someMetaIsMissing = toIds.Count != bGManyTablesRelationI.RelatedMetas.Count;
	}

	public void ToDatabase(int index, string value)
	{
		BGUtil.FromString(relation, index, value);
	}

	public string ToExternalFormat(int index)
	{
		if (!someMetaIsMissing)
		{
			return BGUtil.ToString(relation, index);
		}
		if (relation.CustomStringFormatSupported)
		{
			return relation.ToCustomString(index);
		}
		string value = relation.ToString(index);
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		return ToExternalFormatInternal(value);
	}

	protected abstract string ToExternalFormatInternal(string value);
}

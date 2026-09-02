namespace BansheeGz.BGDatabase;

public abstract class BGSyncRelationResolverByIdST : BGSyncRelationResolver
{
	protected readonly BGField relation;

	protected readonly BGMetaEntity backUpMeta;

	protected BGSyncRelationResolverByIdST(BGField relation, BGRepo backUpRepo)
	{
		this.relation = relation;
		BGRelationI bGRelationI = (BGRelationI)relation;
		if (bGRelationI.RelatedMeta == null && backUpRepo != null)
		{
			backUpMeta = backUpRepo.GetMeta(bGRelationI.ToId);
		}
	}

	public void ToDatabase(int index, string value)
	{
		BGUtil.FromString(relation, index, value);
	}

	public string ToExternalFormat(int index)
	{
		if (backUpMeta == null)
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

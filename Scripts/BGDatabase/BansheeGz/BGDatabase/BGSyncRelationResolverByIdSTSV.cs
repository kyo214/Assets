namespace BansheeGz.BGDatabase;

public class BGSyncRelationResolverByIdSTSV : BGSyncRelationResolverByIdST
{
	public BGSyncRelationResolverByIdSTSV(BGField relation, BGRepo backUpRepo)
		: base(relation, backUpRepo)
	{
	}

	protected override string ToExternalFormatInternal(string value)
	{
		if (value.Length != 22)
		{
			return value;
		}
		BGId entityId = new BGId(value);
		return BGFieldRelationSingle.IdToString(entityId, backUpMeta[entityId]);
	}
}

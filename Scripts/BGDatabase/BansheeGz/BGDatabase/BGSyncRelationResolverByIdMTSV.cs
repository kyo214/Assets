namespace BansheeGz.BGDatabase;

public class BGSyncRelationResolverByIdMTSV : BGSyncRelationResolverByIdMT
{
	public BGSyncRelationResolverByIdMTSV(BGFieldManyRelationsSingle relation, BGRepo backUpRepo)
		: base(relation, backUpRepo)
	{
	}

	protected override string ToExternalFormatInternal(string value)
	{
		BGRowRef bGRowRef = BGFieldRelationMA<BGEntity, BGRowRef>.StringToRowRef(value);
		if (bGRowRef == null)
		{
			return value;
		}
		return BGFieldRelationMA<BGEntity, BGRowRef>.RowRefToString(bGRowRef, backUpRepo);
	}
}

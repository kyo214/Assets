namespace BansheeGz.BGDatabase;

public class BGSyncRelationResolverFieldSTSV : BGSyncRelationResolverFieldST
{
	public BGSyncRelationResolverFieldSTSV(BGSyncRowResolver rowResolver, BGField relation, BGRepo backupRepo)
		: base(rowResolver, relation, backupRepo)
	{
	}

	protected override void ToDatabaseInternal(int index, string value)
	{
		BGRowRef bGRowRef = rowResolver.FromString(value);
		if (bGRowRef == null)
		{
			throw new BGException("Can not resolve referenced entity, using $ as a reference and [$] row resolver.", value, rowResolver);
		}
		string value2 = BGFieldRelationSingle.IdToString(bGRowRef.EntityId, null);
		relation.FromString(index, value2);
	}

	protected override string ToExternalFormatInternal(string dbValue)
	{
		BGId bGId = BGFieldRelationSA<BGEntity, BGId>.IdFromString(dbValue);
		string text = rowResolver.ToString(bGId);
		if (string.IsNullOrEmpty(text))
		{
			throw new BGException("Entity ID value is empty, entity id $ , entity resolver=[$]", bGId, rowResolver);
		}
		return text;
	}
}

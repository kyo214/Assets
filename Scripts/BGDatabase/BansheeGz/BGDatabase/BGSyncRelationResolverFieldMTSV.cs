using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGSyncRelationResolverFieldMTSV : BGSyncRelationResolverFieldMT
{
	private BGFieldManyRelationsSingle relationField => (BGFieldManyRelationsSingle)relation;

	public BGSyncRelationResolverFieldMTSV(List<BGSyncRowResolver> rowResolvers, BGFieldManyRelationsSingle relation, BGRepo backupRepo)
		: base(rowResolvers, relation, backupRepo)
	{
	}

	protected override void ToDatabaseInternal(int index, string value)
	{
		BGRowRef rowRef = Resolve(value);
		relation.FromString(index, BGFieldRelationMA<BGEntity, BGRowRef>.RowRefToString(rowRef));
	}

	public override string ToExternalFormat(int index)
	{
		BGRowRef storedValue = relationField.GetStoredValue(index);
		if (storedValue == null)
		{
			return null;
		}
		return Resolve(storedValue);
	}
}

using System;
using System.Text;

namespace BansheeGz.BGDatabase;

public class BGSyncRelationResolverByIdSTMV : BGSyncRelationResolverByIdST
{
	private static readonly StringBuilder builder = new StringBuilder();

	public BGSyncRelationResolverByIdSTMV(BGFieldRelationMultiple relation, BGRepo backUpRepo)
		: base(relation, backUpRepo)
	{
	}

	protected override string ToExternalFormatInternal(string value)
	{
		string[] array = value.Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length == 0)
		{
			return value;
		}
		try
		{
			builder.Length = 0;
			string[] array2 = array;
			foreach (string value2 in array2)
			{
				BGId entityId = BGFieldRelationSA<BGEntity, BGId>.IdFromString(value2);
				if (entityId.IsEmpty)
				{
					return value;
				}
				if (builder.Length != 0)
				{
					builder.Append('|');
				}
				builder.Append(BGFieldRelationMultiple.IdToString(entityId, backUpMeta[entityId]));
			}
			return builder.ToString();
		}
		catch
		{
			return value;
		}
		finally
		{
			builder.Length = 0;
		}
	}
}

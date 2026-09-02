using System;
using System.Text;

namespace BansheeGz.BGDatabase;

public class BGSyncRelationResolverByIdMTMV : BGSyncRelationResolverByIdMT
{
	private static readonly StringBuilder builder = new StringBuilder();

	public BGSyncRelationResolverByIdMTMV(BGFieldManyRelationsMultiple relation, BGRepo backUpRepo)
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
				BGRowRef bGRowRef = BGFieldRelationMA<BGEntity, BGRowRef>.StringToRowRef(value2);
				if (bGRowRef == null)
				{
					return value;
				}
				if (builder.Length != 0)
				{
					builder.Append('|');
				}
				builder.Append(BGFieldRelationMA<BGEntity, BGRowRef>.RowRefToString(bGRowRef, backUpRepo).Replace("|", ""));
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

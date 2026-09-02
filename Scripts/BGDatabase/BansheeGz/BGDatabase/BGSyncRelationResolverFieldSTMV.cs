using System;
using System.Text;

namespace BansheeGz.BGDatabase;

public class BGSyncRelationResolverFieldSTMV : BGSyncRelationResolverFieldST
{
	private static readonly StringBuilder builder = new StringBuilder();

	public BGSyncRelationResolverFieldSTMV(BGSyncRowResolver rowResolver, BGFieldRelationMultiple relation, BGRepo backupRepo)
		: base(rowResolver, relation, backupRepo)
	{
	}

	protected override void ToDatabaseInternal(int index, string value)
	{
		string[] array = value.Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length == 0)
		{
			return;
		}
		builder.Length = 0;
		try
		{
			string[] array2 = array;
			foreach (string text in array2)
			{
				BGRowRef bGRowRef = rowResolver.FromString(text);
				if (bGRowRef == null)
				{
					throw new BGException("Can not resolve referenced entity, using $ as a reference and [$] row resolver.", text, rowResolver);
				}
				if (builder.Length != 0)
				{
					builder.Append('|');
				}
				builder.Append(bGRowRef.EntityId);
			}
			relation.FromString(index, builder.ToString());
		}
		finally
		{
			builder.Length = 0;
		}
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
			foreach (string text in array2)
			{
				BGId bGId = BGFieldRelationSA<BGEntity, BGId>.IdFromString(text);
				if (bGId.IsEmpty)
				{
					throw new BGException("Can not convert string value=[$] to row ID", text);
				}
				string value2 = rowResolver.ToString(bGId);
				if (string.IsNullOrEmpty(value2))
				{
					throw new BGException("Entity ID value is empty, entity id $ , entity resolver=[$]", bGId, rowResolver);
				}
				if (builder.Length != 0)
				{
					builder.Append('|');
				}
				builder.Append(value2);
			}
			return builder.ToString();
		}
		finally
		{
			builder.Length = 0;
		}
	}
}

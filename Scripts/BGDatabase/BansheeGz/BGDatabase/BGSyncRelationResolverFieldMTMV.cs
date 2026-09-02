using System;
using System.Collections.Generic;
using System.Text;

namespace BansheeGz.BGDatabase;

public class BGSyncRelationResolverFieldMTMV : BGSyncRelationResolverFieldMT
{
	private static readonly StringBuilder builder = new StringBuilder();

	private BGFieldManyRelationsMultiple relationField => (BGFieldManyRelationsMultiple)relation;

	public BGSyncRelationResolverFieldMTMV(List<BGSyncRowResolver> rowResolvers, BGFieldManyRelationsMultiple relation, BGRepo backupRepo)
		: base(rowResolvers, relation, backupRepo)
	{
	}

	protected override void ToDatabaseInternal(int index, string value)
	{
		string[] array = value.Split(new char[1] { '|' }, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length != 0)
		{
			List<BGRowRef> list = new List<BGRowRef>();
			string[] array2 = array;
			foreach (string idFieldString in array2)
			{
				BGRowRef item = Resolve(idFieldString);
				list.Add(item);
			}
			relationField.SetStoredValue(index, list);
		}
	}

	public override string ToExternalFormat(int index)
	{
		List<BGRowRef> storedValue = relationField.GetStoredValue(index);
		if (storedValue == null || storedValue.Count == 0)
		{
			return null;
		}
		try
		{
			builder.Length = 0;
			foreach (BGRowRef item in storedValue)
			{
				string text = Resolve(item);
				if (text != null)
				{
					if (builder.Length != 0)
					{
						builder.Append('|');
					}
					builder.Append(text);
				}
			}
			return builder.ToString();
		}
		finally
		{
			builder.Length = 0;
		}
	}
}

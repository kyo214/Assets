using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public abstract class BGSyncRelationResolverFieldMT : BGSyncRelationResolver
{
	protected readonly Dictionary<BGId, BGSyncRowResolver> metaId2rowResolver = new Dictionary<BGId, BGSyncRowResolver>();

	protected readonly Dictionary<string, BGSyncRowResolver> metaName2rowResolver = new Dictionary<string, BGSyncRowResolver>();

	protected readonly BGField relation;

	protected readonly BGRepo backupRepo;

	public BGSyncRelationResolverFieldMT(List<BGSyncRowResolver> rowResolvers, BGField relation, BGRepo backupRepo)
	{
		foreach (BGSyncRowResolver rowResolver in rowResolvers)
		{
			metaId2rowResolver[rowResolver.MetaId] = rowResolver;
			metaName2rowResolver[rowResolver.MetaName] = rowResolver;
		}
		this.relation = relation;
		this.backupRepo = backupRepo;
	}

	public void ToDatabase(int index, string value)
	{
		if (!string.IsNullOrEmpty(value))
		{
			value = value.Trim();
			ToDatabaseInternal(index, value);
		}
	}

	protected abstract void ToDatabaseInternal(int index, string value);

	public abstract string ToExternalFormat(int index);

	protected string Resolve(BGRowRef value)
	{
		if (!metaId2rowResolver.TryGetValue(value.MetaId, out var value2))
		{
			throw new BGException("Can not find a resolver for $ field, using $ meta ID", relation.FullName, value.MetaId);
		}
		string text = value2.ToString(value.EntityId);
		if (string.IsNullOrEmpty(text))
		{
			throw new BGException("Entity ID value is empty, entity id $ , entity resolver=[$]", value.EntityId, value2);
		}
		return value2.MetaName + "." + text;
	}

	protected BGRowRef Resolve(string idFieldString)
	{
		int num = idFieldString.IndexOf('.');
		if (num > 0 && num < idFieldString.Length - 1)
		{
			string key = idFieldString.Substring(0, num);
			if (metaName2rowResolver.TryGetValue(key, out var value))
			{
				string value2 = idFieldString.Substring(num + 1, idFieldString.Length - num - 1);
				BGRowRef bGRowRef = value.FromString(value2);
				if (bGRowRef == null)
				{
					throw new BGException("Can not resolve entity for $ field, using $ value", relation.FullName, idFieldString);
				}
				return bGRowRef;
			}
		}
		foreach (KeyValuePair<string, BGSyncRowResolver> item in metaName2rowResolver)
		{
			BGSyncRowResolver value3 = item.Value;
			BGRowRef bGRowRef2 = value3.FromString(idFieldString);
			if (bGRowRef2 != null)
			{
				return bGRowRef2;
			}
		}
		throw new BGException("Can not resolve entity for $ field, using $ value", relation.FullName, idFieldString);
	}
}

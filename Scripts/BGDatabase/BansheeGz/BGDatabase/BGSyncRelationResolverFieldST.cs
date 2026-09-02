namespace BansheeGz.BGDatabase;

public abstract class BGSyncRelationResolverFieldST : BGSyncRelationResolver
{
	protected readonly BGSyncRowResolver rowResolver;

	protected readonly BGField relation;

	protected readonly BGRepo backupRepo;

	protected BGSyncRelationResolverFieldST(BGSyncRowResolver rowResolver, BGField relation, BGRepo backupRepo)
	{
		this.rowResolver = rowResolver;
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

	public string ToExternalFormat(int index)
	{
		string text = relation.ToString(index);
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		return ToExternalFormatInternal(text);
	}

	protected abstract string ToExternalFormatInternal(string dbValue);
}

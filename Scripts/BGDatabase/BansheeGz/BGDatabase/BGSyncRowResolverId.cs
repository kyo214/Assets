namespace BansheeGz.BGDatabase;

public class BGSyncRowResolverId : BGSyncRowResolver
{
	private readonly string metaName;

	private readonly BGId metaId;

	public BGId MetaId => metaId;

	public string MetaName => metaName;

	public BGSyncRowResolverId(BGId metaId, string metaName)
	{
		this.metaId = metaId;
		this.metaName = metaName;
	}

	public BGRowRef FromString(string value)
	{
		if (BGId.TryParse(value, out var id))
		{
			return new BGRowRef(metaId, id);
		}
		return null;
	}

	public string ToString(BGId rowId)
	{
		if (!rowId.IsEmpty)
		{
			return rowId.ToString();
		}
		return null;
	}

	public override string ToString()
	{
		return "Resolver by id, table=" + metaName;
	}
}

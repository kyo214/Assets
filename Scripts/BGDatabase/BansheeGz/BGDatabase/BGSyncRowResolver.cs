namespace BansheeGz.BGDatabase;

public interface BGSyncRowResolver
{
	BGId MetaId { get; }

	string MetaName { get; }

	BGRowRef FromString(string value);

	string ToString(BGId rowId);
}

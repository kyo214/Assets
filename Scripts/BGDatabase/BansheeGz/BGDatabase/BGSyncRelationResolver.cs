namespace BansheeGz.BGDatabase;

public interface BGSyncRelationResolver
{
	void ToDatabase(int index, string value);

	string ToExternalFormat(int index);
}

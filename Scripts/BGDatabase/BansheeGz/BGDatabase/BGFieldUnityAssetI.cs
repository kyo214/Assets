namespace BansheeGz.BGDatabase;

public interface BGFieldUnityAssetI
{
	string GetAssetPath(int entityIndex);

	void SetAssetPath(int entityIndex, string path);
}

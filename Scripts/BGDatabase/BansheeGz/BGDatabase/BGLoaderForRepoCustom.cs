using System.IO;

namespace BansheeGz.BGDatabase;

public class BGLoaderForRepoCustom : BGLoaderForRepo
{
	public const string LoaderName = "Custom";

	public const string CustomDatabaseGuid = "3637ea689da0cff4b8d5c0fb5d609c15";

	public override string Name => "Custom";

	public override byte[] Load(LoadRequest request)
	{
		if (BGRepo.DefaultRepoCustomLoaderModel == null)
		{
			return null;
		}
		byte[] array = null;
		if (IsEmpty(request))
		{
			BGRepoCustomLoaderModel.DatabaseResource databaseResource = request.databaseResource;
			if (databaseResource != null)
			{
				BGRepo.DefaultRepoAssetId = ((databaseResource.AssetId > 0) ? databaseResource.AssetId : 0);
				BGRepo.DefaultRepoAssetPath = ((!string.IsNullOrEmpty(databaseResource.AssetPath)) ? databaseResource.AssetPath : null) ?? "bansheegz_database";
			}
			else
			{
				BGRepo.DefaultRepoAssetId = 0;
				BGRepo.DefaultRepoAssetPath = null;
			}
			return BGRepo.DefaultRepoCustomLoaderModel.MainDatabaseResource.Content;
		}
		string key = ToPath(request);
		return BGRepo.DefaultRepoCustomLoaderModel.Get(key)?.Content;
	}

	protected override string ToPath(LoadRequest request)
	{
		string fileName = Path.GetFileName(request.basePath);
		string basePath = Path.ChangeExtension(fileName, null);
		return BGLoaderForRepo.AppendPaths(basePath, request.paths);
	}
}

using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGLoaderForRepoResources : BGLoaderForRepo
{
	public const string LoaderName = "Resources";

	private static string[] pathes = new string[7] { "bansheegz_database", "bansheegz_database_default5", "bansheegz_database_default4", "bansheegz_database_default3", "bansheegz_database_default2", "bansheegz_database_default1", "bansheegz_database_default" };

	public static string[] Pathes => pathes;

	public override string Name => "Resources";

	public override byte[] Load(LoadRequest request)
	{
		TextAsset textAsset = null;
		if (IsEmpty(request))
		{
			for (int i = 0; i < pathes.Length; i++)
			{
				string text = pathes[i];
				textAsset = Load(text);
				if (!(textAsset == null))
				{
					BGRepo.DefaultRepoAssetId = textAsset.GetInstanceID();
					BGRepo.DefaultRepoAssetPath = text;
					break;
				}
			}
		}
		else
		{
			string text2 = ToPath(request);
			TextAsset[] array = Resources.LoadAll<TextAsset>(text2);
			byte[] array2 = null;
			if (array != null && array.Length != 0)
			{
				if (array.Length > 1)
				{
					throw new BGException("You have more than one file with name $ under Resources folder! Please, backup all these files and leave the only right one under Resources folder!", text2);
				}
				textAsset = array[0];
			}
		}
		if (!(textAsset == null))
		{
			return textAsset.bytes;
		}
		return null;
	}

	protected override string ToPath(LoadRequest request)
	{
		if (IsEmpty(request))
		{
			return pathes[0];
		}
		return BGLoaderForRepo.AppendPaths(request.basePath, request.paths);
	}

	public TextAsset Load(string path)
	{
		return Resources.Load<TextAsset>(path);
	}
}

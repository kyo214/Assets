using UnityEngine;

namespace BansheeGz.BGDatabase;

[AssetLoaderDescriptor(Name = "Resources", ManagerType = "BansheeGz.BGDatabase.Editor.BGAssetLoaderManagerResources")]
public class BGAssetLoaderResources : BGAssetLoaderA
{
	public override string Name => "Resources";

	public override T Load<T>(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return null;
		}
		return Resources.Load<T>(path);
	}

	public override T[] LoadAll<T>(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return null;
		}
		return Resources.LoadAll<T>(path);
	}
}

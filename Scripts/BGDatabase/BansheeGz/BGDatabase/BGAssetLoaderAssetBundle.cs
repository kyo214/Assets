using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[AssetLoaderDescriptor(Name = "AssetBundle", ManagerType = "BansheeGz.BGDatabase.Editor.BGAssetLoaderManagerAssetBundle")]
public class BGAssetLoaderAssetBundle : BGAssetLoaderA
{
	[Serializable]
	private struct JsonConfig
	{
		public string AssetBundle;
	}

	private string assetBundle;

	public string AssetBundle
	{
		get
		{
			return assetBundle;
		}
		set
		{
			assetBundle = value;
		}
	}

	public override string Name => "AssetBundle[" + assetBundle + "]";

	private AssetBundle TargetBundle
	{
		get
		{
			IEnumerable<AssetBundle> allLoadedAssetBundles = UnityEngine.AssetBundle.GetAllLoadedAssetBundles();
			if (allLoadedAssetBundles == null)
			{
				return null;
			}
			AssetBundle result = null;
			foreach (AssetBundle item in allLoadedAssetBundles)
			{
				if (string.Equals(item.name, assetBundle))
				{
					result = item;
					break;
				}
			}
			return result;
		}
	}

	public override T Load<T>(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return null;
		}
		AssetBundle targetBundle = TargetBundle;
		if (!(targetBundle == null))
		{
			return targetBundle.LoadAsset<T>(path);
		}
		return null;
	}

	public override T[] LoadAll<T>(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return null;
		}
		AssetBundle targetBundle = TargetBundle;
		if (!(targetBundle == null))
		{
			return targetBundle.LoadAssetWithSubAssets<T>(path);
		}
		return null;
	}

	public override string ConfigToString()
	{
		return JsonUtility.ToJson(new JsonConfig
		{
			AssetBundle = assetBundle
		});
	}

	public override void ConfigFromString(string config)
	{
		assetBundle = JsonUtility.FromJson<JsonConfig>(config).AssetBundle;
	}

	public override byte[] ConfigToBytes()
	{
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter(4 + BGBinaryWriter.GetBytesCount(assetBundle));
		bGBinaryWriter.AddInt(1);
		bGBinaryWriter.AddString(assetBundle);
		return bGBinaryWriter.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		BGBinaryReader bGBinaryReader = new BGBinaryReader(config);
		int num = bGBinaryReader.ReadInt();
		if (num == 1)
		{
			assetBundle = bGBinaryReader.ReadString();
			return;
		}
		throw new BGException("Unknown version: $", num);
	}
}

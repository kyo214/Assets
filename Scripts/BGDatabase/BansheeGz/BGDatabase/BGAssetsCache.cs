using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public abstract class BGAssetsCache
{
	private class BGAssetsCacheDefault : BGAssetsCache
	{
		private readonly Dictionary<string, Object> key2Asset;

		private readonly Dictionary<string, Object[]> key2AssetAll;

		public BGAssetsCacheDefault()
		{
			key2Asset = new Dictionary<string, Object>();
			key2AssetAll = new Dictionary<string, Object[]>();
		}

		public BGAssetsCacheDefault(int capacity)
		{
			key2Asset = new Dictionary<string, Object>(capacity);
			key2AssetAll = new Dictionary<string, Object[]>();
		}

		protected override bool TryToGetAsset(string key, out Object asset)
		{
			asset = null;
			if (key == null)
			{
				return false;
			}
			if (!key2Asset.TryGetValue(key, out asset))
			{
				return false;
			}
			if (asset == null)
			{
				key2Asset.Remove(key);
				return false;
			}
			return true;
		}

		protected override bool TryToGetAssetAll(string key, out Object[] assets)
		{
			assets = null;
			if (key == null)
			{
				return false;
			}
			if (!key2AssetAll.TryGetValue(key, out assets))
			{
				return false;
			}
			if (assets == null)
			{
				key2Asset.Remove(key);
				return false;
			}
			return true;
		}

		protected override bool AddAsset(string key, Object asset)
		{
			if (key == null || asset == null)
			{
				return false;
			}
			key2Asset[key] = asset;
			return true;
		}

		protected override bool AddAssetAll(string key, Object[] assets)
		{
			if (key == null || assets == null || assets.Length == 0)
			{
				return false;
			}
			key2AssetAll[key] = assets;
			return true;
		}

		protected override void ClearAssets()
		{
			key2Asset.Clear();
			key2AssetAll.Clear();
		}
	}

	private static BGAssetsCache Instance;

	public static bool Enabled
	{
		get
		{
			return Instance != null;
		}
		set
		{
			if (value)
			{
				if (Instance == null)
				{
					Instance = new BGAssetsCacheDefault();
				}
				return;
			}
			if (Instance != null)
			{
				Clear();
			}
			Instance = null;
		}
	}

	public static void SetEnabled(int capacity)
	{
		Instance = new BGAssetsCacheDefault(capacity);
	}

	public static void SetInstance(BGAssetsCache cache)
	{
		Instance = cache;
	}

	public static bool TryToGet(string key, out Object asset)
	{
		return Instance.TryToGetAsset(key, out asset);
	}

	public static bool Add(string key, Object asset)
	{
		return Instance.AddAsset(key, asset);
	}

	public static bool TryToGetAll(string key, out Object[] assets)
	{
		return Instance.TryToGetAssetAll(key, out assets);
	}

	public static bool AddAll(string key, Object[] assets)
	{
		return Instance.AddAssetAll(key, assets);
	}

	public static void Clear()
	{
		Instance.ClearAssets();
	}

	protected abstract bool TryToGetAsset(string key, out Object asset);

	protected abstract bool TryToGetAssetAll(string key, out Object[] assets);

	protected abstract bool AddAsset(string key, Object asset);

	protected abstract bool AddAssetAll(string key, Object[] assets);

	protected abstract void ClearAssets();
}

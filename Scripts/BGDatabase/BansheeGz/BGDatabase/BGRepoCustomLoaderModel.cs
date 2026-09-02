using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGRepoCustomLoaderModel
{
	public class DatabaseResource
	{
		private string assetPath;

		private int assetId;

		private byte[] content;

		public virtual string AssetPath
		{
			get
			{
				return assetPath;
			}
			set
			{
				assetPath = value;
			}
		}

		public virtual int AssetId
		{
			get
			{
				return assetId;
			}
			set
			{
				assetId = value;
			}
		}

		public virtual byte[] Content
		{
			get
			{
				return content;
			}
			set
			{
				content = value;
			}
		}

		public DatabaseResource()
		{
		}

		public DatabaseResource(byte[] content)
		{
			this.content = content;
		}

		public DatabaseResource(string assetPath, int assetId, byte[] content)
		{
			this.assetPath = assetPath;
			this.assetId = assetId;
			this.content = content;
		}
	}

	public const string DatabaseKey = "bansheegz_database";

	private Dictionary<string, DatabaseResource> key2Resource;

	private DatabaseResource mainDatabaseResource;

	public DatabaseResource MainDatabaseResource => mainDatabaseResource;

	public BGRepoCustomLoaderModel(DatabaseResource mainDatabaseResource)
	{
		this.mainDatabaseResource = mainDatabaseResource ?? throw new BGException("mainDatabaseResource can not be null");
	}

	public DatabaseResource Get(string key)
	{
		if (key2Resource == null)
		{
			return null;
		}
		if (!key2Resource.TryGetValue(key, out var value))
		{
			return null;
		}
		return value;
	}

	public void Add(string key, DatabaseResource resource)
	{
		if (key2Resource == null)
		{
			key2Resource = new Dictionary<string, DatabaseResource>();
		}
		key2Resource[key] = resource;
	}

	public bool Remove(string key)
	{
		if (key2Resource == null)
		{
			return false;
		}
		return key2Resource.Remove(key);
	}

	public void ForEachKey(Action<string, DatabaseResource> action)
	{
		if (key2Resource == null || action == null)
		{
			return;
		}
		foreach (KeyValuePair<string, DatabaseResource> item in key2Resource)
		{
			action(item.Key, item.Value);
		}
	}

	public static bool IsDatabaseKey(string key)
	{
		return string.Equals(key, "bansheegz_database");
	}
}

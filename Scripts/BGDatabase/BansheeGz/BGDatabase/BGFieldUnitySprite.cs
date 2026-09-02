using System;
using UnityEngine;
using UnityEngine.U2D;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "unitySprite", Folder = "Unity Asset", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerUnitySprite")]
public class BGFieldUnitySprite : BGFieldUnityAssetA<Sprite>, BGAddressablesAssetCustomLoaderI
{
	public enum LocationEnum
	{
		Single = 0,
		Multiple = 1,
		SpriteAtlas = 2
	}

	[Flags]
	public enum LocationConstraintEnum
	{
		None = 0,
		Single = 1,
		Multiple = 2,
		SpriteAtlas = 4
	}

	[Serializable]
	private class SpriteConfig : JsonConfig
	{
		public int LocationConstraint;
	}

	public struct SpriteLocation
	{
		private readonly LocationEnum location;

		private readonly string assetPath;

		private readonly string subAssetPath;

		private readonly string fullPath;

		public LocationEnum Location => location;

		public string AssetPath => assetPath;

		public string SubAssetPath => subAssetPath;

		public string FullPath => fullPath;

		public SpriteLocation(LocationEnum location, string assetPath, string subAssetPath)
		{
			this.location = location;
			this.assetPath = assetPath;
			this.subAssetPath = subAssetPath;
			switch (location)
			{
			case LocationEnum.Single:
				fullPath = assetPath;
				break;
			case LocationEnum.Multiple:
			case LocationEnum.SpriteAtlas:
			{
				int num = (int)location;
				string text = num.ToString() ?? "";
				text += ":";
				text += assetPath;
				text += ":";
				text += subAssetPath;
				fullPath = text;
				break;
			}
			default:
				throw new ArgumentOutOfRangeException("location");
			}
		}

		public SpriteLocation(string path)
		{
			fullPath = path ?? throw new ArgumentException("path is null");
			location = LocationEnum.Single;
			assetPath = path;
			subAssetPath = null;
			if (path.Length <= 2 || path[1] != ':')
			{
				return;
			}
			char c = path[0];
			switch (c)
			{
			case '0':
				assetPath = assetPath.Substring(2);
				break;
			case '1':
			case '2':
			{
				int num = assetPath.LastIndexOf(':');
				if (num > 2 && num < assetPath.Length - 1)
				{
					subAssetPath = assetPath.Substring(num + 1);
					assetPath = assetPath.Substring(2, num - 2);
					location = ((c == '1') ? LocationEnum.Multiple : LocationEnum.SpriteAtlas);
				}
				break;
			}
			}
		}

		public override string ToString()
		{
			return fullPath;
		}
	}

	public const ushort CodeType = 55;

	private const char PathSeparator = ':';

	private static readonly Type spriteArray = typeof(Sprite).MakeArrayType();

	private LocationConstraintEnum locationConstraint;

	public override ushort TypeCode => 55;

	public LocationConstraintEnum LocationConstraint
	{
		get
		{
			return locationConstraint;
		}
		set
		{
			if (locationConstraint != value)
			{
				locationConstraint = value;
				base.Meta.Repo.Events.MetaWasChanged(base.Meta);
			}
		}
	}

	public override Sprite this[int entityIndex]
	{
		get
		{
			string storedValue = GetStoredValue(entityIndex);
			if (string.IsNullOrEmpty(storedValue))
			{
				return null;
			}
			if (BGAssetsCache.Enabled && BGAssetsCache.TryToGet(storedValue, out var asset) && asset is Sprite result)
			{
				return result;
			}
			Sprite sprite = null;
			if (assetLoader is BGAssetLoaderAddressables)
			{
				string addressablesAddress = GetAddressablesAddress(entityIndex);
				if (string.IsNullOrEmpty(addressablesAddress))
				{
					return null;
				}
				Sprite sprite2 = assetLoader.Load<Sprite>(addressablesAddress);
				if (sprite2 != null && sprite2.name.EndsWith("(Clone)"))
				{
					sprite2.name = sprite2.name.Replace("(Clone)", "");
				}
				sprite = sprite2;
			}
			else
			{
				SpriteLocation spriteLocation = new SpriteLocation(storedValue);
				switch (spriteLocation.Location)
				{
				case LocationEnum.Single:
					sprite = assetLoader.Load<Sprite>(spriteLocation.AssetPath);
					break;
				case LocationEnum.Multiple:
				{
					Sprite[] array = assetLoader.LoadAll<Sprite>(spriteLocation.AssetPath);
					if (array == null)
					{
						return null;
					}
					foreach (Sprite sprite3 in array)
					{
						if (string.Equals(sprite3.name, spriteLocation.SubAssetPath))
						{
							sprite = sprite3;
							break;
						}
					}
					break;
				}
				case LocationEnum.SpriteAtlas:
				{
					SpriteAtlas spriteAtlas = assetLoader.Load<SpriteAtlas>(spriteLocation.AssetPath);
					if (spriteAtlas == null)
					{
						return null;
					}
					sprite = spriteAtlas.GetSprite(spriteLocation.SubAssetPath);
					if (sprite != null && sprite.name.EndsWith("(Clone)"))
					{
						sprite.name = sprite.name.Replace("(Clone)", "");
					}
					break;
				}
				default:
					throw new ArgumentOutOfRangeException("location.Location");
				}
			}
			if (sprite != null)
			{
				if (BGAddressablesMonitor.Enabled)
				{
					BGAddressablesMonitor.AssetWasLoaded(this, base.Meta.FindEntityId(entityIndex));
				}
				if (BGAssetsCache.Enabled)
				{
					BGAssetsCache.Add(storedValue, sprite);
				}
			}
			return sprite;
		}
	}

	public BGFieldUnitySprite(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldUnitySprite(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldUnitySprite(meta, id, name);
	}

	public override string ConfigToString()
	{
		SpriteConfig obj = new SpriteConfig
		{
			LoaderType = assetLoader.GetType().FullName,
			LoaderConfig = assetLoader.ConfigToString(),
			LocationConstraint = (int)locationConstraint
		};
		return JsonUtility.ToJson(obj);
	}

	public override void ConfigFromString(string config)
	{
		if (string.IsNullOrEmpty(config))
		{
			assetLoader = new BGAssetLoaderResources();
			return;
		}
		SpriteConfig spriteConfig = JsonUtility.FromJson<SpriteConfig>(config);
		assetLoader = BGUtil.Create<BGAssetLoaderA>(spriteConfig.LoaderType, includePrivateConstructors: false, Array.Empty<object>());
		assetLoader.ConfigFromString(spriteConfig.LoaderConfig);
		locationConstraint = (LocationConstraintEnum)spriteConfig.LocationConstraint;
	}

	public override byte[] ConfigToBytes()
	{
		string assemblyQualifiedName = assetLoader.GetType().AssemblyQualifiedName;
		byte[] value = assetLoader.ConfigToBytes();
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter(4 + BGBinaryWriter.GetBytesCount(assemblyQualifiedName) + BGBinaryWriter.GetBytesCount(value));
		bGBinaryWriter.AddInt(2);
		bGBinaryWriter.AddString(assemblyQualifiedName);
		bGBinaryWriter.AddByteArray(value);
		bGBinaryWriter.AddInt((int)locationConstraint);
		return bGBinaryWriter.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		BGBinaryReader bGBinaryReader = new BGBinaryReader(config);
		int num = bGBinaryReader.ReadInt();
		switch (num)
		{
		case 1:
			ReadLoader(bGBinaryReader);
			break;
		case 2:
			ReadLoader(bGBinaryReader);
			locationConstraint = (LocationConstraintEnum)bGBinaryReader.ReadInt();
			break;
		default:
			throw new BGException("Unknown version: $", num);
		}
	}

	private void ReadLoader(BGBinaryReader reader)
	{
		assetLoader = BGUtil.Create<BGAssetLoaderA>(reader.ReadString(), includePrivateConstructors: false, Array.Empty<object>());
		assetLoader.ConfigFromBytes(reader.ReadByteArray());
	}

	public BGAddressablesLoaderModel GetAddressablesLoaderModel(int entityIndex)
	{
		string storedValue = GetStoredValue(entityIndex);
		if (string.IsNullOrEmpty(storedValue))
		{
			return null;
		}
		SpriteLocation spriteLocation = new SpriteLocation(storedValue);
		return spriteLocation.Location switch
		{
			LocationEnum.Single => new BGAddressablesLoaderModel(storedValue, typeof(Sprite)), 
			LocationEnum.Multiple => new BGAddressablesLoaderModel(spriteLocation.AssetPath, spriteArray), 
			LocationEnum.SpriteAtlas => new BGAddressablesLoaderModel(spriteLocation.AssetPath, typeof(SpriteAtlas)), 
			_ => throw new ArgumentOutOfRangeException("location.Location"), 
		};
	}

	public override string GetAddressablesAddress(int entityIndex)
	{
		string storedValue = GetStoredValue(entityIndex);
		if (string.IsNullOrEmpty(storedValue))
		{
			return null;
		}
		SpriteLocation spriteLocation = new SpriteLocation(storedValue);
		switch (spriteLocation.Location)
		{
		case LocationEnum.Single:
			return storedValue;
		case LocationEnum.Multiple:
		case LocationEnum.SpriteAtlas:
			if (string.IsNullOrEmpty(spriteLocation.AssetPath) || string.IsNullOrEmpty(spriteLocation.SubAssetPath))
			{
				return null;
			}
			return spriteLocation.AssetPath + "[" + spriteLocation.SubAssetPath + "]";
		default:
			throw new ArgumentOutOfRangeException("location.Location");
		}
	}
}

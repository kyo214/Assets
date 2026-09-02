using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[FieldDescriptor(Name = "unityObject", Folder = "Unity Asset", ManagerType = "BansheeGz.BGDatabase.Editor.BGFieldManagerUnityObject")]
public class BGFieldUnityObject : BGFieldUnityAssetA<UnityEngine.Object>, BGAddressablesAssetCustomLoaderI
{
	public enum AssetLocationEnum
	{
		Single = 0,
		Complex = 1
	}

	[Serializable]
	protected class JsonConfigObject : JsonConfig
	{
		public string AssetTypeName;

		public bool AllowSubclasses;
	}

	public struct AssetLocation
	{
		private readonly AssetLocationEnum location;

		private readonly string assetPath;

		private readonly string subAssetPath;

		private readonly string fullPath;

		public AssetLocationEnum Location => location;

		public string AssetPath => assetPath;

		public string SubAssetPath => subAssetPath;

		public string FullPath => fullPath;

		public AssetLocation(AssetLocationEnum location, string assetPath, string subAssetPath)
		{
			this.location = location;
			this.assetPath = assetPath;
			this.subAssetPath = subAssetPath;
			switch (location)
			{
			case AssetLocationEnum.Single:
				fullPath = assetPath;
				break;
			case AssetLocationEnum.Complex:
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

		public AssetLocation(string path)
		{
			fullPath = path ?? throw new ArgumentException("path is null");
			location = AssetLocationEnum.Single;
			assetPath = path;
			subAssetPath = null;
			if (path.Length <= 2 || path[1] != ':')
			{
				return;
			}
			switch (path[0])
			{
			case '0':
				assetPath = assetPath.Substring(2);
				break;
			case '1':
			{
				int num = assetPath.LastIndexOf(':');
				if (num > 2 && num < assetPath.Length - 1)
				{
					subAssetPath = assetPath.Substring(num + 1);
					assetPath = assetPath.Substring(2, num - 2);
					location = AssetLocationEnum.Complex;
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

	public const ushort CodeType = 52;

	private const char PathSeparator = ':';

	private string assetTypeName;

	private Type assetType;

	private bool typeLoadTried;

	private bool allowSubclasses;

	public override ushort TypeCode => 52;

	public new Type AssetType
	{
		get
		{
			if (assetType != null || typeLoadTried)
			{
				return assetType;
			}
			typeLoadTried = true;
			if (string.IsNullOrEmpty(assetTypeName))
			{
				return assetType;
			}
			Type type = BGUtil.GetType(assetTypeName);
			if (type != null && type.IsSubclassOf(typeof(UnityEngine.Object)))
			{
				assetType = type;
			}
			return assetType;
		}
		set
		{
			if (value == assetType)
			{
				return;
			}
			if (value == null)
			{
				assetType = null;
				assetTypeName = null;
			}
			else
			{
				if (!value.IsSubclassOf(typeof(UnityEngine.Object)))
				{
					throw new BGException("Can not change assetType, cause submitted value type is not inherited from UnityEngine.Object, value=$", value.FullName);
				}
				assetType = value;
				assetTypeName = value.AssemblyQualifiedName;
			}
			base.events.MetaWasChanged(base.Meta);
		}
	}

	public bool AllowSubclasses
	{
		get
		{
			return allowSubclasses;
		}
		set
		{
			if (allowSubclasses != value)
			{
				allowSubclasses = value;
				base.events.MetaWasChanged(base.Meta);
			}
		}
	}

	public override UnityEngine.Object this[int entityIndex]
	{
		get
		{
			string storedValue = GetStoredValue(entityIndex);
			if (string.IsNullOrEmpty(storedValue))
			{
				return null;
			}
			if (BGAssetsCache.Enabled && BGAssetsCache.TryToGet(storedValue, out var asset))
			{
				return asset;
			}
			UnityEngine.Object obj = null;
			if (assetLoader is BGAssetLoaderAddressables)
			{
				string addressablesAddress = GetAddressablesAddress(entityIndex);
				if (string.IsNullOrEmpty(addressablesAddress))
				{
					return null;
				}
				obj = assetLoader.Load<UnityEngine.Object>(addressablesAddress);
			}
			else
			{
				AssetLocation assetLocation = new AssetLocation(storedValue);
				switch (assetLocation.Location)
				{
				case AssetLocationEnum.Single:
					obj = assetLoader.Load<UnityEngine.Object>(assetLocation.AssetPath);
					break;
				case AssetLocationEnum.Complex:
				{
					if (assetLoader is BGAssetLoaderAddressables)
					{
						obj = assetLoader.Load<UnityEngine.Object>(assetLocation.AssetPath + "[" + assetLocation.SubAssetPath + "]");
						break;
					}
					UnityEngine.Object[] array = assetLoader.LoadAll<UnityEngine.Object>(assetLocation.AssetPath);
					if (array == null)
					{
						return null;
					}
					foreach (UnityEngine.Object obj2 in array)
					{
						if (string.Equals(obj2.name, assetLocation.SubAssetPath))
						{
							obj = obj2;
							break;
						}
					}
					break;
				}
				default:
					throw new ArgumentOutOfRangeException("location.Location");
				}
			}
			if (obj != null)
			{
				if (BGAddressablesMonitor.Enabled)
				{
					BGAddressablesMonitor.AssetWasLoaded(this, base.Meta.FindEntityId(entityIndex));
				}
				if (BGAssetsCache.Enabled)
				{
					BGAssetsCache.Add(storedValue, obj);
				}
			}
			return obj;
		}
	}

	public BGFieldUnityObject(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	internal BGFieldUnityObject(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	protected override Func<BGMetaEntity, BGId, string, BGField> CreateFieldFactory()
	{
		return (BGMetaEntity meta, BGId id, string name) => new BGFieldUnityObject(meta, id, name);
	}

	public override string ConfigToString()
	{
		return ConfigToString(new JsonConfigObject
		{
			AssetTypeName = assetTypeName,
			AllowSubclasses = allowSubclasses
		});
	}

	public override void ConfigFromString(string config)
	{
		ConfigFromString(config, (JsonConfigObject jsonConfig) =>
		{
			assetTypeName = jsonConfig.AssetTypeName;
			allowSubclasses = jsonConfig.AllowSubclasses;
		});
	}

	public override byte[] ConfigToBytes()
	{
		string assemblyQualifiedName = assetLoader.GetType().AssemblyQualifiedName;
		byte[] value = assetLoader.ConfigToBytes();
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter(4 + BGBinaryWriter.GetBytesCount(assemblyQualifiedName) + BGBinaryWriter.GetBytesCount(value));
		bGBinaryWriter.AddInt(3);
		bGBinaryWriter.AddString(assemblyQualifiedName);
		bGBinaryWriter.AddByteArray(value);
		bGBinaryWriter.AddString(assetTypeName);
		bGBinaryWriter.AddBool(allowSubclasses);
		return bGBinaryWriter.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		BGBinaryReader bGBinaryReader = new BGBinaryReader(config);
		int num = bGBinaryReader.ReadInt();
		switch (num)
		{
		case 1:
			assetLoader = BGUtil.Create<BGAssetLoaderA>(bGBinaryReader.ReadString(), includePrivateConstructors: false, Array.Empty<object>());
			assetLoader.ConfigFromBytes(bGBinaryReader.ReadByteArray());
			break;
		case 2:
			assetLoader = BGUtil.Create<BGAssetLoaderA>(bGBinaryReader.ReadString(), includePrivateConstructors: false, Array.Empty<object>());
			assetLoader.ConfigFromBytes(bGBinaryReader.ReadByteArray());
			assetTypeName = bGBinaryReader.ReadString();
			ResetType();
			break;
		case 3:
			assetLoader = BGUtil.Create<BGAssetLoaderA>(bGBinaryReader.ReadString(), includePrivateConstructors: false, Array.Empty<object>());
			assetLoader.ConfigFromBytes(bGBinaryReader.ReadByteArray());
			assetTypeName = bGBinaryReader.ReadString();
			allowSubclasses = bGBinaryReader.ReadBool();
			ResetType();
			break;
		default:
			throw new BGException("Unknown version: $", num);
		}
	}

	private void ResetType()
	{
		assetType = null;
		typeLoadTried = false;
	}

	public bool CanBeAssigned(UnityEngine.Object value)
	{
		if (value == null)
		{
			return true;
		}
		if (AssetType == null)
		{
			return true;
		}
		if (AssetType != value.GetType())
		{
			if (!allowSubclasses)
			{
				return false;
			}
			if (!value.GetType().IsSubclassOf(AssetType))
			{
				return false;
			}
		}
		return true;
	}

	public override string GetAddressablesAddress(int entityIndex)
	{
		string storedValue = GetStoredValue(entityIndex);
		if (string.IsNullOrEmpty(storedValue))
		{
			return null;
		}
		AssetLocation assetLocation = new AssetLocation(storedValue);
		switch (assetLocation.Location)
		{
		case AssetLocationEnum.Single:
			return storedValue;
		case AssetLocationEnum.Complex:
			if (string.IsNullOrEmpty(assetLocation.AssetPath) || string.IsNullOrEmpty(assetLocation.SubAssetPath))
			{
				return null;
			}
			return assetLocation.AssetPath + "[" + assetLocation.SubAssetPath + "]";
		default:
			throw new ArgumentOutOfRangeException("location.Location");
		}
	}

	public BGAddressablesLoaderModel GetAddressablesLoaderModel(int entityIndex)
	{
		string storedValue = GetStoredValue(entityIndex);
		if (string.IsNullOrEmpty(storedValue))
		{
			return null;
		}
		AssetLocation assetLocation = new AssetLocation(storedValue);
		return assetLocation.Location switch
		{
			AssetLocationEnum.Single => new BGAddressablesLoaderModel(storedValue, typeof(UnityEngine.Object)), 
			AssetLocationEnum.Complex => new BGAddressablesLoaderModel(assetLocation.AssetPath + "[" + assetLocation.SubAssetPath + "]", typeof(UnityEngine.Object)), 
			_ => throw new ArgumentOutOfRangeException("location.Location"), 
		};
	}
}

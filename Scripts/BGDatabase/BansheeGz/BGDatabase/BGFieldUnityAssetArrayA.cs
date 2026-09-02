using System;
using System.Text;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public abstract class BGFieldUnityAssetArrayA<T> : BGFieldCachedA<T[], string>, BGBinaryBulkLoaderClass, BGAssetLoaderA.WithLoaderI, BGStorableString, BGStorable<string>, BGAddressablesAssetI, BGFieldUnityAssetI where T : UnityEngine.Object
{
	[Serializable]
	private struct JsonConfig
	{
		public string LoaderType;

		public string LoaderConfig;
	}

	private BGAssetLoaderA assetLoader;

	public BGAssetLoaderA AssetLoader
	{
		get
		{
			return assetLoader;
		}
		set
		{
			if (value != assetLoader)
			{
				assetLoader = value ?? throw new BGException("Loader can not be null");
				base.events.MetaWasChanged(base.Meta);
			}
		}
	}

	public virtual Type AssetType => ValueType;

	public override bool ReadOnly => true;

	public override bool StoredValueIsTheSameAsValueType => false;

	public override T[] this[int entityIndex]
	{
		get
		{
			string storedValue = GetStoredValue(entityIndex);
			if (string.IsNullOrEmpty(storedValue))
			{
				return null;
			}
			if (BGAssetsCache.Enabled && BGAssetsCache.TryToGetAll(storedValue, out var assets))
			{
				return (T[])assets;
			}
			T[] array = assetLoader.LoadAll<T>(storedValue);
			if (array != null)
			{
				if (BGAddressablesMonitor.Enabled)
				{
					BGAddressablesMonitor.AssetWasLoaded(this, base.Meta.FindEntityId(entityIndex));
				}
				if (BGAssetsCache.Enabled)
				{
					UnityEngine.Object[] assets2 = array;
					BGAssetsCache.AddAll(storedValue, assets2);
				}
			}
			return array;
		}
		set
		{
		}
	}

	protected BGFieldUnityAssetArrayA(BGMetaEntity meta, string name)
		: base(meta, name)
	{
		assetLoader = new BGAssetLoaderResources();
	}

	protected BGFieldUnityAssetArrayA(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	public override string ConfigToString()
	{
		return JsonUtility.ToJson(new JsonConfig
		{
			LoaderType = assetLoader.GetType().FullName,
			LoaderConfig = assetLoader.ConfigToString()
		});
	}

	public override void ConfigFromString(string config)
	{
		if (string.IsNullOrEmpty(config))
		{
			assetLoader = new BGAssetLoaderResources();
			return;
		}
		JsonConfig jsonConfig = JsonUtility.FromJson<JsonConfig>(config);
		assetLoader = BGUtil.Create<BGAssetLoaderA>(jsonConfig.LoaderType, includePrivateConstructors: false, Array.Empty<object>());
		assetLoader.ConfigFromString(jsonConfig.LoaderConfig);
	}

	public override byte[] ConfigToBytes()
	{
		string assemblyQualifiedName = assetLoader.GetType().AssemblyQualifiedName;
		byte[] value = assetLoader.ConfigToBytes();
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter(4 + BGBinaryWriter.GetBytesCount(assemblyQualifiedName) + BGBinaryWriter.GetBytesCount(value));
		bGBinaryWriter.AddInt(1);
		bGBinaryWriter.AddString(assemblyQualifiedName);
		bGBinaryWriter.AddByteArray(value);
		return bGBinaryWriter.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		BGBinaryReader bGBinaryReader = new BGBinaryReader(config);
		int num = bGBinaryReader.ReadInt();
		if (num == 1)
		{
			assetLoader = BGUtil.Create<BGAssetLoaderA>(bGBinaryReader.ReadString(), includePrivateConstructors: false, Array.Empty<object>());
			assetLoader.ConfigFromBytes(bGBinaryReader.ReadByteArray());
			return;
		}
		throw new BGException("Unknown version: $", num);
	}

	public override void OnEntityDelete(BGEntity entity)
	{
		if (BGAddressablesMonitor.Enabled && BGAddressablesMonitor.UnloadOnRowDelete)
		{
			BGAddressablesMonitor.UnloadAsset(this, entity.Id, entity.Index);
		}
		base.OnEntityDelete(entity);
	}

	public override byte[] ToBytes(int entityIndex)
	{
		string storedValue = GetStoredValue(entityIndex);
		if (storedValue != null)
		{
			return Encoding.UTF8.GetBytes(storedValue);
		}
		return null;
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		if (segment.Count == 0)
		{
			StoreItems[entityIndex] = null;
		}
		else
		{
			StoreItems[entityIndex] = Encoding.UTF8.GetString(segment.Array, segment.Offset, segment.Count);
		}
	}

	public void FromBytes(BGBinaryBulkRequestClass request)
	{
		byte[] array = request.Array;
		BGBinaryBulkRequestClass.CellRequest[] cellRequests = request.CellRequests;
		int num = cellRequests.Length;
		Encoding uTF = Encoding.UTF8;
		for (int i = 0; i < num; i++)
		{
			BGBinaryBulkRequestClass.CellRequest cellRequest = cellRequests[i];
			try
			{
				StoreItems[cellRequest.EntityIndex] = uTF.GetString(array, cellRequest.Offset, cellRequest.Count);
			}
			catch (Exception obj)
			{
				request.OnError?.Invoke(obj);
			}
		}
	}

	public override string ToString(int entityIndex)
	{
		return GetStoredValue(entityIndex);
	}

	public override void FromString(int entityIndex, string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			StoreItems[entityIndex] = null;
		}
		else
		{
			StoreItems[entityIndex] = value;
		}
	}

	public string GetAssetPath(int entityIndex)
	{
		return GetStoredValue(entityIndex);
	}

	public void SetAssetPath(int entityIndex, string path)
	{
		SetStoredValue(entityIndex, path);
	}

	public string GetAddressablesAddress(int entityIndex)
	{
		return GetStoredValue(entityIndex);
	}
}

using System;
using System.Text;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public abstract class BGFieldUnityAssetA<T> : BGFieldCachedA<T, string>, BGBinaryBulkLoaderClass, BGAssetLoaderA.WithLoaderI, BGStorableString, BGStorable<string>, BGAddressablesAssetI, BGFieldUnityAssetI where T : UnityEngine.Object
{
	[Serializable]
	protected class JsonConfig
	{
		public string LoaderType;

		public string LoaderConfig;
	}

	protected BGAssetLoaderA assetLoader;

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

	public override T this[int entityIndex]
	{
		get
		{
			string storedValue = GetStoredValue(entityIndex);
			if (string.IsNullOrEmpty(storedValue))
			{
				return null;
			}
			if (BGAssetsCache.Enabled && BGAssetsCache.TryToGet(storedValue, out var asset) && asset is T result)
			{
				return result;
			}
			T val = assetLoader.Load<T>(storedValue);
			if (val != null)
			{
				if (BGAddressablesMonitor.Enabled)
				{
					BGAddressablesMonitor.AssetWasLoaded(this, base.Meta.FindEntityId(entityIndex));
				}
				if (BGAssetsCache.Enabled)
				{
					BGAssetsCache.Add(storedValue, val);
				}
			}
			return val;
		}
		set
		{
		}
	}

	protected BGFieldUnityAssetA(BGMetaEntity meta, string name)
		: base(meta, name)
	{
		assetLoader = new BGAssetLoaderResources();
	}

	protected BGFieldUnityAssetA(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	public override string ConfigToString()
	{
		return ConfigToString(new JsonConfig());
	}

	public override void ConfigFromString(string config)
	{
		ConfigFromString<JsonConfig>(config, null);
	}

	protected virtual string ConfigToString(JsonConfig config)
	{
		config.LoaderType = assetLoader.GetType().FullName;
		config.LoaderConfig = assetLoader.ConfigToString();
		return JsonUtility.ToJson(config);
	}

	protected virtual void ConfigFromString<T>(string config, Action<T> callback) where T : JsonConfig
	{
		if (string.IsNullOrEmpty(config))
		{
			assetLoader = new BGAssetLoaderResources();
			return;
		}
		T val = JsonUtility.FromJson<T>(config);
		assetLoader = BGUtil.Create<BGAssetLoaderA>(val.LoaderType, includePrivateConstructors: false, Array.Empty<object>());
		assetLoader.ConfigFromString(val.LoaderConfig);
		callback?.Invoke(val);
	}

	public override byte[] ConfigToBytes()
	{
		string assemblyQualifiedName = assetLoader.GetType().AssemblyQualifiedName;
		byte[] value = assetLoader.ConfigToBytes();
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter(4 + BGBinaryWriter.GetBytesCount(assemblyQualifiedName) + BGBinaryWriter.GetBytesCount(value));
		bGBinaryWriter.AddInt(1);
		bGBinaryWriter.AddString(assemblyQualifiedName);
		bGBinaryWriter.AddByteArray(value);
		ConfigToBytes(bGBinaryWriter);
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
			ConfigFromBytes(num, bGBinaryReader);
			return;
		}
		throw new BGException("Unknown version: $", num);
	}

	protected virtual void ConfigToBytes(BGBinaryWriter writer)
	{
	}

	protected virtual void ConfigFromBytes(int version, BGBinaryReader reader)
	{
	}

	public override void OnEntityDelete(BGEntity entity)
	{
		if (BGAddressablesMonitor.Enabled && BGAddressablesMonitor.UnloadOnRowDelete)
		{
			BGAddressablesMonitor.UnloadAsset(this, entity.Id, entity.Index);
		}
		base.OnEntityDelete(entity);
	}

	public virtual string GetAssetPath(int entityIndex)
	{
		return GetStoredValue(entityIndex);
	}

	public void SetAssetPath(int entityIndex, string path)
	{
		SetStoredValue(entityIndex, path);
	}

	public virtual string GetAddressablesAddress(int entityIndex)
	{
		return GetStoredValue(entityIndex);
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
}

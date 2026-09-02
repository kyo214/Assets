using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGSyncIdConfig : ISerializationCallbackReceiver, BGConfigurableBinaryI
{
	public enum IdConfigEnum : byte
	{
		IdColumn = 0,
		NoId = 1,
		Index = 2,
		Field = 3
	}

	[Serializable]
	public class BGSyncIdConfigMeta : BGObjectI
	{
		public string metaIdStr;

		public IdConfigEnum configType;

		public string fieldIdStr;

		public BGId MetaId
		{
			get
			{
				if (!BGId.TryParse(metaIdStr, out var id))
				{
					return BGId.Empty;
				}
				return id;
			}
		}

		public BGId Id => MetaId;

		public BGId FieldId
		{
			get
			{
				if (!BGId.TryParse(fieldIdStr, out var id))
				{
					return BGId.Empty;
				}
				return id;
			}
		}
	}

	[SerializeField]
	private List<BGSyncIdConfigMeta> metaConfigs = new List<BGSyncIdConfigMeta>();

	private Dictionary<BGId, BGSyncIdConfigMeta> metaId2Config = new Dictionary<BGId, BGSyncIdConfigMeta>();

	public int CountMetas => metaId2Config.Count;

	public void OnBeforeSerialize()
	{
		metaConfigs.Clear();
		bool ok = BGRepo.Ok;
		foreach (KeyValuePair<BGId, BGSyncIdConfigMeta> item in metaId2Config)
		{
			if (!ok || BGRepo.I.HasMeta(item.Key))
			{
				metaConfigs.Add(item.Value);
			}
		}
	}

	public void OnAfterDeserialize()
	{
		metaId2Config.Clear();
		foreach (BGSyncIdConfigMeta metaConfig in metaConfigs)
		{
			if (BGId.TryParse(metaConfig.metaIdStr, out var id))
			{
				metaId2Config[id] = metaConfig;
			}
		}
	}

	public string GetError(BGRepo repo)
	{
		foreach (KeyValuePair<BGId, BGSyncIdConfigMeta> item in metaId2Config)
		{
			BGSyncIdConfigMeta value = item.Value;
			if (value.configType != IdConfigEnum.Field)
			{
				continue;
			}
			BGId metaId = value.MetaId;
			if (metaId == BGId.Empty)
			{
				continue;
			}
			BGMetaEntity meta = repo.GetMeta(metaId);
			if (meta != null)
			{
				BGId fieldId = value.FieldId;
				if (fieldId.IsEmpty)
				{
					return "Field is not set for " + meta.Name;
				}
				if (!meta.HasField(fieldId))
				{
					BGId bGId = fieldId;
					return "Can not find a field with ID=" + bGId.ToString() + " in meta" + meta.Name;
				}
			}
		}
		return null;
	}

	public BGSyncIdConfigMeta GetMetaConfig(BGId metaId)
	{
		if (!metaId2Config.TryGetValue(metaId, out var value))
		{
			return null;
		}
		return value;
	}

	public BGSyncIdConfigMeta EnsureMetaConfig(BGId metaId)
	{
		BGSyncIdConfigMeta bGSyncIdConfigMeta = GetMetaConfig(metaId);
		if (bGSyncIdConfigMeta == null)
		{
			bGSyncIdConfigMeta = AddMetaConfig(metaId);
		}
		return bGSyncIdConfigMeta;
	}

	private BGSyncIdConfigMeta AddMetaConfig(BGId metaId)
	{
		BGSyncIdConfigMeta bGSyncIdConfigMeta = new BGSyncIdConfigMeta
		{
			metaIdStr = metaId.ToString()
		};
		metaId2Config[metaId] = bGSyncIdConfigMeta;
		return bGSyncIdConfigMeta;
	}

	public void RemoveMetaSetting(BGId metaId)
	{
		metaId2Config.Remove(metaId);
	}

	public void ForEach(Action<BGId, BGSyncIdConfigMeta> action)
	{
		foreach (KeyValuePair<BGId, BGSyncIdConfigMeta> item in metaId2Config)
		{
			action(item.Key, item.Value);
		}
	}

	public static bool IsSupported(BGField field)
	{
		if (!(field is BGFieldString))
		{
			return field is BGFieldInt;
		}
		return true;
	}

	public byte[] ConfigToBytes()
	{
		OnBeforeSerialize();
		BGBinaryWriter writer = new BGBinaryWriter();
		writer.AddInt(1);
		writer.AddArray(() =>
		{
			foreach (BGSyncIdConfigMeta metaConfig in metaConfigs)
			{
				writer.AddString(metaConfig.metaIdStr);
				writer.AddString(metaConfig.fieldIdStr);
				writer.AddInt((int)metaConfig.configType);
			}
		}, metaConfigs.Count);
		return writer.ToArray();
	}

	public void ConfigFromBytes(ArraySegment<byte> config)
	{
		if (config.Count >= 8)
		{
			metaConfigs.Clear();
			metaId2Config.Clear();
			BGBinaryReader reader = new BGBinaryReader(config);
			int num = reader.ReadInt();
			if (num != 1)
			{
				throw new Exception("Unsupported version " + num);
			}
			reader.ReadArray(() =>
			{
				string metaIdStr = reader.ReadString();
				string fieldIdStr = reader.ReadString();
				IdConfigEnum configType = (IdConfigEnum)reader.ReadInt();
				metaConfigs.Add(new BGSyncIdConfigMeta
				{
					configType = configType,
					fieldIdStr = fieldIdStr,
					metaIdStr = metaIdStr
				});
			});
			OnAfterDeserialize();
		}
	}
}

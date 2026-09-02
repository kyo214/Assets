using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGSyncRelationsConfig : ISerializationCallbackReceiver, BGConfigurableBinaryI
{
	public enum RelationConfigEnum : byte
	{
		IdColumn = 0,
		Field = 1
	}

	public enum DefaultRelationConfigEnum : byte
	{
		IdColumn = 0,
		Name = 1,
		IdConfig = 2
	}

	[Serializable]
	public class BGSyncRelationConfigMeta : BGObjectI
	{
		public string metaIdStr;

		public RelationConfigEnum configType;

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
	private List<BGSyncRelationConfigMeta> metaConfigs = new List<BGSyncRelationConfigMeta>();

	[SerializeField]
	private DefaultRelationConfigEnum defaultConfig;

	private Dictionary<BGId, BGSyncRelationConfigMeta> metaId2Config = new Dictionary<BGId, BGSyncRelationConfigMeta>();

	public int CountMetas => metaId2Config.Count;

	public DefaultRelationConfigEnum DefaultConfig
	{
		get
		{
			return defaultConfig;
		}
		set
		{
			defaultConfig = value;
		}
	}

	public void OnBeforeSerialize()
	{
		metaConfigs.Clear();
		bool ok = BGRepo.Ok;
		foreach (KeyValuePair<BGId, BGSyncRelationConfigMeta> item in metaId2Config)
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
		foreach (BGSyncRelationConfigMeta metaConfig in metaConfigs)
		{
			if (BGId.TryParse(metaConfig.metaIdStr, out var id))
			{
				metaId2Config[id] = metaConfig;
			}
		}
	}

	public string GetError(BGRepo repo)
	{
		foreach (KeyValuePair<BGId, BGSyncRelationConfigMeta> item in metaId2Config)
		{
			BGSyncRelationConfigMeta value = item.Value;
			if (value.configType != RelationConfigEnum.Field)
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

	public bool HasMetaConfig(BGId metaId)
	{
		return GetMetaConfig(metaId) != null;
	}

	public BGSyncRelationConfigMeta GetMetaConfig(BGId metaId)
	{
		if (!metaId2Config.TryGetValue(metaId, out var value))
		{
			return null;
		}
		return value;
	}

	public BGSyncRelationConfigMeta EnsureMetaConfig(BGId metaId)
	{
		BGSyncRelationConfigMeta bGSyncRelationConfigMeta = GetMetaConfig(metaId);
		if (bGSyncRelationConfigMeta == null)
		{
			bGSyncRelationConfigMeta = AddMetaConfig(metaId);
		}
		return bGSyncRelationConfigMeta;
	}

	private BGSyncRelationConfigMeta AddMetaConfig(BGId metaId)
	{
		BGSyncRelationConfigMeta bGSyncRelationConfigMeta = new BGSyncRelationConfigMeta
		{
			metaIdStr = metaId.ToString()
		};
		metaId2Config[metaId] = bGSyncRelationConfigMeta;
		return bGSyncRelationConfigMeta;
	}

	public void RemoveMetaSetting(BGId metaId)
	{
		metaId2Config.Remove(metaId);
	}

	public void ForEach(Action<BGId, BGSyncRelationConfigMeta> action)
	{
		foreach (KeyValuePair<BGId, BGSyncRelationConfigMeta> item in metaId2Config)
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
		writer.AddInt(2);
		writer.AddByte((byte)defaultConfig);
		writer.AddArray(() =>
		{
			foreach (BGSyncRelationConfigMeta metaConfig in metaConfigs)
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
		if (config.Count < 8)
		{
			return;
		}
		metaConfigs.Clear();
		metaId2Config.Clear();
		BGBinaryReader reader = new BGBinaryReader(config);
		int num = reader.ReadInt();
		switch (num)
		{
		case 1:
			reader.ReadArray(() =>
			{
				string metaIdStr = reader.ReadString();
				string fieldIdStr = reader.ReadString();
				RelationConfigEnum configType = (RelationConfigEnum)reader.ReadInt();
				metaConfigs.Add(new BGSyncRelationConfigMeta
				{
					configType = configType,
					fieldIdStr = fieldIdStr,
					metaIdStr = metaIdStr
				});
			});
			break;
		case 2:
			defaultConfig = (DefaultRelationConfigEnum)reader.ReadByte();
			reader.ReadArray(() =>
			{
				string metaIdStr = reader.ReadString();
				string fieldIdStr = reader.ReadString();
				RelationConfigEnum configType = (RelationConfigEnum)reader.ReadInt();
				metaConfigs.Add(new BGSyncRelationConfigMeta
				{
					configType = configType,
					fieldIdStr = fieldIdStr,
					metaIdStr = metaIdStr
				});
			});
			break;
		default:
			throw new Exception("Unsupported version " + num);
		}
		OnAfterDeserialize();
	}
}

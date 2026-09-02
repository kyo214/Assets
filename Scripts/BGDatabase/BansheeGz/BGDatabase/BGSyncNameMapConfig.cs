using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGSyncNameMapConfig : BGConfigurableBinaryI
{
	[Serializable]
	public class NameMap
	{
		public string Id;

		public string Name;

		public bool HasMapping => !string.IsNullOrEmpty(Name);

		public NameMap(string id)
		{
			Id = id;
		}

		public override string ToString()
		{
			return Id + "/" + Name;
		}
	}

	[Serializable]
	public class MetaMap : NameMap
	{
		public List<NameMap> Fields;

		public int CountFields
		{
			get
			{
				if (Fields == null)
				{
					return 0;
				}
				int num = 0;
				foreach (NameMap field in Fields)
				{
					if (field.HasMapping)
					{
						num++;
					}
				}
				return num;
			}
		}

		public MetaMap(string id)
			: base(id)
		{
		}

		public string GetFieldName(BGField field)
		{
			NameMap fieldMap = GetFieldMap(field.Id.ToString());
			if (fieldMap != null && fieldMap.HasMapping)
			{
				return fieldMap.Name;
			}
			return field.Name;
		}

		public NameMap GetFieldMap(string fieldId)
		{
			if (Fields == null)
			{
				return null;
			}
			foreach (NameMap field in Fields)
			{
				if (string.Equals(field.Id, fieldId))
				{
					return field;
				}
			}
			return null;
		}

		public NameMap EnsureFieldMap(string fieldId)
		{
			return GetFieldMap(fieldId) ?? AddFieldMap(fieldId);
		}

		private NameMap AddFieldMap(string fieldId)
		{
			if (Fields == null)
			{
				Fields = new List<NameMap>();
			}
			NameMap nameMap = new NameMap(fieldId);
			Fields.Add(nameMap);
			return nameMap;
		}

		public bool Trim(BGRepo repo)
		{
			BGMetaEntity meta = repo.GetMeta(BGId.Parse(Id));
			if (meta == null)
			{
				return false;
			}
			bool flag = base.HasMapping;
			if (Fields != null)
			{
				for (int num = Fields.Count - 1; num >= 0; num--)
				{
					NameMap nameMap = Fields[num];
					flag = flag || nameMap.HasMapping;
					if (!nameMap.HasMapping || !meta.HasField(BGId.Parse(nameMap.Id)))
					{
						Fields.RemoveAt(num);
					}
				}
			}
			return flag;
		}

		public BGId GetFieldId(string fieldName)
		{
			if (Fields == null)
			{
				return BGId.Empty;
			}
			foreach (NameMap field in Fields)
			{
				if (string.Equals(field.Name, fieldName))
				{
					return BGId.Parse(field.Id);
				}
			}
			return BGId.Empty;
		}

		public bool HasFieldMapping(BGId fieldId)
		{
			if (Fields == null)
			{
				return false;
			}
			string b = fieldId.ToString();
			foreach (NameMap field in Fields)
			{
				if (string.Equals(field.Id, b))
				{
					return true;
				}
			}
			return false;
		}
	}

	public interface BGNameConfigOwner
	{
		BGSyncNameMapConfig NameMapConfig { get; set; }

		bool NameMapConfigEnabled { get; set; }
	}

	private const string DisallowedCharacters = "\\/*?:[]";

	[SerializeField]
	private List<MetaMap> metas = new List<MetaMap>();

	public int CountMetas
	{
		get
		{
			if (metas == null)
			{
				return 0;
			}
			int num = 0;
			foreach (MetaMap meta in metas)
			{
				if (meta.HasMapping)
				{
					num++;
				}
			}
			return num;
		}
	}

	public int CountFields
	{
		get
		{
			if (metas == null)
			{
				return 0;
			}
			int num = 0;
			foreach (MetaMap meta in metas)
			{
				num += meta.CountFields;
			}
			return num;
		}
	}

	public string GetError(BGRepo repo)
	{
		if (metas == null)
		{
			return null;
		}
		foreach (MetaMap metaConfig in metas)
		{
			if (!metaConfig.HasMapping)
			{
				continue;
			}
			if (metaConfig.Name.Length > 31)
			{
				return "Sheet name [" + metaConfig.Name + "] length exceeds maximum number of characters (31)";
			}
			string text = "\\/*?:[]";
			for (int i = 0; i < text.Length; i++)
			{
				char value = text[i];
				if (metaConfig.Name.IndexOf(value) != -1)
				{
					return "Meta name [" + metaConfig.Name + "] contains a prohibited character (" + value + ")";
				}
			}
			BGMetaEntity meta = repo.GetMeta(BGId.Parse(metaConfig.Id));
			if (meta == null)
			{
				continue;
			}
			BGMetaEntity bGMetaEntity = repo.FindMeta((BGMetaEntity m) => m.Id != meta.Id && string.Equals(metaConfig.Name, m.Name));
			if (bGMetaEntity != null)
			{
				return "Mapped name [" + metaConfig.Name + "], used for [" + meta.Name + "] meta conflicts with [" + bGMetaEntity.Name + "] meta name";
			}
			foreach (MetaMap meta3 in metas)
			{
				if (metaConfig != meta3 && meta3.HasMapping && repo.HasMeta(BGId.Parse(metaConfig.Id)) && repo.HasMeta(BGId.Parse(meta3.Id)) && string.Equals(metaConfig.Name, meta3.Name))
				{
					return "The same name [" + metaConfig.Name + "] is used by multiple metas";
				}
			}
		}
		foreach (MetaMap meta4 in metas)
		{
			BGMetaEntity meta2 = repo.GetMeta(BGId.Parse(meta4.Id));
			if (meta2 == null || meta4.Fields == null)
			{
				continue;
			}
			foreach (NameMap configField in meta4.Fields)
			{
				if (!configField.HasMapping)
				{
					continue;
				}
				BGField field = meta2.GetField(BGId.Parse(configField.Id), errorIfNotFound: false);
				if (field == null)
				{
					continue;
				}
				BGField bGField = meta2.FindField((BGField f) => f.Id != field.Id && string.Equals(configField.Name, f.Name));
				if (bGField != null)
				{
					return "Mapped name [" + configField.Name + "], used for [" + field.FullName + "] field conflicts with [" + bGField.FullName + "] field";
				}
				foreach (NameMap field2 in meta4.Fields)
				{
					if (configField != field2 && field2.HasMapping && meta2.HasField(BGId.Parse(configField.Id)) && meta2.HasField(BGId.Parse(field2.Id)) && string.Equals(configField.Name, field2.Name))
					{
						return "The same field name [" + configField.Name + "] is used by multiple fields of [" + meta2.Name + "] meta";
					}
				}
			}
		}
		return null;
	}

	public void Trim(BGRepo repo = null)
	{
		if (metas == null)
		{
			return;
		}
		for (int num = metas.Count - 1; num >= 0; num--)
		{
			MetaMap metaMap = metas[num];
			if (!metaMap.Trim(repo ?? BGRepo.I))
			{
				metas.RemoveAt(num);
			}
		}
	}

	public string GetName(BGMetaEntity meta)
	{
		MetaMap metaMap = GetMetaMap(meta.Id.ToString());
		if (metaMap != null && metaMap.HasMapping)
		{
			return metaMap.Name;
		}
		return meta.Name;
	}

	public string GetName(BGField field)
	{
		MetaMap metaMap = GetMetaMap(field.MetaId.ToString());
		if (metaMap != null)
		{
			return metaMap.GetFieldName(field);
		}
		return field.Name;
	}

	public BGId GetDatabaseMetaId(string sheetName)
	{
		if (metas == null)
		{
			return BGId.Empty;
		}
		foreach (MetaMap meta in metas)
		{
			if (string.Equals(meta.Name, sheetName))
			{
				return BGId.Parse(meta.Id);
			}
		}
		return BGId.Empty;
	}

	public bool HasMetaConfig(BGId metaId)
	{
		return GetMetaMap(metaId.ToString())?.HasMapping ?? false;
	}

	private bool HasMetaConfig(string sheetName)
	{
		return !GetDatabaseMetaId(sheetName).IsEmpty;
	}

	public BGId GetDatabaseFieldId(BGId metaId, string headerName)
	{
		return GetMetaMap(metaId.ToString())?.GetFieldId(headerName) ?? BGId.Empty;
	}

	private bool HasFieldConfig(BGId metaId, string fieldName)
	{
		MetaMap metaMap = GetMetaMap(metaId.ToString());
		if (metaMap == null)
		{
			return false;
		}
		return !metaMap.GetFieldId(fieldName).IsEmpty;
	}

	private bool HasFieldConfig(BGId metaId, BGId fieldId)
	{
		return GetMetaMap(metaId.ToString())?.HasFieldMapping(fieldId) ?? false;
	}

	public MetaMap EnsureMetaMap(string metaId)
	{
		return GetMetaMap(metaId) ?? AddMetaMap(metaId);
	}

	private MetaMap AddMetaMap(string metaId)
	{
		MetaMap metaMap = new MetaMap(metaId);
		metas.Add(metaMap);
		return metaMap;
	}

	public MetaMap GetMetaMap(string metaId)
	{
		if (metas != null)
		{
			foreach (MetaMap meta in metas)
			{
				if (string.Equals(meta.Id, metaId))
				{
					return meta;
				}
			}
		}
		return null;
	}

	public MetaMap GetMetaMapByName(string name)
	{
		if (metas != null)
		{
			foreach (MetaMap meta in metas)
			{
				if (string.Equals(meta.Name, name))
				{
					return meta;
				}
			}
		}
		return null;
	}

	public BGMetaEntity Map(BGRepo repo, string sheetName)
	{
		if (HasMetaConfig(sheetName))
		{
			BGId databaseMetaId = GetDatabaseMetaId(sheetName);
			return repo.GetMeta(databaseMetaId);
		}
		BGMetaEntity bGMetaEntity = repo[sheetName];
		if (bGMetaEntity != null && HasMetaConfig(bGMetaEntity.Id))
		{
			return null;
		}
		return bGMetaEntity;
	}

	public BGField Map(BGMetaEntity meta, string headerName)
	{
		if (HasFieldConfig(meta.Id, headerName))
		{
			BGId databaseFieldId = GetDatabaseFieldId(meta.Id, headerName);
			return meta.GetField(databaseFieldId, errorIfNotFound: false);
		}
		BGField field = meta.GetField(headerName, errorIfNotFound: false);
		if (field != null && HasFieldConfig(meta.Id, field.Id))
		{
			return null;
		}
		return field;
	}

	public void Clear()
	{
		metas?.Clear();
	}

	public byte[] ConfigToBytes()
	{
		BGBinaryWriter writer = new BGBinaryWriter();
		writer.AddInt(1);
		writer.AddArray(() =>
		{
			foreach (MetaMap metaMap in metas)
			{
				writer.AddString(metaMap.Id);
				writer.AddString(metaMap.Name);
				writer.AddArray(() =>
				{
					if (metaMap.Fields == null || metaMap.Fields.Count == 0)
					{
						return;
					}
					foreach (NameMap field in metaMap.Fields)
					{
						writer.AddString(field.Id);
						writer.AddString(field.Name);
					}
				}, metaMap.Fields?.Count ?? 0);
			}
		}, metas.Count);
		return writer.ToArray();
	}

	public void ConfigFromBytes(ArraySegment<byte> config)
	{
		if (config.Count < 8)
		{
			return;
		}
		metas.Clear();
		BGBinaryReader reader = new BGBinaryReader(config);
		int num = reader.ReadInt();
		if (num == 1)
		{
			reader.ReadArray(() =>
			{
				string id = reader.ReadString();
				string name = reader.ReadString();
				MetaMap metaMap = new MetaMap(id)
				{
					Name = name
				};
				metas.Add(metaMap);
				reader.ReadArray(() =>
				{
					string id2 = reader.ReadString();
					string name2 = reader.ReadString();
					metaMap.Fields = metaMap.Fields ?? new List<NameMap>();
					metaMap.Fields.Add(new NameMap(id2)
					{
						Name = name2
					});
				});
			});
			return;
		}
		throw new Exception("Unsupported version " + num);
	}
}

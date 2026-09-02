using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGSyncDataStructure
{
	public class BGSyncMetaData
	{
		private readonly BGSyncDataStructure structure;

		public readonly List<BGSyncFieldData> fields = new List<BGSyncFieldData>();

		public readonly string sheetName;

		public string metaName;

		public bool disabled;

		public int idColumn = -1;

		private BGSyncDisabledConfig disabledConfig;

		public BGSyncDataStructure Structure => structure;

		public string Error
		{
			get
			{
				if (string.IsNullOrEmpty(metaName))
				{
					return "Meta name not set";
				}
				string text = BGMetaObject.CheckName(metaName);
				if (text != null)
				{
					return text;
				}
				foreach (BGSyncMetaData meta in structure.metas)
				{
					if (meta != this && string.Equals(meta.metaName, metaName))
					{
						return "Duplicate name: " + metaName + ". Name must be unique";
					}
				}
				return null;
			}
		}

		public string ErrorIncludingFields
		{
			get
			{
				string error = Error;
				if (error != null)
				{
					return error;
				}
				foreach (BGSyncFieldData field in fields)
				{
					if (!string.IsNullOrEmpty(field.Error))
					{
						string text = (string.Equals(field.headerName, field.fieldName) ? field.headerName : (field.headerName + " (" + field.fieldName + ")"));
						return "[" + text + "]: " + field.Error;
					}
				}
				return null;
			}
		}

		public BGSyncDisabledConfig DisabledConfig
		{
			get
			{
				return disabledConfig;
			}
			set
			{
				disabledConfig = value;
				if (value == null)
				{
					foreach (BGSyncFieldData field in fields)
					{
						field.DisabledConfig = null;
					}
					return;
				}
				foreach (BGSyncFieldData field2 in fields)
				{
					field2.DisabledConfig = value;
				}
			}
		}

		public BGSyncMetaData(BGSyncDataStructure structure, string sheetName)
		{
			this.structure = structure;
			this.sheetName = (metaName = sheetName);
		}

		public void SetDisabled(bool disabled)
		{
			this.disabled = disabled;
			disabledConfig?.SetDisabled(sheetName, disabled);
		}

		public BGSyncFieldData GetFieldByHeaderName(string headerName)
		{
			foreach (BGSyncFieldData field in fields)
			{
				if (field.headerName == headerName)
				{
					return field;
				}
			}
			return null;
		}

		public override string ToString()
		{
			if (!(metaName == sheetName))
			{
				return sheetName + " (" + metaName + ")";
			}
			return sheetName;
		}
	}

	public class BGSyncFieldData : BGObjectI
	{
		private readonly BGSyncMetaData meta;

		public string headerName;

		public string fieldName;

		public Type fieldType;

		public bool disabled;

		private readonly BGId id;

		private BGSyncDisabledConfig disabledConfig;

		public Action addHandlerTest;

		public object addHandler;

		public BGId Id => id;

		public BGSyncMetaData Meta => meta;

		public string Error
		{
			get
			{
				if (disabled)
				{
					return null;
				}
				if (fieldType == null)
				{
					return "Field type not set";
				}
				if (!typeof(BGField).IsAssignableFrom(fieldType))
				{
					return "Field type is not assignable to BGField";
				}
				if (string.IsNullOrEmpty(fieldName))
				{
					return "Field name not set";
				}
				string text = BGMetaObject.CheckName(fieldName);
				if (text != null)
				{
					return text;
				}
				foreach (BGSyncFieldData field in meta.fields)
				{
					if (field != this && string.Equals(field.fieldName, fieldName))
					{
						return "Duplicate name: " + fieldName + ". Name must be unique";
					}
				}
				if (addHandlerTest != null)
				{
					try
					{
						addHandlerTest();
					}
					catch (Exception ex)
					{
						return "Error: " + ex.Message;
					}
				}
				return null;
			}
		}

		public BGSyncDisabledConfig DisabledConfig
		{
			get
			{
				return disabledConfig;
			}
			set
			{
				disabledConfig = value;
				if (value != null && value.HasField(meta.sheetName, fieldName))
				{
					disabled = true;
				}
			}
		}

		public BGSyncFieldData(BGSyncMetaData meta, string headerName)
		{
			id = BGId.NewId;
			this.meta = meta;
			this.headerName = (fieldName = headerName);
		}

		public void SetDisabled(bool disabled)
		{
			this.disabled = disabled;
			disabledConfig?.SetDisabled(meta.sheetName, fieldName, disabled);
		}

		public override string ToString()
		{
			if (!(headerName == fieldName))
			{
				return headerName + " (" + fieldName + ")";
			}
			return headerName;
		}
	}

	public readonly List<BGSyncMetaData> metas = new List<BGSyncMetaData>();

	private BGSyncNameMapConfig namesConfig;

	public BGSyncDisabledConfig DisabledConfig
	{
		set
		{
			if (value == null)
			{
				foreach (BGSyncMetaData meta in metas)
				{
					meta.DisabledConfig = null;
				}
				return;
			}
			foreach (BGSyncMetaData meta2 in metas)
			{
				if (value.HasTable(meta2.metaName))
				{
					meta2.disabled = true;
				}
				meta2.DisabledConfig = value;
			}
		}
	}

	public int ErrorsCount
	{
		get
		{
			int num = 0;
			foreach (BGSyncMetaData meta in metas)
			{
				if (meta.disabled)
				{
					continue;
				}
				if (meta.Error != null)
				{
					num++;
				}
				foreach (BGSyncFieldData field in meta.fields)
				{
					if (field.Error != null)
					{
						num++;
					}
				}
			}
			return num;
		}
	}

	public BGSyncNameMapConfig NameConfig
	{
		get
		{
			BGSyncNameMapConfig result = namesConfig;
			foreach (BGSyncMetaData meta in metas)
			{
				if (meta.disabled)
				{
					continue;
				}
				if (!string.Equals(meta.metaName, meta.sheetName))
				{
					BGSyncNameMapConfig.MetaMap metaMap = EnsureMetaMap(ref result, meta.metaName);
					if (metaMap == null)
					{
						continue;
					}
					metaMap.Name = meta.sheetName;
				}
				foreach (BGSyncFieldData field3 in meta.fields)
				{
					if (field3.disabled || string.Equals(field3.fieldName, field3.headerName))
					{
						continue;
					}
					BGSyncNameMapConfig.MetaMap metaMap2 = EnsureMetaMap(ref result, meta.metaName);
					if (metaMap2 != null)
					{
						BGField field2 = BGRepo.I.GetMeta(meta.metaName).GetField(field3.fieldName, errorIfNotFound: false);
						if (field2 != null)
						{
							metaMap2.EnsureFieldMap(field2.Id.ToString()).Name = field3.headerName;
						}
					}
				}
			}
			return result;
		}
	}

	public void SetNamesConfig(BGSyncNameMapConfig namesConfig, BGRepo repo)
	{
		this.namesConfig = namesConfig;
		if (this.namesConfig == null)
		{
			return;
		}
		foreach (BGSyncMetaData meta3 in metas)
		{
			BGSyncNameMapConfig.MetaMap metaMap = this.namesConfig.GetMetaMapByName(meta3.sheetName);
			if (metaMap == null && meta3.metaName != null)
			{
				BGMetaEntity meta = repo.GetMeta(meta3.metaName);
				if (meta != null)
				{
					metaMap = this.namesConfig.GetMetaMap(meta.Id.ToString());
				}
			}
			if (metaMap == null || !BGId.TryParse(metaMap.Id, out var id))
			{
				continue;
			}
			BGMetaEntity meta2 = repo.GetMeta(id);
			if (meta2 == null)
			{
				continue;
			}
			meta3.metaName = meta2.Name;
			List<BGSyncNameMapConfig.NameMap> fields = metaMap.Fields;
			if (BGUtil.IsEmpty(fields))
			{
				continue;
			}
			foreach (BGSyncNameMapConfig.NameMap item in fields)
			{
				if (!item.HasMapping || !BGId.TryParse(item.Id, out var id2))
				{
					continue;
				}
				BGField field = meta2.GetField(id2, errorIfNotFound: false);
				if (field != null)
				{
					BGSyncFieldData fieldByHeaderName = meta3.GetFieldByHeaderName(item.Name);
					if (fieldByHeaderName != null)
					{
						fieldByHeaderName.fieldName = field.Name;
					}
				}
			}
		}
	}

	public BGMergeSettingsEntity GetSetting(BGRepo repo)
	{
		BGMergeSettingsEntity bGMergeSettingsEntity = new BGMergeSettingsEntity();
		foreach (BGSyncMetaData meta2 in metas)
		{
			if (meta2.disabled)
			{
				continue;
			}
			BGMetaEntity meta = repo.GetMeta(meta2.metaName);
			if (meta == null)
			{
				continue;
			}
			BGMergeSettingsEntity.MetaSettings metaSettings = bGMergeSettingsEntity.Ensure(meta.Id);
			metaSettings.AddMissing = true;
			metaSettings.UpdateMatching = true;
			metaSettings.UseIncludedFields = true;
			foreach (BGSyncFieldData field2 in meta2.fields)
			{
				if (!field2.disabled)
				{
					BGField field = meta.GetField(field2.fieldName, errorIfNotFound: false);
					if (field != null)
					{
						metaSettings.AddField(field.Id);
					}
				}
			}
		}
		return bGMergeSettingsEntity;
	}

	private static BGSyncNameMapConfig.MetaMap EnsureMetaMap(ref BGSyncNameMapConfig result, string metaName)
	{
		BGMetaEntity meta = BGRepo.I.GetMeta(metaName);
		if (meta == null)
		{
			return null;
		}
		if (result == null)
		{
			result = new BGSyncNameMapConfig();
		}
		return result.EnsureMetaMap(meta.Id.ToString());
	}
}

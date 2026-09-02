using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGModdingRepoProtection
{
	public enum FieldSettingEnum
	{
		Inherited = 0,
		Enabled = 1,
		EnabledHigh = 2,
		Disabled = 3,
		DisabledHigh = 4
	}

	[Serializable]
	internal class DataProtectionJson
	{
		[SerializeField]
		internal List<DataProtectionMetaJson> list = new List<DataProtectionMetaJson>();
	}

	[Serializable]
	internal class DataProtectionMetaJson
	{
		public string MetaId;

		public bool DisableAdd;

		public bool DisableDelete;

		public bool DisableEdit;

		public readonly List<DataProtectionFieldJson> FieldsEdit = new List<DataProtectionFieldJson>();

		public readonly List<DataProtectionObjectJson> RowsEdit = new List<DataProtectionObjectJson>();

		public readonly List<DataProtectionObjectJson> RowsDelete = new List<DataProtectionObjectJson>();

		public readonly List<DataProtectionCellJson> CellsEdit = new List<DataProtectionCellJson>();
	}

	[Serializable]
	internal class DataProtectionObjectJson
	{
		public string Id;

		public bool Disabled;
	}

	[Serializable]
	internal class DataProtectionFieldJson
	{
		public string Id;

		public FieldSettingEnum Disabled;
	}

	[Serializable]
	internal class DataProtectionCellJson
	{
		public string Id;

		public List<DataProtectionObjectJson> RowsData = new List<DataProtectionObjectJson>();
	}

	private readonly Dictionary<BGId, BGModdingMetaProtection> meta2Protection = new Dictionary<BGId, BGModdingMetaProtection>();

	private readonly BGRepo repo;

	public event Action Changed;

	public BGModdingRepoProtection(BGRepo repo)
	{
		this.repo = repo;
	}

	internal DataProtectionJson ConfigToJsonObject()
	{
		DataProtectionJson dataProtectionJson = new DataProtectionJson();
		foreach (KeyValuePair<BGId, BGModdingMetaProtection> item in meta2Protection)
		{
			BGModdingMetaProtection value = item.Value;
			DataProtectionMetaJson dataProtectionMetaJson = new DataProtectionMetaJson
			{
				MetaId = item.Key.ToString(),
				DisableAdd = value.addDisabled,
				DisableDelete = value.deleteDisabled,
				DisableEdit = value.editDisabled
			};
			ListToJson(value.Fields, dataProtectionMetaJson.FieldsEdit);
			ListToJson(value.RowsEdit, dataProtectionMetaJson.RowsEdit);
			ListToJson(value.RowsDelete, dataProtectionMetaJson.RowsDelete);
			DictionaryToJson(value.Cells, dataProtectionMetaJson.CellsEdit);
			dataProtectionJson.list.Add(dataProtectionMetaJson);
		}
		return dataProtectionJson;
	}

	private void ListToJson(Dictionary<BGId, bool> ids, List<DataProtectionObjectJson> jsonList)
	{
		foreach (KeyValuePair<BGId, bool> id in ids)
		{
			jsonList.Add(new DataProtectionObjectJson
			{
				Id = id.Key.ToString(),
				Disabled = id.Value
			});
		}
	}

	private void ListToJson(Dictionary<BGId, FieldSettingEnum> ids, List<DataProtectionFieldJson> jsonList)
	{
		foreach (KeyValuePair<BGId, FieldSettingEnum> id in ids)
		{
			jsonList.Add(new DataProtectionFieldJson
			{
				Id = id.Key.ToString(),
				Disabled = id.Value
			});
		}
	}

	private void DictionaryToJson(Dictionary<BGId, Dictionary<BGId, bool>> dictionary, List<DataProtectionCellJson> targetList)
	{
		foreach (KeyValuePair<BGId, Dictionary<BGId, bool>> item in dictionary)
		{
			if (item.Value.Count == 0)
			{
				continue;
			}
			List<DataProtectionObjectJson> list = new List<DataProtectionObjectJson>(item.Value.Count);
			foreach (KeyValuePair<BGId, bool> item2 in item.Value)
			{
				list.Add(new DataProtectionObjectJson
				{
					Id = item2.Key.ToString(),
					Disabled = item2.Value
				});
			}
			if (list.Count != 0)
			{
				targetList.Add(new DataProtectionCellJson
				{
					Id = item.Key.ToString(),
					RowsData = list
				});
			}
		}
	}

	internal void ConfigJsonObject(DataProtectionJson config)
	{
		ClearMetas();
		if (config == null || config.list == null)
		{
			return;
		}
		foreach (DataProtectionMetaJson item in config.list)
		{
			if (TryParse(item.MetaId, out var id))
			{
				BGMetaEntity meta = repo.GetMeta(id);
				if (meta != null)
				{
					BGModdingMetaProtection bGModdingMetaProtection = new BGModdingMetaProtection();
					Add(id, bGModdingMetaProtection);
					bGModdingMetaProtection.addDisabled = item.DisableAdd;
					bGModdingMetaProtection.editDisabled = item.DisableEdit;
					bGModdingMetaProtection.deleteDisabled = item.DisableDelete;
					ListFromJson(bGModdingMetaProtection.Fields, item.FieldsEdit);
					ListFromJson(bGModdingMetaProtection.RowsEdit, item.RowsEdit);
					ListFromJson(bGModdingMetaProtection.RowsDelete, item.RowsDelete);
					DictionaryFromJson(bGModdingMetaProtection.Cells, item.CellsEdit);
				}
			}
		}
	}

	private void ClearMetas()
	{
		if (meta2Protection.Count > 0)
		{
			foreach (KeyValuePair<BGId, BGModdingMetaProtection> item in meta2Protection)
			{
				item.Value.Changed -= FireEvent;
			}
		}
		meta2Protection.Clear();
	}

	private void ListFromJson(Dictionary<BGId, bool> targetDict, List<DataProtectionObjectJson> jsonData)
	{
		foreach (DataProtectionObjectJson jsonDatum in jsonData)
		{
			if (TryParse(jsonDatum.Id, out var id))
			{
				targetDict[id] = jsonDatum.Disabled;
			}
		}
	}

	private void ListFromJson(Dictionary<BGId, FieldSettingEnum> targetDict, List<DataProtectionFieldJson> jsonData)
	{
		foreach (DataProtectionFieldJson jsonDatum in jsonData)
		{
			if (TryParse(jsonDatum.Id, out var id))
			{
				targetDict[id] = jsonDatum.Disabled;
			}
		}
	}

	private void DictionaryFromJson(Dictionary<BGId, Dictionary<BGId, bool>> fieldId2RowId2Disabled, List<DataProtectionCellJson> jsonData)
	{
		foreach (DataProtectionCellJson jsonDatum in jsonData)
		{
			if (jsonDatum.RowsData == null || jsonDatum.RowsData.Count == 0 || !TryParse(jsonDatum.Id, out var id))
			{
				continue;
			}
			foreach (DataProtectionObjectJson rowsDatum in jsonDatum.RowsData)
			{
				if (TryParse(rowsDatum.Id, out var id2))
				{
					Dictionary<BGId, bool> dictionary = EnsureDict(fieldId2RowId2Disabled, id);
					dictionary[id2] = rowsDatum.Disabled;
				}
			}
		}
	}

	private Dictionary<BGId, bool> EnsureDict(Dictionary<BGId, Dictionary<BGId, bool>> fieldId2RowId2Disabled, BGId fieldId)
	{
		if (fieldId2RowId2Disabled.TryGetValue(fieldId, out var value))
		{
			return value;
		}
		value = new Dictionary<BGId, bool>();
		fieldId2RowId2Disabled.Add(fieldId, value);
		FireEvent();
		return value;
	}

	private bool TryParse(string idValue, out BGId id)
	{
		id = BGId.Parse(idValue);
		return !id.IsEmpty;
	}

	public void ConfigToBytes(BGBinaryWriter writer, int version)
	{
		if (version == 2)
		{
			writer.AddArray(() =>
			{
				foreach (KeyValuePair<BGId, BGModdingMetaProtection> item in meta2Protection)
				{
					writer.AddId(item.Key);
					BGModdingMetaProtection value = item.Value;
					writer.AddBool(value.addDisabled);
					writer.AddBool(value.deleteDisabled);
					writer.AddBool(value.editDisabled);
					AddArray(writer, value.Fields);
					AddArray(writer, value.RowsEdit);
					AddArray(writer, value.RowsDelete);
					AddDict(writer, value.Cells);
				}
			}, meta2Protection.Count);
			return;
		}
		throw new BGException("unsupported version: $", version);
	}

	private void AddDict(BGBinaryWriter writer, Dictionary<BGId, Dictionary<BGId, bool>> fieldId2RowsIds)
	{
		writer.AddArray(() =>
		{
			foreach (KeyValuePair<BGId, Dictionary<BGId, bool>> fieldId2RowsId in fieldId2RowsIds)
			{
				writer.AddId(fieldId2RowsId.Key);
				Dictionary<BGId, bool> rowIds = fieldId2RowsId.Value;
				writer.AddArray(() =>
				{
					foreach (KeyValuePair<BGId, bool> item in rowIds)
					{
						writer.AddId(item.Key);
						writer.AddBool(item.Value);
					}
				}, rowIds.Count);
			}
		}, fieldId2RowsIds.Count);
	}

	private void AddArray(BGBinaryWriter writer, Dictionary<BGId, bool> collection)
	{
		writer.AddArray(() =>
		{
			foreach (KeyValuePair<BGId, bool> item in collection)
			{
				writer.AddId(item.Key);
				writer.AddBool(item.Value);
			}
		}, collection.Count);
	}

	private void AddArray(BGBinaryWriter writer, Dictionary<BGId, FieldSettingEnum> collection)
	{
		writer.AddArray(() =>
		{
			foreach (KeyValuePair<BGId, FieldSettingEnum> item in collection)
			{
				writer.AddId(item.Key);
				writer.AddInt((int)item.Value);
			}
		}, collection.Count);
	}

	public void ConfigFromBytes(BGBinaryReader reader, int version)
	{
		if (version == 2)
		{
			ClearMetas();
			reader.ReadArray(() =>
			{
				BGModdingMetaProtection bGModdingMetaProtection = new BGModdingMetaProtection();
				BGId metaId = reader.ReadId();
				Add(metaId, bGModdingMetaProtection);
				bGModdingMetaProtection.addDisabled = reader.ReadBool();
				bGModdingMetaProtection.deleteDisabled = reader.ReadBool();
				bGModdingMetaProtection.editDisabled = reader.ReadBool();
				ReadArray(reader, bGModdingMetaProtection.Fields);
				ReadArray(reader, bGModdingMetaProtection.RowsEdit);
				ReadArray(reader, bGModdingMetaProtection.RowsDelete);
				ReadDict(reader, bGModdingMetaProtection.Cells);
			});
			return;
		}
		throw new BGException("Unknown version: $", version);
	}

	private void Add(BGId metaId, BGModdingMetaProtection moddingMetaConfig)
	{
		meta2Protection[metaId] = moddingMetaConfig;
		moddingMetaConfig.Changed += FireEvent;
	}

	private void ReadDict(BGBinaryReader reader, Dictionary<BGId, Dictionary<BGId, bool>> targetDict)
	{
		reader.ReadArray(() =>
		{
			BGId key = reader.ReadId();
			if (!targetDict.TryGetValue(key, out var rowId2Disabled))
			{
				rowId2Disabled = new Dictionary<BGId, bool>();
				targetDict[key] = rowId2Disabled;
			}
			reader.ReadArray(() =>
			{
				BGId key2 = reader.ReadId();
				rowId2Disabled[key2] = reader.ReadBool();
			});
		});
	}

	private void ReadArray(BGBinaryReader reader, Dictionary<BGId, bool> dict)
	{
		reader.ReadArray(() =>
		{
			BGId key = reader.ReadId();
			dict[key] = reader.ReadBool();
		});
	}

	private void ReadArray(BGBinaryReader reader, Dictionary<BGId, FieldSettingEnum> dict)
	{
		reader.ReadArray(() =>
		{
			BGId key = reader.ReadId();
			dict[key] = (FieldSettingEnum)reader.ReadInt();
		});
	}

	public BGModdingRepoProtection CloneTo(BGRepo toRepo)
	{
		BGModdingRepoProtection bGModdingRepoProtection = new BGModdingRepoProtection(toRepo);
		foreach (KeyValuePair<BGId, BGModdingMetaProtection> item in meta2Protection)
		{
			bGModdingRepoProtection.Add(item.Key, item.Value.Clone());
		}
		return bGModdingRepoProtection;
	}

	public bool Has(BGId metaId)
	{
		return meta2Protection.ContainsKey(metaId);
	}

	public BGModdingMetaProtection Get(BGId metaId)
	{
		return BGUtil.Get(meta2Protection, metaId);
	}

	public BGModdingMetaProtection Ensure(BGId metaId)
	{
		BGModdingMetaProtection bGModdingMetaProtection = Get(metaId);
		if (bGModdingMetaProtection != null)
		{
			return bGModdingMetaProtection;
		}
		bGModdingMetaProtection = new BGModdingMetaProtection();
		Add(metaId, bGModdingMetaProtection);
		FireEvent();
		return bGModdingMetaProtection;
	}

	public bool Remove(BGId metaId)
	{
		if (!meta2Protection.TryGetValue(metaId, out var value))
		{
			return false;
		}
		value.Changed -= FireEvent;
		meta2Protection.Remove(metaId);
		FireEvent();
		return true;
	}

	private void FireEvent()
	{
		Changed?.Invoke();
	}

	public void Trim()
	{
		HashSet<BGId> hashSet = new HashSet<BGId>();
		foreach (KeyValuePair<BGId, BGModdingMetaProtection> item in meta2Protection)
		{
			BGId key = item.Key;
			BGModdingMetaProtection metaSettings = item.Value;
			BGMetaEntity meta = repo.GetMeta(key);
			if (meta == null)
			{
				hashSet.Add(key);
				continue;
			}
			RemoveIf(metaSettings.fields, (BGId id, FieldSettingEnum v) => !meta.HasField(id) || v == FieldSettingEnum.Inherited, (BGId id) =>
			{
				metaSettings.SetFieldEdit(id, FieldSettingEnum.Inherited);
			});
			RemoveIf(metaSettings.rowsEdit, (BGId id, bool v) => !meta.HasEntity(id), (BGId id) =>
			{
				metaSettings.RemoveRowsEdit(id);
			});
			RemoveIf(metaSettings.rowsDelete, (BGId id, bool v) => !meta.HasEntity(id), (BGId id) =>
			{
				metaSettings.RemoveRowsDelete(id);
			});
			HashSet<BGId> hashSet2 = new HashSet<BGId>();
			foreach (KeyValuePair<BGId, Dictionary<BGId, bool>> cell in metaSettings.cells)
			{
				BGId key2 = cell.Key;
				if (!meta.HasField(key2))
				{
					hashSet2.Add(key2);
					continue;
				}
				HashSet<BGId> hashSet3 = new HashSet<BGId>();
				foreach (KeyValuePair<BGId, bool> item2 in cell.Value)
				{
					if (!meta.HasEntity(item2.Key))
					{
						hashSet3.Add(item2.Key);
					}
				}
				foreach (BGId item3 in hashSet3)
				{
					cell.Value.Remove(item3);
				}
			}
			foreach (BGId item4 in hashSet2)
			{
				metaSettings.RemoveCellField(item4);
			}
		}
		foreach (BGId item5 in hashSet)
		{
			Remove(item5);
		}
	}

	private static void RemoveIf<T>(Dictionary<BGId, T> id2value, Func<BGId, T, bool> toRemovePredicate, Action<BGId> remove)
	{
		HashSet<BGId> hashSet = new HashSet<BGId>();
		foreach (KeyValuePair<BGId, T> item in id2value)
		{
			BGId key = item.Key;
			if (toRemovePredicate(key, item.Value))
			{
				hashSet.Add(key);
			}
		}
		foreach (BGId item2 in hashSet)
		{
			remove(item2);
		}
	}

	public bool Remove(BGId metaId, BGId fieldId, BGId entityId)
	{
		if (!meta2Protection.TryGetValue(metaId, out var value))
		{
			return false;
		}
		bool flag = false;
		if (value.cells.TryGetValue(fieldId, out var value2))
		{
			flag = value2.Remove(entityId);
		}
		if (flag)
		{
			FireEvent();
		}
		return flag;
	}

	public void AddDisabled(BGId metaId, BGId fieldId, BGId entityId)
	{
		AddDisabled(metaId, fieldId, entityId, disabled: true);
	}

	public void AddEnabled(BGId metaId, BGId fieldId, BGId entityId)
	{
		AddDisabled(metaId, fieldId, entityId, disabled: false);
	}

	private void AddDisabled(BGId metaId, BGId fieldId, BGId entityId, bool disabled)
	{
		BGModdingMetaProtection bGModdingMetaProtection = Ensure(metaId);
		Dictionary<BGId, bool> dictionary = EnsureDict(bGModdingMetaProtection.cells, fieldId);
		if (!dictionary.TryGetValue(entityId, out var value) || value != disabled)
		{
			dictionary[entityId] = disabled;
			FireEvent();
		}
	}

	public bool? Get(BGId metaId, BGId fieldId, BGId entityId)
	{
		return Get(metaId)?.Get(fieldId, entityId);
	}

	public bool? GetRowDelete(BGId metaId, BGId entityId)
	{
		return Get(metaId)?.GetRowDelete(entityId);
	}

	public bool? GetRowEdit(BGId metaId, BGId entityId)
	{
		return Get(metaId)?.GetRowEdit(entityId);
	}

	public bool AddRowDelete(BGId metaId, BGId entityId, bool disabled)
	{
		BGModdingMetaProtection bGModdingMetaProtection = Ensure(metaId);
		return bGModdingMetaProtection.AddRowDeleteDisabled(entityId, disabled);
	}

	public bool AddRowEdit(BGId metaId, BGId entityId, bool disabled)
	{
		BGModdingMetaProtection bGModdingMetaProtection = Ensure(metaId);
		return bGModdingMetaProtection.AddRowEditDisabled(entityId, disabled);
	}

	public bool RemoveRowEdit(BGId metaId, BGId entityId)
	{
		return Get(metaId)?.RemoveRowEdit(entityId) ?? false;
	}

	public bool RemoveRowDelete(BGId metaId, BGId entityId)
	{
		return Get(metaId)?.RemoveRowDelete(entityId) ?? false;
	}

	public FieldSettingEnum GetFieldEdit(BGId metaId, BGId fieldId)
	{
		return Get(metaId)?.GetFieldEdit(fieldId) ?? FieldSettingEnum.Inherited;
	}

	public bool IsAddDisabled(BGId metaId)
	{
		return Get(metaId)?.addDisabled ?? false;
	}

	public bool IsDeleteDisabled(BGId metaId, BGId entityId)
	{
		BGModdingMetaProtection bGModdingMetaProtection = Get(metaId);
		if (bGModdingMetaProtection == null)
		{
			return false;
		}
		bool? rowDelete = bGModdingMetaProtection.GetRowDelete(entityId);
		if (!rowDelete.HasValue)
		{
			return bGModdingMetaProtection.DeleteDisabled;
		}
		return rowDelete.Value;
	}

	public bool IsEditDisabled(BGId metaId, BGId fieldId, BGId entityId)
	{
		if (!meta2Protection.TryGetValue(metaId, out var value))
		{
			return false;
		}
		bool? flag = null;
		if (value.cells.TryGetValue(fieldId, out var value2) && value2.TryGetValue(entityId, out var value3))
		{
			flag = value3;
		}
		if (flag.HasValue)
		{
			return flag.Value;
		}
		if (value.fields.TryGetValue(fieldId, out var value4))
		{
			switch (value4)
			{
			case FieldSettingEnum.EnabledHigh:
				return false;
			case FieldSettingEnum.DisabledHigh:
				return true;
			}
		}
		if (value.rowsEdit.TryGetValue(entityId, out var value5))
		{
			return value5;
		}
		return value4 switch
		{
			FieldSettingEnum.Inherited => value.EditDisabled, 
			FieldSettingEnum.Enabled => false, 
			FieldSettingEnum.Disabled => true, 
			_ => throw new Exception("unexpected error at the end of IsEditDisabled"), 
		};
	}
}

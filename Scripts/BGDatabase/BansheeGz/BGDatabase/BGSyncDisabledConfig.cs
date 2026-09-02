using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[Serializable]
public class BGSyncDisabledConfig
{
	[Serializable]
	public class MetaMap
	{
		[SerializeField]
		public string MetaName;

		[SerializeField]
		public List<string> Fields;

		public MetaMap(string name)
		{
			MetaName = name;
		}

		public bool HasField(string fieldName)
		{
			if (Fields == null)
			{
				return false;
			}
			return Fields.Find((string s) => string.Equals(s, fieldName)) != null;
		}

		public void RemoveField(string fieldName)
		{
			if (Fields != null)
			{
				Fields.RemoveAll((string s) => string.Equals(s, fieldName));
			}
		}
	}

	[SerializeField]
	public List<string> IgnoreTables = new List<string>();

	[SerializeField]
	public List<MetaMap> IgnoreFields = new List<MetaMap>();

	public bool HasTable(string tableName)
	{
		if (IgnoreTables == null)
		{
			return false;
		}
		return IgnoreTables.Find((string s) => string.Equals(tableName, s)) != null;
	}

	public MetaMap GetTableWithFields(string tableName)
	{
		if (IgnoreFields == null)
		{
			return null;
		}
		return IgnoreFields.Find((MetaMap s) => string.Equals(tableName, s.MetaName));
	}

	public bool HasTableWithFields(string tableName)
	{
		return GetTableWithFields(tableName) != null;
	}

	public void SetDisabled(string sheetName, bool disabled)
	{
		if (disabled)
		{
			if (!HasTable(sheetName))
			{
				IgnoreTables = IgnoreTables ?? new List<string>();
				IgnoreTables.Add(sheetName);
			}
		}
		else if (HasTable(sheetName))
		{
			IgnoreTables.RemoveAll((string s) => string.Equals(s, sheetName));
		}
	}

	public bool HasField(string sheetName, string fieldName)
	{
		return GetTableWithFields(sheetName)?.HasField(fieldName) ?? false;
	}

	public void SetDisabled(string sheetName, string fieldName, bool disabled)
	{
		if (disabled)
		{
			if (!HasField(sheetName, fieldName))
			{
				MetaMap metaMap = GetTableWithFields(sheetName);
				if (metaMap == null)
				{
					metaMap = new MetaMap(sheetName);
					IgnoreFields = IgnoreFields ?? new List<MetaMap>();
					IgnoreFields.Add(metaMap);
				}
				metaMap.Fields = metaMap.Fields ?? new List<string>();
				metaMap.Fields.Add(fieldName);
			}
		}
		else if (HasField(sheetName, fieldName))
		{
			MetaMap tableWithFields = GetTableWithFields(sheetName);
			tableWithFields.RemoveField(fieldName);
		}
	}
}

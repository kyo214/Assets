using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public abstract class BGLiveUpdateDataSourceA
{
	public class BGLiveUpdateDataWithOrigin : BGLiveUpdateDataProcessor.BGLiveUpdateData
	{
		private readonly int[] indexes;

		private readonly int idIndex = -1;

		public int[] Indexes => indexes;

		public int IdIndex => idIndex;

		public bool HasId => idIndex >= 0;

		public BGLiveUpdateDataWithOrigin(BGMetaEntity meta, BGField[] fields, int[] indexes, int idIndex)
			: base(meta, fields)
		{
			this.indexes = indexes;
			this.idIndex = idIndex;
		}
	}

	protected BGRepo defaultRepo;

	protected BGAddonLiveUpdate addon;

	protected BGLiveUpdateLoaderA loader;

	public string LocalFileID;

	protected BGLiveUpdateDataSourceA(BGLiveUpdateContext context)
	{
		defaultRepo = context.Repo;
		addon = context.addon;
		if (context.loader != null)
		{
			loader = context.loader;
		}
		else if (context.isAsynchronous)
		{
			loader = new BGLiveUpdateLoaderUnityWebRequest(context.timeOut, context.asyncComplete);
		}
		else
		{
			loader = new BGLiveUpdateLoaderWebClient(context.timeOut);
		}
	}

	public abstract void Load(BGMetaEntity meta, BGMergeSettingsEntity actualSettings, BGLiveUpdateUrl url = null, bool applyLastDataOnFailure = false);

	public virtual void Complete()
	{
		loader.Complete();
	}

	protected void Error(BGMergeSettingsEntity actualSettings, BGMetaEntity meta, Exception ex)
	{
		string text = ex.Message ?? ("unknown error: " + ex.GetType().FullName);
		addon.Log.InvalidMetaCount++;
		addon.Log.SetError(meta.Id, new BGException(text));
		Debug.LogError("Error while loading meta " + text);
		actualSettings.ExcludeMeta(meta.Id);
	}

	protected void Error(BGMergeSettingsEntity actualSettings, BGMetaEntity meta, string error)
	{
		error = error ?? "unknown error";
		addon.Log.InvalidMetaCount++;
		BGException exception = new BGException("Error while loading meta '" + meta.Name + "': " + error);
		addon.Log.SetError(meta.Id, exception);
		if (!BGUtil.TestIsRunning)
		{
			Debug.LogException(exception);
		}
		actualSettings.ExcludeMeta(meta.Id);
	}

	protected void LogDetail(string message, params object[] parameters)
	{
		addon.Log.AddDetail(message, parameters);
	}

	protected BGLiveUpdateDataWithOrigin MapFields(BGMergeSettingsEntity actualSettings, BGMetaEntity meta, string[] fieldNames)
	{
		LogDetail("==== Mapping for '$' table started, reading headers (field names)..", meta.Name);
		if (fieldNames == null || fieldNames.Length == 0)
		{
			Error(actualSettings, meta, "No field names");
			return null;
		}
		BGIdDictionary<KeyValuePair<int, BGField>> fieldId2Column = new BGIdDictionary<KeyValuePair<int, BGField>>();
		int num = -1;
		bool idFound = false;
		for (int i = 0; i < fieldNames.Length; i++)
		{
			string text = fieldNames[i];
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text.Trim()))
			{
				BGField bGField = MapField(actualSettings, meta, text, i, ref idFound, (BGId id) => fieldId2Column.ContainsKey(id));
				if (idFound && num < 0)
				{
					num = i;
				}
				else if (bGField != null)
				{
					fieldId2Column.Add(bGField.Id, new KeyValuePair<int, BGField>(i, bGField));
					LogDetail("Index $. Header $. Field mapped ok.", i, text);
				}
			}
		}
		if (num < 0)
		{
			LogDetail("no _id column- all rows will be considered as new ones!");
		}
		if (fieldId2Column.Count == 0)
		{
			LogDetail("No field was mapped! Fields are resolved by name (from the first row). You need at least one field to be mapped properly. Aborting..");
			return null;
		}
		BGField[] array = new BGField[fieldId2Column.Count];
		int[] array2 = new int[fieldId2Column.Count];
		int num2 = 0;
		foreach (KeyValuePair<BGId, KeyValuePair<int, BGField>> item in fieldId2Column)
		{
			array2[num2] = item.Value.Key;
			array[num2] = item.Value.Value;
			num2++;
		}
		BGLiveUpdateDataWithOrigin result = new BGLiveUpdateDataWithOrigin(meta, array, array2, num);
		LogDetail("==== Mapping for '$' table ended.", meta.Name);
		return result;
	}

	private BGField MapField(BGMergeSettingsEntity actualSettings, BGMetaEntity meta, string fieldName, int index, ref bool idFound, Func<BGId, bool> checkDuplicate)
	{
		if ("_id".Equals(fieldName) && !idFound)
		{
			idFound = true;
			LogDetail("Index $. Id column mapped ok.", index);
			return null;
		}
		BGField field = meta.GetField(fieldName, errorIfNotFound: false);
		if (field == null)
		{
			LogDetail("Index $. Header $. Field with such name can not be found or not included in settings . Skipping..", index, fieldName);
			return null;
		}
		if (checkDuplicate(field.Id))
		{
			LogDetail("Index $. Header $. Duplicate (the same name was already mapped). Skipping..", index, fieldName);
			return null;
		}
		if (!actualSettings.IsFieldIncluded(field))
		{
			LogDetail("Index $. Header $. Field is not included into settings. Skipping..", index, fieldName);
			return null;
		}
		return field;
	}

	protected BGId ReadId(string idString, int index)
	{
		if (index < 0)
		{
			return BGId.Empty;
		}
		if (idString == null || idString.Trim().Equals(string.Empty))
		{
			return BGId.Empty;
		}
		try
		{
			return new BGId(idString);
		}
		catch (Exception)
		{
			LogDetail("id value is invalid ($) for index $! This row will be considered as new one", idString, index);
			return BGId.Empty;
		}
	}
}

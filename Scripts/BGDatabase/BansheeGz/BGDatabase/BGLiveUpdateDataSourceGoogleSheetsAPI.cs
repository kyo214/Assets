using System;
using System.IO;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGLiveUpdateDataSourceGoogleSheetsAPI : BGLiveUpdateDataSourceA
{
	public BGLiveUpdateDataSourceGoogleSheetsAPI(BGLiveUpdateContext context)
		: base(context)
	{
	}

	protected string GetUrl(string metaName)
	{
		return "https://sheets.googleapis.com/v4/spreadsheets/" + addon.SpreadsheetId + "/values/" + metaName + "?key=" + addon.ApiKey;
	}

	public override void Load(BGMetaEntity meta, BGMergeSettingsEntity actualSettings, BGLiveUpdateUrl url = null, bool applyLastDataOnFailure = false)
	{
		BGLiveUpdateLoaderA.LoadContext loadContext = ((url == null || url.URL == null) ? new BGLiveUpdateLoaderA.LoadContext(GetUrl(meta.Name), addon.Log) : new BGLiveUpdateLoaderA.LoadContext(url.URL, addon.Log, url.HttpMethod, url.HttpParametersAsTuples, url.HttpHeadersAsTuples));
		if (applyLastDataOnFailure)
		{
			loadContext.LocalFileName = Path.ChangeExtension("BGD_LU_" + (string.IsNullOrEmpty(LocalFileID) ? "GH_" : (LocalFileID + "_")) + BGAddonPartition.ToFilePath(meta.Id), "json");
		}
		loader.Load(loadContext, (BGLiveUpdateLoaderA.LoadResultText result) =>
		{
			LoaderCallback(meta, actualSettings, result);
		});
	}

	private void LoaderCallback(BGMetaEntity meta, BGMergeSettingsEntity actualSettings, BGLiveUpdateLoaderA.LoadResultText result)
	{
		try
		{
			BGLiveUpdateDataProcessor.BGLiveUpdateData bGLiveUpdateData = ParseJson(meta, actualSettings, result);
			if (bGLiveUpdateData != null)
			{
				new BGLiveUpdateDataProcessor(addon, defaultRepo).Process(bGLiveUpdateData);
				addon.Log.OkMetaCount++;
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
			Error(actualSettings, meta, ex);
		}
	}

	private BGLiveUpdateDataProcessor.BGLiveUpdateData ParseJson(BGMetaEntity meta, BGMergeSettingsEntity actualSettings, BGLiveUpdateLoaderA.LoadResultText result)
	{
		if (result.IsError)
		{
			Error(actualSettings, meta, result.Error);
			return null;
		}
		if (string.IsNullOrEmpty(result.Result))
		{
			Error(actualSettings, meta, "Loaded result is null");
			return null;
		}
		JSONNode jSONNode = JSON.Parse(result.Result);
		if (jSONNode == null)
		{
			Error(actualSettings, meta, "Parsed JSON object is null");
			return null;
		}
		JSONNode jSONNode2 = jSONNode["values"];
		if (jSONNode2 == null || !jSONNode2.IsArray)
		{
			Error(actualSettings, meta, "Values are null");
			return null;
		}
		JSONArray jSONArray = jSONNode2 as JSONArray;
		if (jSONArray == null || jSONArray.Count < 2)
		{
			Error(actualSettings, meta, "No values in json feed");
			return null;
		}
		JSONNode jSONNode3 = jSONArray[0];
		string[] array = null;
		if (jSONNode3 != null && jSONNode3.Count > 0)
		{
			array = new string[jSONNode3.Count];
			for (int i = 0; i < jSONNode3.Count; i++)
			{
				array[i] = jSONNode3[i];
			}
		}
		BGLiveUpdateDataWithOrigin bGLiveUpdateDataWithOrigin = MapFields(actualSettings, meta, array);
		if (bGLiveUpdateDataWithOrigin == null)
		{
			return null;
		}
		LogDetail("==== Reading $ rows..", jSONArray.Count - 1);
		for (int j = 1; j < jSONArray.Count; j++)
		{
			JSONNode jSONNode4 = jSONArray[j];
			if (!jSONNode4.IsArray)
			{
				LogDetail("WARNING! json values with index $ are not array!", j);
				continue;
			}
			JSONArray jSONArray2 = jSONNode4 as JSONArray;
			if (jSONArray2 == null)
			{
				LogDetail("WARNING! json values with index $ can not be cast to JSONArray!", j);
				continue;
			}
			if (jSONArray2.Count == 0)
			{
				LogDetail("WARNING! json values array with index $ has no values! skipping this row..", j);
				continue;
			}
			BGId entityId = BGId.Empty;
			if (Get(jSONArray2, bGLiveUpdateDataWithOrigin.IdIndex, out var val))
			{
				entityId = ReadId(val, bGLiveUpdateDataWithOrigin.IdIndex);
			}
			string[] array2 = new string[bGLiveUpdateDataWithOrigin.Fields.Length];
			for (int k = 0; k < bGLiveUpdateDataWithOrigin.Fields.Length; k++)
			{
				BGField bGField = bGLiveUpdateDataWithOrigin.Fields[k];
				if (!Get(jSONArray2, bGLiveUpdateDataWithOrigin.Indexes[k], out var val2))
				{
					LogDetail("WARNING! can not get field value (field=$), index $!", bGField.Name, j);
				}
				else
				{
					array2[k] = val2;
				}
			}
			bGLiveUpdateDataWithOrigin.Add(entityId, array2, addon.Log, j);
		}
		LogDetail("==== Rows are read.");
		LogDetail("======== Meta $ loaded.", meta.Name);
		return bGLiveUpdateDataWithOrigin;
	}

	private static bool Get(JSONArray row, int index, out string val)
	{
		val = null;
		if (index < 0 || index >= row.Count)
		{
			return false;
		}
		JSONNode jSONNode = row[index];
		if (jSONNode == null || !jSONNode.IsString)
		{
			return false;
		}
		val = (jSONNode as JSONString).Value;
		return true;
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGLiveUpdateDataSourceChartsAPI : BGLiveUpdateDataSourceA
{
	public BGLiveUpdateDataSourceChartsAPI(BGLiveUpdateContext context)
		: base(context)
	{
	}

	private string GetUrl(string metaName)
	{
		return "https://docs.google.com/spreadsheets/d/" + addon.SpreadsheetId + "/gviz/tq?tqx=out:csv&sheet=" + metaName;
	}

	public override void Load(BGMetaEntity meta, BGMergeSettingsEntity actualSettings, BGLiveUpdateUrl url = null, bool applyLastDataOnFailure = false)
	{
		BGLiveUpdateLoaderA.LoadContext loadContext = ((url == null || url.URL == null) ? new BGLiveUpdateLoaderA.LoadContext(GetUrl(meta.Name), addon.Log) : new BGLiveUpdateLoaderA.LoadContext(url.URL, addon.Log, url.HttpMethod, url.HttpParametersAsTuples, url.HttpHeadersAsTuples));
		if (applyLastDataOnFailure)
		{
			loadContext.LocalFileName = Path.ChangeExtension("BGD_LU_" + (string.IsNullOrEmpty(LocalFileID) ? "CHARTS_" : (LocalFileID + "_")) + BGAddonPartition.ToFilePath(meta.Id), "csv");
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
			BGLiveUpdateDataProcessor.BGLiveUpdateData bGLiveUpdateData = ParseCsv(meta, actualSettings, result);
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

	private BGLiveUpdateDataProcessor.BGLiveUpdateData ParseCsv(BGMetaEntity meta, BGMergeSettingsEntity actualSettings, BGLiveUpdateLoaderA.LoadResultText result)
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
		if (result.Result.StartsWith("<") && result.Result.IndexOf("><html", 0, 24, StringComparison.OrdinalIgnoreCase) != -1)
		{
			Error(actualSettings, meta, "It looks like the result is not a valid CSV content. Try to open Visualization API URL in your browser (the URL can be found here: https://www.bansheegz.com/BGDatabase/Addons/LiveUpdate ). ");
			return null;
		}
		using CsvFileReader csvFileReader = new CsvFileReader(new MemoryStream(Encoding.UTF8.GetBytes(result.Result)));
		List<string> list = new List<string>();
		if (!csvFileReader.ReadRow(list) || list.Count == 0)
		{
			Error(actualSettings, meta, "Loaded csv data does not have field names");
			return null;
		}
		BGLiveUpdateDataWithOrigin bGLiveUpdateDataWithOrigin = MapFields(actualSettings, meta, list.ToArray());
		if (bGLiveUpdateDataWithOrigin == null)
		{
			return null;
		}
		list.Clear();
		int num = 1;
		while (csvFileReader.ReadRow(list))
		{
			BGId entityId = ((bGLiveUpdateDataWithOrigin.HasId && bGLiveUpdateDataWithOrigin.IdIndex < list.Count) ? ReadId(list[bGLiveUpdateDataWithOrigin.IdIndex], bGLiveUpdateDataWithOrigin.IdIndex) : BGId.Empty);
			string[] array = new string[bGLiveUpdateDataWithOrigin.Fields.Length];
			for (int i = 0; i < bGLiveUpdateDataWithOrigin.Fields.Length; i++)
			{
				array[i] = list[bGLiveUpdateDataWithOrigin.Indexes[i]];
			}
			bGLiveUpdateDataWithOrigin.Add(entityId, array, addon.Log, num);
			list.Clear();
			num++;
		}
		return bGLiveUpdateDataWithOrigin;
	}
}

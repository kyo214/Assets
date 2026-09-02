using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGLiveUpdateDataSourceExcelExport(BGLiveUpdateContext context) : BGLiveUpdateDataSourceA(context)
{
	public interface LiveUpdateExcelParser
	{
		LiveUpdateExcelData Parse(byte[] data, bool UseXml, BGMetaEntity[] metas);
	}

	public class LiveUpdateExcelData
	{
		public LiveUpdateExcelSheet[] sheets;
	}

	public class LiveUpdateExcelSheet
	{
		public string Name;

		public string[,] Data;
	}

	public const string ParserTypeName = "BansheeGz.BGDatabase.BGLiveUpdateExcelParser";

	private readonly List<BGMetaEntity> metas = new List<BGMetaEntity>();

	private BGMergeSettingsEntity actualSettings;

	private string url;

	private string localFileName;

	private LiveUpdateExcelParser Parser
	{
		get
		{
			Type type = BGUtil.GetType("BansheeGz.BGDatabase.BGLiveUpdateExcelParser");
			if (type == null)
			{
				return null;
			}
			return Activator.CreateInstance(type) as LiveUpdateExcelParser;
		}
	}

	public static string GetUrl(string spreadSheetId)
	{
		return "https://docs.google.com/spreadsheets/d/" + spreadSheetId + "/export";
	}

	public override void Load(BGMetaEntity meta, BGMergeSettingsEntity actualSettings, BGLiveUpdateUrl url = null, bool applyLastDataOnFailure = false)
	{
		this.actualSettings = actualSettings;
		if (url != null && url.URL != null)
		{
			this.url = url.URL;
		}
		if (applyLastDataOnFailure)
		{
			localFileName = Path.ChangeExtension("BGD_LU_" + (string.IsNullOrEmpty(LocalFileID) ? "EXCEL" : LocalFileID), "xlsx");
		}
		metas.Add(meta);
	}

	public override void Complete()
	{
		if (metas.Count != 0)
		{
			loader.Load(new BGLiveUpdateLoaderA.LoadContext(url ?? GetUrl(addon.SpreadsheetId), addon.Log)
			{
				LocalFileName = localFileName
			}, Loaded);
		}
		base.Complete();
	}

	private void Loaded(BGLiveUpdateLoaderA.LoadResultBinary result)
	{
		if (result.IsError)
		{
			MarkForError(result.Error);
			return;
		}
		if (result.Result == null || result.Result.Length == 0)
		{
			MarkForError("Loaded result is null");
			return;
		}
		LiveUpdateExcelParser parser = Parser;
		if (parser == null)
		{
			MarkForError("Can not create Excel parser of type BansheeGz.BGDatabase.BGLiveUpdateExcelParser. You need to download it at addon's page https://www.bansheegz.com/BGDatabase/Addons/LiveUpdate/");
			return;
		}
		LiveUpdateExcelData liveUpdateExcelData;
		try
		{
			liveUpdateExcelData = parser.Parse(result.Result, UseXml: true, metas.ToArray());
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
			MarkForError("Can not parse Excel file: " + ex.Message);
			return;
		}
		if (liveUpdateExcelData == null || liveUpdateExcelData.sheets == null || liveUpdateExcelData.sheets.Length == 0)
		{
			MarkForError("Excel file does not have any useful information");
			return;
		}
		Dictionary<string, BGMetaEntity> dictionary = new Dictionary<string, BGMetaEntity>();
		foreach (BGMetaEntity meta in metas)
		{
			dictionary.Add(meta.Name, meta);
		}
		LiveUpdateExcelSheet[] sheets = liveUpdateExcelData.sheets;
		foreach (LiveUpdateExcelSheet liveUpdateExcelSheet in sheets)
		{
			if (!dictionary.TryGetValue(liveUpdateExcelSheet.Name, out var value))
			{
				continue;
			}
			dictionary.Remove(liveUpdateExcelSheet.Name);
			string[,] data = liveUpdateExcelSheet.Data;
			if (data == null)
			{
				Error(actualSettings, value, "No data for meta " + value.Name);
				return;
			}
			int length = data.GetLength(0);
			int length2 = data.GetLength(1);
			if (length < 2 || length2 < 2)
			{
				Error(actualSettings, value, "No data for meta " + value.Name);
				return;
			}
			BGLiveUpdateDataWithOrigin bGLiveUpdateDataWithOrigin = MapFields(actualSettings, value, GetRow(data, 0));
			if (bGLiveUpdateDataWithOrigin == null)
			{
				continue;
			}
			bool flag = bGLiveUpdateDataWithOrigin.HasId && bGLiveUpdateDataWithOrigin.IdIndex < length2;
			for (int j = 1; j < length; j++)
			{
				BGId entityId = (flag ? ReadId(data[j, bGLiveUpdateDataWithOrigin.IdIndex], bGLiveUpdateDataWithOrigin.IdIndex) : BGId.Empty);
				string[] array = new string[bGLiveUpdateDataWithOrigin.Fields.Length];
				for (int k = 0; k < bGLiveUpdateDataWithOrigin.Fields.Length; k++)
				{
					array[k] = data[j, bGLiveUpdateDataWithOrigin.Indexes[k]];
				}
				bGLiveUpdateDataWithOrigin.Add(entityId, array, addon.Log, j);
			}
			try
			{
				new BGLiveUpdateDataProcessor(addon, defaultRepo).Process(bGLiveUpdateDataWithOrigin);
				addon.Log.OkMetaCount++;
			}
			catch (Exception ex2)
			{
				Debug.LogException(ex2);
				Error(actualSettings, value, ex2);
			}
		}
		foreach (KeyValuePair<string, BGMetaEntity> item in dictionary)
		{
			Error(actualSettings, item.Value, "No data for meta " + item.Value.Name);
		}
	}

	private string[] GetRow(string[,] sheetData, int row)
	{
		int length = sheetData.GetLength(1);
		string[] array = new string[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = sheetData[row, i];
		}
		return array;
	}

	private void MarkForError(string resultError)
	{
		foreach (BGMetaEntity meta in metas)
		{
			Error(actualSettings, meta, resultError);
		}
	}
}

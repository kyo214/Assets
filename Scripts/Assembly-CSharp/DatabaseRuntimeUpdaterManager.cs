using BansheeGz.BGDatabase;
using Database.Tools;
using UnityEngine;

public class DatabaseRuntimeUpdaterManager : MonoBehaviour
{
	private enum UpdateType
	{
		Excel = 0,
		GoogleSheets = 1
	}

	[SerializeField]
	private DatabaseUpdaterConfig _databaseUpdaterConfig;

	[SerializeField]
	private string _iniFileName = "DatabaseSettings";

	[SerializeField]
	private BGExcelImportGo _excelImportManager;

	[SerializeField]
	private UpdateType _updateType;

	private void Awake()
	{
		GetDatabaseSettings();
		UpdateDatabase();
	}

	private void UpdateDatabase()
	{
		UpdateDatabase(_updateType);
		_databaseUpdaterConfig.UpdateLibrary();
	}

	private void UpdateDatabase(UpdateType type)
	{
		switch (type)
		{
		case UpdateType.Excel:
			_excelImportManager.Import();
			break;
		case UpdateType.GoogleSheets:
			BGAddonLiveUpdate.LoadDefault();
			break;
		}
	}

	private void GetDatabaseSettings()
	{
		IniParser iniParser = new IniParser();
		if (iniParser.DoesExist(Application.streamingAssetsPath, _iniFileName))
		{
			iniParser.Load(Application.streamingAssetsPath, _iniFileName);
		}
		else
		{
			iniParser = CreateDefaultIniFile();
			iniParser.Save(_iniFileName, Application.streamingAssetsPath);
		}
		_updateType = (UpdateType)int.Parse(iniParser.Get("DatabaseSettings", "UpdateType"));
		_excelImportManager.ExcelFile = iniParser.Get("ExcelSettings", "ExcelPath");
		bool.TryParse(iniParser.Get("ExcelSettings", "DisableGUI"), out _excelImportManager.DisableGUI);
	}

	private IniParser CreateDefaultIniFile()
	{
		IniParser iniParser = new IniParser();
		iniParser.Set("DatabaseSettings", "UpdateType", "0");
		iniParser.Set("ExcelSettings", "ExcelPath", "WMODatabase.xlsx");
		iniParser.Set("ExcelSettings", "DisableGUI", "true");
		return iniParser;
	}
}

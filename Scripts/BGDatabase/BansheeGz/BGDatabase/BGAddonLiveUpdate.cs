using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[AddonDescriptor(Name = "LiveUpdate", ManagerType = "BansheeGz.BGDatabase.Editor.BGAddonManagerLiveUpdate")]
public class BGAddonLiveUpdate : BGAddon
{
	public enum DataSourceTypeEnum
	{
		GoogleSheetsAPI = 0,
		VisualizationAPI = 1,
		ExcelExport = 2
	}

	[Serializable]
	private class Settings
	{
		public BGMergeSettingsEntity MergeSettings = new BGMergeSettingsEntity();

		public string SpreadsheetId;

		public string ApiKey;

		public bool ManualLoad;

		public float Timeout;

		public bool InBuildOnly;

		public bool PrintLogOnLoad;

		public int LogLevel;

		public string ValueResolverType;

		public int DataSourceType;

		public bool ForceAsynchronous;

		public BGLiveUpdateSourceTypeEnum sourceType;

		public BGLiveUpdateUrls Urls;

		public bool ApplyLastOnFailure;
	}

	public const float MinTimeout = 1f;

	public const float MaxTimeout = 30f;

	private const float DefaultTimeout = 5f;

	private BGMergeSettingsEntity mergeSettings = new BGMergeSettingsEntity();

	private BGLiveUpdateSourceTypeEnum sourceType;

	private BGLiveUpdateUrls urls;

	private string spreadsheetId;

	private string apiKey;

	private bool manualLoad;

	private float timeout = 5f;

	private bool inBuildOnly;

	private BGLiveUpdateLog.LogLevelEnum logLevel;

	private bool printLogOnLoad;

	private BGLiveUpdateLog log;

	private BGLiveUpdateValueResolver valueResolver;

	private string valueResolverType;

	private bool valueResolverLoadTried;

	private DataSourceTypeEnum dataSourceType;

	private bool forceAsynchronous;

	private bool applyLastOnFailure;

	private static bool loadTried;

	private static BGRepo remoteRepo;

	private static BGMergeSettingsEntity actualSettings;

	public static bool SuppressLoading { get; set; }

	public BGMergeSettingsEntity MergeSettings => mergeSettings;

	public string SpreadsheetId
	{
		get
		{
			return spreadsheetId;
		}
		set
		{
			if (!string.Equals(spreadsheetId, value))
			{
				spreadsheetId = value;
				FireChange();
			}
		}
	}

	public string ApiKey
	{
		get
		{
			return apiKey;
		}
		set
		{
			if (!string.Equals(apiKey, value))
			{
				apiKey = value;
				FireChange();
			}
		}
	}

	public bool ManualLoad
	{
		get
		{
			return manualLoad;
		}
		set
		{
			if (manualLoad != value)
			{
				manualLoad = value;
				FireChange();
			}
		}
	}

	public float Timeout
	{
		get
		{
			return timeout;
		}
		set
		{
			if (!((double)Math.Abs(timeout - value) < 0.001) && !(value < 1f) && !(value > 30f))
			{
				timeout = value;
				FireChange();
			}
		}
	}

	public bool InBuildOnly
	{
		get
		{
			return inBuildOnly;
		}
		set
		{
			if (inBuildOnly != value)
			{
				inBuildOnly = value;
				FireChange();
			}
		}
	}

	public BGLiveUpdateLog.LogLevelEnum LogLevel
	{
		get
		{
			return logLevel;
		}
		set
		{
			if (logLevel != value)
			{
				logLevel = value;
				FireChange();
			}
		}
	}

	public bool PrintLogOnLoad
	{
		get
		{
			return printLogOnLoad;
		}
		set
		{
			if (printLogOnLoad != value)
			{
				printLogOnLoad = value;
				FireChange();
			}
		}
	}

	public BGLiveUpdateSourceTypeEnum SourceType
	{
		get
		{
			return sourceType;
		}
		set
		{
			if (sourceType != value)
			{
				sourceType = value;
				FireChange();
			}
		}
	}

	public BGLiveUpdateLog Log
	{
		get
		{
			if (log != null)
			{
				return log;
			}
			log = new BGLiveUpdateLog(logLevel);
			return log;
		}
	}

	private bool IsPlaying
	{
		get
		{
			if (!Application.isPlaying)
			{
				return BGUtil.TestIsRunning;
			}
			return true;
		}
	}

	public bool IsLoadingOnStartInEditor
	{
		get
		{
			if (!ManualLoad)
			{
				return !InBuildOnly;
			}
			return false;
		}
	}

	public DataSourceTypeEnum DataSourceType
	{
		get
		{
			return dataSourceType;
		}
		set
		{
			if (dataSourceType != value)
			{
				dataSourceType = value;
				FireChange();
			}
		}
	}

	public bool ForceAsynchronous
	{
		get
		{
			return forceAsynchronous;
		}
		set
		{
			if (forceAsynchronous != value)
			{
				forceAsynchronous = value;
				FireChange();
			}
		}
	}

	public string ValueResolverType
	{
		get
		{
			return valueResolverType;
		}
		set
		{
			if (!string.Equals(valueResolverType, value))
			{
				valueResolverType = value;
				valueResolverLoadTried = false;
				valueResolver = null;
				FireChange();
			}
		}
	}

	public BGLiveUpdateUrls Urls
	{
		get
		{
			if (urls == null)
			{
				urls = new BGLiveUpdateUrls(this);
			}
			return urls;
		}
	}

	public BGLiveUpdateValueResolver ValueResolver
	{
		get
		{
			if (valueResolver == null && !string.IsNullOrEmpty(valueResolverType) && !valueResolverLoadTried)
			{
				valueResolverLoadTried = true;
				try
				{
					valueResolver = BGUtil.Create<BGLiveUpdateValueResolver>(valueResolverType, includePrivateConstructors: false, Array.Empty<object>());
					if (valueResolver == null)
					{
						throw new Exception("Can not create value resolver with type " + valueResolverType);
					}
				}
				catch (Exception exception)
				{
					Debug.Log("Can not create value resolver with type " + valueResolverType);
					Debug.LogException(exception);
				}
			}
			return valueResolver;
		}
		set
		{
			valueResolver = value;
		}
	}

	private bool IsAsynchronous => Application.platform == RuntimePlatform.WebGLPlayer;

	public string Error
	{
		get
		{
			switch (SourceType)
			{
			case BGLiveUpdateSourceTypeEnum.GoogleSheets:
				if (string.IsNullOrEmpty(SpreadsheetId))
				{
					return "SpreadsheetId is not set";
				}
				if (DataSourceType == DataSourceTypeEnum.ExcelExport)
				{
					Type type = BGUtil.GetType("BansheeGz.BGDatabase.BGLiveUpdateExcelParser");
					if (type == null)
					{
						return "ExcelExport DataSource type requires additional setup. See docs page for more details: https://www.bansheegz.com/BGDatabase/Addons/LiveUpdate/";
					}
				}
				break;
			case BGLiveUpdateSourceTypeEnum.WebServer:
			{
				if (Urls.urls == null || Urls.urls.Count == 0)
				{
					return "URLs are empty! Add at least one URL";
				}
				for (int i = 0; i < Urls.urls.Count; i++)
				{
					BGLiveUpdateUrl bGLiveUpdateUrl = Urls.urls[i];
					if (string.IsNullOrEmpty(bGLiveUpdateUrl.URL))
					{
						return "URL is not set for [" + i + "] record (zero-based)";
					}
					if (string.IsNullOrEmpty(bGLiveUpdateUrl.MetaId))
					{
						return "Meta is not set for [" + i + "] record (zero-based)";
					}
					if (!BGRepo.I.HasMeta(BGId.Parse(bGLiveUpdateUrl.MetaId)))
					{
						return "Repo does not have meta with id [" + bGLiveUpdateUrl.MetaId + "]! record # " + i + " (zero-based)";
					}
				}
				break;
			}
			default:
				throw new ArgumentOutOfRangeException("addon.SourceType");
			}
			if (!MergeSettings.HasAny(BGRepo.I))
			{
				return "No tables are included to merge settings";
			}
			return null;
		}
	}

	public bool ApplyLastOnFailure
	{
		get
		{
			return applyLastOnFailure;
		}
		set
		{
			if (applyLastOnFailure != value)
			{
				applyLastOnFailure = value;
				FireChange();
			}
		}
	}

	public static BGMergeSettingsEntity ActualSettings => actualSettings;

	public event Action OnLoadComplete;

	public BGAddonLiveUpdate()
	{
		mergeSettings.OnChange += SettingsChanged;
	}

	public override string ConfigToString()
	{
		return JsonUtility.ToJson(new Settings
		{
			SpreadsheetId = spreadsheetId,
			ApiKey = apiKey,
			MergeSettings = mergeSettings,
			ManualLoad = manualLoad,
			Timeout = timeout,
			InBuildOnly = inBuildOnly,
			PrintLogOnLoad = printLogOnLoad,
			LogLevel = (int)logLevel,
			ValueResolverType = valueResolverType,
			DataSourceType = (int)dataSourceType,
			ForceAsynchronous = forceAsynchronous,
			Urls = urls,
			sourceType = sourceType,
			ApplyLastOnFailure = applyLastOnFailure
		});
	}

	public override void ConfigFromString(string config)
	{
		Settings settings = JsonUtility.FromJson<Settings>(config);
		apiKey = settings.ApiKey;
		spreadsheetId = settings.SpreadsheetId;
		mergeSettings = settings.MergeSettings;
		manualLoad = settings.ManualLoad;
		timeout = ((settings.Timeout < 1f || settings.Timeout > 30f) ? 5f : settings.Timeout);
		inBuildOnly = settings.InBuildOnly;
		printLogOnLoad = settings.PrintLogOnLoad;
		logLevel = (BGLiveUpdateLog.LogLevelEnum)settings.LogLevel;
		valueResolverType = settings.ValueResolverType;
		dataSourceType = (DataSourceTypeEnum)settings.DataSourceType;
		forceAsynchronous = settings.ForceAsynchronous;
		urls = settings.Urls;
		if (urls != null)
		{
			urls.Addon = this;
		}
		sourceType = settings.sourceType;
		applyLastOnFailure = settings.ApplyLastOnFailure;
		mergeSettings.OnChange += SettingsChanged;
	}

	public override byte[] ConfigToBytes()
	{
		byte[] value = mergeSettings.ConfigToBytes();
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter(4 + BGBinaryWriter.GetBytesCount(apiKey) + BGBinaryWriter.GetBytesCount(spreadsheetId) + BGBinaryWriter.GetBytesCount(value));
		bGBinaryWriter.AddInt(8);
		bGBinaryWriter.AddString(apiKey);
		bGBinaryWriter.AddString(spreadsheetId);
		bGBinaryWriter.AddByteArray(value);
		bGBinaryWriter.AddBool(manualLoad);
		bGBinaryWriter.AddFloat(timeout);
		bGBinaryWriter.AddBool(inBuildOnly);
		bGBinaryWriter.AddInt((int)logLevel);
		bGBinaryWriter.AddBool(printLogOnLoad);
		bGBinaryWriter.AddString(valueResolverType);
		bGBinaryWriter.AddInt((int)dataSourceType);
		bGBinaryWriter.AddBool(forceAsynchronous);
		bGBinaryWriter.AddInt((int)sourceType);
		bGBinaryWriter.AddByteArray(Urls.ConfigToBytes());
		bGBinaryWriter.AddBool(applyLastOnFailure);
		return bGBinaryWriter.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		BGBinaryReader bGBinaryReader = new BGBinaryReader(config);
		int num = bGBinaryReader.ReadInt();
		switch (num)
		{
		case 1:
			apiKey = bGBinaryReader.ReadString();
			spreadsheetId = bGBinaryReader.ReadString();
			mergeSettings.ConfigFromBytes(bGBinaryReader.ReadByteArray());
			break;
		case 2:
			apiKey = bGBinaryReader.ReadString();
			spreadsheetId = bGBinaryReader.ReadString();
			mergeSettings.ConfigFromBytes(bGBinaryReader.ReadByteArray());
			manualLoad = bGBinaryReader.ReadBool();
			break;
		case 3:
			apiKey = bGBinaryReader.ReadString();
			spreadsheetId = bGBinaryReader.ReadString();
			mergeSettings.ConfigFromBytes(bGBinaryReader.ReadByteArray());
			manualLoad = bGBinaryReader.ReadBool();
			timeout = bGBinaryReader.ReadFloat();
			inBuildOnly = bGBinaryReader.ReadBool();
			break;
		case 4:
			apiKey = bGBinaryReader.ReadString();
			spreadsheetId = bGBinaryReader.ReadString();
			mergeSettings.ConfigFromBytes(bGBinaryReader.ReadByteArray());
			manualLoad = bGBinaryReader.ReadBool();
			timeout = bGBinaryReader.ReadFloat();
			inBuildOnly = bGBinaryReader.ReadBool();
			logLevel = (BGLiveUpdateLog.LogLevelEnum)bGBinaryReader.ReadInt();
			printLogOnLoad = bGBinaryReader.ReadBool();
			break;
		case 5:
			apiKey = bGBinaryReader.ReadString();
			spreadsheetId = bGBinaryReader.ReadString();
			mergeSettings.ConfigFromBytes(bGBinaryReader.ReadByteArray());
			manualLoad = bGBinaryReader.ReadBool();
			timeout = bGBinaryReader.ReadFloat();
			inBuildOnly = bGBinaryReader.ReadBool();
			logLevel = (BGLiveUpdateLog.LogLevelEnum)bGBinaryReader.ReadInt();
			printLogOnLoad = bGBinaryReader.ReadBool();
			valueResolverType = bGBinaryReader.ReadString();
			break;
		case 6:
			apiKey = bGBinaryReader.ReadString();
			spreadsheetId = bGBinaryReader.ReadString();
			mergeSettings.ConfigFromBytes(bGBinaryReader.ReadByteArray());
			manualLoad = bGBinaryReader.ReadBool();
			timeout = bGBinaryReader.ReadFloat();
			inBuildOnly = bGBinaryReader.ReadBool();
			logLevel = (BGLiveUpdateLog.LogLevelEnum)bGBinaryReader.ReadInt();
			printLogOnLoad = bGBinaryReader.ReadBool();
			valueResolverType = bGBinaryReader.ReadString();
			dataSourceType = (DataSourceTypeEnum)bGBinaryReader.ReadInt();
			forceAsynchronous = bGBinaryReader.ReadBool();
			break;
		case 7:
		case 8:
			apiKey = bGBinaryReader.ReadString();
			spreadsheetId = bGBinaryReader.ReadString();
			mergeSettings.ConfigFromBytes(bGBinaryReader.ReadByteArray());
			manualLoad = bGBinaryReader.ReadBool();
			timeout = bGBinaryReader.ReadFloat();
			inBuildOnly = bGBinaryReader.ReadBool();
			logLevel = (BGLiveUpdateLog.LogLevelEnum)bGBinaryReader.ReadInt();
			printLogOnLoad = bGBinaryReader.ReadBool();
			valueResolverType = bGBinaryReader.ReadString();
			dataSourceType = (DataSourceTypeEnum)bGBinaryReader.ReadInt();
			forceAsynchronous = bGBinaryReader.ReadBool();
			sourceType = (BGLiveUpdateSourceTypeEnum)bGBinaryReader.ReadInt();
			Urls.ConfigFromBytes(bGBinaryReader.ReadByteArray());
			urls.Addon = this;
			if (num == 8)
			{
				applyLastOnFailure = bGBinaryReader.ReadBool();
			}
			break;
		default:
			throw new BGException("Unknown version: $", num);
		}
	}

	public override BGAddon CloneTo(BGRepo repo)
	{
		BGAddonLiveUpdate bGAddonLiveUpdate = new BGAddonLiveUpdate
		{
			Repo = repo,
			mergeSettings = (BGMergeSettingsEntity)mergeSettings.Clone(),
			apiKey = apiKey,
			spreadsheetId = spreadsheetId,
			manualLoad = manualLoad,
			timeout = timeout,
			inBuildOnly = inBuildOnly,
			logLevel = logLevel,
			printLogOnLoad = printLogOnLoad,
			valueResolverType = valueResolverType,
			dataSourceType = dataSourceType,
			forceAsynchronous = forceAsynchronous,
			sourceType = sourceType,
			applyLastOnFailure = applyLastOnFailure
		};
		bGAddonLiveUpdate.urls = urls?.CloneTo(bGAddonLiveUpdate);
		bGAddonLiveUpdate.mergeSettings.OnChange += bGAddonLiveUpdate.SettingsChanged;
		return bGAddonLiveUpdate;
	}

	private void SettingsChanged()
	{
		FireChange();
	}

	public override void OnLoad()
	{
		if (BGRepo.DefaultRepo(Repo) && !manualLoad)
		{
			Load();
		}
	}

	public void Load(bool allowToCacheLoadResult = false)
	{
		if (SuppressLoading || !IsPlaying)
		{
			return;
		}
		try
		{
			if (!(manualLoad & allowToCacheLoadResult) || remoteRepo == null)
			{
				LoadInternal();
			}
			if (!IsAsynchronous)
			{
				Merge();
			}
		}
		catch (Exception ex)
		{
			Log.Exception = ex.Message ?? "Unknown error";
			Debug.LogException(ex);
		}
		finally
		{
			if (OnLoadComplete != null && !IsAsynchronous)
			{
				OnLoadComplete();
			}
		}
	}

	private void Merge()
	{
		if (remoteRepo == null)
		{
			return;
		}
		new BGMergerEntity(null, remoteRepo, Repo, actualSettings).Merge();
		BGRepoEvents events = Repo.Events;
		if (!events.On || events.IsInBatch)
		{
			return;
		}
		BGEventArgsBatch batchEvent = Repo.Events.EnsureBatch();
		try
		{
			Repo.ForEachMeta((BGMetaEntity meta) =>
			{
				if (actualSettings.IsMetaIncluded(meta.Id))
				{
					batchEvent.AddMetaWithUpdatedEntities(meta.Id);
				}
			});
			Repo.Events.FireBatchEvent();
		}
		finally
		{
			Repo.Events.ClearBatch();
		}
	}

	private void AsyncComplete()
	{
		Merge();
		if (printLogOnLoad && (IsAsynchronous || forceAsynchronous))
		{
			Log.PrintToConsole();
		}
		OnLoadComplete?.Invoke();
	}

	private void LoadInternal()
	{
		if ((inBuildOnly && Application.isEditor) || (loadTried && !manualLoad))
		{
			return;
		}
		Log.Clear();
		Log.Status = BGLiveUpdateLog.StatusEnum.LoadAttempted;
		log.Repo = Repo;
		loadTried = true;
		string error = Error;
		if (!string.IsNullOrEmpty(error))
		{
			Log.Exception = error;
			Debug.LogError(error);
			return;
		}
		RemoteCertificateValidationCallback serverCertificateValidationCallback = ServicePointManager.ServerCertificateValidationCallback;
		try
		{
			ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) => true;
			ProcessLoad();
		}
		finally
		{
			ServicePointManager.ServerCertificateValidationCallback = serverCertificateValidationCallback;
		}
	}

	private void ProcessLoad()
	{
		int timeOut = ((timeout < 1f || timeout > 30f) ? 5000 : ((int)(timeout * 1000f)));
		BGLiveUpdateContext bGLiveUpdateContext = new BGLiveUpdateContext(Repo, this, timeOut, IsAsynchronous || forceAsynchronous, AsyncComplete);
		BGLiveUpdateDataSourceA dataSource;
		switch (sourceType)
		{
		case BGLiveUpdateSourceTypeEnum.GoogleSheets:
			switch (dataSourceType)
			{
			case DataSourceTypeEnum.GoogleSheetsAPI:
				dataSource = new BGLiveUpdateDataSourceGoogleSheetsAPI(bGLiveUpdateContext);
				break;
			case DataSourceTypeEnum.VisualizationAPI:
				dataSource = new BGLiveUpdateDataSourceChartsAPI(bGLiveUpdateContext);
				break;
			case DataSourceTypeEnum.ExcelExport:
				dataSource = new BGLiveUpdateDataSourceExcelExport(bGLiveUpdateContext);
				break;
			default:
				throw new ArgumentOutOfRangeException("dataSourceType");
			}
			break;
		case BGLiveUpdateSourceTypeEnum.WebServer:
			dataSource = new BGLiveUpdateDataSourceUrls(bGLiveUpdateContext, urls);
			break;
		default:
			throw new ArgumentOutOfRangeException("sourceType");
		}
		BGRepo bGRepo = mergeSettings.NewRepo(Repo, copyValues: false);
		actualSettings = (BGMergeSettingsEntity)mergeSettings.Clone();
		bGRepo.ForEachMeta((BGMetaEntity meta) =>
		{
			if (actualSettings.IsMetaIncluded(meta.Id))
			{
				dataSource.Load(meta, actualSettings, null, applyLastOnFailure);
			}
		});
		remoteRepo = bGRepo;
		dataSource.Complete();
		if (printLogOnLoad && !bGLiveUpdateContext.isAsynchronous)
		{
			Log.PrintToConsole();
		}
	}

	public static void Reset()
	{
		remoteRepo = null;
		loadTried = false;
		actualSettings = null;
	}

	public static void LoadDefault(bool allowToCacheLoadResult = false)
	{
		BGAddonLiveUpdate bGAddonLiveUpdate = BGRepo.I.Addons.Get<BGAddonLiveUpdate>();
		if (bGAddonLiveUpdate == null)
		{
			throw new Exception("Can not invoke LiveUpdate addon on default repo, cause addon is not enabled!");
		}
		bGAddonLiveUpdate.Load(allowToCacheLoadResult);
	}
}

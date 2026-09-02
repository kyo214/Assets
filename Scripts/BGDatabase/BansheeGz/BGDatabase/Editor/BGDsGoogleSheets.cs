using System;
using UnityEngine;

namespace BansheeGz.BGDatabase.Editor;

[Descriptor(Name = "GoogleSheets")]
public class BGDsGoogleSheets : BGDataSource, BGSyncNameMapConfig.BGNameConfigOwner
{
	public enum DataSourceTypeEnum
	{
		OAuth = 0,
		Service = 1,
		APIKey = 2,
		Anonymous = 3
	}

	[Serializable]
	private class Settings
	{
		public ActionsTypeEnum ActionsType;

		public int DataSourceType;

		public string ClientId;

		public string ClientSecret;

		public string ApplicationName;

		public string SpreadSheetId;

		public string AccessToken;

		public string RefreshToken;

		public string APIKey;

		public string ClientEmail;

		public string PrivateKey;

		public bool NameMapConfigEnabled;

		public BGSyncNameMapConfig NameMapConfig;

		public BGSyncDisabledConfig DisabledConfig;

		public bool IdConfigEnabled;

		public BGSyncIdConfig IdConfig;

		public BGSyncRelationsConfig RelationsConfig;

		public bool RelationsConfigEnabled;

		public ReadFormatEnum ReadFormat;

		public string ReadFormatCountry;

		public WriteFormatEnum WriteFormat;

		public string WriteFormatCountry;
	}

	public enum ReadFormatEnum : byte
	{
		CurrentLocalCulture = 0,
		InvariantCulture = 1,
		CultureBased = 2,
		UseSpreadsheetCulture = 3
	}

	public enum WriteFormatEnum : byte
	{
		InvariantCulture = 0,
		CurrentLocalCulture = 1,
		CultureBased = 2,
		UseSpreadsheetCulture = 3
	}

	public const string PluginPage = "https://www.bansheegz.com/BGDatabase/Downloads/EditorGoogleSheets";

	public const string ImplementationType = "BansheeGz.BGDatabase.BGGoogleSheetService";

	public const string WrongScriptingRuntimeVersion = "To use Google Sheets you need to switch ScriptingRuntimeVersion to 4.x. Set 'File->Build Settings..->Player Settings->Other Settings->Scripting Runtime Version*' parameter to NET 4.x";

	public DataSourceTypeEnum DataSourceType;

	public string ClientId;

	public string ClientSecret;

	public string ApplicationName;

	public string SpreadSheetId;

	public string AccessToken;

	public string RefreshToken;

	public string APIKey;

	public string ClientEmail;

	public string PrivateKey;

	public ReadFormatEnum ReadFormat = ReadFormatEnum.UseSpreadsheetCulture;

	public string ReadFormatCountry;

	public WriteFormatEnum WriteFormat = WriteFormatEnum.UseSpreadsheetCulture;

	public string WriteFormatCountry;

	public BGSyncNameMapConfig NameMapConfig { get; set; }

	public BGSyncDisabledConfig DisabledConfig { get; set; }

	public bool NameMapConfigEnabled { get; set; }

	public BGSyncIdConfig IdConfig { get; set; }

	public bool IdConfigEnabled { get; set; }

	public BGSyncRelationsConfig RelationsConfig { get; set; }

	public bool RelationsConfigEnabled { get; set; }

	public override bool IsExportAllowed
	{
		get
		{
			if (base.IsExportAllowed && DataSourceType != DataSourceTypeEnum.APIKey)
			{
				return DataSourceType != DataSourceTypeEnum.Anonymous;
			}
			return false;
		}
	}

	public BGGoogleSheetServiceI Service
	{
		get
		{
			BGGoogleSheetServiceI notInitiatedService = NotInitiatedService;
			notInitiatedService?.Init(this);
			return notInitiatedService;
		}
	}

	public static BGGoogleSheetServiceI NotInitiatedService
	{
		get
		{
			Type type = BGUtil.GetType("BansheeGz.BGDatabase.BGGoogleSheetService");
			if (type == null)
			{
				return null;
			}
			try
			{
				return Activator.CreateInstance(type) as BGGoogleSheetServiceI;
			}
			catch
			{
				return null;
			}
		}
	}

	public override string Error
	{
		get
		{
			string error = null;
			if (string.IsNullOrEmpty(SpreadSheetId))
			{
				return "No spreadsheet ID";
			}
			if (CheckForSpaceChar(SpreadSheetId, "SpreadSheet Id", ref error))
			{
				return error;
			}
			switch (DataSourceType)
			{
			case DataSourceTypeEnum.OAuth:
				if (string.IsNullOrEmpty(ClientId))
				{
					return "No client Id";
				}
				if (string.IsNullOrEmpty(ClientSecret))
				{
					return "No client secret";
				}
				if (string.IsNullOrEmpty(ApplicationName))
				{
					return "No application name";
				}
				if (string.IsNullOrEmpty(AccessToken))
				{
					return "No access token";
				}
				if (string.IsNullOrEmpty(RefreshToken))
				{
					return "No refresh token";
				}
				if (CheckForSpaceChar(ClientId, "Client id", ref error))
				{
					return error;
				}
				if (CheckForSpaceChar(ClientSecret, "Client secret", ref error))
				{
					return error;
				}
				if (CheckForSpaceChar(AccessToken, "Access Token", ref error))
				{
					return error;
				}
				if (CheckForSpaceChar(RefreshToken, "Refresh Token", ref error))
				{
					return error;
				}
				break;
			case DataSourceTypeEnum.Service:
				if (string.IsNullOrEmpty(ClientEmail))
				{
					return "No client email";
				}
				if (string.IsNullOrEmpty(PrivateKey))
				{
					return "No private key";
				}
				if (CheckForSpaceChar(ClientEmail, "Client Email", ref error))
				{
					return error;
				}
				break;
			case DataSourceTypeEnum.APIKey:
				if (string.IsNullOrEmpty(APIKey))
				{
					return "No API key";
				}
				if (CheckForSpaceChar(APIKey, "API Key", ref error))
				{
					return error;
				}
				break;
			default:
				throw new ArgumentOutOfRangeException("DataSourceType");
			case DataSourceTypeEnum.Anonymous:
				break;
			}
			return null;
		}
	}

	private bool CheckForSpaceChar(string parameter, string parameterName, ref string error)
	{
		error = null;
		int num = parameter.IndexOf(' ');
		if (num == -1)
		{
			return false;
		}
		error = $"[{parameterName}] has invalid space char at position {num}";
		return true;
	}

	public override string ConfigToString()
	{
		if (NameMapConfigEnabled && NameMapConfig != null)
		{
			NameMapConfig.Trim();
		}
		return JsonUtility.ToJson(new Settings
		{
			ClientId = ClientId,
			AccessToken = AccessToken,
			ApplicationName = ApplicationName,
			SpreadSheetId = SpreadSheetId,
			ClientSecret = ClientSecret,
			RefreshToken = RefreshToken,
			DataSourceType = (int)DataSourceType,
			APIKey = APIKey,
			ClientEmail = ClientEmail,
			PrivateKey = PrivateKey,
			NameMapConfig = (NameMapConfigEnabled ? NameMapConfig : null),
			NameMapConfigEnabled = NameMapConfigEnabled,
			IdConfig = (IdConfigEnabled ? IdConfig : null),
			IdConfigEnabled = IdConfigEnabled,
			DisabledConfig = DisabledConfig,
			RelationsConfigEnabled = RelationsConfigEnabled,
			RelationsConfig = RelationsConfig,
			ActionsType = base.ActionsType,
			ReadFormat = ReadFormat,
			ReadFormatCountry = ReadFormatCountry,
			WriteFormat = WriteFormat,
			WriteFormatCountry = WriteFormatCountry
		});
	}

	public override void ConfigFromString(string config)
	{
		Settings settings = JsonUtility.FromJson<Settings>(config);
		ClientId = settings.ClientId;
		ClientSecret = settings.ClientSecret;
		ApplicationName = settings.ApplicationName;
		SpreadSheetId = settings.SpreadSheetId;
		AccessToken = settings.AccessToken;
		RefreshToken = settings.RefreshToken;
		DataSourceType = (DataSourceTypeEnum)settings.DataSourceType;
		APIKey = settings.APIKey;
		ClientEmail = settings.ClientEmail;
		PrivateKey = settings.PrivateKey;
		NameMapConfig = settings.NameMapConfig;
		NameMapConfigEnabled = settings.NameMapConfigEnabled;
		DisabledConfig = settings.DisabledConfig;
		IdConfig = settings.IdConfig;
		IdConfigEnabled = settings.IdConfigEnabled;
		RelationsConfigEnabled = settings.RelationsConfigEnabled;
		RelationsConfig = settings.RelationsConfig;
		base.ActionsType = settings.ActionsType;
		ReadFormat = settings.ReadFormat;
		ReadFormatCountry = settings.ReadFormatCountry;
		WriteFormat = settings.WriteFormat;
		WriteFormatCountry = settings.WriteFormatCountry;
	}

	public BGGoogleSheetServiceI TryToCreateService(BGLogger logger)
	{
		BGGoogleSheetServiceI service = Service;
		if (service != null)
		{
			logger?.AppendLine("GoogleSheet service is created successfully");
			return service;
		}
		string message = "Error: Can not create GoogleSheet service, cause Google plugin is not installed. Please, download GoogleSheets plugin at https://www.bansheegz.com/BGDatabase/Downloads + and make sure your scripting runtime parameter 'File->Build Settings..->Player Settings->Other Settings->Scripting Runtime Version*' is set to NET 4.x";
		logger?.AppendWarning(message);
		throw new Exception(message);
	}
}

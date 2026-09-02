using System;
using System.IO;
using UnityEngine;

namespace BansheeGz.BGDatabase.Editor;

[Descriptor(Name = "Excel")]
public class BGDsExcel : BGDsFileBased, BGSyncNameMapConfig.BGNameConfigOwner
{
	[Serializable]
	protected new class Settings : BGDsFileBased.Settings
	{
		public new ActionsTypeEnum ActionsType;

		public BGSyncNameMapConfig NameMapConfig;

		public bool NameMapConfigEnabled;

		public bool IdConfigEnabled;

		public bool RelationsConfigEnabled;

		public BGSyncIdConfig IdConfig;

		public BGSyncDisabledConfig DisabledConfig;

		public BGSyncRelationsConfig RelationsConfig;
	}

	public const string ImplementationType = "BansheeGz.BGDatabase.Editor.BGExcelService";

	public const string PluginPage = "https://www.bansheegz.com/BGDatabase/Downloads/EditorExcel";

	public BGSyncNameMapConfig NameMapConfig { get; set; }

	public bool NameMapConfigEnabled { get; set; }

	public BGSyncIdConfig IdConfig { get; set; }

	public bool IdConfigEnabled { get; set; }

	public BGSyncRelationsConfig RelationsConfig { get; set; }

	public bool RelationsConfigEnabled { get; set; }

	public BGSyncDisabledConfig DisabledConfig { get; set; }

	public static BGExcelServiceI Service
	{
		get
		{
			Type type = BGUtil.GetType("BansheeGz.BGDatabase.Editor.BGExcelService");
			if (type == null)
			{
				return null;
			}
			try
			{
				return Activator.CreateInstance(type) as BGExcelServiceI;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return null;
			}
		}
	}

	public override string ConfigToString()
	{
		if (NameMapConfigEnabled && NameMapConfig != null)
		{
			NameMapConfig.Trim();
		}
		return JsonUtility.ToJson(new Settings
		{
			Path = Path,
			NameMapConfig = NameMapConfig,
			NameMapConfigEnabled = NameMapConfigEnabled,
			IdConfig = (IdConfigEnabled ? IdConfig : null),
			IdConfigEnabled = IdConfigEnabled,
			RelationsConfig = (RelationsConfigEnabled ? RelationsConfig : null),
			RelationsConfigEnabled = RelationsConfigEnabled,
			DisabledConfig = DisabledConfig,
			ActionsType = base.ActionsType
		});
	}

	public override void ConfigFromString(string config)
	{
		Settings settings = JsonUtility.FromJson<Settings>(config);
		Path = settings.Path;
		NameMapConfig = settings.NameMapConfig;
		NameMapConfigEnabled = settings.NameMapConfigEnabled;
		IdConfig = settings.IdConfig;
		IdConfigEnabled = settings.IdConfigEnabled;
		RelationsConfig = settings.RelationsConfig;
		RelationsConfigEnabled = settings.RelationsConfigEnabled;
		DisabledConfig = settings.DisabledConfig;
		base.ActionsType = settings.ActionsType;
	}

	public static bool UseXml(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return false;
		}
		string extension = System.IO.Path.GetExtension(path);
		if (!".xlsx".Equals(extension))
		{
			return "xlsx".Equals(extension);
		}
		return true;
	}
}

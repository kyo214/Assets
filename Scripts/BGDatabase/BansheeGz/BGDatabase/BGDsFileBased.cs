using System;
using System.IO;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public abstract class BGDsFileBased : BGDataSource
{
	[Serializable]
	protected class Settings
	{
		public ActionsTypeEnum ActionsType;

		public string Path;
	}

	public string Path;

	public override string Error
	{
		get
		{
			if (string.IsNullOrEmpty(Path))
			{
				return "No path defined";
			}
			if (!Directory.Exists(System.IO.Path.GetDirectoryName(RealPath)))
			{
				return "Folder for the file does not exist: " + RealPath;
			}
			return null;
		}
	}

	public string RealPath
	{
		get
		{
			string text = Path;
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			if (!System.IO.Path.IsPathRooted(text))
			{
				text = System.IO.Path.Combine(Application.dataPath, text);
			}
			return text;
		}
	}

	public override string ConfigToString()
	{
		return JsonUtility.ToJson(new Settings
		{
			Path = Path,
			ActionsType = base.ActionsType
		});
	}

	public override void ConfigFromString(string config)
	{
		Settings settings = JsonUtility.FromJson<Settings>(config);
		Path = settings.Path;
		base.ActionsType = settings.ActionsType;
	}
}

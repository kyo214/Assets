using System;
using UnityEngine;

namespace BansheeGz.BGDatabase.Editor;

[Descriptor(Name = "Json", SupportSettings = false)]
public class BGDsJson : BGDsFileBased
{
	[Serializable]
	protected new class Settings : BGDsFileBased.Settings
	{
		public BGJsonFormatEnum Format;
	}

	public BGJsonFormatEnum Format;

	public override bool RequireMergeSettings => false;

	public override string ConfigToString()
	{
		return JsonUtility.ToJson(new Settings
		{
			Path = Path,
			ActionsType = base.ActionsType,
			Format = Format
		});
	}

	public override void ConfigFromString(string config)
	{
		Settings settings = JsonUtility.FromJson<Settings>(config);
		Path = settings.Path;
		base.ActionsType = settings.ActionsType;
		Format = settings.Format;
	}
}

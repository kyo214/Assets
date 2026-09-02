using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

[AddonDescriptor(Name = "LazyLoad", ManagerType = "BansheeGz.BGDatabase.Editor.BGAddonManagerLazyLoad")]
public class BGAddonLazyLoad : BGAddon
{
	[Serializable]
	private class Settings
	{
		public bool DisconnectEntity;

		public bool EnabledForPlayMode;
	}

	private bool disconnectEntity;

	private bool enabledForPlayMode;

	public bool DisconnectEntity
	{
		get
		{
			return disconnectEntity;
		}
		set
		{
			if (disconnectEntity != value)
			{
				disconnectEntity = value;
				FireChange();
			}
		}
	}

	public bool EnabledForPlayMode
	{
		get
		{
			return enabledForPlayMode;
		}
		set
		{
			if (enabledForPlayMode != value)
			{
				enabledForPlayMode = value;
				FireChange();
			}
		}
	}

	public override string ConfigToString()
	{
		return JsonUtility.ToJson(new Settings
		{
			DisconnectEntity = disconnectEntity,
			EnabledForPlayMode = enabledForPlayMode
		});
	}

	public override void ConfigFromString(string config)
	{
		Settings settings = JsonUtility.FromJson<Settings>(config);
		disconnectEntity = settings.DisconnectEntity;
		enabledForPlayMode = settings.EnabledForPlayMode;
	}

	public override byte[] ConfigToBytes()
	{
		BGBinaryWriter bGBinaryWriter = new BGBinaryWriter(6);
		bGBinaryWriter.AddInt(1);
		bGBinaryWriter.AddBool(disconnectEntity);
		bGBinaryWriter.AddBool(enabledForPlayMode);
		return bGBinaryWriter.ToArray();
	}

	public override void ConfigFromBytes(ArraySegment<byte> config)
	{
		BGBinaryReader bGBinaryReader = new BGBinaryReader(config);
		int num = bGBinaryReader.ReadInt();
		if (num == 1)
		{
			disconnectEntity = bGBinaryReader.ReadBool();
			enabledForPlayMode = bGBinaryReader.ReadBool();
			return;
		}
		throw new BGException("Unknown version: $", num);
	}

	public override BGAddon CloneTo(BGRepo repo)
	{
		return new BGAddonLazyLoad
		{
			Repo = repo,
			disconnectEntity = disconnectEntity,
			enabledForPlayMode = enabledForPlayMode
		};
	}

	public static bool Enabled(BGRepo repo)
	{
		if (!repo.Addons.Has<BGAddonLazyLoad>())
		{
			return false;
		}
		if (BGLocalizationUglyHacks.HasLocalizationAddon(repo))
		{
			return false;
		}
		BGAddonLazyLoad bGAddonLazyLoad = repo.Addons.Get<BGAddonLazyLoad>();
		if (!bGAddonLazyLoad.EnabledForPlayMode)
		{
			return Application.isEditor;
		}
		return true;
	}
}

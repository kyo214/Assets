using System.Collections.Generic;

namespace DarkTonic.MasterAudio;

public class MasterAudioSettings : SingletonScriptable<MasterAudioSettings>
{
	public const string AssetName = "MasterAudioSettings.asset";

	public const string AssetFolder = "Assets/Resources/MasterAudio";

	public const string ResourcePath = "MasterAudio/MasterAudioSettings";

	public bool UseDbScale;

	public bool RemoveUnplayedDueToProbabilityVariation = true;

	public bool UseCentsPitch;

	public bool HideLogoNav;

	public bool EditMAFolder;

	public string InstallationFolderPath = "Assets/Plugins/DarkTonic/MasterAudio";

	public MasterAudio.MixerWidthMode MixerWidthSetting;

	public bool BusesShownInNarrow = true;

	public bool ShowWelcomeWindowOnStart = true;

	static MasterAudioSettings()
	{
		SingletonScriptable<MasterAudioSettings>.AssetNameToLoad = string.Format("{0}/{1}", "Assets/Resources/MasterAudio", "MasterAudioSettings.asset");
		SingletonScriptable<MasterAudioSettings>.ResourceNameToLoad = "MasterAudio/MasterAudioSettings";
		SingletonScriptable<MasterAudioSettings>.FoldersToCreate = new List<string> { "Assets/Resources", "Assets/Resources/MasterAudio" };
	}
}

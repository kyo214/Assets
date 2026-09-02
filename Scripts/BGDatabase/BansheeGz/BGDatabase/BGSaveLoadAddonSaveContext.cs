namespace BansheeGz.BGDatabase;

public class BGSaveLoadAddonSaveContext
{
	public readonly string ConfigName = "Default";

	public bool FireBeforeSaveEvents = true;

	public bool MergeDataFromMTAddon = true;

	public BGSaveLoadAddonSaveContext()
	{
	}

	public BGSaveLoadAddonSaveContext(string configName)
	{
		ConfigName = configName;
	}
}

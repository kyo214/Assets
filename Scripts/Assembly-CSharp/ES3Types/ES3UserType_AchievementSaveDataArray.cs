using _Modules.Achievement.Scripts;

namespace ES3Types;

public class ES3UserType_AchievementSaveDataArray : ES3ArrayType
{
	public static ES3Type Instance;

	public ES3UserType_AchievementSaveDataArray()
		: base(typeof(AchievementSaveData[]), ES3UserType_AchievementSaveData.Instance)
	{
		Instance = this;
	}
}

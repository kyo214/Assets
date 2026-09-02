using _Modules.Achievement.Scripts;

namespace ES3Types;

public class ES3UserType_GameStatisticSaveDataArray : ES3ArrayType
{
	public static ES3Type Instance;

	public ES3UserType_GameStatisticSaveDataArray()
		: base(typeof(GameStatisticSaveData[]), ES3UserType_GameStatisticSaveData.Instance)
	{
		Instance = this;
	}
}

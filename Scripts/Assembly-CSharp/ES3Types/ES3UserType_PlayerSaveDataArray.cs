using _Modules.Data.Scripts;

namespace ES3Types;

public class ES3UserType_PlayerSaveDataArray : ES3ArrayType
{
	public static ES3Type Instance;

	public ES3UserType_PlayerSaveDataArray()
		: base(typeof(PlayerSaveData[]), ES3UserType_PlayerSaveData.Instance)
	{
		Instance = this;
	}
}

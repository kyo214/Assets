using _Modules.Data.Scripts;

namespace ES3Types;

public class ES3UserType_UserSaveDataArray : ES3ArrayType
{
	public static ES3Type Instance;

	public ES3UserType_UserSaveDataArray()
		: base(typeof(UserSaveData[]), ES3UserType_UserSaveData.Instance)
	{
		Instance = this;
	}
}

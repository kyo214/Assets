namespace ES3Types;

public class ES3UserType_GameDataArray : ES3ArrayType
{
	public static ES3Type Instance;

	public ES3UserType_GameDataArray()
		: base(typeof(GameData[]), ES3UserType_GameData.Instance)
	{
		Instance = this;
	}
}

namespace ES3Types;

public class ES3UserType_OptionDataArray : ES3ArrayType
{
	public static ES3Type Instance;

	public ES3UserType_OptionDataArray()
		: base(typeof(GlobalSaveData.OptionData[]), ES3UserType_OptionData.Instance)
	{
		Instance = this;
	}
}

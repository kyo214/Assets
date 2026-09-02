namespace ES3Types;

public class ES3UserType_ScoreDataNetworkArray : ES3ArrayType
{
	public static ES3Type Instance;

	public ES3UserType_ScoreDataNetworkArray()
		: base(typeof(ScoreDataNetwork[]), ES3UserType_ScoreDataNetwork.Instance)
	{
		Instance = this;
	}
}

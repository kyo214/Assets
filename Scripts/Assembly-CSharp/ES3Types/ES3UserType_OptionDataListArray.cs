using UnityEngine.UI;

namespace ES3Types;

public class ES3UserType_OptionDataListArray : ES3ArrayType
{
	public static ES3Type Instance;

	public ES3UserType_OptionDataListArray()
		: base(typeof(Dropdown.OptionDataList[]), ES3UserType_OptionDataList.Instance)
	{
		Instance = this;
	}
}

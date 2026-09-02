namespace ES3Types;

public class ES3UserType_InventoryObjectArray : ES3ArrayType
{
	public static ES3Type Instance;

	public ES3UserType_InventoryObjectArray()
		: base(typeof(InventoryObject[]), ES3UserType_InventoryObject.Instance)
	{
		Instance = this;
	}
}

namespace ES3Types;

public class ES3UserType_StatusEffectItemObjectArray : ES3ArrayType
{
	public static ES3Type Instance;

	public ES3UserType_StatusEffectItemObjectArray()
		: base(typeof(InventoryObject.StatusEffectItemObject[]), ES3UserType_StatusEffectItemObject.Instance)
	{
		Instance = this;
	}
}

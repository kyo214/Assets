using UnityEngine.Scripting;

namespace ES3Types;

[Preserve]
[ES3Properties(new string[] { "BaseName", "AdditionalName" })]
public class ES3UserType_StatusEffectItemObject : ES3ObjectType
{
	public static ES3Type Instance;

	public ES3UserType_StatusEffectItemObject()
		: base(typeof(InventoryObject.StatusEffectItemObject))
	{
		Instance = this;
		priority = 1;
	}

	protected override void WriteObject(object obj, ES3Writer writer)
	{
		InventoryObject.StatusEffectItemObject statusEffectItemObject = (InventoryObject.StatusEffectItemObject)obj;
		writer.WriteProperty("BaseName", statusEffectItemObject.BaseName, ES3Type_string.Instance);
		writer.WriteProperty("AdditionalName", statusEffectItemObject.AdditionalName, ES3Type_string.Instance);
	}

	protected override void ReadObject<T>(ES3Reader reader, object obj)
	{
		InventoryObject.StatusEffectItemObject statusEffectItemObject = (InventoryObject.StatusEffectItemObject)obj;
		foreach (string property in reader.Properties)
		{
			if (!(property == "BaseName"))
			{
				if (property == "AdditionalName")
				{
					statusEffectItemObject.AdditionalName = reader.Read<string>(ES3Type_string.Instance);
				}
				else
				{
					reader.Skip();
				}
			}
			else
			{
				statusEffectItemObject.BaseName = reader.Read<string>(ES3Type_string.Instance);
			}
		}
	}

	protected override object ReadObject<T>(ES3Reader reader)
	{
		InventoryObject.StatusEffectItemObject statusEffectItemObject = new InventoryObject.StatusEffectItemObject();
		ReadObject<T>(reader, statusEffectItemObject);
		return statusEffectItemObject;
	}
}

using System.Collections.Generic;
using ES3Internal;
using UnityEngine.Scripting;

namespace ES3Types;

[Preserve]
[ES3Properties(new string[]
{
	"UniqueID", "ID", "Name", "IdxInventory", "ItemType", "Amount", "Ammo", "Durability", "IsUsable", "IsEquippable",
	"MaxItemInSlot", "IsCombinable", "IsOpenable", "equip", "statusEffects"
})]
public class ES3UserType_InventoryObject : ES3ObjectType
{
	public static ES3Type Instance;

	public ES3UserType_InventoryObject()
		: base(typeof(InventoryObject))
	{
		Instance = this;
		priority = 1;
	}

	protected override void WriteObject(object obj, ES3Writer writer)
	{
		InventoryObject inventoryObject = (InventoryObject)obj;
		writer.WriteProperty("UniqueID", inventoryObject.UniqueID, ES3Type_int.Instance);
		writer.WriteProperty("ID", inventoryObject.ID, ES3Type_int.Instance);
		writer.WriteProperty("Name", inventoryObject.Name, ES3Type_string.Instance);
		writer.WriteProperty("IdxInventory", inventoryObject.IdxInventory, ES3Type_int.Instance);
		writer.WriteProperty("ItemType", inventoryObject.ItemType, ES3Type_string.Instance);
		writer.WriteProperty("Amount", inventoryObject.Amount, ES3Type_int.Instance);
		writer.WriteProperty("Ammo", inventoryObject.Ammo, ES3Type_int.Instance);
		writer.WriteProperty("Durability", inventoryObject.Durability, ES3Type_float.Instance);
		writer.WriteProperty("IsUsable", inventoryObject.IsUsable, ES3Type_bool.Instance);
		writer.WriteProperty("IsEquippable", inventoryObject.IsEquippable, ES3Type_bool.Instance);
		writer.WriteProperty("MaxItemInSlot", inventoryObject.MaxItemInSlot, ES3Type_int.Instance);
		writer.WriteProperty("IsCombinable", inventoryObject.IsCombinable, ES3Type_bool.Instance);
		writer.WriteProperty("IsOpenable", inventoryObject.IsOpenable, ES3Type_bool.Instance);
		writer.WriteProperty("equip", inventoryObject.equip, ES3Type_bool.Instance);
		writer.WriteProperty("statusEffects", inventoryObject.statusEffects, ES3TypeMgr.GetOrCreateES3Type(typeof(List<InventoryObject.StatusEffectItemObject>)));
	}

	protected override void ReadObject<T>(ES3Reader reader, object obj)
	{
		InventoryObject inventoryObject = (InventoryObject)obj;
		foreach (string property in reader.Properties)
		{
			switch (property)
			{
			case "UniqueID":
				inventoryObject.UniqueID = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "ID":
				inventoryObject.ID = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "Name":
				inventoryObject.Name = reader.Read<string>(ES3Type_string.Instance);
				break;
			case "IdxInventory":
				inventoryObject.IdxInventory = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "ItemType":
				inventoryObject.ItemType = reader.Read<string>(ES3Type_string.Instance);
				break;
			case "Amount":
				inventoryObject.Amount = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "Ammo":
				inventoryObject.Ammo = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "Durability":
				inventoryObject.Durability = reader.Read<float>(ES3Type_float.Instance);
				break;
			case "IsUsable":
				inventoryObject.IsUsable = reader.Read<bool>(ES3Type_bool.Instance);
				break;
			case "IsEquippable":
				inventoryObject.IsEquippable = reader.Read<bool>(ES3Type_bool.Instance);
				break;
			case "MaxItemInSlot":
				inventoryObject.MaxItemInSlot = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "IsCombinable":
				inventoryObject.IsCombinable = reader.Read<bool>(ES3Type_bool.Instance);
				break;
			case "IsOpenable":
				inventoryObject.IsOpenable = reader.Read<bool>(ES3Type_bool.Instance);
				break;
			case "equip":
				inventoryObject.equip = reader.Read<bool>(ES3Type_bool.Instance);
				break;
			case "statusEffects":
				inventoryObject.statusEffects = reader.Read<List<InventoryObject.StatusEffectItemObject>>();
				break;
			default:
				reader.Skip();
				break;
			}
		}
	}

	protected override object ReadObject<T>(ES3Reader reader)
	{
		InventoryObject inventoryObject = new InventoryObject();
		ReadObject<T>(reader, inventoryObject);
		return inventoryObject;
	}
}

using Toked.Crafting;
using UnityEngine;

namespace Toked.Item;

public abstract class CustomItemActionBase : ScriptableObject
{
	public abstract void AddItemAction(PlayerController playerController, InventoryObject inventoryObject, ItemScriptableObject itemData = null);

	public abstract void EquipAction(PlayerController playerController, InventoryObject inventoryObject);

	public abstract void UnequipAction(PlayerController playerController, InventoryObject inventoryObject);
}

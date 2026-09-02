using System.Collections.Generic;
using Toked.Crafting;
using Toked.StatusEffect;
using UnityEngine;

namespace Toked.Item;

[CreateAssetMenu(fileName = "StatusEffectCustomItemAction", menuName = "WMO/ScriptableObjects/Item/Item Equip Action/StatusEffectCustomItemAction", order = 0)]
public class StatusEffectCustomItemAction : CustomItemActionBase
{
	[SerializeField]
	private List<StatusEffectScriptableObject> _statusEffectsList;

	public override void AddItemAction(PlayerController playerController, InventoryObject inventoryObject, ItemScriptableObject itemData = null)
	{
		if ((object)itemData == null)
		{
			itemData = DataManager.Instance.GetItemData(inventoryObject.ID.ToString());
		}
		if (!(itemData != null) || !itemData.UseCustomEquipInventoryEffect || !itemData.CustomEquipInventoryEffectSO)
		{
			return;
		}
		foreach (StatusEffectScriptableObject statusEffects in _statusEffectsList)
		{
			inventoryObject.AddSetStatusEffectWithItemId(statusEffects);
		}
		if (!itemData.ManualApplyEffectItemInventory || itemData.DefaultEquipValue)
		{
			EquipAction(playerController, inventoryObject);
		}
	}

	public override void EquipAction(PlayerController playerController, InventoryObject inventoryObject)
	{
		inventoryObject.ApplyStatusEffect(playerController.StatusEffectController);
		inventoryObject.equip = true;
		playerController.inventoryManager.GetItemInventorySlotUI(inventoryObject.IdxInventory)?.SetActiveEquip(active: true);
	}

	public override void UnequipAction(PlayerController playerController, InventoryObject inventoryObject)
	{
		inventoryObject.RemoveStatusEffect(playerController.StatusEffectController);
		inventoryObject.equip = false;
		playerController.inventoryManager.GetItemInventorySlotUI(inventoryObject.IdxInventory)?.SetActiveEquip(active: false);
	}
}

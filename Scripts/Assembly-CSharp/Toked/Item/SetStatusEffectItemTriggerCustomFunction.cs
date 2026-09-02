using System;
using System.Collections.Generic;
using Toked.StatusEffect;
using UnityEngine;

namespace Toked.Item;

public class SetStatusEffectItemTriggerCustomFunction : ItemInteractableCustomFunction
{
	[SerializeField]
	private ItemPickable _itemPickable;

	[SerializeField]
	private List<StatusEffectScriptableObject> _statusEffectScriptableObjectList;

	public override void Execute(PlayerController playerController = null)
	{
		if (playerController == null)
		{
			return;
		}
		if ((object)_itemPickable == null)
		{
			_itemPickable = GetComponent<ItemPickable>();
		}
		foreach (StatusEffectScriptableObject statusEffectScriptableObject2 in _statusEffectScriptableObjectList)
		{
			if ((bool)statusEffectScriptableObject2)
			{
				StatusEffectScriptableObject statusEffectScriptableObject = statusEffectScriptableObject2.CloneStatusEffectSO(destroyOnRemove: true);
				statusEffectScriptableObject.StatusEffectData.SetAdditionalName(_itemPickable.uniqueID.ToString(), _itemPickable.itemID.ToString());
				if (statusEffectScriptableObject is IItemEffect itemEffect)
				{
					itemEffect.Init(_itemPickable.itemID, _itemPickable.uniqueID);
				}
				playerController.StatusEffectController.ApplyStatus(playerController, statusEffectScriptableObject);
			}
		}
		PlayerData data = playerController.data;
		data.OnRemoveItemInventoryEvent = (Action<InventoryObject>)Delegate.Combine(data.OnRemoveItemInventoryEvent, (Action<InventoryObject>)((InventoryObject item) =>
		{
			OnDropItemAction(item, _itemPickable, playerController);
		}));
	}

	private void OnDropItemAction(InventoryObject itemInventoryObject, ItemPickable itemPickable, PlayerController playerController)
	{
		if (itemInventoryObject.UniqueID != itemPickable.uniqueID || itemInventoryObject.ID != itemPickable.itemID)
		{
			return;
		}
		StatusEffectController statusEffectController = playerController.StatusEffectController;
		if (statusEffectController != null)
		{
			foreach (StatusEffectScriptableObject statusEffectScriptableObject in _statusEffectScriptableObjectList)
			{
				string key = $"{statusEffectScriptableObject.StatusEffectData.BaseName}_{itemInventoryObject.UniqueID}_{itemInventoryObject.ID}";
				statusEffectController.ClearStatus(key);
			}
		}
		PlayerData data = playerController.data;
		data.OnRemoveItemInventoryEvent = (Action<InventoryObject>)Delegate.Remove(data.OnRemoveItemInventoryEvent, (Action<InventoryObject>)((InventoryObject item) =>
		{
			OnDropItemAction(item, _itemPickable, playerController);
		}));
	}
}

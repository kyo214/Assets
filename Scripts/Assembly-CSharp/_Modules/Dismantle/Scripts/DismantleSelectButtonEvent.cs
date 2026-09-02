using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Modules.Dismantle.Scripts;

public class DismantleSelectButtonEvent : MonoBehaviour
{
	public Action<InventoryObject> OnSelectDismantleItem;

	public Action OnDeselectDismantleItem;

	private PlayerController _playerController;

	private PlayerController PlayerController => _playerController ?? (_playerController = NetworkGameManager.Instance.ownPlayer);

	public void OnSelectButtonInventoryOptions(BaseEventData eventData)
	{
		if (UIGameManager.Instance.uiInventory.isVisible)
		{
			string text = PlayerController.inventoryManager.targetInventory.name;
			int index = int.Parse(text.Substring(13, text.Length - 13));
			OnSelectDismantleItem?.Invoke(PlayerController.data.arrInventory[index]);
		}
	}

	public void OnDeselectButtonInventoryOptions(BaseEventData eventData)
	{
		if (UIGameManager.Instance.uiInventory.isVisible)
		{
			OnDeselectDismantleItem?.Invoke();
		}
	}
}

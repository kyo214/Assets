using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Toked.Inventory;

public class InventoryItemSlotButton : MonoBehaviour
{
	[SerializeField]
	private Button _button;

	[FormerlySerializedAs("_navigationInventory")]
	[SerializeField]
	private Navigation _inventoryNavigation;

	[FormerlySerializedAs("_navigationCrafting")]
	[SerializeField]
	private Navigation _craftingNavigation;

	public void SetNavigationCrafting()
	{
		_button.navigation = _craftingNavigation;
	}

	public void SetNavigationInventory()
	{
		_button.navigation = _inventoryNavigation;
	}
}

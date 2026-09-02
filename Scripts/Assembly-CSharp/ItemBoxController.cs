using System.Collections.Generic;
using UnityEngine;

public class ItemBoxController : MonoBehaviour
{
	public List<InventoryObject> arrItem = new List<InventoryObject>();

	public void InitItemBox()
	{
		arrItem.Clear();
		InventoryObject item = new InventoryObject
		{
			ID = 201,
			Name = "Green Herb",
			ItemType = "HealingItem",
			Amount = 1,
			MaxItemInSlot = 1
		};
		arrItem.Add(item);
	}

	public int FindTotalInventory(int inventoryID)
	{
		int num = 0;
		foreach (InventoryObject item in arrItem)
		{
			if (item.ID == inventoryID)
			{
				num += item.Amount;
			}
		}
		return num;
	}
}

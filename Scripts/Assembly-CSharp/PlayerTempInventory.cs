using System;
using System.Collections.Generic;

[Serializable]
public class PlayerTempInventory
{
	public string DeviceID;

	public List<InventoryObject> ArrInventory = new List<InventoryObject>();
}

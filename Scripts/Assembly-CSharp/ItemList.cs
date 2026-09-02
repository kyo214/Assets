using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemList
{
	public ItemTypeEnum ItemType;

	public List<GameObject> gameObjects = new List<GameObject>();
}

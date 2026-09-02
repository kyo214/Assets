using System;
using UnityEngine;

[Serializable]
public class TerrainGroup : MonoBehaviour
{
	public int GroupID;

	public void UpdateChildTerrains()
	{
		Terrain[] componentsInChildren = GetComponentsInChildren<Terrain>();
		foreach (Terrain obj in componentsInChildren)
		{
			_ = obj.gameObject;
			obj.groupingID = GroupID;
		}
	}

	public void DestroyChildTerrains()
	{
		Terrain[] componentsInChildren = GetComponentsInChildren<Terrain>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
		}
	}
}

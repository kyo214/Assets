using UnityEngine;

namespace DestroyIt;

public static class TerrainExtensions
{
	public static TerrainTree ClosestTreeToPoint(this Terrain terrain, Vector3 point)
	{
		TreeInstance[] treeInstances = terrain.terrainData.treeInstances;
		if (treeInstances.Length == 0)
		{
			return null;
		}
		TerrainTree terrainTree = new TerrainTree
		{
			Index = -1
		};
		float num = float.MaxValue;
		for (int i = 0; i < treeInstances.Length; i++)
		{
			Vector3 vector = Vector3.Scale(treeInstances[i].position, terrain.terrainData.size) + terrain.transform.position;
			float num2 = Vector3.Distance(vector, point);
			if (num2 < num)
			{
				num = num2;
				terrainTree.Index = i;
				terrainTree.Position = vector;
				terrainTree.TreeInstance = treeInstances[i];
			}
		}
		return terrainTree;
	}

	public static Vector3 WorldPositionOfTree(this Terrain terrain, int treeIndex)
	{
		TreeInstance[] treeInstances = terrain.terrainData.treeInstances;
		if (treeInstances.Length == 0)
		{
			return Vector3.zero;
		}
		return Vector3.Scale(treeInstances[treeIndex].position, terrain.terrainData.size) + terrain.transform.position;
	}
}

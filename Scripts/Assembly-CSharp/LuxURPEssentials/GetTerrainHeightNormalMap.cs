using UnityEngine;

namespace LuxURPEssentials;

[RequireComponent(typeof(Terrain))]
public class GetTerrainHeightNormalMap : MonoBehaviour
{
	public TerrainData targetTerrainData;

	public string savePathTerrainHeightNormalMap;

	public void GetTerData()
	{
		Terrain terrain = (Terrain)GetComponent(typeof(Terrain));
		targetTerrainData = terrain.terrainData;
	}
}

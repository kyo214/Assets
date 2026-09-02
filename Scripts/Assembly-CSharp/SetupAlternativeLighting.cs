using System.Collections.Generic;
using UnityEngine;

public class SetupAlternativeLighting : MonoBehaviour
{
	public List<BakeOnPrefab> PrefabWithLightmaps;

	[SerializeField]
	private List<LightmapData> _mapDatas;

	private void Awake()
	{
		PrefabWithLightmaps = new List<BakeOnPrefab>();
	}

	public void AddInfluencedPrefab(BakeOnPrefab prefab)
	{
		PrefabWithLightmaps.Add(prefab);
	}

	private void Start()
	{
		Invoke("ApplyLightmaps", 2f);
	}

	public void ApplyLightmaps()
	{
		_ = LightmapSettings.lightmaps;
		_mapDatas = new List<LightmapData>();
		for (int i = 0; i < PrefabWithLightmaps.Count; i++)
		{
			for (int j = 0; j < PrefabWithLightmaps[i].LightmapColor.Length; j++)
			{
				int num = CheckUnique(PrefabWithLightmaps[i].LightmapColor[j]);
				if (num < 0)
				{
					LightmapData item = new LightmapData
					{
						lightmapColor = PrefabWithLightmaps[i].LightmapColor[j],
						lightmapDir = PrefabWithLightmaps[i].LightmapDir[j]
					};
					_mapDatas.Add(item);
					PrefabWithLightmaps[i].LightmapIndex[j] = _mapDatas.Count - 1;
				}
				else
				{
					PrefabWithLightmaps[i].LightmapIndex[j] = num;
				}
			}
			for (int k = 0; k < PrefabWithLightmaps[i].Meshes.Length; k++)
			{
				PrefabWithLightmaps[i].Meshes[k].lightmapIndex = PrefabWithLightmaps[i].LightmapIndex[k];
				PrefabWithLightmaps[i].Meshes[k].lightmapScaleOffset = PrefabWithLightmaps[i].LightMapScaleOffset[k];
			}
		}
		LightmapSettings.lightmaps = _mapDatas.ToArray();
	}

	private int CheckUnique(Texture2D lightmapColor)
	{
		for (int i = 0; i < _mapDatas.Count; i++)
		{
			if (lightmapColor == _mapDatas[i].lightmapColor)
			{
				return i;
			}
		}
		return -1;
	}

	private void TestSwap()
	{
		PrefabWithLightmaps[0].SwapState(1);
	}
}

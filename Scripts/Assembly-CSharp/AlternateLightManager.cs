using UnityEngine;

public class AlternateLightManager : MonoBehaviour
{
	[SerializeField]
	private Texture2D[] _lightmapLs;

	[SerializeField]
	private Texture2D[] _lightmapDirs;

	private LightmapData[] _lightmapsData;

	[SerializeField]
	private MeshRenderer _testMesh;

	private void Start()
	{
		SetupLightMaps();
	}

	private void SetupLightMaps()
	{
		_lightmapsData = new LightmapData[_lightmapLs.Length];
		for (int i = 0; i < _lightmapsData.Length; i++)
		{
			_lightmapsData[i] = new LightmapData();
			_lightmapsData[i].lightmapColor = _lightmapLs[i];
			_lightmapsData[i].lightmapDir = _lightmapDirs[i];
		}
		LightmapSettings.lightmaps = _lightmapsData;
		InvokeRepeating("ChangeLightMap", 2f, 2f);
	}

	private void ChangeLightMap()
	{
		int lightmapIndex = (_testMesh.lightmapIndex + 1) % _lightmapLs.Length;
		_testMesh.lightmapIndex = lightmapIndex;
	}
}

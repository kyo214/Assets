using System.Collections.Generic;
using UnityEngine;

namespace VolumetricFogAndMist2;

public static class Tools
{
	public static Color ColorBlack = Color.black;

	private static Mesh _fullScreenMesh;

	public static Mesh fullscreenMesh
	{
		get
		{
			if (_fullScreenMesh != null)
			{
				return _fullScreenMesh;
			}
			float y = 1f;
			float y2 = 0f;
			_fullScreenMesh = new Mesh();
			_fullScreenMesh.SetVertices(new List<Vector3>
			{
				new Vector3(-1f, -1f, 0f),
				new Vector3(-1f, 1f, 0f),
				new Vector3(1f, -1f, 0f),
				new Vector3(1f, 1f, 0f)
			});
			_fullScreenMesh.SetUVs(0, new List<Vector2>
			{
				new Vector2(0f, y2),
				new Vector2(0f, y),
				new Vector2(1f, y2),
				new Vector2(1f, y)
			});
			_fullScreenMesh.SetIndices(new int[6] { 0, 1, 2, 2, 1, 3 }, MeshTopology.Triangles, 0, calculateBounds: false);
			_fullScreenMesh.UploadMeshData(markNoLongerReadable: true);
			return _fullScreenMesh;
		}
	}

	public static void CheckCamera(ref Camera cam)
	{
		if (cam != null)
		{
			return;
		}
		cam = Camera.main;
		if (!(cam == null))
		{
			return;
		}
		Camera[] array = Object.FindObjectsOfType<Camera>();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].isActiveAndEnabled && array[i].gameObject.activeInHierarchy)
			{
				cam = array[i];
				break;
			}
		}
	}

	public static VolumetricFogManager CheckMainManager()
	{
		VolumetricFogManager volumetricFogManager = VolumetricFogManager.GetManagerIfExists();
		if (volumetricFogManager == null)
		{
			GameObject gameObject = new GameObject();
			volumetricFogManager = gameObject.AddComponent<VolumetricFogManager>();
			gameObject.name = volumetricFogManager.managerName;
		}
		return volumetricFogManager;
	}

	public static void CheckManager<T>(ref T manager) where T : Component
	{
		if (!(manager == null))
		{
			return;
		}
		VolumetricFogManager volumetricFogManager = CheckMainManager();
		if (!(volumetricFogManager == null))
		{
			manager = volumetricFogManager.GetComponentInChildren<T>(includeInactive: true);
			if (manager == null)
			{
				GameObject gameObject = new GameObject();
				gameObject.transform.SetParent(volumetricFogManager.transform, worldPositionStays: false);
				manager = gameObject.AddComponent<T>();
				gameObject.name = ((IVolumetricFogManager)manager).managerName;
			}
		}
	}
}

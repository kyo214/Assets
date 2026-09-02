using UnityEngine;

namespace MeshCombineStudio;

public struct GameObjectLayer(GameObject go)
{
	public GameObject go = go;

	public int layer = go.layer;

	public void RestoreLayer()
	{
		go.layer = layer;
	}
}

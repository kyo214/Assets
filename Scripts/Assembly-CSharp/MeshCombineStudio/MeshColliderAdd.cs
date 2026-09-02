using UnityEngine;

namespace MeshCombineStudio;

public struct MeshColliderAdd(GameObject go, Mesh mesh)
{
	public GameObject go = go;

	public Mesh mesh = mesh;
}

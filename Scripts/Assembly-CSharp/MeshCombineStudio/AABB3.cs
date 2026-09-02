using UnityEngine;

namespace MeshCombineStudio;

public struct AABB3(Vector3 min, Vector3 max)
{
	public Vector3 min = min;

	public Vector3 max = max;
}

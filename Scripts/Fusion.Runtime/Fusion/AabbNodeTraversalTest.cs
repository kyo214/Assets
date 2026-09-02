using UnityEngine;

namespace Fusion;

internal class AabbNodeTraversalTest : IBoundsTraversalTest
{
	private Vector3 _min;

	private Vector3 _max;

	internal AabbNodeTraversalTest(Vector3 center, Vector3 extents)
	{
		SetTestSettings(center, extents);
	}

	internal void SetTestSettings(Vector3 center, Vector3 extents)
	{
		_min.x = center.x - extents.x;
		_min.y = center.y - extents.y;
		_min.z = center.z - extents.z;
		_max.x = center.x + extents.x;
		_max.y = center.y + extents.y;
		_max.z = center.z + extents.z;
	}

	public bool Check(ref BVHNode.CachedBounds bounds)
	{
		return _min.x <= bounds.Max.x && _max.x >= bounds.Min.x && _min.y <= bounds.Max.y && _max.y >= bounds.Min.y && _min.z <= bounds.Max.z && _max.z >= bounds.Min.z;
	}
}

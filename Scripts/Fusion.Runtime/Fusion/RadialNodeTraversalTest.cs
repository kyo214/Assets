using UnityEngine;

namespace Fusion;

internal class RadialNodeTraversalTest : IBoundsTraversalTest
{
	private Vector3 _center;

	private float _radius;

	internal RadialNodeTraversalTest(Vector3 center, float radius)
	{
		SetTestSettings(center, radius);
	}

	internal void SetTestSettings(Vector3 center, float radius)
	{
		_center = center;
		_radius = radius;
	}

	public bool Check(ref BVHNode.CachedBounds bounds)
	{
		return LagCompensationUtils.LocalAABBSphereIntersection(bounds.Extents, _center - bounds.Center, _radius);
	}
}

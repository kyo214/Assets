using UnityEngine;

namespace Fusion;

internal class BVHHelper
{
	private RadialNodeTraversalTest RadialNodeTest = new RadialNodeTraversalTest(Vector3.zero, 0f);

	private RayNodeTraversalTest RayNodeTest = new RayNodeTraversalTest(Vector3.zero, Vector3.zero, 0f);

	private AabbNodeTraversalTest AabbNodeTest = new AabbNodeTraversalTest(Vector3.zero, Vector3.zero);

	internal IBoundsTraversalTest RadialNodeTraversalTest(Vector3 center, float radius)
	{
		RadialNodeTest.SetTestSettings(center, radius);
		return RadialNodeTest;
	}

	internal IBoundsTraversalTest RayNodeTraversalTest(Vector3 origin, Vector3 direction, float length)
	{
		RayNodeTest.SetTestSettings(origin, direction, length);
		return RayNodeTest;
	}

	internal IBoundsTraversalTest AabbNodeTraversalTest(Vector3 center, Vector3 extent)
	{
		AabbNodeTest.SetTestSettings(center, center);
		return AabbNodeTest;
	}
}

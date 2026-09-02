namespace Fusion;

internal interface IBoundsTraversalTest
{
	bool Check(ref BVHNode.CachedBounds bounds);
}

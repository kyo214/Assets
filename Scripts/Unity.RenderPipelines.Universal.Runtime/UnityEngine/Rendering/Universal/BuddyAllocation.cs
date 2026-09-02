using Unity.Mathematics;

namespace UnityEngine.Rendering.Universal;

internal struct BuddyAllocation(int level, int index)
{
	public int level = level;

	public int index = index;

	public uint2 index2D => SpaceFillingCurves.DecodeMorton2D((uint)index);
}

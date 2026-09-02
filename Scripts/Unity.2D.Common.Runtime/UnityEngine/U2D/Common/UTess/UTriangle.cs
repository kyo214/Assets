using Unity.Mathematics;

namespace UnityEngine.U2D.Common.UTess;

internal struct UTriangle
{
	public float2 va;

	public float2 vb;

	public float2 vc;

	public UCircle c;

	public float area;

	public int3 indices;
}

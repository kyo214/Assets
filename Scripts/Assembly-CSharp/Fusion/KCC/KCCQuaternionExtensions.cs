using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion.KCC;

public static class KCCQuaternionExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNaN(this Quaternion quaternion)
	{
		if (!float.IsNaN(quaternion.x) && !float.IsNaN(quaternion.y) && !float.IsNaN(quaternion.z))
		{
			return float.IsNaN(quaternion.w);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsZero(this Quaternion quaternion)
	{
		if (quaternion.x == 0f && quaternion.y == 0f && quaternion.z == 0f)
		{
			return quaternion.w == 0f;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsEqual(this Quaternion quaternion, Quaternion other)
	{
		if (quaternion.x == other.x && quaternion.y == other.y && quaternion.z == other.z)
		{
			return quaternion.w == other.w;
		}
		return false;
	}
}

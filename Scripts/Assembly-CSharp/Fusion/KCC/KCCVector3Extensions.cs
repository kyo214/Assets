using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion.KCC;

public static class KCCVector3Extensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 OnlyX(this Vector3 vector)
	{
		vector.y = 0f;
		vector.z = 0f;
		return vector;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 OnlyY(this Vector3 vector)
	{
		vector.x = 0f;
		vector.z = 0f;
		return vector;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 OnlyZ(this Vector3 vector)
	{
		vector.x = 0f;
		vector.y = 0f;
		return vector;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 OnlyXY(this Vector3 vector)
	{
		vector.z = 0f;
		return vector;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 OnlyXZ(this Vector3 vector)
	{
		vector.y = 0f;
		return vector;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 OnlyYZ(this Vector3 vector)
	{
		vector.x = 0f;
		return vector;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 XZ0(this Vector3 vector)
	{
		return new Vector3(vector.x, vector.z, 0f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 X0Y(this Vector3 vector)
	{
		return new Vector3(vector.x, 0f, vector.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNaN(this Vector3 vector)
	{
		if (!float.IsNaN(vector.x) && !float.IsNaN(vector.y))
		{
			return float.IsNaN(vector.z);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsZero(this Vector3 vector)
	{
		if (vector.x == 0f && vector.y == 0f)
		{
			return vector.z == 0f;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsAlmostZero(this Vector3 vector, float tolerance = 0.01f)
	{
		if (vector.x < tolerance && vector.x > 0f - tolerance && vector.y < tolerance && vector.y > 0f - tolerance && vector.z < tolerance)
		{
			return vector.z > 0f - tolerance;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsEqual(this Vector3 vector, Vector3 other)
	{
		if (vector.x == other.x && vector.y == other.y)
		{
			return vector.z == other.z;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool AlmostEquals(this Vector3 vectorA, Vector3 vectorB, float tolerance = 0.01f)
	{
		return (vectorA - vectorB).IsAlmostZero(tolerance);
	}
}

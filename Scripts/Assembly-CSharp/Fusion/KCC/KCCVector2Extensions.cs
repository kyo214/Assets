using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion.KCC;

public static class KCCVector2Extensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 X0Y(this Vector2 vector)
	{
		return new Vector3(vector.x, 0f, vector.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsNaN(this Vector2 vector)
	{
		if (!float.IsNaN(vector.x))
		{
			return float.IsNaN(vector.y);
		}
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsZero(this Vector2 vector)
	{
		if (vector.x == 0f)
		{
			return vector.y == 0f;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsAlmostZero(this Vector2 vector, float tolerance = 0.01f)
	{
		if (vector.x < tolerance && vector.x > 0f - tolerance && vector.y < tolerance)
		{
			return vector.y > 0f - tolerance;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsEqual(this Vector2 vector, Vector2 other)
	{
		if (vector.x == other.x)
		{
			return vector.y == other.y;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool AlmostEquals(this Vector2 vectorA, Vector2 vectorB, float tolerance = 0.01f)
	{
		return (vectorA - vectorB).IsAlmostZero(tolerance);
	}
}

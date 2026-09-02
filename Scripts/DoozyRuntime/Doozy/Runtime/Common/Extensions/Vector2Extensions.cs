using System;
using UnityEngine;

namespace Doozy.Runtime.Common.Extensions;

public static class Vector2Extensions
{
	public static Vector2 Round(this Vector2 target, int decimals = 1)
	{
		return new Vector2((float)Math.Round(target.x, decimals), (float)Math.Round(target.y, decimals));
	}

	public static Vector2 Clamp(this Vector2 target, Vector2 min, Vector2 max)
	{
		target.x = Mathf.Clamp(target.x, min.x, max.x);
		target.y = Mathf.Clamp(target.y, min.y, max.y);
		return target;
	}

	public static Vector2 Clamp01(this Vector2 target)
	{
		target.x = Mathf.Clamp01(target.x);
		target.y = Mathf.Clamp01(target.y);
		return target;
	}

	public static bool Approximately(this Vector2 target, Vector2 other)
	{
		if (Mathf.Approximately(target.x, other.x))
		{
			return Mathf.Approximately(target.y, other.y);
		}
		return false;
	}

	public static float InverseLerp(Vector2 a, Vector2 b, Vector2 value)
	{
		Vector2 vector = b - a;
		float num = Vector3.Dot(value - a, vector);
		float num2 = Vector3.Dot(vector, vector);
		if (num2 == 0f)
		{
			return 0f;
		}
		return num / num2;
	}
}

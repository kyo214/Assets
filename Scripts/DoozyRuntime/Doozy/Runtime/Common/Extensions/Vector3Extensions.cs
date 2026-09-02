using System;
using UnityEngine;

namespace Doozy.Runtime.Common.Extensions;

public static class Vector3Extensions
{
	public static Vector3 Round(this Vector3 target, int decimals = 1)
	{
		return new Vector3((float)Math.Round(target.x, decimals), (float)Math.Round(target.y, decimals), (float)Math.Round(target.z, decimals));
	}

	public static Vector3 Clamp(this Vector3 target, Vector3 min, Vector3 max)
	{
		target.x = Mathf.Clamp(target.x, min.x, max.x);
		target.y = Mathf.Clamp(target.y, min.y, max.y);
		target.z = Mathf.Clamp(target.z, min.z, max.z);
		return target;
	}

	public static Vector3 Clamp01(this Vector3 target)
	{
		target.x = Mathf.Clamp01(target.x);
		target.y = Mathf.Clamp01(target.y);
		target.z = Mathf.Clamp01(target.z);
		return target;
	}

	public static bool Approximately(this Vector3 target, Vector3 other)
	{
		if (Mathf.Approximately(target.x, other.x) && Mathf.Approximately(target.y, other.y))
		{
			return Mathf.Approximately(target.z, other.z);
		}
		return false;
	}

	public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
	{
		Vector3 vector = b - a;
		float num = Vector3.Dot(value - a, vector);
		float num2 = Vector3.Dot(vector, vector);
		if (num2 == 0f)
		{
			return 0f;
		}
		return num / num2;
	}
}

using System;
using UnityEngine;

namespace Doozy.Runtime.Common.Extensions;

public static class FloatExtensions
{
	public const float k_Tolerance = 0.0001f;

	public static float Round(this float target, int decimals = 1)
	{
		return (float)Math.Round(target, decimals);
	}

	public static float Clamp(this float target, float min, float max)
	{
		return Mathf.Clamp(target, min, max);
	}

	public static float Clamp01(this float target)
	{
		return Mathf.Clamp01(target);
	}

	public static bool Approximately(this float target, float otherValue)
	{
		return target.Approximately(otherValue, 0.0001f);
	}

	public static bool Approximately(this float target, float otherValue, float tolerance)
	{
		return Mathf.Abs(target - otherValue) < tolerance;
	}

	public static float Abs(this float target)
	{
		return Mathf.Abs(target);
	}

	public static float RoundToMultiple(this float target, float multiple)
	{
		float num = target % multiple;
		if (num < 0.5f)
		{
			return target - num;
		}
		return target + multiple - num;
	}

	public static float RoundToMultiple(this float target, int multiple)
	{
		return target.RoundToMultiple((float)multiple);
	}

	public static float RoundToMultiple(this float target, float multiple, float offset)
	{
		float num = target % multiple;
		if (num < offset)
		{
			return target - num;
		}
		return target + multiple - num;
	}

	public static bool CloseTo(this float target, float otherValue, float tolerance)
	{
		return Mathf.Abs(target - otherValue) <= tolerance;
	}
}

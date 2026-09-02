using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion.KCC;

public static class KCCMathUtility
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Map(float inMin, float inMax, float outMin, float outMax, float value)
	{
		if (value <= inMin)
		{
			return outMin;
		}
		if (value >= inMax)
		{
			return outMax;
		}
		return (outMax - outMin) * ((value - inMin) / (inMax - inMin)) + outMin;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float EasyIn2(float t)
	{
		return t * t;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float EasyIn3(float t)
	{
		return t * t * t;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float EasyIn4(float t)
	{
		return t * t * t * t;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float EasyOut2(float t)
	{
		t = 1f - t;
		return 1f - t * t;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float EasyOut3(float t)
	{
		t = 1f - t;
		return 1f - t * t * t;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float EasyOut4(float t)
	{
		t = 1f - t;
		return 1f - t * t * t * t;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float EasyInOut2(float t)
	{
		t *= 2f;
		if (t <= 1f)
		{
			t = 0.5f * t * t;
		}
		else
		{
			t--;
			t = -0.5f * (t * (t - 2f) - 1f);
		}
		return t;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float EasyInOut3(float t)
	{
		t *= 2f;
		if (t <= 1f)
		{
			t = 0.5f * t * t * t;
		}
		else
		{
			t -= 2f;
			t = 0.5f * (t * t * t + 2f);
		}
		return t;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float EasyInOut4(float t)
	{
		t *= 2f;
		if (t <= 1f)
		{
			t = 0.5f * t * t * t * t;
		}
		else
		{
			t -= 2f;
			t = -0.5f * (t * t * t * t - 2f);
		}
		return t;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float FastAtan(float x)
	{
		return (0.9723941f - 0.19194795f * x * x) * x;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float FastAtan2(float y, float x)
	{
		if (x != 0f)
		{
			float num = ((x >= 0f) ? x : (0f - x));
			float num2 = ((y >= 0f) ? y : (0f - y));
			if (num > num2)
			{
				if (x > 0f)
				{
					return FastAtan(y / x);
				}
				if (y >= 0f)
				{
					return FastAtan(y / x) + MathF.PI;
				}
				return FastAtan(y / x) - MathF.PI;
			}
			if (y > 0f)
			{
				return 0f - FastAtan(x / y) + MathF.PI / 2f;
			}
			return 0f - FastAtan(x / y) - MathF.PI / 2f;
		}
		if (y > 0f)
		{
			return MathF.PI / 2f;
		}
		if (y < 0f)
		{
			return -MathF.PI / 2f;
		}
		return 0f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float InterpolateRange(float from, float to, float min, float max, float alpha)
	{
		float num = max - min;
		if (num <= 0f)
		{
			throw new ArgumentException("max must be greater than min!");
		}
		if (from < min)
		{
			from = min;
		}
		else if (from > max)
		{
			from = max;
		}
		if (to < min)
		{
			to = min;
		}
		else if (to > max)
		{
			to = max;
		}
		if (from == to)
		{
			return from;
		}
		float num2 = num * 0.5f;
		float num3;
		if (from < to)
		{
			if (to - from <= num2)
			{
				num3 = Mathf.Lerp(from, to, alpha);
			}
			else
			{
				num3 = Mathf.Lerp(from + num, to, alpha);
				if (num3 > max)
				{
					num3 -= num;
				}
			}
		}
		else if (from - to <= num2)
		{
			num3 = Mathf.Lerp(from, to, alpha);
		}
		else
		{
			num3 = Mathf.Lerp(from - num, to, alpha);
			if (num3 <= min)
			{
				num3 += num;
			}
		}
		return num3;
	}
}

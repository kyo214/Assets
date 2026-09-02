using System;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Easings;

public static class EasingFunction
{
	private const float PI = MathF.PI;

	private const float HALF_PI = MathF.PI / 2f;

	private const float TWO_PI = MathF.PI * 2f;

	private const float C1 = 1.70158f;

	private const float C2 = 2.5949094f;

	private const float C3 = 2.70158f;

	private const float C4 = MathF.PI * 2f / 3f;

	private const float C5 = MathF.PI * 4f / 9f;

	public static float LinearInterpolation(float progress)
	{
		progress = Mathf.Clamp01(progress);
		return progress;
	}

	public static float QuadraticEaseIn(float progress)
	{
		progress = Mathf.Clamp01(progress);
		return Mathf.Pow(progress, 2f);
	}

	public static float QuadraticEaseOut(float progress)
	{
		progress = Mathf.Clamp01(progress);
		return 0f - progress * (progress - 2f);
	}

	public static float QuadraticEaseInOut(float progress)
	{
		progress = Mathf.Clamp01(progress);
		if (progress < 0.5f)
		{
			return 2f * Mathf.Pow(progress, 2f);
		}
		return -2f * Mathf.Pow(progress, 2f) + 4f * progress - 1f;
	}

	public static float CubicEaseIn(float progress)
	{
		progress = Mathf.Clamp01(progress);
		return Mathf.Pow(progress, 3f);
	}

	public static float CubicEaseOut(float progress)
	{
		progress = Mathf.Clamp01(progress);
		return Mathf.Pow(progress - 1f, 3f) + 1f;
	}

	public static float CubicEaseInOut(float progress)
	{
		progress = Mathf.Clamp01(progress);
		if (progress < 0.5f)
		{
			return 4f * Mathf.Pow(progress, 3f);
		}
		float f = 2f * progress - 2f;
		return 0.5f * Mathf.Pow(f, 3f) + 1f;
	}

	public static float QuarticEaseIn(float progress)
	{
		progress = Mathf.Clamp01(progress);
		return Mathf.Pow(progress, 4f);
	}

	public static float QuarticEaseOut(float progress)
	{
		progress = Mathf.Clamp01(progress);
		return Mathf.Pow(progress - 1f, 3f) * (1f - progress) + 1f;
	}

	public static float QuarticEaseInOut(float progress)
	{
		progress = Mathf.Clamp01(progress);
		if (progress < 0.5f)
		{
			return 8f * Mathf.Pow(progress, 4f);
		}
		float f = progress - 1f;
		return -8f * Mathf.Pow(f, 4f) + 1f;
	}

	public static float QuinticEaseIn(float progress)
	{
		progress = Mathf.Clamp01(progress);
		return Mathf.Pow(progress, 5f);
	}

	public static float QuinticEaseOut(float progress)
	{
		progress = Mathf.Clamp01(progress);
		return Mathf.Pow(progress - 1f, 5f) + 1f;
	}

	public static float QuinticEaseInOut(float progress)
	{
		progress = Mathf.Clamp01(progress);
		if (progress < 0.5f)
		{
			return 16f * Mathf.Pow(progress, 5f);
		}
		float f = 2f * progress - 2f;
		return 0.5f * Mathf.Pow(f, 5f) + 1f;
	}

	public static float SineEaseIn(float progress)
	{
		progress = Mathf.Clamp01(progress);
		return Mathf.Sin((progress - 1f) * (MathF.PI / 2f)) + 1f;
	}

	public static float SineEaseOut(float progress)
	{
		progress = Mathf.Clamp01(progress);
		return Mathf.Sin(progress * (MathF.PI / 2f));
	}

	public static float SineEaseInOut(float progress)
	{
		progress = Mathf.Clamp01(progress);
		return 0.5f * (1f - Mathf.Cos(progress * MathF.PI));
	}

	public static float CircularEaseIn(float progress)
	{
		progress = Mathf.Clamp01(progress);
		return 1f - Mathf.Sqrt(1f - Mathf.Pow(progress, 2f));
	}

	public static float CircularEaseOut(float progress)
	{
		progress = Mathf.Clamp01(progress);
		return Mathf.Sqrt((2f - progress) * progress);
	}

	public static float CircularEaseInOut(float progress)
	{
		progress = Mathf.Clamp01(progress);
		if (progress < 0.5f)
		{
			return 0.5f * (1f - Mathf.Sqrt(1f - 4f * Mathf.Pow(progress, 2f)));
		}
		return 0.5f * (Mathf.Sqrt((0f - (2f * progress - 3f)) * (2f * progress - 1f)) + 1f);
	}

	public static float ExponentialEaseIn(float progress)
	{
		progress = Mathf.Clamp01(progress);
		if (!Mathf.Approximately(progress, 0f))
		{
			return Mathf.Pow(2f, 10f * (progress - 1f));
		}
		return progress;
	}

	public static float ExponentialEaseOut(float progress)
	{
		progress = Mathf.Clamp01(progress);
		if (!Mathf.Approximately(progress, 1f))
		{
			return 1f - Mathf.Pow(2f, -10f * progress);
		}
		return progress;
	}

	public static float ExponentialEaseInOut(float progress)
	{
		progress = Mathf.Clamp01(progress);
		if (Mathf.Approximately(progress, 0f) || Mathf.Approximately(progress, 1f))
		{
			return progress;
		}
		if (progress < 0.5f)
		{
			return 0.5f * Mathf.Pow(2f, 20f * progress - 10f);
		}
		return -0.5f * Mathf.Pow(2f, -20f * progress + 10f) + 1f;
	}

	public static float ElasticEaseIn(float progress)
	{
		progress = Mathf.Clamp01(progress);
		return Mathf.Sin(20.420353f * progress) * Mathf.Pow(2f, 10f * (progress - 1f));
	}

	public static float ElasticEaseOut(float progress)
	{
		progress = Mathf.Clamp01(progress);
		return Mathf.Sin(-20.420353f * (progress + 1f)) * Mathf.Pow(2f, -10f * progress) + 1f;
	}

	public static float ElasticEaseInOut(float progress)
	{
		progress = Mathf.Clamp01(progress);
		if ((double)progress < 0.5)
		{
			return 0.5f * Mathf.Sin(20.420353f * (2f * progress)) * Mathf.Pow(2f, 10f * (2f * progress - 1f));
		}
		return 0.5f * (Mathf.Sin(-20.420353f * (2f * progress - 1f + 1f)) * Mathf.Pow(2f, -10f * (2f * progress - 1f)) + 2f);
	}

	public static float BackEaseIn(float progress)
	{
		progress = Mathf.Clamp01(progress);
		return Mathf.Pow(progress, 3f) - progress * Mathf.Sin(progress * MathF.PI);
	}

	public static float BackEaseOut(float progress)
	{
		progress = Mathf.Clamp01(progress);
		float num = 1f - progress;
		return 1f - (Mathf.Pow(num, 3f) - num * Mathf.Sin(num * MathF.PI));
	}

	public static float BackEaseInOut(float progress)
	{
		progress = Mathf.Clamp01(progress);
		if (progress < 0.5f)
		{
			float num = 2f * progress;
			return 0.5f * (Mathf.Pow(num, 3f) - num * Mathf.Sin(num * MathF.PI));
		}
		float num2 = 1f - (2f * progress - 1f);
		return 0.5f * (1f - (Mathf.Pow(num2, 3f) - num2 * Mathf.Sin(num2 * MathF.PI))) + 0.5f;
	}

	public static float BounceEaseIn(float progress)
	{
		progress = Mathf.Clamp01(progress);
		return 1f - BounceEaseOut(1f - progress);
	}

	public static float BounceEaseOut(float progress)
	{
		progress = Mathf.Clamp01(progress);
		if (progress < 372f / 1023f)
		{
			return 121f * Mathf.Pow(progress, 2f) / 16f;
		}
		if (progress < 744f / 1023f)
		{
			return 9.075f * Mathf.Pow(progress, 2f) - 9.9f * progress + 3.4f;
		}
		if ((double)progress < 0.9)
		{
			return 12.066482f * Mathf.Pow(progress, 2f) - 19.635458f * progress + 8.898061f;
		}
		return 10.8f * Mathf.Pow(progress, 2f) - 20.52f * progress + 10.72f;
	}

	public static float BounceEaseInOut(float progress)
	{
		progress = Mathf.Clamp01(progress);
		if (progress < 0.5f)
		{
			return 0.5f * BounceEaseIn(progress * 2f);
		}
		return 0.5f * BounceEaseOut(progress * 2f - 1f) + 0.5f;
	}
}

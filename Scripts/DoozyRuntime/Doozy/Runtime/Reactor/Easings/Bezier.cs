using System;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Easings;

[Serializable]
public class Bezier : IEasing
{
	[SerializeField]
	private float Ax;

	[SerializeField]
	private float Ay;

	[SerializeField]
	private float Bx;

	[SerializeField]
	private float By;

	public Vector2 a => new Vector2(ax, ay);

	public float ax => Ax;

	public float ay => Ay;

	public Vector2 b => new Vector2(bx, By);

	public float bx => Bx;

	public float by => By;

	public Bezier(float ax, float ay, float bx, float by)
	{
		Ax = ax;
		Ay = ay;
		Bx = bx;
		By = by;
	}

	public Bezier(Vector2 a, Vector2 b)
		: this(a.x, a.y, b.x, b.y)
	{
	}

	public Bezier(Bezier other)
		: this(other.ax, other.ay, other.bx, other.by)
	{
	}

	public float Evaluate(float progress)
	{
		return Evaluate(a, b, Mathf.Clamp01(progress));
	}

	private static float A(float x, float y)
	{
		return 1f - 3f * y + 3f * x;
	}

	private static float B(float x, float y)
	{
		return 3f * y - 6f * x;
	}

	private static float C(float x)
	{
		return 3f * x;
	}

	private static float CalcBezier(float t, float a, float b)
	{
		return ((A(a, b) * t + B(a, b)) * t + C(a)) * t;
	}

	private static float GetSlope(float t, float a, float b)
	{
		return 3f * A(a, b) * t * t + 2f * B(a, b) * t + C(a);
	}

	private static float CalculateTime(float t, float a, float b)
	{
		float num = t;
		for (int i = 0; i < 4; i++)
		{
			float slope = GetSlope(num, a, b);
			if (slope == 0f)
			{
				return num;
			}
			float num2 = CalcBezier(num, a, b) - t;
			num -= num2 / slope;
		}
		return num;
	}

	private static float Calculate(float ax, float ay, float bx, float by, float t)
	{
		if (Mathf.Approximately(ax, ay) && Mathf.Approximately(bx, by))
		{
			return t;
		}
		return CalcBezier(CalculateTime(t, ax, bx), ay, by);
	}

	public static float Evaluate(float ax, float ay, float bx, float by, float t)
	{
		return Calculate(ax, ay, bx, by, t);
	}

	public static float Evaluate(Vector2 a, Vector2 b, float t)
	{
		return Calculate(a.x, a.y, b.x, b.y, t);
	}

	public static float Evaluate(Bezier b, float t)
	{
		return Evaluate(b.a.x, b.a.y, b.b.x, b.b.y, t);
	}

	public static float Evaluate(float progress, Ease ease)
	{
		progress = Mathf.Clamp01(progress);
		return ease switch
		{
			Ease.Linear => Evaluate(0f, 0f, 1f, 1f, progress), 
			Ease.Easy => Evaluate(0.25f, 0.1f, 0.25f, 1f, progress), 
			Ease.InEasy => Evaluate(0.42f, 0f, 1f, 1f, progress), 
			Ease.OutEasy => Evaluate(0f, 0f, 0.58f, 1f, progress), 
			Ease.InOutEasy => Evaluate(0.42f, 0f, 0.58f, 1f, progress), 
			Ease.InSine => Evaluate(0.47f, 0f, 0.745f, 0.715f, progress), 
			Ease.OutSine => Evaluate(0.39f, 0.575f, 0.565f, 1f, progress), 
			Ease.InOutSine => Evaluate(0.445f, 0.05f, 0.55f, 0.95f, progress), 
			Ease.InQuad => Evaluate(0.55f, 0.085f, 0.68f, 0.53f, progress), 
			Ease.OutQuad => Evaluate(0.25f, 0.46f, 0.45f, 0.94f, progress), 
			Ease.InOutQuad => Evaluate(0.455f, 0.03f, 0.515f, 0.955f, progress), 
			Ease.InCubic => Evaluate(0.55f, 0.055f, 0.675f, 0.19f, progress), 
			Ease.OutCubic => Evaluate(0.215f, 0.61f, 0.355f, 1f, progress), 
			Ease.InOutCubic => Evaluate(0.645f, 0.045f, 0.355f, 1f, progress), 
			Ease.InQuart => Evaluate(0.895f, 0.03f, 0.685f, 0.22f, progress), 
			Ease.OutQuart => Evaluate(0.165f, 0.84f, 0.44f, 1f, progress), 
			Ease.InOutQuart => Evaluate(0.77f, 0f, 0.175f, 1f, progress), 
			Ease.InQuint => Evaluate(0.755f, 0.05f, 0.855f, 0.06f, progress), 
			Ease.OutQuint => Evaluate(0.23f, 1f, 0.32f, 1f, progress), 
			Ease.InOutQuint => Evaluate(0.86f, 0f, 0.07f, 1f, progress), 
			Ease.InExpo => Evaluate(0.95f, 0.05f, 0.795f, 0.035f, progress), 
			Ease.OutExpo => Evaluate(0.19f, 1f, 0.22f, 1f, progress), 
			Ease.InOutExpo => Evaluate(1f, 0f, 0f, 1f, progress), 
			Ease.InCirc => Evaluate(0.6f, 0.04f, 0.98f, 0.335f, progress), 
			Ease.OutCirc => Evaluate(0.075f, 0.82f, 0.165f, 1f, progress), 
			Ease.InOutCirc => Evaluate(0.785f, 0.135f, 0.15f, 0.86f, progress), 
			Ease.InBack => Evaluate(0.8f, -0.4f, 0f, 1f, progress), 
		};
	}
}

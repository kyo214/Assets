using UnityEngine;

namespace Doozy.Runtime.Reactor.Easings.Eases;

public class CubicEaseOutEasing : IEasing
{
	private static float Calculate(float start, float end, float progress)
	{
		return Mathf.LerpUnclamped(start, end, EasingFunction.CubicEaseOut(progress));
	}

	public float Evaluate(float progress)
	{
		return Calculate(0f, 1f, Mathf.Clamp01(progress));
	}
}

using UnityEngine;

namespace Doozy.Runtime.Reactor.Easings.Eases;

public class ExponentialEaseOutEasing : IEasing
{
	private static float Calculate(float start, float end, float progress)
	{
		return Mathf.LerpUnclamped(start, end, EasingFunction.ExponentialEaseOut(progress));
	}

	public float Evaluate(float progress)
	{
		return Calculate(0f, 1f, Mathf.Clamp01(progress));
	}
}

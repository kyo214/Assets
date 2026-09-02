using UnityEngine;

namespace Doozy.Runtime.Reactor.Easings.Eases;

public class BackEaseInEasing : IEasing
{
	private static float Calculate(float start, float end, float progress)
	{
		return Mathf.LerpUnclamped(start, end, EasingFunction.BackEaseIn(progress));
	}

	public float Evaluate(float progress)
	{
		return Calculate(0f, 1f, Mathf.Clamp01(progress));
	}
}

using UnityEngine;

namespace Doozy.Runtime.Reactor.Easings.Eases;

public class EasyEaseInEasing : IEasing
{
	private static float Calculate(float start, float end, float progress)
	{
		return Mathf.LerpUnclamped(start, end, Bezier.Evaluate(progress, Ease.InEasy));
	}

	public float Evaluate(float progress)
	{
		return Calculate(0f, 1f, Mathf.Clamp01(progress));
	}
}

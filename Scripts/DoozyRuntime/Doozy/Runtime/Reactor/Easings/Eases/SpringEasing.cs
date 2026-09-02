using System;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Easings.Eases;

public class SpringEasing : IEasing
{
	private static float Calculate(float start, float end, float progress)
	{
		progress = Mathf.Clamp01(progress);
		progress = (Mathf.Sin(progress * MathF.PI * (0.2f + 2.5f * Mathf.Pow(progress, 3f))) * Mathf.Pow(1f - progress, 2.2f) + progress) * (1f + 1.2f * (1f - progress));
		return Mathf.LerpUnclamped(start, end, progress);
	}

	public float Evaluate(float progress)
	{
		return Calculate(0f, 1f, Mathf.Clamp01(progress));
	}
}

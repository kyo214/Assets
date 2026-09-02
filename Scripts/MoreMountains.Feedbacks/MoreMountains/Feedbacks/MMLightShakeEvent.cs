using System.Runtime.InteropServices;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MMLightShakeEvent
{
	public delegate void Delegate(float shakeDuration, bool relativeValues, bool modifyColor, Gradient colorOverTime, AnimationCurve intensityCurve, float remapIntensityZero, float remapIntensityOne, AnimationCurve rangeCurve, float remapRangeZero, float remapRangeOne, AnimationCurve shadowStrengthCurve, float remapShadowStrengthZero, float remapShadowStrengthOne, float feedbacksIntensity = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool useRange = false, float eventRange = 0f, Vector3 eventOriginPosition = default(Vector3));

	private static event Delegate OnEvent;

	public static void Register(Delegate callback)
	{
		OnEvent += callback;
	}

	public static void Unregister(Delegate callback)
	{
		OnEvent -= callback;
	}

	public static void Trigger(float shakeDuration, bool relativeValues, bool modifyColor, Gradient colorOverTime, AnimationCurve intensityCurve, float remapIntensityZero, float remapIntensityOne, AnimationCurve rangeCurve, float remapRangeZero, float remapRangeOne, AnimationCurve shadowStrengthCurve, float remapShadowStrengthZero, float remapShadowStrengthOne, float feedbacksIntensity = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool useRange = false, float eventRange = 0f, Vector3 eventOriginPosition = default(Vector3))
	{
		OnEvent?.Invoke(shakeDuration, relativeValues, modifyColor, colorOverTime, intensityCurve, remapIntensityZero, remapIntensityOne, rangeCurve, remapRangeZero, remapRangeOne, shadowStrengthCurve, remapShadowStrengthZero, remapShadowStrengthOne, feedbacksIntensity, channel, resetShakerValuesAfterShake, resetTargetValuesAfterShake, useRange, eventRange, eventOriginPosition);
	}
}

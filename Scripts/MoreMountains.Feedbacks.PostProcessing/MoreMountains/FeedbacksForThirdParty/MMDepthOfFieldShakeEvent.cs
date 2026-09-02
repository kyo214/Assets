using System.Runtime.InteropServices;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MMDepthOfFieldShakeEvent
{
	public delegate void Delegate(AnimationCurve focusDistance, float duration, float remapFocusDistanceMin, float remapFocusDistanceMax, AnimationCurve aperture, float remapApertureMin, float remapApertureMax, AnimationCurve focalLength, float remapFocalLengthMin, float remapFocalLengthMax, bool relativeValues = false, float feedbacksIntensity = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false);

	private static event Delegate OnEvent;

	public static void Register(Delegate callback)
	{
		OnEvent += callback;
	}

	public static void Unregister(Delegate callback)
	{
		OnEvent -= callback;
	}

	public static void Trigger(AnimationCurve focusDistance, float duration, float remapFocusDistanceMin, float remapFocusDistanceMax, AnimationCurve aperture, float remapApertureMin, float remapApertureMax, AnimationCurve focalLength, float remapFocalLengthMin, float remapFocalLengthMax, bool relativeValues = false, float feedbacksIntensity = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
	{
		OnEvent?.Invoke(focusDistance, duration, remapFocusDistanceMin, remapFocusDistanceMax, aperture, remapApertureMin, remapApertureMax, focalLength, remapFocalLengthMin, remapFocalLengthMax, relativeValues, feedbacksIntensity, channel, resetShakerValuesAfterShake, resetTargetValuesAfterShake, forwardDirection, timescaleMode, stop);
	}
}

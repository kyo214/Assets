using System.Runtime.InteropServices;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MMColorAdjustmentsShakeEvent_HDRP
{
	public delegate void Delegate(AnimationCurve shakePostExposure, float remapPostExposureZero, float remapPostExposureOne, AnimationCurve shakeHueShift, float remapHueShiftZero, float remapHueShiftOne, AnimationCurve shakeSaturation, float remapSaturationZero, float remapSaturationOne, AnimationCurve shakeContrast, float remapContrastZero, float remapContrastOne, MMColorAdjustmentsShaker_HDRP.ColorFilterModes colorFilterMode, Gradient colorFilterGradient, Color colorFilterDestination, AnimationCurve colorFilterCurve, float duration, bool relativeValues = false, float attenuation = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false);

	private static event Delegate OnEvent;

	public static void Register(Delegate callback)
	{
		OnEvent += callback;
	}

	public static void Unregister(Delegate callback)
	{
		OnEvent -= callback;
	}

	public static void Trigger(AnimationCurve shakePostExposure, float remapPostExposureZero, float remapPostExposureOne, AnimationCurve shakeHueShift, float remapHueShiftZero, float remapHueShiftOne, AnimationCurve shakeSaturation, float remapSaturationZero, float remapSaturationOne, AnimationCurve shakeContrast, float remapContrastZero, float remapContrastOne, MMColorAdjustmentsShaker_HDRP.ColorFilterModes colorFilterMode, Gradient colorFilterGradient, Color colorFilterDestination, AnimationCurve colorFilterCurve, float duration, bool relativeValues = false, float attenuation = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
	{
		OnEvent?.Invoke(shakePostExposure, remapPostExposureZero, remapPostExposureOne, shakeHueShift, remapHueShiftZero, remapHueShiftOne, shakeSaturation, remapSaturationZero, remapSaturationOne, shakeContrast, remapContrastZero, remapContrastOne, colorFilterMode, colorFilterGradient, colorFilterDestination, colorFilterCurve, duration, relativeValues, attenuation, channel, resetShakerValuesAfterShake, resetTargetValuesAfterShake, forwardDirection, timescaleMode, stop);
	}
}

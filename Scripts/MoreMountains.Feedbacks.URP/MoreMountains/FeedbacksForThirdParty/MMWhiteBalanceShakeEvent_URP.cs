using System.Runtime.InteropServices;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MMWhiteBalanceShakeEvent_URP
{
	public delegate void Delegate(AnimationCurve temperature, float duration, float remapTemperatureMin, float remapTemperatureMax, AnimationCurve tint, float remapTintMin, float remapTintMax, bool relativeValues = false, float attenuation = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false);

	private static event Delegate OnEvent;

	public static void Register(Delegate callback)
	{
		OnEvent += callback;
	}

	public static void Unregister(Delegate callback)
	{
		OnEvent -= callback;
	}

	public static void Trigger(AnimationCurve temperature, float duration, float remapTemperatureMin, float remapTemperatureMax, AnimationCurve tint, float remapTintMin, float remapTintMax, bool relativeValues = false, float attenuation = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
	{
		OnEvent?.Invoke(temperature, duration, remapTemperatureMin, remapTemperatureMax, tint, remapTintMin, remapTintMax, relativeValues, attenuation, channel, resetShakerValuesAfterShake, resetTargetValuesAfterShake, forwardDirection, timescaleMode, stop);
	}
}

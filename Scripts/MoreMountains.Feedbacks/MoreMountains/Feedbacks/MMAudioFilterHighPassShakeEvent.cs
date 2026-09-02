using System.Runtime.InteropServices;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MMAudioFilterHighPassShakeEvent
{
	public delegate void Delegate(AnimationCurve highPassCurve, float duration, float remapMin, float remapMax, bool relativeHighPass = false, float feedbacksIntensity = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false);

	private static event Delegate OnEvent;

	public static void Register(Delegate callback)
	{
		OnEvent += callback;
	}

	public static void Unregister(Delegate callback)
	{
		OnEvent -= callback;
	}

	public static void Trigger(AnimationCurve highPassCurve, float duration, float remapMin, float remapMax, bool relativeHighPass = false, float feedbacksIntensity = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
	{
		OnEvent?.Invoke(highPassCurve, duration, remapMin, remapMax, relativeHighPass, feedbacksIntensity, channel, resetShakerValuesAfterShake, resetTargetValuesAfterShake, forwardDirection, timescaleMode, stop);
	}
}

using System.Runtime.InteropServices;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MMAudioSourceStereoPanShakeEvent
{
	public delegate void Delegate(AnimationCurve stereoPanCurve, float duration, float remapMin, float remapMax, bool relativeStereoPan = false, float feedbacksIntensity = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false);

	private static event Delegate OnEvent;

	public static void Register(Delegate callback)
	{
		OnEvent += callback;
	}

	public static void Unregister(Delegate callback)
	{
		OnEvent -= callback;
	}

	public static void Trigger(AnimationCurve stereoPanCurve, float duration, float remapMin, float remapMax, bool relativeStereoPan = false, float feedbacksIntensity = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
	{
		OnEvent?.Invoke(stereoPanCurve, duration, remapMin, remapMax, relativeStereoPan, feedbacksIntensity, channel, resetShakerValuesAfterShake, resetTargetValuesAfterShake, forwardDirection, timescaleMode, stop);
	}
}

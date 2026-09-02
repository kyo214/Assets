using System.Runtime.InteropServices;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MMExposureShakeEvent_HDRP
{
	public delegate void Delegate(AnimationCurve fixedExposure, float duration, float remapMin, float remapMax, bool relativeFixedExposure = false, float attenuation = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false);

	private static event Delegate OnEvent;

	public static void Register(Delegate callback)
	{
		OnEvent += callback;
	}

	public static void Unregister(Delegate callback)
	{
		OnEvent -= callback;
	}

	public static void Trigger(AnimationCurve fixedExposure, float duration, float remapMin, float remapMax, bool relativeFixedExposure = false, float attenuation = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
	{
		OnEvent?.Invoke(fixedExposure, duration, remapMin, remapMax, relativeFixedExposure, attenuation, channel, resetShakerValuesAfterShake, resetTargetValuesAfterShake, forwardDirection, timescaleMode, stop);
	}
}

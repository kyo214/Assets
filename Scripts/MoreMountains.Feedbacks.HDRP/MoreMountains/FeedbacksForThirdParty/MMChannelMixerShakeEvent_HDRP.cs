using System.Runtime.InteropServices;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MMChannelMixerShakeEvent_HDRP
{
	public delegate void Delegate(AnimationCurve shakeRed, float remapRedZero, float remapRedOne, AnimationCurve shakeGreen, float remapGreenZero, float remapGreenOne, AnimationCurve shakeBlue, float remapBlueZero, float remapBlueOne, float duration, bool relativeValues = false, float attenuation = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false);

	private static event Delegate OnEvent;

	public static void Register(Delegate callback)
	{
		OnEvent += callback;
	}

	public static void Unregister(Delegate callback)
	{
		OnEvent -= callback;
	}

	public static void Trigger(AnimationCurve shakeRed, float remapRedZero, float remapRedOne, AnimationCurve shakeGreen, float remapGreenZero, float remapGreenOne, AnimationCurve shakeBlue, float remapBlueZero, float remapBlueOne, float duration, bool relativeValues = false, float attenuation = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
	{
		OnEvent?.Invoke(shakeRed, remapRedZero, remapRedOne, shakeGreen, remapGreenZero, remapGreenOne, shakeBlue, remapBlueZero, remapBlueOne, duration, relativeValues, attenuation, channel, resetShakerValuesAfterShake, resetTargetValuesAfterShake, forwardDirection, timescaleMode, stop);
	}
}

using System.Runtime.InteropServices;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct MMSpriteRendererShakeEvent
{
	public delegate void Delegate(float shakeDuration, bool modifyColor, Gradient colorOverTime, bool flipX, bool flipY, float feedbacksIntensity = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool useRange = false, float eventRange = 0f, Vector3 eventOriginPosition = default(Vector3));

	private static event Delegate OnEvent;

	public static void Register(Delegate callback)
	{
		OnEvent += callback;
	}

	public static void Unregister(Delegate callback)
	{
		OnEvent -= callback;
	}

	public static void Trigger(float shakeDuration, bool modifyColor, Gradient colorOverTime, bool flipX, bool flipY, float feedbacksIntensity = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool useRange = false, float eventRange = 0f, Vector3 eventOriginPosition = default(Vector3))
	{
		OnEvent?.Invoke(shakeDuration, modifyColor, colorOverTime, flipX, flipY, feedbacksIntensity, channel, resetShakerValuesAfterShake, resetTargetValuesAfterShake, useRange, eventRange, eventOriginPosition);
	}
}

using System;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[Serializable]
public class WiggleProperties
{
	[Header("Status")]
	public bool WigglePermitted = true;

	[Header("Type")]
	public WiggleTypes WiggleType = WiggleTypes.Random;

	public bool UseUnscaledTime;

	public bool StartWigglingAutomatically = true;

	public bool SmoothPingPong = true;

	[Header("Speed")]
	public bool UseSpeedCurve;

	public AnimationCurve SpeedCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	[Header("Frequency")]
	public float FrequencyMin;

	public float FrequencyMax = 1f;

	[Header("Amplitude")]
	public Vector3 AmplitudeMin = Vector3.zero;

	public Vector3 AmplitudeMax = Vector3.one;

	public bool RelativeAmplitude = true;

	public bool UniformValues;

	[Header("Curve")]
	public AnimationCurve Curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public Vector3 RemapCurveZeroMin = Vector3.zero;

	public Vector3 RemapCurveZeroMax = Vector3.zero;

	public Vector3 RemapCurveOneMin = Vector3.one;

	public Vector3 RemapCurveOneMax = Vector3.one;

	public bool RelativeCurveAmplitude = true;

	public bool CurvePingPong;

	[Header("Pause")]
	public float PauseMin;

	public float PauseMax;

	[Header("Limited Time")]
	public bool LimitedTime;

	public float LimitedTimeTotal;

	public AnimationCurve LimitedTimeFalloff = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	public bool LimitedTimeResetValue = true;

	[MMFReadOnly]
	public float LimitedTimeLeft;

	[Header("Noise Frequency")]
	public Vector3 NoiseFrequencyMin = Vector3.zero;

	public Vector3 NoiseFrequencyMax = Vector3.one;

	[Header("Noise Shift")]
	public Vector3 NoiseShiftMin = Vector3.zero;

	public Vector3 NoiseShiftMax = Vector3.zero;

	public float GetDeltaTime()
	{
		if (!UseUnscaledTime)
		{
			return Time.deltaTime;
		}
		return Time.unscaledDeltaTime;
	}

	public float GetTime()
	{
		if (!UseUnscaledTime)
		{
			return Time.time;
		}
		return Time.unscaledTime;
	}
}

using UnityEngine;

namespace MoreMountains.Feedbacks;

public struct InternalWiggleProperties
{
	public Vector3 returnVector;

	public Vector3 newValue;

	public Vector3 initialValue;

	public Vector3 startValue;

	public float timeSinceLastChange;

	public float randomFrequency;

	public Vector3 randomNoiseFrequency;

	public Vector3 randomAmplitude;

	public Vector3 randomNoiseShift;

	public float timeSinceLastPause;

	public float pauseDuration;

	public float noiseElapsedTime;

	public Vector3 limitedTimeValueSave;

	public Vector3 remapZero;

	public Vector3 remapOne;

	public float curveDirection;
}

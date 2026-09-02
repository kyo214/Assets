using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("More Mountains/Feedbacks/Shakers/Various/MMWiggle")]
public class MMWiggle : MonoBehaviour
{
	public enum UpdateModes
	{
		Update = 0,
		FixedUpdate = 1,
		LateUpdate = 2
	}

	[Tooltip("the selected update mode")]
	public UpdateModes UpdateMode;

	[Tooltip("whether or not position wiggle is active")]
	public bool PositionActive;

	[Tooltip("whether or not rotation wiggle is active")]
	public bool RotationActive;

	[Tooltip("whether or not scale wiggle is active")]
	public bool ScaleActive;

	[Tooltip("all public info related to position wiggling")]
	public WiggleProperties PositionWiggleProperties;

	[Tooltip("all public info related to rotation wiggling")]
	public WiggleProperties RotationWiggleProperties;

	[Tooltip("all public info related to scale wiggling")]
	public WiggleProperties ScaleWiggleProperties;

	[Tooltip("a debug duration used in conjunction with the debug buttons")]
	public float DebugWiggleDuration = 2f;

	protected InternalWiggleProperties _positionInternalProperties;

	protected InternalWiggleProperties _rotationInternalProperties;

	protected InternalWiggleProperties _scaleInternalProperties;

	public virtual void WigglePosition(float duration)
	{
		WiggleValue(ref PositionWiggleProperties, ref _positionInternalProperties, duration);
	}

	public virtual void WiggleRotation(float duration)
	{
		WiggleValue(ref RotationWiggleProperties, ref _rotationInternalProperties, duration);
	}

	public virtual void WiggleScale(float duration)
	{
		WiggleValue(ref ScaleWiggleProperties, ref _scaleInternalProperties, duration);
	}

	protected virtual void WiggleValue(ref WiggleProperties property, ref InternalWiggleProperties internalProperties, float duration)
	{
		InitializeRandomValues(ref property, ref internalProperties);
		internalProperties.limitedTimeValueSave = internalProperties.initialValue;
		property.LimitedTime = true;
		property.LimitedTimeLeft = duration;
		property.LimitedTimeTotal = duration;
		property.WigglePermitted = true;
	}

	protected virtual void Start()
	{
		Initialization();
	}

	public virtual void Initialization()
	{
		_positionInternalProperties.initialValue = base.transform.localPosition;
		_positionInternalProperties.startValue = base.transform.localPosition;
		_rotationInternalProperties.initialValue = base.transform.localEulerAngles;
		_rotationInternalProperties.startValue = base.transform.localEulerAngles;
		_scaleInternalProperties.initialValue = base.transform.localScale;
		_scaleInternalProperties.startValue = base.transform.localScale;
		InitializeRandomValues(ref PositionWiggleProperties, ref _positionInternalProperties);
		InitializeRandomValues(ref RotationWiggleProperties, ref _rotationInternalProperties);
		InitializeRandomValues(ref ScaleWiggleProperties, ref _scaleInternalProperties);
	}

	protected virtual void InitializeRandomValues(ref WiggleProperties properties, ref InternalWiggleProperties internalProperties)
	{
		internalProperties.newValue = internalProperties.initialValue;
		internalProperties.timeSinceLastChange = 0f;
		internalProperties.returnVector = Vector3.zero;
		internalProperties.randomFrequency = Random.Range(properties.FrequencyMin, properties.FrequencyMax);
		internalProperties.randomNoiseFrequency = Vector3.zero;
		internalProperties.randomAmplitude = Vector3.zero;
		internalProperties.timeSinceLastPause = 0f;
		internalProperties.pauseDuration = 0f;
		internalProperties.noiseElapsedTime = 0f;
		internalProperties.curveDirection = 1f;
		properties.LimitedTimeLeft = properties.LimitedTimeTotal;
		RandomizeVector3(ref internalProperties.randomAmplitude, properties.AmplitudeMin, properties.AmplitudeMax);
		RandomizeVector3(ref internalProperties.randomNoiseFrequency, properties.NoiseFrequencyMin, properties.NoiseFrequencyMax);
		RandomizeVector3(ref internalProperties.randomNoiseShift, properties.NoiseShiftMin, properties.NoiseShiftMax);
		RandomizeVector3(ref internalProperties.remapZero, properties.RemapCurveZeroMin, properties.RemapCurveZeroMax);
		RandomizeVector3(ref internalProperties.remapOne, properties.RemapCurveOneMin, properties.RemapCurveOneMax);
		internalProperties.newValue = DetermineNewValue(properties, internalProperties.newValue, internalProperties.initialValue, ref internalProperties.startValue, ref internalProperties.randomAmplitude, ref internalProperties.randomFrequency, ref internalProperties.pauseDuration);
	}

	protected virtual void Update()
	{
		if (UpdateMode == UpdateModes.Update)
		{
			ProcessUpdate();
		}
	}

	protected virtual void LateUpdate()
	{
		if (UpdateMode == UpdateModes.LateUpdate)
		{
			ProcessUpdate();
		}
	}

	protected virtual void FixedUpdate()
	{
		if (UpdateMode == UpdateModes.FixedUpdate)
		{
			ProcessUpdate();
		}
	}

	protected virtual void ProcessUpdate()
	{
		_positionInternalProperties.returnVector = base.transform.localPosition;
		if (UpdateValue(PositionActive, PositionWiggleProperties, ref _positionInternalProperties))
		{
			base.transform.localPosition = _positionInternalProperties.returnVector;
		}
		_rotationInternalProperties.returnVector = base.transform.localEulerAngles;
		if (UpdateValue(RotationActive, RotationWiggleProperties, ref _rotationInternalProperties))
		{
			base.transform.localEulerAngles = _rotationInternalProperties.returnVector;
		}
		_scaleInternalProperties.returnVector = base.transform.localScale;
		if (UpdateValue(ScaleActive, ScaleWiggleProperties, ref _scaleInternalProperties))
		{
			base.transform.localScale = _scaleInternalProperties.returnVector;
		}
	}

	protected virtual bool UpdateValue(bool valueActive, WiggleProperties properties, ref InternalWiggleProperties internalProperties)
	{
		if (!valueActive)
		{
			return false;
		}
		if (!properties.WigglePermitted)
		{
			return false;
		}
		if (properties.LimitedTime && properties.LimitedTimeTotal > 0f)
		{
			float limitedTimeLeft = properties.LimitedTimeLeft;
			properties.LimitedTimeLeft -= properties.GetDeltaTime();
			if (properties.LimitedTimeLeft <= 0f)
			{
				if (limitedTimeLeft > 0f && properties.LimitedTimeResetValue)
				{
					internalProperties.returnVector = internalProperties.limitedTimeValueSave;
					properties.LimitedTimeLeft = 0f;
					properties.WigglePermitted = false;
					return true;
				}
				return false;
			}
		}
		switch (properties.WiggleType)
		{
		case WiggleTypes.PingPong:
			return MoveVector3TowardsTarget(ref internalProperties.returnVector, properties, ref internalProperties.startValue, internalProperties.initialValue, ref internalProperties.newValue, ref internalProperties.timeSinceLastPause, ref internalProperties.timeSinceLastChange, ref internalProperties.randomAmplitude, ref internalProperties.randomFrequency, ref internalProperties.pauseDuration, internalProperties.randomFrequency);
		case WiggleTypes.Random:
			return MoveVector3TowardsTarget(ref internalProperties.returnVector, properties, ref internalProperties.startValue, internalProperties.initialValue, ref internalProperties.newValue, ref internalProperties.timeSinceLastPause, ref internalProperties.timeSinceLastChange, ref internalProperties.randomAmplitude, ref internalProperties.randomFrequency, ref internalProperties.pauseDuration, internalProperties.randomFrequency);
		case WiggleTypes.Noise:
			internalProperties.returnVector = AnimateNoiseValue(ref internalProperties, properties);
			return true;
		case WiggleTypes.Curve:
			internalProperties.returnVector = AnimateCurveValue(ref internalProperties, properties);
			return true;
		default:
			return false;
		}
	}

	protected float ApplyFalloff(WiggleProperties properties)
	{
		float result = 1f;
		if (properties.LimitedTime && properties.LimitedTimeTotal > 0f)
		{
			float time = (properties.LimitedTimeTotal - properties.LimitedTimeLeft) / properties.LimitedTimeTotal;
			result = properties.LimitedTimeFalloff.Evaluate(time);
		}
		return result;
	}

	protected virtual Vector3 AnimateNoiseValue(ref InternalWiggleProperties internalProperties, WiggleProperties properties)
	{
		internalProperties.noiseElapsedTime += properties.GetDeltaTime();
		internalProperties.newValue.x = (Mathf.PerlinNoise(internalProperties.randomNoiseFrequency.x * internalProperties.noiseElapsedTime, internalProperties.randomNoiseShift.x) * 2f - 1f) * internalProperties.randomAmplitude.x;
		internalProperties.newValue.y = (Mathf.PerlinNoise(internalProperties.randomNoiseFrequency.y * internalProperties.noiseElapsedTime, internalProperties.randomNoiseShift.y) * 2f - 1f) * internalProperties.randomAmplitude.y;
		internalProperties.newValue.z = (Mathf.PerlinNoise(internalProperties.randomNoiseFrequency.z * internalProperties.noiseElapsedTime, internalProperties.randomNoiseShift.z) * 2f - 1f) * internalProperties.randomAmplitude.z;
		internalProperties.newValue *= ApplyFalloff(properties);
		if (properties.RelativeAmplitude)
		{
			internalProperties.newValue += internalProperties.initialValue;
		}
		if (properties.UniformValues)
		{
			internalProperties.newValue.y = internalProperties.newValue.x;
			internalProperties.newValue.z = internalProperties.newValue.x;
		}
		return internalProperties.newValue;
	}

	protected virtual Vector3 AnimateCurveValue(ref InternalWiggleProperties internalProperties, WiggleProperties properties)
	{
		internalProperties.timeSinceLastPause += properties.GetDeltaTime();
		internalProperties.timeSinceLastChange += properties.GetDeltaTime();
		if (internalProperties.timeSinceLastPause < internalProperties.pauseDuration)
		{
			float percent = ((internalProperties.curveDirection == 1f) ? 1f : 0f);
			EvaluateCurve(properties.Curve, percent, internalProperties.remapZero, internalProperties.remapOne, ref internalProperties.newValue);
			if (properties.RelativeCurveAmplitude)
			{
				internalProperties.newValue += internalProperties.initialValue;
			}
		}
		if (internalProperties.timeSinceLastPause == internalProperties.timeSinceLastChange)
		{
			internalProperties.timeSinceLastChange = 0f;
		}
		if (internalProperties.randomFrequency > 0f)
		{
			float num = internalProperties.timeSinceLastChange / internalProperties.randomFrequency;
			if (internalProperties.curveDirection < 0f)
			{
				num = 1f - num;
			}
			EvaluateCurve(properties.Curve, num, internalProperties.remapZero, internalProperties.remapOne, ref internalProperties.newValue);
			if (internalProperties.timeSinceLastChange > internalProperties.randomFrequency)
			{
				internalProperties.timeSinceLastChange = 0f;
				internalProperties.timeSinceLastPause = 0f;
				if (properties.CurvePingPong)
				{
					internalProperties.curveDirection = 0f - internalProperties.curveDirection;
				}
				RandomizeFloat(ref internalProperties.randomFrequency, properties.FrequencyMin, properties.FrequencyMax);
			}
		}
		if (properties.RelativeCurveAmplitude)
		{
			internalProperties.newValue = internalProperties.initialValue + internalProperties.newValue;
		}
		return internalProperties.newValue;
	}

	protected virtual void EvaluateCurve(AnimationCurve curve, float percent, Vector3 remapMin, Vector3 remapMax, ref Vector3 returnValue)
	{
		returnValue.x = MMFeedbacksHelpers.Remap(curve.Evaluate(percent), 0f, 1f, remapMin.x, remapMax.x);
		returnValue.y = MMFeedbacksHelpers.Remap(curve.Evaluate(percent), 0f, 1f, remapMin.y, remapMax.y);
		returnValue.z = MMFeedbacksHelpers.Remap(curve.Evaluate(percent), 0f, 1f, remapMin.z, remapMax.z);
	}

	protected virtual bool MoveVector3TowardsTarget(ref Vector3 movedValue, WiggleProperties properties, ref Vector3 startValue, Vector3 initialValue, ref Vector3 destinationValue, ref float timeSinceLastPause, ref float timeSinceLastValueChange, ref Vector3 randomAmplitude, ref float randomFrequency, ref float pauseDuration, float frequency)
	{
		timeSinceLastPause += properties.GetDeltaTime();
		timeSinceLastValueChange += properties.GetDeltaTime();
		if (timeSinceLastPause < pauseDuration)
		{
			return false;
		}
		if (timeSinceLastPause == timeSinceLastValueChange)
		{
			timeSinceLastValueChange = 0f;
		}
		if (frequency > 0f)
		{
			float num = timeSinceLastValueChange / frequency;
			if (!properties.UseSpeedCurve)
			{
				movedValue = Vector3.Lerp(startValue, destinationValue, num);
			}
			else
			{
				float t = properties.SpeedCurve.Evaluate(num);
				movedValue = Vector3.LerpUnclamped(startValue, destinationValue, t);
			}
			if (timeSinceLastValueChange > frequency)
			{
				timeSinceLastValueChange = 0f;
				timeSinceLastPause = 0f;
				movedValue = destinationValue;
				destinationValue = DetermineNewValue(properties, movedValue, initialValue, ref startValue, ref randomAmplitude, ref randomFrequency, ref pauseDuration);
			}
		}
		return true;
	}

	protected virtual Vector3 DetermineNewValue(WiggleProperties properties, Vector3 newValue, Vector3 initialValue, ref Vector3 startValue, ref Vector3 randomAmplitude, ref float randomFrequency, ref float pauseDuration)
	{
		switch (properties.WiggleType)
		{
		case WiggleTypes.PingPong:
			if (properties.RelativeAmplitude)
			{
				if (newValue == properties.AmplitudeMin + initialValue)
				{
					newValue = properties.AmplitudeMax;
					startValue = properties.AmplitudeMin;
				}
				else
				{
					newValue = properties.AmplitudeMin;
					startValue = properties.AmplitudeMax;
				}
				startValue += initialValue;
				newValue += initialValue;
			}
			else
			{
				newValue = ((newValue == properties.AmplitudeMin) ? properties.AmplitudeMax : properties.AmplitudeMin);
				startValue = ((newValue == properties.AmplitudeMin) ? properties.AmplitudeMax : properties.AmplitudeMin);
			}
			RandomizeFloat(ref randomFrequency, properties.FrequencyMin, properties.FrequencyMax);
			RandomizeFloat(ref pauseDuration, properties.PauseMin, properties.PauseMax);
			if (properties.UniformValues)
			{
				newValue.y = newValue.x;
				newValue.z = newValue.x;
			}
			return newValue;
		case WiggleTypes.Random:
			startValue = newValue;
			RandomizeFloat(ref randomFrequency, properties.FrequencyMin, properties.FrequencyMax);
			RandomizeVector3(ref randomAmplitude, properties.AmplitudeMin, properties.AmplitudeMax);
			RandomizeFloat(ref pauseDuration, properties.PauseMin, properties.PauseMax);
			newValue = randomAmplitude;
			if (properties.UniformValues)
			{
				newValue.y = newValue.x;
				newValue.z = newValue.x;
			}
			newValue *= ApplyFalloff(properties);
			if (properties.RelativeAmplitude)
			{
				newValue += initialValue;
			}
			return newValue;
		default:
			return Vector3.zero;
		}
	}

	protected virtual float RandomizeFloat(ref float randomizedFloat, float floatMin, float floatMax)
	{
		randomizedFloat = Random.Range(floatMin, floatMax);
		return randomizedFloat;
	}

	protected virtual Vector3 RandomizeVector3(ref Vector3 randomizedVector, Vector3 vectorMin, Vector3 vectorMax)
	{
		randomizedVector.x = Random.Range(vectorMin.x, vectorMax.x);
		randomizedVector.y = Random.Range(vectorMin.y, vectorMax.y);
		randomizedVector.z = Random.Range(vectorMin.z, vectorMax.z);
		return randomizedVector;
	}
}

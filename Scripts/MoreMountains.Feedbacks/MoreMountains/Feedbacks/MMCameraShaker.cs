using UnityEngine;

namespace MoreMountains.Feedbacks;

[RequireComponent(typeof(MMWiggle))]
[AddComponentMenu("More Mountains/Feedbacks/Shakers/Camera/MMCameraShaker")]
public class MMCameraShaker : MonoBehaviour
{
	[Tooltip("the channel to broadcast this shake on")]
	public int Channel;

	[Tooltip("a cooldown, in seconds, after a shake, during which no other shake can start")]
	public float CooldownBetweenShakes;

	protected MMWiggle _wiggle;

	protected float _shakeStartedTimestamp = float.MinValue;

	protected virtual void Awake()
	{
		_wiggle = GetComponent<MMWiggle>();
	}

	public virtual void ShakeCamera(float duration, float amplitude, float frequency, float amplitudeX, float amplitudeY, float amplitudeZ, bool useUnscaledTime)
	{
		if (!(Time.unscaledTime - _shakeStartedTimestamp < CooldownBetweenShakes))
		{
			if (amplitudeX != 0f || amplitudeY != 0f || amplitudeZ != 0f)
			{
				_wiggle.PositionWiggleProperties.AmplitudeMin.x = 0f - amplitudeX;
				_wiggle.PositionWiggleProperties.AmplitudeMin.y = 0f - amplitudeY;
				_wiggle.PositionWiggleProperties.AmplitudeMin.z = 0f - amplitudeZ;
				_wiggle.PositionWiggleProperties.AmplitudeMax.x = amplitudeX;
				_wiggle.PositionWiggleProperties.AmplitudeMax.y = amplitudeY;
				_wiggle.PositionWiggleProperties.AmplitudeMax.z = amplitudeZ;
			}
			else
			{
				_wiggle.PositionWiggleProperties.AmplitudeMin = Vector3.one * (0f - amplitude);
				_wiggle.PositionWiggleProperties.AmplitudeMax = Vector3.one * amplitude;
			}
			_shakeStartedTimestamp = Time.time;
			_wiggle.PositionWiggleProperties.UseUnscaledTime = useUnscaledTime;
			_wiggle.PositionWiggleProperties.FrequencyMin = frequency;
			_wiggle.PositionWiggleProperties.FrequencyMax = frequency;
			_wiggle.PositionWiggleProperties.NoiseFrequencyMin = frequency * Vector3.one;
			_wiggle.PositionWiggleProperties.NoiseFrequencyMax = frequency * Vector3.one;
			_wiggle.WigglePosition(duration);
		}
	}

	public virtual void OnCameraShakeEvent(float duration, float amplitude, float frequency, float amplitudeX, float amplitudeY, float amplitudeZ, bool infinite, int channel, bool useUnscaledTime)
	{
		if (channel == Channel)
		{
			ShakeCamera(duration, amplitude, frequency, amplitudeX, amplitudeY, amplitudeZ, useUnscaledTime);
		}
	}

	protected virtual void OnEnable()
	{
		MMCameraShakeEvent.Register(OnCameraShakeEvent);
	}

	protected virtual void OnDisable()
	{
		MMCameraShakeEvent.Unregister(OnCameraShakeEvent);
	}
}

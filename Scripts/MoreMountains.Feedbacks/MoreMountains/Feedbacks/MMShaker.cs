using UnityEngine;

namespace MoreMountains.Feedbacks;

public class MMShaker : MonoBehaviour
{
	[Header("Shake Settings")]
	[Tooltip("the channel to listen to - has to match the one on the feedback")]
	public int Channel;

	[Tooltip("the duration of the shake, in seconds")]
	public float ShakeDuration = 0.2f;

	[Tooltip("if this is true this shaker will play on awake")]
	public bool PlayOnAwake;

	[Tooltip("if this is true, a new shake can happen while shaking")]
	public bool Interruptible = true;

	[Tooltip("if this is true, this shaker will always reset target values, regardless of how it was called")]
	public bool AlwaysResetTargetValuesAfterShake;

	[Tooltip("a cooldown, in seconds, after a shake, during which no other shake can start")]
	public float CooldownBetweenShakes;

	[Tooltip("whether or not this shaker is shaking right now")]
	[MMFReadOnly]
	public bool Shaking;

	[HideInInspector]
	public bool ForwardDirection = true;

	[HideInInspector]
	public TimescaleModes TimescaleMode;

	[HideInInspector]
	internal bool _listeningToEvents;

	protected float _shakeStartedTimestamp = float.MinValue;

	protected float _remappedTimeSinceStart;

	protected bool _resetShakerValuesAfterShake;

	protected bool _resetTargetValuesAfterShake;

	protected float _journey;

	public bool ListeningToEvents => _listeningToEvents;

	public virtual float GetTime()
	{
		if (TimescaleMode != TimescaleModes.Scaled)
		{
			return Time.unscaledTime;
		}
		return Time.time;
	}

	public virtual float GetDeltaTime()
	{
		if (TimescaleMode != TimescaleModes.Scaled)
		{
			return Time.unscaledDeltaTime;
		}
		return Time.deltaTime;
	}

	protected virtual void Awake()
	{
		Shaking = false;
		Initialization();
		if (!_listeningToEvents)
		{
			StartListening();
		}
		base.enabled = PlayOnAwake;
	}

	protected virtual void Initialization()
	{
	}

	public virtual void StartShaking()
	{
		_journey = (ForwardDirection ? 0f : ShakeDuration);
		if (!(GetTime() - _shakeStartedTimestamp < CooldownBetweenShakes) && !Shaking)
		{
			base.enabled = true;
			_shakeStartedTimestamp = GetTime();
			Shaking = true;
			GrabInitialValues();
			ShakeStarts();
		}
	}

	protected virtual void ShakeStarts()
	{
	}

	protected virtual void GrabInitialValues()
	{
	}

	protected virtual void Update()
	{
		if (Shaking)
		{
			Shake();
			_journey += (ForwardDirection ? GetDeltaTime() : (0f - GetDeltaTime()));
		}
		if (Shaking && (_journey < 0f || _journey > ShakeDuration))
		{
			Shaking = false;
			ShakeComplete();
		}
	}

	protected virtual void Shake()
	{
	}

	protected virtual float ShakeFloat(AnimationCurve curve, float remapMin, float remapMax, bool relativeIntensity, float initialValue)
	{
		float num = 0f;
		float time = MMFeedbacksHelpers.Remap(_journey, 0f, ShakeDuration, 0f, 1f);
		num = MMFeedbacksHelpers.Remap(curve.Evaluate(time), 0f, 1f, remapMin, remapMax);
		if (relativeIntensity)
		{
			num += initialValue;
		}
		return num;
	}

	protected virtual void ResetTargetValues()
	{
	}

	protected virtual void ResetShakerValues()
	{
	}

	protected virtual void ShakeComplete()
	{
		if (_resetTargetValuesAfterShake || AlwaysResetTargetValuesAfterShake)
		{
			ResetTargetValues();
		}
		if (_resetShakerValuesAfterShake)
		{
			ResetShakerValues();
		}
		base.enabled = false;
	}

	protected virtual void OnEnable()
	{
		StartShaking();
	}

	protected virtual void OnDestroy()
	{
		StopListening();
	}

	protected virtual void OnDisable()
	{
		if (Shaking)
		{
			ShakeComplete();
		}
	}

	public virtual void Play()
	{
		if (!(GetTime() - _shakeStartedTimestamp < CooldownBetweenShakes))
		{
			base.enabled = true;
		}
	}

	public virtual void Stop()
	{
		Shaking = false;
		ShakeComplete();
	}

	public virtual void StartListening()
	{
		_listeningToEvents = true;
	}

	public virtual void StopListening()
	{
		_listeningToEvents = false;
	}

	protected virtual bool CheckEventAllowed(int channel, bool useRange = false, float range = 0f, Vector3 eventOriginPosition = default(Vector3))
	{
		if (channel != Channel && channel != -1 && Channel != -1)
		{
			return false;
		}
		if (!base.gameObject.activeInHierarchy)
		{
			return false;
		}
		if (useRange && Vector3.Distance(base.transform.position, eventOriginPosition) > range)
		{
			return false;
		}
		return true;
	}
}

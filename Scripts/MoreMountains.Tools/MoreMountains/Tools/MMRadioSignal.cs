using UnityEngine;

namespace MoreMountains.Tools;

public abstract class MMRadioSignal : MonoBehaviour
{
	public enum SignalModes
	{
		OneTime = 0,
		Persistent = 1,
		Driven = 2
	}

	public enum TimeScales
	{
		Unscaled = 0,
		Scaled = 1
	}

	[Header("Signal")]
	public SignalModes SignalMode = SignalModes.Persistent;

	public TimeScales TimeScale;

	public float Duration = 2f;

	public float GlobalMultiplier = 1f;

	[MMReadOnly]
	public float CurrentLevel;

	[Header("Play Settings")]
	[MMReadOnly]
	public bool Playing;

	[Range(0f, 1f)]
	public float DriverTime;

	public bool PlayOnStart = true;

	public MMRadioSignalOnValueChange OnValueChange;

	[Header("Debug")]
	[MMInspectorButton("StartShaking")]
	public bool StartShakingButton;

	protected float _signalTime;

	protected float _shakeStartedTimestamp;

	protected float _levelLastFrame;

	public virtual float Level => CurrentLevel;

	public float TimescaleTime
	{
		get
		{
			if (TimeScale != TimeScales.Scaled)
			{
				return Time.unscaledTime;
			}
			return Time.time;
		}
	}

	public float TimescaleDeltaTime
	{
		get
		{
			if (TimeScale != TimeScales.Scaled)
			{
				return Time.unscaledDeltaTime;
			}
			return Time.deltaTime;
		}
	}

	protected virtual void Awake()
	{
		Initialization();
		if (PlayOnStart)
		{
			StartShaking();
		}
		base.enabled = PlayOnStart;
	}

	protected virtual void Initialization()
	{
		CurrentLevel = 0f;
	}

	public virtual void StartShaking()
	{
		if (!Playing)
		{
			base.enabled = true;
			_shakeStartedTimestamp = TimescaleTime;
			Playing = true;
			ShakeStarts();
		}
	}

	protected virtual void ShakeStarts()
	{
	}

	protected virtual void Update()
	{
		ProcessUpdate();
		if (SignalMode == SignalModes.Driven)
		{
			ProcessDrivenMode();
		}
		else if (SignalMode == SignalModes.Persistent)
		{
			_signalTime += TimescaleDeltaTime;
			if (_signalTime > Duration)
			{
				_signalTime = 0f;
			}
			DriverTime = MMMaths.Remap(_signalTime, 0f, Duration, 0f, 1f);
		}
		else
		{
			_ = SignalMode;
		}
		if (Playing || SignalMode == SignalModes.Driven)
		{
			Shake();
		}
		if (SignalMode == SignalModes.OneTime && Playing && TimescaleTime - _shakeStartedTimestamp > Duration)
		{
			ShakeComplete();
		}
		if (_levelLastFrame != Level && OnValueChange != null)
		{
			OnValueChange.Invoke(Level);
		}
		_levelLastFrame = Level;
	}

	protected virtual void ProcessDrivenMode()
	{
	}

	protected virtual void ProcessUpdate()
	{
	}

	protected virtual void Shake()
	{
	}

	public virtual float GraphValue(float time)
	{
		return 0f;
	}

	protected virtual void ShakeComplete()
	{
		Playing = false;
		base.enabled = false;
	}

	protected virtual void OnEnable()
	{
		StartShaking();
	}

	protected virtual void OnDestroy()
	{
	}

	protected virtual void OnDisable()
	{
		if (Playing)
		{
			ShakeComplete();
		}
	}

	public virtual void Play()
	{
		base.enabled = true;
	}

	public virtual void Stop()
	{
		ShakeComplete();
	}

	public virtual float ApplyBias(float t, float bias)
	{
		if (bias == 0.5f)
		{
			return t;
		}
		bias = MMMaths.Remap(bias, 0f, 1f, 1f, 0f);
		float num = bias * 2f - 1f;
		t = ((!(num < 0f)) ? Mathf.Pow(t, Mathf.Max(1f - num, 0.01f)) : (1f - Mathf.Pow(1f - t, Mathf.Max(1f + num, 0.01f))));
		return t;
	}
}

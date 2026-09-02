using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("More Mountains/Feedbacks/Shakers/Various/MMTimeManager")]
public class MMTimeManager : MonoBehaviour
{
	[Header("Default Values")]
	[MMFInformation("Put this component in your scene and it'll catch MMFreezeFrameEvents and MMTimeScaleEvents, allowing you to control the flow of time.", MMFInformationAttribute.InformationType.Info, false)]
	[Tooltip("The reference timescale, to which the system will go back to after all time is changed")]
	public float NormalTimescale = 1f;

	[Tooltip("The reference timescale, to which the system will go back to after all time is changed")]
	public float DefaultLerpSpeed = 1f;

	[Tooltip("The reference timescale, to which the system will go back to after all time is changed")]
	public bool DefaultLerpTimescale;

	[Header("Debug")]
	[Tooltip("the current, real time, time scale")]
	[MMFReadOnly]
	public float CurrentTimeScale = 1f;

	[Tooltip("the time scale the system is lerping towards")]
	[MMFReadOnly]
	public float TargetTimeScale = 1f;

	[Tooltip("whether or not the timescale should be lerping")]
	[MMFReadOnly]
	public bool LerpTimescale = true;

	[Tooltip("the speed at which the timescale should lerp towards its target")]
	[MMFReadOnly]
	public float LerpSpeed;

	[MMFInspectorButton("TestButtonToSlowDownTime")]
	public bool TestButton;

	protected Stack<TimeScaleProperties> _timeScaleProperties;

	protected float _frozenTimeLeft = -1f;

	protected TimeScaleProperties _currentProperty;

	protected float _initialFixedDeltaTime;

	protected float _initialMaximumDeltaTime;

	protected virtual void TestButtonToSlowDownTime()
	{
		MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, 0.5f, 3f, lerp: true, 1f, infinite: false);
	}

	protected virtual void Start()
	{
		Initialization();
	}

	public virtual void Initialization()
	{
		TargetTimeScale = NormalTimescale;
		_timeScaleProperties = new Stack<TimeScaleProperties>();
		_initialFixedDeltaTime = Time.fixedDeltaTime;
		_initialMaximumDeltaTime = Time.maximumDeltaTime;
		ApplyTimeScale(NormalTimescale);
		if (LerpSpeed <= 0f)
		{
			LerpSpeed = 1f;
		}
	}

	protected virtual void Update()
	{
		while (_timeScaleProperties.Count > 0)
		{
			_currentProperty = _timeScaleProperties.Peek();
			TargetTimeScale = _currentProperty.TimeScale;
			LerpSpeed = _currentProperty.LerpSpeed;
			LerpTimescale = _currentProperty.Lerp;
			_currentProperty.Duration -= Time.unscaledDeltaTime;
			_timeScaleProperties.Pop();
			_timeScaleProperties.Push(_currentProperty);
			if (_currentProperty.Duration > 0f || _currentProperty.Infinite)
			{
				break;
			}
			Unfreeze();
		}
		if (_timeScaleProperties.Count == 0)
		{
			TargetTimeScale = NormalTimescale;
			LerpTimescale = DefaultLerpTimescale;
			LerpSpeed = DefaultLerpSpeed;
		}
		if (LerpTimescale)
		{
			if (LerpSpeed <= 0f)
			{
				LerpSpeed = 1f;
			}
			ApplyTimeScale(Mathf.Lerp(Time.timeScale, TargetTimeScale, Time.unscaledDeltaTime * LerpSpeed));
		}
		else
		{
			ApplyTimeScale(TargetTimeScale);
		}
	}

	protected virtual void ApplyTimeScale(float newValue)
	{
		Time.timeScale = newValue;
		if (newValue != 0f)
		{
			Time.fixedDeltaTime = _initialFixedDeltaTime * newValue;
		}
		Time.maximumDeltaTime = _initialMaximumDeltaTime * newValue;
		CurrentTimeScale = Time.timeScale;
	}

	protected virtual void SetTimeScale(float newTimeScale)
	{
		_timeScaleProperties.Clear();
		ApplyTimeScale(newTimeScale);
	}

	protected virtual void SetTimeScale(TimeScaleProperties timeScaleProperties)
	{
		_timeScaleProperties.Push(timeScaleProperties);
	}

	protected virtual void ResetTimeScale()
	{
		SetTimeScale(NormalTimescale);
	}

	protected virtual void Unfreeze()
	{
		if (_timeScaleProperties.Count > 0)
		{
			_timeScaleProperties.Pop();
		}
		else
		{
			ResetTimeScale();
		}
	}

	public virtual void SetTimescaleTo(float newNormalTimeScale)
	{
		MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, newNormalTimeScale, 0f, lerp: false, 0f, infinite: true);
	}

	public virtual void OnTimeScaleEvent(MMTimeScaleMethods timeScaleMethod, float timeScale, float duration, bool lerp, float lerpSpeed, bool infinite)
	{
		TimeScaleProperties timeScale2 = new TimeScaleProperties
		{
			TimeScale = timeScale,
			Duration = duration,
			Lerp = lerp,
			LerpSpeed = lerpSpeed,
			Infinite = infinite
		};
		switch (timeScaleMethod)
		{
		case MMTimeScaleMethods.Reset:
			ResetTimeScale();
			break;
		case MMTimeScaleMethods.For:
			SetTimeScale(timeScale2);
			break;
		case MMTimeScaleMethods.Unfreeze:
			Unfreeze();
			break;
		}
	}

	public virtual void OnMMFreezeFrameEvent(float duration)
	{
		_frozenTimeLeft = duration;
		SetTimeScale(new TimeScaleProperties
		{
			Duration = duration,
			Lerp = false,
			LerpSpeed = 0f,
			TimeScale = 0f
		});
	}

	private void OnEnable()
	{
		MMFreezeFrameEvent.Register(OnMMFreezeFrameEvent);
		MMTimeScaleEvent.Register(OnTimeScaleEvent);
	}

	private void OnDisable()
	{
		MMFreezeFrameEvent.Unregister(OnMMFreezeFrameEvent);
		MMTimeScaleEvent.Unregister(OnTimeScaleEvent);
	}
}

using UnityEngine;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Camera/MMPostProcessingMovingFilter")]
public class MMPostProcessingMovingFilter : MonoBehaviour
{
	public enum TimeScales
	{
		Unscaled = 0,
		Scaled = 1
	}

	[Header("Settings")]
	public int Channel;

	public TimeScales TimeScale;

	public MMTweenType Curve = new MMTweenType(MMTween.MMTweenCurve.EaseInCubic);

	public bool Active;

	[MMVector(new string[] { "On", "Off" })]
	public Vector2 FilterOffset = new Vector2(0f, 5f);

	public bool AddToInitialPosition = true;

	[Header("Tests")]
	public float TestDuration = 0.5f;

	[MMInspectorButton("PostProcessingToggle")]
	public bool PostProcessingToggleButton;

	[MMInspectorButton("PostProcessingTriggerOff")]
	public bool PostProcessingTriggerOffButton;

	[MMInspectorButton("PostProcessingTriggerOn")]
	public bool PostProcessingTriggerOnButton;

	protected bool _lastReachedState;

	protected float _duration = 2f;

	protected float _lastMovementStartedAt;

	protected Vector3 _initialPosition;

	protected Vector3 _newPosition;

	protected virtual void Start()
	{
		Initialization();
	}

	protected virtual void Initialization()
	{
		_lastMovementStartedAt = 0f;
		if (AddToInitialPosition)
		{
			_initialPosition = base.transform.localPosition;
		}
		else
		{
			_initialPosition = Vector3.zero;
		}
		_newPosition = _initialPosition;
		_newPosition.y = (Active ? (_initialPosition.y + FilterOffset.x) : (_initialPosition.y + FilterOffset.y));
		base.transform.localPosition = _newPosition;
		_lastReachedState = Active;
	}

	protected virtual void Update()
	{
		if (_lastReachedState != Active)
		{
			MoveTowardsCurrentTarget();
		}
	}

	protected virtual void MoveTowardsCurrentTarget()
	{
		if (_newPosition != base.transform.localPosition)
		{
			base.transform.localPosition = _newPosition;
		}
		float startValue = (Active ? (_initialPosition.y + FilterOffset.y) : (_initialPosition.y + FilterOffset.x));
		float num = (Active ? (_initialPosition.y + FilterOffset.x) : (_initialPosition.y + FilterOffset.y));
		float num2 = ((TimeScale == TimeScales.Unscaled) ? Time.unscaledTime : Time.time);
		_newPosition = base.transform.localPosition;
		_newPosition.y = MMTween.Tween(num2 - _lastMovementStartedAt, 0f, _duration, startValue, num, Curve);
		if (num2 - _lastMovementStartedAt > _duration)
		{
			_newPosition.y = num;
			base.transform.localPosition = _newPosition;
			_lastReachedState = Active;
		}
	}

	public virtual void OnMMPostProcessingMovingFilterEvent(MMTweenType curve, bool active, bool toggle, float duration, int channel = 0, bool stop = false)
	{
		if (channel != Channel && channel != -1 && Channel != -1)
		{
			return;
		}
		if (stop)
		{
			_lastReachedState = Active;
			return;
		}
		Curve = curve;
		_duration = duration;
		if (toggle)
		{
			Active = !Active;
		}
		else
		{
			Active = active;
		}
		float lastMovementStartedAt = ((TimeScale == TimeScales.Unscaled) ? Time.unscaledTime : Time.time);
		_lastMovementStartedAt = lastMovementStartedAt;
	}

	protected virtual void OnEnable()
	{
		MMPostProcessingMovingFilterEvent.Register(OnMMPostProcessingMovingFilterEvent);
	}

	protected virtual void OnDisable()
	{
		MMPostProcessingMovingFilterEvent.Unregister(OnMMPostProcessingMovingFilterEvent);
	}

	protected virtual void PostProcessingToggle()
	{
		MMPostProcessingMovingFilterEvent.Trigger(new MMTweenType(MMTween.MMTweenCurve.EaseInOutCubic), active: false, toggle: true, TestDuration);
	}

	protected virtual void PostProcessingTriggerOff()
	{
		MMPostProcessingMovingFilterEvent.Trigger(new MMTweenType(MMTween.MMTweenCurve.EaseInOutCubic), active: false, toggle: false, TestDuration);
	}

	protected virtual void PostProcessingTriggerOn()
	{
		MMPostProcessingMovingFilterEvent.Trigger(new MMTweenType(MMTween.MMTweenCurve.EaseInOutCubic), active: true, toggle: false, TestDuration);
	}
}

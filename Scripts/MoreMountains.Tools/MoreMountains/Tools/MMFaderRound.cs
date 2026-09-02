using UnityEngine;

namespace MoreMountains.Tools;

[RequireComponent(typeof(CanvasGroup))]
[AddComponentMenu("More Mountains/Tools/GUI/MMFaderRound")]
public class MMFaderRound : MonoBehaviour, MMEventListener<MMFadeEvent>, MMEventListenerBase, MMEventListener<MMFadeInEvent>, MMEventListener<MMFadeOutEvent>, MMEventListener<MMFadeStopEvent>
{
	public enum CameraModes
	{
		Main = 0,
		Override = 1
	}

	[Header("Bindings")]
	public CameraModes CameraMode;

	[MMEnumCondition("CameraMode", new int[] { 1 })]
	public Camera TargetCamera;

	public RectTransform FaderBackground;

	public RectTransform FaderMask;

	[Header("Identification")]
	public int ID;

	[Header("Mask")]
	[MMVector(new string[] { "min", "max" })]
	public Vector2 MaskScale;

	[Header("Timing")]
	public float DefaultDuration = 0.2f;

	public MMTweenType DefaultTween = new MMTweenType(MMTween.MMTweenCurve.LinearTween);

	public bool IgnoreTimescale = true;

	[Header("Interaction")]
	public bool ShouldBlockRaycasts;

	[Header("Debug")]
	public Transform DebugWorldPositionTarget;

	[MMInspectorButton("FadeIn1Second")]
	public bool FadeIn1SecondButton;

	[MMInspectorButton("FadeOut1Second")]
	public bool FadeOut1SecondButton;

	[MMInspectorButton("DefaultFade")]
	public bool DefaultFadeButton;

	[MMInspectorButton("ResetFader")]
	public bool ResetFaderButton;

	protected CanvasGroup _canvasGroup;

	protected float _initialScale;

	protected float _currentTargetScale;

	protected float _currentDuration;

	protected MMTweenType _currentCurve;

	protected bool _fading;

	protected float _fadeStartedAt;

	protected virtual void ResetFader()
	{
		FaderMask.transform.localScale = MaskScale.x * Vector3.one;
	}

	protected virtual void DefaultFade()
	{
		MMFadeEvent.Trigger(DefaultDuration, MaskScale.y, DefaultTween, ID, IgnoreTimescale, DebugWorldPositionTarget.transform.position);
	}

	protected virtual void FadeIn1Second()
	{
		MMFadeInEvent.Trigger(1f, DefaultTween, ID, IgnoreTimescale, DebugWorldPositionTarget.transform.position);
	}

	protected virtual void FadeOut1Second()
	{
		MMFadeOutEvent.Trigger(1f, DefaultTween, ID, IgnoreTimescale, DebugWorldPositionTarget.transform.position);
	}

	protected virtual void Awake()
	{
		Initialization();
	}

	protected virtual void Initialization()
	{
		if (CameraMode == CameraModes.Main)
		{
			TargetCamera = Camera.main;
		}
		_canvasGroup = GetComponent<CanvasGroup>();
		FaderMask.transform.localScale = MaskScale.x * Vector3.one;
	}

	protected virtual void Update()
	{
		if (!(_canvasGroup == null) && _fading)
		{
			Fade();
		}
	}

	protected virtual void Fade()
	{
		float num = (IgnoreTimescale ? Time.unscaledTime : Time.time);
		float endTime = _fadeStartedAt + _currentDuration;
		if (num - _fadeStartedAt < _currentDuration)
		{
			float num2 = MMTween.Tween(num, _fadeStartedAt, endTime, _initialScale, _currentTargetScale, _currentCurve);
			FaderMask.transform.localScale = num2 * Vector3.one;
		}
		else
		{
			StopFading();
		}
	}

	protected virtual void StopFading()
	{
		FaderMask.transform.localScale = _currentTargetScale * Vector3.one;
		_fading = false;
		if (FaderMask.transform.localScale == MaskScale.y * Vector3.one)
		{
			DisableFader();
		}
	}

	protected virtual void DisableFader()
	{
		if (ShouldBlockRaycasts)
		{
			_canvasGroup.blocksRaycasts = false;
		}
		_canvasGroup.alpha = 0f;
	}

	protected virtual void EnableFader()
	{
		if (ShouldBlockRaycasts)
		{
			_canvasGroup.blocksRaycasts = true;
		}
		_canvasGroup.alpha = 1f;
	}

	protected virtual void StartFading(float initialAlpha, float endAlpha, float duration, MMTweenType curve, int id, bool ignoreTimeScale, Vector3 worldPosition)
	{
		if (id == ID)
		{
			if (TargetCamera == null)
			{
				Debug.LogWarning(base.name + " : You're using a fader round but its TargetCamera hasn't been setup in its inspector. It can't fade.");
				return;
			}
			FaderMask.anchoredPosition = Vector3.zero;
			Vector3 vector = TargetCamera.WorldToViewportPoint(worldPosition);
			vector.x = Mathf.Clamp01(vector.x);
			vector.y = Mathf.Clamp01(vector.y);
			vector.z = Mathf.Clamp01(vector.z);
			FaderMask.anchorMin = vector;
			FaderMask.anchorMax = vector;
			IgnoreTimescale = ignoreTimeScale;
			EnableFader();
			_fading = true;
			_initialScale = initialAlpha;
			_currentTargetScale = endAlpha;
			_fadeStartedAt = (IgnoreTimescale ? Time.unscaledTime : Time.time);
			_currentCurve = curve;
			_currentDuration = duration;
			float num = MMTween.Tween(0f, 0f, duration, _initialScale, _currentTargetScale, _currentCurve);
			FaderMask.transform.localScale = num * Vector3.one;
		}
	}

	public virtual void OnMMEvent(MMFadeEvent fadeEvent)
	{
		_currentTargetScale = ((fadeEvent.TargetAlpha == -1f) ? MaskScale.y : fadeEvent.TargetAlpha);
		StartFading(FaderMask.transform.localScale.x, _currentTargetScale, fadeEvent.Duration, fadeEvent.Curve, fadeEvent.ID, fadeEvent.IgnoreTimeScale, fadeEvent.WorldPosition);
	}

	public virtual void OnMMEvent(MMFadeInEvent fadeEvent)
	{
		StartFading(MaskScale.y, MaskScale.x, fadeEvent.Duration, fadeEvent.Curve, fadeEvent.ID, fadeEvent.IgnoreTimeScale, fadeEvent.WorldPosition);
	}

	public virtual void OnMMEvent(MMFadeOutEvent fadeEvent)
	{
		StartFading(MaskScale.x, MaskScale.y, fadeEvent.Duration, fadeEvent.Curve, fadeEvent.ID, fadeEvent.IgnoreTimeScale, fadeEvent.WorldPosition);
	}

	public virtual void OnMMEvent(MMFadeStopEvent fadeStopEvent)
	{
		if (fadeStopEvent.ID == ID)
		{
			_fading = false;
		}
	}

	protected virtual void OnEnable()
	{
		this.MMEventStartListening<MMFadeEvent>();
		this.MMEventStartListening<MMFadeStopEvent>();
		this.MMEventStartListening<MMFadeInEvent>();
		this.MMEventStartListening<MMFadeOutEvent>();
	}

	protected virtual void OnDisable()
	{
		this.MMEventStopListening<MMFadeEvent>();
		this.MMEventStopListening<MMFadeStopEvent>();
		this.MMEventStopListening<MMFadeInEvent>();
		this.MMEventStopListening<MMFadeOutEvent>();
	}
}

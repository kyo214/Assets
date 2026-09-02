using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Tools;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(Image))]
[AddComponentMenu("More Mountains/Tools/GUI/MMFader")]
public class MMFader : MonoBehaviour, MMEventListener<MMFadeEvent>, MMEventListenerBase, MMEventListener<MMFadeInEvent>, MMEventListener<MMFadeOutEvent>, MMEventListener<MMFadeStopEvent>
{
	public enum ForcedInitStates
	{
		None = 0,
		Active = 1,
		Inactive = 2
	}

	[Header("Identification")]
	[Tooltip("the ID for this fader (0 is default), set more IDs if you need more than one fader")]
	public int ID;

	[Header("Opacity")]
	[Tooltip("the opacity the fader should be at when inactive")]
	public float InactiveAlpha;

	[Tooltip("the opacity the fader should be at when active")]
	public float ActiveAlpha = 1f;

	[Tooltip("determines whether a state should be forced on init")]
	public ForcedInitStates ForcedInitState = ForcedInitStates.Inactive;

	[Header("Timing")]
	[Tooltip("the default duration of the fade in/out")]
	public float DefaultDuration = 0.2f;

	[Tooltip("the default curve to use for this fader")]
	public MMTweenType DefaultTween = new MMTweenType(MMTween.MMTweenCurve.LinearTween);

	[Tooltip("whether or not the fade should happen in unscaled time")]
	public bool IgnoreTimescale = true;

	[Tooltip("whether or not this fader can cause a fade if the requested final alpha is the same as the current one")]
	public bool CanFadeToCurrentAlpha = true;

	[Header("Interaction")]
	[Tooltip("whether or not the fader should block raycasts when visible")]
	public bool ShouldBlockRaycasts;

	[Header("Debug")]
	[MMInspectorButton("FadeIn1Second")]
	public bool FadeIn1SecondButton;

	[MMInspectorButton("FadeOut1Second")]
	public bool FadeOut1SecondButton;

	[MMInspectorButton("DefaultFade")]
	public bool DefaultFadeButton;

	[MMInspectorButton("ResetFader")]
	public bool ResetFaderButton;

	protected CanvasGroup _canvasGroup;

	protected Image _image;

	protected float _initialAlpha;

	protected float _currentTargetAlpha;

	protected float _currentDuration;

	protected MMTweenType _currentCurve;

	protected bool _fading;

	protected float _fadeStartedAt;

	protected bool _frameCountOne;

	protected virtual void ResetFader()
	{
		_canvasGroup.alpha = InactiveAlpha;
	}

	protected virtual void DefaultFade()
	{
		MMFadeEvent.Trigger(DefaultDuration, ActiveAlpha, DefaultTween, ID);
	}

	protected virtual void FadeIn1Second()
	{
		MMFadeInEvent.Trigger(1f, new MMTweenType(MMTween.MMTweenCurve.LinearTween));
	}

	protected virtual void FadeOut1Second()
	{
		MMFadeOutEvent.Trigger(1f, new MMTweenType(MMTween.MMTweenCurve.LinearTween));
	}

	protected virtual void Awake()
	{
		Initialization();
	}

	protected virtual void Initialization()
	{
		_canvasGroup = GetComponent<CanvasGroup>();
		_image = GetComponent<Image>();
		if (ForcedInitState == ForcedInitStates.Inactive)
		{
			_canvasGroup.alpha = InactiveAlpha;
			_image.enabled = false;
		}
		else if (ForcedInitState == ForcedInitStates.Active)
		{
			_canvasGroup.alpha = ActiveAlpha;
			_image.enabled = true;
		}
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
		if (_frameCountOne)
		{
			if (Time.frameCount <= 2)
			{
				_canvasGroup.alpha = _initialAlpha;
				return;
			}
			_fadeStartedAt = (IgnoreTimescale ? Time.unscaledTime : Time.time);
			num = _fadeStartedAt;
			_frameCountOne = false;
		}
		float endTime = _fadeStartedAt + _currentDuration;
		if (num - _fadeStartedAt < _currentDuration)
		{
			float alpha = MMTween.Tween(num, _fadeStartedAt, endTime, _initialAlpha, _currentTargetAlpha, _currentCurve);
			_canvasGroup.alpha = alpha;
		}
		else
		{
			StopFading();
		}
	}

	protected virtual void StopFading()
	{
		_canvasGroup.alpha = _currentTargetAlpha;
		_fading = false;
		if (_canvasGroup.alpha == InactiveAlpha)
		{
			DisableFader();
		}
	}

	protected virtual void DisableFader()
	{
		_image.enabled = false;
		if (ShouldBlockRaycasts)
		{
			_canvasGroup.blocksRaycasts = false;
		}
	}

	protected virtual void EnableFader()
	{
		_image.enabled = true;
		if (ShouldBlockRaycasts)
		{
			_canvasGroup.blocksRaycasts = true;
		}
	}

	protected virtual void StartFading(float initialAlpha, float endAlpha, float duration, MMTweenType curve, int id, bool ignoreTimeScale)
	{
		if (id == ID && (CanFadeToCurrentAlpha || _canvasGroup.alpha != endAlpha))
		{
			IgnoreTimescale = ignoreTimeScale;
			EnableFader();
			_fading = true;
			_initialAlpha = initialAlpha;
			_currentTargetAlpha = endAlpha;
			_fadeStartedAt = (IgnoreTimescale ? Time.unscaledTime : Time.time);
			_currentCurve = curve;
			_currentDuration = duration;
			if (Time.frameCount == 1)
			{
				_frameCountOne = true;
			}
		}
	}

	public virtual void OnMMEvent(MMFadeEvent fadeEvent)
	{
		_currentTargetAlpha = ((fadeEvent.TargetAlpha == -1f) ? ActiveAlpha : fadeEvent.TargetAlpha);
		StartFading(_canvasGroup.alpha, _currentTargetAlpha, fadeEvent.Duration, fadeEvent.Curve, fadeEvent.ID, fadeEvent.IgnoreTimeScale);
	}

	public virtual void OnMMEvent(MMFadeInEvent fadeEvent)
	{
		StartFading(InactiveAlpha, ActiveAlpha, fadeEvent.Duration, fadeEvent.Curve, fadeEvent.ID, fadeEvent.IgnoreTimeScale);
	}

	public virtual void OnMMEvent(MMFadeOutEvent fadeEvent)
	{
		StartFading(ActiveAlpha, InactiveAlpha, fadeEvent.Duration, fadeEvent.Curve, fadeEvent.ID, fadeEvent.IgnoreTimeScale);
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

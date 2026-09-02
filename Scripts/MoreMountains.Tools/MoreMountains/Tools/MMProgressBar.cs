using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MoreMountains.Tools;

[MMRequiresConstantRepaint]
[AddComponentMenu("More Mountains/Tools/GUI/MMProgressBar")]
public class MMProgressBar : MMMonoBehaviour
{
	public enum MMProgressBarStates
	{
		Idle = 0,
		Decreasing = 1,
		Increasing = 2,
		InDecreasingDelay = 3,
		InIncreasingDelay = 4
	}

	public enum FillModes
	{
		LocalScale = 0,
		FillAmount = 1,
		Width = 2,
		Height = 3,
		Anchor = 4
	}

	public enum BarDirections
	{
		LeftToRight = 0,
		RightToLeft = 1,
		UpToDown = 2,
		DownToUp = 3
	}

	public enum TimeScales
	{
		UnscaledTime = 0,
		Time = 1
	}

	public enum BarFillModes
	{
		SpeedBased = 0,
		FixedDuration = 1
	}

	[MMInspectorGroup("Bindings", true, 10)]
	public string PlayerID;

	public Transform ForegroundBar;

	[FormerlySerializedAs("DelayedBar")]
	public Transform DelayedBarDecreasing;

	public Transform DelayedBarIncreasing;

	[MMInspectorGroup("Fill Settings", true, 11)]
	[FormerlySerializedAs("StartValue")]
	[Range(0f, 1f)]
	public float MinimumBarFillValue;

	[FormerlySerializedAs("EndValue")]
	[Range(0f, 1f)]
	public float MaximumBarFillValue = 1f;

	public bool SetInitialFillValueOnStart;

	[MMCondition("SetInitialFillValueOnStart", true)]
	[Range(0f, 1f)]
	public float InitialFillValue;

	public BarDirections BarDirection;

	public FillModes FillMode;

	public TimeScales TimeScale;

	public BarFillModes BarFillMode;

	[MMInspectorGroup("Foreground Bar Settings", true, 12)]
	public bool LerpForegroundBar = true;

	[MMCondition("LerpForegroundBar", true)]
	public float LerpForegroundBarSpeedDecreasing = 15f;

	[FormerlySerializedAs("LerpForegroundBarSpeed")]
	[MMCondition("LerpForegroundBar", true)]
	public float LerpForegroundBarSpeedIncreasing = 15f;

	[MMCondition("LerpForegroundBar", true)]
	public float LerpForegroundBarDurationDecreasing = 0.2f;

	[MMCondition("LerpForegroundBar", true)]
	public float LerpForegroundBarDurationIncreasing = 0.2f;

	[MMCondition("LerpForegroundBar", true)]
	public AnimationCurve LerpForegroundBarCurveDecreasing = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	[MMCondition("LerpForegroundBar", true)]
	public AnimationCurve LerpForegroundBarCurveIncreasing = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	[MMInspectorGroup("Delayed Bar Decreasing", true, 13)]
	[FormerlySerializedAs("Delay")]
	public float DecreasingDelay = 1f;

	[FormerlySerializedAs("LerpDelayedBar")]
	public bool LerpDecreasingDelayedBar = true;

	[FormerlySerializedAs("LerpDelayedBarSpeed")]
	[MMCondition("LerpDecreasingDelayedBar", true)]
	public float LerpDecreasingDelayedBarSpeed = 15f;

	[FormerlySerializedAs("LerpDelayedBarDuration")]
	[MMCondition("LerpDecreasingDelayedBar", true)]
	public float LerpDecreasingDelayedBarDuration = 0.2f;

	[FormerlySerializedAs("LerpDelayedBarCurve")]
	[MMCondition("LerpDecreasingDelayedBar", true)]
	public AnimationCurve LerpDecreasingDelayedBarCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	[MMInspectorGroup("Delayed Bar Increasing", true, 18)]
	public float IncreasingDelay = 1f;

	public bool LerpIncreasingDelayedBar = true;

	[MMCondition("LerpIncreasingDelayedBar", true)]
	public float LerpIncreasingDelayedBarSpeed = 15f;

	[MMCondition("LerpIncreasingDelayedBar", true)]
	public float LerpIncreasingDelayedBarDuration = 0.2f;

	[MMCondition("LerpIncreasingDelayedBar", true)]
	public AnimationCurve LerpIncreasingDelayedBarCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	[MMInspectorGroup("Bump", true, 14)]
	public bool BumpScaleOnChange = true;

	public bool BumpOnIncrease;

	public bool BumpOnDecrease;

	public float BumpDuration = 0.2f;

	public bool ChangeColorWhenBumping = true;

	[MMCondition("ChangeColorWhenBumping", true)]
	public Color BumpColor = Color.white;

	[FormerlySerializedAs("BumpAnimationCurve")]
	public AnimationCurve BumpScaleAnimationCurve = new AnimationCurve(new Keyframe(1f, 1f), new Keyframe(0.3f, 1.05f), new Keyframe(1f, 1f));

	public AnimationCurve BumpColorAnimationCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1f), new Keyframe(1f, 0f));

	[MMInspectorGroup("Events", true, 16)]
	public UnityEvent OnBump;

	public UnityEvent OnBarMovementDecreasingStart;

	public UnityEvent OnBarMovementDecreasingStop;

	public UnityEvent OnBarMovementIncreasingStart;

	public UnityEvent OnBarMovementIncreasingStop;

	[MMInspectorGroup("Text", true, 20)]
	public Text PercentageText;

	public string TextPrefix;

	public string TextSuffix;

	public float TextValueMultiplier = 1f;

	public string TextFormat = "{000}";

	[MMInspectorGroup("Debug", true, 15)]
	[Range(0f, 1f)]
	public float DebugNewTargetValue;

	[MMInspectorButton("DebugUpdateBar")]
	public bool DebugUpdateBarButton;

	[MMInspectorButton("DebugSetBar")]
	public bool DebugSetBarButton;

	[MMInspectorButton("Bump")]
	public bool TestBumpButton;

	[MMInspectorButton("Plus10Percent")]
	public bool Plus10PercentButton;

	[MMInspectorButton("Minus10Percent")]
	public bool Minus10PercentButton;

	[MMInspectorGroup("Debug Read Only", true, 19)]
	[Range(0f, 1f)]
	public float BarProgress;

	[Range(0f, 1f)]
	public float BarTarget;

	[Range(0f, 1f)]
	public float DelayedBarIncreasingProgress;

	[Range(0f, 1f)]
	public float DelayedBarDecreasingProgress;

	protected bool _initialized;

	protected Vector2 _initialBarSize;

	protected Color _initialColor;

	protected Vector3 _initialScale;

	protected Image _foregroundImage;

	protected Image _delayedDecreasingImage;

	protected Image _delayedIncreasingImage;

	protected Vector3 _targetLocalScale = Vector3.one;

	protected float _newPercent;

	protected float _percentLastTimeBarWasUpdated;

	protected float _lastUpdateTimestamp;

	protected float _time;

	protected float _deltaTime;

	protected int _direction;

	protected Coroutine _coroutine;

	protected bool _coroutineShouldRun;

	protected bool _isDelayedBarIncreasingNotNull;

	protected bool _isDelayedBarDecreasingNotNull;

	protected bool _actualUpdate;

	protected Vector2 _anchorVector;

	protected float _delayedBarDecreasingProgress;

	protected float _delayedBarIncreasingProgress;

	protected MMProgressBarStates CurrentState;

	public bool Bumping { get; protected set; }

	public virtual void UpdateBar01(float normalizedValue)
	{
		UpdateBar(Mathf.Clamp01(normalizedValue), 0f, 1f);
	}

	public virtual void UpdateBar(float currentValue, float minValue, float maxValue)
	{
		if (!_initialized)
		{
			Initialization();
		}
		if (!base.gameObject.activeInHierarchy)
		{
			base.gameObject.SetActive(value: true);
		}
		_newPercent = MMMaths.Remap(currentValue, minValue, maxValue, MinimumBarFillValue, MaximumBarFillValue);
		_actualUpdate = BarTarget != _newPercent;
		if (!_actualUpdate)
		{
			return;
		}
		if (CurrentState != MMProgressBarStates.Idle)
		{
			if ((CurrentState == MMProgressBarStates.Decreasing || CurrentState == MMProgressBarStates.InDecreasingDelay) && _newPercent >= BarTarget)
			{
				StopCoroutine(_coroutine);
				SetBar01(BarTarget);
			}
			if ((CurrentState == MMProgressBarStates.Increasing || CurrentState == MMProgressBarStates.InIncreasingDelay) && _newPercent <= BarTarget)
			{
				StopCoroutine(_coroutine);
				SetBar01(BarTarget);
			}
		}
		_percentLastTimeBarWasUpdated = BarProgress;
		_delayedBarDecreasingProgress = DelayedBarDecreasingProgress;
		_delayedBarIncreasingProgress = DelayedBarIncreasingProgress;
		BarTarget = _newPercent;
		if (_newPercent != _percentLastTimeBarWasUpdated && !Bumping)
		{
			Bump();
		}
		DetermineDeltaTime();
		_lastUpdateTimestamp = _time;
		DetermineDirection();
		if (_direction < 0)
		{
			OnBarMovementDecreasingStart?.Invoke();
		}
		else
		{
			OnBarMovementIncreasingStart?.Invoke();
		}
		if (_coroutine != null)
		{
			StopCoroutine(_coroutine);
		}
		_coroutineShouldRun = true;
		if (base.gameObject.activeInHierarchy)
		{
			_coroutine = StartCoroutine(UpdateBarsCo());
		}
		else
		{
			SetBar(currentValue, minValue, maxValue);
		}
		UpdateText();
	}

	public virtual void SetBar(float currentValue, float minValue, float maxValue)
	{
		float bar = MMMaths.Remap(currentValue, minValue, maxValue, 0f, 1f);
		SetBar01(bar);
	}

	public virtual void SetBar01(float newPercent)
	{
		if (!_initialized)
		{
			Initialization();
		}
		newPercent = MMMaths.Remap(newPercent, 0f, 1f, MinimumBarFillValue, MaximumBarFillValue);
		BarProgress = newPercent;
		DelayedBarDecreasingProgress = newPercent;
		DelayedBarIncreasingProgress = newPercent;
		BarTarget = newPercent;
		_percentLastTimeBarWasUpdated = newPercent;
		_delayedBarDecreasingProgress = DelayedBarDecreasingProgress;
		_delayedBarIncreasingProgress = DelayedBarIncreasingProgress;
		SetBarInternal(newPercent, ForegroundBar, _foregroundImage, _initialBarSize);
		SetBarInternal(newPercent, DelayedBarDecreasing, _delayedDecreasingImage, _initialBarSize);
		SetBarInternal(newPercent, DelayedBarIncreasing, _delayedIncreasingImage, _initialBarSize);
		UpdateText();
		_coroutineShouldRun = false;
		CurrentState = MMProgressBarStates.Idle;
	}

	protected virtual void Start()
	{
		Initialization();
	}

	protected virtual void OnEnable()
	{
		if (_initialized && _foregroundImage != null)
		{
			_foregroundImage.color = _initialColor;
		}
	}

	public virtual void Initialization()
	{
		_isDelayedBarDecreasingNotNull = DelayedBarDecreasing != null;
		_isDelayedBarIncreasingNotNull = DelayedBarIncreasing != null;
		_initialScale = base.transform.localScale;
		if (ForegroundBar != null)
		{
			_foregroundImage = ForegroundBar.GetComponent<Image>();
			_initialBarSize = _foregroundImage.rectTransform.sizeDelta;
		}
		if (DelayedBarDecreasing != null)
		{
			_delayedDecreasingImage = DelayedBarDecreasing.GetComponent<Image>();
		}
		if (DelayedBarIncreasing != null)
		{
			_delayedIncreasingImage = DelayedBarIncreasing.GetComponent<Image>();
		}
		_initialized = true;
		if (_foregroundImage != null)
		{
			_initialColor = _foregroundImage.color;
		}
		_percentLastTimeBarWasUpdated = BarProgress;
		if (SetInitialFillValueOnStart)
		{
			SetBar01(InitialFillValue);
		}
	}

	protected virtual void DebugUpdateBar()
	{
		UpdateBar01(DebugNewTargetValue);
	}

	protected virtual void DebugSetBar()
	{
		SetBar01(DebugNewTargetValue);
	}

	public virtual void Plus10Percent()
	{
		float value = BarTarget + 0.1f;
		value = Mathf.Clamp(value, 0f, 1f);
		UpdateBar01(value);
	}

	public virtual void Minus10Percent()
	{
		float value = BarTarget - 0.1f;
		value = Mathf.Clamp(value, 0f, 1f);
		UpdateBar01(value);
	}

	protected virtual void UpdateText()
	{
		if (!(PercentageText == null))
		{
			PercentageText.text = TextPrefix + (BarTarget * TextValueMultiplier).ToString(TextFormat) + TextSuffix;
		}
	}

	protected virtual IEnumerator UpdateBarsCo()
	{
		while (_coroutineShouldRun)
		{
			DetermineDeltaTime();
			DetermineDirection();
			UpdateBars();
			yield return null;
		}
		CurrentState = MMProgressBarStates.Idle;
	}

	protected virtual void DetermineDeltaTime()
	{
		_deltaTime = ((TimeScale == TimeScales.Time) ? Time.deltaTime : Time.unscaledDeltaTime);
		_time = ((TimeScale == TimeScales.Time) ? Time.time : Time.unscaledTime);
	}

	protected virtual void DetermineDirection()
	{
		_direction = ((_newPercent > _percentLastTimeBarWasUpdated) ? 1 : (-1));
	}

	protected virtual void UpdateBars()
	{
		float t = 0f;
		float t2;
		if (_direction < 0)
		{
			float num = ComputeNewFill(LerpForegroundBar, LerpForegroundBarSpeedDecreasing, LerpForegroundBarDurationDecreasing, LerpForegroundBarCurveDecreasing, 0f, _percentLastTimeBarWasUpdated, out t2);
			SetBarInternal(num, ForegroundBar, _foregroundImage, _initialBarSize);
			SetBarInternal(num, DelayedBarIncreasing, _delayedIncreasingImage, _initialBarSize);
			BarProgress = num;
			DelayedBarIncreasingProgress = num;
			CurrentState = MMProgressBarStates.Decreasing;
			if (_time - _lastUpdateTimestamp > DecreasingDelay)
			{
				float num2 = ComputeNewFill(LerpDecreasingDelayedBar, LerpDecreasingDelayedBarSpeed, LerpDecreasingDelayedBarDuration, LerpDecreasingDelayedBarCurve, DecreasingDelay, _delayedBarDecreasingProgress, out t);
				SetBarInternal(num2, DelayedBarDecreasing, _delayedDecreasingImage, _initialBarSize);
				DelayedBarDecreasingProgress = num2;
				CurrentState = MMProgressBarStates.InDecreasingDelay;
			}
		}
		else
		{
			float num = ComputeNewFill(LerpForegroundBar, LerpForegroundBarSpeedIncreasing, LerpForegroundBarDurationIncreasing, LerpForegroundBarCurveIncreasing, 0f, _delayedBarIncreasingProgress, out t2);
			SetBarInternal(num, DelayedBarIncreasing, _delayedIncreasingImage, _initialBarSize);
			DelayedBarIncreasingProgress = num;
			CurrentState = MMProgressBarStates.Increasing;
			if (DelayedBarIncreasing == null)
			{
				num = ComputeNewFill(LerpForegroundBar, LerpForegroundBarSpeedIncreasing, LerpForegroundBarDurationIncreasing, LerpForegroundBarCurveIncreasing, 0f, _percentLastTimeBarWasUpdated, out t);
				SetBarInternal(num, DelayedBarDecreasing, _delayedDecreasingImage, _initialBarSize);
				SetBarInternal(num, ForegroundBar, _foregroundImage, _initialBarSize);
				BarProgress = num;
				DelayedBarDecreasingProgress = num;
				CurrentState = MMProgressBarStates.InDecreasingDelay;
			}
			else if (_time - _lastUpdateTimestamp > IncreasingDelay)
			{
				float num2 = ComputeNewFill(LerpIncreasingDelayedBar, LerpForegroundBarSpeedIncreasing, LerpForegroundBarDurationIncreasing, LerpForegroundBarCurveIncreasing, IncreasingDelay, _delayedBarDecreasingProgress, out t);
				SetBarInternal(num2, DelayedBarDecreasing, _delayedDecreasingImage, _initialBarSize);
				SetBarInternal(num2, ForegroundBar, _foregroundImage, _initialBarSize);
				BarProgress = num2;
				DelayedBarDecreasingProgress = num2;
				CurrentState = MMProgressBarStates.InDecreasingDelay;
			}
		}
		if (t2 >= 1f && t >= 1f)
		{
			_coroutineShouldRun = false;
			if (_direction > 0)
			{
				OnBarMovementIncreasingStop?.Invoke();
			}
			else
			{
				OnBarMovementDecreasingStop?.Invoke();
			}
		}
	}

	protected virtual float ComputeNewFill(bool lerpBar, float barSpeed, float barDuration, AnimationCurve barCurve, float delay, float lastPercent, out float t)
	{
		float num = 0f;
		t = 0f;
		if (lerpBar)
		{
			float num2 = 0f;
			float x = _time - _lastUpdateTimestamp - delay;
			float num3 = barSpeed;
			if (num3 == 0f)
			{
				num3 = 1f;
			}
			float b = ((BarFillMode == BarFillModes.FixedDuration) ? barDuration : (Mathf.Abs(_newPercent - lastPercent) / num3));
			num2 = MMMaths.Remap(x, 0f, b, 0f, 1f);
			num2 = (t = Mathf.Clamp(num2, 0f, 1f));
			if (t < 1f)
			{
				num2 = barCurve.Evaluate(num2);
				num = Mathf.LerpUnclamped(lastPercent, _newPercent, num2);
			}
			else
			{
				num = _newPercent;
			}
		}
		else
		{
			num = _newPercent;
		}
		return Mathf.Clamp(num, 0f, 1f);
	}

	protected virtual void SetBarInternal(float newAmount, Transform bar, Image image, Vector2 initialSize)
	{
		if (bar == null)
		{
			return;
		}
		switch (FillMode)
		{
		case FillModes.LocalScale:
			_targetLocalScale = Vector3.one;
			switch (BarDirection)
			{
			case BarDirections.LeftToRight:
				_targetLocalScale.x = newAmount;
				break;
			case BarDirections.RightToLeft:
				_targetLocalScale.x = 1f - newAmount;
				break;
			case BarDirections.DownToUp:
				_targetLocalScale.y = newAmount;
				break;
			case BarDirections.UpToDown:
				_targetLocalScale.y = 1f - newAmount;
				break;
			}
			bar.localScale = _targetLocalScale;
			break;
		case FillModes.Width:
			if (!(image == null))
			{
				float size = MMMaths.Remap(newAmount, 0f, 1f, 0f, initialSize.x);
				image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
			}
			break;
		case FillModes.Height:
			if (!(image == null))
			{
				float size2 = MMMaths.Remap(newAmount, 0f, 1f, 0f, initialSize.y);
				image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size2);
			}
			break;
		case FillModes.FillAmount:
			if (!(image == null))
			{
				image.fillAmount = newAmount;
			}
			break;
		case FillModes.Anchor:
			if (!(image == null))
			{
				switch (BarDirection)
				{
				case BarDirections.LeftToRight:
					_anchorVector.x = 0f;
					_anchorVector.y = 0f;
					image.rectTransform.anchorMin = _anchorVector;
					_anchorVector.x = newAmount;
					_anchorVector.y = 1f;
					image.rectTransform.anchorMax = _anchorVector;
					break;
				case BarDirections.RightToLeft:
					_anchorVector.x = newAmount;
					_anchorVector.y = 0f;
					image.rectTransform.anchorMin = _anchorVector;
					_anchorVector.x = 1f;
					_anchorVector.y = 1f;
					image.rectTransform.anchorMax = _anchorVector;
					break;
				case BarDirections.DownToUp:
					_anchorVector.x = 0f;
					_anchorVector.y = 0f;
					image.rectTransform.anchorMin = _anchorVector;
					_anchorVector.x = 1f;
					_anchorVector.y = newAmount;
					image.rectTransform.anchorMax = _anchorVector;
					break;
				case BarDirections.UpToDown:
					_anchorVector.x = 0f;
					_anchorVector.y = newAmount;
					image.rectTransform.anchorMin = _anchorVector;
					_anchorVector.x = 1f;
					_anchorVector.y = 1f;
					image.rectTransform.anchorMax = _anchorVector;
					break;
				}
			}
			break;
		}
	}

	public virtual void Bump()
	{
		bool flag = false;
		if (!_initialized)
		{
			return;
		}
		DetermineDirection();
		if (BumpOnIncrease && _direction > 0)
		{
			flag = true;
		}
		if (BumpOnDecrease && _direction < 0)
		{
			flag = true;
		}
		if (BumpScaleOnChange)
		{
			flag = true;
		}
		if (flag)
		{
			if (base.gameObject.activeInHierarchy)
			{
				StartCoroutine(BumpCoroutine());
			}
			OnBump?.Invoke();
		}
	}

	protected virtual IEnumerator BumpCoroutine()
	{
		float journey = 0f;
		Bumping = true;
		while (journey <= BumpDuration)
		{
			journey += _deltaTime;
			float time = Mathf.Clamp01(journey / BumpDuration);
			float num = BumpScaleAnimationCurve.Evaluate(time);
			float t = BumpColorAnimationCurve.Evaluate(time);
			base.transform.localScale = num * _initialScale;
			if (ChangeColorWhenBumping && _foregroundImage != null)
			{
				_foregroundImage.color = Color.Lerp(_initialColor, BumpColor, t);
			}
			yield return null;
		}
		if (ChangeColorWhenBumping && _foregroundImage != null)
		{
			_foregroundImage.color = _initialColor;
		}
		Bumping = false;
		yield return null;
	}

	public virtual void ShowBar()
	{
		base.gameObject.SetActive(value: true);
	}

	public virtual void HideBar(float delay)
	{
		if (delay <= 0f)
		{
			base.gameObject.SetActive(value: false);
		}
		else if (base.gameObject.activeInHierarchy)
		{
			StartCoroutine(HideBarCo(delay));
		}
	}

	protected virtual IEnumerator HideBarCo(float delay)
	{
		yield return MMCoroutine.WaitFor(delay);
		base.gameObject.SetActive(value: false);
	}
}

using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MoreMountains.FeedbacksForThirdParty;

[RequireComponent(typeof(Volume))]
[AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMWhiteBalanceShaker_URP")]
public class MMWhiteBalanceShaker_URP : MMShaker
{
	[Tooltip("whether or not to add to the initial value")]
	public bool RelativeValues = true;

	[Header("Temperature")]
	[Tooltip("the curve used to animate the temperature value on")]
	public AnimationCurve ShakeTemperature = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-100f, 100f)]
	public float RemapTemperatureZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-100f, 100f)]
	public float RemapTemperatureOne = 100f;

	[Header("Tint")]
	[Tooltip("the curve used to animate the tint value on")]
	public AnimationCurve ShakeTint = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-100f, 100f)]
	public float RemapTintZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-100f, 100f)]
	public float RemapTintOne = 100f;

	protected Volume _volume;

	protected WhiteBalance _whiteBalance;

	protected float _initialTemperature;

	protected float _initialTint;

	protected float _originalShakeDuration;

	protected bool _originalRelativeValues;

	protected AnimationCurve _originalShakeTemperature;

	protected float _originalRemapTemperatureZero;

	protected float _originalRemapTemperatureOne;

	protected AnimationCurve _originalShakeTint;

	protected float _originalRemapTintZero;

	protected float _originalRemapTintOne;

	protected override void Initialization()
	{
		base.Initialization();
		_volume = base.gameObject.GetComponent<Volume>();
		_volume.profile.TryGet<WhiteBalance>(out _whiteBalance);
	}

	protected override void Shake()
	{
		float x = ShakeFloat(ShakeTemperature, RemapTemperatureZero, RemapTemperatureOne, RelativeValues, _initialTemperature);
		_whiteBalance.temperature.Override(x);
		float x2 = ShakeFloat(ShakeTint, RemapTintZero, RemapTintOne, RelativeValues, _initialTint);
		_whiteBalance.tint.Override(x2);
	}

	protected override void GrabInitialValues()
	{
		_initialTemperature = _whiteBalance.temperature.value;
		_initialTint = _whiteBalance.tint.value;
	}

	public virtual void OnWhiteBalanceShakeEvent(AnimationCurve temperature, float duration, float remapTemperatureMin, float remapTemperatureMax, AnimationCurve tint, float remapTintMin, float remapTintMax, bool relativeValues = false, float attenuation = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
	{
		if (!CheckEventAllowed(channel) || (!Interruptible && Shaking))
		{
			return;
		}
		if (stop)
		{
			Stop();
			return;
		}
		_resetShakerValuesAfterShake = resetShakerValuesAfterShake;
		_resetTargetValuesAfterShake = resetTargetValuesAfterShake;
		if (resetShakerValuesAfterShake)
		{
			_originalShakeDuration = ShakeDuration;
			_originalShakeTemperature = ShakeTemperature;
			_originalRemapTemperatureZero = RemapTemperatureZero;
			_originalRemapTemperatureOne = RemapTemperatureOne;
			_originalRelativeValues = RelativeValues;
			_originalShakeTint = ShakeTint;
			_originalRemapTintZero = RemapTintZero;
			_originalRemapTintOne = RemapTintOne;
		}
		TimescaleMode = timescaleMode;
		ShakeDuration = duration;
		ShakeTemperature = temperature;
		RemapTemperatureZero = remapTemperatureMin * attenuation;
		RemapTemperatureOne = remapTemperatureMax * attenuation;
		RelativeValues = relativeValues;
		ShakeTint = tint;
		RemapTintZero = remapTintMin;
		RemapTintOne = remapTintMax;
		ForwardDirection = forwardDirection;
		Play();
	}

	protected override void ResetTargetValues()
	{
		base.ResetTargetValues();
		_whiteBalance.temperature.Override(_initialTemperature);
		_whiteBalance.tint.Override(_initialTint);
	}

	protected override void ResetShakerValues()
	{
		base.ResetShakerValues();
		ShakeDuration = _originalShakeDuration;
		ShakeTemperature = _originalShakeTemperature;
		RemapTemperatureZero = _originalRemapTemperatureZero;
		RemapTemperatureOne = _originalRemapTemperatureOne;
		RelativeValues = _originalRelativeValues;
		ShakeTint = _originalShakeTint;
		RemapTintZero = _originalRemapTintZero;
		RemapTintOne = _originalRemapTintOne;
	}

	public override void StartListening()
	{
		base.StartListening();
		MMWhiteBalanceShakeEvent_URP.Register(OnWhiteBalanceShakeEvent);
	}

	public override void StopListening()
	{
		base.StopListening();
		MMWhiteBalanceShakeEvent_URP.Unregister(OnWhiteBalanceShakeEvent);
	}
}

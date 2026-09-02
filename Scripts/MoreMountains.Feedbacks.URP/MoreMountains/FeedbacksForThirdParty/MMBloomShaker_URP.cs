using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MoreMountains.FeedbacksForThirdParty;

[RequireComponent(typeof(Volume))]
[AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMBloomShaker_URP")]
public class MMBloomShaker_URP : MMShaker
{
	public bool RelativeValues = true;

	[Header("Intensity")]
	[Tooltip("the curve used to animate the intensity value on")]
	public AnimationCurve ShakeIntensity = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	public float RemapIntensityZero;

	[Tooltip("the value to remap the curve's 1 to")]
	public float RemapIntensityOne = 1f;

	[Header("Threshold")]
	[Tooltip("the curve used to animate the threshold value on")]
	public AnimationCurve ShakeThreshold = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	public float RemapThresholdZero;

	[Tooltip("the value to remap the curve's 1 to")]
	public float RemapThresholdOne;

	protected Volume _volume;

	protected Bloom _bloom;

	protected float _initialIntensity;

	protected float _initialThreshold;

	protected float _originalShakeDuration;

	protected bool _originalRelativeIntensity;

	protected AnimationCurve _originalShakeIntensity;

	protected float _originalRemapIntensityZero;

	protected float _originalRemapIntensityOne;

	protected AnimationCurve _originalShakeThreshold;

	protected float _originalRemapThresholdZero;

	protected float _originalRemapThresholdOne;

	protected override void Initialization()
	{
		base.Initialization();
		_volume = base.gameObject.GetComponent<Volume>();
		_volume.profile.TryGet<Bloom>(out _bloom);
	}

	protected override void Shake()
	{
		float x = ShakeFloat(ShakeIntensity, RemapIntensityZero, RemapIntensityOne, RelativeValues, _initialIntensity);
		_bloom.intensity.Override(x);
		float x2 = ShakeFloat(ShakeThreshold, RemapThresholdZero, RemapThresholdOne, RelativeValues, _initialThreshold);
		_bloom.threshold.Override(x2);
	}

	protected override void GrabInitialValues()
	{
		_initialIntensity = _bloom.intensity.value;
		_initialThreshold = _bloom.threshold.value;
	}

	public virtual void OnBloomShakeEvent(AnimationCurve intensity, float duration, float remapMin, float remapMax, AnimationCurve threshold, float remapThresholdMin, float remapThresholdMax, bool relativeIntensity = false, float attenuation = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
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
			_originalShakeIntensity = ShakeIntensity;
			_originalRemapIntensityZero = RemapIntensityZero;
			_originalRemapIntensityOne = RemapIntensityOne;
			_originalRelativeIntensity = RelativeValues;
			_originalShakeThreshold = ShakeThreshold;
			_originalRemapThresholdZero = RemapThresholdZero;
			_originalRemapThresholdOne = RemapThresholdOne;
		}
		TimescaleMode = timescaleMode;
		ShakeDuration = duration;
		ShakeIntensity = intensity;
		RemapIntensityZero = remapMin * attenuation;
		RemapIntensityOne = remapMax * attenuation;
		RelativeValues = relativeIntensity;
		ShakeThreshold = threshold;
		RemapThresholdZero = remapThresholdMin;
		RemapThresholdOne = remapThresholdMax;
		ForwardDirection = forwardDirection;
		Play();
	}

	protected override void ResetTargetValues()
	{
		base.ResetTargetValues();
		_bloom.intensity.Override(_initialIntensity);
		_bloom.threshold.Override(_initialThreshold);
	}

	protected override void ResetShakerValues()
	{
		base.ResetShakerValues();
		ShakeDuration = _originalShakeDuration;
		ShakeIntensity = _originalShakeIntensity;
		RemapIntensityZero = _originalRemapIntensityZero;
		RemapIntensityOne = _originalRemapIntensityOne;
		RelativeValues = _originalRelativeIntensity;
		ShakeThreshold = _originalShakeThreshold;
		RemapThresholdZero = _originalRemapThresholdZero;
		RemapThresholdOne = _originalRemapThresholdOne;
	}

	public override void StartListening()
	{
		base.StartListening();
		MMBloomShakeEvent_URP.Register(OnBloomShakeEvent);
	}

	public override void StopListening()
	{
		base.StopListening();
		MMBloomShakeEvent_URP.Unregister(OnBloomShakeEvent);
	}
}

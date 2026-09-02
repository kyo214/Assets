using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MoreMountains.FeedbacksForThirdParty;

[RequireComponent(typeof(Volume))]
[AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMLensDistortionShaker_URP")]
public class MMLensDistortionShaker_URP : MMShaker
{
	[Header("Intensity")]
	[Tooltip("whether or not to add to the initial value")]
	public bool RelativeIntensity;

	[Tooltip("the curve used to animate the intensity value on")]
	public AnimationCurve ShakeIntensity = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.2f, 1f), new Keyframe(0.25f, -1f), new Keyframe(0.35f, 0.7f), new Keyframe(0.4f, -0.7f), new Keyframe(0.6f, 0.3f), new Keyframe(0.65f, -0.3f), new Keyframe(0.8f, 0.1f), new Keyframe(0.85f, -0.1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-100f, 100f)]
	public float RemapIntensityZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-100f, 100f)]
	public float RemapIntensityOne = 0.5f;

	protected Volume _volume;

	protected LensDistortion _lensDistortion;

	protected float _initialIntensity;

	protected float _originalShakeDuration;

	protected AnimationCurve _originalShakeIntensity;

	protected float _originalRemapIntensityZero;

	protected float _originalRemapIntensityOne;

	protected bool _originalRelativeIntensity;

	protected override void Initialization()
	{
		base.Initialization();
		_volume = base.gameObject.GetComponent<Volume>();
		_volume.profile.TryGet<LensDistortion>(out _lensDistortion);
	}

	protected virtual void Reset()
	{
		ShakeDuration = 0.8f;
	}

	protected override void Shake()
	{
		float x = ShakeFloat(ShakeIntensity, RemapIntensityZero, RemapIntensityOne, RelativeIntensity, _initialIntensity);
		_lensDistortion.intensity.Override(x);
	}

	protected override void GrabInitialValues()
	{
		_initialIntensity = _lensDistortion.intensity.value;
	}

	public virtual void OnMMLensDistortionShakeEvent(AnimationCurve intensity, float duration, float remapMin, float remapMax, bool relativeIntensity = false, float attenuation = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
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
			_originalRelativeIntensity = RelativeIntensity;
		}
		TimescaleMode = timescaleMode;
		ShakeDuration = duration;
		ShakeIntensity = intensity;
		RemapIntensityZero = remapMin * attenuation;
		RemapIntensityOne = remapMax * attenuation;
		RelativeIntensity = relativeIntensity;
		ForwardDirection = forwardDirection;
		Play();
	}

	protected override void ResetTargetValues()
	{
		base.ResetTargetValues();
		_lensDistortion.intensity.Override(_initialIntensity);
	}

	protected override void ResetShakerValues()
	{
		base.ResetShakerValues();
		ShakeDuration = _originalShakeDuration;
		ShakeIntensity = _originalShakeIntensity;
		RemapIntensityZero = _originalRemapIntensityZero;
		RemapIntensityOne = _originalRemapIntensityOne;
		RelativeIntensity = _originalRelativeIntensity;
	}

	public override void StartListening()
	{
		base.StartListening();
		MMLensDistortionShakeEvent_URP.Register(OnMMLensDistortionShakeEvent);
	}

	public override void StopListening()
	{
		base.StopListening();
		MMLensDistortionShakeEvent_URP.Unregister(OnMMLensDistortionShakeEvent);
	}
}

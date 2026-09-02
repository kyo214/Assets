using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MoreMountains.FeedbacksForThirdParty;

[RequireComponent(typeof(Volume))]
[AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMChromaticAberrationShaker_URP")]
public class MMChromaticAberrationShaker_URP : MMShaker
{
	[Header("Intensity")]
	[Tooltip("whether or not to add to the initial value")]
	public bool RelativeIntensity;

	[Tooltip("the curve used to animate the intensity value on")]
	public AnimationCurve ShakeIntensity = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(0f, 1f)]
	public float RemapIntensityZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(0f, 1f)]
	public float RemapIntensityOne = 1f;

	protected Volume _volume;

	protected ChromaticAberration _chromaticAberration;

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
		_volume.profile.TryGet<ChromaticAberration>(out _chromaticAberration);
	}

	protected override void Shake()
	{
		float x = ShakeFloat(ShakeIntensity, RemapIntensityZero, RemapIntensityOne, RelativeIntensity, _initialIntensity);
		_chromaticAberration.intensity.Override(x);
	}

	protected override void GrabInitialValues()
	{
		_initialIntensity = _chromaticAberration.intensity.value;
	}

	public virtual void OnMMChromaticAberrationShakeEvent(AnimationCurve intensity, float duration, float remapMin, float remapMax, bool relativeIntensity = false, float attenuation = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
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
		_chromaticAberration.intensity.Override(_initialIntensity);
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
		MMChromaticAberrationShakeEvent_URP.Register(OnMMChromaticAberrationShakeEvent);
	}

	public override void StopListening()
	{
		base.StopListening();
		MMChromaticAberrationShakeEvent_URP.Unregister(OnMMChromaticAberrationShakeEvent);
	}
}

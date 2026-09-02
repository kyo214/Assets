using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("More Mountains/Feedbacks/Shakers/Audio/MMAudioFilterLowPassShaker")]
[RequireComponent(typeof(AudioLowPassFilter))]
public class MMAudioFilterLowPassShaker : MMShaker
{
	[Header("Low Pass")]
	[Tooltip("whether or not to add to the initial value")]
	public bool RelativeLowPass;

	[Tooltip("the curve used to animate the intensity value on")]
	public AnimationCurve ShakeLowPass = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.5f, 0f), new Keyframe(1f, 1f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(10f, 22000f)]
	public float RemapLowPassZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(10f, 22000f)]
	public float RemapLowPassOne = 10000f;

	protected AudioLowPassFilter _targetAudioLowPassFilter;

	protected float _initialLowPass;

	protected float _originalShakeDuration;

	protected bool _originalRelativeLowPass;

	protected AnimationCurve _originalShakeLowPass;

	protected float _originalRemapLowPassZero;

	protected float _originalRemapLowPassOne;

	protected override void Initialization()
	{
		base.Initialization();
		_targetAudioLowPassFilter = base.gameObject.GetComponent<AudioLowPassFilter>();
	}

	protected virtual void Reset()
	{
		ShakeDuration = 2f;
	}

	protected override void Shake()
	{
		float cutoffFrequency = ShakeFloat(ShakeLowPass, RemapLowPassZero, RemapLowPassOne, RelativeLowPass, _initialLowPass);
		_targetAudioLowPassFilter.cutoffFrequency = cutoffFrequency;
	}

	protected override void GrabInitialValues()
	{
		_initialLowPass = _targetAudioLowPassFilter.cutoffFrequency;
	}

	public virtual void OnMMAudioFilterLowPassShakeEvent(AnimationCurve lowPassCurve, float duration, float remapMin, float remapMax, bool relativeLowPass = false, float feedbacksIntensity = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
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
			_originalShakeLowPass = ShakeLowPass;
			_originalRemapLowPassZero = RemapLowPassZero;
			_originalRemapLowPassOne = RemapLowPassOne;
			_originalRelativeLowPass = RelativeLowPass;
		}
		TimescaleMode = timescaleMode;
		ShakeDuration = duration;
		ShakeLowPass = lowPassCurve;
		RemapLowPassZero = remapMin * feedbacksIntensity;
		RemapLowPassOne = remapMax * feedbacksIntensity;
		RelativeLowPass = relativeLowPass;
		ForwardDirection = forwardDirection;
		Play();
	}

	protected override void ResetTargetValues()
	{
		base.ResetTargetValues();
		_targetAudioLowPassFilter.cutoffFrequency = _initialLowPass;
	}

	protected override void ResetShakerValues()
	{
		base.ResetShakerValues();
		ShakeDuration = _originalShakeDuration;
		ShakeLowPass = _originalShakeLowPass;
		RemapLowPassZero = _originalRemapLowPassZero;
		RemapLowPassOne = _originalRemapLowPassOne;
		RelativeLowPass = _originalRelativeLowPass;
	}

	public override void StartListening()
	{
		base.StartListening();
		MMAudioFilterLowPassShakeEvent.Register(OnMMAudioFilterLowPassShakeEvent);
	}

	public override void StopListening()
	{
		base.StopListening();
		MMAudioFilterLowPassShakeEvent.Unregister(OnMMAudioFilterLowPassShakeEvent);
	}
}

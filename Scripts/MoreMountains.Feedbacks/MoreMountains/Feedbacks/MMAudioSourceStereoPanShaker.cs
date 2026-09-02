using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("More Mountains/Feedbacks/Shakers/Audio/MMAudioSourceStereoPanShaker")]
[RequireComponent(typeof(AudioSource))]
public class MMAudioSourceStereoPanShaker : MMShaker
{
	[Header("Stereo Pan")]
	[Tooltip("whether or not to add to the initial value")]
	public bool RelativeStereoPan;

	[Tooltip("the curve used to animate the intensity value on")]
	public AnimationCurve ShakeStereoPan = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1f), new Keyframe(0.6f, -1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-1f, 1f)]
	public float RemapStereoPanZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-1f, 1f)]
	public float RemapStereoPanOne = 1f;

	protected AudioSource _targetAudioSource;

	protected float _initialStereoPan;

	protected float _originalShakeDuration;

	protected bool _originalRelativeValues;

	protected AnimationCurve _originalShakeStereoPan;

	protected float _originalRemapStereoPanZero;

	protected float _originalRemapStereoPanOne;

	protected override void Initialization()
	{
		base.Initialization();
		_targetAudioSource = base.gameObject.GetComponent<AudioSource>();
	}

	protected virtual void Reset()
	{
		ShakeDuration = 2f;
	}

	protected override void Shake()
	{
		float panStereo = ShakeFloat(ShakeStereoPan, RemapStereoPanZero, RemapStereoPanOne, RelativeStereoPan, _initialStereoPan);
		_targetAudioSource.panStereo = panStereo;
	}

	protected override void GrabInitialValues()
	{
		_initialStereoPan = _targetAudioSource.panStereo;
	}

	public virtual void OnMMAudioSourceStereoPanShakeEvent(AnimationCurve stereoPanCurve, float duration, float remapMin, float remapMax, bool relativeStereoPan = false, float feedbacksIntensity = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
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
			_originalShakeStereoPan = ShakeStereoPan;
			_originalRemapStereoPanZero = RemapStereoPanZero;
			_originalRemapStereoPanOne = RemapStereoPanOne;
			_originalRelativeValues = RelativeStereoPan;
		}
		TimescaleMode = timescaleMode;
		ShakeDuration = duration;
		ShakeStereoPan = stereoPanCurve;
		RemapStereoPanZero = remapMin * feedbacksIntensity;
		RemapStereoPanOne = remapMax * feedbacksIntensity;
		RelativeStereoPan = relativeStereoPan;
		ForwardDirection = forwardDirection;
		Play();
	}

	protected override void ResetTargetValues()
	{
		base.ResetTargetValues();
		_targetAudioSource.panStereo = _initialStereoPan;
	}

	protected override void ResetShakerValues()
	{
		base.ResetShakerValues();
		ShakeDuration = _originalShakeDuration;
		ShakeStereoPan = _originalShakeStereoPan;
		RemapStereoPanZero = _originalRemapStereoPanZero;
		RemapStereoPanOne = _originalRemapStereoPanOne;
		RelativeStereoPan = _originalRelativeValues;
	}

	public override void StartListening()
	{
		base.StartListening();
		MMAudioSourceStereoPanShakeEvent.Register(OnMMAudioSourceStereoPanShakeEvent);
	}

	public override void StopListening()
	{
		base.StopListening();
		MMAudioSourceStereoPanShakeEvent.Unregister(OnMMAudioSourceStereoPanShakeEvent);
	}
}

using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("More Mountains/Feedbacks/Shakers/Audio/MMAudioSourcePitchShaker")]
[RequireComponent(typeof(AudioSource))]
public class MMAudioSourcePitchShaker : MMShaker
{
	[Header("Pitch")]
	[Tooltip("whether or not to add to the initial value")]
	public bool RelativePitch;

	[Tooltip("the curve used to animate the intensity value on")]
	public AnimationCurve ShakePitch = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.5f, 0f), new Keyframe(1f, 1f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-3f, 3f)]
	public float RemapPitchZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-3f, 3f)]
	public float RemapPitchOne = 1f;

	protected AudioSource _targetAudioSource;

	protected float _initialPitch;

	protected float _originalShakeDuration;

	protected bool _originalRelativePitch;

	protected AnimationCurve _originalShakePitch;

	protected float _originalRemapPitchZero;

	protected float _originalRemapPitchOne;

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
		float pitch = ShakeFloat(ShakePitch, RemapPitchZero, RemapPitchOne, RelativePitch, _initialPitch);
		_targetAudioSource.pitch = pitch;
	}

	protected override void GrabInitialValues()
	{
		_initialPitch = _targetAudioSource.pitch;
	}

	public virtual void OnMMAudioSourcePitchShakeEvent(AnimationCurve pitchCurve, float duration, float remapMin, float remapMax, bool relativePitch = false, float feedbacksIntensity = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
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
			_originalShakePitch = ShakePitch;
			_originalRemapPitchZero = RemapPitchZero;
			_originalRemapPitchOne = RemapPitchOne;
			_originalRelativePitch = RelativePitch;
		}
		TimescaleMode = timescaleMode;
		ShakeDuration = duration;
		ShakePitch = pitchCurve;
		RemapPitchZero = remapMin * feedbacksIntensity;
		RemapPitchOne = remapMax * feedbacksIntensity;
		RelativePitch = relativePitch;
		ForwardDirection = forwardDirection;
		Play();
	}

	protected override void ResetTargetValues()
	{
		base.ResetTargetValues();
		_targetAudioSource.pitch = _initialPitch;
	}

	protected override void ResetShakerValues()
	{
		base.ResetShakerValues();
		ShakeDuration = _originalShakeDuration;
		ShakePitch = _originalShakePitch;
		RemapPitchZero = _originalRemapPitchZero;
		RemapPitchOne = _originalRemapPitchOne;
		RelativePitch = _originalRelativePitch;
	}

	public override void StartListening()
	{
		base.StartListening();
		MMAudioSourcePitchShakeEvent.Register(OnMMAudioSourcePitchShakeEvent);
	}

	public override void StopListening()
	{
		base.StopListening();
		MMAudioSourcePitchShakeEvent.Unregister(OnMMAudioSourcePitchShakeEvent);
	}
}

using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MoreMountains.FeedbacksForThirdParty;

[RequireComponent(typeof(Volume))]
[AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMChannelMixerShaker_URP")]
public class MMChannelMixerShaker_URP : MMShaker
{
	public bool RelativeValues = true;

	[Header("Red")]
	[Tooltip("the curve used to animate the red value on")]
	public AnimationCurve ShakeRed = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-200f, 200f)]
	public float RemapRedZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-200f, 200f)]
	public float RemapRedOne = 200f;

	[Header("Green")]
	[Tooltip("the curve used to animate the green value on")]
	public AnimationCurve ShakeGreen = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-200f, 200f)]
	public float RemapGreenZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-200f, 200f)]
	public float RemapGreenOne = 200f;

	[Header("Blue")]
	[Tooltip("the curve used to animate the blue value on")]
	public AnimationCurve ShakeBlue = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-200f, 200f)]
	public float RemapBlueZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-200f, 200f)]
	public float RemapBlueOne = 200f;

	protected Volume _volume;

	protected ChannelMixer _channelMixer;

	protected float _initialRed;

	protected float _initialGreen;

	protected float _initialBlue;

	protected float _initialContrast;

	protected Color _initialColorFilterColor;

	protected float _originalShakeDuration;

	protected bool _originalRelativeValues;

	protected AnimationCurve _originalShakeRed;

	protected float _originalRemapRedZero;

	protected float _originalRemapRedOne;

	protected AnimationCurve _originalShakeGreen;

	protected float _originalRemapGreenZero;

	protected float _originalRemapGreenOne;

	protected AnimationCurve _originalShakeBlue;

	protected float _originalRemapBlueZero;

	protected float _originalRemapBlueOne;

	protected override void Initialization()
	{
		base.Initialization();
		_volume = base.gameObject.GetComponent<Volume>();
		_volume.profile.TryGet<ChannelMixer>(out _channelMixer);
	}

	protected virtual void Reset()
	{
		ShakeDuration = 0.8f;
	}

	protected override void Shake()
	{
		float x = ShakeFloat(ShakeRed, RemapRedZero, RemapRedOne, RelativeValues, _initialRed);
		_channelMixer.redOutRedIn.Override(x);
		float x2 = ShakeFloat(ShakeGreen, RemapGreenZero, RemapGreenOne, RelativeValues, _initialGreen);
		_channelMixer.greenOutGreenIn.Override(x2);
		float x3 = ShakeFloat(ShakeBlue, RemapBlueZero, RemapBlueOne, RelativeValues, _initialBlue);
		_channelMixer.blueOutBlueIn.Override(x3);
	}

	protected override void GrabInitialValues()
	{
		_initialRed = _channelMixer.redOutRedIn.value;
		_initialGreen = _channelMixer.greenOutGreenIn.value;
		_initialBlue = _channelMixer.blueOutBlueIn.value;
	}

	public virtual void OnMMChannelMixerShakeEvent(AnimationCurve shakeRed, float remapRedZero, float remapRedOne, AnimationCurve shakeGreen, float remapGreenZero, float remapGreenOne, AnimationCurve shakeBlue, float remapBlueZero, float remapBlueOne, float duration, bool relativeValues = false, float attenuation = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
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
			_originalRelativeValues = RelativeValues;
			_originalShakeRed = ShakeRed;
			_originalRemapRedZero = RemapRedZero;
			_originalRemapRedOne = RemapRedOne;
			_originalShakeGreen = ShakeGreen;
			_originalRemapGreenZero = RemapGreenZero;
			_originalRemapGreenOne = RemapGreenOne;
			_originalShakeBlue = ShakeBlue;
			_originalRemapBlueZero = RemapBlueZero;
			_originalRemapBlueOne = RemapBlueOne;
		}
		TimescaleMode = timescaleMode;
		ShakeDuration = duration;
		RelativeValues = relativeValues;
		ShakeRed = shakeRed;
		RemapRedZero = remapRedZero;
		RemapRedOne = remapRedOne;
		ShakeGreen = shakeGreen;
		RemapGreenZero = remapGreenZero;
		RemapGreenOne = remapGreenOne;
		ShakeBlue = shakeBlue;
		RemapBlueZero = remapBlueZero;
		RemapBlueOne = remapBlueOne;
		ForwardDirection = forwardDirection;
		Play();
	}

	protected override void ResetTargetValues()
	{
		base.ResetTargetValues();
		_channelMixer.redOutRedIn.Override(_initialRed);
		_channelMixer.greenOutGreenIn.Override(_initialGreen);
		_channelMixer.blueOutBlueIn.Override(_initialBlue);
	}

	protected override void ResetShakerValues()
	{
		base.ResetShakerValues();
		ShakeDuration = _originalShakeDuration;
		RelativeValues = _originalRelativeValues;
		ShakeRed = _originalShakeRed;
		RemapRedZero = _originalRemapRedZero;
		RemapRedOne = _originalRemapRedOne;
		ShakeGreen = _originalShakeGreen;
		RemapGreenZero = _originalRemapGreenZero;
		RemapGreenOne = _originalRemapGreenOne;
		ShakeBlue = _originalShakeBlue;
		RemapBlueZero = _originalRemapBlueZero;
		RemapBlueOne = _originalRemapBlueOne;
	}

	public override void StartListening()
	{
		base.StartListening();
		MMChannelMixerShakeEvent_URP.Register(OnMMChannelMixerShakeEvent);
	}

	public override void StopListening()
	{
		base.StopListening();
		MMChannelMixerShakeEvent_URP.Unregister(OnMMChannelMixerShakeEvent);
	}
}

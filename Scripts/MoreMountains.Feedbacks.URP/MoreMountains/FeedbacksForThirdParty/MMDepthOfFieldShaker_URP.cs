using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MoreMountains.FeedbacksForThirdParty;

[RequireComponent(typeof(Volume))]
[AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMDepthOfFieldShaker_URP")]
public class MMDepthOfFieldShaker_URP : MMShaker
{
	public bool RelativeValues = true;

	[Header("Focus Distance")]
	[Tooltip("the curve used to animate the focus distance value on")]
	public AnimationCurve ShakeFocusDistance = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	public float RemapFocusDistanceZero;

	[Tooltip("the value to remap the curve's 1 to")]
	public float RemapFocusDistanceOne = 3f;

	[Header("Aperture")]
	[Tooltip("the curve used to animate the aperture value on")]
	public AnimationCurve ShakeAperture = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Range(0.1f, 32f)]
	[Tooltip("the value to remap the curve's 0 to")]
	public float RemapApertureZero;

	[Range(0.1f, 32f)]
	[Tooltip("the value to remap the curve's 1 to")]
	public float RemapApertureOne;

	[Header("Focal Length")]
	[Tooltip("the curve used to animate the focal length value on")]
	public AnimationCurve ShakeFocalLength = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(0f, 300f)]
	public float RemapFocalLengthZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(0f, 300f)]
	public float RemapFocalLengthOne;

	protected Volume _volume;

	protected DepthOfField _depthOfField;

	protected float _initialFocusDistance;

	protected float _initialAperture;

	protected float _initialFocalLength;

	protected float _originalShakeDuration;

	protected bool _originalRelativeValues;

	protected AnimationCurve _originalShakeFocusDistance;

	protected float _originalRemapFocusDistanceZero;

	protected float _originalRemapFocusDistanceOne;

	protected AnimationCurve _originalShakeAperture;

	protected float _originalRemapApertureZero;

	protected float _originalRemapApertureOne;

	protected AnimationCurve _originalShakeFocalLength;

	protected float _originalRemapFocalLengthZero;

	protected float _originalRemapFocalLengthOne;

	protected override void Initialization()
	{
		base.Initialization();
		_volume = base.gameObject.GetComponent<Volume>();
		_volume.profile.TryGet<DepthOfField>(out _depthOfField);
	}

	protected override void Shake()
	{
		float x = ShakeFloat(ShakeFocusDistance, RemapFocusDistanceZero, RemapFocusDistanceOne, RelativeValues, _initialFocusDistance);
		_depthOfField.focusDistance.Override(x);
		float x2 = ShakeFloat(ShakeAperture, RemapApertureZero, RemapApertureOne, RelativeValues, _initialAperture);
		_depthOfField.aperture.Override(x2);
		float x3 = ShakeFloat(ShakeFocalLength, RemapFocalLengthZero, RemapFocalLengthOne, RelativeValues, _initialFocalLength);
		_depthOfField.focalLength.Override(x3);
	}

	protected virtual void Reset()
	{
		ShakeDuration = 2f;
	}

	protected override void GrabInitialValues()
	{
		_initialFocusDistance = _depthOfField.focusDistance.value;
		_initialAperture = _depthOfField.aperture.value;
		_initialFocalLength = _depthOfField.focalLength.value;
	}

	public virtual void OnDepthOfFieldShakeEvent(AnimationCurve focusDistance, float duration, float remapFocusDistanceMin, float remapFocusDistanceMax, AnimationCurve aperture, float remapApertureMin, float remapApertureMax, AnimationCurve focalLength, float remapFocalLengthMin, float remapFocalLengthMax, bool relativeValues = false, float attenuation = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
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
			_originalShakeFocusDistance = ShakeFocusDistance;
			_originalRemapFocusDistanceZero = RemapFocusDistanceZero;
			_originalRemapFocusDistanceOne = RemapFocusDistanceOne;
			_originalShakeAperture = ShakeAperture;
			_originalRemapApertureZero = RemapApertureZero;
			_originalRemapApertureOne = RemapApertureOne;
			_originalShakeFocalLength = ShakeFocalLength;
			_originalRemapFocalLengthZero = RemapFocalLengthZero;
			_originalRemapFocalLengthOne = RemapFocalLengthOne;
		}
		TimescaleMode = timescaleMode;
		ShakeDuration = duration;
		RelativeValues = relativeValues;
		ShakeFocusDistance = focusDistance;
		RemapFocusDistanceZero = remapFocusDistanceMin;
		RemapFocusDistanceOne = remapFocusDistanceMax;
		ShakeAperture = aperture;
		RemapApertureZero = remapApertureMin;
		RemapApertureOne = remapApertureMax;
		ShakeFocalLength = focalLength;
		RemapFocalLengthZero = remapFocalLengthMin;
		RemapFocalLengthOne = remapFocalLengthMax;
		ForwardDirection = forwardDirection;
		Play();
	}

	protected override void ResetTargetValues()
	{
		base.ResetTargetValues();
		_depthOfField.focusDistance.Override(_initialFocusDistance);
		_depthOfField.aperture.Override(_initialAperture);
		_depthOfField.focalLength.Override(_initialFocalLength);
	}

	protected override void ResetShakerValues()
	{
		base.ResetShakerValues();
		ShakeDuration = _originalShakeDuration;
		RelativeValues = _originalRelativeValues;
		ShakeFocusDistance = _originalShakeFocusDistance;
		RemapFocusDistanceZero = _originalRemapFocusDistanceZero;
		RemapFocusDistanceOne = _originalRemapFocusDistanceOne;
		ShakeAperture = _originalShakeAperture;
		RemapApertureZero = _originalRemapApertureZero;
		RemapApertureOne = _originalRemapApertureOne;
		ShakeFocalLength = _originalShakeFocalLength;
		RemapFocalLengthZero = _originalRemapFocalLengthZero;
		RemapFocalLengthOne = _originalRemapFocalLengthOne;
	}

	public override void StartListening()
	{
		base.StartListening();
		MMDepthOfFieldShakeEvent_URP.Register(OnDepthOfFieldShakeEvent);
	}

	public override void StopListening()
	{
		base.StopListening();
		MMDepthOfFieldShakeEvent_URP.Unregister(OnDepthOfFieldShakeEvent);
	}
}

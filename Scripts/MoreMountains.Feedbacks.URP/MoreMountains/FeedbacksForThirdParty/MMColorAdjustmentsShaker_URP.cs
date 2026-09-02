using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MoreMountains.FeedbacksForThirdParty;

[RequireComponent(typeof(Volume))]
[AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMColorAdjustmentsShaker_URP")]
public class MMColorAdjustmentsShaker_URP : MMShaker
{
	public enum ColorFilterModes
	{
		None = 0,
		Gradient = 1,
		Interpolate = 2
	}

	public bool RelativeValues = true;

	[Header("Post Exposure")]
	[Tooltip("the curve used to animate the focus distance value on")]
	public AnimationCurve ShakePostExposure = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	public float RemapPostExposureZero;

	[Tooltip("the value to remap the curve's 1 to")]
	public float RemapPostExposureOne = 1f;

	[Header("Hue Shift")]
	[Tooltip("the curve used to animate the aperture value on")]
	public AnimationCurve ShakeHueShift = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Range(-180f, 180f)]
	[Tooltip("the value to remap the curve's 0 to")]
	public float RemapHueShiftZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-180f, 180f)]
	public float RemapHueShiftOne = 180f;

	[Header("Saturation")]
	[Tooltip("the curve used to animate the focal length value on")]
	public AnimationCurve ShakeSaturation = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-100f, 100f)]
	public float RemapSaturationZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-100f, 100f)]
	public float RemapSaturationOne = 100f;

	[Header("Contrast")]
	[Tooltip("the curve used to animate the focal length value on")]
	public AnimationCurve ShakeContrast = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-100f, 100f)]
	public float RemapContrastZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-100f, 100f)]
	public float RemapContrastOne = 100f;

	[Header("Color Filter")]
	[Tooltip("the color filter mode to work with (none, over a gradient, or interpolate to a destination color")]
	public ColorFilterModes ColorFilterMode;

	[Tooltip("the gradient over which to modify the color filter")]
	[MMFEnumCondition("ColorFilterMode", new int[] { 1 })]
	[GradientUsage(true)]
	public Gradient ColorFilterGradient;

	[Tooltip("the destination color to match when in Interpolate mode")]
	[MMFEnumCondition("ColorFilterMode", new int[] { 2 })]
	public Color ColorFilterDestination = Color.yellow;

	[Tooltip("the curve over which to interpolate the color filter")]
	[MMFEnumCondition("ColorFilterMode", new int[] { 2 })]
	public AnimationCurve ColorFilterCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	protected Volume _volume;

	protected ColorAdjustments _colorAdjustments;

	protected float _initialPostExposure;

	protected float _initialHueShift;

	protected float _initialSaturation;

	protected float _initialContrast;

	protected Color _initialColorFilterColor;

	protected float _originalShakeDuration;

	protected bool _originalRelativeValues;

	protected AnimationCurve _originalShakePostExposure;

	protected float _originalRemapPostExposureZero;

	protected float _originalRemapPostExposureOne;

	protected AnimationCurve _originalShakeHueShift;

	protected float _originalRemapHueShiftZero;

	protected float _originalRemapHueShiftOne;

	protected AnimationCurve _originalShakeSaturation;

	protected float _originalRemapSaturationZero;

	protected float _originalRemapSaturationOne;

	protected AnimationCurve _originalShakeContrast;

	protected float _originalRemapContrastZero;

	protected float _originalRemapContrastOne;

	protected ColorFilterModes _originalColorFilterMode;

	protected Gradient _originalColorFilterGradient;

	protected Color _originalColorFilterDestination;

	protected AnimationCurve _originalColorFilterCurve;

	protected override void Initialization()
	{
		base.Initialization();
		_volume = base.gameObject.GetComponent<Volume>();
		_volume.profile.TryGet<ColorAdjustments>(out _colorAdjustments);
	}

	protected virtual void Reset()
	{
		ShakeDuration = 0.8f;
	}

	protected override void Shake()
	{
		float x = ShakeFloat(ShakePostExposure, RemapPostExposureZero, RemapPostExposureOne, RelativeValues, _initialPostExposure);
		_colorAdjustments.postExposure.Override(x);
		float x2 = ShakeFloat(ShakeHueShift, RemapHueShiftZero, RemapHueShiftOne, RelativeValues, _initialHueShift);
		_colorAdjustments.hueShift.Override(x2);
		float x3 = ShakeFloat(ShakeSaturation, RemapSaturationZero, RemapSaturationOne, RelativeValues, _initialSaturation);
		_colorAdjustments.saturation.Override(x3);
		float x4 = ShakeFloat(ShakeContrast, RemapContrastZero, RemapContrastOne, RelativeValues, _initialContrast);
		_colorAdjustments.contrast.Override(x4);
		_remappedTimeSinceStart = MMFeedbacksHelpers.Remap(Time.time - _shakeStartedTimestamp, 0f, ShakeDuration, 0f, 1f);
		if (ColorFilterMode == ColorFilterModes.Gradient)
		{
			_colorAdjustments.colorFilter.Override(ColorFilterGradient.Evaluate(_remappedTimeSinceStart));
		}
		else if (ColorFilterMode == ColorFilterModes.Interpolate)
		{
			float t = ColorFilterCurve.Evaluate(_remappedTimeSinceStart);
			_colorAdjustments.colorFilter.Override(Color.LerpUnclamped(_initialColorFilterColor, ColorFilterDestination, t));
		}
	}

	protected override void GrabInitialValues()
	{
		_initialPostExposure = _colorAdjustments.postExposure.value;
		_initialHueShift = _colorAdjustments.hueShift.value;
		_initialSaturation = _colorAdjustments.saturation.value;
		_initialContrast = _colorAdjustments.contrast.value;
		_initialColorFilterColor = _colorAdjustments.colorFilter.value;
	}

	public virtual void OnMMColorGradingShakeEvent(AnimationCurve shakePostExposure, float remapPostExposureZero, float remapPostExposureOne, AnimationCurve shakeHueShift, float remapHueShiftZero, float remapHueShiftOne, AnimationCurve shakeSaturation, float remapSaturationZero, float remapSaturationOne, AnimationCurve shakeContrast, float remapContrastZero, float remapContrastOne, ColorFilterModes colorFilterMode, Gradient colorFilterGradient, Color colorFilterDestination, AnimationCurve colorFilterCurve, float duration, bool relativeValues = false, float attenuation = 1f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
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
			_originalShakePostExposure = ShakePostExposure;
			_originalRemapPostExposureZero = RemapPostExposureZero;
			_originalRemapPostExposureOne = RemapPostExposureOne;
			_originalShakeHueShift = ShakeHueShift;
			_originalRemapHueShiftZero = RemapHueShiftZero;
			_originalRemapHueShiftOne = RemapHueShiftOne;
			_originalShakeSaturation = ShakeSaturation;
			_originalRemapSaturationZero = RemapSaturationZero;
			_originalRemapSaturationOne = RemapSaturationOne;
			_originalShakeContrast = ShakeContrast;
			_originalRemapContrastZero = RemapContrastZero;
			_originalRemapContrastOne = RemapContrastOne;
			_originalColorFilterMode = ColorFilterMode;
			_originalColorFilterGradient = ColorFilterGradient;
			_originalColorFilterDestination = ColorFilterDestination;
			_originalColorFilterCurve = ColorFilterCurve;
		}
		TimescaleMode = timescaleMode;
		ShakeDuration = duration;
		RelativeValues = relativeValues;
		ShakePostExposure = shakePostExposure;
		RemapPostExposureZero = remapPostExposureZero;
		RemapPostExposureOne = remapPostExposureOne;
		ShakeHueShift = shakeHueShift;
		RemapHueShiftZero = remapHueShiftZero;
		RemapHueShiftOne = remapHueShiftOne;
		ShakeSaturation = shakeSaturation;
		RemapSaturationZero = remapSaturationZero;
		RemapSaturationOne = remapSaturationOne;
		ShakeContrast = shakeContrast;
		RemapContrastZero = remapContrastZero;
		RemapContrastOne = remapContrastOne;
		ColorFilterMode = colorFilterMode;
		ColorFilterGradient = colorFilterGradient;
		ColorFilterDestination = colorFilterDestination;
		ColorFilterCurve = colorFilterCurve;
		ForwardDirection = forwardDirection;
		Play();
	}

	protected override void ResetTargetValues()
	{
		base.ResetTargetValues();
		_colorAdjustments.postExposure.Override(_initialPostExposure);
		_colorAdjustments.hueShift.Override(_initialHueShift);
		_colorAdjustments.saturation.Override(_initialSaturation);
		_colorAdjustments.contrast.Override(_initialContrast);
		_colorAdjustments.colorFilter.Override(_initialColorFilterColor);
	}

	protected override void ResetShakerValues()
	{
		base.ResetShakerValues();
		ShakeDuration = _originalShakeDuration;
		RelativeValues = _originalRelativeValues;
		ShakePostExposure = _originalShakePostExposure;
		RemapPostExposureZero = _originalRemapPostExposureZero;
		RemapPostExposureOne = _originalRemapPostExposureOne;
		ShakeHueShift = _originalShakeHueShift;
		RemapHueShiftZero = _originalRemapHueShiftZero;
		RemapHueShiftOne = _originalRemapHueShiftOne;
		ShakeSaturation = _originalShakeSaturation;
		RemapSaturationZero = _originalRemapSaturationZero;
		RemapSaturationOne = _originalRemapSaturationOne;
		ShakeContrast = _originalShakeContrast;
		RemapContrastZero = _originalRemapContrastZero;
		RemapContrastOne = _originalRemapContrastOne;
		ColorFilterMode = _originalColorFilterMode;
		ColorFilterGradient = _originalColorFilterGradient;
		ColorFilterDestination = _originalColorFilterDestination;
		ColorFilterCurve = _originalColorFilterCurve;
	}

	public override void StartListening()
	{
		base.StartListening();
		MMColorAdjustmentsShakeEvent_URP.Register(OnMMColorGradingShakeEvent);
	}

	public override void StopListening()
	{
		base.StopListening();
		MMColorAdjustmentsShakeEvent_URP.Unregister(OnMMColorGradingShakeEvent);
	}
}

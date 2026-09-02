using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("")]
[FeedbackPath("Haptics/Haptics DEPRECATED!")]
[FeedbackHelp("This feedback has been deprecated, and is just here to avoid errors in case you were to update from an old version. Use the new haptic feedbacks instead.")]
public class MMFeedbackHaptics : MMFeedback
{
	public enum HapticTypes
	{
		Selection = 0,
		Success = 1,
		Warning = 2,
		Failure = 3,
		LightImpact = 4,
		MediumImpact = 5,
		HeavyImpact = 6,
		RigidImpact = 7,
		SoftImpact = 8,
		None = 9
	}

	public enum HapticMethods
	{
		NativePreset = 0,
		Transient = 1,
		Continuous = 2,
		AdvancedPattern = 3,
		Stop = 4,
		AdvancedTransient = 5,
		AdvancedContinuous = 6
	}

	public enum Timescales
	{
		ScaledTime = 0,
		UnscaledTime = 1
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("Haptics")]
	[Tooltip("the method to use when triggering this haptic feedback")]
	public HapticMethods HapticMethod;

	[Tooltip("the type of native preset to use")]
	[MMFEnumCondition("HapticMethod", new int[] { 0 })]
	public HapticTypes HapticType = HapticTypes.None;

	[Tooltip("the intensity of the transient haptic")]
	[MMFEnumCondition("HapticMethod", new int[] { 1 })]
	public float TransientIntensity = 1f;

	[Tooltip("the sharpness of the transient haptic")]
	[MMFEnumCondition("HapticMethod", new int[] { 1 })]
	public float TransientSharpness = 1f;

	[Tooltip("whether or not to vibrate on iOS when in AdvancedTransient mode")]
	[MMFEnumCondition("HapticMethod", new int[] { 5 })]
	public bool ATVibrateIOS = true;

	[Tooltip("the intensity on iOS when in AdvancedTransient mode")]
	[MMFEnumCondition("HapticMethod", new int[] { 5 })]
	public float ATIOSIntensity = 1f;

	[Tooltip("the sharpness on iOS when in AdvancedTransient mode")]
	[MMFEnumCondition("HapticMethod", new int[] { 5 })]
	public float ATIOSSharpness = 1f;

	[Tooltip("whether or not to vibrate on android when in AdvancedTransient mode")]
	[MMFEnumCondition("HapticMethod", new int[] { 5 })]
	public bool ATVibrateAndroid = true;

	[Tooltip("whether or not to vibrate on android if no support for advanced vibrations when in AdvancedTransient mode")]
	[MMFEnumCondition("HapticMethod", new int[] { 5 })]
	public bool ATVibrateAndroidIfNoSupport;

	[Tooltip("the intensity on android when in AdvancedTransient mode")]
	[MMFEnumCondition("HapticMethod", new int[] { 5 })]
	public float ATAndroidIntensity = 1f;

	[Tooltip("the sharpness on android when in AdvancedTransient mode")]
	[MMFEnumCondition("HapticMethod", new int[] { 5 })]
	public float ATAndroidSharpness = 1f;

	[Tooltip("whether or not to rumble when in AdvancedTransient mode")]
	[MMFEnumCondition("HapticMethod", new int[] { 5 })]
	public bool ATRumble = true;

	[Tooltip("the rumble intensity when in AdvancedTransient mode")]
	[MMFEnumCondition("HapticMethod", new int[] { 5 })]
	public float ATRumbleIntensity = 1f;

	[Tooltip("the rumble sharpness when in AdvancedTransient mode")]
	[MMFEnumCondition("HapticMethod", new int[] { 5 })]
	public float ATRumbleSharpness = 1f;

	[Tooltip("the controllerID when in AdvancedTransient mode")]
	[MMFEnumCondition("HapticMethod", new int[] { 5 })]
	public int ATRumbleControllerID = -1;

	[Tooltip("the intensity that should be used to initialize the continuous haptic")]
	[MMFEnumCondition("HapticMethod", new int[] { 2 })]
	public float InitialContinuousIntensity = 1f;

	[Tooltip("the curve used to tween the continuous intensity")]
	[MMFEnumCondition("HapticMethod", new int[] { 2 })]
	public AnimationCurve ContinuousIntensityCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

	[Tooltip("the sharpness that should be used to initialize the continuous haptic")]
	[MMFEnumCondition("HapticMethod", new int[] { 2 })]
	public float InitialContinuousSharpness = 1f;

	[Tooltip("the curve used to tween the continuous sharpness")]
	[MMFEnumCondition("HapticMethod", new int[] { 2 })]
	public AnimationCurve ContinuousSharpnessCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

	[Tooltip("the duration of the continuous haptic")]
	[MMFEnumCondition("HapticMethod", new int[] { 2 })]
	public float ContinuousDuration = 1f;

	[Tooltip("whether or not to trigger advanced patterns on iOS")]
	[MMFEnumCondition("HapticMethod", new int[] { 3 })]
	public bool APVibrateIOS = true;

	[Tooltip("the AHAP file to use to trigger a pattern on iOS")]
	[MMFEnumCondition("HapticMethod", new int[] { 3 })]
	public TextAsset AHAPFileForIOS;

	[Tooltip("whether or not to trigger advanced patterns on Android")]
	[MMFEnumCondition("HapticMethod", new int[] { 3 })]
	public bool APVibrateAndroid = true;

	[Tooltip("whether or not to vibrate if there's no haptics support")]
	[MMFEnumCondition("HapticMethod", new int[] { 3 })]
	public bool APVibrateAndroidIfNoSupport;

	[Tooltip("whether or not to trigger advanced patterns on rumble")]
	[MMFEnumCondition("HapticMethod", new int[] { 3 })]
	public bool APRumble = true;

	[Tooltip("the amount of times this should repeat on Android (-1 : zero, 0 : infinite, 1 : one time, 2 : twice, etc)")]
	[MMFEnumCondition("HapticMethod", new int[] { 3 })]
	public int AndroidRepeat = -1;

	public int RumbleRepeat = -1;

	[Tooltip("a haptic type to play on older iOS APIs (prior to iOS 13)")]
	[MMFEnumCondition("HapticMethod", new int[] { 3 })]
	public HapticTypes OldIOSFallback;

	[Tooltip("whether to run this on scaled or unscaled time")]
	[MMFEnumCondition("HapticMethod", new int[] { 3 })]
	public Timescales Timescale = Timescales.UnscaledTime;

	[Header("Rumble")]
	[Tooltip("whether or not this feedback should trigger a rumble on gamepad")]
	public bool AllowRumble = true;

	[Tooltip("the ID of the controller to rumble (-1 : auto/current, 0 : first controller, 1 : second controller, etc)")]
	public int ControllerID = -1;

	[Header("Deprecated Feedback")]
	public bool OutputDeprecationWarning = true;

	protected static bool _continuousPlaying = false;

	protected static float _continuousStartedAt = 0f;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && OutputDeprecationWarning)
		{
			Debug.LogWarning(base.name + " : the haptic feedback on this object is using the old version of Nice Vibrations, and won't work anymore. Replace it with any of the new haptic feedbacks.");
		}
	}
}

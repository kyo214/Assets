using System.Collections;
using Lofelt.NiceVibrations;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("")]
[FeedbackPath("Haptics/Haptic Continuous")]
[FeedbackHelp("Add this feedback to play a continuous haptic of the specified amplitude and frequency over a certain duration. This feedback will also let you randomize these, and modulate them over time.")]
public class MMF_NVContinuous : MMF_Feedback
{
	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Haptic Amplitude", true, 31, false, false)]
	[Tooltip("the minimum amplitude at which this clip should play (amplitude will be randomized between MinAmplitude and MaxAmplitude)")]
	[Range(0f, 1f)]
	public float MinAmplitude = 1f;

	[Tooltip("the maximum amplitude at which this clip should play (amplitude will be randomized between MinAmplitude and MaxAmplitude)")]
	[Range(0f, 1f)]
	public float MaxAmplitude = 1f;

	[MMFInspectorGroup("Haptic Frequency", true, 32, false, false)]
	[Tooltip("the minimum frequency at which this clip should play (frequency will be randomized between MinFrequency and MaxFrequency)")]
	[Range(0f, 1f)]
	public float MinFrequency = 1f;

	[Tooltip("the maximum frequency at which this clip should play (frequency will be randomized between MinFrequency and MaxFrequency)")]
	[Range(0f, 1f)]
	public float MaxFrequency = 1f;

	[MMFInspectorGroup("Duration", true, 33, false, false)]
	[Tooltip("the minimum duration at which this clip should play (duration will be randomized between MinDuration and MaxDuration)")]
	public float MinDuration = 1f;

	[Tooltip("the maximum duration at which this clip should play (duration will be randomized between MinDuration and MaxDuration)")]
	public float MaxDuration = 1f;

	[MMFInspectorGroup("Real-time Modulation", true, 34, false, false)]
	[Tooltip("whether or not to modulate the haptic signal at runtime")]
	public bool UseRealTimeModulation;

	[Tooltip("if UseRealTimeModulation:true, the curve along which to modulate amplitude for this continuous haptic, over its total duration")]
	[MMFCondition("UseRealTimeModulation", true)]
	public AnimationCurve AmplitudeMultiplication = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("if UseRealTimeModulation:true, the curve along which to modulate frequency for this continuous haptic, over its total duration")]
	[MMFCondition("UseRealTimeModulation", true)]
	public AnimationCurve ShiftFrequency = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[MMFInspectorGroup("Settings", true, 16, false, false)]
	[Tooltip("a set of settings you can tweak to specify how and when exactly this haptic should play")]
	public MMFeedbackNVSettings HapticSettings;

	protected Coroutine _coroutine;

	protected float _duration;

	public override float FeedbackDuration
	{
		get
		{
			return ApplyTimeMultiplier(_duration);
		}
		set
		{
			_duration = value;
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && HapticSettings.CanPlay())
		{
			float amplitude = Random.Range(MinAmplitude, MaxAmplitude);
			float frequency = Random.Range(MinFrequency, MaxFrequency);
			_duration = Random.Range(MinDuration, MaxDuration);
			HapticSettings.SetGamepad();
			HapticPatterns.PlayConstant(amplitude, frequency, FeedbackDuration);
			if (UseRealTimeModulation)
			{
				_coroutine = Owner.StartCoroutine(RealtimeModulationCo());
			}
		}
	}

	protected virtual IEnumerator RealtimeModulationCo()
	{
		IsPlaying = true;
		float journey = (NormalPlayDirection ? 0f : FeedbackDuration);
		while (journey >= 0f && journey <= FeedbackDuration && FeedbackDuration > 0f)
		{
			float time = MMFeedbacksHelpers.Remap(journey, 0f, FeedbackDuration, 0f, 1f);
			HapticController.clipLevel = AmplitudeMultiplication.Evaluate(time);
			HapticController.clipFrequencyShift = ShiftFrequency.Evaluate(time);
			journey += (NormalPlayDirection ? FeedbackDeltaTime : (0f - FeedbackDeltaTime));
			yield return null;
		}
		HapticController.clipLevel = AmplitudeMultiplication.Evaluate(FinalNormalizedTime);
		HapticController.clipFrequencyShift = ShiftFrequency.Evaluate(FinalNormalizedTime);
		IsPlaying = false;
		_coroutine = null;
		yield return null;
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			IsPlaying = false;
			HapticController.Stop();
			if (Active && _coroutine != null)
			{
				Owner.StopCoroutine(_coroutine);
				_coroutine = null;
			}
		}
	}
}

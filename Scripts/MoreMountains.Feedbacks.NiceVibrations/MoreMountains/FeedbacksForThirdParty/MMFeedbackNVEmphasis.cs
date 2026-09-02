using Lofelt.NiceVibrations;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("")]
[FeedbackPath("Haptics/Haptic Emphasis")]
[FeedbackHelp("Use this feedback to play an Emphasis haptics, short haptic bursts whose amplitude and frequency can be controlled in real time, also called Transients in CoreHaptics/iOS")]
public class MMFeedbackNVEmphasis : MMFeedback
{
	public static bool FeedbackTypeAuthorized = true;

	[Header("Haptic Amplitude")]
	[Tooltip("the minimum amplitude at which this clip should play (amplitude will be randomized between MinAmplitude and MaxAmplitude)")]
	[Range(0f, 1f)]
	public float MinAmplitude = 1f;

	[Tooltip("the maximum amplitude at which this clip should play (amplitude will be randomized between MinAmplitude and MaxAmplitude)")]
	[Range(0f, 1f)]
	public float MaxAmplitude = 1f;

	[Header("Haptic Frequency")]
	[Tooltip("the minimum frequency at which this clip should play (frequency will be randomized between MinFrequency and MaxFrequency)")]
	[Range(0f, 1f)]
	public float MinFrequency = 1f;

	[Tooltip("the maximum frequency at which this clip should play (frequency will be randomized between MinFrequency and MaxFrequency)")]
	[Range(0f, 1f)]
	public float MaxFrequency = 1f;

	[Header("Settings")]
	[Tooltip("a set of settings you can tweak to specify how and when exactly this haptic should play")]
	public MMFeedbackNVSettings HapticSettings;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && HapticSettings.CanPlay())
		{
			float amplitude = Random.Range(MinAmplitude, MaxAmplitude);
			float frequency = Random.Range(MinFrequency, MaxFrequency);
			HapticSettings.SetGamepad();
			HapticPatterns.PlayEmphasis(amplitude, frequency);
		}
	}
}

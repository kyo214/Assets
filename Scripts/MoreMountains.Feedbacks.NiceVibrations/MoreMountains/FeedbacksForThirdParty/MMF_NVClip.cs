using Lofelt.NiceVibrations;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("")]
[FeedbackPath("Haptics/Haptic Clip")]
[FeedbackHelp("This feedback will let you play a haptic clip, and randomize its level and frequency.")]
public class MMF_NVClip : MMF_Feedback
{
	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Haptic Clip", true, 13, true, false)]
	[Tooltip("the haptic clip to play with this feedback")]
	public HapticClip Clip;

	[Tooltip("a preset to play should the device you're running your game on doesn't support playing haptic clips")]
	public HapticPatterns.PresetType FallbackPreset = HapticPatterns.PresetType.LightImpact;

	[Tooltip("whether or not this clip should play on a loop, until stopped (won't work on gamepads)")]
	public bool Loop;

	[Tooltip("at what timestamp this clip should start playing")]
	public float SeekTime;

	[MMFInspectorGroup("Level", true, 14, false, false)]
	[Tooltip("the minimum level at which this clip should play (level will be randomized between MinLevel and MaxLevel)")]
	[Range(0f, 5f)]
	public float MinLevel = 1f;

	[Tooltip("the maximum level at which this clip should play (level will be randomized between MinLevel and MaxLevel)")]
	[Range(0f, 5f)]
	public float MaxLevel = 1f;

	[MMFInspectorGroup("Frequency Shift", true, 15, false, false)]
	[Tooltip("the minimum frequency shift at which this clip should play (frequency shift will be randomized between MinFrequencyShift and MaxFrequencyShift)")]
	[Range(-1f, 1f)]
	public float MinFrequencyShift;

	[Tooltip("the maximum frequency shift at which this clip should play (frequency shift will be randomized between MinFrequencyShift and MaxFrequencyShift)")]
	[Range(-1f, 1f)]
	public float MaxFrequencyShift;

	[MMFInspectorGroup("Settings", true, 16, false, false)]
	[Tooltip("a set of settings you can tweak to specify how and when exactly this haptic should play")]
	public MMFeedbackNVSettings HapticSettings;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && HapticSettings.CanPlay() && !(Clip == null))
		{
			HapticController.Load(Clip);
			HapticSettings.SetGamepad();
			HapticController.fallbackPreset = FallbackPreset;
			HapticController.Loop(Loop);
			HapticController.Seek(SeekTime);
			HapticController.clipLevel = Random.Range(MinLevel, MaxLevel);
			HapticController.clipFrequencyShift = Random.Range(MinFrequencyShift, MaxFrequencyShift);
			HapticController.Play();
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			IsPlaying = false;
			HapticController.Stop();
		}
	}
}

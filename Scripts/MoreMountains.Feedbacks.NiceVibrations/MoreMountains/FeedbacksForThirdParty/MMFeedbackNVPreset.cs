using Lofelt.NiceVibrations;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("")]
[FeedbackPath("Haptics/Haptic Preset")]
[FeedbackHelp("Use this feedback to play a preset haptic, limited but super simple predifined haptic patterns")]
public class MMFeedbackNVPreset : MMFeedback
{
	public static bool FeedbackTypeAuthorized = true;

	[Header("Haptic Preset")]
	[Tooltip("the preset to play with this feedback")]
	public HapticPatterns.PresetType Preset = HapticPatterns.PresetType.LightImpact;

	[Header("Settings")]
	[Tooltip("a set of settings you can tweak to specify how and when exactly this haptic should play")]
	public MMFeedbackNVSettings HapticSettings;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && HapticSettings.CanPlay())
		{
			HapticSettings.SetGamepad();
			HapticPatterns.PlayPreset(Preset);
		}
	}
}

using Lofelt.NiceVibrations;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("")]
[FeedbackPath("Haptics/Haptic Control")]
[FeedbackHelp("Add this feedback to interact with haptics at a global level, stopping them all, enabling or disabling them, adjusting their global level or initializing/release the haptic engine.")]
public class MMFeedbackNVControl : MMFeedback
{
	public enum ControlTypes
	{
		Stop = 0,
		EnableHaptics = 1,
		DisableHaptics = 2,
		AdjustHapticsLevel = 3,
		Initialize = 4,
		Release = 5
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("Haptic Control")]
	[Tooltip("the type of control order to trigger when playing this feedback - check Nice Vibrations' documentation for the exact behaviour of these")]
	public ControlTypes ControlType;

	[Tooltip("the output level when in AdjustHapticsLevel mode")]
	[MMFEnumCondition("ControlType", new int[] { 3 })]
	public float OutputLevel = 1f;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			switch (ControlType)
			{
			case ControlTypes.Stop:
				HapticController.Stop();
				break;
			case ControlTypes.EnableHaptics:
				HapticController.hapticsEnabled = true;
				break;
			case ControlTypes.DisableHaptics:
				HapticController.hapticsEnabled = false;
				break;
			case ControlTypes.AdjustHapticsLevel:
				HapticController.outputLevel = OutputLevel;
				break;
			case ControlTypes.Initialize:
				LofeltHaptics.Initialize();
				HapticController.Init();
				break;
			case ControlTypes.Release:
				LofeltHaptics.Release();
				break;
			}
		}
	}
}

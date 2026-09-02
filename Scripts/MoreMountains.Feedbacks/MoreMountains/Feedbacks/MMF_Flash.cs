using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("On play, this feedback will broadcast a MMFlashEvent. If you create a UI image with a MMFlash component on it (see example in the Demo scene), it will intercept that event, and flash (usually you'll want it to take the full size of your screen, but that's not mandatory). In the feedback's inspector, you can define the color of the flash, its duration, alpha, and a FlashID. That FlashID needs to be the same on your feedback and MMFlash for them to work together. This allows you to have multiple MMFlashs in your scene, and flash them separately.")]
[FeedbackPath("Camera/Flash")]
public class MMF_Flash : MMF_Feedback
{
	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Flash", true, 37, false, false)]
	[Tooltip("the color of the flash")]
	public Color FlashColor = Color.white;

	[Tooltip("the flash duration (in seconds)")]
	public float FlashDuration = 0.2f;

	[Tooltip("the alpha of the flash")]
	public float FlashAlpha = 1f;

	[Tooltip("the ID of the flash (usually 0). You can specify on each MMFlash object an ID, allowing you to have different flash images in one scene and call them separately (one for damage, one for health pickups, etc)")]
	public int FlashID;

	public override float FeedbackDuration
	{
		get
		{
			return ApplyTimeMultiplier(FlashDuration);
		}
		set
		{
			FlashDuration = value;
		}
	}

	public override bool HasChannel => true;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			float num = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
			MMFlashEvent.Trigger(FlashColor, FeedbackDuration * num, FlashAlpha, FlashID, Channel, Timing.TimescaleMode);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			MMFlashEvent.Trigger(FlashColor, FeedbackDuration, FlashAlpha, FlashID, Channel, Timing.TimescaleMode, stop: true);
		}
	}
}

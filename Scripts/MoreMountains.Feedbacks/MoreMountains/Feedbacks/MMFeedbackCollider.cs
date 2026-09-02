using System;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will let you enable/disable/toggle a target collider, or change its trigger status")]
[FeedbackPath("GameObject/Collider")]
public class MMFeedbackCollider : MMFeedback
{
	public enum Modes
	{
		Enable = 0,
		Disable = 1,
		ToggleActive = 2,
		Trigger = 3,
		NonTrigger = 4,
		ToggleTrigger = 5
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("Collider")]
	[Tooltip("the collider to act upon")]
	public Collider TargetCollider;

	public Modes Mode = Modes.Disable;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && TargetCollider != null)
		{
			ApplyChanges(Mode);
		}
	}

	protected virtual void ApplyChanges(Modes mode)
	{
		switch (mode)
		{
		case Modes.Enable:
			TargetCollider.enabled = true;
			break;
		case Modes.Disable:
			TargetCollider.enabled = false;
			break;
		case Modes.ToggleActive:
			TargetCollider.enabled = !TargetCollider.enabled;
			break;
		case Modes.Trigger:
			TargetCollider.isTrigger = true;
			break;
		case Modes.NonTrigger:
			TargetCollider.isTrigger = false;
			break;
		case Modes.ToggleTrigger:
			TargetCollider.isTrigger = !TargetCollider.isTrigger;
			break;
		default:
			throw new ArgumentOutOfRangeException("mode", mode, null);
		}
	}
}

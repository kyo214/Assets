using System;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will let you enable/disable/toggle a target collider 2D, or change its trigger status")]
[FeedbackPath("GameObject/Collider2D")]
public class MMF_Collider2D : MMF_Feedback
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

	[MMFInspectorGroup("Collider 2D", true, 12, true, false)]
	[Tooltip("the collider to act upon")]
	public Collider2D TargetCollider2D;

	public Modes Mode = Modes.Disable;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && !(TargetCollider2D == null))
		{
			ApplyChanges(Mode);
		}
	}

	protected virtual void ApplyChanges(Modes mode)
	{
		switch (mode)
		{
		case Modes.Enable:
			TargetCollider2D.enabled = true;
			break;
		case Modes.Disable:
			TargetCollider2D.enabled = false;
			break;
		case Modes.ToggleActive:
			TargetCollider2D.enabled = !TargetCollider2D.enabled;
			break;
		case Modes.Trigger:
			TargetCollider2D.isTrigger = true;
			break;
		case Modes.NonTrigger:
			TargetCollider2D.isTrigger = false;
			break;
		case Modes.ToggleTrigger:
			TargetCollider2D.isTrigger = !TargetCollider2D.isTrigger;
			break;
		default:
			throw new ArgumentOutOfRangeException("mode", mode, null);
		}
	}
}

using UnityEngine;

namespace MoreMountains.Feel;

public class BounceManager : MonoBehaviour
{
	[Header("Cooldown")]
	[Tooltip("a duration, in seconds, between two jumps, during which jumps are prevented")]
	public float CooldownDuration = 1f;

	[Header("Bindings")]
	[Tooltip("the animator of the 'no feedback' version")]
	public Animator NoFeedbackAnimator;

	[Tooltip("the animator of the 'feedback' version")]
	public Animator FeedbackAnimator;

	protected float _lastJumpStartedAt = -100f;

	protected virtual void Update()
	{
		HandleInput();
	}

	protected virtual void HandleInput()
	{
		if (FeelDemosInputHelper.CheckMainActionInputPressedThisFrame())
		{
			Jump();
		}
	}

	protected virtual void Jump()
	{
		if (!(Time.time - _lastJumpStartedAt < CooldownDuration))
		{
			if (FeedbackAnimator.isActiveAndEnabled)
			{
				FeedbackAnimator.SetTrigger("Jump");
			}
			if (NoFeedbackAnimator.isActiveAndEnabled)
			{
				NoFeedbackAnimator.SetTrigger("Jump");
			}
			_lastJumpStartedAt = Time.time;
		}
	}
}

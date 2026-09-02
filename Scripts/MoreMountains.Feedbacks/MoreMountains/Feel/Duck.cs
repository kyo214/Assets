using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.Feel;

public class Duck : MonoBehaviour
{
	[Header("Cooldown")]
	[Tooltip("a duration, in seconds, between two jumps, during which jumps are prevented")]
	public float CooldownDuration = 1f;

	[Header("Feedbacks")]
	[Tooltip("a feedback to call when jumping")]
	public MMFeedbacks JumpFeedback;

	[Tooltip("a feedback to call when landing")]
	public MMFeedbacks LandingFeedback;

	[Tooltip("a feedback to call when trying to jump while in cooldown")]
	public MMFeedbacks DeniedFeedback;

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
		if (Time.time - _lastJumpStartedAt < CooldownDuration)
		{
			DeniedFeedback?.PlayFeedbacks();
			return;
		}
		JumpFeedback?.PlayFeedbacks();
		_lastJumpStartedAt = Time.time;
	}

	public virtual void Land()
	{
		LandingFeedback?.PlayFeedbacks();
	}
}

using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.Feel;

public class Blob : MonoBehaviour
{
	[Header("Cooldown")]
	[Tooltip("a duration, in seconds, between two moves, during which moves are prevented")]
	public float CooldownDuration = 1f;

	[Header("Feedbacks")]
	[Tooltip("a feedback to call when moving")]
	public MMFeedbacks MoveFeedback;

	[Tooltip("a feedback to call when trying to move while in cooldown")]
	public MMFeedbacks DeniedFeedback;

	protected float _lastMoveStartedAt = -100f;

	protected virtual void Update()
	{
		HandleInput();
	}

	protected virtual void HandleInput()
	{
		if (FeelDemosInputHelper.CheckMainActionInputPressedThisFrame())
		{
			Move();
		}
	}

	protected virtual void Move()
	{
		if (Time.time - _lastMoveStartedAt < CooldownDuration)
		{
			DeniedFeedback?.PlayFeedbacks();
			return;
		}
		MoveFeedback?.PlayFeedbacks();
		_lastMoveStartedAt = Time.time;
	}
}

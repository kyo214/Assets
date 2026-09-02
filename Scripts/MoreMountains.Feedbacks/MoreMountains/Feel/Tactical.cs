using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.Feel;

public class Tactical : MonoBehaviour
{
	[Header("Cooldown")]
	[Tooltip("a duration, in seconds, between two shots, during which shots are prevented")]
	public float CooldownDuration = 0.1f;

	[Header("Bindings")]
	[Tooltip("the position of the shot's impact")]
	public Transform ImpactPosition;

	[Header("Feedbacks")]
	[Tooltip("a feedback to call when shooting")]
	public MMFeedbacks ShootFeedback;

	[Tooltip("a feedback to call when shooting stops")]
	public MMFeedbacks ShootStopFeedback;

	[Tooltip("a feedback to call when a reload happens")]
	public MMFeedbacks ReloadFeedback;

	protected float _lastJumpStartedAt = -100f;

	protected int _magazine = 15;

	protected virtual void Update()
	{
		HandleInput();
	}

	protected virtual void HandleInput()
	{
		if (FeelDemosInputHelper.CheckMainActionInputPressed())
		{
			Shoot();
		}
		if (FeelDemosInputHelper.CheckMainActionInputUpThisFrame())
		{
			ShootStop();
		}
	}

	protected virtual void Shoot()
	{
		if (Time.time - _lastJumpStartedAt > CooldownDuration)
		{
			float feedbacksIntensity = Random.Range(20, 200);
			ShootFeedback?.PlayFeedbacks(ImpactPosition.position, feedbacksIntensity);
			_lastJumpStartedAt = Time.time;
			_magazine--;
		}
	}

	protected virtual void ShootStop()
	{
		ShootStopFeedback?.PlayFeedbacks();
		if (_magazine < 0)
		{
			ReloadFeedback?.PlayFeedbacks();
			_magazine = 15;
		}
	}
}

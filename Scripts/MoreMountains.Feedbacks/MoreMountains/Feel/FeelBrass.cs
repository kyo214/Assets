using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feel;

public class FeelBrass : MonoBehaviour
{
	[Header("Bindings")]
	public MMAudioAnalyzer TargetAnalyzer;

	public Light TargetLight;

	[Header("Cooldown")]
	[Tooltip("a duration, in seconds, between two special dance moves, during which moves are prevented")]
	public float CooldownDuration = 0.1f;

	[Header("Feedbacks")]
	[Tooltip("a feedback to play when doing a special dance move")]
	public MMFeedbacks SpecialDanceMoveFeedbacks;

	protected float _lastMoveStartedAt = -100f;

	protected virtual void Update()
	{
		HandleInput();
		ControlLightIntensity();
	}

	protected virtual void HandleInput()
	{
		if (FeelDemosInputHelper.CheckMainActionInputPressedThisFrame())
		{
			SpecialDanceMove();
		}
	}

	protected virtual void ControlLightIntensity()
	{
		TargetLight.intensity = TargetAnalyzer.NormalizedBufferedAmplitude * 5f;
	}

	protected virtual void SpecialDanceMove()
	{
		if (Time.time - _lastMoveStartedAt >= CooldownDuration)
		{
			SpecialDanceMoveFeedbacks?.PlayFeedbacks();
			_lastMoveStartedAt = Time.time;
		}
	}
}

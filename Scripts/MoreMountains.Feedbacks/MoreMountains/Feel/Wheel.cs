using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.Feel;

public class Wheel : MonoBehaviour
{
	[Header("Binding")]
	[Tooltip("the part of the wheel that rotates")]
	public Transform RotatingPart;

	[Header("Settings")]
	[Tooltip("the speed at which the wheel should rotate")]
	public float RotationSpeed = 20f;

	[Header("Feedbacks")]
	[Tooltip("a feedback to call when the wheel starts turning")]
	public MMFeedbacks TurnFeedback;

	[Tooltip("a feedback to call when the wheel stops turning")]
	public MMFeedbacks TurnStopFeedback;

	protected bool _turning;

	protected virtual void Update()
	{
		HandleInput();
		HandleWheel();
	}

	protected virtual void HandleInput()
	{
		if (FeelDemosInputHelper.CheckMainActionInputPressedThisFrame())
		{
			Turn();
		}
		if (FeelDemosInputHelper.CheckMainActionInputUpThisFrame())
		{
			TurnStop();
		}
	}

	protected virtual void HandleWheel()
	{
		if (_turning)
		{
			RotatingPart.transform.Rotate(base.transform.right, RotationSpeed * Time.deltaTime);
		}
	}

	protected virtual void Turn()
	{
		if (!_turning)
		{
			TurnFeedback?.PlayFeedbacks();
		}
		_turning = true;
	}

	protected virtual void TurnStop()
	{
		TurnFeedback?.StopFeedbacks();
		TurnStopFeedback?.PlayFeedbacks();
		_turning = false;
	}
}

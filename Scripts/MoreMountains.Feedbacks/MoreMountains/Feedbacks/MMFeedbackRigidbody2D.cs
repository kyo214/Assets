using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will let you apply forces and torques (relative or not) to a Rigidbody.")]
[FeedbackPath("GameObject/Rigidbody2D")]
public class MMFeedbackRigidbody2D : MMFeedback
{
	public enum Modes
	{
		AddForce = 0,
		AddRelativeForce = 1,
		AddTorque = 2
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("Rigidbody")]
	[Tooltip("the rigidbody to target on play")]
	public Rigidbody2D TargetRigidbody2D;

	[Tooltip("the selected mode for this feedback")]
	public Modes Mode;

	[Tooltip("the min force or torque to apply")]
	[MMFEnumCondition("Mode", new int[] { 0, 1 })]
	public Vector2 MinForce;

	[Tooltip("the max force or torque to apply")]
	[MMFEnumCondition("Mode", new int[] { 0, 1 })]
	public Vector2 MaxForce;

	[Tooltip("the min torque to apply to this rigidbody on play")]
	[MMFEnumCondition("Mode", new int[] { 2 })]
	public float MinTorque;

	[Tooltip("the max torque to apply to this rigidbody on play")]
	[MMFEnumCondition("Mode", new int[] { 2 })]
	public float MaxTorque;

	[Tooltip("the force mode to apply")]
	public ForceMode2D AppliedForceMode = ForceMode2D.Impulse;

	protected Vector2 _force;

	protected float _torque;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized || TargetRigidbody2D == null)
		{
			return;
		}
		switch (Mode)
		{
		case Modes.AddForce:
			_force.x = Random.Range(MinForce.x, MaxForce.x);
			_force.y = Random.Range(MinForce.y, MaxForce.y);
			if (!Timing.ConstantIntensity)
			{
				_force *= feedbacksIntensity;
			}
			TargetRigidbody2D.AddForce(_force, AppliedForceMode);
			break;
		case Modes.AddRelativeForce:
			_force.x = Random.Range(MinForce.x, MaxForce.x);
			_force.y = Random.Range(MinForce.y, MaxForce.y);
			if (!Timing.ConstantIntensity)
			{
				_force *= feedbacksIntensity;
			}
			TargetRigidbody2D.AddRelativeForce(_force, AppliedForceMode);
			break;
		case Modes.AddTorque:
			_torque = Random.Range(MinTorque, MaxTorque);
			if (!Timing.ConstantIntensity)
			{
				_torque *= feedbacksIntensity;
			}
			TargetRigidbody2D.AddTorque(_torque, AppliedForceMode);
			break;
		}
	}
}

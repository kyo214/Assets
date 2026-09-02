using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will let you apply forces and torques (relative or not) to a Rigidbody.")]
[FeedbackPath("GameObject/Rigidbody")]
public class MMFeedbackRigidbody : MMFeedback
{
	public enum Modes
	{
		AddForce = 0,
		AddRelativeForce = 1,
		AddTorque = 2,
		AddRelativeTorque = 3
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("Rigidbody")]
	[Tooltip("the rigidbody to target on play")]
	public Rigidbody TargetRigidbody;

	[Tooltip("the selected mode for this feedback")]
	public Modes Mode;

	[Tooltip("the min force or torque to apply")]
	public Vector3 MinForce;

	[Tooltip("the max force or torque to apply")]
	public Vector3 MaxForce;

	[Tooltip("the force mode to apply")]
	public ForceMode AppliedForceMode = ForceMode.Impulse;

	protected Vector3 _force;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && !(TargetRigidbody == null))
		{
			_force.x = Random.Range(MinForce.x, MaxForce.x);
			_force.y = Random.Range(MinForce.y, MaxForce.y);
			_force.z = Random.Range(MinForce.z, MaxForce.z);
			if (!Timing.ConstantIntensity)
			{
				_force *= feedbacksIntensity;
			}
			switch (Mode)
			{
			case Modes.AddForce:
				TargetRigidbody.AddForce(_force, AppliedForceMode);
				break;
			case Modes.AddRelativeForce:
				TargetRigidbody.AddRelativeForce(_force, AppliedForceMode);
				break;
			case Modes.AddTorque:
				TargetRigidbody.AddTorque(_force, AppliedForceMode);
				break;
			case Modes.AddRelativeTorque:
				TargetRigidbody.AddRelativeTorque(_force, AppliedForceMode);
				break;
			}
		}
	}
}

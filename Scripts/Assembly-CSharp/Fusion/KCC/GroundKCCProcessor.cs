using UnityEngine;

namespace Fusion.KCC;

public class GroundKCCProcessor : KCCProcessor, IGroundKCCProcessor
{
	public static readonly int DefaultPriority = 2000;

	[SerializeField]
	[Tooltip("Maximum allowed speed the KCC can move with player input.")]
	private float _kinematicSpeed = 8f;

	[SerializeField]
	[Range(0f, 1f)]
	[Tooltip("How fast the KCC slows down if the actual kinematic speed is higher (typically when leaving processor with higher speed).")]
	private float _kinematicSpeedLimitFactor = 1f;

	[SerializeField]
	[Range(0f, 1f)]
	[Tooltip("How fast the KCC starts accelerating in recently calculated kinematic direction. Existing kinematic velocity is reduced based on angle between the velocity and new direction.")]
	private float _kinematicDirectionResponsivity = 1f;

	[SerializeField]
	[Tooltip("Kinematic velocity is accelerated by a costant value.")]
	private float _constantKinematicAcceleration;

	[SerializeField]
	[Tooltip("Kinematic velocity is accelerated by calculated kinematic speed multiplied by this.")]
	private float _relativeKinematicAcceleration = 50f;

	[SerializeField]
	[Tooltip("Kinematic velocity is accelerated by (calculated kinematic speed - actual kinematic speed) multiplied by this. The faster KCC moves, the less acceleration is applied.")]
	private float _proportionalKinematicAcceleration;

	[SerializeField]
	[Tooltip("Kinematic velocity is decelerated by a costant value.")]
	private float _constantKinematicFriction;

	[SerializeField]
	[Tooltip("Kinematic velocity is decelerated by calculated kinematic speed multiplied by this.")]
	private float _relativeKinematicFriction;

	[SerializeField]
	[Tooltip("Kinematic velocity is decelerated by actual kinematic speed multiplied by this. The faster KCC moves, the more deceleration is applied.")]
	private float _proportionalKinematicFriction = 35f;

	[SerializeField]
	[Tooltip("Dynamic velocity is decelerated by actual dynamic speed multiplied by this. The faster KCC moves, the more deceleration is applied.")]
	private float _proportionalDynamicFriction = 20f;

	[SerializeField]
	[Tooltip("Resets dynamic velocity upon grounding.")]
	private bool _clearDynamicVelocityOnTouch;

	[SerializeField]
	[Range(0f, 1f)]
	[Tooltip("How fast input direction propagates to kinematic direction.")]
	private float _inputResponsivity = 1f;

	[SerializeField]
	[Range(0f, 90f)]
	[Tooltip("Maximum ground angle.")]
	private float _maxGroundAngle = 64f;

	[SerializeField]
	[Range(0f, 90f)]
	[Tooltip("Angle at which KCC starts moving slower (resulting kinematic speed is linear interpolation between base kinematic speed and 0).")]
	private float _slowWalkAngle = 48f;

	[SerializeField]
	[Tooltip("Custom jump multiplier.")]
	private float _jumpMultiplier = 1f;

	[SerializeField]
	[Tooltip("Relative priority. Default ground processor priority is 2000.")]
	private int _relativePriority;

	public float KinematicSpeed => _kinematicSpeed;

	public float JumpMultiplier => _jumpMultiplier;

	public override float Priority => DefaultPriority + _relativePriority;

	public override EKCCStages GetValidStages(KCC kcc, KCCData data)
	{
		EKCCStages eKCCStages = EKCCStages.SetInputProperties | EKCCStages.ProcessPhysicsQuery;
		if (data.IsGrounded)
		{
			eKCCStages |= EKCCStages.SetDynamicVelocity;
			eKCCStages |= EKCCStages.SetKinematicDirection;
			eKCCStages |= EKCCStages.SetKinematicTangent;
			eKCCStages |= EKCCStages.SetKinematicSpeed;
			eKCCStages |= EKCCStages.SetKinematicVelocity;
		}
		return eKCCStages;
	}

	public override void SetInputProperties(KCC kcc, KCCData data)
	{
		data.MaxGroundAngle = _maxGroundAngle;
		SuppressOtherProcessors(kcc);
	}

	public override void SetDynamicVelocity(KCC kcc, KCCData data)
	{
		if (!data.IsSteppingUp && (data.IsSnappingToGround || data.GroundDistance > 0.001f))
		{
			data.DynamicVelocity += data.Gravity * data.DeltaTime;
		}
		if (!data.JumpImpulse.IsZero() && _jumpMultiplier > 0f)
		{
			Vector3 normalized = data.JumpImpulse.normalized;
			data.DynamicVelocity -= Vector3.Scale(data.DynamicVelocity, normalized);
			data.DynamicVelocity += data.JumpImpulse / kcc.Settings.Mass * _jumpMultiplier;
			data.HasJumped = true;
		}
		data.DynamicVelocity += data.ExternalVelocity;
		data.DynamicVelocity += data.ExternalAcceleration * data.DeltaTime;
		data.DynamicVelocity += data.ExternalImpulse / kcc.Settings.Mass;
		data.DynamicVelocity += data.ExternalForce / kcc.Settings.Mass * data.DeltaTime;
		if (!data.DynamicVelocity.IsZero())
		{
			if (data.DynamicVelocity.IsAlmostZero(0.001f))
			{
				data.DynamicVelocity = default;
			}
			else
			{
				Vector3 one = Vector3.one;
				if (data.GroundDistance > 0.001f || data.IsSnappingToGround)
				{
					one.y = 0f;
				}
				data.DynamicVelocity += KCCPhysicsUtility.GetFriction(data.DynamicVelocity, data.DynamicVelocity, one, data.GroundNormal, data.KinematicSpeed, clampSpeed: true, 0f, 0f, _proportionalDynamicFriction, data.DeltaTime, kcc.FixedData.DeltaTime);
			}
		}
		SuppressOtherProcessors(kcc);
	}

	public override void SetKinematicDirection(KCC kcc, KCCData data)
	{
		Vector3 toDirection = data.InputDirection.OnlyXZ();
		Vector3 fromDirection = data.KinematicDirection.OnlyXZ();
		data.KinematicDirection = KCCUtility.EasyLerpDirection(fromDirection, toDirection, data.DeltaTime, _inputResponsivity);
		SuppressOtherProcessors(kcc);
	}

	public override void SetKinematicTangent(KCC kcc, KCCData data)
	{
		data.KinematicTangent = default;
		if (!data.KinematicDirection.IsAlmostZero(0.0001f) && KCCPhysicsUtility.ProjectOnGround(data.GroundNormal, data.KinematicDirection, out var projectedVector))
		{
			data.KinematicTangent = projectedVector.normalized;
		}
		else
		{
			data.KinematicTangent = data.GroundTangent;
		}
		SuppressOtherProcessors(kcc);
	}

	public override void SetKinematicSpeed(KCC kcc, KCCData data)
	{
		if (data.GroundAngle <= _slowWalkAngle || Vector3.Dot(data.KinematicTangent, Vector3.up) <= 0f)
		{
			data.KinematicSpeed = _kinematicSpeed;
		}
		else
		{
			float num = KCCMathUtility.Map(_slowWalkAngle, data.MaxGroundAngle, 0f, 1f, data.GroundAngle);
			data.KinematicSpeed = Mathf.Lerp(0f, _kinematicSpeed, 1f - num);
		}
		SuppressOtherProcessors(kcc);
	}

	public override void SetKinematicVelocity(KCC kcc, KCCData data)
	{
		if (!data.KinematicVelocity.IsAlmostZero() && KCCPhysicsUtility.ProjectOnGround(data.GroundNormal, data.KinematicVelocity, out var projectedVector))
		{
			data.KinematicVelocity = projectedVector.normalized * data.KinematicVelocity.magnitude;
		}
		if (data.KinematicDirection.IsAlmostZero())
		{
			data.KinematicVelocity += KCCPhysicsUtility.GetFriction(data.KinematicVelocity, data.KinematicVelocity, Vector3.one, data.GroundNormal, data.KinematicSpeed, clampSpeed: true, _constantKinematicFriction, _relativeKinematicFriction, _proportionalKinematicFriction, data.DeltaTime, kcc.FixedData.DeltaTime);
			SuppressOtherProcessors(kcc);
			return;
		}
		if (_kinematicDirectionResponsivity > 0f)
		{
			data.KinematicVelocity -= data.KinematicVelocity * (1f - Mathf.Clamp01(Vector3.Dot(data.KinematicVelocity.OnlyXZ().normalized, data.KinematicDirection.OnlyXZ().normalized))) * Mathf.Min(_kinematicDirectionResponsivity, 1f);
		}
		Vector3 dynamicVelocity = data.DynamicVelocity;
		Vector3 kinematicVelocity = data.KinematicVelocity;
		Vector3 vector = kinematicVelocity;
		if (vector.IsZero())
		{
			vector = data.KinematicTangent;
		}
		Vector3 acceleration = KCCPhysicsUtility.GetAcceleration(kinematicVelocity, data.KinematicTangent, Vector3.one, data.KinematicSpeed, clampSpeed: false, data.KinematicDirection.magnitude, _constantKinematicAcceleration, _relativeKinematicAcceleration, _proportionalKinematicAcceleration, data.DeltaTime, kcc.FixedData.DeltaTime);
		Vector3 friction = KCCPhysicsUtility.GetFriction(kinematicVelocity, vector, Vector3.one, data.GroundNormal, data.KinematicSpeed, clampSpeed: false, _constantKinematicFriction, _relativeKinematicFriction, _proportionalKinematicFriction, data.DeltaTime, kcc.FixedData.DeltaTime);
		kinematicVelocity = KCCPhysicsUtility.CombineAccelerationAndFriction(kinematicVelocity, acceleration, friction);
		float num = Mathf.Max(b: data.KinematicVelocity.OnlyXZ().magnitude * Mathf.Clamp01(1f - _kinematicSpeedLimitFactor * (data.DeltaTime / kcc.FixedData.DeltaTime)), a: data.KinematicSpeed);
		float magnitude = kinematicVelocity.magnitude;
		if (magnitude > num)
		{
			kinematicVelocity *= num / magnitude;
		}
		Vector3 vector2 = dynamicVelocity.OnlyXZ();
		Vector3 vector3 = kinematicVelocity.OnlyXZ();
		Vector3 vector4 = vector2 + vector3;
		float magnitude2 = vector2.magnitude;
		float num2 = vector4.magnitude - Mathf.Max(magnitude2, num);
		if (num2 > 0f)
		{
			kinematicVelocity -= kinematicVelocity.normalized * num2;
		}
		if (data.HasJumped && kinematicVelocity.y < 0f)
		{
			kinematicVelocity.y = 0f;
		}
		data.KinematicVelocity = kinematicVelocity;
		SuppressOtherProcessors(kcc);
	}

	public override void ProcessPhysicsQuery(KCC kcc, KCCData data)
	{
		if (!data.IsGrounded)
		{
			return;
		}
		if (data.WasGrounded && !data.IsSnappingToGround && data.DynamicVelocity.y < 0f && data.DynamicVelocity.OnlyXZ().IsAlmostZero())
		{
			data.DynamicVelocity.y = 0f;
		}
		if (!data.WasGrounded)
		{
			if (_clearDynamicVelocityOnTouch)
			{
				data.DynamicVelocity = default;
			}
			Vector3 projectedVector;
			if (data.KinematicVelocity.OnlyXZ().IsAlmostZero())
			{
				data.KinematicVelocity.y = 0f;
			}
			else if (KCCPhysicsUtility.ProjectOnGround(data.GroundNormal, data.KinematicVelocity, out projectedVector))
			{
				data.KinematicVelocity = projectedVector.normalized * data.KinematicVelocity.magnitude;
			}
		}
		SuppressOtherProcessors(kcc);
	}

	private static void SuppressOtherProcessors(KCC kcc)
	{
		kcc.SuppressProcessors<IGroundKCCProcessor>();
	}
}

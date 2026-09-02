using UnityEngine;

namespace Fusion.KCC;

public class AirKCCProcessor : KCCProcessor, IAirKCCProcessor
{
	public static readonly int DefaultPriority = 1000;

	[SerializeField]
	[Tooltip("Maximum allowed speed the KCC can move with player input.")]
	private float _kinematicSpeed = 8f;

	[SerializeField]
	[Range(0f, 1f)]
	[Tooltip("How fast the KCC slows down if the actual kinematic speed is higher (typically when leaving processor with higher speed).")]
	private float _kinematicSpeedLimitFactor = 0.025f;

	[SerializeField]
	[Range(0f, 1f)]
	[Tooltip("How fast the KCC starts accelerating in recently calculated kinematic direction. Existing kinematic velocity is reduced based on angle between the velocity and new direction.")]
	private float _kinematicDirectionResponsivity;

	[SerializeField]
	[Tooltip("Kinematic velocity is accelerated by a costant value.")]
	private float _constantKinematicAcceleration;

	[SerializeField]
	[Tooltip("Kinematic velocity is accelerated by calculated kinematic speed multiplied by this.")]
	private float _relativeKinematicAcceleration = 5f;

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
	private float _proportionalKinematicFriction = 2f;

	[SerializeField]
	[Tooltip("Dynamic velocity is decelerated by actual dynamic speed multiplied by this. The faster KCC moves, the more deceleration is applied.")]
	private float _proportionalDynamicFriction = 2f;

	[SerializeField]
	[Range(0f, 1f)]
	[Tooltip("How fast input direction propagates to kinematic direction.")]
	private float _inputResponsivity = 0.75f;

	[SerializeField]
	[Range(0f, 1f)]
	[Tooltip("How much impact has overall vertical velocity on limiting kinematic velocity.")]
	private float _verticalVelocityImpact = 0.25f;

	[SerializeField]
	[Tooltip("Custom gravity multiplier.")]
	private float _gravityMultiplier = 1f;

	[SerializeField]
	[Tooltip("Relative priority. Default air processor priority is 1000.")]
	private int _relativePriority;

	public float KinematicSpeed => _kinematicSpeed;

	public float GravityMultiplier => _gravityMultiplier;

	public override float Priority => DefaultPriority + _relativePriority;

	public override EKCCStages GetValidStages(KCC kcc, KCCData data)
	{
		EKCCStages eKCCStages = EKCCStages.ProcessPhysicsQuery;
		if (!data.IsGrounded)
		{
			eKCCStages |= EKCCStages.SetInputProperties;
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
		data.Gravity *= _gravityMultiplier;
		SuppressOtherProcessors(kcc);
	}

	public override void SetDynamicVelocity(KCC kcc, KCCData data)
	{
		data.DynamicVelocity += data.Gravity * data.DeltaTime;
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
				data.DynamicVelocity += KCCPhysicsUtility.GetFriction(data.DynamicVelocity, data.DynamicVelocity, new Vector3(1f, 0f, 1f), data.KinematicSpeed, clampSpeed: true, 0f, 0f, _proportionalDynamicFriction, data.DeltaTime, kcc.FixedData.DeltaTime);
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
		if (!data.KinematicDirection.IsAlmostZero(0.0001f))
		{
			data.KinematicTangent = data.KinematicDirection.normalized;
		}
		else
		{
			data.KinematicTangent = data.TransformDirection;
		}
		SuppressOtherProcessors(kcc);
	}

	public override void SetKinematicSpeed(KCC kcc, KCCData data)
	{
		data.KinematicSpeed = _kinematicSpeed;
		SuppressOtherProcessors(kcc);
	}

	public override void SetKinematicVelocity(KCC kcc, KCCData data)
	{
		if (data.KinematicDirection.IsZero())
		{
			data.KinematicVelocity += KCCPhysicsUtility.GetFriction(data.KinematicVelocity, data.KinematicVelocity, new Vector3(1f, 0f, 1f), data.KinematicSpeed, clampSpeed: true, _constantKinematicFriction, _relativeKinematicFriction, _proportionalKinematicFriction, data.DeltaTime, kcc.FixedData.DeltaTime);
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
		Vector3 friction = KCCPhysicsUtility.GetFriction(kinematicVelocity, vector, new Vector3(1f, 0f, 1f), data.KinematicSpeed, clampSpeed: false, _constantKinematicFriction, _relativeKinematicFriction, _proportionalKinematicFriction, data.DeltaTime, kcc.FixedData.DeltaTime);
		kinematicVelocity = KCCPhysicsUtility.CombineAccelerationAndFriction(kinematicVelocity, acceleration, friction);
		float num = Mathf.Max(b: new Vector3(data.KinematicVelocity.x, data.KinematicVelocity.y * _verticalVelocityImpact, data.KinematicVelocity.z).magnitude * Mathf.Clamp01(1f - _kinematicSpeedLimitFactor * (data.DeltaTime / kcc.FixedData.DeltaTime)), a: data.KinematicSpeed);
		float magnitude = kinematicVelocity.magnitude;
		if (magnitude > num)
		{
			kinematicVelocity *= num / magnitude;
		}
		Vector3 vector2 = dynamicVelocity;
		vector2.y *= _verticalVelocityImpact;
		Vector3 vector3 = kinematicVelocity;
		vector3.y *= _verticalVelocityImpact;
		Vector3 vector4 = vector2 + vector3;
		float magnitude2 = vector2.magnitude;
		float num2 = vector4.magnitude - Mathf.Max(magnitude2, num);
		if (num2 > 0f)
		{
			kinematicVelocity -= kinematicVelocity.normalized * num2;
		}
		data.KinematicVelocity = kinematicVelocity;
		SuppressOtherProcessors(kcc);
	}

	public override void ProcessPhysicsQuery(KCC kcc, KCCData data)
	{
		if (!data.IsGrounded && !data.WasGrounded)
		{
			if (data.DynamicVelocity.y > 0f && data.DeltaTime > 0f && ((data.TargetPosition - data.BasePosition) / data.DeltaTime).y.IsAlmostZero())
			{
				data.DynamicVelocity.y = 0f;
			}
			SuppressOtherProcessors(kcc);
		}
	}

	private static void SuppressOtherProcessors(KCC kcc)
	{
		kcc.SuppressProcessors<IAirKCCProcessor>();
	}
}

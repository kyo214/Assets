using UnityEngine;

namespace Fusion.KCC;

public sealed class KCCData
{
	public int Frame;

	public int Tick;

	public float Alpha;

	public float Time;

	public float DeltaTime;

	public float UnscaledDeltaTime;

	public Vector3 BasePosition;

	public Vector3 DesiredPosition;

	public Vector3 TargetPosition;

	public Vector3 InputDirection;

	public Vector3 JumpImpulse;

	public Vector3 Gravity;

	public float MaxGroundAngle;

	public float MaxWallAngle;

	public float MaxHangAngle;

	public int MaxMoveSteps;

	public Vector3 ExternalVelocity;

	public Vector3 ExternalAcceleration;

	public Vector3 ExternalImpulse;

	public Vector3 ExternalForce;

	public Vector3 ExternalDelta;

	public float KinematicSpeed;

	public Vector3 KinematicTangent;

	public Vector3 KinematicDirection;

	public Vector3 KinematicVelocity;

	public Vector3 DynamicVelocity;

	public float RealSpeed;

	public Vector3 RealVelocity;

	public bool HasJumped;

	public bool HasTeleported;

	public bool IsGrounded;

	public bool WasGrounded;

	public bool IsSteppingUp;

	public bool WasSteppingUp;

	public bool IsSnappingToGround;

	public bool WasSnappingToGround;

	public Vector3 GroundNormal;

	public Vector3 GroundTangent;

	public Vector3 GroundPosition;

	public float GroundDistance;

	public float GroundAngle;

	public readonly KCCCollisions Collisions = new KCCCollisions();

	public readonly KCCModifiers Modifiers = new KCCModifiers();

	public readonly KCCIgnores Ignores = new KCCIgnores();

	public readonly KCCHits Hits = new KCCHits();

	private float _lookPitch;

	private float _lookYaw;

	private Quaternion _lookRotation;

	private bool _lookRotationCalculated;

	private Vector3 _lookDirection;

	private bool _lookDirectionCalculated;

	private Quaternion _transformRotation;

	private bool _transformRotationCalculated;

	private Vector3 _transformDirection;

	private bool _transformDirectionCalculated;

	public float LookPitch
	{
		get
		{
			return _lookPitch;
		}
		set
		{
			if (_lookPitch != value)
			{
				_lookPitch = value;
				_lookRotationCalculated = false;
				_lookDirectionCalculated = false;
				_transformRotationCalculated = false;
				_transformDirectionCalculated = false;
			}
		}
	}

	public float LookYaw
	{
		get
		{
			return _lookYaw;
		}
		set
		{
			if (_lookYaw != value)
			{
				_lookYaw = value;
				_lookRotationCalculated = false;
				_lookDirectionCalculated = false;
				_transformRotationCalculated = false;
				_transformDirectionCalculated = false;
			}
		}
	}

	public Quaternion LookRotation
	{
		get
		{
			if (!_lookRotationCalculated)
			{
				_lookRotation = Quaternion.Euler(_lookPitch, _lookYaw, 0f);
				_lookRotationCalculated = true;
			}
			return _lookRotation;
		}
	}

	public Vector3 LookDirection
	{
		get
		{
			if (!_lookDirectionCalculated)
			{
				_lookDirection = LookRotation * Vector3.forward;
				_lookDirectionCalculated = true;
			}
			return _lookDirection;
		}
	}

	public Quaternion TransformRotation
	{
		get
		{
			if (!_transformRotationCalculated)
			{
				_transformRotation = Quaternion.Euler(0f, _lookYaw, 0f);
				_transformRotationCalculated = true;
			}
			return _transformRotation;
		}
	}

	public Vector3 TransformDirection
	{
		get
		{
			if (!_transformDirectionCalculated)
			{
				_transformDirection = TransformRotation * Vector3.forward;
				_transformDirectionCalculated = true;
			}
			return _transformDirection;
		}
	}

	public Vector3 DesiredVelocity => KinematicVelocity + DynamicVelocity;

	public bool IsOnEdge
	{
		get
		{
			if (!IsGrounded)
			{
				return WasGrounded;
			}
			return false;
		}
	}

	public Vector2 GetLookRotation(bool pitch, bool yaw)
	{
		Vector2 result = default;
		if (pitch)
		{
			result.x = _lookPitch;
		}
		if (yaw)
		{
			result.y = _lookYaw;
		}
		return result;
	}

	public void Clear()
	{
		Collisions.Clear();
		Modifiers.Clear();
		Ignores.Clear();
		Hits.Clear();
	}

	public void CopyFromOther(KCCData other)
	{
		Frame = other.Frame;
		Tick = other.Tick;
		Alpha = other.Alpha;
		Time = other.Time;
		DeltaTime = other.DeltaTime;
		UnscaledDeltaTime = other.UnscaledDeltaTime;
		BasePosition = other.BasePosition;
		DesiredPosition = other.DesiredPosition;
		TargetPosition = other.TargetPosition;
		LookPitch = other.LookPitch;
		LookYaw = other.LookYaw;
		InputDirection = other.InputDirection;
		JumpImpulse = other.JumpImpulse;
		Gravity = other.Gravity;
		MaxGroundAngle = other.MaxGroundAngle;
		MaxWallAngle = other.MaxWallAngle;
		MaxHangAngle = other.MaxHangAngle;
		MaxMoveSteps = other.MaxMoveSteps;
		ExternalVelocity = other.ExternalVelocity;
		ExternalAcceleration = other.ExternalAcceleration;
		ExternalImpulse = other.ExternalImpulse;
		ExternalForce = other.ExternalForce;
		ExternalDelta = other.ExternalDelta;
		KinematicSpeed = other.KinematicSpeed;
		KinematicTangent = other.KinematicTangent;
		KinematicDirection = other.KinematicDirection;
		KinematicVelocity = other.KinematicVelocity;
		DynamicVelocity = other.DynamicVelocity;
		RealSpeed = other.RealSpeed;
		RealVelocity = other.RealVelocity;
		HasJumped = other.HasJumped;
		HasTeleported = other.HasTeleported;
		IsGrounded = other.IsGrounded;
		WasGrounded = other.WasGrounded;
		IsSteppingUp = other.IsSteppingUp;
		WasSteppingUp = other.WasSteppingUp;
		IsSnappingToGround = other.IsSnappingToGround;
		WasSnappingToGround = other.WasSnappingToGround;
		GroundNormal = other.GroundNormal;
		GroundTangent = other.GroundTangent;
		GroundPosition = other.GroundPosition;
		GroundDistance = other.GroundDistance;
		GroundAngle = other.GroundAngle;
		Collisions.CopyFromOther(other.Collisions);
		Modifiers.CopyFromOther(other.Modifiers);
		Ignores.CopyFromOther(other.Ignores);
		Hits.CopyFromOther(other.Hits);
	}
}

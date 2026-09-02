using UnityEngine;

namespace MoreMountains.Tools;

public class MMFaceDirection : MonoBehaviour
{
	public enum UpdateModes
	{
		Update = 0,
		LateUpdate = 1,
		FixedUpdate = 2
	}

	public enum ForwardVectors
	{
		Forward = 0,
		Up = 1,
		Right = 2
	}

	public enum FacingModes
	{
		MovementDirection = 0,
		Target = 1
	}

	[Header("Facing Mode")]
	public FacingModes FacingMode;

	[MMEnumCondition("FacingMode", new int[] { 1 })]
	public Transform FacingTarget;

	[MMEnumCondition("FacingMode", new int[] { 0 })]
	public float MinimumMovementThreshold = 0.2f;

	[Header("Directions")]
	public ForwardVectors ForwardVector;

	public Vector3 DirectionRotationAngles = Vector3.zero;

	[Header("Axis Locks")]
	public bool LockXAxis;

	public bool LockYAxis;

	public bool LockZAxis;

	[Header("Timing")]
	public UpdateModes UpdateMode = UpdateModes.LateUpdate;

	public float InterpolationSpeed = 0.15f;

	protected Vector3 _direction;

	protected Vector3 _positionLastFrame;

	protected Transform _transform;

	protected Vector3 _upwards;

	protected Vector3 _targetPosition;

	protected virtual void Awake()
	{
		Initialization();
	}

	protected virtual void Initialization()
	{
		_transform = base.transform;
		_positionLastFrame = _transform.position;
		switch (ForwardVector)
		{
		case ForwardVectors.Forward:
			_upwards = Vector3.forward;
			break;
		case ForwardVectors.Up:
			_upwards = Vector3.up;
			break;
		case ForwardVectors.Right:
			_upwards = Vector3.right;
			break;
		}
	}

	protected virtual void FaceDirection()
	{
		if (FacingMode == FacingModes.Target)
		{
			_targetPosition = FacingTarget.position;
			if (LockXAxis)
			{
				_targetPosition.x = _transform.position.x;
			}
			if (LockYAxis)
			{
				_targetPosition.y = _transform.position.y;
			}
			if (LockZAxis)
			{
				_targetPosition.z = _transform.position.z;
			}
			_direction = _targetPosition - _transform.position;
			_direction = Quaternion.Euler(DirectionRotationAngles.x, DirectionRotationAngles.y, DirectionRotationAngles.z) * _direction;
			ApplyRotation();
		}
		if (FacingMode == FacingModes.MovementDirection)
		{
			_direction = (_transform.position - _positionLastFrame).normalized;
			if (LockXAxis)
			{
				_direction.x = 0f;
			}
			if (LockYAxis)
			{
				_direction.y = 0f;
			}
			if (LockZAxis)
			{
				_direction.z = 0f;
			}
			_direction = Quaternion.Euler(DirectionRotationAngles.x, DirectionRotationAngles.y, DirectionRotationAngles.z) * _direction;
			if (Vector3.Distance(_transform.position, _positionLastFrame) > MinimumMovementThreshold)
			{
				ApplyRotation();
				_positionLastFrame = _transform.position;
			}
		}
	}

	protected virtual void ApplyRotation()
	{
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(_direction, _upwards), InterpolationSpeed * Time.time);
	}

	protected virtual void Update()
	{
		if (UpdateMode == UpdateModes.Update)
		{
			FaceDirection();
		}
	}

	protected virtual void LateUpdate()
	{
		if (UpdateMode == UpdateModes.LateUpdate)
		{
			FaceDirection();
		}
	}

	protected virtual void FixedUpdate()
	{
		if (UpdateMode == UpdateModes.FixedUpdate)
		{
			FaceDirection();
		}
	}
}

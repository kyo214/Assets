using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MoreMountains.Tools;

[Serializable]
public class MMAim
{
	public enum AimControls
	{
		Off = 0,
		PrimaryMovement = 1,
		SecondaryMovement = 2,
		Mouse = 3,
		Script = 4
	}

	public enum RotationModes
	{
		Free = 0,
		Strict4Directions = 1,
		Strict8Directions = 2
	}

	[Header("Control Mode")]
	[MMInformation("Pick a control mode : mouse (aims towards the pointer), primary movement (you'll aim towards the current input direction), or secondary movement (aims towards a second input axis, think twin stick shooters), and set minimum and maximum angles.", MMInformationAttribute.InformationType.Info, false)]
	public AimControls AimControl = AimControls.SecondaryMovement;

	public RotationModes RotationMode;

	public InputAction MousePositionAction;

	[Header("Limits")]
	[Range(-180f, 180f)]
	public float MinimumAngle = -180f;

	[Range(-180f, 180f)]
	public float MaximumAngle = 180f;

	[MMReadOnly]
	public float CurrentAngle;

	protected float[] _possibleAngleValues;

	protected Vector3 _currentAim = Vector3.zero;

	protected Vector3 _direction;

	protected Vector3 _mousePosition;

	protected Vector2 _inputSystemMousePosition;

	protected Camera _mainCamera;

	public Vector3 CurrentPosition { get; set; }

	public Vector2 PrimaryMovement { get; set; }

	public Vector2 SecondaryMovement { get; set; }

	public virtual void Initialization()
	{
		if (RotationMode == RotationModes.Strict4Directions)
		{
			_possibleAngleValues = new float[5];
			_possibleAngleValues[0] = -180f;
			_possibleAngleValues[1] = -90f;
			_possibleAngleValues[2] = 0f;
			_possibleAngleValues[3] = 90f;
			_possibleAngleValues[4] = 180f;
		}
		if (RotationMode == RotationModes.Strict8Directions)
		{
			_possibleAngleValues = new float[9];
			_possibleAngleValues[0] = -180f;
			_possibleAngleValues[1] = -135f;
			_possibleAngleValues[2] = -90f;
			_possibleAngleValues[3] = -45f;
			_possibleAngleValues[4] = 0f;
			_possibleAngleValues[5] = 45f;
			_possibleAngleValues[6] = 90f;
			_possibleAngleValues[7] = 135f;
			_possibleAngleValues[8] = 180f;
		}
		_mainCamera = Camera.main;
		MousePositionAction.Enable();
		MousePositionAction.performed += (InputAction.CallbackContext context) =>
		{
			_inputSystemMousePosition = context.ReadValue<Vector2>();
		};
		MousePositionAction.canceled += (InputAction.CallbackContext context) =>
		{
			_inputSystemMousePosition = Vector2.zero;
		};
	}

	public virtual Vector2 GetCurrentAim()
	{
		switch (AimControl)
		{
		case AimControls.Off:
			_currentAim = Vector2.zero;
			break;
		case AimControls.PrimaryMovement:
			_currentAim = PrimaryMovement;
			break;
		case AimControls.SecondaryMovement:
			_currentAim = SecondaryMovement;
			break;
		case AimControls.Mouse:
			_mousePosition = _inputSystemMousePosition;
			_mousePosition.z = 10f;
			_direction = _mainCamera.ScreenToWorldPoint(_mousePosition);
			_direction.z = CurrentPosition.z;
			_currentAim = _direction - CurrentPosition;
			break;
		default:
			_currentAim = Vector2.zero;
			break;
		case AimControls.Script:
			break;
		}
		CurrentAngle = Mathf.Atan2(_currentAim.y, _currentAim.x) * 57.29578f;
		if (CurrentAngle < MinimumAngle || CurrentAngle > MaximumAngle)
		{
			float f = Mathf.DeltaAngle(CurrentAngle, MinimumAngle);
			float f2 = Mathf.DeltaAngle(CurrentAngle, MaximumAngle);
			CurrentAngle = ((Mathf.Abs(f) < Mathf.Abs(f2)) ? MinimumAngle : MaximumAngle);
		}
		if (RotationMode == RotationModes.Strict4Directions || RotationMode == RotationModes.Strict8Directions)
		{
			CurrentAngle = MMMaths.RoundToClosest(CurrentAngle, _possibleAngleValues);
		}
		CurrentAngle = Mathf.Clamp(CurrentAngle, MinimumAngle, MaximumAngle);
		_currentAim = ((_currentAim.magnitude == 0f) ? Vector2.zero : MMMaths.RotateVector2(Vector2.right, CurrentAngle));
		return _currentAim;
	}

	public virtual void SetAim(Vector2 newAim)
	{
		_currentAim = newAim;
	}
}

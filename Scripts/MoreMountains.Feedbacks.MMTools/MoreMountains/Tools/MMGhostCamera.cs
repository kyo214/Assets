using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Camera/MMGhostCamera")]
public class MMGhostCamera : MonoBehaviour
{
	[Header("Speed")]
	public float MovementSpeed = 10f;

	public float RunFactor = 4f;

	public float Acceleration = 5f;

	public float Deceleration = 5f;

	public float RotationSpeed = 40f;

	[Header("Controls")]
	public InputAction HorizontalAction;

	public InputAction VerticalAction;

	public InputAction MousePositionAction;

	public Key ActivateKey = Key.LeftShift;

	public Key UpKey = Key.Space;

	public Key DownKey = Key.C;

	public Key ControlsModeSwitchKey = Key.M;

	public Key TimescaleModificationKey = Key.F;

	public Key RunKey = Key.RightShift;

	[Header("Mouse")]
	public float MouseSensitivity = 0.02f;

	public float MobileStickSensitivity = 2f;

	[Header("Timescale Modification")]
	public float TimescaleModifier = 0.5f;

	[Header("Settings")]
	public bool AutoActivation = true;

	public bool MovementEnabled = true;

	public bool RotationEnabled = true;

	[MMReadOnly]
	public bool Active;

	[MMReadOnly]
	public bool TimeAltered;

	[Header("Virtual Joysticks")]
	public bool UseMobileControls;

	[MMCondition("UseMobileControls", true)]
	public GameObject LeftStickContainer;

	[MMCondition("UseMobileControls", true)]
	public GameObject RightStickContainer;

	[MMCondition("UseMobileControls", true)]
	public MMTouchJoystick LeftStick;

	[MMCondition("UseMobileControls", true)]
	public MMTouchJoystick RightStick;

	protected Vector3 _currentInput;

	protected Vector3 _lerpedInput;

	protected Vector3 _normalizedInput;

	protected float _acceleration;

	protected float _deceleration;

	protected Vector3 _movementVector = Vector3.zero;

	protected float _speedMultiplier;

	protected Vector3 _newEulerAngles;

	protected Vector2 _mouseInput;

	protected virtual void Start()
	{
		if (AutoActivation)
		{
			ToggleFreeCamera();
		}
		HorizontalAction.Enable();
		VerticalAction.Enable();
		MousePositionAction.Enable();
		HorizontalAction.performed += (InputAction.CallbackContext context) =>
		{
			_currentInput.x = context.ReadValue<float>();
		};
		VerticalAction.performed += (InputAction.CallbackContext context) =>
		{
			_currentInput.z = context.ReadValue<float>();
		};
		MousePositionAction.performed += (InputAction.CallbackContext context) =>
		{
			_mouseInput = context.ReadValue<Vector2>();
		};
		HorizontalAction.canceled += (InputAction.CallbackContext context) =>
		{
			_currentInput.x = 0f;
		};
		VerticalAction.canceled += (InputAction.CallbackContext context) =>
		{
			_currentInput.z = 0f;
		};
		MousePositionAction.canceled += (InputAction.CallbackContext context) =>
		{
			_mouseInput = Vector2.zero;
		};
	}

	protected virtual void Update()
	{
		if (Keyboard.current[ActivateKey].wasPressedThisFrame)
		{
			ToggleFreeCamera();
		}
		if (Active)
		{
			GetInput();
			Translate();
			Rotate();
			Move();
			HandleMobileControls();
		}
	}

	protected virtual void GetInput()
	{
		if (UseMobileControls && !(LeftStick == null))
		{
			_currentInput.x = LeftStick._joystickValue.x;
			_currentInput.y = 0f;
			_currentInput.z = LeftStick._joystickValue.y;
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool isPressed = Keyboard.current[UpKey].isPressed;
		flag = Keyboard.current[DownKey].isPressed;
		flag2 = Keyboard.current[RunKey].isPressed;
		flag3 = Keyboard.current[TimescaleModificationKey].wasPressedThisFrame;
		_currentInput.y = 0f;
		if (isPressed)
		{
			_currentInput.y = 1f;
		}
		if (flag)
		{
			_currentInput.y = -1f;
		}
		_speedMultiplier = (flag2 ? RunFactor : 1f);
		_normalizedInput = _currentInput.normalized;
		if (flag3)
		{
			ToggleSlowMotion();
		}
	}

	protected virtual void HandleMobileControls()
	{
		if (Keyboard.current[ControlsModeSwitchKey].wasPressedThisFrame)
		{
			UseMobileControls = !UseMobileControls;
		}
		if (UseMobileControls)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		else if (Active)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		if (LeftStickContainer != null)
		{
			LeftStickContainer?.SetActive(UseMobileControls);
		}
		if (RightStickContainer != null)
		{
			RightStickContainer?.SetActive(UseMobileControls);
		}
	}

	protected virtual void Translate()
	{
		if (MovementEnabled)
		{
			if (Acceleration == 0f || Deceleration == 0f)
			{
				_lerpedInput = _currentInput;
			}
			else if (_normalizedInput.magnitude == 0f)
			{
				_acceleration = Mathf.Lerp(_acceleration, 0f, Deceleration * Time.deltaTime);
				_lerpedInput = Vector3.Lerp(_lerpedInput, _lerpedInput * _acceleration, Time.deltaTime * Deceleration);
			}
			else
			{
				_acceleration = Mathf.Lerp(_acceleration, 1f, Acceleration * Time.deltaTime);
				_lerpedInput = Vector3.ClampMagnitude(_normalizedInput, _acceleration);
			}
			_movementVector = _lerpedInput;
			_movementVector *= MovementSpeed * _speedMultiplier;
			if (_movementVector.magnitude > MovementSpeed * _speedMultiplier)
			{
				_movementVector = Vector3.ClampMagnitude(_movementVector, MovementSpeed * _speedMultiplier);
			}
		}
	}

	protected virtual void Rotate()
	{
		if (RotationEnabled)
		{
			_newEulerAngles = base.transform.eulerAngles;
			if (!UseMobileControls || LeftStick == null)
			{
				_newEulerAngles.x += (0f - _mouseInput.y) * 359f * MouseSensitivity;
				_newEulerAngles.y += _mouseInput.x * 359f * MouseSensitivity;
			}
			else
			{
				_newEulerAngles.x += (0f - RightStick._joystickValue.y) * MobileStickSensitivity;
				_newEulerAngles.y += RightStick._joystickValue.x * MobileStickSensitivity;
			}
			_newEulerAngles = Vector3.Lerp(base.transform.eulerAngles, _newEulerAngles, Time.deltaTime * RotationSpeed);
		}
	}

	protected virtual void Move()
	{
		base.transform.eulerAngles = _newEulerAngles;
		base.transform.position += base.transform.rotation * _movementVector * Time.deltaTime;
	}

	protected virtual void ToggleSlowMotion()
	{
		TimeAltered = !TimeAltered;
		if (TimeAltered)
		{
			MMTimeScaleEvent.Trigger(MMTimeScaleMethods.For, TimescaleModifier, 1f, lerp: true, 5f, infinite: true);
		}
		else
		{
			MMTimeScaleEvent.Trigger(MMTimeScaleMethods.Unfreeze, 1f, 0f, lerp: false, 0f, infinite: false);
		}
	}

	protected virtual void ToggleFreeCamera()
	{
		Active = !Active;
		Cursor.lockState = (Active ? CursorLockMode.Locked : CursorLockMode.None);
		Cursor.visible = !Active;
	}
}

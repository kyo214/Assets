using UnityEngine;
using UnityEngine.Events;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Camera/MMOrbitalCamera")]
public class MMOrbitalCamera : MonoBehaviour
{
	public enum Modes
	{
		Mouse = 0,
		Touch = 1
	}

	[Header("Setup")]
	public Modes Mode = Modes.Touch;

	public Transform Target;

	public Vector3 TargetOffset;

	[MMReadOnly]
	public float DistanceToTarget = 5f;

	[Header("Rotation")]
	public bool RotationEnabled = true;

	public Vector2 RotationSpeed = new Vector2(200f, 200f);

	public int MinVerticalAngleLimit = -80;

	public int MaxVerticalAngleLimit = 80;

	[Header("Zoom")]
	public bool ZoomEnabled = true;

	public float MinimumZoomDistance = 0.6f;

	public float MaximumZoomDistance = 20f;

	public int ZoomSpeed = 40;

	public float ZoomDampening = 5f;

	[Header("Mouse Zoom")]
	public float MouseWheelSpeed = 10f;

	public float MaxMouseWheelClamp = 10f;

	[Header("Steps")]
	public float StepThreshold = 1f;

	public UnityEvent StepFeedback;

	protected float _angleX;

	protected float _angleY;

	protected float _currentDistance;

	protected float _desiredDistance;

	protected Quaternion _currentRotation;

	protected Quaternion _desiredRotation;

	protected Quaternion _rotation;

	protected Vector3 _position;

	protected float _scrollWheelAmount;

	protected float _stepBuffer;

	protected virtual void Start()
	{
		Initialization();
	}

	public virtual void Initialization()
	{
		if (Target == null)
		{
			Debug.LogError(base.gameObject.name + " : the MMOrbitalCamera doesn't have a target.");
			return;
		}
		DistanceToTarget = Vector3.Distance(Target.position, base.transform.position);
		_currentDistance = DistanceToTarget;
		_desiredDistance = DistanceToTarget;
		_position = base.transform.position;
		_rotation = base.transform.rotation;
		_currentRotation = base.transform.rotation;
		_desiredRotation = base.transform.rotation;
		_angleX = Vector3.Angle(Vector3.right, base.transform.right);
		_angleY = Vector3.Angle(Vector3.up, base.transform.up);
	}

	protected virtual void LateUpdate()
	{
		if (!(Target == null))
		{
			Rotation();
			Zoom();
			StepDetection();
			ApplyMovement();
		}
	}

	protected virtual void Rotation()
	{
		if (!RotationEnabled)
		{
			return;
		}
		if (Mode == Modes.Touch && Input.touchCount > 0)
		{
			if (Input.touches[0].phase == TouchPhase.Moved && Input.touchCount == 1)
			{
				float num = Screen.currentResolution.height;
				if (Input.touches[0].position.y < num / 4f)
				{
					return;
				}
				float num2 = Input.touches[0].deltaPosition.magnitude / Input.touches[0].deltaTime;
				_angleX += Input.touches[0].deltaPosition.x * RotationSpeed.x * Time.deltaTime * num2 * 1E-05f;
				_angleY -= Input.touches[0].deltaPosition.y * RotationSpeed.y * Time.deltaTime * num2 * 1E-05f;
				_stepBuffer += Input.touches[0].deltaPosition.x;
				_angleY = MMMaths.ClampAngle(_angleY, MinVerticalAngleLimit, MaxVerticalAngleLimit);
				_desiredRotation = Quaternion.Euler(_angleY, _angleX, 0f);
				_currentRotation = base.transform.rotation;
				_rotation = Quaternion.Lerp(_currentRotation, _desiredRotation, Time.deltaTime * ZoomDampening);
				base.transform.rotation = _rotation;
			}
			else if (Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Began)
			{
				_desiredRotation = base.transform.rotation;
			}
			if (base.transform.rotation != _desiredRotation)
			{
				_rotation = Quaternion.Lerp(base.transform.rotation, _desiredRotation, Time.deltaTime * ZoomDampening);
				base.transform.rotation = _rotation;
			}
		}
		else if (Mode == Modes.Mouse)
		{
			_angleX += Input.GetAxis("Mouse X") * RotationSpeed.x * Time.deltaTime;
			_angleY += (0f - Input.GetAxis("Mouse Y")) * RotationSpeed.y * Time.deltaTime;
			_angleY = Mathf.Clamp(_angleY, MinVerticalAngleLimit, MaxVerticalAngleLimit);
			_desiredRotation = Quaternion.Euler(new Vector3(_angleY, _angleX, 0f));
			_currentRotation = base.transform.rotation;
			_rotation = Quaternion.Lerp(_currentRotation, _desiredRotation, Time.deltaTime * ZoomDampening);
			base.transform.rotation = _rotation;
		}
	}

	protected virtual void StepDetection()
	{
		if (Mathf.Abs(_stepBuffer) > StepThreshold)
		{
			StepFeedback?.Invoke();
			_stepBuffer = 0f;
		}
	}

	protected virtual void Zoom()
	{
		if (!ZoomEnabled)
		{
			return;
		}
		if (Mode == Modes.Touch && Input.touchCount > 0)
		{
			if (Input.touchCount == 2)
			{
				Touch touch = Input.GetTouch(0);
				Touch touch2 = Input.GetTouch(1);
				Vector2 vector = touch.position - touch.deltaPosition;
				Vector2 vector2 = touch2.position - touch2.deltaPosition;
				float magnitude = (vector - vector2).magnitude;
				float magnitude2 = (touch.position - touch2.position).magnitude;
				float num = magnitude - magnitude2;
				_desiredDistance += num * Time.deltaTime * (float)ZoomSpeed * Mathf.Abs(_desiredDistance) * 0.001f;
				_desiredDistance = Mathf.Clamp(_desiredDistance, MinimumZoomDistance, MaximumZoomDistance);
				_currentDistance = Mathf.Lerp(_currentDistance, _desiredDistance, Time.deltaTime * ZoomDampening);
			}
		}
		else if (Mode == Modes.Mouse)
		{
			_scrollWheelAmount += (0f - Input.GetAxis("Mouse ScrollWheel")) * MouseWheelSpeed;
			_scrollWheelAmount = Mathf.Clamp(_scrollWheelAmount, 0f - MaxMouseWheelClamp, MaxMouseWheelClamp);
			float scrollWheelAmount = _scrollWheelAmount;
			_desiredDistance += scrollWheelAmount * Time.deltaTime * (float)ZoomSpeed * Mathf.Abs(_desiredDistance) * 0.001f;
			_desiredDistance = Mathf.Clamp(_desiredDistance, MinimumZoomDistance, MaximumZoomDistance);
			_currentDistance = Mathf.Lerp(_currentDistance, _desiredDistance, Time.deltaTime * ZoomDampening);
		}
	}

	protected virtual void ApplyMovement()
	{
		_position = Target.position - (_rotation * Vector3.forward * _currentDistance + TargetOffset);
		base.transform.position = _position;
	}
}

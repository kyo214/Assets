using UnityEngine;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Movement/MMAutoRotate")]
public class MMAutoRotate : MonoBehaviour
{
	public enum UpdateModes
	{
		Update = 0,
		LateUpdate = 1,
		FixedUpdate = 2
	}

	[Header("Rotation")]
	public bool Rotating = true;

	[MMCondition("Rotating", true)]
	public Space RotationSpace = Space.Self;

	public UpdateModes UpdateMode;

	[MMCondition("Rotating", true)]
	public Vector3 RotationSpeed = new Vector3(100f, 0f, 0f);

	[Header("Orbit")]
	public bool Orbiting;

	[MMCondition("Orbiting", true)]
	public bool AdditiveOrbitRotation;

	[MMCondition("Orbiting", true)]
	public Transform OrbitCenterTransform;

	[MMCondition("Orbiting", true)]
	public Vector3 OrbitCenterOffset = Vector3.zero;

	[MMCondition("Orbiting", true)]
	public Vector3 OrbitRotationAxis = new Vector3(0f, 1f, 0f);

	[MMCondition("Orbiting", true)]
	public float OrbitRotationSpeed = 10f;

	[MMCondition("Orbiting", true)]
	public float OrbitRadius = 3f;

	[MMCondition("Orbiting", true)]
	public float OrbitCorrectionSpeed = 10f;

	[Header("Settings")]
	public bool DrawGizmos = true;

	[MMCondition("DrawGizmos", true)]
	public Color OrbitPlaneColor = new Color(54f, 169f, 225f, 0.02f);

	[MMCondition("DrawGizmos", true)]
	public Color OrbitLineColor = new Color(225f, 225f, 225f, 0.1f);

	[HideInInspector]
	public Vector3 _orbitCenter;

	[HideInInspector]
	public Vector3 _worldRotationAxis;

	[HideInInspector]
	public Plane _rotationPlane;

	[HideInInspector]
	public Vector3 _snappedPosition;

	[HideInInspector]
	public Vector3 _radius;

	protected Quaternion _newRotation;

	protected Vector3 _desiredOrbitPosition;

	private Vector3 _previousPosition;

	protected virtual void Start()
	{
		_rotationPlane = default;
	}

	protected virtual void Update()
	{
		if (UpdateMode == UpdateModes.Update)
		{
			Rotate();
		}
	}

	protected virtual void FixedUpdate()
	{
		if (UpdateMode == UpdateModes.FixedUpdate)
		{
			Rotate();
		}
	}

	protected virtual void LateUpdate()
	{
		if (UpdateMode == UpdateModes.LateUpdate)
		{
			Rotate();
		}
	}

	public virtual void Rotate(bool status)
	{
		Rotating = status;
	}

	public virtual void Orbit(bool status)
	{
		Orbiting = status;
	}

	protected virtual void Rotate()
	{
		if (Rotating)
		{
			base.transform.Rotate(RotationSpeed * Time.deltaTime, RotationSpace);
		}
		if (Orbiting)
		{
			_orbitCenter = OrbitCenterTransform.transform.position + OrbitCenterOffset;
			if (AdditiveOrbitRotation)
			{
				_worldRotationAxis = OrbitCenterTransform.TransformDirection(OrbitRotationAxis);
			}
			else
			{
				_worldRotationAxis = OrbitRotationAxis;
			}
			_rotationPlane.SetNormalAndPosition(_worldRotationAxis.normalized, _orbitCenter);
			_snappedPosition = _rotationPlane.ClosestPointOnPlane(base.transform.position);
			_radius = OrbitRadius * Vector3.Normalize(_snappedPosition - _orbitCenter);
			_newRotation = Quaternion.AngleAxis(OrbitRotationSpeed * Time.deltaTime, _worldRotationAxis);
			_desiredOrbitPosition = _orbitCenter + _newRotation * _radius;
			base.transform.position = Vector3.Lerp(base.transform.position, _desiredOrbitPosition, OrbitCorrectionSpeed * Time.deltaTime);
			_previousPosition = _desiredOrbitPosition;
		}
	}
}

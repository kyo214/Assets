using UnityEngine;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Movement/MMStayInPlace")]
public class MMStayInPlace : MonoBehaviour
{
	public enum Spaces
	{
		World = 0,
		Local = 1
	}

	public enum UpdateModes
	{
		Update = 0,
		FixedUpdate = 1,
		LateUpdate = 2
	}

	[Header("Modes")]
	public UpdateModes UpdateMode = UpdateModes.LateUpdate;

	public Spaces Space;

	[Header("Attributes")]
	public bool FixedPosition = true;

	public bool FixedRotation = true;

	public bool FixedScale = true;

	[Header("Overrides")]
	public bool OverridePosition;

	[MMCondition("OverridePosition", true)]
	public Vector3 OverridePositionValue;

	public bool OverrideRotation;

	[MMCondition("OverrideRotation", true)]
	public Vector3 OverrideRotationValue;

	public bool OverrideScale;

	[MMCondition("OverrideScale", true)]
	public Vector3 OverrideScaleValue;

	protected Vector3 _initialPosition;

	protected Quaternion _initialRotation;

	protected Vector3 _initialScale;

	protected virtual void Awake()
	{
		Initialization();
	}

	protected virtual void Initialization()
	{
		_initialPosition = ((Space == Spaces.World) ? base.transform.position : base.transform.localPosition);
		_initialRotation = ((Space == Spaces.World) ? base.transform.rotation : base.transform.localRotation);
		_initialScale = ((Space == Spaces.World) ? base.transform.position : base.transform.localScale);
		if (OverridePosition)
		{
			_initialPosition = OverridePositionValue;
		}
		if (OverrideRotation)
		{
			_initialRotation = Quaternion.Euler(OverrideRotationValue);
		}
		if (OverrideScale)
		{
			_initialScale = OverrideScaleValue;
		}
	}

	protected virtual void Update()
	{
		if (UpdateMode == UpdateModes.Update)
		{
			StayInPlace();
		}
	}

	protected virtual void FixedUpdate()
	{
		if (UpdateMode == UpdateModes.FixedUpdate)
		{
			StayInPlace();
		}
	}

	protected virtual void LateUpdate()
	{
		if (UpdateMode == UpdateModes.LateUpdate)
		{
			StayInPlace();
		}
	}

	protected virtual void StayInPlace()
	{
		if (Space == Spaces.World)
		{
			if (FixedPosition)
			{
				base.transform.position = _initialPosition;
			}
			if (FixedRotation)
			{
				base.transform.rotation = _initialRotation;
			}
		}
		else
		{
			if (FixedPosition)
			{
				base.transform.localPosition = _initialPosition;
			}
			if (FixedRotation)
			{
				base.transform.localRotation = _initialRotation;
			}
		}
		if (FixedScale)
		{
			base.transform.localScale = _initialScale;
		}
	}
}

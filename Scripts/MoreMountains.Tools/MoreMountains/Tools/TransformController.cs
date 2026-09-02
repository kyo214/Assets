using UnityEngine;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Property Controllers/TransformController")]
public class TransformController : MonoBehaviour
{
	[Header("Position")]
	public bool ControlPositionX;

	[MMCondition("ControlPositionX", true)]
	public float PositionX;

	public bool ControlPositionY;

	[MMCondition("ControlPositionY", true)]
	public float PositionY;

	public bool ControlPositionZ;

	[MMCondition("ControlPositionZ", true)]
	public float PositionZ;

	[Header("Local Position")]
	public bool ControlLocalPositionX;

	[MMCondition("ControlLocalPositionX", true)]
	public float LocalPositionX;

	public bool ControlLocalPositionY;

	[MMCondition("ControlLocalPositionY", true)]
	public float LocalPositionY;

	public bool ControlLocalPositionZ;

	[MMCondition("ControlLocalPositionZ", true)]
	public float LocalPositionZ;

	[Header("Rotation")]
	public bool ControlRotationX;

	[MMCondition("ControlRotationX", true)]
	public float RotationX;

	public bool ControlRotationY;

	[MMCondition("ControlRotationY", true)]
	public float RotationY;

	public bool ControlRotationZ;

	[MMCondition("ControlRotationZ", true)]
	public float RotationZ;

	[Header("Local Rotation")]
	public bool ControlLocalRotationX;

	[MMCondition("ControlLocalRotationX", true)]
	public float LocalRotationX;

	public bool ControlLocalRotationY;

	[MMCondition("ControlLocalRotationY", true)]
	public float LocalRotationY;

	public bool ControlLocalRotationZ;

	[MMCondition("ControlLocalRotationZ", true)]
	public float LocalRotationZ;

	[Header("Scale")]
	public bool ControlScaleX;

	[MMCondition("ControlScaleX", true)]
	public float ScaleX;

	public bool ControlScaleY;

	[MMCondition("ControlScaleY", true)]
	public float ScaleY;

	public bool ControlScaleZ;

	[MMCondition("ControlScaleZ", true)]
	public float ScaleZ;

	protected Vector3 _position;

	protected Vector3 _localPosition;

	protected Vector3 _rotation;

	protected Vector3 _localRotation;

	protected Vector3 _scale;

	protected virtual void Update()
	{
		_position = base.transform.position;
		_localPosition = base.transform.localPosition;
		_rotation = base.transform.eulerAngles;
		_localRotation = base.transform.localEulerAngles;
		_scale = base.transform.localScale;
		if (ControlPositionX)
		{
			_position.x = PositionX;
			base.transform.position = _position;
		}
		if (ControlPositionY)
		{
			_position.y = PositionY;
			base.transform.position = _position;
		}
		if (ControlPositionZ)
		{
			_position.z = PositionZ;
			base.transform.position = _position;
		}
		if (ControlLocalPositionX)
		{
			_localPosition.x = LocalPositionX;
			base.transform.localPosition = _localPosition;
		}
		if (ControlLocalPositionY)
		{
			_localPosition.y = LocalPositionY;
			base.transform.localPosition = _localPosition;
		}
		if (ControlLocalPositionZ)
		{
			_localPosition.z = LocalPositionZ;
			base.transform.localPosition = _localPosition;
		}
		if (ControlRotationX)
		{
			_rotation.x = RotationX;
			base.transform.eulerAngles = _rotation;
		}
		if (ControlRotationY)
		{
			_rotation.y = RotationY;
			base.transform.eulerAngles = _rotation;
		}
		if (ControlRotationZ)
		{
			_rotation.z = RotationZ;
			base.transform.eulerAngles = _rotation;
		}
		if (ControlLocalRotationX)
		{
			_localRotation.x = LocalRotationX;
			base.transform.localEulerAngles = _localRotation;
		}
		if (ControlLocalRotationY)
		{
			_localRotation.y = LocalRotationY;
			base.transform.localEulerAngles = _localRotation;
		}
		if (ControlLocalRotationZ)
		{
			_localRotation.z = LocalRotationZ;
			base.transform.localEulerAngles = _localRotation;
		}
		if (ControlScaleX)
		{
			_scale.x = ScaleX;
			base.transform.localScale = _scale;
		}
		if (ControlScaleY)
		{
			_scale.y = ScaleY;
			base.transform.localScale = _scale;
		}
		if (ControlScaleZ)
		{
			_scale.z = ScaleZ;
			base.transform.localScale = _scale;
		}
	}
}

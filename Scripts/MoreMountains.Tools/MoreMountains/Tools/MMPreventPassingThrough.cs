using UnityEngine;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Movement/MMPreventPassingThrough")]
public class MMPreventPassingThrough : MonoBehaviour
{
	public LayerMask ObstaclesLayerMask;

	public float SkinWidth = 0.1f;

	protected float _smallestBoundsWidth;

	protected float _adjustedSmallestBoundsWidth;

	protected float _squaredBoundsWidth;

	protected Vector3 _positionLastFrame;

	protected Rigidbody _rigidbody;

	protected Collider _collider;

	protected Vector3 _lastMovement;

	protected float _lastMovementSquared;

	protected virtual void Start()
	{
		Initialization();
	}

	protected virtual void Initialization()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_positionLastFrame = _rigidbody.position;
		_collider = GetComponent<Collider>();
		_smallestBoundsWidth = Mathf.Min(Mathf.Min(_collider.bounds.extents.x, _collider.bounds.extents.y), _collider.bounds.extents.z);
		_adjustedSmallestBoundsWidth = _smallestBoundsWidth * (1f - SkinWidth);
		_squaredBoundsWidth = _smallestBoundsWidth * _smallestBoundsWidth;
	}

	protected virtual void OnEnable()
	{
		_positionLastFrame = base.transform.position;
	}

	protected virtual void FixedUpdate()
	{
		_lastMovement = _rigidbody.position - _positionLastFrame;
		_lastMovementSquared = _lastMovement.sqrMagnitude;
		if (_lastMovementSquared > _squaredBoundsWidth)
		{
			float num = Mathf.Sqrt(_lastMovementSquared);
			if (Physics.Raycast(_positionLastFrame, _lastMovement, out var hitInfo, num, ObstaclesLayerMask.value))
			{
				if (!hitInfo.collider)
				{
					return;
				}
				if (hitInfo.collider.isTrigger)
				{
					hitInfo.collider.SendMessage("OnTriggerEnter", _collider);
				}
				if (!hitInfo.collider.isTrigger)
				{
					_rigidbody.position = hitInfo.point - _lastMovement / num * _adjustedSmallestBoundsWidth;
				}
			}
		}
		_positionLastFrame = _rigidbody.position;
	}
}

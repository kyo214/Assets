using UnityEngine;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Movement/MMPreventPassingThrough2D")]
public class MMPreventPassingThrough2D : MonoBehaviour
{
	public LayerMask ObstaclesLayerMask;

	public float SkinWidth = 0.1f;

	public bool RepositionRigidbody = true;

	[Header("Debug")]
	[MMReadOnly]
	public RaycastHit2D Hit;

	protected float _smallestBoundsWidth;

	protected float _adjustedSmallestBoundsWidth;

	protected float _squaredBoundsWidth;

	protected Vector3 _positionLastFrame;

	protected Rigidbody2D _rigidbody;

	protected Collider2D _collider;

	protected Vector2 _lastMovement;

	protected float _lastMovementSquared;

	protected virtual void Start()
	{
		Initialization();
	}

	protected virtual void Initialization()
	{
		_rigidbody = GetComponent<Rigidbody2D>();
		_positionLastFrame = _rigidbody.position;
		_collider = GetComponent<Collider2D>();
		_smallestBoundsWidth = Mathf.Min(Mathf.Min(_collider.bounds.extents.x, _collider.bounds.extents.y), _collider.bounds.extents.z);
		_adjustedSmallestBoundsWidth = _smallestBoundsWidth * (1f - SkinWidth);
		_squaredBoundsWidth = _smallestBoundsWidth * _smallestBoundsWidth;
	}

	protected virtual void OnEnable()
	{
		_positionLastFrame = base.transform.position;
	}

	protected virtual void Update()
	{
		_lastMovement = base.transform.position - _positionLastFrame;
		_lastMovementSquared = _lastMovement.sqrMagnitude;
		if (_lastMovementSquared > _squaredBoundsWidth)
		{
			float num = Mathf.Sqrt(_lastMovementSquared);
			RaycastHit2D hit = MMDebug.RayCast(_positionLastFrame, _lastMovement.normalized, num, ObstaclesLayerMask, Color.blue, drawGizmo: true);
			if (hit.collider != null)
			{
				if (hit.collider.isTrigger)
				{
					hit.collider.SendMessage("OnTriggerEnter2D", _collider, SendMessageOptions.DontRequireReceiver);
				}
				if (!hit.collider.isTrigger)
				{
					Hit = hit;
					base.gameObject.SendMessage("PreventedCollision2D", Hit, SendMessageOptions.DontRequireReceiver);
					if (RepositionRigidbody)
					{
						base.transform.position = hit.point - _lastMovement / num * _adjustedSmallestBoundsWidth;
						_rigidbody.position = hit.point - _lastMovement / num * _adjustedSmallestBoundsWidth;
					}
				}
			}
		}
		_positionLastFrame = base.transform.position;
	}
}

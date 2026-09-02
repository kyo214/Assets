using Cinemachine;
using UnityEngine;

namespace MoreMountains.Tools;

[RequireComponent(typeof(Collider2D))]
public class MMCinemachineZone2D : MMCinemachineZone
{
	protected Collider2D _collider2D;

	protected Collider2D _confinerCollider2D;

	protected Rigidbody2D _confinerRigidbody2D;

	protected CompositeCollider2D _confinerCompositeCollider2D;

	protected BoxCollider2D _boxCollider2D;

	protected CircleCollider2D _circleCollider2D;

	protected PolygonCollider2D _polygonCollider2D;

	protected CinemachineConfiner _cinemachineConfiner;

	protected override void InitializeCollider()
	{
		_collider2D = GetComponent<Collider2D>();
		_boxCollider2D = GetComponent<BoxCollider2D>();
		_circleCollider2D = GetComponent<CircleCollider2D>();
		_polygonCollider2D = GetComponent<PolygonCollider2D>();
		_collider2D.isTrigger = true;
	}

	protected override void SetupConfiner()
	{
		_confinerRigidbody2D = _confinerGameObject.AddComponent<Rigidbody2D>();
		_confinerRigidbody2D.bodyType = RigidbodyType2D.Static;
		_confinerRigidbody2D.simulated = false;
		_confinerRigidbody2D.useAutoMass = true;
		_confinerRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
		CopyCollider();
		_confinerGameObject.transform.localPosition = Vector3.zero;
		_confinerRigidbody2D.bodyType = RigidbodyType2D.Static;
		_confinerRigidbody2D.useAutoMass = false;
		_confinerCompositeCollider2D = _confinerGameObject.AddComponent<CompositeCollider2D>();
		_confinerCompositeCollider2D.geometryType = CompositeCollider2D.GeometryType.Polygons;
		_cinemachineConfiner = VirtualCamera.gameObject.MMGetComponentAroundOrAdd<CinemachineConfiner>();
		_cinemachineConfiner.m_ConfineMode = CinemachineConfiner.Mode.Confine2D;
		_cinemachineConfiner.m_ConfineScreenEdges = true;
		_cinemachineConfiner.m_BoundingShape2D = _confinerCompositeCollider2D;
	}

	protected virtual void CopyCollider()
	{
		if (_boxCollider2D != null)
		{
			BoxCollider2D boxCollider2D = _confinerGameObject.AddComponent<BoxCollider2D>();
			boxCollider2D.size = _boxCollider2D.size;
			boxCollider2D.offset = _boxCollider2D.offset;
			boxCollider2D.usedByComposite = true;
			boxCollider2D.isTrigger = true;
		}
		if (_circleCollider2D != null)
		{
			CircleCollider2D circleCollider2D = _confinerGameObject.AddComponent<CircleCollider2D>();
			circleCollider2D.isTrigger = true;
			circleCollider2D.usedByComposite = true;
			circleCollider2D.offset = _circleCollider2D.offset;
			circleCollider2D.radius = _circleCollider2D.radius;
		}
		if (_polygonCollider2D != null)
		{
			PolygonCollider2D polygonCollider2D = _confinerGameObject.AddComponent<PolygonCollider2D>();
			polygonCollider2D.isTrigger = true;
			polygonCollider2D.usedByComposite = true;
			polygonCollider2D.offset = _polygonCollider2D.offset;
			polygonCollider2D.points = _polygonCollider2D.points;
		}
	}

	protected virtual void OnTriggerEnter2D(Collider2D collider)
	{
		if (TriggerMask.MMContains(collider.gameObject))
		{
			StartCoroutine(EnableCamera(state: true, 0));
			OnEnterZoneEvent.Invoke();
		}
	}

	protected virtual void OnTriggerExit2D(Collider2D collider)
	{
		if (TriggerMask.MMContains(collider.gameObject))
		{
			StartCoroutine(EnableCamera(state: false, 0));
			OnExitZoneEvent.Invoke();
		}
	}
}

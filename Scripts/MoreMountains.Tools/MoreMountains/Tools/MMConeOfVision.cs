using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools;

[Serializable]
[AddComponentMenu("More Mountains/Tools/Vision/MMConeOfVision")]
public class MMConeOfVision : MonoBehaviour
{
	public struct RaycastData(bool hit, Vector3 point, float distance, float angle)
	{
		public bool Hit = hit;

		public Vector3 Point = point;

		public float Distance = distance;

		public float Angle = angle;
	}

	public struct MeshEdgePosition(Vector3 pointA, Vector3 pointB)
	{
		public Vector3 PointA = pointA;

		public Vector3 PointB = pointB;
	}

	[Header("Vision")]
	public LayerMask ObstacleMask;

	public float VisionRadius = 5f;

	[Range(0f, 360f)]
	public float VisionAngle = 20f;

	[MMReadOnly]
	public Vector3 Direction;

	[MMReadOnly]
	public Vector3 EulerAngles;

	public Vector3 Offset;

	[Header("Target scanning")]
	public bool ShouldScanForTargets = true;

	public LayerMask TargetMask;

	public float ScanFrequencyInSeconds = 1f;

	[MMReadOnly]
	public List<Transform> VisibleTargets = new List<Transform>();

	[Header("Mesh")]
	public bool ShouldDrawMesh = true;

	public float MeshDensity = 0.2f;

	public int EdgePrecision = 3;

	public float EdgeThreshold = 0.5f;

	public MeshFilter VisionMeshFilter;

	protected Mesh _visionMesh;

	protected Collider[] _targetsWithinDistance;

	protected Transform _target;

	protected Vector3 _directionToTarget;

	protected float _distanceToTarget;

	protected float _lastScanTimestamp;

	protected List<Vector3> _viewPoints = new List<Vector3>();

	protected RaycastData _oldViewCast;

	protected RaycastData _viewCast;

	protected Vector3[] _vertices;

	protected int[] _triangles;

	protected Vector3 _minPoint;

	protected Vector3 _maxPoint;

	protected Vector3 _direction;

	protected RaycastData _returnRaycastData;

	protected RaycastHit _raycastAtAngleHit;

	protected int _numberOfVerticesLastTime;

	public Vector3 Center => base.transform.position + Offset;

	protected virtual void Awake()
	{
		_visionMesh = new Mesh();
		if (ShouldDrawMesh)
		{
			VisionMeshFilter.mesh = _visionMesh;
		}
	}

	protected virtual void LateUpdate()
	{
		if (Time.time - _lastScanTimestamp > ScanFrequencyInSeconds && ShouldScanForTargets)
		{
			ScanForTargets();
		}
		DrawMesh();
	}

	public virtual void SetDirectionAndAngles(Vector3 direction, Vector3 eulerAngles)
	{
		Direction = direction;
		EulerAngles = eulerAngles;
	}

	protected virtual void ScanForTargets()
	{
		_lastScanTimestamp = Time.time;
		VisibleTargets.Clear();
		_targetsWithinDistance = Physics.OverlapSphere(Center, VisionRadius, TargetMask);
		Collider[] targetsWithinDistance = _targetsWithinDistance;
		foreach (Collider collider in targetsWithinDistance)
		{
			_target = collider.transform;
			_directionToTarget = (_target.position - Center).normalized;
			if (!(Vector3.Angle(Direction, _directionToTarget) < VisionAngle / 2f))
			{
				continue;
			}
			_distanceToTarget = Vector3.Distance(Center, _target.position);
			bool flag = false;
			foreach (Transform visibleTarget in VisibleTargets)
			{
				if (visibleTarget == _target)
				{
					flag = true;
				}
			}
			if (!Physics.Raycast(Center, _directionToTarget, _distanceToTarget, ObstacleMask) && !flag)
			{
				VisibleTargets.Add(_target);
			}
		}
	}

	protected virtual void DrawMesh()
	{
		if (!ShouldDrawMesh)
		{
			return;
		}
		int num = Mathf.RoundToInt(MeshDensity * VisionAngle);
		float num2 = VisionAngle / (float)num;
		_viewPoints.Clear();
		for (int i = 0; i <= num; i++)
		{
			float angle = num2 * (float)i + EulerAngles.y - VisionAngle / 2f;
			_viewCast = RaycastAtAngle(angle);
			if (i > 0)
			{
				bool flag = Mathf.Abs(_oldViewCast.Distance - _viewCast.Distance) > EdgeThreshold;
				if (_oldViewCast.Hit != _viewCast.Hit || ((_oldViewCast.Hit && _viewCast.Hit) & flag))
				{
					MeshEdgePosition meshEdgePosition = FindMeshEdgePosition(_oldViewCast, _viewCast);
					if (meshEdgePosition.PointA != Vector3.zero)
					{
						_viewPoints.Add(meshEdgePosition.PointA);
					}
					if (meshEdgePosition.PointB != Vector3.zero)
					{
						_viewPoints.Add(meshEdgePosition.PointB);
					}
				}
			}
			_viewPoints.Add(_viewCast.Point);
			_oldViewCast = _viewCast;
		}
		int num3 = _viewPoints.Count + 1;
		if (num3 != _numberOfVerticesLastTime)
		{
			Array.Resize(ref _vertices, num3);
			Array.Resize(ref _triangles, (num3 - 2) * 3);
		}
		_vertices[0] = Offset;
		for (int j = 0; j < num3 - 1; j++)
		{
			_vertices[j + 1] = base.transform.InverseTransformPoint(_viewPoints[j]);
			if (j < num3 - 2)
			{
				_triangles[j * 3] = 0;
				_triangles[j * 3 + 1] = j + 1;
				_triangles[j * 3 + 2] = j + 2;
			}
		}
		_visionMesh.Clear();
		_visionMesh.vertices = _vertices;
		_visionMesh.triangles = _triangles;
		_visionMesh.RecalculateNormals();
		_numberOfVerticesLastTime = num3;
	}

	private MeshEdgePosition FindMeshEdgePosition(RaycastData minimumViewCast, RaycastData maximumViewCast)
	{
		float num = minimumViewCast.Angle;
		float num2 = maximumViewCast.Angle;
		_minPoint = minimumViewCast.Point;
		_maxPoint = maximumViewCast.Point;
		for (int i = 0; i < EdgePrecision; i++)
		{
			float num3 = (num + num2) / 2f;
			RaycastData raycastData = RaycastAtAngle(num3);
			bool flag = Mathf.Abs(minimumViewCast.Distance - raycastData.Distance) > EdgeThreshold;
			if (raycastData.Hit == minimumViewCast.Hit && !flag)
			{
				num = num3;
				_minPoint = raycastData.Point;
			}
			else
			{
				num2 = num3;
				_maxPoint = raycastData.Point;
			}
		}
		return new MeshEdgePosition(_minPoint, _maxPoint);
	}

	private RaycastData RaycastAtAngle(float angle)
	{
		_direction = MMMaths.DirectionFromAngle(angle, 0f);
		if (Physics.Raycast(Center, _direction, out _raycastAtAngleHit, VisionRadius, ObstacleMask))
		{
			_returnRaycastData.Hit = true;
			_returnRaycastData.Point = _raycastAtAngleHit.point;
			_returnRaycastData.Distance = _raycastAtAngleHit.distance;
			_returnRaycastData.Angle = angle;
		}
		else
		{
			_returnRaycastData.Hit = false;
			_returnRaycastData.Point = Center + _direction * VisionRadius;
			_returnRaycastData.Distance = VisionRadius;
			_returnRaycastData.Angle = angle;
		}
		return _returnRaycastData;
	}
}

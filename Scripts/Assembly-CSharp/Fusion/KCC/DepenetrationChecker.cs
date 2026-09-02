using System;
using UnityEngine;

namespace Fusion.KCC;

public sealed class DepenetrationChecker : MonoBehaviour
{
	[SerializeField]
	private Transform _origin;

	[SerializeField]
	private Transform _depenetrated;

	[SerializeField]
	private Collider _collider;

	[SerializeField]
	private float _radius = 0.35f;

	[SerializeField]
	private float _height = 1.8f;

	[SerializeField]
	private float _extent = 0.035f;

	[SerializeField]
	private float _maxGroundAngle = 75f;

	[SerializeField]
	private float _maxWallAngle = 5f;

	[SerializeField]
	private LayerMask _collisionLayerMask = 1;

	[SerializeField]
	private int _subSteps = 3;

	[SerializeField]
	private Vector3 _dynamicVelocity;

	private KCCData _data = new KCCData();

	private Collider[] _hitColliders = new Collider[64];

	private KCCOverlapInfo _overlapInfo = new KCCOverlapInfo(64);

	private KCCResolver _resolver = new KCCResolver(64);

	private void Update()
	{
		Vector3 position = _origin.position;
		Vector3 position2 = base.transform.position;
		KCCData data = _data;
		data.BasePosition = position;
		data.DesiredPosition = position2;
		data.TargetPosition = position2;
		data.DynamicVelocity = _dynamicVelocity;
		data.MaxGroundAngle = _maxGroundAngle;
		data.MaxWallAngle = _maxWallAngle;
		data.WasGrounded = data.IsGrounded;
		data.WasSteppingUp = data.IsSteppingUp;
		data.WasSnappingToGround = data.IsSnappingToGround;
		data.IsGrounded = false;
		data.IsSteppingUp = false;
		data.IsSnappingToGround = false;
		data.GroundNormal = default;
		data.GroundTangent = default;
		data.GroundPosition = default;
		data.GroundDistance = 0f;
		data.GroundAngle = 0f;
		OverlapCapsule(_overlapInfo, _data, data.TargetPosition, _radius, _height, _radius, _collisionLayerMask, QueryTriggerInteraction.Collide);
		_overlapInfo.ToggleConvexMeshColliders(convex: false);
		data.TargetPosition = DepenetrateColliders(_overlapInfo, data, data.BasePosition, data.TargetPosition, probeGrounding: true, probeSteppingUp: true, _subSteps);
		_overlapInfo.ToggleConvexMeshColliders(convex: true);
		if (!data.TargetPosition.IsEqual(position2))
		{
			Debug.DrawLine(position2, position2 + (data.TargetPosition - position2).normalized, Color.magenta);
			Debug.DrawLine(position2, data.TargetPosition, Color.green);
		}
		_depenetrated.position = data.TargetPosition;
	}

	private Vector3 DepenetrateColliders(KCCOverlapInfo overlapInfo, KCCData data, Vector3 basePosition, Vector3 targetPosition, bool probeGrounding, bool probeSteppingUp, int maxSubSteps)
	{
		if (overlapInfo.ColliderHitCount == 0)
		{
			return targetPosition;
		}
		if (overlapInfo.ColliderHitCount == 1)
		{
			return DepenetrateSingle(overlapInfo, data, basePosition, targetPosition, probeGrounding);
		}
		return DepenetrateMultiple(overlapInfo, data, basePosition, targetPosition, probeGrounding, maxSubSteps);
	}

	private Vector3 DepenetrateSingle(KCCOverlapInfo overlapInfo, KCCData data, Vector3 basePosition, Vector3 targetPosition, bool probeGrounding)
	{
		bool flag = false;
		float num = 0f;
		Vector3 vector = Vector3.up;
		float num2 = 0f;
		KCCOverlapHit kCCOverlapHit = overlapInfo.ColliderHits[0];
		kCCOverlapHit.HasPenetration = Physics.ComputePenetration(_collider, targetPosition, Quaternion.identity, kCCOverlapHit.Collider, kCCOverlapHit.Transform.position, kCCOverlapHit.Transform.rotation, out var direction, out var distance);
		if (kCCOverlapHit.HasPenetration)
		{
			Debug.DrawLine(kCCOverlapHit.Transform.position, kCCOverlapHit.Transform.position + direction, Color.yellow);
			Debug.DrawLine(kCCOverlapHit.Transform.position, kCCOverlapHit.Transform.position + direction * distance, Color.red);
			kCCOverlapHit.IsWithinExtent = true;
			flag = true;
			num = Mathf.Cos(Mathf.Clamp(data.MaxGroundAngle, 0f, 90f) * (MathF.PI / 180f));
			float num3 = Vector3.Dot(direction, Vector3.up);
			if (num3 >= num)
			{
				kCCOverlapHit.CollisionType = ECollisionType.Ground;
				data.IsGrounded = true;
				vector = direction;
			}
			else
			{
				probeGrounding = false;
				float num4 = 0f - Mathf.Cos(Mathf.Clamp(90f - data.MaxWallAngle, 0f, 90f) * (MathF.PI / 180f));
				if (num3 > 0f - num4)
				{
					kCCOverlapHit.CollisionType = ECollisionType.Slope;
				}
				else if (num3 >= num4)
				{
					kCCOverlapHit.CollisionType = ECollisionType.Wall;
				}
				else
				{
					float num5 = 0f - Mathf.Cos(Mathf.Clamp(90f - data.MaxHangAngle, 0f, 90f) * (MathF.PI / 180f));
					if (num3 >= num5)
					{
						kCCOverlapHit.CollisionType = ECollisionType.Hang;
					}
					else
					{
						kCCOverlapHit.CollisionType = ECollisionType.Top;
					}
				}
				if (num3 > 0f && distance >= 1E-06f && data.DynamicVelocity.y <= 0f && Vector3.Dot((targetPosition - basePosition).OnlyXZ(), direction.OnlyXZ()) < 0f)
				{
					KCCPhysicsUtility.ProjectVerticalPenetration(ref direction, ref distance);
				}
			}
			targetPosition += direction * distance;
		}
		if (probeGrounding && !data.IsGrounded)
		{
			if (!flag)
			{
				num = Mathf.Cos(Mathf.Clamp(data.MaxGroundAngle, 0f, 90f) * (MathF.PI / 180f));
			}
			if (KCCPhysicsUtility.CheckGround(_collider, targetPosition, kCCOverlapHit.Collider, kCCOverlapHit.Transform.position, kCCOverlapHit.Transform.rotation, _radius, _height, _extent, num, out var groundNormal, out var groundDistance, out var isWithinExtent))
			{
				vector = groundNormal;
				num2 = groundDistance;
				data.IsGrounded = true;
				kCCOverlapHit.CollisionType = ECollisionType.Ground;
			}
			kCCOverlapHit.IsWithinExtent |= isWithinExtent;
		}
		if (data.IsGrounded)
		{
			data.GroundNormal = vector;
			data.GroundAngle = Vector3.Angle(vector, Vector3.up);
			data.GroundPosition = targetPosition + new Vector3(0f, _radius, 0f) - vector * (_radius + num2);
			data.GroundDistance = num2;
		}
		return targetPosition;
	}

	private Vector3 DepenetrateMultiple(KCCOverlapInfo overlapInfo, KCCData data, Vector3 basePosition, Vector3 targetPosition, bool probeGrounding, int maxSubSteps)
	{
		float num = Mathf.Cos(Mathf.Clamp(data.MaxGroundAngle, 0f, 90f) * (MathF.PI / 180f));
		float num2 = 0f - Mathf.Cos(Mathf.Clamp(90f - data.MaxWallAngle, 0f, 90f) * (MathF.PI / 180f));
		float num3 = 0f - Mathf.Cos(Mathf.Clamp(90f - data.MaxHangAngle, 0f, 90f) * (MathF.PI / 180f));
		int num4 = 0;
		float num5 = 0f;
		float num6 = 0f;
		Vector3 vector = default;
		Vector3 groundNormal = default;
		Vector3 lhs = (targetPosition - basePosition).OnlyXZ();
		_resolver.Reset();
		for (int i = 0; i < overlapInfo.ColliderHitCount; i++)
		{
			KCCOverlapHit kCCOverlapHit = overlapInfo.ColliderHits[i];
			kCCOverlapHit.HasPenetration = Physics.ComputePenetration(_collider, targetPosition, Quaternion.identity, kCCOverlapHit.Collider, kCCOverlapHit.Transform.position, kCCOverlapHit.Transform.rotation, out var direction, out var distance);
			if (!kCCOverlapHit.HasPenetration)
			{
				continue;
			}
			Debug.DrawLine(kCCOverlapHit.Transform.position, kCCOverlapHit.Transform.position + direction, Color.yellow);
			Debug.DrawLine(kCCOverlapHit.Transform.position, kCCOverlapHit.Transform.position + direction * distance, Color.red);
			kCCOverlapHit.IsWithinExtent = true;
			float num7 = Vector3.Dot(direction, Vector3.up);
			if (num7 >= num)
			{
				kCCOverlapHit.CollisionType = ECollisionType.Ground;
				data.IsGrounded = true;
				num4++;
				if (num7 >= num6)
				{
					num6 = num7;
					vector = direction;
				}
				groundNormal += direction * num7;
			}
			else
			{
				if (num7 > 0f - num2)
				{
					kCCOverlapHit.CollisionType = ECollisionType.Slope;
				}
				else if (num7 >= num2)
				{
					kCCOverlapHit.CollisionType = ECollisionType.Wall;
				}
				else if (num7 >= num3)
				{
					kCCOverlapHit.CollisionType = ECollisionType.Hang;
				}
				else
				{
					kCCOverlapHit.CollisionType = ECollisionType.Top;
				}
				if (num7 > 0f && distance >= 1E-06f && data.DynamicVelocity.y <= 0f && Vector3.Dot(lhs, direction.OnlyXZ()) < 0f)
				{
					KCCPhysicsUtility.ProjectVerticalPenetration(ref direction, ref distance);
				}
			}
			_resolver.AddCorrection(direction, distance);
		}
		int num8 = Mathf.Max(0, maxSubSteps);
		float num9 = 1f - (float)Mathf.Min(num8, 2) * 0.25f;
		if (_resolver.Size == 2)
		{
			_resolver.GetCorrection(0, out var direction2);
			_resolver.GetCorrection(1, out var direction3);
			if (Vector3.Dot(direction2, direction3) >= 0f)
			{
				targetPosition += _resolver.CalculateMinMax() * num9;
			}
			else
			{
				targetPosition += _resolver.CalculateBinary() * num9;
			}
		}
		else
		{
			targetPosition += _resolver.CalculateGradientDescent(12, 0.0001f) * num9;
		}
		while (num8 > 0)
		{
			num8--;
			_resolver.Reset();
			for (int j = 0; j < overlapInfo.ColliderHitCount; j++)
			{
				KCCOverlapHit kCCOverlapHit2 = overlapInfo.ColliderHits[j];
				if (!Physics.ComputePenetration(_collider, targetPosition, Quaternion.identity, kCCOverlapHit2.Collider, kCCOverlapHit2.Transform.position, kCCOverlapHit2.Transform.rotation, out var direction4, out var distance2))
				{
					continue;
				}
				float num10 = Vector3.Dot(direction4, Vector3.up);
				if (!kCCOverlapHit2.HasPenetration)
				{
					Debug.DrawLine(kCCOverlapHit2.Transform.position, kCCOverlapHit2.Transform.position + direction4, Color.yellow);
					Debug.DrawLine(kCCOverlapHit2.Transform.position, kCCOverlapHit2.Transform.position + direction4 * distance2, Color.red);
					if (num10 >= num)
					{
						kCCOverlapHit2.CollisionType = ECollisionType.Ground;
						data.IsGrounded = true;
						num4++;
						if (num10 >= num6)
						{
							num6 = num10;
							vector = direction4;
						}
						groundNormal += direction4 * num10;
					}
					else if (num10 > 0f - num2)
					{
						kCCOverlapHit2.CollisionType = ECollisionType.Slope;
					}
					else if (num10 >= num2)
					{
						kCCOverlapHit2.CollisionType = ECollisionType.Wall;
					}
					else if (num10 >= num3)
					{
						kCCOverlapHit2.CollisionType = ECollisionType.Hang;
					}
					else
					{
						kCCOverlapHit2.CollisionType = ECollisionType.Top;
					}
				}
				kCCOverlapHit2.HasPenetration = true;
				kCCOverlapHit2.IsWithinExtent = true;
				if (num10 < num && num10 > 0f && distance2 >= 1E-06f && data.DynamicVelocity.y <= 0f && Vector3.Dot(lhs, direction4.OnlyXZ()) < 0f)
				{
					KCCPhysicsUtility.ProjectVerticalPenetration(ref direction4, ref distance2);
				}
				_resolver.AddCorrection(direction4, distance2);
			}
			if (_resolver.Size == 0)
			{
				break;
			}
			switch (num8)
			{
			case 0:
				if (_resolver.Size == 2)
				{
					_resolver.GetCorrection(0, out var direction5);
					_resolver.GetCorrection(1, out var direction6);
					if (Vector3.Dot(direction5, direction6) >= 0f)
					{
						targetPosition += _resolver.CalculateGradientDescent(12, 0.0001f);
					}
					else
					{
						targetPosition += _resolver.CalculateBinary();
					}
				}
				else
				{
					targetPosition += _resolver.CalculateGradientDescent(12, 0.0001f);
				}
				break;
			case 1:
				targetPosition += _resolver.CalculateMinMax() * 0.75f;
				break;
			default:
				targetPosition += _resolver.CalculateMinMax() * 0.5f;
				break;
			}
		}
		if (probeGrounding && !data.IsGrounded)
		{
			Vector3 vector2 = Vector3.up;
			float num11 = 1000f;
			for (int k = 0; k < overlapInfo.ColliderHitCount; k++)
			{
				KCCOverlapHit kCCOverlapHit3 = overlapInfo.ColliderHits[k];
				if (KCCPhysicsUtility.CheckGround(_collider, targetPosition, kCCOverlapHit3.Collider, kCCOverlapHit3.Transform.position, kCCOverlapHit3.Transform.rotation, _radius, _height, _extent, num, out var groundNormal2, out var groundDistance, out var isWithinExtent))
				{
					data.IsGrounded = true;
					if (groundDistance < num11)
					{
						vector2 = groundNormal2;
						num11 = groundDistance;
					}
					kCCOverlapHit3.CollisionType = ECollisionType.Ground;
				}
				kCCOverlapHit3.IsWithinExtent |= isWithinExtent;
			}
			if (data.IsGrounded)
			{
				vector = vector2;
				groundNormal = vector2;
				num5 = num11;
				num4 = 1;
			}
		}
		if (data.IsGrounded)
		{
			if (num4 <= 1)
			{
				groundNormal = vector;
			}
			else
			{
				groundNormal.Normalize();
			}
			data.GroundNormal = groundNormal;
			data.GroundAngle = Vector3.Angle(data.GroundNormal, Vector3.up);
			data.GroundPosition = targetPosition + new Vector3(0f, _radius, 0f) - data.GroundNormal * (_radius + num5);
			data.GroundDistance = num5;
		}
		return targetPosition;
	}

	private bool OverlapCapsule(KCCOverlapInfo overlapInfo, KCCData data, Vector3 position, float radius, float height, float extent, LayerMask layerMask, QueryTriggerInteraction triggerInteraction)
	{
		overlapInfo.Reset(deep: false);
		overlapInfo.Position = position;
		overlapInfo.Radius = radius;
		overlapInfo.Height = height;
		overlapInfo.Extent = extent;
		overlapInfo.LayerMask = layerMask;
		overlapInfo.TriggerInteraction = triggerInteraction;
		Vector3 point = position + new Vector3(0f, height - radius, 0f);
		Vector3 point2 = position + new Vector3(0f, radius, 0f);
		Collider[] hitColliders = _hitColliders;
		int num = Physics.defaultPhysicsScene.OverlapCapsule(point2, point, radius + extent, hitColliders, layerMask, triggerInteraction);
		for (int i = 0; i < num; i++)
		{
			Collider collider = hitColliders[i];
			if (collider != _collider)
			{
				overlapInfo.AddHit(collider);
			}
		}
		return overlapInfo.AllHitCount > 0;
	}
}

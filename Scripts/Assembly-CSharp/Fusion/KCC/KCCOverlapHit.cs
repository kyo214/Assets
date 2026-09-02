using System;
using UnityEngine;

namespace Fusion.KCC;

public sealed class KCCOverlapHit
{
	public EColliderType Type;

	public Collider Collider;

	public Transform Transform;

	public bool IsConvex;

	public bool IsTrigger;

	public bool IsPrimitive;

	public bool IsConvertible;

	public bool IsWithinExtent;

	public bool HasPenetration;

	public float MaxPenetration;

	public float UpDirectionDot;

	public ECollisionType CollisionType;

	public Vector3 CachedPosition;

	public Quaternion CachedRotation;

	private static readonly Type SphereColliderType = typeof(SphereCollider);

	private static readonly Type CapsuleColliderType = typeof(CapsuleCollider);

	private static readonly Type BoxColliderType = typeof(BoxCollider);

	private static readonly Type MeshColliderType = typeof(MeshCollider);

	private static readonly Type TerrainColliderType = typeof(TerrainCollider);

	public bool IsValid()
	{
		return Type != EColliderType.None;
	}

	public bool Set(Collider collider)
	{
		Type type = collider.GetType();
		if (type == BoxColliderType)
		{
			Type = EColliderType.Box;
			IsConvex = true;
			IsPrimitive = true;
			IsConvertible = false;
		}
		else if (type == MeshColliderType)
		{
			MeshCollider meshCollider = (MeshCollider)collider;
			Type = EColliderType.Mesh;
			IsConvex = meshCollider.convex;
			IsPrimitive = false;
			IsConvertible = false;
			if (IsConvex)
			{
				Mesh sharedMesh = meshCollider.sharedMesh;
				IsConvertible = sharedMesh != null && sharedMesh.isReadable;
			}
		}
		else if (type == TerrainColliderType)
		{
			Type = EColliderType.Terrain;
			IsConvex = false;
			IsPrimitive = false;
			IsConvertible = false;
		}
		else if (type == SphereColliderType)
		{
			Type = EColliderType.Sphere;
			IsConvex = true;
			IsPrimitive = true;
			IsConvertible = false;
		}
		else
		{
			if (!(type == CapsuleColliderType))
			{
				return false;
			}
			Type = EColliderType.Capsule;
			IsConvex = true;
			IsPrimitive = true;
			IsConvertible = false;
		}
		Collider = collider;
		Transform = collider.transform;
		IsTrigger = collider.isTrigger;
		IsWithinExtent = false;
		HasPenetration = false;
		MaxPenetration = 0f;
		UpDirectionDot = 0f;
		CollisionType = ECollisionType.None;
		return true;
	}

	public void Reset()
	{
		Type = EColliderType.None;
		Collider = null;
		Transform = null;
	}

	public void CopyFromOther(KCCOverlapHit other)
	{
		Type = other.Type;
		Collider = other.Collider;
		Transform = other.Transform;
		IsConvex = other.IsConvex;
		IsTrigger = other.IsTrigger;
		IsPrimitive = other.IsPrimitive;
		IsConvertible = other.IsConvertible;
		IsWithinExtent = other.IsWithinExtent;
		HasPenetration = other.HasPenetration;
		MaxPenetration = other.MaxPenetration;
		UpDirectionDot = other.UpDirectionDot;
		CollisionType = other.CollisionType;
	}
}

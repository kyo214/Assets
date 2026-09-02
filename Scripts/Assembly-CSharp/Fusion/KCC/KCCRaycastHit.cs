using System;
using UnityEngine;

namespace Fusion.KCC;

public sealed class KCCRaycastHit
{
	public EColliderType Type;

	public Collider Collider;

	public Transform Transform;

	public bool IsTrigger;

	public bool IsPrimitive;

	public RaycastHit RaycastHit;

	private static readonly Type SphereColliderType = typeof(SphereCollider);

	private static readonly Type CapsuleColliderType = typeof(CapsuleCollider);

	private static readonly Type BoxColliderType = typeof(BoxCollider);

	private static readonly Type MeshColliderType = typeof(MeshCollider);

	private static readonly Type TerrainColliderType = typeof(TerrainCollider);

	public bool IsValid()
	{
		return Type != EColliderType.None;
	}

	public bool Set(RaycastHit raycastHit)
	{
		Collider collider = raycastHit.collider;
		Type type = collider.GetType();
		if (type == BoxColliderType)
		{
			Type = EColliderType.Box;
			IsPrimitive = true;
		}
		else if (type == MeshColliderType)
		{
			Type = EColliderType.Mesh;
			IsPrimitive = false;
		}
		else if (type == TerrainColliderType)
		{
			Type = EColliderType.Terrain;
			IsPrimitive = false;
		}
		else if (type == SphereColliderType)
		{
			Type = EColliderType.Sphere;
			IsPrimitive = true;
		}
		else
		{
			if (!(type == CapsuleColliderType))
			{
				return false;
			}
			Type = EColliderType.Capsule;
			IsPrimitive = true;
		}
		Collider = collider;
		Transform = collider.transform;
		IsTrigger = collider.isTrigger;
		RaycastHit = raycastHit;
		return true;
	}

	public void Reset()
	{
		Type = EColliderType.None;
		Collider = null;
		Transform = null;
		RaycastHit = default;
	}

	public void CopyFromOther(KCCRaycastHit other)
	{
		Type = other.Type;
		Collider = other.Collider;
		Transform = other.Transform;
		IsTrigger = other.IsTrigger;
		IsPrimitive = other.IsPrimitive;
		RaycastHit = other.RaycastHit;
	}
}

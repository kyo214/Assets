using UnityEngine;

namespace Fusion.KCC;

public sealed class KCCOverlapInfo
{
	public Vector3 Position;

	public float Radius;

	public float Height;

	public float Extent;

	public LayerMask LayerMask;

	public QueryTriggerInteraction TriggerInteraction;

	public KCCOverlapHit[] AllHits;

	public int AllHitCount;

	public KCCOverlapHit[] ColliderHits;

	public int ColliderHitCount;

	public KCCOverlapHit[] TriggerHits;

	public int TriggerHitCount;

	public KCCOverlapInfo(int maxHits)
	{
		AllHits = new KCCOverlapHit[maxHits];
		TriggerHits = new KCCOverlapHit[maxHits];
		ColliderHits = new KCCOverlapHit[maxHits];
		for (int i = 0; i < maxHits; i++)
		{
			AllHits[i] = new KCCOverlapHit();
		}
	}

	public void AddHit(Collider collider)
	{
		if (AllHitCount == AllHits.Length)
		{
			return;
		}
		KCCOverlapHit kCCOverlapHit = AllHits[AllHitCount];
		if (kCCOverlapHit.Set(collider))
		{
			AllHitCount++;
			if (kCCOverlapHit.IsTrigger)
			{
				TriggerHits[TriggerHitCount] = kCCOverlapHit;
				TriggerHitCount++;
			}
			else
			{
				ColliderHits[ColliderHitCount] = kCCOverlapHit;
				ColliderHitCount++;
			}
		}
	}

	public void ToggleConvexMeshColliders(bool convex)
	{
		for (int i = 0; i < ColliderHitCount; i++)
		{
			KCCOverlapHit kCCOverlapHit = ColliderHits[i];
			if (kCCOverlapHit.Type == EColliderType.Mesh && kCCOverlapHit.IsConvertible)
			{
				((MeshCollider)kCCOverlapHit.Collider).convex = convex;
			}
		}
	}

	public bool AllWithinExtent()
	{
		_ = AllHits;
		int i = 0;
		for (int allHitCount = AllHitCount; i < allHitCount; i++)
		{
			if (!AllHits[i].IsWithinExtent)
			{
				return false;
			}
		}
		return true;
	}

	public void Reset(bool deep)
	{
		Position = default;
		Radius = 0f;
		Height = 0f;
		Extent = 0f;
		LayerMask = default;
		TriggerInteraction = QueryTriggerInteraction.Collide;
		AllHitCount = 0;
		TriggerHitCount = 0;
		ColliderHitCount = 0;
		if (deep)
		{
			int i = 0;
			for (int num = AllHits.Length; i < num; i++)
			{
				AllHits[i].Reset();
			}
		}
	}

	public void CopyFromOther(KCCOverlapInfo other)
	{
		Position = other.Position;
		Radius = other.Radius;
		Height = other.Height;
		Extent = other.Extent;
		LayerMask = other.LayerMask;
		TriggerInteraction = other.TriggerInteraction;
		AllHitCount = other.AllHitCount;
		TriggerHitCount = 0;
		ColliderHitCount = 0;
		for (int i = 0; i < AllHitCount; i++)
		{
			KCCOverlapHit kCCOverlapHit = AllHits[i];
			kCCOverlapHit.CopyFromOther(other.AllHits[i]);
			if (kCCOverlapHit.IsTrigger)
			{
				TriggerHits[TriggerHitCount] = kCCOverlapHit;
				TriggerHitCount++;
			}
			else
			{
				ColliderHits[ColliderHitCount] = kCCOverlapHit;
				ColliderHitCount++;
			}
		}
	}
}

using UnityEngine;

namespace Fusion.KCC;

public sealed class KCCRaycastInfo
{
	public Vector3 Origin;

	public Vector3 Direction;

	public float MaxDistance;

	public float Radius;

	public LayerMask LayerMask;

	public QueryTriggerInteraction TriggerInteraction;

	public KCCRaycastHit[] AllHits;

	public int AllHitCount;

	public KCCRaycastHit[] ColliderHits;

	public int ColliderHitCount;

	public KCCRaycastHit[] TriggerHits;

	public int TriggerHitCount;

	public KCCRaycastInfo(int maxHits)
	{
		AllHits = new KCCRaycastHit[maxHits];
		TriggerHits = new KCCRaycastHit[maxHits];
		ColliderHits = new KCCRaycastHit[maxHits];
		for (int i = 0; i < maxHits; i++)
		{
			AllHits[i] = new KCCRaycastHit();
		}
	}

	public void AddHit(RaycastHit raycastHit)
	{
		if (AllHitCount == AllHits.Length)
		{
			return;
		}
		KCCRaycastHit kCCRaycastHit = AllHits[AllHitCount];
		if (kCCRaycastHit.Set(raycastHit))
		{
			AllHitCount++;
			if (kCCRaycastHit.IsTrigger)
			{
				TriggerHits[TriggerHitCount] = kCCRaycastHit;
				TriggerHitCount++;
			}
			else
			{
				ColliderHits[ColliderHitCount] = kCCRaycastHit;
				ColliderHitCount++;
			}
		}
	}

	public void Reset(bool deep)
	{
		Origin = default;
		Direction = default;
		MaxDistance = 0f;
		Radius = 0f;
		LayerMask = default;
		TriggerInteraction = QueryTriggerInteraction.Collide;
		AllHitCount = 0;
		ColliderHitCount = 0;
		TriggerHitCount = 0;
		if (deep)
		{
			int i = 0;
			for (int num = AllHits.Length; i < num; i++)
			{
				AllHits[i].Reset();
			}
		}
	}
}

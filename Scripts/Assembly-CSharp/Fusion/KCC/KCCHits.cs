using System.Collections.Generic;
using UnityEngine;

namespace Fusion.KCC;

public sealed class KCCHits
{
	public readonly List<KCCHit> All = new List<KCCHit>();

	private static readonly KCCFastStack<KCCHit> _pool = new KCCFastStack<KCCHit>(256, createInstances: true);

	public int Count => All.Count;

	public bool HasCollider(Collider collider)
	{
		int index;
		return Find(collider, out index) != null;
	}

	public KCCHit Add(KCCOverlapHit overlapHit)
	{
		KCCHit kCCHit = _pool.PopOrCreate();
		kCCHit.Collider = overlapHit.Collider;
		kCCHit.Transform = overlapHit.Transform;
		kCCHit.CollisionType = overlapHit.CollisionType;
		All.Add(kCCHit);
		return kCCHit;
	}

	public void CopyFromOther(KCCHits other)
	{
		int count = All.Count;
		int count2 = other.All.Count;
		if (count == count2)
		{
			if (count != 0)
			{
				for (int i = 0; i < count; i++)
				{
					KCCHit kCCHit = All[i];
					KCCHit kCCHit2 = other.All[i];
					kCCHit.Collider = kCCHit2.Collider;
					kCCHit.Transform = kCCHit2.Transform;
					kCCHit.CollisionType = kCCHit2.CollisionType;
				}
			}
		}
		else
		{
			Clear();
			for (int j = 0; j < count2; j++)
			{
				KCCHit kCCHit = _pool.PopOrCreate();
				KCCHit kCCHit2 = other.All[j];
				kCCHit.Collider = kCCHit2.Collider;
				kCCHit.Transform = kCCHit2.Transform;
				kCCHit.CollisionType = kCCHit2.CollisionType;
				All.Add(kCCHit);
			}
		}
	}

	public void Clear()
	{
		int i = 0;
		for (int count = All.Count; i < count; i++)
		{
			KCCHit kCCHit = All[i];
			kCCHit.Collider = null;
			kCCHit.Transform = null;
			kCCHit.CollisionType = ECollisionType.None;
			_pool.Push(kCCHit);
		}
		All.Clear();
	}

	private KCCHit Find(Collider collider, out int index)
	{
		int i = 0;
		for (int count = All.Count; i < count; i++)
		{
			KCCHit kCCHit = All[i];
			if ((object)kCCHit.Collider == collider)
			{
				index = i;
				return kCCHit;
			}
		}
		index = -1;
		return null;
	}
}

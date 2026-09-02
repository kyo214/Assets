using System.Collections.Generic;
using UnityEngine;

namespace Fusion.KCC;

public sealed class KCCIgnores
{
	public readonly List<KCCIgnore> All = new List<KCCIgnore>();

	private static readonly KCCFastStack<KCCIgnore> _pool = new KCCFastStack<KCCIgnore>(128, createInstances: true);

	public int Count => All.Count;

	public bool HasCollider(Collider collider)
	{
		int index;
		return Find(collider, out index) != null;
	}

	public KCCIgnore Add(NetworkObject networkObject, Collider collider, bool checkExisting)
	{
		KCCIgnore kCCIgnore = (checkExisting ? Find(collider, out var _) : null);
		if (kCCIgnore == null)
		{
			kCCIgnore = _pool.PopOrCreate();
			kCCIgnore.NetworkID = KCCNetworkID.GetNetworkID(networkObject);
			kCCIgnore.NetworkObject = networkObject;
			kCCIgnore.Collider = collider;
			All.Add(kCCIgnore);
		}
		return kCCIgnore;
	}

	public bool Add(NetworkObject networkObject, KCCNetworkID networkID)
	{
		if (networkObject == null)
		{
			return false;
		}
		KCCIgnore kCCIgnore = _pool.PopOrCreate();
		kCCIgnore.NetworkID = networkID;
		kCCIgnore.NetworkObject = networkObject;
		kCCIgnore.Collider = networkObject.GetComponentNoAlloc<Collider>();
		All.Add(kCCIgnore);
		return true;
	}

	public bool Remove(Collider collider)
	{
		KCCIgnore kCCIgnore = Find(collider, out var index);
		if (kCCIgnore != null)
		{
			All.RemoveAt(index);
			kCCIgnore.Clear();
			_pool.Push(kCCIgnore);
			return true;
		}
		return false;
	}

	public void CopyFromOther(KCCIgnores other)
	{
		int count = All.Count;
		int count2 = other.All.Count;
		if (count == count2)
		{
			if (count != 0)
			{
				for (int i = 0; i < count; i++)
				{
					All[i].CopyFromOther(other.All[i]);
				}
			}
			return;
		}
		Clear();
		for (int j = 0; j < count2; j++)
		{
			KCCIgnore kCCIgnore = _pool.PopOrCreate();
			kCCIgnore.CopyFromOther(other.All[j]);
			All.Add(kCCIgnore);
		}
	}

	public void Clear()
	{
		int i = 0;
		for (int count = All.Count; i < count; i++)
		{
			KCCIgnore kCCIgnore = All[i];
			kCCIgnore.Clear();
			_pool.Push(kCCIgnore);
		}
		All.Clear();
	}

	private KCCIgnore Find(Collider collider, out int index)
	{
		int i = 0;
		for (int count = All.Count; i < count; i++)
		{
			KCCIgnore kCCIgnore = All[i];
			if ((object)kCCIgnore.Collider == collider)
			{
				index = i;
				return kCCIgnore;
			}
		}
		index = -1;
		return null;
	}
}

using System.Collections.Generic;
using UnityEngine;

namespace Fusion.KCC;

public sealed class KCCCollisions : KCCInteractions<KCCCollision>
{
	public bool HasCollider(Collider collider)
	{
		int index;
		return Find(collider, out index) != null;
	}

	public bool HasProcessor<T>() where T : class
	{
		int i = 0;
		for (int count = All.Count; i < count; i++)
		{
			if (All[i].Processor is T)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasProcessor<T>(T processor) where T : Component, IKCCProcessor
	{
		int i = 0;
		for (int count = All.Count; i < count; i++)
		{
			if (All[i].Processor == processor)
			{
				return true;
			}
		}
		return false;
	}

	public T GetProcessor<T>() where T : class
	{
		int i = 0;
		for (int count = All.Count; i < count; i++)
		{
			if (All[i].Processor is T result)
			{
				return result;
			}
		}
		return null;
	}

	public void GetProcessors<T>(List<T> processors, bool clearList = true) where T : class
	{
		if (clearList)
		{
			processors.Clear();
		}
		int i = 0;
		for (int count = All.Count; i < count; i++)
		{
			if (All[i].Processor is T item)
			{
				processors.Add(item);
			}
		}
	}

	public KCCCollision Add(NetworkObject networkObject, IKCCInteractionProvider provider, Collider collider)
	{
		KCCCollision fromPool = KCCInteractions<KCCCollision>.GetFromPool();
		fromPool.Collider = collider;
		fromPool.Processor = ((provider is IKCCProcessorProvider iKCCProcessorProvider) ? iKCCProcessorProvider.GetProcessor() : null);
		AddInternal(fromPool, networkObject, provider, invokeInitialize: false);
		return fromPool;
	}

	private KCCCollision Find(Collider collider, out int index)
	{
		int i = 0;
		for (int count = All.Count; i < count; i++)
		{
			KCCCollision kCCCollision = All[i];
			if ((object)kCCCollision.Collider == collider)
			{
				index = i;
				return kCCCollision;
			}
		}
		index = -1;
		return null;
	}
}

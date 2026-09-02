using System.Collections.Generic;
using UnityEngine;

namespace Fusion.KCC;

public sealed class KCCModifiers : KCCInteractions<KCCModifier>
{
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
}

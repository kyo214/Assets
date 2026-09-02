using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMAutoExecution : MonoBehaviour
{
	public List<MMAutoExecutionItem> Events;

	protected virtual void Awake()
	{
		foreach (MMAutoExecutionItem @event in Events)
		{
			if (@event.AutoExecuteOnAwake && @event.Event != null)
			{
				@event.Event.Invoke();
			}
		}
	}

	protected virtual void Start()
	{
		foreach (MMAutoExecutionItem @event in Events)
		{
			if (@event.AutoExecuteOnStart && @event.Event != null)
			{
				@event.Event.Invoke();
			}
		}
	}

	protected virtual void OnEnable()
	{
		foreach (MMAutoExecutionItem @event in Events)
		{
			if (@event.AutoExecuteOnEnable && @event.Event != null)
			{
				@event.Event.Invoke();
			}
		}
	}

	protected virtual void OnDisable()
	{
		foreach (MMAutoExecutionItem @event in Events)
		{
			if (@event.AutoExecuteOnDisable && @event.Event != null)
			{
				@event.Event.Invoke();
			}
		}
	}

	protected virtual void OnInstantiate()
	{
		foreach (MMAutoExecutionItem @event in Events)
		{
			if (@event.AutoExecuteOnInstantiate && @event.Event != null)
			{
				@event.Event.Invoke();
			}
		}
	}
}

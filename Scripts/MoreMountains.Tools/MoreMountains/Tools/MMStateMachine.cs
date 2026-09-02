using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools;

public class MMStateMachine<T> : MMIStateMachine where T : struct, IComparable, IConvertible, IFormattable
{
	public delegate void OnStateChangeDelegate();

	public GameObject Target;

	public OnStateChangeDelegate OnStateChange;

	public bool TriggerEvents { get; set; }

	public T CurrentState { get; protected set; }

	public T PreviousState { get; protected set; }

	public MMStateMachine(GameObject target, bool triggerEvents)
	{
		Target = target;
		TriggerEvents = triggerEvents;
	}

	public virtual void ChangeState(T newState)
	{
		if (!EqualityComparer<T>.Default.Equals(newState, CurrentState))
		{
			PreviousState = CurrentState;
			CurrentState = newState;
			OnStateChange?.Invoke();
			if (TriggerEvents)
			{
				MMEventManager.TriggerEvent(new MMStateChangeEvent<T>(this));
			}
		}
	}

	public virtual void RestorePreviousState()
	{
		CurrentState = PreviousState;
		OnStateChange?.Invoke();
		if (TriggerEvents)
		{
			MMEventManager.TriggerEvent(new MMStateChangeEvent<T>(this));
		}
	}
}

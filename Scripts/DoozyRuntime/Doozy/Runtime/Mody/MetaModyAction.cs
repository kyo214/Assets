using System;
using Doozy.Runtime.Signals;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody;

[Serializable]
public abstract class MetaModyAction<T> : ModyAction
{
	public T ActionValue;

	public UnityAction<T> actionCallback { get; private set; }

	protected MetaModyAction(MonoBehaviour behaviour, string actionName, UnityAction<T> callback)
		: base(behaviour, actionName)
	{
		actionCallback = callback;
		HasValue = true;
		ValueType = typeof(T);
		IgnoreSignalValue = false;
		ReactToAnySignal = false;
	}

	protected override void Run(Signal signal)
	{
		if (ReactToAnySignal)
		{
			if (IgnoreSignalValue)
			{
				actionCallback?.Invoke(ActionValue);
				return;
			}
			if (signal != null && signal.valueType == ValueType)
			{
				ActionValue = signal.GetValueUnsafe<T>();
			}
			actionCallback?.Invoke(ActionValue);
		}
		else if (signal != null && signal.hasValue && !(signal.valueType != ValueType))
		{
			if (IgnoreSignalValue)
			{
				actionCallback?.Invoke(ActionValue);
				return;
			}
			ActionValue = signal.GetValueUnsafe<T>();
			actionCallback?.Invoke(ActionValue);
		}
	}

	public void SetValue(T value)
	{
		ActionValue = value;
	}

	public override bool SetValue(object objectValue)
	{
		return SetValue(objectValue, restrictValueType: true);
	}

	internal override bool SetValue(object objectValue, bool restrictValueType)
	{
		if (objectValue == null)
		{
			return false;
		}
		if (restrictValueType && objectValue.GetType() != ValueType)
		{
			return false;
		}
		try
		{
			SetValue((T)objectValue);
			return true;
		}
		catch
		{
			return false;
		}
	}
}

using System;
using Doozy.Runtime.Signals;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody.Actions;

[Serializable]
public class SimpleModyAction : ModyAction
{
	public UnityAction actionCallback { get; private set; }

	public SimpleModyAction(MonoBehaviour behaviour, string actionName, UnityAction callback)
		: base(behaviour, actionName)
	{
		actionCallback = callback;
	}

	protected override void Run(Signal signal)
	{
		actionCallback?.Invoke();
	}

	public override bool SetValue(object objectValue)
	{
		return false;
	}

	internal override bool SetValue(object objectValue, bool restrictValueType)
	{
		return false;
	}
}

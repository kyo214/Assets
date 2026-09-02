using System;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody.Actions;

[Serializable]
public class IntModyAction : MetaModyAction<int>
{
	public IntModyAction(MonoBehaviour behaviour, string actionName, UnityAction<int> callback)
		: base(behaviour, actionName, callback)
	{
	}
}

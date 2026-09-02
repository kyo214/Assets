using System;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody.Actions;

[Serializable]
public class BoolModyAction : MetaModyAction<bool>
{
	public BoolModyAction(MonoBehaviour behaviour, string actionName, UnityAction<bool> callback)
		: base(behaviour, actionName, callback)
	{
	}
}

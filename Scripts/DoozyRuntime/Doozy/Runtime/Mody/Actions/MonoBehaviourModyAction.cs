using System;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody.Actions;

[Serializable]
public class MonoBehaviourModyAction : MetaModyAction<MonoBehaviour>
{
	public MonoBehaviourModyAction(MonoBehaviour behaviour, string actionName, UnityAction<MonoBehaviour> callback)
		: base(behaviour, actionName, callback)
	{
	}
}

using System;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody.Actions;

[Serializable]
public class LongModyAction : MetaModyAction<long>
{
	public LongModyAction(MonoBehaviour behaviour, string actionName, UnityAction<long> callback)
		: base(behaviour, actionName, callback)
	{
	}
}

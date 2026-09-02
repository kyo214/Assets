using System;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody.Actions;

[Serializable]
public class DoubleModyAction : MetaModyAction<double>
{
	public DoubleModyAction(MonoBehaviour behaviour, string actionName, UnityAction<double> callback)
		: base(behaviour, actionName, callback)
	{
	}
}

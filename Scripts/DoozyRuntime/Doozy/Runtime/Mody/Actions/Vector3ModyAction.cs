using System;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody.Actions;

[Serializable]
public class Vector3ModyAction : MetaModyAction<Vector3>
{
	public Vector3ModyAction(MonoBehaviour behaviour, string actionName, UnityAction<Vector3> callback)
		: base(behaviour, actionName, callback)
	{
	}
}

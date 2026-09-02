using System;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody.Actions;

[Serializable]
public class Vector4ModyAction : MetaModyAction<Vector4>
{
	public Vector4ModyAction(MonoBehaviour behaviour, string actionName, UnityAction<Vector4> callback)
		: base(behaviour, actionName, callback)
	{
	}
}

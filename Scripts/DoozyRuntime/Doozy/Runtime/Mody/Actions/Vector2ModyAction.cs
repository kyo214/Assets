using System;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody.Actions;

[Serializable]
public class Vector2ModyAction : MetaModyAction<Vector2>
{
	public Vector2ModyAction(MonoBehaviour behaviour, string actionName, UnityAction<Vector2> callback)
		: base(behaviour, actionName, callback)
	{
	}
}

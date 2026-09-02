using System;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody.Actions;

[Serializable]
public class GameObjectModyAction : MetaModyAction<GameObject>
{
	public GameObjectModyAction(MonoBehaviour behaviour, string actionName, UnityAction<GameObject> callback)
		: base(behaviour, actionName, callback)
	{
	}
}

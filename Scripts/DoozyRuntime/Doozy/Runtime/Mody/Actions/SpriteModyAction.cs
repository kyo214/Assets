using System;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody.Actions;

[Serializable]
public class SpriteModyAction : MetaModyAction<Sprite>
{
	public SpriteModyAction(MonoBehaviour behaviour, string actionName, UnityAction<Sprite> callback)
		: base(behaviour, actionName, callback)
	{
	}
}

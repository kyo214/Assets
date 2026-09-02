using System;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody.Actions;

[Serializable]
public class Texture2DModyAction : MetaModyAction<Texture2D>
{
	public Texture2DModyAction(MonoBehaviour behaviour, string actionName, UnityAction<Texture2D> callback)
		: base(behaviour, actionName, callback)
	{
	}
}

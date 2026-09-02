using System;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody.Actions;

[Serializable]
public class TextureModyAction : MetaModyAction<Texture>
{
	public TextureModyAction(MonoBehaviour behaviour, string actionName, UnityAction<Texture> callback)
		: base(behaviour, actionName, callback)
	{
	}
}

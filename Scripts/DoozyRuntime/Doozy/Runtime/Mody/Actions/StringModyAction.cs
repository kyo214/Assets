using System;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody.Actions;

[Serializable]
public class StringModyAction : MetaModyAction<string>
{
	public StringModyAction(MonoBehaviour behaviour, string actionName, UnityAction<string> callback)
		: base(behaviour, actionName, callback)
	{
	}
}

using System;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody.Actions;

[Serializable]
public class ColorModyAction : MetaModyAction<Color>
{
	public ColorModyAction(MonoBehaviour behaviour, string actionName, UnityAction<Color> callback)
		: base(behaviour, actionName, callback)
	{
	}
}

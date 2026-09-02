using System;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody.Actions;

[Serializable]
public class Color32ModyAction : MetaModyAction<Color32>
{
	public Color32ModyAction(MonoBehaviour behaviour, string actionName, UnityAction<Color32> callback)
		: base(behaviour, actionName, callback)
	{
	}
}

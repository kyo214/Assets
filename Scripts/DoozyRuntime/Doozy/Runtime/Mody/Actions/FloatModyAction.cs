using System;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody.Actions;

[Serializable]
public class FloatModyAction : MetaModyAction<float>
{
	public FloatModyAction(MonoBehaviour behaviour, string actionName, UnityAction<float> callback)
		: base(behaviour, actionName, callback)
	{
	}
}

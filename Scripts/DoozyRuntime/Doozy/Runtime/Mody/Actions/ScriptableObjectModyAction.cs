using System;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody.Actions;

[Serializable]
public class ScriptableObjectModyAction : MetaModyAction<ScriptableObject>
{
	public ScriptableObjectModyAction(MonoBehaviour behaviour, string actionName, UnityAction<ScriptableObject> callback)
		: base(behaviour, actionName, callback)
	{
	}

	public override bool SetValue(object objectValue)
	{
		return SetValue(objectValue, restrictValueType: false);
	}
}

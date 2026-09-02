using System;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody.Actions;

[Serializable]
public class GenericModyAction : MetaModyAction<UnityEngine.Object>
{
	public GenericModyAction(MonoBehaviour behaviour, string actionName, UnityAction<UnityEngine.Object> callback)
		: base(behaviour, actionName, callback)
	{
	}

	public override bool SetValue(object objectValue)
	{
		return SetValue(objectValue, restrictValueType: false);
	}
}

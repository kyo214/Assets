using System;
using UnityEngine;

namespace _Modules.DialogueSystem;

public abstract class DialogueCustomActionBase : ScriptableObject
{
	public abstract void Invoke(Action onCompleted);
}

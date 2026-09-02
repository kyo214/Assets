using System;
using UnityEngine.Events;

namespace MoreMountains.Tools;

[Serializable]
public class MMAutoExecutionItem
{
	public bool AutoExecuteOnAwake;

	public bool AutoExecuteOnEnable;

	public bool AutoExecuteOnDisable;

	public bool AutoExecuteOnStart;

	public bool AutoExecuteOnInstantiate;

	public UnityEvent Event;
}

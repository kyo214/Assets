using UnityEngine.Events;

namespace UnityEngine.VFX.Utility;

[ExecuteAlways]
[RequireComponent(typeof(VisualEffect))]
internal class VFXOutputEventUnityEvent : VFXOutputEventAbstractHandler
{
	public UnityEvent onEvent;

	public override bool canExecuteInEditor => false;

	public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
	{
		onEvent?.Invoke();
	}
}

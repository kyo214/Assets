using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Doozy.Runtime.UIManager.Triggers;

[AddComponentMenu("UI/Triggers/UIDeselected")]
public class UIDeselectedTrigger : SignalProvider, IDeselectHandler, IEventSystemHandler
{
	public BaseEventDataEvent OnTrigger = new BaseEventDataEvent();

	public UIDeselectedTrigger()
		: base(ProviderType.Local, "UI", "Deselected", typeof(UIDeselectedTrigger))
	{
	}

	public void OnDeselect(BaseEventData eventData)
	{
		SendSignal(eventData);
		OnTrigger?.Invoke(eventData);
	}
}

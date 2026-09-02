using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Doozy.Runtime.UIManager.Triggers;

[AddComponentMenu("UI/Triggers/UISelected")]
public class UISelectedTrigger : SignalProvider, ISelectHandler, IEventSystemHandler
{
	public BaseEventDataEvent OnTrigger = new BaseEventDataEvent();

	public UISelectedTrigger()
		: base(ProviderType.Local, "UI", "Selected", typeof(UISelectedTrigger))
	{
	}

	public void OnSelect(BaseEventData eventData)
	{
		SendSignal(eventData);
		OnTrigger?.Invoke(eventData);
	}
}

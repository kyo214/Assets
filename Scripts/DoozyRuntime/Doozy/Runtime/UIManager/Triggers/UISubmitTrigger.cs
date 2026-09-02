using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Doozy.Runtime.UIManager.Triggers;

[AddComponentMenu("UI/Triggers/UISubmit")]
public class UISubmitTrigger : SignalProvider, ISubmitHandler, IEventSystemHandler
{
	public BaseEventDataEvent OnTrigger = new BaseEventDataEvent();

	public UISubmitTrigger()
		: base(ProviderType.Local, "UI", "Submit", typeof(UISubmitTrigger))
	{
	}

	public void OnSubmit(BaseEventData eventData)
	{
		if (!UISettings.interactionsDisabled)
		{
			SendSignal(eventData);
			OnTrigger?.Invoke(eventData);
		}
	}
}

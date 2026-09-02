using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Doozy.Runtime.UIManager.Triggers;

[AddComponentMenu("UI/Triggers/PointerDown")]
public class PointerDownTrigger : SignalProvider, IPointerDownHandler, IEventSystemHandler
{
	public PointerEventDataEvent OnTrigger = new PointerEventDataEvent();

	public PointerDownTrigger()
		: base(ProviderType.Local, "Pointer", "Down", typeof(PointerDownTrigger))
	{
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!UISettings.interactionsDisabled)
		{
			SendSignal(eventData);
			OnTrigger?.Invoke(eventData);
		}
	}
}

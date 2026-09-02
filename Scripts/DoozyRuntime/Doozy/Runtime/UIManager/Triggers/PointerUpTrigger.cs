using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Doozy.Runtime.UIManager.Triggers;

[AddComponentMenu("UI/Triggers/PointerUp")]
public class PointerUpTrigger : SignalProvider, IPointerUpHandler, IEventSystemHandler
{
	public PointerEventDataEvent OnTrigger = new PointerEventDataEvent();

	public PointerUpTrigger()
		: base(ProviderType.Local, "Pointer", "Up", typeof(PointerUpTrigger))
	{
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (!UISettings.interactionsDisabled)
		{
			SendSignal(eventData);
			OnTrigger?.Invoke(eventData);
		}
	}
}

using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Doozy.Runtime.UIManager.Triggers;

[AddComponentMenu("UI/Triggers/PointerEnter")]
public class PointerEnterTrigger : SignalProvider, IPointerEnterHandler, IEventSystemHandler
{
	public PointerEventDataEvent OnTrigger = new PointerEventDataEvent();

	public PointerEnterTrigger()
		: base(ProviderType.Local, "Pointer", "Enter", typeof(PointerEnterTrigger))
	{
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!UISettings.interactionsDisabled)
		{
			SendSignal(eventData);
			OnTrigger?.Invoke(eventData);
		}
	}
}

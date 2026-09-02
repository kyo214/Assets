using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Doozy.Runtime.UIManager.Triggers;

[AddComponentMenu("UI/Triggers/PointerExit")]
public class PointerExitTrigger : SignalProvider, IPointerExitHandler, IEventSystemHandler
{
	public PointerEventDataEvent OnTrigger = new PointerEventDataEvent();

	public PointerExitTrigger()
		: base(ProviderType.Local, "Pointer", "Exit", typeof(PointerExitTrigger))
	{
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (!UISettings.interactionsDisabled)
		{
			SendSignal(eventData);
			OnTrigger?.Invoke(eventData);
		}
	}
}

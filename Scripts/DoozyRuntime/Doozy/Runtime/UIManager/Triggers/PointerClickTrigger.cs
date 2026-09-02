using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Doozy.Runtime.UIManager.Triggers;

[AddComponentMenu("UI/Triggers/PointerClick")]
public class PointerClickTrigger : SignalProvider, IPointerClickHandler, IEventSystemHandler
{
	public PointerEventDataEvent OnTrigger = new PointerEventDataEvent();

	public PointerClickTrigger()
		: base(ProviderType.Local, "Pointer", "Click", typeof(PointerClickTrigger))
	{
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!UISettings.interactionsDisabled)
		{
			SendSignal(eventData);
			OnTrigger?.Invoke(eventData);
		}
	}
}

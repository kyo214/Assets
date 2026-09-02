using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Doozy.Runtime.UIManager.Triggers;

[AddComponentMenu("UI/Triggers/PointerMiddleClick")]
public class PointerMiddleClickTrigger : SignalProvider, IPointerClickHandler, IEventSystemHandler
{
	public PointerEventDataEvent OnTrigger = new PointerEventDataEvent();

	public PointerMiddleClickTrigger()
		: base(ProviderType.Local, "Pointer", "Middle Click", typeof(PointerMiddleClickTrigger))
	{
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!UISettings.interactionsDisabled && eventData.button == PointerEventData.InputButton.Middle)
		{
			SendSignal(eventData);
			OnTrigger?.Invoke(eventData);
		}
	}
}

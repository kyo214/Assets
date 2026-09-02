using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Doozy.Runtime.UIManager.Triggers;

[AddComponentMenu("UI/Triggers/PointerLeftClick")]
public class PointerLeftClickTrigger : SignalProvider, IPointerClickHandler, IEventSystemHandler
{
	public PointerEventDataEvent OnTrigger = new PointerEventDataEvent();

	public PointerLeftClickTrigger()
		: base(ProviderType.Local, "Pointer", "Left Click", typeof(PointerLeftClickTrigger))
	{
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!UISettings.interactionsDisabled && eventData.button == PointerEventData.InputButton.Left)
		{
			SendSignal(eventData);
			OnTrigger?.Invoke(eventData);
		}
	}
}

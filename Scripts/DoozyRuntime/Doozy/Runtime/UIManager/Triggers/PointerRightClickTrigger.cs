using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Doozy.Runtime.UIManager.Triggers;

[AddComponentMenu("UI/Triggers/PointerRightClick")]
public class PointerRightClickTrigger : SignalProvider, IPointerClickHandler, IEventSystemHandler
{
	public PointerEventDataEvent OnTrigger = new PointerEventDataEvent();

	public PointerRightClickTrigger()
		: base(ProviderType.Local, "Pointer", "Right Click", typeof(PointerRightClickTrigger))
	{
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!UISettings.interactionsDisabled && eventData.button == PointerEventData.InputButton.Right)
		{
			SendSignal(eventData);
			OnTrigger?.Invoke(eventData);
		}
	}
}

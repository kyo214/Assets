using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Doozy.Runtime.UIManager.Triggers;

[AddComponentMenu("UI/Triggers/PointerDoubleClick")]
public class PointerDoubleClickTrigger : SignalProvider, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	public PointerEventDataEvent OnTrigger = new PointerEventDataEvent();

	public const float k_DoubleClickRegisterInterval = 0.2f;

	private bool m_ClickedOnce;

	private float m_ClickTime;

	public PointerDoubleClickTrigger()
		: base(ProviderType.Local, "Pointer", "Double Click", typeof(PointerDoubleClickTrigger))
	{
		Reset();
	}

	public void Reset()
	{
		m_ClickedOnce = false;
		m_ClickTime = 0f;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (!m_ClickedOnce)
		{
			m_ClickedOnce = true;
			m_ClickTime = Time.unscaledTime;
			return;
		}
		if (Time.unscaledTime - m_ClickTime > 0.2f)
		{
			m_ClickedOnce = true;
			m_ClickTime = Time.unscaledTime;
			return;
		}
		Reset();
		if (!UISettings.interactionsDisabled)
		{
			SendSignal(eventData);
			OnTrigger?.Invoke(eventData);
		}
	}
}

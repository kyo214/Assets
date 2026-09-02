using System.Collections;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Doozy.Runtime.UIManager.Triggers;

[AddComponentMenu("UI/Triggers/PointerLongClick")]
public class PointerLongClickTrigger : SignalProvider, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerExitHandler
{
	public PointerEventDataEvent OnTrigger = new PointerEventDataEvent();

	public float k_LongClickRegisterInterval = 0.5f;

	private float m_LongClickTriggerTime;

	private Coroutine run { get; set; }

	public PointerLongClickTrigger()
		: base(ProviderType.Local, "Pointer", "Long Click", typeof(PointerLongClickTrigger))
	{
		Reset();
	}

	public void Reset()
	{
		if (run != null)
		{
			StopCoroutine(run);
			run = null;
		}
		m_LongClickTriggerTime = 0f;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		Reset();
		m_LongClickTriggerTime = Time.unscaledTime + k_LongClickRegisterInterval;
		run = StartCoroutine(Run(eventData));
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		Reset();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Reset();
	}

	private IEnumerator Run(PointerEventData eventData)
	{
		while (m_LongClickTriggerTime > Time.unscaledTime)
		{
			yield return null;
		}
		if (!UISettings.interactionsDisabled)
		{
			SendSignal(eventData);
			OnTrigger?.Invoke(eventData);
			Reset();
		}
	}
}

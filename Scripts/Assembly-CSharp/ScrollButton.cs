using System;
using System.Collections;
using Doozy.Runtime.UIManager.Components;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(UIButton))]
public class ScrollButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	private enum ScrollButtonDirection
	{
		UP = 0,
		DOWN = 1
	}

	[SerializeField]
	private ScrollButtonDirection direction;

	[SerializeField]
	private float stepSize;

	[SerializeField]
	private float scrollFrequency;

	[SerializeField]
	private Scrollbar scrollbar;

	[SerializeField]
	private ScrollElementsContainer scrollElementsContainer;

	private float signedStepSize;

	private UIButton button;

	private void Awake()
	{
		button = GetComponent<UIButton>();
	}

	private void Start()
	{
		signedStepSize = ((direction == ScrollButtonDirection.DOWN) ? (stepSize * -1f) : stepSize);
		scrollbar.onValueChanged.AddListener((float val) =>
		{
			HandleScrollViewChanged();
		});
		ScrollElementsContainer obj = scrollElementsContainer;
		obj.OnContainerChildrenChanged = (Action)Delegate.Combine(obj.OnContainerChildrenChanged, new Action(HandleScrollViewChanged));
	}

	private void HandleScrollViewChanged()
	{
		if (Mathf.Approximately(scrollbar.size, 1f))
		{
			SetButtonState(enabled: false);
		}
		else
		{
			HandleScrollValueChanged(scrollbar.value);
		}
	}

	private void HandleScrollValueChanged(float value)
	{
		value = value.RoundDecimalPlaces(2);
		if (direction == ScrollButtonDirection.DOWN && Mathf.Approximately(value, 0f))
		{
			SetButtonState(enabled: false);
		}
		else if (direction == ScrollButtonDirection.UP && Mathf.Approximately(value, 1f))
		{
			SetButtonState(enabled: false);
		}
		else
		{
			SetButtonState(enabled: true);
		}
	}

	private void SetButtonState(bool enabled)
	{
		button.interactable = enabled;
		if (!enabled)
		{
			CancelInvoke("ScrollContent");
			StopAllCoroutines();
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (button.IsInteractable())
		{
			InvokeRepeating("ScrollContent", 0f, scrollFrequency);
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		CancelInvoke("ScrollContent");
		StopAllCoroutines();
	}

	private void ScrollContent()
	{
		StopAllCoroutines();
		StartCoroutine(SmoothScrolling(scrollbar.value, Mathf.Clamp01(scrollbar.value + signedStepSize * scrollbar.size.RoundDecimalPlaces(2)), scrollFrequency));
	}

	private IEnumerator SmoothScrolling(float minValue, float maxValue, float totalTime)
	{
		float time = 0f;
		totalTime = Mathf.Abs(maxValue - minValue) * totalTime;
		while (time <= totalTime)
		{
			float t = time / totalTime;
			scrollbar.value = Mathf.Lerp(minValue, maxValue, t);
			time += totalTime * Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
	}

	private void OnDestroy()
	{
		scrollbar.onValueChanged.RemoveAllListeners();
		ScrollElementsContainer obj = scrollElementsContainer;
		obj.OnContainerChildrenChanged = (Action)Delegate.Remove(obj.OnContainerChildrenChanged, new Action(HandleScrollViewChanged));
	}
}

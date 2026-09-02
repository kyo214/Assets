using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.UIManager.Components;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Doozy.Runtime.UIManager.Containers;

public static class UIPopupExtensions
{
	public static T Reset<T>(this T popup) where T : UIPopup
	{
		popup.parentRectTransform = null;
		return popup;
	}

	public static T SetParent<T>(this T popup, RectTransform parent) where T : UIPopup
	{
		popup.parentRectTransform = parent;
		if (parent == null)
		{
			return popup;
		}
		popup.rectTransform.SetParent(parent, worldPositionStays: true);
		popup.rectTransform.CenterPivot().ExpandToParentSize(resetScaleToOne: true);
		return popup;
	}

	public static T SetOverrideSorting<T>(this T popup, bool overrideSortingOrder, bool apply = false) where T : UIPopup
	{
		popup.OverrideSorting = overrideSortingOrder;
		if (apply)
		{
			ApplyOverrideSorting(popup);
		}
		return popup;
	}

	public static T ApplyOverrideSorting<T>(this T popup) where T : UIPopup
	{
		if (!popup.OverrideSorting)
		{
			return popup;
		}
		popup.canvas.overrideSorting = true;
		popup.canvas.sortingOrder = 32766;
		if (!popup.canvas.gameObject.activeInHierarchy)
		{
			Debug.Log("Cannot apply override sorting order to popup " + popup.name + " because it is not active in the scene");
		}
		if (!popup.canvas.enabled)
		{
			Debug.Log("Cannot apply override sorting order to popup " + popup.name + " because its canvas is not enabled");
		}
		return popup;
	}

	public static T SetTexts<T>(this T popup, params string[] texts) where T : UIPopup
	{
		int num = texts.Length;
		if (num == 0)
		{
			return popup;
		}
		if (popup.Labels == null)
		{
			Debug.LogWarning("Cannot set texts for popup " + popup.name + " because it has no labels references");
			return popup;
		}
		popup.Labels = popup.Labels.RemoveNulls();
		if (popup.Labels.Count == 0)
		{
			Debug.LogWarning("Cannot set texts for popup " + popup.name + " because it has no labels references");
			return popup;
		}
		for (int i = 0; i < popup.Labels.Count; i++)
		{
			TextMeshProUGUI textMeshProUGUI = popup.Labels[i];
			if (!(textMeshProUGUI == null))
			{
				textMeshProUGUI.SetText((i < num) ? texts[i] : string.Empty);
				textMeshProUGUI.ForceMeshUpdate();
			}
		}
		return popup;
	}

	public static T SetSprites<T>(this T popup, params Sprite[] sprites) where T : UIPopup
	{
		int num = sprites.Length;
		if (num == 0)
		{
			return popup;
		}
		if (popup.Images == null)
		{
			Debug.LogWarning("Cannot set sprites for popup " + popup.name + " because it has no image references");
			return popup;
		}
		popup.Images = popup.Images.RemoveNulls();
		if (popup.Images.Count == 0)
		{
			Debug.LogWarning("Cannot set sprites for popup " + popup.name + " because it has no image references");
			return popup;
		}
		for (int i = 0; i < popup.Images.Count; i++)
		{
			Image image = popup.Images[i];
			if (!(image == null))
			{
				image.sprite = ((i < num) ? sprites[i] : null);
			}
		}
		return popup;
	}

	public static T SetEvents<T>(this T popup, params UnityEvent[] events) where T : UIPopup
	{
		int num = events.Length;
		if (num == 0)
		{
			return popup;
		}
		bool flag = popup.GetComponentInChildren<GraphicRaycaster>();
		if (popup.Buttons == null)
		{
			Debug.LogWarning("Cannot set events for popup " + popup.name + " because it has no button references");
			return popup;
		}
		popup.Buttons = popup.Buttons.RemoveNulls();
		if (popup.Buttons.Count == 0)
		{
			Debug.LogWarning("Cannot set events for popup " + popup.name + " because it has no button references");
			return popup;
		}
		for (int i = 0; i < popup.Buttons.Count; i++)
		{
			UIButton uIButton = popup.Buttons[i];
			if (uIButton == null)
			{
				continue;
			}
			if (!flag && !uIButton.GetComponent<GraphicRaycaster>())
			{
				uIButton.gameObject.AddComponent<GraphicRaycaster>();
			}
			if (num > i)
			{
				UnityEvent unityEvent = events[i];
				if (unityEvent != null)
				{
					uIButton.onClickBehaviour.Event.AddListener(unityEvent.Invoke);
					uIButton.onSubmitBehaviour.Event.AddListener(unityEvent.Invoke);
				}
			}
		}
		return popup;
	}

	public static T SetEvents<T>(this T popup, params UnityAction[] actions) where T : UIPopup
	{
		int num = actions.Length;
		if (num == 0)
		{
			return popup;
		}
		bool flag = popup.GetComponentInChildren<GraphicRaycaster>();
		if (popup.Buttons == null)
		{
			Debug.LogWarning("Cannot set events for popup " + popup.name + " because it has no button references");
			return popup;
		}
		popup.Buttons = popup.Buttons.RemoveNulls();
		if (popup.Buttons.Count == 0)
		{
			Debug.LogWarning("Cannot set events for popup " + popup.name + " because it has no button references");
			return popup;
		}
		for (int i = 0; i < popup.Buttons.Count; i++)
		{
			UIButton uIButton = popup.Buttons[i];
			if (uIButton == null)
			{
				continue;
			}
			if (!flag && !uIButton.GetComponent<GraphicRaycaster>())
			{
				uIButton.gameObject.AddComponent<GraphicRaycaster>();
			}
			if (num > i)
			{
				UnityAction unityAction = actions[i];
				if (unityAction != null)
				{
					uIButton.onClickBehaviour.Event.AddListener(unityAction.Invoke);
					uIButton.onSubmitBehaviour.Event.AddListener(unityAction.Invoke);
				}
			}
		}
		return popup;
	}

	public static T ShowFromQueue<T>(this T popup, string queueName = "Default") where T : UIPopup
	{
		if (string.IsNullOrEmpty(queueName))
		{
			Debug.LogError("Cannot show popup " + popup.name + " from queue because the queue name is null or empty");
			return popup;
		}
		UIPopup.AddPopupToQueue(popup);
		return popup;
	}
}

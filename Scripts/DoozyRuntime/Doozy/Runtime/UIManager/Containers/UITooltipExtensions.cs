using System.Linq;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Doozy.Runtime.UIManager.Containers;

public static class UITooltipExtensions
{
	public static T Reset<T>(this T tooltip) where T : UITooltip
	{
		tooltip.updateTarget = true;
		tooltip.tooltipRootCanvas = null;
		tooltip.targetRectTransform = null;
		tooltip.parentRectTransform = null;
		tooltip.trigger = null;
		tooltip.followTarget = null;
		return tooltip;
	}

	public static T SetParent<T>(this T tooltip, RectTransform parent) where T : UITooltip
	{
		tooltip.tooltipRootCanvas = null;
		tooltip.parentRectTransform = parent;
		if (parent == null)
		{
			return tooltip;
		}
		tooltip.rectTransform.SetParent(parent, worldPositionStays: true);
		tooltip.rectTransform.CenterPivot();
		tooltip.rectTransform.localScale = Vector3.one;
		tooltip.rectTransform.anchoredPosition3D = tooltip.CustomStartPosition;
		tooltip.tooltipRootCanvas = parent.GetComponentInParent<Canvas>().rootCanvas;
		return tooltip;
	}

	public static T SetTrigger<T>(this T tooltip, UITooltipTrigger target) where T : UITooltip
	{
		tooltip.trigger = target;
		return tooltip;
	}

	public static T SetFollowTarget<T>(this T tooltip, GameObject target) where T : UITooltip
	{
		tooltip.followTarget = target;
		return tooltip;
	}

	public static T SetFollowTargetFromUITag<T>(this T tooltip, string category, string name) where T : UITooltip
	{
		tooltip.followTarget = null;
		UITag uITag = UITag.GetTags(category, name).FirstOrDefault();
		if (uITag != null)
		{
			tooltip.followTarget = uITag.gameObject;
		}
		return tooltip;
	}

	public static T SetKeepInScreen<T>(this T tooltip, bool keepInScreen) where T : UITooltip
	{
		tooltip.KeepInScreen = keepInScreen;
		return tooltip;
	}

	public static T SetOverrideSorting<T>(this T target, bool overrideSortingOrder, bool apply = false) where T : UITooltip
	{
		target.OverrideSorting = overrideSortingOrder;
		if (apply)
		{
			ApplyOverrideSorting(target);
		}
		return target;
	}

	public static T ApplyOverrideSorting<T>(this T target) where T : UITooltip
	{
		if (!target.OverrideSorting)
		{
			return target;
		}
		target.canvas.overrideSorting = true;
		target.canvas.sortingOrder = 32767;
		if (!target.canvas.gameObject.activeInHierarchy)
		{
			Debug.Log("Cannot apply override sorting order to tooltip " + target.name + " because it is not active in the scene");
		}
		if (!target.canvas.enabled)
		{
			Debug.Log("Cannot apply override sorting order to tooltip " + target.name + " because its canvas is not enabled");
		}
		return target;
	}

	public static T SetTexts<T>(this T tooltip, params string[] texts) where T : UITooltip
	{
		int num = texts.Length;
		if (num == 0)
		{
			return tooltip;
		}
		for (int i = 0; i < tooltip.Labels.Count; i++)
		{
			TextMeshProUGUI textMeshProUGUI = tooltip.Labels[i];
			if (!(textMeshProUGUI == null))
			{
				textMeshProUGUI.SetText((i < num) ? texts[i] : string.Empty);
				textMeshProUGUI.ForceMeshUpdate();
			}
		}
		return tooltip;
	}

	public static T SetSprites<T>(this T tooltip, params Sprite[] sprites) where T : UITooltip
	{
		int num = sprites.Length;
		if (num == 0)
		{
			return tooltip;
		}
		for (int i = 0; i < tooltip.Images.Count; i++)
		{
			Image image = tooltip.Images[i];
			if (!(image == null))
			{
				image.sprite = ((i < num) ? sprites[i] : null);
			}
		}
		return tooltip;
	}

	public static T SetEvents<T>(this T tooltip, params UnityEvent[] events) where T : UITooltip
	{
		int num = events.Length;
		if (num == 0)
		{
			return tooltip;
		}
		bool flag = tooltip.GetComponentInChildren<GraphicRaycaster>();
		for (int i = 0; i < tooltip.Buttons.Count; i++)
		{
			UIButton uIButton = tooltip.Buttons[i];
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
		return tooltip;
	}

	public static T SetEvents<T>(this T tooltip, params UnityAction[] actions) where T : UITooltip
	{
		int num = actions.Length;
		if (num == 0)
		{
			return tooltip;
		}
		bool flag = tooltip.GetComponentInChildren<GraphicRaycaster>();
		for (int i = 0; i < tooltip.Buttons.Count; i++)
		{
			UIButton uIButton = tooltip.Buttons[i];
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
		return tooltip;
	}
}

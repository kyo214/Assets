using UnityEngine;

namespace Doozy.Runtime.UIManager.Extensions;

public static class RectTransformExtensions
{
	public static RectTransform ResetCanvasGroup(this RectTransform target, bool interactable = true, bool blockRaycasts = true, bool addCanvasGroupIfNotFound = false)
	{
		CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
		if (canvasGroup == null)
		{
			if (!addCanvasGroupIfNotFound)
			{
				return target;
			}
			canvasGroup = target.gameObject.AddComponent<CanvasGroup>();
		}
		canvasGroup.interactable = interactable;
		canvasGroup.blocksRaycasts = blockRaycasts;
		return target;
	}
}

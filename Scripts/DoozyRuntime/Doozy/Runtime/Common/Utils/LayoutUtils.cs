using Doozy.Runtime.Common.Layouts;
using UnityEngine;
using UnityEngine.UI;

namespace Doozy.Runtime.Common.Utils;

public static class LayoutUtils
{
	public static bool IsInLayoutGroup(this RectTransform target)
	{
		if (!target.GetLayoutGroupInParent())
		{
			return false;
		}
		LayoutElement component = target.GetComponent<LayoutElement>();
		if (component == null)
		{
			return true;
		}
		return !component.ignoreLayout;
	}

	public static bool ContainsLayoutGroup(this RectTransform target)
	{
		return target.GetComponentInChildren<LayoutGroup>();
	}

	public static LayoutGroup GetLayoutGroupInParent(this RectTransform target)
	{
		if (!(target.parent != null))
		{
			return null;
		}
		return target.parent.GetComponent<LayoutGroup>();
	}

	public static UIBehaviourHandler GetUIBehaviourHandler(this RectTransform target)
	{
		return target.GetComponent<UIBehaviourHandler>() ?? target.gameObject.AddComponent<UIBehaviourHandler>();
	}

	public static UIBehaviourHandler GetUIBehaviourHandler(this LayoutGroup target)
	{
		return target.GetComponent<UIBehaviourHandler>() ?? target.gameObject.AddComponent<UIBehaviourHandler>();
	}
}

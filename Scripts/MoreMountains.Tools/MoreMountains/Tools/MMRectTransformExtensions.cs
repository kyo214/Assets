using UnityEngine;

namespace MoreMountains.Tools;

public static class MMRectTransformExtensions
{
	public static void MMSetLeft(this RectTransform rt, float left)
	{
		rt.offsetMin = new Vector2(left, rt.offsetMin.y);
	}

	public static void MMSetRight(this RectTransform rt, float right)
	{
		rt.offsetMax = new Vector2(0f - right, rt.offsetMax.y);
	}

	public static void MMSetTop(this RectTransform rt, float top)
	{
		rt.offsetMax = new Vector2(rt.offsetMax.x, 0f - top);
	}

	public static void MMSetBottom(this RectTransform rt, float bottom)
	{
		rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
	}
}

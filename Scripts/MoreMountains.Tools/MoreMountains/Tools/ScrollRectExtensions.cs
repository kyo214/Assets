using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Tools;

public static class ScrollRectExtensions
{
	public static void MMScrollToTop(this ScrollRect scrollRect)
	{
		scrollRect.normalizedPosition = new Vector2(0f, 1f);
	}

	public static void MMScrollToBottom(this ScrollRect scrollRect)
	{
		scrollRect.normalizedPosition = new Vector2(0f, 0f);
	}
}

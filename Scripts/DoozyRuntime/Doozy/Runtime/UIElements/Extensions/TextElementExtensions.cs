using UnityEngine.UIElements;

namespace Doozy.Runtime.UIElements.Extensions;

public static class TextElementExtensions
{
	public static T SetText<T>(this T target, string text) where T : TextElement
	{
		target.text = text;
		return target;
	}

	public static string GetText<T>(this T target) where T : TextElement
	{
		return target.text;
	}
}

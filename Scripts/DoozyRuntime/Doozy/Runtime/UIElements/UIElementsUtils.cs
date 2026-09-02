using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine.UIElements;

namespace Doozy.Runtime.UIElements;

public static class UIElementsUtils
{
	public static void AddClass(string className, IEnumerable<VisualElement> elements, bool removeNulls = true)
	{
		if (className.IsNullOrEmpty() || elements == null)
		{
			return;
		}
		if (removeNulls)
		{
			elements = elements.Where((VisualElement item) => item != null);
		}
		foreach (VisualElement element in elements)
		{
			element.AddClass(className);
		}
	}

	public static void RemoveClass(string className, IEnumerable<VisualElement> elements, bool removeNulls = true)
	{
		if (className.IsNullOrEmpty() || elements == null)
		{
			return;
		}
		if (removeNulls)
		{
			elements = elements.Where((VisualElement item) => item != null);
		}
		foreach (VisualElement element in elements)
		{
			element.RemoveClass(className);
		}
	}
}

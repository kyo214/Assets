using System;
using UnityEngine;

namespace Doozy.Runtime.Colors;

[Serializable]
public class ThemeColor
{
	public Color ColorOnDark;

	public Color ColorOnLight;

	public virtual bool isDarkTheme { get; set; }

	public Color color
	{
		get
		{
			if (!isDarkTheme)
			{
				return ColorOnLight;
			}
			return ColorOnDark;
		}
	}

	public ThemeColor()
	{
		ColorOnDark = (ColorOnLight = Color.white);
	}
}

using UnityEngine;

namespace MoreMountains.Tools;

public class MMBackgroundColorAttribute : PropertyAttribute
{
	public MMBackgroundAttributeColor Color;

	public MMBackgroundColorAttribute(MMBackgroundAttributeColor color = MMBackgroundAttributeColor.Yellow)
	{
		Color = color;
	}
}

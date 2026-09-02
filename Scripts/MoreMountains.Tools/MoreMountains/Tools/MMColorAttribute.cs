using UnityEngine;

namespace MoreMountains.Tools;

public class MMColorAttribute : PropertyAttribute
{
	public Color color;

	public MMColorAttribute(float red = 1f, float green = 0f, float blue = 0f)
	{
		color = new Color(red, green, blue, 1f);
	}
}

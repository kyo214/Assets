using UnityEngine;

namespace MoreMountains.Tools;

public static class RectExtensions
{
	public static bool MMIntersects(this Rect thisRectangle, Rect otherRectangle)
	{
		if (!(thisRectangle.x > otherRectangle.xMax) && !(thisRectangle.xMax < otherRectangle.x) && !(thisRectangle.y > otherRectangle.yMax))
		{
			return !(thisRectangle.yMax < otherRectangle.y);
		}
		return false;
	}
}

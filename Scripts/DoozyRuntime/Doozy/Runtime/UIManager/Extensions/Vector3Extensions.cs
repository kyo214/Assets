using UnityEngine;

namespace Doozy.Runtime.UIManager.Extensions;

public static class Vector3Extensions
{
	public static Vector3 SetZToOne(this Vector3 target)
	{
		return new Vector3(target.x, target.y, 1f);
	}

	public static Vector3 SetZToZero(this Vector3 target)
	{
		return new Vector3(target.x, target.y, 0f);
	}
}

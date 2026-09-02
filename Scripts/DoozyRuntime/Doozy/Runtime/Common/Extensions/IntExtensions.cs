using UnityEngine;

namespace Doozy.Runtime.Common.Extensions;

public static class IntExtensions
{
	public static int Clamp(this int target, int min, int max)
	{
		return Mathf.Clamp(target, min, max);
	}
}

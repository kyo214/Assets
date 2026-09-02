using UnityEngine;

namespace MoreMountains.Tools;

public static class MMFloatExtensions
{
	public static float MMNormalizeAngle(this float angleInDegrees)
	{
		angleInDegrees %= 360f;
		if (angleInDegrees < 0f)
		{
			angleInDegrees += 360f;
		}
		return angleInDegrees;
	}

	public static float RoundDown(this float number, int decimalPlaces)
	{
		return Mathf.Floor(number * Mathf.Pow(10f, decimalPlaces)) / Mathf.Pow(10f, decimalPlaces);
	}
}

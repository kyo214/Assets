using System;
using UnityEngine;

namespace MoreMountains.Tools;

public static class MMVector2Extensions
{
	public static Vector2 MMRotate(this Vector2 vector, float angleInDegrees)
	{
		float num = Mathf.Sin(angleInDegrees * (MathF.PI / 180f));
		float num2 = Mathf.Cos(angleInDegrees * (MathF.PI / 180f));
		float x = vector.x;
		float y = vector.y;
		vector.x = num2 * x - num * y;
		vector.y = num * x + num2 * y;
		return vector;
	}

	public static Vector2 MMSetX(this Vector2 vector, float newValue)
	{
		vector.x = newValue;
		return vector;
	}

	public static Vector2 MMSetY(this Vector2 vector, float newValue)
	{
		vector.y = newValue;
		return vector;
	}
}

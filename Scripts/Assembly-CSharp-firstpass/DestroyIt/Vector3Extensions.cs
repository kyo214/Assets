using UnityEngine;

namespace DestroyIt;

public static class Vector3Extensions
{
	public static Vector3 LerpByDistance(this Vector3 startPoint, Vector3 endPoint, float distance)
	{
		return distance * Vector3.Normalize(endPoint - startPoint) + startPoint;
	}

	public static Vector3 ClosestDirection(this Vector3 vector)
	{
		Vector3[] obj = new Vector3[6]
		{
			Vector3.left,
			Vector3.right,
			Vector3.forward,
			Vector3.back,
			Vector3.up,
			Vector3.down
		};
		Vector3 result = Vector3.zero;
		float num = float.NegativeInfinity;
		Vector3[] array = obj;
		foreach (Vector3 vector2 in array)
		{
			float num2 = Vector3.Dot(vector, vector2);
			if (num2 > num)
			{
				result = vector2;
				num = num2;
			}
		}
		return result;
	}
}

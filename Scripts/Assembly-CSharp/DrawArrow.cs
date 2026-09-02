using UnityEngine;

public static class DrawArrow
{
	private enum TargetType
	{
		Gizmo = 0,
		Debug = 1,
		Handle = 2
	}

	public static void ForHandle(in Vector3 pos, in Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20f)
	{
	}

	public static void ForHandle(in Vector3 pos, in Vector3 direction, in Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20f)
	{
		Arrow(TargetType.Handle, in pos, in direction, in color, arrowHeadLength, arrowHeadAngle);
	}

	public static void ForGizmo(in Vector3 pos, in Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20f)
	{
		Arrow(TargetType.Gizmo, in pos, in direction, Gizmos.color, arrowHeadLength, arrowHeadAngle);
	}

	public static void ForGizmo(in Vector3 pos, in Vector3 direction, in Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20f)
	{
		Arrow(TargetType.Gizmo, in pos, in direction, in color, arrowHeadLength, arrowHeadAngle);
	}

	public static void ForDebug(in Vector3 pos, in Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20f)
	{
		Debug.DrawRay(pos, direction);
		Arrow(TargetType.Debug, in pos, in direction, Gizmos.color, arrowHeadLength, arrowHeadAngle);
	}

	public static void ForDebug(in Vector3 pos, in Vector3 direction, in Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20f)
	{
		Debug.DrawRay(pos, direction, color);
		Arrow(TargetType.Debug, in pos, in direction, in color, arrowHeadLength, arrowHeadAngle);
	}

	private static void Arrow(TargetType targetType, in Vector3 pos, in Vector3 direction, in Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20f)
	{
		Vector3 vector = Quaternion.LookRotation(direction) * Quaternion.Euler(arrowHeadAngle, 0f, 0f) * Vector3.back * arrowHeadLength;
		Vector3 vector2 = Quaternion.LookRotation(direction) * Quaternion.Euler(0f - arrowHeadAngle, 0f, 0f) * Vector3.back * arrowHeadLength;
		Vector3 vector3 = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, arrowHeadAngle, 0f) * Vector3.back * arrowHeadLength;
		Vector3 vector4 = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, 0f - arrowHeadAngle, 0f) * Vector3.back * arrowHeadLength;
		Vector3 vector5 = pos + direction;
		switch (targetType)
		{
		case TargetType.Gizmo:
		{
			Color color2 = Gizmos.color;
			Gizmos.color = color;
			Gizmos.DrawRay(pos, direction);
			Gizmos.DrawRay(vector5, vector);
			Gizmos.DrawRay(vector5, vector2);
			Gizmos.DrawRay(vector5, vector3);
			Gizmos.DrawRay(vector5, vector4);
			Gizmos.color = color2;
			break;
		}
		case TargetType.Debug:
			Debug.DrawRay(vector5, vector, color);
			Debug.DrawRay(vector5, vector2, color);
			Debug.DrawRay(vector5, vector3, color);
			Debug.DrawRay(vector5, vector4, color);
			break;
		case TargetType.Handle:
			break;
		}
	}
}

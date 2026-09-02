using System;
using UnityEngine;

namespace Doozy.Runtime.Reactor;

public static class ReactorUtils
{
	public static MoveDirection Reverse(this MoveDirection target)
	{
		return target switch
		{
			MoveDirection.Left => MoveDirection.Right, 
			MoveDirection.Top => MoveDirection.Bottom, 
			MoveDirection.Right => MoveDirection.Left, 
			MoveDirection.Bottom => MoveDirection.Top, 
			MoveDirection.TopLeft => MoveDirection.BottomRight, 
			MoveDirection.TopCenter => MoveDirection.BottomCenter, 
			MoveDirection.TopRight => MoveDirection.BottomLeft, 
			MoveDirection.MiddleLeft => MoveDirection.MiddleRight, 
			MoveDirection.MiddleCenter => MoveDirection.MiddleCenter, 
			MoveDirection.MiddleRight => MoveDirection.MiddleLeft, 
			MoveDirection.BottomLeft => MoveDirection.TopRight, 
			MoveDirection.BottomCenter => MoveDirection.TopCenter, 
			MoveDirection.BottomRight => MoveDirection.TopLeft, 
			MoveDirection.CustomPosition => MoveDirection.CustomPosition, 
			_ => throw new ArgumentOutOfRangeException("target", target, null), 
		};
	}

	public static Vector3 GetMoveOutPosition(RectTransform target, MoveDirection moveToDirection, Vector3 fromPosition)
	{
		return GetTargetPosition(target, moveToDirection, fromPosition, target.localScale, target.localEulerAngles);
	}

	public static Vector3 GetMoveOutPosition(RectTransform target, MoveDirection moveToDirection, Vector3 fromPosition, Vector3 toLocalScale, Vector3 toLocalEulerAngles)
	{
		return GetTargetPosition(target, moveToDirection, fromPosition, toLocalScale, toLocalEulerAngles);
	}

	public static Vector3 GetMoveInPosition(RectTransform target, MoveDirection moveFromDirection, Vector3 toValue)
	{
		return GetTargetPosition(target, moveFromDirection, toValue, target.localScale, target.localEulerAngles);
	}

	public static Vector3 GetMoveInPosition(RectTransform target, MoveDirection moveFromDirection, Vector3 toValue, Vector3 fromLocalScale, Vector3 fromLocalEulerAngles)
	{
		return GetTargetPosition(target, moveFromDirection, toValue, fromLocalScale, fromLocalEulerAngles);
	}

	public static Vector3 GetTargetPosition(RectTransform target, MoveDirection moveDirection, Vector3 startPosition, Vector3 targetLocalScale, Vector3 targetLocalEulerAngles)
	{
		if (target == null || target.parent == null)
		{
			return Vector3.zero;
		}
		RectTransform component = target.parent.GetComponent<RectTransform>();
		component.ForceUpdateRectTransforms();
		Rect rect = component.rect;
		Rect rect2 = target.rect;
		Vector2 pivot = target.pivot;
		float num = 0f;
		switch (moveDirection)
		{
		case MoveDirection.Left:
		case MoveDirection.TopLeft:
		case MoveDirection.MiddleLeft:
		case MoveDirection.BottomLeft:
			num = rect2.width * targetLocalScale.x * (1f - pivot.x) + rect.width * (1f - pivot.x) * target.anchorMin.x + rect.width * pivot.x * target.anchorMax.x;
			break;
		case MoveDirection.Right:
		case MoveDirection.TopRight:
		case MoveDirection.MiddleRight:
		case MoveDirection.BottomRight:
			num = rect.width + rect2.width * targetLocalScale.x * pivot.x - rect.width * (1f - pivot.x) * target.anchorMin.x - rect.width * pivot.x * target.anchorMax.x;
			break;
		}
		float num2 = 0f;
		switch (moveDirection)
		{
		case MoveDirection.Top:
		case MoveDirection.TopLeft:
		case MoveDirection.TopCenter:
		case MoveDirection.TopRight:
			num2 = rect.height + rect2.height * targetLocalScale.y * pivot.y - rect.height * (1f - pivot.y) * target.anchorMin.y - rect.height * pivot.y * target.anchorMax.y;
			break;
		case MoveDirection.Bottom:
		case MoveDirection.BottomLeft:
		case MoveDirection.BottomCenter:
		case MoveDirection.BottomRight:
			num2 = rect2.height * targetLocalScale.y * (1f - pivot.y) + rect.height * (1f - pivot.y) * target.anchorMin.y + rect.height * pivot.y * target.anchorMax.y;
			break;
		}
		float x = startPosition.x;
		float y = startPosition.y;
		float z = startPosition.z;
		float num3 = 0f;
		float num4 = 0f;
		Vector3 result;
		switch (moveDirection)
		{
		case MoveDirection.Left:
			result = new Vector3(0f - num, y, z);
			num3 = -1f;
			num4 = 0f;
			break;
		case MoveDirection.Right:
			result = new Vector3(num, y, z);
			num3 = 1f;
			num4 = 0f;
			break;
		case MoveDirection.Top:
			result = new Vector3(x, num2, z);
			num3 = 0f;
			num4 = 1f;
			break;
		case MoveDirection.Bottom:
			result = new Vector3(x, 0f - num2, z);
			num3 = 0f;
			num4 = -1f;
			break;
		case MoveDirection.TopLeft:
			result = new Vector3(0f - num, num2, z);
			num3 = -1f;
			num4 = 1f;
			break;
		case MoveDirection.TopCenter:
			result = new Vector3(0f, num2, z);
			num3 = 0f;
			num4 = 1f;
			break;
		case MoveDirection.TopRight:
			result = new Vector3(num, num2, z);
			num3 = 1f;
			num4 = 1f;
			break;
		case MoveDirection.MiddleLeft:
			result = new Vector3(0f - num, 0f, z);
			num3 = -1f;
			num4 = 0f;
			break;
		case MoveDirection.MiddleCenter:
			result = new Vector3(0f, 0f, z);
			num3 = 0f;
			num4 = 0f;
			break;
		case MoveDirection.MiddleRight:
			result = new Vector3(num, 0f, z);
			num3 = 1f;
			num4 = 0f;
			break;
		case MoveDirection.BottomLeft:
			result = new Vector3(0f - num, 0f - num2, z);
			num3 = -1f;
			num4 = -1f;
			break;
		case MoveDirection.BottomCenter:
			result = new Vector3(0f, 0f - num2, z);
			num3 = 0f;
			num4 = -1f;
			break;
		case MoveDirection.BottomRight:
			result = new Vector3(num, 0f - num2, z);
			num3 = 1f;
			num4 = -1f;
			break;
		default:
			result = startPosition;
			break;
		}
		if (Mathf.Approximately(0f, targetLocalEulerAngles.z))
		{
			return result;
		}
		float num5 = Mathf.Abs(targetLocalEulerAngles.z % 180f);
		float f = num5 * (MathF.PI / 180f);
		float num6 = rect2.width * targetLocalScale.x;
		float num7 = rect2.height * targetLocalScale.y;
		float num8;
		float num9;
		if (Mathf.Approximately(num5, 0f) || Mathf.Approximately(num5, 90f))
		{
			num8 = num6;
			num9 = num7;
		}
		else if (num5 < 90f)
		{
			num8 = num6 * Mathf.Cos(f) + num7 * Mathf.Sin(f);
			num9 = num6 * Mathf.Sin(f) + num7 * Mathf.Cos(f);
		}
		else
		{
			num5 -= 90f;
			f = num5 * (MathF.PI / 180f);
			num8 = num7 * Mathf.Cos(f) + num6 * Mathf.Sin(f);
			num9 = num7 * Mathf.Sin(f) + num6 * Mathf.Cos(f);
		}
		float num10 = (num8 - num6) / 2f;
		float num11 = (num9 - num7) / 2f;
		return new Vector3(result.x + num10 * num3, result.y + num11 * num4, result.z);
	}
}

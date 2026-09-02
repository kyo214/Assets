using System;
using UnityEngine;

namespace Doozy.Runtime.UIDesigner;

public static class RectTransformExtensions
{
	public static Rect DesignerRect(this RectTransform target)
	{
		Rect rect = target.rect;
		Vector3 localEulerAngles = target.localEulerAngles;
		Vector3 localScale = target.localScale;
		Vector2 vector = new Vector2(Mathf.Abs(localScale.x), Mathf.Abs(localScale.y));
		float num = rect.width * vector.x;
		float num2 = rect.height * vector.y;
		float num3 = rect.x + rect.width / 2f - num / 2f;
		float num4 = rect.y + rect.height / 2f - num2 / 2f;
		float num5 = 0f;
		float num6 = 0f;
		float num7 = Mathf.Abs(localEulerAngles.z % 180f);
		float f = num7 * (MathF.PI / 180f);
		float num8;
		float num9;
		if (Mathf.Approximately(num7, 0f) || Mathf.Approximately(num7, 90f))
		{
			num8 = num;
			num9 = num2;
		}
		else if (num7 < 90f)
		{
			num8 = num * Mathf.Cos(f) + num2 * Mathf.Sin(f);
			num9 = num * Mathf.Sin(f) + num2 * Mathf.Cos(f);
		}
		else
		{
			num7 -= 90f;
			f = num7 * (MathF.PI / 180f);
			num8 = num2 * Mathf.Cos(f) + num * Mathf.Sin(f);
			num9 = num2 * Mathf.Sin(f) + num * Mathf.Cos(f);
		}
		num5 = (num8 - num) / 2f;
		num6 = (num9 - num2) / 2f;
		return new Rect(num3 + num5, num4 + num6, num8, num9);
	}

	public static RectTransform ChangePivot(this RectTransform target, Vector2 pivot)
	{
		Vector2 sizeDelta = target.sizeDelta;
		Vector3 localScale = target.localScale;
		Vector2 vector = target.pivot - pivot;
		float num = vector.x * sizeDelta.x * localScale.x;
		float num2 = vector.y * sizeDelta.y * localScale.y;
		float f = target.rotation.eulerAngles.z * MathF.PI / 180f;
		Vector3 vector2 = new Vector3(Mathf.Cos(f) * num - Mathf.Sin(f) * num2, Mathf.Sin(f) * num + Mathf.Cos(f) * num2);
		target.pivot = pivot;
		target.localPosition -= vector2;
		return target;
	}
}

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Toked;

public static class ScrollRectExtensions
{
	public static void SnapTo(this ScrollRect scrollRect, RectTransform target)
	{
		scrollRect.content.localPosition = CalculateSnapPosition(scrollRect, target);
		scrollRect.content.ForceUpdateRectTransforms();
	}

	public static void SmoothSnapTo(this ScrollRect scrollRect, RectTransform target)
	{
		scrollRect.content.DOKill();
		scrollRect.content.DOLocalMove(CalculateSnapPosition(scrollRect, target), 0.6f);
		scrollRect.content.ForceUpdateRectTransforms();
	}

	private static Vector2 CalculateSnapPosition(ScrollRect scrollRect, RectTransform target)
	{
		scrollRect.content.ForceUpdateRectTransforms();
		scrollRect.viewport.ForceUpdateRectTransforms();
		Vector2 vector = scrollRect.viewport.localPosition;
		Vector2 vector2 = target.localPosition;
		Vector2 vector3 = new Vector2(0f - (vector.x * scrollRect.viewport.localScale.x + vector2.x * scrollRect.content.localScale.x), 0f - (vector.y * scrollRect.viewport.localScale.y + vector2.y * scrollRect.content.localScale.y));
		scrollRect.content.localPosition = vector3;
		Rect rect = TransformRectFromTo(scrollRect.content.transform, scrollRect.viewport);
		float num = rect.xMin - scrollRect.viewport.rect.xMin;
		if (num > 0f)
		{
			vector3.x -= num;
		}
		float num2 = rect.xMax - scrollRect.viewport.rect.xMax;
		if (num2 < 0f)
		{
			vector3.x -= num2;
		}
		float num3 = rect.yMin - scrollRect.viewport.rect.yMin;
		if (num3 > 0f)
		{
			vector3.y -= num3;
		}
		float num4 = rect.yMax - scrollRect.viewport.rect.yMax;
		if (num4 < 0f)
		{
			vector3.y -= num4;
		}
		return vector3;
	}

	public static Rect TransformRectFromTo(Transform from, Transform to)
	{
		RectTransform component = from.GetComponent<RectTransform>();
		RectTransform component2 = to.GetComponent<RectTransform>();
		if (component != null && component2 != null)
		{
			Vector3[] array = new Vector3[4];
			Vector3[] array2 = new Vector3[4];
			Matrix4x4 worldToLocalMatrix = to.worldToLocalMatrix;
			component.GetWorldCorners(array);
			for (int i = 0; i < 4; i++)
			{
				array2[i] = worldToLocalMatrix.MultiplyPoint3x4(array[i]);
			}
			return new Rect(array2[0].x, array2[0].y, array2[2].x - array2[1].x, array2[1].y - array2[0].y);
		}
		return default;
	}
}

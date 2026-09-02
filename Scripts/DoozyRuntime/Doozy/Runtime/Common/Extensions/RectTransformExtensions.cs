using UnityEngine;

namespace Doozy.Runtime.Common.Extensions;

public static class RectTransformExtensions
{
	public static Rect ToScreenSpace(this RectTransform rectTransform)
	{
		Vector3 lossyScale = rectTransform.lossyScale;
		Vector2 vector = Vector2.Scale(rectTransform.rect.size, new Vector3(Mathf.Abs(lossyScale.x), Mathf.Abs(lossyScale.y), Mathf.Abs(lossyScale.z)));
		return new Rect((Vector2)rectTransform.position - vector * rectTransform.pivot, vector);
	}

	public static RectTransform Copy(this RectTransform target, RectTransform from)
	{
		target.localScale = from.localScale;
		target.anchorMin = from.anchorMin;
		target.anchorMax = from.anchorMax;
		target.pivot = from.pivot;
		target.sizeDelta = from.sizeDelta;
		target.anchoredPosition3D = from.anchoredPosition3D;
		return target;
	}

	public static RectTransform ExpandToParentSize(this RectTransform target, bool resetScaleToOne)
	{
		if (resetScaleToOne)
		{
			target.ResetLocalScaleToOne();
		}
		target.AnchorMinToZero();
		target.AnchorMaxToOne();
		target.CenterPivot();
		target.SizeDeltaToZero();
		target.ResetAnchoredPosition3D();
		target.ResetLocalPosition();
		return target;
	}

	public static RectTransform Center(this RectTransform target, bool resetScaleToOne)
	{
		if (resetScaleToOne)
		{
			target.ResetLocalScaleToOne();
		}
		target.AnchorMinToCenter();
		target.AnchorMaxToCenter();
		target.CenterPivot();
		target.SizeDeltaToZero();
		return target;
	}

	public static RectTransform ResetAnchoredPosition3D(this RectTransform target)
	{
		target.anchoredPosition3D = Vector3.zero;
		return target;
	}

	public static RectTransform ResetLocalPosition(this RectTransform target)
	{
		target.localPosition = Vector3.zero;
		return target;
	}

	public static RectTransform ResetLocalScaleToOne(this RectTransform target)
	{
		target.localScale = Vector3.one;
		return target;
	}

	public static RectTransform AnchorMinToZero(this RectTransform target)
	{
		target.anchorMin = Vector2.zero;
		return target;
	}

	public static RectTransform AnchorMinToCenter(this RectTransform target)
	{
		target.anchorMin = new Vector2(0.5f, 0.5f);
		return target;
	}

	public static RectTransform AnchorMaxToOne(this RectTransform target)
	{
		target.anchorMax = Vector2.one;
		return target;
	}

	public static RectTransform AnchorMaxToCenter(this RectTransform target)
	{
		target.anchorMax = new Vector2(0.5f, 0.5f);
		return target;
	}

	public static RectTransform CenterPivot(this RectTransform target)
	{
		target.pivot = new Vector2(0.5f, 0.5f);
		return target;
	}

	public static RectTransform SizeDeltaToZero(this RectTransform target)
	{
		target.sizeDelta = Vector2.zero;
		return target;
	}
}

using UnityEngine;

namespace Toked;

public static class RectTransformExtensions
{
	public enum PivotPresets
	{
		TopLeft = 0,
		TopCenter = 1,
		TopRight = 2,
		MiddleLeft = 3,
		MiddleCenter = 4,
		MiddleRight = 5,
		BottomLeft = 6,
		BottomCenter = 7,
		BottomRight = 8
	}

	public static Vector2 GetGUIElementOffset(this RectTransform rect)
	{
		Rect rect2 = new Rect(0f, 0f, Screen.width, Screen.height);
		Vector3[] array = new Vector3[4];
		rect.GetWorldCorners(array);
		Vector2 result = new Vector2(0f, 0f);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].x < rect2.xMin)
			{
				result.x = rect2.xMin - array[i].x;
			}
			if (array[i].x > rect2.xMax)
			{
				result.x = rect2.xMax - array[i].x;
			}
			if (array[i].y < rect2.yMin)
			{
				result.y = rect2.yMin - array[i].y;
			}
			if (array[i].y > rect2.yMax)
			{
				result.y = rect2.yMax - array[i].y;
			}
		}
		return result;
	}

	private static int CountCornersVisibleFrom(this RectTransform rectTransform, Camera camera = null)
	{
		Rect rect = new Rect(0f, 0f, Screen.width, Screen.height);
		Vector3[] array = new Vector3[4];
		rectTransform.GetWorldCorners(array);
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 point;
			if (camera != null)
			{
				point = camera.WorldToScreenPoint(array[i]);
			}
			else
			{
				Debug.Log(rectTransform.gameObject.name + " :: " + array[i].ToString("F2"));
				point = array[i];
			}
			if (rect.Contains(point))
			{
				num++;
			}
		}
		return num;
	}

	public static bool IsFullyVisibleFrom(this RectTransform rectTransform, Camera camera = null)
	{
		if (!rectTransform.gameObject.activeInHierarchy)
		{
			return false;
		}
		return rectTransform.CountCornersVisibleFrom(camera) == 4;
	}

	public static bool IsVisibleFrom(this RectTransform rectTransform, Camera camera = null)
	{
		if (!rectTransform.gameObject.activeInHierarchy)
		{
			return false;
		}
		return rectTransform.CountCornersVisibleFrom(camera) > 0;
	}

	public static bool WorldSpaceOverlaps(this RectTransform overlaping, RectTransform overlaped)
	{
		Vector3[] array = new Vector3[4];
		overlaping.GetWorldCorners(array);
		Rect other = new Rect(array[0], array[2] - array[0]);
		overlaped.GetWorldCorners(array);
		return new Rect(array[0], array[2] - array[0]).Overlaps(other, allowInverse: true);
	}

	public static void SetPivot(this RectTransform target, PivotPresets preset)
	{
		target.SetPivot(GetVector2FromPivot(preset));
	}

	public static void SetPivot2(this RectTransform target, PivotPresets preset)
	{
		target.SetPivot2(GetVector2FromPivot(preset));
	}

	private static void SetPivot(this RectTransform target, Vector2 pivot)
	{
		if (!(target == null))
		{
			Vector2 vector = pivot - target.pivot;
			vector.Scale(target.rect.size);
			Vector3 position = target.position + target.TransformVector(vector);
			target.pivot = pivot;
			target.position = position;
		}
	}

	private static void SetPivot2(this RectTransform rect, Vector2 pivot)
	{
		if (!(rect == null))
		{
			Vector3 vector = rect.pivot - pivot;
			vector.Scale(rect.rect.size);
			vector.Scale(rect.localScale);
			vector = rect.transform.localRotation * vector;
			rect.pivot = pivot;
			rect.localPosition -= vector;
		}
	}

	private static Vector2 GetVector2FromPivot(PivotPresets preset)
	{
		return preset switch
		{
			PivotPresets.TopLeft => new Vector2(0f, 1f), 
			PivotPresets.TopCenter => new Vector2(0.5f, 1f), 
			PivotPresets.TopRight => new Vector2(1f, 1f), 
			PivotPresets.MiddleLeft => new Vector2(0f, 0.5f), 
			PivotPresets.MiddleCenter => new Vector2(0.5f, 0.5f), 
			PivotPresets.MiddleRight => new Vector2(1f, 0.5f), 
			PivotPresets.BottomLeft => new Vector2(0f, 0f), 
			PivotPresets.BottomCenter => new Vector2(0.5f, 0f), 
			PivotPresets.BottomRight => new Vector2(1f, 0f), 
			_ => default, 
		};
	}
}

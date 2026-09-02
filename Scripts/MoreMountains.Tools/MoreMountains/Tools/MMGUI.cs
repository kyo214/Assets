using UnityEngine;
using UnityEngine.EventSystems;

namespace MoreMountains.Tools;

public static class MMGUI
{
	public static void SetSize(RectTransform rectTransform, Vector2 newSize)
	{
		Vector2 size = rectTransform.rect.size;
		Vector2 vector = newSize - size;
		rectTransform.offsetMin -= new Vector2(vector.x * rectTransform.pivot.x, vector.y * rectTransform.pivot.y);
		rectTransform.offsetMax += new Vector2(vector.x * (1f - rectTransform.pivot.x), vector.y * (1f - rectTransform.pivot.y));
	}

	public static bool PointOrTouchBlockedByUI()
	{
		if (EventSystem.current.IsPointerOverGameObject())
		{
			return true;
		}
		if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began && EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
		{
			return true;
		}
		return false;
	}

	public static Texture2D MakeTex(int width, int height, Color color)
	{
		Color[] array = new Color[width * height];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = color;
		}
		Texture2D texture2D = new Texture2D(width, height);
		texture2D.SetPixels(array);
		texture2D.Apply();
		return texture2D;
	}
}

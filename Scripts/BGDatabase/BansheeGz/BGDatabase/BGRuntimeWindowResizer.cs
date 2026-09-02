using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGRuntimeWindowResizer
{
	private enum ResizeMode
	{
		None = 0,
		Horizontal = 1,
		Vertical = 2,
		Diagonal = 3
	}

	private const int MinWidth = 200;

	private const int MinHeight = 100;

	private Texture2D resizeIcon;

	private readonly BGRuntimeWindow window;

	private ResizeMode resizeMode;

	private Vector2 draggingStartPosition;

	private Texture2D ResizeIcon
	{
		get
		{
			if (resizeIcon != null)
			{
				return resizeIcon;
			}
			resizeIcon = BGRTStyle.Resizer;
			return resizeIcon;
		}
	}

	public BGRuntimeWindowResizer(BGRuntimeWindow window)
	{
		this.window = window;
	}

	public void Process()
	{
		if (window.WindowParameters.resizingIsDisabled)
		{
			return;
		}
		if (Event.current.type == EventType.MouseUp)
		{
			resizeMode = ResizeMode.None;
		}
		Vector2 mousePosition = Event.current.mousePosition;
		Rect area = window.Area;
		Texture2D texture2D = ResizeIcon;
		Rect rect = new Rect(area);
		rect.x = area.xMax - (float)texture2D.width * 0.5f;
		rect.y = area.yMax - (float)texture2D.height * 0.5f;
		rect.width = texture2D.width;
		rect.height = texture2D.height;
		if (rect.Contains(mousePosition))
		{
			DrawResizeCursor(mousePosition, texture2D);
			if (Event.current.type == EventType.MouseDown)
			{
				resizeMode = ResizeMode.Diagonal;
			}
			Event.current.Use();
		}
		else
		{
			Cursor.visible = true;
		}
		switch (resizeMode)
		{
		case ResizeMode.Horizontal:
			AdjustWidth(mousePosition);
			break;
		case ResizeMode.Vertical:
			AdjustHeight(mousePosition);
			break;
		case ResizeMode.Diagonal:
			AdjustWidth(mousePosition);
			AdjustHeight(mousePosition);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case ResizeMode.None:
			break;
		}
	}

	private void AdjustHeight(Vector2 mousePosition)
	{
		if (mousePosition.y - window.Area.y > 100f)
		{
			window.Area = new Rect(window.Area)
			{
				height = mousePosition.y - window.Area.y
			};
		}
	}

	private void AdjustWidth(Vector2 mousePosition)
	{
		if (mousePosition.x - window.Area.x > 200f)
		{
			window.Area = new Rect(window.Area)
			{
				width = mousePosition.x - window.Area.x
			};
		}
	}

	public static void DrawResizeCursor(Vector2 mousePosition, Texture2D texture)
	{
		Cursor.visible = false;
		GUI.DrawTexture(new Rect
		{
			x = mousePosition.x - (float)texture.width * 0.5f,
			y = mousePosition.y - (float)texture.height * 0.5f,
			width = texture.width,
			height = texture.height
		}, texture);
	}
}

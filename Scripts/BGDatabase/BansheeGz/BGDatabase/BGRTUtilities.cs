using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public static class BGRTUtilities
{
	public static int MinHeight = 22;

	public static readonly GUILayoutOption[] OptionsMinRect = GetOptions(MinHeight, MinHeight);

	public static bool EventIsRepaint => Event.current.type == EventType.Repaint;

	public static void Vertical(Action callback, params GUILayoutOption[] options)
	{
		GUILayout.BeginVertical(options);
		callback();
		GUILayout.EndVertical();
	}

	public static void Vertical(GUIStyle style, Action callback)
	{
		GUILayout.BeginVertical(style);
		callback();
		GUILayout.EndVertical();
	}

	public static void Vertical(GUIStyle style, Action callback, params GUILayoutOption[] options)
	{
		GUILayout.BeginVertical(style, options);
		callback();
		GUILayout.EndVertical();
	}

	public static void Horizontal(Action callback, params GUILayoutOption[] options)
	{
		GUILayout.BeginHorizontal(options);
		callback();
		GUILayout.EndHorizontal();
	}

	public static void Horizontal(GUIStyle style, Action callback)
	{
		GUILayout.BeginHorizontal(style);
		callback();
		GUILayout.EndHorizontal();
	}

	public static void Horizontal(GUIStyle style, Action callback, params GUILayoutOption[] options)
	{
		GUILayout.BeginHorizontal(style, options);
		callback();
		GUILayout.EndHorizontal();
	}

	public static GUILayoutOption[] GetOptions(float width, float height)
	{
		return new GUILayoutOption[2]
		{
			GUILayout.Width(width),
			GUILayout.Height(height)
		};
	}

	public static T GetAttribute<T>(Type type) where T : Attribute
	{
		return (T)Attribute.GetCustomAttribute(type, typeof(T));
	}

	public static bool Log(bool condition, string message, params object[] parameters)
	{
		if (!condition)
		{
			return false;
		}
		Debug.Log(BGUtil.Format(message, parameters));
		return true;
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

	public static void Label(string label, int width)
	{
		Label(label, GUILayout.Width(width));
	}

	public static void Label(string label)
	{
		Label(label, (GUILayoutOption[])null);
	}

	public static void Label(string label, params GUILayoutOption[] @params)
	{
		GUILayout.Label(label, BGRTStyle.Editor_label, @params);
	}

	public static void Label(GUIContent label, int width)
	{
		Label(label, GUILayout.Width(width));
	}

	public static void Label(GUIContent label)
	{
		Label(label, (GUILayoutOption[])null);
	}

	public static void Label(GUIContent label, params GUILayoutOption[] @params)
	{
		GUILayout.Label(label, BGRTStyle.Editor_label, @params);
	}

	public static bool Button(string message, int width = 0)
	{
		if (width != 0)
		{
			return GUILayout.Button(message, BGRTStyle.Button, GUILayout.Width(width));
		}
		return GUILayout.Button(message, BGRTStyle.Button);
	}

	public static void ResetHotControl()
	{
		GUIUtility.hotControl = 0;
		GUIUtility.keyboardControl = 0;
	}

	public static void Try(Action action)
	{
		try
		{
			action();
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
			BGRTDialog.Info("Error! " + ex.Message);
		}
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGDatabaseMonitorGo : MonoBehaviour
{
	public int fontSize = 14;

	public BGRuntimeWindow.BGWindowParameters windowParameters;

	public BGRuntimeWindow.BGHotKey showHideKey;

	public BGRuntimeWindow.BGHotKey minimizeMaximizeKey;

	public static BGDatabaseMonitorGo I;

	private static Vector2 lastMousePosition;

	public static float LabelHeight;

	private BGRuntimeWindow window;

	public static bool Disabled;

	private int page;

	private BGRTDropDownList<BGMonitorPage> metaDropDown;

	private int oldFonSize;

	private void Start()
	{
		if (!(I != null))
		{
			I = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			ResetFont();
			window = new BGRuntimeWindow(MyGui, windowParameters, minimizeMaximizeKey, showHideKey, null);
			List<BGMonitorPage> list = new List<BGMonitorPage>
			{
				new BGMonitorPageHome(),
				new BGMonitorPageLiveUpdateStatus()
			};
			metaDropDown = new BGRTDropDownList<BGMonitorPage>((BGMonitorPage page) => GUILayout.Button(page.Name, BGRTStyle.Button), (BGMonitorPage page) => GUILayout.Button(page.Name, BGRTStyle.Button), () => list)
			{
				Current = list[0]
			};
		}
	}

	private void ResetFont()
	{
		oldFonSize = fontSize;
		GUIStyle gUIStyle = new GUIStyle("label")
		{
			fontSize = fontSize
		};
		LabelHeight = gUIStyle.CalcSize(new GUIContent("Q")).y;
		BGRTUtilities.MinHeight = (int)LabelHeight;
		BGRTStyle.Reset();
	}

	private void OnGUI()
	{
		if (!(I != this) && !Disabled)
		{
			if (oldFonSize != fontSize)
			{
				ResetFont();
			}
			lastMousePosition = Event.current.mousePosition;
			window.Gui();
		}
	}

	private void OnDestroy()
	{
		if (!(I != this))
		{
			I = null;
			Disabled = false;
			BGRTPopup.Reset();
		}
	}

	private void MyGui()
	{
		if (!BGRepo.Ok)
		{
			BGRTUtilities.Label("Database error: " + BGRepo.DefaultRepoErrorOnLoad);
			return;
		}
		BGRTUtilities.Horizontal(() =>
		{
			BGRTUtilities.Label("Database is loaded ok!");
			GUILayout.Space(4f);
			BGRTUtilities.Label("Tools >>");
			metaDropDown.Gui();
			GUILayout.FlexibleSpace();
		});
		metaDropDown.Current.Gui();
	}

	public static void Popup(int width, int height, string title, Func<bool> action, Action onClose = null)
	{
		float x = ((!((double)lastMousePosition.x < (double)Screen.width * 0.5)) ? (lastMousePosition.x - (float)width) : lastMousePosition.x);
		float y = ((!((double)lastMousePosition.y < (double)Screen.height * 0.5)) ? (lastMousePosition.y - (float)height) : lastMousePosition.y);
		BGRTPopup.Popup(new Rect(new Vector2(x, y), new Vector2(width, height)), title, action, onClose);
	}
}

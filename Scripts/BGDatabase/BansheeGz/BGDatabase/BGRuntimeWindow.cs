using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGRuntimeWindow
{
	[Serializable]
	public class BGWindowParameters
	{
		[Tooltip("Screen rectangle for editor window")]
		public Rect area = new Rect(100f, 100f, 670f, 400f);

		[Tooltip("Should editor be minimized on start")]
		public bool minimized;

		[Tooltip("Should editor be hidden on start")]
		public bool closed;

		[Tooltip("Windows can not be moved if it's true")]
		public bool movingIsDisabled;

		[Tooltip("Windows can not be resized if it's true")]
		public bool resizingIsDisabled;
	}

	[Serializable]
	public class BGHotKey
	{
		[Tooltip("Shortcut key")]
		public KeyCode key;

		[Tooltip("Should Shift key also be pressed")]
		public bool keyShift;

		[Tooltip("Should Ctrl key also be pressed")]
		public bool keyCtrl;

		[Tooltip("Should Alt key also be pressed")]
		public bool keyAlt;

		public bool Pressed
		{
			get
			{
				if (key == KeyCode.None)
				{
					return false;
				}
				if (Event.current.type != EventType.KeyDown)
				{
					return false;
				}
				if (Event.current.keyCode != key)
				{
					return false;
				}
				if (keyShift && !Event.current.shift)
				{
					return false;
				}
				if (keyCtrl && !Event.current.control)
				{
					return false;
				}
				if (keyAlt && !Event.current.alt)
				{
					return false;
				}
				return true;
			}
		}
	}

	private bool dragging;

	private Vector2 dragPosition;

	private readonly Queue<Action> contentActions = new Queue<Action>();

	private readonly BGWindowParameters windowParameters;

	private readonly BGRuntimeWindowResizer windowResizer;

	private readonly BGHotKey minimizeKey;

	private readonly BGHotKey showHideKey;

	private Action contentAction;

	private readonly Action onMouseUp;

	internal BGWindowParameters WindowParameters => windowParameters;

	public Rect Area
	{
		get
		{
			return windowParameters.area;
		}
		set
		{
			windowParameters.area = value;
		}
	}

	private bool InsideTitle
	{
		get
		{
			Rect rect = new Rect(windowParameters.area);
			rect.height = BGRTUtilities.MinHeight;
			rect.width = windowParameters.area.width - (float)(BGRTUtilities.MinHeight * 2);
			return rect.Contains(Event.current.mousePosition);
		}
	}

	public BGRuntimeWindow(Action contentAction, BGWindowParameters windowParameters, BGHotKey minimizeKey, BGHotKey showHideKey, Action onMouseUp)
	{
		this.contentAction = contentAction;
		this.windowParameters = windowParameters;
		this.minimizeKey = minimizeKey;
		this.showHideKey = showHideKey;
		this.onMouseUp = onMouseUp;
		windowResizer = new BGRuntimeWindowResizer(this);
	}

	public void Gui()
	{
		using (BGRTUsing.CursorColor(Color.black))
		{
			using (BGRTUsing.DisableGUI(BGRTPopup.Active))
			{
				ProcessInput();
				Window();
				AfterGui();
			}
			BGRTPopup.Gui();
		}
	}

	private void Window()
	{
		if (windowParameters.closed)
		{
			return;
		}
		Rect screenRect;
		if (windowParameters.minimized)
		{
			Rect rect = new Rect(windowParameters.area);
			rect.height = BGRTUtilities.MinHeight;
			rect.width = 200f;
			screenRect = rect;
		}
		else
		{
			screenRect = windowParameters.area;
		}
		GUILayout.BeginArea(screenRect);
		Title();
		if (!windowParameters.minimized)
		{
			BGRTUtilities.Vertical(BGRTStyle.Box, () =>
			{
				contentAction();
			}, GUILayout.ExpandWidth(expand: true), GUILayout.ExpandHeight(expand: true));
		}
		GUILayout.EndArea();
	}

	private void Title()
	{
		BGRTUtilities.Horizontal(() =>
		{
			float labelHeight = BGDatabaseMonitorGo.LabelHeight;
			GUILayout.Label(GetHeader(), BGRTStyle.WindowTitle, GUILayout.Height(labelHeight));
			Rect lastRect = GUILayoutUtility.GetLastRect();
			if (GUI.Button(new Rect(lastRect)
			{
				x = lastRect.xMax - labelHeight * 2f + 1f,
				width = labelHeight - 2f,
				y = lastRect.y + 1f,
				height = lastRect.yMax - 2f
			}, windowParameters.minimized ? "O" : "_", BGRTStyle.Button))
			{
				windowParameters.minimized = !windowParameters.minimized;
			}
			if (GUI.Button(new Rect(lastRect)
			{
				x = lastRect.xMax - labelHeight + 1f,
				width = labelHeight - 2f,
				y = lastRect.y + 1f,
				height = lastRect.yMax - 2f
			}, "X", BGRTStyle.Button))
			{
				windowParameters.closed = true;
			}
		});
	}

	private string GetHeader()
	{
		return "BGDatabaseMonitor";
	}

	private void ProcessInput()
	{
		if (!windowParameters.closed)
		{
			Vector2 mousePosition = Event.current.mousePosition;
			if (Event.current.type == EventType.MouseDown && InsideTitle && !windowParameters.movingIsDisabled)
			{
				dragging = true;
				dragPosition = mousePosition;
			}
			if (Event.current.type == EventType.MouseUp)
			{
				if (dragging)
				{
					dragging = false;
				}
				else
				{
					onMouseUp?.Invoke();
				}
			}
			if (dragging)
			{
				Vector2 vector = mousePosition - dragPosition;
				windowParameters.area.x += vector.x;
				windowParameters.area.y += vector.y;
				dragPosition = mousePosition;
			}
		}
		if (minimizeKey.Pressed)
		{
			windowParameters.minimized = !windowParameters.minimized;
		}
		if (showHideKey.Pressed)
		{
			windowParameters.closed = !windowParameters.closed;
		}
	}

	private void AfterGui()
	{
		if (!windowParameters.closed && !windowParameters.minimized)
		{
			windowResizer.Process();
		}
	}

	public void Push(Action gui)
	{
		contentActions.Enqueue(contentAction);
		contentAction = gui;
	}

	public void Pop()
	{
		contentAction = contentActions.Dequeue();
	}
}

using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGRTUsing
{
	private struct BGDisabledGUI : IDisposable
	{
		private readonly bool oldEnabled;

		private readonly bool condition;

		internal BGDisabledGUI(bool condition)
		{
			this.condition = condition;
			if (condition)
			{
				oldEnabled = GUI.enabled;
				GUI.enabled = false;
			}
			else
			{
				oldEnabled = true;
			}
		}

		public void Dispose()
		{
			if (condition)
			{
				GUI.enabled = oldEnabled;
			}
		}
	}

	private struct BGCursorColor : IDisposable
	{
		private readonly Color color = GUI.skin.settings.cursorColor;

		internal BGCursorColor(Color color)
		{
			GUI.skin.settings.cursorColor = color;
		}

		public void Dispose()
		{
			GUI.skin.settings.cursorColor = color;
		}
	}

	public static IDisposable DisableGUI(bool condition)
	{
		return new BGDisabledGUI(condition);
	}

	public static IDisposable CursorColor(Color color)
	{
		return new BGCursorColor(color);
	}
}

using System;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGRTScrollView
{
	private Vector2 position;

	private readonly Action view;

	private readonly bool alwaysShowHorizontal;

	private readonly bool alwaysShowVertical;

	public Vector2 Position
	{
		get
		{
			return position;
		}
		set
		{
			position = value;
			OnChange?.Invoke();
		}
	}

	public event Action OnChange;

	public BGRTScrollView(Action view, bool alwaysShowHorizontal = false, bool alwaysShowVertical = false)
	{
		this.view = view;
		this.alwaysShowHorizontal = alwaysShowHorizontal;
		this.alwaysShowVertical = alwaysShowVertical;
	}

	public void Gui()
	{
		BeginScroll();
		view();
		GUILayout.EndScrollView();
	}

	private void BeginScroll(params GUILayoutOption[] options)
	{
		Vector2 vector = GUILayout.BeginScrollView(position, alwaysShowHorizontal, alwaysShowVertical, options);
		if (OnChange != null && vector != position)
		{
			position = vector;
			OnChange();
		}
		else
		{
			position = vector;
		}
	}
}

using System;
using UnityEngine;

namespace Doozy.Runtime.Colors;

public class SelectableColor
{
	private bool m_IsDarkTheme;

	public readonly ColorEvent onStateChanged;

	public readonly ThemeColor Normal;

	public readonly ThemeColor Highlighted;

	public readonly ThemeColor Pressed;

	public readonly ThemeColor Selected;

	public readonly ThemeColor Disabled;

	public bool isDarkTheme
	{
		get
		{
			return m_IsDarkTheme;
		}
		set
		{
			m_IsDarkTheme = value;
			Normal.isDarkTheme = m_IsDarkTheme;
			Highlighted.isDarkTheme = m_IsDarkTheme;
			Pressed.isDarkTheme = m_IsDarkTheme;
			Selected.isDarkTheme = m_IsDarkTheme;
			Disabled.isDarkTheme = m_IsDarkTheme;
			SelectionStateChanged(currentColor);
		}
	}

	public SelectionState currentState { get; private set; }

	public Color currentColor => GetCurrentColor();

	public Color normalColor => Normal.color;

	public Color highlightedColor => Highlighted.color;

	public Color pressedColor => Pressed.color;

	public Color selectedColor => Selected.color;

	public Color disabledColor => Disabled.color;

	public SelectableColor(ColorEvent onStateChanged = null)
	{
		Normal = new ThemeColor
		{
			isDarkTheme = isDarkTheme
		};
		Highlighted = new ThemeColor
		{
			isDarkTheme = isDarkTheme
		};
		Pressed = new ThemeColor
		{
			isDarkTheme = isDarkTheme
		};
		Selected = new ThemeColor
		{
			isDarkTheme = isDarkTheme
		};
		Disabled = new ThemeColor
		{
			isDarkTheme = isDarkTheme
		};
		this.onStateChanged = onStateChanged ?? new ColorEvent();
		SetSelectionState(SelectionState.Normal);
	}

	private Color GetCurrentColor()
	{
		return currentState switch
		{
			SelectionState.Normal => normalColor, 
			SelectionState.Highlighted => highlightedColor, 
			SelectionState.Pressed => pressedColor, 
			SelectionState.Selected => selectedColor, 
			SelectionState.Disabled => disabledColor, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	internal void SetSelectionState(SelectionState state)
	{
		currentState = state;
		SelectionStateChanged(currentColor);
	}

	internal void SelectionStateChanged(Color color)
	{
		onStateChanged?.Invoke(color);
	}
}

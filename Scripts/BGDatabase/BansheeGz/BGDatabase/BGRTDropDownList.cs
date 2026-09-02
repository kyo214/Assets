using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public class BGRTDropDownList<T>
{
	protected Func<List<T>> Provider;

	private Func<T, bool> gui;

	private Func<T, bool> itemGui;

	private T current;

	public bool ManualClosing;

	public T Current
	{
		get
		{
			return current;
		}
		set
		{
			current = value;
		}
	}

	public event Action OnChange;

	public BGRTDropDownList(Func<T, bool> gui, Func<T, bool> itemGui, Func<List<T>> provider)
	{
		this.gui = gui;
		this.itemGui = itemGui;
		Provider = provider;
	}

	public List<T> Provide()
	{
		return Provider();
	}

	public void Gui()
	{
		if (!gui(current))
		{
			return;
		}
		List<T> list = Provider();
		bool exit = false;
		BGRTScrollView scrollView = new BGRTScrollView(() =>
		{
			if (list == null || list.Count == 0)
			{
				GUILayout.Label("No data", BGRTStyle.Editor_label);
				return;
			}
			foreach (T item in list)
			{
				if (itemGui(item))
				{
					current = item;
					exit = true;
					OnChange?.Invoke();
					break;
				}
			}
		});
		BGDatabaseMonitorGo.Popup(400, 300, "Choose value", () =>
		{
			scrollView.Gui();
			return !ManualClosing & exit;
		});
	}
}

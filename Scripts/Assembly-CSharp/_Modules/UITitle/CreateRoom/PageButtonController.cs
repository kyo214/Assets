using System;
using System.Collections.Generic;
using Doozy.Runtime.UIManager.Components;
using UnityEngine;

namespace _Modules.UITitle.CreateRoom;

public class PageButtonController : MonoBehaviour
{
	[SerializeField]
	private PageButtonUI _uiTogglePrefab;

	[SerializeField]
	private UIToggleGroup _toggleGroup;

	[SerializeField]
	private List<PageButtonUI> _uiToggleList;

	private int _currentActiveButtonIndex;

	private int _lastActiveButtonIndex;

	private int _totalPage;

	public int CurrentActiveButtonIndex => _currentActiveButtonIndex;

	public int TotalPage => _totalPage;

	public void Init(int item, Action<int> onClickAction, List<bool> disableList, List<bool> lockList)
	{
		_totalPage = item;
		if (disableList.Count > 0)
		{
			for (int i = 0; i < item; i++)
			{
				InitButton(i, onClickAction, disableList[i], lockList[i]);
			}
		}
		DisableToggleUI(item);
	}

	private void InitButton(int index, Action<int> onClickAction, bool disableImage, bool lockImage)
	{
		PageButtonUI toggleUI = GetToggleUI(index);
		toggleUI.SetToggleGroup(_toggleGroup);
		toggleUI.Init(() =>
		{
			onClickAction(index);
		}, disableImage, lockImage);
	}

	public PageButtonUI GetToggleUI(int index)
	{
		if (index >= _uiToggleList.Count)
		{
			PageButtonUI pageButtonUI = UnityEngine.Object.Instantiate(_uiTogglePrefab, _toggleGroup.transform);
			_uiToggleList.Add(pageButtonUI);
			return pageButtonUI;
		}
		PageButtonUI pageButtonUI2 = _uiToggleList[index];
		pageButtonUI2.SetActive(active: true);
		return pageButtonUI2;
	}

	private void DisableToggleUI(int index)
	{
		_lastActiveButtonIndex = index;
		int num = _uiToggleList.Count - 1;
		for (int i = index; i <= num; i++)
		{
			PageButtonUI pageButtonUI = _uiToggleList[i];
			if ((bool)pageButtonUI)
			{
				pageButtonUI.SetActive(active: false);
			}
		}
	}

	public void SetToggleOn(int index)
	{
		_currentActiveButtonIndex = index;
		GetToggleUI(index).SetActiveToggle(active: true);
	}
}

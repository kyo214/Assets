using System;
using System.Collections.Generic;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Modules.UITitle.CreateRoom;

public class CreateQuickJoinPanelUI : MonoBehaviour
{
	[SerializeField]
	private UIView _uiView;

	[SerializeField]
	private UIButton _startButton;

	[SerializeField]
	private UIButton _closeButton;

	[SerializeField]
	private List<CreateGameSettingPanelBase> _listPanel = new List<CreateGameSettingPanelBase>();

	private GameObject _lastSelectedButton;

	private bool _isLoad;

	private void Start()
	{
		_closeButton.onClickEvent.AddListener(OnClickCloseButton);
		_startButton.onClickEvent.AddListener(OnClickStartButton);
		InitOnValuePanelChanged();
	}

	private void OnDestroy()
	{
		RemoveOnValuePanelChanged();
	}

	public void InitOnValuePanelChanged()
	{
		foreach (CreateGameSettingPanelBase item in _listPanel)
		{
			item.OnChangeValueEvent = (Action<bool>)Delegate.Combine(item.OnChangeValueEvent, new Action<bool>(OnValuePanelChangedAction));
		}
	}

	public void RemoveOnValuePanelChanged()
	{
		foreach (CreateGameSettingPanelBase item in _listPanel)
		{
			item.OnChangeValueEvent = (Action<bool>)Delegate.Remove(item.OnChangeValueEvent, new Action<bool>(OnValuePanelChangedAction));
		}
	}

	public void Show(GameObject lastSelectedButton)
	{
		_lastSelectedButton = lastSelectedButton;
		EventSystem.current.SetSelectedGameObject(null);
		_uiView.Show();
	}

	private void OnClickCloseButton()
	{
	}

	private void OnClickStartButton()
	{
		foreach (CreateGameSettingPanelBase item in _listPanel)
		{
			item?.SetDataWhenCreateGame(_isLoad);
		}
		UITitleMenuManager.Instance.ClickAutoJoinCreate();
	}

	public void SetLoad(bool isLoad)
	{
		_isLoad = isLoad;
	}

	private void OnValuePanelChangedAction(bool valid)
	{
		_startButton.interactable = GetValueValid();
		bool GetValueValid()
		{
			foreach (CreateGameSettingPanelBase item in _listPanel)
			{
				if (!item.IsCurrentValueValid)
				{
					return false;
				}
			}
			return true;
		}
	}
}

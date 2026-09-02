using System;
using System.Collections.Generic;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Modules.UITitle.CreateRoom;

public class CreateRoomPanelUI : MonoBehaviour
{
	[SerializeField]
	private UIView _uiView;

	[SerializeField]
	private UIButton _startButton;

	[SerializeField]
	private UIButton _closeButton;

	[SerializeField]
	private List<CreateGameSettingPanelBase> _listPanel = new List<CreateGameSettingPanelBase>();

	[SerializeField]
	private UIButton _uiSelectedHiddenView;

	[SerializeField]
	private UIButton _uiSelectedSaveHiddenView;

	private GameObject _lastSelectedButton;

	private bool _isLoad;

	private void Start()
	{
		_uiView.OnHiddenCallback.Event.AddListener(OnHiddenAction);
		_closeButton.onClickEvent.AddListener(OnClickCloseButton);
		_startButton.onClickEvent.AddListener(OnClickStartButton);
		InitOnValuePanelChanged();
	}

	private void OnHiddenAction()
	{
		if (GameModes.Instance.CheckDisableSaveData())
		{
			_uiSelectedHiddenView?.Select();
		}
		else
		{
			_uiSelectedSaveHiddenView?.Select();
		}
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

	public void SetLoad(bool isLoad)
	{
		_isLoad = isLoad;
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
		AudioManager.PlaySFX("ui_gamestart");
		foreach (CreateGameSettingPanelBase item in _listPanel)
		{
			item?.SetDataWhenCreateGame(_isLoad);
		}
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

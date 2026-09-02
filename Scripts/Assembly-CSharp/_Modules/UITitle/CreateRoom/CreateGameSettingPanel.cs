using System;
using System.Collections.Generic;
using DG.Tweening;
using Doozy.Runtime.UIManager.Components;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Modules.UITitle.CreateRoom;

public class CreateGameSettingPanel<T> : CreateGameSettingPanelBase
{
	[SerializeField]
	protected UIButton _uiSelectable;

	[SerializeField]
	protected TMP_Text _valueText;

	[SerializeField]
	protected Localize _valueLocalize;

	[SerializeField]
	protected UIButton _leftButton;

	[SerializeField]
	protected UIButton _rightButton;

	[SerializeField]
	private PageButtonController _pageButtonController;

	protected int _index = 1;

	protected int _itemCount = 1;

	protected List<T> _listData = new List<T>();

	private InputAction _playerInputActions;

	public UIButton UISelectable => _uiSelectable;

	private void Start()
	{
		Init();
		_leftButton.onClickEvent.AddListener(OnClickLeftButton);
		_rightButton.onClickEvent.AddListener(OnClickRightButton);
		_playerInputActions = UITitleMenuManager.Instance.playerInput.actions.FindAction("UI/Navigate");
		_pageButtonController?.Init(_itemCount, SetValue, GetDisableData(), GetLockData());
		OnChangeValueEvent = (Action<bool>)Delegate.Combine(OnChangeValueEvent, new Action<bool>(OnChangeValueEventAction));
	}

	private void OnDestroy()
	{
		OnChangeValueEvent = (Action<bool>)Delegate.Remove(OnChangeValueEvent, new Action<bool>(OnChangeValueEventAction));
	}

	protected virtual void OnChangeValueEventAction(bool isValidData)
	{
		if (isValidData)
		{
			_valueText.DOFade(1f, 0f);
		}
		else
		{
			_valueText.DOFade(0.3f, 0f);
		}
	}

	private void Update()
	{
		InputUpdateFunction();
	}

	protected virtual void Init()
	{
		InitDataList();
		_itemCount = _listData.Count;
		SetCurrentValue();
	}

	protected virtual void InitDataList()
	{
	}

	private void OnClickLeftButton()
	{
		_index--;
		_index = ((_index >= 0) ? _index : 0);
		SetCurrentValue();
	}

	private void OnClickRightButton()
	{
		_index++;
		_index = ((_index >= _itemCount) ? (_itemCount - 1) : _index);
		SetCurrentValue();
	}

	public virtual void SetValue(int index)
	{
		_index = index;
		SetCurrentValue();
	}

	protected virtual void SetCurrentValue()
	{
		_valueText.text = "";
		_valueLocalize.SetTerm(GetTermValue());
		_pageButtonController?.SetToggleOn(_index);
		OnValueChangedAction(_index);
	}

	protected virtual string GetTermValue()
	{
		return "";
	}

	protected virtual List<bool> GetDisableData()
	{
		return new List<bool>(_itemCount);
	}

	protected virtual List<bool> GetLockData()
	{
		return new List<bool>(_itemCount);
	}

	public override void OnValueChangedAction(int index)
	{
	}

	public override void SetDataWhenCreateGame(bool isLoad)
	{
	}

	private void InputUpdateFunction()
	{
		if (_uiSelectable.isSelected)
		{
			if (_playerInputActions.WasPressedThisFrame() && _playerInputActions.ReadValue<Vector2>().x > 0.9f)
			{
				OnClickRightButton();
			}
			else if (_playerInputActions.WasPressedThisFrame() && _playerInputActions.ReadValue<Vector2>().x < -0.9f)
			{
				OnClickLeftButton();
			}
		}
	}
}

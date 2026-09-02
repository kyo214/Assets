using System;
using System.Collections;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using I2.Loc;
using TMPro;
using Toked;
using UnityEngine;

namespace _Modules.UIGlobal;

public class PopupUIManager : GenericSingleton<PopupUIManager>
{
	public enum Type
	{
		YesNo = 0,
		OK = 1
	}

	[SerializeField]
	private UIView _uiView;

	[SerializeField]
	private TMP_Text _popupText;

	[SerializeField]
	private Localize _localizeText;

	[SerializeField]
	private RectTransform _yesNoRectTransform;

	[SerializeField]
	private UIButton _yesButton;

	[SerializeField]
	private UIButton _noButton;

	[SerializeField]
	private RectTransform _okRectTransform;

	[SerializeField]
	private UIButton _okButton;

	private Action OnYesButtonClickedEvent;

	private Action OnNoButtonClickedEvent;

	private Action OnOkButtonClickedEvent;

	private Type _currentType;

	private PlayerInputActions _playerInput;

	private Coroutine _inputCoroutine;

	public bool IsShow()
	{
		return !_uiView.isHidden;
	}

	public void Show(Type type, string localizeKey, Action onYesButtonAction = null, Action onNoButtonAction = null)
	{
		_currentType = type;
		switch (type)
		{
		case Type.YesNo:
			ShowYesNo(localizeKey, onYesButtonAction, onNoButtonAction);
			break;
		case Type.OK:
			ShowOk(localizeKey, onYesButtonAction);
			break;
		}
	}

	private void ShowYesNo(string localizeKey, Action onYesButtonAction = null, Action onNoButtonAction = null)
	{
		ResetUI();
		_yesNoRectTransform.gameObject.SetActive(value: true);
		_localizeText.SetTerm(localizeKey);
		OnYesButtonClickedEvent = onYesButtonAction;
		OnNoButtonClickedEvent = onNoButtonAction;
		_uiView.Show();
		_noButton.Select();
		CheckInput();
	}

	private void ShowOk(string localizeKey, Action onOkButtonAction = null)
	{
		ResetUI();
		_okRectTransform.gameObject.SetActive(value: true);
		_localizeText.SetTerm(localizeKey);
		OnOkButtonClickedEvent = onOkButtonAction;
		_uiView.Show();
		_okButton.Select();
		CheckInput();
	}

	public void Hide()
	{
		_uiView.Hide();
		StopCheckInputCoroutine();
	}

	private void Start()
	{
		_playerInput = InputManager.inputActions;
		_yesButton.onClickEvent.AddListener(OnYesButtonClickedAction);
		_noButton.onClickEvent.AddListener(OnNoButtonClickedAction);
		_okButton.onClickEvent.AddListener(OnOkButtonClickedAction);
	}

	private void OnYesButtonClickedAction()
	{
		Hide();
		OnYesButtonClickedEvent?.Invoke();
		OnYesButtonClickedEvent = null;
	}

	private void OnNoButtonClickedAction()
	{
		Hide();
		OnNoButtonClickedEvent?.Invoke();
		OnNoButtonClickedEvent = null;
	}

	private void OnOkButtonClickedAction()
	{
		Hide();
		OnOkButtonClickedEvent?.Invoke();
		OnOkButtonClickedEvent = null;
	}

	private void ResetUI()
	{
		OnYesButtonClickedEvent = null;
		OnNoButtonClickedEvent = null;
		OnOkButtonClickedEvent = null;
		_yesNoRectTransform.gameObject.SetActive(value: false);
		_okRectTransform.gameObject.SetActive(value: false);
		_popupText.text = "";
	}

	private void CheckInput()
	{
		StopCheckInputCoroutine();
		_inputCoroutine = StartCoroutine(DoCheckInput());
	}

	private void StopCheckInputCoroutine()
	{
		InputManager.ToggleActionMap(InputManager.PlayerInputToggleAction.NONE);
		if (_inputCoroutine != null)
		{
			StopCoroutine(_inputCoroutine);
			_inputCoroutine = null;
		}
	}

	private IEnumerator DoCheckInput()
	{
		InputManager.ToggleActionMap(InputManager.PlayerInputToggleAction.NONE, InputManager.inputActions.UI);
		while (!_playerInput.UI.Cancel.WasPerformedThisFrame())
		{
			yield return null;
		}
		Cancel();
	}

	private void Cancel()
	{
		switch (_currentType)
		{
		case Type.YesNo:
			OnNoButtonClickedAction();
			break;
		case Type.OK:
			OnOkButtonClickedAction();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	public void UI_SelectButton(UIButton button)
	{
		button.Select();
	}
}

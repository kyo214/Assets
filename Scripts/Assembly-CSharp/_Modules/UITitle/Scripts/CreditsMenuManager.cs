using DG.Tweening;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using Toked;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Modules.UITitle.Scripts;

public class CreditsMenuManager : MonoBehaviour
{
	[SerializeField]
	private UIView _creditsUIView;

	[SerializeField]
	private RectTransform _creditPanelRectTransform;

	[SerializeField]
	private UIButton _closeButton;

	[SerializeField]
	private float _creditRollAnimationTime;

	[SerializeField]
	private float _targetCeditPosition = 2954f;

	private bool _running;

	private PlayerInputActions _playerInputActions;

	private void Start()
	{
		_creditsUIView.OnShowCallback.Event.AddListener(OnOpen);
		_creditsUIView.OnHideCallback.Event.AddListener(OnClose);
		_closeButton?.onClickEvent.AddListener(Close);
	}

	private void OnEnable()
	{
		InitInputBinding();
	}

	private void OnDisable()
	{
		RemoveInputBinding();
	}

	public void Open()
	{
		_creditsUIView.Show();
	}

	public void Close()
	{
		_creditsUIView.Hide();
	}

	private void OnOpen()
	{
		ResetCredits();
		_running = true;
		_creditPanelRectTransform.DOAnchorPosY(_targetCeditPosition, _creditRollAnimationTime).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
		InputManager.ToggleActionMap(InputManager.PlayerInputToggleAction.NONE, InputManager.inputActions.UI);
	}

	private void OnClose()
	{
		ResetCredits();
		InputManager.ToggleActionMap(InputManager.PlayerInputToggleAction.NONE);
		UITitleMenuManager.Instance?.BackToTitleMenu();
	}

	private void ResetCredits()
	{
		_running = false;
		_creditPanelRectTransform.DOKill();
		_creditPanelRectTransform.anchoredPosition = Vector2.zero;
	}

	private void InitInputBinding()
	{
		_playerInputActions = InputManager.inputActions;
		_playerInputActions.UI.Cancel.performed += OnCancelInput;
	}

	private void RemoveInputBinding()
	{
		_playerInputActions.UI.Cancel.performed -= OnCancelInput;
	}

	private void OnCancelInput(InputAction.CallbackContext obj)
	{
		if (_running)
		{
			Close();
		}
	}
}

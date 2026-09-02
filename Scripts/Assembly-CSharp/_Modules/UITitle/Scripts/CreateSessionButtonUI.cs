using System;
using Doozy.Runtime.UIManager.Components;
using I2.Loc;
using Toked;
using UnityEngine;
using _Modules.UIGlobal;

namespace _Modules.UITitle.Scripts;

public class CreateSessionButtonUI : MonoBehaviour
{
	[SerializeField]
	[TermsPopup("")]
	protected string _gameOverLocalizeId = "PopupUI/GameOverSaveDataConfirmation";

	[SerializeField]
	[TermsPopup("")]
	protected string _gameClearedLocalizeId = "PopupUI/GameClearedSaveDataConfirmation";

	[SerializeField]
	[TermsPopup("")]
	protected string _canLoadSaveDataLocalizeId = "PopupUI/CantLoadSaveDataConfirmation";

	[SerializeField]
	[TermsPopup("")]
	protected string _canLoadSaveDataVersionLocalizeId = "PopupUI/CantLoadSaveDataVersionConfirmation";

	[SerializeField]
	private UIButton _button;

	protected GameData _gameData;

	protected int _index;

	protected bool _initialized;

	protected Action<CreateSessionButtonUI, bool> OnClickButtonEvent;

	[SerializeField]
	protected StartContinueSpriteSwapper _startContinueSpriteSwapper;

	[SerializeField]
	protected LoadGameDescriptionPanelUI _loadGameDescriptionPanel;

	public void SelectButton()
	{
		if (!_button.isSelected)
		{
			_button.Select();
		}
	}

	private void Start()
	{
		_loadGameDescriptionPanel.InitDeleteButton(OnClickDeleteButton);
		_button.pressedState.stateEvent.Event.AddListener(OnClickButton);
		_button.normalState.stateEvent.Event.AddListener(OnDeselectedButtonAction);
		_button.highlightedState.stateEvent.Event.AddListener(OnSelectedButtonAction);
		_button.onSelectedEvent.AddListener(OnSelectedButtonAction);
		_button.onDeselectedEvent.AddListener(OnDeselectedButtonAction);
	}

	protected virtual void OnClickButton()
	{
		if (!_initialized || _loadGameDescriptionPanel.Hovered)
		{
			return;
		}
		GlobalSaveData.instance.currentSelectedDataIndex = _index;
		bool flag = _gameData != null;
		if (flag)
		{
			if (_gameData.ResetData || _gameData.IsCompleted || !_gameData.CheckVersionCompability())
			{
				ShowConfirmation();
			}
			else
			{
				LoadGame(flag);
			}
		}
	}

	protected void LoadGame(bool isLoadGame)
	{
		UITitleMenuManager.Instance.flowControlGraph.PauseFlow();
		AudioManager.PlaySFX("ui_gamestart");
		OnClickButtonEvent?.Invoke(this, isLoadGame);
		GlobalSaveData.instance.gameData = _gameData ?? new GameData();
		if (isLoadGame)
		{
			GameModes.Instance.SetGameModeSetting(_gameData);
		}
	}

	protected void ResetGameData()
	{
		_gameData = null;
		SetUINewGame();
	}

	protected void ShowConfirmation()
	{
		UITitleMenuManager.Instance.flowControlGraph.PauseFlow();
		AudioManager.PlaySFX("ui_confirm");
		string text = "";
		text = ((!_gameData.CheckVersionCompability()) ? _canLoadSaveDataVersionLocalizeId : (_gameData.IsCompleted ? _gameClearedLocalizeId : ((!_gameData.ResetData) ? _canLoadSaveDataLocalizeId : _gameOverLocalizeId)));
		GenericSingleton<PopupUIManager>.Instance.Show(PopupUIManager.Type.YesNo, text, YesButtonAction, NoButtonAction);
		void NoButtonAction()
		{
			UITitleMenuManager.Instance.flowControlGraph.ResumeFlow();
			SelectButton();
		}
		void YesButtonAction()
		{
			UITitleMenuManager.Instance.flowControlGraph.ResumeFlow();
			AudioManager.PlaySFX("ui_confirm");
			ResetGameData();
		}
	}

	protected virtual void OnClickDeleteButton()
	{
		if (_initialized && _loadGameDescriptionPanel.gameObject.activeSelf)
		{
			_gameData = null;
			SetUINewGame();
		}
	}

	public virtual void Init(int index, Action<CreateSessionButtonUI, bool> onClickButtonAction)
	{
		_startContinueSpriteSwapper?.Init(index, _gameData);
		_loadGameDescriptionPanel.Init(_gameData);
	}

	protected void SetUIContinue(GameData gameData)
	{
		_startContinueSpriteSwapper?.ActiveContinueSpriteImage();
		_loadGameDescriptionPanel.SetActive(active: false);
	}

	protected void SetUINewGame()
	{
		_startContinueSpriteSwapper?.ActiveStartSpriteImage();
		_loadGameDescriptionPanel.SetActive(active: false);
	}

	private void OnSelectedButtonAction()
	{
		if (_gameData != null)
		{
			_loadGameDescriptionPanel.SetActive(active: true);
		}
	}

	private void OnDeselectedButtonAction()
	{
		_loadGameDescriptionPanel.SetActive(active: false);
	}
}

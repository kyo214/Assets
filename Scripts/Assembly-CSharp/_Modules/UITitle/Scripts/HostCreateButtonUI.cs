using System;
using UnityEngine;
using _Modules.UITitle.CreateRoom;

namespace _Modules.UITitle.Scripts;

public class HostCreateButtonUI : CreateSessionButtonUI
{
	[SerializeField]
	private CreateRoomLobbyPanelUI _createRoomPanelLobbyUI;

	public CreateRoomLobbyPanelUI CreateRoomLobbyPanelUI => _createRoomPanelLobbyUI;

	protected override void OnClickDeleteButton()
	{
		base.OnClickDeleteButton();
		GlobalSaveData.DeleteHostSaveData(_index);
	}

	public override void Init(int index, Action<CreateSessionButtonUI, bool> onClickButtonAction)
	{
		OnClickButtonEvent = OnClickButtonAction;
		_gameData = null;
		if (GlobalSaveData.instance.CheckMultiplayerInGameDataExists(index))
		{
			_gameData = GlobalSaveData.instance.LoadMultiplayerGameData(index);
			if (_gameData != null)
			{
				SetUIContinue(_gameData);
			}
			else
			{
				SetUINewGame();
			}
			_createRoomPanelLobbyUI.SetValue(_gameData?.LastRoomSessionType ?? 0);
		}
		else
		{
			SetUINewGame();
		}
		if (!_initialized)
		{
			_index = index;
			_initialized = true;
		}
		base.Init(index, onClickButtonAction);
		void OnClickButtonAction(CreateSessionButtonUI button, bool isLoadGame)
		{
			onClickButtonAction?.Invoke(button, isLoadGame);
			HostCreateButtonUI hostCreateButtonUI = button as HostCreateButtonUI;
			if (isLoadGame)
			{
				hostCreateButtonUI?.CreateRoomLobbyPanelUI.SetDataWhenCreateGame(isLoadGame);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;
using _Modules.UITitle.CreateRoom;

namespace _Modules.UITitle.Scripts;

public class CoopUIController : MonoBehaviour
{
	[SerializeField]
	private List<HostCreateButtonUI> _hostCreateButtonList;

	private bool _isPrivateRoom = true;

	[SerializeField]
	private CreateRoomPanelUI _createRoomPanelUI;

	public void UI_SetPrivateRoom(bool isPrivateRoom)
	{
		_isPrivateRoom = isPrivateRoom;
	}

	public void SetCoopUI()
	{
		Action<CreateSessionButtonUI, bool> onClickButtonAction = OnClickButtonAction;
		for (int i = 0; i < _hostCreateButtonList.Count; i++)
		{
			_hostCreateButtonList[i].Init(i, onClickButtonAction);
		}
		_hostCreateButtonList[0]?.SelectButton();
	}

	private void OnClickButtonAction(CreateSessionButtonUI button, bool isLoadGame)
	{
		_createRoomPanelUI.SetLoad(isLoadGame);
	}
}

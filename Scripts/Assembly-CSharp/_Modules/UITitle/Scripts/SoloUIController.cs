using System;
using System.Collections.Generic;
using UnityEngine;
using _Modules.UITitle.CreateRoom;

namespace _Modules.UITitle.Scripts;

public class SoloUIController : MonoBehaviour
{
	[SerializeField]
	private List<SoloCreateButtonUI> _soloCreateButtonList;

	[SerializeField]
	private CreateSoloPanelUI _createRoomPanelUI;

	public void SetSoloUI()
	{
		Action<CreateSessionButtonUI, bool> onClickButtonAction = OnClickButtonAction;
		for (int i = 0; i < _soloCreateButtonList.Count; i++)
		{
			_soloCreateButtonList[i].Init(i, onClickButtonAction);
		}
		_soloCreateButtonList[0]?.SelectButton();
	}

	private void OnClickButtonAction(CreateSessionButtonUI button, bool isLoadGame)
	{
		_createRoomPanelUI.SetLoad(isLoadGame);
		if (isLoadGame)
		{
			UITitleMenuManager.Instance.ClickSolo(isLoadGame);
		}
	}
}

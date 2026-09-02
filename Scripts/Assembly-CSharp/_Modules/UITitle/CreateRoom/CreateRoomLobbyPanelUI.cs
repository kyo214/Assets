using System;
using System.Linq;

namespace _Modules.UITitle.CreateRoom;

public class CreateRoomLobbyPanelUI : CreateGameSettingPanel<CreateRoomLobbyPanelUI.LobbyType>
{
	public enum LobbyType
	{
		Private = 0,
		Public = 1
	}

	public bool IsPrivateLobby => _index == 0;

	protected override string GetTermValue()
	{
		LobbyType index = (LobbyType)_index;
		return "Menu/" + index;
	}

	protected override void InitDataList()
	{
		_listData = Enum.GetValues(typeof(LobbyType)).Cast<LobbyType>().ToList();
		_index = 0;
	}

	public override void SetDataWhenCreateGame(bool isLoad)
	{
		if (IsPrivateLobby)
		{
			UITitleMenuManager.Instance.ClickCreatePrivateRoom(isLoad);
		}
		else
		{
			UITitleMenuManager.Instance.ClickCreatePublicRoom(isLoad);
		}
	}
}

using System;

namespace _Modules.UITitle.Scripts;

public class SoloCreateButtonUI : CreateSessionButtonUI
{
	protected override void OnClickDeleteButton()
	{
		base.OnClickDeleteButton();
		GlobalSaveData.DeleteSoloSaveData(_index);
	}

	public override void Init(int index, Action<CreateSessionButtonUI, bool> onClickButtonAction)
	{
		OnClickButtonEvent = onClickButtonAction;
		_gameData = null;
		if (GlobalSaveData.instance.CheckInGameDataExists(index))
		{
			_gameData = GlobalSaveData.instance.LoadSoloGameData(index);
			if (_gameData != null)
			{
				SetUIContinue(_gameData);
			}
			else
			{
				SetUINewGame();
			}
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
	}
}

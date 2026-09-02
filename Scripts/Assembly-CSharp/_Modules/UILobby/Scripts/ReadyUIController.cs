using System.Collections.Generic;
using UnityEngine;

namespace _Modules.UILobby.Scripts;

public class ReadyUIController : MonoBehaviour
{
	[SerializeField]
	private List<UITabPlayer> _uiTabPlayer;

	public void Init(PlayerController playerController)
	{
		if (!(playerController == null))
		{
			_uiTabPlayer[playerController.network.GetIDX()]?.SetSkillPerksUI(playerController);
		}
	}

	public void InitPerk(PlayerController playerController)
	{
		if (!(playerController == null))
		{
			_uiTabPlayer[playerController.network.GetIDX()]?.SetPerksUIInfo(playerController);
		}
	}

	public void InitSkill(PlayerController playerController)
	{
		if (!(playerController == null))
		{
			_uiTabPlayer[playerController.network.GetIDX()]?.SetSkillUIInfo(playerController);
		}
	}

	public void Hide(PlayerController playerController)
	{
		if (!(playerController == null))
		{
			_uiTabPlayer[playerController.network.GetIDX()]?.HideUI();
		}
	}

	public UITabPlayer GetUITabPlayer(int index)
	{
		if (index >= 0 && index < _uiTabPlayer.Count)
		{
			return _uiTabPlayer[index];
		}
		return null;
	}
}

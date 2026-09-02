using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Modules.UIInGame.Scripts;

namespace _Modules.UIResult.Scripts;

public class UIPlayerResultPanel : MonoBehaviour
{
	[SerializeField]
	private CharacterAvatarUIController _characterAvatarUIController;

	[SerializeField]
	private TextMeshProUGUI _playerNameTxt;

	[SerializeField]
	private Image _iconDeadImage;

	[SerializeField]
	private Image _frameNormalImage;

	[SerializeField]
	private Image _frameDeadImage;

	[SerializeField]
	private TextMeshProUGUI _playerKillTxt;

	[SerializeField]
	private TextMeshProUGUI _playerDyingTxt;

	[SerializeField]
	private TextMeshProUGUI _playerSkillLearnPointTxt;

	[SerializeField]
	private UIMaterialResultPanel _materialResultPanel;

	public void Init(PlayerController playerController)
	{
		Debug.Log("Init Player Result");
		base.gameObject.SetActive(value: true);
		if (playerController.network.isLocalPlayer)
		{
			_playerNameTxt.color = Color.yellow;
		}
		int iDX = playerController.network.GetIDX();
		bool playerAliveStatus = GetPlayerAliveStatus(iDX);
		_frameNormalImage.gameObject.SetActive(playerAliveStatus);
		_frameDeadImage.gameObject.SetActive(!playerAliveStatus);
		_iconDeadImage.gameObject.SetActive(!playerAliveStatus);
		_playerKillTxt.text = playerController.ScorePlayerNetwork.GetTotalKillPerMission().ToString();
		_playerDyingTxt.text = playerController.ScorePlayerNetwork.ScoreDataPerMission.DeathCount.ToString();
		if (NetworkGameManager.Instance.isServer)
		{
			playerController.ScorePlayerNetwork.SetTotalScoreFromScoreMission();
		}
		_characterAvatarUIController.ChangeHeadAvatarSprite(playerController.data.PlayerSkinData.GetHeadSkinAvatar());
		_characterAvatarUIController.ChangeBodyAvatarSprite(playerController.data.PlayerSkinData.GetBodySkinAvatar());
		_playerNameTxt.text = playerController.network.GetPlayerName();
		_playerSkillLearnPointTxt.text = GetSkillLearnPoint().ToString();
		_materialResultPanel.Set(playerController.data.MaterialInventoryManager.GetInGameMaterialData(), playerAliveStatus);
	}

	private bool GetPlayerAliveStatus(int index)
	{
		return !NetworkGameManager.Instance.GetPlayer(index).network.isDeadResult;
	}

	private int GetSkillLearnPoint()
	{
		if (!UIResultManager.Instance.WinCondition)
		{
			return 0;
		}
		return UIResultManager.Instance._resultMission.SkillPointReward;
	}
}

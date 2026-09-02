using System.Globalization;
using Cysharp.Threading.Tasks;
using I2.Loc;
using TMPro;
using Toked.Skill;
using UnityEngine;
using UnityEngine.UI;
using _Modules.UIInGame.Scripts;

namespace _Modules.UIResult.Scripts;

public class UIPlayerFinalResultPanel : MonoBehaviour
{
	[SerializeField]
	private CharacterAvatarUIController _characterAvatarUIController;

	[SerializeField]
	private TextMeshProUGUI _playerNameTxt;

	[SerializeField]
	private Localize _playerPerkTxt;

	[SerializeField]
	private Image _playerPerkImg;

	[SerializeField]
	private TextMeshProUGUI _playerHPTxt;

	[SerializeField]
	private TextMeshProUGUI _playerStaminaTxt;

	[SerializeField]
	private TextMeshProUGUI _playerKillLabel;

	[SerializeField]
	private TextMeshProUGUI _playerEliteKillLabel;

	[SerializeField]
	private TextMeshProUGUI _playerPuzzleSolvedLabel;

	[SerializeField]
	private TextMeshProUGUI _playerDyingLabel;

	[SerializeField]
	private TextMeshProUGUI _playerKillTxt;

	[SerializeField]
	private TextMeshProUGUI _playerEliteKillTxt;

	[SerializeField]
	private TextMeshProUGUI _playerPuzzleSolvedTxt;

	[SerializeField]
	private TextMeshProUGUI _playerDyingTxt;

	[SerializeField]
	private TextMeshProUGUI _scoreTxt;

	[SerializeField]
	private UISkillStatusController _skillStatus;

	public void Init(PlayerController playerController, int life)
	{
		base.gameObject.SetActive(value: true);
		string perkId = playerController.data.SkillData.PerkId;
		SkillScriptableObject data = DataManager.Instance.Get<PerkLibraryScriptableObject>().GetData(perkId);
		_playerPerkTxt.SetTerm(data.SkillNameLocalizeId);
		_playerPerkImg.sprite = data.SkillSprite;
		int iDX = playerController.network.GetIDX();
		_playerHPTxt.text = playerController.data.GetMaxHealth().ToString(CultureInfo.InvariantCulture);
		_playerStaminaTxt.text = playerController.data.GetMaxStamina().ToString(CultureInfo.InvariantCulture);
		_playerKillLabel.text = string.Format("{0} ({1})", LocalizationManager.GetTranslation("Menu/EnemiesKilled"), playerController.ScorePlayerNetwork.GetTotalKill());
		_playerDyingLabel.text = string.Format("{0} ({1})", LocalizationManager.GetTranslation("Menu/Dying"), playerController.ScorePlayerNetwork.ScoreDataTotal.DeathCount);
		_playerPuzzleSolvedLabel.text = string.Format("{0} ({1})", LocalizationManager.GetTranslation("Menu/PuzzleSolved"), playerController.ScorePlayerNetwork.ScoreDataTotal.PuzzleSolved);
		_playerEliteKillLabel.text = string.Format("{0} ({1})", LocalizationManager.GetTranslation("Menu/EliteEnemiesKilled"), playerController.ScorePlayerNetwork.ScoreDataTotal.KillEliteCount);
		_playerKillTxt.text = (playerController.ScorePlayerNetwork.GetTotalKill() * ScoreManager.Instance.GetScoreConfig.ScorePerKillZombie).ToString();
		_playerDyingTxt.text = (playerController.ScorePlayerNetwork.ScoreDataTotal.DeathCount * ScoreManager.Instance.GetScoreConfig.DeathPenalty).ToString();
		_playerPuzzleSolvedTxt.text = (playerController.ScorePlayerNetwork.ScoreDataTotal.PuzzleSolved * ScoreManager.Instance.GetScoreConfig.ScorePerPuzzle).ToString();
		_playerEliteKillTxt.text = (playerController.ScorePlayerNetwork.ScoreDataTotal.KillEliteCount * ScoreManager.Instance.GetScoreConfig.ScorePerKillElite).ToString();
		_characterAvatarUIController.ChangeHeadAvatarSprite(playerController.data.PlayerSkinData.GetHeadSkinAvatar());
		_characterAvatarUIController.ChangeBodyAvatarSprite(playerController.data.PlayerSkinData.GetBodySkinAvatar());
		_playerNameTxt.text = playerController.network.GetPlayerName();
		_skillStatus.Init(playerController, initEvent: false);
		if (playerController.network.isLocalPlayer)
		{
			_playerNameTxt.color = Color.yellow;
			ScoreManager.Instance.SubmitLeaderboard(iDX, life);
		}
		UniTaskUtil.DelayedCall(this, 1f, () =>
		{
			_scoreTxt.text = playerController.ScorePlayerNetwork.TotalScore.ToString("N0");
		}).Forget();
	}
}

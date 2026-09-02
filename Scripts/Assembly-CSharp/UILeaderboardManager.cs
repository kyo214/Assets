using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using I2.Loc;
using Steamworks;
using Steamworks.Data;
using TMPro;
using Toked;
using Toked.Skill;
using UnityEngine;
using UnityEngine.UI;
using _Modules.GameSystem.BaseScripts.Difficulty;
using _Modules.UITitle.CreateRoom;

public class UILeaderboardManager : MonoBehaviour
{
	[SerializeField]
	private List<PlayerLeaderboard> _listPlayerLeaderboard = new List<PlayerLeaderboard>();

	[SerializeField]
	private Canvas _leaderboardCanvas;

	private Dictionary<ulong, Sprite> _avatarCache = new Dictionary<ulong, Sprite>();

	[SerializeField]
	private PageButtonController _pageButtonGlobalFriend;

	[SerializeField]
	private Localize _localizeGlobalFriend;

	[SerializeField]
	private GameObject _fecthingDataObj;

	[SerializeField]
	private Toggle _toggleUIYourRank;

	[SerializeField]
	private GameObject _detailLeaderboard;

	[SerializeField]
	private TMP_Text _textRank;

	[SerializeField]
	private Localize _textDifficulty;

	[SerializeField]
	private TMP_Text _labelEnemiesKilled;

	[SerializeField]
	private TMP_Text _textEnemiesKilled;

	[SerializeField]
	private TMP_Text _labelEliteEnemiesKilled;

	[SerializeField]
	private TMP_Text _textEliteEnemiesKilled;

	[SerializeField]
	private TMP_Text _labelPuzzleSolved;

	[SerializeField]
	private TMP_Text _textPuzzleSolved;

	[SerializeField]
	private TMP_Text _labelDying;

	[SerializeField]
	private TMP_Text _textDying;

	[SerializeField]
	private TMP_Text _labelRevive;

	[SerializeField]
	private TMP_Text _textRevive;

	[SerializeField]
	private TMP_Text _labelLife;

	[SerializeField]
	private TMP_Text _textLive;

	[SerializeField]
	private TMP_Text _labelPlayTime;

	[SerializeField]
	private TMP_Text _textPlayTime;

	[SerializeField]
	private TMP_Text _textDiffBonus;

	[SerializeField]
	private TMP_Text _textScore;

	[SerializeField]
	private Localize _textFilterTotalPlayer;

	[SerializeField]
	private RectTransform _panel;

	[SerializeField]
	private float _initBottomPanel;

	[SerializeField]
	private GameObject _teammateUIObject;

	[SerializeField]
	private List<UILeaderboardPlayerDetail> _leaderboardDetail = new List<UILeaderboardPlayerDetail>();

	[SerializeField]
	private LeaderboardDetails details;

	[SerializeField]
	public bool FilterIsCoop;

	[SerializeField]
	public bool FilterShowFriendOnly;

	public bool IsShowing => _leaderboardCanvas.enabled;

	private void Start()
	{
		_pageButtonGlobalFriend.Init(2, SetGlobalFriend, GetDisableData(), GetLockData());
		_pageButtonGlobalFriend.SetToggleOn(0);
		_initBottomPanel = _panel.offsetMin.y;
	}

	public void Show()
	{
		string nodeName = UITitleMenuManager.Instance.flowControlGraph.flow.activeNode.nodeName;
		if ((nodeName == "Main Menu" || nodeName == "New Game") && !IsShowing)
		{
			_fecthingDataObj.SetActive(value: true);
			_leaderboardCanvas.enabled = true;
			InitLeaderboard();
		}
	}

	public void ChangeGlobalFriendTab(bool isIncrease)
	{
		int num = _pageButtonGlobalFriend.CurrentActiveButtonIndex;
		if (isIncrease && num < _pageButtonGlobalFriend.TotalPage - 1)
		{
			num++;
			SetGlobalFriend(num);
		}
		else if (!isIncrease && num > 0)
		{
			num--;
			SetGlobalFriend(num);
		}
		_pageButtonGlobalFriend.SetToggleOn(num);
	}

	public void SetGlobalFriend(int index)
	{
		if (index == 0)
		{
			_localizeGlobalFriend.SetTerm("Menu/Global");
			FilterShowFriendOnly = false;
		}
		else
		{
			_localizeGlobalFriend.SetTerm("Menu/Friends");
			FilterShowFriendOnly = true;
		}
		InitLeaderboard(FilterShowFriendOnly, _toggleUIYourRank.isOn);
	}

	public void SetToggleYourRank()
	{
		_toggleUIYourRank.isOn = !_toggleUIYourRank.isOn;
		if (_pageButtonGlobalFriend.CurrentActiveButtonIndex == 0)
		{
			InitLeaderboard(isFriendOnly: false, _toggleUIYourRank.isOn);
		}
		else
		{
			InitLeaderboard(isFriendOnly: true, _toggleUIYourRank.isOn);
		}
	}

	public List<bool> GetDisableData()
	{
		return new List<bool>(2);
	}

	public List<bool> GetLockData()
	{
		return new List<bool>(2);
	}

	public async Task InitLeaderboard(bool isFriendOnly = false, bool isYourRank = false)
	{
		for (int i = 0; i < _listPlayerLeaderboard.Count; i++)
		{
			_listPlayerLeaderboard[i].gameObject.SetActive(value: false);
		}
		if (isFriendOnly)
		{
			if (isYourRank)
			{
				await SteamManager.Instance.SteamLeaderBoard.FilterLeaderboard(null, FilterIsCoop);
				await FetchingData(await SteamManager.Instance.SteamLeaderBoard.GetScoreAroundUserAsync(0, 0), isFriendOnly);
			}
			else
			{
				await SteamManager.Instance.SteamLeaderBoard.FilterLeaderboard(null, FilterIsCoop);
				await FetchingData(await SteamManager.Instance.SteamLeaderBoard.GetScoresFromFriendsAsync(), isFriendOnly);
			}
		}
		else if (isYourRank)
		{
			await SteamManager.Instance.SteamLeaderBoard.FilterLeaderboard(null, FilterIsCoop);
			await FetchingData(await SteamManager.Instance.SteamLeaderBoard.GetScoreAroundUserAsync(0, 0));
		}
		else
		{
			await SteamManager.Instance.SteamLeaderBoard.FilterLeaderboard(null, FilterIsCoop);
			await FetchingData(await SteamManager.Instance.SteamLeaderBoard.GetScoreAsync(30));
		}
	}

	public async Task FetchingData(LeaderboardEntry[] listPlayer, bool isfrendOnly = false)
	{
		int num = 0;
		_fecthingDataObj.SetActive(value: false);
		int ctrIdx = num;
		for (int i = 0; i < _listPlayerLeaderboard.Count; i++)
		{
			if (i < listPlayer.Length)
			{
				_listPlayerLeaderboard[i].gameObject.SetActive(value: true);
				Sprite spriteAvatar = await GetAvatar(listPlayer[ctrIdx].User.Id.Value);
				if (!FilterIsCoop && listPlayer[ctrIdx].User.Id.Value == SteamManager.Instance.SteamLeaderBoard.UserLeaderboard.User.Id.Value)
				{
					SteamManager.Instance.SteamLeaderBoard.UpdateRankUser(listPlayer[ctrIdx].GlobalRank);
				}
				_listPlayerLeaderboard[i].SetPlayerLeaderboard(listPlayer[ctrIdx].User.Id.Value, listPlayer[ctrIdx].GlobalRank, listPlayer[ctrIdx].User.Name, spriteAvatar, listPlayer[ctrIdx].Score, listPlayer[ctrIdx]);
				ctrIdx++;
			}
		}
	}

	public void Hide()
	{
		if (IsShowing)
		{
			ClearAvatarCache();
			_leaderboardCanvas.enabled = false;
		}
	}

	public void HideDetail()
	{
		_detailLeaderboard.SetActive(value: false);
	}

	public async Task<Sprite> GetAvatar(ulong steamId)
	{
		if (_avatarCache.TryGetValue(steamId, out var value))
		{
			return value;
		}
		Steamworks.Data.Image? image = await new Friend(steamId).GetMediumAvatarAsync();
		if (!image.HasValue)
		{
			Debug.Log("No Avatar");
			return null;
		}
		Texture2D texture2D = new Texture2D((int)image.Value.Width, (int)image.Value.Height, TextureFormat.RGBA32, mipChain: false);
		texture2D.LoadRawTextureData(image.Value.Data);
		texture2D.Apply();
		Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
		_avatarCache.Add(steamId, sprite);
		return sprite;
	}

	public void ShowDetail(int idxSlot)
	{
		details = MathFunc.Int32CompressedToObject<LeaderboardDetails>(_listPlayerLeaderboard[idxSlot].LbEntry.Details);
		if (details == null)
		{
			details = MathFunc.Int32ToObject<LeaderboardDetails>(_listPlayerLeaderboard[idxSlot].LbEntry.Details);
		}
		_detailLeaderboard.SetActive(value: true);
		if (details.TotP == 1)
		{
			Vector2 offsetMin = _panel.offsetMin;
			offsetMin.y = -74f;
			_panel.offsetMin = offsetMin;
			_teammateUIObject.SetActive(value: false);
		}
		else
		{
			Vector2 offsetMin2 = _panel.offsetMin;
			offsetMin2.y = _initBottomPanel;
			_panel.offsetMin = offsetMin2;
			_teammateUIObject.SetActive(value: true);
		}
		_textRank.text = LocalizationManager.GetTranslation("Menu/Rank") + " " + _listPlayerLeaderboard[idxSlot].LbEntry.GlobalRank;
		_textDifficulty.SetTerm(DataManager.Instance.Get<DifficultyScriptableObjectLibrary>()?.GetData((DifficultySetting.Difficulty)details.Dif).DifficultyLocalization);
		_labelEnemiesKilled.text = string.Format("{0} ({1})", LocalizationManager.GetTranslation("Menu/EnemiesKilled"), details.K);
		_textEnemiesKilled.text = (details.K * ScoreManager.Instance.GetScoreConfig.ScorePerKillZombie).ToString();
		_labelEliteEnemiesKilled.text = string.Format("{0} ({1})", LocalizationManager.GetTranslation("Menu/EliteEnemiesKilled"), details.KE);
		_textEliteEnemiesKilled.text = (details.KE * ScoreManager.Instance.GetScoreConfig.ScorePerKillElite).ToString();
		_labelPuzzleSolved.text = string.Format("{0} ({1})", LocalizationManager.GetTranslation("Menu/PuzzleSolved"), details.Pzl);
		_textPuzzleSolved.text = (details.Pzl * ScoreManager.Instance.GetScoreConfig.ScorePerPuzzle).ToString();
		_labelDying.text = string.Format("{0} ({1})", LocalizationManager.GetTranslation("Menu/Dying"), details.D);
		_textDying.text = (details.D * ScoreManager.Instance.GetScoreConfig.DeathPenalty).ToString();
		_labelLife.text = string.Format("{0} ({1})", LocalizationManager.GetTranslation("Menu/RemainingLanterns"), details.Life);
		_textLive.text = (details.Life * ScoreManager.Instance.GetScoreConfig.Life).ToString();
		int num = 0;
		if (details.Life >= 1)
		{
			num = Mathf.RoundToInt(Mathf.Clamp(ScoreManager.Instance.GetScoreConfig.MaxTimeAllMapBySecond - (float)details.Time, 0f, ScoreManager.Instance.GetScoreConfig.MaxTimeAllMapBySecond) / ScoreManager.Instance.GetScoreConfig.MaxTimeAllMapBySecond * ScoreManager.Instance.GetScoreConfig.MaxTimeRewardBonus);
		}
		_labelPlayTime.text = LocalizationManager.GetTranslation("Menu/PlayTime") + " (" + MathFunc.GetHourMinuteSecond(details.Time) + ")";
		_textPlayTime.text = num.ToString();
		_textDiffBonus.text = "x" + (1f + (float)(int)details.Dif * 0.1f);
		_textScore.text = LocalizationManager.GetTranslation("Menu/Score") + "   <color=yellow>" + _listPlayerLeaderboard[idxSlot].LbEntry.Score + "</color>";
		for (int i = 0; i < 4; i++)
		{
			_leaderboardDetail[i].gameObject.SetActive(value: false);
		}
		int num2 = 1;
		bool flag = false;
		for (int j = 0; j < 4; j++)
		{
			if (j < details.TotP)
			{
				if (!flag && details.ID[j] == _listPlayerLeaderboard[idxSlot].LbEntry.User.Id.Value)
				{
					_leaderboardDetail[0].gameObject.SetActive(value: true);
					FetchingDetailData(j, 0);
					flag = true;
				}
				else
				{
					_leaderboardDetail[num2].gameObject.SetActive(value: true);
					FetchingDetailData(j, num2);
					num2++;
				}
			}
		}
	}

	public async Task FetchingDetailData(int idxPlayerDetail, int idxSlot)
	{
		ulong num = details.ID[idxPlayerDetail];
		SteamFriends.RequestUserInformation(num);
		Friend friend = new Friend(num);
		PerkLibraryScriptableObject perkLibraryScriptableObject = DataManager.Instance.Get<PerkLibraryScriptableObject>();
		_leaderboardDetail[idxSlot].TextPerksName.text = LocalizationManager.GetTranslation(perkLibraryScriptableObject.GetData(details.Prks[idxPlayerDetail]).SkillNameLocalizeId);
		_leaderboardDetail[idxSlot].ImagePerks.sprite = perkLibraryScriptableObject.GetData(details.Prks[idxPlayerDetail]).SkillSprite;
		_leaderboardDetail[idxSlot].TextScore.text = details.ScrP[idxPlayerDetail].ToString();
		if (details.FP[idxPlayerDetail])
		{
			_leaderboardDetail[idxSlot].FriendPAss.gameObject.SetActive(value: true);
		}
		else
		{
			_leaderboardDetail[idxSlot].FriendPAss.gameObject.SetActive(value: false);
		}
		_leaderboardDetail[idxSlot].TextName.text = "";
		_leaderboardDetail[idxSlot].ImageAvatar.color = UnityEngine.Color.black;
		await UniTask.WaitUntil(() => friend.Name != "[unknown]");
		_leaderboardDetail[idxSlot].TextName.text = friend.Name;
		Sprite sprite = await GetAvatar(details.ID[idxPlayerDetail]);
		_leaderboardDetail[idxSlot].ImageAvatar.color = UnityEngine.Color.white;
		_leaderboardDetail[idxSlot].ImageAvatar.sprite = sprite;
	}

	public void ChangeFilterTotalPlayer(bool isIncrease)
	{
		FilterIsCoop = !FilterIsCoop;
		if (FilterIsCoop)
		{
			_textFilterTotalPlayer.SetTerm("Menu/CoopGame");
		}
		else
		{
			_textFilterTotalPlayer.SetTerm("Menu/SoloGame");
		}
		InitLeaderboard(FilterShowFriendOnly, _toggleUIYourRank.isOn);
	}

	private void OnDestroy()
	{
		foreach (Sprite value in _avatarCache.Values)
		{
			if (value != null)
			{
				Object.Destroy(value.texture);
				Object.Destroy(value);
			}
		}
		_avatarCache.Clear();
	}

	private void ClearAvatarCache()
	{
		foreach (Sprite value in _avatarCache.Values)
		{
			if (!(value == null))
			{
				Object.Destroy(value.texture);
				Object.Destroy(value);
			}
		}
		_avatarCache.Clear();
	}
}

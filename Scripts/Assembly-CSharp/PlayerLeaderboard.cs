using Steamworks.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLeaderboard : MonoBehaviour
{
	[SerializeField]
	public ulong SteamID;

	[SerializeField]
	private TextMeshProUGUI _rankTxt;

	[SerializeField]
	private TextMeshProUGUI _playerNameTxt;

	[SerializeField]
	private UnityEngine.UI.Image _avatarImage;

	[SerializeField]
	private TextMeshProUGUI _scoreTxt;

	[SerializeField]
	public LeaderboardEntry LbEntry;

	public void SetPlayerLeaderboard(ulong uid, int rank, string playerName, Sprite spriteAvatar, int score, LeaderboardEntry paramlbEntry)
	{
		LbEntry = paramlbEntry;
		_playerNameTxt.text = playerName;
		SteamID = uid;
		_rankTxt.text = rank.ToString();
		_avatarImage.sprite = spriteAvatar;
		_avatarImage.color = UnityEngine.Color.white;
		_scoreTxt.text = score.ToString();
	}
}

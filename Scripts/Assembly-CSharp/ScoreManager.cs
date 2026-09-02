using System.Threading.Tasks;
using Toked;
using Toked.Skill;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
	[SerializeField]
	private ScoreConfig _scoreConfig;

	public int time;

	public int wave;

	public int TotalGameScore;

	public bool IsCalculateIndividualScore;

	[SerializeField]
	private LeaderboardDetails details;

	[SerializeField]
	private LeaderboardDetails testConvertDetails;

	public static ScoreManager Instance { get; private set; }

	public ScoreConfig GetScoreConfig => _scoreConfig;

	public LeaderboardDetails GetDetails => details;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	public async Task SubmitLeaderboard(int idxPlayer, int life, int overrideScore = -1)
	{
		details = new LeaderboardDetails();
		for (int i = 0; i < NetworkGameManager.Instance.arrPlayerNetworkController.Count; i++)
		{
			if ((bool)NetworkGameManager.Instance.arrPlayerNetworkController[i])
			{
				details.TotP++;
			}
		}
		int num = life * _scoreConfig.Life;
		details.Life = (byte)life;
		if (life >= 1)
		{
			int num2 = Mathf.RoundToInt(Mathf.Clamp(_scoreConfig.MaxTimeAllMapBySecond - GameManagerPhoton.Instance.TotalMissionTime, 0f, _scoreConfig.MaxTimeAllMapBySecond) / _scoreConfig.MaxTimeAllMapBySecond * _scoreConfig.MaxTimeRewardBonus);
			num += num2;
		}
		details.Time = (short)Mathf.RoundToInt(GameManagerPhoton.Instance.TotalMissionTime);
		int totalScore = 0;
		int num3 = 0;
		int num4 = 0;
		for (int j = 0; j < NetworkGameManager.Instance.arrPlayerNetworkController.Count; j++)
		{
			if (!NetworkGameManager.Instance.arrPlayerNetworkController[j])
			{
				continue;
			}
			PlayerController playerController = NetworkGameManager.Instance.arrPlayerNetworkController[j];
			ScoreResult scoreResult = CalculatePlayerScore(playerController.ScorePlayerNetwork.ScoreDataTotal, _scoreConfig);
			details.ID.Add(playerController.network.playerPhoton.SteamIDUlong);
			if (playerController.network.playerPhoton.IsFriendPass)
			{
				details.FP.Add(item: true);
			}
			else
			{
				details.FP.Add(item: false);
			}
			details.ScrP.Add(scoreResult.TotalScore);
			if (details.ScrP[num4] < 0)
			{
				details.ScrP[num4] = 0;
			}
			details.ScrP[num4] = details.ScrP[num4] + num;
			details.ScrP[num4] = details.ScrP[num4] + Mathf.RoundToInt((float)(details.ScrP[num4] * (int)GameModes.Instance.GetDifficultyData().DifficultySetting) * _scoreConfig.AdditionMultiplyDifficulty);
			if (playerController.network.GetIDX() == idxPlayer)
			{
				num3 = details.ScrP[num4];
			}
			totalScore += details.ScrP[num4];
			playerController.ScorePlayerNetwork.TotalScore = details.ScrP[num4];
			if (IsCalculateIndividualScore)
			{
				if (playerController.network.GetIDX() == idxPlayer)
				{
					details.D = playerController.ScorePlayerNetwork.ScoreDataTotal.DeathCount;
					details.Pzl = playerController.ScorePlayerNetwork.ScoreDataTotal.PuzzleSolved;
					details.K = (short)(playerController.ScorePlayerNetwork.ScoreDataTotal.KillZombieCount + playerController.ScorePlayerNetwork.ScoreDataTotal.KillEliteCount);
					details.KE = playerController.ScorePlayerNetwork.ScoreDataTotal.KillEliteCount;
					details.Rev = playerController.ScorePlayerNetwork.ScoreDataTotal.ReviveOtherPlayer;
				}
			}
			else
			{
				details.D += playerController.ScorePlayerNetwork.ScoreDataTotal.DeathCount;
				details.Pzl += playerController.ScorePlayerNetwork.ScoreDataTotal.PuzzleSolved;
				details.K += (short)(playerController.ScorePlayerNetwork.ScoreDataTotal.KillZombieCount + playerController.ScorePlayerNetwork.ScoreDataTotal.KillEliteCount);
				details.KE += playerController.ScorePlayerNetwork.ScoreDataTotal.KillEliteCount;
				details.Rev += playerController.ScorePlayerNetwork.ScoreDataTotal.ReviveOtherPlayer;
			}
			DataManager.Instance.Get<PerkLibraryScriptableObject>();
			details.Prks.Add(playerController.data.SkillData.PerkId);
			num4++;
		}
		details.Dif = (byte)GameModes.Instance.GetDifficultyData().DifficultySetting;
		if (totalScore < 0)
		{
			totalScore = 0;
		}
		if (num3 < 0)
		{
			num3 = 0;
		}
		TotalGameScore = totalScore;
		if (IsCalculateIndividualScore)
		{
			totalScore = num3;
		}
		int[] detailsArrInt = MathFunc.ObjectToInt32Compressed(details);
		testConvertDetails = MathFunc.Int32CompressedToObject<LeaderboardDetails>(detailsArrInt);
		if (SteamManager.Initialized && !GameModes.Instance.isEvent && !GameModes.Instance.isInitDemo)
		{
			if (overrideScore >= 0)
			{
				totalScore = overrideScore;
			}
			Debug.Log("Submit LeaderBoard " + totalScore);
			if (NetworkGameManager.Instance.mode != NetworkGameManager.MultiplayerMode.Solo)
			{
				await SteamManager.Instance.SteamLeaderBoard.FilterLeaderboard(null, isCoop: true);
			}
			else
			{
				await SteamManager.Instance.SteamLeaderBoard.FilterLeaderboard();
			}
			await SteamManager.Instance.SteamLeaderBoard.SubmitLeaderboard(totalScore, detailsArrInt);
		}
	}

	public static ScoreResult CalculatePlayerScore(ScoreDataNetwork data, ScoreConfig config)
	{
		ScoreResult result = default;
		result.KillEnemiesScore = (data.KillZombieCount + data.KillEliteCount) * config.ScorePerKillZombie;
		result.KillEliteScore = data.KillEliteCount * config.ScorePerKillElite;
		result.PuzzleScore = data.PuzzleSolved * config.ScorePerPuzzle;
		result.DeathPenalty = data.DeathCount * config.DeathPenalty;
		result.ReviveOtherPlayer = data.ReviveOtherPlayer * config.ReviveOtherPlayer;
		result.TotalScore = result.KillEnemiesScore + result.KillEliteScore + result.PuzzleScore + result.DeathPenalty;
		return result;
	}
}

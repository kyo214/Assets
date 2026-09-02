using System;
using System.Threading.Tasks;
using Steamworks.Data;
using UnityEngine;

namespace _Modules.Steam.Scripts;

public class SteamLeaderBoard : MonoBehaviour
{
	public const string LeaderboardName = "Scenario_0_Individu";

	public const string LeaderboardNameCoop = "Scenario_0_Coop";

	private Leaderboard _lbIndividu;

	private Leaderboard _lbSubmit;

	public int UserRank;

	public LeaderboardEntry UserLeaderboard;

	private bool _init;

	public async void Init(Func<Task> onComplete = null)
	{
		try
		{
			await Init("Scenario_0_Individu", onComplete);
		}
		catch (Exception ex)
		{
			Debug.LogError("Error loading leaderboards: " + ex.Message);
			throw;
		}
	}

	public async Task FilterLeaderboard(Func<Task> onComplete = null, bool isCoop = false)
	{
		_init = false;
		try
		{
			if (isCoop)
			{
				await Init("Scenario_0_Coop", onComplete, isForSubmitLeaderboard: true);
			}
			else
			{
				await Init("Scenario_0_Individu", onComplete, isForSubmitLeaderboard: true);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Error loading leaderboards: " + ex.Message);
			throw;
		}
	}

	private async Task Init(string leaderboardName, Func<Task> onComplete = null, bool isForSubmitLeaderboard = false)
	{
		_ = 1;
		try
		{
			Debug.Log("Init Leaderboard " + leaderboardName);
			try
			{
				Leaderboard? leaderboard = await SteamLeaderBoardSystem.GetLeaderBoards(leaderboardName);
				if (leaderboard.HasValue)
				{
					if (isForSubmitLeaderboard)
					{
						_lbSubmit = leaderboard.Value;
					}
					else
					{
						_lbIndividu = leaderboard.Value;
					}
					_init = true;
					if (onComplete != null)
					{
						await onComplete();
					}
				}
				else
				{
					Debug.LogError("Steam leaderboard did not have value");
				}
			}
			catch (Exception ex)
			{
				_init = false;
				Debug.LogError("Error loading leaderboards: " + ex.Message);
			}
		}
		catch (Exception ex2)
		{
			Debug.LogError("Error loading leaderboards: " + ex2.Message);
		}
	}

	public async Task UpdateRankIndividual()
	{
		LeaderboardEntry[] array = await GetScoreAroundUserAsync(0, 0);
		UserRank = array.Rank;
		if ((bool)UITitleMenuManager.Instance)
		{
			UITitleMenuManager.Instance.RankText.text = UserRank.ToString();
		}
		if (array.Length != 0)
		{
			UserLeaderboard = array[0];
		}
	}

	public void UpdateRankUser(int newUserRank)
	{
		UserRank = newUserRank;
		if ((bool)UITitleMenuManager.Instance)
		{
			UITitleMenuManager.Instance.RankText.text = UserRank.ToString();
		}
	}

	public async Task SubmitLeaderboard(int score, params int[] details)
	{
		if (_init)
		{
			LeaderboardEntry[] array = await GetScoreAroundUserAsync(0, 0);
			if (array == null || array.Length == 0)
			{
				await ReplaceScore(score, details);
			}
			else if (score > array[0].Score)
			{
				await ReplaceScore(score, details);
			}
		}
	}

	public async Task ReplaceScore(int score, params int[] details)
	{
		if (_init)
		{
			await SteamLeaderBoardSystem.ReplaceLeaderboard(_lbSubmit, score, details);
		}
	}

	public async Task<LeaderboardEntry[]> GetScoreAroundUserAsync(int start = -10, int end = 10)
	{
		if (!_init)
		{
			return null;
		}
		return await _lbSubmit.GetScoresAroundUserAsync(start, end);
	}

	public async Task<LeaderboardEntry[]> GetScoreIndividu()
	{
		if (!_init)
		{
			return null;
		}
		return await _lbIndividu.GetScoresAroundUserAsync(0, 0);
	}

	public async Task<LeaderboardEntry[]> GetScoreAsync(int count, int offset = 1)
	{
		if (!_init)
		{
			return null;
		}
		return await _lbSubmit.GetScoresAsync(count, offset);
	}

	public async Task<LeaderboardEntry[]> GetScoresFromFriendsAsync()
	{
		if (!_init)
		{
			return null;
		}
		return await _lbSubmit.GetScoresFromFriendsAsync();
	}

	public void ForceInitLeaderboard(string leaderBoardName)
	{
		Init(leaderBoardName);
	}

	public void SubmitLeaderboardIndividualDebug(int score, params int[] details)
	{
		SubmitLeaderboard(score, details);
	}

	public void ReplaceLeaderboardIndividualDebug(int score, params int[] details)
	{
		ReplaceScore(score, details);
	}
}

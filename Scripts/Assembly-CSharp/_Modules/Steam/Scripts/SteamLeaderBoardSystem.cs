using System;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

namespace _Modules.Steam.Scripts;

public static class SteamLeaderBoardSystem
{
	public static async Task SubmitLeaderboard(Leaderboard leaderboard, int value, int[] details = null)
	{
		Debug.Log("Submit score " + value);
		LeaderboardUpdate? leaderboardUpdate = await leaderboard.SubmitScoreAsync(value, details ?? Array.Empty<int>());
		if (!leaderboardUpdate.HasValue)
		{
			Debug.LogError("leaderboardUpdate is null");
		}
		else
		{
			Debug.Log("Rank = " + leaderboardUpdate.Value.NewGlobalRank);
		}
	}

	public static async Task ReplaceLeaderboard(Leaderboard leaderboard, int value, int[] details = null)
	{
		LeaderboardUpdate? leaderboardUpdate = await leaderboard.ReplaceScore(value, details ?? Array.Empty<int>());
		if (!leaderboardUpdate.HasValue)
		{
			Debug.LogError("leaderboardUpdate is null");
		}
		else
		{
			Debug.Log(leaderboardUpdate.Value);
		}
	}

	public static async Task<Leaderboard?> GetLeaderBoards(string lbName)
	{
		try
		{
			return await FindOrCreateLeaderboardAsync(lbName);
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
		return null;
	}

	private static async Task<Leaderboard?> FindOrCreateLeaderboardAsync(string leaderboardName)
	{
		try
		{
			return await SteamUserStats.FindOrCreateLeaderboardAsync(leaderboardName, LeaderboardSort.Ascending, LeaderboardDisplay.Numeric);
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
		return null;
	}
}

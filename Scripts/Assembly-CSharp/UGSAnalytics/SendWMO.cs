using System.Collections.Generic;

namespace UGSAnalytics;

public static class SendWMO
{
	private static Dictionary<string, object> param;

	public static string LobbyID;

	public static void Init()
	{
		param = new Dictionary<string, object>();
	}

	public static void Destroy()
	{
		param.Clear();
		param = null;
	}

	public static void SetLobbyID(string lobbyID)
	{
		LobbyID = lobbyID;
	}

	public static void GameLobby(List<string> playerList)
	{
		string text = "";
		for (int i = 0; i < playerList.Count; i++)
		{
			text += playerList[i];
			if (i != playerList.Count - 1)
			{
				text += ", ";
			}
		}
		param.Clear();
		param.Add("lobbyID", LobbyID);
		param.Add("userList", text);
		DataCollection.SendCustomEvent("gameLobby", param);
	}

	public static void TimeSpentInArea(string areaName, float time)
	{
		param.Clear();
		param.Add("areaName", areaName);
		param.Add("time", time);
		DataCollection.SendCustomEvent("timeSpentInArea", param);
	}

	public static void PuzzleComplete(string puzzleName, float time)
	{
		param.Clear();
		param.Add("puzzleName", puzzleName);
		param.Add("time", time);
		DataCollection.SendCustomEvent("puzzleCompletionTime", param);
	}

	public static void ClueFound(string puzzleName, float time)
	{
		param.Clear();
		param.Add("puzzleName", puzzleName);
		param.Add("time", time);
		DataCollection.SendCustomEvent("clueFoundTime", param);
	}

	public static void ClueInteraction(string puzzleName)
	{
		param.Clear();
		param.Add("puzzleName", puzzleName);
		param.Add("lobbyID", LobbyID);
		DataCollection.SendCustomEvent("clueInteracted", param);
	}

	public static void KeyItemFound(string puzzleName, float time)
	{
		param.Clear();
		param.Add("puzzleName", puzzleName);
		param.Add("time", time);
		DataCollection.SendCustomEvent("keyItemFoundTime", param);
	}

	public static void PuzzleOpen(string puzzleName, float time)
	{
		param.Clear();
		param.Add("puzzleName", puzzleName);
		param.Add("time", time);
		DataCollection.SendCustomEvent("puzzleOpenTime", param);
	}

	public static void WeaponPickup(string weaponName, int frequency)
	{
		param.Clear();
		param.Add("lobbyID", LobbyID);
		param.Add("weaponName", weaponName);
		param.Add("frequency", frequency);
		DataCollection.SendCustomEvent("weaponPickup", param);
	}

	public static void WeaponUsed(string weaponName, int frequency)
	{
		param.Clear();
		param.Add("lobbyID", LobbyID);
		param.Add("weaponName", weaponName);
		param.Add("frequency", frequency);
		DataCollection.SendCustomEvent("weaponDamage", param);
	}

	public static void WeaponKill(string weaponName, int frequency)
	{
		param.Clear();
		param.Add("lobbyID", LobbyID);
		param.Add("weaponName", weaponName);
		param.Add("frequency", frequency);
		DataCollection.SendCustomEvent("weaponKill", param);
	}

	public static void WeaponMultikill(string weaponName, int frequency)
	{
		param.Clear();
		param.Add("lobbyID", LobbyID);
		param.Add("weaponName", weaponName);
		param.Add("frequency", frequency);
		DataCollection.SendCustomEvent("WeaponMultikill", param);
	}
}

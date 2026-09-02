using System;
using System.Collections.Generic;
using Steamworks;

namespace _Modules.Steam.Scripts;

public static class SteamRichPresence
{
	[Serializable]
	public class RichPresenceValueVariable
	{
		public string id;

		public string value;
	}

	public static void SetRichPresenceCustom(string idText)
	{
		if (SteamManager.Initialized)
		{
			SteamFriends.SetRichPresence("steam_display", idText);
		}
	}

	public static void SetRichPresenceVariableValue(List<RichPresenceValueVariable> variables, string idText)
	{
		if (!SteamManager.Initialized)
		{
			return;
		}
		foreach (RichPresenceValueVariable variable in variables)
		{
			SteamFriends.SetRichPresence(variable.id, variable.value);
		}
		SteamFriends.SetRichPresence("steam_display", idText);
	}

	public static void SetRichPresence(string idText, string valueIdText, string valueText)
	{
		if (SteamManager.Initialized)
		{
			SteamFriends.SetRichPresence(valueIdText, valueText);
			SteamFriends.SetRichPresence("steam_display", idText);
		}
	}

	public static void SetRichPresence(string valueIdText)
	{
		SetRichPresence("#StatusFull", "text", valueIdText);
	}

	public static void ClearRichPresence()
	{
		if (SteamManager.Initialized)
		{
			SteamFriends.ClearRichPresence();
		}
	}
}

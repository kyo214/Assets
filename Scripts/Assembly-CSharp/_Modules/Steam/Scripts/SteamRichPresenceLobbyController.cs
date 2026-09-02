using UnityEngine;

namespace _Modules.Steam.Scripts;

[RequireComponent(typeof(SteamRichPresenceComponent))]
public class SteamRichPresenceLobbyController : MonoBehaviour
{
	[SerializeField]
	private SteamRichPresenceComponent _steamRichPresence;

	[SerializeField]
	private string _steamRichPresenceSoloKey = "#Status_SoloLobby";

	[SerializeField]
	private string _steamRichPresenceCoopKey = "#Status_CoopLobby";

	[SerializeField]
	private string _steamRichPresenceVariableKey = "player";

	public void UpdateRichPresence()
	{
		if (NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Solo)
		{
			_steamRichPresence.SetRichPresenceID(_steamRichPresenceSoloKey);
		}
		else
		{
			_steamRichPresence.SetRichPresenceID(_steamRichPresenceCoopKey);
			SteamRichPresenceComponent steamRichPresence = _steamRichPresence;
			string steamRichPresenceVariableKey = _steamRichPresenceVariableKey;
			string text = NetworkGameManager.Instance.arrPlayerController.Count.ToString();
			int mAX_PLAYERS = PhotonMultiplayerManager.MAX_PLAYERS;
			steamRichPresence.SetValueVariable(steamRichPresenceVariableKey, text + "/" + mAX_PLAYERS);
		}
		_steamRichPresence.UpdateRichPresence();
	}
}

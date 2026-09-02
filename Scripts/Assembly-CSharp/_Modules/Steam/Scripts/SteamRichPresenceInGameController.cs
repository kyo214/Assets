using I2.Loc;
using MoreMountains.Tools;
using UnityEngine;

namespace _Modules.Steam.Scripts;

[RequireComponent(typeof(SteamRichPresenceComponent))]
public class SteamRichPresenceInGameController : MonoBehaviour
{
	[SerializeField]
	private SteamRichPresenceComponent _steamRichPresence;

	[SerializeField]
	private string _steamRichPresenceSoloKey = "#Status_Solo_InGame";

	[SerializeField]
	private string _steamRichPresenceCoopKey = "#Status_CoopInGame";

	[SerializeField]
	private string _steamRichPresenceVariableKey = "map";

	public void Start()
	{
		UpdateRichPresence();
	}

	public void UpdateRichPresence()
	{
		if (!NetworkGameManager.Instance)
		{
			SteamRichPresence.ClearRichPresence();
			return;
		}
		if (NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Solo)
		{
			_steamRichPresence.SetRichPresenceID(_steamRichPresenceSoloKey);
		}
		else
		{
			_steamRichPresence.SetRichPresenceID(_steamRichPresenceCoopKey);
		}
		string valueText = LocalizationManager.GetTranslation(GameManagerPhoton.Instance?.CurrentMission?.MapNameLocalization)?.ToTitleCase() ?? "";
		_steamRichPresence.SetValueVariable(_steamRichPresenceVariableKey, valueText);
		_steamRichPresence.UpdateRichPresence();
	}
}

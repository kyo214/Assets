using System;
using Dissonance;
using UnityEngine;

public class VoiceBroadcastController : MonoBehaviour
{
	public VoiceBroadcastTrigger voiceBroadcast;

	private PlayerController _player;

	public static VoiceBroadcastController Instance { get; private set; }

	public int ctrHide { get; private set; }

	private PlayerController Player => _player ?? (_player = NetworkGameManager.Instance.ownPlayer);

	public static event Action<PlayerController> OnPlayerSpeakingEvent;

	public static event Action<PlayerController> OnPlayerEndSpeakEvent;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void FixedUpdate()
	{
		if ((bool)ChatSystem.Instance && (bool)Player)
		{
			ctrHide--;
			if (!VoiceChatGlobalController.Instance.IsMuted() && voiceBroadcast._isVadSpeaking && !ChatSystem.Instance.IconSpeaking[Player.network.GetIDX()].activeSelf)
			{
				ChatSystem.Instance.IconSpeaking[Player.network.GetIDX()].SetActive(value: true);
				ctrHide = 60;
			}
			else if (ChatSystem.Instance.IconSpeaking[Player.network.GetIDX()].activeSelf && ctrHide <= 0)
			{
				ChatSystem.Instance.IconSpeaking[Player.network.GetIDX()].SetActive(value: false);
				OnPlayerEndSpeakEvent?.Invoke(Player);
			}
			if (ChatSystem.Instance.IconSpeaking[Player.network.GetIDX()].activeSelf)
			{
				OnPlayerSpeakingEvent?.Invoke(Player);
			}
		}
	}
}

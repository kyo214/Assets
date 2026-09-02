using System;
using System.Collections.Generic;
using Dissonance;
using Toked;
using UnityEngine;

public class VoiceChatGlobalController : MonoBehaviour
{
	public DissonanceComms VoiceComms;

	private bool _isInitialized;

	private const int ITEM_RADIO = 365;

	[SerializeField]
	private float _rangeProximity;

	[SerializeField]
	private float _maxDistanceAudio = -1f;

	[SerializeField]
	private float _rangeProximityReduction;

	[SerializeField]
	private float _reduction = 0.3f;

	[SerializeField]
	private List<string> ListMicrophone = new List<string>();

	public List<VoiceSoundControl> ListVoiceSound = new List<VoiceSoundControl>();

	private Dictionary<VoicePlayerState, PlayerController> ListPlayerSpeaking = new Dictionary<VoicePlayerState, PlayerController>();

	private float timer;

	public static VoiceChatGlobalController Instance { get; private set; }

	public static event Action<PlayerController, bool> OnPlayerSpeakingEvent;

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
		if (VoiceComms.IsNetworkInitialized && !_isInitialized && (bool)NetworkGameManager.Instance && (bool)NetworkGameManager.Instance.ownPlayer)
		{
			if (NetworkGameManager.Instance.isServer)
			{
				if (VoiceComms.LocalPlayerName != null)
				{
					NetworkGameManager.Instance.ownPlayer.network.playerPhoton.voiceChatName = VoiceComms.LocalPlayerName;
				}
			}
			else if (VoiceComms.LocalPlayerName != null)
			{
				NetworkGameManager.Instance.ownPlayer.network.playerPhoton.RpcSetVoiceChatName(VoiceComms.LocalPlayerName);
			}
			_isInitialized = true;
			SetMuted(!NetworkGameManager.Instance.ownPlayer.IsMicOn);
			UIGameManager.Instance.micOn.SetActive(NetworkGameManager.Instance.ownPlayer.IsMicOn);
			UIGameManager.Instance.micOff.SetActive(!NetworkGameManager.Instance.ownPlayer.IsMicOn);
			VoiceComms.MicrophoneName = GlobalOptionsManager.Instance.microphoneName;
		}
		timer += Time.fixedDeltaTime;
		if (!(timer >= 0.1f))
		{
			return;
		}
		timer = 0f;
		if (ListPlayerSpeaking.Count <= 0)
		{
			return;
		}
		foreach (KeyValuePair<VoicePlayerState, PlayerController> item in ListPlayerSpeaking)
		{
			if (NetworkGameManager.Instance.ownPlayer != item.Value)
			{
				bool bothHaveRadioHT = NetworkGameManager.Instance.ownPlayer.data.FindInventory(365) != null && item.Value.data.FindInventory(365) != null;
				CheckingVoiceChat(item.Key, item.Value, bothHaveRadioHT);
			}
		}
	}

	private void OnEnable()
	{
		VoiceComms.OnPlayerStartedSpeaking += StartSpeaking;
		VoiceComms.OnPlayerStoppedSpeaking += StopSpeaking;
	}

	private void OnDisable()
	{
		VoiceComms.OnPlayerStartedSpeaking -= StartSpeaking;
		VoiceComms.OnPlayerStoppedSpeaking -= StopSpeaking;
	}

	public void SetMuted(bool Value)
	{
		VoiceComms.IsMuted = Value;
	}

	public bool IsMuted()
	{
		return VoiceComms.IsMuted;
	}

	public void StartSpeaking(VoicePlayerState voiceState)
	{
		foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerController)
		{
			if (voiceState.Name.Contains(item.network.playerPhoton.voiceChatName))
			{
				if (NetworkGameManager.Instance.ownPlayer != item)
				{
					bool flag = NetworkGameManager.Instance.ownPlayer.data.FindInventory(365) != null && item.data.FindInventory(365) != null;
					ListPlayerSpeaking.Add(voiceState, item);
					CheckingVoiceChat(voiceState, item, flag);
					OnPlayerSpeakingEvent?.Invoke(item, flag);
				}
				ChatSystem.Instance.IconSpeaking[item.network.GetIDX()].SetActive(value: true);
			}
		}
	}

	private void CheckingVoiceChat(VoicePlayerState voiceState, PlayerController player, bool bothHaveRadioHT)
	{
		if (voiceState?.Playback == null)
		{
			return;
		}
		PlayerController ownPlayer = NetworkGameManager.Instance.ownPlayer;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		float num = MathFunc.DistanceSameYPos(player.transform.position, ownPlayer.transform.position);
		int num2 = Mathf.FloorToInt(_rangeProximity + _rangeProximityReduction);
		if (num < _rangeProximity)
		{
			flag2 = true;
		}
		Vector3 normalized = (player.weaponPos.position - ownPlayer.weaponPos.position).normalized;
		if (player.RoomName == ownPlayer.RoomName)
		{
			flag = true;
		}
		if (num < (float)num2 && !Physics.Raycast(NetworkGameManager.Instance.ownPlayer.weaponPos.position, normalized, num2, GameManager.Instance.wallFloorCollider))
		{
			flag3 = true;
		}
		foreach (VoiceSoundControl item in ListVoiceSound)
		{
			if (!(item != null) || voiceState.Playback == null || !(voiceState.Playback.PlayerName == item.VoicePlayback.PlayerName))
			{
				continue;
			}
			item.IsInitSetParent(player.audioListener.transform, player);
			item.AudioSource.maxDistance = num2;
			item.AudioSource.spatialBlend = 1f;
			if (flag2)
			{
				item.SetSameRoomAudioMixer();
			}
			else if (flag3 && !bothHaveRadioHT)
			{
				item.SetSameRoomAudioMixer();
			}
			else if (!flag)
			{
				if (bothHaveRadioHT)
				{
					item.SetHtAudioMixer();
				}
				else
				{
					item.SetDifferentRoomAudioMixer();
				}
			}
			else if (bothHaveRadioHT)
			{
				item.SetHtAudioMixer();
			}
			else
			{
				item.SetSameRoomAudioMixer();
			}
			break;
		}
	}

	public void StopSpeaking(VoicePlayerState voiceState)
	{
		foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerController)
		{
			if (voiceState.Name.Contains(item.network.playerPhoton.voiceChatName))
			{
				ListPlayerSpeaking.Remove(voiceState);
				ChatSystem.Instance.IconSpeaking[item.network.GetIDX()].SetActive(value: false);
			}
		}
	}

	public VoicePlayerState GetVoiceChat(string id)
	{
		foreach (VoicePlayerState player in VoiceComms.Players)
		{
			if (player.Name.Contains(id))
			{
				return player;
			}
		}
		return null;
	}

	public VoiceSoundControl GetVoiceSoundControl(string id)
	{
		if (string.IsNullOrWhiteSpace(id))
		{
			return null;
		}
		foreach (VoiceSoundControl item in ListVoiceSound)
		{
			if (item.VoicePlayback.PlayerName == id)
			{
				return item;
			}
		}
		return null;
	}
}

using System;
using System.Collections.Generic;
using Toked;
using UnityEngine;

namespace _Modules.Cutscene.Scripts;

public class CutsceneManager : GenericSingleton<CutsceneManager>
{
	private static Dictionary<string, CutsceneTrigger> _cutsceneTriggerDictionary = new Dictionary<string, CutsceneTrigger>();

	[SerializeField]
	private CutsceneNetworkManager _cutsceneNetworkPrefab;

	private CutsceneNetworkManager _cutsceneNetworkManager;

	[SerializeField]
	private CutsceneTimelineManager _cutsceneTimelineManager;

	[SerializeField]
	private CutsceneVideoPlayerManager _cutsceneVideoPlayerManager;

	[SerializeField]
	private CinematicBlackBarController _cinematicBlackBarController;

	private CutsceneScriptableObject _cutsceneScriptableObject;

	public CutsceneNetworkManager CutsceneNetworkManager
	{
		get
		{
			return _cutsceneNetworkManager;
		}
		set
		{
			_cutsceneNetworkManager = value;
		}
	}

	public CinematicBlackBarController CinematicBlackBarController => _cinematicBlackBarController;

	public CutsceneScriptableObject CutsceneSo => _cutsceneScriptableObject;

	public bool AllSkip { get; set; } = true;

	public static void Add(CutsceneTrigger cutsceneTrigger)
	{
		_cutsceneTriggerDictionary.TryAdd(cutsceneTrigger.ID, cutsceneTrigger);
	}

	public static void Remove(CutsceneTrigger cutsceneTrigger)
	{
		if (_cutsceneTriggerDictionary.ContainsKey(cutsceneTrigger.ID))
		{
			_cutsceneTriggerDictionary.Remove(cutsceneTrigger.ID);
		}
	}

	private void InitCutsceneManager(NetworkGameManager.MultiplayerMode mode, PhotonMultiplayerManager photonMultiplayerManager)
	{
		if (mode != NetworkGameManager.MultiplayerMode.Solo && NetworkGameManager.Instance.isServer)
		{
			_cutsceneNetworkManager = NetworkGameManager.Instance.photonNetworking._runner.Spawn(_cutsceneNetworkPrefab, Vector3.zero, Quaternion.identity, NetworkGameManager.Instance.photonNetworking._runner.LocalPlayer);
		}
	}

	private void OnEnable()
	{
		PhotonMultiplayerManager.OnStartServer += InitCutsceneManager;
	}

	private void OnDisable()
	{
		PhotonMultiplayerManager.OnStartServer -= InitCutsceneManager;
	}

	public void PlayCutscene(CutsceneScriptableObject cutsceneScriptableObject)
	{
		if ((bool)cutsceneScriptableObject)
		{
			_cutsceneScriptableObject = cutsceneScriptableObject;
			switch (cutsceneScriptableObject.CutsceneEnumType)
			{
			case CutsceneType.TIMELINE:
				PlayTimelineCutscene(cutsceneScriptableObject);
				break;
			case CutsceneType.VIDEO:
				PlayVideoCutscene(cutsceneScriptableObject);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}

	private void PlayVideoCutscene(CutsceneScriptableObject cutsceneScriptableObject)
	{
		if ((bool)_cutsceneVideoPlayerManager)
		{
			OnBeforeStartCutscene();
			switch (cutsceneScriptableObject.VideoSourceEnumType)
			{
			case CutsceneScriptableObject.VideoSourceType.VIDEOCLIP:
				_cutsceneVideoPlayerManager.Play(cutsceneScriptableObject.VideoClip, cutsceneScriptableObject.Skippable, OnCompleteCutscene);
				break;
			case CutsceneScriptableObject.VideoSourceType.FILEPATH:
				_cutsceneVideoPlayerManager.Play(cutsceneScriptableObject.VideoClipPath, cutsceneScriptableObject.Skippable, OnCompleteCutscene);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		void OnBeforeStartCutscene()
		{
			SetSkipCutsceneNetwork(setActive: false);
			SetShowCutsceneNetwork(setActive: true);
			cutsceneScriptableObject.InvokeOnBeforeStartAction();
		}
		void OnCompleteCutscene()
		{
			SetShowCutsceneNetwork(setActive: false);
			cutsceneScriptableObject.InvokeOnCompletedAction();
		}
	}

	private void PlayTimelineCutscene(CutsceneScriptableObject cutsceneScriptableObject)
	{
		if ((bool)_cutsceneTimelineManager)
		{
			OnBeforeStartCutscene();
			_cutsceneTimelineManager.Play(cutsceneScriptableObject.PlayableDirectorId, cutsceneScriptableObject.Skippable, OnCompleteCutscene);
		}
		void OnBeforeStartCutscene()
		{
			SetSkipCutsceneNetwork(setActive: false);
			SetShowCutsceneNetwork(setActive: true);
		}
		void OnCompleteCutscene()
		{
			SetShowCutsceneNetwork(setActive: false);
		}
	}

	public static CutsceneTrigger GetCutsceneTrigger(string key)
	{
		if (!_cutsceneTriggerDictionary.TryGetValue(key, out var value))
		{
			return null;
		}
		return value;
	}

	public void Play(string key)
	{
		if (_cutsceneTriggerDictionary.ContainsKey(key))
		{
			_cutsceneTriggerDictionary[key].TriggerCutscene();
		}
	}

	public bool GetSkipInput()
	{
		if (!InputManager.inputActions.UI.Menu.IsPressed())
		{
			return InputManager.inputActions.UI.Click.IsPressed();
		}
		return true;
	}

	public void PlayCutsceneNetwork(string cutsceneId)
	{
		if ((bool)_cutsceneNetworkManager)
		{
			_cutsceneNetworkManager.Rpc_PlayCutscene(cutsceneId);
		}
		else
		{
			Play(cutsceneId);
		}
	}

	private void SetShowCutsceneNetwork(bool setActive)
	{
		if (setActive)
		{
			InputManager.ToggleActionMap(InputManager.PlayerInputToggleAction.NONE, InputManager.inputActions.UI);
		}
		else
		{
			InputManager.ToggleActionMap(InputManager.PlayerInputToggleAction.NONE);
		}
		if ((bool)_cutsceneNetworkManager)
		{
			_cutsceneNetworkManager.Rpc_SetShowCutscene(setActive);
		}
	}

	public void SetSkipCutsceneNetwork(bool setActive)
	{
		if (!_cutsceneNetworkManager)
		{
			AllSkip = true;
			return;
		}
		byte iDX = NetworkGameManager.Instance.ownPlayer.network.GetIDX();
		if (NetworkGameManager.Instance.isServer)
		{
			_cutsceneNetworkManager.arrPlayerSkipCutscene.Set(iDX, setActive);
		}
		else
		{
			_cutsceneNetworkManager.Rpc_SetSkipCutscene(iDX, setActive);
		}
	}

	public bool GetOwnStatusSkip()
	{
		if (!_cutsceneNetworkManager)
		{
			return true;
		}
		return _cutsceneNetworkManager.GetOwnSkipStatus();
	}
}

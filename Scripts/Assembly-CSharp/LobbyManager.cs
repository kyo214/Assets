using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Doozy.Runtime.UIManager.Containers;
using Fusion;
using Toked;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using _Modules.Cutscene.Scripts;

public class LobbyManager : MonoBehaviour
{
	public enum LobbyStateEnum
	{
		NPC = 0,
		Map = 1,
		Car = 2
	}

	[Serializable]
	public class ListStateGO
	{
		public List<GameObject> ListGameObject;
	}

	public bool allReady;

	public XTimer timerCountDown;

	public UIView UIHintControl;

	public UIView UIDialogueOnboarding;

	public GameObject playerStats;

	public GameObject readyBoard;

	public GameObject textReady;

	public GameObject textUnready;

	public Slider sliderReady;

	public bool testMode;

	public bool disclaimerShowing;

	public bool changeScene;

	public bool isRetry;

	public bool initClientRetry;

	public bool sceneInitialized;

	public bool _artifactInitialized;

	[SerializeField]
	private List<GameObject> _listArtifacts = new List<GameObject>();

	[SerializeField]
	private CutsceneLobbyController _cutsceneLobbyController;

	public List<ItemPickable> ListItemLobby = new List<ItemPickable>();

	private int _listArtifactCount;

	public Canvas CanvasLobby;

	public GameObject UIResult;

	public LobbyStateEnum LobbyState;

	public List<ListStateGO> ListStateGameobjects = new List<ListStateGO>();

	public List<TriggerEvent> ListAreaTutorial1 = new List<TriggerEvent>();

	public List<TriggerEvent> ListAreaTutorial2 = new List<TriggerEvent>();

	public List<GameObject> ListDeactivateOnPhase2 = new List<GameObject>();

	public static LobbyManager Instance { get; private set; }

	private void Awake()
	{
		timerCountDown = GetComponent<XTimer>();
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
		if ((bool)NetworkGameManager.Instance && (bool)NetworkGameManager.Instance.ownPlayer)
		{
			CameraGame.Instance.mainCam.GetComponent<AudioListener>().enabled = false;
		}
		if (GameManagerPhoton.Instance == null)
		{
			StartLobby();
			SetLobbyState(LobbyStateEnum.Map);
		}
		else
		{
			LobbyState = LobbyStateEnum.NPC;
			AudioManager.ChangeVolumeMaster((float)GlobalSaveData.instance.optionData.volMaster / 100f);
			AudioManager.ChangeVolumeSFX((float)GlobalSaveData.instance.optionData.volSFX / 100f);
			AudioManager.ChangeVolumeBGM((float)GlobalSaveData.instance.optionData.volMusic / 100f);
			AudioManager.SetBGMFixed(value: false);
			AudioManager.PlayBGM("BGM", "StageClear", 1f);
			NetworkGameManager.Instance.ownPlayer.network.charControllerPhoton.gravity = 0f;
			UIResult.SetActive(value: true);
			UIResultManager.Instance.StartCoroutine(UIResultManager.Instance.ShowUIResult());
			SetLobbyState(LobbyStateEnum.Map);
		}
		if (NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Solo)
		{
			RectTransform component = UIGameManager.Instance.fpsObject.GetComponent<RectTransform>();
			component.anchoredPosition = new Vector3(component.anchoredPosition.x, -129f);
		}
		GameModes.Instance.modeGame = "Default";
	}

	public void StartLobby()
	{
		if ((bool)NetworkGameManager.Instance && (bool)NetworkGameManager.Instance.ownPlayer)
		{
			NetworkGameManager.Instance.ownPlayer.network.SetTargetIdxCamTarget(NetworkGameManager.Instance.ownPlayer.network.GetIDX());
			NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: true);
		}
		SetItemLobby();
		AudioManager.ChangeVolumeMaster((float)GlobalSaveData.instance.optionData.volMaster / 100f);
		AudioManager.ChangeVolumeSFX((float)GlobalSaveData.instance.optionData.volSFX / 100f);
		AudioManager.ChangeVolumeBGM((float)GlobalSaveData.instance.optionData.volMusic / 100f);
		AudioManager.PlayBGM("Lobby", "lobby", 1f);
		UIGameManager.Instance.fadeBlack.DOFade(1f, 0f);
		initClientRetry = false;
		GameModes.Instance.isShowingDisclaimer = false;
		if (UIGameManager.Instance.sessionName != null)
		{
			if (NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Solo)
			{
				UIGameManager.Instance.buttonCopyClipboard.SetActive(value: false);
				UIGameManager.Instance.sessionName.text = "";
				UIGameManager.Instance.sessionName.transform.parent.gameObject.SetActive(value: false);
			}
			else
			{
				if (NetworkGameManager.Instance.isReconnecting)
				{
					UIGameManager.Instance.sessionName.transform.parent.gameObject.SetActive(value: true);
				}
				UIGameManager.Instance.buttonCopyClipboard.SetActive(value: false);
				if (NetworkGameManager.Instance.sessionName != "")
				{
					UIGameManager.Instance.sessionName.text = "******";
				}
				else
				{
					UIGameManager.Instance.sessionName.text = "";
				}
			}
			if (NetworkGameManager.Instance.arrPlayerController.Count <= 1 && NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Auto)
			{
				PlayerBoard.Instance.ObjectWaiting.SetActive(value: true);
			}
		}
		if (NetworkGameManager.Instance.arrPlayerController.Count == 0)
		{
			isRetry = false;
			NetworkGameManager.Instance.StartGame(NetworkGameManager.Instance.mode, NetworkGameManager.Instance.sessionName);
			UIGameManager.Instance.loading.discaimer.SetActive(value: false);
		}
		else
		{
			isRetry = true;
			if (NetworkGameManager.Instance.isServer)
			{
				Dictionary<string, SessionProperty> customProperties = new Dictionary<string, SessionProperty> { ["status"] = "Open" };
				PhotonMultiplayerManager.Instance._runner.SessionInfo.UpdateCustomProperties(customProperties);
			}
			NetworkGameManager.Instance.ownPlayer.network.SetHealth(NetworkGameManager.Instance.ownPlayer.data.GetMaxHealth());
			foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerController)
			{
				item.network.charControllerPhoton.enabled = true;
				item.roomColliders.Clear();
				item.network.SetInGame(value: false);
			}
			foreach (PlayerController item2 in NetworkGameManager.Instance.arrPlayerController)
			{
				if (item2.network.isLocalPlayer)
				{
					NetworkGameManager.Instance.ownPlayer.InitPlayerInGame();
				}
				else
				{
					item2.InitOtherPlayerInGame();
				}
			}
			UniTaskUtil.DelayedCall(this, 1f, InputManager.EnableInput).Forget();
			PlayerController ownPlayer = NetworkGameManager.Instance.ownPlayer;
			ownPlayer.audioListener.enabled = true;
			ownPlayer.audioListener.transform.localPosition = new Vector3(ownPlayer.audioListener.transform.localPosition.x, 0.325f, ownPlayer.audioListener.transform.localPosition.z);
			GameManager.Instance.SpawnPhotonGameManager();
			if (!NetworkGameManager.Instance.isServer && (bool)NetworkGameManager.Instance.arrPlayerController[0].network.playerPhoton.inGame)
			{
				initClientRetry = true;
				UIGameManager.Instance.loading.loadingUI.SetActive(value: true);
			}
			UIGameManager.Instance.RefreshPlayerCountText();
		}
		_artifactInitialized = false;
		_listArtifactCount = _listArtifacts.Count;
		Invoke("EnableLobbyGodMode", 1f);
		if ((bool)GameManagerPhoton.Instance)
		{
			GameManagerPhoton.Instance.RandomizeSeedPuzzle();
		}
		foreach (PlayerController item3 in NetworkGameManager.Instance.arrPlayerNetworkController)
		{
			if (item3 != null && !item3.network.isLocalPlayer && (bool)item3.network.playerPhoton.disconnected)
			{
				PlayerBoard.Instance.ObjectWaiting.SetActive(value: false);
				item3.DisconnectedTimer.StartDuration(91f);
				item3.DespawnTimer.StartDuration(90f);
			}
		}
	}

	public void SceneInit()
	{
		if ((bool)GameManagerPhoton.Instance && GameManagerPhoton.Instance.Phase >= 1)
		{
			foreach (GameObject item in ListDeactivateOnPhase2)
			{
				item.SetActive(value: false);
			}
		}
		if (PhotonMultiplayerManager.Instance.activeIngameScene != "Lobby")
		{
			PhotonMultiplayerManager.Instance._runner.SetActiveScene(SceneManager.GetActiveScene().name);
			PhotonMultiplayerManager.Instance.activeIngameScene = SceneManager.GetActiveScene().name;
			PhotonMultiplayerManager.Instance.sceneLoaded = false;
		}
	}

	public void Update()
	{
		if (!_artifactInitialized && (bool)GameManager.Instance.gameManagerPhoton)
		{
			for (int i = 0; i < _listArtifactCount; i++)
			{
				if (GameManagerPhoton.Instance.ArrMissionCleared.Get(i))
				{
					_listArtifacts[i].SetActive(value: true);
				}
			}
			_artifactInitialized = true;
		}
		if (!sceneInitialized && NetworkGameManager.Instance.photonNetworking._runner.IsRunning)
		{
			SceneInit();
			sceneInitialized = true;
		}
		if (NetworkGameManager.Instance.arrPlayerController.Count > 0 && initClientRetry && !NetworkGameManager.Instance.isServer && UIGameManager.Instance.loading.loadingUI.activeSelf)
		{
			initClientRetry = false;
			UniTaskUtil.DelayedCall(this, 1f, () =>
			{
				UIGameManager.Instance.loading.loadingUI.SetActive(value: false);
			}).Forget();
		}
		if (UIGameManager.Instance.loading.pressAnyKey.activeSelf && Input.anyKey && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
		{
			UIGameManager.Instance.PressAnyKeyToLobby();
		}
		if (timerCountDown.isCompleted() && !changeScene)
		{
			if (NetworkGameManager.Instance.isServer)
			{
				bool flag = false;
				for (int num = NetworkGameManager.Instance.arrPlayerNetworkController.Count - 1; num >= 0; num--)
				{
					PlayerController playerController = NetworkGameManager.Instance.arrPlayerNetworkController[num];
					if (playerController != null && (bool)playerController.network.playerPhoton.IsDisconnected && string.IsNullOrWhiteSpace(playerController.data.SkillData.PerkId))
					{
						flag = true;
						KickPlayerDisconnected(playerController, isUpdateCustomProps: false);
					}
				}
				Dictionary<string, SessionProperty> customProperties = new Dictionary<string, SessionProperty> { ["status"] = "Close" };
				if (flag)
				{
					string text = "";
					foreach (string item in NetworkGameManager.Instance.arrPlayerIDDisconnected)
					{
						text = text + item + "|";
					}
					customProperties = new Dictionary<string, SessionProperty>
					{
						["PlayersDisconnect"] = text,
						["status"] = "Close"
					};
				}
				PhotonMultiplayerManager.Instance._runner.SessionInfo.UpdateCustomProperties(customProperties);
				NetworkGameManager.Instance.ownPlayer.network.ExecSyncModifier();
			}
			if (UIGameManager.Instance.uiObjective != null)
			{
				UIGameManager.Instance.uiObjective.SetActive(value: false);
			}
			if (!UIGameManager.Instance.UIMenuNote.isHidden && UIGameManager.Instance.UIMenuNote.gameObject.activeSelf)
			{
				NetworkGameManager.Instance.ownPlayer.CloseNote();
			}
			else if (!UIGameManager.Instance.UIMenuPuzzle.isHidden)
			{
				NetworkGameManager.Instance.ownPlayer.ClosePuzzle();
			}
			UIGameManager.Instance.buttonCopyClipboard.SetActive(value: false);
			UIGameManager.Instance.sessionName.text = "";
			UIGameManager.Instance.sessionName.transform.parent.gameObject.SetActive(value: false);
			for (InventoryObject inventoryObject = NetworkGameManager.Instance.ownPlayer.data.FindInventory(55); inventoryObject != null; inventoryObject = NetworkGameManager.Instance.ownPlayer.data.FindInventory(55))
			{
				NetworkGameManager.Instance.ownPlayer.inventoryManager.FunctionItemDrop(inventoryObject.IdxInventory, isSwapWeapon: false);
			}
			GlobalSaveData.instance.optionData.IsTutorialMoveCleared = true;
			GlobalSaveData.instance.optionData.IsTutorialSprintCleared = true;
			GlobalSaveData.instance.optionData.IsTutorialDashCleared = true;
			GlobalSaveData.instance.optionData.IsTutorialMeleeCleared = true;
			GlobalSaveData.instance.optionData.IsTutorialShootCleared = true;
			_cutsceneLobbyController.PlayCutscene();
		}
		if (timerCountDown.isRunning && UIGameManager.Instance.txtCountDown.text != Mathf.FloorToInt(timerCountDown.interval).ToString())
		{
			AudioManager.PlaySFX("ui_countdown");
			UIGameManager.Instance.txtCountDown.text = Mathf.FloorToInt(timerCountDown.interval).ToString();
		}
	}

	public void KickPlayerDisconnected(PlayerController player, bool isUpdateCustomProps = true)
	{
		for (int i = 0; i < NetworkGameManager.Instance.arrPlayerNetworkController.Count; i++)
		{
			if (NetworkGameManager.Instance.arrPlayerNetworkController[i] == player)
			{
				NetworkGameManager.Instance.arrPlayerNetworkController[i] = null;
			}
		}
		UIGameManager.Instance.RefreshPlayerCountText();
		NetworkGameManager.Instance.arrPlayerIDDisconnected.Remove(player.network.playerPhoton.PlayerDeviceID);
		NetworkGameManager.Instance.arrPlayerDisconnected.Remove(player);
		string text = "";
		foreach (PlayerRef spawnedCharacter in NetworkGameManager.Instance.SpawnedCharacters)
		{
			if (PhotonMultiplayerManager.Instance._runner.GetPlayerObject(spawnedCharacter) == player.network.networkObj)
			{
				NetworkGameManager.Instance.SpawnedCharacters.Remove(spawnedCharacter);
				break;
			}
		}
		if (!isUpdateCustomProps)
		{
			return;
		}
		foreach (string item in NetworkGameManager.Instance.arrPlayerIDDisconnected)
		{
			text = text + item + "|";
		}
		Dictionary<string, SessionProperty> customProperties = new Dictionary<string, SessionProperty> { ["PlayersDisconnect"] = text };
		PhotonMultiplayerManager.Instance._runner.SessionInfo.UpdateCustomProperties(customProperties);
		PhotonMultiplayerManager.Instance._runner.Despawn(player.network.networkObj, allowPredicted: true);
	}

	public void LoadInGameScene()
	{
		SaveItemLobby();
		if (NetworkGameManager.Instance.isServer && (bool)GameManagerPhoton.Instance)
		{
			GameManagerPhoton.Instance.ResetLobbyVariables();
			GameManagerPhoton.Instance.IsWin = false;
			GameManagerPhoton.Instance.showResult = false;
		}
		SO_MissionMap currentMission = GameManagerPhoton.Instance.CurrentMission;
		InitMaterialInventory(currentMission);
		DisableLobbyGodMode();
		UIGameManager.Instance.loading.loadingUI.SetActive(value: true);
		UnityEngine.Random.Range(0, 6);
		InputManager.EnableInput();
		if (NetworkGameManager.Instance.isServer)
		{
			if (PhotonMultiplayerManager.Instance != null && PhotonMultiplayerManager.Instance._runner != null)
			{
				if (NetworkGameManager.Instance.Mission == 0)
				{
					NetworkGameManager.Instance.Mission = 1;
				}
				if (currentMission.SceneName.IndexOf("SocialMedia", StringComparison.Ordinal) >= 0)
				{
					NetworkGameManager.Instance.ownPlayer.audioListener.enabled = false;
					GlobalUIManager.Instance.ClickGoToScene(currentMission.SceneName);
				}
				else if (GameModes.Instance.modeGame == "PVP")
				{
					PhotonMultiplayerManager.Instance._runner.SetActiveScene(currentMission.SceneName + "-PVP");
					PhotonMultiplayerManager.Instance.activeIngameScene = currentMission.SceneName + "-PVP";
				}
				else
				{
					PhotonMultiplayerManager.Instance._runner.SetActiveScene(currentMission.SceneName);
					PhotonMultiplayerManager.Instance.activeIngameScene = currentMission.SceneName;
				}
			}
			changeScene = true;
		}
		else if (currentMission.SceneName.IndexOf("SocialMedia", StringComparison.Ordinal) >= 0)
		{
			NetworkGameManager.Instance.ownPlayer.audioListener.enabled = false;
			GlobalUIManager.Instance.ClickGoToScene(currentMission.SceneName);
		}
		PhotonMultiplayerManager.Instance.sceneLoaded = false;
	}

	private void InitMaterialInventory(SO_MissionMap currentMission)
	{
		foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerController)
		{
			item.data.MaterialInventoryManager.SyncMainMaterialInventory();
			if (item.network.isLocalPlayer)
			{
				item.data.MaterialInventoryManager.ResetInGameMaterialInventory();
				if (currentMission.transferMainMaterialInventoryToInGame)
				{
					item.data.MaterialInventoryManager.TransferMaterialToInGameInventory();
				}
			}
			else
			{
				item.data.MaterialInventoryManager.SyncInGameMaterialInventory();
			}
		}
	}

	public void EnableLobbyGodMode()
	{
		foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerNetworkController)
		{
			item?.network.SetGodMode(isGodMode: true);
		}
	}

	public void DisableLobbyGodMode()
	{
		foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerNetworkController)
		{
			if (item != null)
			{
				item.network.SetGodMode(isGodMode: false);
			}
		}
	}

	public void TestShowResultWin()
	{
		GameManagerPhoton.Instance.IsWin = true;
		ScoreManager.Instance.time = 356;
		AudioManager.StopBGM();
		AudioManager.ChangeVolumeMaster((float)GlobalSaveData.instance.optionData.volMaster / 100f);
		AudioManager.ChangeVolumeSFX((float)GlobalSaveData.instance.optionData.volSFX / 100f);
		AudioManager.ChangeVolumeBGM((float)GlobalSaveData.instance.optionData.volMusic / 100f);
		AudioManager.PlayBGM("BGM", "StageClear", 1f);
		NetworkGameManager.Instance.ownPlayer.network.charControllerPhoton.gravity = 0f;
		UIResult.SetActive(value: true);
		UIResultManager.Instance.StartCoroutine(UIResultManager.Instance.ShowUIResult());
	}

	public void CheckLobbyState()
	{
		switch (LobbyState)
		{
		case LobbyStateEnum.NPC:
			UIGameManager.Instance.missionObjectiveText.SetTerm("Goal/ReachMap");
			break;
		case LobbyStateEnum.Map:
			UIGameManager.Instance.missionObjectiveText.SetTerm("Goal/ReachMap");
			break;
		case LobbyStateEnum.Car:
		{
			MissionSelection missionSelection = MissionLobbyManager.Instance.MissionBoard.GetMissionSelection(NetworkGameManager.Instance.Mission);
			if ((bool)missionSelection && !missionSelection.Icon.activeSelf)
			{
				missionSelection.Icon.SetActive(value: true);
			}
			UIGameManager.Instance.missionObjectiveText.SetTerm("Goal/ReachAmbulance");
			break;
		}
		}
		for (int i = 0; i < ListStateGameobjects.Count; i++)
		{
			if (LobbyState == (LobbyStateEnum)i)
			{
				continue;
			}
			foreach (GameObject item in ListStateGameobjects[i].ListGameObject)
			{
				item.SetActive(value: false);
			}
		}
		if (ListStateGameobjects.Count > 0)
		{
			foreach (GameObject item2 in ListStateGameobjects[(int)LobbyState].ListGameObject)
			{
				item2.SetActive(value: true);
			}
		}
		UIGameManager.Instance.SetMissionLocation(UIGameManager.Instance.missionLocationText, null, UIGameManager.Instance.missionLocationTextField);
	}

	public void SetLobbyState(LobbyStateEnum state, bool allClient = false)
	{
		if (LobbyState != state)
		{
			if (allClient)
			{
				LobbyState = state;
			}
			if (NetworkGameManager.Instance.isServer)
			{
				LobbyState = state;
				GameManagerPhoton.Instance.StateLobby = (byte)LobbyState;
			}
			CheckLobbyState();
		}
	}

	public void SetItemLobby()
	{
		if (GameModes.Instance.isEvent)
		{
			foreach (ItemPickable item in ListItemLobby)
			{
				item.itemCollider.enabled = false;
				item.SetSpriteEnable(value: false);
			}
		}
		if (!GameManagerPhoton.Instance)
		{
			return;
		}
		foreach (int item2 in GameManagerPhoton.Instance.ListItemUIDLobbyPickedUp)
		{
			foreach (ItemPickable item3 in ListItemLobby)
			{
				if (item3.uniqueID == item2)
				{
					item3.itemCollider.enabled = false;
					item3.SetSpriteEnable(value: false);
					break;
				}
			}
		}
		if (!NetworkGameManager.Instance.isServer)
		{
			return;
		}
		foreach (ItemSpawn item4 in GameManagerPhoton.Instance.ListItemSpawnToLobby)
		{
			if (item4.Durability > 0)
			{
				NetworkGameManager.Instance.ownPlayer.network.SetSpawnItem(item4.IDItem, item4.Pos, (byte)item4.Durability, (byte)item4.Ammo);
			}
			else
			{
				NetworkGameManager.Instance.ownPlayer.network.SetSpawnItem(item4.IDItem, item4.Pos, (byte)item4.Amount, (byte)item4.Ammo);
			}
		}
		GameManagerPhoton.Instance.ListItemSpawnToLobby.Clear();
	}

	public void SaveItemLobby()
	{
		if (!(GameManagerPhoton.Instance != null))
		{
			return;
		}
		GameManagerPhoton.Instance.ListItemUIDLobbyPickedUp.Clear();
		for (int i = 0; i < ListItemLobby.Count; i++)
		{
			if (!ListItemLobby[i].itemCollider.enabled)
			{
				GameManagerPhoton.Instance.ListItemUIDLobbyPickedUp.Add(ListItemLobby[i].uniqueID);
			}
		}
	}

	public List<int> GetLobbyPickUp()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < ListItemLobby.Count; i++)
		{
			if (!ListItemLobby[i].itemCollider.enabled)
			{
				list.Add(ListItemLobby[i].uniqueID);
			}
		}
		return list;
	}
}

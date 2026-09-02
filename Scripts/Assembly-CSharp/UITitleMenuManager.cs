using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Doozy.Runtime.Nody;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using Fusion;
using Fusion.Photon.Realtime;
using I2.Loc;
using Steamworks.Data;
using TMPro;
using Toked;
using UGSAnalytics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using _Modules.Steam.Scripts;

public class UITitleMenuManager : MonoBehaviour
{
	public FlowController flowControlGraph;

	public UIButton btnSolo;

	public UIButton btnCoop;

	public UIButton btnOptions;

	public UIButton btnReconnect;

	public UIButton btnPatchNote;

	public Localize TextBtnSolo;

	public GameObject titleObject;

	public GameObject objectBot;

	[SerializeField]
	public GameObject _objectPatchNote;

	public UIView UIMainMenu;

	public UIView UIUsername;

	public UIView UIPatchNote;

	public UIView UIRegion;

	public TMP_InputField inputFieldRoomJoin;

	public TMP_InputField inputFieldUserName;

	public bool goToLobby;

	public string nextScene;

	[SerializeField]
	private UnityEngine.UI.Image blackCrossfadeImage;

	[SerializeField]
	private UnityEngine.UI.Image flash;

	[SerializeField]
	private UIView uiStart;

	[SerializeField]
	private UIView uiHost;

	[SerializeField]
	private UIView uiJoin;

	[SerializeField]
	private UIView uiSolo;

	[SerializeField]
	private UIView uiSelectHostData;

	public bool isPopup;

	private bool init;

	private bool showMainMenu;

	public TextMeshProUGUI textUsername;

	public TextMeshProUGUI textRegion;

	public TextMeshProUGUI textBuildVersion;

	public GameObject _loadingObject;

	public TextMeshProUGUI LoadingText;

	public TextMeshProUGUI RegionText;

	public TextMeshProUGUI FindingRoomText;

	public PlayerInput playerInput;

	private InputAction scrollPatchNoteAction;

	private bool changeSceneToLobby;

	[SerializeField]
	private InternetChecker checker;

	public ScrollRect patchNoteScrollRect;

	public float scrollSpeed = 2f;

	[SerializeField]
	private GameObject _leaderboardObject;

	public TextMeshProUGUI RankText;

	[SerializeField]
	private UILeaderboardManager _uiLeaderboardManager;

	public static UITitleMenuManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
		flowControlGraph.onStart.AddListener(InitTitleMenu);
	}

	private void Start()
	{
		if (SteamManager.Initialized)
		{
			InitLeaderBoard();
		}
		else
		{
			_leaderboardObject.SetActive(value: false);
		}
		blackCrossfadeImage.enabled = true;
		blackCrossfadeImage.DOFade(0f, 0.75f).SetDelay(1f);
		SendWMO.Init();
		nextScene = "Lobby";
		AudioManager.ChangeLowPass(22000f);
		textBuildVersion.text = "V. " + GlobalSaveData.instance.buildVer;
		GlobalOptionsManager.Instance.usingGamepad = GetComponent<PlayerInput>().currentControlScheme.Equals("Gamepad");
		PhotonAppSettings.Instance.AppSettings.Server = "";
		CheckLobby();
		if (GameModes.Instance.isEvent)
		{
			GetComponent<PlayerInput>().actions.RemoveAllBindingOverrides();
		}
		if ((object)playerInput == null)
		{
			playerInput = GetComponent<PlayerInput>();
		}
		if ((bool)VoiceChatGlobalController.Instance)
		{
			UnityEngine.Object.Destroy(VoiceChatGlobalController.Instance.gameObject);
		}
		if ((bool)VoiceBroadcastController.Instance)
		{
			UnityEngine.Object.Destroy(VoiceBroadcastController.Instance.gameObject);
		}
		GlobalSaveData.instance.ResetGameData();
		if (GameModes.Instance.isEvent)
		{
			btnCoop.gameObject.SetActive(value: false);
			Navigation navigation = btnSolo.navigation;
			navigation.selectOnDown = btnOptions;
			btnSolo.navigation = navigation;
			Navigation navigation2 = btnOptions.navigation;
			navigation2.selectOnUp = btnSolo;
			btnOptions.navigation = navigation2;
			TextBtnSolo.SetTerm("Menu/StartGame");
			GlobalSaveData.instance.optionData.IsFirstTimeControlShowed = false;
			GlobalSaveData.instance.optionData.SkipIntroControl = false;
			GlobalSaveData.instance.optionData.SkipIntroDialogue = false;
			GlobalSaveData.instance.optionData.EnableTutorial = true;
			GlobalSaveData.instance.optionData.IsTutorialMoveCleared = false;
			GlobalSaveData.instance.optionData.IsTutorialSprintCleared = false;
			GlobalSaveData.instance.optionData.IsTutorialDashCleared = false;
			GlobalSaveData.instance.optionData.IsTutorialMeleeCleared = false;
			GlobalSaveData.instance.optionData.IsTutorialShootCleared = false;
		}
		GlobalOptionsManager.Instance.seed = 0;
		if ((bool)checker)
		{
			checker.OnInternetFirstConnected += OnInternetConnected;
			checker.OnInternetFirstLostConnected += OnInternetLostConnection;
		}
		if ((bool)NetworkGameManager.Instance)
		{
			NetworkGameManager.Instance.isSyncingMissionMap = false;
			NetworkGameManager.Instance.isReconnecting = false;
		}
		scrollPatchNoteAction = playerInput.actions["Look"];
	}

	public void OnDestroy()
	{
		if ((bool)checker)
		{
			checker.OnInternetFirstConnected -= OnInternetConnected;
			checker.OnInternetFirstLostConnected -= OnInternetLostConnection;
		}
	}

	public void OnInternetLostConnection()
	{
		if (!NetworkGameManager.Instance.ShowButtonReconnect)
		{
			return;
		}
		foreach (SessionInfo session in NetworkGameManager.Instance.sessionList)
		{
			if (GlobalSaveData.instance.optionData.lastRoomCode == session.Name)
			{
				btnReconnect.gameObject.SetActive(value: true);
				Navigation navigation = btnSolo.navigation;
				navigation.selectOnUp = btnReconnect;
				btnSolo.navigation = navigation;
				Navigation navigation2 = btnPatchNote.navigation;
				navigation2.selectOnDown = btnReconnect;
				btnPatchNote.navigation = navigation2;
			}
		}
		NetworkGameManager.Instance.ShowButtonReconnect = false;
	}

	public void OnInternetConnected()
	{
		CheckLobby();
	}

	public async void CheckLobby()
	{
		await PhotonMultiplayerManager.Instance._runner.JoinSessionLobby(SessionLobby.ClientServer, "WMO", null, null, false);
	}

	public void InitTitleMenu()
	{
		if (!flowControlGraph.initialized)
		{
			return;
		}
		if (GlobalSaveData.instance.optionData.region == "")
		{
			UIRegion.Show();
			return;
		}
		if (GlobalSaveData.instance.UserSaveData.UserName == "")
		{
			UIUsername.Show();
			inputFieldUserName.Select();
			SteamApi.OpenFloatingKeyboard(inputFieldUserName, 12);
			_objectPatchNote.SetActive(value: false);
			return;
		}
		ShowTitle();
		if (UIPatchNote.isHidden)
		{
			ShowInfoBot();
		}
		flowControlGraph.SetActiveNodeByName("Main Menu");
		textUsername.text = GlobalSaveData.instance.UserSaveData.UserName;
		textRegion.text = GetRegionName(GlobalSaveData.instance.optionData.region);
	}

	private void FixedUpdate()
	{
		if (goToLobby && NetworkGameManager.Instance.networkInitialized)
		{
			if (!PhotonMultiplayerManager.Instance)
			{
				UnityEngine.Object.Instantiate(NetworkGameManager.Instance.PhotonNetworkingPrefab);
			}
			playerInput.DeactivateInput();
			NetworkGameManager.Instance.arrPlayerDisconnected.Clear();
			NetworkGameManager.Instance.arrPlayerIDDisconnected.Clear();
			NetworkGameManager.Instance.ListPlayerTempInventory.Clear();
			if ((bool)PhotonMultiplayerManager.Instance)
			{
				UnityEngine.Object.Destroy(PhotonMultiplayerManager.Instance._runner);
			}
			if (NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Solo)
			{
				PhotonAppSettings.Instance.AppSettings.Server = "Localhost";
			}
			else
			{
				PhotonAppSettings.Instance.AppSettings.Server = "";
			}
			AudioManager.StopBGM(3.5f);
			uiStart.Hide();
			if ((bool)uiHost)
			{
				uiHost.Hide();
			}
			uiJoin?.Hide();
			if ((bool)uiSolo)
			{
				uiSolo.Hide();
			}
			if ((bool)uiSelectHostData)
			{
				uiSelectHostData.Hide();
			}
			goToLobby = false;
			flash.enabled = true;
			flash.DOFade(0.8f, 0.15f);
			flash.DOFade(0f, 1f).SetDelay(0.15f);
			blackCrossfadeImage.enabled = true;
			UniTaskUtil.DelayedCall(this, 1.5f, () =>
			{
				PhotonMultiplayerManager.Instance._runner = PhotonMultiplayerManager.Instance.gameObject.AddComponent<NetworkRunner>();
				if (NetworkGameManager.Instance.mode != NetworkGameManager.MultiplayerMode.Auto)
				{
					PhotonMultiplayerManager.Instance.JoinSession(NetworkGameManager.Instance.mode, NetworkGameManager.Instance.sessionName);
				}
			}).Forget();
			blackCrossfadeImage.DOFade(1f, 2f).SetDelay(1.5f).OnComplete(() =>
			{
				_loadingObject.SetActive(value: true);
				if (NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Auto)
				{
					PhotonMultiplayerManager.Instance.JoinSession(NetworkGameManager.Instance.mode, NetworkGameManager.Instance.sessionName);
					LoadingText.gameObject.SetActive(value: false);
					FindingRoomText.gameObject.SetActive(value: true);
				}
				else
				{
					LoadingText.gameObject.SetActive(value: true);
					FindingRoomText.gameObject.SetActive(value: false);
				}
				if (GlobalOptionsManager.Instance.seed == 0)
				{
					GlobalOptionsManager.Instance.seed = int.Parse(DateTime.Now.ToString("ddHHmmss"));
				}
				changeSceneToLobby = true;
			});
		}
		if (changeSceneToLobby && (PhotonMultiplayerManager.Instance._sessionConnected || NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Solo))
		{
			GlobalUIManager.Instance.ClickGoToScene(nextScene);
			changeSceneToLobby = false;
		}
	}

	private void Update()
	{
		if (!UIPatchNote.isHidden && GlobalOptionsManager.Instance.usingGamepad)
		{
			Vector2 vector = scrollPatchNoteAction.ReadValue<Vector2>();
			if (Mathf.Abs(vector.y) > 0.1f)
			{
				patchNoteScrollRect.verticalNormalizedPosition += vector.y * Time.deltaTime * scrollSpeed;
				patchNoteScrollRect.verticalNormalizedPosition = Mathf.Clamp01(patchNoteScrollRect.verticalNormalizedPosition);
			}
		}
	}

	public void BackToTitleMenu()
	{
		flowControlGraph.SetActiveNodeByName("Main Menu");
		ShowTitle();
		ShowInfoBot();
	}

	public void HideTitle()
	{
		titleObject.SetActive(value: false);
	}

	public void ShowTitle()
	{
		titleObject.SetActive(value: true);
	}

	public void HideUI(UIView uiView)
	{
		uiView.InstantHide();
	}

	public void HideInfoBot()
	{
		_objectPatchNote.SetActive(value: false);
		objectBot.SetActive(value: false);
	}

	public void ShowInfoBot()
	{
		textRegion.text = GetRegionName(GlobalSaveData.instance.optionData.region);
		if (GlobalSaveData.instance.UserSaveData.UserName != "")
		{
			_objectPatchNote.SetActive(value: true);
		}
		objectBot.SetActive(value: true);
	}

	public void GoToScene(string sceneName)
	{
		nextScene = sceneName;
		goToLobby = true;
		NetworkGameManager.Instance.mode = NetworkGameManager.MultiplayerMode.Solo;
		NetworkGameManager.Instance.sessionName = null;
	}

	public void ClickStartGame()
	{
		if (GameModes.Instance.isEvent)
		{
			flowControlGraph.enabled = false;
			NetworkGameManager.Instance.isPrivateRoom = true;
			goToLobby = true;
			NetworkGameManager.Instance.mode = NetworkGameManager.MultiplayerMode.Solo;
			NetworkGameManager.Instance.sessionName = null;
			HideTitle();
			HideInfoBot();
			AudioManager.PlaySFX("ui_gamestart");
		}
	}

	public void ClickSolo(bool isLoadGame)
	{
		AudioManager.PlaySFX("ui_gamestart");
		NetworkGameManager.Instance.isPrivateRoom = true;
		goToLobby = true;
		NetworkGameManager.Instance.mode = NetworkGameManager.MultiplayerMode.Solo;
		NetworkGameManager.Instance.sessionName = null;
		NetworkGameManager.Instance.isLoadGame = isLoadGame;
		HideTitle();
		HideInfoBot();
	}

	public void UI_ClickSoloDisableSaveData()
	{
		if (GameModes.Instance.CheckDisableSaveData())
		{
			flowControlGraph.enabled = false;
			GlobalSaveData.instance.gameData = new GameData();
			ClickSolo(isLoadGame: false);
			AudioManager.PlaySFX("ui_gamestart");
		}
	}

	public void UI_ClickCreatRoomSaveData(bool isPrivate)
	{
		if (GameModes.Instance.CheckDisableSaveData())
		{
			flowControlGraph.enabled = false;
			GlobalSaveData.instance.gameData = new GameData();
			ClickCreateRoom(isPrivate, isLoadGame: false);
			AudioManager.PlaySFX("ui_gamestart");
		}
	}

	public void ClickCreatePrivateRoom(bool isLoadGame)
	{
		ClickCreateRoom(isPrivate: true, isLoadGame);
	}

	public void ClickCreatePublicRoom(bool isLoadGame)
	{
		ClickCreateRoom(isPrivate: false, isLoadGame);
	}

	public void ClickCreateRoom(bool isPrivate, bool isLoadGame)
	{
		AudioManager.PlaySFX("ui_gamestart");
		goToLobby = true;
		NetworkGameManager.Instance.mode = NetworkGameManager.MultiplayerMode.Server;
		NetworkGameManager.Instance.isPrivateRoom = isPrivate;
		NetworkGameManager.Instance.isLoadGame = isLoadGame;
		HideTitle();
		HideInfoBot();
	}

	public void ClickAutoJoinCreate()
	{
		AudioManager.PlaySFX("ui_gamestart");
		GlobalSaveData.instance.gameData = new GameData();
		NetworkGameManager.Instance.isPrivateRoom = false;
		goToLobby = true;
		NetworkGameManager.Instance.mode = NetworkGameManager.MultiplayerMode.Auto;
		NetworkGameManager.Instance.sessionName = null;
		NetworkGameManager.Instance.isLoadGame = true;
		HideTitle();
		Instance.HideInfoBot();
	}

	public void ClickJoinRoom(string roomCode = "")
	{
		if (roomCode != "" || inputFieldRoomJoin.text != "")
		{
			AudioManager.PlaySFX("ui_gamestart");
			NetworkGameManager.Instance.isPrivateRoom = false;
			goToLobby = true;
			NetworkGameManager.Instance.mode = NetworkGameManager.MultiplayerMode.Client;
			if (roomCode != "")
			{
				NetworkGameManager.Instance.sessionName = roomCode;
			}
			else if (inputFieldRoomJoin.text != "")
			{
				NetworkGameManager.Instance.sessionName = inputFieldRoomJoin.text;
			}
			GlobalSaveData.instance.gameData = new GameData();
			NetworkGameManager.Instance.isLoadGame = true;
			GlobalSaveData.instance.optionData.lastRoomCode = inputFieldRoomJoin.text;
			GlobalSaveData.instance.SaveOptionData();
		}
		else if (inputFieldRoomJoin.text == "")
		{
			SteamApi.OpenFloatingKeyboard(inputFieldRoomJoin, 6);
		}
	}

	public void SelectButton(UIButton button)
	{
		button.Select();
	}

	public void SelectInputField(TMP_InputField inputField)
	{
		inputField.Select();
	}

	public void UsernameCreated()
	{
		if (GlobalSaveData.instance.UserSaveData.UserName == "")
		{
			DataCollection.SendAccept();
		}
		if (inputFieldUserName.text != "")
		{
			if (GlobalSaveData.instance.UserSaveData.UserName == "")
			{
				flowControlGraph.SetActiveNodeByName("Main Menu");
			}
			UIUsername.Hide();
			textUsername.text = inputFieldUserName.text;
			GlobalSaveData.instance.UserSaveData.UserName = inputFieldUserName.text;
			GlobalSaveData.instance.SaveUserData();
			ShowInfoBot();
			UIMainMenu.Show();
		}
		else
		{
			inputFieldUserName.image.DOKill();
			inputFieldUserName.image.color = new UnityEngine.Color(1f, 0f, 0f);
			inputFieldUserName.image.DOColor(new UnityEngine.Color(1f, 1f, 1f), 0.5f);
			SteamApi.OpenFloatingKeyboard(inputFieldUserName, 12);
		}
	}

	public string GetRegionName(string code)
	{
		return LocalizationManager.GetTranslation("Menu/" + code);
	}

	public void ShowUIUsername()
	{
		string nodeName = flowControlGraph.flow.activeNode.nodeName;
		if (nodeName == "Main Menu" || nodeName == "New Game")
		{
			if (UIMainMenu.isHidden)
			{
				flowControlGraph.SetActiveNodeByName("Main Menu");
			}
			UIMainMenu.Hide();
			HideTitle();
			UIUsername.Show();
			HideInfoBot();
			inputFieldUserName.Select();
		}
	}

	public void ShowUIPatchNote()
	{
		string nodeName = flowControlGraph.flow.activeNode.nodeName;
		if (nodeName == "Main Menu" || nodeName == "New Game")
		{
			GlobalSaveData.instance.IsPatchNoteShown = true;
			if (UIMainMenu.isHidden)
			{
				flowControlGraph.SetActiveNodeByName("Main Menu");
			}
			UIMainMenu.Hide();
			HideTitle();
			UIPatchNote.Show();
			HideInfoBot();
		}
	}

	public void ShowUIRegion()
	{
		string nodeName = flowControlGraph.flow.activeNode.nodeName;
		if (nodeName == "Main Menu" || nodeName == "New Game")
		{
			if (UIMainMenu.isHidden)
			{
				flowControlGraph.SetActiveNodeByName("Main Menu");
			}
			UIMainMenu.Hide();
			isPopup = true;
			HideTitle();
			UIRegion.Show();
			HideInfoBot();
		}
	}

	public void OnKeyChange()
	{
		inputFieldRoomJoin.text = inputFieldRoomJoin.text.ToUpper();
	}

	public void OnSubmit(InputAction.CallbackContext value)
	{
		if (!UIUsername.isHidden && value.canceled)
		{
			UsernameCreated();
			if (inputFieldUserName.text != "")
			{
				AudioManager.PlaySFX("ui_confirm");
			}
		}
		else if (!uiJoin.isHidden && inputFieldRoomJoin.text != "" && value.started)
		{
			ClickJoinRoom(inputFieldRoomJoin.text);
		}
		if (!UIPatchNote.isHidden && value.started)
		{
			UIPatchNote.InstantHide();
			ShowInfoBot();
			UniTaskUtil.DelayedCall(this, 0.2f, () =>
			{
				UIMainMenu.Show();
			}).Forget();
		}
	}

	public void OnCancel(InputAction.CallbackContext value)
	{
		if (value.started)
		{
			if (!uiJoin.isHidden)
			{
				uiJoin.Hide();
				uiStart.Show();
			}
			if (!UIUsername.isHidden && GlobalSaveData.instance.UserSaveData.UserName != "")
			{
				UIUsername.Hide();
				ShowInfoBot();
				UIMainMenu.Show();
			}
			if (!UIPatchNote.isHidden)
			{
				UIPatchNote.InstantHide();
				ShowInfoBot();
				UIMainMenu.Show();
			}
			if (_uiLeaderboardManager.IsShowing)
			{
				_uiLeaderboardManager.Hide();
			}
		}
	}

	public void OnLeftTabClick()
	{
		if (UIRegion.isHidden && UIUsername.isHidden && objectBot.activeSelf)
		{
			ShowUIUsername();
			AudioManager.PlaySFX("ui_confirm");
		}
	}

	public void OnRightTabClick()
	{
		if (UIUsername.isHidden && UIRegion.isHidden && objectBot.activeSelf)
		{
			ShowUIRegion();
			AudioManager.PlaySFX("ui_confirm");
		}
	}

	public void DeviceChange(PlayerInput myPlayerInput)
	{
		if (GlobalOptionsManager.Instance != null)
		{
			GlobalOptionsManager.Instance.DeviceChange(myPlayerInput);
		}
	}

	public void ShowJoinRoom()
	{
		inputFieldRoomJoin.Select();
	}

	public void Reconnect()
	{
		HideInfoBot();
		UIMainMenu.Hide();
		NetworkGameManager.Instance.isPrivateRoom = false;
		goToLobby = true;
		NetworkGameManager.Instance.isReconnecting = true;
		NetworkGameManager.Instance.mode = NetworkGameManager.MultiplayerMode.Client;
		NetworkGameManager.Instance.sessionName = GlobalSaveData.instance.optionData.lastRoomCode;
		GlobalSaveData.instance.gameData = new GameData();
		NetworkGameManager.Instance.isLoadGame = true;
	}

	public void ShowLeaderboard()
	{
		_uiLeaderboardManager.Show();
	}

	public void InitLeaderBoard()
	{
		SteamLeaderBoard steamLeaderBoard = SteamManager.Instance.SteamLeaderBoard;
		steamLeaderBoard.Init(OnComplete);
		async Task OnComplete()
		{
			LeaderboardEntry[] array = await steamLeaderBoard.GetScoreAroundUserAsync(0, 0);
			if (array != null && array.Length != 0)
			{
				LeaderboardEntry userLeaderboard = array[0];
				steamLeaderBoard.UserLeaderboard = userLeaderboard;
				steamLeaderBoard.UserRank = userLeaderboard.GlobalRank;
				RankText.text = userLeaderboard.GlobalRank.ToString();
			}
			else
			{
				RankText.text = "-";
			}
		}
	}
}

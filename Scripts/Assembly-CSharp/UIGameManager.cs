using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Doozy.Runtime.Reactor.Animators;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using Fusion;
using I2.Loc;
using TMPro;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using _Modules.CharacterSkin.Scripts;
using _Modules.Steam.Scripts;
using _Modules.UIInGame.Scripts;
using _Modules.UILobby.Scripts;

public class UIGameManager : MonoBehaviour
{
	public CanvasGroup canvasGroup;

	public CanvasScaler canvasScaler;

	public LoadingInGame loading;

	public RectTransform crosshair;

	public GridLayoutGroup crosshairGrid;

	public UIView uiInGame;

	public UIView uiInventory;

	public UIView uiPause;

	public UIView uiConfirmation;

	public UIView uiFailedConnect;

	public UIView uiOptions;

	public Canvas uiSteamFriends;

	public UIView uiTabKill;

	public UIView uiDebug;

	public UIView uiChatWheel;

	public GameObject uiObjective;

	public Canvas canvasCrosshair;

	public List<Image> weaponIconList = new List<Image>();

	public List<Image> ammoIconList = new List<Image>();

	public List<TextMeshProUGUI> txtAmountList = new List<TextMeshProUGUI>();

	public List<UIPlayerInfo> ArrPlayerInfo = new List<UIPlayerInfo>();

	public List<TextMeshProUGUI> txtKillList = new List<TextMeshProUGUI>();

	public GameObject inventoryOptions;

	public Localize titleWeapon;

	public Localize dscWeapon;

	public TextMeshProUGUI titleWeaponText;

	public TextMeshProUGUI dscWeaponText;

	public ReadyUIController readyUIController;

	public TextMeshProUGUI txtCountDown;

	public List<Animator> arrStatusHP = new List<Animator>();

	public InputSystemUIInputModule uiInputModule;

	public Localize txtTermHpStatusPlayer;

	public TextMeshProUGUI txtFailedConnect;

	public Localize txtInfoUITerm;

	public TextMeshProUGUI txtTime;

	public TextMeshProUGUI txtKill;

	public TextMeshProUGUI txtTimer;

	public TextMeshProUGUI txtHpValuePlayer;

	[SerializeField]
	private TMP_Text _armorText;

	[SerializeField]
	private Image _armorIconImage;

	public Slider barStamina;

	public Slider energyDrainBarStamina;

	public TextMeshProUGUI txtStaminaValuePlayer;

	public GameObject NoStaminaEffect;

	public RandomizeImage flashRed;

	public Image flashRed2;

	public Image flashGreen;

	public Image flashGreen2;

	public Image vfxCritical;

	public SpriteRenderer cursorGrenade;

	public TextMeshProUGUI fpsText;

	private float _fps;

	private int _frameCount;

	private float _timeAccumulator;

	public GameObject mapUI;

	public GameObject spectateObject;

	public UIAnimator animUIInventory;

	public Image throwableImage;

	public Image healingItemImage;

	public Image throwableIconImage;

	public Image healingItemIconImage;

	public bool isUIInvisible;

	public UIView UIMenuPuzzle;

	public UIView blankUI;

	public UIView UIMenuNote;

	public UIView UIMenuMap;

	public Image btnNextNote;

	public Image btnPrevNote;

	public TextMeshProUGUI txtTitleNote;

	public TextMeshProUGUI txtNote;

	public Image imgNote;

	public List<string> notes = new List<string>();

	public int pageNote;

	public GameObject roomTextCanvas;

	public RectTransform mapImage;

	public Text txtPingSource;

	public TextMeshProUGUI txtPing;

	public TextMeshProUGUI txtConnectionType;

	public TextMeshProUGUI txtTotEnemy;

	public TextMeshProUGUI txtReviveTimer;

	public GameObject uiHordeIncoming;

	public GameObject LabelHordeInfiniteIncoming;

	public GameObject LabelHordeIncoming;

	public GameObject fpsObject;

	public Transform playerInfo;

	public float mapImageXOffset;

	public float mapImageYOffset;

	public float mapImageXScaling;

	public float mapImageYScaling;

	public TextMeshProUGUI sessionName;

	public Image SessionFlashImage;

	public GameObject buttonCopyClipboard;

	public bool UIProgressing;

	public List<ConvertNote> arrConvertedText = new List<ConvertNote>();

	public UIInGameController uIInGameController;

	public Sprite defaultMapSprite;

	public Image missionMapImage;

	public Localize missionLocationText;

	public Localize missionObjectiveText;

	public TextMeshProUGUI missionLocationTextField;

	public TextMeshProUGUI textTotalPlayers;

	public GameObject micOff;

	public GameObject micOn;

	public GameObject sprintOff;

	public GameObject sprintOn;

	public Image fadeBlack;

	public List<string> ListAdditionalMissionTerm = new List<string>();

	public UIResultManager uIResultManager;

	public GameObject IconHostReady;

	public TextMeshProUGUI TextTimeIncomingWave;

	[SerializeField]
	private SteamRichPresenceLobbyController _steamRichPresenceLobbyController;

	private float _timer;

	private const float Interval = 1f;

	public static UIGameManager Instance { get; private set; }

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
		flashRed.image.DOFade(0f, 0f);
	}

	private void Start()
	{
		if (LobbyManager.Instance != null && NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Solo)
		{
			if (uiObjective != null)
			{
				uiObjective.SetActive(value: false);
				uiObjective = null;
			}
			if ((bool)IconHostReady)
			{
				IconHostReady.SetActive(value: false);
			}
		}
		if (roomTextCanvas == null)
		{
			roomTextCanvas = GameObject.Find("RoomText");
		}
		if ((bool)missionLocationText)
		{
			missionLocationTextField = missionLocationText.GetComponent<TextMeshProUGUI>();
		}
		SetMissionLocation(missionLocationText, null, missionLocationTextField);
		crosshairGrid = crosshair.GetComponent<GridLayoutGroup>();
		flashRed2.transform.SetSiblingIndex(1);
		vfxCritical.transform.SetSiblingIndex(1);
		if (LobbyManager.Instance == null)
		{
			if ((bool)NetworkGameManager.Instance.ownPlayer)
			{
				NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: false);
			}
			HideMapNameText();
		}
		if (!GlobalSaveData.instance.optionData.showFpsRtt)
		{
			Instance.fpsObject.SetActive(value: false);
		}
		else
		{
			Instance.fpsObject.SetActive(value: true);
		}
		if ((bool)NetworkGameManager.Instance && (bool)NetworkGameManager.Instance.ownPlayer)
		{
			micOn.SetActive(NetworkGameManager.Instance.ownPlayer.IsMicOn);
			micOff.SetActive(!NetworkGameManager.Instance.ownPlayer.IsMicOn);
		}
		SetUIPing();
	}

	private void Update()
	{
		_frameCount++;
		_timer += Time.unscaledDeltaTime;
		if (!(_timer < 1f))
		{
			_fps = (float)_frameCount / _timer;
			_timer = 0f;
			_frameCount = 0;
			UpdateUIPingFPS();
		}
	}

	private void UpdateUIPingFPS()
	{
		fpsText.text = $"{_fps:F0}";
		RefreshUIPing();
	}

	private void FixedUpdate()
	{
		SetPosPlayerText();
	}

	private void SetUIPing()
	{
		if (NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Solo)
		{
			txtPing.text = txtPingSource.text;
			txtConnectionType.text = "Solo";
		}
		else if (PhotonMultiplayerManager.Instance._runner.CurrentConnectionType == ConnectionType.None)
		{
			txtPing.text = txtPingSource.text;
			txtConnectionType.text = "Host";
		}
		else
		{
			txtPing.text = txtPingSource.text;
			txtConnectionType.text = PhotonMultiplayerManager.Instance._runner.CurrentConnectionType.ToString();
		}
	}

	private void RefreshUIPing()
	{
		if (NetworkGameManager.Instance.mode != NetworkGameManager.MultiplayerMode.Solo)
		{
			txtPing.text = txtPingSource.text;
			if (PhotonMultiplayerManager.Instance._runner.CurrentConnectionType != ConnectionType.None)
			{
				txtConnectionType.text = PhotonMultiplayerManager.Instance._runner.CurrentConnectionType.ToString();
			}
		}
	}

	private void SetPosPlayerText()
	{
		if (!(CameraGame.Instance.mainCam != null))
		{
			return;
		}
		for (int i = 0; i < NetworkGameManager.Instance.arrPlayerController.Count; i++)
		{
			if (NetworkGameManager.Instance.arrPlayerController[i] != null)
			{
				Vector3 localPosition = WorldToCanvasPoint(NetworkGameManager.Instance.arrPlayerController[i].top.position);
				ArrPlayerInfo[NetworkGameManager.Instance.arrPlayerController[i].network.playerIdx].rectTransform.localPosition = localPosition;
			}
		}
	}

	public Vector3 WorldToCanvasPoint(Vector3 worldPosition)
	{
		if (canvasScaler == null)
		{
			canvasScaler = GetComponent<CanvasScaler>();
		}
		float num = canvasScaler.referenceResolution.y / (float)Screen.height;
		Vector3 result = CameraGame.Instance.mainCam.WorldToScreenPoint(worldPosition);
		result.x = Mathf.Round((result.x - (float)Screen.width / 2f) * num);
		result.y = Mathf.Round((result.y - (float)Screen.height / 2f) * num);
		result.z = 0f;
		return result;
	}

	public void DisableControlPlayer()
	{
		NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: false);
		NetworkGameManager.Instance.ownPlayer.direction = new Vector3(0f, 0f, 0f);
	}

	public void BackToInGame(UIView uiHide)
	{
		if ((bool)NetworkGameManager.Instance.ownPlayer)
		{
			NetworkGameManager.Instance.ownPlayer.DelayInputTimer.StartDuration(0.5f);
		}
		if ((bool)NetworkGameManager.Instance.ownPlayer)
		{
			if (!LobbyManager.Instance || ((bool)LobbyManager.Instance && (bool)LobbyManager.Instance.UIResult && !LobbyManager.Instance.UIResult.activeSelf))
			{
				NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: true);
			}
			NetworkGameManager.Instance.ownPlayer.network.SetPlayerAFK(value: false);
		}
		GameManager.Instance.ResumeGameTime();
		if ((bool)uiHide)
		{
			uiHide.Hide();
		}
		if (isUIInvisible)
		{
			return;
		}
		uiInGame.Show();
		if (LobbyManager.Instance == null)
		{
			if (uiObjective != null)
			{
				uiObjective.SetActive(value: true);
			}
			if (LobbyManager.Instance == null)
			{
				mapUI.SetActive(value: true);
			}
		}
		else if (uiHide != null && uiHide.gameObject.name == "View - Pause" && uiObjective != null)
		{
			uiObjective.SetActive(value: true);
		}
	}

	public void ShowConfirmation()
	{
		uiPause.Hide();
		uiConfirmation.Show();
	}

	public void CloseConfirmation()
	{
		uiPause.Show();
		uiConfirmation.Hide();
	}

	public void Reconnect()
	{
		uiFailedConnect.Hide();
		NetworkGameManager.Instance.StartGame(NetworkGameManager.Instance.mode, NetworkGameManager.Instance.sessionName);
	}

	public void QuickQuitGame(bool withoutConnection = false)
	{
		Time.timeScale = 1f;
		GameManager.Instance.quitGame = true;
		GlobalUIManager.Instance.ClickGoToScene("MainMenu");
		if (withoutConnection)
		{
			uiFailedConnect.Hide();
		}
		else
		{
			NetworkGameManager.Instance.Shutdown();
		}
		loading.loadingUI.SetActive(value: true);
	}

	public void QuitGame(bool withoutConnection = false)
	{
		if (LobbyManager.Instance != null)
		{
			LobbyManager.Instance.SaveItemLobby();
			GlobalSaveData.instance.SaveGameData(NetworkGameManager.Instance.ownPlayer, GameManagerPhoton.Instance);
		}
		Time.timeScale = 1f;
		GameManager.Instance.quitGame = true;
		if (withoutConnection)
		{
			uiFailedConnect.Hide();
		}
		if ((bool)NetworkGameManager.Instance.ownPlayer)
		{
			NetworkGameManager.Instance.ownPlayer.network.playerPhoton.RpcSetQuitGame();
		}
		StartCoroutine(DelayQuitGame(!withoutConnection));
		loading.loadingUI.SetActive(value: true);
		if (!NetworkGameManager.Instance.IsErrorConnection)
		{
			GlobalSaveData.instance.optionData.lastRoomCode = "";
			GlobalSaveData.instance.optionData.lastSeed = 0;
		}
		GlobalSaveData.instance.SaveOptionData();
		NetworkGameManager.Instance.IsErrorConnection = false;
	}

	public IEnumerator DelayQuitGame(bool isNeedShutdown = false)
	{
		yield return new WaitForSeconds(1f);
		GlobalUIManager.Instance.ClickGoToScene("MainMenu");
		if (isNeedShutdown)
		{
			NetworkGameManager.Instance.Shutdown();
		}
	}

	public void ShowHideInventory()
	{
		if (Instance.uiPause.isHidden && Instance.UIMenuPuzzle.isHidden && (Instance.UIMenuNote.isHidden || !Instance.UIMenuNote.gameObject.activeSelf))
		{
			bool showKillTab = SceneManager.GetActiveScene().name != "Lobby" && NetworkGameManager.Instance.mode != NetworkGameManager.MultiplayerMode.Solo;
			if (Instance.uiInventory.isHidden && NetworkGameManager.Instance.ownPlayer.network.GetHealth() > 0f)
			{
				ShowInventoryUI(selectFirstButton: true, showKillTab);
			}
			else if (!Instance.uiInventory.isHidden)
			{
				HideInventoryUI(showKillTab);
			}
		}
	}

	public void HideInventory()
	{
		if (Instance.UIMenuPuzzle.isHidden && Instance.UIMenuNote.isHidden)
		{
			bool showKillTab = SceneManager.GetActiveScene().name != "Lobby" && NetworkGameManager.Instance.mode != NetworkGameManager.MultiplayerMode.Solo;
			if (!Instance.uiInventory.isHidden)
			{
				HideInventoryUI(showKillTab);
			}
		}
	}

	public void ShowInventoryWhenCraft()
	{
		if (Instance.uiPause.isHidden && (Instance.UIMenuNote.isHidden || !Instance.UIMenuNote.gameObject.activeSelf) && Instance.uiInventory.isHidden && NetworkGameManager.Instance.ownPlayer.network.GetHealth() > 0f)
		{
			ShowInventoryUI(selectFirstButton: false);
			if ((bool)uIInGameController)
			{
				uIInGameController?.SetCraftingMaterialsUI(show: false);
				uIInGameController.SetPlayerStatusUI(setActive: false);
				uIInGameController.SetReadyStatusUI(setActive: false);
			}
		}
	}

	public void HideInventoryWhenCraft()
	{
		if (!Instance.uiInventory.isHidden)
		{
			HideInventoryUI();
			uIInGameController?.SetPlayerStatusUI(setActive: true);
			if (LobbyManager.Instance != null)
			{
				uIInGameController?.SetReadyStatusUI(setActive: true);
			}
		}
	}

	private void ShowInventoryUI(bool selectFirstButton = true, bool showKillTab = false)
	{
		fpsObject.SetActive(value: false);
		EventSystem.current.SetSelectedGameObject(null);
		animUIInventory.PlayFromToProgress(0f, 1f);
		NetworkGameManager.Instance.ownPlayer.network.SetPlayerAFK(value: true);
		AudioManager.PlaySFX("inventory-open");
		Instance.uiInventory.Show();
		if (uiObjective != null && LobbyManager.Instance != null)
		{
			uiObjective.SetActive(value: false);
		}
		if (LobbyManager.Instance == null)
		{
			if (uiObjective != null)
			{
				uiObjective.SetActive(value: false);
			}
			mapUI.SetActive(value: false);
		}
		Instance.inventoryOptions.SetActive(value: false);
		NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: false);
		NetworkGameManager.Instance.ownPlayer.inventoryManager.frameInventory.Play("Inventory" + (NetworkGameManager.Instance.ownPlayer.data.GetMaxInventory() - 2));
		NetworkGameManager.Instance.ownPlayer.direction = Vector3.zero;
		NetworkGameManager.Instance.ownPlayer.fsmUpperBody.SetBool("isMoving", value: false);
		NetworkGameManager.Instance.ownPlayer.fsmLowerBody.SetBool("isMoving", value: false);
		NetworkGameManager.Instance.ownPlayer.animLowerChar.Play("LegIdle" + NetworkGameManager.Instance.ownPlayer.angleRot, 1);
		foreach (Button item in NetworkGameManager.Instance.ownPlayer.inventoryManager.buttonInventory)
		{
			item.interactable = true;
		}
		if (NetworkGameManager.Instance.ownPlayer.fsmUpperBody.GetBool(NetworkGameManager.Instance.ownPlayer.IsThrowingAnim))
		{
			NetworkGameManager.Instance.ownPlayer.network.ExecCancelThrow();
			Instance.cursorGrenade.gameObject.SetActive(value: false);
		}
		if (showKillTab)
		{
			NetworkGameManager.Instance.ownPlayer.InitPlayerList();
			Instance.uiTabKill.Show();
		}
		if (selectFirstButton)
		{
			NetworkGameManager.Instance.ownPlayer.inventoryManager.SelectFirstButton();
		}
		uIInGameController?.SetCraftingMaterialsUI(show: true);
	}

	private void HideInventoryUI(bool showKillTab = false)
	{
		if (GlobalSaveData.instance.optionData.showFpsRtt)
		{
			Instance.fpsObject.SetActive(value: true);
		}
		EventSystem.current.SetSelectedGameObject(null);
		animUIInventory.PlayFromToProgress(1f, 0f);
		NetworkGameManager.Instance.ownPlayer.network.SetPlayerAFK(value: false);
		AudioManager.PlaySFX("inventory-close");
		Instance.uiInventory.Hide();
		if (!Instance.isUIInvisible)
		{
			if (LobbyManager.Instance == null)
			{
				Instance.mapUI.SetActive(value: true);
			}
			if (Instance.uiObjective != null)
			{
				Instance.uiObjective.SetActive(value: true);
			}
		}
		Instance.inventoryOptions.SetActive(value: false);
		NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: true);
		foreach (GameObject item in NetworkGameManager.Instance.ownPlayer.inventoryManager.inventoryPick)
		{
			item.SetActive(value: false);
		}
		foreach (Button item2 in NetworkGameManager.Instance.ownPlayer.inventoryManager.buttonInventory)
		{
			item2.interactable = false;
		}
		if (showKillTab)
		{
			Instance.uiTabKill.Hide();
		}
		uIInGameController?.SetCraftingMaterialsUI(show: false);
	}

	public void ShowOptions()
	{
		NetworkGameManager.Instance.ownPlayer.network.SetPlayerAFK(value: true);
		uiOptions.Show();
		uiPause.Hide();
	}

	public void HideOptions()
	{
		if (Instance != null)
		{
			foreach (ConvertNote item in Instance.arrConvertedText)
			{
				item.textMesh.text = Instance.ConvertNote(item.initText);
			}
		}
		NetworkGameManager.Instance.ownPlayer.network.SetPlayerAFK(value: false);
		uiOptions.Hide();
		if (OptionsManager.Instance.IsShowControlOnly)
		{
			OptionsManager.Instance.IsShowControlOnly = false;
			OptionsManager.Instance.TabButtonObject.SetActive(value: true);
			uiOptions.Hide();
			BackToInGame(null);
		}
		else
		{
			uiPause.Show();
		}
	}

	public void SelectButton(UIButton button)
	{
		button.Select();
	}

	public void SelectButtonUnity(Button button)
	{
		button.Select();
	}

	public void ShowFailedConnect(string codeI2Lang)
	{
		NetworkGameManager.Instance.IsErrorConnection = true;
		txtFailedConnect.text = LocalizationManager.GetTranslation("Menu/" + codeI2Lang);
		Debug.Log("----ERROR " + codeI2Lang);
		uiFailedConnect.Show();
	}

	public void ChangeNotePage(bool isLeft)
	{
		if (isLeft)
		{
			ChangeNotePage(new Vector3(-1f, 0f, 0f));
		}
		else
		{
			ChangeNotePage(new Vector3(1f, 0f, 0f));
		}
	}

	public void GoToExternalLink(string extLink)
	{
		Application.OpenURL(extLink);
	}

	public void PressAnyKeyToLobby()
	{
		Instance.loading.discaimer.SetActive(value: false);
		Instance.loading.pressAnyKey.SetActive(value: false);
		Instance.loading.loadingUI.SetActive(value: false);
		Instance.loading.loadingScan.SetActive(value: true);
		Instance.loading.loadingText.SetActive(value: true);
		UniTaskUtil.DelayedCall(this, 0.1f, () =>
		{
			NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: true);
		}).Forget();
	}

	public void ChangeNotePage(Vector3 direction)
	{
		if (notes.Count > 1)
		{
			Instance.btnPrevNote.enabled = true;
			Instance.btnNextNote.enabled = true;
		}
		if (direction.x <= -0.5f)
		{
			if (pageNote > 0)
			{
				AudioManager.PlaySFX("examine-paper-change-page");
				pageNote--;
			}
		}
		else if (direction.x >= 0.5f && pageNote < notes.Count - 1)
		{
			AudioManager.PlaySFX("examine-paper-change-page");
			pageNote++;
		}
		if (pageNote == 0)
		{
			Instance.btnPrevNote.enabled = false;
		}
		else if (pageNote == notes.Count - 1)
		{
			Instance.btnNextNote.enabled = false;
		}
		txtNote.richText = false;
		if (notes.Count > pageNote)
		{
			txtNote.text = Instance.ConvertNote(notes[pageNote]);
		}
		txtNote.richText = true;
	}

	public void ShowPlayerInfo(string info)
	{
		playerInfo.gameObject.SetActive(value: true);
		playerInfo.DOKill();
		playerInfo.DOScale(0f, 0f);
		for (int i = 0; i < 2; i++)
		{
			playerInfo.GetChild(i).GetComponent<TextMeshProUGUI>().text = info;
		}
		playerInfo.DOScale(1f, 0.3f).SetEase(Ease.OutQuad);
		playerInfo.DOScale(0f, 0.2f).SetDelay(5f).OnComplete(() =>
		{
			playerInfo.gameObject.SetActive(value: true);
		});
	}

	public void CopyRoomCodeToClipboard()
	{
		GUIUtility.systemCopyBuffer = NetworkGameManager.Instance.sessionName;
	}

	public string ConvertNote(string strNote)
	{
		string text = strNote;
		int num = -1;
		int num2 = -1;
		string substr = "[#";
		int num3 = Count(text, substr);
		for (int i = 0; i < num3; i++)
		{
			num = text.IndexOf("[#");
			num2 = text.IndexOf("#]");
			if (num >= 0 && num2 >= 0 && num < num2)
			{
				text = text.Replace(text.Substring(num, num2 - num + 2), LocalizationManager.GetTranslation("Note/" + text.Substring(num + 2, num2 - num - 2)));
			}
		}
		substr = "[*";
		num3 = Count(text, substr);
		for (int j = 0; j < num3; j++)
		{
			num = text.IndexOf("[*");
			num2 = text.IndexOf("*]");
			if (num >= 0 && num2 >= 0 && num < num2)
			{
				text = text.Replace(text.Substring(num, num2 - num + 2), "<b><color=#7fc1ff>" + text.Substring(num + 2, num2 - num - 2) + "</color></b>");
			}
		}
		substr = "[%";
		num3 = Count(text, substr);
		for (int k = 0; k < num3; k++)
		{
			num = text.IndexOf("[%");
			num2 = text.IndexOf("%]");
			if (num >= 0 && num2 >= 0 && num < num2)
			{
				text = text.Replace(text.Substring(num, num2 - num + 2), "<b><color=#324c65>" + text.Substring(num + 2, num2 - num - 2) + "</color></b>");
			}
		}
		substr = "[$";
		num3 = Count(text, substr);
		for (int l = 0; l < num3; l++)
		{
			num = text.IndexOf("[$");
			num2 = text.IndexOf("$]");
			if (num >= 0 && num2 >= 0 && num < num2)
			{
				text = text.Replace(text.Substring(num, num2 - num + 2), "<b><color=#ff5a5a>" + text.Substring(num + 2, num2 - num - 2) + "</color></b>");
			}
		}
		substr = "[&";
		num3 = Count(text, substr);
		for (int m = 0; m < num3; m++)
		{
			num = text.IndexOf("[&");
			num2 = text.IndexOf("&]");
			if (num >= 0 && num2 >= 0 && num < num2)
			{
				text = text.Replace(text.Substring(num, num2 - num + 2), "<b><color=#ce8127>" + text.Substring(num + 2, num2 - num - 2) + "</color></b>");
			}
		}
		return text;
	}

	private int Count(string s, string substr, StringComparison strComp = StringComparison.CurrentCulture)
	{
		int num = 0;
		for (int num2 = s.IndexOf(substr, strComp); num2 != -1; num2 = s.IndexOf(substr, num2 + substr.Length, strComp))
		{
			num++;
		}
		return num;
	}

	public void SetUIVisibility(bool setActiveUI)
	{
		isUIInvisible = !setActiveUI;
		canvasGroup.alpha = (setActiveUI ? 1 : 0);
	}

	public void SetMissionLocation(Localize location, Localize objective, TextMeshProUGUI locationText = null)
	{
		if (!(NetworkGameManager.Instance != null))
		{
			return;
		}
		_ = NetworkGameManager.Instance.Mission - 1;
		_ = 0;
		if (objective != null && (bool)GameManagerPhoton.Instance && (bool)GameManagerPhoton.Instance.CurrentMission)
		{
			objective.SetTerm(GameManagerPhoton.Instance.CurrentMission.MissionObjective.MissionObjectiveLocalization);
		}
		if (location != null && (bool)GameManagerPhoton.Instance && (bool)GameManagerPhoton.Instance.CurrentMission)
		{
			location.SetTerm(GameManagerPhoton.Instance.CurrentMission.MapNameLocalization);
		}
		if ((bool)locationText && (bool)LobbyManager.Instance)
		{
			if (LobbyManager.Instance.LobbyState == LobbyManager.LobbyStateEnum.Car)
			{
				locationText.text = LocalizationManager.GetTranslation("Menu/Mission").ToUpper() + " : " + location.GetMainTargetsText();
			}
			else
			{
				locationText.text = LocalizationManager.GetTranslation("Menu/Mission").ToUpper() + " : ---";
			}
			if ((bool)missionMapImage && (bool)GameManagerPhoton.Instance)
			{
				missionMapImage.sprite = GameManagerPhoton.Instance.CurrentMission?.GetMapImage() ?? defaultMapSprite;
			}
		}
	}

	public void ShowSteamFriends()
	{
		NetworkGameManager.Instance.ownPlayer.network.SetPlayerAFK(value: true);
		uiSteamFriends.enabled = true;
		uiPause.Hide();
	}

	public void ShowUIInGame(UIView uiClose)
	{
		if (uiClose != null && !uiClose.isHidden)
		{
			uiClose.Hide();
		}
		if (!Instance.isUIInvisible)
		{
			uiInGame.Show();
			if (Instance.uiObjective != null && Instance.uiObjective != null)
			{
				Instance.uiObjective.SetActive(value: true);
			}
			mapUI.SetActive(value: true);
		}
		NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: true);
		ChatSystem.Instance.ItemCommand.SetActive(value: false);
		uiTabKill.gameObject.SetActive(value: true);
		if (NetworkGameManager.Instance.isServer)
		{
			NetworkGameManager.Instance.ownPlayer.network.playerPhoton.IsInteractingPuzzle = false;
		}
		else
		{
			NetworkGameManager.Instance.ownPlayer.network.playerPhoton.RpcSetInteractingPuzzle(value: false);
		}
	}

	public void ChangeMiniAvatarReadyStatus(int index, PlayerSkinData playerSkinData)
	{
		readyUIController?.GetUITabPlayer(index)?.ChangePlayerAvatar(playerSkinData.GetHeadSkinAvatar(), playerSkinData.GetBodySkinAvatar());
	}

	public void SetPerkSkillUIInfo(PlayerController playerController)
	{
		readyUIController?.Init(playerController);
	}

	public void SetSkillUIInfo(PlayerController playerController)
	{
		readyUIController?.InitSkill(playerController);
	}

	public void SetPerksUIInfo(PlayerController playerController)
	{
		readyUIController?.InitPerk(playerController);
	}

	public void HidePerkSkillUIInfo(PlayerController playerController)
	{
		readyUIController?.Hide(playerController);
	}

	public void ClickCBReady(int idxPlayer)
	{
		if (!LobbyManager.Instance || LobbyManager.Instance.LobbyState != LobbyManager.LobbyStateEnum.Car || (idxPlayer != -1 && (idxPlayer < 0 || !NetworkGameManager.Instance.GetPlayer(idxPlayer).network.isLocalPlayer)))
		{
			return;
		}
		if (NetworkGameManager.Instance.ownPlayer.network.GetReadyLobby())
		{
			SurvivorLobbyManager.Instance._txtReady.SetTerm("Menu/Ready");
		}
		else
		{
			UniTaskUtil.DelayedCall(this, 0.2f, () =>
			{
				SurvivorLobbyManager.Instance._txtReady.SetTerm("Menu/NotReady");
			}).Forget();
		}
		NetworkGameManager.Instance.ownPlayer.network.SetPlayerReady(!NetworkGameManager.Instance.ownPlayer.network.GetReadyLobby());
	}

	public void RefreshPlayerCountText()
	{
		if ((bool)textTotalPlayers)
		{
			if (NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Solo)
			{
				textTotalPlayers.text = NetworkGameManager.Instance.arrPlayerController.Count + "/1";
			}
			else
			{
				TextMeshProUGUI textMeshProUGUI = textTotalPlayers;
				string text = NetworkGameManager.Instance.arrPlayerController.Count.ToString();
				int mAX_PLAYERS = PhotonMultiplayerManager.MAX_PLAYERS;
				textMeshProUGUI.text = text + "/" + mAX_PLAYERS;
			}
		}
		_steamRichPresenceLobbyController?.UpdateRichPresence();
	}

	public void HideMapNameText()
	{
	}

	public void ShowMapNameText()
	{
	}

	public void SetThrowableShortcutSprite(Sprite sprite)
	{
		throwableImage.enabled = true;
		throwableImage.sprite = sprite;
		throwableIconImage.enabled = false;
	}

	public void HideThrowableShortcutSprite()
	{
		throwableImage.enabled = false;
		throwableIconImage.enabled = true;
	}

	public void SetHealingShortcutSprite(Sprite sprite)
	{
		healingItemImage.enabled = true;
		healingItemImage.sprite = sprite;
		healingItemIconImage.enabled = false;
	}

	public void HideHealingShortcutSprite()
	{
		healingItemImage.enabled = false;
		healingItemIconImage.enabled = true;
	}

	public string GetAttachedWeaponName(int itemID, bool isUsingParentheses = true)
	{
		string text = BGDatabase_Weapon.GetEntityByKeyid(itemID).Name;
		string text2 = " ";
		if (isUsingParentheses)
		{
			text2 = " (";
		}
		bool flag = false;
		if (text.IndexOf("_B", StringComparison.Ordinal) >= 0)
		{
			text2 += "B";
			flag = true;
		}
		if (text.IndexOf("_S", StringComparison.Ordinal) >= 0)
		{
			if (flag)
			{
				text2 += "+";
			}
			text2 += "S";
			flag = true;
		}
		if (text.IndexOf("_M", StringComparison.Ordinal) >= 0)
		{
			if (flag)
			{
				text2 += "+";
			}
			text2 += "M";
			flag = true;
		}
		if (!flag)
		{
			text2 = "";
		}
		else if (isUsingParentheses)
		{
			text2 += ")";
		}
		return text2;
	}

	public string GetAttachedWeaponDesc(int itemID)
	{
		string text = " " + LocalizationManager.GetTranslation("Weapon/DscUpgradedWeapon1");
		string text2 = BGDatabase_Weapon.GetEntityByKeyid(itemID).Name;
		string text3 = " ";
		bool flag = false;
		if (text2.IndexOf("_B", StringComparison.Ordinal) >= 0)
		{
			text3 += LocalizationManager.GetTranslation("Item/Item362");
			flag = true;
		}
		if (text2.IndexOf("_S", StringComparison.Ordinal) >= 0)
		{
			if (flag)
			{
				text3 += " + ";
			}
			text3 += LocalizationManager.GetTranslation("Item/Item363");
			flag = true;
		}
		if (text2.IndexOf("_M", StringComparison.Ordinal) >= 0)
		{
			if (flag)
			{
				text3 += " + ";
			}
			text3 += LocalizationManager.GetTranslation("Item/Item364");
			flag = true;
		}
		if (!flag)
		{
			return "";
		}
		return text.Replace("[x]", text3);
	}

	public void UpdateArmorUI(int durability)
	{
		if (durability > 0)
		{
			_armorIconImage.gameObject.SetActive(value: true);
			_armorText.text = durability.ToString();
			_armorText.gameObject.SetActive(value: true);
		}
		else
		{
			_armorIconImage.gameObject.SetActive(value: false);
			_armorText.text = "0";
			_armorText.gameObject.SetActive(value: false);
		}
	}
}

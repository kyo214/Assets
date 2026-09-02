using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Chronos;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DarkTonic.MasterAudio;
using Fusion;
using TMPro;
using Toked;
using Toked.StatusEffect;
using Toked.Weapon;
using Toked.Weapon.Throwable;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;
using _Modules.CharacterSkin;
using _Modules.CharacterSkin.Scripts;
using _Modules.Effects.StatusEffectsScripts;
using _Modules.Player.BaseScripts;

public class PlayerController : MonoBehaviour
{
	public Chronos.Timeline timeline;

	public static string PLAYER_COLLIDER_TAG = "PlayerCollider";

	public static string PLAYER_TAG = "Player";

	public PlayerData data;

	public PlayerNetwork network;

	public WeaponController weaponController;

	public FeedbackPlayerController feedbackController;

	public InventoryManager inventoryManager;

	public ArmorManager ArmorManager;

	public ItemBoxController ItemBoxController;

	public PlayerCharacterRenderController characterRenderController;

	public List<SpriteRenderer> allSpriteParts = new List<SpriteRenderer>();

	public List<SpriteRenderer> upperParts = new List<SpriteRenderer>();

	public List<Light> allLights = new List<Light>();

	public List<float> allLightIntensity = new List<float>();

	public SpriteLibrary headSpriteLib;

	[FormerlySerializedAs("_statsData")]
	[SerializeField]
	private PlayerMultiplyStats multiplyStatsData;

	public SpriteRenderer shadow;

	public AudioListener audioListener;

	public PlayerInput playerInput;

	public Animator fsmUpperBody;

	public Animator fsmLowerBody;

	public Vector3 direction;

	public Vector3 directionDash;

	public Vector2 inputRotation;

	public Vector2 prevInputRotation;

	public Vector3 angleInput;

	public Vector3 angleGround;

	public Vector2 prevNav;

	public Animator animUpperChar;

	public Animator animLowerChar;

	public bool isLMBDown;

	public bool isRMBDown;

	public bool isBtnDashDown;

	public bool isDashDown;

	public bool isBtnSprintDown;

	public bool isSprintDown;

	public bool enableMoveChar = true;

	public Transform origin;

	public Transform top;

	public Transform weaponPos;

	public Transform weaponPosSprite;

	public Transform object2D;

	[SerializeField]
	private float controllerDeadZone = 0.2f;

	public Transform meleeCollider;

	public Transform RoundMeleeCollider;

	public Transform punchCollider;

	public bool isAimingToggle;

	public bool isAiming;

	public bool isThrowing;

	public bool isAttacking;

	public bool isShooting;

	public bool isAttackMelee;

	public bool isAttackMeleeSwing;

	public bool isAttackBtnPressed;

	public bool isRangeActive;

	public bool isDashing;

	public bool isDashingMove;

	public bool canDash;

	public bool isSprinting;

	public bool canSprint;

	public bool runPressing;

	public bool canGrenade;

	public bool isHurt;

	public bool isLowHealth;

	[SerializeField]
	private bool _isGod;

	[SerializeField]
	private bool _isGhost;

	[SerializeField]
	private bool _isNoStamina;

	public bool IsDoubleDamage;

	public bool IsSpeedIncreaseBy2;

	[SerializeField]
	private bool _isMaxSpeed;

	[SerializeField]
	private Vector3 _aimDirection;

	private Vector3 _lastAngleGamepad;

	private Vector3 _lastAngleGamepadGrenade;

	public Vector2 prevAim;

	public float angleRot;

	public float angleRotWithoutCam;

	public float prevAngleRot;

	public float angleWalk;

	public LayerMask layerCollider;

	public LayerMask layerColliderFriendlyFire;

	public GameObject itemCollision;

	public Collider itemCollisionCollider;

	public string functionItemCollision;

	public GameObject flashlight;

	public bool isDecreasingSanity;

	public bool initPos;

	public UnityEvent onSkip;

	public BoxCollider reviveArea;

	public BoxCollider healArea;

	public GameObject playerCollider;

	public Collider playerColliderComponent;

	public FieldOfView fov;

	public FieldOfView fov2;

	private bool _pointerSkipDown;

	private float _pointerDownTimer;

	private float _requiredHoldTime = 1f;

	private static readonly int IsMovingAnim = Animator.StringToHash("isMoving");

	private static readonly int IsMeleeAnim = Animator.StringToHash("isMelee");

	private static readonly int IsShooting = Animator.StringToHash("isShooting");

	public int IsThrowingAnim = Animator.StringToHash("isThrowing");

	public Animator iconCharMapAnimator;

	public SpriteRenderer iconCharDeadMap;

	public Transform iconCharMap;

	public Transform directionMap;

	public Vector3 navigationMap;

	public GameObject cursorLocalPlayer;

	public int scalingMap;

	public float animspeed;

	public SoundStepType soundStepType;

	public List<SoundStepType> ArrSoundStepTypeCollide = new List<SoundStepType>();

	public ShellCasing shell;

	[SerializeField]
	private int targetIdxCamBeforeRevive;

	public XTimer delaySpectator;

	public string RoomName;

	public List<string> roomColliders = new List<string>();

	public SpriteRenderer sweatVFX;

	public Transform targetedPoint;

	private bool isPressingActionPuzzle;

	public bool isPermadeath;

	public bool isEntangled;

	public int ctrReleaseEntangled;

	public int ctrGetUp;

	public int maxCtrReleaseEntangled;

	public int maxCtrGetUp;

	public SpriteRenderer bloodPool;

	public XTimer reviveTimer;

	public CharacterTrail trail;

	public SortingGroup sortGroup;

	public GameObject objectFOVCollider;

	public XTimer invincibleTimer;

	public XTimer DisconnectedTimer;

	public XTimer DespawnTimer;

	public XTimer DelayInputTimer;

	private float _sprintTolerance;

	public bool IsMicOn;

	[SerializeField]
	private StatusEffectController _statusEffectController;

	[SerializeField]
	private DizzinessManager _dizzinessManager;

	public ScorePlayer ScorePlayerNetwork;

	[SerializeField]
	private StatusEffectDebugUI _statusEffectDebugUIPrefab;

	private StatusEffectDebugUI _statsDebugUI;

	private IScramble _inputScramble = ControlScrambler.GenerateScramble(ControlScrambler.ScrambleType.None);

	private Vector2 _rawInput = Vector2.zero;

	public PlayerMultiplyStats PlayerMultiplyStatsData => multiplyStatsData;

	public bool IsGod => _isGod;

	public bool IsGhost => _isGhost;

	public bool IsNoStamina => _isNoStamina;

	public bool IsMaxSpeed => _isMaxSpeed;

	public int TargetIdxCamBeforeRevive => targetIdxCamBeforeRevive;

	public bool IsMale => data.PlayerSkinData.Gender == CharacterSkinData.Gender.Male;

	public StatusEffectController StatusEffectController => _statusEffectController ?? (_statusEffectController = GetComponent<StatusEffectController>());

	public DizzinessManager DizzinessManager => _dizzinessManager ?? (_dizzinessManager = GetComponent<DizzinessManager>());

	public StatusEffectDebugUI StatsDebugUI
	{
		get
		{
			if (_statsDebugUI == null)
			{
				InitStatsValueDebug();
			}
			return _statsDebugUI;
		}
	}

	public void SetTargetIdxCamBeforeRevive(int value)
	{
		targetIdxCamBeforeRevive = value;
	}

	private void Awake()
	{
		if ((object)playerInput == null)
		{
			playerInput = GetComponent<PlayerInput>();
		}
		playerInput.neverAutoSwitchControlSchemes = true;
		if ((object)data == null)
		{
			data = GetComponent<PlayerData>();
		}
		if ((object)network == null)
		{
			network = GetComponent<PlayerNetwork>();
		}
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		for (int i = 0; i < allLights.Count; i++)
		{
			allLightIntensity.Add(allLights[i].intensity);
		}
		canGrenade = false;
		canSprint = true;
	}

	private void Start()
	{
		PlayerMultiplyStats playerMultiplyStatsData = PlayerMultiplyStatsData;
		playerMultiplyStatsData.OnPlayerStatsChangedEvents = (Action<PlayerStatsSO>)Delegate.Combine(playerMultiplyStatsData.OnPlayerStatsChangedEvents, new Action<PlayerStatsSO>(UpdateStatsValueDebug));
		data.SkillData.OnChangedAdditionalPerkSkillEvent += OnAdditionalPerkChangedAction;
	}

	private void FixedUpdate()
	{
		if (NetworkGameManager.Instance.isServer)
		{
			if (reviveTimer.isCompleted())
			{
				network.playerPhoton.reviveTimerSecond = 0;
			}
			else if (reviveTimer.isRunning)
			{
				network.playerPhoton.reviveTimerSecond = (byte)Mathf.RoundToInt(reviveTimer.interval);
			}
		}
		if (delaySpectator.isCompleted() && !GameManager.Instance.gameOver && network.GetHealth() <= 0f)
		{
			UIGameManager.Instance.spectateObject.SetActive(value: true);
			UIGameManager.Instance.uIInGameController.SetPlayerStatusUI(setActive: false);
			UIGameManager.Instance.uIInGameController.SetInventoryStatusUI(setActive: false);
		}
		if (network.isLocalPlayer)
		{
			if (isSprinting)
			{
				float value = -0.25f * PlayerMultiplyStatsData.GetMultiplyStaminaSprintConsumption();
				data.AddSubCurrentStamina(value);
				UIGameManager.Instance.barStamina.value = data.GetStamina() / data.GetMaxStamina();
				if (data.GetStamina() <= 0f)
				{
					network.StopSprint();
				}
			}
			if (Mathf.FloorToInt(MathFunc.Distance(angleInput, weaponPos.position) * 10f) >= 5 || GlobalOptionsManager.Instance.usingGamepad)
			{
				origin.rotation = Quaternion.Lerp(origin.rotation, Quaternion.LookRotation(angleInput - weaponPos.position, Vector3.up), 0.25f);
				origin.localEulerAngles = new Vector3(0f, origin.localEulerAngles.y, 0f);
			}
			if (itemCollisionCollider != null && !itemCollisionCollider.enabled)
			{
				itemCollision = null;
				itemCollisionCollider = null;
				functionItemCollision = "";
				ChatSystem.Instance.ItemCommand.SetActive(value: false);
			}
		}
		else
		{
			origin.DOLocalRotate(new Vector3(0f, network.GetAngleInputNetwork(), 0f), 30f).SetSpeedBased(isSpeedBased: true);
		}
		_ = origin.transform;
		CrosshairPositionGamepad(inputRotation);
		CrosshairPositionMouse(inputRotation);
	}

	private void Update()
	{
		if (animUpperChar.speed == 0f && isAttackMelee && !isLMBDown && network.isLocalPlayer)
		{
			SetAnimUpperSpeed(1f);
		}
		if (_pointerSkipDown)
		{
			_pointerDownTimer += Time.deltaTime;
			if (LobbyManager.Instance != null)
			{
				LobbyManager.Instance.sliderReady.value = _pointerDownTimer / _requiredHoldTime;
			}
			if (_pointerDownTimer >= _requiredHoldTime)
			{
				if (onSkip != null)
				{
					onSkip.Invoke();
				}
				ResetButtonHold();
			}
		}
		if ((!UIGameManager.Instance.UIMenuMap.isHidden || ((bool)MissionLobbyManager.Instance && MissionLobbyManager.Instance.MissionDetailMap.IsVisible && GlobalOptionsManager.Instance.usingGamepad)) && UIGameManager.Instance.mapImage != null)
		{
			UIGameManager.Instance.mapImage.position += -navigationMap * 9f;
			UIGameManager.Instance.mapImage.position = new Vector2(Mathf.RoundToInt(UIGameManager.Instance.mapImage.position.x), Mathf.RoundToInt(UIGameManager.Instance.mapImage.position.y));
		}
		if (DespawnTimer.isCompleted())
		{
			DisconnectedTimer.StopDuration();
			if (NetworkGameManager.Instance.isServer)
			{
				PlayerTempInventory playerTempInventory = new PlayerTempInventory();
				playerTempInventory.DeviceID = network.playerPhoton.PlayerDeviceID;
				playerTempInventory.ArrInventory = data.arrInventory.ToList();
				NetworkGameManager.Instance.ListPlayerTempInventory.Add(playerTempInventory);
				NetworkGameManager.Instance.arrPlayerIDDisconnected.Remove(network.playerPhoton.PlayerDeviceID);
				NetworkGameManager.Instance.arrPlayerDisconnected.Remove(this);
				PhotonMultiplayerManager.Instance.UpdateSessionDisconnectedPlayer();
				foreach (PlayerRef spawnedCharacter in NetworkGameManager.Instance.SpawnedCharacters)
				{
					if (PhotonMultiplayerManager.Instance._runner.GetPlayerObject(spawnedCharacter) == network.networkObj)
					{
						NetworkGameManager.Instance.SpawnedCharacters.Remove(spawnedCharacter);
						PhotonMultiplayerManager.Instance._runner.Despawn(network.networkObj);
						break;
					}
				}
			}
		}
		if (DisconnectedTimer.isRunning && LobbyManager.Instance != null)
		{
			int iDX = network.GetIDX();
			if (!PlayerBoard.Instance.boardPlayerList[iDX].activeSelf)
			{
				PlayerBoard.Instance.boardPlayerList[iDX].SetActive(value: true);
				PlayerBoard.Instance.playerNameList[iDX].text = network.GetPlayerName();
				UIGameManager.Instance.ChangeMiniAvatarReadyStatus(iDX, data.PlayerSkinData);
				UIGameManager.Instance.readyUIController?.GetUITabPlayer(iDX)?.SetDisconnectedUI();
			}
			UIGameManager.Instance.readyUIController?.GetUITabPlayer(iDX)?.SetDisconnectedUI(Mathf.FloorToInt(DisconnectedTimer.interval).ToString());
		}
	}

	private void OnDestroy()
	{
		PlayerMultiplyStats playerMultiplyStatsData = PlayerMultiplyStatsData;
		playerMultiplyStatsData.OnPlayerStatsChangedEvents = (Action<PlayerStatsSO>)Delegate.Remove(playerMultiplyStatsData.OnPlayerStatsChangedEvents, new Action<PlayerStatsSO>(UpdateStatsValueDebug));
		data.SkillData.OnChangedAdditionalPerkSkillEvent -= OnAdditionalPerkChangedAction;
		if ((bool)VoiceChatGlobalController.Instance)
		{
			foreach (VoiceSoundControl item in VoiceChatGlobalController.Instance.ListVoiceSound)
			{
				if (item != null && item.Player == this)
				{
					item.SetToOriginParent();
					break;
				}
			}
		}
		if ((bool)UIGameManager.Instance && network.isLocalPlayer)
		{
			UIGameManager.Instance.uIInGameController.RemoveEvent();
		}
		GameManager.Instance.arrItemInteractable.Remove(reviveArea.gameObject.GetComponent<ItemInteractable>());
		NetworkGameManager.Instance.arrPlayerController.Remove(this);
		UIGameManager.Instance.RefreshPlayerCountText();
		UIGameManager.Instance.HidePerkSkillUIInfo(this);
		if (SurvivorLobbyManager.Instance != null)
		{
			SurvivorLobbyManager.Instance.ShowBoard();
			LobbyManager.Instance.allReady = true;
			if (NetworkGameManager.Instance.arrPlayerController.Count > 0)
			{
				for (int i = 0; i < NetworkGameManager.Instance.arrPlayerController.Count; i++)
				{
					int iDX = NetworkGameManager.Instance.arrPlayerController[i].network.GetIDX();
					if ((bool)NetworkGameManager.Instance.ownPlayer.network.playerPhoton.Runner && !GameManager.Instance.gameManagerPhoton.arrPlayerReady[iDX])
					{
						LobbyManager.Instance.allReady = false;
					}
				}
			}
			else
			{
				LobbyManager.Instance.allReady = false;
			}
			UIGameManager.Instance.txtCountDown.gameObject.SetActive(value: false);
		}
		if (GameManager.Instance.quitGame || !(UIGameManager.Instance != null))
		{
			return;
		}
		for (int j = 0; j < 4; j++)
		{
			PlayerBoard.Instance.boardPlayerList[j].SetActive(value: false);
			UIGameManager.Instance.ArrPlayerInfo[j].TextPlayerName.text = "";
			if ((bool)LobbyManager.Instance)
			{
				UIGameManager.Instance.readyUIController?.GetUITabPlayer(j)?.SetReconnectedUI();
			}
		}
		for (int k = 0; k < NetworkGameManager.Instance.arrPlayerController.Count; k++)
		{
			PlayerNetwork playerNetwork = NetworkGameManager.Instance.arrPlayerController[k].network;
			int iDX2 = playerNetwork.GetIDX();
			UIGameManager.Instance.ArrPlayerInfo[iDX2].TextPlayerName.text = playerNetwork.GetPlayerName();
			if ((!playerNetwork.isLocalPlayer && LobbyManager.Instance == null) || (!playerNetwork.isLocalPlayer && LobbyManager.Instance != null && LobbyManager.Instance.testMode) || (LobbyManager.Instance != null && !LobbyManager.Instance.testMode))
			{
				PlayerBoard.Instance.boardPlayerList[iDX2].SetActive(value: true);
				PlayerBoard.Instance.ChangeAvatarPlayerBoard(iDX2, playerNetwork.playerController.data.PlayerSkinData);
				UIGameManager.Instance.SetPerkSkillUIInfo(playerNetwork.playerController);
				UIGameManager.Instance.ChangeMiniAvatarReadyStatus(iDX2, playerNetwork.playerController.data.PlayerSkinData);
			}
		}
	}

	public async UniTask Init()
	{
		CancellationToken token = this.GetCancellationTokenOnDestroy();
		SetActiveDeadIconChar(isActive: false);
		isPermadeath = false;
		enableMoveChar = true;
		network.charControllerPhoton.charControl.enabled = true;
		network.SetIdxPlayer();
		if (!NetworkGameManager.Instance.arrPlayerController.Exists((PlayerController p) => p == this))
		{
			NetworkGameManager.Instance.arrPlayerController.Add(this);
		}
		if (NetworkGameManager.Instance.arrPlayerController.Count >= 2 && PlayerBoard.Instance != null && (bool)PlayerBoard.Instance.ObjectWaiting && PlayerBoard.Instance.ObjectWaiting.activeSelf)
		{
			PlayerBoard.Instance.ObjectWaiting.SetActive(value: false);
		}
		if (network.isLocalPlayer)
		{
			network.SetPlayerName();
			network.playerPhoton.RPCSetPlayerDeviceID(SystemInfo.deviceUniqueIdentifier);
			if (GameModes.Instance.isInitDemo)
			{
				network.playerPhoton.RpcSetFriendPass(v: true);
			}
			NetworkGameManager.Instance.ownPlayer = this;
			InitPlayerInGame(init: true);
			if (!NetworkGameManager.Instance.isLoadGame)
			{
				UniTaskUtil.DelayedCall(this, 1f, InitPerkSelector).Forget();
			}
			else
			{
				DialogueSystem.Instance.IsFinishedIntroDialogue = true;
				if (NetworkGameManager.Instance.mode != NetworkGameManager.MultiplayerMode.Solo)
				{
					UIGameManager.Instance.sessionName?.transform.parent.gameObject.SetActive(value: true);
				}
			}
		}
		else
		{
			InitOtherPlayerInGame(init: true);
		}
		network.charControllerPhoton.Collider.gameObject.tag = "Player";
		if (ItemBoxNetwork.instance == null && NetworkGameManager.Instance.isServer && GameModes.Instance.isItemBoxGlobal)
		{
			NetworkGameManager.Instance.photonNetworking._runner.Spawn(GameManager.Instance.ItemBoxNetworkPrefab, Vector3.zero, Quaternion.identity, NetworkGameManager.Instance.photonNetworking._runner.LocalPlayer);
		}
		UIGameManager.Instance.RefreshPlayerCountText();
		ChatSystem.Instance.IsStaticChat = false;
		if ((bool)network.playerPhoton.disconnected)
		{
			Disconnected();
		}
		if (!LobbyManager.Instance)
		{
			return;
		}
		while (GameManagerPhoton.Instance == null)
		{
			await UniTask.Yield(token);
		}
		for (int num = 0; num < NetworkGameManager.Instance.arrPlayerController.Count; num++)
		{
			int iDX = NetworkGameManager.Instance.arrPlayerController[num].network.GetIDX();
			UIGameManager.Instance.readyUIController?.GetUITabPlayer(iDX)?.SetCheckBox(GameManagerPhoton.Instance.arrPlayerReady[iDX]);
			if (GameManagerPhoton.Instance.arrPlayerReady[iDX])
			{
				UIGameManager.Instance.readyUIController?.GetUITabPlayer(iDX)?.SetReadyUI();
				PlayerBoard.Instance.boardPlayerList[iDX].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f);
				if (NetworkGameManager.Instance.arrPlayerController[num].network.isLocalPlayer && LobbyManager.Instance != null)
				{
					LobbyManager.Instance.textReady.SetActive(value: true);
					LobbyManager.Instance.textUnready.SetActive(value: false);
				}
				continue;
			}
			UIGameManager.Instance.readyUIController?.GetUITabPlayer(iDX)?.SetUnreadyUI();
			LobbyManager.Instance.timerCountDown.StopDuration();
			PlayerBoard.Instance.boardPlayerList[iDX].transform.GetChild(0).GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f);
			if (NetworkGameManager.Instance.arrPlayerController[num].network.isLocalPlayer)
			{
				LobbyManager.Instance.textReady.SetActive(value: false);
				LobbyManager.Instance.textUnready.SetActive(value: true);
			}
		}
	}

	public void InitPerkSelector()
	{
		if (!network.isLocalPlayer)
		{
			return;
		}
		if ((bool)LobbyManager.Instance && (bool)GameManagerPhoton.Instance && !network.playerPhoton.IsDialogueOnboardingNPCShowed && (bool)DialogueSystem.Instance)
		{
			if (GlobalSaveData.instance.optionData.SkipIntroDialogue)
			{
				DialogueSystem.Instance.ShowUI(DialogueSystem.Instance.ShowPerksAction);
			}
			else
			{
				DialogueSystem.Instance.ShowUI(null);
			}
		}
		if (NetworkGameManager.Instance.mode != NetworkGameManager.MultiplayerMode.Solo)
		{
			UIGameManager.Instance.sessionName?.transform.parent.gameObject.SetActive(value: true);
		}
	}

	public void InitPlayerInGame(bool init = false)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			if ((bool)VoiceChatGlobalController.Instance && VoiceChatGlobalController.Instance.VoiceComms.LocalPlayerName != null)
			{
				network.playerPhoton.voiceChatName = VoiceChatGlobalController.Instance.VoiceComms.LocalPlayerName;
			}
			network.playerPhoton.IsInteractingPuzzle = false;
		}
		else
		{
			if ((bool)VoiceChatGlobalController.Instance && VoiceChatGlobalController.Instance.VoiceComms.LocalPlayerName != null)
			{
				network.playerPhoton.RpcSetVoiceChatName(VoiceChatGlobalController.Instance.VoiceComms.LocalPlayerName);
			}
			network.playerPhoton.RpcSetInteractingPuzzle(value: false);
		}
		ArrSoundStepTypeCollide.Clear();
		SetBtnSprint(newIsBtnSprintDown: false);
		SetActiveDeadIconChar(isActive: false);
		trail.InitTrails();
		isLowHealth = false;
		_aimDirection = new Vector3(0f, -1f, 0f);
		isPermadeath = false;
		reviveArea.enabled = true;
		SetAnimLowerSpeed(1f);
		SetAnimUpperSpeed(1f);
		bloodPool.gameObject.SetActive(value: false);
		roomColliders.Clear();
		fov.visibleTargets.Clear();
		fov.prevVisibleTargets.Clear();
		Cursor.visible = true;
		animUpperChar.transform.DOKill();
		weaponController.isMeleeCharging = false;
		weaponController.isHalfMeleeCharging = false;
		soundStepType = SoundStepType.CONCRETE;
		isAiming = false;
		isThrowing = false;
		if (isEntangled)
		{
			isEntangled = false;
			network.charControllerPhoton.charControl.enabled = true;
			network.ExecReleaseEnTangled();
		}
		fsmUpperBody.Play("Idle");
		fsmUpperBody.SetBool(IsMeleeAnim, value: false);
		fsmUpperBody.SetBool(IsShooting, value: false);
		fsmUpperBody.SetBool(IsThrowingAnim, value: false);
		if (LobbyManager.Instance != null)
		{
			if (GameModes.Instance.isShowingDisclaimer)
			{
				network.SetEnableControl(value: false);
				UIGameManager.Instance.loading.loadingScan.SetActive(value: false);
				UIGameManager.Instance.loading.loadingText.SetActive(value: false);
				UIGameManager.Instance.loading.pressAnyKey.SetActive(value: true);
			}
			else
			{
				UIGameManager.Instance.loading.loadingUI.SetActive(value: false);
				UIGameManager.Instance.fadeBlack.DOFade(0f, 0.75f).SetDelay(0.7f);
			}
			for (int i = 0; i < allLights.Count; i++)
			{
				allLights[i].DOIntensity(allLightIntensity[i], 0.5f);
			}
			UIGameManager.Instance.ArrPlayerInfo[network.GetIDX()].gameObject.SetActive(value: true);
			network.SetGodMode(isGodMode: true);
		}
		UIGameManager.Instance.ArrPlayerInfo[network.GetIDX()].transform.SetAsLastSibling();
		if (!InitScene.Instance.isBackToSplashScreen && !InitScene.Instance.isBackToMainMenu && SceneManager.GetActiveScene().name != "MainMenu")
		{
			GameManager.Instance.SpawnPhotonGameManager();
		}
		canDash = true;
		canSprint = true;
		playerInput.uiInputModule = UIGameManager.Instance.uiInputModule;
		shadow.color = new Color(shadow.color.r, shadow.color.g, shadow.color.b, 0.7f);
		initPos = true;
		CameraGame.Instance.RemoveAllMember();
		CameraGame.Instance.CinemachineTarget.AddMember(base.transform, 1f, 3f);
		CameraGame.Instance.cutOut.targetObject = base.transform;
		if (GameManager.Instance != null)
		{
			CameraGame.Instance.mainCam.GetComponent<AudioListener>().enabled = false;
		}
		audioListener.enabled = true;
		audioListener.transform.localPosition = new Vector3(audioListener.transform.localPosition.x, 0.325f, audioListener.transform.localPosition.z);
		MasterAudio.AudioListenerChanged(audioListener);
		playerInput.enabled = true;
		network.charControllerPhoton.charControl.enabled = true;
		enableMoveChar = true;
		reviveArea.enabled = false;
		animspeed = 1f;
		network.charControllerPhoton.Collider.gameObject.layer = LayerMask.NameToLayer("Character");
		network.charControllerPhoton.SetLayerMask(GameManager.Instance.layerMaskLive);
		playerCollider.SetActive(value: true);
		inventoryManager = GameManager.Instance.arrInventoryManager[0];
		inventoryManager.player = this;
		if (NetworkGameManager.Instance.isReconnecting)
		{
			if (!network.playerPhoton.IsDisconnectedOnLobby && !LobbyManager.Instance)
			{
				data.isSyncPosReconnect = true;
			}
			if ((bool)network.playerPhoton.IsDisconnectedOnLobby)
			{
				network.playerPhoton.RpcSetDisconnectedOnLobby(value: false);
			}
			data.isInitReconnect = true;
		}
		WaitForGameManagerPhoton().Forget();
		itemCollision = null;
		itemCollisionCollider = null;
		functionItemCollision = "";
		flashlight.SetActive(SceneManager.GetActiveScene().name != "Lobby");
		if (LobbyManager.Instance == null)
		{
			PlayerBoard.Instance.boardPlayerList[network.GetIDX()].SetActive(value: false);
		}
		if (!init)
		{
			for (int j = 0; j < NetworkGameManager.Instance.arrPlayerController.Count; j++)
			{
				if ((!NetworkGameManager.Instance.arrPlayerController[j].network.isLocalPlayer && LobbyManager.Instance == null) || (!NetworkGameManager.Instance.arrPlayerController[j].network.isLocalPlayer && LobbyManager.Instance != null && LobbyManager.Instance.testMode) || (LobbyManager.Instance != null && !LobbyManager.Instance.testMode))
				{
					PlayerBoard.Instance.boardPlayerList[NetworkGameManager.Instance.arrPlayerController[j].network.GetIDX()].SetActive(value: true);
					PlayerBoard.Instance.playerNameList[NetworkGameManager.Instance.arrPlayerController[j].network.GetIDX()].text = NetworkGameManager.Instance.arrPlayerController[j].network.GetPlayerName();
				}
				UIGameManager.Instance.ArrPlayerInfo[NetworkGameManager.Instance.arrPlayerController[j].network.GetIDX()].TextPlayerName.text = NetworkGameManager.Instance.arrPlayerController[j].network.GetPlayerName();
				if (NetworkGameManager.Instance.arrPlayerController[j].network.GetIDX() == 0 && NetworkGameManager.Instance.mode != NetworkGameManager.MultiplayerMode.Solo)
				{
					UIGameManager.Instance.ArrPlayerInfo[NetworkGameManager.Instance.arrPlayerController[j].network.GetIDX()].IconHostObject.SetActive(value: true);
				}
			}
		}
		if (GlobalOptionsManager.Instance.usingWeaponSelect)
		{
			weaponController.weaponSelect = 0;
		}
		if (GlobalOptionsManager.Instance.usingWeaponSelect)
		{
			ChangeWeaponPlayer(0);
		}
		if (NetworkGameManager.Instance.isServer)
		{
			network.SelectWeapon(0);
		}
		ChangePlayerAvatar(data.PlayerSkinData);
		ItemInteractable component = reviveArea.gameObject.GetComponent<ItemInteractable>();
		component.UniqueID = (short)(10000 + network.GetIDX());
		GameManager.Instance.arrItemInteractable.Add(component);
		GameManager.Instance.arrItemInteractable.Sort((ItemInteractable p1, ItemInteractable p2) => p1.UniqueID.CompareTo(p2.UniqueID));
		directionMap.gameObject.SetActive(value: true);
		UIGameManager.Instance.txtStaminaValuePlayer.text = Mathf.RoundToInt(data.GetStamina()) + "/" + data.GetStamina();
		UIGameManager.Instance.txtHpValuePlayer.text = Mathf.RoundToInt(network.GetHealth()) + "/" + data.GetMaxHealth();
		ArmorManager.UpdateCurrentArmor();
	}

	private async UniTask WaitForGameManagerPhoton()
	{
		CancellationToken token = this.GetCancellationTokenOnDestroy();
		while (GameManagerPhoton.Instance == null)
		{
			await UniTask.Yield(token);
		}
		data.Init();
	}

	public void ChangePlayerAvatar(PlayerSkinData playerSkinData)
	{
		byte iDX = network.GetIDX();
		if (network.isLocalPlayer)
		{
			UIGameManager.Instance.uIInGameController.ChangeCharacterAvatarUI(playerSkinData);
		}
		PlayerBoard.Instance.ChangeAvatarPlayerBoard(iDX, playerSkinData);
		UIGameManager.Instance.ChangeMiniAvatarReadyStatus(iDX, playerSkinData);
		if ((bool)SurvivorLobbyManager.Instance)
		{
			SurvivorLobbyManager.Instance.ChangeAvatar(iDX, playerSkinData);
		}
	}

	public void InitOtherPlayerInGame(bool init = false)
	{
		SetActiveDeadIconChar(isActive: false);
		ArrSoundStepTypeCollide.Clear();
		trail.InitTrails();
		canSprint = true;
		canDash = true;
		isLowHealth = false;
		isPermadeath = false;
		reviveArea.enabled = true;
		SetAnimLowerSpeed(1f);
		SetAnimUpperSpeed(1f);
		bloodPool.gameObject.SetActive(value: false);
		roomColliders.Clear();
		animUpperChar.transform.DOKill();
		weaponController.isMeleeCharging = false;
		weaponController.isHalfMeleeCharging = false;
		soundStepType = SoundStepType.CONCRETE;
		isAiming = false;
		isThrowing = false;
		fsmUpperBody.Play("Idle");
		fsmUpperBody.SetBool(IsMeleeAnim, value: false);
		fsmUpperBody.SetBool(IsShooting, value: false);
		fsmUpperBody.SetBool(IsThrowingAnim, value: false);
		for (int i = 0; i < allLights.Count; i++)
		{
			allLights[i].DOIntensity(allLightIntensity[i], 0.5f);
		}
		shadow.color = new Color(shadow.color.r, shadow.color.g, shadow.color.b, 0.7f);
		if (NetworkGameManager.Instance.isServer)
		{
			initPos = true;
		}
		inventoryManager = GameManager.Instance.GetInventoryPlayerNull(1);
		inventoryManager.player = this;
		enableMoveChar = true;
		network.charControllerPhoton.charControl.enabled = true;
		reviveArea.enabled = false;
		animspeed = 1f;
		network.charControllerPhoton.Collider.enabled = true;
		network.charControllerPhoton.SetLayerMask(GameManager.Instance.layerMaskLive);
		network.charControllerPhoton.Collider.gameObject.layer = LayerMask.NameToLayer("Character");
		playerCollider.SetActive(value: true);
		audioListener.enabled = false;
		CameraGame.Instance.RotateCamera(0, isInit: true);
		WaitForGameManagerPhoton().Forget();
		flashlight.SetActive(SceneManager.GetActiveScene().name != "Lobby");
		if (!init)
		{
			for (int j = 0; j < NetworkGameManager.Instance.arrPlayerController.Count; j++)
			{
				PlayerNetwork playerNetwork = NetworkGameManager.Instance.arrPlayerController[j].network;
				int iDX = playerNetwork.GetIDX();
				if ((!playerNetwork.isLocalPlayer && LobbyManager.Instance == null) || (!playerNetwork.isLocalPlayer && LobbyManager.Instance != null && LobbyManager.Instance.testMode) || (LobbyManager.Instance != null && !LobbyManager.Instance.testMode))
				{
					PlayerBoard.Instance.boardPlayerList[iDX].SetActive(value: true);
					PlayerBoard.Instance.playerNameList[iDX].text = playerNetwork.GetPlayerName();
				}
				UIGameManager.Instance.ArrPlayerInfo[iDX].gameObject.SetActive(value: true);
				UIGameManager.Instance.ArrPlayerInfo[iDX].TextPlayerName.text = playerNetwork.GetPlayerName();
				if (iDX == 0 && NetworkGameManager.Instance.mode != NetworkGameManager.MultiplayerMode.Solo)
				{
					UIGameManager.Instance.ArrPlayerInfo[iDX].IconHostObject.SetActive(value: true);
				}
			}
		}
		if (NetworkGameManager.Instance.isServer)
		{
			network.SelectWeapon(0);
		}
		int iDX2 = network.GetIDX();
		weaponController.SyncWeaponLocalVariable();
		PlayerBoard.Instance.ChangeAvatarPlayerBoard(iDX2, data.PlayerSkinData);
		UIGameManager.Instance.SetPerkSkillUIInfo(data.playerController);
		UIGameManager.Instance.ChangeMiniAvatarReadyStatus(iDX2, data.PlayerSkinData);
		if (LobbyManager.Instance != null)
		{
			UIGameManager.Instance.readyUIController?.GetUITabPlayer(iDX2)?.SetUnreadyUI();
		}
		ItemInteractable component = reviveArea.gameObject.GetComponent<ItemInteractable>();
		component.UniqueID = (short)(10000 + iDX2);
		GameManager.Instance.arrItemInteractable.Add(component);
		GameManager.Instance.arrItemInteractable.Sort((ItemInteractable p1, ItemInteractable p2) => p1.UniqueID.CompareTo(p2.UniqueID));
		iconCharMap.parent = base.transform;
		directionMap.gameObject.SetActive(value: false);
		iconCharMap.gameObject.SetActive(value: true);
		iconCharMap.DOScale(20f, 0f);
		iconCharMapAnimator.Play(data.PlayerSkinData.GetPlayerAvatarSkin());
		if (GameModes.Instance.modeGame == "PVP")
		{
			objectFOVCollider.SetActive(value: true);
		}
	}

	public NetworkInputData InputNetworkPlayer()
	{
		Vector3 vector = IsoDirection(direction.normalized);
		float num = 9f;
		if (vector != Vector3.zero)
		{
			num = Quaternion.LookRotation(vector, Vector3.up).eulerAngles.y;
			if (num < 0f)
			{
				num += 360f;
			}
			num = Mathf.RoundToInt(num / 45f);
		}
		Quaternion quaternion = Quaternion.LookRotation(angleInput - weaponPos.position, Vector3.up);
		float num2 = 100f;
		num2 = quaternion.eulerAngles.y;
		if (num2 < 0f)
		{
			num += 360f;
		}
		num2 = Mathf.RoundToInt(num2 / 45f);
		return new NetworkInputData
		{
			inputDataMove = (byte)(num * 10f + num2),
			inputDataClick = (short)(Convert.ToInt32(isLMBDown) + Convert.ToInt32(isRMBDown) * 10 + Convert.ToInt32(isBtnDashDown) * 100 + Convert.ToInt32(isBtnSprintDown) * 1000 + 10000)
		};
	}

	public void OnInteract(InputAction.CallbackContext value)
	{
		bool flag = false;
		bool flag2 = false;
		if (UIGameManager.Instance.uiPause.isHidden)
		{
			if (itemCollision != null)
			{
				if (OptionsManager.Instance.IsShowControlOnly)
				{
					if (value.performed && !UIGameManager.Instance.uiOptions.isHidden)
					{
						AudioManager.PlaySFX("ui_cancel");
						OptionsManager.Instance.IsShowControlOnly = false;
						OptionsManager.Instance.TabButtonObject.SetActive(value: true);
						UIGameManager.Instance.uiOptions.Hide();
						UIGameManager.Instance.BackToInGame(null);
					}
				}
				else if (itemCollision.GetComponent<ItemPickable>() != null && enableMoveChar && (!GlobalOptionsManager.Instance.usingGamepad || (GlobalOptionsManager.Instance.usingGamepad && network.GetEnableControl())))
				{
					if (value.performed)
					{
						bool flag3 = true;
						if ((bool)ArmoryLobbyManager.Instance && !ArmoryLobbyManager.Instance.UIMenu.isHidden)
						{
							flag3 = false;
						}
						ItemPickable component = itemCollision.GetComponent<ItemPickable>();
						if (component.itemType == "Note" && UIGameManager.Instance.uiInventory.isHidden)
						{
							if (UIGameManager.Instance.UIMenuNote.isHidden)
							{
								component.ShowNote();
								flag = true;
							}
						}
						else if (flag3 && ((!animUpperChar.GetCurrentAnimatorStateInfo(0).IsTag("Shoot") && component.itemType == "Weapon") || (component.itemType != "Weapon" && component.itemType != "Note")))
						{
							PickObject(component);
						}
					}
				}
				else if (network.GetEnableControl() && itemCollision.GetComponent<ItemInteractable>() != null)
				{
					ItemInteractable component2 = itemCollision.GetComponent<ItemInteractable>();
					if (value.performed && enableMoveChar && !component2.timerDelay.isRunning && !component2.isProgressing)
					{
						if (component2.isNoNeedItem(this))
						{
							if (!GameManager.Instance.isHordeMode && GlobalOptionsManager.Instance.enableVOItem)
							{
								if (component2.VOMaleInteract != "" && IsMale)
								{
									AudioManager.PlaySFX(component2.VOMaleInteract);
								}
								else if (component2.VOFemaleInteract != "")
								{
									AudioManager.PlaySFX(component2.VOFemaleInteract);
								}
							}
							if (component2.isNeedProgress)
							{
								_ = component2.functionInteract == "Barricade";
								if (0 == 0)
								{
									network.SetPlayerAFK(value: true);
									enableMoveChar = false;
									network.ExecStartProgressInteract((short)component2.UniqueID, network.GetIDX());
									if (angleRot == 0f)
									{
										angleRot = 45f;
									}
									else if (angleRot == 90f)
									{
										angleRot = 135f;
									}
									else if (angleRot == 180f)
									{
										angleRot = 135f;
									}
									else if (angleRot == 270f)
									{
										angleRot = 225f;
									}
									if (component2.IsOnStartInteractShowMonologue && component2.MonologueID != -1 && (!component2.IsInteractionMonologueMultiplayerOnly || NetworkGameManager.Instance.mode != NetworkGameManager.MultiplayerMode.Solo))
									{
										network.ShowBaloonChat(ChatType.MONOLOGUE, component2.MonologueID, -1, -1, -1, 10);
									}
								}
							}
							else if (component2.isShowUI)
							{
								flag2 = true;
								if (component2.listItemToActivate.Count == 0 && (bool)component2.UIMenu)
								{
									component2.UIMenu.Show();
									if (UIGameManager.Instance.uiObjective != null)
									{
										UIGameManager.Instance.uiObjective.SetActive(value: false);
									}
									if (component2.UIMenu.GetComponent(typeof(IPuzzle)) != null)
									{
										IPuzzle puzzle = component2.UIMenu.GetComponent(typeof(IPuzzle)) as IPuzzle;
										if (component2.Password != "")
										{
											puzzle.SetPassword(component2.Password);
										}
										puzzle.Show();
									}
									UIGameManager.Instance.mapUI.SetActive(value: false);
									UIGameManager.Instance.uiTabKill.InstantHide();
									UIGameManager.Instance.uiTabKill.gameObject.SetActive(value: false);
									if (component2.UIMenu.GetComponent<IPuzzle>() != null)
									{
										if (NetworkGameManager.Instance.isServer)
										{
											network.playerPhoton.IsInteractingPuzzle = true;
										}
										else
										{
											network.playerPhoton.RpcSetInteractingPuzzle(value: true);
										}
										component2.UIMenu.GetComponent<IPuzzle>().SetInteractableObject(component2);
										flag2 = true;
									}
									UIGameManager.Instance.UIMenuPuzzle = component2.UIMenu;
									ChatSystem.Instance.ItemCommand.SetActive(value: false);
									network.SetEnableControl(value: false);
									direction = Vector3.zero;
									if (!component2.isUIGameStillShowing)
									{
										UIGameManager.Instance.uiInGame.Hide();
									}
									if (component2.isLocked)
									{
										component2.lockMap.SetActive(value: true);
									}
								}
								else
								{
									network.ExecInteractObject((short)component2.UniqueID, component2.triggerOnReverse);
								}
							}
							else if (component2.ListDialogue.Count > 0)
							{
								DialogueSystem.Instance.ShowUI(component2.ListDialogue[component2.IdxDialogue], 0.2f);
								component2.IdxDialogue++;
								if (component2.IdxDialogue >= component2.ListDialogue.Count)
								{
									component2.IdxDialogue = 0;
								}
							}
							else if (component2.isShowControlOptions)
							{
								if (NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Solo)
								{
									GameManager.Instance.PauseGameTime();
								}
								network.SetPlayerAFK(value: true);
								UIGameManager.Instance.uiPause.Hide();
								UIGameManager.Instance.uiInventory.Hide();
								UIGameManager.Instance.uiInGame.Hide();
								UIGameManager.Instance.uiOptions.Show();
								OptionsManager.Instance.ShowControl();
								OptionsManager.Instance.btnDisplay.gameObject.SetActive(value: false);
								OptionsManager.Instance.btnGameplay.gameObject.SetActive(value: false);
								OptionsManager.Instance.btnAudio.gameObject.SetActive(value: false);
								OptionsManager.Instance.ShowControl();
								OptionsManager.Instance.IsShowControlOnly = true;
								OptionsManager.Instance.TabButtonObject.SetActive(value: false);
								if (UIGameManager.Instance.uiObjective != null)
								{
									UIGameManager.Instance.uiObjective.SetActive(value: false);
								}
								if (LobbyManager.Instance == null)
								{
									UIGameManager.Instance.mapUI.SetActive(value: false);
								}
								NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: false);
								inventoryManager.frameInventory.Play("Inventory" + (data.GetMaxInventory() - 2));
								if (enableMoveChar)
								{
									direction = Vector3.zero;
									fsmUpperBody.SetBool("isMoving", value: false);
									fsmLowerBody.SetBool("isMoving", value: false);
									animLowerChar.Play("LegIdle" + NetworkGameManager.Instance.ownPlayer.angleRot, 1);
								}
							}
							else
							{
								network.ExecInteractObject((short)component2.UniqueID, component2.triggerOnReverse);
							}
						}
						else
						{
							if (component2.functionInteract == "RepairCar")
							{
								network.ShowBaloonChat(ChatType.MONOLOGUE, 19, -1, -1, -1, 10);
							}
							if (!GameManager.Instance.isHordeMode)
							{
								if (component2.VOMaleNeedItem != "")
								{
									AudioManager.PlaySFX(component2.VOMaleNeedItem);
								}
								else
								{
									AudioManager.PlaySFX(component2.VOFemaleNeedItem);
								}
							}
						}
					}
				}
			}
			if (fsmUpperBody.GetBool("isReviving") && value.canceled)
			{
				StopInteractProgress();
			}
			if (value.performed && !GlobalOptionsManager.Instance.usingGamepad)
			{
				if (!UIGameManager.Instance.UIMenuNote.isHidden && UIGameManager.Instance.UIMenuNote.gameObject.activeSelf && !flag)
				{
					CloseNote();
				}
				else if (!UIGameManager.Instance.UIMenuPuzzle.isHidden && !flag2)
				{
					bool flag4 = true;
					if (MissionLobbyManager.Instance != null && MissionLobbyManager.Instance.missionBrief.enabled)
					{
						AudioManager.PlaySFX("ui_cancel");
						MissionLobbyManager.Instance.missionBrief.enabled = false;
						flag4 = false;
					}
					else if ((bool)MissionLobbyManager.Instance && MissionLobbyManager.Instance.MissionDetailMap.IsVisible)
					{
						flag4 = false;
						MissionLobbyManager.Instance.MissionDetailMap.CloseUI();
						AudioManager.PlaySFX("ui_cancel");
					}
					if (flag4)
					{
						ClosePuzzle();
					}
				}
				else if (!UIGameManager.Instance.UIMenuMap.isHidden)
				{
					CloseMap();
				}
			}
			else if (!UIGameManager.Instance.UIMenuPuzzle.isHidden && !flag2)
			{
				if (value.performed && UIGameManager.Instance.UIMenuPuzzle.GetComponent(typeof(IPuzzle)) is IPuzzle puzzle2)
				{
					puzzle2.Action1Press();
					isPressingActionPuzzle = true;
				}
				if (value.canceled)
				{
					if (isPressingActionPuzzle && UIGameManager.Instance.UIMenuPuzzle.GetComponent(typeof(IPuzzle)) is IPuzzle puzzle3)
					{
						puzzle3.Action1Release();
					}
					isPressingActionPuzzle = false;
				}
			}
		}
		if (value.performed)
		{
			InputReleasingEntangled();
			InputGetUp();
		}
	}

	public void OnAiming(InputAction.CallbackContext value)
	{
		if (value.performed)
		{
			if (!UIGameManager.Instance.UIMenuNote.isHidden && UIGameManager.Instance.UIMenuNote.gameObject.activeSelf)
			{
				CloseNote();
			}
			else if (!UIGameManager.Instance.UIMenuPuzzle.isHidden && UIGameManager.Instance.uiInventory.isHidden)
			{
				bool flag = false;
				if (MissionLobbyManager.Instance != null && MissionLobbyManager.Instance.missionBrief.enabled)
				{
					flag = true;
				}
				if (!flag)
				{
					ClosePuzzle();
				}
				else
				{
					MissionLobbyManager.Instance.missionBrief.enabled = false;
				}
			}
			else if (!GlobalOptionsManager.Instance.usingWeaponSelect)
			{
				if (UIGameManager.Instance.uiInventory.isHidden && UIGameManager.Instance.uiPause.isHidden && UIGameManager.Instance.UIMenuPuzzle.isHidden && weaponController.idWeaponRange > 0)
				{
					Cursor.visible = false;
				}
				isRMBDown = true;
				if (fsmUpperBody.GetBool(IsThrowingAnim))
				{
					network.ExecCancelThrow();
					UIGameManager.Instance.cursorGrenade.gameObject.SetActive(value: false);
				}
			}
		}
		if (value.canceled && !GlobalOptionsManager.Instance.usingWeaponSelect)
		{
			Cursor.visible = true;
			isRMBDown = false;
		}
	}

	public void OnReload(InputAction.CallbackContext value)
	{
		if (value.performed && network.GetEnableControl() && enableMoveChar && !isAttackMelee && !fsmUpperBody.GetBool(IsMeleeAnim))
		{
			if (!isShooting && !animUpperChar.GetCurrentAnimatorStateInfo(0).IsTag("PumpAction"))
			{
				weaponController.TriggerReload();
			}
			weaponController.reloadStateTimer.StartDuration(0.7f);
		}
	}

	public void OnInputShowInventory(InputAction.CallbackContext value)
	{
		if (value.performed)
		{
			if (fsmUpperBody.GetBool("isReviving"))
			{
				StopInteractProgress();
			}
			if (!UIGameManager.Instance.UIMenuNote.isHidden && UIGameManager.Instance.UIMenuNote.gameObject.activeSelf)
			{
				CloseNote();
			}
			else if (!UIGameManager.Instance.UIMenuPuzzle.isHidden)
			{
				ClosePuzzle();
			}
			else if (!UIGameManager.Instance.UIMenuMap.isHidden)
			{
				CloseMap();
			}
			else if (!DialogueSystem.Instance || DialogueSystem.Instance.GetUIView.isHidden)
			{
				UIGameManager.Instance.ShowHideInventory();
			}
		}
	}

	public void SetScrambleModifierInput(ControlScrambler.ScrambleType scrambleInputType)
	{
		_inputScramble = ControlScrambler.GenerateScramble(scrambleInputType);
		RefreshInputDirection();
	}

	public void SetScrambleModifierInput(IScramble iScramble)
	{
		_inputScramble = iScramble;
		RefreshInputDirection();
	}

	private void RefreshInputDirection()
	{
		if (!_rawInput.Equals(Vector2.zero))
		{
			Vector2 vector = _inputScramble?.Apply(_rawInput) ?? _rawInput;
			direction = new Vector3(vector.x, 0f, vector.y);
		}
	}

	public void OnInputMoving(InputAction.CallbackContext value)
	{
		_rawInput = value.ReadValue<Vector2>();
		Vector2 vector = _inputScramble?.Apply(_rawInput) ?? _rawInput;
		if (isDashing)
		{
			direction = (value.canceled ? new Vector3(0f, 0f, 0f) : new Vector3(vector.x, 0f, vector.y));
		}
		else if (isEntangled)
		{
			direction = (value.canceled ? new Vector3(0f, 0f, 0f) : new Vector3(vector.x, 0f, vector.y));
		}
		if (UIGameManager.Instance.UIMenuMap.isHidden && enableMoveChar)
		{
			if ((bool)MissionLobbyManager.Instance && MissionLobbyManager.Instance.MissionDetailMap.IsVisible && GlobalOptionsManager.Instance.usingGamepad)
			{
				navigationMap = (value.canceled ? new Vector2(0f, 0f) : _rawInput);
			}
			else
			{
				direction = (value.canceled ? new Vector3(0f, 0f, 0f) : new Vector3(vector.x, 0f, vector.y));
				if (network.GetEnableControl())
				{
					DOTween.Kill("SprintTolerance");
					DOTween.Kill("FocusCam");
					DOTween.To(() => CameraGame.Instance.camTransposer.m_CameraDistance, (float x) =>
					{
						CameraGame.Instance.camTransposer.m_CameraDistance = x;
					}, CameraGame.Instance.maxZoomOutCam, 2f).SetId("FocusCam").SetEase(Ease.Linear);
					if (isSprintDown && canSprint && direction != Vector3.zero && !isAttacking && !isAiming && !isRMBDown && value.started)
					{
						network.StartSprint();
					}
					if (isAiming && isRangeActive && !isAttackMelee)
					{
						if (value.canceled)
						{
							DOTween.Kill("AccuracySubtract");
							DOTween.To(() => weaponController.accuracy, (float x) =>
							{
								weaponController.accuracy = x;
							}, weaponController.minRangeAccuracy, weaponController.timeAccuracy).SetId("AccuracySubtract").SetEase(Ease.Linear);
						}
						else if (value.performed)
						{
							DOTween.Kill("AccuracySubtract");
							weaponController.accuracy = weaponController.maxRangeAccuracy;
						}
					}
				}
			}
		}
		else if (!UIGameManager.Instance.UIMenuMap.isHidden && !isDashing)
		{
			navigationMap = (value.canceled ? new Vector2(0f, 0f) : _rawInput);
		}
		if (value.canceled || (GlobalOptionsManager.Instance.usingGamepad && direction == Vector3.zero))
		{
			DOTween.Kill("FocusCam");
			DOTween.To(() => CameraGame.Instance.camTransposer.m_CameraDistance, (float x) =>
			{
				CameraGame.Instance.camTransposer.m_CameraDistance = x;
			}, CameraGame.Instance.minZoomOutCam, 1.5f).SetId("FocusCam").SetEase(Ease.Linear);
			_rawInput = (direction = new Vector3(0f, 0f, 0f));
			if (isSprinting && !isAttacking)
			{
				network.StopSprint();
				if (GlobalSaveData.instance.optionData.sprintModeToggle)
				{
					_sprintTolerance = 0f;
					DOTween.To(() => _sprintTolerance, (float x) =>
					{
						_sprintTolerance = x;
					}, 1f, 0.05f).SetId("SprintTolerance").OnComplete(() =>
					{
						SetBtnSprint(newIsBtnSprintDown: false);
					});
				}
			}
		}
		NavigateFunction(value);
	}

	public void OnInputRotating(InputAction.CallbackContext value)
	{
		if (network.GetEnableControl())
		{
			inputRotation = value.ReadValue<Vector2>();
			if (direction == Vector3.zero)
			{
				AnglePlayerAim(inputRotation);
			}
			if (ChatWheel.Instance != null)
			{
				ChatWheel.Instance.MoveCursor(inputRotation);
			}
		}
	}

	public void OnInputAttack(InputAction.CallbackContext value)
	{
		if (fsmUpperBody.GetBool(IsThrowingAnim))
		{
			network.ExecCancelThrow();
			UIGameManager.Instance.cursorGrenade.gameObject.SetActive(value: false);
		}
		if (value.started)
		{
			isLMBDown = true;
			if (!UIGameManager.Instance.UIMenuPuzzle.isHidden)
			{
				if (UIGameManager.Instance.UIMenuPuzzle.GetComponent(typeof(IPuzzle)) is IPuzzle puzzle)
				{
					puzzle.Action1Press();
				}
			}
			else if (UIGameManager.Instance.ArrPlayerInfo[network.GetIDX()].HealBarObject.activeSelf)
			{
				UIPlayerInfo playerInfo = UIGameManager.Instance.ArrPlayerInfo[network.GetIDX()];
				int idxTouchedBar = -1;
				for (int i = 0; i < playerInfo.listTargetStitch.Count; i++)
				{
					if (playerInfo.listTargetStitch[i].gameObject.activeSelf && playerInfo.listTargetStitch[i].transform.localScale.x == 1f && Mathf.Abs(playerInfo.PointerStitch.anchoredPosition.x - playerInfo.listTargetStitch[i].anchoredPosition.x) <= playerInfo.listTargetStitch[i].sizeDelta.x)
					{
						idxTouchedBar = i;
					}
				}
				if (idxTouchedBar >= 0)
				{
					playerInfo.TextHealingValue.text = "+" + (network.playerPhoton.healingValue + 5);
					playerInfo.listTargetStitch[idxTouchedBar].DOScale(0f, 0.2f).OnComplete(() =>
					{
						playerInfo.listTargetStitch[idxTouchedBar].gameObject.SetActive(value: false);
					});
					network.playerPhoton.RpcSetHealingValue((byte)(network.playerPhoton.healingValue + 5), (byte)idxTouchedBar);
					playerInfo.RedBarBG.gameObject.SetActive(value: true);
					playerInfo.RedBarBG.DOKill();
					playerInfo.RedBarBG.color = new Color(0.13f, 1f, 0.25f, 1f);
					playerInfo.RedBarBG.DOFade(0f, 0.7f).OnComplete(() =>
					{
						playerInfo.RedBarBG.gameObject.SetActive(value: false);
					});
					AudioManager.PlaySFX("ui-heal-good");
					playerInfo.TextHealingReviveValue.gameObject.SetActive(value: true);
					playerInfo.TextHealingReviveValue.text = "+5";
					playerInfo.TextHealingReviveValue.DOKill();
					playerInfo.TextHealingReviveValue.rectTransform.DOKill();
					playerInfo.TextHealingReviveValue.rectTransform.localPosition = Vector2.zero;
					playerInfo.TextHealingReviveValue.DOFade(0.6f, 0f);
					playerInfo.TextHealingReviveValue.rectTransform.DOLocalMoveY(18f, 0.6f);
					playerInfo.TextHealingReviveValue.DOFade(0f, 0.3f).SetDelay(1f).OnComplete(() =>
					{
						playerInfo.TextHealingReviveValue.gameObject.SetActive(value: false);
					});
				}
				else if (network.playerPhoton.healingValue >= 11)
				{
					playerInfo.TextHealingValue.text = "+" + (network.playerPhoton.healingValue - 10);
					playerInfo.TextHealingValue.transform.DOShakePosition(0.2f, 2f, 30, 90f, snapping: true, fadeOut: false);
					network.playerPhoton.RpcSetHealingValue((byte)(network.playerPhoton.healingValue - 10), 100);
					playerInfo.RedBarBG.gameObject.SetActive(value: true);
					playerInfo.RedBarBG.DOKill();
					playerInfo.RedBarBG.color = new Color(0.56f, 0.13f, 0.07f, 1f);
					playerInfo.RedBarBG.DOFade(0f, 0.7f).OnComplete(() =>
					{
						playerInfo.RedBarBG.gameObject.SetActive(value: false);
					});
					playerInfo.TextHealingReviveValue.gameObject.SetActive(value: true);
					playerInfo.TextHealingReviveValue.text = "-10";
					playerInfo.TextHealingReviveValue.DOKill();
					playerInfo.TextHealingReviveValue.rectTransform.DOKill();
					playerInfo.TextHealingReviveValue.rectTransform.localPosition = Vector2.zero;
					playerInfo.TextHealingReviveValue.DOFade(0.6f, 0f);
					playerInfo.TextHealingReviveValue.rectTransform.DOLocalMoveY(18f, 0.6f);
					playerInfo.TextHealingReviveValue.DOFade(0f, 0.3f).SetDelay(1f).OnComplete(() =>
					{
						playerInfo.TextHealingReviveValue.gameObject.SetActive(value: false);
					});
					AudioManager.PlaySFX("ui-heal-bad");
				}
			}
		}
		if (value.canceled)
		{
			isLMBDown = false;
			network.ExecReleaseAttack();
			if (!UIGameManager.Instance.UIMenuPuzzle.isHidden && UIGameManager.Instance.UIMenuPuzzle.GetComponent(typeof(IPuzzle)) is IPuzzle puzzle2)
			{
				puzzle2.Action1Release();
			}
		}
	}

	public void OnInputThrowObject(InputAction.CallbackContext value)
	{
		InventoryObject inventoryObject = data.FindInventory(data.idThrowable);
		if (inventoryObject == null || NetworkGameManager.Instance.ownPlayer.DelayInputTimer.isRunning || !network.GetEnableControl() || !(network.GetHealth() > 0f) || fsmUpperBody.GetBool(IsMeleeAnim) || inventoryObject.Amount <= 0 || animUpperChar.GetCurrentAnimatorStateInfo(0).IsTag("PumpAction") || animUpperChar.GetCurrentAnimatorStateInfo(0).IsTag("Shoot") || animUpperChar.GetCurrentAnimatorStateInfo(0).IsTag("Reload"))
		{
			return;
		}
		if (value.started && canGrenade)
		{
			ThrowableWeapon throwableWeapon = DataManager.Instance.Get<WeaponLibraryScriptableObject>()?.GetData(inventoryObject.ID) as ThrowableWeapon;
			bool flag = !throwableWeapon || throwableWeapon.ShowCursor;
			network.ExecThrowPose();
			if (!UIGameManager.Instance.isUIInvisible & flag)
			{
				UIGameManager.Instance.cursorGrenade.gameObject.SetActive(value: true);
			}
			UIGameManager.Instance.cursorGrenade.transform.position = angleGround;
		}
		else if (value.canceled && fsmUpperBody.GetBool(IsThrowingAnim))
		{
			Vector3 targetPosition = MathFunc.GetTargetPosition(new Vector3(weaponPos.position.x, UIGameManager.Instance.cursorGrenade.transform.position.y, weaponPos.position.z), UIGameManager.Instance.cursorGrenade.transform.position, 0.1f);
			network.ExecThrowGrenade(targetPosition);
		}
	}

	public void OnInputDash(InputAction.CallbackContext value)
	{
		if (network.GetEnableControl() && value.started && canDash && data.GetStamina() > 0f)
		{
			isBtnDashDown = true;
			directionDash = network.GetAngledirection();
		}
		if (value.canceled)
		{
			isBtnDashDown = false;
		}
	}

	public void OnInputSprint(InputAction.CallbackContext value)
	{
		if (!value.started && !value.canceled)
		{
			return;
		}
		if (GlobalSaveData.instance.optionData.sprintModeToggle)
		{
			if (value.started)
			{
				if (data.GetStamina() > 0f)
				{
					SetBtnSprint(!isBtnSprintDown);
				}
				else
				{
					SetBtnSprint(newIsBtnSprintDown: false);
				}
			}
		}
		else if (value.started)
		{
			SetBtnSprint(newIsBtnSprintDown: true);
		}
		else if (value.canceled)
		{
			SetBtnSprint(newIsBtnSprintDown: false);
		}
	}

	public void OnInputHeal(InputAction.CallbackContext value)
	{
		if (UIGameManager.Instance.UIMenuMap.isHidden && value.performed && network.GetEnableControl() && !NetworkGameManager.Instance.ownPlayer.DelayInputTimer.isRunning && enableMoveChar && data.idHealing > 0)
		{
			InventoryObject inventoryObject = data.FindInventory(data.idHealing);
			if (inventoryObject != null)
			{
				inventoryManager.FunctionItemUse(inventoryObject.IdxInventory, isHealthCheck: true);
			}
		}
	}

	public void OnInputChatWheel(InputAction.CallbackContext value)
	{
		if (value.started && network.GetHealth() > 0f && network.GetEnableControl())
		{
			ChatWheel.Instance.ShowChatWheel();
		}
		if (value.canceled && !UIGameManager.Instance.uiChatWheel.isHidden)
		{
			ChatWheel.Instance.HideChatWheel();
		}
	}

	public void OnInputRotateLeftCam(InputAction.CallbackContext value)
	{
		if (network.GetEnableControl() && value.started && network.GetHealth() > 0f)
		{
			CameraGame.Instance.RotateCamera(CameraGame.Instance.CamRotationPerClick);
		}
	}

	public void OnInputRotateRightCam(InputAction.CallbackContext value)
	{
		if (network.GetEnableControl() && value.started && network.GetHealth() > 0f)
		{
			CameraGame.Instance.RotateCamera(-CameraGame.Instance.CamRotationPerClick);
		}
	}

	public void OnInputZoomInCam(InputAction.CallbackContext value)
	{
		if (value.started && (!UIGameManager.Instance.UIMenuMap.isHidden || ((bool)MissionLobbyManager.Instance && MissionLobbyManager.Instance.MissionDetailMap.IsVisible)) && UIGameManager.Instance.mapImage.localScale.x - 0.2f < 2f)
		{
			UIGameManager.Instance.mapImage.DOScale(UIGameManager.Instance.mapImage.localScale.x + 0.2f, 0.2f);
		}
	}

	public void OnInputZoomOutCam(InputAction.CallbackContext value)
	{
		if (value.started && (!UIGameManager.Instance.UIMenuMap.isHidden || ((bool)MissionLobbyManager.Instance && MissionLobbyManager.Instance.MissionDetailMap.IsVisible)) && UIGameManager.Instance.mapImage.localScale.x - 0.2f > 0.8f)
		{
			UIGameManager.Instance.mapImage.DOScale(UIGameManager.Instance.mapImage.localScale.x - 0.2f, 0.2f);
		}
	}

	public void OnInputTiltUp(InputAction.CallbackContext value)
	{
		network.GetEnableControl();
	}

	public void OnInputTiltDown(InputAction.CallbackContext value)
	{
		network.GetEnableControl();
	}

	public void OnInputNavigate(InputAction.CallbackContext value)
	{
		NavigateFunction(value);
	}

	public void NavigateFunction(InputAction.CallbackContext value)
	{
		if (value.performed)
		{
			if (!UIGameManager.Instance.UIMenuPuzzle.isHidden && prevNav == Vector2.zero)
			{
				if (UIGameManager.Instance.UIMenuPuzzle.GetComponent(typeof(IPuzzle)) is IPuzzle puzzle)
				{
					puzzle.Navigate(value.ReadValue<Vector2>());
				}
				prevNav = value.ReadValue<Vector2>();
			}
			if (!UIGameManager.Instance.UIMenuNote.isHidden && UIGameManager.Instance.UIMenuNote.gameObject.activeSelf && prevNav == Vector2.zero)
			{
				UIGameManager.Instance.ChangeNotePage(value.ReadValue<Vector2>());
				prevNav = value.ReadValue<Vector2>();
			}
			if (!enableMoveChar && network.GetHealth() <= 0f && network.GetLife() <= 0 && (value.ReadValue<Vector2>().x < 0f || value.ReadValue<Vector2>().x > 0f) && !delaySpectator.isRunning && prevNav == Vector2.zero)
			{
				int num = 0;
				int num2 = 0;
				CameraGame.Instance.RemoveAllMember();
				for (int i = 0; i < NetworkGameManager.Instance.arrPlayerController.Count; i++)
				{
					if (NetworkGameManager.Instance.arrPlayerController[i].network.GetIDX() == network.GetIdxTargetCam())
					{
						num = i;
						NetworkGameManager.Instance.arrPlayerController[i].audioListener.enabled = false;
						NetworkGameManager.Instance.arrPlayerController[i].fov.enabled = false;
					}
				}
				bool flag = false;
				num2 = num;
				if (value.ReadValue<Vector2>().x < 0f)
				{
					num2--;
					if (num2 < 0)
					{
						num2 = NetworkGameManager.Instance.arrPlayerController.Count - 1;
					}
					flag = true;
				}
				else if (value.ReadValue<Vector2>().x > 0f)
				{
					num2++;
					if (num2 >= NetworkGameManager.Instance.arrPlayerController.Count)
					{
						num2 = 0;
					}
					flag = true;
				}
				if (flag)
				{
					GameManager.Instance.ChangeSpectator(NetworkGameManager.Instance.arrPlayerController[num2].network.GetIDX(), NetworkGameManager.Instance.arrPlayerController[num].network.GetIDX());
					if (NetworkGameManager.Instance.arrPlayerController[num2].network.isLocalPlayer)
					{
						CameraGame.Instance.CinemachineTarget.AddMember(CameraGame.Instance.targetCursor, 0.5f, 2f);
						if (!isPermadeath)
						{
							AudioManager.PlaySFX("ui-heartbeat");
							if (NetworkGameManager.Instance.arrPlayerController[num2].IsMale)
							{
								AudioManager.PlaySFXTransform("male_dyingBreath", base.transform, network.isLocalPlayer);
							}
							else
							{
								AudioManager.PlaySFXTransform("female_dyingBreath", base.transform, network.isLocalPlayer);
							}
							AudioManager.ChangeLowPass(2000f);
							UIGameManager.Instance.flashRed2.DOFade(0f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InQuad);
						}
						UIGameManager.Instance.vfxCritical.color = new Color(1f, 1f, 1f, 0.15f);
						UIGameManager.Instance.flashRed2.color = new Color(1f, 1f, 1f, 0.1f);
						CameraGame.Instance.colorA.saturation.value = -45f;
					}
					else
					{
						AudioManager.StopSFX("ui-heartbeat");
						if (NetworkGameManager.Instance.arrPlayerController[num2].IsMale)
						{
							AudioManager.StopSFX("male_dyingBreath");
						}
						else
						{
							AudioManager.StopSFX("female_dyingBreath");
						}
						AudioManager.ChangeLowPass(22000f);
						UIGameManager.Instance.flashRed2.DOKill();
						CameraGame.Instance.colorA.saturation.value = 0f;
						UIGameManager.Instance.vfxCritical.color = new Color(1f, 1f, 1f, 0f);
					}
					NetworkGameManager.Instance.ownPlayer.network.SetTargetIdxCamTarget(NetworkGameManager.Instance.arrPlayerController[num2].network.GetIDX());
				}
				prevNav = value.ReadValue<Vector2>();
			}
		}
		if (value.ReadValue<Vector2>() == Vector2.zero)
		{
			prevNav = Vector2.zero;
		}
	}

	public void OnChangePrimaryWeapon(InputAction.CallbackContext value)
	{
		if (value.performed && GlobalOptionsManager.Instance.usingWeaponSelect)
		{
			ChangeWeaponPlayer(0);
		}
	}

	public void OnChangeSecondaryWeapon(InputAction.CallbackContext value)
	{
		if (value.performed)
		{
			_ = GlobalOptionsManager.Instance.usingWeaponSelect;
		}
	}

	public void OnMenuClick(InputAction.CallbackContext value)
	{
		if (!value.performed || !UIGameManager.Instance.uiInventory.isHidden || !DialogueSystem.Instance.GetUIView.isHidden || !UIGameManager.Instance.uiConfirmation.isHidden || UIGameManager.Instance.uiOptions.isVisible || !UIGameManager.Instance.UIMenuMap.isHidden || !UIGameManager.Instance.UIMenuPuzzle.isHidden || (!UIGameManager.Instance.UIMenuNote.isHidden && UIGameManager.Instance.UIMenuNote.gameObject.activeSelf))
		{
			return;
		}
		if (UIGameManager.Instance.uiPause.isHidden && GlobalOptionsManager.Instance.usingGamepad && !UIGameManager.Instance.loading.loadingUI.activeSelf)
		{
			if (NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Solo && LobbyManager.Instance == null && !UIGameManager.Instance.loading.loadingUI.activeSelf)
			{
				GameManager.Instance.PauseGameTime();
			}
			network.SetPlayerAFK(value: true);
			AudioManager.PlaySFX("ui_confirm");
			UIGameManager.Instance.uiPause.Show();
			UIGameManager.Instance.uiInventory.Hide();
			if (UIGameManager.Instance.isUIInvisible)
			{
				UIGameManager.Instance.uiInGame.Show();
			}
			UIGameManager.Instance.uiOptions.Hide();
			if (UIGameManager.Instance.uiObjective != null)
			{
				UIGameManager.Instance.uiObjective.SetActive(value: false);
			}
			UIGameManager.Instance.mapUI.SetActive(value: false);
			NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: false);
			inventoryManager.frameInventory.Play("Inventory" + (data.GetMaxInventory() - 2));
			if (enableMoveChar)
			{
				direction = Vector3.zero;
				fsmUpperBody.SetBool("isMoving", value: false);
				fsmLowerBody.SetBool("isMoving", value: false);
				animLowerChar.Play("LegIdle" + NetworkGameManager.Instance.ownPlayer.angleRot, 1);
			}
		}
		else if (!UIGameManager.Instance.uiPause.isHidden && GlobalOptionsManager.Instance.usingGamepad)
		{
			if (NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Solo)
			{
				GameManager.Instance.ResumeGameTime();
			}
			EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
			network.SetPlayerAFK(value: false);
			AudioManager.PlaySFX("ui_cancel");
			if (UIGameManager.Instance.isUIInvisible)
			{
				UIGameManager.Instance.uiInGame.Show();
			}
			UIGameManager.Instance.uiPause.Hide();
			NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: true);
			if (UIGameManager.Instance.uiObjective != null && UIGameManager.Instance.uiObjective != null)
			{
				UIGameManager.Instance.uiObjective.SetActive(value: true);
			}
			if (LobbyManager.Instance == null)
			{
				UIGameManager.Instance.mapUI.SetActive(value: true);
			}
		}
	}

	public void OnTabClick(InputAction.CallbackContext value)
	{
	}

	public void OnShowMap(InputAction.CallbackContext value)
	{
		if (!value.performed || !(LobbyManager.Instance == null) || !UIGameManager.Instance.uiPause.isHidden || !UIGameManager.Instance.UIMenuNote.isHidden || !UIGameManager.Instance.UIMenuPuzzle.isHidden || !UIGameManager.Instance.mapUI.transform.parent.gameObject.activeSelf)
		{
			return;
		}
		if (UIGameManager.Instance.UIMenuMap.isHidden)
		{
			if (CameraMiniMap.Instance != null)
			{
				foreach (ItemPickable item in GameManager.Instance.arrItemPickable)
				{
					if (item.itemMap != null)
					{
						item.itemMap.transform.DOLocalRotate(new Vector3(90f, 0f, 0f), 0f);
					}
				}
			}
			if (!UIGameManager.Instance.uiInventory.isHidden)
			{
				UIGameManager.Instance.HideInventory();
			}
			network.SetPlayerAFK(value: true);
			AudioManager.PlaySFX("ui_confirm");
			UIGameManager.Instance.uiInGame.Hide();
			UIGameManager.Instance.mapUI.SetActive(value: false);
			CameraMiniMap.Instance.gameObject.SetActive(value: false);
			CameraGame.Instance.cameraMap.gameObject.SetActive(value: true);
			NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: false);
			if (enableMoveChar)
			{
				direction = Vector3.zero;
				fsmUpperBody.SetBool("isMoving", value: false);
				fsmLowerBody.SetBool("isMoving", value: false);
				animLowerChar.Play("LegIdle" + NetworkGameManager.Instance.ownPlayer.angleRot, 1);
			}
			UIGameManager.Instance.UIMenuMap.Show();
			CameraGame.Instance.RotateRoomText(isSetDefault: true);
			UIGameManager.Instance.mapImage.DOScale(1.4f, 0.2f);
			if (UIGameManager.Instance.uiObjective != null)
			{
				UIGameManager.Instance.uiObjective.SetActive(value: false);
			}
			foreach (PlayerController item2 in NetworkGameManager.Instance.arrPlayerController)
			{
				if (item2.network.isLocalPlayer)
				{
					item2.iconCharMap.DOScale(20f, 0f);
					item2.iconCharMapAnimator.Play(data.PlayerSkinData.GetPlayerAvatarSkin());
					item2.iconCharMap.parent = item2.transform;
					item2.cursorLocalPlayer.SetActive(value: true);
				}
				item2.iconCharMap.DORotate(new Vector3(90f, 0f, 0f), 0f);
			}
			navigationMap = new Vector2(0f, 0f);
			UIGameManager.Instance.mapImage.localPosition = new Vector2(Mathf.RoundToInt(iconCharMap.transform.position.x * (0f - UIGameManager.Instance.mapImageXScaling) + UIGameManager.Instance.mapImageXOffset), Mathf.RoundToInt(iconCharMap.transform.position.z * (0f - UIGameManager.Instance.mapImageYScaling) + UIGameManager.Instance.mapImageYOffset));
			UIGameManager.Instance.uiTabKill.InstantHide();
			UIGameManager.Instance.uiTabKill.gameObject.SetActive(value: false);
		}
		else
		{
			CloseMap();
		}
	}

	public void OnHoldSkip(InputAction.CallbackContext value)
	{
		if (value.started)
		{
			_ = LobbyManager.Instance != null;
		}
		else if (value.canceled && LobbyManager.Instance != null)
		{
			ResetButtonHold();
		}
	}

	public void OnCloseInventoryClick(InputAction.CallbackContext value)
	{
		if (value.performed && UIGameManager.Instance.uiInventory.isVisible && !UIGameManager.Instance.UIMenuPuzzle.isHidden)
		{
			NetworkGameManager.Instance.ownPlayer.DelayInputTimer.StartDuration(0.5f);
			ClosePuzzle();
		}
	}

	public void OnCancelClick(InputAction.CallbackContext value)
	{
		if (!value.performed || ((bool)DialogueSystem.Instance && !DialogueSystem.Instance.GetUIView.isHidden))
		{
			return;
		}
		if (UIGameManager.Instance.uiInventory.isHidden)
		{
			if (!UIGameManager.Instance.uiConfirmation.isHidden)
			{
				return;
			}
			if (UIGameManager.Instance.uiOptions.isVisible)
			{
				NetworkGameManager.Instance.ownPlayer.DelayInputTimer.StartDuration(0.5f);
				if (!OptionsManager.Instance.IsShowControlOnly)
				{
					AudioManager.PlaySFX("ui_cancel");
					UIGameManager.Instance.uiPause.Show();
					UIGameManager.Instance.uiOptions.Hide();
					GlobalSaveData.instance.SaveOptionData();
					if (UIGameManager.Instance.uiObjective != null)
					{
						UIGameManager.Instance.uiObjective.SetActive(value: false);
					}
					UIGameManager.Instance.mapUI.SetActive(value: false);
					{
						foreach (ConvertNote item in UIGameManager.Instance.arrConvertedText)
						{
							item.textMesh.text = UIGameManager.Instance.ConvertNote(item.initText);
						}
						return;
					}
				}
				AudioManager.PlaySFX("ui_cancel");
				OptionsManager.Instance.IsShowControlOnly = false;
				OptionsManager.Instance.TabButtonObject.SetActive(value: true);
				UIGameManager.Instance.uiOptions.Hide();
				UIGameManager.Instance.BackToInGame(null);
			}
			else if (!UIGameManager.Instance.UIMenuMap.isHidden)
			{
				NetworkGameManager.Instance.ownPlayer.DelayInputTimer.StartDuration(0.5f);
				CloseMap();
			}
			else if (!UIGameManager.Instance.UIMenuPuzzle.isHidden)
			{
				NetworkGameManager.Instance.ownPlayer.DelayInputTimer.StartDuration(0.5f);
				bool flag = false;
				if (MissionLobbyManager.Instance != null && MissionLobbyManager.Instance.missionBrief.enabled)
				{
					flag = true;
				}
				else if ((bool)MissionLobbyManager.Instance && MissionLobbyManager.Instance.MissionDetailMap.IsVisible)
				{
					flag = true;
					MissionLobbyManager.Instance.MissionDetailMap.CloseUI();
					AudioManager.PlaySFX("ui_cancel");
				}
				if (!flag)
				{
					ClosePuzzle();
				}
				else
				{
					MissionLobbyManager.Instance.missionBrief.enabled = false;
				}
			}
			else if (!UIGameManager.Instance.UIMenuNote.isHidden && UIGameManager.Instance.UIMenuNote.gameObject.activeSelf)
			{
				NetworkGameManager.Instance.ownPlayer.DelayInputTimer.StartDuration(0.5f);
				CloseNote();
			}
			else if (UIGameManager.Instance.uiPause.isHidden && !GlobalOptionsManager.Instance.usingGamepad && !UIGameManager.Instance.loading.loadingUI.activeSelf)
			{
				if (NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Solo && LobbyManager.Instance == null)
				{
					GameManager.Instance.PauseGameTime();
				}
				network.SetPlayerAFK(value: true);
				AudioManager.PlaySFX("ui_confirm");
				UIGameManager.Instance.uiPause.Show();
				UIGameManager.Instance.uiInventory.Hide();
				UIGameManager.Instance.uiInGame.Hide();
				if (UIGameManager.Instance.uiObjective != null)
				{
					UIGameManager.Instance.uiObjective.SetActive(value: false);
				}
				if (LobbyManager.Instance == null)
				{
					UIGameManager.Instance.mapUI.SetActive(value: false);
				}
				UIGameManager.Instance.uiOptions.Hide();
				NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: false);
				inventoryManager.frameInventory.Play("Inventory" + (data.GetMaxInventory() - 2));
				if (enableMoveChar)
				{
					direction = Vector3.zero;
					fsmUpperBody.SetBool("isMoving", value: false);
					fsmLowerBody.SetBool("isMoving", value: false);
					animLowerChar.Play("LegIdle" + NetworkGameManager.Instance.ownPlayer.angleRot, 1);
				}
			}
			else
			{
				if (UIGameManager.Instance.uiPause.isHidden)
				{
					return;
				}
				bool flag2 = false;
				if (GlobalOptionsManager.Instance.usingGamepad)
				{
					flag2 = true;
				}
				else if ((bool)SurvivorLobbyManager.Instance && SurvivorLobbyManager.Instance.GetSteamFriendView().IsShow)
				{
					SurvivorLobbyManager.Instance.GetSteamFriendView().Hide();
				}
				else
				{
					flag2 = true;
				}
				if (flag2)
				{
					NetworkGameManager.Instance.ownPlayer.DelayInputTimer.StartDuration(0.5f);
					if (NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Solo)
					{
						GameManager.Instance.ResumeGameTime();
					}
					network.SetPlayerAFK(value: true);
					EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
					network.SetPlayerAFK(value: false);
					AudioManager.PlaySFX("ui_cancel");
					if (!UIGameManager.Instance.isUIInvisible)
					{
						UIGameManager.Instance.uiInGame.Show();
					}
					UIGameManager.Instance.uiPause.Hide();
					if (UIGameManager.Instance.uiObjective != null && UIGameManager.Instance.uiObjective != null)
					{
						UIGameManager.Instance.uiObjective.SetActive(value: true);
					}
					if (LobbyManager.Instance == null)
					{
						UIGameManager.Instance.mapUI.SetActive(value: true);
					}
					NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: true);
				}
			}
		}
		else if (UIGameManager.Instance.inventoryOptions.activeSelf)
		{
			NetworkGameManager.Instance.ownPlayer.DelayInputTimer.StartDuration(0.5f);
			UIGameManager.Instance.inventoryOptions.SetActive(value: false);
			foreach (GameObject item2 in inventoryManager.inventoryPick)
			{
				item2.SetActive(value: false);
			}
			if (inventoryManager.targetInventory != null)
			{
				inventoryManager.targetInventory.GetComponent<Button>().Select();
			}
		}
		else if (ArmoryLobbyManager.Instance != null && ArmoryLobbyManager.Instance.OptionMenu.activeSelf)
		{
			NetworkGameManager.Instance.ownPlayer.DelayInputTimer.StartDuration(0.5f);
			ArmoryLobbyManager.Instance.ResetUI();
		}
		else if (UIGameManager.Instance.uiInventory.isVisible)
		{
			NetworkGameManager.Instance.ownPlayer.DelayInputTimer.StartDuration(0.5f);
			if (!UIGameManager.Instance.UIMenuPuzzle.isHidden)
			{
				ClosePuzzle();
			}
			else
			{
				UIGameManager.Instance.ShowHideInventory();
			}
		}
	}

	public void ShowDebug(InputAction.CallbackContext value)
	{
		if (!GameModes.Instance.isDebug || !value.performed)
		{
			return;
		}
		if (UIGameManager.Instance.uiDebug.isVisible)
		{
			UIGameManager.Instance.uiDebug.Hide();
			EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
			network.SetPlayerAFK(value: false);
			AudioManager.PlaySFX("ui_cancel");
			if (!UIGameManager.Instance.isUIInvisible)
			{
				UIGameManager.Instance.uiInGame.Show();
				if (LobbyManager.Instance == null)
				{
					UIGameManager.Instance.mapUI.SetActive(value: true);
				}
				if ((bool)UIGameManager.Instance.uiObjective)
				{
					UIGameManager.Instance.uiObjective.SetActive(value: true);
				}
			}
			else
			{
				UIGameManager.Instance.mapUI.SetActive(value: false);
				UIGameManager.Instance.uiObjective.SetActive(value: false);
			}
			NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: true);
		}
		else
		{
			UIGameManager.Instance.uiDebug.Show();
			network.SetPlayerAFK(value: true);
			AudioManager.PlaySFX("ui_confirm");
			UIGameManager.Instance.uiPause.Hide();
			UIGameManager.Instance.uiInventory.Hide();
			UIGameManager.Instance.uiInGame.Hide();
			UIGameManager.Instance.uiOptions.Hide();
			NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: false);
			direction = Vector3.zero;
		}
	}

	public void ShowBossScream(InputAction.CallbackContext value)
	{
		if (value.performed && GameManager.Instance.bossAnim != null)
		{
			GameManager.Instance.bossAnim.Play("scream");
			GameManager.Instance.bossAnim.Play("headScream", 3);
		}
	}

	public void CopyCode(InputAction.CallbackContext value)
	{
		if ((bool)UIGameManager.Instance.sessionName && value.started && UIGameManager.Instance.sessionName.transform.parent.gameObject.activeSelf)
		{
			UIGameManager.Instance.SessionFlashImage.DOKill(complete: true);
			UIGameManager.Instance.SessionFlashImage.DOFade(0.5f, 0f);
			UIGameManager.Instance.SessionFlashImage.DOFade(0f, 0.2f);
			AudioManager.PlaySFX("ui_confirm");
			GUIUtility.systemCopyBuffer = NetworkGameManager.Instance.sessionName;
		}
	}

	public void ShowCode(InputAction.CallbackContext value)
	{
		if ((bool)UIGameManager.Instance.sessionName)
		{
			if (value.started)
			{
				UIGameManager.Instance.sessionName.text = NetworkGameManager.Instance.sessionName;
			}
			else if (value.canceled)
			{
				UIGameManager.Instance.sessionName.text = "******";
			}
		}
	}

	public void VoiceChat(InputAction.CallbackContext value)
	{
		if (GlobalSaveData.instance.optionData.voiceChatMode == 0)
		{
			if (value.started)
			{
				IsMicOn = true;
			}
			if (value.canceled)
			{
				IsMicOn = false;
			}
		}
		else if (value.started)
		{
			if (VoiceChatGlobalController.Instance.IsMuted())
			{
				IsMicOn = true;
			}
			else
			{
				IsMicOn = false;
			}
		}
		if (value.started || value.canceled)
		{
			VoiceChatGlobalController.Instance.SetMuted(!IsMicOn);
			UIGameManager.Instance.micOn.SetActive(IsMicOn);
			UIGameManager.Instance.micOff.SetActive(!IsMicOn);
		}
	}

	public void DebugUnlockPuzzle(InputAction.CallbackContext value)
	{
		if (!value.started || !GameModes.Instance.isDebug)
		{
			return;
		}
		if (itemCollision != null)
		{
			ItemInteractable component = itemCollision.GetComponent<ItemInteractable>();
			if (component != null && component.isShowUI && component.UIMenu.GetComponent(typeof(IPuzzle)) is IPuzzle puzzle)
			{
				StartCoroutine(puzzle.PuzzleUnlocked());
			}
		}
		else if ((bool)UIGameManager.Instance.UIMenuPuzzle && !UIGameManager.Instance.UIMenuPuzzle.isHidden)
		{
			StartCoroutine((UIGameManager.Instance.UIMenuPuzzle.GetComponent(typeof(IPuzzle)) as IPuzzle)?.PuzzleUnlocked());
		}
	}

	public void DebugReviveHeal(InputAction.CallbackContext value)
	{
		if (value.started && GameModes.Instance.isDebug)
		{
			network.AddSubHealth(-1000f, trueDamage: true);
		}
	}

	public void DebugChangeModifier(InputAction.CallbackContext value)
	{
		if (!value.started || !GameModes.Instance.isDebug || !MissionLobbyManager.Instance || MissionLobbyManager.Instance.UIMenu.isHidden || !NetworkGameManager.Instance.isServer || !GameManagerPhoton.Instance.CurrentMission)
		{
			return;
		}
		if (GameManagerPhoton.Instance.CurrentMission.ListModifier.Count > 0)
		{
			foreach (Image item in MissionLobbyManager.Instance.MissionBoard.ListModifierIcon)
			{
				item.gameObject.SetActive(value: false);
			}
			int iD = GameManagerPhoton.Instance.CurrentMission.ListModifier[0].ID;
			iD++;
			if (GlobalMissionManager.Instance.GetMissionModifier(iD) == null)
			{
				iD = 0;
			}
			GameManagerPhoton.Instance.CurrentMission.ListModifier.Clear();
			GameManagerPhoton.Instance.CurrentMission.ListModifier.Add(GlobalMissionManager.Instance.GetMissionModifier(iD));
		}
		else
		{
			GameManagerPhoton.Instance.CurrentMission.ListModifier.Add(GlobalMissionManager.Instance.GetMissionModifier(0));
		}
		MissionLobbyManager.Instance.MissionBoard.ListModifierIcon[0].gameObject.SetActive(value: true);
		MissionLobbyManager.Instance.MissionBoard.ListModifierIcon[0].sprite = GameManagerPhoton.Instance.CurrentMission.ListModifier[0].spriteIcon;
		MissionLobbyManager.Instance.MissionBoard.ListModifierLocalizeText[0].SetTerm(GameManagerPhoton.Instance.CurrentMission.ListModifier[0].ModifierNameLocalization);
	}

	public void DebugChangeObjective(InputAction.CallbackContext value)
	{
		if (!value.started || !GameModes.Instance.isDebug || !MissionLobbyManager.Instance || MissionLobbyManager.Instance.UIMenu.isHidden || !NetworkGameManager.Instance.isServer || !GameManagerPhoton.Instance.CurrentMission)
		{
			return;
		}
		_ = GameManagerPhoton.Instance.CurrentMission;
		MissionSelection missionSelection = MissionLobbyManager.Instance.MissionBoard.GetMissionSelection(GameManagerPhoton.Instance.CurrentMission.MissionID);
		if (missionSelection.objectiveLevel <= 3)
		{
			int num = 0;
			for (int i = 0; i < GlobalMissionManager.Instance.ListRandomizeMissionObjectiveLv3.Count; i++)
			{
				if (missionSelection.MissionData.MissionObjective.ID == GlobalMissionManager.Instance.ListRandomizeMissionObjectiveLv3[i].ID)
				{
					num = i + 1;
					break;
				}
			}
			if (num >= GlobalMissionManager.Instance.ListRandomizeMissionObjectiveLv3.Count)
			{
				num = 0;
			}
			missionSelection.MissionData.MissionObjective = GlobalMissionManager.Instance.ListRandomizeMissionObjectiveLv3[num];
		}
		else
		{
			int num2 = 0;
			for (int j = 0; j < GlobalMissionManager.Instance.ListRandomizeMissionObjectiveLv4.Count; j++)
			{
				if (missionSelection.MissionData.MissionObjective.ID == GlobalMissionManager.Instance.ListRandomizeMissionObjectiveLv4[j].ID)
				{
					num2 = j + 1;
					break;
				}
			}
			if (num2 >= GlobalMissionManager.Instance.ListRandomizeMissionObjectiveLv4.Count)
			{
				num2 = 0;
			}
			missionSelection.MissionData.MissionObjective = GlobalMissionManager.Instance.ListRandomizeMissionObjectiveLv4[num2];
		}
		missionSelection.StickerObjective.sprite = missionSelection.MissionData.MissionObjective.IconSticker;
		missionSelection.SetUI();
		missionSelection.MissionData.PlayerSpawningIdx = 0;
		network.playerPhoton.RpcSyncMissionObjective((byte)missionSelection.MissionData.MissionObjective.ID, (byte)missionSelection.objectiveLevel);
		MissionLobbyManager.Instance.MissionBoard.TextScenarioLabel.SetTerm(missionSelection.MissionData.MapNameLocalization);
		MissionLobbyManager.Instance.MissionBoard.TextScenarioDesc.SetTerm(missionSelection.MissionData.MissionObjective.MissionModeDescLocalization);
		if (missionSelection.MissionData.MissionObjective.IsCountdownEndlessHordeEnable)
		{
			int num3 = 0;
			num3 = missionSelection.MissionData.MissionObjective.GetCountdownTimerEndlessHorde(NetworkGameManager.Instance.arrPlayerController.Count) / 60;
			MissionLobbyManager.Instance.MissionBoard.TextFieldScenarioDesc.text = MissionLobbyManager.Instance.MissionBoard.TextFieldScenarioDesc.text.Replace("(x)", num3.ToString());
		}
		if (missionSelection.MissionData.MissionObjective.MinTargetDestroy > 0 && missionSelection.MissionData.MissionObjective.TargetType != "")
		{
			MissionLobbyManager.Instance.MissionBoard.TextFieldScenarioDesc.text = MissionLobbyManager.Instance.MissionBoard.TextFieldScenarioDesc.text.Replace("(x)", missionSelection.MissionData.MissionObjective.MinTargetDestroy.ToString());
		}
	}

	public void OnSkip()
	{
		if (SceneManager.GetActiveScene().name == "Lobby")
		{
			network.SetPlayerReady(!network.GetReadyLobby());
		}
	}

	private void ResetButtonHold()
	{
		if (LobbyManager.Instance != null)
		{
			LobbyManager.Instance.sliderReady.gameObject.SetActive(value: false);
		}
		_pointerSkipDown = false;
		_pointerDownTimer = 0f;
	}

	public void AnglePlayerAim(Vector2 aim, bool towardsFunctionOn = true, bool isForceSetInput = false)
	{
		if (GlobalOptionsManager.Instance.usingGamepad)
		{
			if (Mathf.Abs(aim.x) > controllerDeadZone || Mathf.Abs(aim.y) > controllerDeadZone)
			{
				_aimDirection = new Vector3(aim.x, aim.y, 0f);
			}
			if (!(CameraGame.Instance.mainCam != null))
			{
				return;
			}
			Vector3 position = weaponPos.position;
			Vector2 vector = CameraGame.Instance.mainCam.WorldToScreenPoint(position) + _aimDirection * 350f;
			Ray ray = CameraGame.Instance.mainCam.ScreenPointToRay(vector);
			Plane plane = new Plane(Vector3.up, new Vector3(0f, position.y, 0f));
			Plane plane2 = new Plane(Vector3.up, new Vector3(0f, 0f, 0f));
			if (plane.Raycast(ray, out var enter))
			{
				Vector3 point = ray.GetPoint(enter);
				if (network.isLocalPlayer)
				{
					GameManager.Instance.targetCrosshair.position = point;
				}
				if ((prevInputRotation != aim || fsmLowerBody.GetBool(IsMovingAnim)) | isForceSetInput)
				{
					angleInput = point;
					prevInputRotation = aim;
				}
			}
			if (plane2.Raycast(ray, out var enter2))
			{
				Vector3 point2 = ray.GetPoint(enter2);
				angleGround = point2;
			}
		}
		else
		{
			if (!(CameraGame.Instance.mainCam != null))
			{
				return;
			}
			Ray ray2 = CameraGame.Instance.mainCam.ScreenPointToRay(aim);
			Plane plane3 = new Plane(Vector3.up, new Vector3(0f, weaponPos.position.y, 0f));
			Plane plane4 = new Plane(Vector3.up, new Vector3(0f, 0f, 0f));
			if (plane3.Raycast(ray2, out var enter3))
			{
				Vector3 point3 = ray2.GetPoint(enter3);
				if (MathFunc.Distance(point3, base.transform.position) < 20f && network.isLocalPlayer && GameManager.Instance != null)
				{
					GameManager.Instance.targetCrosshair.position = point3;
				}
				if ((prevInputRotation != aim || fsmLowerBody.GetBool(IsMovingAnim)) | isForceSetInput)
				{
					angleInput = point3;
					prevInputRotation = aim;
				}
			}
			if (plane4.Raycast(ray2, out var enter4))
			{
				Vector3 point4 = ray2.GetPoint(enter4);
				angleGround = point4;
			}
		}
	}

	public void AnglePlayer(Vector3 directionPlayer, Vector3 rotatePlayer)
	{
		if (!enableMoveChar)
		{
			return;
		}
		if (network.GetEnableControl())
		{
			if (directionPlayer != new Vector3(0f, 0f, 0f))
			{
				fsmUpperBody.SetBool(IsMovingAnim, value: true);
				fsmLowerBody.SetBool(IsMovingAnim, value: true);
				angleWalk = Mathf.FloorToInt(Quaternion.LookRotation(directionPlayer, Vector3.up).eulerAngles.y);
				angleWalk = Mathf.FloorToInt((angleWalk + 22.5f) / 45f) * 45;
				if (angleWalk < 0f)
				{
					angleWalk = 315f;
				}
				else if (Math.Abs(angleWalk - 360f) < 1f)
				{
					angleWalk = 0f;
				}
				if (!network.isLocalPlayer)
				{
					angleWalk -= CameraGame.Instance.camRotate;
				}
				if (angleWalk < 0f)
				{
					angleWalk = Mathf.RoundToInt(angleWalk + 360f);
				}
				else if (angleWalk >= 360f)
				{
					angleWalk = Mathf.RoundToInt(angleWalk - 360f);
				}
				if (Math.Abs(angleWalk - 360f) < 1f)
				{
					angleWalk = 0f;
				}
				float num = 0f;
				num = ((!network.isLocalPlayer) ? ((float)(Mathf.FloorToInt(Quaternion.LookRotation(rotatePlayer, Vector3.up).eulerAngles.y) - 45)) : ((float)(Mathf.FloorToInt(Quaternion.LookRotation(rotatePlayer - weaponPos.position, Vector3.up).eulerAngles.y) - 45)));
				if (num < 0f)
				{
					num += 360f;
				}
				num %= 360f;
				float num2 = Mathf.Abs(num - prevAngleRot);
				if (num2 > 180f)
				{
					num2 = 360f - num2;
				}
				if (num2 >= 28f)
				{
					angleRot = Mathf.FloorToInt((num + 22.5f) / 45f) * 45;
					angleRotWithoutCam = angleRot;
					angleRot -= CameraGame.Instance.camRotate - 45;
					if (angleRot < 0f)
					{
						angleRot = Mathf.RoundToInt(angleRot + 360f);
					}
					else if (angleRot >= 360f)
					{
						angleRot = Mathf.RoundToInt(angleRot - 360f);
					}
					if (Math.Abs(angleRot - 360f) < 1f)
					{
						angleRot = 0f;
					}
					prevAngleRot = angleRot;
				}
				float num3 = Mathf.DeltaAngle(angleRot, angleWalk);
				if ((angleRot <= 180f && (num3 > 90f || num3 <= -90f)) || (angleRot > 180f && (num3 >= 90f || num3 < -90f)))
				{
					int num4 = Mathf.RoundToInt(angleWalk + 180f);
					if (num4 >= 360)
					{
						num4 -= 360;
					}
					if (angleRot == 0f && angleWalk == 270f)
					{
						animLowerChar.Play("LegMove270", 1);
					}
					else
					{
						animLowerChar.Play("LegBMove" + num4, 1);
					}
				}
				else if (angleRot == 0f && angleWalk == 90f)
				{
					animLowerChar.Play("LegBMove" + 270, 1);
				}
				else
				{
					animLowerChar.Play("LegMove" + angleWalk, 1);
				}
				weaponController.ReloadAnimation();
			}
			else
			{
				fsmUpperBody.SetBool(IsMovingAnim, value: false);
				fsmLowerBody.SetBool(IsMovingAnim, value: false);
				float num5 = 0f;
				num5 = ((!network.isLocalPlayer) ? ((float)(Mathf.FloorToInt(Quaternion.LookRotation(rotatePlayer, Vector3.up).eulerAngles.y) - 45)) : ((float)(Mathf.FloorToInt(Quaternion.LookRotation(rotatePlayer - weaponPos.position, Vector3.up).eulerAngles.y) - 45)));
				if (num5 < 0f)
				{
					num5 += 360f;
				}
				num5 %= 360f;
				float num6 = Mathf.Abs(num5 - prevAngleRot);
				if (num6 > 180f)
				{
					num6 = 360f - num6;
				}
				if (num6 >= 28f)
				{
					angleRot = Mathf.FloorToInt((num5 + 22.5f) / 45f) * 45;
					angleRotWithoutCam = angleRot;
					angleRot -= CameraGame.Instance.camRotate - 45;
					if (angleRot < 0f)
					{
						angleRot = Mathf.RoundToInt(angleRot + 360f);
					}
					else if (angleRot >= 360f)
					{
						angleRot = Mathf.RoundToInt(angleRot - 360f);
					}
					if (Math.Abs(angleRot - 360f) < 1f)
					{
						angleRot = 0f;
					}
				}
				animLowerChar.Play("LegIdle" + angleRot, 1);
				weaponController.ReloadAnimation();
				prevAngleRot = angleRot;
			}
			if (angleRotWithoutCam < 0f)
			{
				angleRotWithoutCam = Mathf.RoundToInt(angleRotWithoutCam + 360f);
			}
			else if (angleRotWithoutCam >= 360f)
			{
				angleRotWithoutCam = Mathf.RoundToInt(angleRotWithoutCam - 360f);
			}
			if (Math.Abs(angleRotWithoutCam - 360f) < 1f)
			{
				angleRotWithoutCam = 0f;
			}
			if (!isDashing)
			{
				Vector3 eulerAngles = meleeCollider.eulerAngles;
				eulerAngles.y = angleRotWithoutCam;
				meleeCollider.eulerAngles = eulerAngles;
			}
		}
		else
		{
			animLowerChar.Play("LegIdle" + angleRot, 1);
		}
	}

	private void CrosshairPositionMouse(Vector2 aim)
	{
		if (GlobalOptionsManager.Instance.usingGamepad || !network.isLocalPlayer || !(UIGameManager.Instance != null))
		{
			return;
		}
		UIGameManager.Instance.crosshairGrid.spacing = new Vector2(-100f + weaponController.accuracy, -100f + weaponController.accuracy);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(UIGameManager.Instance.canvasCrosshair.transform as RectTransform, aim, UIGameManager.Instance.canvasCrosshair.worldCamera, out var localPoint);
		UIGameManager.Instance.crosshair.transform.position = UIGameManager.Instance.canvasCrosshair.transform.TransformPoint(localPoint);
		Vector3 vector = new Vector3(angleGround.x, weaponPos.position.y, angleGround.z);
		Vector3 normalized = (vector - weaponPos.position).normalized;
		float maxDistance = Vector3.Distance(weaponPos.position, vector);
		if (canGrenade && UIGameManager.Instance.cursorGrenade.gameObject.activeSelf)
		{
			if (Physics.Raycast(weaponPos.position, normalized, out var hitInfo, maxDistance, GameManager.Instance.layerGrenade))
			{
				Vector3 vector2 = weaponController.transform.position + normalized * hitInfo.distance;
				UIGameManager.Instance.cursorGrenade.transform.position = Vector3.Lerp(UIGameManager.Instance.cursorGrenade.transform.position, new Vector3(vector2.x, angleGround.y, vector2.z), 0.25f);
			}
			else
			{
				UIGameManager.Instance.cursorGrenade.transform.position = Vector3.Lerp(UIGameManager.Instance.cursorGrenade.transform.position, angleGround, 0.25f);
			}
		}
	}

	private void CrosshairPositionGamepad(Vector3 aim)
	{
		if (!GlobalOptionsManager.Instance.usingGamepad || !network.isLocalPlayer || (!(Mathf.Abs(aim.x) > controllerDeadZone) && !(Mathf.Abs(aim.y) > controllerDeadZone) && !(aim == Vector3.zero)))
		{
			return;
		}
		if (aim == Vector3.zero)
		{
			Vector3 vector = Quaternion.Euler(0f, angleRot, 0f) * Vector3.forward;
			prevAim = new Vector2(vector.x, vector.z) * 0.2f;
			aim = new Vector2(vector.x, vector.z) * 0.2f;
		}
		float num = 150f + Vector2.Distance(Vector2.zero, aim) * 200f;
		if (!(CameraGame.Instance.mainCam != null))
		{
			return;
		}
		UIGameManager.Instance.crosshairGrid.spacing = new Vector2(-100f + weaponController.accuracy, -100f + weaponController.accuracy);
		Vector3 vector2 = CameraGame.Instance.mainCam.WorldToScreenPoint(new Vector3(weaponPos.position.x, weaponPos.position.y - 0.1f, weaponPos.position.z));
		Vector3 normalized = new Vector3(aim.x, aim.y, 0f).normalized;
		if (aim != Vector3.zero)
		{
			UIGameManager.Instance.crosshair.position = vector2 + normalized * num;
			_lastAngleGamepad = normalized;
		}
		else
		{
			UIGameManager.Instance.crosshair.position = vector2 + _lastAngleGamepad * 150f;
		}
		if (canGrenade && UIGameManager.Instance.cursorGrenade.gameObject.activeSelf)
		{
			Vector3 vector3 = IsoDirection(new Vector3(aim.x, 0f, aim.y).normalized);
			if (aim != Vector3.zero)
			{
				float num2 = 2f + Vector2.Distance(Vector2.zero, aim) * 3f;
				UIGameManager.Instance.cursorGrenade.transform.position = origin.position + vector3 * num2;
				_lastAngleGamepadGrenade = vector3;
			}
			else
			{
				UIGameManager.Instance.cursorGrenade.transform.position = origin.position + _lastAngleGamepadGrenade * 2f;
			}
		}
	}

	public void PickObject(ItemPickable item)
	{
		if (item != null)
		{
			item.PickObject(this);
		}
	}

	public void OnRemoveObject(int uniqueID)
	{
		ItemPickable itemPickable = GameManager.Instance.GetItemPickable(uniqueID);
		if (itemPickable != null)
		{
			if (itemPickable.itemCollider.enabled)
			{
				itemPickable.OnRemoveObjectCustomFunction?.Execute(this);
				itemPickable.itemCollider.enabled = false;
				itemPickable.SetSpriteEnable(value: false);
				if ((bool)itemPickable.GameObjectMap)
				{
					itemPickable.GameObjectMap.SetActive(value: false);
				}
				itemPickable.itemMap.enabled = false;
				if (NetworkGameManager.Instance.ownPlayer != null && NetworkGameManager.Instance.ownPlayer.itemCollision == itemPickable.gameObject)
				{
					NetworkGameManager.Instance.ownPlayer.itemCollision = null;
					NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
					ChatSystem.Instance.ItemCommand.SetActive(value: false);
				}
				itemCollision = null;
				itemCollisionCollider = null;
			}
			else if (NetworkGameManager.Instance.isServer && itemPickable.itemType != "Material")
			{
				InventoryObject inventoryObject = data.FindInventory(itemPickable.itemID);
				if (inventoryObject != null)
				{
					int idxInventory = inventoryObject.IdxInventory;
					if (idxInventory <= 1)
					{
						weaponController.UnEquipWeapon(idxInventory, fromServer: false);
					}
					data.RemoveInventory(idxInventory, syncNetwork: true, duplicateItem: true, itemPickable.amount);
				}
			}
			if (itemPickable.roomCollider != null)
			{
				itemPickable.roomCollider.CheckMap(this);
			}
		}
		RoomCollider roomCollider = GameManager.Instance.GetRoomCollider(RoomName);
		if (roomCollider != null)
		{
			roomCollider.CheckMap(this);
		}
	}

	public async UniTask Dash(Vector3 dirDash = default(Vector3), bool isUsingStamina = true, bool isDashAttack = false, bool isTrailEffectEnable = true, float delayStart = 0.1f, float durationDash = 0.2f, float delayEnd = 0.1f)
	{
		CancellationToken token = this.GetCancellationTokenOnDestroy();
		Vector3 vector = network.GetAngledirection();
		if (dirDash != default(Vector3))
		{
			vector = dirDash;
		}
		if (!(vector != Vector3.zero))
		{
			return;
		}
		canDash = false;
		isDashing = true;
		enableMoveChar = false;
		AudioManager.PlaySFXTransform("player-dash", base.transform, network.isLocalPlayer);
		if (isDashAttack)
		{
			animLowerChar.Play("LegDodge" + angleRot + "-" + angleRot, 1, 0f);
		}
		else
		{
			animLowerChar.Play("LegDodge" + angleRot + "-" + angleWalk, 1, 0f);
		}
		if (!weaponController.isMeleeCharging && !isAttackMelee)
		{
			SetAnimUpperSpeed(1f);
		}
		SetAnimLowerSpeed(animspeed);
		if (network.isLocalPlayer & isUsingStamina)
		{
			data.AddSubCurrentStamina((0f - data.dodgeStamina) * PlayerMultiplyStatsData.GetMultiplyStaminaDashConsumption());
			UIGameManager.Instance.barStamina.DOValue(data.GetStamina() / data.GetMaxStamina(), 0.15f);
		}
		await UniTask.Delay(TimeSpan.FromSeconds(delayStart), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		if (isTrailEffectEnable)
		{
			trail.StartTrail();
		}
		if (PlayerMultiplyStatsData.GetDashAttackDamage() > 0f)
		{
			Vector3 eulerAngles = meleeCollider.eulerAngles;
			eulerAngles.y = angleWalk + (float)(CameraGame.Instance.camRotate - 45);
			meleeCollider.eulerAngles = eulerAngles;
			weaponController.ShowMeleeCollider();
		}
		isDashingMove = true;
		await UniTask.Delay(TimeSpan.FromSeconds(durationDash), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		if (!weaponController.isMeleeCharging && !isAttackMelee)
		{
			SetAnimUpperSpeed(1f);
		}
		SetAnimLowerSpeed(animspeed);
		isDashingMove = false;
		await UniTask.Delay(TimeSpan.FromSeconds(delayEnd), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		if (network.GetHealth() > 0f)
		{
			enableMoveChar = true;
		}
		isDashing = false;
		if (!isLMBDown && (isAiming || isRMBDown) && fsmUpperBody.GetBool(IsShooting))
		{
			data.SetCurrentMoveSpeed(data.GetMoveAimSpeed());
			float animUpperSpeed = 0.55f;
			if (data.GetMoveAimSpeed() < 1f)
			{
				animUpperSpeed = 0.45f;
			}
			else if (data.GetMoveAimSpeed() > 1.5f)
			{
				animUpperSpeed = 0.75f;
			}
			SetAnimLowerSpeed(animspeed);
			SetAnimUpperSpeed(animUpperSpeed);
			isSprinting = false;
		}
		else
		{
			if (!weaponController.isMeleeCharging && !isAttackMelee)
			{
				if (animUpperChar.GetCurrentAnimatorStateInfo(0).IsTag("Reload"))
				{
					if (weaponController.rangeWeaponType == RangeWeaponType.Shotgun)
					{
						SetAnimUpperSpeed(2f);
					}
					else if (weaponController.rangeWeaponType == RangeWeaponType.SMG || weaponController.rangeWeaponType == RangeWeaponType.Crossbow)
					{
						SetAnimUpperSpeed(0.7f);
					}
					else
					{
						SetAnimUpperSpeed(1f);
					}
				}
				else
				{
					SetAnimUpperSpeed(1f);
				}
			}
			SetAnimLowerSpeed(animspeed);
		}
		float animlowerSpeed = animLowerChar.speed;
		AnglePlayerAim(inputRotation, towardsFunctionOn: true, isForceSetInput: true);
		await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		if (isTrailEffectEnable)
		{
			trail.StopTrail();
		}
		directionDash = Vector3.zero;
		if (canSprint)
		{
			canDash = true;
		}
		SetAnimLowerSpeed(animlowerSpeed);
		AnglePlayerAim(inputRotation, towardsFunctionOn: true, isForceSetInput: true);
		await UniTask.Delay(TimeSpan.FromSeconds(0.30000001192092896), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		AnglePlayerAim(inputRotation, towardsFunctionOn: true, isForceSetInput: true);
	}

	public void StartSprint()
	{
		isSprinting = true;
		data.SetCurrentMoveSpeed(data.GetSprintSpeed());
		animspeed = 1.2f;
		SetAnimLowerSpeed(1.35f);
	}

	public void StopSprint()
	{
		animspeed = 1f;
		SetAnimLowerSpeed(1f);
		isSprinting = false;
		if (isAiming || isAttacking)
		{
			data.SetCurrentMoveSpeed(data.GetMoveAimSpeed());
		}
		else
		{
			data.SetCurrentMoveSpeed(data.GetInitialMoveSpeed());
		}
	}

	public void SetAiming(bool value, bool isWithoutCheckRMBDown = false)
	{
		if ((isRMBDown != value || !network.isLocalPlayer) && (!((isRMBDown != value) | isWithoutCheckRMBDown) || network.isLocalPlayer))
		{
			return;
		}
		isRMBDown = value;
		if (weaponController.idWeaponRange <= 0 || !isRangeActive || isAttackMelee)
		{
			return;
		}
		if (value)
		{
			if (network.GetEnableControl() && (enableMoveChar || isDashing))
			{
				if (!isAimingToggle)
				{
					isAiming = isRMBDown;
				}
				SetAimingSpeed();
			}
		}
		else
		{
			if (!isAimingToggle)
			{
				isAiming = isRMBDown;
			}
			weaponController.ctrBulletShoot = 0;
			SetAimingSpeed();
		}
	}

	public void SetAimingSpeed(bool isFirstShoot = true)
	{
		if (isAiming)
		{
			fsmUpperBody.SetBool("isReloading", value: false);
			if (isSprinting)
			{
				isSprinting = false;
			}
			data.SetCurrentMoveSpeed(data.GetMoveAimSpeed());
			float num = 0.55f;
			if (data.GetMoveAimSpeed() < 1f)
			{
				num = 0.45f;
			}
			else if (data.GetMoveAimSpeed() > 1.5f)
			{
				num = 0.75f;
			}
			SetAnimLowerSpeed(num);
			SetAnimUpperSpeed(num);
			if (isFirstShoot)
			{
				DOTween.Kill("AccuracySubtract");
				weaponController.accuracy = weaponController.maxRangeAccuracy;
			}
			else
			{
				weaponController.accuracy += 35f;
				if (weaponController.accuracy > weaponController.maxRangeAccuracy)
				{
					weaponController.accuracy = weaponController.maxRangeAccuracy;
				}
			}
			if (!network.isLocalPlayer || UIGameManager.Instance.isUIInvisible)
			{
				return;
			}
			UIGameManager.Instance.crosshair.gameObject.SetActive(value: true);
			if (!fsmLowerBody.GetBool(IsMovingAnim))
			{
				DOTween.Kill("AccuracySubtract");
				DOTween.To(() => weaponController.accuracy, (float x) =>
				{
					weaponController.accuracy = x;
				}, weaponController.minRangeAccuracy, weaponController.timeAccuracy).SetId("AccuracySubtract").SetEase(Ease.Linear);
			}
		}
		else
		{
			if (network.isLocalPlayer && UIGameManager.Instance.crosshair != null)
			{
				UIGameManager.Instance.crosshair.gameObject.SetActive(value: false);
			}
			if (isSprintDown && canSprint)
			{
				StartSprint();
			}
			else
			{
				data.SetCurrentMoveSpeed(data.GetInitialMoveSpeed());
			}
			SetAnimLowerSpeed(animspeed);
		}
	}

	public void DeviceChange(PlayerInput myPlayerInput)
	{
		GlobalOptionsManager.Instance.DeviceChange(myPlayerInput);
	}

	private Vector3 IsoDirection(Vector3 theDirection)
	{
		return Matrix4x4.Rotate(Quaternion.Euler(0f, CameraGame.Instance.camRotate, 0f)).MultiplyPoint3x4(theDirection);
	}

	public void ChangeWeaponPlayer(int idx)
	{
		AudioManager.PlaySFXTransform("ranged_pickup", base.transform, network.isLocalPlayer);
		weaponController.weaponSelect = idx;
		switch (idx)
		{
		case 0:
		{
			isRMBDown = false;
			for (int j = 0; j < SkinManager.Instance.listMeleeWeapon.Count; j++)
			{
				if (SkinManager.Instance.listMeleeWeapon[j].name == "Melee_" + network.GetIdWeapon0())
				{
					weaponController.meleeSpriteLib.spriteLibraryAsset = SkinManager.Instance.listMeleeWeapon[j];
				}
			}
			break;
		}
		case 1:
		{
			isRMBDown = true;
			for (int i = 0; i < SkinManager.Instance.listRangeWeapon.Count; i++)
			{
				if (SkinManager.Instance.listRangeWeapon[i].name == "Range_" + DataManager.Instance.GetBaseWeapon(network.GetIdWeapon1()))
				{
					weaponController.rangeSpriteLib.spriteLibraryAsset = SkinManager.Instance.listRangeWeapon[i];
				}
			}
			break;
		}
		}
		network.SelectWeapon(idx);
	}

	public void ThrowWeapon(Vector3 posThrowObject, int idItem)
	{
		(DataManager.Instance.Get<WeaponLibraryScriptableObject>()?.GetData(idItem) as ThrowableWeapon)?.Throw(this, posThrowObject);
	}

	public void SetAnimUpperSpeed(float value)
	{
		animUpperChar.speed = value * timeline.timeScale;
	}

	public void SetAnimLowerSpeed(float value)
	{
		animLowerChar.speed = value * timeline.timeScale;
	}

	public void CloseNote()
	{
		AudioManager.PlaySFX("examine-corpse");
		UIGameManager.Instance.UIMenuNote.Hide();
		if (!UIGameManager.Instance.isUIInvisible)
		{
			UIGameManager.Instance.uiInGame.Show();
			if (UIGameManager.Instance.uiObjective != null && UIGameManager.Instance.uiObjective != null)
			{
				UIGameManager.Instance.uiObjective.SetActive(value: true);
			}
			if (LobbyManager.Instance == null)
			{
				UIGameManager.Instance.mapUI.SetActive(value: true);
			}
		}
		network.SetEnableControl(value: true);
		itemCollision = null;
		itemCollisionCollider = null;
		functionItemCollision = "";
		ChatSystem.Instance.ItemCommand.SetActive(value: false);
		UIGameManager.Instance.uiTabKill.gameObject.SetActive(value: true);
		EventSystem.current.SetSelectedGameObject(null);
	}

	public void ClosePuzzle(bool forceShow = false)
	{
		if ((bool)SurvivorLobbyManager.Instance && SurvivorLobbyManager.Instance.GetSteamFriendView().IsShow)
		{
			SurvivorLobbyManager.Instance.GetSteamFriendView().Hide();
		}
		else
		{
			if (UIGameManager.Instance.UIProgressing)
			{
				return;
			}
			UIGameManager.Instance.UIMenuPuzzle.Hide();
			UniTaskUtil.DelayedCall(this, 0.1f, () =>
			{
				network.SetEnableControl(value: true);
			}).Forget();
			enableMoveChar = true;
			if (!UIGameManager.Instance.isUIInvisible | forceShow)
			{
				UIGameManager.Instance.uiInGame.Show();
				if (LobbyManager.Instance == null)
				{
					UIGameManager.Instance.mapUI.SetActive(value: true);
				}
				if (UIGameManager.Instance.uiObjective != null && UIGameManager.Instance.uiObjective != null)
				{
					UIGameManager.Instance.uiObjective.SetActive(value: true);
				}
			}
			itemCollision = null;
			itemCollisionCollider = null;
			functionItemCollision = "";
			ChatSystem.Instance.ItemCommand.SetActive(value: false);
			if (UIGameManager.Instance.UIMenuPuzzle.GetComponent(typeof(IPuzzle)) is IPuzzle puzzle)
			{
				puzzle.Hide();
			}
			else
			{
				UIGameManager.Instance.UIMenuPuzzle.Hide();
			}
			UIGameManager.Instance.uiTabKill.gameObject.SetActive(value: true);
			UIGameManager.Instance.blankUI.InstantHide();
			UIGameManager.Instance.UIMenuPuzzle = UIGameManager.Instance.blankUI;
			EventSystem.current.SetSelectedGameObject(null);
			if (NetworkGameManager.Instance.isServer)
			{
				network.playerPhoton.IsInteractingPuzzle = false;
			}
			else
			{
				network.playerPhoton.RpcSetInteractingPuzzle(value: false);
			}
		}
	}

	public void CloseMap()
	{
		if (CameraMiniMap.Instance != null && GlobalSaveData.instance.optionData.autoMinimap == 1)
		{
			foreach (ItemPickable item in GameManager.Instance.arrItemPickable)
			{
				if (item.itemMap != null)
				{
					item.itemMap.transform.DOLocalRotate(new Vector3(90f, 0f, -CameraGame.Instance.camRotate), 0f);
				}
			}
		}
		network.SetPlayerAFK(value: false);
		CameraMiniMap.Instance.gameObject.SetActive(value: true);
		AudioManager.PlaySFX("ui_cancel");
		CameraGame.Instance.cameraMap.gameObject.SetActive(value: false);
		if (!UIGameManager.Instance.isUIInvisible)
		{
			UIGameManager.Instance.uiInGame.Show();
			if (UIGameManager.Instance.uiObjective != null && UIGameManager.Instance.uiObjective != null)
			{
				UIGameManager.Instance.uiObjective.SetActive(value: true);
			}
			if (LobbyManager.Instance == null)
			{
				UIGameManager.Instance.mapUI.SetActive(value: true);
			}
		}
		NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: true);
		UIGameManager.Instance.UIMenuMap.Hide();
		CameraGame.Instance.RotateRoomText();
		UIGameManager.Instance.HideMapNameText();
		foreach (PlayerController item2 in NetworkGameManager.Instance.arrPlayerController)
		{
			if (item2.network.isLocalPlayer)
			{
				item2.iconCharMap.DOScale(4f, 0f);
				item2.iconCharMapAnimator.Play("Default");
				item2.iconCharMap.parent = origin;
				item2.directionMap.DOLocalRotate(new Vector3(90f, 0f, 0f), 0f);
				item2.iconCharMap.DOLocalRotate(new Vector3(90f, 0f, 0f), 0f);
				item2.cursorLocalPlayer.SetActive(value: false);
			}
			else if (GlobalSaveData.instance.optionData.autoMinimap == 1)
			{
				item2.iconCharMap.DORotate(new Vector3(90f, 0f, -CameraGame.Instance.camRotate), 0f);
			}
		}
		UIGameManager.Instance.uiTabKill.gameObject.SetActive(value: true);
		InitPlayerList();
	}

	public void InitPlayerList()
	{
		for (int i = 0; i < 4; i++)
		{
			PlayerBoard.Instance.boardPlayerList[i].SetActive(value: false);
		}
		for (int j = 0; j < NetworkGameManager.Instance.arrPlayerController.Count; j++)
		{
			PlayerNetwork playerNetwork = NetworkGameManager.Instance.arrPlayerController[j].network;
			int iDX = playerNetwork.GetIDX();
			if ((playerNetwork.isLocalPlayer || !(LobbyManager.Instance == null)) && (playerNetwork.isLocalPlayer || !(LobbyManager.Instance != null) || !LobbyManager.Instance.testMode) && (!(LobbyManager.Instance != null) || LobbyManager.Instance.testMode))
			{
				continue;
			}
			if (playerNetwork.GetIdWeapon0() > 0)
			{
				PlayerBoard.Instance.Weapon0[iDX].enabled = true;
				PlayerBoard.Instance.Weapon0[iDX].sprite = DataManager.Instance.GetItemSprite(playerNetwork.GetIdWeapon0().ToString());
			}
			else
			{
				PlayerBoard.Instance.Weapon0[iDX].enabled = false;
			}
			if (playerNetwork.GetIdWeapon1() > 0)
			{
				PlayerBoard.Instance.Weapon1[iDX].enabled = true;
				PlayerBoard.Instance.Weapon1[iDX].sprite = DataManager.Instance.GetItemSprite(playerNetwork.GetIdWeapon1().ToString());
			}
			else
			{
				PlayerBoard.Instance.Weapon1[iDX].enabled = false;
			}
			PlayerBoard.Instance.SetPlayerSkill(playerNetwork.playerController);
			PlayerBoard.Instance.Hp[iDX].text = Mathf.RoundToInt(playerNetwork.GetHealth()).ToString();
			PlayerBoard.Instance.boardPlayerList[iDX].SetActive(value: true);
			PlayerBoard.Instance.playerNameList[iDX].text = playerNetwork.GetPlayerName();
			PlayerBoard.Instance.ChangeAvatarPlayerBoard(iDX, playerNetwork.playerController.data.PlayerSkinData);
			UIGameManager.Instance.ChangeMiniAvatarReadyStatus(iDX, playerNetwork.playerController.data.PlayerSkinData);
			UIGameManager.Instance.SetPerkSkillUIInfo(playerNetwork.playerController);
			for (int k = 0; k < PlayerBoard.Instance.inventoryItem[iDX].item.Count; k++)
			{
				PlayerBoard.Instance.inventoryItem[iDX].item[k].gameObject.SetActive(value: false);
			}
			int num = 0;
			for (int l = 2; l < NetworkGameManager.Instance.arrPlayerController[j].data.arrInventory.Count; l++)
			{
				InventoryObject inventoryObject = playerNetwork.playerController.data.arrInventory[l];
				if (inventoryObject.Name != "Null" && inventoryObject.ID != -1)
				{
					PlayerBoard.Instance.inventoryItem[iDX].item[num].gameObject.SetActive(value: true);
					PlayerBoard.Instance.inventoryItem[iDX].item[num].sprite = DataManager.Instance.GetItemSprite(inventoryObject.ID.ToString());
					num++;
				}
			}
		}
	}

	public void InitPlayerInventoryBoard()
	{
		for (int i = 0; i < NetworkGameManager.Instance.arrPlayerController.Count; i++)
		{
			PlayerNetwork playerNetwork = NetworkGameManager.Instance.arrPlayerController[i].network;
			int iDX = playerNetwork.GetIDX();
			if ((playerNetwork.isLocalPlayer || !(LobbyManager.Instance == null)) && (playerNetwork.isLocalPlayer || !(LobbyManager.Instance != null) || !LobbyManager.Instance.testMode) && (!(LobbyManager.Instance != null) || LobbyManager.Instance.testMode))
			{
				continue;
			}
			if (playerNetwork.GetIdWeapon0() > 0)
			{
				PlayerBoard.Instance.Weapon0[iDX].enabled = true;
				PlayerBoard.Instance.Weapon0[iDX].sprite = DataManager.Instance.GetItemSprite(playerNetwork.GetIdWeapon0().ToString());
			}
			else
			{
				PlayerBoard.Instance.Weapon0[iDX].enabled = false;
			}
			if (playerNetwork.GetIdWeapon1() > 0)
			{
				PlayerBoard.Instance.Weapon1[iDX].enabled = true;
				PlayerBoard.Instance.Weapon1[iDX].sprite = DataManager.Instance.GetItemSprite(playerNetwork.GetIdWeapon1().ToString());
			}
			else
			{
				PlayerBoard.Instance.Weapon1[iDX].enabled = false;
			}
			for (int j = 0; j < PlayerBoard.Instance.inventoryItem[iDX].item.Count; j++)
			{
				PlayerBoard.Instance.inventoryItem[iDX].item[j].gameObject.SetActive(value: false);
			}
			int num = 0;
			for (int k = 2; k < NetworkGameManager.Instance.arrPlayerController[i].data.arrInventory.Count; k++)
			{
				InventoryObject inventoryObject = playerNetwork.playerController.data.arrInventory[k];
				if (inventoryObject.Name != "Null" && inventoryObject.ID != -1)
				{
					if (num < PlayerBoard.Instance.inventoryItem[iDX].item.Count)
					{
						PlayerBoard.Instance.inventoryItem[iDX].item[num].gameObject.SetActive(value: true);
						PlayerBoard.Instance.inventoryItem[iDX].item[num].sprite = DataManager.Instance.GetItemSprite(inventoryObject.ID.ToString());
					}
					num++;
				}
			}
		}
	}

	public void StopInteractProgress(ItemInteractable item = null)
	{
		if (!(itemCollision != null))
		{
			return;
		}
		if (item == null)
		{
			item = itemCollision.GetComponent<ItemInteractable>();
		}
		if (item != null)
		{
			if (!item.isAutoProgress)
			{
				network.SetPlayerAFK(value: false);
				enableMoveChar = true;
				network.ExecStopProgressInteract((short)item.UniqueID);
				fsmUpperBody.SetBool("isReviving", value: false);
				if (item.boxCollider.enabled)
				{
					ChatSystem.Instance.ItemCommand.gameObject.SetActive(value: true);
				}
			}
		}
		else
		{
			network.SetPlayerAFK(value: false);
			enableMoveChar = true;
			fsmUpperBody.SetBool("isReviving", value: false);
			network.ExecStopProgressInteract();
		}
	}

	public void InputReleasingEntangled()
	{
		if (!isEntangled || !(network.GetHealth() > 0f))
		{
			return;
		}
		ctrReleaseEntangled--;
		PlayerStatusUI.Instance.ProgresBar[network.GetIDX()].value = (float)(maxCtrReleaseEntangled - ctrReleaseEntangled) * 1f / (float)maxCtrReleaseEntangled;
		if (ctrReleaseEntangled <= 0)
		{
			isEntangled = false;
			network.charControllerPhoton.charControl.enabled = true;
			animUpperChar.Play("TangledEnd" + angleRot, -1, 0f);
			animLowerChar.Play("LegDown" + angleRot);
			UniTaskUtil.DelayedCall(this, 0.6f, () =>
			{
				network.ExecReleaseEnTangled();
			}).Forget();
			CameraGame.Instance.CameraShake();
		}
	}

	public void InputGetUp()
	{
		if (network.GetLife() > 0 && network.GetHealth() <= 0f && ctrGetUp > 0)
		{
			ctrGetUp--;
			PlayerStatusUI.Instance.ProgresBar[network.GetIDX()].value = (float)(maxCtrGetUp - ctrGetUp) * 1f / (float)maxCtrGetUp;
			if (ctrGetUp <= 0)
			{
				PlayerStatusUI.Instance.SetDisableMashButton(network.GetIDX());
				network.SetHealth(data.GetMaxHealth());
				network.SetLife((byte)(network.GetLife() - 1));
				UIGameManager.Instance.ArrPlayerInfo[network.GetIDX()].TextProgressMashButton.transform.parent.parent.gameObject.SetActive(value: false);
			}
		}
	}

	public void ReleaseEntangled()
	{
		if (network.GetHealth() > 0f || (bool)LobbyManager.Instance)
		{
			if (network.isLocalPlayer)
			{
				UIGameManager.Instance.ArrPlayerInfo[network.GetIDX()].ChargeMeleeProgressObject.SetActive(value: false);
				weaponController.MeleeTween.Kill();
				PlayerStatusUI.Instance.SetDisableMashButton(network.GetIDX());
			}
			animUpperChar.Play("IdleMelee" + angleRot);
			animLowerChar.Play("LegIdle" + angleRot);
			sortGroup.sortingLayerName = "Default";
			isEntangled = false;
			network.charControllerPhoton.charControl.enabled = true;
			network.SetEnableControl(value: true);
			enableMoveChar = true;
			network.charControllerPhoton.charControl.detectCollisions = true;
			network.charControllerPhoton.Collider.enabled = true;
			Physics.SyncTransforms();
			SetAnimLowerSpeed(1f);
			SetAnimUpperSpeed(1f);
			animUpperChar.transform.DOKill();
			if (!isLMBDown)
			{
				weaponController.isMeleeCharging = false;
				weaponController.isHalfMeleeCharging = false;
			}
			isAiming = false;
			fsmUpperBody.Play("Idle");
			fsmUpperBody.SetBool("isMoving", value: false);
			fsmUpperBody.SetBool("isMelee", value: false);
			fsmUpperBody.SetBool("isShooting", value: false);
			fsmUpperBody.SetBool("isReviving", value: false);
			fsmUpperBody.SetBool("isReloading", value: false);
			isAttacking = false;
			isAttackMelee = false;
			isShooting = false;
			isThrowing = false;
			isAiming = false;
			isAttackBtnPressed = false;
			enableMoveChar = true;
			animspeed = 1f;
			network.charControllerPhoton.Collider.gameObject.layer = LayerMask.NameToLayer("Character");
			network.charControllerPhoton.SetLayerMask(GameManager.Instance.layerMaskLive);
			playerCollider.SetActive(value: true);
			Physics.SyncTransforms();
			itemCollision = null;
			itemCollisionCollider = null;
			functionItemCollision = "";
			flashlight.SetActive(SceneManager.GetActiveScene().name != "Lobby");
		}
	}

	public void AddStamina(int staminaSprintConsumption, bool recoveryStamina = true, float multiplier = 1f)
	{
		data.AddSubCurrentStamina((float)staminaSprintConsumption * multiplier, recoveryStamina);
		UIGameManager.Instance.barStamina.value = data.GetStamina() / data.GetMaxStamina();
	}

	public void SuperStamina(bool isunlimitedStamina)
	{
		_isNoStamina = isunlimitedStamina;
	}

	public void SetGodMode(bool godMode)
	{
		_isGod = godMode;
	}

	public void SetMaxSpeed(bool maxSpeed)
	{
		_isMaxSpeed = maxSpeed;
	}

	public void SetGhostMode(bool ghostMode)
	{
		_isGhost = ghostMode;
	}

	public void SetActiveDeadIconChar(bool isActive)
	{
		iconCharDeadMap?.gameObject.SetActive(isActive);
	}

	public void SetBtnSprint(bool newIsBtnSprintDown)
	{
		if (newIsBtnSprintDown)
		{
			UIGameManager.Instance.sprintOn.SetActive(value: true);
			UIGameManager.Instance.sprintOff.SetActive(value: false);
		}
		else
		{
			UIGameManager.Instance.sprintOn.SetActive(value: false);
			UIGameManager.Instance.sprintOff.SetActive(value: true);
		}
		isBtnSprintDown = newIsBtnSprintDown;
	}

	public void Disconnected()
	{
		object2D.transform.localScale = new Vector3(0f, 0f, 0f);
		iconCharMap.gameObject.SetActive(value: false);
		if (LobbyManager.Instance != null)
		{
			UIGameManager.Instance.readyUIController?.GetUITabPlayer(network.GetIDX())?.SetDisconnectedUI();
		}
		if (GameManager.Instance != null)
		{
			UIGameManager.Instance.ArrPlayerInfo[network.GetIDX()].gameObject.SetActive(value: false);
			UIGameManager.Instance.ArrPlayerInfo[network.GetIDX()].TextPlayerName.text = "";
		}
		if (network.GetHealth() <= 0f && !isPermadeath)
		{
			reviveArea.enabled = false;
		}
	}

	public void Reconnected()
	{
		object2D.transform.localScale = new Vector3(3f, 3.1f, 1f);
		iconCharMap.gameObject.SetActive(value: true);
		iconCharMap.DOScale(20f, 0f);
		iconCharMapAnimator.Play(data.PlayerSkinData.GetPlayerAvatarSkin());
		if (GameManager.Instance != null)
		{
			UIGameManager.Instance.ArrPlayerInfo[network.GetIDX()].gameObject.SetActive(value: true);
			UIGameManager.Instance.ArrPlayerInfo[network.GetIDX()].TextPlayerName.text = network.GetPlayerName();
		}
		if (LobbyManager.Instance != null)
		{
			PlayerBoard.Instance.boardPlayerList[network.GetIDX()].SetActive(value: true);
			PlayerBoard.Instance.playerNameList[network.GetIDX()].text = network.GetPlayerName();
			UIGameManager.Instance.readyUIController?.GetUITabPlayer(network.GetIDX())?.SetReconnectedUI();
			flashlight.SetActive(value: false);
			PlayerBoard.Instance.ObjectWaiting.SetActive(value: false);
			UIGameManager.Instance.ChangeMiniAvatarReadyStatus(network.GetIDX(), data.PlayerSkinData);
			UIGameManager.Instance.SetPerkSkillUIInfo(network.playerController);
			GameManager.Instance.gameManagerPhoton.arrPlayerReady.Set(0, value: false);
			LobbyManager.Instance.timerCountDown.StopDuration();
			LobbyManager.Instance.allReady = false;
			UIGameManager.Instance.txtCountDown.gameObject.SetActive(value: false);
		}
		if (network.GetHealth() <= 0f && !isPermadeath)
		{
			reviveArea.enabled = true;
		}
		if (inventoryManager == null)
		{
			inventoryManager = GameManager.Instance.GetInventoryPlayerNull(1);
			inventoryManager.player = this;
		}
	}

	public void UpdatePlayerStats()
	{
		if (weaponController.idWeaponRange > 0)
		{
			weaponController.timeAccuracy = 1f * PlayerMultiplyStatsData.GetMultiplyTimerGunAccuracy();
			weaponController.BuffWeaponRange(weaponController.idWeaponRange);
		}
	}

	public void SetAdditionalSpeed(float value)
	{
		timeline.clock.localTimeScale = value;
	}

	private void InitStatsValueDebug()
	{
		if (!GameModes.Instance.isDebug || _statusEffectDebugUIPrefab == null)
		{
			return;
		}
		if (_statsDebugUI == null)
		{
			_statsDebugUI = UnityEngine.Object.Instantiate(_statusEffectDebugUIPrefab, characterRenderController.transform);
		}
		if (network.isLocalPlayer)
		{
			_statsDebugUI.gameObject.SetActive(GameDebug.Instance.ShowStatusEffectDebug);
		}
		else
		{
			_statsDebugUI.gameObject.SetActive(GameDebug.Instance.ShowStatusEffectDebug && GameDebug.Instance.ShowAllPlayerDebug);
		}
		_statsDebugUI.CreateTextDebug("AdditionalPerks = " + string.Join(",", data.SkillData.AdditionalPerkSkillDataList.ToArray()), "AdditionalPerks").gameObject.SetActive(GameDebug.Instance.ShowAllPlayerStatsDebug);
		foreach (PlayerStatsSO listStat in PlayerMultiplyStatsData.ListStats)
		{
			string text = listStat.name;
			_statsDebugUI.CreateTextDebug($"{text} = {listStat.Value}", text).gameObject.SetActive(GameDebug.Instance.ShowAllPlayerStatsDebug);
		}
		_statsDebugUI.CreateTextDebug($"AnotherSpeedModifier = {timeline.clock.timeScale}", "AnotherSpeedModifier").gameObject.SetActive(GameDebug.Instance.ShowAllPlayerStatsDebug);
		_statsDebugUI.CreateTextDebug($"Walk Speed = {data.GetCurrentMoveSpeed() * timeline.clock.timeScale}", "MoveSpeed");
	}

	public void SetActiveModifierStatsDebug(bool active)
	{
		foreach (KeyValuePair<string, TMP_Text> item in StatsDebugUI?.TextDebugDict)
		{
			if (!(item.Key == "MoveSpeed") && item.Value != null)
			{
				item.Value.gameObject.SetActive(active);
			}
		}
	}

	private void UpdateStatsValueDebug(PlayerStatsSO playerStatsSo = null)
	{
		if (!GameModes.Instance.isDebug || !GameDebug.Instance.ShowStatusEffectDebug)
		{
			return;
		}
		Dictionary<string, TMP_Text> dictionary = StatsDebugUI?.TextDebugDict;
		if (dictionary == null)
		{
			return;
		}
		TMP_Text value = null;
		if ((bool)playerStatsSo)
		{
			string text = playerStatsSo.name;
			dictionary.TryGetValue(text, out value);
			if (value != null)
			{
				value.text = $"{text} = {playerStatsSo.Value}";
			}
		}
		dictionary.TryGetValue("AdditionalPerks", out value);
		if ((bool)value)
		{
			value.text = "AdditionalPerks = " + string.Join(",", data.SkillData.AdditionalPerkSkillDataList.ToArray());
		}
		dictionary.TryGetValue("AnotherSpeedModifier", out value);
		if ((bool)value)
		{
			value.text = $"AnotherSpeedModifier = {timeline.clock.timeScale}";
		}
		dictionary.TryGetValue("MoveSpeed", out value);
		if ((bool)value)
		{
			if (isSprinting)
			{
				value.text = "Sprint Speed = " + data.GetCurrentMoveSpeed() * timeline.clock.timeScale;
			}
			else
			{
				value.text = "Walk Speed = " + data.GetCurrentMoveSpeed() * timeline.clock.timeScale;
			}
		}
	}

	private void OnAdditionalPerkChangedAction(string perks)
	{
		UpdateStatsValueDebug();
	}

	public void ForceUpdateStatsValueDebug(PlayerStatsSO playerStatsSo = null)
	{
		if (GameModes.Instance.isDebug)
		{
			UniTaskUtil.DelayedCall(this, 0.53f, () =>
			{
				UpdateStatsValueDebug(playerStatsSo);
			}).Forget();
		}
	}
}

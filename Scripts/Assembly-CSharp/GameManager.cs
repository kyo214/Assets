using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DestroyIt;
using Fusion;
using Toked;
using Toked.Crafting;
using Toked.StatusEffect;
using UGSAnalytics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using _Modules.Cutscene.Scripts;
using _Modules.Map.Scripts;
using _Modules.UILobby.Scripts;

public class GameManager : MonoBehaviour
{
	[SerializeField]
	public List<NetworkPrefabRef> _arrEnemyPrefab = new List<NetworkPrefabRef>();

	[SerializeField]
	public List<NetworkPrefabRef> _arrElitePrefab = new List<NetworkPrefabRef>();

	[SerializeField]
	public List<NetworkPrefabRef> _arrOtherPrefab = new List<NetworkPrefabRef>();

	[SerializeField]
	private NetworkPrefabRef _photonPrefab;

	public GameManagerPhoton gameManagerPhoton;

	public NetworkPrefabRef ItemBoxNetworkPrefab;

	public AstarPath AStarPath;

	public WaveEnemyManager waveManager;

	public XTimer timer;

	public List<PosEnemy> arrInitPosEnemy = new List<PosEnemy>();

	public List<EnemyController> arrEnemyController = new List<EnemyController>();

	public List<TriggerEvent> arrEventTrigger = new List<TriggerEvent>();

	public List<Transform> arrSpriteItemPickable = new List<Transform>();

	public List<ItemPickable> arrItemPickable = new List<ItemPickable>();

	public List<ItemInteractable> arrItemInteractable = new List<ItemInteractable>();

	public List<RoomCollider> arrRoom = new List<RoomCollider>();

	public List<DestructibleObject> arrDestructibleObject = new List<DestructibleObject>();

	public List<InventoryManager> arrInventoryManager = new List<InventoryManager>();

	public List<SoundTriggerController> arrSoundTrigger = new List<SoundTriggerController>();

	public List<GameObject> arrRemovableMeshRendererObj = new List<GameObject>();

	public List<RandomizeItem> ListRandomizeItem = new List<RandomizeItem>();

	[SerializeField]
	private MapManager _mapManager;

	public List<bool> ListPlayerWin = new List<bool>();

	public List<bool> ListPlayerInAreaWin = new List<bool>();

	public Transform targetCrosshair;

	public Transform parentItem;

	public Transform parentInteract;

	public int totEnemySpawn;

	public BGMState BGMMode;

	public string waveType;

	public float incHpEnemyPerWave;

	public float incAttackEnemyPerWave;

	public bool enemyStartChasing;

	public bool electricity;

	public bool quitGame;

	public bool isKicked;

	public bool gameOver;

	public LayerMask layerMaskDead;

	public LayerMask layerGrenade;

	public LayerMask layerMaskLive;

	public LayerMask wallFloorCollider;

	public bool enableRoomFogOfWar;

	public bool isTestMode;

	public bool isHordeMode;

	public bool isInfiniteHordeMode;

	public bool initSyncitemPickableLobby;

	public bool initSyncInteractableLobby;

	public bool initSyncInventory;

	public Material outline;

	public Animator bossAnim;

	public bool IsEliteSpawning;

	public bool IsCutscenePlaying;

	public bool SpawnInitEnemy;

	public bool InitEnemySpawned;

	public List<ItemInteractable> ListBrimCarInteractable = new List<ItemInteractable>();

	public Tweener HealProgressBarTween;

	public static Action<ItemPickable> OnSpawnNewItemFromDrop;

	public Transform BloodPool;

	public Transform MapTransform;

	[SerializeField]
	private PentagramLampController _pentagramLampController;

	[SerializeField]
	private CutsceneTrigger _cutsceneTrigger;

	public static GameManager Instance { get; private set; }

	public MapManager MapManager
	{
		get
		{
			if (_mapManager == null)
			{
				_mapManager = UnityEngine.Object.FindObjectOfType<MapManager>(includeInactive: false);
			}
			return _mapManager;
		}
		set
		{
			_mapManager = value;
		}
	}

	private void Awake()
	{
		if ((bool)GameManagerPhoton.Instance && gameManagerPhoton == null)
		{
			gameManagerPhoton = GameManagerPhoton.Instance;
		}
		if (SceneManager.GetActiveScene().name != "Lobby" && NetworkGameManager.Instance != null)
		{
			for (int i = 0; i < NetworkGameManager.Instance.arrPlayerController.Count; i++)
			{
				NetworkGameManager.Instance.arrPlayerController[i].initPos = true;
			}
		}
		if (PhotonMultiplayerManager.Instance != null)
		{
			_ = PhotonMultiplayerManager.Instance._runner != null;
		}
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
		waveManager = GetComponent<WaveEnemyManager>();
		if ((bool)GameManagerPhoton.Instance)
		{
			if (GameManagerPhoton.Instance.CurrentMission.MissionObjective.IsSpawnEndlessHordeFromBeginning)
			{
				GameModes.Instance.modeGame = "Defense";
			}
			if (GameManagerPhoton.Instance.CurrentMission.MissionObjective.IsPVP)
			{
				GameModes.Instance.modeGame = "PVP";
				GameModes.Instance.friendlyFire = true;
				GameModes.Instance.isGrenadeFriendlyFire = true;
			}
		}
		ItemInteractable[] array = UnityEngine.Object.FindObjectsOfType<ItemInteractable>(includeInactive: true);
		foreach (ItemInteractable itemInteractable in array)
		{
			if (itemInteractable.functionInteract != "Revive")
			{
				arrItemInteractable.Add(itemInteractable);
			}
		}
		arrItemInteractable.Sort((ItemInteractable p1, ItemInteractable p2) => p1.UniqueID.CompareTo(p2.UniqueID));
	}

	public void Start()
	{
		if (isTestMode)
		{
			SpawnInitEnemy = true;
		}
		GameModes.Instance.Init();
		StartCoroutine(GameModes.Instance.InitGameModeSettings());
		if (AudioManager.GetPlaylistName() != "BGM" && AudioManager.GetPlaylistName() != "Lobby" && !LobbyManager.Instance)
		{
			AudioManager.PlayBGM("BGM", "Empty");
		}
		quitGame = false;
		gameOver = false;
		if ((bool)NetworkGameManager.Instance.ownPlayer)
		{
			PlayerController ownPlayer = NetworkGameManager.Instance.ownPlayer;
			ownPlayer.audioListener.enabled = true;
			ownPlayer.audioListener.transform.localPosition = new Vector3(ownPlayer.audioListener.transform.localPosition.x, 0.325f, ownPlayer.audioListener.transform.localPosition.z);
		}
		if (SceneManager.GetActiveScene().name != "Lobby")
		{
			if (NetworkGameManager.Instance.isServer)
			{
				Dictionary<string, SessionProperty> customProperties = new Dictionary<string, SessionProperty> { ["status"] = "Close" };
				PhotonMultiplayerManager.Instance._runner.SessionInfo.UpdateCustomProperties(customProperties);
			}
			for (int i = 0; i < NetworkGameManager.Instance.arrPlayerNetworkController.Count; i++)
			{
				if ((bool)NetworkGameManager.Instance.arrPlayerNetworkController[i])
				{
					NetworkGameManager.Instance.arrPlayerNetworkController[i].ScorePlayerNetwork.ResetScorePerMission();
				}
			}
			if (NetworkGameManager.Instance.isServer)
			{
				Debug.Log("Scene Loaded" + PhotonMultiplayerManager.Instance.sceneLoaded);
				if (PhotonMultiplayerManager.Instance.sceneLoaded)
				{
					SpawnPhotonGameManager();
				}
				for (int j = 0; j < NetworkGameManager.Instance.arrPlayerController.Count; j++)
				{
					NetworkGameManager.Instance.arrPlayerController[j].network.SetInGame(value: true);
					NetworkGameManager.Instance.arrPlayerController[j].ScorePlayerNetwork.ResetScorePerMission();
					NetworkGameManager.Instance.arrPlayerController[j].network.playerPhoton.IsSurvive = false;
				}
				if (Instance.gameManagerPhoton != null)
				{
					Instance.gameManagerPhoton.arrPlayerReady.Set(0, value: false);
					Instance.gameManagerPhoton.arrPlayerReady.Set(1, value: false);
				}
			}
			AudioManager.StopBGM();
			foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerNetworkController)
			{
				if (!item)
				{
					continue;
				}
				if (item.network.isLocalPlayer)
				{
					NetworkGameManager.Instance.ownPlayer.InitPlayerInGame();
				}
				else
				{
					item.InitOtherPlayerInGame();
					if ((bool)item.network.playerPhoton.IsDisconnected || string.IsNullOrWhiteSpace(item.data.SkillData.PerkId))
					{
						item.Disconnected();
					}
				}
				item.network.playerPhoton.reviveTimer = 90;
			}
			incHpEnemyPerWave = BGDatabase_GameConfig.GetEntity(GameModes.Instance.modeGame).IncHpEnemyPerWave;
			incAttackEnemyPerWave = BGDatabase_GameConfig.GetEntity(GameModes.Instance.modeGame).IncAttackEnemyPerWave;
			waveType = BGDatabase_GameConfig.GetEntityByKeyid(GameModes.Instance.modeGame).WaveType;
			GameModes.Instance.chancePercentDropScraps = BGDatabase_GameConfig.GetEntity(GameModes.Instance.modeGame).ChanceDropScraps;
			GameModes.Instance.chancePercentDropGunPowder = BGDatabase_GameConfig.GetEntity(GameModes.Instance.modeGame).ChanceDropGunPowder;
			GameModes.Instance.chancePercentDropChemical = BGDatabase_GameConfig.GetEntity(GameModes.Instance.modeGame).ChanceDropChemical;
			Instance.BGMMode = BGMState.Battle;
			AudioManager.PlayAmbient("Empty");
			List<string> list = new List<string>();
			foreach (PlayerController item2 in NetworkGameManager.Instance.arrPlayerController)
			{
				list.Add(item2.network.GetPlayerName());
			}
			if (list.Count > 0)
			{
				SendWMO.GameLobby(list);
			}
		}
		if ((bool)_pentagramLampController)
		{
			StartCoroutine(CheckPentagramLife());
		}
		if (!GameModes.Instance.isDebug)
		{
			return;
		}
		foreach (PlayerController item3 in NetworkGameManager.Instance.arrPlayerNetworkController)
		{
			if (item3 != null)
			{
				item3.network.charControllerPhoton.ExludeLayerCharCollider(GameDebug.Instance.wallColliderMask);
			}
		}
	}

	private IEnumerator CheckPentagramLife()
	{
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		_pentagramLampController.Init(gameManagerPhoton.Life);
	}

	public void NetworkUpdate()
	{
		if (NetworkGameManager.Instance.isReconnecting && NetworkGameManager.Instance.ownPlayer != null)
		{
			NetworkGameManager.Instance.isSyncingMissionMap = true;
			UniTaskUtil.DelayedCall(this, 1f, () =>
			{
				if (NetworkGameManager.Instance.ownPlayer.network.playerPhoton.MaxInventorySlot > 0)
				{
					NetworkGameManager.Instance.ownPlayer.data.SetMaxInventoryLocal(NetworkGameManager.Instance.ownPlayer.network.playerPhoton.MaxInventorySlot);
				}
				NetworkGameManager.Instance.ownPlayer.network.playerPhoton.RPCRequestSync();
				NetworkGameManager.Instance.ownPlayer.network.SetTargetIdxCamTarget(NetworkGameManager.Instance.ownPlayer.network.GetIDX());
			}).Forget();
			NetworkGameManager.Instance.isReconnecting = false;
		}
		int num = 0;
		if ((bool)MissionManager.Instance && MissionManager.Instance.IsTimerCountdownMode && MissionManager.Instance.TimerCountdown > 0)
		{
			num = MissionManager.Instance.TimerCountdown - Mathf.FloorToInt(timer.interval);
			if (num == 30)
			{
				UIGameManager.Instance.txtTime.color = Color.red;
			}
			if (num <= 30)
			{
				UIGameManager.Instance.txtTime.transform.localPosition = new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-1f, 3f), 0f);
			}
			else if (num <= 60)
			{
				UIGameManager.Instance.txtTime.transform.localPosition = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(0f, 2f), 0f);
			}
			if (num == 49 && NetworkGameManager.Instance.isServer)
			{
				gameManagerPhoton.RpcExecAlertHorde();
				if (waveManager.levelHorde < 3)
				{
					waveManager.levelHorde = 3;
					waveManager.InitHorde();
				}
			}
			if (num == 0)
			{
				UIGameManager.Instance.txtTime.text = MathFunc.GetMinute(num).ToString("00") + ":" + MathFunc.GetSecond(num).ToString("00");
				isInfiniteHordeMode = true;
				UIGameManager.Instance.uiHordeIncoming.SetActive(value: true);
				UIGameManager.Instance.LabelHordeInfiniteIncoming.SetActive(value: true);
				UIGameManager.Instance.LabelHordeIncoming.SetActive(value: false);
			}
			if (!isInfiniteHordeMode)
			{
				if (num >= 0)
				{
					UIGameManager.Instance.txtTime.text = MathFunc.GetMinute(num).ToString("00") + ":" + MathFunc.GetSecond(num).ToString("00");
				}
				else if (num < 0 && !isInfiniteHordeMode)
				{
					Debug.Log("Timer is running = " + timer.isRunning);
					UIGameManager.Instance.txtTime.color = Color.red;
					UIGameManager.Instance.txtTime.text = MathFunc.GetMinute(0f).ToString("00") + ":" + MathFunc.GetSecond(0f).ToString("00");
					isInfiniteHordeMode = true;
					UIGameManager.Instance.uiHordeIncoming.SetActive(value: true);
					UIGameManager.Instance.LabelHordeInfiniteIncoming.SetActive(value: true);
					UIGameManager.Instance.LabelHordeIncoming.SetActive(value: false);
					CameraGame.Instance.SetColorAdjustmentEffect(new Color(1f, 0.85f, 0.85f), 0f);
					BloodPool.gameObject.SetActive(value: true);
					BloodPool.DOScale(new Vector3(100f, 0.01f, 100f), 0f);
				}
			}
		}
		if (NetworkGameManager.Instance.isServer && NetworkGameManager.Instance.ownPlayer != null && SpawnInitEnemy && !InitEnemySpawned)
		{
			SpawnInitEnemy = false;
			InitEnemySpawned = true;
			foreach (PosEnemy item in arrInitPosEnemy.ToList())
			{
				if (item == null || !item.gameObject.activeSelf)
				{
					arrInitPosEnemy.Remove(item);
				}
				else
				{
					if (item.posType != PosEnemy.PosType.PosInitZombie && item.posType != PosEnemy.PosType.PosInitElite && item.posType != PosEnemy.PosType.PosInitFloatingZombie && item.posType != PosEnemy.PosType.PosInitFakeDeadZombie && item.posType != PosEnemy.PosType.PosOtherPrefab)
					{
						continue;
					}
					GameObject gameObject = null;
					if (item.posType == PosEnemy.PosType.PosInitElite)
					{
						gameObject = SpawnEnemyPhoton(item, item.eliteType);
					}
					else if (item.posType == PosEnemy.PosType.PosOtherPrefab)
					{
						Debug.Log("Spawn Kucing");
						gameObject = SpawnEnemyPhoton(item, (int)item.OtherType, isOtherType: true);
					}
					else
					{
						gameObject = SpawnEnemyPhoton(item, (int)item.ZombieType);
					}
					EnemyController component = gameObject.gameObject.GetComponent<EnemyController>();
					if (item.posType != PosEnemy.PosType.PosInitElite && item.posType != PosEnemy.PosType.PosOtherPrefab)
					{
						component.data.type = (int)item.ZombieType;
					}
					component.isAlwaysChasing = item.isAlwaysChasing;
					component.object2D.transform.DOLocalRotate(new Vector3(0f, CameraGame.Instance.camRotate, 0f), 0.02f);
					component.network.networkPhoton.isDeaf = item.isDeaf;
					component.network.networkPhoton.isDisableCollider = false;
					component.isMoveable = item.IsMoveable;
					if (!component.isMoveable)
					{
						component.movement.timerChangeState.StopDuration();
						component.attack.fov.enabled = false;
						component.network.SetAnimation("Idle" + component.data.arrWeaponState[component.data.weaponState] + (UnityEngine.Random.Range(0, 4) * 90 + 45));
					}
					if (item.posType == PosEnemy.PosType.PosInitFakeDeadZombie)
					{
						component.isFakeDead = true;
						component.movement.timerChangeState.StopDuration();
						component.attack.fov.enabled = false;
						if (item.InitAngleEnemy > 0)
						{
							component.movement.angleAnim = component.movement.SetAngleByCam(item.InitAngleEnemy);
						}
						else
						{
							component.movement.angleAnim = UnityEngine.Random.Range(0, 4) * 90 + 45;
						}
						component.network.SetAnimation("Dead2Melee" + component.movement.angleAnim);
						component.transform.position = new Vector3(component.transform.position.x, 0f, component.transform.position.z);
						component.object2D.position = component.transform.position;
					}
					if (item.posType == PosEnemy.PosType.PosInitFloatingZombie)
					{
						component.SetState(EnemyState.Hovering);
						component.network.SetIsHovering(value: true);
					}
					item.lastEnemySpawned = component;
					item.IsMoveable = true;
					if (item.isDisableCollider)
					{
						component.network.networkPhoton.isDisableCollider = true;
					}
				}
			}
		}
		waveManager.UpdateNetwork();
		if (UIGameManager.Instance != null && UIGameManager.Instance.txtTimer.isActiveAndEnabled)
		{
			if (MathFunc.GetMinute(waveManager.waveTimer.interval) < 99)
			{
				UIGameManager.Instance.txtTimer.text = MathFunc.GetMinute(waveManager.waveTimer.interval).ToString("00") + ":" + MathFunc.GetSecond(waveManager.waveTimer.interval).ToString("00");
			}
			else
			{
				UIGameManager.Instance.txtTimer.text = MathFunc.GetMinute(waveManager.waveTimer.interval) + ":" + MathFunc.GetSecond(waveManager.waveTimer.interval).ToString("00");
			}
		}
	}

	public EnemyController GetEnemy(byte idx)
	{
		EnemyController result = null;
		for (int i = 0; i < arrEnemyController.Count; i++)
		{
			if (arrEnemyController[i].network.GetIDX() == idx)
			{
				result = arrEnemyController[i];
				break;
			}
		}
		return result;
	}

	public int GetTotEnemyActive()
	{
		int num = 0;
		foreach (EnemyController item in Instance.arrEnemyController)
		{
			if (!item.network.IsNonActive())
			{
				num++;
			}
		}
		return num;
	}

	public InventoryManager GetInventoryPlayerNull(int startFrom)
	{
		InventoryManager result = null;
		for (int i = startFrom; i < arrInventoryManager.Count; i++)
		{
			if (arrInventoryManager[i].player == null)
			{
				result = arrInventoryManager[i];
				break;
			}
		}
		return result;
	}

	public ItemPickable GetItemPickable(int uniqueID, int itemID = -1)
	{
		ItemPickable result = null;
		for (int num = arrItemPickable.Count - 1; num >= 0; num--)
		{
			ItemPickable itemPickable = arrItemPickable[num];
			if (itemPickable.uniqueID == uniqueID)
			{
				if (itemPickable.itemID == itemID)
				{
					result = itemPickable;
					break;
				}
				if (itemID < 0)
				{
					result = itemPickable;
					break;
				}
			}
		}
		return result;
	}

	public ItemInteractable GetItemInteractable(int uniqueID, bool itemOnly = false)
	{
		ItemInteractable result = null;
		if (uniqueID >= 10000)
		{
			if (!itemOnly)
			{
				result = ((uniqueID < 10010) ? NetworkGameManager.Instance.GetPlayer(uniqueID - 10000).reviveArea.gameObject.GetComponent<ItemInteractable>() : NetworkGameManager.Instance.GetPlayer(uniqueID - 10010).healArea.gameObject.GetComponent<ItemInteractable>());
			}
		}
		else
		{
			foreach (ItemInteractable item in Instance.arrItemInteractable)
			{
				if (item.UniqueID == uniqueID)
				{
					result = item;
					break;
				}
			}
		}
		return result;
	}

	public GameObject SpawnEnemyPhoton(PosEnemy posEnemy = null, int type = 0, bool isOtherType = false, Vector3 spawnPos = default(Vector3))
	{
		NetworkObject networkObject = null;
		if ((bool)posEnemy)
		{
			spawnPos = posEnemy.transform.position;
		}
		if (NetworkGameManager.Instance.photonNetworking._runner != null && NetworkGameManager.Instance.photonNetworking._runner != null)
		{
			if ((bool)posEnemy)
			{
				if (posEnemy.posType == PosEnemy.PosType.PosInitElite || posEnemy.posType == PosEnemy.PosType.PosEliteHorde)
				{
					networkObject = ((type != 200) ? NetworkGameManager.Instance.photonNetworking._runner.Spawn(_arrElitePrefab[type - 100], posEnemy.transform.position, Quaternion.identity, NetworkGameManager.Instance.photonNetworking._runner.LocalPlayer) : NetworkGameManager.Instance.photonNetworking._runner.Spawn(_arrElitePrefab[2], posEnemy.transform.position, Quaternion.identity, NetworkGameManager.Instance.photonNetworking._runner.LocalPlayer));
				}
				else if (isOtherType)
				{
					networkObject = NetworkGameManager.Instance.photonNetworking._runner.Spawn(_arrOtherPrefab[type], posEnemy.transform.position, Quaternion.identity, NetworkGameManager.Instance.photonNetworking._runner.LocalPlayer);
				}
				else
				{
					int num = UnityEngine.Random.Range(0, 100);
					networkObject = (((float)num < GlobalMissionManager.Instance.ModExplodingZombie.CurrentValue) ? NetworkGameManager.Instance.photonNetworking._runner.Spawn(_arrEnemyPrefab[1], posEnemy.transform.position, Quaternion.identity, NetworkGameManager.Instance.photonNetworking._runner.LocalPlayer) : ((!((float)num - GlobalMissionManager.Instance.ModExplodingZombie.CurrentValue < GlobalMissionManager.Instance.ModToxinZombie.CurrentValue)) ? NetworkGameManager.Instance.photonNetworking._runner.Spawn(_arrEnemyPrefab[0], posEnemy.transform.position, Quaternion.identity, NetworkGameManager.Instance.photonNetworking._runner.LocalPlayer) : NetworkGameManager.Instance.photonNetworking._runner.Spawn(_arrEnemyPrefab[2], posEnemy.transform.position, Quaternion.identity, NetworkGameManager.Instance.photonNetworking._runner.LocalPlayer)));
				}
			}
			else
			{
				networkObject = NetworkGameManager.Instance.photonNetworking._runner.Spawn(_arrElitePrefab[type - 100], spawnPos, Quaternion.identity, NetworkGameManager.Instance.photonNetworking._runner.LocalPlayer);
			}
		}
		return networkObject.gameObject;
	}

	public void SpawnPhotonGameManager()
	{
		if (gameManagerPhoton == null)
		{
			NetworkGameManager.Instance.photonNetworking._runner.Spawn(_photonPrefab, Vector3.zero, Quaternion.identity, NetworkGameManager.Instance.photonNetworking._runner.LocalPlayer);
		}
	}

	public void SetActiveEnemy(bool setActive)
	{
		foreach (EnemyController item in arrEnemyController)
		{
			item?.SetEnableAI(setActive);
		}
	}

	public void DropItem(int idx, byte amount, byte itemValue, Vector3 pos, int idxItem, bool isSpreading = false, PlayerController fromPlayer = null, int idxInventoryPlayer = -1, bool isFading = false, bool isRemoveFromLocalPlayer = true)
	{
		byte ammo = 0;
		byte durability = 0;
		BGDatabase_Item entityByKeyid = BGDatabase_Item.GetEntityByKeyid(idx);
		if (entityByKeyid != null && entityByKeyid.Durability > 0)
		{
			durability = itemValue;
		}
		else
		{
			ammo = itemValue;
		}
		if (idxItem == -1 && idx != -1)
		{
			SpawnNewItem(idx, pos, isSpreading, amount, ammo, -1, isActive: true, isFading, isVisibleMap: true, durability);
		}
		else
		{
			ItemPickable t = Instance.GetItemPickable(idxItem, idx);
			if (t != null)
			{
				t.itemCollider.enabled = true;
				t.rigidbody.useGravity = true;
				t.rigidbody.isKinematic = false;
				if (t.DropToInitPos)
				{
					t.rigidbody.useGravity = false;
					t.rigidbody.isKinematic = true;
					t.transform.parent.DOJump(t.InitParentPosition, 1f, 1, 0.5f);
				}
				else if (isSpreading)
				{
					UnityEngine.Random.InitState(t.uniqueID + t.itemID);
					float x = UnityEngine.Random.Range(-0.7f, 0.7f);
					float z = UnityEngine.Random.Range(-0.7f, 0.7f);
					UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
					t.rigidbody.velocity = new Vector3(x, 0f, z).normalized * 2.15f;
				}
				else
				{
					t.rigidbody.velocity = new Vector3(0f, 4f, 0f);
				}
				t.itemCollider.enabled = true;
				t.SetSpriteEnable(value: true);
				t.transform.parent.position = pos;
				t.itemSprite.transform.parent.DOLocalRotate(new Vector3(0f, CameraGame.Instance.camRotate, 0f), 0f);
				t.amount = amount;
				if (t.itemID >= 400)
				{
					string text = t.itemID.ToString();
					t.itemType = "Material";
					t.itemName = DataManager.Instance.GetValueDatabase<string>("Item", text, "Name");
					CraftMaterialScriptableObject craftMaterialScriptableObject = DataManager.Instance.Get<CraftMaterialLibraryScriptableObject>().FindByItemIdKey(text);
					Vector2 vector = (craftMaterialScriptableObject ? craftMaterialScriptableObject.MinMaxDropAmount : new Vector2(1f, 1f));
					t.amount = (byte)UnityEngine.Random.Range(vector.x, vector.y);
				}
				t.ammo = ammo;
				t.durability = durability;
				foreach (RoomCollider item in Instance.arrRoom)
				{
					for (int i = 0; i < item.boxColliders.Count; i++)
					{
						Bounds bounds = item.boxColliders[i].bounds;
						bounds.Expand(1.5f);
						if (bounds.Contains(t.transform.position))
						{
							if (item.itemList.Find((ItemPickable itemPickable) => itemPickable.uniqueID == t.uniqueID) == null)
							{
								item.itemList.Add(t);
								t.roomCollider = item;
							}
							else
							{
								t.roomCollider = item;
							}
						}
					}
				}
				if (isFading)
				{
					DOVirtual.DelayedCall(10f, () =>
					{
						t.SetDisableObject(isFading: true);
					}).SetId("FadeItem" + t.uniqueID);
				}
			}
			else
			{
				SpawnNewItem(idx, pos, isSpreading, amount, ammo, idxItem, isActive: true, isFading: false, isVisibleMap: true, durability);
			}
		}
		if (!(fromPlayer != null) || idxInventoryPlayer == -1)
		{
			return;
		}
		fromPlayer.data.arrInventory[idxInventoryPlayer].Amount -= amount;
		if ((idxInventoryPlayer <= 1 || fromPlayer.data.arrInventory[idxInventoryPlayer].Amount <= 0) && (isRemoveFromLocalPlayer || !fromPlayer.network.isLocalPlayer))
		{
			fromPlayer.data.RemoveInventoryOtherPlayer(idxInventoryPlayer);
		}
		if ((bool)fromPlayer.inventoryManager && (bool)fromPlayer.inventoryManager.txtAmountList[idxInventoryPlayer] && fromPlayer.inventoryManager.txtAmountList[idxInventoryPlayer].gameObject.activeSelf)
		{
			if (fromPlayer.data.arrInventory[idxInventoryPlayer].Amount > 0)
			{
				fromPlayer.inventoryManager.txtAmountList[idxInventoryPlayer].text = fromPlayer.data.arrInventory[idxInventoryPlayer].Amount.ToString();
			}
			else
			{
				fromPlayer.inventoryManager.txtAmountList[idxInventoryPlayer].text = "";
				fromPlayer.inventoryManager.ammoIconList[idxInventoryPlayer].gameObject.SetActive(value: false);
			}
		}
		if (!UIGameManager.Instance.uiInventory.isHidden)
		{
			NetworkGameManager.Instance.ownPlayer.InitPlayerInventoryBoard();
		}
	}

	public void SpawnNewItem(int idx, Vector3 pos, bool isSpreading, byte amount, byte ammo, int uid = -1, bool isActive = true, bool isFading = false, bool isVisibleMap = true, int durability = -1)
	{
		ItemScriptableObject itemData = DataManager.Instance.GetItemData(idx.ToString());
		if (itemData == null)
		{
			return;
		}
		GameObject itemPrefab = itemData.ItemPrefab;
		if (itemPrefab == null)
		{
			return;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(itemPrefab, parentItem);
		gameObject.transform.position = pos;
		ItemPickable itemPick = gameObject.GetComponentInChildren<ItemPickable>(includeInactive: false);
		if (uid != -1)
		{
			itemPick.uniqueID = uid;
		}
		else
		{
			itemPick.uniqueID = -1;
		}
		Instance.arrItemPickable.Add(itemPick);
		Instance.arrItemPickable.Sort((ItemPickable p1, ItemPickable p2) => p1.uniqueID.CompareTo(p2.uniqueID));
		itemPick.rigidbody.useGravity = true;
		itemPick.rigidbody.isKinematic = false;
		if (isSpreading)
		{
			UniTaskUtil.DelayedCall(this, 0.1f, () =>
			{
				UnityEngine.Random.InitState(itemPick.uniqueID + itemPick.itemID);
				float x = UnityEngine.Random.Range(-0.7f, 0.7f);
				float z = UnityEngine.Random.Range(-0.7f, 0.7f);
				UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
				itemPick.rigidbody.velocity = new Vector3(x, 0f, z).normalized * 2.15f;
			}).Forget();
		}
		else
		{
			itemPick.rigidbody.velocity = new Vector3(0f, 4f, 0f);
		}
		itemPick.SetSpriteEnable(value: true);
		if (itemPick.itemID < 200)
		{
			itemPick.ammo = ammo;
		}
		if (itemPick.itemID >= 100 && itemPick.itemID < 200)
		{
			itemPick.amount = amount;
		}
		else if (itemPick.itemID >= 200)
		{
			itemPick.amount = amount;
		}
		UniTaskUtil.DelayedCall(this, 1f, () =>
		{
			itemPick.durability = durability;
		}).Forget();
		itemPick.rigidbody.transform.GetChild(0).DOLocalRotate(new Vector3(0f, CameraGame.Instance.camRotate, 0f), 0f);
		foreach (RoomCollider item in Instance.arrRoom)
		{
			List<BoxCollider> boxColliders = item.boxColliders;
			for (int num = 0; num < boxColliders.Count; num++)
			{
				Bounds bounds = boxColliders[num].bounds;
				bounds.Expand(1.5f);
				if (bounds.Contains(base.transform.position))
				{
					item.itemList.Add(itemPick);
					itemPick.roomCollider = item;
				}
			}
		}
		if (itemPick.itemID > 0)
		{
			itemPick.itemMap.sprite = itemData.ItemSprite;
			if (GlobalSaveData.instance.optionData.autoMinimap == 1)
			{
				if (itemPick.itemMap != null)
				{
					itemPick.itemMap.transform.DOLocalRotate(new Vector3(90f, 0f, -CameraGame.Instance.camRotate), 0f);
				}
			}
			else if (itemPick.itemMap != null)
			{
				itemPick.itemMap.transform.DOLocalRotate(new Vector3(90f, 0f, 0f), 0f);
			}
			itemPick.itemMap.enabled = isVisibleMap;
		}
		if (!isActive)
		{
			UniTaskUtil.DelayedCall(this, 0.1f, () =>
			{
				if (itemPick.itemCollider != null)
				{
					itemPick.itemCollider.enabled = false;
				}
				itemPick.SetSpriteEnable(value: false);
				itemPick.OnRemoveObjectCustomFunction?.Execute();
			}).Forget();
		}
		if (isFading)
		{
			DOVirtual.DelayedCall(10f, () =>
			{
				itemPick.SetDisableObject(isFading: true);
			}).SetId("FadeItem" + itemPick.uniqueID);
		}
		itemPick.IsSpawnedFromObject = true;
		OnSpawnNewItemFromDrop?.Invoke(itemPick);
	}

	public void ItemInteract(short uniqueID, byte idxPlayer, bool triggerOnReverse, bool isForceInteract = false)
	{
		ItemInteractable itemInteractable = GetItemInteractable(uniqueID);
		if (!itemInteractable)
		{
			return;
		}
		PlayerController playerController = null;
		if ((bool)itemInteractable.transform.parent)
		{
			playerController = itemInteractable.transform.parent.GetComponent<PlayerController>();
		}
		PlayerController player = NetworkGameManager.Instance.GetPlayer(idxPlayer);
		if (itemInteractable.IsPuzzle && itemInteractable.listItemToActivate.Count == 0)
		{
			if (NetworkGameManager.Instance.isServer && !itemInteractable.IsSolved && (bool)player)
			{
				player.ScorePlayerNetwork.IncreasePuzzleSolved();
			}
			itemInteractable.IsSolved = true;
			if (itemInteractable.isLocked)
			{
				itemInteractable.isLocked = false;
				itemInteractable.lockMap.SetActive(value: false);
			}
			CheckRoomMap(player, itemInteractable.RoomColliderItem);
		}
		if (isForceInteract)
		{
			itemInteractable.listItemToActivate.Clear();
		}
		if (itemInteractable.functionInteract == "TriggerHorde" && !isHordeMode)
		{
			waveManager.cueHordeTimer.StartDuration(0.1f);
			UniTaskUtil.DelayedCall(this, 0.5f, () =>
			{
				waveManager.buildUpHordeTimer.StartDuration(0.1f);
			}, ignoreTimeScale: false).Forget();
			UniTaskUtil.DelayedCall(this, 1f, () =>
			{
				waveManager.hordeTimer.StartDuration(0.1f);
			}, ignoreTimeScale: false).Forget();
		}
		if (itemInteractable.isNeedProgress)
		{
			foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerController)
			{
				if (item.network.GetHealth() > 0f && (itemInteractable.fromPlayer == item || item.itemCollision == itemInteractable.gameObject))
				{
					UIGameManager.Instance.ArrPlayerInfo[item.network.GetIDX()].ProgressBarObject.SetActive(value: false);
					UIGameManager.Instance.ArrPlayerInfo[item.network.GetIDX()].HealBarObject.SetActive(value: false);
					itemInteractable.labelItemCommandOff = false;
					itemInteractable.isProgressing = false;
					item.enableMoveChar = true;
					item.fsmUpperBody.SetBool("isReviving", value: false);
					item.itemCollision = null;
					item.functionItemCollision = "";
					itemInteractable.fromPlayer = null;
				}
			}
			if (itemInteractable.functionInteract == "Revive")
			{
				if (playerController.network.isLocalPlayer)
				{
					CameraGame.Instance.RemoveAllMember();
					foreach (PlayerController item2 in NetworkGameManager.Instance.arrPlayerController)
					{
						item2.audioListener.enabled = false;
						item2.fov.enabled = false;
					}
					CameraGame.Instance.CinemachineTarget.AddMember(CameraGame.Instance.targetCursor, 0.5f, 2f);
					ChangeSpectator(playerController.network.GetIDX(), playerController.TargetIdxCamBeforeRevive);
					playerController.audioListener.enabled = true;
					playerController.fov.enabled = true;
					UIGameManager.Instance.spectateObject.SetActive(value: false);
					UIGameManager.Instance.uIInGameController.SetPlayerStatusUI(setActive: true);
					UIGameManager.Instance.uIInGameController.SetInventoryStatusUI(setActive: true);
				}
				AudioManager.StopSFXTransform(playerController.transform);
				playerController.enableMoveChar = true;
				playerController.flashlight.SetActive(value: true);
				playerController.reviveArea.enabled = false;
				playerController.shadow.color = new Color(playerController.shadow.color.r, playerController.shadow.color.g, playerController.shadow.color.b, 0.7f);
				playerController.network.charControllerPhoton.SetLayerMask(Instance.layerMaskLive);
				playerController.network.charControllerPhoton.Collider.gameObject.layer = LayerMask.NameToLayer("Character");
				playerController.playerCollider.SetActive(value: true);
				playerController.SetAnimUpperSpeed(1f);
				playerController.isPermadeath = false;
				if (NetworkGameManager.Instance.isServer)
				{
					player.ScorePlayerNetwork.IncreaseReviveOther();
					playerController.network.playerPhoton.health = (short)Mathf.RoundToInt((float)(player.network.playerPhoton.healingValue * 100) * playerController.PlayerMultiplyStatsData.GetMultiplyHealthRestored());
					playerController.network.playerPhoton.targetIdxCam = playerController.network.GetIDX();
				}
				playerController.data.SetCurrentMoveSpeed(playerController.data.GetInitialMoveSpeed());
			}
			else if (itemInteractable.functionInteract == "HealOther" && (bool)playerController)
			{
				playerController.healArea.enabled = false;
				if (playerController.network.isLocalPlayer)
				{
					UIGameManager.Instance.flashGreen.enabled = true;
					UIGameManager.Instance.flashGreen.DOKill();
					UIGameManager.Instance.flashGreen.DOFade(0.1f, 0f);
					UIGameManager.Instance.flashGreen.DOFade(0f, 0.6f).SetDelay(0.03f).OnComplete(() =>
					{
						UIGameManager.Instance.flashGreen.enabled = false;
					});
				}
				if (NetworkGameManager.Instance.isServer)
				{
					short health = playerController.network.playerPhoton.health;
					health += (short)(player.network.playerPhoton.healingValue * 100);
					if ((float)health >= playerController.data.GetMaxHealth() * 100f)
					{
						health = (short)(playerController.data.GetMaxHealth() * 100f);
					}
					playerController.network.playerPhoton.health = health;
				}
				itemInteractable.timerDelay.StartDuration(5f);
			}
			else if (itemInteractable.functionInteract == "LockPick")
			{
				itemInteractable.TriggerAnimation(player.network.isLocalPlayer);
				itemInteractable.DisableCollider();
			}
			else if (itemInteractable.functionInteract == "Barricade")
			{
				itemInteractable.TriggerAnimation(player.network.isLocalPlayer, player);
				itemInteractable.DisableCollider();
			}
			else if (itemInteractable.functionInteract == "RepairCar")
			{
				if (ChatSystem.Instance.timerCountdown.interval > 0f)
				{
					itemInteractable.TriggerAnimation(player.network.isLocalPlayer, player);
					float num = ChatSystem.Instance.timerCountdown.interval - 100f;
					if (num <= 0f)
					{
						num = 0.1f;
					}
					DOTween.To(() => ChatSystem.Instance.timerCountdown.interval, (float x) =>
					{
						ChatSystem.Instance.timerCountdown.interval = x;
					}, num, 0.3f).OnComplete(() =>
					{
						if (NetworkGameManager.Instance.isServer)
						{
							gameManagerPhoton.RpcSyncTimerCountdown(ChatSystem.Instance.timerCountdown.interval);
						}
					});
				}
			}
			else if (itemInteractable.functionInteract == "NeedItem")
			{
				itemInteractable.TriggerAnimation(player.network.isLocalPlayer, player);
				if (itemInteractable.isNoNeedItem(player, withChat: false))
				{
					itemInteractable.DisableCollider();
				}
				RoomCollider roomCollider = GetRoomCollider(player.RoomName);
				if ((bool)roomCollider)
				{
					roomCollider.CheckMap(player);
				}
			}
			else
			{
				itemInteractable.TriggerAnimation(player.network.isLocalPlayer, player);
				if (itemInteractable.functionInteract != "Pet")
				{
					itemInteractable.DisableCollider();
				}
			}
			ChatSystem.Instance.ItemCommand.SetActive(value: false);
			return;
		}
		itemInteractable.triggerOnReverse = triggerOnReverse;
		if (itemInteractable.functionInteract == "NeedItem")
		{
			itemInteractable.TriggerAnimation(player.network.isLocalPlayer, player, playSFX: true, 1f, noTriggerReverse: false, isForceInteract);
			if (itemInteractable.isNoNeedItem(player, withChat: false) && !itemInteractable.isShowUI && !itemInteractable.isTriggerReverse)
			{
				itemInteractable.DisableCollider();
			}
		}
		else
		{
			itemInteractable.TriggerAnimation(player.network.isLocalPlayer, player);
		}
	}

	public void UnlockItem(byte uniqueID)
	{
		ItemInteractable itemInteractable = GetItemInteractable(uniqueID);
		itemInteractable.isLocked = false;
		itemInteractable.isTriggered = true;
		if (itemInteractable.doorCollider != null)
		{
			itemInteractable.doorCollider.transform.gameObject.layer = 22;
			Instance.AStarPath.UpdateGraphs(itemInteractable.doorCollider.bounds);
		}
		Instance.AStarPath.FlushGraphUpdates();
	}

	public void StartProgressInteract(short uniqueID, byte playerID)
	{
		ItemInteractable itemInteractable = GetItemInteractable(uniqueID);
		if (itemInteractable == null)
		{
			Debug.LogWarning($"Item {uniqueID} not found");
		}
		if (!itemInteractable || !itemInteractable.isNeedProgress)
		{
			return;
		}
		PlayerController player = NetworkGameManager.Instance.GetPlayer(playerID);
		if (!itemInteractable.isProgressing)
		{
			itemInteractable.isProgressing = true;
			itemInteractable.labelItemCommandOff = true;
			ChatSystem.Instance.ItemCommand.gameObject.SetActive(value: false);
			UIPlayerInfo uIPlayerInfo = UIGameManager.Instance.ArrPlayerInfo[playerID];
			if (!UIGameManager.Instance.isUIInvisible)
			{
				uIPlayerInfo.ProgressBarObject.SetActive(value: true);
				if (itemInteractable.functionInteract == "Revive" || itemInteractable.functionInteract == "HealOther")
				{
					uIPlayerInfo.HealBarObject.SetActive(value: true);
					uIPlayerInfo.NormalBarObject.SetActive(value: false);
					Image radialBar = uIPlayerInfo.ProgressBarRadial;
					radialBar.fillAmount = 0f;
					uIPlayerInfo.PointerStitch.DOKill();
					uIPlayerInfo.PointerStitch.anchoredPosition = new Vector2(0f, 7.95f);
					uIPlayerInfo.PointerStitch.DOAnchorPos(new Vector2(90f, 7.95f), player.PlayerMultiplyStatsData.GetMultiplyTimeRevive()).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
					HealProgressBarTween.Kill();
					HealProgressBarTween = DOTween.To(() => radialBar.fillAmount, (float x) =>
					{
						radialBar.fillAmount = x;
					}, 1f, itemInteractable.progressTimeToComplete).SetEase(Ease.Linear);
					float num = 2f;
					player.network.playerPhoton.RpcSetHealingValue(30, 200);
					uIPlayerInfo.TextHealingValue.text = "+30";
					for (int num2 = 0; num2 < uIPlayerInfo.listTargetStitch.Count; num2++)
					{
						uIPlayerInfo.listTargetStitch[num2].gameObject.SetActive(value: true);
						num = UnityEngine.Random.Range(num, num + 16f);
						int num3 = UnityEngine.Random.Range(5, 16);
						uIPlayerInfo.listTargetStitch[num2].DOKill();
						uIPlayerInfo.listTargetStitch[num2].DOScale(1f, 0f);
						uIPlayerInfo.listTargetStitch[num2].anchoredPosition = new Vector2(num, uIPlayerInfo.listTargetStitch[num2].anchoredPosition.y);
						uIPlayerInfo.listTargetStitch[num2].sizeDelta = new Vector2(num3, uIPlayerInfo.listTargetStitch[num2].sizeDelta.y);
						num += (float)(6 + num3);
					}
				}
				else
				{
					uIPlayerInfo.HealBarObject.SetActive(value: false);
					uIPlayerInfo.NormalBarObject.SetActive(value: true);
					uIPlayerInfo.BarProgressTransform.localScale = new Vector3(0f, 1f, 1f);
				}
			}
			if (itemInteractable.iconAnimationName != "")
			{
				uIPlayerInfo.iconBarAnimator.Play(itemInteractable.iconAnimationName);
			}
			else if (itemInteractable.functionInteract == "Revive")
			{
				uIPlayerInfo.transform.SetAsLastSibling();
				itemInteractable.parentCharacter.reviveTimer.PauseDuration();
				uIPlayerInfo.iconBarAnimator.Play("Revive");
			}
			else
			{
				uIPlayerInfo.iconBarAnimator.Play("Interact");
			}
			if (itemInteractable.isProgressing && itemInteractable.fromPlayer != null && itemInteractable.fromPlayer != NetworkGameManager.Instance.GetPlayer(playerID))
			{
				itemInteractable.fromPlayer.StopInteractProgress();
			}
			itemInteractable.fromPlayer = NetworkGameManager.Instance.GetPlayer(playerID);
			if (itemInteractable.fromPlayer.isAttacking)
			{
				itemInteractable.fromPlayer.isAttacking = false;
				itemInteractable.fromPlayer.isAttackMelee = false;
				if (itemInteractable.fromPlayer.network.isLocalPlayer)
				{
					UIGameManager.Instance.ArrPlayerInfo[playerID].ChargeMeleeProgressObject.SetActive(value: false);
				}
				itemInteractable.fromPlayer.weaponController.MeleeTween.Kill();
			}
			itemInteractable.fromPlayer.itemCollision = itemInteractable.gameObject;
			if (itemInteractable.functionInteract == "LockPick")
			{
				AudioManager.PlaySFXTransform("lockpick", itemInteractable.fromPlayer.transform, itemInteractable.fromPlayer.network.isLocalPlayer);
			}
			itemInteractable.fromPlayer.fsmUpperBody.SetBool("isReviving", value: true);
			itemInteractable.fromPlayer.fsmUpperBody.SetBool("isMelee", value: false);
			itemInteractable.fromPlayer.fsmUpperBody.SetBool("isShooting", value: false);
			itemInteractable.fromPlayer.enableMoveChar = false;
			if (itemInteractable.fromPlayer.network.isLocalPlayer)
			{
				UIGameManager.Instance.ArrPlayerInfo[playerID].ChargeMeleeProgressObject.SetActive(value: false);
			}
			itemInteractable.fromPlayer.weaponController.ReleaseAttack();
			float num4 = 0f;
			if (itemInteractable.functionInteract == "Barricade")
			{
				UIGameManager.Instance.ArrPlayerInfo[playerID].Divide.SetActive(value: true);
				if (itemInteractable.Hp <= itemInteractable.MaxHp / 2 && itemInteractable.Hp > 0)
				{
					num4 = itemInteractable.progressTimeToComplete;
					itemInteractable.timerProgress.StartDuration(num4);
					itemInteractable.timerProgress.interval = itemInteractable.progressTimeToComplete / 2f;
				}
				else
				{
					num4 = itemInteractable.progressTimeToComplete;
					itemInteractable.timerProgress.StartDuration(num4);
				}
			}
			else if (itemInteractable.functionInteract == "Revive")
			{
				UIGameManager.Instance.ArrPlayerInfo[playerID].Divide.SetActive(value: false);
				num4 = itemInteractable.progressTimeToComplete * itemInteractable.fromPlayer.PlayerMultiplyStatsData.GetMultiplyTimeRevive();
				itemInteractable.timerProgress.StartDuration(num4);
				itemInteractable.parentCharacter.reviveTimer.PauseDuration();
			}
			else
			{
				UIGameManager.Instance.ArrPlayerInfo[playerID].Divide.SetActive(value: false);
				num4 = itemInteractable.progressTimeToComplete;
				itemInteractable.timerProgress.StartDuration(num4);
			}
		}
		else if (player != null && NetworkGameManager.Instance.isServer)
		{
			player.StopInteractProgress(itemInteractable);
		}
	}

	public void StopProgressInteract(int uniqueID, byte playerID)
	{
		ItemInteractable itemInteractable = null;
		if (uniqueID >= 0)
		{
			itemInteractable = GetItemInteractable(uniqueID);
		}
		if (itemInteractable != null && itemInteractable.isNeedProgress)
		{
			PlayerController player = NetworkGameManager.Instance.GetPlayer(playerID);
			if (player != null)
			{
				UIGameManager.Instance.ArrPlayerInfo[playerID].TweenTargetAnimation.DOPause();
				if (itemInteractable.functionInteract == "Revive")
				{
					UIGameManager.Instance.ArrPlayerInfo[NetworkGameManager.Instance.ownPlayer.network.GetIDX()].transform.SetAsLastSibling();
					itemInteractable.parentCharacter.reviveTimer.ResumeDuration();
				}
				UIGameManager.Instance.ArrPlayerInfo[player.network.GetIDX()].ProgressBarObject.SetActive(value: false);
				UIGameManager.Instance.ArrPlayerInfo[player.network.GetIDX()].HealBarObject.SetActive(value: false);
				player.fsmUpperBody.SetBool("isReviving", value: false);
				if (player.network.GetHealth() > 0f)
				{
					player.enableMoveChar = true;
				}
				itemInteractable.labelItemCommandOff = false;
				itemInteractable.timerProgress.StopDuration();
				itemInteractable.isProgressing = false;
				itemInteractable.fromPlayer = null;
				itemInteractable.labelItemCommandOff = true;
				HealProgressBarTween.Kill();
			}
			return;
		}
		PlayerController player2 = NetworkGameManager.Instance.GetPlayer(playerID);
		if (player2 != null)
		{
			UIGameManager.Instance.ArrPlayerInfo[playerID].TweenTargetAnimation.DOPause();
			UIGameManager.Instance.ArrPlayerInfo[player2.network.GetIDX()].ProgressBarObject.SetActive(value: false);
			UIGameManager.Instance.ArrPlayerInfo[player2.network.GetIDX()].HealBarObject.SetActive(value: false);
			player2.fsmUpperBody.SetBool("isReviving", value: false);
			if (player2.network.GetHealth() > 0f)
			{
				player2.enableMoveChar = true;
			}
			HealProgressBarTween.Kill();
		}
	}

	public int GetIdxItemPool(int idx, bool isQuickDrop = false)
	{
		int result = -1;
		foreach (ItemPickable item in Instance.arrItemPickable)
		{
			if (item.itemSprite != null && !item.itemSprite.enabled && !item.itemCollider.enabled && item.itemID == idx)
			{
				result = item.uniqueID;
				if (isQuickDrop)
				{
					item.itemCollider.enabled = true;
				}
				break;
			}
		}
		return result;
	}

	public void ChangeSpectator(int playerIDX, int prevPlayer = -1)
	{
		PlayerController player = NetworkGameManager.Instance.GetPlayer(playerIDX);
		CameraGame.Instance.CinemachineTarget.AddMember(player.transform, 1f, 3f);
		player.audioListener.enabled = true;
		player.fov.enabled = true;
		if (prevPlayer != -1)
		{
			RoomCollider roomCollider = null;
			foreach (RoomCollider item in Instance.arrRoom)
			{
				if (item != null)
				{
					if (!item.listPlayerCollided[prevPlayer] && item.listPlayerCollided[playerIDX])
					{
						roomCollider = item;
						item.TurnOnLight();
					}
					else if (item.listPlayerCollided[prevPlayer] && !item.listPlayerCollided[playerIDX])
					{
						item.TurnOffLight();
					}
				}
			}
			if (roomCollider != null)
			{
				foreach (PlayerController item2 in NetworkGameManager.Instance.arrPlayerController)
				{
					if (roomCollider.listPlayerCollided[item2.network.GetIDX()])
					{
						for (int i = 0; i < item2.allLights.Count; i++)
						{
							item2.allLights[i].DOIntensity(item2.allLightIntensity[i], 0.5f);
						}
						UIGameManager.Instance.ArrPlayerInfo[item2.network.GetIDX()].gameObject.SetActive(value: true);
					}
					else
					{
						for (int j = 0; j < item2.allLights.Count; j++)
						{
							item2.allLights[j].DOIntensity(0f, 0.5f);
						}
						UIGameManager.Instance.ArrPlayerInfo[item2.network.GetIDX()].gameObject.SetActive(value: false);
					}
				}
			}
		}
		NetworkGameManager.Instance.ownPlayer.SetTargetIdxCamBeforeRevive(playerIDX);
	}

	public void TurnOffElectricity()
	{
	}

	public void DestroyObjectGame(ObjectCollisionBullet objCollision, byte playerID)
	{
		if (objCollision != null && !objCollision.isDisabled)
		{
			if (objCollision.activateObject != null)
			{
				objCollision.activateObject.transform.parent = objCollision.transform.parent;
				objCollision.activateObject.SetActive(value: true);
				UniTaskUtil.DelayedCall(this, objCollision.delayDestroy, () =>
				{
					UnityEngine.Object.Destroy(objCollision.activateObject);
				}).Forget();
			}
			if (!objCollision.isExplosiveObject)
			{
				CameraGame.Instance.CameraShake(0.3f);
			}
			if (objCollision.destructibleComp != null)
			{
				if (objCollision.destructObject.destroyedObject != null)
				{
					if (objCollision.destructObject.normalObject != null)
					{
						objCollision.destructObject.normalObject.SetActive(value: false);
					}
					objCollision.destructObject.destroyedObject.SetActive(value: true);
					if (objCollision.ObjectAnimationAfterDestroy.Count > 0)
					{
						for (int num = 0; num < objCollision.ObjectAnimationAfterDestroy.Count; num++)
						{
							objCollision.ObjectAnimationAfterDestroy[num].AnimatorObject.SetTrigger(objCollision.ObjectAnimationAfterDestroy[num].TriggerAnimation);
						}
					}
				}
				if (!objCollision.isExplosiveObject)
				{
					objCollision.destructibleComp.ApplyDamage(100f);
					if (objCollision.SFXName != "")
					{
						AudioManager.PlaySFXTransform(objCollision.SFXName, objCollision.transform, isLocalPlayerTrigger: false);
					}
					if (objCollision.destructibleComp.currentHitPoints <= 0f)
					{
						if (objCollision.activateObject != null)
						{
							objCollision.activateObject.transform.parent = objCollision.transform.parent;
							objCollision.activateObject.SetActive(value: true);
							UniTaskUtil.DelayedCall(this, objCollision.delayDestroy, () =>
							{
								UnityEngine.Object.Destroy(objCollision.activateObject);
							}).Forget();
						}
						objCollision.ObjectCollider.enabled = false;
					}
				}
				else
				{
					AudioManager.PlaySFXTransform("impactBullet_Metal", objCollision.transform, isLocalPlayerTrigger: false);
					if (objCollision.parentObject != null)
					{
						objCollision.parentObject.GetComponent<MeshRenderer>().material.DOColor(new Color(10f, 0f, 0f), 2.5f);
						objCollision.transform.DOShakeRotation(3f, 2f, 50, 90f, fadeOut: false).SetEase(Ease.InQuint);
						UniTaskUtil.DelayedCall(this, 2f, () =>
						{
							ObjectExplosion(objCollision, NetworkGameManager.Instance.GetPlayer(playerID).weaponController);
						}).Forget();
					}
					else
					{
						ObjectExplosion(objCollision, NetworkGameManager.Instance.GetPlayer(playerID).weaponController);
					}
					objCollision.isDisabled = true;
				}
			}
		}
		if (objCollision != null && objCollision.destructObject != null)
		{
			objCollision.gameObject.layer = 0;
			if (AStarPath != null)
			{
				objCollision.destructObject.colliderObject.enabled = true;
				AStarPath.UpdateGraphs(objCollision.destructObject.colliderObject.bounds);
				AStarPath.FlushGraphUpdates();
				objCollision.destructObject.colliderObject.enabled = false;
			}
		}
	}

	public void ObjectExplosion(ObjectCollisionBullet objCollision, WeaponController weaponController)
	{
		if (MathFunc.Distance(objCollision.transform.position, weaponController.transform.position) < 4f)
		{
			CameraGame.Instance.CameraShake(0.7f, 0.7f);
		}
		Instance.CheckModifierExplosionCallHorde();
		weaponController.CheckExplosionDamage(objCollision.transform);
		weaponController.CheckEnemyAggro(objCollision.transform);
		if (objCollision.destructObject != null)
		{
			objCollision.gameObject.layer = 0;
			if (AStarPath != null)
			{
				AStarPath.UpdateGraphs(objCollision.destructObject.colliderObject.bounds);
				AStarPath.FlushGraphUpdates();
			}
		}
		Destructible destructibleComp = objCollision.destructibleComp;
		if (objCollision.SFXName != "")
		{
			AudioManager.PlaySFXTransform(objCollision.SFXName, objCollision.transform, isLocalPlayerTrigger: false);
		}
		destructibleComp.ApplyDamage(100f);
		if (objCollision.ObjectCollider != null && destructibleComp.currentHitPoints <= 0f)
		{
			objCollision.ObjectCollider.enabled = false;
		}
	}

	public RoomCollider GetRoomCollider(string roomName)
	{
		RoomCollider result = null;
		foreach (RoomCollider item in arrRoom)
		{
			if (item.RoomName == roomName)
			{
				result = item;
				break;
			}
		}
		return result;
	}

	public void ShowItemMap(int uniqueID)
	{
		ItemPickable itemPickable = GetItemPickable(uniqueID);
		if (!(itemPickable != null) || !(itemPickable.itemMap != null) || !itemPickable.itemCollider.enabled)
		{
			return;
		}
		itemPickable.itemMap.enabled = true;
		itemPickable.itemMap.sprite = DataManager.Instance.GetItemSprite(itemPickable.itemID.ToString());
		if (GlobalSaveData.instance.optionData.autoMinimap == 1)
		{
			if (itemPickable.itemMap != null)
			{
				itemPickable.itemMap.transform.DOLocalRotate(new Vector3(90f, 0f, -CameraGame.Instance.camRotate), 0f);
			}
		}
		else if (itemPickable.itemMap != null)
		{
			itemPickable.itemMap.transform.DOLocalRotate(new Vector3(90f, 0f, 0f), 0f);
		}
	}

	public void HideAllPlayer(Transform parent = null)
	{
		foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerNetworkController)
		{
			if (!(item == null))
			{
				item.network.SetEnableControl(value: false);
				item.characterRenderController.HideCharacter();
				if (parent != null && parent.gameObject.activeSelf)
				{
					item.gameObject.transform.SetParent(parent);
					item.transform.localPosition = Vector3.zero;
				}
			}
		}
	}

	public void ShowAllPlayer(bool setInput = true, bool dontDestroyOnLoad = true)
	{
		foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerNetworkController)
		{
			if (!(item == null))
			{
				if (dontDestroyOnLoad)
				{
					item.gameObject.transform.SetParent(null);
					UnityEngine.Object.DontDestroyOnLoad(item);
				}
				item.network.SetEnableControl(setInput);
				item.characterRenderController.ShowCharacter();
			}
		}
	}

	public bool CheckWin(bool isEnterWinArea = false)
	{
		int num = 0;
		int num2 = 0;
		foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerController)
		{
			if (item.network.GetHealth() > 0f)
			{
				num2++;
				if (ListPlayerInAreaWin[item.network.GetIDX()])
				{
					num++;
				}
				else if (isEnterWinArea)
				{
					item.network.SetHealth(0f);
				}
			}
		}
		foreach (PlayerController item2 in NetworkGameManager.Instance.arrPlayerNetworkController)
		{
			if (item2 != null && (bool)item2.network.playerPhoton.disconnected)
			{
				item2.network.SetHealth(0f);
			}
		}
		if (num > 0)
		{
			if ((bool)_mapManager)
			{
				_mapManager.CheckWinCondition();
			}
			else
			{
				NetworkGameManager.Instance.StartCoroutine(NetworkGameManager.Instance.WinLevel());
			}
			return true;
		}
		return false;
	}

	public void PauseGameTime()
	{
		if (NetworkGameManager.Instance.mode != NetworkGameManager.MultiplayerMode.Solo)
		{
			return;
		}
		Time.timeScale = 0f;
		foreach (EnemyController item in arrEnemyController)
		{
			item.SetEnableAI(value: false);
		}
		waveManager.cueHordeTimer.PauseDuration();
		waveManager.hordeTimer.PauseDuration();
		waveManager.spawnTimer.PauseDuration();
		waveManager.hordingTimer.PauseDuration();
		waveManager.roamingTimer.PauseDuration();
		timer.PauseDuration();
	}

	public void ResumeGameTime()
	{
		if (NetworkGameManager.Instance.mode != NetworkGameManager.MultiplayerMode.Solo)
		{
			return;
		}
		Time.timeScale = 1f;
		foreach (EnemyController item in arrEnemyController)
		{
			item.SetEnableAI(value: true);
		}
		waveManager.cueHordeTimer.ResumeDuration();
		waveManager.hordeTimer.ResumeDuration();
		waveManager.spawnTimer.ResumeDuration();
		waveManager.hordingTimer.ResumeDuration();
		waveManager.roamingTimer.ResumeDuration();
		timer.ResumeDuration();
	}

	public void RandomizeItem()
	{
		foreach (WeaponMapType item in gameManagerPhoton.CurrentMission.ListWeapon)
		{
			foreach (RandomizeItem item2 in ListRandomizeItem)
			{
				if (item.WeaponType != item2.WeaponType)
				{
					continue;
				}
				foreach (ItemList itemList in item2.ItemLists)
				{
					if (itemList.ItemType == item.Weapon)
					{
						continue;
					}
					foreach (GameObject gameObject in itemList.gameObjects)
					{
						UnityEngine.Object.Destroy(gameObject);
					}
				}
				ListRandomizeItem.Remove(item2);
				break;
			}
		}
		foreach (RandomizeItem item3 in ListRandomizeItem)
		{
			if (item3.WeaponType == WeaponTypeEnum.NONE)
			{
				continue;
			}
			foreach (ItemList itemList2 in item3.ItemLists)
			{
				foreach (GameObject gameObject2 in itemList2.gameObjects)
				{
					UnityEngine.Object.Destroy(gameObject2);
				}
			}
		}
		arrItemPickable.Sort((ItemPickable p1, ItemPickable p2) => p1.uniqueID.CompareTo(p2.uniqueID));
	}

	public void ExecBarricadeBotBroken(byte uniqueID, Vector3 sourcePos)
	{
		gameManagerPhoton.RpcBarricadeBotBroken(uniqueID, sourcePos);
	}

	public void ExecBarricadeTopBroken(byte uniqueID, Vector3 sourcePos)
	{
		gameManagerPhoton.RpcBarricadeTopBroken(uniqueID, sourcePos);
	}

	public void CheckRoomMap(PlayerController player, RoomCollider roomCollider = null)
	{
		if (roomCollider != null)
		{
			foreach (RoomCollider item in arrRoom)
			{
				if (item.RoomName == player.RoomName || item == roomCollider)
				{
					item.CheckMap(player);
				}
			}
			return;
		}
		foreach (RoomCollider item2 in arrRoom)
		{
			if (item2.RoomName == player.RoomName)
			{
				item2.CheckMap(player);
			}
		}
	}

	public void CheckModifierExplosionCallHorde()
	{
		if (GlobalMissionManager.Instance.ModEnableExplosionsHorde.CurrentValue >= 1f && !LobbyManager.Instance && !isHordeMode && !isInfiniteHordeMode)
		{
			waveManager.InitHorde(isInit: false, 1);
			waveManager.cueHordeTimer.StartDuration(0.1f);
			UniTaskUtil.DelayedCall(this, 0.5f, () =>
			{
				waveManager.buildUpHordeTimer.StartDuration(0.1f);
			}, ignoreTimeScale: false).Forget();
			UniTaskUtil.DelayedCall(this, 1f, () =>
			{
				waveManager.hordeTimer.StartDuration(0.1f);
			}, ignoreTimeScale: false).Forget();
		}
	}

	public void TriggerWin(bool usingGameCutscene = false)
	{
		IsCutscenePlaying = true;
		GameManagerPhoton.Instance.IsWin = true;
		NetworkGameManager.Instance.ownPlayer.GetComponent<StatusEffectController>().ClearAllStatusEffect();
		if (usingGameCutscene)
		{
			if (_cutsceneTrigger != null)
			{
				_cutsceneTrigger?.PlayCutscene();
			}
			else
			{
				NetworkGameManager.Instance.StartCoroutine(NetworkGameManager.Instance.WinLevel());
			}
		}
	}

	public void DisconnectFromServer()
	{
		Time.timeScale = 1f;
		NetworkGameManager.Instance.Shutdown();
		quitGame = true;
		GlobalUIManager.Instance.ClickGoToScene("MainMenu");
	}
}

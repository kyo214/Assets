using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Doozy.Runtime.UIManager.Containers;
using I2.Loc;
using Toked;
using Toked.Item;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ItemInteractable : MonoBehaviour
{
	public InteractableType Type;

	public int Hp;

	[SerializeField]
	private int _maxHp;

	public Transform _labelPos;

	public string labelNameInit;

	public string labelName;

	public string objectName;

	public string labelNameReverse;

	public bool labelItemCommandOff;

	public PlayerController fromPlayer;

	public Transform toObject;

	public PlayerController parentCharacter;

	public int idxTrigger;

	public string functionInteract;

	public bool isTriggerOnce;

	public bool isTriggerReverse;

	public bool isTriggered;

	public bool isAutomaticClose;

	public bool triggerOnReverse;

	public bool isClue;

	public ItemInteractable syncObject;

	public float DelayObjectActiveAfterComplete;

	private bool isShowingCountdown;

	public bool ShowCountdownLabelBeforeComplete;

	public string termCountdownBeforeComplete;

	public GameObject ObjectActiveSpecial;

	public GameObject ObjectActiveAfterComplete;

	public GameObject ObjectInactiveAfterComplete;

	public List<int> listItemToActivate = new List<int>();

	public bool destroyItemNeed = true;

	public bool needItemToActivate;

	public string Password;

	public int ClueID = -1;

	public bool isShowUI;

	public bool isUIGameStillShowing;

	public UIView UIMenu;

	public bool isShowControlOptions;

	public bool isLocked;

	public bool isLockedFromOtherSide;

	public int itemIDUnlock = -1;

	public bool isNeedProgress;

	public bool isAutoProgress;

	public float progressTimeToComplete;

	public string iconAnimationName;

	public bool isProgressing;

	public Outline outline;

	public Animator animatorTrigger1;

	public List<string> animationName1 = new List<string>();

	public string SetAnimatorTrigger1;

	public bool isNeedSyncStateAnimator;

	public Animator animatorTrigger2;

	public List<string> animationName2 = new List<string>();

	public Animator animatorBarricade;

	public List<string> animationNameBarricade = new List<string>();

	public GameObject objectTopBarricade;

	public GameObject objectBotBarricade;

	public Vector3 posInitObjectTopBarricade;

	public Vector3 posInitObjectBotBarricade;

	public Vector3 rotInitObjectTopBarricade;

	public Vector3 rotInitObjectBotBarricade;

	public bool isBotBroken;

	public bool isTopBroken;

	public JumpEnemyCollider JumpCollider;

	public string sfxNameTriggered;

	public string sfxNameTriggered2;

	public string sfxNameReverseTriggered;

	public string VOMaleNeedItem;

	public string VOFemaleNeedItem;

	public string VOMaleInteract;

	public string VOFemaleInteract;

	[SerializeField]
	private List<RoomCollider> lightSwitchList = new List<RoomCollider>();

	public int spawnItemID = -1;

	public Transform posSpawnItem;

	public SpriteRenderer spriteMap;

	public bool triggerScanAstar;

	public GameObject lockMap;

	public SpriteRenderer IconMap;

	public bool afterCompleteShowNote;

	public ItemPickable note;

	public UIView UInote;

	public XTimer timerProgress;

	public XTimer timerDelay;

	[SerializeField]
	private List<PosEnemy> listEnableHordeSpawnAfterTriggered = new List<PosEnemy>();

	public List<DialogSO> ListDialogue = new List<DialogSO>();

	public int IdxDialogue;

	public int showChatID = -1;

	public int MonologueID = -1;

	public bool IsOnStartInteractShowMonologue;

	public bool IsInteractionMonologueMultiplayerOnly;

	public bool triggerWinLevel;

	public BoxCollider boxCollider;

	public BoxCollider doorCollider;

	private static readonly int SpeedAnimation = Animator.StringToHash("speedAnimation");

	public bool IsIgnoreMapCleared;

	public bool IsClue;

	public bool IsSolved;

	public bool IsPuzzle;

	public int UniqueID;

	public RoomCollider RoomColliderItem;

	[SerializeField]
	private ItemInteractableCustomFunction _interactableCustomFunction;

	[SerializeField]
	private IconItemType _iconItemType;

	[SerializeField]
	private bool _isAdditionalObjective;

	[SerializeField]
	private int _idxAdditionalObjective;

	public bool IsBRIMCar;

	public UnityEvent onCompleteInteractObjectEvent;

	public int MaxHp
	{
		get
		{
			return _maxHp;
		}
		set
		{
			_maxHp = value;
		}
	}

	private void Awake()
	{
		IsSolved = IsIgnoreMapCleared;
		triggerScanAstar = false;
	}

	private void Start()
	{
		if (UIMenu != null && UIMenu.GetComponent(typeof(IPuzzle)) != null)
		{
			IsPuzzle = true;
		}
		if (((bool)syncObject || isLockedFromOtherSide || isLocked || IsPuzzle || doorCollider != null) && animatorTrigger1 != null)
		{
			if (doorCollider == null)
			{
				doorCollider = animatorTrigger1.transform.GetComponent<BoxCollider>();
			}
			if (doorCollider != null)
			{
				doorCollider.transform.gameObject.layer = 8;
				if (GameManager.Instance.AStarPath != null)
				{
					GameManager.Instance.AStarPath.UpdateGraphs(doorCollider.bounds);
				}
			}
		}
		if (GameManager.Instance.AStarPath != null)
		{
			GameManager.Instance.AStarPath.FlushGraphUpdates();
		}
		foreach (RoomCollider item in GameManager.Instance.arrRoom)
		{
			for (int i = 0; i < item.boxColliders.Count; i++)
			{
				if (boxCollider != null && boxCollider.bounds.Intersects(item.boxColliders[i].bounds))
				{
					lightSwitchList.Add(item);
				}
			}
		}
		labelNameInit = labelName;
		if (animatorTrigger1 != null && animatorTrigger1.gameObject.GetComponent<DoorControl>() != null)
		{
			animatorTrigger1.gameObject.GetComponent<DoorControl>().interactObj = this;
		}
		if (triggerOnReverse)
		{
			if (animatorTrigger1 != null)
			{
				if (animationName1.Count > 0 && animationName1[idxTrigger] != "")
				{
					animatorTrigger1.SetFloat(SpeedAnimation, 1f);
					animatorTrigger1.Play(animationName1[idxTrigger], -1, 0f);
					triggerScanAstar = true;
				}
				else if (SetAnimatorTrigger1 != "")
				{
					animatorTrigger1.SetTrigger(SetAnimatorTrigger1);
					triggerScanAstar = true;
				}
			}
			if (animationName2.Count > 0 && animatorTrigger2 != null && animationName2[idxTrigger] != "")
			{
				animatorTrigger2.SetFloat(SpeedAnimation, 1f);
				animatorTrigger2.Play(animationName2[idxTrigger], -1, 0f);
				triggerScanAstar = true;
			}
			labelName = labelNameReverse;
		}
		if (isNeedProgress)
		{
			if (functionInteract == "Revive" || functionInteract == "HealOther")
			{
				parentCharacter = base.transform.parent.GetComponent<PlayerController>();
				progressTimeToComplete = BGDatabase_GameConfig.GetEntity(GameModes.Instance.modeGame).ReviveTime;
			}
			if (functionInteract == "Barricade")
			{
				objectBotBarricade = animatorBarricade.transform.GetChild(0).gameObject;
				objectTopBarricade = animatorBarricade.transform.GetChild(1).gameObject;
				objectTopBarricade.SetActive(value: false);
				objectBotBarricade.SetActive(value: false);
				isTriggered = false;
				boxCollider.enabled = true;
				posInitObjectTopBarricade = objectTopBarricade.transform.position;
				posInitObjectBotBarricade = objectBotBarricade.transform.position;
				rotInitObjectTopBarricade = objectTopBarricade.transform.localEulerAngles;
				rotInitObjectBotBarricade = objectBotBarricade.transform.localEulerAngles;
				animatorBarricade.enabled = false;
			}
		}
		foreach (RoomCollider item2 in GameManager.Instance.arrRoom)
		{
			BoxCollider[] componentsInChildren = item2.GetComponentsInChildren<BoxCollider>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				if (componentsInChildren[j].bounds.Contains(base.transform.position))
				{
					item2.interactionList.Add(this);
					RoomColliderItem = item2;
				}
			}
		}
		if (listItemToActivate.Count > 0)
		{
			needItemToActivate = true;
		}
		if (IsBRIMCar)
		{
			GameManager.Instance.ListBrimCarInteractable.Add(this);
			if ((bool)GameManagerPhoton.Instance && (bool)GameManagerPhoton.Instance.CurrentMission && (bool)GameManagerPhoton.Instance.CurrentMission.MissionObjective)
			{
				if (!GameManagerPhoton.Instance.CurrentMission.IsFixedMissionObjective)
				{
					listItemToActivate.Clear();
					if (GameManagerPhoton.Instance.CurrentMission.MissionObjective.MissionKeyItem > 0)
					{
						listItemToActivate.Add(GameManagerPhoton.Instance.CurrentMission.MissionObjective.MissionKeyItem);
					}
				}
				if (GameManagerPhoton.Instance.CurrentMission.MissionObjective.TimerCountdownCarRepairing > 0)
				{
					DelayObjectActiveAfterComplete = GameManagerPhoton.Instance.CurrentMission.MissionObjective.TimerCountdownCarRepairing;
				}
				if (GameManagerPhoton.Instance.CurrentMission.MissionObjective.IsCarRepairingOnStart)
				{
					UniTaskUtil.DelayedCall(this, 5f, () =>
					{
						ObjectActiveSpecial.SetActive(value: true);
						termCountdownBeforeComplete = "Menu/RepairingEngine";
						UIMissionObjective.Instance.FrameEscape.SetActive(value: true);
						UIMissionObjective.Instance.TextEscape.text = LocalizationManager.GetTranslation("Menu/RepairingEngine");
						if (NetworkGameManager.Instance.isServer)
						{
							NetworkGameManager.Instance.ownPlayer.network.ExecInteractObject((short)UniqueID);
						}
					}, ignoreTimeScale: false).Forget();
				}
			}
		}
		if (functionInteract == "Pet")
		{
			GameManager.Instance.arrItemInteractable.Add(this);
			GameManager.Instance.arrItemInteractable.Sort((ItemInteractable p1, ItemInteractable p2) => p1.UniqueID.CompareTo(p2.UniqueID));
		}
		if (triggerScanAstar && triggerOnReverse)
		{
			UniTaskUtil.DelayedCall(this, 1f, ScanAstarItemCollider).Forget();
		}
	}

	private void FixedUpdate()
	{
		if (isNeedProgress)
		{
			if (timerProgress.isRunning)
			{
				foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerController)
				{
					if (!(item.itemCollision == base.gameObject) && !(item == fromPlayer))
					{
						continue;
					}
					if (item.network.GetHealth() > 0f)
					{
						UIGameManager.Instance.ArrPlayerInfo[item.network.GetIDX()].BarProgressTransform.localScale = new Vector3(1f - timerProgress.interval / timerProgress.initInterval, 1f, 1f);
						if (functionInteract == "Barricade" && 1f - timerProgress.interval / timerProgress.initInterval > 0.5f && !objectBotBarricade.activeSelf)
						{
							Hp = _maxHp / 2;
							if (ObjectActiveAfterComplete != null)
							{
								ObjectActiveAfterComplete.SetActive(value: true);
							}
							objectBotBarricade.GetComponent<Collider>().enabled = false;
							objectBotBarricade.GetComponent<Rigidbody>().isKinematic = true;
							objectBotBarricade.transform.DOKill();
							DOTween.Kill("DropBotBarricade");
							isBotBroken = false;
							objectBotBarricade.transform.position = posInitObjectBotBarricade;
							objectBotBarricade.transform.localEulerAngles = rotInitObjectBotBarricade;
							animatorBarricade.enabled = true;
							objectBotBarricade.SetActive(value: true);
							animatorBarricade.enabled = true;
							animatorBarricade.Play(animationNameBarricade[0], -1, 0f);
						}
					}
					else
					{
						UIGameManager.Instance.ArrPlayerInfo[item.network.GetIDX()].ProgressBarObject.SetActive(value: false);
						UIGameManager.Instance.ArrPlayerInfo[item.network.GetIDX()].HealBarObject.SetActive(value: false);
						isProgressing = false;
					}
				}
			}
			if (fromPlayer != null && timerProgress.isCompleted() && fromPlayer.network.GetHealth() > 0f)
			{
				fromPlayer.fsmUpperBody.SetBool("isReviving", value: false);
				UIGameManager.Instance.ArrPlayerInfo[fromPlayer.network.GetIDX()].ProgressBarObject.SetActive(value: false);
				UIGameManager.Instance.ArrPlayerInfo[fromPlayer.network.GetIDX()].HealBarObject.SetActive(value: false);
				labelItemCommandOff = false;
				isProgressing = false;
				fromPlayer.enableMoveChar = true;
				if (fromPlayer.network.isLocalPlayer)
				{
					fromPlayer.network.ExecInteractObject((short)UniqueID);
				}
			}
		}
		if (isShowingCountdown)
		{
			Vector3 localPosition = UIGameManager.Instance.WorldToCanvasPoint(_labelPos.position);
			ChatSystem.Instance.ObjectEscape.transform.localPosition = localPosition;
		}
	}

	public void ScanAstarItemCollider()
	{
		if (!(animatorTrigger1 != null))
		{
			return;
		}
		triggerScanAstar = false;
		if (GameManager.Instance.AStarPath != null)
		{
			bool num = boxCollider.enabled;
			boxCollider.enabled = true;
			GameManager.Instance.AStarPath.UpdateGraphs(boxCollider.bounds);
			if (doorCollider != null)
			{
				doorCollider.transform.gameObject.layer = 22;
				GameManager.Instance.AStarPath.UpdateGraphs(doorCollider.bounds);
			}
			GameManager.Instance.AStarPath.FlushGraphUpdates();
			if (!num)
			{
				boxCollider.enabled = false;
			}
		}
	}

	private void OnDisable()
	{
		if ((bool)NetworkGameManager.Instance && (bool)NetworkGameManager.Instance.ownPlayer && NetworkGameManager.Instance.ownPlayer.itemCollision == base.gameObject)
		{
			if (!NetworkGameManager.Instance.ownPlayer.fsmUpperBody.GetBool("isReviving"))
			{
				NetworkGameManager.Instance.ownPlayer.itemCollision = null;
				NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
				NetworkGameManager.Instance.ownPlayer.functionItemCollision = "";
			}
			labelItemCommandOff = false;
			if ((bool)ChatSystem.Instance)
			{
				ChatSystem.Instance.ItemCommand.SetActive(value: false);
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (!other.CompareTag("Player") || !NetworkGameManager.Instance.ownPlayer || !timerDelay || timerDelay.isRunning || !(other.transform == NetworkGameManager.Instance.ownPlayer.transform) || (isAutomaticClose && (!isAutomaticClose || isTriggered)) || isProgressing)
		{
			return;
		}
		if (NetworkGameManager.Instance.ownPlayer.functionItemCollision != "Revive" && NetworkGameManager.Instance.ownPlayer.functionItemCollision != "HealOther" && ChatSystem.Instance.ItemCommand != null)
		{
			if ((!ChatSystem.Instance.ItemCommand.gameObject.activeSelf || ChatSystem.Instance.LabelTermItemCommand.Term != labelName) && !labelItemCommandOff && !UIGameManager.Instance.isUIInvisible)
			{
				ChatSystem.Instance.ItemCommand.gameObject.SetActive(value: true);
				Vector3 localPosition = UIGameManager.Instance.WorldToCanvasPoint(_labelPos.position);
				ChatSystem.Instance.ItemCommand.transform.localPosition = localPosition;
			}
			bool flag = true;
			if (NetworkGameManager.Instance.ownPlayer.itemCollision != null)
			{
				flag = ((MathFunc.DistanceSameYPos(base.gameObject.transform.position, NetworkGameManager.Instance.ownPlayer.transform.position) < MathFunc.DistanceSameYPos(NetworkGameManager.Instance.ownPlayer.itemCollision.transform.position, NetworkGameManager.Instance.ownPlayer.transform.position)) ? true : false);
			}
			if (ChatSystem.Instance.LabelTermItemCommand.GetMainTargetsText() == "")
			{
				if (labelName != "")
				{
					ChatSystem.Instance.LabelTermItemCommand.SetTerm("Menu/" + labelName);
				}
				else
				{
					ChatSystem.Instance.LabelTermItemCommand.SetTerm("Menu/EmptyField");
				}
			}
			if ((NetworkGameManager.Instance.ownPlayer.itemCollision != base.gameObject) & flag)
			{
				ChatSystem.Instance.SetIcon(_iconItemType);
				if (labelName != "")
				{
					ChatSystem.Instance.LabelTermItemCommand.SetTerm("Menu/" + labelName);
				}
				else
				{
					ChatSystem.Instance.LabelTermItemCommand.SetTerm("Menu/EmptyField");
				}
				NetworkGameManager.Instance.ownPlayer.itemCollision = base.gameObject;
				NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = boxCollider;
				NetworkGameManager.Instance.ownPlayer.functionItemCollision = functionInteract;
			}
		}
		if (CameraGame.Instance.mainCam != null && ChatSystem.Instance.ItemCommand != null && NetworkGameManager.Instance.ownPlayer.itemCollision == base.gameObject)
		{
			Vector3 vector = UIGameManager.Instance.WorldToCanvasPoint(_labelPos.position);
			Transform obj = ChatSystem.Instance.ItemCommand.transform;
			Vector3 vector2 = Vector3.Lerp(b: new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), vector.z), a: obj.localPosition, t: Time.deltaTime * 12f);
			obj.localPosition = new Vector3(Mathf.Round(vector2.x), Mathf.Round(vector2.y), vector2.z);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!other.CompareTag("Player"))
		{
			return;
		}
		if (isAutomaticClose && isTriggered)
		{
			if (animatorTrigger1 != null && animationName1[idxTrigger] != "")
			{
				animatorTrigger1.SetFloat(SpeedAnimation, -1f);
				animatorTrigger1.Play(animationName1[idxTrigger], -1, 1f);
				triggerScanAstar = true;
				AudioManager.PlaySFXTransform(sfxNameReverseTriggered, base.transform, isLocalPlayerTrigger: false);
				if (isTriggerOnce)
				{
					UniTaskUtil.DelayedCall(this, 0.65f, () =>
					{
						animatorTrigger1.enabled = false;
					}).Forget();
				}
			}
			if (animatorTrigger2 != null && animationName2[idxTrigger] != "")
			{
				animatorTrigger2.SetFloat(SpeedAnimation, -1f);
				animatorTrigger2.Play(animationName2[idxTrigger], -1, 1f);
				triggerScanAstar = true;
				if (isTriggerOnce)
				{
					UniTaskUtil.DelayedCall(this, 0.65f, () =>
					{
						animatorTrigger2.enabled = false;
					}).Forget();
				}
			}
			if (isTriggerOnce)
			{
				DisableCollider();
			}
			else
			{
				isTriggered = false;
			}
		}
		if (!(other.transform == NetworkGameManager.Instance.ownPlayer.transform))
		{
			return;
		}
		if (!NetworkGameManager.Instance.ownPlayer.fsmUpperBody.GetBool("isReviving"))
		{
			NetworkGameManager.Instance.ownPlayer.itemCollision = null;
			NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
			NetworkGameManager.Instance.ownPlayer.functionItemCollision = "";
		}
		labelItemCommandOff = false;
		if ((bool)ChatSystem.Instance)
		{
			ChatSystem.Instance.ItemCommand.SetActive(value: false);
		}
		if (isTriggerOnce && isTriggered)
		{
			DisableCollider();
		}
		if (!(NetworkGameManager.Instance.ownPlayer.transform == other.transform))
		{
			return;
		}
		foreach (RoomCollider lightSwitch in lightSwitchList)
		{
			if (lightSwitch != null && !lightSwitch.isCollided)
			{
				lightSwitch.TurnOffLight();
			}
		}
	}

	public void DisableCollider()
	{
		boxCollider.enabled = false;
	}

	public void EnableCollider()
	{
		boxCollider.enabled = true;
		isTriggered = false;
	}

	public void TriggerAnimation(bool isUsedByLocalPlayer, PlayerController playerInteractor = null, bool playSFX = true, float speedAnim = 1f, bool noTriggerReverse = false, bool isForceInteract = false)
	{
		bool flag = false;
		bool flushGraphUpdate = false;
		List<int> list = new List<int>();
		int num = 0;
		for (int i = 0; i < 3; i++)
		{
			list.Add(-1);
		}
		List<int> list2 = new List<int>();
		if (listItemToActivate.Count > 0 && playerInteractor != null)
		{
			for (int num2 = listItemToActivate.Count - 1; num2 >= 0; num2--)
			{
				InventoryObject inventoryObject = playerInteractor.data.FindInventory(listItemToActivate[num2]);
				if (inventoryObject != null)
				{
					list2.Add(inventoryObject.ID);
					list[num] = listItemToActivate[num2];
					num++;
					if (destroyItemNeed)
					{
						playerInteractor.data.RemoveInventory(inventoryObject.IdxInventory, syncNetwork: false);
					}
					bool flag2 = false;
					if (listItemToActivate.Count >= 2 && listItemToActivate[0] == listItemToActivate[1])
					{
						flag2 = true;
					}
					listItemToActivate.RemoveAt(num2);
					if (listItemToActivate.Count == 0)
					{
						flag = true;
					}
					if (flag2)
					{
						break;
					}
				}
			}
		}
		if (isLocked)
		{
			bool flag3 = false;
			if (itemIDUnlock != -1 && !isLockedFromOtherSide && listItemToActivate.Count == 0)
			{
				for (int j = 0; j < playerInteractor.data.arrInventory.Count; j++)
				{
					if (playerInteractor.data.arrInventory[j].ID == itemIDUnlock && playerInteractor.data.arrInventory[j].Name != null)
					{
						triggerScanAstar = true;
						flag3 = true;
						isLocked = false;
						isTriggered = true;
						if (doorCollider != null)
						{
							doorCollider.transform.gameObject.layer = 22;
							GameManager.Instance.AStarPath.UpdateGraphs(doorCollider.bounds);
							flushGraphUpdate = true;
						}
						lockMap.SetActive(value: false);
						if (isUsedByLocalPlayer)
						{
							NetworkGameManager.Instance.ownPlayer.network.ShowBaloonChat(ChatType.UNLOCKED, itemIDUnlock, -1, -1, -1, 10);
							playerInteractor.data.RemoveInventory(j);
						}
					}
				}
			}
			if (itemIDUnlock <= 0 && !isLockedFromOtherSide && listItemToActivate.Count == 0)
			{
				flag3 = true;
				isLocked = false;
				if (animatorTrigger1 == null)
				{
					isTriggered = true;
				}
				if (doorCollider != null)
				{
					doorCollider.transform.gameObject.layer = 22;
					GameManager.Instance.AStarPath.UpdateGraphs(doorCollider.bounds);
					flushGraphUpdate = true;
				}
				lockMap.SetActive(value: false);
				if (isUsedByLocalPlayer)
				{
					NetworkGameManager.Instance.ownPlayer.network.ShowBaloonChat(ChatType.UNLOCKED, list[0], list[1], list[2], -1, 10);
				}
				if (playerInteractor != null)
				{
					RoomCollider roomCollider = GameManager.Instance.GetRoomCollider(playerInteractor.RoomName);
					if ((bool)roomCollider)
					{
						roomCollider.CheckMap(playerInteractor);
					}
				}
			}
			else if (list2.Count > 0)
			{
				if (list2.Count == 1)
				{
					playerInteractor?.network.ShowBaloonChat(ChatType.USE_ITEM, list2[0], -1, -1, -1, 10);
				}
				if (list2.Count == 2)
				{
					playerInteractor?.network.ShowBaloonChat(ChatType.USE_ITEM, list2[0], list2[1], -1, -1, 10);
				}
			}
			if (!flag3 && listItemToActivate.Count == 0)
			{
				if (isLockedFromOtherSide)
				{
					lockMap.SetActive(value: true);
					if (isUsedByLocalPlayer)
					{
						AudioManager.PlaySFX("door-locked");
						NetworkGameManager.Instance.ownPlayer.network.ShowBaloonChat(ChatType.LOCKED_OTHER_SIDE, -1, -1, -1, -1, 10);
					}
					else if (playerInteractor != null)
					{
						playerInteractor.network.ShowBaloonChat(ChatType.LOCKED_OTHER_SIDE, -1, -1, -1, -1, 10);
					}
				}
				else
				{
					lockMap.SetActive(value: true);
					AudioManager.PlaySFX("door-locked");
					if (isUsedByLocalPlayer)
					{
						NetworkGameManager.Instance.ownPlayer.network.ShowBaloonChat(ChatType.LOCKED, -1, -1, -1, UniqueID, 10);
					}
					else if (playerInteractor != null)
					{
						playerInteractor.network.ShowBaloonChat(ChatType.LOCKED, -1, -1, -1, UniqueID, 10);
					}
				}
			}
		}
		if (isShowUI && UIMenu != null && !isUsedByLocalPlayer && !UIMenu.isHidden)
		{
			if (UIMenu.GetComponent(typeof(IPuzzle)) != null)
			{
				IPuzzle puzzle = UIMenu.GetComponent(typeof(IPuzzle)) as IPuzzle;
				if (puzzle.GetInteractableObject() == null || puzzle.GetInteractableObject() == this)
				{
					puzzle.Hide();
					UIMenu.Hide();
					if (!UIGameManager.Instance.isUIInvisible)
					{
						UIGameManager.Instance.uiInGame.Show();
					}
					ChatSystem.Instance.ItemCommand.SetActive(value: false);
					if (LobbyManager.Instance == null)
					{
						UIGameManager.Instance.mapUI.SetActive(value: true);
					}
					if (UIGameManager.Instance.uiObjective != null)
					{
						UIGameManager.Instance.uiObjective.SetActive(value: true);
					}
				}
			}
			if (listItemToActivate.Count == 0 && !isTriggerReverse)
			{
				DisableCollider();
			}
			UniTaskUtil.DelayedCall(this, 0.5f, () =>
			{
				NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: true);
			}).Forget();
			if (functionInteract == "LockPick")
			{
				AudioManager.StopSFX("lockpick_loop");
			}
		}
		bool flag4 = false;
		if (isShowUI || IsClue)
		{
			if (flag || IsClue)
			{
				flag4 = true;
				if (isUsedByLocalPlayer)
				{
					UIMenu.Show();
					if (UIMenu.GetComponent(typeof(IPuzzle)) != null)
					{
						IPuzzle puzzle2 = UIMenu.GetComponent(typeof(IPuzzle)) as IPuzzle;
						if (Password != "")
						{
							puzzle2.SetPassword(Password);
						}
						puzzle2.Show();
						if (playerInteractor != null)
						{
							if (NetworkGameManager.Instance.isServer)
							{
								playerInteractor.network.playerPhoton.IsInteractingPuzzle = true;
							}
							else
							{
								playerInteractor.network.playerPhoton.RpcSetInteractingPuzzle(value: true);
							}
						}
					}
					UIGameManager.Instance.mapUI.SetActive(value: false);
					UIGameManager.Instance.uiTabKill.InstantHide();
					UIGameManager.Instance.uiTabKill.gameObject.SetActive(value: false);
					if (UIMenu.GetComponent<IPuzzle>() != null)
					{
						UIMenu.GetComponent<IPuzzle>().SetInteractableObject(this);
						UIGameManager.Instance.UIMenuPuzzle = UIMenu;
					}
					ChatSystem.Instance.ItemCommand.SetActive(value: false);
					if (playerInteractor != null)
					{
						playerInteractor.network.SetEnableControl(value: false);
						playerInteractor.direction = Vector3.zero;
					}
					UIGameManager.Instance.uiInGame.Hide();
				}
			}
			else if (listItemToActivate.Count == 0)
			{
				isShowUI = false;
				UIMenu = null;
			}
		}
		if (!flag4)
		{
			if (!isLocked && listItemToActivate.Count == 0)
			{
				if ((isTriggerOnce && (!isTriggered | isForceInteract)) || !isTriggerOnce)
				{
					if (syncObject != null && (bool)doorCollider && doorCollider.enabled)
					{
						Bounds bounds = new Bounds(doorCollider.bounds.center, doorCollider.bounds.size * 2f);
						doorCollider.transform.gameObject.layer = 22;
						doorCollider.enabled = false;
						GameManager.Instance.AStarPath.UpdateGraphs(bounds);
						flushGraphUpdate = true;
					}
					if (functionInteract == "Barricade")
					{
						objectBotBarricade.GetComponent<Collider>().enabled = false;
						objectBotBarricade.GetComponent<Rigidbody>().isKinematic = true;
						objectBotBarricade.transform.DOKill();
						DOTween.Kill("DropBotBarricade");
						isBotBroken = false;
						objectBotBarricade.transform.position = posInitObjectBotBarricade;
						objectBotBarricade.transform.localEulerAngles = rotInitObjectBotBarricade;
						objectBotBarricade.SetActive(value: true);
						objectTopBarricade.GetComponent<Collider>().enabled = false;
						objectTopBarricade.GetComponent<Rigidbody>().isKinematic = true;
						objectTopBarricade.transform.DOKill();
						DOTween.Kill("DropTopBarricade");
						isTopBroken = false;
						objectTopBarricade.transform.position = posInitObjectTopBarricade;
						objectTopBarricade.transform.localEulerAngles = rotInitObjectTopBarricade;
						objectTopBarricade.SetActive(value: true);
						Hp = _maxHp;
						NetworkGameManager.Instance.ownPlayer.itemCollision = null;
						NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
						NetworkGameManager.Instance.ownPlayer.functionItemCollision = "";
						animatorBarricade.enabled = true;
						animatorBarricade.Play(animationNameBarricade[1], -1, 0f);
					}
					if (animatorTrigger1 != null)
					{
						if (!triggerOnReverse | noTriggerReverse)
						{
							if (animationName1.Count > 0 && animationName1[idxTrigger] != "")
							{
								animatorTrigger1.SetFloat(SpeedAnimation, speedAnim);
								animatorTrigger1.Play(animationName1[idxTrigger], -1, 0f);
								if (playSFX)
								{
									AudioManager.PlaySFXTransform(sfxNameTriggered, base.transform, isUsedByLocalPlayer);
									if (sfxNameTriggered2 != "")
									{
										AudioManager.PlaySFXTransform(sfxNameTriggered2, base.transform, isUsedByLocalPlayer);
									}
								}
								if (isUsedByLocalPlayer)
								{
									foreach (RoomCollider lightSwitch in lightSwitchList)
									{
										if (lightSwitch != null && !lightSwitch.isCollided)
										{
											lightSwitch.TurnOnLight();
										}
									}
								}
								triggerScanAstar = true;
							}
							else if (SetAnimatorTrigger1 != "")
							{
								animatorTrigger1.SetTrigger(SetAnimatorTrigger1);
								triggerScanAstar = true;
							}
						}
						else
						{
							animatorTrigger1.SetFloat(SpeedAnimation, 0f - speedAnim);
							animatorTrigger1.Play(animationName1[idxTrigger], -1, 1f);
							triggerScanAstar = true;
							if (playSFX)
							{
								AudioManager.PlaySFXTransform(sfxNameReverseTriggered, base.transform, isUsedByLocalPlayer);
							}
							if (isUsedByLocalPlayer)
							{
								foreach (RoomCollider lightSwitch2 in lightSwitchList)
								{
									if (lightSwitch2 != null && !lightSwitch2.isCollided)
									{
										lightSwitch2.TurnOffLight();
									}
								}
							}
						}
					}
					if (animatorTrigger2 != null && animationName2.Count > 0)
					{
						if (!triggerOnReverse | noTriggerReverse)
						{
							animatorTrigger2.SetFloat(SpeedAnimation, speedAnim);
							animatorTrigger2.Play(animationName2[idxTrigger], -1, 0f);
							triggerScanAstar = true;
						}
						else
						{
							animatorTrigger2.SetFloat(SpeedAnimation, 0f - speedAnim);
							animatorTrigger2.Play(animationName2[idxTrigger], -1, 1f);
							triggerScanAstar = true;
						}
					}
					if (!triggerOnReverse && spawnItemID >= 0 && posSpawnItem != null && playerInteractor != null)
					{
						IsSolved = true;
						if (NetworkGameManager.Instance.isServer)
						{
							playerInteractor.network.SetSpawnItem(spawnItemID, posSpawnItem.position);
						}
						spawnItemID = -1;
						spriteMap.enabled = false;
						RoomCollider roomCollider2 = GameManager.Instance.GetRoomCollider(playerInteractor.RoomName);
						if ((bool)roomCollider2)
						{
							roomCollider2.CheckMap(playerInteractor);
						}
					}
					if (isTriggerOnce)
					{
						isTriggered = true;
						if (!isAutomaticClose)
						{
							boxCollider.enabled = false;
						}
						ChatSystem.Instance.ItemCommand.SetActive(value: false);
						if (IconMap != null && IconMap.gameObject.activeSelf)
						{
							IconMap.gameObject.SetActive(value: false);
						}
						if (playerInteractor != null)
						{
							RoomCollider roomCollider3 = GameManager.Instance.GetRoomCollider(playerInteractor.RoomName);
							if ((bool)roomCollider3)
							{
								roomCollider3.CheckMap(playerInteractor);
							}
						}
					}
					else
					{
						if (isTriggerReverse)
						{
							triggerOnReverse = !triggerOnReverse;
							if (noTriggerReverse)
							{
								triggerOnReverse = true;
							}
							if (!triggerOnReverse)
							{
								labelName = labelNameInit;
							}
							else
							{
								labelName = labelNameReverse;
							}
							if (labelName != "")
							{
								ChatSystem.Instance.LabelTermItemCommand.SetTerm("Menu/" + labelName);
							}
						}
						if (idxTrigger < animationName1.Count - 1)
						{
							idxTrigger++;
						}
						else
						{
							idxTrigger = 0;
						}
					}
					for (int num3 = 0; num3 < listEnableHordeSpawnAfterTriggered.Count; num3++)
					{
						if (listEnableHordeSpawnAfterTriggered[num3] != null)
						{
							listEnableHordeSpawnAfterTriggered[num3].canSpawnHordeType = true;
						}
					}
					if (_isAdditionalObjective && (bool)UIMissionObjective.Instance)
					{
						UIMissionObjective.Instance.SetCheckboxAdditionalObjective(_idxAdditionalObjective);
					}
				}
				if (ObjectActiveAfterComplete != null)
				{
					if (playSFX)
					{
						AudioManager.PlaySFXTransform(sfxNameTriggered, base.transform, isUsedByLocalPlayer);
						if (sfxNameTriggered2 != "")
						{
							AudioManager.PlaySFXTransform(sfxNameTriggered2, base.transform, isUsedByLocalPlayer);
						}
					}
					ItemInteractable itemActive = ObjectActiveAfterComplete.GetComponent<ItemInteractable>();
					if (base.isActiveAndEnabled)
					{
						if (DelayObjectActiveAfterComplete > 0f && ShowCountdownLabelBeforeComplete)
						{
							Vector3 vector = UIGameManager.Instance.WorldToCanvasPoint(_labelPos.position);
							ChatSystem.Instance.ObjectEscape.transform.position = new Vector3(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), vector.z);
							ChatSystem.Instance.ObjectEscape.SetActive(value: true);
							ChatSystem.Instance.TextNameEscape.SetTerm(termCountdownBeforeComplete);
							if (!ChatSystem.Instance.timerCountdown.isRunning)
							{
								ChatSystem.Instance.timerCountdown.StartDuration(DelayObjectActiveAfterComplete);
							}
							if ((bool)NetworkGameManager.Instance && NetworkGameManager.Instance.isServer)
							{
								ChatSystem.Instance.SyncingTimerCountdown = Mathf.RoundToInt(ChatSystem.Instance.timerCountdown.interval) - 10;
								GameManagerPhoton.Instance?.RpcSyncTimerCountdown(ChatSystem.Instance.timerCountdown.interval, isStartDuration: true);
							}
							ChatSystem.Instance.ItemInteractableCountdown = this;
							isShowingCountdown = true;
						}
						else
						{
							UniTaskUtil.DelayedCall(this, DelayObjectActiveAfterComplete, () =>
							{
								if (itemActive != null)
								{
									ObjectActiveAfterComplete.SetActive(value: true);
									itemActive.boxCollider.enabled = true;
									if (itemActive != null)
									{
										itemActive.TriggerAnimation(isUsedByLocalPlayer);
									}
								}
								else
								{
									ObjectActiveAfterComplete.SetActive(value: true);
								}
								if (ShowCountdownLabelBeforeComplete)
								{
									if (NetworkGameManager.Instance.arrPlayerController.Count > 1)
									{
										ChatSystem.Instance.TextNameEscape.SetTerm("Menu/AgentsInCircle");
										MissionManager.Instance.IsCountAgentInCircle = true;
									}
									else
									{
										ChatSystem.Instance.ObjectEscape.SetActive(value: false);
										isShowingCountdown = false;
									}
								}
							}).Forget();
						}
					}
				}
				if (ObjectInactiveAfterComplete != null && !ShowCountdownLabelBeforeComplete && base.isActiveAndEnabled)
				{
					UniTaskUtil.DelayedCall(this, DelayObjectActiveAfterComplete, () =>
					{
						ObjectInactiveAfterComplete.layer = 1;
						Collider component = ObjectInactiveAfterComplete.GetComponent<Collider>();
						if (component != null)
						{
							Bounds bounds3 = new Bounds(component.bounds.center, component.bounds.size * 2f);
							component.enabled = false;
							GameManager.Instance.AStarPath.UpdateGraphs(bounds3);
							flushGraphUpdate = true;
						}
						for (int k = 0; k < ObjectInactiveAfterComplete.transform.childCount; k++)
						{
							Collider component2 = ObjectInactiveAfterComplete.transform.GetChild(k).GetComponent<Collider>();
							if (component2 != null)
							{
								Bounds bounds4 = new Bounds(component2.bounds.center, component2.bounds.size);
								component2.enabled = false;
								GameManager.Instance.AStarPath.UpdateGraphs(bounds4);
								flushGraphUpdate = true;
							}
						}
						ObjectInactiveAfterComplete.SetActive(value: false);
					}).Forget();
				}
				if (syncObject != null)
				{
					syncObject.isLocked = false;
					syncObject.isTriggered = true;
					if (syncObject.doorCollider != null)
					{
						syncObject.doorCollider.transform.gameObject.layer = 22;
						Bounds bounds2 = new Bounds(syncObject.doorCollider.bounds.center, syncObject.doorCollider.bounds.size * 2f);
						syncObject.doorCollider.enabled = false;
						GameManager.Instance.AStarPath.UpdateGraphs(bounds2);
						flushGraphUpdate = true;
					}
					if (syncObject.lockMap != null)
					{
						syncObject.lockMap.SetActive(value: false);
					}
					syncObject.isLockedFromOtherSide = false;
					syncObject.isTriggered = isTriggered;
					syncObject.triggerOnReverse = triggerOnReverse;
					syncObject.idxTrigger = idxTrigger;
					syncObject.labelName = labelName;
				}
			}
			ChatSystem.Instance.HideBaloonChat(playerInteractor, this);
		}
		if (doorCollider != null && doorCollider.transform.gameObject.layer == 8 && listItemToActivate.Count == 0 && !isLocked)
		{
			doorCollider.transform.gameObject.layer = 22;
			GameManager.Instance.AStarPath.UpdateGraphs(doorCollider.bounds);
			flushGraphUpdate = true;
		}
		if (flushGraphUpdate)
		{
			GameManager.Instance.AStarPath.FlushGraphUpdates();
		}
		if ((bool)playerInteractor)
		{
			if (showChatID != -1)
			{
				playerInteractor.network.ShowBaloonChat((ChatType)showChatID, -1, -1, -1, -1, 10);
			}
			else if (MonologueID != -1)
			{
				playerInteractor.network.ShowBaloonChat(ChatType.MONOLOGUE, MonologueID, -1, -1, -1, 10);
			}
		}
		if (triggerScanAstar)
		{
			UniTaskUtil.DelayedCall(this, 1f, ScanAstarItemCollider).Forget();
		}
		_interactableCustomFunction?.Execute(playerInteractor);
		onCompleteInteractObjectEvent?.Invoke();
	}

	public IEnumerator SetEnableControl()
	{
		yield return new WaitForSeconds(0.5f);
		NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: true);
	}

	public void BrokeBotBarricade(Vector3 pos)
	{
		animatorBarricade.enabled = false;
		objectBotBarricade.GetComponent<Collider>().enabled = true;
		objectBotBarricade.GetComponent<Rigidbody>().isKinematic = false;
		objectBotBarricade.GetComponent<Rigidbody>().AddForce((base.transform.position - pos).normalized * UnityEngine.Random.Range(2, 4), ForceMode.Impulse);
		isBotBroken = true;
		DOVirtual.DelayedCall(1.5f, DropBotBarricade).SetId("DropBotBarricade");
	}

	public void BrokeTopBarricade(Vector3 pos)
	{
		animatorBarricade.enabled = false;
		objectTopBarricade.GetComponent<Collider>().enabled = true;
		objectTopBarricade.GetComponent<Rigidbody>().isKinematic = false;
		objectTopBarricade.GetComponent<Rigidbody>().AddForce((base.transform.position - pos).normalized * 4f, ForceMode.Impulse);
		isTopBroken = true;
		DOVirtual.DelayedCall(1.5f, DropTopBarricade).SetId("DropTopBarricade");
	}

	private void DropTopBarricade()
	{
		objectTopBarricade.transform.DOMoveY(-0.3f, 1f).SetEase(Ease.OutQuad).OnComplete(() =>
		{
			objectTopBarricade.SetActive(value: false);
			EnableCollider();
			isTopBroken = false;
		});
		objectTopBarricade.GetComponent<Collider>().enabled = false;
		objectTopBarricade.GetComponent<Rigidbody>().isKinematic = true;
	}

	private void DropBotBarricade()
	{
		objectBotBarricade.transform.DOMoveY(-0.3f, 1f).SetEase(Ease.OutQuad).OnComplete(() =>
		{
			objectBotBarricade.SetActive(value: false);
			EnableCollider();
			isBotBroken = false;
		});
		objectBotBarricade.GetComponent<Collider>().enabled = false;
		objectBotBarricade.GetComponent<Rigidbody>().isKinematic = true;
	}

	public bool isNoNeedItem(PlayerController player, bool withChat = true)
	{
		bool flag = false;
		if (listItemToActivate.Count > 0)
		{
			foreach (int item in listItemToActivate)
			{
				if (player != null && player.data.FindInventory(item) != null)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				if (withChat && functionInteract != "RepairCar")
				{
					ChatSystem.Instance.HideBaloonChat(player, this);
					if (listItemToActivate.Count == 1)
					{
						player.network.ShowBaloonChat(ChatType.NEED_2_ITEM, listItemToActivate[0], -1, -1, -1, 10);
					}
					if (listItemToActivate.Count == 2)
					{
						player.network.ShowBaloonChat(ChatType.NEED_2_ITEM, listItemToActivate[0], listItemToActivate[1], -1, -1, 10);
					}
					if (listItemToActivate.Count == 3)
					{
						player.network.ShowBaloonChat(ChatType.NEED_3_ITEM, listItemToActivate[0], listItemToActivate[1], listItemToActivate[2], -1, 10);
					}
				}
				if (isLocked)
				{
					lockMap.SetActive(value: true);
				}
			}
		}
		else
		{
			flag = true;
		}
		return flag;
	}

	public void AttackedBarricade()
	{
		GameManagerPhoton.Instance.RPCBarricadeAttacked((byte)UniqueID, isDebugging: true);
	}

	public void AttackBarricade(bool isDebugging = true)
	{
		if (Hp <= 0)
		{
			return;
		}
		Hp -= 10;
		if (Hp > 0)
		{
			if ((animatorBarricade.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f || !animatorBarricade.enabled) && Hp > MaxHp / 2 && !isTopBroken)
			{
				animatorBarricade.enabled = false;
				objectTopBarricade.transform.DOKill();
				objectTopBarricade.transform.DOShakePosition(0.3f, 0.05f, 15);
			}
			if ((animatorBarricade.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f || !animatorBarricade.enabled) && !isBotBroken)
			{
				animatorBarricade.enabled = false;
				objectBotBarricade.transform.DOKill();
				objectBotBarricade.transform.DOShakePosition(0.3f, 0.05f, 15);
			}
		}
		if (Hp > 0 && Hp <= MaxHp / 2 && !isTopBroken)
		{
			isTopBroken = true;
			if (NetworkGameManager.Instance.isServer)
			{
				GameManager.Instance.ExecBarricadeTopBroken((byte)UniqueID, base.transform.position);
			}
		}
		if (Hp <= 0 && !isBotBroken)
		{
			isBotBroken = true;
			if (NetworkGameManager.Instance.ownPlayer.functionItemCollision == "Barricade")
			{
				NetworkGameManager.Instance.ownPlayer.StopInteractProgress();
			}
			if (NetworkGameManager.Instance.isServer)
			{
				GameManager.Instance.ExecBarricadeBotBroken((byte)UniqueID, base.transform.position);
			}
		}
		if (((Hp > 0) & isDebugging) && NetworkGameManager.Instance.isServer)
		{
			GameManagerPhoton.Instance.RPCBarricadeAttacked((byte)UniqueID, isDebugging: true);
		}
		CheckBarricade();
	}

	private void CheckBarricade()
	{
		if (Hp > 0)
		{
			return;
		}
		if (JumpCollider.ObstaclePath.activeSelf)
		{
			JumpCollider.ObstaclePath.SetActive(value: false);
		}
		foreach (EnemyController item in GameManager.Instance.arrEnemyController)
		{
			if (item.barricadeCollider != null && item.barricadeCollider.barricade == this)
			{
				item.StopAttackBarricade().Forget();
			}
		}
	}
}

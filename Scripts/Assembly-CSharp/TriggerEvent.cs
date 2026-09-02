using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using I2.Loc;
using Toked;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class TriggerEvent : MonoBehaviour
{
	private enum TriggerInvokeType
	{
		KNOCK_KNOCK = 0
	}

	private static readonly int IsShooting = Animator.StringToHash("isShooting");

	[SerializeField]
	private TriggerEventType triggerType;

	[SerializeField]
	private bool isCollided;

	[SerializeField]
	private int _uniqueID = -1;

	[SerializeField]
	private float delayTrigger;

	[SerializeField]
	private Animator anim;

	[SerializeField]
	private string sfxName;

	[SerializeField]
	private float delaySFX;

	[SerializeField]
	private string sfxDelayName;

	[SerializeField]
	private bool isInteractItem;

	[SerializeField]
	private ItemInteractable interactionItem;

	[SerializeField]
	private List<PosEnemy> arrPosEnemy = new List<PosEnemy>();

	[SerializeField]
	private RoomCollider roomCollider;

	[SerializeField]
	private int ctrColliderSpawnEnemy;

	[SerializeField]
	private bool isStartTriggerInvoke;

	[SerializeField]
	private float invokeTime;

	[SerializeField]
	private Animator objectTargetInvoke;

	[SerializeField]
	private ItemInteractable interactObjInvoke;

	[SerializeField]
	private TriggerInvokeType invokeType;

	[SerializeField]
	private bool isShaking;

	[SerializeField]
	private PlayerController targetPlayer;

	[SerializeField]
	private int IDLocalMonologue;

	[SerializeField]
	private int IDLocalMonologue2;

	[SerializeField]
	private int IDLocalMonologue3;

	[SerializeField]
	private int IDItemDrop;

	[SerializeField]
	private bool _alwaysShowChat;

	[SerializeField]
	private bool _isAlwaysEnable;

	[SerializeField]
	private bool _isRegisterMeleeInput;

	[SerializeField]
	private bool _isRegisterRangeInput;

	[SerializeField]
	private bool _isRegisterDashInput;

	[SerializeField]
	private int _idWeaponRangeLocal;

	[SerializeField]
	private float _progress;

	[SerializeField]
	private float _maxProgress;

	[SerializeField]
	private string _termLabelProgress;

	[SerializeField]
	private List<GameObject> _listObjectActivatedAfterTriggered = new List<GameObject>();

	[SerializeField]
	private Collider _collider;

	[SerializeField]
	private HashSet<PlayerController> _insideTrigger = new HashSet<PlayerController>();

	[SerializeField]
	private bool _isGamepad;

	public UnityEvent OnCollided;

	public bool IsCollided
	{
		get
		{
			return isCollided;
		}
		set
		{
			isCollided = value;
		}
	}

	public int UniqueID
	{
		get
		{
			return _uniqueID;
		}
		set
		{
			_uniqueID = value;
		}
	}

	public Animator Anim
	{
		get
		{
			return anim;
		}
		set
		{
			anim = value;
		}
	}

	private void Start()
	{
		if (GetComponent<MeshRenderer>() != null)
		{
			GetComponent<MeshRenderer>().enabled = false;
		}
		_collider = GetComponent<Collider>();
		if (ctrColliderSpawnEnemy == 0)
		{
			if (NetworkGameManager.Instance.arrPlayerController.Count == 1)
			{
				ctrColliderSpawnEnemy = Random.Range(2, 4);
			}
			else if (NetworkGameManager.Instance.arrPlayerController.Count == 2)
			{
				ctrColliderSpawnEnemy = Random.Range(2, 5);
			}
			else if (NetworkGameManager.Instance.arrPlayerController.Count == 3)
			{
				ctrColliderSpawnEnemy = Random.Range(3, 6);
			}
			else
			{
				ctrColliderSpawnEnemy = Random.Range(4, 7);
			}
		}
		if (ctrColliderSpawnEnemy != 1 && triggerType == TriggerEventType.WAKEUP_ENEMIES)
		{
			ctrColliderSpawnEnemy = Random.Range(1, 3);
		}
		if (isStartTriggerInvoke || triggerType == TriggerEventType.KNOCK_KNOCK)
		{
			Invoke(invokeType.ToString(), invokeTime + Random.Range(0f, 0.5f));
		}
		if ((triggerType == TriggerEventType.TUTORIAL_MELEE && GlobalSaveData.instance.optionData.IsTutorialMeleeCleared) || (triggerType == TriggerEventType.TUTORIAL_RANGE && GlobalSaveData.instance.optionData.IsTutorialShootCleared) || (triggerType == TriggerEventType.TUTORIAL_MOVE && GlobalSaveData.instance.optionData.IsTutorialMoveCleared) || (triggerType == TriggerEventType.TUTORIAL_SPRINT && GlobalSaveData.instance.optionData.IsTutorialSprintCleared) || (triggerType == TriggerEventType.TUTORIAL_DASH && GlobalSaveData.instance.optionData.IsTutorialDashCleared))
		{
			foreach (GameObject item in _listObjectActivatedAfterTriggered)
			{
				item.SetActive(value: true);
			}
			isCollided = true;
			_collider.enabled = false;
			base.gameObject.SetActive(value: false);
		}
		if (!GlobalSaveData.instance.optionData.EnableTutorial && (triggerType == TriggerEventType.TUTORIAL_MELEE || triggerType == TriggerEventType.TUTORIAL_RANGE || triggerType == TriggerEventType.TUTORIAL_MOVE || triggerType == TriggerEventType.TUTORIAL_SPRINT || triggerType == TriggerEventType.TUTORIAL_DASH))
		{
			isCollided = true;
			_collider.enabled = false;
			base.gameObject.SetActive(value: false);
		}
		GameManager.Instance?.arrEventTrigger.Add(this);
	}

	public void OnTriggerEnter(Collider other)
	{
		if (isCollided || !other.CompareTag("Player"))
		{
			return;
		}
		PlayerController component = other.GetComponent<PlayerController>();
		_isGamepad = GlobalOptionsManager.Instance.usingGamepad;
		if (triggerType == TriggerEventType.MONOLOGUE_LOCAL)
		{
			if (component.network.isLocalPlayer)
			{
				targetPlayer = component;
				UniTaskUtil.DelayedCall(this, delayTrigger, () =>
				{
					ChatSystem.Instance.ShowBaloonChat(targetPlayer.network.GetIDX(), ChatType.MONOLOGUE, (short)IDLocalMonologue, -1, -1, -1, 10, _alwaysShowChat);
				}).Forget();
				if (!_isAlwaysEnable)
				{
					_collider.enabled = false;
					isCollided = true;
				}
			}
		}
		else if (triggerType == TriggerEventType.TUTORIAL_MELEE)
		{
			if (component.network.isLocalPlayer)
			{
				targetPlayer = component;
				if (!_isRegisterMeleeInput)
				{
					targetPlayer.playerInput.actions.FindActionMap("Player").FindAction("Shoot/Attack").performed += OnAttackPerformed;
					targetPlayer.playerInput.actions.FindActionMap("Player").FindAction("Shoot/Attack").canceled += OnAttackReleased;
					_isRegisterMeleeInput = true;
				}
				UniTaskUtil.DelayedCall(this, delayTrigger, () =>
				{
					ChatSystem.Instance.ShowBaloonChat(targetPlayer.network.GetIDX(), ChatType.TUTORIAL, (short)IDLocalMonologue, -1, -1, -1, 10, _alwaysShowChat);
				}).Forget();
				if (!_isAlwaysEnable)
				{
					_collider.enabled = false;
					isCollided = true;
				}
			}
		}
		else if (triggerType == TriggerEventType.TUTORIAL_RANGE)
		{
			if (component.network.isLocalPlayer)
			{
				targetPlayer = component;
				if (!_isRegisterRangeInput)
				{
					targetPlayer.playerInput.actions.FindActionMap("Player").FindAction("Aim Mode").performed += OnAimPerformed;
					targetPlayer.playerInput.actions.FindActionMap("Player").FindAction("Aim Mode").canceled += OnAimReleased;
					targetPlayer.playerInput.actions.FindActionMap("Player").FindAction("Shoot/Attack").performed += OnAttackPerformed;
					_isRegisterRangeInput = true;
				}
				UniTaskUtil.DelayedCall(this, delayTrigger, () =>
				{
					ChatSystem.Instance.ShowBaloonChat(targetPlayer.network.GetIDX(), ChatType.TUTORIAL, (short)IDLocalMonologue, -1, -1, -1, 10, _alwaysShowChat);
				}).Forget();
				if (!_isAlwaysEnable)
				{
					isCollided = true;
				}
			}
		}
		else if (triggerType == TriggerEventType.TUTORIAL_MOVE)
		{
			if (component.network.isLocalPlayer)
			{
				targetPlayer = component;
				UniTaskUtil.DelayedCall(this, delayTrigger, () =>
				{
					ChatSystem.Instance.ShowBaloonChat(targetPlayer.network.GetIDX(), ChatType.TUTORIAL, (short)IDLocalMonologue, -1, -1, -1, 10, _alwaysShowChat);
				}).Forget();
				if (!_isAlwaysEnable)
				{
					_collider.enabled = false;
					isCollided = true;
				}
			}
		}
		else if (triggerType == TriggerEventType.TUTORIAL_SPRINT)
		{
			if (component.network.isLocalPlayer)
			{
				targetPlayer = component;
				UniTaskUtil.DelayedCall(this, delayTrigger, () =>
				{
					ChatSystem.Instance.ShowBaloonChat(targetPlayer.network.GetIDX(), ChatType.TUTORIAL, (short)IDLocalMonologue, -1, -1, -1, 10, _alwaysShowChat);
				}).Forget();
				if (!_isAlwaysEnable)
				{
					_collider.enabled = false;
					isCollided = true;
				}
			}
		}
		else if (triggerType == TriggerEventType.TUTORIAL_DASH)
		{
			if (component.network.isLocalPlayer)
			{
				targetPlayer = component;
				if (!_isRegisterDashInput)
				{
					targetPlayer.playerInput.actions.FindActionMap("Player").FindAction("Dash").performed += OnDashPerformed;
					_isRegisterDashInput = true;
				}
				UniTaskUtil.DelayedCall(this, delayTrigger, () =>
				{
					ChatSystem.Instance.ShowBaloonChat(targetPlayer.network.GetIDX(), ChatType.TUTORIAL, (short)IDLocalMonologue, -1, -1, -1, 10, _alwaysShowChat);
				}).Forget();
				if (!_isAlwaysEnable)
				{
					_collider.enabled = false;
					isCollided = true;
				}
			}
		}
		else if (triggerType == TriggerEventType.INTERACT_ITEM)
		{
			UniTaskUtil.DelayedCall(this, delayTrigger, () =>
			{
				interactionItem.TriggerAnimation(isUsedByLocalPlayer: true);
			}).Forget();
			isCollided = true;
			_collider.enabled = false;
		}
		else if (triggerType == TriggerEventType.ACTIVATE_OBJECT)
		{
			foreach (GameObject item in _listObjectActivatedAfterTriggered)
			{
				item.SetActive(value: true);
			}
		}
		else if (triggerType == TriggerEventType.SPAWN_ENEMIES)
		{
			ctrColliderSpawnEnemy--;
			int num = Random.Range(0, 100);
			if (ctrColliderSpawnEnemy <= 0 && num <= 20)
			{
				if (NetworkGameManager.Instance.isServer)
				{
					foreach (PosEnemy item2 in arrPosEnemy)
					{
						if ((bool)item2)
						{
							if (item2.posEnter.Count > 0)
							{
								EnemySpawner.Instance.SpawnEnemy(item2, item2.posEnter[Random.Range(0, item2.posEnter.Count)].transform, 0, isHorde: false);
							}
							item2.isSpawnable = true;
						}
					}
				}
				isCollided = true;
				_collider.enabled = false;
			}
		}
		else if (triggerType == TriggerEventType.WAKEUP_ENEMIES)
		{
			if (NetworkGameManager.Instance.isServer)
			{
				ctrColliderSpawnEnemy--;
				if (ctrColliderSpawnEnemy <= 0)
				{
					targetPlayer = component;
					Invoke("TriggerWakeUpEnemies", delayTrigger);
					isCollided = true;
					_collider.enabled = false;
				}
			}
		}
		else if (triggerType == TriggerEventType.KNOCK_KNOCK)
		{
			interactObjInvoke.TriggerAnimation(isUsedByLocalPlayer: false, null, playSFX: false, 1.7f, noTriggerReverse: true);
			AudioManager.PlaySFXTransform("door-slammed", objectTargetInvoke.transform, isLocalPlayerTrigger: false);
			if (NetworkGameManager.Instance.isServer)
			{
				foreach (PosEnemy item3 in arrPosEnemy)
				{
					if (item3 != null && item3.lastEnemySpawned != null)
					{
						PlayerController component2 = other.GetComponent<PlayerController>();
						component2.targetedPoint.position = new Vector3(component2.targetedPoint.position.x, item3.lastEnemySpawned.transform.position.y, component2.targetedPoint.position.z);
						item3.lastEnemySpawned.attack.targetChasing = component2.targetedPoint;
						item3.lastEnemySpawned.isMoveable = true;
						item3.lastEnemySpawned.isAlwaysChasing = true;
						item3.lastEnemySpawned.attack.fov.enabled = true;
						item3.lastEnemySpawned.ChasingObject(item3.lastEnemySpawned.attack.targetChasing, isSightPlayer: true);
						item3.lastEnemySpawned.attack.SetAITarget(item3.lastEnemySpawned.attack.targetChasing);
					}
				}
			}
			isCollided = true;
			_collider.enabled = false;
		}
		else if (triggerType == TriggerEventType.CHASE_TARGET)
		{
			if (NetworkGameManager.Instance.isServer)
			{
				foreach (PosEnemy item4 in arrPosEnemy)
				{
					if (item4 != null && item4.lastEnemySpawned != null)
					{
						PlayerController component3 = other.GetComponent<PlayerController>();
						component3.targetedPoint.position = new Vector3(component3.targetedPoint.position.x, item4.lastEnemySpawned.transform.position.y, component3.targetedPoint.position.z);
						item4.lastEnemySpawned.attack.targetChasing = component3.targetedPoint;
						item4.lastEnemySpawned.isMoveable = true;
						item4.lastEnemySpawned.isAlwaysChasing = true;
						item4.lastEnemySpawned.attack.fov.enabled = true;
						item4.lastEnemySpawned.ChasingObject(item4.lastEnemySpawned.attack.targetChasing, isSightPlayer: true);
						item4.lastEnemySpawned.attack.SetAITarget(item4.lastEnemySpawned.attack.targetChasing);
					}
				}
			}
			isCollided = true;
			_collider.enabled = false;
		}
		else if (triggerType == TriggerEventType.DISABLE_DEAF)
		{
			if (NetworkGameManager.Instance.isServer)
			{
				foreach (PosEnemy item5 in arrPosEnemy)
				{
					if (item5 != null && item5.lastEnemySpawned != null)
					{
						item5.lastEnemySpawned.network.networkPhoton.isDeaf = false;
					}
				}
			}
			isCollided = true;
			_collider.enabled = false;
		}
		else if (triggerType == TriggerEventType.ENVIRONMENT_SEQUENCE)
		{
			targetPlayer = component;
			Invoke("TriggerEnvironmentEvent", delayTrigger);
			isCollided = true;
			_collider.enabled = false;
		}
		else if (triggerType != TriggerEventType.DROP_ITEM)
		{
			anim.Play("Default");
			AudioManager.PlaySFXTransform(sfxName, base.transform, isLocalPlayerTrigger: false);
			StartCoroutine(DelayPlaySFX());
			isCollided = true;
			_collider.enabled = false;
		}
		OnCollided?.Invoke();
		_insideTrigger.Add(component);
	}

	private void OnAimPerformed(InputAction.CallbackContext obj)
	{
		if (_idWeaponRangeLocal == 55 && (bool)targetPlayer)
		{
			ChatSystem.Instance.ShowBaloonChat(targetPlayer.network.GetIDX(), ChatType.TUTORIAL, (short)IDLocalMonologue3, -1, -1, -1, 10, alwaysShowChat: true);
		}
	}

	private void OnAimReleased(InputAction.CallbackContext obj)
	{
		if (_idWeaponRangeLocal == 55 && (bool)targetPlayer)
		{
			ChatSystem.Instance.ShowBaloonChat(targetPlayer.network.GetIDX(), ChatType.TUTORIAL, (short)IDLocalMonologue2, -1, -1, -1, 10, alwaysShowChat: true);
		}
	}

	private void OnDashPerformed(InputAction.CallbackContext obj)
	{
		if (!targetPlayer)
		{
			return;
		}
		if (LobbyManager.Instance != null)
		{
			if (targetPlayer.direction != Vector3.zero && targetPlayer.data.GetStamina() > 0f && !targetPlayer.isDashing)
			{
				IncreaseProgress(1f);
			}
		}
		else
		{
			targetPlayer.playerInput.actions.FindActionMap("Player").FindAction("Dash").performed -= OnDashPerformed;
		}
	}

	private void OnAttackPerformed(InputAction.CallbackContext obj)
	{
		if ((bool)targetPlayer && targetPlayer.data.GetStamina() > 0f && triggerType == TriggerEventType.TUTORIAL_MELEE)
		{
			ChatSystem.Instance.ShowBaloonChat(targetPlayer.network.GetIDX(), ChatType.TUTORIAL, (short)IDLocalMonologue2, -1, -1, -1, 10, alwaysShowChat: true);
		}
		else if (targetPlayer.isAiming && triggerType == TriggerEventType.TUTORIAL_RANGE)
		{
			IncreaseProgress(1f);
		}
	}

	private void OnAttackReleased(InputAction.CallbackContext obj)
	{
		if ((bool)targetPlayer)
		{
			ChatSystem.Instance.ShowBaloonChat(targetPlayer.network.GetIDX(), ChatType.TUTORIAL, (short)IDLocalMonologue, -1, -1, -1, 10, alwaysShowChat: true);
			if (!targetPlayer.weaponController.chargeTimer.isRunning && targetPlayer.isAttacking)
			{
				IncreaseProgress(1f);
			}
		}
	}

	private void IncreaseProgress(float value)
	{
		if (!(_progress < _maxProgress))
		{
			return;
		}
		UIGameManager.Instance.ArrPlayerInfo[targetPlayer.network.GetIDX()].BarProgressTutorialTransform.localScale = new Vector3(_progress / _maxProgress, 1f, 1f);
		_progress += value;
		UIGameManager.Instance.ArrPlayerInfo[targetPlayer.network.GetIDX()].ProgressBarTutorialObject.SetActive(value: true);
		if (triggerType == TriggerEventType.TUTORIAL_MELEE || triggerType == TriggerEventType.TUTORIAL_RANGE || triggerType == TriggerEventType.TUTORIAL_DASH)
		{
			UIGameManager.Instance.ArrPlayerInfo[targetPlayer.network.GetIDX()].TextTutorialText.text = LocalizationManager.GetTranslation("Menu/" + _termLabelProgress) + " [" + _progress + "/" + _maxProgress + "]";
		}
		else
		{
			UIGameManager.Instance.ArrPlayerInfo[targetPlayer.network.GetIDX()].TextTutorialText.text = LocalizationManager.GetTranslation("Menu/" + _termLabelProgress);
		}
		UIGameManager.Instance.ArrPlayerInfo[targetPlayer.network.GetIDX()].BarProgressTutorialTransform.DOScaleX(_progress / _maxProgress, 0.1f);
		if (!Mathf.Approximately(_progress, _maxProgress))
		{
			return;
		}
		foreach (GameObject item in _listObjectActivatedAfterTriggered)
		{
			item.SetActive(value: true);
		}
		isCollided = true;
		_collider.enabled = false;
		ExitCollider(targetPlayer, isDisableProgressBar: false);
		UniTaskUtil.DelayedCall(this, 0.3f, () =>
		{
			UIGameManager.Instance.ArrPlayerInfo[targetPlayer.network.GetIDX()].ProgressBarTutorialObject.SetActive(value: false);
			UIGameManager.Instance.ArrPlayerInfo[targetPlayer.network.GetIDX()].BarProgressTutorialTransform.localScale = new Vector3(0f, 1f, 1f);
			base.gameObject.SetActive(value: false);
		}).Forget();
		if (triggerType == TriggerEventType.TUTORIAL_MELEE)
		{
			GlobalSaveData.instance.optionData.IsTutorialMeleeCleared = true;
		}
		else if (triggerType == TriggerEventType.TUTORIAL_RANGE)
		{
			GlobalSaveData.instance.optionData.IsTutorialShootCleared = true;
		}
		else if (triggerType == TriggerEventType.TUTORIAL_MOVE)
		{
			GlobalSaveData.instance.optionData.IsTutorialMoveCleared = true;
		}
		else if (triggerType == TriggerEventType.TUTORIAL_SPRINT)
		{
			GlobalSaveData.instance.optionData.IsTutorialSprintCleared = true;
		}
		else if (triggerType == TriggerEventType.TUTORIAL_DASH)
		{
			GlobalSaveData.instance.optionData.IsTutorialDashCleared = true;
		}
		if (GlobalSaveData.instance.optionData.IsTutorialMeleeCleared && GlobalSaveData.instance.optionData.IsTutorialShootCleared && GlobalSaveData.instance.optionData.IsTutorialMoveCleared && GlobalSaveData.instance.optionData.IsTutorialSprintCleared && GlobalSaveData.instance.optionData.IsTutorialDashCleared)
		{
			GlobalSaveData.instance.optionData.EnableTutorial = false;
		}
		GlobalSaveData.instance.SaveOptionData();
	}

	private void OnTriggerExit(Collider other)
	{
		if (!other.CompareTag("Player"))
		{
			return;
		}
		PlayerController component = other.GetComponent<PlayerController>();
		if (!(component == null))
		{
			if (triggerType == TriggerEventType.TUTORIAL_MELEE || triggerType == TriggerEventType.TUTORIAL_RANGE || triggerType == TriggerEventType.TUTORIAL_MOVE || triggerType == TriggerEventType.TUTORIAL_SPRINT || triggerType == TriggerEventType.TUTORIAL_DASH)
			{
				ExitCollider(component);
			}
			else if (triggerType == TriggerEventType.MONOLOGUE_LOCAL)
			{
				ExitCollider(component, isDisableProgressBar: false);
			}
			_insideTrigger.Remove(component);
		}
	}

	public void ExitCollider(PlayerController targetPlayer, bool isDisableProgressBar = true)
	{
		if (targetPlayer.network.isLocalPlayer && _alwaysShowChat)
		{
			ChatSystem.Instance.HideBaloonChatMonologue(targetPlayer);
			if (isDisableProgressBar && UIGameManager.Instance.ArrPlayerInfo[NetworkGameManager.Instance.ownPlayer.network.GetIDX()].ProgressBarTutorialObject.activeSelf)
			{
				UIGameManager.Instance.ArrPlayerInfo[NetworkGameManager.Instance.ownPlayer.network.GetIDX()].ProgressBarTutorialObject.SetActive(value: false);
			}
			if (_isRegisterMeleeInput)
			{
				targetPlayer.playerInput.actions.FindActionMap("Player").FindAction("Shoot/Attack").performed -= OnAttackPerformed;
				targetPlayer.playerInput.actions.FindActionMap("Player").FindAction("Shoot/Attack").canceled -= OnAttackReleased;
				_isRegisterMeleeInput = false;
			}
			else if (_isRegisterRangeInput)
			{
				targetPlayer.playerInput.actions.FindActionMap("Player").FindAction("Aim Mode").performed -= OnAimPerformed;
				targetPlayer.playerInput.actions.FindActionMap("Player").FindAction("Aim Mode").canceled -= OnAimReleased;
				targetPlayer.playerInput.actions.FindActionMap("Player").FindAction("Shoot/Attack").performed -= OnAttackPerformed;
				_isRegisterRangeInput = false;
			}
			else if (_isRegisterDashInput)
			{
				targetPlayer.playerInput.actions.FindActionMap("Player").FindAction("Dash").performed -= OnDashPerformed;
				_isRegisterDashInput = false;
			}
		}
	}

	private void FixedUpdate()
	{
		if (_insideTrigger.Count <= 0)
		{
			return;
		}
		foreach (PlayerController item in _insideTrigger)
		{
			if (triggerType == TriggerEventType.DROP_ITEM)
			{
				if (IDItemDrop <= 0 || !item.network.isLocalPlayer || !(item.fsmUpperBody != null) || item.fsmUpperBody.GetBool(IsShooting))
				{
					continue;
				}
				targetPlayer = item;
				if (targetPlayer.data.FindInventory(IDItemDrop) != null)
				{
					AudioManager.PlaySFXTransform(sfxName, base.transform, isLocalPlayerTrigger: false);
				}
				for (int i = 0; i < targetPlayer.data.arrInventory.Count; i++)
				{
					if (targetPlayer.data.arrInventory[i].ID == IDItemDrop)
					{
						targetPlayer.inventoryManager.FunctionItemDrop(i, isSwapWeapon: false, isQuickDrop: true);
						targetPlayer.data.arrInventory[i].ResetData();
					}
				}
			}
			else if (triggerType == TriggerEventType.TUTORIAL_RANGE && _isRegisterRangeInput)
			{
				if (!item.network.isLocalPlayer)
				{
					continue;
				}
				targetPlayer = item;
				if (_idWeaponRangeLocal != targetPlayer.data.arrInventory[1].ID)
				{
					_idWeaponRangeLocal = targetPlayer.data.arrInventory[1].ID;
					if (targetPlayer.data.arrInventory[1].ID == 55)
					{
						ChatSystem.Instance.ShowBaloonChat(targetPlayer.network.GetIDX(), ChatType.TUTORIAL, (short)IDLocalMonologue2, -1, -1, -1, 10, _alwaysShowChat);
					}
					else
					{
						ChatSystem.Instance.ShowBaloonChat(targetPlayer.network.GetIDX(), ChatType.TUTORIAL, (short)IDLocalMonologue, -1, -1, -1, 10, _alwaysShowChat);
					}
				}
			}
			else if (triggerType == TriggerEventType.TUTORIAL_MOVE && DialogueSystem.Instance.IsFinishedIntroDialogue)
			{
				if (item.network.isLocalPlayer)
				{
					if (_isGamepad != GlobalOptionsManager.Instance.usingGamepad)
					{
						ChatSystem.Instance.HideBaloonChat(targetPlayer);
						ChatSystem.Instance.ShowBaloonChat(targetPlayer.network.GetIDX(), ChatType.TUTORIAL, (short)IDLocalMonologue, -1, -1, -1, 10, _alwaysShowChat);
						_isGamepad = GlobalOptionsManager.Instance.usingGamepad;
					}
					if (NetworkGameManager.Instance.ownPlayer.direction != Vector3.zero)
					{
						targetPlayer = item;
						IncreaseProgress(0.03f);
					}
				}
			}
			else if (triggerType == TriggerEventType.TUTORIAL_SPRINT && NetworkGameManager.Instance.ownPlayer.direction != Vector3.zero && NetworkGameManager.Instance.ownPlayer.isSprinting && DialogueSystem.Instance.IsFinishedIntroDialogue && item.network.isLocalPlayer)
			{
				targetPlayer = item;
				IncreaseProgress(0.03f);
			}
		}
	}

	private void TriggerWakeUpEnemies()
	{
		foreach (PosEnemy item in arrPosEnemy)
		{
			if (item != null && item.lastEnemySpawned != null)
			{
				if (item.lastEnemySpawned.network.GetIsHovering() && !item.lastEnemySpawned.network.networkPhoton.isFallingHovering)
				{
					targetPlayer.targetedPoint.position = new Vector3(targetPlayer.targetedPoint.position.x, item.lastEnemySpawned.transform.position.y, targetPlayer.targetedPoint.position.z);
					item.lastEnemySpawned.attack.targetChasing = targetPlayer.targetedPoint;
					item.lastEnemySpawned.network.networkPhoton.isFallingHovering = true;
				}
				else if (item.lastEnemySpawned.isFakeDead && item.lastEnemySpawned.attack.targetChasing == null && targetPlayer != null)
				{
					targetPlayer.targetedPoint.position = new Vector3(targetPlayer.targetedPoint.position.x, item.lastEnemySpawned.transform.position.y, targetPlayer.targetedPoint.position.z);
					item.lastEnemySpawned.attack.targetChasing = targetPlayer.targetedPoint;
					item.lastEnemySpawned.timerStunt.StartDuration(Random.Range(0.5f, 1f));
				}
			}
		}
	}

	private void TriggerEnvironmentEvent()
	{
		if (anim != null)
		{
			anim.CrossFade("Activate", 0f);
		}
		else
		{
			Debug.LogWarning("ENVIRONMENT_SEQUENCE trigger requires Animation Controller to function.");
		}
		if (isShaking)
		{
			CameraGame.Instance.CameraShake();
		}
		if (sfxName != "")
		{
			AudioManager.PlaySFXTransform(sfxName, base.transform, isLocalPlayerTrigger: false);
		}
	}

	private IEnumerator DelayPlaySFX()
	{
		yield return new WaitForSeconds(delaySFX);
		AudioManager.PlaySFXTransform(sfxDelayName, base.transform, isLocalPlayerTrigger: false);
	}

	private void KNOCK_KNOCK()
	{
		if (objectTargetInvoke != null && (objectTargetInvoke.GetCurrentAnimatorStateInfo(0).IsName("Attacked") || objectTargetInvoke.GetCurrentAnimatorStateInfo(0).IsName("New State")))
		{
			if (isCollided)
			{
				interactObjInvoke.TriggerAnimation(isUsedByLocalPlayer: false, null, playSFX: false, 1.7f, noTriggerReverse: true);
				AudioManager.PlaySFXTransform("door-slammed", objectTargetInvoke.transform, isLocalPlayerTrigger: false);
			}
			else
			{
				objectTargetInvoke.Play("Attacked", -1, 0f);
				AudioManager.PlaySFXTransform("door-knockedHard", objectTargetInvoke.transform, isLocalPlayerTrigger: false);
				Invoke(invokeType.ToString(), invokeTime + Random.Range(0f, 0.5f));
			}
		}
		else
		{
			isCollided = true;
		}
	}

	private void OnDisable()
	{
		RemoveEvent();
	}

	private void OnDestroy()
	{
		RemoveEvent();
	}

	private void RemoveEvent()
	{
		if ((bool)targetPlayer)
		{
			if ((bool)UIGameManager.Instance.ArrPlayerInfo[targetPlayer.network.GetIDX()])
			{
				UIGameManager.Instance.ArrPlayerInfo[targetPlayer.network.GetIDX()].ProgressBarTutorialObject.SetActive(value: false);
				UIGameManager.Instance.ArrPlayerInfo[targetPlayer.network.GetIDX()].ChargeMeleeProgressObject.SetActive(value: false);
			}
			if (_isRegisterMeleeInput)
			{
				ChatSystem.Instance.HideBaloonChatMonologue(targetPlayer);
				targetPlayer.playerInput.actions.FindActionMap("Player").FindAction("Shoot/Attack").performed -= OnAttackPerformed;
				targetPlayer.playerInput.actions.FindActionMap("Player").FindAction("Shoot/Attack").canceled -= OnAttackReleased;
				_isRegisterMeleeInput = false;
			}
			if (_isRegisterRangeInput)
			{
				ChatSystem.Instance.HideBaloonChatMonologue(targetPlayer);
				targetPlayer.playerInput.actions.FindActionMap("Player").FindAction("Aim Mode").performed -= OnAimPerformed;
				targetPlayer.playerInput.actions.FindActionMap("Player").FindAction("Aim Mode").canceled -= OnAimReleased;
				targetPlayer.playerInput.actions.FindActionMap("Player").FindAction("Shoot/Attack").performed -= OnAttackPerformed;
				_isRegisterRangeInput = false;
				targetPlayer.fsmUpperBody.SetBool("isShooting", value: false);
			}
			if (_isRegisterDashInput)
			{
				ChatSystem.Instance.HideBaloonChatMonologue(targetPlayer);
				targetPlayer.playerInput.actions.FindActionMap("Player").FindAction("Dash").performed -= OnDashPerformed;
				_isRegisterDashInput = false;
			}
		}
	}

	public void ResetProgress()
	{
		_progress = 0f;
		if ((bool)_collider)
		{
			_collider.enabled = true;
		}
		isCollided = false;
	}
}

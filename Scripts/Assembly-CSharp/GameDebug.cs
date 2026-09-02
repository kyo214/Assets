using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using Toked.Skill;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using _Modules.GameSystem.BaseScripts.Difficulty;

public class GameDebug : MonoBehaviour
{
	[SerializeField]
	private LayerMask wallThroughMask;

	public LayerMask wallColliderMask;

	public bool wallThrough;

	[SerializeField]
	private TMP_InputField textItemID;

	[SerializeField]
	private TMP_InputField textSkillPoints;

	[SerializeField]
	private TMP_InputField _animationSpeedInputField;

	[SerializeField]
	private TMP_Dropdown dropDownSkill;

	[SerializeField]
	private TMP_Dropdown dropDownDifficulty;

	[SerializeField]
	private CraftingManager _craftingManager;

	[SerializeField]
	private TMP_Text _noIngredientsRequireText;

	[SerializeField]
	private TMP_Text _statusEffectDebugText;

	[SerializeField]
	private TMP_Text _statusEffectAllPlayerDebugText;

	[SerializeField]
	private TMP_Text _statsAllDebugText;

	[SerializeField]
	private TMP_Text _enemyDebugText;

	[SerializeField]
	private TMP_Text _wallColliderText;

	[SerializeField]
	private TMP_InputField _scoreText;

	private const int ID_NOTE = 308;

	public TMP_Text RandomizeMaptext;

	[SerializeField]
	private InputActionReference _keyF6;

	[SerializeField]
	private InputActionReference _keyF7;

	private bool _showAllPlayerDebug;

	private bool _showStatusEffectDebug;

	private bool _showAllPlayerStatsDebug;

	private bool _showEnemyDebug;

	public static GameDebug Instance { get; private set; }

	public bool ShowAllPlayerDebug => _showAllPlayerDebug;

	public bool ShowStatusEffectDebug => _showStatusEffectDebug;

	public bool ShowAllPlayerStatsDebug => _showAllPlayerStatsDebug;

	public bool ShowEnemyDebug => _showEnemyDebug;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
			return;
		}
		Instance = this;
		_keyF6.action.Enable();
		_keyF7.action.Enable();
	}

	private void OnEnable()
	{
		_keyF6.action.started += OnPressF6Started;
		_keyF7.action.started += OnPressF7Started;
	}

	private void OnDisable()
	{
		_keyF6.action.started -= OnPressF6Started;
		_keyF7.action.started -= OnPressF7Started;
	}

	private void OnPressF6Started(InputAction.CallbackContext obj)
	{
		if (!(LobbyManager.Instance == null) || !GameManager.Instance || !GameManagerPhoton.Instance || !GameModes.Instance.isDebug)
		{
			return;
		}
		ChatSystem.Instance.timerCountdown.interval = 0.1f;
		foreach (ItemInteractable item in GameManager.Instance.ListBrimCarInteractable)
		{
			if (!item.isActiveAndEnabled)
			{
				continue;
			}
			item.listItemToActivate.Clear();
			ItemInteractable itemInteractable = item.ObjectActiveAfterComplete.GetComponentInChildren<ItemInteractable>();
			if (itemInteractable == null)
			{
				itemInteractable = item.ObjectActiveAfterComplete.GetComponent<ItemInteractable>();
			}
			if (itemInteractable != null)
			{
				item.ObjectActiveAfterComplete.SetActive(value: true);
				itemInteractable.boxCollider.enabled = true;
				item.boxCollider.enabled = false;
			}
			else
			{
				item.ObjectActiveAfterComplete.SetActive(value: true);
			}
			if (item.ShowCountdownLabelBeforeComplete)
			{
				if (NetworkGameManager.Instance.arrPlayerController.Count > 1)
				{
					ChatSystem.Instance.TextNameEscape.SetTerm("Menu/AgentsInCircle");
					MissionManager.Instance.IsCountAgentInCircle = true;
				}
				else
				{
					ChatSystem.Instance.ObjectEscape.SetActive(value: false);
				}
			}
		}
	}

	private void OnPressF7Started(InputAction.CallbackContext obj)
	{
		if (LobbyManager.Instance == null && (bool)GameManagerPhoton.Instance)
		{
			Debug.Log("Press F7");
		}
	}

	private void Start()
	{
		wallThroughMask = (int)wallThroughMask | (1 << LayerMask.NameToLayer("Enemythrough"));
		InitSkillDropDown();
		InitTextRandomizeMap();
		InitDifficulty();
	}

	private void InitSkillDropDown()
	{
		dropDownSkill.ClearOptions();
		List<string> list = new List<string>();
		foreach (SkillScriptableObject data in DataManager.Instance.Get<SkillLibraryScriptableObject>().DataList)
		{
			list.Add(data.ID);
		}
		dropDownSkill.AddOptions(list);
	}

	private void InitDifficulty()
	{
		dropDownDifficulty.ClearOptions();
		List<string> list = new List<string>();
		foreach (DifficultyScriptableObject data in DataManager.Instance.Get<DifficultyScriptableObjectLibrary>().DataList)
		{
			list.Add(data.GetDifficultyData().DifficultySetting.ToString());
		}
		dropDownDifficulty.AddOptions(list);
	}

	public void ClickKillAllEnemies()
	{
		NetworkGameManager.Instance.ownPlayer.network.playerPhoton.RpcKillAllEnemies();
	}

	public void ClickToggleUIVIsibile()
	{
		ToggleUIVIsibile();
	}

	public void ClickUnlockAllDoor()
	{
		NetworkGameManager.Instance.ownPlayer.network.playerPhoton.RpcUnlockAllDoors();
	}

	public void ClickSetGodMode()
	{
		NetworkGameManager.Instance.ownPlayer.network.playerPhoton.RpcSetGodMode();
	}

	public void ClickSetGhostMode()
	{
		NetworkGameManager.Instance.ownPlayer.network.playerPhoton.RpcSetGhostMode();
	}

	public void ClickWallThrough()
	{
		NetworkGameManager.Instance.ownPlayer.network.playerPhoton.RpcWallThrough();
	}

	public void ClickSuperStamina()
	{
		NetworkGameManager.Instance.ownPlayer.network.playerPhoton.RpcSuperStamina();
	}

	public void ClickAmmo999()
	{
		Ammo999();
	}

	public void ClickSpeedMax()
	{
		NetworkGameManager.Instance.ownPlayer.network.playerPhoton.RpcSpeedMax();
	}

	public void ClickShowAllItem()
	{
		NetworkGameManager.Instance.ownPlayer.network.playerPhoton.RpcShowAllItem();
	}

	public void KillAllEnemies()
	{
		foreach (EnemyController item in GameManager.Instance.arrEnemyController)
		{
			item.Hurt(9999f, 0.1f, execShakingCam: true, NetworkGameManager.Instance.ownPlayer.network.GetIDX(), 1);
		}
	}

	public void ToggleUIVIsibile()
	{
		if (UIGameManager.Instance.isUIInvisible)
		{
			UIGameManager.Instance.isUIInvisible = false;
			UIGameManager.Instance.uiInGame.Show();
			UIGameManager.Instance.uiObjective.SetActive(value: true);
			UIGameManager.Instance.mapUI.SetActive(value: true);
			UIGameManager.Instance.uiTabKill.gameObject.SetActive(value: true);
			UIGameManager.Instance.uiDebug.Hide();
			NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: true);
		}
		else
		{
			UIGameManager.Instance.isUIInvisible = true;
			UIGameManager.Instance.uiInGame.Hide();
			UIGameManager.Instance.uiObjective.SetActive(value: false);
			UIGameManager.Instance.mapUI.SetActive(value: false);
			UIGameManager.Instance.uiTabKill.InstantHide();
			UIGameManager.Instance.uiTabKill.gameObject.SetActive(value: false);
			UIGameManager.Instance.uiDebug.Hide();
			NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: true);
		}
	}

	public void UnlockAllDoor()
	{
		foreach (ItemInteractable item in GameManager.Instance.arrItemInteractable)
		{
			if (!item.isLocked && item.Type != InteractableType.DOOR)
			{
				continue;
			}
			item.listItemToActivate.Clear();
			item.itemIDUnlock = -1;
			item.isLocked = false;
			if (item.doorCollider != null)
			{
				item.doorCollider.transform.gameObject.layer = 22;
				if (GameManager.Instance.AStarPath != null)
				{
					GameManager.Instance.AStarPath.UpdateGraphs(item.doorCollider.bounds);
				}
			}
		}
		if (GameManager.Instance.AStarPath != null)
		{
			GameManager.Instance.AStarPath.FlushGraphUpdates();
		}
	}

	public void SetGodMode(PlayerController player)
	{
		player.network.SetGodMode(!player.IsGod);
	}

	public void SetGhostMode(PlayerController player)
	{
		player.SetGhostMode(!player.IsGhost);
		if (player.IsGhost)
		{
			player.characterRenderController.HideCharacter();
			player.flashlight.SetActive(value: false);
			CutoutObject.Instance.wallMask = 0;
			UIGameManager.Instance.ArrPlayerInfo[player.network.GetIDX()].TextPlayerName.text = "";
		}
		else
		{
			player.characterRenderController.ShowCharacter();
			player.flashlight.SetActive(value: true);
			CutoutObject.Instance.enabled = true;
			CutoutObject.Instance.wallMask = CutoutObject.Instance.initWallMask;
			UIGameManager.Instance.ArrPlayerInfo[player.network.GetIDX()].TextPlayerName.text = player.network.GetPlayerName();
		}
		player.playerCollider.SetActive(!player.IsGhost);
	}

	public void ResetCollider(PlayerController player)
	{
		player.network.charControllerPhoton.ExludeLayerCharCollider(wallColliderMask);
	}

	public void WallThrough()
	{
		wallThrough = !wallThrough;
		ObjectCollisionBullet[] array;
		DoorControl[] array2;
		if (wallThrough)
		{
			_wallColliderText.text = "ENABLE WALL COLLIDER";
			foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerNetworkController)
			{
				if (item != null)
				{
					item.network.charControllerPhoton.ExludeLayerCharCollider(wallThroughMask);
				}
			}
			array = Object.FindObjectsByType<ObjectCollisionBullet>(FindObjectsInactive.Include, FindObjectsSortMode.None);
			for (int i = 0; i < array.Length; i++)
			{
				Collider[] components = array[i].GetComponents<Collider>();
				foreach (Collider collider in components)
				{
					if (collider != null)
					{
						collider.enabled = false;
					}
				}
			}
			array2 = Object.FindObjectsByType<DoorControl>(FindObjectsInactive.Include, FindObjectsSortMode.None);
			for (int i = 0; i < array2.Length; i++)
			{
				Collider component = array2[i].GetComponent<Collider>();
				if (component != null)
				{
					component.isTrigger = true;
				}
			}
			return;
		}
		_wallColliderText.text = "DISABLE WALL COLLIDER";
		foreach (PlayerController item2 in NetworkGameManager.Instance.arrPlayerNetworkController)
		{
			if (item2 != null)
			{
				item2.network.charControllerPhoton.ExludeLayerCharCollider(wallColliderMask);
			}
		}
		array = Object.FindObjectsByType<ObjectCollisionBullet>(FindObjectsInactive.Include, FindObjectsSortMode.None);
		for (int i = 0; i < array.Length; i++)
		{
			Collider[] components = array[i].GetComponents<Collider>();
			foreach (Collider collider2 in components)
			{
				if (collider2 != null)
				{
					collider2.enabled = true;
				}
			}
		}
		array2 = Object.FindObjectsByType<DoorControl>(FindObjectsInactive.Include, FindObjectsSortMode.None);
		for (int i = 0; i < array2.Length; i++)
		{
			Collider component2 = array2[i].GetComponent<Collider>();
			if (component2 != null)
			{
				component2.isTrigger = false;
			}
		}
	}

	public void SuperStamina(PlayerController player)
	{
		player.SuperStamina(!player.IsNoStamina);
	}

	public void Ammo999()
	{
		NetworkGameManager.Instance.ownPlayer.data.arrInventory[1].Ammo = 999;
		NetworkGameManager.Instance.ownPlayer.inventoryManager.txtAmountList[1].text = NetworkGameManager.Instance.ownPlayer.data.arrInventory[1].Ammo.ToString();
		UIGameManager.Instance.txtAmountList[1].text = NetworkGameManager.Instance.ownPlayer.data.arrInventory[1].Ammo + "/" + NetworkGameManager.Instance.ownPlayer.weaponController.GetTotalAmmoWeaponString();
	}

	public void SpeedMax(PlayerController player)
	{
		player.SetMaxSpeed(!player.IsMaxSpeed);
		player.data.SetCurrentMoveSpeed(player.data.GetInitialMoveSpeed());
	}

	public void ShowAllItem(bool isNoteOnly = false)
	{
		if (isNoteOnly)
		{
			foreach (ItemPickable item in GameManager.Instance.arrItemPickable)
			{
				if (item.itemID == 308)
				{
					GameManager.Instance.ShowItemMap(item.uniqueID);
				}
			}
			return;
		}
		foreach (ItemPickable item2 in GameManager.Instance.arrItemPickable)
		{
			GameManager.Instance.ShowItemMap(item2.uniqueID);
		}
		foreach (ItemInteractable item3 in GameManager.Instance.arrItemInteractable)
		{
			if (item3.spriteMap != null && item3.spawnItemID >= 0)
			{
				item3.spriteMap.transform.parent = item3.transform.parent.parent;
				item3.spriteMap.transform.localScale = new Vector3(20f, 20f, 20f);
				item3.spriteMap.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
				item3.spriteMap.enabled = true;
				item3.spriteMap.sprite = DataManager.Instance.GetItemSprite(item3.spawnItemID.ToString());
			}
		}
		ObjectCollisionBullet[] array = Object.FindObjectsOfType<ObjectCollisionBullet>(includeInactive: true);
		foreach (ObjectCollisionBullet objectCollisionBullet in array)
		{
			if (objectCollisionBullet.spawnItemID >= 0 && objectCollisionBullet.SpawnItem == ObjectCollisionBullet.SpawnItemMode.SPAWN_ITEM_ID)
			{
				objectCollisionBullet.ItemMap.transform.parent = objectCollisionBullet.transform.parent.parent;
				objectCollisionBullet.ItemMap.transform.localScale = new Vector3(20f, 20f, 20f);
				objectCollisionBullet.ItemMap.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
				objectCollisionBullet.ItemMap.enabled = true;
				objectCollisionBullet.ItemMap.sprite = DataManager.Instance.GetItemSprite(objectCollisionBullet.spawnItemID.ToString());
			}
		}
	}

	public void SpawnItem()
	{
		if (textItemID.text != "")
		{
			NetworkGameManager.Instance.ownPlayer.network.SetSpawnItem(int.Parse(textItemID.text), NetworkGameManager.Instance.ownPlayer.transform.position);
		}
	}

	public void AddSkillPoint()
	{
		if (!string.IsNullOrWhiteSpace(textSkillPoints.text))
		{
			int point = (int.TryParse(textSkillPoints.text, out point) ? point : 0);
			NetworkGameManager.Instance.ownPlayer.data.AddSkillPoint(point);
		}
	}

	public void AddSkill()
	{
		string text = ((dropDownSkill.options.Count > 0) ? dropDownSkill.options[dropDownSkill.value].text : "");
		if (!string.IsNullOrWhiteSpace(text))
		{
			(DataManager.Instance.Get<SkillLibraryScriptableObject>()?.GetData(text))?.ExecuteEffectSkill(NetworkGameManager.Instance.ownPlayer);
		}
	}

	public void ChangeAnimationSpeed()
	{
		if (!string.IsNullOrWhiteSpace(_animationSpeedInputField.text))
		{
			float speed = (float.TryParse(_animationSpeedInputField.text, out speed) ? speed : 0f);
			NetworkGameManager.Instance.ownPlayer.network.ExecSetAdditionalSpeed(speed);
		}
	}

	public void ChangeDifficulty()
	{
		if (dropDownDifficulty.interactable)
		{
			GameModes.Instance.SetDifficultyNetwork((DifficultySetting.Difficulty)dropDownDifficulty.value);
		}
	}

	public void ToggleNoIngredientsRequire()
	{
		if (!(_craftingManager == null))
		{
			bool noIngredientsRequire = _craftingManager.NoIngredientsRequire;
			_craftingManager.SetNoIngredientsRequire(!noIngredientsRequire);
			_noIngredientsRequireText.text = $"Crafting Ingredients Require = {noIngredientsRequire}";
		}
	}

	public void ToggleShowStatusEffectDebug()
	{
		_showStatusEffectDebug = !_showStatusEffectDebug;
		SetActiveStatusEffectUI(_showStatusEffectDebug);
		_statusEffectDebugText.text = $"Show Status Effect Debug = {_showStatusEffectDebug}";
	}

	public void ToggleShowAllStatusEffectDebug()
	{
		_showAllPlayerDebug = !_showAllPlayerDebug;
		SetActiveStatusEffectUI(_showStatusEffectDebug);
		string text = (_showAllPlayerDebug ? "All Players" : "Player Only");
		_statusEffectAllPlayerDebugText.text = "Show Status Effect = " + text;
	}

	public void ToggleShowAllStatsDebug()
	{
		_showAllPlayerStatsDebug = !_showAllPlayerStatsDebug;
		SetActiveStatusEffectUI(_showStatusEffectDebug);
		string text = (_showAllPlayerStatsDebug ? "Speed Stats Only" : "All Stats Modifier");
		_statsAllDebugText.text = "Show All Stats = " + text;
	}

	public void ToggleShowEnemyDebug()
	{
		_showEnemyDebug = !_showEnemyDebug;
		SetActiveEnemyDebugUI(_showEnemyDebug);
		_enemyDebugText.text = $"Show Enemy Debug = {_showEnemyDebug}";
	}

	private void SetActiveStatusEffectUI(bool active)
	{
		if (_showAllPlayerDebug)
		{
			SetActiveAllPlayer(active);
			SetActiveAllPlayerStats(active);
			return;
		}
		SetActiveAllPlayer(setActive: false);
		SetActiveAllPlayerStats(setActive: false);
		NetworkGameManager.Instance.ownPlayer?.StatusEffectController.StatusEffectDebugUI?.gameObject.SetActive(active);
		SetActiveStats();
		static void SetActiveAllPlayer(bool setActive)
		{
			foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerController)
			{
				item.StatusEffectController.StatusEffectDebugUI?.gameObject.SetActive(setActive);
				item.StatsDebugUI?.gameObject.SetActive(setActive);
			}
		}
		void SetActiveAllPlayerStats(bool setActive)
		{
			foreach (PlayerController item2 in NetworkGameManager.Instance.arrPlayerController)
			{
				if (item2 == null)
				{
					break;
				}
				item2.StatsDebugUI?.gameObject.SetActive(setActive);
				item2.SetActiveModifierStatsDebug(_showAllPlayerStatsDebug);
			}
		}
		void SetActiveStats()
		{
			PlayerController ownPlayer = NetworkGameManager.Instance.ownPlayer;
			if (!(ownPlayer == null))
			{
				ownPlayer.StatsDebugUI?.gameObject.SetActive(active);
			}
		}
	}

	private void SetActiveEnemyDebugUI(bool active)
	{
		SetActiveAllEnemy(active);
		static void SetActiveAllEnemy(bool setActive)
		{
			foreach (EnemyController item in GameManager.Instance.arrEnemyController)
			{
				item.StatsDebugUI?.gameObject.SetActive(setActive);
			}
		}
	}

	public void SetReviveFullHealth()
	{
		NetworkGameManager.Instance.ownPlayer.network.SetHealth(NetworkGameManager.Instance.ownPlayer.data.GetMaxHealth());
	}

	public void TriggerHorde()
	{
		GameManager.Instance.waveManager.cueHordeTimer.interval = 0.1f;
		UniTaskUtil.DelayedCall(this, 0.5f, () =>
		{
			GameManager.Instance.waveManager.buildUpHordeTimer.interval = 0.1f;
		}).Forget();
		UniTaskUtil.DelayedCall(this, 1f, () =>
		{
			GameManager.Instance.waveManager.hordeTimer.interval = 0.1f;
		}).Forget();
	}

	public void ShowAllMap()
	{
		if (!MissionLobbyManager.Instance)
		{
			return;
		}
		foreach (MissionSelection item in MissionLobbyManager.Instance.MissionBoard.AllMissionSelection)
		{
			if (!item.MissionData.AlwaysLocked)
			{
				item.MissionData.IsLocked = false;
				item.MissionData.IsHide = false;
				item.SetUI();
				item.GetComponent<Button>().enabled = true;
				item.MapImage.gameObject.SetActive(value: true);
				item.InactiveImage.gameObject.SetActive(value: false);
				item.IconCleared.gameObject.SetActive(value: false);
			}
		}
		GameManagerPhoton.Instance.RPCUnlockAllMap();
	}

	public void InitTextRandomizeMap()
	{
		if ((bool)GameManagerPhoton.Instance && GameManagerPhoton.Instance.IsRandomizeMapOnDefeat)
		{
			RandomizeMaptext.text = "Randomize Map on Defeat=On";
		}
		else
		{
			RandomizeMaptext.text = "Randomize Map on Defeat=Off";
		}
	}

	public void ToggleRandomizeMap()
	{
		if (NetworkGameManager.Instance.isServer)
		{
			GameManagerPhoton.Instance.IsRandomizeMapOnDefeat = !GameManagerPhoton.Instance.IsRandomizeMapOnDefeat;
			if (GameManagerPhoton.Instance.IsRandomizeMapOnDefeat)
			{
				RandomizeMaptext.text = "Randomize Map on Defeat=On";
			}
			else
			{
				RandomizeMaptext.text = "Randomize Map on Defeat=Off";
			}
		}
	}

	public void OnShowAction()
	{
		RefreshDifficultyUI();
		void RefreshDifficultyUI()
		{
			dropDownDifficulty.interactable = NetworkGameManager.Instance.isServer;
			dropDownDifficulty.value = (int)GameModes.Instance.GetDifficultyData().DifficultySetting;
		}
	}

	public void Disconnect()
	{
		GameManager.Instance?.DisconnectFromServer();
	}

	public void SubmitLeaderboard()
	{
		if (!SteamManager.Initialized || GameModes.Instance.isEvent || GameModes.Instance.isInitDemo)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < NetworkGameManager.Instance.arrPlayerNetworkController.Count; i++)
		{
			if (NetworkGameManager.Instance.arrPlayerNetworkController[i] != null)
			{
				num++;
			}
		}
		ScoreManager.Instance.SubmitLeaderboard(NetworkGameManager.Instance.ownPlayer.network.GetIDX(), GameManagerPhoton.Instance.Life, int.Parse(_scoreText.text));
	}
}

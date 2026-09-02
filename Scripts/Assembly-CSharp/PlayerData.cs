using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Toked;
using Toked.Crafting;
using Toked.Inventory;
using Toked.StatusEffect;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using _Modules.CharacterSkin.Scripts;
using _Modules.Player.Data;

public class PlayerData : MonoBehaviour
{
	[SerializeField]
	private int sanityLevel = 1;

	[SerializeField]
	private string carJob;

	[SerializeField]
	private float maxHealth;

	[SerializeField]
	private float sanity;

	[SerializeField]
	private int currentTextStamina;

	[SerializeField]
	private float stamina;

	[SerializeField]
	private float maxStamina;

	[SerializeField]
	private float staminaRegen;

	[SerializeField]
	private float delayStaminaRegen;

	[SerializeField]
	private float maxSanity;

	[SerializeField]
	private int maxInventory;

	[SerializeField]
	private float moveCurrentSpeed;

	[SerializeField]
	private float moveSpeed;

	[SerializeField]
	private float sprintSpeed;

	public float dodgeStamina;

	[SerializeField]
	private float moveBackSpeed;

	[SerializeField]
	private float moveAimSpeed;

	[SerializeField]
	private string weapon;

	public bool isInitReconnect;

	public bool isSyncPosReconnect;

	[SerializeField]
	private MaterialInventoryManager _materialInventoryManager;

	[SerializeField]
	private PlayerSkillData _skillData;

	private PlayerSkillDataNetwork _playerSkillNetworkData;

	[SerializeField]
	private PlayerSkinData _playerSkinData;

	public List<InventoryObject> arrInventory = new List<InventoryObject>();

	public int idThrowable;

	public int idHealing;

	public bool firstInitialized;

	public bool initialized;

	public Transform barHealth;

	public PlayerController playerController;

	public WeaponController weaponController;

	public List<string> ListSpecialCombine = new List<string>();

	private int MAX_INVENTORY_SLOT;

	private static readonly List<int> exceptionItemSaveData = new List<int> { 55 };

	[SerializeField]
	private bool regenActive;

	private Coroutine regenCoroutine;

	public Action<InventoryObject> OnRemoveItemInventoryEvent;

	public string CharacterJob => carJob;

	public MaterialInventoryManager MaterialInventoryManager => _materialInventoryManager;

	public MaterialInventory MainMaterialInventory => _materialInventoryManager.MainMaterialInventory;

	public MaterialInventory MaterialInventory => _materialInventoryManager.InGameMaterialInventory;

	public PlayerSkillData SkillData => _skillData;

	public PlayerSkillDataNetwork PlayerSkillNetworkData => _playerSkillNetworkData ?? (_playerSkillNetworkData = _skillData.GetComponent<PlayerSkillDataNetwork>());

	public PlayerSkinData PlayerSkinData => _playerSkinData;

	public void Init()
	{
		bool initInventory = false;
		if (!initialized || GameModes.Instance.isDemo)
		{
			initialized = true;
			if (!firstInitialized && playerController.network.isLocalPlayer && NetworkGameManager.Instance.isLoadGame)
			{
				LoadSaveData(out initInventory);
			}
			else
			{
				playerController.DizzinessManager.ClearPoints();
				InitPlayer(out initInventory);
			}
		}
		if (firstInitialized || !NetworkGameManager.Instance.isLoadGame)
		{
			SetDefaultInventory(initInventory);
		}
		if (!isInitReconnect)
		{
			MaterialInventoryManager.SyncInGameMaterialInventory();
			PlayerSkillNetworkData.SyncToLocalVariable();
			playerController.network.playerPhoton.SyncVariableToLocal();
			MaterialInventoryManager.SyncMainMaterialInventory();
			playerController.StatusEffectController.SyncStatusEffectController();
			if (playerController.network.isLocalPlayer)
			{
				GlobalSaveData.instance.IsTriggerSaveDataOnInitPlayer = true;
			}
		}
		MAX_INVENTORY_SLOT = arrInventory.Count;
		if (UIGameManager.Instance != null)
		{
			InitImageInventoryLocal();
			UIGameManager.Instance.SetPerkSkillUIInfo(playerController);
			UIGameManager.Instance.uIResultManager?.UILoseResult.Init(GameManagerPhoton.Instance.Life);
		}
		firstInitialized = true;
		isInitReconnect = false;
		if (playerController.network.isLocalPlayer && GlobalSaveData.instance.IsTriggerSaveDataOnInitPlayer)
		{
			GlobalSaveData.instance.IsTriggerSaveDataOnInitPlayer = false;
			GlobalSaveData.instance.SaveGameData(playerController, GameManagerPhoton.Instance);
		}
	}

	protected void InitPlayer(out bool initInventory)
	{
		InitPlayerStats(carJob);
		InitInventory(out initInventory);
		if (firstInitialized)
		{
			return;
		}
		if (GameModes.Instance.isEvent)
		{
			_skillData.AddSkillPoint(1);
			_materialInventoryManager.AddMaterial(MaterialInventoryManager.InventoryType.Auto, 400, 60);
			_materialInventoryManager.AddMaterial(MaterialInventoryManager.InventoryType.Auto, 401, 2);
			_materialInventoryManager.AddMaterial(MaterialInventoryManager.InventoryType.Auto, 402, 2);
			_materialInventoryManager.AddMaterial(MaterialInventoryManager.InventoryType.Auto, 404, 4);
		}
		if (!NetworkGameManager.Instance.isLoadGame)
		{
			playerController.ItemBoxController.InitItemBox();
		}
		if (playerController.network.isLocalPlayer && NetworkGameManager.Instance.isServer)
		{
			if (GlobalSaveData.instance.gameData == null || GlobalSaveData.instance.gameData.Life == 0)
			{
				GlobalSaveData.instance.gameData = new GameData();
				GlobalSaveData.instance.gameData.Seed = GameManagerPhoton.Instance.Seed;
				UIGameManager.Instance.uIResultManager?.UILoseResult.Init(GlobalSaveData.instance.gameData.Life);
			}
			GameManagerPhoton.Instance.SyncVariable(GlobalSaveData.instance.gameData);
		}
	}

	public void InitPlayerStats(string characterJob)
	{
		moveSpeed = BGDatabase_Character.GetEntityByKeyid(characterJob).MoveSpeed * 0.45f;
		sprintSpeed = BGDatabase_Character.GetEntityByKeyid(characterJob).SprintSpeed * 0.45f;
		moveBackSpeed = BGDatabase_Character.GetEntityByKeyid(characterJob).MoveBackwardSpeed * 0.45f;
		moveAimSpeed = BGDatabase_Character.GetEntityByKeyid(characterJob).MoveAimSpeed;
		if (!firstInitialized)
		{
			maxInventory = BGDatabase_Character.GetEntityByKeyid(characterJob).MaxInventory;
			if (NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Solo)
			{
				maxInventory = 8;
			}
			maxHealth = BGDatabase_Character.GetEntityByKeyid(characterJob).Health;
			stamina = BGDatabase_Character.GetEntityByKeyid(characterJob).Stamina;
			maxStamina = stamina;
		}
		if (playerController.network.isLocalPlayer)
		{
			UIGameManager.Instance.txtStaminaValuePlayer.text = Mathf.RoundToInt(GetStamina()) + "/" + maxStamina;
		}
		moveCurrentSpeed = moveSpeed;
		playerController.network.SetHealth(maxHealth, init: true);
		sanity = BGDatabase_Character.GetEntityByKeyid(characterJob).MaxSanity;
		maxSanity = sanity;
		dodgeStamina = BGDatabase_Character.GetEntityByKeyid(characterJob).DodgeStamina;
		staminaRegen = BGDatabase_Character.GetEntityByKeyid(characterJob).StaminaRegen;
		delayStaminaRegen = BGDatabase_Character.GetEntityByKeyid(characterJob).DelayStaminaRegen;
		SetCurrentMoveSpeed(GetInitialMoveSpeed());
	}

	public void InitImageInventoryLocal()
	{
		if (arrInventory.Count < maxInventory)
		{
			return;
		}
		for (int i = 0; i < maxInventory; i++)
		{
			if (i <= 1 && playerController.network.isLocalPlayer)
			{
				UIGameManager.Instance.weaponIconList[i].transform.parent.gameObject.SetActive(value: true);
				if (arrInventory[i].Name != "Null")
				{
					UIGameManager.Instance.weaponIconList[i].sprite = DataManager.Instance.GetItemSprite(arrInventory[i].ID.ToString());
					UIGameManager.Instance.weaponIconList[i].color = new Color(255f, 255f, 255f, 255f);
				}
				else
				{
					UIGameManager.Instance.weaponIconList[i].sprite = null;
					UIGameManager.Instance.weaponIconList[i].color = new Color(255f, 255f, 255f, 0f);
				}
			}
			Image image = null;
			if ((bool)playerController.inventoryManager)
			{
				image = playerController.inventoryManager.inventoryIconList[i];
			}
			if ((bool)image)
			{
				image.transform.gameObject.SetActive(value: true);
			}
			if (playerController.network.isLocalPlayer && (bool)playerController.inventoryManager)
			{
				playerController.inventoryManager.buttonInventory[i].transform.gameObject.SetActive(value: true);
			}
			if (arrInventory[i].Name != "Null")
			{
				if (i == 1 && (bool)playerController.inventoryManager && (bool)playerController.inventoryManager.IconSlotWeapon)
				{
					playerController.inventoryManager.IconSlotWeapon.gameObject.SetActive(value: false);
				}
				if ((bool)image)
				{
					image.sprite = DataManager.Instance.GetItemSprite(arrInventory[i].ID.ToString());
					image.color = new Color(255f, 255f, 255f, 255f);
					image.raycastTarget = false;
				}
			}
			else
			{
				if (i == 1 && (bool)playerController.inventoryManager && (bool)playerController.inventoryManager.IconSlotWeapon)
				{
					playerController.inventoryManager.IconSlotWeapon.gameObject.SetActive(value: true);
				}
				if ((bool)image)
				{
					image.sprite = null;
					image.color = new Color(255f, 255f, 255f, 0f);
					image.raycastTarget = false;
				}
			}
		}
		for (int j = maxInventory; j < 12; j++)
		{
			if ((bool)playerController.inventoryManager)
			{
				playerController.inventoryManager.inventoryIconList[j].transform.gameObject.SetActive(value: false);
				if (playerController.network.isLocalPlayer)
				{
					playerController.inventoryManager.buttonInventory[j].transform.gameObject.SetActive(value: false);
				}
			}
		}
		if (arrInventory.Count > 0 && weaponController.idBaseWeaponRange != -1 && arrInventory[weaponController.idxWeaponRange].Name != "Null" && playerController.network.isLocalPlayer)
		{
			UIGameManager.Instance.txtAmountList[weaponController.idxWeaponRange].text = arrInventory[weaponController.idxWeaponRange].Ammo + "/" + playerController.weaponController.GetTotalAmmoWeaponString();
		}
	}

	private void LoadSaveData(out bool initInventory)
	{
		initInventory = true;
		if (NetworkGameManager.Instance.isServer)
		{
			GlobalSaveData.instance.gameData.LoadData(playerController, GameManagerPhoton.Instance);
		}
		else
		{
			if (isInitReconnect)
			{
				InitClientReconnect(out initInventory);
			}
			if (string.IsNullOrEmpty(_skillData.PerkId) || !isInitReconnect)
			{
				bool flag = GameManagerPhoton.Instance.CheckClientHasSaveData(GlobalSaveData.instance.UserSaveData.UserUniqueId);
				bool flag2 = GlobalSaveData.instance.CheckMultiplayerClientInGameDataExists(GameManagerPhoton.Instance.ServerName);
				Debug.Log("CLIENT HAS SAVE DATA = " + flag);
				Debug.Log("CLIENT MULTIPLAYER INGAME DATA EXIST = " + flag2);
				bool flag3 = flag & flag2;
				NetworkGameManager.Instance.isLoadGame = flag3;
				if (flag3)
				{
					GameData gameData = (GlobalSaveData.instance.gameData = GlobalSaveData.instance.LoadMultiplayerClientGameData(GameManagerPhoton.Instance.ServerName));
					if (gameData == null)
					{
						InitClientNewData(out initInventory);
					}
					else if (gameData.ResetData || gameData.IsCompleted)
					{
						InitClientNewData(out initInventory);
					}
					else
					{
						GlobalSaveData.instance.gameData.LoadData(playerController, GameManagerPhoton.Instance);
					}
					playerController.network.playerPhoton.RPCRequestSyncMap();
				}
				else
				{
					Debug.Log("DELETE SAVE CLIENT DATA");
					DeleteClientSaveFile();
					InitClientNewData(out initInventory);
					if ((bool)MissionLobbyManager.Instance)
					{
						MissionLobbyManager.Instance.InitMap();
					}
				}
			}
		}
		LobbyManager.Instance.SetItemLobby();
		UIGameManager.Instance.uIResultManager?.UILoseResult.Init(GlobalSaveData.instance.gameData.Life);
		if (string.IsNullOrEmpty(_skillData.PerkId))
		{
			UniTaskUtil.DelayedCall(this, 1f, InitPerkSelector).Forget();
		}
		static void DeleteClientSaveFile()
		{
			string[] array = GameManagerPhoton.Instance.ServerName.Split('_');
			GlobalSaveData.DeleteClientSaveFileContains(string.Join("_", array[0], array[1]));
		}
		void InitClientNewData(out bool initInventory2)
		{
			GlobalSaveData instance = GlobalSaveData.instance;
			if (instance.gameData == null)
			{
				instance.gameData = new GameData();
			}
			GlobalSaveData.instance.gameData.Seed = GameManagerPhoton.Instance.Seed;
			InitDefaultPlayer(out initInventory2);
		}
		void InitClientReconnect(out bool initInventory2)
		{
			GlobalSaveData instance = GlobalSaveData.instance;
			if (instance.gameData == null)
			{
				instance.gameData = new GameData();
			}
			GlobalSaveData.instance.gameData.Seed = GameManagerPhoton.Instance.Seed;
			InitReconnectPlayer(out initInventory2);
		}
		void InitDefaultPlayer(out bool reference)
		{
			playerController.ItemBoxController.InitItemBox();
			InitPlayer(out reference);
			SetDefaultInventory(reference);
			GlobalSaveData.instance.IsTriggerSaveDataOnInitPlayer = true;
		}
		void InitReconnectPlayer(out bool reference)
		{
			InitPlayer(out reference);
			SetDefaultInventory(reference);
			MaterialInventoryManager.SyncInGameMaterialInventory();
			PlayerSkillNetworkData.SyncToLocalVariable();
			playerController.network.playerPhoton.SyncVariableToLocal();
			MaterialInventoryManager.SyncMainMaterialInventory();
		}
	}

	private void InitPerkSelector()
	{
		playerController.InitPerkSelector();
	}

	public void SetInventoryData(List<InventoryObject> inventoryDataList)
	{
		arrInventory = NormalizeInventoryData(inventoryDataList);
	}

	public void ApplyInventoryStatusEffect()
	{
		foreach (InventoryObject item in arrInventory)
		{
			ItemScriptableObject itemData = DataManager.Instance.GetItemData(item.ID.ToString());
			if (itemData != null && itemData.ManualApplyEffectItemInventory)
			{
				if (item.equip)
				{
					item.ApplyStatusEffect(playerController.StatusEffectController);
				}
			}
			else
			{
				item.ApplyStatusEffect(playerController.StatusEffectController);
			}
		}
	}

	public List<InventoryObject> GetNormalizeInventoryData()
	{
		return NormalizeInventoryData(arrInventory);
	}

	public static List<InventoryObject> NormalizeInventoryData(List<InventoryObject> inventoryDataList)
	{
		List<InventoryObject> list = new List<InventoryObject>();
		foreach (InventoryObject inventoryData in inventoryDataList)
		{
			if (inventoryData.Name != "Null" && exceptionItemSaveData.Contains(inventoryData.ID) && inventoryData.IdxInventory != 0)
			{
				inventoryData.ResetData();
			}
			list.Add(new InventoryObject(inventoryData));
		}
		return list;
	}

	public int GetWeaponRange()
	{
		return NormalizeWeaponData(weaponController.idWeaponRange);
	}

	public int GetWeaponMelee()
	{
		return NormalizeWeaponData(weaponController.idWeaponMelee, 1);
	}

	public static int NormalizeWeaponData(int itemId, int defaultValue = -1)
	{
		if (!exceptionItemSaveData.Contains(itemId))
		{
			return itemId;
		}
		return defaultValue;
	}

	public void InitInventory(out bool initInventory, bool isInitInventoryPerks = true)
	{
		initInventory = false;
		if (arrInventory.Count == 0)
		{
			if (playerController.network.isLocalPlayer)
			{
				playerController.isRangeActive = false;
				if (!isInitReconnect)
				{
					weaponController.idWeaponMelee = -1;
				}
				weaponController.idWeaponRange = -1;
				weaponController.idBaseWeaponRange = -1;
			}
			weaponController.idxWeaponRange = 1;
			weaponController.prevIdWeaponMelee = -1;
			weaponController.prevIdWeaponRange = -1;
			weaponController.prevAmmoWeaponRange = -1;
			playerController.isRangeActive = false;
			arrInventory.Clear();
			for (int i = 0; i < 12; i++)
			{
				InventoryObject inventoryObject = new InventoryObject();
				inventoryObject.ResetData();
				arrInventory.Add(inventoryObject);
			}
			if (isInitInventoryPerks)
			{
				for (int j = 0; j < 6; j++)
				{
					if (DataManager.Instance.GetValueDatabase("Character", carJob, "Inventory" + j) != 0)
					{
						if ((!firstInitialized && !isInitReconnect) || j <= 1)
						{
							AddInventory(DataManager.Instance.GetValueDatabase("Character", carJob, "Inventory" + j), isOnPick: false, 0, -1, init: true);
						}
						else
						{
							AddInventory(-1);
						}
					}
					else
					{
						AddInventory(-1);
					}
				}
			}
			initInventory = true;
		}
		if (!playerController.network.isLocalPlayer)
		{
			return;
		}
		if (weaponController.idWeaponMelee == -1)
		{
			playerController.network.SetWeapon0(0);
		}
		else
		{
			playerController.network.SetWeapon0(weaponController.idWeaponMelee);
		}
		if (playerController.network.GetIdWeapon1() <= 0 || !isInitReconnect)
		{
			if (weaponController.idWeaponRange <= 0)
			{
				playerController.network.SetWeapon1(0);
			}
			else
			{
				playerController.network.SetWeapon1(weaponController.idWeaponRange);
			}
		}
	}

	private void SetDefaultInventory(bool initInventory)
	{
		if (!playerController.network.isLocalPlayer || initInventory || arrInventory.Count < maxInventory)
		{
			return;
		}
		for (int i = 0; i < maxInventory; i++)
		{
			if (arrInventory[i].ItemType == "HealingItem" && arrInventory[i].IsUsable)
			{
				if (!UIGameManager.Instance.healingItemImage.enabled && playerController.network.isLocalPlayer)
				{
					UIGameManager.Instance.SetHealingShortcutSprite(DataManager.Instance.GetItemSprite(arrInventory[i].ID.ToString()));
					idHealing = arrInventory[i].ID;
					playerController.inventoryManager.txtAmountHealingItem.text = FindTotalInventory(idHealing).ToString();
				}
			}
			else if (arrInventory[i].ID < 100 && arrInventory[i].ID > 0)
			{
				if (BGDatabase_Weapon.GetEntityByKeyid(arrInventory[i].ID).WeaponType == "Throw")
				{
					if (!UIGameManager.Instance.throwableImage.enabled && playerController.network.isLocalPlayer)
					{
						UIGameManager.Instance.SetThrowableShortcutSprite(DataManager.Instance.GetItemSprite(arrInventory[i].ID.ToString()));
						idThrowable = arrInventory[i].ID;
						playerController.canGrenade = true;
					}
					if (playerController.network.isLocalPlayer)
					{
						playerController.inventoryManager.txtAmountThrowableItem.text = FindTotalInventory(idThrowable).ToString();
					}
				}
				else if (BGDatabase_Weapon.GetEntityByKeyid(arrInventory[i].ID).WeaponType == "Range")
				{
					playerController.inventoryManager.txtAmountList[i].gameObject.SetActive(value: true);
					if (i <= 1)
					{
						playerController.inventoryManager.txtAmountList[i].text = arrInventory[i].Ammo + "/" + playerController.weaponController.GetTotalAmmoWeaponString();
					}
					else
					{
						playerController.inventoryManager.txtAmountList[i].text = arrInventory[i].Ammo.ToString();
					}
				}
			}
			else if (arrInventory[i].ItemType == "Ammunition")
			{
				playerController.inventoryManager.txtAmountList[i].gameObject.SetActive(value: true);
				playerController.inventoryManager.txtAmountList[i].text = arrInventory[i].Amount.ToString();
			}
			if (i > 1 && arrInventory[i].ID > 0)
			{
				ItemInventorySlotUI itemInventorySlotUI = playerController.inventoryManager.GetItemInventorySlotUI(i);
				itemInventorySlotUI?.SetActiveArmor(arrInventory[i].Durability.ToString());
				itemInventorySlotUI?.SetActiveEquip(arrInventory[i].equip);
			}
		}
	}

	public void ResetData()
	{
		arrInventory.Clear();
		InitInventory(out var _, isInitInventoryPerks: false);
		if (playerController.network.isLocalPlayer)
		{
			AddInventory(1);
		}
		if (NetworkGameManager.Instance.isServer)
		{
			playerController.network.SetWeapon0(1);
		}
	}

	public void SetCharacterJob(string jobId, bool applyStats = false)
	{
		carJob = jobId;
		if (applyStats)
		{
			InitPlayerStats(jobId);
		}
	}

	public int AddInventory(int inventoryID, bool isOnPick = false, int amount = 0, int itemValueOrAmmo = -1, bool init = false, bool isCombine = false, bool canStacking = true, int uniqueID = -1)
	{
		int num = -1;
		string text;
		if (inventoryID >= 300)
		{
			text = "Item";
		}
		else if (inventoryID >= 200)
		{
			text = "HealingItem";
		}
		else if (inventoryID >= 100)
		{
			text = "Ammunition";
			num = itemValueOrAmmo;
		}
		else
		{
			text = "Weapon";
			num = itemValueOrAmmo;
		}
		if (num >= 255)
		{
			num = -1;
		}
		int num2 = -1;
		bool flag = false;
		if (inventoryID != -1)
		{
			if (FindInventory(inventoryID) != null)
			{
				int num3 = amount;
				if (text == "Ammunition")
				{
					if (amount == 0)
					{
						amount = DataManager.Instance.GetValueDatabase(text, inventoryID.ToString(), "Amount");
					}
				}
				else if (amount == 0)
				{
					amount = 1;
				}
				bool flag2 = true;
				for (int i = 0; i < arrInventory.Count; i++)
				{
					InventoryObject inventoryObject = arrInventory[i];
					if (inventoryObject.ID == -1 && (inventoryObject.IdxInventory > 1 || inventoryObject.IdxInventory == 0))
					{
						flag2 = false;
					}
					if (!((inventoryObject.ID == inventoryID && inventoryObject.Amount < DataManager.Instance.GetValueDatabase(text, inventoryID.ToString(), "MaxItemInSlot") && amount > 0) & canStacking))
					{
						continue;
					}
					if (inventoryObject.Name == "Null")
					{
						inventoryObject.Name = DataManager.Instance.GetValueDatabase(text, inventoryID, "Name");
					}
					if (inventoryObject.Amount + amount <= DataManager.Instance.GetValueDatabase(text, inventoryID.ToString(), "MaxItemInSlot"))
					{
						inventoryObject.Amount += amount;
						amount = 0;
						flag = true;
						num2 = inventoryObject.IdxInventory;
						playerController.inventoryManager.txtAmountList[num2].text = inventoryObject.Amount.ToString();
						if (inventoryObject.ItemType == "Weapon" && playerController.network.isLocalPlayer && BGDatabase_Weapon.GetEntityByKeyid(inventoryObject.ID).WeaponType == "Throw" && inventoryObject.ID == idThrowable)
						{
							playerController.inventoryManager.txtAmountThrowableItem.text = FindTotalInventory(idThrowable).ToString();
						}
						playerController.network.ExecSyncDataInventory(num2, inventoryObject.Amount);
						break;
					}
					if (inventoryObject.Amount + amount > DataManager.Instance.GetValueDatabase(text, inventoryID.ToString(), "MaxItemInSlot"))
					{
						int num4 = inventoryObject.Amount + amount;
						inventoryObject.Amount = DataManager.Instance.GetValueDatabase(text, inventoryID.ToString(), "MaxItemInSlot");
						amount = num4 - DataManager.Instance.GetValueDatabase(text, inventoryID.ToString(), "MaxItemInSlot");
						num2 = inventoryObject.IdxInventory;
						playerController.inventoryManager.txtAmountList[num2].text = inventoryObject.Amount.ToString();
						playerController.network.ExecSyncDataInventory(num2, inventoryObject.Amount);
					}
				}
				if ((amount > 0 && text == "Ammunition" && amount != num3) & flag2)
				{
					playerController.network.SetSpawnItem(inventoryID, playerController.weaponPos.position, amount);
				}
			}
			int num5 = 0;
			if (!flag && (text != "Weapon" || !GameModes.Instance.weaponInBackpack))
			{
				for (int j = 0; j < maxInventory; j++)
				{
					if (arrInventory[j].Name != "Null")
					{
						num5++;
					}
				}
			}
			if (arrInventory.Count > 0 && !flag && (num5 < maxInventory || text == "Weapon"))
			{
				bool isCombinable = false;
				if (text == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(inventoryID).WeaponType == "Range")
				{
					isCombinable = true;
				}
				InventoryObject inventoryObject2 = new InventoryObject
				{
					UniqueID = uniqueID,
					ID = inventoryID,
					Name = DataManager.Instance.GetValueDatabase(text, inventoryID, "Name"),
					IdxInventory = -1,
					ItemType = text,
					IsOpenable = false,
					IsCombinable = isCombinable,
					Amount = 1,
					Durability = -1f
				};
				if (text == "Weapon")
				{
					if (num == -1)
					{
						inventoryObject2.Ammo = DataManager.Instance.GetValueDatabase(text, inventoryID.ToString(), "MagazineSize");
					}
					else
					{
						inventoryObject2.Ammo = num;
					}
					if (BGDatabase_Weapon.GetEntityByKeyid(inventoryID).WeaponType == "Throw")
					{
						if (amount == 0)
						{
							amount = 1;
						}
						inventoryObject2.Amount = amount;
					}
					else
					{
						inventoryObject2.IsEquippable = true;
					}
				}
				else
				{
					if (text == "Item" && BGDatabase_Item.GetEntityByKeyid(inventoryID).Durability > 0)
					{
						if (itemValueOrAmmo == -1)
						{
							itemValueOrAmmo = BGDatabase_Item.GetEntityByKeyid(inventoryID).Durability;
						}
						inventoryObject2.Durability = itemValueOrAmmo;
					}
					inventoryObject2.IsEquippable = false;
				}
				if (text == "HealingItem")
				{
					inventoryObject2.IsUsable = BGDatabase_HealingItem.GetEntityByKeyid(inventoryID).IsUsable;
					inventoryObject2.IsCombinable = true;
				}
				if (text == "Item")
				{
					inventoryObject2.IsCombinable = true;
					inventoryObject2.IsUsable = BGDatabase_Item.GetEntityByKeyid(inventoryID).IsUsable;
					inventoryObject2.IsOpenable = BGDatabase_Item.GetEntityByKeyid(inventoryID).IsOpenable;
					if (amount == 0)
					{
						amount = 1;
					}
					inventoryObject2.Amount = amount;
				}
				if (text == "Ammunition")
				{
					inventoryObject2.Amount = amount;
				}
				inventoryObject2.MaxItemInSlot = DataManager.Instance.GetValueDatabase(text, inventoryID.ToString(), "MaxItemInSlot");
				int num6 = 0;
				int num7 = maxInventory;
				if (!GameModes.Instance.weaponInBackpack && text == "Weapon")
				{
					if (BGDatabase_Weapon.GetEntityByKeyid(inventoryID).WeaponType == "Melee")
					{
						weaponController.prevIdWeaponMelee = weaponController.idWeaponMelee;
					}
					else if (BGDatabase_Weapon.GetEntityByKeyid(inventoryID).WeaponType == "Range")
					{
						weaponController.prevIdWeaponRange = weaponController.idWeaponRange;
						weaponController.prevAmmoWeaponRange = arrInventory[1].Ammo;
					}
					if (BGDatabase_Weapon.GetEntityByKeyid(inventoryID).WeaponType == "Melee")
					{
						if (arrInventory[0].Name != "Null" && playerController.network.GetIdWeapon0() > 0)
						{
							playerController.inventoryManager.FunctionItemDrop(0, isSwapWeapon: true);
							num6 = 0;
							num7 = 1;
						}
					}
					else if (BGDatabase_Weapon.GetEntityByKeyid(inventoryID).WeaponType == "Range")
					{
						if (arrInventory[1].Name != "Null" && playerController.network.GetIdWeapon1() > 0)
						{
							playerController.inventoryManager.FunctionItemDrop(1, isSwapWeapon: true);
							num6 = 1;
							num7 = 2;
						}
					}
					else
					{
						num6 = 2;
					}
				}
				else if (text == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(inventoryID).WeaponType == "Range" && arrInventory[1].Name == "Null")
				{
					num6 = 1;
					num7 = 2;
				}
				else if (text == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(inventoryID).WeaponType == "Melee" && arrInventory[0].Name == "Null")
				{
					num6 = 0;
					num7 = 1;
				}
				else
				{
					num6 = 2;
				}
				for (int k = num6; k < num7; k++)
				{
					if (!(arrInventory[k].Name == "Null"))
					{
						continue;
					}
					inventoryObject2.IdxInventory = k;
					if (playerController.network.isLocalPlayer && (isOnPick | isCombine))
					{
						if (text == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(inventoryID).WeaponType == "Range")
						{
							playerController.network.ExecAddInventory(inventoryObject2.ID, k, inventoryObject2.Ammo, uniqueID);
						}
						else
						{
							playerController.network.ExecAddInventory(inventoryObject2.ID, k, inventoryObject2.Amount, uniqueID, (int)inventoryObject2.Durability);
						}
					}
					arrInventory[k] = inventoryObject2;
					CheckItemInventory(inventoryID, k, text, inventoryObject2, init);
					num2 = k;
					break;
				}
				if (num2 != -1)
				{
					if (text == "HealingItem")
					{
						if (!UIGameManager.Instance.healingItemImage.enabled && playerController.network.isLocalPlayer && inventoryObject2.IsUsable)
						{
							UIGameManager.Instance.SetHealingShortcutSprite(DataManager.Instance.GetItemSprite(inventoryID.ToString()));
							idHealing = inventoryID;
						}
						if (UIGameManager.Instance.healingItemImage.enabled)
						{
							playerController.inventoryManager.txtAmountHealingItem.text = FindTotalInventory(idHealing).ToString();
						}
					}
					ItemScriptableObject itemData = DataManager.Instance.GetItemData(inventoryID.ToString());
					if (itemData != null && itemData.UseCustomEquipInventoryEffect)
					{
						itemData.CustomEquipInventoryEffectSO?.AddItemAction(playerController, inventoryObject2, itemData);
					}
					ItemInventorySlotUI itemInventorySlotUI = playerController.inventoryManager.GetItemInventorySlotUI(num2);
					if (inventoryID > 0 && num2 > 1)
					{
						if (inventoryObject2.Durability > 0f)
						{
							itemInventorySlotUI?.SetActiveArmor(inventoryObject2.Durability.ToString());
						}
						itemInventorySlotUI?.SetActiveEquip(inventoryObject2.equip);
					}
					else
					{
						itemInventorySlotUI?.SetActiveArmor();
						itemInventorySlotUI?.SetActiveEquip(active: false);
					}
				}
			}
		}
		if (UIGameManager.Instance != null)
		{
			InitImageInventoryLocal();
		}
		return num2;
	}

	public void SetStatusEffectItemInventory(int inventorySlotIndex, List<StatusEffectScriptableObject> statusEffectScriptableObjects)
	{
	}

	public void AddObject(short id, byte idxInventory, byte amount, int uniqueID = -1, short durability = -1, bool isSyncReconnect = false)
	{
		string text = "";
		bool isCombinable = false;
		bool isUsable = false;
		bool isOpenable = false;
		bool isEquippable = false;
		int maxItemInSlot = 1;
		if (id >= 300)
		{
			text = "Item";
			isCombinable = true;
			isUsable = BGDatabase_Item.GetEntityByKeyid(id).IsUsable;
			isOpenable = BGDatabase_Item.GetEntityByKeyid(id).IsOpenable;
			maxItemInSlot = BGDatabase_Item.GetEntityByKeyid(id).MaxItemInSlot;
		}
		else if (id >= 200)
		{
			text = "HealingItem";
			isCombinable = true;
			isUsable = true;
			maxItemInSlot = BGDatabase_HealingItem.GetEntityByKeyid(id).MaxItemInSlot;
		}
		else if (id >= 100)
		{
			text = "Ammunition";
			isCombinable = true;
			maxItemInSlot = BGDatabase_Ammunition.GetEntityByKeyid(id).MaxItemInSlot;
		}
		else if (id > 0)
		{
			text = "Weapon";
			maxItemInSlot = BGDatabase_Weapon.GetEntityByKeyid(DataManager.Instance.GetBaseWeapon(id)).MaxItemInSlot;
			isCombinable = true;
			isEquippable = true;
		}
		if (text == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(id).WeaponType == "Range")
		{
			isCombinable = true;
		}
		string text2 = "Null";
		if (id > 0)
		{
			text2 = DataManager.Instance.GetValueDatabase(text, id, "Name");
		}
		InventoryObject newObject = new InventoryObject
		{
			UniqueID = uniqueID,
			ID = id,
			Name = text2,
			IdxInventory = idxInventory,
			ItemType = text,
			IsCombinable = isCombinable,
			IsOpenable = isOpenable,
			IsUsable = isUsable,
			IsEquippable = isEquippable,
			Amount = amount,
			Durability = durability,
			MaxItemInSlot = maxItemInSlot
		};
		if (text == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(id).WeaponType == "Range")
		{
			newObject.Ammo = amount;
			newObject.Amount = 1;
		}
		else
		{
			newObject.Amount = amount;
		}
		if (playerController.network.isLocalPlayer)
		{
			UniTaskUtil.DelayedCall(this, 0.3f, InitItemInventoryEffect).Forget();
		}
		if (idxInventory < arrInventory.Count)
		{
			arrInventory[idxInventory] = newObject;
		}
		if (!UIGameManager.Instance.uiInventory.isHidden)
		{
			NetworkGameManager.Instance.ownPlayer.InitPlayerInventoryBoard();
		}
		if (SurvivorLobbyManager.Instance != null)
		{
			SurvivorLobbyManager.Instance.Show();
		}
		if (playerController.network.isLocalPlayer)
		{
			bool isEquipableWeapon = true;
			if (isSyncReconnect && idxInventory != 1)
			{
				isEquipableWeapon = false;
			}
			CheckItemInventory(id, idxInventory, text, newObject, init: false, isEquipableWeapon);
		}
		void InitItemInventoryEffect()
		{
			ItemScriptableObject itemData = DataManager.Instance.GetItemData(id.ToString());
			if (itemData != null)
			{
				if (itemData.UseCustomEquipInventoryEffect)
				{
					itemData.CustomEquipInventoryEffectSO?.AddItemAction(playerController, newObject, itemData);
				}
				else
				{
					itemData.ItemPickable.ItemIntractableStatusEffect?.Execute(playerController, newObject);
				}
			}
		}
	}

	public void AddItemBox(short id, byte amount, short durability)
	{
		if (id > 0)
		{
			string text = "";
			bool isCombinable = false;
			bool isUsable = false;
			bool isOpenable = false;
			bool isEquippable = false;
			int maxItemInSlot = 1;
			if (id >= 300)
			{
				text = "Item";
				isCombinable = true;
				isUsable = BGDatabase_Item.GetEntityByKeyid(id).IsUsable;
				isOpenable = BGDatabase_Item.GetEntityByKeyid(id).IsOpenable;
				maxItemInSlot = BGDatabase_Item.GetEntityByKeyid(id).MaxItemInSlot;
			}
			else if (id >= 200)
			{
				text = "HealingItem";
				isCombinable = true;
				isUsable = true;
				maxItemInSlot = BGDatabase_HealingItem.GetEntityByKeyid(id).MaxItemInSlot;
			}
			else if (id >= 100)
			{
				text = "Ammunition";
				isCombinable = true;
				maxItemInSlot = BGDatabase_Ammunition.GetEntityByKeyid(id).MaxItemInSlot;
			}
			else if (id > 0)
			{
				text = "Weapon";
				maxItemInSlot = BGDatabase_Weapon.GetEntityByKeyid(DataManager.Instance.GetBaseWeapon(id)).MaxItemInSlot;
				isCombinable = true;
				isEquippable = true;
			}
			if (text == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(id).WeaponType == "Range")
			{
				isCombinable = true;
			}
			string text2 = "Null";
			if (id > 0)
			{
				text2 = DataManager.Instance.GetValueDatabase(text, id, "Name");
			}
			InventoryObject inventoryObject = new InventoryObject
			{
				ID = id,
				Name = text2,
				IdxInventory = 0,
				ItemType = text,
				IsCombinable = isCombinable,
				IsOpenable = isOpenable,
				IsUsable = isUsable,
				IsEquippable = isEquippable,
				Amount = amount,
				MaxItemInSlot = maxItemInSlot,
				Durability = durability
			};
			if (text == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(id).WeaponType == "Range")
			{
				inventoryObject.Ammo = amount;
				inventoryObject.Amount = 1;
			}
			else
			{
				inventoryObject.Amount = amount;
			}
			if (GameModes.Instance.isItemBoxGlobal)
			{
				ItemBoxNetwork.InventoryObjectNetwork newObject = new ItemBoxNetwork.InventoryObjectNetwork
				{
					ID = inventoryObject.ID,
					Ammo = inventoryObject.Ammo,
					Amount = inventoryObject.Amount,
					MaxItemInSlot = inventoryObject.MaxItemInSlot
				};
				ItemBoxNetwork.instance.AddItem(newObject);
			}
			else
			{
				playerController.ItemBoxController.arrItem.Add(inventoryObject);
			}
		}
	}

	public void CheckItemInventory(int inventoryID, int idxSlot, string inventoryType, InventoryObject newObject, bool init = false, bool isEquipableWeapon = true)
	{
		playerController.inventoryManager.GetItemInventorySlotUI(idxSlot)?.ResetAmount();
		switch (inventoryType)
		{
		case "Weapon":
			if (BGDatabase_Weapon.GetEntityByKeyid(inventoryID).WeaponType == "Melee")
			{
				playerController.inventoryManager.GetItemInventorySlotUI(idxSlot)?.ResetAmount();
				if (isEquipableWeapon)
				{
					if (!GameModes.Instance.weaponInBackpack)
					{
						playerController.inventoryManager.WeaponEquip(idxSlot, -1, init);
					}
					else if (weaponController.idWeaponMelee == -1)
					{
						playerController.inventoryManager.WeaponEquip(idxSlot, -1, init);
					}
				}
			}
			else if (BGDatabase_Weapon.GetEntityByKeyid(inventoryID).WeaponType == "Range")
			{
				playerController.inventoryManager.ammoIconList[idxSlot].gameObject.SetActive(value: true);
				playerController.inventoryManager.txtAmountList[idxSlot].gameObject.SetActive(value: true);
				if (idxSlot != 1)
				{
					playerController.inventoryManager.txtAmountList[idxSlot].text = newObject.Ammo.ToString();
				}
				if (isEquipableWeapon)
				{
					Debug.Log("Add Weapon Range");
					if (!GameModes.Instance.weaponInBackpack)
					{
						playerController.inventoryManager.WeaponEquip(idxSlot, newObject.Ammo, init);
					}
					else if (weaponController.idWeaponRange <= 0)
					{
						playerController.inventoryManager.WeaponEquip(idxSlot, newObject.Ammo, init);
					}
				}
			}
			else if (BGDatabase_Weapon.GetEntityByKeyid(inventoryID).WeaponType == "Throw")
			{
				if (!UIGameManager.Instance.throwableImage.enabled && playerController.network.isLocalPlayer)
				{
					UIGameManager.Instance.SetThrowableShortcutSprite(DataManager.Instance.GetItemSprite(inventoryID.ToString()));
					idThrowable = inventoryID;
					playerController.canGrenade = true;
				}
				if (playerController.network.isLocalPlayer)
				{
					playerController.inventoryManager.txtAmountThrowableItem.text = FindTotalInventory(idThrowable).ToString();
				}
			}
			break;
		case "Ammunition":
			if (idxSlot < playerController.inventoryManager.txtAmountList.Count)
			{
				playerController.inventoryManager.txtAmountList[idxSlot].gameObject.SetActive(value: true);
				playerController.inventoryManager.txtAmountList[idxSlot].text = newObject.Amount.ToString();
			}
			break;
		case "HealingItem":
			if (!UIGameManager.Instance.healingItemImage.enabled && playerController.network.isLocalPlayer)
			{
				UIGameManager.Instance.SetHealingShortcutSprite(DataManager.Instance.GetItemSprite(inventoryID.ToString()));
				idHealing = inventoryID;
				playerController.inventoryManager.txtAmountHealingItem.text = FindTotalInventory(idHealing).ToString();
			}
			break;
		default:
			if (inventoryType != "Null" && newObject.MaxItemInSlot > 1)
			{
				playerController.inventoryManager.txtAmountList[idxSlot].gameObject.SetActive(value: true);
				playerController.inventoryManager.txtAmountList[idxSlot].text = newObject.Amount.ToString();
			}
			break;
		}
		ItemInventorySlotUI itemInventorySlotUI = playerController.inventoryManager.GetItemInventorySlotUI(idxSlot);
		if (inventoryID > 0 && idxSlot > 1)
		{
			if (newObject.Durability > 0f)
			{
				itemInventorySlotUI?.SetActiveArmor(newObject.Durability.ToString());
			}
			itemInventorySlotUI?.SetActiveEquip(newObject.equip);
		}
		else
		{
			itemInventorySlotUI?.SetActiveArmor();
			itemInventorySlotUI?.SetActiveEquip(active: false);
		}
	}

	public void RemoveInventoryOtherPlayer(int idx, bool isDuplicateItem = false, int itemAmount = 0)
	{
		if (idx >= arrInventory.Count)
		{
			return;
		}
		if (arrInventory[idx].ItemType == "Weapon" || arrInventory[idx].ItemType == "HealingItem" || arrInventory[idx].ItemType == "Item" || !isDuplicateItem || !playerController.network.isLocalPlayer)
		{
			if (playerController.network.isLocalPlayer & isDuplicateItem)
			{
				if (arrInventory[idx].ID == idHealing)
				{
					int num = FindTotalInventory(idHealing) - 1;
					playerController.inventoryManager.txtAmountHealingItem.text = num.ToString();
					if (num <= 0)
					{
						UIGameManager.Instance.HideHealingShortcutSprite();
						playerController.inventoryManager.txtAmountHealingItem.text = "";
						idHealing = -1;
					}
				}
				if (arrInventory[idx].ID == idThrowable)
				{
					int num2 = FindTotalInventory(idThrowable) - 1;
					playerController.inventoryManager.txtAmountThrowableItem.text = num2.ToString();
					if (num2 <= 0)
					{
						UIGameManager.Instance.HideThrowableShortcutSprite();
						playerController.inventoryManager.txtAmountThrowableItem.text = "";
						idThrowable = -1;
						playerController.canGrenade = false;
					}
				}
			}
			if (playerController.network.isLocalPlayer)
			{
				if ((arrInventory[idx].ID != -1 && arrInventory[idx].ItemType == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(arrInventory[idx].ID).WeaponType != "Throw") & isDuplicateItem)
				{
					if (idx == 0)
					{
						if (playerController.weaponController.prevIdWeaponMelee == -1)
						{
							playerController.weaponController.idWeaponMelee = -1;
							playerController.weaponController.idxSkinWeapon0 = -1;
							playerController.weaponController.meleeObject.SetActive(value: false);
							InitImageInventoryLocal();
							arrInventory[idx].ResetData();
						}
						else
						{
							arrInventory[idx].ResetData();
							AddInventory(playerController.weaponController.prevIdWeaponMelee, isOnPick: true);
						}
					}
					if (idx == 1)
					{
						if (playerController.weaponController.prevIdWeaponRange == -1)
						{
							playerController.weaponController.idWeaponRange = -1;
							playerController.weaponController.idBaseWeaponRange = -1;
							playerController.weaponController.idxSkinWeapon1 = -1;
							playerController.isRangeActive = false;
							UIGameManager.Instance.ammoIconList[1].gameObject.SetActive(value: false);
							UIGameManager.Instance.txtAmountList[1].gameObject.SetActive(value: false);
							InitImageInventoryLocal();
							arrInventory[idx].ResetData();
						}
						else
						{
							arrInventory[idx].ResetData();
							AddInventory(playerController.weaponController.prevIdWeaponRange, isOnPick: true, 0, playerController.weaponController.prevAmmoWeaponRange);
						}
					}
				}
				else
				{
					arrInventory[idx].ResetData();
				}
			}
			else
			{
				arrInventory[idx].ResetData();
			}
		}
		else if (playerController.network.isLocalPlayer)
		{
			if (arrInventory[idx].Amount == itemAmount)
			{
				playerController.inventoryManager.txtAmountList[idx].gameObject.SetActive(value: false);
				arrInventory[idx].ResetData();
			}
			else if (arrInventory[idx].Amount > itemAmount)
			{
				arrInventory[idx].Amount = arrInventory[idx].Amount - itemAmount;
				playerController.inventoryManager.txtAmountList[idx].text = arrInventory[idx].Amount.ToString();
			}
			else
			{
				playerController.inventoryManager.txtAmountList[idx].gameObject.SetActive(value: false);
				arrInventory[idx].Name = "Null";
				itemAmount -= arrInventory[idx].Amount;
				for (int num3 = arrInventory.Count - 1; num3 >= 0; num3--)
				{
					if (arrInventory[num3].Amount >= itemAmount && arrInventory[num3].Name != "Null" && arrInventory[num3].ID == arrInventory[idx].ID && idx != num3)
					{
						arrInventory[num3].Amount -= itemAmount;
						playerController.inventoryManager.txtAmountList[num3].text = arrInventory[num3].Amount.ToString();
						break;
					}
				}
				arrInventory[idx].ResetData();
			}
		}
		if (UIGameManager.Instance != null && playerController.network.isLocalPlayer)
		{
			InitImageInventoryLocal();
		}
	}

	public void RemoveInventory(int idx, bool syncNetwork = true, bool duplicateItem = false, int itemAmount = 0)
	{
		InventoryObject obj = new InventoryObject(arrInventory[idx]);
		arrInventory[idx].ResetDataAndRemoveEffect(playerController.StatusEffectController);
		if ((bool)playerController.inventoryManager)
		{
			playerController.inventoryManager.GetItemInventorySlotUI(idx)?.ResetAmount();
		}
		if (syncNetwork)
		{
			if (duplicateItem)
			{
				playerController.network.ExecRemoveInventoryDuplicate(idx, itemAmount);
			}
			else
			{
				playerController.network.ExecRemoveInventory(idx);
			}
		}
		if (idx <= 1)
		{
			weaponController.UnEquipWeapon(idx, fromServer: false);
		}
		if (UIGameManager.Instance != null)
		{
			InitImageInventoryLocal();
		}
		OnRemoveItemInventoryEvent?.Invoke(obj);
	}

	public void RemoveInventoryData(int idx, bool syncNetwork = true)
	{
		InventoryObject obj = new InventoryObject(arrInventory[idx]);
		arrInventory[idx].ResetDataAndRemoveEffect(playerController.StatusEffectController);
		if (syncNetwork)
		{
			playerController.network.ExecRemoveInventoryData(idx);
		}
		OnRemoveItemInventoryEvent?.Invoke(obj);
	}

	public InventoryObject FindInventory(int inventoryID)
	{
		InventoryObject inventoryObject = arrInventory.Find((InventoryObject x) => x.ID == inventoryID);
		if (inventoryObject?.Name == "Null")
		{
			inventoryObject = null;
		}
		return inventoryObject;
	}

	public List<InventoryObject> GetAllInventoryData()
	{
		List<InventoryObject> list = new List<InventoryObject>();
		foreach (InventoryObject item in arrInventory)
		{
			if (item.ID != -1)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public int FindTotalInventory(int inventoryID)
	{
		int num = 0;
		foreach (InventoryObject item in arrInventory)
		{
			if (item.ID == inventoryID)
			{
				num += item.Amount;
			}
		}
		return num;
	}

	public bool CheckInventory(int inventoryID, int amount)
	{
		return FindTotalInventory(inventoryID) >= amount;
	}

	public void DecreaseSanity(float decreaseValue)
	{
		sanity -= decreaseValue;
	}

	public float GetMaxHealth()
	{
		return maxHealth;
	}

	public int GetMaxInventory()
	{
		return maxInventory;
	}

	public void SetMaxInventoryLocal(int value)
	{
		maxInventory = value;
	}

	public bool IsMaxSlotInventory()
	{
		return maxInventory == MAX_INVENTORY_SLOT;
	}

	public float GetCurrentMoveSpeed()
	{
		return moveCurrentSpeed * playerController.PlayerMultiplyStatsData.GetMultiplyMovementSpeed();
	}

	public float GetInitialMoveSpeed()
	{
		return moveSpeed * playerController.PlayerMultiplyStatsData.GetMultiplyMovementSpeed();
	}

	public float GetSprintSpeed()
	{
		return sprintSpeed * playerController.PlayerMultiplyStatsData.GetMultiplyMovementSpeed() * playerController.PlayerMultiplyStatsData.GetMultiplySprintSpeed();
	}

	public float GetMoveAimSpeed()
	{
		if (playerController.isAiming)
		{
			return weaponController.speedAim * playerController.PlayerMultiplyStatsData.GetMultiplyRangePenaltyMove();
		}
		return moveAimSpeed * playerController.PlayerMultiplyStatsData.GetMultiplyMeleePenaltyMove();
	}

	public float GetStamina()
	{
		if (stamina < 0f)
		{
			stamina = 0f;
		}
		else
		{
			stamina = Mathf.Min(stamina, GetCurrentMaxStamina());
		}
		return stamina;
	}

	public float GetMaxStamina()
	{
		return maxStamina;
	}

	public float GetCurrentMaxStamina()
	{
		return Mathf.Max(1f, maxStamina * playerController.PlayerMultiplyStatsData.GetMultiplyStamina());
	}

	public void SetCurrentMoveSpeed(float _speed)
	{
		if (playerController.isLowHealth)
		{
			_speed *= 0.7f;
		}
		if (playerController.IsMaxSpeed)
		{
			_speed *= 3f;
		}
		else if (playerController.IsSpeedIncreaseBy2)
		{
			_speed *= 1.2f;
		}
		DOTween.To(() => moveCurrentSpeed, (float x) =>
		{
			moveCurrentSpeed = x;
		}, _speed, 0.5f);
		if (GameModes.Instance.isDebug)
		{
			playerController.PlayerMultiplyStatsData.OnPlayerStatsChangedEvents?.Invoke(null);
			UniTaskUtil.DelayedCall(playerController.PlayerMultiplyStatsData, 0.53f, () =>
			{
				playerController.PlayerMultiplyStatsData.OnPlayerStatsChangedEvents?.Invoke(null);
			}).Forget();
		}
	}

	public void AddSlotInventory(int addSlot = 2)
	{
		int num = Math.Clamp(maxInventory + addSlot, maxInventory, MAX_INVENTORY_SLOT);
		maxInventory = num;
		SyncMaxInventory(maxInventory);
	}

	public void SyncMaxInventory(int maxPlayerInventory)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			playerController.network.playerPhoton.MaxInventorySlot = (byte)maxPlayerInventory;
		}
		else
		{
			playerController.network.playerPhoton.RpcSetMaxInventory((byte)maxPlayerInventory);
		}
	}

	public void TransferMaterialToMainInventory(Dictionary<string, MaterialInventoryData> totalMaterialInventoryDic)
	{
		foreach (KeyValuePair<string, MaterialInventoryData> item in totalMaterialInventoryDic)
		{
			item.Value.CraftMaterialScriptableObject.AddMaterial(this, item.Value.Amount, MaterialInventoryManager.InventoryType.Main);
		}
		MaterialInventoryManager.SyncMainMaterialInventory();
	}

	public void AddMaxHealth(int addHealth, bool isIncreassedByPerks)
	{
		if (isIncreassedByPerks)
		{
			if (NetworkGameManager.Instance.isServer)
			{
				if (playerController.network.isLocalPlayer)
				{
					SetHealth(maxHealth + (float)addHealth);
				}
				else
				{
					SetHealth(maxHealth);
				}
			}
			else if (playerController.network.isLocalPlayer)
			{
				SetHealth(maxHealth + (float)addHealth);
			}
		}
		else
		{
			SetHealth(maxHealth + (float)addHealth);
		}
	}

	public void SetLocalMaxHealth(float newMaxHealth)
	{
		maxHealth = Mathf.RoundToInt(newMaxHealth);
		if (playerController.network.isLocalPlayer)
		{
			Debug.Log(playerController.network.GetPlayerName() + " Set Local Max Health = " + newMaxHealth);
			UIGameManager.Instance.txtHpValuePlayer.text = Mathf.RoundToInt(playerController.network.GetHealth()) + "/" + maxHealth;
			playerController.network.playerPhoton.CheckPlayerDying(playerController);
		}
	}

	public void SetLocalMaxStamina(float newMaxStamina, bool isSyncToAllPlayer = false)
	{
		maxStamina = Mathf.RoundToInt(newMaxStamina);
		stamina = maxStamina;
		if (playerController.network.isLocalPlayer)
		{
			UIGameManager.Instance.txtStaminaValuePlayer.text = Mathf.RoundToInt(GetStamina()) + "/" + maxStamina;
		}
	}

	public void SetHealth(float health)
	{
		Debug.Log(playerController.network.GetPlayerName() + " Set Max Health = " + health);
		maxHealth = health;
		playerController.network.SetHealth(maxHealth);
		playerController.network.playerPhoton.RpcSyncMaxHealth(maxHealth);
		if (playerController.network.isLocalPlayer)
		{
			UIGameManager.Instance.txtHpValuePlayer.text = maxHealth + "/" + maxHealth;
		}
	}

	public void AddMaxStamina(float addStamina, bool isIncreasedByPerks)
	{
		if (isIncreasedByPerks)
		{
			if (NetworkGameManager.Instance.isServer)
			{
				if (playerController.network.isLocalPlayer)
				{
					SetStamina(maxStamina + addStamina);
				}
				else
				{
					SetStamina(maxStamina);
				}
			}
			else if (playerController.network.isLocalPlayer)
			{
				SetStamina(maxStamina + addStamina);
			}
		}
		else
		{
			SetStamina(maxStamina + addStamina);
		}
	}

	public void SetStamina(float setStamina)
	{
		maxStamina = (stamina = setStamina);
		if (playerController.network.isLocalPlayer)
		{
			UIGameManager.Instance.txtStaminaValuePlayer.text = Mathf.RoundToInt(GetStamina()) + "/" + maxStamina;
			DOTween.To(() => stamina, (float x) =>
			{
				stamina = x;
			}, GetCurrentMaxStamina(), staminaRegen).SetSpeedBased(isSpeedBased: true).SetDelay(delayStaminaRegen)
				.OnUpdate(SetStaminaUI)
				.SetId("StmReg")
				.SetEase(Ease.Linear);
		}
		if (playerController.IsMale)
		{
			AudioManager.PlaySFX("male-stamina-recover");
		}
		else
		{
			AudioManager.PlaySFX("female-stamina-recover");
		}
		playerController.network.playerPhoton.RpcSyncMaxStamina(GetMaxStamina());
	}

	public void AddSubCurrentStamina(float value, bool recoveryStamina = true)
	{
		if (stamina > 0f)
		{
			SetCurrentStamina(Mathf.Min(stamina + value, GetCurrentMaxStamina()), recoveryStamina);
		}
		else if (!recoveryStamina)
		{
			DOTween.Kill("StmReg");
		}
	}

	public void RecoverCurrentStamina()
	{
		SetCurrentStamina(stamina);
	}

	private void SetCurrentStamina(float value, bool recoveryStamina = true)
	{
		DOTween.Kill("StmReg");
		if (!playerController.IsNoStamina)
		{
			stamina = value;
		}
		if (currentTextStamina != Mathf.RoundToInt(GetStamina()))
		{
			UIGameManager.Instance.txtStaminaValuePlayer.text = Mathf.RoundToInt(GetStamina()) + "/" + maxStamina;
		}
		currentTextStamina = Mathf.RoundToInt(GetStamina());
		float multiplyStaminaRecovery = playerController.PlayerMultiplyStatsData.GetMultiplyStaminaRecovery();
		float num = 1f;
		if (multiplyStaminaRecovery > 1f)
		{
			num = playerController.PlayerMultiplyStatsData.GetMultiplyStaminaRecovery() / 3f;
		}
		if (stamina <= 0f)
		{
			playerController.sweatVFX.enabled = true;
			playerController.SetBtnSprint(newIsBtnSprintDown: false);
			stamina = 0f;
			if (playerController.IsMale)
			{
				AudioManager.PlaySFX("male-stamina-low");
			}
			else
			{
				AudioManager.PlaySFX("female-stamina-low");
			}
			if (recoveryStamina)
			{
				DOTween.To(() => stamina, (float x) =>
				{
					stamina = x;
				}, GetCurrentMaxStamina(), staminaRegen * 2.3f * multiplyStaminaRecovery).SetSpeedBased(isSpeedBased: true).SetDelay(delayStaminaRegen * 2.3f * num)
					.OnUpdate(SetStaminaUI)
					.SetId("StmReg")
					.SetEase(Ease.Linear)
					.OnStart(StartUpdateStamina);
			}
		}
		else if (recoveryStamina)
		{
			if (playerController.sweatVFX.enabled)
			{
				playerController.sweatVFX.enabled = false;
			}
			DOTween.To(() => stamina, (float x) =>
			{
				stamina = x;
			}, GetCurrentMaxStamina(), staminaRegen * multiplyStaminaRecovery).SetSpeedBased(isSpeedBased: true).SetDelay(delayStaminaRegen * num)
				.OnUpdate(SetStaminaUI)
				.SetId("StmReg")
				.SetEase(Ease.Linear);
		}
		UIGameManager.Instance.energyDrainBarStamina.value = 1f - playerController.PlayerMultiplyStatsData.GetMultiplyStamina();
	}

	private void SetStaminaUI()
	{
		if (currentTextStamina != Mathf.RoundToInt(GetStamina()))
		{
			UIGameManager.Instance.txtStaminaValuePlayer.text = Mathf.RoundToInt(GetStamina()) + "/" + maxStamina;
		}
		UIGameManager.Instance.barStamina.value = GetStamina() / maxStamina;
		currentTextStamina = Mathf.RoundToInt(GetStamina());
	}

	private void StartUpdateStamina()
	{
		if (playerController.IsMale)
		{
			AudioManager.StopSFX("male-stamina-low");
			AudioManager.PlaySFX("male-stamina-recover");
		}
		else
		{
			AudioManager.StopSFX("female-stamina-low");
			AudioManager.PlaySFX("female-stamina-recover");
		}
		playerController.sweatVFX.enabled = false;
	}

	public List<string> GetSkillLearn()
	{
		return _skillData.SkillLearnDataList;
	}

	public void AddSkillLearn(string id)
	{
		_skillData.AddSkillLearn(id);
	}

	public bool CheckSkillLearn(string id)
	{
		return _skillData.CheckSkillLearn(id);
	}

	public int GetTotalSkillLearn()
	{
		return _skillData.GetTotalSkillLearn();
	}

	public void ResetSkillLearnData()
	{
		_skillData.ResetSkillLearnData();
	}

	public void AddSkillPoint(int point)
	{
		_skillData.AddSkillPoint(point);
	}

	public void RemoveSkillPoint(int point)
	{
		_skillData.RemoveSkillPoint(point);
	}

	public void ResetSkillPoint()
	{
		_skillData.ResetSkillPoint();
	}

	public bool CheckSkillPoint(int point)
	{
		return _skillData.CheckSkillPoint(point);
	}

	public int GetSkillPoint()
	{
		return _skillData.SkillPoint;
	}

	public MaterialInventory GetCurrentMaterialInventory()
	{
		return MaterialInventoryManager.GetMaterialInventory();
	}

	public void OnDropItemClick(InputAction.CallbackContext value)
	{
		if ((!ArmoryLobbyManager.Instance || ArmoryLobbyManager.Instance.UIMenu.isHidden) && !UIGameManager.Instance.uiInventory.isHidden && NetworkGameManager.Instance.ownPlayer.network.GetHealth() > 0f && (bool)EventSystem.current.currentSelectedGameObject)
		{
			AudioManager.PlaySFX("ui_select");
			Transform transform = null;
			InventoryManager inventoryManager = NetworkGameManager.Instance.ownPlayer.inventoryManager;
			if (UIGameManager.Instance.inventoryOptions.activeSelf)
			{
				transform = inventoryManager.targetInventory;
				UIGameManager.Instance.inventoryOptions.SetActive(value: false);
				UIGameManager.Instance.titleWeapon.SetTerm("EmptyField");
				UIGameManager.Instance.dscWeapon.SetTerm("EmptyField");
				UIGameManager.Instance.titleWeapon.GetComponent<TextMeshProUGUI>().text = "";
				UIGameManager.Instance.dscWeapon.GetComponent<TextMeshProUGUI>().text = "";
			}
			else
			{
				transform = EventSystem.current.currentSelectedGameObject.transform;
			}
			int num = int.Parse(transform.name.Substring(13, transform.name.Length - 13));
			if (num != 0)
			{
				inventoryManager.FunctionItemDrop(num, isSwapWeapon: false);
			}
		}
	}

	public void OnCombineItemClick(InputAction.CallbackContext value)
	{
		if (((bool)ArmoryLobbyManager.Instance && !ArmoryLobbyManager.Instance.UIMenu.isHidden) || UIGameManager.Instance.uiInventory.isHidden || !(NetworkGameManager.Instance.ownPlayer.network.GetHealth() > 0f))
		{
			return;
		}
		InventoryManager inventoryManager = NetworkGameManager.Instance.ownPlayer.inventoryManager;
		if (inventoryManager.btnCombineWith.gameObject.activeSelf && UIGameManager.Instance.inventoryOptions.activeSelf)
		{
			AudioManager.PlaySFX("ui_select");
			inventoryManager.targetInventory2 = EventSystem.current.currentSelectedGameObject.transform;
			inventoryManager.CombiningItem();
			UIGameManager.Instance.titleWeapon.SetTerm("EmptyField");
			UIGameManager.Instance.dscWeapon.SetTerm("EmptyField");
			foreach (GameObject item in inventoryManager.inventoryPick)
			{
				item.SetActive(value: false);
			}
			UIGameManager.Instance.titleWeapon.GetComponent<TextMeshProUGUI>().text = "";
			UIGameManager.Instance.dscWeapon.GetComponent<TextMeshProUGUI>().text = "";
			return;
		}
		AudioManager.PlaySFX("ui_select");
		if (!UIGameManager.Instance.inventoryOptions.activeSelf && (bool)EventSystem.current.currentSelectedGameObject)
		{
			inventoryManager.targetInventory = EventSystem.current.currentSelectedGameObject.transform;
		}
		int index = int.Parse(inventoryManager.targetInventory.name.Substring(13, inventoryManager.targetInventory.name.Length - 13));
		foreach (GameObject item2 in inventoryManager.inventoryPick)
		{
			item2.SetActive(value: false);
		}
		inventoryManager.inventoryPick[index].SetActive(value: true);
		UIGameManager.Instance.inventoryOptions.SetActive(value: true);
		inventoryManager.targetInventory.GetComponent<Button>().Select();
		inventoryManager.btnCombineWith.gameObject.SetActive(value: true);
		inventoryManager.btnStore.gameObject.SetActive(value: false);
		inventoryManager.btnAssign.gameObject.SetActive(value: false);
		inventoryManager.btnEquip.gameObject.SetActive(value: false);
		inventoryManager.btnUnequip.gameObject.SetActive(value: false);
		inventoryManager.btnUse.gameObject.SetActive(value: false);
		inventoryManager.btnCombine.gameObject.SetActive(value: false);
		inventoryManager.btnOpen.gameObject.SetActive(value: false);
		inventoryManager.btnUnloadAmmo.gameObject.SetActive(value: false);
		inventoryManager.btnSelectItemDismantle.gameObject.SetActive(value: false);
		inventoryManager.btnDrop.gameObject.SetActive(value: false);
		inventoryManager.btnDropAll.gameObject.SetActive(value: false);
	}

	public void ResetDefaultMelee()
	{
		arrInventory[0].ID = 1;
		arrInventory[0].Name = "Baseball Bat";
		arrInventory[0].Amount = 1;
		arrInventory[0].ItemType = "Weapon";
	}

	public void StartRegenHp(float regenRate, float regenThreshold, float regenInterval)
	{
		if (playerController.network.isLocalPlayer)
		{
			regenActive = true;
			if (regenCoroutine == null)
			{
				regenCoroutine = StartCoroutine(RegenRoutine(regenRate, regenThreshold, regenInterval));
			}
		}
	}

	private void OnDestroy()
	{
		if (regenCoroutine != null)
		{
			StopCoroutine(regenCoroutine);
			regenCoroutine = null;
		}
	}

	private IEnumerator RegenRoutine(float regenRate, float regenThreshold, float regenInterval)
	{
		while (regenActive)
		{
			if (playerController.network.GetHealth() < regenThreshold && playerController.network.GetHealth() > 0f && playerController.direction == Vector3.zero && !playerController.isAttacking && !playerController.isEntangled && !playerController.isThrowing)
			{
				UIGameManager.Instance.ArrPlayerInfo[playerController.network.GetIDX()].TextHealingValue2.gameObject.SetActive(value: true);
				UIGameManager.Instance.ArrPlayerInfo[playerController.network.GetIDX()].TextHealingValue2.DOKill();
				UIGameManager.Instance.ArrPlayerInfo[playerController.network.GetIDX()].TextHealingValue2.rectTransform.DOKill();
				UIGameManager.Instance.ArrPlayerInfo[playerController.network.GetIDX()].TextHealingValue2.rectTransform.localPosition = Vector2.zero;
				UIGameManager.Instance.ArrPlayerInfo[playerController.network.GetIDX()].TextHealingValue2.DOFade(1f, 0f);
				UIGameManager.Instance.ArrPlayerInfo[playerController.network.GetIDX()].TextHealingValue2.rectTransform.DOLocalMoveY(18f, 1f);
				UIGameManager.Instance.ArrPlayerInfo[playerController.network.GetIDX()].TextHealingValue2.DOFade(0f, 0.3f).SetDelay(1f).OnComplete(() =>
				{
					UIGameManager.Instance.ArrPlayerInfo[playerController.network.GetIDX()].TextHealingValue2.gameObject.SetActive(value: false);
				});
				playerController.network.AddSubHealth(regenRate);
			}
			yield return new WaitForSeconds(regenInterval);
		}
		regenCoroutine = null;
	}
}

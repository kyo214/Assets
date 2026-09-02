using System;
using System.Collections.Generic;
using DG.Tweening;
using Doozy.Runtime.UIManager.Containers;
using I2.Loc;
using Sirenix.Utilities;
using TMPro;
using Toked;
using Toked.Crafting;
using Toked.Inventory;
using Toked.StatusEffect;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using _Modules.Dismantle.Scripts;

public class InventoryManager : MonoBehaviour
{
	public enum TransferItemType
	{
		INVENTORY_TO_INVENTORY = 0,
		INVENTORY_TO_ARMORY = 1,
		ARMORY_TO_INVENTORY = 2,
		ARMORY_TO_ARMORY = 3
	}

	private static readonly int IsReloading = Animator.StringToHash("isReloading");

	public Transform targetInventory;

	public Transform targetInventory2;

	public PlayerController player;

	public int idxPlayer;

	public Animator frameInventory;

	public List<Image> inventoryIconList = new List<Image>();

	[SerializeField]
	private List<ItemInventorySlotUI> _inventorySlotUI = new List<ItemInventorySlotUI>();

	public List<Image> ammoIconList = new List<Image>();

	public List<TextMeshProUGUI> txtAmountList = new List<TextMeshProUGUI>();

	public List<GameObject> inventoryPick = new List<GameObject>();

	public List<Button> buttonInventory = new List<Button>();

	public List<InventoryItemSlotButton> buttonInventoryList;

	public List<ItemSlot> itemSlotList;

	public List<GameObject> KeyButtonInfo = new List<GameObject>();

	public TextMeshProUGUI txtAmountHealingItem;

	public TextMeshProUGUI txtAmountThrowableItem;

	public Button btnStore;

	public Button btnAssign;

	public Button btnEquip;

	public Button btnEquipItem;

	public Button btnUnequip;

	public Button btnUnequipItem;

	public Button btnUse;

	public Button btnCombine;

	public Button btnCombineWith;

	public Button btnOpen;

	public Button btnUnloadAmmo;

	public Button btnSelectItemDismantle;

	public Button btnDrop;

	public TextMeshProUGUI txtDrop;

	public Button btnDropAll;

	public Image IconSlotWeapon;

	public GameObject AdditionalKeyBtnObject;

	public DismantleSelectButtonEvent DismantleSelectButtonEvent;

	private bool _canDismantle;

	public Action<int> OnDropItem;

	public Action OnInventorySlotChangedEvent;

	public Action<InventoryObject> OnSelectDismantleItem;

	public Action OnDeselectDismantleItem;

	public Action<InventoryObject> OnDismantleItem;

	public Action<InventoryObject> OnDismantleButtonEvent;

	private Action OnCustomEquipItemAction;

	private Action OnCustomUnequipItemAction;

	[SerializeField]
	private List<Button> arrButton = new List<Button>();

	private void Awake()
	{
		foreach (Button item in buttonInventory)
		{
			item.interactable = false;
		}
	}

	private void Start()
	{
		if (btnEquip != null)
		{
			arrButton.Add(btnSelectItemDismantle);
			arrButton.Add(btnStore);
			arrButton.Add(btnAssign);
			arrButton.Add(btnEquip);
			arrButton.Add(btnEquipItem);
			arrButton.Add(btnUnequip);
			arrButton.Add(btnUnequipItem);
			arrButton.Add(btnUse);
			arrButton.Add(btnCombine);
			arrButton.Add(btnCombineWith);
			arrButton.Add(btnOpen);
			arrButton.Add(btnUnloadAmmo);
			arrButton.Add(btnDrop);
			arrButton.Add(btnDropAll);
		}
	}

	public void ItemClick()
	{
		if (!UIGameManager.Instance.uiInventory.isVisible)
		{
			return;
		}
		if (btnCombineWith.gameObject.activeSelf && UIGameManager.Instance.inventoryOptions.activeSelf)
		{
			targetInventory2 = EventSystem.current.currentSelectedGameObject.transform;
			CombiningItem();
			UIGameManager.Instance.titleWeapon.SetTerm("EmptyField");
			UIGameManager.Instance.dscWeapon.SetTerm("EmptyField");
			foreach (GameObject item in inventoryPick)
			{
				item.SetActive(value: false);
			}
			UIGameManager.Instance.titleWeapon.GetComponent<TextMeshProUGUI>().text = "";
			UIGameManager.Instance.dscWeapon.GetComponent<TextMeshProUGUI>().text = "";
			return;
		}
		if (player.data.arrInventory[int.Parse(EventSystem.current.currentSelectedGameObject.name.Substring(13, EventSystem.current.currentSelectedGameObject.name.Length - 13))].Name == "Null")
		{
			UIGameManager.Instance.inventoryOptions.SetActive(value: false);
			foreach (GameObject item2 in inventoryPick)
			{
				item2.SetActive(value: false);
			}
			targetInventory = null;
			UIGameManager.Instance.titleWeapon.SetTerm("EmptyField");
			UIGameManager.Instance.dscWeapon.SetTerm("EmptyField");
			UIGameManager.Instance.titleWeapon.GetComponent<TextMeshProUGUI>().text = "";
			UIGameManager.Instance.dscWeapon.GetComponent<TextMeshProUGUI>().text = "";
			return;
		}
		if (targetInventory != null && UIGameManager.Instance.inventoryOptions.activeSelf && EventSystem.current.currentSelectedGameObject == targetInventory.gameObject)
		{
			UIGameManager.Instance.inventoryOptions.SetActive(value: false);
			foreach (GameObject item3 in inventoryPick)
			{
				item3.SetActive(value: false);
			}
			targetInventory = null;
			UIGameManager.Instance.titleWeapon.SetTerm("EmptyField");
			UIGameManager.Instance.dscWeapon.SetTerm("EmptyField");
			UIGameManager.Instance.titleWeapon.GetComponent<TextMeshProUGUI>().text = "";
			UIGameManager.Instance.dscWeapon.GetComponent<TextMeshProUGUI>().text = "";
			return;
		}
		foreach (GameObject item4 in inventoryPick)
		{
			item4.SetActive(value: false);
		}
		UIGameManager.Instance.inventoryOptions.SetActive(value: true);
		targetInventory = EventSystem.current.currentSelectedGameObject.transform;
		for (int i = 0; i < arrButton.Count; i++)
		{
			arrButton[i]?.gameObject.SetActive(value: false);
		}
		int num = int.Parse(targetInventory.name.Substring(13, targetInventory.name.Length - 13));
		if (num == 0)
		{
			return;
		}
		InventoryObject inventoryObject = player.data.arrInventory[num];
		ItemScriptableObject itemData = DataManager.Instance.GetItemData(inventoryObject.ID.ToString());
		if (itemData.CheckInventoryItemEquip())
		{
			OnCustomEquipItemAction = () =>
			{
				itemData.CustomEquipInventoryEffectSO.EquipAction(player, inventoryObject);
			};
			OnCustomUnequipItemAction = () =>
			{
				itemData.CustomEquipInventoryEffectSO.UnequipAction(player, inventoryObject);
			};
			if (inventoryObject.equip)
			{
				btnEquipItem.gameObject.SetActive(value: false);
				btnUnequipItem.gameObject.SetActive(value: true);
			}
			else
			{
				btnEquipItem.gameObject.SetActive(value: true);
				btnUnequipItem.gameObject.SetActive(value: false);
			}
		}
		else
		{
			OnCustomEquipItemAction = null;
			OnCustomUnequipItemAction = null;
		}
		if (ArmoryLobbyManager.Instance != null && !ArmoryLobbyManager.Instance.UIMenu.isHidden)
		{
			btnStore.gameObject.SetActive(value: true);
		}
		btnDrop.gameObject.SetActive(value: true);
		txtDrop.text = LocalizationManager.GetTranslation("Menu/Drop").ToUpper();
		inventoryPick[num].SetActive(value: true);
		if (player.data.arrInventory[num].ItemType == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(player.data.arrInventory[num].ID).WeaponType == "Throw")
		{
			btnAssign.gameObject.SetActive(value: true);
		}
		if (GameModes.Instance.weaponInBackpack && player.data.arrInventory[num].IsEquippable && num > 1)
		{
			btnEquip.gameObject.SetActive(value: true);
		}
		if (player.data.arrInventory[num].ItemType == "Ammunition")
		{
			int num2 = Mathf.CeilToInt(player.data.arrInventory[num].MaxItemInSlot / 4);
			if (player.data.arrInventory[num].Amount > num2)
			{
				txtDrop.text = txtDrop.text.Replace(" (X)", $" ({num2})");
				btnDropAll.gameObject.SetActive(value: true);
			}
			else
			{
				txtDrop.text = txtDrop.text.Replace(" (X)", $" ({player.data.arrInventory[num].Amount})");
			}
		}
		else
		{
			txtDrop.text = txtDrop.text.Replace(" (X)", " ");
		}
		if (player.data.arrInventory[num].IsOpenable)
		{
			btnOpen.gameObject.SetActive(value: true);
		}
		if (player.data.arrInventory[num].IsCombinable || player.data.arrInventory[num].ItemType == "Ammunition")
		{
			btnCombine.gameObject.SetActive(value: true);
			if (num == 0)
			{
				btnCombine.gameObject.SetActive(value: false);
			}
		}
		if (player.data.arrInventory[num].IsUsable)
		{
			btnUse.gameObject.SetActive(value: true);
			if (player.data.arrInventory[num].ItemType == "HealingItem")
			{
				btnAssign.gameObject.SetActive(value: true);
			}
		}
		if (player.data.arrInventory[num].ItemType == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(player.data.arrInventory[num].ID)?.WeaponType == "Range")
		{
			BGDatabase_Weapon entityByKeyid = BGDatabase_Weapon.GetEntityByKeyid(player.data.arrInventory[num].ID);
			if (entityByKeyid != null && !entityByKeyid.IsTrainingWeapon)
			{
				btnUnloadAmmo.gameObject.SetActive(value: true);
			}
		}
		if (GetStatusDismantle() && BGDatabase_ItemDismantle.GetEntityByKeyItem(player.data.arrInventory[num].ID) != null)
		{
			btnSelectItemDismantle?.gameObject.SetActive(value: true);
		}
		int num3 = -1;
		for (int num4 = 0; num4 < arrButton.Count; num4++)
		{
			if (!arrButton[num4].isActiveAndEnabled)
			{
				continue;
			}
			Navigation navigation = new Navigation
			{
				mode = Navigation.Mode.Explicit
			};
			if (num3 != -1)
			{
				navigation.selectOnUp = arrButton[num3];
			}
			for (int num5 = num4; num5 < arrButton.Count; num5++)
			{
				if (num4 != num5 && arrButton[num5].isActiveAndEnabled)
				{
					navigation.selectOnDown = arrButton[num5];
					break;
				}
			}
			num3 = num4;
			arrButton[num4].navigation = navigation;
		}
		for (int num6 = 0; num6 < arrButton.Count; num6++)
		{
			if (arrButton[num6].isActiveAndEnabled)
			{
				arrButton[num6].Select();
				break;
			}
		}
		if (player.data.arrInventory[num].ItemType == "Weapon")
		{
			UIGameManager.Instance.titleWeapon.SetTerm(player.data.arrInventory[num].ItemType + "/" + player.data.arrInventory[num].ItemType + DataManager.Instance.GetBaseWeapon(player.data.arrInventory[num].ID));
			UIGameManager.Instance.dscWeapon.SetTerm(player.data.arrInventory[num].ItemType + "/Dsc" + player.data.arrInventory[num].ItemType + DataManager.Instance.GetBaseWeapon(player.data.arrInventory[num].ID));
			string attachedWeaponName = UIGameManager.Instance.GetAttachedWeaponName(player.data.arrInventory[num].ID);
			if (attachedWeaponName != "" && UIGameManager.Instance.titleWeaponText != null)
			{
				UIGameManager.Instance.titleWeaponText.text += attachedWeaponName;
			}
			string attachedWeaponDesc = UIGameManager.Instance.GetAttachedWeaponDesc(player.data.arrInventory[num].ID);
			if (attachedWeaponDesc != "" && UIGameManager.Instance.dscWeaponText != null)
			{
				UIGameManager.Instance.dscWeaponText.text += attachedWeaponDesc;
			}
			if (BGDatabase_Weapon.GetEntityByKeyid(player.data.arrInventory[num].ID)?.WeaponType == "Range")
			{
				BGDatabase_Weapon entityByKeyid2 = BGDatabase_Weapon.GetEntityByKeyid(player.data.arrInventory[num].ID);
				if (entityByKeyid2 != null && !entityByKeyid2.IsTrainingWeapon)
				{
					btnUnloadAmmo.gameObject.SetActive(value: true);
				}
			}
		}
		else
		{
			UIGameManager.Instance.titleWeapon.SetTerm(player.data.arrInventory[num].ItemType + "/" + player.data.arrInventory[num].ItemType + player.data.arrInventory[num].ID);
			UIGameManager.Instance.dscWeapon.SetTerm(player.data.arrInventory[num].ItemType + "/Dsc" + player.data.arrInventory[num].ItemType + player.data.arrInventory[num].ID);
		}
	}

	public void ItemShowDesc(int idx)
	{
		AudioManager.PlaySFX("ui_select");
		if (idx < player.data.arrInventory.Count)
		{
			ItemShowDesc(player.data.arrInventory[idx]);
		}
	}

	public void ItemShowDesc(BaseEventData eventData)
	{
		string text = eventData.selectedObject.name;
		if (UIGameManager.Instance.uiInventory.isVisible && (!UIGameManager.Instance.inventoryOptions.activeSelf || btnCombineWith.gameObject.activeSelf))
		{
			int index = int.Parse(EventSystem.current.currentSelectedGameObject.transform.name.Substring(13, text.Length - 13));
			ItemShowDesc(player.data.arrInventory[index]);
		}
	}

	public void ItemShowDesc(InventoryObject inventoryObject)
	{
		UIGameManager.Instance.titleWeaponText.text = "";
		UIGameManager.Instance.dscWeaponText.text = "";
		if (inventoryObject.ItemType == "Weapon")
		{
			UIGameManager.Instance.titleWeapon.SetTerm(inventoryObject.ItemType + "/" + inventoryObject.ItemType + DataManager.Instance.GetBaseWeapon(inventoryObject.ID));
			UIGameManager.Instance.dscWeapon.SetTerm(inventoryObject.ItemType + "/Dsc" + inventoryObject.ItemType + DataManager.Instance.GetBaseWeapon(inventoryObject.ID));
			string attachedWeaponName = UIGameManager.Instance.GetAttachedWeaponName(inventoryObject.ID);
			if (attachedWeaponName != "" && UIGameManager.Instance.titleWeaponText != null)
			{
				UIGameManager.Instance.titleWeaponText.text += attachedWeaponName;
			}
			string attachedWeaponDesc = UIGameManager.Instance.GetAttachedWeaponDesc(inventoryObject.ID);
			if (attachedWeaponDesc != "" && UIGameManager.Instance.dscWeaponText != null)
			{
				UIGameManager.Instance.dscWeaponText.text += attachedWeaponDesc;
			}
		}
		else
		{
			UIGameManager.Instance.titleWeapon.SetTerm(inventoryObject.ItemType + "/" + inventoryObject.ItemType + inventoryObject.ID);
			UIGameManager.Instance.dscWeapon.SetTerm(inventoryObject.ItemType + "/Dsc" + inventoryObject.ItemType + inventoryObject.ID);
		}
		if (string.IsNullOrWhiteSpace(UIGameManager.Instance.titleWeaponText.text))
		{
			UIGameManager.Instance.titleWeaponText.text = inventoryObject.Name;
		}
		if (string.IsNullOrWhiteSpace(UIGameManager.Instance.dscWeaponText.text))
		{
			UIGameManager.Instance.dscWeaponText.text = inventoryObject.Name;
		}
		if (inventoryObject.HasStatusEffect())
		{
			string curseLocalization = inventoryObject.GetCurseLocalization(UIGameManager.Instance.titleWeaponText.text);
			UIGameManager.Instance.titleWeaponText.text = curseLocalization;
			UIGameManager.Instance.dscWeaponText.text += inventoryObject.GetEffectLocalization();
		}
		if (!(inventoryObject.Name == "Null"))
		{
			return;
		}
		if (!btnCombineWith.gameObject.activeSelf)
		{
			UIGameManager.Instance.inventoryOptions.SetActive(value: false);
			foreach (GameObject item in inventoryPick)
			{
				item.SetActive(value: false);
			}
			targetInventory = null;
		}
		UIGameManager.Instance.titleWeapon.SetTerm("EmptyField");
		UIGameManager.Instance.dscWeapon.SetTerm("EmptyField");
		UIGameManager.Instance.titleWeapon.GetComponent<TextMeshProUGUI>().text = "";
		UIGameManager.Instance.dscWeapon.GetComponent<TextMeshProUGUI>().text = "";
	}

	public void ItemCombine()
	{
		targetInventory.GetComponent<Button>().Select();
		btnCombineWith.gameObject.SetActive(value: true);
		btnAssign.gameObject.SetActive(value: false);
		btnEquip.gameObject.SetActive(value: false);
		btnUnequip.gameObject.SetActive(value: false);
		btnUse.gameObject.SetActive(value: false);
		btnCombine.gameObject.SetActive(value: false);
		btnOpen.gameObject.SetActive(value: false);
		btnUnloadAmmo.gameObject.SetActive(value: false);
		btnSelectItemDismantle.gameObject.SetActive(value: false);
		btnDrop.gameObject.SetActive(value: false);
		btnDropAll.gameObject.SetActive(value: false);
	}

	public void ItemUse()
	{
		int idxInventory = int.Parse(targetInventory.name.Substring(13, targetInventory.name.Length - 13));
		FunctionItemUse(idxInventory);
	}

	public void ItemAssign()
	{
		FunctionItemAssign(int.Parse(targetInventory.name.Substring(13, targetInventory.name.Length - 13)));
	}

	public void ItemStore()
	{
		if (NetworkGameManager.Instance.ownPlayer.ItemBoxController.arrItem.Count >= 15)
		{
			return;
		}
		FunctionItemPutToArmory(int.Parse(targetInventory.name.Substring(13, targetInventory.name.Length - 13)));
		UIGameManager.Instance.inventoryOptions.SetActive(value: false);
		foreach (GameObject item in inventoryPick)
		{
			item.SetActive(value: false);
		}
		if (targetInventory != null)
		{
			targetInventory.GetComponent<Button>().Select();
			targetInventory = null;
		}
		ArmoryLobbyManager.Instance.ShowItem();
	}

	public void FunctionItemAssign(int idxInventory)
	{
		if (player.data.arrInventory[idxInventory].ItemType == "HealingItem")
		{
			if (player.data.arrInventory[idxInventory].IsUsable)
			{
				player.data.idHealing = player.data.arrInventory[idxInventory].ID;
				UIGameManager.Instance.SetHealingShortcutSprite(DataManager.Instance.GetItemSprite(player.data.idHealing.ToString()));
				if (player.network.isLocalPlayer)
				{
					txtAmountHealingItem.text = player.data.FindTotalInventory(player.data.idHealing).ToString();
				}
			}
		}
		else if (player.data.arrInventory[idxInventory].ItemType == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(player.data.arrInventory[idxInventory].ID).WeaponType == "Throw")
		{
			player.data.idThrowable = player.data.arrInventory[idxInventory].ID;
			UIGameManager.Instance.SetThrowableShortcutSprite(DataManager.Instance.GetItemSprite(player.data.idThrowable.ToString()));
			txtAmountThrowableItem.text = player.data.FindTotalInventory(player.data.idThrowable).ToString();
			player.canGrenade = true;
		}
		if (!player.network.isLocalPlayer)
		{
			return;
		}
		UIGameManager.Instance.inventoryOptions.SetActive(value: false);
		foreach (GameObject item in inventoryPick)
		{
			item.SetActive(value: false);
		}
		if (targetInventory != null)
		{
			targetInventory.GetComponent<Button>().Select();
		}
		targetInventory = null;
		OnInventorySlotChangedEvent?.Invoke();
	}

	public void FunctionItemUse(int idxInventory, bool isHealthCheck = false)
	{
		if (!player.data.arrInventory[idxInventory].IsUsable)
		{
			return;
		}
		bool flag = false;
		if (player.data.arrInventory[idxInventory].ItemType == "HealingItem" && (!isHealthCheck || BGDatabase_HealingItem.GetEntityByKeyid(player.data.arrInventory[idxInventory].ID).HealingValuePercent == 0 || (player.network.GetHealth() > 0f && player.network.GetHealth() < player.data.GetMaxHealth())))
		{
			UIGameManager.Instance.flashGreen.enabled = true;
			UIGameManager.Instance.flashGreen.DOKill();
			UIGameManager.Instance.flashGreen.DOFade(0.1f, 0f);
			UIGameManager.Instance.flashGreen.DOFade(0f, 0.6f).SetDelay(0.03f).OnComplete(() =>
			{
				UIGameManager.Instance.flashGreen.enabled = false;
			});
			AudioManager.PlaySFX("herb_pickup");
			ItemScriptableObject itemData = DataManager.Instance.GetItemData(player.data.arrInventory[idxInventory].ID.ToString());
			if (itemData.UseCustomItemEffect)
			{
				StatusEffectScriptableObject statusEffectScriptableObject = itemData.CustomItemEffectSO;
				if (itemData.CantStackingStatusEffect)
				{
					statusEffectScriptableObject = itemData.CustomItemEffectSO.CloneStatusEffectSO(destroyOnRemove: true, $"{itemData.ID}_{idxInventory}");
				}
				player.StatusEffectController.ApplyStatus(player, statusEffectScriptableObject);
			}
			else
			{
				player.network.AddSubHealth((float)BGDatabase_HealingItem.GetEntityByKeyid(player.data.arrInventory[idxInventory].ID).HealingValuePercent * player.PlayerMultiplyStatsData.GetMultiplyHealthPotency());
				if (player.network.GetHealth() > player.data.GetMaxHealth())
				{
					player.network.SetHealth(player.data.GetMaxHealth());
				}
				StatusEffectScriptableObject statusEffectScriptableObject2 = itemData.AdditionalStatusEffectSO;
				if (itemData.CantStackingStatusEffect)
				{
					statusEffectScriptableObject2 = itemData.AdditionalStatusEffectSO?.CloneStatusEffectSO(destroyOnRemove: true, $"{itemData.ID}_{idxInventory}");
				}
				player.StatusEffectController.ApplyStatus(player, statusEffectScriptableObject2);
			}
			player.data.RemoveInventory(idxInventory);
			SetHealItemShortcut();
			flag = true;
		}
		if (player.data.arrInventory[idxInventory].ItemType == "Item")
		{
			if (player.itemCollision != null && player.itemCollision.GetComponent<ItemInteractable>() != null)
			{
				ItemInteractable component = player.itemCollision.GetComponent<ItemInteractable>();
				if (component.isLocked && component.itemIDUnlock == player.data.arrInventory[idxInventory].ID)
				{
					player.network.ShowBaloonChat(ChatType.UNLOCKED, component.itemIDUnlock, -1, -1, -1, 10);
					component.isLocked = false;
					component.isTriggered = true;
					if (component.doorCollider != null)
					{
						component.doorCollider.transform.gameObject.layer = 22;
						GameManager.Instance.AStarPath.UpdateGraphs(component.doorCollider.bounds);
						GameManager.Instance.AStarPath.FlushGraphUpdates();
					}
					player.network.SetUnlockItem(component.UniqueID);
					player.data.RemoveInventory(idxInventory);
					flag = true;
					UIGameManager.Instance.uiInventory.Hide();
					UIGameManager.Instance.animUIInventory.PlayFromToProgress(1f, 0f);
					UIGameManager.Instance.inventoryOptions.SetActive(value: false);
					NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: true);
				}
			}
			if (BGDatabase_Item.GetEntityByKeyid(player.data.arrInventory[idxInventory].ID).UseFunction == "AddSlot" && player.data.GetMaxInventory() <= 8)
			{
				player.data.AddSlotInventory();
				AudioManager.PlaySFX("herb_pickup");
				player.data.RemoveInventory(idxInventory);
				flag = true;
			}
		}
		if (!flag)
		{
			return;
		}
		UIGameManager.Instance.inventoryOptions.SetActive(value: false);
		foreach (GameObject item in inventoryPick)
		{
			item.SetActive(value: false);
		}
		if (targetInventory != null)
		{
			targetInventory.GetComponent<Button>().Select();
			targetInventory = null;
		}
		OnInventorySlotChangedEvent?.Invoke();
		void SetHealItemShortcut()
		{
			if (player.data.FindTotalInventory(player.data.idHealing) <= 0)
			{
				player.data.idHealing = -1;
				foreach (InventoryObject item2 in player.data.arrInventory)
				{
					if (item2.ItemType == "HealingItem" && item2.IsUsable && item2.Name != "Null")
					{
						player.data.idHealing = item2.ID;
						UIGameManager.Instance.SetHealingShortcutSprite(DataManager.Instance.GetItemSprite(player.data.idHealing.ToString()));
						break;
					}
				}
			}
			if (player.data.idHealing == -1)
			{
				UIGameManager.Instance.HideHealingShortcutSprite();
				txtAmountHealingItem.text = "";
			}
			else
			{
				txtAmountHealingItem.text = player.data.FindTotalInventory(player.data.idHealing).ToString();
			}
		}
	}

	public void CombiningItem()
	{
		bool flag = false;
		if (targetInventory2 != null && targetInventory != null && targetInventory2 != targetInventory)
		{
			flag = FunctionCombiningItem(int.Parse(targetInventory.name.Substring(13, targetInventory.name.Length - 13)), int.Parse(targetInventory2.name.Substring(13, targetInventory2.name.Length - 13)));
		}
		if (!flag)
		{
			UIGameManager.Instance.inventoryOptions.SetActive(value: false);
			targetInventory = null;
			btnCombineWith.gameObject.SetActive(value: false);
		}
	}

	public bool FunctionCombiningItem(int idx1, int idx2, TransferItemType transerType = TransferItemType.INVENTORY_TO_INVENTORY)
	{
		bool flag = false;
		int num = -1;
		InventoryObject[] array = new InventoryObject[2];
		switch (transerType)
		{
		case TransferItemType.INVENTORY_TO_INVENTORY:
			array[0] = player.data.arrInventory[idx1];
			array[1] = player.data.arrInventory[idx2];
			break;
		case TransferItemType.ARMORY_TO_INVENTORY:
			player.data.arrInventory.Add(new InventoryObject());
			array[0] = player.data.arrInventory[player.data.arrInventory.Count - 1];
			array[0].SetInventoryObject(player.ItemBoxController.arrItem[idx1]);
			array[0].IdxInventory = player.data.arrInventory.Count - 1;
			array[1] = player.data.arrInventory[idx2];
			break;
		case TransferItemType.INVENTORY_TO_ARMORY:
			array[0] = player.data.arrInventory[idx1];
			player.data.arrInventory.Add(new InventoryObject());
			array[1] = player.data.arrInventory[player.data.arrInventory.Count - 1];
			array[1].SetInventoryObject(player.ItemBoxController.arrItem[idx2]);
			array[1].IdxInventory = player.data.arrInventory.Count - 1;
			break;
		case TransferItemType.ARMORY_TO_ARMORY:
			if (idx1 < 0)
			{
				return false;
			}
			player.data.arrInventory.Add(new InventoryObject());
			array[0] = player.data.arrInventory[player.data.arrInventory.Count - 1];
			array[0].SetInventoryObject(player.ItemBoxController.arrItem[idx1]);
			array[0].IdxInventory = player.data.arrInventory.Count - 1;
			if (idx2 < 0)
			{
				if (idx1 >= 0)
				{
					player.data.arrInventory[player.data.arrInventory.Count - 1] = null;
					player.data.arrInventory.RemoveAt(player.data.arrInventory.Count - 1);
				}
				return false;
			}
			player.data.arrInventory.Add(new InventoryObject());
			array[1] = player.data.arrInventory[player.data.arrInventory.Count - 1];
			array[1].SetInventoryObject(player.ItemBoxController.arrItem[idx2]);
			array[1].IdxInventory = player.data.arrInventory.Count - 1;
			break;
		}
		for (int i = 0; i < 2; i++)
		{
			int num2 = 0;
			if (i == 0)
			{
				num2 = 1;
			}
			if (player.data.ListSpecialCombine.Count > 0)
			{
				foreach (string item in player.data.ListSpecialCombine)
				{
					string[] array2 = item.Split('|');
					if ((array2[0] == array[i].ID.ToString() && array2[1] == array[num2].ID.ToString()) || (array2[1] == array[i].ID.ToString() && array2[0] == array[num2].ID.ToString()))
					{
						if (array[i].ItemType == "HealingItem" && array[num2].ItemType == "HealingItem")
						{
							num = CombinedHealingItem(array[i].IdxInventory, array[num2].IdxInventory, int.Parse(array2[2]));
						}
						flag = true;
					}
				}
			}
			if (flag)
			{
				break;
			}
			if (array[i].ItemType == "Weapon" && array[num2].ItemType == "Ammunition")
			{
				if (BGDatabase_Weapon.GetEntityByKeyid(DataManager.Instance.GetBaseWeapon(array[i].ID)).AmmoTypeID != array[num2].ID)
				{
					continue;
				}
				int num3 = player.weaponController.GetMagazineSize(equipedWeapon: false, array[i].ID, DataManager.Instance.GetBaseWeapon(array[i].ID)) - array[i].Ammo;
				if (array[num2].Amount > num3)
				{
					array[i].Ammo = player.weaponController.GetMagazineSize(equipedWeapon: false, array[i].ID, DataManager.Instance.GetBaseWeapon(array[i].ID));
					array[num2].Amount -= num3;
					player.network.ExecSyncAmmoWeaponInventory(array[i].IdxInventory, array[i].Ammo);
					player.network.ExecSyncAmountInventory(array[num2].IdxInventory, array[num2].Amount);
					if (array[num2].IdxInventory < txtAmountList.Count)
					{
						txtAmountList[array[num2].IdxInventory].text = array[num2].Amount.ToString();
					}
				}
				else
				{
					array[i].Ammo += array[num2].Amount;
					player.network.ExecSyncAmmoWeaponInventory(array[i].IdxInventory, array[i].Ammo);
					if (array[num2].IdxInventory < txtAmountList.Count)
					{
						txtAmountList[array[num2].IdxInventory].gameObject.SetActive(value: false);
					}
					player.data.RemoveInventory(array[num2].IdxInventory);
				}
				if (array[i].IdxInventory < txtAmountList.Count)
				{
					txtAmountList[array[i].IdxInventory].text = array[i].Ammo.ToString();
				}
				player.data.InitImageInventoryLocal();
				AudioManager.PlaySFX("inventory_combine_success");
				flag = true;
				break;
			}
			if (array[i].ItemType == "Weapon" && array[num2].ItemType == "Item")
			{
				for (int j = 0; j < BGDatabase_Weapon.CountEntities; j++)
				{
					int keys = BGDatabase_Weapon.GetEntity(j).Keys;
					if (BGDatabase_Weapon.GetEntityByKeyid(keys).CraftRecipe == null)
					{
						continue;
					}
					foreach (string item2 in BGDatabase_Weapon.GetEntityByKeyid(keys).CraftRecipe)
					{
						string[] array3 = MathFunc.SplitString(item2, '+');
						if (array3.Length == 2 && array3[0] == array[i].ID.ToString() && array3[1] == array[num2].ID.ToString())
						{
							int ammo = array[i].Ammo;
							player.data.RemoveInventory(array[i].IdxInventory);
							player.data.RemoveInventory(array[num2].IdxInventory);
							num = player.data.AddInventory(keys, isOnPick: false, 1, ammo, init: false, isCombine: true);
							if (array[i].IdxInventory == player.weaponController.idxWeaponRange)
							{
								player.weaponController.EquipWeaponInventory(array[i].IdxInventory, ammo);
								itemSlotList[1].Flashing();
							}
							flag = true;
							AudioManager.PlaySFX("inventory_combine_success");
							break;
						}
					}
				}
			}
			else if (array[i].ItemType == "HealingItem" && array[num2].ItemType == "HealingItem")
			{
				for (int k = 0; k < BGDatabase_HealingItem.CountEntities; k++)
				{
					int num4 = BGDatabase_HealingItem.GetEntity(k).Keys;
					if ((BGDatabase_HealingItem.GetEntityByKeyid(num4).CombineItem0 == array[i].Name && BGDatabase_HealingItem.GetEntityByKeyid(num4).CombineItem1 == array[num2].Name) || (BGDatabase_HealingItem.GetEntityByKeyid(num4).CombineItem0 == array[num2].Name && BGDatabase_HealingItem.GetEntityByKeyid(num4).CombineItem1 == array[i].Name))
					{
						BGDatabase_HealingItem entityByKeyid = BGDatabase_HealingItem.GetEntityByKeyid(num4);
						if (entityByKeyid != null && entityByKeyid.BaseKey > 0)
						{
							num4 = entityByKeyid.BaseKey;
						}
						num = CombinedHealingItem(array[i].IdxInventory, array[num2].IdxInventory, num4);
						flag = true;
						break;
					}
				}
			}
			else if (array[i].ItemType == "Item" && array[num2].ItemType == "Item")
			{
				for (int l = 0; l < BGDatabase_Item.CountEntities; l++)
				{
					int keys2 = BGDatabase_Item.GetEntity(l).Keys;
					if ((BGDatabase_Item.GetEntityByKeyid(keys2).CombineItem0 == array[i].ID.ToString() && BGDatabase_Item.GetEntityByKeyid(keys2).CombineItem1 == array[num2].ID.ToString()) || (BGDatabase_Item.GetEntityByKeyid(keys2).CombineItem0 == array[num2].ID.ToString() && BGDatabase_Item.GetEntityByKeyid(keys2).CombineItem1 == array[i].ID.ToString()))
					{
						player.data.RemoveInventory(array[i].IdxInventory);
						player.data.RemoveInventory(array[num2].IdxInventory);
						num = player.data.AddInventory(keys2, isOnPick: false, 0, -1, init: false, isCombine: true);
						AudioManager.PlaySFX("inventory_combine_success");
						flag = true;
						break;
					}
				}
			}
			else
			{
				if (!(array[i].ItemType == "Ammunition") || !(array[num2].ItemType == "Ammunition") || array[i].ID != array[num2].ID)
				{
					continue;
				}
				int num5 = BGDatabase_Ammunition.GetEntityByKeyid(array[num2].ID).MaxItemInSlot - array[num2].Amount;
				if (array[i].Amount > num5)
				{
					array[num2].Amount = BGDatabase_Ammunition.GetEntityByKeyid(array[num2].ID).MaxItemInSlot;
					array[i].Amount -= num5;
					player.network.ExecSyncAmountInventory(array[num2].IdxInventory, array[num2].Amount);
					player.network.ExecSyncAmountInventory(array[i].IdxInventory, array[i].Amount);
					if (array[i].IdxInventory < txtAmountList.Count)
					{
						txtAmountList[array[i].IdxInventory].text = array[i].Amount.ToString();
					}
				}
				else
				{
					array[num2].Amount += array[i].Amount;
					if (array[i].IdxInventory < txtAmountList.Count)
					{
						txtAmountList[array[i].IdxInventory].gameObject.SetActive(value: false);
					}
					player.data.RemoveInventory(array[i].IdxInventory);
				}
				if (array[num2].IdxInventory < txtAmountList.Count)
				{
					txtAmountList[array[num2].IdxInventory].text = array[num2].Amount.ToString();
				}
				player.data.InitImageInventoryLocal();
				AudioManager.PlaySFX("inventory_combine_success");
				flag = true;
				break;
			}
		}
		UIGameManager.Instance.inventoryOptions.SetActive(value: false);
		targetInventory = null;
		btnCombineWith.gameObject.SetActive(value: false);
		if (flag)
		{
			OnInventorySlotChangedEvent?.Invoke();
		}
		switch (transerType)
		{
		case TransferItemType.ARMORY_TO_INVENTORY:
			if (player.data.arrInventory[player.data.arrInventory.Count - 1].ID != -1)
			{
				player.ItemBoxController.arrItem[idx1].SetInventoryObject(player.data.arrInventory[player.data.arrInventory.Count - 1]);
			}
			else if (GameModes.Instance.isItemBoxGlobal)
			{
				ItemBoxNetwork.instance.RemoveItem(idx1);
			}
			else
			{
				player.ItemBoxController.arrItem.RemoveAt(idx1);
			}
			player.data.arrInventory[player.data.arrInventory.Count - 1] = null;
			player.data.arrInventory.RemoveAt(player.data.arrInventory.Count - 1);
			if (num != -1)
			{
				itemSlotList[num].Flashing();
			}
			player.data.InitImageInventoryLocal();
			break;
		case TransferItemType.INVENTORY_TO_ARMORY:
			if (num != -1)
			{
				player.ItemBoxController.arrItem[idx2].SetInventoryObject(player.data.arrInventory[num]);
				player.data.RemoveInventory(num);
				int num7 = player.data.FindTotalInventory(player.data.idHealing);
				if (num7 <= 0)
				{
					UIGameManager.Instance.HideHealingShortcutSprite();
					txtAmountHealingItem.text = "";
				}
				else
				{
					txtAmountHealingItem.text = num7.ToString();
				}
			}
			else if (player.data.arrInventory[idx1].ID != -1 && player.data.arrInventory[player.data.arrInventory.Count - 1].ID == -1)
			{
				player.ItemBoxController.arrItem[idx2].SetInventoryObject(player.data.arrInventory[idx1]);
				player.data.RemoveInventory(idx1);
			}
			else if (player.data.arrInventory[idx1].ID == -1 && player.data.arrInventory[player.data.arrInventory.Count - 1].ID != -1)
			{
				player.ItemBoxController.arrItem[idx2].SetInventoryObject(player.data.arrInventory[player.data.arrInventory.Count - 1]);
			}
			else if (player.data.arrInventory[idx1].ID != -1 && player.data.arrInventory[player.data.arrInventory.Count - 1].ID != -1)
			{
				player.ItemBoxController.arrItem[idx2].SetInventoryObject(player.data.arrInventory[player.data.arrInventory.Count - 1]);
			}
			player.data.arrInventory[player.data.arrInventory.Count - 1] = null;
			player.data.arrInventory.RemoveAt(player.data.arrInventory.Count - 1);
			if (player.data.arrInventory[idx1].IdxInventory == player.weaponController.idxWeaponRange)
			{
				player.weaponController.EquipWeaponInventory(player.data.arrInventory[idx1].IdxInventory, player.data.arrInventory[idx1].Ammo);
			}
			player.data.InitImageInventoryLocal();
			break;
		case TransferItemType.ARMORY_TO_ARMORY:
		{
			bool flag2 = false;
			if (num != -1)
			{
				player.ItemBoxController.arrItem[idx1].SetInventoryObject(player.data.arrInventory[num]);
				player.data.RemoveInventory(num);
				flag2 = true;
			}
			else if (player.data.arrInventory[player.data.arrInventory.Count - 2].ID != -1 && player.data.arrInventory[player.data.arrInventory.Count - 1].ID == -1)
			{
				player.ItemBoxController.arrItem[idx1].SetInventoryObject(player.data.arrInventory[player.data.arrInventory.Count - 2]);
				flag2 = true;
			}
			else if (player.data.arrInventory[player.data.arrInventory.Count - 2].ID == -1 && player.data.arrInventory[player.data.arrInventory.Count - 1].ID != -1)
			{
				player.ItemBoxController.arrItem[idx1].SetInventoryObject(player.data.arrInventory[player.data.arrInventory.Count - 1]);
				flag2 = true;
			}
			if (flag2)
			{
				if (GameModes.Instance.isItemBoxGlobal)
				{
					ItemBoxNetwork.instance.RemoveItem(idx2);
				}
				else
				{
					player.ItemBoxController.arrItem.RemoveAt(idx2);
				}
				player.network.ExecAddRemoveItemBoxToServer(idx2, isRemove: true);
			}
			for (int m = 0; m < 2; m++)
			{
				player.data.arrInventory[player.data.arrInventory.Count - 1] = null;
				player.data.arrInventory.RemoveAt(player.data.arrInventory.Count - 1);
			}
			int num6 = player.data.FindTotalInventory(player.data.idHealing);
			if (num6 <= 0)
			{
				UIGameManager.Instance.HideHealingShortcutSprite();
				txtAmountHealingItem.text = "";
			}
			else
			{
				txtAmountHealingItem.text = num6.ToString();
			}
			break;
		}
		default:
			if (num != -1)
			{
				itemSlotList[num].Flashing();
			}
			break;
		}
		return flag;
	}

	private int CombinedHealingItem(int idxInv1, int idxInv2, int entry)
	{
		if (player.data.arrInventory[idxInv1].ID == player.data.idHealing || player.data.arrInventory[idxInv2].ID == player.data.idHealing)
		{
			UIGameManager.Instance.SetHealingShortcutSprite(DataManager.Instance.GetItemSprite(entry.ToString()));
			player.data.idHealing = entry;
		}
		player.data.RemoveInventory(idxInv1);
		player.data.RemoveInventory(idxInv2);
		int result = player.data.AddInventory(entry, isOnPick: false, 0, -1, init: false, isCombine: true);
		txtAmountHealingItem.text = player.data.FindTotalInventory(player.data.idHealing).ToString();
		AudioManager.PlaySFX("inventory_combine_success");
		return result;
	}

	public void ItemOpen()
	{
		int idx = int.Parse(targetInventory.name.Substring(13, targetInventory.name.Length - 13));
		FunctionOpenItem(idx);
	}

	public bool FunctionOpenItem(int idx)
	{
		bool flag = false;
		string spawnItemOpen = BGDatabase_Item.GetEntityByKeyid(player.data.arrInventory[idx].ID).SpawnItemOpen;
		if (spawnItemOpen != "-")
		{
			player.data.RemoveInventory(idx);
			player.data.AddInventory(int.Parse(spawnItemOpen), isOnPick: true);
			flag = true;
			AudioManager.PlaySFX("inventory_combine_success");
		}
		else if (BGDatabase_Item.GetEntityByKeyid(player.data.arrInventory[idx].ID).ShowPuzzleUI != "-")
		{
			Transform transform = UIPuzzle.Instance.transform.Find(BGDatabase_Item.GetEntityByKeyid(player.data.arrInventory[idx].ID).ShowPuzzleUI);
			UIView uIView = null;
			if ((bool)transform)
			{
				uIView = transform.GetComponent<UIView>();
			}
			if (uIView != null)
			{
				UIGameManager.Instance.mapUI.SetActive(value: false);
				UIGameManager.Instance.uiTabKill.InstantHide();
				UIGameManager.Instance.uiTabKill.gameObject.SetActive(value: false);
				UIGameManager.Instance.animUIInventory.PlayFromToProgress(1f, 0f);
				UIGameManager.Instance.uiInventory.InstantHide();
				UIGameManager.Instance.uIInGameController?.SetCraftingMaterialsUI(show: false);
				foreach (GameObject item in NetworkGameManager.Instance.ownPlayer.inventoryManager.inventoryPick)
				{
					item.SetActive(value: false);
				}
				foreach (Button item2 in NetworkGameManager.Instance.ownPlayer.inventoryManager.buttonInventory)
				{
					item2.interactable = false;
				}
				UIGameManager.Instance.UIMenuPuzzle = uIView;
				uIView.Show();
			}
		}
		UIGameManager.Instance.inventoryOptions.SetActive(value: false);
		targetInventory = null;
		if (flag)
		{
			OnInventorySlotChangedEvent?.Invoke();
		}
		return flag;
	}

	public void ItemDrop()
	{
		int num = int.Parse(targetInventory.name.Substring(13, targetInventory.name.Length - 13));
		if (CanBeDrop(num))
		{
			FunctionItemDrop(num, isSwapWeapon: false);
		}
	}

	public void ItemUnloadAmmo()
	{
		int num = int.Parse(targetInventory.name.Substring(13, targetInventory.name.Length - 13));
		FunctionUnloadAmmo(num);
		UIGameManager.Instance.inventoryOptions.SetActive(value: false);
		targetInventory.GetComponent<Button>()?.Select();
		inventoryPick[num].SetActive(value: false);
	}

	public void ItemDismantleButton()
	{
		int index = int.Parse(targetInventory.name.Substring(13, targetInventory.name.Length - 13));
		OnDismantleButtonEvent?.Invoke(player.data.arrInventory[index]);
		UIGameManager.Instance.inventoryOptions.SetActive(value: false);
	}

	public void ItemDismantle()
	{
		int idxInventory = int.Parse(targetInventory.name.Substring(13, targetInventory.name.Length - 13));
		ItemDismantle(idxInventory);
	}

	public void ItemDismantle(int idxInventory)
	{
		FunctionUnloadAmmo(idxInventory);
		FunctionDismantleItem(idxInventory);
		UIGameManager.Instance.inventoryOptions.SetActive(value: false);
	}

	public void ItemDropQuarterAmount()
	{
		int idxSlot = int.Parse(targetInventory.name.Substring(13, targetInventory.name.Length - 13));
		if (CanBeDrop(idxSlot))
		{
			FunctionItemDrop(int.Parse(targetInventory.name.Substring(13, targetInventory.name.Length - 13)), isSwapWeapon: false, isQuickDrop: false, isDropQuarter: true);
		}
	}

	public void FunctionItemDrop(int idx, bool isSwapWeapon, bool isQuickDrop = false, bool isDropQuarter = false)
	{
		if (!(player.data.arrInventory[idx].Name != "Null"))
		{
			return;
		}
		int iD = player.data.arrInventory[idx].ID;
		player.data.arrInventory[idx].equip = false;
		switch (idx)
		{
		case 0:
			player.weaponController.idWeaponMelee = -1;
			player.weaponController.idxSkinWeapon0 = -1;
			player.weaponController.meleeObject.SetActive(value: false);
			player.network.UnequipWeapon0();
			break;
		case 1:
			if (player.isAiming)
			{
				player.data.SetCurrentMoveSpeed(player.data.GetInitialMoveSpeed());
				player.SetAnimLowerSpeed(1f);
				player.SetAnimUpperSpeed(1f);
			}
			if (player.fsmUpperBody.GetBool(IsReloading))
			{
				player.fsmUpperBody.SetBool("isReloading", value: false);
			}
			player.weaponController.idWeaponRange = -1;
			player.weaponController.idBaseWeaponRange = -1;
			player.weaponController.idxSkinWeapon1 = -1;
			player.isAiming = false;
			UIGameManager.Instance.crosshair.gameObject.SetActive(value: false);
			player.weaponController.playerController.isRangeActive = false;
			if (player.network.isLocalPlayer)
			{
				UIGameManager.Instance.ammoIconList[idx].gameObject.SetActive(value: false);
				UIGameManager.Instance.txtAmountList[idx].gameObject.SetActive(value: false);
			}
			player.network.UnequipWeapon1();
			break;
		}
		bool flag = false;
		bool flag2 = false;
		if (iD == player.data.idHealing)
		{
			int num = player.data.FindTotalInventory(player.data.idHealing) - 1;
			txtAmountHealingItem.text = num.ToString();
			if (num <= 0)
			{
				UIGameManager.Instance.HideHealingShortcutSprite();
				txtAmountHealingItem.text = "";
				player.data.idHealing = -1;
				flag = true;
			}
			if (player.data.idHealing == -1)
			{
				UIGameManager.Instance.HideHealingShortcutSprite();
				txtAmountHealingItem.text = "";
			}
		}
		if (iD == player.data.idThrowable)
		{
			flag2 = true;
		}
		if ((player.data.arrInventory[idx].ItemType == "Ammunition") & isDropQuarter)
		{
			int num2 = Mathf.CeilToInt(player.data.arrInventory[idx].MaxItemInSlot / 4);
			if (player.data.arrInventory[idx].Amount > num2)
			{
				txtDrop.text = txtDrop.text.Replace(" (X)", $" ({num2})");
				btnDropAll.gameObject.SetActive(value: true);
				player.network.SetDropItemFromPlayer(iD, num2, player.data.arrInventory[idx].Ammo, idx, isQuickDrop, player.data.arrInventory[idx].UniqueID);
			}
			else
			{
				player.network.SetDropItemFromPlayer(iD, player.data.arrInventory[idx].Amount, player.data.arrInventory[idx].Ammo, idx, isQuickDrop, player.data.arrInventory[idx].UniqueID);
				player.data.RemoveInventory(idx);
			}
		}
		else
		{
			int ammo = player.data.arrInventory[idx].Ammo;
			if (player.data.arrInventory[idx].Durability > 0f)
			{
				ammo = (int)player.data.arrInventory[idx].Durability;
			}
			player.network.SetDropItemFromPlayer(iD, player.data.arrInventory[idx].Amount, ammo, idx, isQuickDrop, player.data.arrInventory[idx].UniqueID);
			player.data.RemoveInventory(idx);
		}
		if (flag2)
		{
			int num3 = player.data.FindTotalInventory(player.data.idThrowable);
			if (player.network.isLocalPlayer)
			{
				txtAmountThrowableItem.text = num3.ToString();
			}
			if (num3 <= 0)
			{
				UIGameManager.Instance.HideThrowableShortcutSprite();
				if (player.network.isLocalPlayer)
				{
					txtAmountThrowableItem.text = "";
				}
				player.data.idThrowable = -1;
				player.canGrenade = false;
			}
		}
		if (flag)
		{
			foreach (InventoryObject item in player.data.arrInventory)
			{
				if (item.ID != -1 && item.ItemType == "HealingItem" && item.IsUsable && item.Name != "Null")
				{
					player.data.idHealing = item.ID;
					UIGameManager.Instance.SetHealingShortcutSprite(DataManager.Instance.GetItemSprite(player.data.idHealing.ToString()));
					txtAmountHealingItem.text = player.data.FindTotalInventory(player.data.idHealing).ToString();
					break;
				}
			}
		}
		if (player.network.isLocalPlayer)
		{
			UIGameManager.Instance.inventoryOptions.SetActive(value: false);
			UIGameManager.Instance.titleWeapon.SetTerm("EmptyField");
			UIGameManager.Instance.dscWeapon.SetTerm("EmptyField");
			UIGameManager.Instance.titleWeapon.GetComponent<TextMeshProUGUI>().text = "";
			UIGameManager.Instance.dscWeapon.GetComponent<TextMeshProUGUI>().text = "";
		}
		foreach (GameObject item2 in inventoryPick)
		{
			if ((bool)item2)
			{
				item2.SetActive(value: false);
			}
		}
		if (targetInventory != null)
		{
			targetInventory.GetComponent<Button>().Select();
		}
		targetInventory = null;
		OnDropItem?.Invoke(iD);
		OnInventorySlotChangedEvent?.Invoke();
		if (UIGameManager.Instance != null && player.network.isLocalPlayer)
		{
			player.data.InitImageInventoryLocal();
		}
	}

	public bool CanBeDrop(int idxSlot, bool isArmorySlot = false)
	{
		if ((LobbyManager.Instance != null) | isArmorySlot)
		{
			return true;
		}
		if ((player.data.arrInventory[idxSlot].ItemType == "HealingItem" && GlobalMissionManager.Instance.ModNoHealingItem.CurrentValue >= 1f) || (player.data.arrInventory[idxSlot].ItemType == "Ammunition" && GlobalMissionManager.Instance.ModNoAmmoLoot.CurrentValue >= 1f))
		{
			return false;
		}
		return true;
	}

	public bool FunctionCombineItemArmory(int idxSourceArmory, int idxTargetArmory)
	{
		return FunctionCombiningItem(idxSourceArmory, idxTargetArmory, TransferItemType.ARMORY_TO_ARMORY);
	}

	public void FunctionItemPutToArmory(int idxInventory, int idxArmory = -1)
	{
		if (idxInventory == 1)
		{
			player.weaponController.idWeaponRange = -1;
			player.weaponController.idBaseWeaponRange = -1;
			player.weaponController.idxSkinWeapon1 = -1;
			player.isAiming = false;
			UIGameManager.Instance.crosshair.gameObject.SetActive(value: false);
			player.weaponController.playerController.isRangeActive = false;
			if (player.network.isLocalPlayer)
			{
				UIGameManager.Instance.ammoIconList[idxInventory].gameObject.SetActive(value: false);
				UIGameManager.Instance.txtAmountList[idxInventory].gameObject.SetActive(value: false);
			}
			player.network.UnequipWeapon1();
		}
		bool flag = false;
		bool flag2 = false;
		if (player.data.arrInventory[idxInventory].ID == player.data.idHealing)
		{
			int num = player.data.FindTotalInventory(player.data.idHealing) - 1;
			txtAmountHealingItem.text = num.ToString();
			if (num <= 0)
			{
				UIGameManager.Instance.HideHealingShortcutSprite();
				txtAmountHealingItem.text = "";
				player.data.idHealing = -1;
				flag = true;
			}
			if (player.data.idHealing == -1)
			{
				UIGameManager.Instance.HideHealingShortcutSprite();
				txtAmountHealingItem.text = "";
			}
		}
		if (player.data.arrInventory[idxInventory].ID == player.data.idThrowable)
		{
			flag2 = true;
		}
		bool flag3 = false;
		if (idxArmory != -1 && FunctionCombiningItem(idxInventory, idxArmory, TransferItemType.INVENTORY_TO_ARMORY))
		{
			flag3 = true;
		}
		if (!flag3)
		{
			InventoryObject inventoryObject = new InventoryObject();
			FillSlot(inventoryObject, player.data.arrInventory[idxInventory]);
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
				player.ItemBoxController.arrItem.Add(inventoryObject);
			}
			player.data.RemoveInventory(idxInventory);
		}
		player.network.ExecAddRemoveItemBoxToServer(player.ItemBoxController.arrItem.Count - 1);
		if (flag2)
		{
			int num2 = player.data.FindTotalInventory(player.data.idThrowable);
			if (player.network.isLocalPlayer)
			{
				txtAmountThrowableItem.text = num2.ToString();
			}
			if (num2 <= 0)
			{
				UIGameManager.Instance.HideThrowableShortcutSprite();
				if (player.network.isLocalPlayer)
				{
					txtAmountThrowableItem.text = "";
				}
				player.data.idThrowable = -1;
				player.canGrenade = false;
			}
		}
		if (flag)
		{
			foreach (InventoryObject item in player.data.arrInventory)
			{
				if (item.ItemType == "HealingItem" && item.IsUsable && item.Name != "Null")
				{
					player.data.idHealing = item.ID;
					UIGameManager.Instance.SetHealingShortcutSprite(DataManager.Instance.GetItemSprite(player.data.idHealing.ToString()));
					txtAmountHealingItem.text = player.data.FindTotalInventory(player.data.idHealing).ToString();
					break;
				}
			}
		}
		if (player.network.isLocalPlayer)
		{
			UIGameManager.Instance.inventoryOptions.SetActive(value: false);
			UIGameManager.Instance.titleWeapon.SetTerm("EmptyField");
			UIGameManager.Instance.dscWeapon.SetTerm("EmptyField");
			UIGameManager.Instance.titleWeapon.GetComponent<TextMeshProUGUI>().text = "";
			UIGameManager.Instance.dscWeapon.GetComponent<TextMeshProUGUI>().text = "";
		}
		foreach (GameObject item2 in inventoryPick)
		{
			item2.SetActive(value: false);
		}
		if (targetInventory != null)
		{
			targetInventory.GetComponent<Button>().Select();
		}
		targetInventory = null;
	}

	public int FunctionItemPutToInventoryFromArmory(int idxArmory, int idxInventory = -1)
	{
		int num = -1;
		bool flag = false;
		if (idxInventory != -1 && FunctionCombiningItem(idxArmory, idxInventory, TransferItemType.ARMORY_TO_INVENTORY))
		{
			flag = true;
		}
		if (!flag)
		{
			if (GameModes.Instance.isItemBoxGlobal)
			{
				int itemValueOrAmmo = ItemBoxNetwork.instance.arrItem.Get(idxArmory).Ammo;
				if (ItemBoxNetwork.instance.arrItem.Get(idxArmory).Durability > 0)
				{
					itemValueOrAmmo = ItemBoxNetwork.instance.arrItem.Get(idxArmory).Durability;
				}
				num = player.data.AddInventory(ItemBoxNetwork.instance.arrItem.Get(idxArmory).ID, isOnPick: true, ItemBoxNetwork.instance.arrItem.Get(idxArmory).Amount, itemValueOrAmmo);
			}
			else
			{
				int itemValueOrAmmo2 = player.ItemBoxController.arrItem[idxArmory].Ammo;
				if (player.ItemBoxController.arrItem[idxArmory].Durability > 0f)
				{
					itemValueOrAmmo2 = (int)player.ItemBoxController.arrItem[idxArmory].Durability;
				}
				num = player.data.AddInventory(player.ItemBoxController.arrItem[idxArmory].ID, isOnPick: true, player.ItemBoxController.arrItem[idxArmory].Amount, itemValueOrAmmo2, init: false, isCombine: false, canStacking: false);
			}
			if (num != -1)
			{
				if (GameModes.Instance.isItemBoxGlobal)
				{
					ItemBoxNetwork.instance.RemoveItem(idxArmory);
				}
				else
				{
					player.ItemBoxController.arrItem.RemoveAt(idxArmory);
				}
			}
		}
		player.network.ExecAddRemoveItemBoxToServer(idxArmory, isRemove: true);
		return num;
	}

	public void FunctionItemDropArmory(int idxArmory)
	{
		if (GameModes.Instance.isItemBoxGlobal)
		{
			if (ItemBoxNetwork.instance.arrItem.Get(idxArmory).ID != -1)
			{
				player.network.SetSpawnItem(ItemBoxNetwork.instance.arrItem.Get(idxArmory).ID, player.weaponPos.position, ItemBoxNetwork.instance.arrItem.Get(idxArmory).Amount, ItemBoxNetwork.instance.arrItem.Get(idxArmory).Ammo, isSpread: true);
				ItemBoxNetwork.instance.RemoveItem(idxArmory);
				player.network.ExecAddRemoveItemBoxToServer(idxArmory, isRemove: true);
			}
		}
		else if (idxArmory < player.ItemBoxController.arrItem.Count)
		{
			player.network.SetSpawnItem(player.ItemBoxController.arrItem[idxArmory].ID, player.weaponPos.position, player.ItemBoxController.arrItem[idxArmory].Amount, player.ItemBoxController.arrItem[idxArmory].Ammo, isSpread: true);
			player.ItemBoxController.arrItem.RemoveAt(idxArmory);
			player.network.ExecAddRemoveItemBoxToServer(idxArmory, isRemove: true);
		}
	}

	public void FunctionSwapSlot(int idx1, int idx2, bool isLocal)
	{
		InventoryObject inventoryObject = new InventoryObject();
		FillSlot(inventoryObject, player.data.arrInventory[idx1]);
		FillSlot(player.data.arrInventory[idx1], player.data.arrInventory[idx2]);
		FillSlot(player.data.arrInventory[idx2], inventoryObject);
		if (isLocal)
		{
			player.network.ExecSwapItem(idx1, idx2);
		}
		player.data.arrInventory[idx1].IdxInventory = idx1;
		player.data.arrInventory[idx2].IdxInventory = idx2;
		if (idx1 <= 1)
		{
			player.data.arrInventory[idx2].equip = false;
		}
		if (idx2 <= 1)
		{
			player.data.arrInventory[idx1].equip = false;
		}
		bool isSwap = false;
		bool flag = false;
		if (player.data.arrInventory[idx1].statusEffects.Count > 0 && player.data.arrInventory[idx2].statusEffects.Count > 0)
		{
			isSwap = true;
			if (player.data.arrInventory[idx1].Name == player.data.arrInventory[idx2].Name)
			{
				flag = true;
			}
		}
		foreach (InventoryObject.StatusEffectItemObject statusEffect in player.data.arrInventory[idx1].statusEffects)
		{
			if (!statusEffect.AdditionalName.IsNullOrWhitespace())
			{
				string oldKey = statusEffect.StatusEffectSo.StatusEffectData.Name;
				statusEffect.StatusEffectSo.StatusEffectData.SetAdditionalName(player.data.arrInventory[idx1].ID.ToString(), player.data.arrInventory[idx1].IdxInventory.ToString());
				statusEffect.SetAdditionalName(player.data.arrInventory[idx1].ID.ToString(), player.data.arrInventory[idx1].IdxInventory.ToString());
				if (!flag)
				{
					player.StatusEffectController.ChangeKeyStatusEffect(oldKey, statusEffect.StatusEffectSo.StatusEffectData.Name, isSwap, flag);
				}
			}
		}
		foreach (InventoryObject.StatusEffectItemObject statusEffect2 in player.data.arrInventory[idx2].statusEffects)
		{
			if (!statusEffect2.AdditionalName.IsNullOrWhitespace())
			{
				string oldKey2 = statusEffect2.StatusEffectSo.StatusEffectData.Name;
				statusEffect2.StatusEffectSo.StatusEffectData.SetAdditionalName(player.data.arrInventory[idx2].ID.ToString(), player.data.arrInventory[idx2].IdxInventory.ToString());
				statusEffect2.SetAdditionalName(player.data.arrInventory[idx2].ID.ToString(), player.data.arrInventory[idx2].IdxInventory.ToString());
				player.StatusEffectController.ChangeKeyStatusEffect(oldKey2, statusEffect2.StatusEffectSo.StatusEffectData.Name, isSwap, flag);
			}
		}
		ItemInventorySlotUI itemInventorySlotUI = GetItemInventorySlotUI(idx1);
		ItemInventorySlotUI itemInventorySlotUI2 = GetItemInventorySlotUI(idx2);
		if ((bool)itemInventorySlotUI && (bool)itemInventorySlotUI2)
		{
			SwapUI(itemInventorySlotUI, itemInventorySlotUI2);
			InventoryObject inventoryObject2 = player.data.arrInventory[idx1];
			InventoryObject inventoryObject3 = player.data.arrInventory[idx2];
			itemInventorySlotUI.SetActiveEquip(inventoryObject2.ID > 0 && idx1 > 1 && inventoryObject2.equip);
			itemInventorySlotUI2.SetActiveEquip(inventoryObject3.ID > 0 && idx2 > 1 && inventoryObject3.equip);
		}
		OnInventorySlotChangedEvent?.Invoke();
		void SwapUI(ItemInventorySlotUI ui1, ItemInventorySlotUI ui2)
		{
			string text = ui1.GetAmmoText()?.text;
			string text2 = ui1.GetArmorText().text;
			bool activeSelf = ui1.GetArmorText().gameObject.activeSelf;
			string text3 = ui2.GetArmorText().text;
			bool activeSelf2 = ui2.GetArmorText().gameObject.activeSelf;
			if (activeSelf)
			{
				ui2.SetActiveArmor(text2);
			}
			else
			{
				ui2.SetActiveArmor();
			}
			if (activeSelf2)
			{
				ui1.SetActiveArmor(text3);
			}
			else
			{
				ui1.SetActiveArmor();
			}
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			if (txtAmountList[idx1].gameObject.activeSelf)
			{
				flag2 = true;
			}
			if (txtAmountList[idx2].gameObject.activeSelf)
			{
				flag3 = true;
			}
			if (ammoIconList[idx1].gameObject.activeSelf)
			{
				flag4 = true;
			}
			if (ammoIconList[idx2].gameObject.activeSelf)
			{
				flag5 = true;
			}
			if (txtAmountList[idx2].gameObject.activeSelf)
			{
				txtAmountList[idx1].gameObject.SetActive(value: true);
			}
			else
			{
				txtAmountList[idx1].gameObject.SetActive(value: false);
			}
			if (flag2)
			{
				txtAmountList[idx2].gameObject.SetActive(value: true);
			}
			else
			{
				txtAmountList[idx2].gameObject.SetActive(value: false);
			}
			if (flag4)
			{
				ammoIconList[idx2].gameObject.SetActive(value: true);
			}
			else
			{
				ammoIconList[idx2].gameObject.SetActive(value: false);
			}
			if (flag3)
			{
				txtAmountList[idx1].gameObject.SetActive(value: true);
				ammoIconList[idx1].gameObject.SetActive(value: true);
			}
			else
			{
				txtAmountList[idx1].gameObject.SetActive(value: false);
				ammoIconList[idx1].gameObject.SetActive(value: false);
			}
			if (flag5)
			{
				ammoIconList[idx1].gameObject.SetActive(value: true);
			}
			else
			{
				ammoIconList[idx1].gameObject.SetActive(value: false);
			}
			txtAmountList[idx1].text = txtAmountList[idx2].text;
			txtAmountList[idx2].text = text;
		}
	}

	public void FunctionUnloadAmmo(int idx)
	{
		if (player.data.arrInventory[idx].Ammo > 0 && BGDatabase_Weapon.GetEntityByKeyid(player.data.arrInventory[idx].ID).WeaponType == "Range")
		{
			int baseWeaponID = BGDatabase_Weapon.GetEntityByKeyid(player.data.arrInventory[idx].ID).BaseWeaponID;
			if (player.data.AddInventory(BGDatabase_Weapon.GetEntityByKeyid(baseWeaponID).AmmoTypeID, isOnPick: true, player.data.arrInventory[idx].Ammo) != -1)
			{
				player.data.arrInventory[idx].Ammo = 0;
				txtAmountList[idx].text = "0";
				player.network.ExecSyncDataInventory(idx, 0);
			}
			else
			{
				player.network.SetSpawnItem(BGDatabase_Weapon.GetEntityByKeyid(player.data.arrInventory[idx].ID).AmmoTypeID, player.weaponPos.position, player.data.arrInventory[idx].Ammo);
				player.data.arrInventory[idx].Ammo = 0;
				txtAmountList[idx].text = "0";
				player.network.ExecSyncDataInventory(idx, 0);
			}
			if (UIGameManager.Instance != null && player.network.isLocalPlayer)
			{
				player.data.InitImageInventoryLocal();
			}
			OnInventorySlotChangedEvent?.Invoke();
		}
	}

	public void FunctionDismantleItem(int idx)
	{
		if (player.data.arrInventory[idx] == null)
		{
			return;
		}
		InventoryObject inventoryObject = new InventoryObject(player.data.arrInventory[idx]);
		ItemToCraftMaterialConverter.ConvertMaterialItemData convertMaterialItemData = ItemToCraftMaterialConverter.DismantleItemToCraftMaterial(inventoryObject);
		if (convertMaterialItemData == null)
		{
			return;
		}
		foreach (MaterialInventoryData value in convertMaterialItemData.Material.Values)
		{
			value.CraftMaterialScriptableObject.AddMaterial(player.data, value.Amount);
		}
		OnDismantleItem?.Invoke(inventoryObject);
		player.data.RemoveInventory(idx);
		if (UIGameManager.Instance != null && player.network.isLocalPlayer)
		{
			player.data.InitImageInventoryLocal();
		}
		OnInventorySlotChangedEvent?.Invoke();
		CheckMainThrowableItemAmount();
	}

	public void SetCanDismantle(bool canDismantle)
	{
		_canDismantle = canDismantle;
	}

	public bool GetStatusDismantle()
	{
		return _canDismantle;
	}

	public void SetGrayOutSlotDismantle(bool active)
	{
		if (active)
		{
			for (int i = 0; i < itemSlotList.Count; i++)
			{
				ItemSlot itemSlot = itemSlotList[i];
				if (itemSlot != null)
				{
					if (i == 0)
					{
						itemSlot.SetGreyOutSlot(active: true);
					}
					else if (itemSlot.gameObject.activeSelf)
					{
						InventoryObject inventoryObject = player.data.arrInventory[i];
						itemSlot.SetGreyOutSlot(BGDatabase_ItemDismantle.GetEntityByKeyItem(inventoryObject.ID) == null);
					}
				}
			}
		}
		else
		{
			for (int j = 0; j < itemSlotList.Count; j++)
			{
				itemSlotList[j].SetGreyOutSlot(active: false);
			}
		}
	}

	public void CheckMainThrowableItemAmount()
	{
		int num = player.data.FindTotalInventory(player.data.idThrowable);
		if (player.data.idThrowable <= 0)
		{
			num = 0;
		}
		if (player.network.isLocalPlayer)
		{
			txtAmountThrowableItem.text = num.ToString();
		}
		if (num <= 0)
		{
			UIGameManager.Instance.HideThrowableShortcutSprite();
			if (player.network.isLocalPlayer)
			{
				txtAmountThrowableItem.text = "";
			}
			player.data.idThrowable = -1;
			player.canGrenade = false;
		}
	}

	private void FillSlot(InventoryObject from, InventoryObject to)
	{
		from.SetInventoryObject(to);
	}

	public void WeaponEquip(int idx, int ammo = -1, bool init = false)
	{
		player.weaponController.EquipWeaponInventory(idx, ammo, init);
		UIGameManager.Instance.inventoryOptions.SetActive(value: false);
		targetInventory = null;
	}

	public void WeaponEquip()
	{
		int num = int.Parse(targetInventory.name.Substring(13, targetInventory.name.Length - 13));
		player.weaponController.EquipWeaponInventory(num, player.data.arrInventory[num].Ammo);
		UIGameManager.Instance.inventoryOptions.SetActive(value: false);
		targetInventory.GetComponent<Button>().Select();
		targetInventory = null;
	}

	public void WeaponUnequip()
	{
		player.weaponController.UnEquipWeapon(int.Parse(targetInventory.name.Substring(13, targetInventory.name.Length - 13)), fromServer: false);
		UIGameManager.Instance.inventoryOptions.SetActive(value: false);
		targetInventory = null;
	}

	public void CustomEquipItem()
	{
		OnCustomEquipItemAction?.Invoke();
		UIGameManager.Instance.inventoryOptions.SetActive(value: false);
		targetInventory.GetComponent<Button>().Select();
		targetInventory = null;
	}

	public void CustomUnequipItem()
	{
		OnCustomUnequipItemAction?.Invoke();
		UIGameManager.Instance.inventoryOptions.SetActive(value: false);
		targetInventory.GetComponent<Button>().Select();
		targetInventory = null;
	}

	public void SelectButtonUnity(Button button)
	{
		if (!UIGameManager.Instance.uiInventory.isHidden)
		{
			button.Select();
		}
	}

	public void SetNavigationInventory()
	{
		foreach (InventoryItemSlotButton buttonInventory in buttonInventoryList)
		{
			buttonInventory.SetNavigationInventory();
		}
	}

	public void SetNavigationCrafting()
	{
		foreach (InventoryItemSlotButton buttonInventory in buttonInventoryList)
		{
			buttonInventory.SetNavigationCrafting();
		}
	}

	public void SelectFirstButton()
	{
		SelectButton(0);
	}

	public void SelectButton(int index)
	{
		if (buttonInventory[index] != null)
		{
			buttonInventory[index].Select();
			ItemShowDesc(index);
		}
	}

	public void OnSelectButtonInventory(BaseEventData eventData)
	{
		if (UIGameManager.Instance.uiInventory.isVisible && GetStatusDismantle())
		{
			string text = eventData.selectedObject.name;
			int index = int.Parse(EventSystem.current.currentSelectedGameObject.transform.name.Substring(13, text.Length - 13));
			OnSelectDismantleItem?.Invoke(player.data.arrInventory[index]);
		}
	}

	public void OnDeselectButtonInventory(BaseEventData eventData)
	{
	}

	public ItemInventorySlotUI GetItemInventorySlotUI(int index)
	{
		if (index >= 0 && index < _inventorySlotUI.Count)
		{
			return _inventorySlotUI[index];
		}
		return null;
	}
}

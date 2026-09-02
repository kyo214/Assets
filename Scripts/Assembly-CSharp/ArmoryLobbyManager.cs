using System.Collections.Generic;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using I2.Loc;
using TMPro;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ArmoryLobbyManager : MonoBehaviour
{
	public UIView UIMenu;

	[SerializeField]
	public string TabItem;

	[SerializeField]
	private List<UIButton> _ListBtnTab = new List<UIButton>();

	public List<ItemSlot> ListItemSlot = new List<ItemSlot>();

	public List<GameObject> ListHighlight = new List<GameObject>();

	public List<TextMeshProUGUI> TxtAmount = new List<TextMeshProUGUI>();

	public List<Image> ListIconAmmo = new List<Image>();

	public PlayerInputActions input;

	public int targetIdxButton;

	public int targetIdxItem;

	public GameObject OptionMenu;

	public Button defaultOptionButton;

	[SerializeField]
	private TextMeshProUGUI _txtDrop;

	public static ArmoryLobbyManager Instance { get; private set; }

	private void OnEnable()
	{
		input = new PlayerInputActions();
		input.UI.Enable();
		input.UI.LeftTab.performed += OnInputLeftTab;
		input.UI.RightTab.performed += OnInputRightTab;
	}

	private void OnDisable()
	{
		input.UI.LeftTab.performed -= OnInputLeftTab;
		input.UI.RightTab.performed -= OnInputRightTab;
		input.UI.Disable();
	}

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
		UIMenu = GetComponent<UIView>();
	}

	public void Show()
	{
		if ((bool)UIGameManager.Instance.uIInGameController.MissionLobby)
		{
			UIGameManager.Instance.uIInGameController.MissionLobby.SetActive(value: false);
		}
		TabItem = "All";
		ShowItem("All");
		UIGameManager.Instance.animUIInventory.PlayFromToProgress(0f, 1f);
		NetworkGameManager.Instance.ownPlayer.network.SetPlayerAFK(value: true);
		AudioManager.PlaySFX("inventory-open");
		UIGameManager.Instance.uiInventory.Show();
		if (!UIGameManager.Instance.isUIInvisible)
		{
			UIGameManager.Instance.uiInGame.Show();
		}
		if (UIGameManager.Instance.uiObjective != null)
		{
			UIGameManager.Instance.uiObjective.SetActive(value: false);
		}
		UIGameManager.Instance.fpsObject.SetActive(value: false);
		int num = 0;
		for (int i = 0; i < NetworkGameManager.Instance.ownPlayer.inventoryManager.buttonInventory.Count; i++)
		{
			Button button = NetworkGameManager.Instance.ownPlayer.inventoryManager.buttonInventory[i];
			button.interactable = true;
			if ((i + 1) % 2 != 0 && num * 3 + 2 < 12)
			{
				Navigation navigation = button.navigation;
				navigation.selectOnLeft = ListItemSlot[num * 3 + 2].ButtonItem;
				button.navigation = navigation;
				if (i >= 2)
				{
					num++;
				}
			}
		}
		num = 0;
		foreach (ItemSlot item in ListItemSlot)
		{
			if ((item.idxSlot + 1) % 3 == 0)
			{
				Navigation navigation2 = item.ButtonItem.navigation;
				navigation2.selectOnRight = NetworkGameManager.Instance.ownPlayer.inventoryManager.buttonInventory[num * 2 + 2];
				item.ButtonItem.navigation = navigation2;
				num++;
			}
		}
		foreach (GameObject item2 in NetworkGameManager.Instance.ownPlayer.inventoryManager.KeyButtonInfo)
		{
			item2.SetActive(value: false);
		}
		ListItemSlot[0].ButtonItem.Select();
		NetworkGameManager.Instance.ownPlayer?.inventoryManager.AdditionalKeyBtnObject?.SetActive(value: false);
	}

	public void Hide()
	{
		if ((bool)UIGameManager.Instance.uIInGameController.MissionLobby)
		{
			UIGameManager.Instance.uIInGameController.MissionLobby.SetActive(value: true);
		}
		OptionMenu.SetActive(value: false);
		UIGameManager.Instance.uiInventory.Hide();
		UIGameManager.Instance.animUIInventory.PlayFromToProgress(1f, 0f);
		if (UIGameManager.Instance.uiObjective != null)
		{
			UIGameManager.Instance.uiObjective.SetActive(value: true);
		}
		if (GlobalSaveData.instance.optionData.showFpsRtt)
		{
			UIGameManager.Instance.fpsObject.SetActive(value: true);
		}
		if (NetworkGameManager.Instance.ownPlayer != null && NetworkGameManager.Instance.ownPlayer.inventoryManager != null)
		{
			for (int i = 0; i < NetworkGameManager.Instance.ownPlayer.inventoryManager.buttonInventory.Count; i++)
			{
				Button button = NetworkGameManager.Instance.ownPlayer.inventoryManager.buttonInventory[i];
				if ((i + 1) % 2 != 0)
				{
					Navigation navigation = button.navigation;
					navigation.selectOnLeft = null;
					button.navigation = navigation;
				}
			}
		}
		UIGameManager.Instance.uiInGame.canvas.sortingOrder = 0;
		UIGameManager.Instance.uiInGame.canvas.overrideSorting = false;
		if (NetworkGameManager.Instance.ownPlayer != null)
		{
			foreach (GameObject item in NetworkGameManager.Instance.ownPlayer.inventoryManager.KeyButtonInfo)
			{
				if (NetworkGameManager.Instance.ownPlayer.inventoryManager != null && item != null)
				{
					item.SetActive(value: true);
				}
			}
		}
		if ((bool)NetworkGameManager.Instance.ownPlayer && (bool)NetworkGameManager.Instance.ownPlayer.inventoryManager.AdditionalKeyBtnObject)
		{
			NetworkGameManager.Instance.ownPlayer.inventoryManager.AdditionalKeyBtnObject.SetActive(value: true);
		}
	}

	public void SelectTab(string tabItem)
	{
		TabItem = tabItem;
		ShowItem(tabItem);
	}

	public void DeactivateButton(UIButton button)
	{
		foreach (UIButton item in _ListBtnTab)
		{
			item.interactable = true;
		}
		button.interactable = false;
	}

	public void SelectButton(UIButton button)
	{
		button.Select();
	}

	public void SelectButtonUnity(Button button)
	{
		button.Select();
	}

	public void ShowItem(string tabItem = "")
	{
		if (UIMenu.isHidden)
		{
			return;
		}
		OptionMenu.SetActive(value: false);
		if (tabItem == "")
		{
			tabItem = TabItem;
		}
		for (int i = 0; i < ListItemSlot.Count; i++)
		{
			TxtAmount[i].text = "";
			ListIconAmmo[i].enabled = false;
			ListItemSlot[i].imageItem.enabled = false;
			ListItemSlot[i].imageItem.sprite = null;
		}
		if (GameModes.Instance.isItemBoxGlobal)
		{
			int num = 0;
			for (int j = 0; j < ItemBoxNetwork.instance.arrItem.Length; j++)
			{
				if (ItemBoxNetwork.instance.arrItem.Get(j).ID != -1 && (ItemBoxNetwork.instance.GetItemType(ItemBoxNetwork.instance.arrItem.Get(j).ID) == tabItem || tabItem == "All"))
				{
					if (ItemBoxNetwork.instance.GetItemType(ItemBoxNetwork.instance.arrItem.Get(j).ID) == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(ItemBoxNetwork.instance.arrItem.Get(j).ID).WeaponType == "Range")
					{
						ListIconAmmo[num].enabled = true;
						TxtAmount[num].text = ItemBoxNetwork.instance.arrItem.Get(j).Ammo.ToString();
					}
					else if (ItemBoxNetwork.instance.GetItemType(ItemBoxNetwork.instance.arrItem.Get(j).ID) == "Ammunition")
					{
						TxtAmount[num].text = ItemBoxNetwork.instance.arrItem.Get(j).Amount.ToString();
					}
					ListIconAmmo[num].color = new Color(1f, 1f, 1f, 1f);
					TxtAmount[num].color = new Color(1f, 1f, 1f, 1f);
					ListItemSlot[num].SetDraggable(newValue: true);
					ListItemSlot[num].imageItem.enabled = true;
					ListItemSlot[num].imageItem.sprite = DataManager.Instance.GetItemSprite(ItemBoxNetwork.instance.arrItem.Get(j).ID.ToString());
					ListItemSlot[num].imageItem.color = new Color(1f, 1f, 1f, 1f);
					num++;
				}
			}
			return;
		}
		int num2 = 0;
		for (int k = 0; k < NetworkGameManager.Instance.ownPlayer.ItemBoxController.arrItem.Count; k++)
		{
			if (NetworkGameManager.Instance.ownPlayer.ItemBoxController.arrItem[k].ItemType == tabItem || tabItem == "All")
			{
				if (NetworkGameManager.Instance.ownPlayer.ItemBoxController.arrItem[k].ItemType == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(NetworkGameManager.Instance.ownPlayer.ItemBoxController.arrItem[k].ID).WeaponType == "Range")
				{
					ListIconAmmo[num2].enabled = true;
					TxtAmount[num2].text = NetworkGameManager.Instance.ownPlayer.ItemBoxController.arrItem[k].Ammo.ToString();
				}
				else if (NetworkGameManager.Instance.ownPlayer.ItemBoxController.arrItem[k].ItemType == "Ammunition")
				{
					TxtAmount[num2].text = NetworkGameManager.Instance.ownPlayer.ItemBoxController.arrItem[k].Amount.ToString();
				}
				ListIconAmmo[num2].color = new Color(1f, 1f, 1f, 1f);
				TxtAmount[num2].color = new Color(1f, 1f, 1f, 1f);
				ListItemSlot[num2].SetDraggable(newValue: true);
				ListItemSlot[num2].imageItem.enabled = true;
				ListItemSlot[num2].imageItem.sprite = DataManager.Instance.GetItemSprite(NetworkGameManager.Instance.ownPlayer.ItemBoxController.arrItem[k].ID.ToString());
				ListItemSlot[num2].imageItem.color = new Color(1f, 1f, 1f, 1f);
				num2++;
			}
		}
		if (!(tabItem != "All"))
		{
			return;
		}
		for (int l = 0; l < NetworkGameManager.Instance.ownPlayer.ItemBoxController.arrItem.Count; l++)
		{
			if (NetworkGameManager.Instance.ownPlayer.ItemBoxController.arrItem[l].ItemType != tabItem)
			{
				if (NetworkGameManager.Instance.ownPlayer.ItemBoxController.arrItem[l].ItemType == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(NetworkGameManager.Instance.ownPlayer.ItemBoxController.arrItem[l].ID).WeaponType == "Range")
				{
					ListIconAmmo[num2].enabled = true;
					TxtAmount[num2].text = NetworkGameManager.Instance.ownPlayer.ItemBoxController.arrItem[l].Ammo.ToString();
				}
				else if (NetworkGameManager.Instance.ownPlayer.ItemBoxController.arrItem[l].ItemType == "Ammunition")
				{
					TxtAmount[num2].text = NetworkGameManager.Instance.ownPlayer.ItemBoxController.arrItem[l].Amount.ToString();
				}
				ListIconAmmo[num2].color = new Color(1f, 1f, 1f, 0.1f);
				TxtAmount[num2].color = new Color(1f, 1f, 1f, 0.1f);
				ListItemSlot[num2].SetDraggable(newValue: false);
				ListItemSlot[num2].imageItem.enabled = true;
				ListItemSlot[num2].imageItem.sprite = DataManager.Instance.GetItemSprite(NetworkGameManager.Instance.ownPlayer.ItemBoxController.arrItem[l].ID.ToString());
				ListItemSlot[num2].imageItem.color = new Color(1f, 1f, 1f, 0.1f);
				num2++;
			}
		}
	}

	public void OnInputLeftTab(InputAction.CallbackContext value)
	{
		int num = -1;
		for (int i = 0; i < _ListBtnTab.Count; i++)
		{
			if (!_ListBtnTab[i].interactable)
			{
				num = i;
				_ListBtnTab[i].interactable = true;
			}
		}
		num = ((num > 0) ? (num - 1) : (_ListBtnTab.Count - 1));
		_ListBtnTab[num].interactable = false;
		SelectTab(_ListBtnTab[num].name);
	}

	public void OnInputRightTab(InputAction.CallbackContext value)
	{
		int num = -1;
		for (int i = 0; i < _ListBtnTab.Count; i++)
		{
			if (!_ListBtnTab[i].interactable)
			{
				num = i;
				_ListBtnTab[i].interactable = true;
			}
		}
		num = ((num < _ListBtnTab.Count - 1) ? (num + 1) : 0);
		_ListBtnTab[num].interactable = false;
		SelectTab(_ListBtnTab[num].name);
	}

	public void ShowDesc()
	{
		if (!(EventSystem.current.currentSelectedGameObject != null))
		{
			return;
		}
		Transform transform = EventSystem.current.currentSelectedGameObject.transform;
		int idxSlot = int.Parse(transform.name.Substring(13, transform.name.Length - 13));
		PlayerController ownPlayer = NetworkGameManager.Instance.ownPlayer;
		int idxArmory = GetIdxArmory(ownPlayer, idxSlot);
		if (idxArmory != -1)
		{
			InventoryObject inventoryObject = ownPlayer.ItemBoxController.arrItem[idxArmory];
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
		}
		else
		{
			UIGameManager.Instance.titleWeapon.SetTerm("EmptyField");
			UIGameManager.Instance.dscWeapon.SetTerm("EmptyField");
			if (UIGameManager.Instance.titleWeaponText != null)
			{
				UIGameManager.Instance.titleWeaponText.text = "";
				UIGameManager.Instance.dscWeaponText.text = "";
			}
		}
	}

	private int GetIdxArmory(PlayerController player, int idxSlot)
	{
		int num = -1;
		int result = -1;
		for (int i = 0; i < player.ItemBoxController.arrItem.Count; i++)
		{
			if (Instance.TabItem == "All" || player.ItemBoxController.arrItem[i].ItemType == Instance.TabItem)
			{
				num++;
				if (num == idxSlot)
				{
					result = i;
					break;
				}
			}
		}
		return result;
	}

	public void ClickItemButton()
	{
		if (UIMenu.isHidden)
		{
			return;
		}
		Transform transform = EventSystem.current.currentSelectedGameObject.transform;
		PlayerController ownPlayer = NetworkGameManager.Instance.ownPlayer;
		targetIdxButton = int.Parse(transform.name.Substring(13, transform.name.Length - 13));
		int idxArmory = GetIdxArmory(ownPlayer, targetIdxButton);
		if (idxArmory <= -1 || OptionMenu.activeSelf)
		{
			OptionMenu.SetActive(value: false);
			for (int i = 0; i < ListHighlight.Count; i++)
			{
				ListHighlight[i].SetActive(value: false);
			}
		}
		else
		{
			OptionMenu.SetActive(value: true);
			_txtDrop.text = LocalizationManager.GetTranslation("Menu/Drop").ToUpper();
			_txtDrop.text = _txtDrop.text.Replace(" (X)", " ");
			targetIdxItem = idxArmory;
			defaultOptionButton.Select();
			ListHighlight[targetIdxButton].SetActive(value: true);
		}
	}

	public void TakeItem()
	{
		if (targetIdxItem >= 0)
		{
			NetworkGameManager.Instance.ownPlayer.inventoryManager.FunctionItemPutToInventoryFromArmory(targetIdxItem);
			ShowItem();
			ResetUI();
		}
	}

	public void DropItem()
	{
		if (targetIdxItem >= 0)
		{
			NetworkGameManager.Instance.ownPlayer.inventoryManager.FunctionItemDropArmory(targetIdxItem);
			ShowItem();
			ResetUI();
		}
	}

	public void ResetUI()
	{
		OptionMenu.SetActive(value: false);
		for (int i = 0; i < ListHighlight.Count; i++)
		{
			ListHighlight[i].SetActive(value: false);
		}
		ListItemSlot[targetIdxButton].ButtonItem.Select();
		targetIdxItem = -1;
		targetIdxButton = -1;
	}
}

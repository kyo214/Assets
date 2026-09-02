using DG.Tweening;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using _Modules.Dismantle.Scripts;

public class ItemSlot : MonoBehaviour, IPointerUpHandler, IEventSystemHandler, IPointerDownHandler, IEndDragHandler, IBeginDragHandler, IDragHandler
{
	private enum ItemSlotType
	{
		Inventory = 0,
		Armory = 1
	}

	public int idxSlot;

	[SerializeField]
	private ItemSlotType _itemSlotType;

	public Image imageItem;

	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private Vector2 prevAnchorPos;

	[SerializeField]
	private Vector2 initAnchorPos;

	[SerializeField]
	private bool isDragging;

	[SerializeField]
	private bool isDragable = true;

	[SerializeField]
	private bool isSwapable = true;

	[SerializeField]
	private bool isHealingSlot;

	[SerializeField]
	private bool isThrowableSlot;

	[SerializeField]
	private bool isWeapon;

	[SerializeField]
	private bool isCanSwapWithNull = true;

	public Button ButtonItem;

	[SerializeField]
	private float clicked;

	[SerializeField]
	private float clicktime;

	[SerializeField]
	private float clickdelay = 0.5f;

	[SerializeField]
	private Image flashImage;

	private void Start()
	{
		if (imageItem != null)
		{
			initAnchorPos = imageItem.rectTransform.anchoredPosition;
		}
		if (base.name.Length > 13)
		{
			idxSlot = int.Parse(base.name.Substring(13, base.name.Length - 13));
		}
		else
		{
			idxSlot = -1;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (!isDragging || !isDragable)
		{
			return;
		}
		isDragging = false;
		InventoryManager inventoryManager = NetworkGameManager.Instance.ownPlayer.inventoryManager;
		GameObject target = eventData.pointerCurrentRaycast.gameObject;
		bool flag = false;
		if (_itemSlotType == ItemSlotType.Armory)
		{
			flag = true;
		}
		if (eventData.pointerCurrentRaycast.gameObject != null)
		{
			if (inventoryManager.GetStatusDismantle())
			{
				ItemSlotDismantleDropDetector component = eventData.pointerCurrentRaycast.gameObject.GetComponent<ItemSlotDismantleDropDetector>();
				if ((bool)component)
				{
					component.DropAction(int.Parse(base.name.Substring(13, base.name.Length - 13)));
					imageItem.rectTransform.anchoredPosition = initAnchorPos;
				}
				else
				{
					OnDropInventory();
				}
			}
			else
			{
				OnDropInventory();
			}
		}
		else
		{
			if (!inventoryManager.CanBeDrop(int.Parse(base.name.Substring(13, base.name.Length - 13)), flag))
			{
				return;
			}
			if (flag)
			{
				int idxArmory = GetIdxArmory(inventoryManager.player, idxSlot);
				if (idxArmory >= 0)
				{
					inventoryManager.FunctionItemDropArmory(idxArmory);
					imageItem.rectTransform.anchoredPosition = initAnchorPos;
					ArmoryLobbyManager.Instance.ShowItem();
				}
			}
			else
			{
				inventoryManager.FunctionItemDrop(int.Parse(base.name.Substring(13, base.name.Length - 13)), isSwapWeapon: false);
				HideItemImage();
			}
		}
		void HideItemImage()
		{
			imageItem.rectTransform.anchoredPosition = initAnchorPos;
			imageItem.sprite = null;
			imageItem.DOFade(0f, 0f);
		}
		void OnDropInventory()
		{
			ItemSlot component2 = eventData.pointerCurrentRaycast.gameObject.GetComponent<ItemSlot>();
			if (component2 != null || target.name == "Board" || target.name == "FrameItem")
			{
				if (component2 != null)
				{
					if (component2 != this)
					{
						if (component2.isHealingSlot && inventoryManager.player.data.arrInventory[idxSlot].ItemType == "HealingItem")
						{
							inventoryManager.FunctionItemAssign(int.Parse(base.name.Substring(13, base.name.Length - 13)));
							imageItem.rectTransform.anchoredPosition = initAnchorPos;
							AudioManager.PlaySFX("inventory_assign");
						}
						else if (component2.isThrowableSlot && inventoryManager.player.data.arrInventory[idxSlot].ItemType == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(inventoryManager.player.data.arrInventory[idxSlot].ID).WeaponType == "Throw")
						{
							inventoryManager.FunctionItemAssign(int.Parse(base.name.Substring(13, base.name.Length - 13)));
							imageItem.rectTransform.anchoredPosition = initAnchorPos;
							AudioManager.PlaySFX("inventory_assign");
						}
						else if (component2.isSwapable)
						{
							int num = int.Parse(component2.name.Substring(13, component2.name.Length - 13));
							if (_itemSlotType == ItemSlotType.Inventory && component2._itemSlotType == ItemSlotType.Inventory)
							{
								bool flag2 = false;
								bool flag3 = false;
								bool flag4 = false;
								bool flag5 = false;
								bool flag6 = false;
								bool flag7 = false;
								bool flag8 = true;
								if (inventoryManager.player.data.arrInventory[idxSlot].ID != -1 && inventoryManager.player.data.arrInventory[idxSlot].ItemType == "Weapon")
								{
									if (BGDatabase_Weapon.GetEntityByKeyid(inventoryManager.player.data.arrInventory[idxSlot].ID).WeaponType == "Melee")
									{
										flag4 = true;
									}
									if (BGDatabase_Weapon.GetEntityByKeyid(inventoryManager.player.data.arrInventory[idxSlot].ID).WeaponType == "Range")
									{
										flag4 = false;
									}
									if (BGDatabase_Weapon.GetEntityByKeyid(inventoryManager.player.data.arrInventory[idxSlot].ID).WeaponType == "Throw")
									{
										flag5 = true;
									}
									flag2 = true;
								}
								if (inventoryManager.player.data.arrInventory[num].ID != -1 && inventoryManager.player.data.arrInventory[num].ItemType == "Weapon")
								{
									if (BGDatabase_Weapon.GetEntityByKeyid(inventoryManager.player.data.arrInventory[num].ID).WeaponType == "Melee")
									{
										flag6 = true;
									}
									if (BGDatabase_Weapon.GetEntityByKeyid(inventoryManager.player.data.arrInventory[num].ID).WeaponType == "Range")
									{
										flag6 = false;
									}
									if (BGDatabase_Weapon.GetEntityByKeyid(inventoryManager.player.data.arrInventory[num].ID).WeaponType == "Throw")
									{
										flag7 = true;
									}
									flag3 = true;
								}
								if ((!isCanSwapWithNull && inventoryManager.player.data.arrInventory[num].ID == -1) || (!component2.isCanSwapWithNull && inventoryManager.player.data.arrInventory[idxSlot].ID == -1) || (flag5 && !flag6 && num == 1) || ((!flag4 & flag7) && idxSlot == 1))
								{
									flag8 = false;
								}
								if (inventoryManager.FunctionCombiningItem(int.Parse(base.name.Substring(13, base.name.Length - 13)), int.Parse(component2.name.Substring(13, component2.name.Length - 13))))
								{
									imageItem.rectTransform.anchoredPosition = initAnchorPos;
								}
								else if (((idxSlot <= 1 && ((flag3 && flag4 == flag6) || inventoryManager.player.data.arrInventory[num].ID == -1)) || (num <= 1 && ((flag2 && flag4 == flag6) || inventoryManager.player.data.arrInventory[idxSlot].ID == -1)) || (idxSlot > 1 && num > 1)) & flag8)
								{
									AudioManager.PlaySFX("inventory-item-move");
									Sprite sprite = component2.imageItem.sprite;
									Vector2 anchoredPosition = component2.imageItem.rectTransform.anchoredPosition;
									component2.imageItem.rectTransform.anchoredPosition = imageItem.rectTransform.anchoredPosition;
									if ((idxSlot <= 1 && inventoryManager.player.data.arrInventory[num].ItemType == "Weapon") || (num <= 1 && inventoryManager.player.data.arrInventory[idxSlot].ItemType == "Weapon"))
									{
										component2.imageItem.rectTransform.DOAnchorPos(anchoredPosition, 0f);
									}
									else
									{
										component2.imageItem.rectTransform.DOAnchorPos(anchoredPosition, 0.2f);
									}
									component2.imageItem.sprite = imageItem.sprite;
									if (component2.imageItem.sprite != null)
									{
										component2.imageItem.DOFade(1f, 0f);
									}
									else
									{
										component2.imageItem.DOFade(0f, 0f);
									}
									imageItem.rectTransform.anchoredPosition = anchoredPosition;
									if ((idxSlot <= 1 && inventoryManager.player.data.arrInventory[num].ItemType == "Weapon") || (num <= 1 && inventoryManager.player.data.arrInventory[idxSlot].ItemType == "Weapon"))
									{
										imageItem.rectTransform.DOAnchorPos(initAnchorPos, 0f);
									}
									else
									{
										imageItem.rectTransform.DOAnchorPos(initAnchorPos, 0.2f);
									}
									imageItem.sprite = sprite;
									if (imageItem.sprite != null)
									{
										imageItem.DOFade(1f, 0f);
									}
									else
									{
										imageItem.DOFade(0f, 0f);
									}
									inventoryManager.FunctionSwapSlot(int.Parse(base.name.Substring(13, base.name.Length - 13)), int.Parse(component2.name.Substring(13, component2.name.Length - 13)), isLocal: true);
									if (idxSlot <= 1 || num <= 1)
									{
										if (inventoryManager.player.data.arrInventory[1].ID != -1 && !flag6)
										{
											inventoryManager.WeaponEquip(1, inventoryManager.player.data.arrInventory[1].Ammo);
										}
										else if (!flag6)
										{
											inventoryManager.player.weaponController.UnEquipWeapon(1, NetworkGameManager.Instance.isServer);
										}
										if ((inventoryManager.player.data.arrInventory[0].ID != -1) & flag6)
										{
											inventoryManager.WeaponEquip(0, 0);
										}
										else if (flag6)
										{
											inventoryManager.player.weaponController.UnEquipWeapon(0, NetworkGameManager.Instance.isServer);
										}
										if (!flag6)
										{
											if (idxSlot != 1)
											{
												if (inventoryManager.player.data.arrInventory[idxSlot].ItemType == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(inventoryManager.player.data.arrInventory[idxSlot].ID).WeaponType == "Range")
												{
													inventoryManager.GetItemInventorySlotUI(idxSlot)?.SetAmmo(inventoryManager.player.data.arrInventory[idxSlot].Ammo.ToString());
												}
												else
												{
													inventoryManager.GetItemInventorySlotUI(idxSlot)?.SetActiveAmmo(inventoryManager.player.data.arrInventory[idxSlot].Ammo.ToString());
												}
											}
											else if (num != 1)
											{
												if (inventoryManager.player.data.arrInventory[num].ItemType == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(inventoryManager.player.data.arrInventory[num].ID).WeaponType == "Range")
												{
													inventoryManager.GetItemInventorySlotUI(num)?.SetAmmo(inventoryManager.player.data.arrInventory[num].Ammo.ToString());
												}
												else
												{
													inventoryManager.GetItemInventorySlotUI(num)?.SetActiveAmmo(inventoryManager.player.data.arrInventory[num].Ammo.ToString());
												}
											}
										}
									}
								}
								else
								{
									BackToPrevSlot();
								}
							}
							else if (_itemSlotType == ItemSlotType.Inventory)
							{
								if (idxSlot != 0)
								{
									if (NetworkGameManager.Instance.ownPlayer.ItemBoxController.arrItem.Count < 15 && NetworkGameManager.Instance.ownPlayer.data.arrInventory[idxSlot].ID != -1)
									{
										int idxArmory2 = GetIdxArmory(inventoryManager.player, num);
										inventoryManager.FunctionItemPutToArmory(idxSlot, idxArmory2);
										imageItem.rectTransform.anchoredPosition = initAnchorPos;
										ArmoryLobbyManager.Instance.ShowItem();
									}
									else
									{
										BackToPrevSlot();
									}
								}
							}
							else if (component2._itemSlotType == ItemSlotType.Inventory)
							{
								int idxArmory3 = GetIdxArmory(inventoryManager.player, idxSlot);
								if (idxArmory3 >= 0)
								{
									int num2 = inventoryManager.FunctionItemPutToInventoryFromArmory(idxArmory3, num);
									if (num2 != -1 && inventoryManager.player.data.arrInventory[num2].ID != -1 && BGDatabase_Weapon.GetEntityByKeyid(inventoryManager.player.data.arrInventory[num2].ID)?.WeaponType == "Melee" && component2.idxSlot == 0)
									{
										inventoryManager.WeaponEquip(num2, inventoryManager.player.data.arrInventory[num2].Ammo);
									}
									if (num2 != -1 && inventoryManager.player.data.arrInventory[num2].ID != -1 && BGDatabase_Weapon.GetEntityByKeyid(inventoryManager.player.data.arrInventory[num2].ID)?.WeaponType == "Range" && component2.idxSlot == 1)
									{
										inventoryManager.WeaponEquip(num2, inventoryManager.player.data.arrInventory[num2].Ammo);
									}
									imageItem.rectTransform.anchoredPosition = initAnchorPos;
									imageItem.sprite = null;
									ArmoryLobbyManager.Instance.ShowItem();
								}
								else
								{
									BackToPrevSlot();
								}
							}
							else
							{
								int idxArmory4 = GetIdxArmory(inventoryManager.player, idxSlot);
								int idxArmory5 = GetIdxArmory(inventoryManager.player, num);
								if (inventoryManager.FunctionCombineItemArmory(idxArmory4, idxArmory5))
								{
									imageItem.rectTransform.anchoredPosition = initAnchorPos;
									imageItem.sprite = null;
									ArmoryLobbyManager.Instance.ShowItem();
								}
								else
								{
									BackToPrevSlot();
								}
							}
						}
						else
						{
							BackToPrevSlot();
						}
					}
					else
					{
						BackToPrevSlot();
					}
				}
				else
				{
					BackToPrevSlot();
				}
			}
			else
			{
				int idx = int.Parse(base.name.Substring(13, base.name.Length - 13));
				if (_itemSlotType == ItemSlotType.Inventory)
				{
					if (inventoryManager.CanBeDrop(idx))
					{
						inventoryManager.FunctionItemDrop(idx, isSwapWeapon: false);
						HideItemImage();
					}
				}
				else
				{
					int idxArmory6 = GetIdxArmory(inventoryManager.player, idx);
					if (idxArmory6 >= 0)
					{
						inventoryManager.FunctionItemDropArmory(idxArmory6);
						imageItem.rectTransform.anchoredPosition = initAnchorPos;
						ArmoryLobbyManager.Instance.ShowItem();
					}
					else
					{
						BackToPrevSlot();
					}
				}
			}
		}
	}

	private void BackToPrevSlot()
	{
		AudioManager.PlaySFX("inventory-item-move");
		imageItem.rectTransform.DOAnchorPos(initAnchorPos, 0.2f);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		AudioManager.PlaySFX("inventory_select");
		if (Mouse.current.rightButton.IsPressed() && isDragable && !NetworkGameManager.Instance.ownPlayer.network.GetEnableControl())
		{
			InventoryManager inventoryManager = NetworkGameManager.Instance.ownPlayer.inventoryManager;
			if (_itemSlotType == ItemSlotType.Inventory)
			{
				if (inventoryManager.CanBeDrop(idxSlot))
				{
					inventoryManager.FunctionItemDrop(idxSlot, isSwapWeapon: false);
					imageItem.rectTransform.anchoredPosition = initAnchorPos;
					imageItem.sprite = null;
					imageItem.DOFade(0f, 0f);
				}
			}
			else
			{
				int idxArmory = GetIdxArmory(inventoryManager.player, idxSlot);
				if (idxArmory >= 0)
				{
					inventoryManager.FunctionItemDropArmory(idxArmory);
					ArmoryLobbyManager.Instance.ShowItem();
				}
			}
		}
		clicked++;
		if (clicked == 1f)
		{
			clicktime = Time.time;
		}
		if (clicked > 1f && Time.time - clicktime < clickdelay)
		{
			clicked = 0f;
			clicktime = 0f;
			DoubleClick();
		}
		else if (clicked > 2f || Time.time - clicktime > 1f)
		{
			clicked = 0f;
		}
	}

	public void DoubleClick()
	{
		InventoryManager inventoryManager = NetworkGameManager.Instance.ownPlayer.inventoryManager;
		if (ArmoryLobbyManager.Instance != null && !ArmoryLobbyManager.Instance.UIMenu.isHidden)
		{
			if (_itemSlotType == ItemSlotType.Inventory && idxSlot != 0)
			{
				if (NetworkGameManager.Instance.ownPlayer.ItemBoxController.arrItem.Count < 15 && NetworkGameManager.Instance.ownPlayer.data.arrInventory[idxSlot].ID != -1)
				{
					inventoryManager.FunctionItemPutToArmory(idxSlot);
					imageItem.rectTransform.anchoredPosition = initAnchorPos;
					imageItem.sprite = null;
					ArmoryLobbyManager.Instance.ShowItem();
				}
			}
			else if (_itemSlotType != ItemSlotType.Inventory)
			{
				int idxArmory = GetIdxArmory(inventoryManager.player, idxSlot);
				if (idxArmory >= 0)
				{
					inventoryManager.FunctionItemPutToInventoryFromArmory(idxArmory);
					imageItem.rectTransform.anchoredPosition = initAnchorPos;
					imageItem.sprite = null;
					ArmoryLobbyManager.Instance.ShowItem();
				}
			}
		}
		else if (_itemSlotType == ItemSlotType.Inventory && idxSlot > 1)
		{
			if (inventoryManager.player.data.arrInventory[idxSlot].ItemType == "HealingItem")
			{
				inventoryManager.FunctionItemUse(int.Parse(base.name.Substring(13, base.name.Length - 13)));
			}
			else if (inventoryManager.player.data.arrInventory[idxSlot].ItemType == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(inventoryManager.player.data.arrInventory[idxSlot].ID).WeaponType == "Throw")
			{
				inventoryManager.FunctionItemAssign(int.Parse(base.name.Substring(13, base.name.Length - 13)));
				AudioManager.PlaySFX("inventory_assign");
			}
			else if (inventoryManager.player.data.arrInventory[idxSlot].ItemType == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(inventoryManager.player.data.arrInventory[idxSlot].ID).WeaponType == "Range")
			{
				AudioManager.PlaySFX("rangedReload_" + inventoryManager.player.data.arrInventory[idxSlot].ID);
				inventoryManager.WeaponEquip(idxSlot, inventoryManager.player.data.arrInventory[idxSlot].Ammo);
			}
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (!isDragging || !isDragable || NetworkGameManager.Instance.ownPlayer.network.GetEnableControl())
		{
			return;
		}
		Vector3 mousePosition = Input.mousePosition;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(imageItem.rectTransform.parent as RectTransform, mousePosition, null, out var localPoint);
		imageItem.rectTransform.localPosition = localPoint;
		if (ArmoryLobbyManager.Instance != null && !ArmoryLobbyManager.Instance.UIMenu.isHidden)
		{
			if (_itemSlotType == ItemSlotType.Inventory)
			{
				UIGameManager.Instance.uiInGame.canvas.overrideSorting = true;
				UIGameManager.Instance.uiInGame.canvas.sortingOrder = 2;
			}
			else
			{
				UIGameManager.Instance.uiInGame.canvas.sortingOrder = 0;
				UIGameManager.Instance.uiInGame.canvas.overrideSorting = false;
			}
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (isDragable && !NetworkGameManager.Instance.ownPlayer.network.GetEnableControl())
		{
			if (!Mouse.current.rightButton.IsPressed())
			{
				isDragging = true;
			}
			prevAnchorPos = imageItem.rectTransform.anchoredPosition;
		}
	}

	public int GetIdxArmory(PlayerController player, int idxSlot)
	{
		int num = -1;
		int result = -1;
		if (GameModes.Instance.isItemBoxGlobal)
		{
			for (int i = 0; i < ItemBoxNetwork.instance.arrItem.Length; i++)
			{
				if (ArmoryLobbyManager.Instance.TabItem == "All" || ItemBoxNetwork.instance.GetItemType(ItemBoxNetwork.instance.arrItem.Get(i).ID) == ArmoryLobbyManager.Instance.TabItem)
				{
					num++;
					if (num == idxSlot)
					{
						result = i;
						break;
					}
				}
			}
		}
		else
		{
			for (int j = 0; j < player.ItemBoxController.arrItem.Count; j++)
			{
				if (ArmoryLobbyManager.Instance.TabItem == "All" || player.ItemBoxController.arrItem[j].ItemType == ArmoryLobbyManager.Instance.TabItem)
				{
					num++;
					if (num == idxSlot)
					{
						result = j;
						break;
					}
				}
			}
		}
		return result;
	}

	public void Flashing()
	{
		flashImage.color = Color.white;
		flashImage.enabled = true;
		flashImage.DOKill();
		flashImage.DOColor(new Color(1f, 1f, 1f, 0.65f), 0.1f);
		flashImage.DOColor(new Color(1f, 1f, 1f, 0f), 0.3f).SetDelay(0.15f).OnComplete(() =>
		{
			flashImage.enabled = false;
		});
	}

	public void SetGreyOutSlot(bool active)
	{
		if (!imageItem)
		{
			return;
		}
		if (imageItem.sprite != null)
		{
			if (active)
			{
				imageItem.DOFade(0.1f, 0f);
			}
			else
			{
				imageItem.DOFade(1f, 0f);
			}
		}
		else
		{
			imageItem.DOFade(0f, 0f);
		}
	}

	public void SetDraggable(bool newValue)
	{
		isDragable = newValue;
	}
}

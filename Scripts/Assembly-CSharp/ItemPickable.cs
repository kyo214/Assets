using System;
using System.Collections.Generic;
using DG.Tweening;
using I2.Loc;
using TMPro;
using Toked;
using Toked.Crafting;
using Toked.Inventory;
using Toked.Item;
using UnityEngine;
using _Modules.Item.Scripts;

public class ItemPickable : MonoBehaviour
{
	public int itemID;

	public int uniqueID;

	public int networkID;

	public int clueID;

	public string itemName;

	public string itemType;

	public bool destroyOnPick;

	public byte amount;

	public short ammo;

	public int durability = -1;

	public Collider itemCollider;

	public Rigidbody rigidbody;

	public bool isClue;

	public Transform labelPos;

	public string labelCommand;

	public List<string> notes = new List<string>();

	public float imageAlpha = 0.027f;

	public float imgScale = 1f;

	public bool showTitle;

	public Sprite noteSprite;

	public Sprite noteGamepadSprite;

	public bool is3D;

	public SpriteRenderer itemSprite;

	public SpriteRenderer effectSprite;

	public SpriteRenderer objectSprite;

	public Sprite assetSprite45;

	public Sprite assetSprite135;

	public int directionSprite = 225;

	public float initScaleX;

	public SpriteRenderer Outline;

	public string pickupSFX;

	public string VOMale;

	public string VOFemale;

	public RoomCollider roomCollider;

	public SpriteRenderer itemMap;

	public GameObject GameObjectMap;

	[SerializeField]
	private ItemInteractableCustomFunction _itemInteractableCustomFunction;

	[SerializeField]
	public ItemInteractableCustomFunction OnRemoveObjectCustomFunction;

	[SerializeField]
	private IconItemType _iconItemType;

	public Vector3 InitParentPosition;

	public bool DropToInitPos;

	private static readonly int Brightness = Shader.PropertyToID("_Brightness");

	private static readonly string BrightnessPropertyName = "_Brightness";

	private Tweener _flashTween;

	private Material _itemMaterial;

	public bool IsCursedItem;

	public bool IsSpawnedFromObject;

	[SerializeField]
	private ItemIntractableStatusEffect _itemIntractableStatusEffect;

	public ItemIntractableStatusEffect ItemIntractableStatusEffect
	{
		get
		{
			return _itemIntractableStatusEffect;
		}
		set
		{
			_itemIntractableStatusEffect = value;
		}
	}

	public event Action<bool> OnSetItemVisibility;

	private void Awake()
	{
		SetSpriteEnable(value: false);
		InitParentPosition = base.transform.parent.position;
		if ((bool)itemSprite)
		{
			_itemMaterial = itemSprite.material;
			_flashTween = _itemMaterial.DOFloat(0f, BrightnessPropertyName, 0.3f).SetAutoKill(autoKillOnCompletion: false).Pause();
		}
	}

	private void Start()
	{
		if (LobbyManager.Instance != null && itemCollider.enabled)
		{
			SetSpriteEnable(value: true);
		}
		if (itemType != "Note")
		{
			if (itemID >= 400)
			{
				string text = itemID.ToString();
				itemType = "Material";
				itemName = DataManager.Instance.GetValueDatabase<string>("Item", text, "Name");
				CraftMaterialScriptableObject craftMaterialScriptableObject = DataManager.Instance.Get<CraftMaterialLibraryScriptableObject>().FindByItemIdKey(text);
				Vector2 vector = (craftMaterialScriptableObject ? craftMaterialScriptableObject.MinMaxDropAmount : new Vector2(1f, 1f));
				amount = (byte)UnityEngine.Random.Range(vector.x, vector.y);
			}
			else
			{
				if (itemID >= 300)
				{
					itemType = "Item";
				}
				else if (itemID >= 200)
				{
					itemType = "HealingItem";
				}
				else if (itemID >= 100)
				{
					itemType = "Ammunition";
					if (amount == 0)
					{
						amount = (byte)DataManager.Instance.GetValueDatabase(itemType, itemID.ToString(), "Amount");
					}
				}
				else
				{
					itemType = "Weapon";
				}
				itemName = DataManager.Instance.GetValueDatabase(itemType, itemID, "Name");
			}
			durability = -1;
			BGDatabase_Item entityByKeyid = BGDatabase_Item.GetEntityByKeyid(itemID);
			if (entityByKeyid != null)
			{
				_ = entityByKeyid.Durability;
				_ = 0;
			}
		}
		if (uniqueID == -1)
		{
			uniqueID = 0;
			GameManager.Instance.arrItemPickable.Sort((ItemPickable p1, ItemPickable p2) => p1.uniqueID.CompareTo(p2.uniqueID));
			if (GameManager.Instance.arrItemPickable.Count > 0)
			{
				List<ItemPickable> arrItemPickable = GameManager.Instance.arrItemPickable;
				uniqueID = arrItemPickable[arrItemPickable.Count - 1].uniqueID + 1;
			}
			if (!GameManager.Instance.arrItemPickable.Contains(this))
			{
				GameManager.Instance.arrItemPickable.Add(this);
				GameManager.Instance.arrItemPickable.Sort((ItemPickable p1, ItemPickable p2) => p1.uniqueID.CompareTo(p2.uniqueID));
			}
		}
		else if (!GameManager.Instance.arrItemPickable.Contains(this))
		{
			GameManager.Instance.arrItemPickable.Add(this);
			GameManager.Instance.arrItemPickable.Sort((ItemPickable p1, ItemPickable p2) => p1.uniqueID.CompareTo(p2.uniqueID));
		}
		if (!is3D)
		{
			GameManager.Instance.arrSpriteItemPickable.Add(base.transform.parent.GetChild(0));
		}
		foreach (RoomCollider item in GameManager.Instance.arrRoom)
		{
			BoxCollider[] componentsInChildren = item.GetComponentsInChildren<BoxCollider>();
			for (int num = 0; num < componentsInChildren.Length; num++)
			{
				Bounds bounds = componentsInChildren[num].bounds;
				bounds.Expand(1.5f);
				if (bounds.Contains(base.transform.position))
				{
					item.itemList.Add(this);
					roomCollider = item;
				}
			}
		}
		if (rigidbody != null)
		{
			rigidbody.mass = 20f;
		}
		if (objectSprite != null)
		{
			initScaleX = Mathf.Abs(objectSprite.transform.localScale.x);
			if ((bool)Outline)
			{
				Outline.sprite = objectSprite.sprite;
			}
		}
		InvokeRepeating("LoopFlash", UnityEngine.Random.Range(1f, 4f), 3f);
		if (LobbyManager.Instance == null)
		{
			CheckModifierMap();
		}
	}

	private void CheckModifierMap()
	{
		if ((itemType == "HealingItem" && GlobalMissionManager.Instance.ModNoHealingItem.CurrentValue >= 1f) || (itemType == "Ammunition" && GlobalMissionManager.Instance.ModNoAmmoLoot.CurrentValue >= 1f))
		{
			if (roomCollider != null)
			{
				roomCollider.itemList.Remove(this);
			}
			SetDisableObject();
			if (!is3D)
			{
				GameManager.Instance.arrSpriteItemPickable.Remove(base.transform.parent.GetChild(0));
			}
			GameManager.Instance.arrItemPickable.Remove(this);
		}
	}

	private void OnDestroy()
	{
		if (!is3D)
		{
			GameManager.Instance.arrSpriteItemPickable.Remove(base.transform.parent.GetChild(0));
		}
		GameManager.Instance.arrItemPickable.Remove(this);
	}

	private void LoopFlash()
	{
		if (this != null && base.isActiveAndEnabled && itemSprite != null)
		{
			_itemMaterial.SetFloat(Brightness, 0.5f);
			_flashTween.Restart();
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (!other.CompareTag("Player"))
		{
			return;
		}
		PlayerController ownPlayer = NetworkGameManager.Instance.ownPlayer;
		if (ownPlayer != null && other.transform == ownPlayer.transform)
		{
			if (!ChatSystem.Instance.ItemCommand.activeSelf && !UIGameManager.Instance.isUIInvisible && (UIGameManager.Instance.UIMenuNote.isHidden || !UIGameManager.Instance.UIMenuNote.gameObject.activeSelf))
			{
				ChatSystem.Instance.ItemCommand.SetActive(value: true);
			}
			bool flag = true;
			if (ownPlayer.itemCollision != null)
			{
				flag = ((MathFunc.DistanceSameYPos(base.gameObject.transform.position, ownPlayer.transform.position) < MathFunc.DistanceSameYPos(ownPlayer.itemCollision.transform.position, ownPlayer.transform.position)) ? true : false);
			}
			if (ChatSystem.Instance.LabelTermItemCommand.GetMainTargetsText() == "")
			{
				SetLabelText(ownPlayer);
			}
			if ((ownPlayer.itemCollision != base.gameObject && !ownPlayer.fsmUpperBody.GetBool("isReviving")) & flag)
			{
				ChatSystem.Instance.SetIcon(_iconItemType);
				SetLabelText(ownPlayer);
				ownPlayer.itemCollision = base.gameObject;
				ownPlayer.itemCollisionCollider = itemCollider;
				ownPlayer.functionItemCollision = "PickUp";
			}
			if (CameraGame.Instance.mainCam != null && ownPlayer.itemCollision == base.gameObject)
			{
				Vector3 vector = CameraGame.Instance.mainCam.WorldToScreenPoint(labelPos.position);
				ChatSystem.Instance.ItemCommand.transform.position = new Vector3(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), vector.z);
			}
			if (itemID > 0 && !itemMap.enabled)
			{
				ownPlayer.network.ExecSyncItemMap((short)uniqueID);
			}
		}
		string GetItemLocalize()
		{
			string text = LocalizationManager.GetTranslation(itemType + "/" + itemType + itemID);
			if (string.IsNullOrEmpty(text))
			{
				text = itemName;
			}
			if ((bool)_itemIntractableStatusEffect)
			{
				string text2 = _itemIntractableStatusEffect?.GetGeneralEffectLocalization();
				if (!string.IsNullOrWhiteSpace(text2))
				{
					text2 += "<br>";
				}
				text = text2 + text;
			}
			return text;
		}
		void GetWeaponCurseLocalize()
		{
			if ((bool)_itemIntractableStatusEffect)
			{
				string text = ChatSystem.Instance.TextItemCommand.text;
				string text2 = _itemIntractableStatusEffect?.GetGeneralEffectLocalization();
				if (!string.IsNullOrWhiteSpace(text2))
				{
					text2 += "<br>";
				}
				text = text2 + text;
				ChatSystem.Instance.TextItemCommand.text = text;
			}
		}
		void SetLabelText(PlayerController playerController)
		{
			if (itemType == "Note")
			{
				ChatSystem.Instance.LabelTermItemCommand.SetTerm("Menu/" + itemName);
			}
			else if (itemType == "Ammunition")
			{
				ChatSystem.Instance.TextItemCommand.text = "<color=#ffec6c>" + GetItemLocalize() + " (" + amount + ")</color>";
			}
			else if (itemType == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(itemID).WeaponType == "Melee")
			{
				ChatSystem.Instance.TextItemCommand.text = GetItemLocalize();
			}
			else if (itemType == "Weapon" && BGDatabase_Weapon.GetEntityByKeyid(itemID).WeaponType == "Range")
			{
				string text = ammo.ToString();
				if (ammo < 0)
				{
					text = BGDatabase_Weapon.GetEntityByKeyid(itemID).MagazineSize.ToString();
				}
				ChatSystem.Instance.TextItemCommand.text = LocalizationManager.GetTranslation(itemType + "/" + itemType + DataManager.Instance.GetBaseWeapon(itemID));
				string attachedWeaponName = UIGameManager.Instance.GetAttachedWeaponName(itemID, isUsingParentheses: false);
				if (attachedWeaponName != "")
				{
					ChatSystem.Instance.TextItemCommand.text += attachedWeaponName;
				}
				GetWeaponCurseLocalize();
				TextMeshProUGUI textItemCommand = ChatSystem.Instance.TextItemCommand;
				textItemCommand.text = textItemCommand.text + " (" + text + ")";
			}
			else if (itemType != "Material")
			{
				ChatSystem.Instance.TextItemCommand.text = "<color=#ffec6c>" + GetItemLocalize() + "</color>";
			}
			else if (itemType == "Material")
			{
				ChatSystem.Instance.TextItemCommand.text = "<color=#45bbf2>" + GetItemLocalize() + " (" + (float)(int)amount * playerController.data.MaterialInventoryManager.BonusLootMaterialMultiply + ")</color>";
			}
			else
			{
				ChatSystem.Instance.TextItemCommand.text = GetItemLocalize() + " (" + amount + ")";
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player") && other.transform == NetworkGameManager.Instance.ownPlayer.transform)
		{
			NetworkGameManager.Instance.ownPlayer.itemCollision = null;
			NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
			NetworkGameManager.Instance.ownPlayer.functionItemCollision = "";
			ChatSystem.Instance.ItemCommand.SetActive(value: false);
		}
	}

	public void ShowNote()
	{
		AudioManager.PlaySFX("examine-corpse");
		UIGameManager.Instance.pageNote = 0;
		ItemPickable component = NetworkGameManager.Instance.ownPlayer.itemCollision.GetComponent<ItemPickable>();
		string strNote = "";
		if (component.notes.Count > 0)
		{
			strNote = component.notes[0];
		}
		if (showTitle && LocalizationManager.GetTranslation("Menu/" + component.itemName) != null)
		{
			UIGameManager.Instance.txtTitleNote.text = LocalizationManager.GetTranslation("Menu/" + component.itemName).ToUpper();
		}
		else
		{
			UIGameManager.Instance.txtTitleNote.text = "";
		}
		UIGameManager.Instance.notes.Clear();
		int num = 0;
		foreach (string note in component.notes)
		{
			UIGameManager.Instance.notes.Add(note);
			num++;
		}
		if (num > 1)
		{
			UIGameManager.Instance.btnPrevNote.enabled = false;
			UIGameManager.Instance.btnNextNote.enabled = true;
		}
		else
		{
			UIGameManager.Instance.btnPrevNote.enabled = false;
			UIGameManager.Instance.btnNextNote.enabled = false;
		}
		NetworkGameManager.Instance.ownPlayer.itemCollision = null;
		NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
		NetworkGameManager.Instance.ownPlayer.functionItemCollision = "";
		ChatSystem.Instance.ItemCommand.SetActive(value: false);
		UIGameManager.Instance.mapUI.SetActive(value: false);
		UIGameManager.Instance.uiTabKill.InstantHide();
		UIGameManager.Instance.uiTabKill.gameObject.SetActive(value: false);
		if (!UIGameManager.Instance.isUIInvisible)
		{
			UIGameManager.Instance.uiObjective.SetActive(value: false);
		}
		UIGameManager.Instance.UIMenuNote.Show();
		UIGameManager.Instance.txtNote.richText = false;
		UIGameManager.Instance.txtNote.text = UIGameManager.Instance.ConvertNote(strNote);
		UIGameManager.Instance.txtNote.richText = true;
		UIGameManager.Instance.imgNote.CrossFadeAlpha(imageAlpha, 0f, ignoreTimeScale: true);
		if (!GlobalOptionsManager.Instance.usingGamepad || noteGamepadSprite == null)
		{
			UIGameManager.Instance.imgNote.sprite = noteSprite;
		}
		else
		{
			UIGameManager.Instance.imgNote.sprite = noteGamepadSprite;
		}
		UIGameManager.Instance.imgNote.rectTransform.DOScale(imgScale, 0f);
		NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: false);
		NetworkGameManager.Instance.ownPlayer.direction = Vector3.zero;
		UIGameManager.Instance.uiInGame.Hide();
	}

	public void SetSpriteEnable(bool value, bool isFading = false)
	{
		if (isFading)
		{
			if (itemSprite != null)
			{
				itemSprite.enabled = true;
				if (!value)
				{
					itemSprite.color = new Color(itemSprite.color.r, itemSprite.color.g, itemSprite.color.b, 1f);
					itemSprite.DOFade(0f, 1f).OnComplete(() =>
					{
						itemSprite.enabled = false;
						itemSprite.color = new Color(itemSprite.color.r, itemSprite.color.g, itemSprite.color.b, 1f);
					});
				}
				else
				{
					itemSprite.color = new Color(itemSprite.color.r, itemSprite.color.g, itemSprite.color.b, 0f);
					itemSprite.DOFade(1f, 1f);
				}
			}
		}
		else if (itemSprite != null)
		{
			itemSprite.color = new Color(itemSprite.color.r, itemSprite.color.g, itemSprite.color.b, 1f);
			itemSprite.enabled = value;
		}
		if (effectSprite != null)
		{
			effectSprite.gameObject.SetActive(value);
		}
		OnSetItemVisibility?.Invoke(value);
	}

	public void PickObject(PlayerController playerController)
	{
		if (itemType == "Material")
		{
			if ((bool)GameManagerPhoton.Instance && (bool)GameManagerPhoton.Instance.CurrentMission)
			{
				if (GameManagerPhoton.Instance.CurrentMission.pickupSharedMaterial)
				{
					GameManagerPhoton.Instance.RpcAddMaterialToAllPlayer(itemID, amount);
				}
				else
				{
					playerController.data.MaterialInventoryManager.AddMaterial(MaterialInventoryManager.InventoryType.Auto, itemID, amount);
				}
			}
			else
			{
				playerController.data.MaterialInventoryManager.AddMaterial(MaterialInventoryManager.InventoryType.Auto, itemID, amount);
			}
			OnPickObject();
			if (destroyOnPick)
			{
				playerController.network.ExecRemoveObject(uniqueID);
			}
			if (NetworkGameManager.Instance.isServer)
			{
				playerController.network.ShowBaloonChat(ChatType.GOT_ITEM, itemID, -1, -1, -1, 10);
			}
		}
		else
		{
			int num = -1;
			if (itemType == "Item" && !BGDatabase_Item.GetEntityByKeyid(itemID).IsNotKeyItem)
			{
				num = uniqueID;
			}
			short itemValueOrAmmo = ammo;
			if (durability > 0)
			{
				itemValueOrAmmo = (short)durability;
			}
			int num2 = playerController.data.AddInventory(itemID, isOnPick: true, amount, itemValueOrAmmo, init: false, isCombine: false, canStacking: true, num);
			if (num2 != -1)
			{
				OnPickObject(playerController.data.arrInventory[num2]);
				if (IsCursedItem)
				{
					playerController.network.ShowBaloonChat(ChatType.GOT_ITEM, itemID, -1, -1, uniqueID, 10);
				}
				else
				{
					playerController.network.ShowBaloonChat(ChatType.GOT_ITEM, itemID, -1, -1, -1, 10);
				}
				if (destroyOnPick)
				{
					playerController.network.ExecRemoveObject(uniqueID);
				}
			}
			else
			{
				playerController.network.ShowBaloonChat(ChatType.ONVENTORY_FULL, -1, -1, -1, -1, 10);
			}
		}
		playerController.itemCollision = null;
		playerController.itemCollisionCollider = null;
		void OnPickObject(InventoryObject itemObject = null)
		{
			if (roomCollider != null)
			{
				roomCollider.itemList.Remove(this);
			}
			if (playerController.network.isLocalPlayer)
			{
				if (!GameManager.Instance.isHordeMode && GlobalOptionsManager.Instance.enableVOItem)
				{
					if (VOMale != "" && playerController.IsMale)
					{
						AudioManager.PlaySFX(VOMale);
					}
					else if (VOFemale != "")
					{
						AudioManager.PlaySFX(VOFemale);
					}
				}
				if (pickupSFX != "")
				{
					AudioManager.PlaySFXTransform(pickupSFX, base.transform, isLocalPlayerTrigger: false);
				}
				else if (itemType == "Ammunition")
				{
					AudioManager.PlaySFX("ammo-pickup");
				}
				else if (itemType == "Weapon")
				{
					AudioManager.PlaySFX("pickup-heavy-guns");
				}
				else
				{
					AudioManager.PlaySFX("herb_pickup");
				}
			}
			if (!NetworkGameManager.Instance.isServer && destroyOnPick)
			{
				SetDisableObject();
				ChatSystem.Instance.ItemCommand.SetActive(value: false);
			}
			_itemIntractableStatusEffect?.Execute(playerController, itemObject);
			_itemInteractableCustomFunction?.Execute(playerController);
			OnRemoveObjectCustomFunction?.Execute(playerController);
		}
	}

	public void SetDisableObject(bool isFading = false)
	{
		DOTween.Kill("FadeItem" + uniqueID);
		itemCollider.enabled = false;
		SetSpriteEnable(value: false, isFading);
		itemMap.enabled = false;
		if ((bool)GameObjectMap)
		{
			GameObjectMap.SetActive(value: false);
		}
		if ((bool)NetworkGameManager.Instance.ownPlayer && NetworkGameManager.Instance.ownPlayer.itemCollision == base.gameObject)
		{
			NetworkGameManager.Instance.ownPlayer.itemCollision = null;
			NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
		}
	}
}

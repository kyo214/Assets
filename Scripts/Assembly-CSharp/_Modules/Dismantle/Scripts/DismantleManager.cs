using System;
using Toked.Crafting;
using UnityEngine;
using UnityEngine.UI;
using _Modules.UIResult.Scripts;

namespace _Modules.Dismantle.Scripts;

public class DismantleManager : MonoBehaviour
{
	[SerializeField]
	private Image _dismantleItemUi;

	[SerializeField]
	private Button _dismantleButton;

	[SerializeField]
	private Button _cancelButton;

	[SerializeField]
	private UIMaterialResultPanel _materialPanelUi;

	private InventoryObject _currentInventoryObject;

	private PlayerController _playerController;

	private bool _initEvents;

	public PlayerController PlayerController
	{
		get
		{
			if (_playerController == null)
			{
				_playerController = NetworkGameManager.Instance?.ownPlayer;
			}
			return _playerController;
		}
	}

	public bool Initialized => _initEvents;

	private void Start()
	{
		_dismantleButton.onClick.AddListener(OnDisMantleButtonAction);
		_cancelButton.onClick.AddListener(OnCancelButtonAction);
		ResetUI();
	}

	private void OnEnable()
	{
		InitEvents();
	}

	private void OnDisable()
	{
		ResetUI();
		RemoveEvents();
	}

	private void InitEvents()
	{
		if (!_initEvents && PlayerController != null && (bool)PlayerController.inventoryManager)
		{
			DismantleSelectButtonEvent dismantleSelectButtonEvent = PlayerController.inventoryManager.DismantleSelectButtonEvent;
			dismantleSelectButtonEvent.OnSelectDismantleItem = (Action<InventoryObject>)Delegate.Combine(dismantleSelectButtonEvent.OnSelectDismantleItem, new Action<InventoryObject>(OnSelectDismantleItem));
			DismantleSelectButtonEvent dismantleSelectButtonEvent2 = PlayerController.inventoryManager.DismantleSelectButtonEvent;
			dismantleSelectButtonEvent2.OnDeselectDismantleItem = (Action)Delegate.Combine(dismantleSelectButtonEvent2.OnDeselectDismantleItem, new Action(OnDeselectDismantleItem));
			InventoryManager inventoryManager = PlayerController.inventoryManager;
			inventoryManager.OnDismantleButtonEvent = (Action<InventoryObject>)Delegate.Combine(inventoryManager.OnDismantleButtonEvent, new Action<InventoryObject>(OnDisMantleItemAction));
			inventoryManager.OnInventorySlotChangedEvent = (Action)Delegate.Combine(inventoryManager.OnInventorySlotChangedEvent, new Action(OnInventorySlotChangedAction));
			inventoryManager.SetCanDismantle(canDismantle: true);
			inventoryManager.SetGrayOutSlotDismantle(active: true);
			_initEvents = true;
		}
	}

	private void RemoveEvents()
	{
		if (_initEvents && PlayerController != null)
		{
			InventoryManager inventoryManager = PlayerController.inventoryManager;
			DismantleSelectButtonEvent dismantleSelectButtonEvent = PlayerController.inventoryManager.DismantleSelectButtonEvent;
			dismantleSelectButtonEvent.OnSelectDismantleItem = (Action<InventoryObject>)Delegate.Remove(dismantleSelectButtonEvent.OnSelectDismantleItem, new Action<InventoryObject>(OnSelectDismantleItem));
			DismantleSelectButtonEvent dismantleSelectButtonEvent2 = PlayerController.inventoryManager.DismantleSelectButtonEvent;
			dismantleSelectButtonEvent2.OnDeselectDismantleItem = (Action)Delegate.Remove(dismantleSelectButtonEvent2.OnDeselectDismantleItem, new Action(OnDeselectDismantleItem));
			inventoryManager.OnDismantleButtonEvent = (Action<InventoryObject>)Delegate.Remove(inventoryManager.OnDismantleButtonEvent, new Action<InventoryObject>(OnDisMantleItemAction));
			inventoryManager.OnInventorySlotChangedEvent = (Action)Delegate.Remove(inventoryManager.OnInventorySlotChangedEvent, new Action(OnInventorySlotChangedAction));
			inventoryManager.SetCanDismantle(canDismantle: false);
			inventoryManager.SetGrayOutSlotDismantle(active: false);
			_initEvents = false;
		}
	}

	public void Init(InventoryObject inventoryObject, bool saveCurrentInventory = true)
	{
		if (inventoryObject == null)
		{
			return;
		}
		ItemToCraftMaterialConverter.ConvertMaterialItemData convertMaterialItemData = ItemToCraftMaterialConverter.DismantleItemToCraftMaterial(inventoryObject);
		if (convertMaterialItemData != null)
		{
			if (saveCurrentInventory)
			{
				_currentInventoryObject = inventoryObject;
			}
			SetImage(DataManager.Instance.GetItemSprite(inventoryObject.ID.ToString()));
			_materialPanelUi.Set(convertMaterialItemData.Material);
			SetActiveDismantleButtonOptions(active: true);
		}
	}

	public void Init(int index)
	{
		Init(PlayerController?.data.arrInventory[index]);
	}

	public void SetNavigation(Selectable onRightButton = null)
	{
		Navigation navigation = _dismantleButton.navigation;
		navigation.selectOnRight = onRightButton;
		_dismantleButton.navigation = navigation;
		Navigation navigation2 = _cancelButton.navigation;
		navigation2.selectOnRight = onRightButton;
		_cancelButton.navigation = navigation2;
	}

	public void SetImage(Sprite sprite)
	{
		_dismantleItemUi.gameObject.SetActive(sprite != null);
		_dismantleItemUi.sprite = sprite;
	}

	public void ResetUI()
	{
		_currentInventoryObject = null;
		_dismantleItemUi.gameObject.SetActive(value: false);
		_dismantleItemUi.sprite = null;
		_materialPanelUi.Init();
		RefreshButtonDismantleOptions();
	}

	public void RefreshUI()
	{
		if (_currentInventoryObject != null)
		{
			Init(_currentInventoryObject);
		}
		else
		{
			ResetUI();
		}
	}

	public void OnShowDismantleUI()
	{
		InitEvents();
		ResetUI();
	}

	public void OnHideDismantleUI()
	{
		RemoveEvents();
	}

	public void RefreshButtonDismantleOptions()
	{
		bool activeDismantleButtonOptions = _currentInventoryObject != null;
		SetActiveDismantleButtonOptions(activeDismantleButtonOptions);
	}

	public void SetActiveDismantleButtonOptions(bool active)
	{
		_dismantleButton.gameObject.SetActive(active);
		_cancelButton.gameObject.SetActive(active);
	}

	public Button GetButtonDismantle()
	{
		return _dismantleButton;
	}

	private void OnDisMantleItemAction(InventoryObject inventoryObject)
	{
		Init(inventoryObject);
		_dismantleButton.Select();
	}

	private void OnInventorySlotChangedAction()
	{
		RefreshInventorySlot();
	}

	private void RefreshInventorySlot()
	{
		PlayerController?.inventoryManager.SetGrayOutSlotDismantle(active: true);
	}

	private void OnDisMantleButtonAction()
	{
		if (_currentInventoryObject != null)
		{
			PlayerController?.inventoryManager.ItemDismantle(_currentInventoryObject.IdxInventory);
			ResetUI();
			RefreshInventorySlot();
			PlayerController?.inventoryManager.SelectButton(2);
		}
	}

	private void OnCancelButtonAction()
	{
		ResetUI();
		RefreshInventorySlot();
		PlayerController?.inventoryManager.SelectButton(2);
	}

	private void OnSelectDismantleItem(InventoryObject inventoryObject)
	{
		Init(inventoryObject, saveCurrentInventory: false);
	}

	private void OnDeselectDismantleItem()
	{
		RefreshUI();
	}
}

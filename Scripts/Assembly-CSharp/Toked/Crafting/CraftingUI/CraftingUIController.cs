using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Doozy.Runtime.UIManager.Containers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Toked.Crafting.CraftingUI;

public class CraftingUIController : MonoBehaviour
{
	[SerializeField]
	private CraftingManager _craftingManager;

	[SerializeField]
	private UIView _view;

	[SerializeField]
	private CraftingDescriptionsUI _craftingDescriptionsUI;

	[SerializeField]
	private CraftingTabGroupUIController _craftingTabGroupUIController;

	[SerializeField]
	private CraftingSkillUIController _craftingSkillUIController;

	[SerializeField]
	private InventoryManager _inventoryManager;

	private bool _initialize;

	private bool _initInventoryEvent;

	private PlayerInputActions _playerInputActions;

	public CraftingDescriptionsUI CraftingDescriptionsUI => _craftingDescriptionsUI;

	public void Start()
	{
		_view.OnShowCallback.Event.AddListener(Init);
		_view.OnHideCallback.Event.AddListener(Hide);
	}

	public void Init()
	{
		NetworkGameManager.Instance.ownPlayer?.inventoryManager.AdditionalKeyBtnObject?.SetActive(value: false);
		if (_craftingManager.SkillLearnPopupUI.IsShow)
		{
			UIGameManager.Instance.ShowInventoryWhenCraft();
			_craftingManager.SkillLearnPopupUI.SelectedFirstButton();
		}
		else
		{
			InitData();
			_inventoryManager.SetNavigationCrafting();
			_craftingTabGroupUIController.SetCurrentActiveTab();
			UIGameManager.Instance.ShowInventoryWhenCraft();
			_craftingSkillUIController.Init();
			InputManager.ToggleActionMap(InputManager.PlayerInputToggleAction.DISABLE_PLAYER_INPUT, InputManager.inputActions.UI, InputManager.inputActions.InventoryUI);
		}
		Debug.Log("Open Crafting");
	}

	private void InitData()
	{
		if (!_initialize)
		{
			if ((object)_inventoryManager == null)
			{
				_inventoryManager = GameManager.Instance.arrInventoryManager[0];
			}
			foreach (CraftRecipeScriptableObject data in _craftingManager.CraftingRecipesLibrarySo.DataList)
			{
				CraftingTabUIControllerBase tab = _craftingTabGroupUIController.GetTab(data.RecipeCategory);
				if ((bool)tab)
				{
					AddButton(tab, data);
				}
			}
			_craftingTabGroupUIController.SetNavigation(_inventoryManager.buttonInventory[2]);
			_initialize = true;
		}
		else
		{
			RefreshButtonData();
		}
	}

	private void Hide()
	{
		if (_initialize)
		{
			NetworkGameManager.Instance.ownPlayer?.inventoryManager.AdditionalKeyBtnObject?.SetActive(value: true);
			InputManager.ToggleActionMap(InputManager.PlayerInputToggleAction.ENABLE_PLAYER_INPUT, InputManager.inputActions.UI, InputManager.inputActions.Player);
			UIGameManager.Instance.HideInventoryWhenCraft();
			_inventoryManager.SetNavigationInventory();
		}
		NetworkGameManager.Instance.ownPlayer?.UpdatePlayerStats();
	}

	private void OnEnable()
	{
		InitInputBinding();
		InventoryManager inventoryManager = _inventoryManager;
		inventoryManager.OnDropItem = (Action<int>)Delegate.Combine(inventoryManager.OnDropItem, new Action<int>(OnDropInventoryItem));
	}

	private void OnDisable()
	{
		RemoveInputBinding();
		InventoryManager inventoryManager = _inventoryManager;
		inventoryManager.OnDropItem = (Action<int>)Delegate.Remove(inventoryManager.OnDropItem, new Action<int>(OnDropInventoryItem));
	}

	public void RefreshButtonData()
	{
		_craftingTabGroupUIController.RefreshButtonData();
	}

	private void OnButtonClick(CraftRecipeScriptableObject craftRecipeScriptableObject)
	{
		GameObject lastSelectedGameObject = EventSystem.current.currentSelectedGameObject;
		_craftingManager.CraftRecipe(craftRecipeScriptableObject);
		RefreshButtonData();
		UniTaskUtil.DelayedCall(this, 0.1f, () =>
		{
			if ((bool)lastSelectedGameObject && !lastSelectedGameObject.activeSelf)
			{
				Selectable selectable = lastSelectedGameObject.GetComponent<CraftingItemButtonUI>()?.Button.navigation.selectOnDown;
				while (selectable == null && (!selectable || !selectable.gameObject.activeSelf))
				{
					selectable = selectable?.navigation.selectOnDown;
				}
				selectable?.Select();
			}
		}).Forget();
		AudioManager.PlaySFX("inventory-craft-success");
	}

	private void OnHover(CraftRecipeScriptableObject craftRecipeSo, List<bool> hoverState)
	{
		_craftingTabGroupUIController.SetHoverMaterial(craftRecipeSo.CraftingIngredientsList, hoverState);
		_craftingDescriptionsUI.Set(craftRecipeSo);
	}

	private void OnUnHover(CraftRecipeScriptableObject craftRecipeSo)
	{
		_craftingTabGroupUIController.SetUnHoverMaterial();
		_craftingDescriptionsUI.Reset();
	}

	private void AddButton(CraftingTabUIControllerBase craftingTabUIController, CraftRecipeScriptableObject craftRecipeSo)
	{
		craftingTabUIController.AddContent(_craftingManager, craftRecipeSo, OnButtonClick, OnHover, OnUnHover);
	}

	private void OnDropInventoryItem(int itemId)
	{
		RefreshButtonData();
	}

	private void InitInputBinding()
	{
		_playerInputActions = InputManager.inputActions;
		_playerInputActions.InventoryUI.LeftTab.performed += OnInputLeftTab;
		_playerInputActions.InventoryUI.RightTab.performed += OnInputRightTab;
		_playerInputActions.InventoryUI.SkillDescription.performed += OnInputSkillSetsDescription;
		_playerInputActions.InventoryUI.Back.performed += OnBackButton;
		GlobalOptionsManager.OnDeviceChangedEvent += OnDeviceChanged;
	}

	private void RemoveInputBinding()
	{
		_playerInputActions.InventoryUI.LeftTab.performed -= OnInputLeftTab;
		_playerInputActions.InventoryUI.RightTab.performed -= OnInputRightTab;
		_playerInputActions.InventoryUI.SkillDescription.performed -= OnInputSkillSetsDescription;
		_playerInputActions.InventoryUI.Back.performed -= OnBackButton;
		GlobalOptionsManager.OnDeviceChangedEvent -= OnDeviceChanged;
	}

	private void OnInputRightTab(InputAction.CallbackContext obj)
	{
		_craftingTabGroupUIController.NextTab();
	}

	private void OnInputLeftTab(InputAction.CallbackContext obj)
	{
		_craftingTabGroupUIController.PreviousTab();
	}

	private void OnBackButton(InputAction.CallbackContext obj)
	{
	}

	private void OnInputSkillSetsDescription(InputAction.CallbackContext obj)
	{
		_craftingSkillUIController.SetFirstButtonSelected();
	}

	private void OnDeviceChanged(GlobalOptionsManager globalOptionsManager)
	{
		if (!_view.isHidden)
		{
			if (globalOptionsManager.usingGamepad)
			{
				_craftingTabGroupUIController.SelectFirstButton();
			}
			else
			{
				EventSystem.current.SetSelectedGameObject(null);
			}
		}
	}
}

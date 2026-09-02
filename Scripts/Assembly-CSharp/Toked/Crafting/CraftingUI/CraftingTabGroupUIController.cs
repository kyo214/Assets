using System.Collections;
using System.Collections.Generic;
using Doozy.Runtime.UIManager.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Toked.Crafting.CraftingUI;

public class CraftingTabGroupUIController : MonoBehaviour
{
	[SerializeField]
	protected CraftingManager _craftingManager;

	protected CraftingUIController _craftingUiController;

	[SerializeField]
	private UIToggleGroup _uiToggleGroup;

	[SerializeField]
	private CraftingTabUIControllerBase[] _craftingTabUIControllerList;

	private int _currentSelected;

	private int _tabCount;

	private bool _init;

	private Button _inventoryFirstButton;

	private Coroutine _selectFirstButtonTabCoroutine;

	private Coroutine _onSelectedCoroutine;

	public CraftingUIController CraftingUIController => _craftingUiController ?? (_craftingUiController = _craftingManager?.GetComponent<CraftingUIController>());

	public UIToggleGroup UiToggleGroup => _uiToggleGroup;

	private void Start()
	{
		_uiToggleGroup.onSelectedEvent.AddListener(OnSelectedTabGroup);
	}

	public CraftingTabUIControllerBase GetCurrentTab()
	{
		return _craftingTabUIControllerList[_currentSelected];
	}

	public CraftingTabUIControllerBase GetTab(CraftRecipeScriptableObject.RecipeCategoryType recipeCategoryType)
	{
		CraftingTabUIControllerBase[] craftingTabUIControllerList = _craftingTabUIControllerList;
		foreach (CraftingTabUIControllerBase craftingTabUIControllerBase in craftingTabUIControllerList)
		{
			if (craftingTabUIControllerBase.CategoryType == recipeCategoryType)
			{
				return craftingTabUIControllerBase;
			}
		}
		return _craftingTabUIControllerList[0];
	}

	public void NextTab()
	{
		CheckInit();
		_currentSelected++;
		if (_currentSelected >= _tabCount)
		{
			_currentSelected = 0;
		}
		SetActiveTab(_currentSelected);
	}

	public void PreviousTab()
	{
		CheckInit();
		_currentSelected--;
		if (_currentSelected < 0)
		{
			_currentSelected = _tabCount - 1;
		}
		SetActiveTab(_currentSelected);
	}

	public void SetCurrentActiveTab()
	{
		CheckInit();
		InitMaterialUi();
		SetActiveTab(_currentSelected);
	}

	private void SetActiveTab(int index)
	{
		_currentSelected = index;
		_uiToggleGroup.toggles[index].SetIsOn(newValue: true);
		SelectFirstButtonTab(index);
		RefreshTab(index);
		CraftingUIController?.CraftingDescriptionsUI?.Reset();
	}

	public void SelectFirstButton()
	{
		if (GlobalOptionsManager.Instance.usingGamepad)
		{
			GetTabUIControllerList(_currentSelected)?.SelectFirstButton();
		}
	}

	private void SelectFirstButtonTab(int tabIndex)
	{
		if (GlobalOptionsManager.Instance.usingGamepad)
		{
			if (_selectFirstButtonTabCoroutine != null)
			{
				StopCoroutine(_selectFirstButtonTabCoroutine);
				_selectFirstButtonTabCoroutine = null;
			}
			_selectFirstButtonTabCoroutine = StartCoroutine(SelectFirstButtonTabCoroutine(tabIndex));
		}
	}

	private IEnumerator SelectFirstButtonTabCoroutine(int tabIndex)
	{
		yield return new WaitForSeconds(0.35f);
		GetTabUIControllerList(tabIndex)?.SelectFirstButton();
	}

	private void CheckInit()
	{
		if (!_init)
		{
			_currentSelected = 0;
			_tabCount = _uiToggleGroup.toggles.Count;
			_init = true;
		}
	}

	public void RefreshButtonData()
	{
		CraftingTabUIControllerBase[] craftingTabUIControllerList = _craftingTabUIControllerList;
		for (int i = 0; i < craftingTabUIControllerList.Length; i++)
		{
			craftingTabUIControllerList[i].RefreshButtonData();
		}
		SetNavigation();
	}

	public void SetNavigation(Button onRightButton = null)
	{
		_inventoryFirstButton = onRightButton;
		CraftingTabUIControllerBase[] craftingTabUIControllerList = _craftingTabUIControllerList;
		for (int i = 0; i < craftingTabUIControllerList.Length; i++)
		{
			craftingTabUIControllerList[i].SetNavigation(onRightButton);
		}
	}

	public void SetNavigation()
	{
		SetNavigation(_inventoryFirstButton);
	}

	private void OnSelectedTabGroup()
	{
		if (_onSelectedCoroutine != null)
		{
			StopCoroutine(_onSelectedCoroutine);
		}
		_onSelectedCoroutine = StartCoroutine(OnSelectedTabGroupCoroutine());
	}

	private IEnumerator OnSelectedTabGroupCoroutine()
	{
		yield return new WaitUntil(() => _uiToggleGroup.isSelected);
		GetTabUIControllerList(_currentSelected)?.SelectFirstButton();
	}

	public void SetHoverMaterial(List<CraftingIngredient> craftingIngredientList, List<bool> hoverState)
	{
		GetTabUIControllerList(_currentSelected)?.SetHoverMaterial(craftingIngredientList, hoverState);
	}

	public void SetUnHoverMaterial()
	{
		GetTabUIControllerList(_currentSelected)?.SetUnHoverMaterial();
	}

	public void InitMaterialUi()
	{
		for (int i = 0; i < _craftingTabUIControllerList.Length; i++)
		{
			_craftingTabUIControllerList[i].InitCraftingMaterial();
		}
	}

	public void RefreshTab(int index)
	{
		CraftingTabUIControllerBase tabUIControllerList = GetTabUIControllerList(index);
		if ((bool)tabUIControllerList)
		{
			tabUIControllerList.Init();
			tabUIControllerList.RefreshButtonData();
		}
	}

	public void SetCurrentSelected(int currentSelected)
	{
		_currentSelected = currentSelected;
		GetTabUIControllerList(_currentSelected)?.RefreshButtonData();
	}

	private CraftingTabUIControllerBase GetTabUIControllerList(int index)
	{
		if (index >= _craftingTabUIControllerList.Length)
		{
			return null;
		}
		return _craftingTabUIControllerList[index];
	}
}

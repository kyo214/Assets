using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Toked.Crafting.CraftingUI;

public class CraftingTabUIControllerBase : MonoBehaviour
{
	[SerializeField]
	private CraftRecipeScriptableObject.RecipeCategoryType _categoryType;

	[SerializeField]
	private CraftingMaterialsUIController _craftingMaterialsUIController;

	private Coroutine _scrollCoroutine;

	public CraftRecipeScriptableObject.RecipeCategoryType CategoryType => _categoryType;

	public CraftingMaterialsUIController CraftingMaterialsUIController => _craftingMaterialsUIController;

	public virtual void Init()
	{
		InitCraftingMaterial();
	}

	public void InitCraftingMaterial()
	{
		_craftingMaterialsUIController.Init();
	}

	public void SetHoverMaterial(List<CraftingIngredient> craftingIngredientList, List<bool> hoverState)
	{
		_craftingMaterialsUIController.SetHover(craftingIngredientList, hoverState);
	}

	public void SetUnHoverMaterial()
	{
		_craftingMaterialsUIController.SetUnHover();
	}

	public virtual void AddContent(CraftingManager craftingManager, CraftRecipeScriptableObject so, Action<CraftRecipeScriptableObject> onClickAction, Action<CraftRecipeScriptableObject, List<bool>> onHoverAction, Action<CraftRecipeScriptableObject> onUnhoverAction)
	{
	}

	public virtual void RefreshButtonData()
	{
	}

	public virtual void SelectFirstButton()
	{
		CraftingItemButtonUI firstButton = GetFirstButton();
		if ((bool)firstButton)
		{
			firstButton.Select();
		}
		else
		{
			NetworkGameManager.Instance.ownPlayer?.inventoryManager?.SelectButton(2);
		}
	}

	protected virtual CraftingItemButtonUI GetFirstButton()
	{
		return null;
	}

	public virtual void SetNavigation(Selectable buttonOnRight = null)
	{
	}

	public virtual void SnapTo(RectTransform target)
	{
	}
}

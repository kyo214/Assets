using System;
using System.Collections.Generic;
using Doozy.Runtime.UIManager.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Toked.Crafting.CraftingUI;

public class CraftingTabUIController : CraftingTabUIControllerBase
{
	[SerializeField]
	private CraftingItemButtonUI _craftingItemButtonPrefab;

	[SerializeField]
	private ScrollRect _scrollRect;

	[SerializeField]
	private Transform _contentTransform;

	private List<CraftingItemButtonUI> _craftingItemButtonUiList = new List<CraftingItemButtonUI>();

	private Coroutine _scrollCoroutine;

	public override void AddContent(CraftingManager craftingManager, CraftRecipeScriptableObject so, Action<CraftRecipeScriptableObject> onClickAction, Action<CraftRecipeScriptableObject, List<bool>> onHoverAction, Action<CraftRecipeScriptableObject> onUnhoverAction)
	{
		CraftingItemButtonUI buttonUI = UnityEngine.Object.Instantiate(_craftingItemButtonPrefab, _contentTransform);
		buttonUI.Init(craftingManager, so, onClickAction, OnHoverAction, onUnhoverAction);
		_craftingItemButtonUiList.Add(buttonUI);
		void OnHoverAction(CraftRecipeScriptableObject arg, List<bool> hasIngredients)
		{
			onHoverAction(arg, hasIngredients);
			if (GlobalOptionsManager.Instance.usingGamepad)
			{
				SnapTo(buttonUI.RectTransform);
			}
		}
	}

	public override void RefreshButtonData()
	{
		foreach (CraftingItemButtonUI craftingItemButtonUi in _craftingItemButtonUiList)
		{
			craftingItemButtonUi.RefreshData();
		}
	}

	protected override CraftingItemButtonUI GetFirstButton()
	{
		if (_craftingItemButtonUiList.Count > 0)
		{
			for (int i = 0; i < _craftingItemButtonUiList.Count; i++)
			{
				CraftingItemButtonUI craftingItemButtonUI = _craftingItemButtonUiList[i];
				if (craftingItemButtonUI.gameObject.activeSelf)
				{
					return craftingItemButtonUI;
				}
			}
		}
		return null;
	}

	public override void SetNavigation(Selectable buttonOnRight = null)
	{
		int count = _craftingItemButtonUiList.Count;
		for (int i = 0; i < count; i++)
		{
			CraftingItemButtonUI craftingItemButtonUI = _craftingItemButtonUiList[i];
			int num = i + 1;
			UIButton uIButton = null;
			while (num < count && uIButton == null)
			{
				UIButton button = _craftingItemButtonUiList[num++].Button;
				uIButton = (button.gameObject.activeSelf ? button : null);
			}
			UIButton uIButton2 = null;
			int num2 = i - 1;
			while (num2 >= 0 && uIButton2 == null)
			{
				UIButton button2 = _craftingItemButtonUiList[num2--].Button;
				uIButton2 = (button2.gameObject.activeSelf ? button2 : null);
			}
			craftingItemButtonUI.SetNavigationButton(new Navigation
			{
				mode = Navigation.Mode.Explicit,
				selectOnDown = uIButton,
				selectOnRight = buttonOnRight,
				selectOnUp = uIButton2
			});
		}
	}

	public override void SnapTo(RectTransform target)
	{
		if (!RectTransformUtility.RectangleContainsScreenPoint(_scrollRect.viewport, target.position))
		{
			_scrollRect.SmoothSnapTo(target);
		}
	}
}

using System;
using System.Collections.Generic;
using Doozy.Runtime.UIManager.Animators;
using Doozy.Runtime.UIManager.Components;
using I2.Loc;
using TMPro;
using Toked.FunctionComponent;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Toked.Crafting.CraftingUI;

public class CraftingItemButtonUI : MonoBehaviour
{
	[SerializeField]
	private RectTransform _rectTransform;

	[SerializeField]
	private CraftingItemRequirementUI _craftingItemRequirementPrefab;

	[SerializeField]
	private CraftRecipeScriptableObject _recipesScriptableObject;

	[SerializeField]
	private UIButton _button;

	[SerializeField]
	private LongClickButton _longClickButton;

	[SerializeField]
	private UISelectableUIAnimator _uiSelectableUIAnimator;

	[SerializeField]
	private Image _recipeImage;

	[SerializeField]
	private TMP_Text _recipeNameText;

	[SerializeField]
	private TMP_Text _discountText;

	[SerializeField]
	private Image _highLightImage;

	[SerializeField]
	private Localize _recipeNameLocalize;

	[SerializeField]
	private Transform _craftingParentTransform;

	[SerializeField]
	private List<CraftingItemRequirementUI> _craftingItemRequirementUis;

	private bool _initialized;

	private List<bool> _hoverStateMaterialUi = new List<bool>();

	private CraftingManager _craftingManager;

	private bool _hasIngredients;

	public RectTransform RectTransform
	{
		get
		{
			if (_rectTransform == null)
			{
				_rectTransform = GetComponent<RectTransform>();
			}
			return _rectTransform;
		}
	}

	public CraftRecipeScriptableObject RecipesScriptableObject => _recipesScriptableObject;

	public UIButton Button => _button;

	public LongClickButton LongClickButton => _longClickButton;

	public event Action OnClickEvents;

	public event Action OnHoverButtonEvents;

	public event Action OnUnhoverButtonEvents;

	public void Init(CraftingManager craftingManager, CraftRecipeScriptableObject so, Action<CraftRecipeScriptableObject> onClickAction, Action<CraftRecipeScriptableObject, List<bool>> onHoverAction, Action<CraftRecipeScriptableObject> onUnhoverAction)
	{
		_craftingManager = craftingManager;
		_recipesScriptableObject = so;
		_recipeImage.sprite = _recipesScriptableObject.RecipeSprite;
		_recipeNameText.text = "";
		_recipeNameLocalize.SetTerm(_recipesScriptableObject.ItemNameLocalizeId);
		if (_recipesScriptableObject.RecipeCategory == CraftRecipeScriptableObject.RecipeCategoryType.Craft && _recipesScriptableObject.ShowAmountText)
		{
			_recipeNameText.text = _recipeNameText.text + " (" + _recipesScriptableObject.CraftAmount + ")";
		}
		OnClickEvents = () =>
		{
			onClickAction(_recipesScriptableObject);
		};
		OnHoverButtonEvents = () =>
		{
			onHoverAction(_recipesScriptableObject, _hoverStateMaterialUi);
		};
		OnUnhoverButtonEvents = () =>
		{
			onUnhoverAction(_recipesScriptableObject);
		};
		InitButton();
		SetUpUI(_recipesScriptableObject);
	}

	public void Select()
	{
		EventSystem.current.SetSelectedGameObject(null);
		_button.Select();
	}

	private void InitButton()
	{
		if (!_initialized)
		{
			_button.normalState.stateEvent.Event.AddListener(UnHoverAction);
			_button.highlightedState.stateEvent.Event.AddListener(HoverAction);
			_button.selectedState.stateEvent.Event.AddListener(HoverAction);
			_button.onPointerEnterBehaviour.Event.AddListener(HoverAction);
			_button.onPointerExitBehaviour.Event.AddListener(UnHoverAction);
			_initialized = true;
		}
	}

	private void SetUpUI(CraftRecipeScriptableObject so)
	{
		int count = so.CraftingIngredientsList.Count;
		int count2 = _craftingItemRequirementUis.Count;
		if (count > count2)
		{
			int num = count - count2;
			for (int i = 0; i < num; i++)
			{
				_craftingItemRequirementUis.Add(UnityEngine.Object.Instantiate(_craftingItemRequirementPrefab, _craftingParentTransform));
			}
		}
		else if (count < count2)
		{
			for (int num2 = count2 - 1; num2 >= count - 1; num2--)
			{
				_craftingItemRequirementUis[num2].gameObject.SetActive(value: false);
			}
		}
		RefreshData(so);
	}

	public void RefreshData(CraftRecipeScriptableObject so = null)
	{
		if (so == null)
		{
			so = _recipesScriptableObject;
		}
		int count = so.CraftingIngredientsList.Count;
		_hoverStateMaterialUi.Clear();
		bool activeButton = true;
		if (so.CheckRequirement(_craftingManager))
		{
			base.gameObject.SetActive(value: true);
			for (int i = 0; i < count; i++)
			{
				CraftingIngredient craftingIngredient = so.CraftingIngredientsList[i];
				bool flag = _craftingManager.CheckIngredient(craftingIngredient);
				Sprite sprite = craftingIngredient.CraftMaterialScriptableObject?.MaterialSprite;
				int amount = ((so.RecipeCategory != CraftRecipeScriptableObject.RecipeCategoryType.Craft) ? ((!_craftingManager.NoIngredientsRequire) ? craftingIngredient.Amount : 0) : ((!_craftingManager.NoIngredientsRequire) ? ((int)((float)craftingIngredient.Amount * _craftingManager.PlayerData.MaterialInventoryManager.DiscountCraftMultiply)) : 0));
				CraftingItemRequirementUI craftingItemRequirementUI = _craftingItemRequirementUis[i];
				craftingItemRequirementUI.Set(sprite, amount);
				craftingItemRequirementUI.SetTextColor(flag);
				craftingItemRequirementUI.gameObject.SetActive(value: true);
				_hoverStateMaterialUi.Add(flag);
				if (!flag)
				{
					activeButton = false;
				}
			}
			SetDiscountText();
			SetActiveButton(activeButton);
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void OnClickButton()
	{
		OnClickEvents?.Invoke();
	}

	private void UnHoverAction()
	{
		_highLightImage.gameObject.SetActive(value: false);
		OnUnhoverButtonEvents?.Invoke();
	}

	private void HoverAction()
	{
		_highLightImage.gameObject.SetActive(value: true);
		OnHoverButtonEvents?.Invoke();
	}

	private void SetActiveButton(bool hasIngredients)
	{
		_hasIngredients = hasIngredients;
		_longClickButton?.SetIntractable(hasIngredients);
	}

	public void SetNavigationButton(Navigation navigation)
	{
		_button.navigation = navigation;
	}

	public void OnClickButton_Button()
	{
		OnClickButton();
	}

	private void SetDiscountText()
	{
		if ((bool)_discountText && _recipesScriptableObject.RecipeCategory == CraftRecipeScriptableObject.RecipeCategoryType.Craft)
		{
			float discountCraft = _craftingManager.PlayerData.MaterialInventoryManager.DiscountCraft;
			if (discountCraft > 0f)
			{
				_discountText.text = $"-{discountCraft * 100f}%";
				_discountText.gameObject.SetActive(value: true);
			}
			else
			{
				_discountText.text = "";
				_discountText.gameObject.SetActive(value: false);
			}
		}
	}
}

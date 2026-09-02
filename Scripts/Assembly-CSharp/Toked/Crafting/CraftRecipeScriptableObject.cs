using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "CraftRecipeScriptableObject", menuName = "WMO/ScriptableObjects/Crafting/Craft Recipe ScriptableObject", order = 0)]
public class CraftRecipeScriptableObject : ScriptableObject
{
	public enum RecipeItemType
	{
		Weapon = 0,
		Heal = 1,
		Ammo = 2,
		Item = 3,
		Skill = 4
	}

	public enum RecipeCategoryType
	{
		Craft = 0,
		Upgrade = 1,
		Skill = 2,
		Dismantle = 3
	}

	public enum CraftStationType
	{
		All = 0,
		Workbench = 1,
		Weaponbench = 2
	}

	[SerializeField]
	private int _sortIndex;

	[SerializeField]
	private string _id;

	[SerializeField]
	private RecipeItemType _recipeItemType;

	[SerializeField]
	private RecipeCategoryType _recipeCategoryType;

	[SerializeField]
	private CraftStationType _craftStation;

	[SerializeField]
	private Sprite _recipeSprite;

	[SerializeField]
	private int _itemCraftId;

	[SerializeField]
	private List<CraftingIngredient> _craftingIngredientsList = new List<CraftingIngredient>();

	[SerializeField]
	private List<CraftingRequirementCheckerAction> _craftingRequirementCheckerActionsList = new List<CraftingRequirementCheckerAction>();

	[SerializeField]
	private bool _showAmountText = true;

	[SerializeField]
	private int _craftAmount = 1;

	[SerializeField]
	[TermsPopup("")]
	private string _itemNameLocalizeId;

	[SerializeField]
	[TermsPopup("")]
	private string _itemDescriptionLocalizeId;

	[SerializeField]
	private bool _useCustomCraftingAction;

	[SerializeField]
	private CraftingBaseAction _craftingAction;

	public int SortIndex
	{
		get
		{
			return _sortIndex;
		}
		set
		{
			_sortIndex = value;
		}
	}

	public string ID
	{
		get
		{
			return _id;
		}
		set
		{
			_id = value;
		}
	}

	public RecipeItemType RecipeItem
	{
		get
		{
			return _recipeItemType;
		}
		set
		{
			_recipeItemType = value;
		}
	}

	public RecipeCategoryType RecipeCategory
	{
		get
		{
			return _recipeCategoryType;
		}
		set
		{
			_recipeCategoryType = value;
		}
	}

	public CraftStationType CraftStation
	{
		get
		{
			return _craftStation;
		}
		set
		{
			_craftStation = value;
		}
	}

	public Sprite RecipeSprite
	{
		get
		{
			return _recipeSprite;
		}
		set
		{
			_recipeSprite = value;
		}
	}

	public List<CraftingIngredient> CraftingIngredientsList
	{
		get
		{
			return _craftingIngredientsList;
		}
		set
		{
			_craftingIngredientsList = value;
		}
	}

	public int ItemCraftId
	{
		get
		{
			return _itemCraftId;
		}
		set
		{
			_itemCraftId = value;
		}
	}

	public bool ShowAmountText
	{
		get
		{
			return _showAmountText;
		}
		set
		{
			_showAmountText = value;
		}
	}

	public int CraftAmount
	{
		get
		{
			return _craftAmount;
		}
		set
		{
			_craftAmount = value;
		}
	}

	public string ItemNameLocalizeId
	{
		get
		{
			return _itemNameLocalizeId;
		}
		set
		{
			_itemNameLocalizeId = value;
		}
	}

	public string ItemDescriptionLocalizeId
	{
		get
		{
			return _itemDescriptionLocalizeId;
		}
		set
		{
			_itemDescriptionLocalizeId = value;
		}
	}

	public bool UseCustomCraftingAction
	{
		get
		{
			return _useCustomCraftingAction;
		}
		set
		{
			_useCustomCraftingAction = value;
		}
	}

	public CraftingBaseAction CraftingAction
	{
		get
		{
			return _craftingAction;
		}
		set
		{
			_craftingAction = value;
		}
	}

	public void Crafting(CraftingManager craftingManager)
	{
		if (_useCustomCraftingAction)
		{
			_craftingAction?.Craft(craftingManager, this);
		}
	}

	public bool CheckRequirement(CraftingManager craftingManager)
	{
		if (_craftingRequirementCheckerActionsList.Count <= 0)
		{
			return true;
		}
		bool flag = true;
		foreach (CraftingRequirementCheckerAction craftingRequirementCheckerActions in _craftingRequirementCheckerActionsList)
		{
			if (!(craftingRequirementCheckerActions == null))
			{
				flag = craftingRequirementCheckerActions.CheckRequirement(this, craftingManager);
				if (!flag)
				{
					return false;
				}
			}
		}
		return flag;
	}

	private static IEnumerable GetItemId()
	{
		ValueDropdownList<int> result = new ValueDropdownList<int>();
		result.Add("None", 0);
		BGDatabase_Ammunition.ForEachEntity((BGDatabase_Ammunition data) =>
		{
			AddToList("Ammunition/" + data.Name, data.Keys);
		});
		BGDatabase_Weapon.ForEachEntity((BGDatabase_Weapon data) =>
		{
			AddToList("Weapon/" + data.Name, data.Keys);
		});
		BGDatabase_Item.ForEachEntity((BGDatabase_Item data) =>
		{
			AddToList("Item/" + data.Name, data.Keys);
		});
		BGDatabase_HealingItem.ForEachEntity((BGDatabase_HealingItem data) =>
		{
			AddToList("HealingItem/" + data.Name, data.Keys);
		});
		return result;
		void AddToList(string inspectorName, int value)
		{
			result.Add(inspectorName, value);
		}
	}
}

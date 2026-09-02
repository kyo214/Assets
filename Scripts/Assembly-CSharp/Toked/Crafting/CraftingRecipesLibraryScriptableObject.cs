using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "CraftingRecipesLibrary", menuName = "WMO/ScriptableObjects/Crafting/Crafting Recipes Library", order = 0)]
public class CraftingRecipesLibraryScriptableObject : ScriptableObjectLibraryDictionaryBase<string, CraftRecipeScriptableObject>
{
	protected override void AddDataDictionary(Dictionary<string, CraftRecipeScriptableObject> dic, CraftRecipeScriptableObject data)
	{
		if (!dic.ContainsKey(data.ID))
		{
			dic.Add(data.ID, data);
		}
	}

	public override void RefreshLibraryDatabase()
	{
	}

	protected override void UpdateData(CraftRecipeScriptableObject data)
	{
		BGDatabase_CraftRecipe bGDatabase_CraftRecipe = BGDatabase_CraftRecipe.FindEntity((BGDatabase_CraftRecipe entity) => entity.IdRecipe == data.ID);
		if (bGDatabase_CraftRecipe == null)
		{
			Debug.LogError("Data not found " + data.ID);
		}
		else
		{
			UpdateData(data, bGDatabase_CraftRecipe);
		}
	}

	private void UpdateData(CraftRecipeScriptableObject data, BGDatabase_CraftRecipe database)
	{
		data.SortIndex = database.Index;
		data.CraftStation = database.CraftStation;
		data.RecipeCategory = database.RecipeCategoryType;
		data.RecipeItem = database.RecipeItemType;
		data.ItemCraftId = database.ItemCraftId;
		data.CraftingIngredientsList = CraftingIngredientsParser(database.CraftingIngredientsList);
		data.CraftAmount = database.CraftAmount;
		data.ItemNameLocalizeId = database.ItemNameLocalizeId;
		data.ItemDescriptionLocalizeId = database.ItemDescriptionLocalizeId;
	}

	public static List<CraftingIngredient> CraftingIngredientsParser(List<string> ingredientsString)
	{
		List<CraftingIngredient> list = new List<CraftingIngredient>();
		if (ingredientsString == null)
		{
			return list;
		}
		int result = 0;
		foreach (string item in ingredientsString)
		{
			string[] array = item.Split("=");
			CraftingIngredient craftingIngredient = new CraftingIngredient();
			craftingIngredient.CraftMaterialScriptableObject = DataManager.Instance.Get<CraftMaterialLibraryScriptableObject>()?.GetData(array[0]);
			int.TryParse(array[1], out result);
			craftingIngredient.Amount = result;
			list.Add(craftingIngredient);
		}
		return list;
	}

	protected override List<CraftRecipeScriptableObject> SortList()
	{
		return _dataDictionary.Values.OrderBy((CraftRecipeScriptableObject o) => o.SortIndex).ToList();
	}

	protected override CraftRecipeScriptableObject CreateSo(string soName)
	{
		return null;
	}

	protected CraftSkillRecipeScriptableObject CreateSkillLearnSo(string soName)
	{
		return null;
	}
}

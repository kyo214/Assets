using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "CraftingSkillRecipesLibraryScriptableObject", menuName = "WMO/ScriptableObjects/Crafting/Crafting Skill Recipes Library", order = 0)]
public class CraftingSkillRecipesLibraryScriptableObject : ScriptableObjectLibraryDictionaryBase<string, CraftSkillRecipeScriptableObject>
{
	protected override void AddDataDictionary(Dictionary<string, CraftSkillRecipeScriptableObject> dic, CraftSkillRecipeScriptableObject data)
	{
		if (!dic.ContainsKey(data.ID))
		{
			dic.Add(data.ID, data);
		}
	}

	public override void RefreshLibraryDatabase()
	{
	}

	protected override void UpdateData(CraftSkillRecipeScriptableObject data)
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

	private void UpdateData(CraftSkillRecipeScriptableObject data, BGDatabase_CraftRecipe database)
	{
		data.SortIndex = database.Index;
		data.CraftStation = database.CraftStation;
		data.RecipeCategory = database.RecipeCategoryType;
		data.RecipeItem = database.RecipeItemType;
		data.ItemCraftId = database.ItemCraftId;
		data.CraftingIngredientsList = CraftingRecipesLibraryScriptableObject.CraftingIngredientsParser(database.CraftingIngredientsList);
		data.CraftAmount = database.CraftAmount;
		data.ItemNameLocalizeId = database.ItemNameLocalizeId;
		data.ItemDescriptionLocalizeId = database.ItemDescriptionLocalizeId;
	}

	protected override List<CraftSkillRecipeScriptableObject> SortList()
	{
		return _dataDictionary.Values.OrderBy((CraftSkillRecipeScriptableObject o) => o.SortIndex).ToList();
	}

	protected override CraftSkillRecipeScriptableObject CreateSo(string soName)
	{
		return null;
	}
}

using System.Collections.Generic;
using Toked.Skill;
using UnityEngine;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "SkillLearnAction", menuName = "WMO/ScriptableObjects/Crafting/Crafting Action/SkillLearnAction", order = 0)]
public class SkillLearnAction : CraftingBaseAction
{
	[SerializeField]
	private int _chooseOptionSkill = 3;

	public override void Craft(CraftingManager craftingManager, CraftRecipeScriptableObject craftRecipeSo)
	{
		Action(craftingManager, craftRecipeSo);
	}

	protected override void Action(CraftingManager craftingManager, CraftRecipeScriptableObject craftRecipeSo)
	{
		CraftSkillRecipeScriptableObject craftSkill = craftRecipeSo as CraftSkillRecipeScriptableObject;
		if ((object)craftSkill != null)
		{
			craftingManager.SkillLearnPopupUI.Init(GetRandomSkill(craftSkill.SkillLearnSOList, craftingManager.PlayerController), () =>
			{
				RemoveMaterial(craftingManager, craftSkill.CraftingIngredientsList);
			});
		}
	}

	private List<SkillScriptableObject> GetRandomSkill(List<SkillScriptableObject> skillLearnSOList, PlayerController playerController)
	{
		List<SkillScriptableObject> list = new List<SkillScriptableObject>(skillLearnSOList);
		list.Shuffle();
		List<SkillScriptableObject> list2 = new List<SkillScriptableObject>();
		int num = 0;
		for (int i = 0; i < list.Count; i++)
		{
			SkillScriptableObject skillScriptableObject = list[i];
			if (skillScriptableObject.CheckSkillGameModeTypeUse() && !playerController.data.CheckSkillLearn(skillScriptableObject.ID))
			{
				list2.Add(skillScriptableObject);
				num++;
			}
			if (num == _chooseOptionSkill)
			{
				break;
			}
		}
		return list2;
	}
}

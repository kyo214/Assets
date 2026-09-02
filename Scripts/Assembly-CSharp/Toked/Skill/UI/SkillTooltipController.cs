using I2.Loc;
using Toked.Crafting.CraftingUI;
using UnityEngine;
using _Modules.UIGlobal;

namespace Toked.Skill.UI;

public class SkillTooltipController : ToolTipController
{
	[SerializeField]
	private CraftingSkillUI _craftingSkillUI;

	public override void ShowTooltipDescription()
	{
		if (!(_craftingSkillUI.SkillScriptableObject == null))
		{
			string skillTooltipDescription = GetSkillTooltipDescription(_craftingSkillUI.SkillScriptableObject);
			string translation = LocalizationManager.GetTranslation(_craftingSkillUI.SkillScriptableObject.SkillNameLocalizeId);
			Vector3 position = base.gameObject.transform.position;
			GenericSingleton<TooltipManager>.Instance.Show(translation, skillTooltipDescription, position, _pivotPresets, _offset);
			_isShowTooltip = true;
		}
	}

	private string GetSkillTooltipDescription(SkillScriptableObject skill)
	{
		string translation = LocalizationManager.GetTranslation(skill.SkillDescriptionLocalizeId);
		return skill.SetStatsValueLocalization(translation);
	}
}

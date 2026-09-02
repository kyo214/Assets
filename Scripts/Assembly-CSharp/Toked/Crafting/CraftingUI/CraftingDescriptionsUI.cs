using I2.Loc;
using TMPro;
using Toked.Skill;
using UnityEngine;

namespace Toked.Crafting.CraftingUI;

public class CraftingDescriptionsUI : MonoBehaviour
{
	[SerializeField]
	private TMP_Text _itemNameText;

	[SerializeField]
	private Localize _itemNameTextLocalize;

	[SerializeField]
	private TMP_Text _descriptionText;

	[SerializeField]
	private Localize _descriptionTextLocalize;

	public void Set(CraftRecipeScriptableObject craftRecipeScriptableObject)
	{
		_itemNameText.text = "";
		_descriptionText.text = "";
		_itemNameTextLocalize.SetTerm(craftRecipeScriptableObject.ItemNameLocalizeId);
		_descriptionTextLocalize.SetTerm(craftRecipeScriptableObject.ItemDescriptionLocalizeId);
	}

	public void Set(SkillScriptableObject skillScriptableObject)
	{
		_itemNameText.text = "";
		_descriptionText.text = "";
		_itemNameTextLocalize.SetTerm(skillScriptableObject.SkillNameLocalizeId);
		_descriptionTextLocalize.SetTerm(skillScriptableObject.SkillDescriptionLocalizeId);
	}

	public void Set(string itemNameTerm, string descriptionTerm)
	{
		_itemNameText.text = "";
		_descriptionText.text = "";
		_itemNameTextLocalize.SetTerm(itemNameTerm);
		_descriptionTextLocalize.SetTerm(descriptionTerm);
	}

	public void Reset()
	{
		_itemNameText.text = "";
		_descriptionText.text = "";
		_itemNameTextLocalize.SetTerm("");
		_descriptionTextLocalize.SetTerm("");
	}
}

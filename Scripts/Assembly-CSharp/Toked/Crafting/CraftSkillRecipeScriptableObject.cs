using System.Collections.Generic;
using Toked.Skill;
using UnityEngine;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "CraftSkillRecipeScriptableObject", menuName = "WMO/ScriptableObjects/Crafting/Craft Skill Recipe ScriptableObject", order = 0)]
public class CraftSkillRecipeScriptableObject : CraftRecipeScriptableObject
{
	[SerializeField]
	private List<SkillScriptableObject> _skillLearnSOList = new List<SkillScriptableObject>();

	public List<SkillScriptableObject> SkillLearnSOList => _skillLearnSOList;
}

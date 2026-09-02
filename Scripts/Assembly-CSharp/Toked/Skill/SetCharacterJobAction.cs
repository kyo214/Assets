using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Toked.Skill;

[CreateAssetMenu(fileName = "SetCharacterJobAction", menuName = "WMO/ScriptableObjects/Skill/Skill Action/Set Character Job Action", order = 0)]
public class SetCharacterJobAction : SkillEffectBaseAction
{
	[SerializeField]
	private string _characterJob = "Default";

	public override void Apply(PlayerController playerController, SkillScriptableObject skillScriptableObject)
	{
		if (_characterJob != "")
		{
			playerController.data.SetCharacterJob(_characterJob);
		}
	}

	private static IEnumerable GetItemId()
	{
		ValueDropdownList<string> result = new ValueDropdownList<string>();
		BGDatabase_Character.ForEachEntity((BGDatabase_Character data) =>
		{
			AddToList(data.Keys ?? "", data.Keys);
		});
		return result;
		void AddToList(string inspectorName, string value)
		{
			result.Add(inspectorName, value);
		}
	}
}

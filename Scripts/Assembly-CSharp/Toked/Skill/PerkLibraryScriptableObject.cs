using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toked.Skill;

[CreateAssetMenu(fileName = "PerkLibraryScriptableObject", menuName = "WMO/ScriptableObjects/Skill/Perk Library ScriptableObject", order = 0)]
public class PerkLibraryScriptableObject : ScriptableObjectLibraryDictionaryBase<string, SkillScriptableObject>
{
	private static readonly string PERKEFFECTS_PATH = "Assets/ScriptableObjects/Crafting/SkillEffect/Perks/";

	protected override void AddDataDictionary(Dictionary<string, SkillScriptableObject> dic, SkillScriptableObject data)
	{
		if (!dic.ContainsKey(data.ID))
		{
			dic.Add(data.ID, data);
		}
	}

	public override void RefreshLibraryDatabase()
	{
	}

	protected override void UpdateData(SkillScriptableObject data)
	{
		BGDatabase_Perks bGDatabase_Perks = BGDatabase_Perks.FindEntity((BGDatabase_Perks entity) => entity.Id == data.ID);
		if (bGDatabase_Perks == null)
		{
			Debug.LogError("Data not found " + data.ID);
		}
		else
		{
			UpdateData(data, bGDatabase_Perks);
		}
	}

	private void UpdateData(SkillScriptableObject data, BGDatabase_Perks database)
	{
		data.SortIndex = database.Index;
		data.ID = database.Id;
		data.SkillNameLocalizeId = database.PerkNameLocalizeId;
		data.SkillDescriptionLocalizeId = database.PerkDescriptionLocalizeId;
	}

	protected override List<SkillScriptableObject> SortList()
	{
		return _dataDictionary.Values.OrderBy((SkillScriptableObject o) => o.SortIndex).ToList();
	}

	protected override SkillScriptableObject CreateSo(string soName)
	{
		return null;
	}
}

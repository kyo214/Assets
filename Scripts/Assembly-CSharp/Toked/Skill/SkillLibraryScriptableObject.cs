using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Toked.Skill;

[CreateAssetMenu(fileName = "SkillLibraryScriptableObject", menuName = "WMO/ScriptableObjects/Skill/Skill Library ScriptableObject", order = 0)]
public class SkillLibraryScriptableObject : ScriptableObjectLibraryDictionaryBase<string, SkillScriptableObject>
{
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
		BGDatabase_Skill bGDatabase_Skill = BGDatabase_Skill.FindEntity((BGDatabase_Skill entity) => entity.Id == data.ID);
		if (bGDatabase_Skill == null)
		{
			Debug.LogError("Data not found " + data.ID);
		}
		else
		{
			UpdateData(data, bGDatabase_Skill);
		}
	}

	private void UpdateData(SkillScriptableObject data, BGDatabase_Skill database)
	{
		data.SortIndex = database.Index;
		data.ID = database.Id;
		data.SkillNameLocalizeId = database.SkillNameLocalizeId;
		data.SkillDescriptionLocalizeId = database.SkillDescriptionLocalizeId;
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

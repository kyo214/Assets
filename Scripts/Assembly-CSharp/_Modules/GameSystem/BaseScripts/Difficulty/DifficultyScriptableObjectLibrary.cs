using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Modules.GameSystem.BaseScripts.Difficulty;

[CreateAssetMenu(fileName = "DifficultyScriptableObjectLibrary", menuName = "WMO/ScriptableObjects/Difficulty/DifficultyScriptableObjectLibrary", order = 0)]
public class DifficultyScriptableObjectLibrary : ScriptableObjectLibraryDictionaryBase<DifficultySetting.Difficulty, DifficultyScriptableObject>
{
	protected override List<DifficultyScriptableObject> SortList()
	{
		return _dataDictionary.Values.OrderBy((DifficultyScriptableObject o) => o.GetDifficultyData().DifficultySetting).ToList();
	}

	protected override void AddDataDictionary(Dictionary<DifficultySetting.Difficulty, DifficultyScriptableObject> dic, DifficultyScriptableObject data)
	{
		if (!dic.ContainsKey(data.GetDifficultyData().DifficultySetting))
		{
			dic.Add(data.GetDifficultyData().DifficultySetting, data);
		}
	}

	protected override void UpdateData(DifficultyScriptableObject data)
	{
	}
}

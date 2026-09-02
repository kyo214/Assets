using System.Collections.Generic;
using UnityEngine;

namespace _Modules.GameSystem.BaseScripts.Scenario;

[CreateAssetMenu(fileName = "ScenarioScriptableObjectLibrary", menuName = "WMO/ScriptableObjects/Scenario/ScenarioScriptableObjectLibrary", order = 0)]
public class ScenarioScriptableObjectLibrary : ScriptableObjectLibraryDictionaryBase<string, ScenarioScriptableObject>
{
	protected override void AddDataDictionary(Dictionary<string, ScenarioScriptableObject> dic, ScenarioScriptableObject data)
	{
		if (!dic.ContainsKey(data.ScenarioId))
		{
			dic.Add(data.ScenarioId, data);
		}
	}

	protected override void UpdateData(ScenarioScriptableObject data)
	{
	}
}

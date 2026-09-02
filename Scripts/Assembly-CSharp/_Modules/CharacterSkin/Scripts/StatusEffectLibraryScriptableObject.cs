using System.Collections.Generic;
using Toked.StatusEffect;
using UnityEngine;

namespace _Modules.CharacterSkin.Scripts;

[CreateAssetMenu(fileName = "StatusEffectLibraryScriptableObject", menuName = "WMO/ScriptableObjects/StatusEffect/StatusEffectLibraryScriptableObject", order = 0)]
public class StatusEffectLibraryScriptableObject : ScriptableObjectLibraryDictionaryBase<string, StatusEffectScriptableObject>
{
	protected override void AddDataDictionary(Dictionary<string, StatusEffectScriptableObject> dic, StatusEffectScriptableObject data)
	{
		if (!dic.ContainsKey(data.StatusEffectData.BaseName))
		{
			dic.Add(data.StatusEffectData.BaseName, data);
		}
	}

	protected override void UpdateData(StatusEffectScriptableObject data)
	{
	}
}

using System.Collections.Generic;
using UnityEngine;

namespace _Modules.CharacterSkin.Scripts;

[CreateAssetMenu(fileName = "CharacterSkinColorLibraryScriptableObject", menuName = "WMO/ScriptableObjects/Skin/CharacterSkinColorLibraryScriptableObject", order = 0)]
public class CharacterSkinColorLibraryScriptableObject : ScriptableObjectLibraryDictionaryBase<string, SkinColorScriptableObject>
{
	protected override void AddDataDictionary(Dictionary<string, SkinColorScriptableObject> dic, SkinColorScriptableObject data)
	{
		if (!dic.ContainsKey(data.SkinColorId))
		{
			dic.Add(data.SkinColorId, data);
		}
	}

	protected override void UpdateData(SkinColorScriptableObject data)
	{
	}
}

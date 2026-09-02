using System.Collections.Generic;
using UnityEngine;

namespace _Modules.CharacterSkin.Scripts;

[CreateAssetMenu(fileName = "CharacterSkinLibraryScriptableObject", menuName = "WMO/ScriptableObjects/Skin/CharacterSkinLibraryScriptableObject", order = 0)]
public class CharacterSkinLibraryScriptableObject : ScriptableObjectLibraryDictionaryBase<string, SkinScriptableObject>
{
	protected override void AddDataDictionary(Dictionary<string, SkinScriptableObject> dic, SkinScriptableObject data)
	{
		if (!dic.ContainsKey(data.CharacterSkinId))
		{
			dic.Add(data.CharacterSkinId, data);
		}
	}

	protected override void UpdateData(SkinScriptableObject data)
	{
	}
}

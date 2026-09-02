using System.Collections.Generic;
using UnityEngine;

namespace _Modules.CharacterSkin.Scripts;

[CreateAssetMenu(fileName = "CharacterColorSkinLibraryScriptableObject", menuName = "WMO/ScriptableObjects/Skin/CharacterColorPaletteLibraryScriptableObject", order = 0)]
public class CharacterColorPaletteLibraryScriptableObject : ScriptableObjectLibraryDictionaryBase<string, SkinColorPaletteScriptableObject>
{
	protected override void AddDataDictionary(Dictionary<string, SkinColorPaletteScriptableObject> dic, SkinColorPaletteScriptableObject data)
	{
		if (!dic.ContainsKey(data.CharacterColorSkinId))
		{
			dic.Add(data.CharacterColorSkinId, data);
		}
	}

	protected override void UpdateData(SkinColorPaletteScriptableObject data)
	{
	}
}

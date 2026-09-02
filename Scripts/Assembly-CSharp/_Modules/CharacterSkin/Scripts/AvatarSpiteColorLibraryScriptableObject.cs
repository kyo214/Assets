using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Modules.CharacterSkin.Scripts;

[CreateAssetMenu(fileName = "AvatarSpiteColorLibraryScriptableObject", menuName = "WMO/ScriptableObjects/Skin/AvatarSpiteColorLibraryScriptableObject", order = 0)]
public class AvatarSpiteColorLibraryScriptableObject : ScriptableObjectLibraryDictionaryBase<SkinColorPaletteScriptableObject, AvatarScriptableObject>
{
	protected override void AddDataDictionary(Dictionary<SkinColorPaletteScriptableObject, AvatarScriptableObject> dic, AvatarScriptableObject data)
	{
		throw new NotImplementedException();
	}

	protected override void UpdateData(AvatarScriptableObject data)
	{
		throw new NotImplementedException();
	}
}

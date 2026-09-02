using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Modules.CharacterSkin.Scripts;

[CreateAssetMenu(fileName = "AvatarSpiteLibraryScriptableObject", menuName = "WMO/ScriptableObjects/Skin/AvatarSpiteLibraryScriptableObject", order = 0)]
public class AvatarSpiteLibraryScriptableObject : ScriptableObjectLibraryDictionaryBase<SkinScriptableObject, AvatarSpiteColorLibraryScriptableObject>
{
	protected override void AddDataDictionary(Dictionary<SkinScriptableObject, AvatarSpiteColorLibraryScriptableObject> dic, AvatarSpiteColorLibraryScriptableObject data)
	{
		throw new NotImplementedException();
	}

	protected override void UpdateData(AvatarSpiteColorLibraryScriptableObject data)
	{
		throw new NotImplementedException();
	}
}

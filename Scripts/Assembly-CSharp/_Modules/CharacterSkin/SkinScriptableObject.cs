using System.Collections.Generic;
using UnityEngine;
using _Modules.GameSystem.BaseScripts;

namespace _Modules.CharacterSkin;

[CreateAssetMenu(fileName = "Skin", menuName = "WMO/ScriptableObjects/Skin/SkinScriptableObject", order = 0)]
public class SkinScriptableObject : ScriptableObject
{
	[SerializeField]
	private CharacterSkinData _characterSkinData;

	[SerializeField]
	private List<UnlockItemRequirementBaseSO> _unlockRequirementList;

	public string CharacterSkinId
	{
		get
		{
			return _characterSkinData.CharacterSkinId;
		}
		set
		{
			_characterSkinData.CharacterSkinId = value;
		}
	}

	public string CharacterSkinName
	{
		get
		{
			return _characterSkinData.CharacterSkinName;
		}
		set
		{
			_characterSkinData.CharacterSkinName = value;
		}
	}

	public CharacterSkinData CharacterSkinData
	{
		get
		{
			return _characterSkinData;
		}
		set
		{
			_characterSkinData = value;
		}
	}

	public bool CheckRequirementUnlock()
	{
		foreach (UnlockItemRequirementBaseSO unlockRequirement in _unlockRequirementList)
		{
			if (!(unlockRequirement == null) && !unlockRequirement.CheckRequirement())
			{
				return false;
			}
		}
		return true;
	}
}

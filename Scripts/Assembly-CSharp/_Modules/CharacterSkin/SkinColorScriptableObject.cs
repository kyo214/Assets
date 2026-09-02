using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Modules.CharacterSkin;

[CreateAssetMenu(fileName = "SkinColorScriptableObject", menuName = "WMO/ScriptableObjects/Skin/SkinColorScriptableObject", order = 0)]
public class SkinColorScriptableObject : SerializedScriptableObject
{
	private readonly string COLOR_PREVIEW_KEY = "ReplaceColor11";

	[SerializeField]
	private string _skinColorId;

	[SerializeField]
	private Dictionary<string, Color> _skinColorDataDict = new Dictionary<string, Color>();

	public string SkinColorId
	{
		get
		{
			return _skinColorId;
		}
		set
		{
			_skinColorId = value;
		}
	}

	public Dictionary<string, Color> SkinColorDataDict
	{
		get
		{
			return _skinColorDataDict;
		}
		set
		{
			_skinColorDataDict = value;
		}
	}

	public Color GetSkinColorPreview()
	{
		return _skinColorDataDict[COLOR_PREVIEW_KEY];
	}
}

using UnityEngine;

namespace _Modules.CharacterSkin;

[CreateAssetMenu(fileName = "Skin", menuName = "WMO/ScriptableObjects/Skin/SkinColorPaletteScriptableObject", order = 0)]
public class SkinColorPaletteScriptableObject : ScriptableObject
{
	public static readonly int ReplaceColor1 = Shader.PropertyToID("_ReplaceColor1");

	[SerializeField]
	private string _characterColorSkinId;

	[SerializeField]
	private string _characterColorSkinIdAlias;

	[SerializeField]
	private string _characterColorSkinName;

	[SerializeField]
	private Color _characterColorSkinPreview;

	[SerializeField]
	private Material _characterColorSkinMaterial;

	public string CharacterColorSkinId
	{
		get
		{
			return _characterColorSkinId;
		}
		set
		{
			_characterColorSkinId = value;
		}
	}

	public string CharacterColorSkinIdAlias
	{
		get
		{
			if (string.IsNullOrWhiteSpace(_characterColorSkinIdAlias))
			{
				_characterColorSkinIdAlias = _characterColorSkinId;
			}
			return _characterColorSkinIdAlias;
		}
		set
		{
			_characterColorSkinIdAlias = value;
		}
	}

	public string CharacterColorSkinName
	{
		get
		{
			return _characterColorSkinName;
		}
		set
		{
			_characterColorSkinName = value;
		}
	}

	public Color CharacterColorSkinPreview
	{
		get
		{
			return _characterColorSkinPreview;
		}
		set
		{
			_characterColorSkinPreview = value;
		}
	}

	public Material CharacterColorSkinMaterial
	{
		get
		{
			return _characterColorSkinMaterial;
		}
		set
		{
			_characterColorSkinMaterial = value;
		}
	}

	public void RefreshCharacterColorSkinIdAlias()
	{
		if (string.IsNullOrWhiteSpace(_characterColorSkinIdAlias))
		{
			_characterColorSkinIdAlias = _characterColorSkinId;
		}
	}
}

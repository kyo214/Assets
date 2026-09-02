using UnityEngine;
using _Modules.CharacterSkin.Scripts;

namespace _Modules.CharacterSkin;

public class CharacterSkinChanger : MonoBehaviour
{
	[SerializeField]
	private CharacterSkinLibraryScriptableObject _characterSkinLibraryScriptableObject;

	[SerializeField]
	private CharacterSkinLibraryScriptableObject _characterBodyLibraryScriptableObject;

	[SerializeField]
	private CharacterColorPaletteLibraryScriptableObject _characterColorPaletteLibraryScriptableObject;

	[SerializeField]
	private CharacterSkinColorLibraryScriptableObject _characterSkinColorLibraryScriptableObject;

	public CharacterSkinLibraryScriptableObject CharacterSkinLibrary
	{
		get
		{
			if (_characterSkinLibraryScriptableObject == null)
			{
				_characterSkinLibraryScriptableObject = SkinManager.Instance.heroSkinLibraryScriptableObject;
			}
			return _characterSkinLibraryScriptableObject;
		}
	}

	public CharacterSkinLibraryScriptableObject CharacterBodyLibraryScriptableObject
	{
		get
		{
			if (_characterBodyLibraryScriptableObject == null)
			{
				_characterBodyLibraryScriptableObject = SkinManager.Instance.heroSkinLibraryScriptableObject;
			}
			return _characterBodyLibraryScriptableObject;
		}
	}

	public CharacterColorPaletteLibraryScriptableObject CharacterColorPaletteLibrary
	{
		get
		{
			if (_characterColorPaletteLibraryScriptableObject == null)
			{
				_characterColorPaletteLibraryScriptableObject = SkinManager.Instance.heroColorPaletteLibraryScriptableObject;
			}
			return _characterColorPaletteLibraryScriptableObject;
		}
	}

	public CharacterSkinColorLibraryScriptableObject CharacterSkinColorLibrary
	{
		get
		{
			if (_characterSkinColorLibraryScriptableObject == null)
			{
				_characterSkinColorLibraryScriptableObject = SkinManager.Instance.heroSkinColorLibraryScriptableObject;
			}
			return _characterSkinColorLibraryScriptableObject;
		}
	}

	public void ChangeHeadSkin(PlayerController playerController, SkinScriptableObject skinScriptableObject)
	{
		playerController.data.PlayerSkinData.SetHeadSkinData(skinScriptableObject.CharacterSkinData);
	}

	public void ChangeBodySkin(PlayerController playerController, SkinScriptableObject skinScriptableObject)
	{
		playerController.data.PlayerSkinData.SetBodySkinData(skinScriptableObject.CharacterSkinData);
	}

	public void ChangeMaterialSkin(PlayerController playerController, SkinColorPaletteScriptableObject skinPaletteScriptableObject)
	{
		playerController.data.PlayerSkinData.SetMaterialSkinData(skinPaletteScriptableObject);
	}

	public void ChangeSkinColor(PlayerController playerController, SkinColorScriptableObject skinColorScriptableObject)
	{
		playerController.data.PlayerSkinData.SetSkinColorData(skinColorScriptableObject);
	}
}

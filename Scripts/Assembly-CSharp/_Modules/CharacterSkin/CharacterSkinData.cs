using System;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.U2D.Animation;
using _Modules.CharacterSkin.Scripts;

namespace _Modules.CharacterSkin;

[Serializable]
public class CharacterSkinData
{
	public enum Gender
	{
		Male = 0,
		Female = 1,
		Other = 2
	}

	[SerializeField]
	private string _characterSkinId;

	[SerializeField]
	private string _characterSkinName;

	[FormerlySerializedAs("_characterSkinLocalize")]
	[SerializeField]
	[TermsPopup("")]
	private string _characterSkinNameLocalize;

	[SerializeField]
	private bool _useColorCharacterPortrait;

	[SerializeField]
	private AvatarSpiteLibraryScriptableObject _avatarSpiteLibraryScriptableObject;

	[SerializeField]
	private Sprite _characterPortraitSprite;

	[SerializeField]
	private Sprite _characterMiniPortraitSprite;

	public Gender skinGender;

	public SpriteLibraryAsset headLibraryAsset;

	public SpriteLibraryAsset headAccLibraryAsset;

	[FormerlySerializedAs("headSkinColor")]
	public SkinColorPaletteScriptableObject headSkinColorPalette;

	public SpriteLibraryAsset bodyLibraryAsset;

	public SpriteLibraryAsset bodyAccLibraryAsset;

	public SpriteLibraryAsset handLibraryAsset;

	public SpriteLibraryAsset hand2LibraryAsset;

	public SpriteLibraryAsset legLibraryAsset;

	public SpriteLibraryAsset leg2LibraryAsset;

	[FormerlySerializedAs("skinColorSo")]
	public SkinColorPaletteScriptableObject skinColorPaletteSo;

	public List<SkinColorPaletteScriptableObject> skinColorPaletteSoList;

	public string CharacterSkinId
	{
		get
		{
			return _characterSkinId;
		}
		set
		{
			_characterSkinId = value;
		}
	}

	public string CharacterSkinName
	{
		get
		{
			return _characterSkinName;
		}
		set
		{
			_characterSkinName = value;
		}
	}

	public string CharacterSkinNameLocalize
	{
		get
		{
			return _characterSkinNameLocalize;
		}
		set
		{
			_characterSkinNameLocalize = value;
		}
	}

	public bool UseColorCharacterPortrait
	{
		get
		{
			return _useColorCharacterPortrait;
		}
		set
		{
			_useColorCharacterPortrait = value;
		}
	}

	public Sprite CharacterPortraitSprite
	{
		get
		{
			return _characterPortraitSprite;
		}
		set
		{
			_characterPortraitSprite = value;
		}
	}

	public Sprite CharacterMiniPortraitSprite
	{
		get
		{
			return _characterMiniPortraitSprite;
		}
		set
		{
			_characterMiniPortraitSprite = value;
		}
	}

	public Gender GetGender()
	{
		if (skinGender != Gender.Other)
		{
			return skinGender;
		}
		return SkinManager.Instance.GetDefaultHeroSkinByIndex(NetworkGameManager.Instance.ownPlayer.network.GetIDX()).skinGender;
	}

	public AvatarScriptableObject GetCharacterSpriteSo(SkinScriptableObject skinScriptableObject, SkinColorPaletteScriptableObject skinColorPaletteScriptableObject)
	{
		SkinColorPaletteScriptableObject key = CheckCompatibleSkinColor(skinColorPaletteScriptableObject);
		object obj = _avatarSpiteLibraryScriptableObject.GetData(skinScriptableObject)?.GetData(key);
		if (obj == null)
		{
			obj = _avatarSpiteLibraryScriptableObject.GetData(skinScriptableObject)?.GetData(skinColorPaletteSo);
			if (obj == null)
			{
				AvatarSpiteColorLibraryScriptableObject data = _avatarSpiteLibraryScriptableObject.GetData(skinScriptableObject);
				if ((object)data == null)
				{
					return null;
				}
				obj = data.GetDataByIndex(0);
			}
		}
		return (AvatarScriptableObject)obj;
	}

	public Sprite GetCharacterSprite(SkinScriptableObject skinScriptableObject, SkinColorPaletteScriptableObject skinColorPaletteScriptableObject)
	{
		return GetCharacterSpriteSo(skinScriptableObject, skinColorPaletteScriptableObject)?.AvatarSprite;
	}

	public Sprite GetMiniCharacterSprite(SkinScriptableObject skinScriptableObject, SkinColorPaletteScriptableObject skinColorPaletteScriptableObject)
	{
		return GetCharacterSpriteSo(skinScriptableObject, skinColorPaletteScriptableObject)?.MiniAvatarSprite;
	}

	public SkinColorPaletteScriptableObject GetSkinColorPaletteById(string id)
	{
		return skinColorPaletteSoList.Find((SkinColorPaletteScriptableObject color) => color.CharacterColorSkinId == id) ?? skinColorPaletteSoList.Find((SkinColorPaletteScriptableObject color) => (color.CharacterColorSkinId.Length > id.Length) ? color.CharacterColorSkinIdAlias.Contains(id) : id.Contains(color.CharacterColorSkinIdAlias)) ?? skinColorPaletteSo;
	}

	public SkinColorPaletteScriptableObject CheckCompatibleSkinColor(SkinColorPaletteScriptableObject skinColorPaletteScriptableObject, bool returnDefaultValue = true)
	{
		SkinColorPaletteScriptableObject skinColorPaletteScriptableObject2 = null;
		if (UseColorCharacterPortrait)
		{
			skinColorPaletteScriptableObject2 = skinColorPaletteSoList.Find((SkinColorPaletteScriptableObject color) => color.CharacterColorSkinId == skinColorPaletteScriptableObject.CharacterColorSkinId);
			if (skinColorPaletteScriptableObject2 == null)
			{
				skinColorPaletteScriptableObject2 = skinColorPaletteSoList.Find((SkinColorPaletteScriptableObject color) => (color.CharacterColorSkinIdAlias.Length > skinColorPaletteScriptableObject.CharacterColorSkinIdAlias.Length) ? color.CharacterColorSkinIdAlias.Contains(skinColorPaletteScriptableObject.CharacterColorSkinIdAlias) : skinColorPaletteScriptableObject.CharacterColorSkinIdAlias.Contains(color.CharacterColorSkinIdAlias));
			}
		}
		else
		{
			skinColorPaletteScriptableObject2 = (headSkinColorPalette ? headSkinColorPalette : null);
		}
		if (skinColorPaletteScriptableObject2 == null)
		{
			skinColorPaletteScriptableObject2 = (returnDefaultValue ? skinColorPaletteSo : skinColorPaletteScriptableObject);
		}
		return skinColorPaletteScriptableObject2;
	}
}

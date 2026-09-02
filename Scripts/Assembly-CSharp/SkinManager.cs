using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using _Modules.CharacterSkin;
using _Modules.CharacterSkin.Scripts;

public class SkinManager : MonoBehaviour
{
	public CharacterSkinLibraryScriptableObject defaultHeroSkinLibraryScriptableObject;

	public CharacterSkinLibraryScriptableObject heroSkinLibraryScriptableObject;

	public CharacterSkinLibraryScriptableObject heroBodySkinLibraryScriptableObject;

	public CharacterColorPaletteLibraryScriptableObject heroColorPaletteLibraryScriptableObject;

	public CharacterSkinColorLibraryScriptableObject heroSkinColorLibraryScriptableObject;

	public AvatarSpiteLibraryScriptableObject avatarMaleLowerSkinLibraryScriptableObject;

	public AvatarSpiteLibraryScriptableObject avatarFemaleLowerSkinLibraryScriptableObject;

	[Header("Weapon Skin")]
	public List<SpriteLibraryAsset> listMeleeWeapon = new List<SpriteLibraryAsset>();

	public List<SpriteLibraryAsset> listRangeWeapon = new List<SpriteLibraryAsset>();

	[SerializeField]
	private List<SkinScriptableObject> _enemyType1SkinList = new List<SkinScriptableObject>();

	[SerializeField]
	private List<SkinScriptableObject> _enemySkinList = new List<SkinScriptableObject>();

	[SerializeField]
	public List<SO_ZombieSkinChange> ListSkinZombieModifier = new List<SO_ZombieSkinChange>();

	[Header("Weapon Skin")]
	public List<SpriteLibraryAsset> listWeaponEnemy0 = new List<SpriteLibraryAsset>();

	public static SkinManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	public CharacterSkinData GetDefaultHeroSkinByIndex(int index)
	{
		return defaultHeroSkinLibraryScriptableObject.GetDataByIndex(index)?.CharacterSkinData;
	}

	public CharacterSkinData GetHeroSkinByIndex(int index)
	{
		return heroSkinLibraryScriptableObject.GetDataByIndex(index)?.CharacterSkinData;
	}

	public CharacterSkinData GetHeroSkinById(string id)
	{
		return GetHeroSkinSOById(id)?.CharacterSkinData;
	}

	public SkinScriptableObject GetHeroSkinSOById(string id)
	{
		return heroSkinLibraryScriptableObject.GetData(id);
	}

	public CharacterSkinData GetHeroSkinBodyById(string id)
	{
		return GetHeroBodySkinSOById(id)?.CharacterSkinData;
	}

	public CharacterSkinData GetHeroBodySkinByIndex(int index)
	{
		return heroBodySkinLibraryScriptableObject.GetDataByIndex(index)?.CharacterSkinData;
	}

	public SkinScriptableObject GetHeroBodySkinSOById(string id)
	{
		return heroBodySkinLibraryScriptableObject.GetData(id);
	}

	public SkinScriptableObject GetHeroBodySkinSOByIndex(int index)
	{
		return heroBodySkinLibraryScriptableObject.GetDataByIndex(index);
	}

	public SkinColorScriptableObject GetHeroSkinColorSOById(string id)
	{
		return heroSkinColorLibraryScriptableObject.GetData(id);
	}

	public SkinColorScriptableObject GetHeroSkinColorSOByIndex(int index)
	{
		return heroSkinColorLibraryScriptableObject.GetDataByIndex(index);
	}

	public AvatarScriptableObject GetAvatarBodySkin(CharacterSkinData.Gender gender, SkinScriptableObject skinTypeSo, SkinColorPaletteScriptableObject skinColorPaletteScriptableObject)
	{
		AvatarSpiteLibraryScriptableObject avatarSpiteLibraryScriptableObject = null;
		switch (gender)
		{
		case CharacterSkinData.Gender.Male:
			avatarSpiteLibraryScriptableObject = avatarMaleLowerSkinLibraryScriptableObject;
			break;
		case CharacterSkinData.Gender.Female:
			avatarSpiteLibraryScriptableObject = avatarFemaleLowerSkinLibraryScriptableObject;
			break;
		}
		AvatarScriptableObject result = null;
		if (avatarSpiteLibraryScriptableObject != null)
		{
			result = avatarSpiteLibraryScriptableObject.GetData(skinTypeSo)?.GetData(skinColorPaletteScriptableObject) ?? avatarSpiteLibraryScriptableObject.GetData(skinTypeSo)?.GetData(skinTypeSo.CharacterSkinData.skinColorPaletteSo) ?? avatarSpiteLibraryScriptableObject.GetData(skinTypeSo)?.GetDataByIndex(0);
		}
		return result;
	}

	public SkinColorPaletteScriptableObject GetHeroColorPaletteByIndex(int index)
	{
		return heroColorPaletteLibraryScriptableObject.GetDataByIndex(index);
	}

	public SkinColorPaletteScriptableObject GetHeroColorPaletteById(string id)
	{
		SkinColorPaletteScriptableObject skinColorPaletteScriptableObject = heroColorPaletteLibraryScriptableObject.GetData(id);
		if (skinColorPaletteScriptableObject == null)
		{
			skinColorPaletteScriptableObject = heroSkinLibraryScriptableObject.GetData(id)?.CharacterSkinData.skinColorPaletteSo;
		}
		return skinColorPaletteScriptableObject;
	}

	public CharacterSkinData GetEnemySkinByType(int type = 0, int idxSeed = -1)
	{
		if (type == 0)
		{
			if (idxSeed != -1)
			{
				UnityEngine.Random.InitState(idxSeed);
			}
			int index = UnityEngine.Random.Range(0, _enemyType1SkinList.Count);
			UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
			return _enemyType1SkinList[index].CharacterSkinData;
		}
		return GetEnemySkin(type);
	}

	public CharacterSkinData GetEnemySkin(int index = -1)
	{
		int count = _enemySkinList.Count;
		if (index >= 0 && index < count)
		{
			return _enemySkinList[index].CharacterSkinData;
		}
		int index2 = UnityEngine.Random.Range(0, count);
		return _enemySkinList[index2].CharacterSkinData;
	}

	public SpriteLibraryAsset GetEnemyWeaponSkin(int index = -1)
	{
		int count = listWeaponEnemy0.Count;
		if (index >= 0 && index < count)
		{
			return listWeaponEnemy0[index];
		}
		int index2 = UnityEngine.Random.Range(0, count);
		return listWeaponEnemy0[index2];
	}

	public void AddListEnemySkin(SkinScriptableObject newSkin)
	{
		_enemySkinList.Add(newSkin);
	}

	public int GetTotalEnemySkin()
	{
		return _enemySkinList.Count;
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Modules.CharacterSkin.Scripts;

public class PlayerSkinData : MonoBehaviour
{
	[SerializeField]
	private string _headSkinId;

	private CharacterSkinData _headSkinData;

	private List<CharacterSkinData> _headUtilitySkinData = new List<CharacterSkinData>();

	private CharacterSkinData _headAccSkinAccData;

	private List<CharacterSkinData> _headAccUtilitySkinData = new List<CharacterSkinData>();

	[SerializeField]
	private string _bodySkinId;

	private CharacterSkinData _bodySkinData;

	private List<CharacterSkinData> _bodyUtilitySkinData = new List<CharacterSkinData>();

	private CharacterSkinData _bodySkinAccData;

	private List<CharacterSkinData> _bodyAccUtilitySkinData = new List<CharacterSkinData>();

	[SerializeField]
	private string _materialSkinId;

	private SkinColorPaletteScriptableObject _materialSkinData;

	[SerializeField]
	private string _skinColorId;

	private SkinColorScriptableObject _skinColorSo;

	[SerializeField]
	private CharacterSkinData.Gender _gender;

	private CharacterSkinData _genderSkinData;

	[NonSerialized]
	private PlayerController _playerController;

	public string HeadSkinId
	{
		get
		{
			if (string.IsNullOrWhiteSpace(_headSkinId))
			{
				_headSkinId = GetHeadSkin().CharacterSkinId;
			}
			return _headSkinId;
		}
		set
		{
			_headSkinId = value;
		}
	}

	public string BodySkinId
	{
		get
		{
			if (string.IsNullOrWhiteSpace(_bodySkinId))
			{
				_bodySkinId = GetBodySkin().CharacterSkinId;
			}
			return _bodySkinId;
		}
		set
		{
			_bodySkinId = value;
		}
	}

	public string MaterialSkinId
	{
		get
		{
			if (string.IsNullOrWhiteSpace(_materialSkinId))
			{
				_materialSkinId = GetMaterialSkin().CharacterColorSkinId;
			}
			return _materialSkinId;
		}
		set
		{
			_materialSkinId = value;
		}
	}

	public string SkinColorId
	{
		get
		{
			if (string.IsNullOrWhiteSpace(_skinColorId))
			{
				_skinColorId = GetSkinColorSo().SkinColorId;
			}
			return _skinColorId;
		}
		set
		{
			_skinColorId = value;
		}
	}

	public CharacterSkinData.Gender Gender
	{
		get
		{
			if (_gender == CharacterSkinData.Gender.Other)
			{
				_gender = GetHeadSkin().GetGender();
			}
			return _gender;
		}
		set
		{
			_gender = value;
		}
	}

	private PlayerController PlayerController
	{
		get
		{
			if (_playerController == null)
			{
				_playerController = GetComponent<PlayerController>();
			}
			return _playerController;
		}
	}

	public event Action<CharacterSkinData> OnHeadDataSkinChangedEvents;

	public event Action<CharacterSkinData> OnBodyDataSkinChangedEvents;

	public event Action<SkinColorPaletteScriptableObject> OnMaterialDataSkinChangedEvents;

	public event Action<SkinColorScriptableObject> OnSkinColorSkinChangedEvents;

	public void LoadSkinData(bool isInit = false)
	{
		if (isInit)
		{
			ResetSkinData();
		}
		SetHeadSkinData(GetHeadSkin(), executeEvent: false);
		SetBodySkinData(GetBodySkin(), executeEvent: false);
		SetMaterialSkinData(GetMaterialSkin(), executeEvent: false);
		SetGenderData(GetGenderCharacterData());
		SetSkinColorData(GetSkinColorSo(), executeEvent: false);
	}

	private void ResetSkinData()
	{
		_headSkinData = null;
		_bodySkinData = null;
		_materialSkinData = null;
		_genderSkinData = null;
		_headSkinId = "";
		_bodySkinId = "";
		_materialSkinId = "";
	}

	public void SetSkinData(CharacterSkinData characterSkinData)
	{
		SetHeadSkinData(characterSkinData);
		SetBodySkinData(characterSkinData);
		SetGenderData(characterSkinData);
		SetMaterialSkinData(characterSkinData.skinColorPaletteSo);
		SetSkinColorData(GetSkinColorSo());
	}

	public void SetHeadSkinData(CharacterSkinData characterSkinData, bool executeEvent = true)
	{
		HeadSkinId = characterSkinData.CharacterSkinId;
		_headSkinData = characterSkinData;
		OnHeadSkinIdChanged(_headSkinData);
		if (executeEvent)
		{
			OnHeadDataSkinChangedEvents?.Invoke(_headSkinData);
		}
	}

	public void SetBodySkinData(CharacterSkinData characterSkinData, bool executeEvent = true)
	{
		BodySkinId = characterSkinData.CharacterSkinId;
		_bodySkinData = characterSkinData;
		OnBodySkinIdChanged(_bodySkinData);
		if (executeEvent)
		{
			OnBodyDataSkinChangedEvents?.Invoke(_bodySkinData);
		}
	}

	public void SetGenderData(CharacterSkinData characterSkinData)
	{
		if (characterSkinData.skinGender == CharacterSkinData.Gender.Other)
		{
			_genderSkinData = SkinManager.Instance.GetDefaultHeroSkinByIndex(PlayerController.network.playerIdx);
			Gender = _genderSkinData.skinGender;
		}
		else
		{
			Gender = characterSkinData.skinGender;
			_genderSkinData = characterSkinData;
		}
	}

	public void SetMaterialSkinData(SkinColorPaletteScriptableObject characterSkinData, bool executeEvent = true)
	{
		MaterialSkinId = characterSkinData.CharacterColorSkinId;
		_materialSkinData = characterSkinData;
		OnMaterialSkinIdChanged(_materialSkinData);
		if (executeEvent)
		{
			OnMaterialDataSkinChangedEvents?.Invoke(_materialSkinData);
		}
	}

	public void SetSkinColorData(SkinColorScriptableObject skinColorSo, bool executeEvent = true)
	{
		SkinColorId = skinColorSo.SkinColorId;
		_skinColorSo = skinColorSo;
		OnSkinColorIdChanged(_skinColorSo);
		if (executeEvent)
		{
			OnSkinColorSkinChangedEvents?.Invoke(_skinColorSo);
		}
	}

	public string GetPlayerAvatarSkin()
	{
		if (_headSkinData != null)
		{
			if (_headSkinData.UseColorCharacterPortrait)
			{
				return _headSkinData.CharacterSkinId + "_" + _headSkinData.GetSkinColorPaletteById(MaterialSkinId).CharacterColorSkinId;
			}
			return _headSkinData.CharacterSkinId;
		}
		return $"Hero{PlayerController.network.playerIdx}";
	}

	public Sprite GetHeadSkinAvatar()
	{
		SkinScriptableObject skinScriptableObject = GetHeadSkinSo();
		Sprite sprite = ((!skinScriptableObject.CharacterSkinData.UseColorCharacterPortrait) ? skinScriptableObject.CharacterSkinData.CharacterPortraitSprite : skinScriptableObject.CharacterSkinData.GetCharacterSprite(skinScriptableObject, GetMaterialSkin()));
		if (sprite == null)
		{
			sprite = SkinManager.Instance.GetDefaultHeroSkinByIndex(PlayerController.network.playerIdx).CharacterPortraitSprite;
		}
		return sprite;
		SkinScriptableObject GetHeadSkinSo()
		{
			return SkinManager.Instance.GetHeroSkinSOById(HeadSkinId);
		}
	}

	public Sprite GetHeadSkinMiniAvatar()
	{
		SkinScriptableObject skinScriptableObject = GetHeadSkinSo();
		Sprite sprite = ((!skinScriptableObject.CharacterSkinData.UseColorCharacterPortrait) ? skinScriptableObject.CharacterSkinData.CharacterMiniPortraitSprite : skinScriptableObject.CharacterSkinData.GetMiniCharacterSprite(skinScriptableObject, GetMaterialSkin()));
		if (sprite == null)
		{
			sprite = SkinManager.Instance.GetDefaultHeroSkinByIndex(PlayerController.network.playerIdx).CharacterMiniPortraitSprite;
		}
		return sprite;
		SkinScriptableObject GetHeadSkinSo()
		{
			return SkinManager.Instance.GetHeroSkinSOById(HeadSkinId);
		}
	}

	public Sprite GetBodySkinAvatar()
	{
		return GetBodySkinAvatarSo().AvatarSprite;
	}

	public Sprite GetBodySkinMiniAvatar()
	{
		return GetBodySkinAvatarSo().MiniAvatarSprite;
	}

	private AvatarScriptableObject GetBodySkinAvatarSo()
	{
		return SkinManager.Instance.GetAvatarBodySkin(Gender, GetBodySkinSo(), GetMaterialSkin());
		SkinScriptableObject GetBodySkinSo()
		{
			return SkinManager.Instance.GetHeroBodySkinSOById(BodySkinId);
		}
	}

	public Material GetHeadSkinMaterial(SkinColorPaletteScriptableObject skinColorPaletteScriptableObject)
	{
		return GetHeadSkin().CheckCompatibleSkinColor(skinColorPaletteScriptableObject).CharacterColorSkinMaterial;
	}

	public CharacterSkinData GetHeadSkin()
	{
		if (_headSkinData != null)
		{
			return _headSkinData;
		}
		if (!string.IsNullOrWhiteSpace(_headSkinId))
		{
			_headSkinData = SkinManager.Instance.GetHeroSkinById(_headSkinId);
		}
		else
		{
			_headSkinData = SkinManager.Instance.GetDefaultHeroSkinByIndex(PlayerController.network.playerIdx);
		}
		return _headSkinData;
	}

	public Material GetHeadUtilitySkinMaterial(SkinColorPaletteScriptableObject skinColorPaletteScriptableObject)
	{
		return GetHeadUtilitySkin()?.CheckCompatibleSkinColor(skinColorPaletteScriptableObject).CharacterColorSkinMaterial;
	}

	public CharacterSkinData GetHeadUtilitySkin()
	{
		if (_headUtilitySkinData.Count <= 0)
		{
			return null;
		}
		return _headUtilitySkinData[0];
	}

	public CharacterSkinData GetHeadAccUtilitySkin()
	{
		if (_headAccUtilitySkinData.Count <= 0)
		{
			return null;
		}
		return _headAccUtilitySkinData[0];
	}

	public CharacterSkinData GetBodySkin()
	{
		if (_bodySkinData != null)
		{
			return _bodySkinData;
		}
		if (!string.IsNullOrWhiteSpace(_bodySkinId))
		{
			_bodySkinData = SkinManager.Instance.GetHeroSkinBodyById(_bodySkinId);
		}
		else
		{
			_bodySkinData = SkinManager.Instance.GetHeroBodySkinByIndex(0);
		}
		return _bodySkinData;
	}

	public Material GetBodySkinMaterial(SkinColorPaletteScriptableObject skinColorPaletteScriptableObject)
	{
		return GetHeadSkin().CheckCompatibleSkinColor(skinColorPaletteScriptableObject).CharacterColorSkinMaterial;
	}

	public Material GetBodyUtilitySkinMaterial(SkinColorPaletteScriptableObject skinColorPaletteScriptableObject)
	{
		return GetBodyUtilitySkin()?.CheckCompatibleSkinColor(skinColorPaletteScriptableObject).CharacterColorSkinMaterial;
	}

	public CharacterSkinData GetBodyUtilitySkin()
	{
		if (_bodyUtilitySkinData.Count <= 0)
		{
			return null;
		}
		return _bodyUtilitySkinData[0];
	}

	public CharacterSkinData GetBodyAccUtilitySkin()
	{
		if (_bodyAccUtilitySkinData.Count <= 0)
		{
			return null;
		}
		return _bodyAccUtilitySkinData[0];
	}

	public SkinColorPaletteScriptableObject GetMaterialSkin()
	{
		if (_materialSkinData != null)
		{
			return _materialSkinData;
		}
		if (!string.IsNullOrWhiteSpace(_materialSkinId))
		{
			_materialSkinData = GetBodySkin().GetSkinColorPaletteById(_materialSkinId);
		}
		else
		{
			_materialSkinData = GetBodySkin().skinColorPaletteSo;
		}
		return _materialSkinData;
	}

	public SkinColorScriptableObject GetSkinColorSo()
	{
		if (_skinColorSo != null)
		{
			return _skinColorSo;
		}
		if (!string.IsNullOrWhiteSpace(_skinColorId))
		{
			_skinColorSo = SkinManager.Instance.GetHeroSkinColorSOById(_skinColorId);
		}
		else
		{
			_skinColorSo = SkinManager.Instance.GetHeroSkinColorSOByIndex(0);
		}
		return _skinColorSo;
	}

	public CharacterSkinData GetGenderCharacterData()
	{
		if (_genderSkinData != null)
		{
			return _genderSkinData;
		}
		_genderSkinData = SkinManager.Instance.GetDefaultHeroSkinByIndex(PlayerController.network.playerIdx);
		return _genderSkinData;
	}

	private void OnHeadSkinIdChanged(CharacterSkinData characterSkinData)
	{
		SetGenderData(characterSkinData);
		SetHeadRender(characterSkinData);
		PlayerController.characterRenderController.ChangeSkinColor(GetSkinColorSo());
		PlayerController.ChangePlayerAvatar(this);
	}

	private void OnBodySkinIdChanged(CharacterSkinData characterSkinData)
	{
		SetBodyRender(characterSkinData);
		PlayerController.characterRenderController.ChangeSkinColor(GetSkinColorSo());
		PlayerController.ChangePlayerAvatar(this);
	}

	private void OnMaterialSkinIdChanged(SkinColorPaletteScriptableObject skinColorPaletteScriptableObject)
	{
		ChangeHeadSkinMaterial(skinColorPaletteScriptableObject);
		ChangeBodySkinMaterial(skinColorPaletteScriptableObject);
		PlayerController.characterRenderController.ChangeSkinColor(GetSkinColorSo());
		PlayerController.ChangePlayerAvatar(this);
	}

	private void OnSkinColorIdChanged(SkinColorScriptableObject skinColorObject)
	{
		PlayerController.characterRenderController.ChangeSkinColor(skinColorObject);
		PlayerController.ChangePlayerAvatar(this);
	}

	private void ChangeHeadSkinMaterial(SkinColorPaletteScriptableObject skinColorPaletteScriptableObject)
	{
		Material material = skinColorPaletteScriptableObject.CharacterColorSkinMaterial;
		Material material2 = ((GetHeadUtilitySkin() != null) ? GetHeadUtilitySkinMaterial(skinColorPaletteScriptableObject) : GetHeadSkinMaterial(skinColorPaletteScriptableObject));
		if (material2 != null)
		{
			material = material2;
		}
		PlayerController.characterRenderController.ChangeHeadMaterial(material);
	}

	public void SetHeadRender(CharacterSkinData characterSkinData, bool isUtility = false)
	{
		CharacterSkinData characterSkinData2 = characterSkinData;
		if (isUtility)
		{
			_headUtilitySkinData.Insert(0, characterSkinData);
		}
		else
		{
			CharacterSkinData headUtilitySkin = GetHeadUtilitySkin();
			if (headUtilitySkin != null)
			{
				characterSkinData2 = headUtilitySkin;
			}
		}
		PlayerController.characterRenderController.ChangeHeadSkin(characterSkinData2.headLibraryAsset);
		PlayerController.characterRenderController.ChangeHeadMaterial(characterSkinData2.CheckCompatibleSkinColor(GetMaterialSkin()).CharacterColorSkinMaterial);
	}

	public void SetHeadAccRender(CharacterSkinData characterSkinData, bool save = false)
	{
		if (save)
		{
			_headAccUtilitySkinData.Insert(0, characterSkinData);
		}
		PlayerController.characterRenderController.ChangeHeadAccSkin(characterSkinData?.headAccLibraryAsset);
	}

	public void ResetHeadRenderUtility(CharacterSkinData characterSkinData)
	{
		for (int i = 0; i < _headUtilitySkinData.Count; i++)
		{
			if (_headUtilitySkinData[i].CharacterSkinId == characterSkinData.CharacterSkinId)
			{
				_headUtilitySkinData.RemoveAt(i);
			}
		}
		CharacterSkinData headUtilitySkin = GetHeadUtilitySkin();
		if (headUtilitySkin != null)
		{
			SetHeadRender(headUtilitySkin);
		}
		else
		{
			SetHeadRender(GetHeadSkin());
		}
		PlayerController.characterRenderController.ChangeSkinColor(GetSkinColorSo());
	}

	public void ResetHeadAccRenderUtility(CharacterSkinData characterSkinData)
	{
		for (int i = 0; i < _headAccUtilitySkinData.Count; i++)
		{
			if (_headAccUtilitySkinData[i].CharacterSkinId == characterSkinData.CharacterSkinId)
			{
				_headAccUtilitySkinData.RemoveAt(i);
			}
		}
		SetHeadAccRender(GetHeadAccUtilitySkin());
	}

	private void ChangeBodySkinMaterial(SkinColorPaletteScriptableObject skinColorPaletteScriptableObject)
	{
		Material material = skinColorPaletteScriptableObject.CharacterColorSkinMaterial;
		Material material2 = ((GetBodyUtilitySkin() != null) ? GetBodyUtilitySkinMaterial(skinColorPaletteScriptableObject) : skinColorPaletteScriptableObject.CharacterColorSkinMaterial);
		if (material2 != null)
		{
			material = material2;
		}
		PlayerController.characterRenderController.ChangeUpperBodyMaterial(material);
		PlayerController.characterRenderController.ChangeLowerBodyMaterial(material);
	}

	public void SetBodyRender(CharacterSkinData characterSkinData, bool isUtility = false)
	{
		CharacterSkinData characterSkinData2 = characterSkinData;
		Material material = null;
		if (isUtility)
		{
			_bodyUtilitySkinData.Insert(0, characterSkinData);
			material = characterSkinData2.CheckCompatibleSkinColor(GetMaterialSkin()).CharacterColorSkinMaterial;
		}
		else
		{
			CharacterSkinData bodyUtilitySkin = GetBodyUtilitySkin();
			if (bodyUtilitySkin != null)
			{
				characterSkinData2 = bodyUtilitySkin;
				material = characterSkinData2.CheckCompatibleSkinColor(GetMaterialSkin()).CharacterColorSkinMaterial;
			}
		}
		if (material == null)
		{
			material = GetMaterialSkin().CharacterColorSkinMaterial;
		}
		PlayerController.characterRenderController.ChangeBodySkin(characterSkinData2.bodyLibraryAsset);
		PlayerController.characterRenderController.ChangeHandSkin(characterSkinData2.handLibraryAsset);
		PlayerController.characterRenderController.ChangeLegSkin(characterSkinData2.legLibraryAsset);
		PlayerController.characterRenderController.ChangeUpperBodyMaterial(material);
		PlayerController.characterRenderController.ChangeLowerBodyMaterial(material);
	}

	public void SetBodyAccRender(CharacterSkinData characterSkinData, bool save = false)
	{
		if (save)
		{
			_bodyAccUtilitySkinData.Insert(0, characterSkinData);
		}
		PlayerController.characterRenderController.ChangeBodyAccLibSkin(characterSkinData?.bodyAccLibraryAsset);
	}

	public void ResetBodyRenderUtility(CharacterSkinData characterSkinData)
	{
		for (int i = 0; i < _bodyUtilitySkinData.Count; i++)
		{
			if (_bodyUtilitySkinData[i].CharacterSkinId == characterSkinData.CharacterSkinId)
			{
				_bodyUtilitySkinData.RemoveAt(i);
			}
		}
		CharacterSkinData bodyUtilitySkin = GetBodyUtilitySkin();
		if (bodyUtilitySkin != null)
		{
			SetBodyRender(bodyUtilitySkin);
		}
		else
		{
			SetBodyRender(GetBodySkin());
		}
		PlayerController.characterRenderController.ChangeSkinColor(GetSkinColorSo());
	}

	public void ResetBodyAccRenderUtility(CharacterSkinData characterSkinData)
	{
		for (int i = 0; i < _bodyAccUtilitySkinData.Count; i++)
		{
			if (_bodyAccUtilitySkinData[i].CharacterSkinId == characterSkinData.CharacterSkinId)
			{
				_bodyAccUtilitySkinData.RemoveAt(i);
			}
		}
		SetBodyAccRender(GetBodyAccUtilitySkin());
	}

	public static Sprite GetHeadSkinAvatar(string headId, string materialSkinId, int indexPlayer)
	{
		SkinScriptableObject skinScriptableObject = GetHeadSkinSo();
		Sprite sprite = ((!skinScriptableObject.CharacterSkinData.UseColorCharacterPortrait) ? skinScriptableObject.CharacterSkinData.CharacterPortraitSprite : skinScriptableObject.CharacterSkinData.GetCharacterSprite(skinScriptableObject, GetMaterialSkin(materialSkinId, indexPlayer)));
		if (sprite == null)
		{
			sprite = SkinManager.Instance.GetDefaultHeroSkinByIndex(indexPlayer).CharacterPortraitSprite;
		}
		return sprite;
		SkinScriptableObject GetHeadSkinSo()
		{
			return SkinManager.Instance.GetHeroSkinSOById(headId);
		}
	}

	public static Sprite GetHeadSkinMiniAvatar(string id, string materialSkinId, int indexPlayer)
	{
		SkinScriptableObject skinScriptableObject = GetHeadSkinSo();
		Sprite sprite = ((!skinScriptableObject.CharacterSkinData.UseColorCharacterPortrait) ? skinScriptableObject.CharacterSkinData.CharacterMiniPortraitSprite : skinScriptableObject.CharacterSkinData.GetMiniCharacterSprite(skinScriptableObject, GetMaterialSkin(materialSkinId, indexPlayer)));
		if (sprite == null)
		{
			sprite = SkinManager.Instance.GetDefaultHeroSkinByIndex(indexPlayer).CharacterMiniPortraitSprite;
		}
		return sprite;
		SkinScriptableObject GetHeadSkinSo()
		{
			return SkinManager.Instance.GetHeroSkinSOById(id);
		}
	}

	public static AvatarScriptableObject GetBodySkinAvatarSo(CharacterSkinData.Gender gender, string bodySkinId, string materialSkinId, int indexPlayer)
	{
		return SkinManager.Instance.GetAvatarBodySkin(gender, GetBodySkinSo(), GetMaterialSkin(materialSkinId, indexPlayer));
		SkinScriptableObject GetBodySkinSo()
		{
			return SkinManager.Instance.GetHeroBodySkinSOById(bodySkinId) ?? SkinManager.Instance.GetHeroBodySkinSOByIndex(indexPlayer);
		}
	}

	public static SkinColorPaletteScriptableObject GetMaterialSkin(string materialSkinId, int indexPlayer)
	{
		if (!string.IsNullOrWhiteSpace(materialSkinId))
		{
			return SkinManager.Instance.GetHeroColorPaletteById(materialSkinId);
		}
		return SkinManager.Instance.GetDefaultHeroSkinByIndex(indexPlayer)?.skinColorPaletteSo;
	}
}

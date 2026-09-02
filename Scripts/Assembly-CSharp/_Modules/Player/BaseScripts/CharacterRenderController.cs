using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using _Modules.CharacterSkin;

namespace _Modules.Player.BaseScripts;

public class CharacterRenderController : MonoBehaviour
{
	[SerializeField]
	private Vector3 _showValue = Vector3.one;

	[SerializeField]
	private GameObject[] _characterRenderGameObject;

	[SerializeField]
	private Animator _characterAnimator;

	[SerializeField]
	private Animator _characterAnimator2;

	[SerializeField]
	protected SpriteRenderer[] allSpriteParts;

	[SerializeField]
	protected SpriteRenderer[] headParts;

	[SerializeField]
	protected SpriteRenderer[] upperParts;

	[SerializeField]
	protected SpriteRenderer[] lowerParts;

	[SerializeField]
	protected SpriteLibraryAsset headAccDefault;

	[SerializeField]
	protected SpriteLibraryAsset bodyAccDefault;

	[SerializeField]
	protected SpriteLibrary _headLib;

	[SerializeField]
	protected SpriteLibrary _headAccLib;

	[SerializeField]
	protected SpriteLibrary _bodyLib;

	[SerializeField]
	protected SpriteLibrary _bodyAccLib;

	[SerializeField]
	protected SpriteLibrary _hand1Lib;

	[SerializeField]
	protected SpriteLibrary _hand2Lib;

	[SerializeField]
	protected SpriteLibrary _hand2BLib;

	[SerializeField]
	protected SpriteLibrary _leg1Lib;

	[SerializeField]
	protected SpriteLibrary _leg2Lib;

	[SerializeField]
	protected SpriteLibrary _weaponLib;

	[SerializeField]
	protected SpriteLibrary _weapon2Lib;

	private readonly Vector3 _hideValue = Vector3.zero;

	public SpriteRenderer[] AllSpriteParts
	{
		get
		{
			return allSpriteParts;
		}
		set
		{
			allSpriteParts = value;
		}
	}

	public SpriteRenderer[] UpperParts
	{
		get
		{
			return upperParts;
		}
		set
		{
			upperParts = value;
		}
	}

	public SpriteLibrary HeadLib
	{
		get
		{
			return _headLib;
		}
		set
		{
			_headLib = value;
		}
	}

	public SpriteLibrary HeadAccLib
	{
		get
		{
			return _headAccLib;
		}
		set
		{
			_headAccLib = value;
		}
	}

	public SpriteLibrary BodyLib
	{
		get
		{
			return _bodyLib;
		}
		set
		{
			_bodyLib = value;
		}
	}

	public SpriteLibrary BodyAccLib
	{
		get
		{
			return _bodyAccLib;
		}
		set
		{
			_bodyAccLib = value;
		}
	}

	public SpriteLibrary Hand1Lib
	{
		get
		{
			return _hand1Lib;
		}
		set
		{
			_hand1Lib = value;
		}
	}

	public SpriteLibrary Hand2Lib
	{
		get
		{
			return _hand2Lib;
		}
		set
		{
			_hand2Lib = value;
		}
	}

	public SpriteLibrary Hand2BLib
	{
		get
		{
			return _hand2BLib;
		}
		set
		{
			_hand2BLib = value;
		}
	}

	public SpriteLibrary Leg1Lib
	{
		get
		{
			return _leg1Lib;
		}
		set
		{
			_leg1Lib = value;
		}
	}

	public SpriteLibrary Leg2Lib
	{
		get
		{
			return _leg2Lib;
		}
		set
		{
			_leg2Lib = value;
		}
	}

	public SpriteLibrary WeaponLib
	{
		get
		{
			return _weaponLib;
		}
		set
		{
			_weaponLib = value;
		}
	}

	public SpriteLibrary Weapon2Lib
	{
		get
		{
			return _weapon2Lib;
		}
		set
		{
			_weapon2Lib = value;
		}
	}

	public Vector3 HideValue => _hideValue;

	public Animator CharacterAnimator => _characterAnimator;

	public Animator CharacterAnimator2 => _characterAnimator2;

	public Material GetHeadMaterial => headParts[^1]?.material;

	public Material GetUpperBodyMaterial => upperParts[^1]?.material;

	public Material GetLowerBodyMaterial => lowerParts[^1]?.material;

	public virtual void ShowCharacter()
	{
		SetActiveCharacterRender(setActive: true);
	}

	public virtual void HideCharacter()
	{
		SetActiveCharacterRender(setActive: false);
	}

	protected void SetActiveCharacterRender(bool setActive)
	{
		for (int i = 0; i < _characterRenderGameObject.Length; i++)
		{
			if (setActive)
			{
				_characterRenderGameObject[i].transform.localScale = _showValue;
			}
			else
			{
				_characterRenderGameObject[i].transform.localScale = _hideValue;
			}
		}
	}

	public virtual void ChangeSkin(CharacterSkinData characterSkinData)
	{
		ChangeHeadSkin(characterSkinData.headLibraryAsset);
		ChangeHeadAccSkin(characterSkinData.headAccLibraryAsset);
		ChangeBodySkin(characterSkinData.bodyLibraryAsset);
		ChangeBodyAccLibSkin(characterSkinData.bodyAccLibraryAsset);
		ChangeHandSkin(characterSkinData.handLibraryAsset);
		ChangeHand2Skin(characterSkinData.hand2LibraryAsset);
		ChangeLegSkin(characterSkinData.legLibraryAsset);
		ChangeLeg2Skin(characterSkinData.leg2LibraryAsset);
	}

	public void ChangeHeadSkin(SpriteLibraryAsset spriteLibraryAsset)
	{
		if ((bool)_headLib)
		{
			_headLib.spriteLibraryAsset = spriteLibraryAsset;
		}
	}

	public void ChangeHeadAccSkin(SpriteLibraryAsset spriteLibraryAsset)
	{
		if ((bool)_headAccLib)
		{
			_headAccLib.spriteLibraryAsset = spriteLibraryAsset ?? headAccDefault;
		}
	}

	public void ChangeBodySkin(SpriteLibraryAsset spriteLibraryAsset)
	{
		if ((bool)_bodyLib)
		{
			_bodyLib.spriteLibraryAsset = spriteLibraryAsset;
		}
	}

	public void ChangeBodyAccLibSkin(SpriteLibraryAsset spriteLibraryAsset)
	{
		if ((bool)_bodyAccLib)
		{
			_bodyAccLib.spriteLibraryAsset = spriteLibraryAsset ?? bodyAccDefault;
		}
	}

	public virtual void ChangeHandSkin(SpriteLibraryAsset spriteLibraryAsset)
	{
		if ((bool)_hand1Lib)
		{
			_hand1Lib.spriteLibraryAsset = spriteLibraryAsset;
		}
		if ((bool)_hand2BLib)
		{
			_hand2BLib.spriteLibraryAsset = spriteLibraryAsset;
		}
	}

	public void ChangeHand2Skin(SpriteLibraryAsset spriteLibraryAsset)
	{
		if ((bool)_hand2Lib)
		{
			_hand2Lib.spriteLibraryAsset = spriteLibraryAsset;
		}
	}

	public virtual void ChangeLegSkin(SpriteLibraryAsset spriteLibraryAsset)
	{
		if ((bool)_leg1Lib)
		{
			_leg1Lib.spriteLibraryAsset = spriteLibraryAsset;
		}
	}

	public void ChangeLeg2Skin(SpriteLibraryAsset spriteLibraryAsset)
	{
		if ((bool)_leg2Lib)
		{
			_leg2Lib.spriteLibraryAsset = spriteLibraryAsset;
		}
	}

	public void ChangeWeaponSkin(SpriteLibraryAsset spriteLibraryAsset)
	{
		if ((bool)_weaponLib)
		{
			_weaponLib.spriteLibraryAsset = spriteLibraryAsset;
		}
	}

	public void ChangeWeapon2Skin(SpriteLibraryAsset spriteLibraryAsset)
	{
		if ((bool)_weapon2Lib)
		{
			_weapon2Lib.spriteLibraryAsset = spriteLibraryAsset;
		}
	}

	public void SetHeadMaterial(Material material)
	{
		SpriteRenderer[] array = headParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].material = material;
		}
	}

	public void SetUpperBodyMaterial(Material material)
	{
		SpriteRenderer[] array = upperParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].material = material;
		}
	}

	public void SetLowerBodyMaterial(Material material)
	{
		SpriteRenderer[] array = lowerParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].material = material;
		}
	}

	public void ChangeHeadMaterial(Material material)
	{
		SpriteRenderer[] array = headParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].material.CopyMatchingPropertiesFromMaterial(material);
		}
	}

	public void ChangeUpperBodyMaterial(Material material)
	{
		SpriteRenderer[] array = upperParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].material.CopyMatchingPropertiesFromMaterial(material);
		}
	}

	public void ChangeLowerBodyMaterial(Material material)
	{
		SpriteRenderer[] array = lowerParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].material.CopyMatchingPropertiesFromMaterial(material);
		}
	}

	public void ChangeAllMaterial(Material material)
	{
		SpriteRenderer[] array = allSpriteParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].material.CopyMatchingPropertiesFromMaterial(material);
		}
	}

	public void ChangeColorHeadParts(Color color)
	{
		SpriteRenderer[] array = headParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].color = color;
		}
	}

	public void ChangeColorBodyParts(Color color)
	{
		SpriteRenderer[] array = upperParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].color = color;
		}
	}

	public void ChangeColorLowerParts(Color color)
	{
		SpriteRenderer[] array = lowerParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].color = color;
		}
	}

	public void ChangeSkinColor(SkinColorScriptableObject skinColorSo)
	{
		if (skinColorSo == null)
		{
			return;
		}
		SpriteRenderer[] array = allSpriteParts;
		for (int i = 0; i < array.Length; i++)
		{
			Material material = array[i].material;
			foreach (KeyValuePair<string, Color> item in skinColorSo.SkinColorDataDict)
			{
				material.SetColor("_" + item.Key, item.Value);
			}
		}
	}

	public void ChangeNVGEnabled(bool enabled)
	{
		SpriteRenderer[] array = allSpriteParts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].material.SetFloat("_NVGEnabled", enabled ? 1 : 0);
		}
		SpriteRenderer component = _weaponLib.GetComponent<SpriteRenderer>();
		if ((bool)component)
		{
			component.material.SetFloat("_NVGEnabled", enabled ? 1 : 0);
		}
	}
}

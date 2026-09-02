using System;
using DG.Tweening;
using Doozy.Runtime.UIManager.Components;
using Toked;
using UnityEngine;
using _Modules.Player.BaseScripts;
using _Modules.UIInGame.Scripts;

namespace _Modules.CharacterSkin.Scripts;

public class CharacterSkinPreviewUI : MonoBehaviour
{
	private enum Direction
	{
		Front = 0,
		FrontRight = 1,
		Right = 2,
		BackRight = 3,
		Back = 4,
		BackLeft = 5,
		Left = 6,
		FrontLeft = 7
	}

	[SerializeField]
	private Transform _characterPreviewRenderGoTransform;

	[SerializeField]
	private CharacterRenderController _characterPreview;

	[SerializeField]
	private CharacterAvatarUIController _characterAvatarPreview;

	[SerializeField]
	private UIButton _leftButton;

	[SerializeField]
	private UIButton _rightButton;

	[SerializeField]
	private RectTransform _cursorRectTransform;

	[SerializeField]
	private Color _lockColorCharacterPreview = Color.grey;

	[SerializeField]
	private Color _unlockColorCharacterPreview = Color.white;

	private int _indexButton;

	private int _directionLength = 4;

	public void Init()
	{
		_directionLength = Enum.GetNames(typeof(Direction)).Length;
		_leftButton.onClickEvent.AddListener(LeftButtonClick);
		_rightButton.onClickEvent.AddListener(RightButtonClick);
		CameraGame.Instance.OnCameraRotateEvent += OnRotateInGameCameraAction;
	}

	private void OnDestroy()
	{
		CameraGame.Instance.OnCameraRotateEvent -= OnRotateInGameCameraAction;
	}

	public void UpdateFunction()
	{
		if (InputManager.inputActions.CharacterCustomize.RotateRight.WasPressedThisFrame())
		{
			InvokeRightButton();
		}
		else if (InputManager.inputActions.CharacterCustomize.RotateLeft.WasPressedThisFrame())
		{
			InvokeLeftButton();
		}
	}

	public void Show()
	{
		_characterPreview.gameObject.SetActive(value: true);
		_characterPreview.CharacterAnimator.enabled = true;
		ResetAnimation();
	}

	public void Close()
	{
		_characterPreview.gameObject.SetActive(value: false);
		_characterPreview.CharacterAnimator.enabled = false;
	}

	public void ChangeHeadPreview(CharacterSkinData headSkinData, CharacterSkinData bodySkinData, SkinColorPaletteScriptableObject skinColorPaletteScriptable, SkinColorScriptableObject skinColorScriptable)
	{
		_characterPreview.ChangeHeadMaterial(headSkinData.CheckCompatibleSkinColor(skinColorPaletteScriptable).CharacterColorSkinMaterial);
		_characterPreview.ChangeHeadSkin(headSkinData.headLibraryAsset);
		_characterPreview.ChangeSkinColor(skinColorScriptable);
		_characterPreview.ChangeNVGEnabled(enabled: false);
		SetHeadAvatar(headSkinData, skinColorPaletteScriptable);
		SetBodyAvatar(headSkinData, bodySkinData, skinColorPaletteScriptable);
	}

	public void ChangeBodyPreview(CharacterSkinData headSkinData, CharacterSkinData bodySkinData, SkinColorPaletteScriptableObject skinColorPaletteScriptable, SkinColorScriptableObject skinColorScriptable)
	{
		_characterPreview.ChangeHeadMaterial(headSkinData.CheckCompatibleSkinColor(skinColorPaletteScriptable).CharacterColorSkinMaterial);
		_characterPreview.ChangeBodySkin(bodySkinData.bodyLibraryAsset);
		_characterPreview.ChangeHandSkin(bodySkinData.handLibraryAsset);
		_characterPreview.ChangeLegSkin(bodySkinData.legLibraryAsset);
		_characterPreview.ChangeSkinColor(skinColorScriptable);
		_characterPreview.ChangeNVGEnabled(enabled: false);
		SetHeadAvatar(headSkinData, skinColorPaletteScriptable);
		SetBodyAvatar(headSkinData, bodySkinData, skinColorPaletteScriptable);
	}

	public void ChangeColorPreview(CharacterSkinData headSkinData, CharacterSkinData bodySkinData, SkinColorPaletteScriptableObject skinColorPaletteScriptable, SkinColorScriptableObject skinColorScriptable)
	{
		_characterPreview.ChangeHeadMaterial(headSkinData.CheckCompatibleSkinColor(skinColorPaletteScriptable).CharacterColorSkinMaterial);
		_characterPreview.ChangeUpperBodyMaterial(skinColorPaletteScriptable.CharacterColorSkinMaterial);
		_characterPreview.ChangeLowerBodyMaterial(skinColorPaletteScriptable.CharacterColorSkinMaterial);
		_characterPreview.ChangeSkinColor(skinColorScriptable);
		_characterPreview.ChangeNVGEnabled(enabled: false);
		SetHeadAvatar(headSkinData, skinColorPaletteScriptable);
		SetBodyAvatar(headSkinData, bodySkinData, skinColorPaletteScriptable);
	}

	public void ChangeSkinColorPreview(CharacterSkinData headSkinData, CharacterSkinData bodySkinData, SkinColorPaletteScriptableObject skinColorPaletteScriptable, SkinColorScriptableObject skinColorScriptable)
	{
		_characterPreview.ChangeHeadMaterial(headSkinData.CheckCompatibleSkinColor(skinColorPaletteScriptable).CharacterColorSkinMaterial);
		_characterPreview.ChangeUpperBodyMaterial(skinColorPaletteScriptable.CharacterColorSkinMaterial);
		_characterPreview.ChangeLowerBodyMaterial(skinColorPaletteScriptable.CharacterColorSkinMaterial);
		_characterPreview.ChangeSkinColor(skinColorScriptable);
		_characterPreview.ChangeNVGEnabled(enabled: false);
		SetHeadAvatar(headSkinData, skinColorPaletteScriptable);
		SetBodyAvatar(headSkinData, bodySkinData, skinColorPaletteScriptable);
	}

	public void RefreshWeapon(PlayerController playerController)
	{
		_characterPreview.ChangeWeaponSkin(playerController.characterRenderController.WeaponLib.spriteLibraryAsset);
	}

	private void SetHeadAvatar(CharacterSkinData headSkinData, SkinColorPaletteScriptableObject skinColorPaletteScriptable)
	{
		SkinScriptableObject heroSkinSOById = SkinManager.Instance.GetHeroSkinSOById(headSkinData.CharacterSkinId);
		Sprite sprite = (heroSkinSOById.CharacterSkinData.UseColorCharacterPortrait ? headSkinData.GetCharacterSprite(heroSkinSOById, skinColorPaletteScriptable) : heroSkinSOById.CharacterSkinData.CharacterPortraitSprite);
		_characterAvatarPreview.ChangeHeadAvatarSprite(sprite);
	}

	private void SetBodyAvatar(CharacterSkinData headSkinData, CharacterSkinData bodySkinData, SkinColorPaletteScriptableObject skinColorPaletteScriptable)
	{
		SkinScriptableObject heroBodySkinSOById = SkinManager.Instance.GetHeroBodySkinSOById(bodySkinData.CharacterSkinId);
		Sprite avatarSprite = SkinManager.Instance.GetAvatarBodySkin(headSkinData.GetGender(), heroBodySkinSOById, skinColorPaletteScriptable).AvatarSprite;
		_characterAvatarPreview.ChangeBodyAvatarSprite(avatarSprite);
	}

	public void InvokeLeftButton()
	{
		_leftButton.onClickEvent.Invoke();
	}

	public void InvokeRightButton()
	{
		_rightButton.onClickEvent.Invoke();
	}

	private void ResetAnimation()
	{
		RotateAnimation(Direction.Front);
		_indexButton = 0;
	}

	private void RotateAnimation(Direction direction)
	{
		switch (direction)
		{
		case Direction.Front:
			ChangeAnimation(180);
			break;
		case Direction.FrontRight:
			ChangeAnimation(135);
			break;
		case Direction.Right:
			ChangeAnimation(90);
			break;
		case Direction.BackRight:
			ChangeAnimation(45);
			break;
		case Direction.Back:
			ChangeAnimation(0);
			break;
		case Direction.BackLeft:
			ChangeAnimation(315);
			break;
		case Direction.Left:
			ChangeAnimation(270);
			break;
		case Direction.FrontLeft:
			ChangeAnimation(225);
			break;
		default:
			throw new ArgumentOutOfRangeException("direction", direction, null);
		}
		void ChangeAnimation(int angle)
		{
			_characterPreview.CharacterAnimator.Play("IdleMelee" + angle);
			_characterPreview.CharacterAnimator2.Play("LegIdle" + angle);
			Vector3 eulerAngles = _cursorRectTransform.eulerAngles;
			_cursorRectTransform.DOLocalRotate(new Vector3(eulerAngles.x, eulerAngles.y, 180 - angle), 0.1f);
		}
	}

	private void LeftButtonClick()
	{
		_indexButton--;
		if (_indexButton < 0)
		{
			_indexButton = _directionLength - 1;
		}
		RotateAnimation((Direction)_indexButton);
		AudioManager.PlaySFX("ui_select");
	}

	private void RightButtonClick()
	{
		_indexButton++;
		if (_indexButton >= _directionLength)
		{
			_indexButton = 0;
		}
		RotateAnimation((Direction)_indexButton);
		AudioManager.PlaySFX("ui_select");
	}

	private void OnRotateInGameCameraAction(int cameraValue)
	{
		if (_characterPreviewRenderGoTransform != null)
		{
			Vector3 eulerAngles = _characterPreviewRenderGoTransform.eulerAngles;
			_characterPreviewRenderGoTransform.eulerAngles = new Vector3(eulerAngles.x, cameraValue - 45, eulerAngles.z);
		}
	}

	public void LockHeadPreview()
	{
		_characterPreview.ChangeColorHeadParts(_lockColorCharacterPreview);
		_characterAvatarPreview.ChangeColorHeadUI(_lockColorCharacterPreview);
	}

	public void UnlockHeadPreview()
	{
		_characterPreview.ChangeColorHeadParts(_unlockColorCharacterPreview);
		_characterAvatarPreview.ChangeColorHeadUI(_unlockColorCharacterPreview);
	}

	public void LockBodyPreview()
	{
		_characterPreview.ChangeColorBodyParts(_lockColorCharacterPreview);
		_characterPreview.ChangeColorLowerParts(_lockColorCharacterPreview);
		_characterAvatarPreview.ChangeColorLowerUI(_lockColorCharacterPreview);
	}

	public void UnlockBodyPreview()
	{
		_characterPreview.ChangeColorBodyParts(_unlockColorCharacterPreview);
		_characterPreview.ChangeColorLowerParts(_unlockColorCharacterPreview);
		_characterAvatarPreview.ChangeColorLowerUI(_unlockColorCharacterPreview);
	}

	public void SetPreviewUI(bool isUnlockUI)
	{
		if (isUnlockUI)
		{
			UnlockAllPreview();
		}
		else
		{
			LockAllPreview();
		}
	}

	public void LockAllPreview()
	{
		LockHeadPreview();
		LockBodyPreview();
	}

	public void UnlockAllPreview()
	{
		UnlockHeadPreview();
		UnlockBodyPreview();
	}
}

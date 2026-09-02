using System.Collections.Generic;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using TMPro;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Modules.CharacterSkin.Scripts;

public class SkinControllerUI : MonoBehaviour
{
	[SerializeField]
	private CharacterSkinChanger _characterSkinChanger;

	[SerializeField]
	private UIView _uiView;

	[SerializeField]
	private CharacterSkinPreviewUI _characterSkinPreviewUI;

	[SerializeField]
	private SkinChangerPanelUI _headPanelUI;

	[SerializeField]
	private SkinChangerPanelUI _bodyPanelUI;

	[SerializeField]
	private SkinChangerPanelUI _colorPanelUI;

	[SerializeField]
	private SkinChangerPanelUI _skinColorPanelUI;

	[SerializeField]
	private UIButton _backButton;

	[SerializeField]
	private UIButton _confirmButton;

	[SerializeField]
	private TMP_Text _fullGameOnlyText;

	private List<SkinScriptableObject> _headSkinDataList = new List<SkinScriptableObject>();

	private List<SkinScriptableObject> _bodySkinDataList = new List<SkinScriptableObject>();

	private List<SkinColorPaletteScriptableObject> _skinColorPaletteDataList = new List<SkinColorPaletteScriptableObject>();

	private List<SkinColorScriptableObject> _skinColorDataList = new List<SkinColorScriptableObject>();

	private int _headSkinCurrentIndex;

	private int _bodySkinCurrentIndex;

	private int _skinColorPaletteCurrentIndex;

	private int _skinColorCurrentIndex;

	private PlayerController _playerController;

	private bool _isShow;

	private const string SKIN_NAME_TERM = "Customize/";

	private void Start()
	{
		foreach (SkinScriptableObject data in _characterSkinChanger.CharacterSkinLibrary.DataList)
		{
			if ((bool)data.CharacterSkinData.headLibraryAsset)
			{
				_headSkinDataList.Add(data);
			}
		}
		foreach (SkinScriptableObject data2 in _characterSkinChanger.CharacterBodyLibraryScriptableObject.DataList)
		{
			if ((bool)data2.CharacterSkinData.bodyLibraryAsset)
			{
				_bodySkinDataList.Add(data2);
			}
		}
		foreach (SkinColorScriptableObject data3 in _characterSkinChanger.CharacterSkinColorLibrary.DataList)
		{
			if ((bool)data3)
			{
				_skinColorDataList.Add(data3);
			}
		}
		_headPanelUI.InitButton(PrevHeadButtonClick, NextHeadButtonClick, DeselectAllButton);
		_bodyPanelUI.InitButton(PrevBodyButtonClick, NextBodyButtonClick, DeselectAllButton);
		_colorPanelUI.InitButton(PrevColorButtonClick, NextColorButtonClick, DeselectAllButton);
		_skinColorPanelUI.InitButton(PrevSkinColorButtonClick, NextSkinColorButtonClick, DeselectAllButton);
		_uiView.OnShowCallback.Event.AddListener(RefreshUI);
		_uiView.OnHideCallback.Event.AddListener(CloseUI);
		_backButton.onClickEvent.AddListener(CloseUI);
		_confirmButton.onClickEvent.AddListener(ConfirmSkin);
		_characterSkinPreviewUI.Init();
	}

	private void Update()
	{
		if (_isShow)
		{
			_characterSkinPreviewUI.UpdateFunction();
			_headPanelUI.UpdateFunction();
			_bodyPanelUI.UpdateFunction();
			_colorPanelUI.UpdateFunction();
			_skinColorPanelUI.UpdateFunction();
			OnInput();
		}
	}

	private void RefreshUI()
	{
		if (_playerController == null)
		{
			_playerController = NetworkGameManager.Instance.ownPlayer;
		}
		_characterSkinPreviewUI.RefreshWeapon(_playerController);
		(int, SkinScriptableObject) headSkin = GetHeadSkin(_playerController.data.PlayerSkinData?.HeadSkinId);
		_headSkinCurrentIndex = headSkin.Item1;
		(int, SkinScriptableObject) bodySkin = GetBodySkin(_playerController.data.PlayerSkinData?.BodySkinId);
		RefreshSkinColorPaletteList(bodySkin.Item2);
		_bodySkinCurrentIndex = bodySkin.Item1;
		(int, SkinColorPaletteScriptableObject) skinColorPalette = GetSkinColorPalette(_playerController.data.PlayerSkinData?.MaterialSkinId);
		_skinColorPaletteCurrentIndex = skinColorPalette.Item1;
		(int, SkinColorScriptableObject) skinColor = GetSkinColor(_playerController.data.PlayerSkinData?.SkinColorId);
		(_skinColorCurrentIndex, _) = skinColor;
		ChangeHeadPreview(headSkin.Item2);
		ChangeBodyPreview(bodySkin.Item2);
		ChangeColorPalettePreview(skinColorPalette.Item2);
		ChangeSkinColorPreview(skinColor.Item2);
		SelectFirstButton();
	}

	private void ConfirmSkin()
	{
		if (_confirmButton.interactable)
		{
			if (_playerController == null)
			{
				_playerController = NetworkGameManager.Instance.ownPlayer;
			}
			SkinScriptableObject currentHeadSkinSo = GetCurrentHeadSkinSo();
			_characterSkinChanger.ChangeHeadSkin(_playerController, currentHeadSkinSo);
			SkinScriptableObject currentBodySkinSo = GetCurrentBodySkinSo();
			_characterSkinChanger.ChangeBodySkin(_playerController, currentBodySkinSo);
			SkinColorPaletteScriptableObject colorPaletteSo = GetColorPaletteSo(_skinColorPaletteCurrentIndex);
			_characterSkinChanger.ChangeMaterialSkin(_playerController, colorPaletteSo);
			SkinColorScriptableObject skinColorSo = GetSkinColorSo(_skinColorCurrentIndex);
			_characterSkinChanger.ChangeSkinColor(_playerController, skinColorSo);
			CloseUI();
		}
	}

	public void ShowUI()
	{
		RefreshUI();
		_characterSkinPreviewUI.Show();
		InputManager.ToggleActionMap(InputManager.PlayerInputToggleAction.DISABLE_PLAYER_INPUT, InputManager.inputActions.UI, InputManager.inputActions.CharacterCustomize);
		_isShow = true;
	}

	public void CloseUI()
	{
		UIGameManager.Instance.BackToInGame(_uiView);
		_characterSkinPreviewUI.Close();
		InputManager.ToggleActionMap(InputManager.PlayerInputToggleAction.ENABLE_PLAYER_INPUT, InputManager.inputActions.UI, InputManager.inputActions.Player);
		_isShow = false;
		AudioManager.PlaySFX("ui_cancel");
		if ((bool)NetworkGameManager.Instance.ownPlayer)
		{
			NetworkGameManager.Instance.ownPlayer.DelayInputTimer.StartDuration(0.5f);
		}
		if (UIGameManager.Instance.uiObjective != null)
		{
			UIGameManager.Instance.uiObjective.SetActive(value: true);
		}
	}

	private (int Index, SkinScriptableObject Data) GetHeadSkin(string skinId)
	{
		if (!string.IsNullOrWhiteSpace(skinId))
		{
			return GetSkin(_headSkinDataList, skinId);
		}
		return (Index: 0, Data: _headSkinDataList[0]);
	}

	private (int Index, SkinScriptableObject Data) GetBodySkin(string skinId)
	{
		if (!string.IsNullOrWhiteSpace(skinId))
		{
			return GetSkin(_bodySkinDataList, skinId);
		}
		return (Index: 0, Data: _bodySkinDataList[0]);
	}

	private (int Index, SkinScriptableObject Data) GetSkin(List<SkinScriptableObject> data, string id)
	{
		for (int i = 0; i < data.Count; i++)
		{
			if (data[i]?.CharacterSkinId == id)
			{
				return (Index: i, Data: data[i]);
			}
		}
		return (Index: 0, Data: data[0]);
	}

	private (int Index, SkinColorPaletteScriptableObject Data) GetSkinColorPalette(string id)
	{
		for (int i = 0; i < _skinColorPaletteDataList.Count; i++)
		{
			if (_skinColorPaletteDataList[i]?.CharacterColorSkinId == id)
			{
				return (Index: i, Data: _skinColorPaletteDataList[i]);
			}
		}
		return (Index: 0, Data: _skinColorPaletteDataList[0]);
	}

	private (int Index, SkinColorScriptableObject Data) GetSkinColor(string id)
	{
		for (int i = 0; i < _skinColorDataList.Count; i++)
		{
			if (_skinColorDataList[i]?.SkinColorId == id)
			{
				return (Index: i, Data: _skinColorDataList[i]);
			}
		}
		return (Index: 0, Data: _skinColorDataList[0]);
	}

	private void RefreshSkinColorPaletteList(SkinScriptableObject characterSkinSo)
	{
		_skinColorPaletteDataList.Clear();
		foreach (SkinColorPaletteScriptableObject skinColorPaletteSo in characterSkinSo.CharacterSkinData.skinColorPaletteSoList)
		{
			if (!_skinColorPaletteDataList.Contains(skinColorPaletteSo))
			{
				_skinColorPaletteDataList.Add(skinColorPaletteSo);
			}
		}
		int count = _skinColorPaletteDataList.Count;
		if (count == 0)
		{
			_skinColorPaletteDataList.Add(characterSkinSo.CharacterSkinData.skinColorPaletteSo ?? _characterSkinChanger.CharacterColorPaletteLibrary.GetDataByIndex(0));
		}
		if (_skinColorPaletteCurrentIndex >= count)
		{
			_skinColorPaletteCurrentIndex = 0;
		}
	}

	private void ChangeHeadPreview(SkinScriptableObject characterSkinSo)
	{
		CharacterSkinData characterSkinData = characterSkinSo.CharacterSkinData;
		AudioManager.PlaySFX("ui_select");
		_characterSkinPreviewUI.ChangeHeadPreview(characterSkinData, GetCurrentBodySkinSo().CharacterSkinData, GetColorPaletteSo(_skinColorPaletteCurrentIndex), GetSkinColorSo(_skinColorCurrentIndex));
		_headPanelUI.SetLocalizeTerm(characterSkinData.CharacterSkinNameLocalize);
		OnHeadValueChanged(characterSkinSo.CheckRequirementUnlock());
		void OnHeadValueChanged(bool isValid)
		{
			if (isValid)
			{
				_headPanelUI.ChangeTextColor(Color.white);
			}
			else
			{
				_headPanelUI.ChangeTextColor(Color.red);
			}
			OnValuePanelChanged();
		}
	}

	private void ChangeBodyPreview(SkinScriptableObject characterSkinSo)
	{
		CharacterSkinData characterSkinData = characterSkinSo.CharacterSkinData;
		AudioManager.PlaySFX("ui_select");
		_characterSkinPreviewUI.ChangeBodyPreview(GetCurrentHeadSkinSo().CharacterSkinData, characterSkinData, GetColorPaletteSo(_skinColorPaletteCurrentIndex), GetSkinColorSo(_skinColorCurrentIndex));
		_bodyPanelUI.SetLocalizeTerm(characterSkinData.CharacterSkinNameLocalize);
		OnBodyValueChanged(characterSkinSo.CheckRequirementUnlock());
		void OnBodyValueChanged(bool isValid)
		{
			if (isValid)
			{
				_bodyPanelUI.ChangeTextColor(Color.white);
			}
			else
			{
				_bodyPanelUI.ChangeTextColor(Color.red);
			}
			OnValuePanelChanged();
		}
	}

	private void ChangeColorPalettePreview(SkinColorPaletteScriptableObject skinColorPaletteScriptable, bool showColorName = false)
	{
		AudioManager.PlaySFX("ui_select");
		CharacterSkinData characterSkinData = GetCurrentHeadSkinSo().CharacterSkinData;
		CharacterSkinData characterSkinData2 = GetCurrentBodySkinSo().CharacterSkinData;
		SkinColorScriptableObject skinColorScriptable = _skinColorDataList[_skinColorCurrentIndex];
		_characterSkinPreviewUI.ChangeColorPreview(characterSkinData, characterSkinData2, skinColorPaletteScriptable, skinColorScriptable);
		ResetColorUIPanel();
		_colorPanelUI.SetValueImage(skinColorPaletteScriptable.CharacterColorSkinPreview);
		void ResetColorUIPanel()
		{
			_colorPanelUI.SetValueText("");
			_colorPanelUI.SetValueImage(new Color(0f, 0f, 0f, 0f));
		}
	}

	private void ChangeSkinColorPreview(SkinColorScriptableObject skinColorScriptableObject, bool showColorName = false)
	{
		AudioManager.PlaySFX("ui_select");
		CharacterSkinData characterSkinData = GetCurrentHeadSkinSo().CharacterSkinData;
		CharacterSkinData characterSkinData2 = GetCurrentBodySkinSo().CharacterSkinData;
		SkinColorPaletteScriptableObject colorPaletteSo = GetColorPaletteSo(_skinColorPaletteCurrentIndex);
		_characterSkinPreviewUI.ChangeColorPreview(characterSkinData, characterSkinData2, colorPaletteSo, skinColorScriptableObject);
		ResetColorUIPanel();
		_skinColorPanelUI.SetValueImage(skinColorScriptableObject.GetSkinColorPreview());
		void ResetColorUIPanel()
		{
			_skinColorPanelUI.SetValueText("");
			_skinColorPanelUI.SetValueImage(new Color(0f, 0f, 0f, 0f));
		}
	}

	private void OnValuePanelChanged()
	{
		bool num = GetCurrentHeadSkinSo().CheckRequirementUnlock();
		bool flag = GetCurrentBodySkinSo().CheckRequirementUnlock();
		bool flag2 = num & flag;
		_confirmButton.interactable = flag2;
		_fullGameOnlyText?.gameObject.SetActive(!flag2);
		_characterSkinPreviewUI.SetPreviewUI(flag2);
	}

	private void PrevHeadButtonClick()
	{
		_headSkinCurrentIndex--;
		if (_headSkinCurrentIndex < 0)
		{
			_headSkinCurrentIndex = _headSkinDataList.Count - 1;
		}
		ChangeHeadPreview(_headSkinDataList[_headSkinCurrentIndex]);
	}

	private void NextHeadButtonClick()
	{
		_headSkinCurrentIndex++;
		if (_headSkinCurrentIndex >= _headSkinDataList.Count)
		{
			_headSkinCurrentIndex = 0;
		}
		ChangeHeadPreview(_headSkinDataList[_headSkinCurrentIndex]);
	}

	private void PrevBodyButtonClick()
	{
		_bodySkinCurrentIndex--;
		if (_bodySkinCurrentIndex < 0)
		{
			_bodySkinCurrentIndex = _bodySkinDataList.Count - 1;
		}
		ChangeBodyPreview(_bodySkinDataList[_bodySkinCurrentIndex]);
		RefreshSkinColorPaletteList(_bodySkinDataList[_bodySkinCurrentIndex]);
		ChangeColorPalettePreview(GetColorPaletteSo(_skinColorPaletteCurrentIndex));
	}

	private void NextBodyButtonClick()
	{
		_bodySkinCurrentIndex++;
		if (_bodySkinCurrentIndex >= _bodySkinDataList.Count)
		{
			_bodySkinCurrentIndex = 0;
		}
		ChangeBodyPreview(_bodySkinDataList[_bodySkinCurrentIndex]);
		RefreshSkinColorPaletteList(_bodySkinDataList[_bodySkinCurrentIndex]);
		ChangeColorPalettePreview(GetColorPaletteSo(_skinColorPaletteCurrentIndex));
	}

	private void PrevColorButtonClick()
	{
		_skinColorPaletteCurrentIndex--;
		if (_skinColorPaletteCurrentIndex < 0)
		{
			_skinColorPaletteCurrentIndex = _skinColorPaletteDataList.Count - 1;
		}
		ChangeColorPalettePreview(GetColorPaletteSo(_skinColorPaletteCurrentIndex));
	}

	private void NextColorButtonClick()
	{
		_skinColorPaletteCurrentIndex++;
		if (_skinColorPaletteCurrentIndex >= _skinColorPaletteDataList.Count)
		{
			_skinColorPaletteCurrentIndex = 0;
		}
		ChangeColorPalettePreview(GetColorPaletteSo(_skinColorPaletteCurrentIndex));
	}

	private void NextSkinColorButtonClick()
	{
		_skinColorCurrentIndex++;
		if (_skinColorCurrentIndex >= _skinColorDataList.Count)
		{
			_skinColorCurrentIndex = 0;
		}
		ChangeSkinColorPreview(GetSkinColorSo(_skinColorCurrentIndex));
	}

	private void PrevSkinColorButtonClick()
	{
		_skinColorCurrentIndex--;
		if (_skinColorCurrentIndex < 0)
		{
			_skinColorCurrentIndex = _skinColorDataList.Count - 1;
		}
		ChangeSkinColorPreview(GetSkinColorSo(_skinColorCurrentIndex));
	}

	private SkinColorPaletteScriptableObject GetColorPaletteSo(int index)
	{
		return _skinColorPaletteDataList[index];
	}

	private SkinColorScriptableObject GetSkinColorSo(int index)
	{
		return _skinColorDataList[index];
	}

	private SkinScriptableObject GetCurrentHeadSkinSo()
	{
		return _headSkinDataList[_headSkinCurrentIndex];
	}

	private SkinScriptableObject GetCurrentBodySkinSo()
	{
		return _bodySkinDataList[_bodySkinCurrentIndex];
	}

	private void SelectFirstButton()
	{
		_headPanelUI.Selected();
	}

	private void DeselectAllButton()
	{
		_headPanelUI.OnUnHoverPanel();
		_bodyPanelUI.OnUnHoverPanel();
		_colorPanelUI.OnUnHoverPanel();
	}

	private void OnEnable()
	{
		GlobalOptionsManager.OnDeviceChangedEvent += OnDeviceChangedAction;
	}

	private void OnDisable()
	{
		GlobalOptionsManager.OnDeviceChangedEvent -= OnDeviceChangedAction;
	}

	private void OnInput()
	{
		if (InputManager.inputActions.CharacterCustomize.Submit.WasPressedThisFrame())
		{
			ConfirmSkin();
		}
		else if (InputManager.inputActions.CharacterCustomize.Back.WasPressedThisFrame())
		{
			CloseUI();
		}
	}

	private void OnDeviceChangedAction(GlobalOptionsManager globalOptionsManager)
	{
		if (_uiView.isVisible)
		{
			if (globalOptionsManager.usingGamepad)
			{
				SelectFirstButton();
			}
			else
			{
				EventSystem.current.SetSelectedGameObject(null);
			}
		}
	}
}

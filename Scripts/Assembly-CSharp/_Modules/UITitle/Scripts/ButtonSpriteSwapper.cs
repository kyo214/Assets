using System;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Modules.UITitle.Scripts;

public class ButtonSpriteSwapper : MonoBehaviour
{
	[SerializeField]
	private RectTransform _panel;

	[SerializeField]
	private TMP_Text _text;

	[SerializeField]
	private LocalizationParamsManager _paramsManager;

	[SerializeField]
	private Image _backgroundImage;

	[SerializeField]
	private Image _glowImage;

	[SerializeField]
	private Sprite _glowSprite;

	public void Init(int index, GameData gameData)
	{
		if ((bool)_text)
		{
			if (gameData != null)
			{
				DateTime lastSaveDateTime = gameData.GetLastSaveDateTime();
				_text.text = lastSaveDateTime.ToShortDateString() + " " + lastSaveDateTime.ToShortTimeString();
			}
			else
			{
				_text.text = string.Empty;
			}
		}
		_paramsManager.SetParameterValue("VALUE", index.ToString());
	}

	private void SetGlowSprite()
	{
		_glowImage.sprite = _glowSprite;
	}

	public void SetActiveImage(bool value)
	{
		if (value)
		{
			SetGlowSprite();
		}
		_backgroundImage.enabled = value;
		_panel.gameObject.SetActive(value);
	}
}

using Doozy.Runtime.UIManager.Components;
using I2.Loc;
using TMPro;
using Toked;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLinkNavigation : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _textDescription;

	[SerializeField]
	private UIButton _buttonSteam;

	[SerializeField]
	private UIButton _buttonDiscord;

	private void Start()
	{
		if (!GameModes.Instance.isEvent && !GameModes.Instance.isDemo)
		{
			Navigation navigation = _buttonDiscord.navigation;
			if (navigation.selectOnLeft == _buttonSteam)
			{
				navigation.selectOnLeft = null;
			}
			if (navigation.selectOnLeft == _buttonSteam)
			{
				navigation.selectOnUp = null;
			}
			_buttonDiscord.navigation = navigation;
			_buttonSteam.gameObject.SetActive(value: false);
		}
	}

	public void OpenLink(string url)
	{
		Application.OpenURL(url);
	}

	public void OpenLinkOverlay(string url)
	{
		SteamApi.OpenWebOverlay(url);
	}

	public void ChangeText(string url)
	{
		SteamApi.OpenWebOverlay(url);
	}

	public void ChangeTextDescription(string term)
	{
		_textDescription.text = LocalizationManager.GetTranslation(term);
	}

	public void ClearTextDescription(string term)
	{
		if (LocalizationManager.GetTranslation(term) == _textDescription.text)
		{
			_textDescription.text = "";
		}
	}
}

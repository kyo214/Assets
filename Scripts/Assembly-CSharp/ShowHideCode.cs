using Doozy.Runtime.UIManager.Components;
using TMPro;
using UnityEngine;

public class ShowHideCode : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField _TFCode;

	[SerializeField]
	private UIButton _buttonShowCode;

	[SerializeField]
	private UIButton _buttonHideCode;

	public void OnEnable()
	{
		HideCode();
	}

	public void ShowCode()
	{
		_TFCode.contentType = TMP_InputField.ContentType.Standard;
		_buttonShowCode.gameObject.SetActive(value: false);
		_buttonHideCode.gameObject.SetActive(value: true);
		_TFCode.textComponent.SetAllDirty();
	}

	public void HideCode()
	{
		_TFCode.contentType = TMP_InputField.ContentType.Password;
		_buttonShowCode.gameObject.SetActive(value: true);
		_buttonHideCode.gameObject.SetActive(value: false);
		_TFCode.textComponent.SetAllDirty();
	}
}

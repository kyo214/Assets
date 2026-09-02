using TMPro;
using Toked;
using UnityEngine;

public class UISteamKeyboardTrigger : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField _tmpInputField;

	private void Start()
	{
		_tmpInputField.onSelect.AddListener(OnInputFieldSelected);
		_tmpInputField.onDeselect.AddListener(OnInputFieldDeselected);
	}

	private void OnInputFieldDeselected(string arg0)
	{
		Debug.Log("<color=#acd550>[SteamApi]</color> Close Floating Keyboard");
	}

	private void OnInputFieldSelected(string arg0)
	{
		SteamApi.OpenFloatingKeyboard(_tmpInputField, 12);
	}

	private void OnDestroy()
	{
		_tmpInputField.onSelect.RemoveAllListeners();
		_tmpInputField.onDeselect.RemoveAllListeners();
	}
}

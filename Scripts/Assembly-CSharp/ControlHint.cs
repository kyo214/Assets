using System.Collections;
using Cysharp.Threading.Tasks;
using Doozy.Runtime.UIManager.Containers;
using Toked;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlHint : MonoBehaviour
{
	[SerializeField]
	private GameObject _messageObject;

	[SerializeField]
	private UIView _view;

	[SerializeField]
	private bool _isUsingGamepad;

	public void Show()
	{
		AudioManager.PlaySFX("examine-paper-change-page");
		NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: false);
		NetworkGameManager.Instance.ownPlayer.playerInput.enabled = false;
		_messageObject.SetActive(value: false);
		StartCoroutine(WaitForKeyPress());
	}

	private IEnumerator WaitForKeyPress()
	{
		if (!GlobalSaveData.instance.optionData.SkipIntroControl)
		{
			yield return new WaitForSeconds(1.5f);
		}
		else
		{
			yield return new WaitForSeconds(0.5f);
		}
		_messageObject.SetActive(value: true);
		yield return new WaitUntil(() => (Gamepad.current != null && (Gamepad.current.buttonWest.wasPressedThisFrame || Gamepad.current.buttonNorth.wasPressedThisFrame || Gamepad.current.buttonSouth.wasPressedThisFrame || Gamepad.current.buttonEast.wasPressedThisFrame)) || (Input.anyKey && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2)));
		NetworkGameManager.Instance.ownPlayer.DelayInputTimer.StartDuration(0.5f);
		OnContinue();
		yield return new WaitForSeconds(0.1f);
	}

	private void OnContinue()
	{
		if (!GlobalSaveData.instance.optionData.IsFirstTimeControlShowed)
		{
			GlobalSaveData.instance.optionData.IsFirstTimeControlShowed = true;
			GlobalSaveData.instance.optionData.SkipIntroControl = true;
		}
		GlobalSaveData.instance.SaveOptionData();
		AudioManager.PlaySFX("ui_cancel");
		_isUsingGamepad = GlobalOptionsManager.Instance.usingGamepad;
		NetworkGameManager.Instance.ownPlayer.playerInput.enabled = true;
		NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: true);
		UniTaskUtil.DelayedCall(this, 0.1f, () =>
		{
			if (_isUsingGamepad)
			{
				GlobalOptionsManager.Instance.SetScheme("Gamepad", Gamepad.current);
			}
		}).Forget();
		_messageObject.gameObject.SetActive(value: false);
		AudioManager.PlaySFX("examine-corpse");
		UIGameManager.Instance.UIMenuNote.Hide();
		if (!UIGameManager.Instance.isUIInvisible)
		{
			UIGameManager.Instance.uiInGame.Show();
			if (UIGameManager.Instance.uiObjective != null && UIGameManager.Instance.uiObjective != null)
			{
				UIGameManager.Instance.uiObjective.SetActive(value: true);
			}
			if (LobbyManager.Instance == null)
			{
				UIGameManager.Instance.mapUI.SetActive(value: true);
			}
		}
		NetworkGameManager.Instance.ownPlayer.itemCollision = null;
		NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
		NetworkGameManager.Instance.ownPlayer.functionItemCollision = "";
		ChatSystem.Instance.ItemCommand.SetActive(value: false);
		UIGameManager.Instance.uiTabKill.gameObject.SetActive(value: true);
		_view.Hide();
	}
}

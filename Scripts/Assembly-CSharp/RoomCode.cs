using UnityEngine;
using UnityEngine.InputSystem;

public class RoomCode : MonoBehaviour
{
	[SerializeField]
	private InputActionReference _copyRoomCode;

	[SerializeField]
	private InputActionReference _showRoomCode;

	private void OnEnable()
	{
		_copyRoomCode.action.Enable();
		_copyRoomCode.action.started += OnCopyRoomCode;
		_showRoomCode.action.Enable();
		_showRoomCode.action.started += OnShowRoomCodeStarted;
		_showRoomCode.action.canceled += OnShowRoomCodeCanceled;
	}

	private void OnDisable()
	{
		_copyRoomCode.action.started -= OnCopyRoomCode;
		_copyRoomCode.action.Disable();
		_showRoomCode.action.started -= OnShowRoomCodeStarted;
		_showRoomCode.action.canceled -= OnShowRoomCodeCanceled;
		_showRoomCode.action.Disable();
	}

	private void OnCopyRoomCode(InputAction.CallbackContext obj)
	{
		if ((bool)NetworkGameManager.Instance.ownPlayer)
		{
			NetworkGameManager.Instance.ownPlayer.CopyCode(obj);
		}
	}

	private void OnShowRoomCodeStarted(InputAction.CallbackContext obj)
	{
		if ((bool)NetworkGameManager.Instance.ownPlayer)
		{
			NetworkGameManager.Instance.ownPlayer.ShowCode(obj);
		}
	}

	private void OnShowRoomCodeCanceled(InputAction.CallbackContext obj)
	{
		if ((bool)NetworkGameManager.Instance.ownPlayer)
		{
			NetworkGameManager.Instance.ownPlayer.ShowCode(obj);
		}
	}
}

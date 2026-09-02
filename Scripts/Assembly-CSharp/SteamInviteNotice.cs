using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SteamInviteNotice : MonoBehaviour, IPointerMoveHandler, IEventSystemHandler
{
	[SerializeField]
	private SteamLobby _lobby;

	[SerializeField]
	private Transform _board;

	[SerializeField]
	private TextMeshProUGUI _invitorText;

	[SerializeField]
	private Button[] _buttons;

	[SerializeField]
	private PlayerInput _rootPlayerInput;

	public static bool ListenerOk;

	private PlayerInput _playerInput;

	private float _fdir;

	private int _navIndex;

	private bool _navMode;

	private bool _showed;

	public static event Action<bool> ActInviteResponse;

	private void Start()
	{
		_board.gameObject.SetActive(value: false);
		_playerInput = GetComponent<PlayerInput>();
		_playerInput.enabled = false;
	}

	private void OnEnable()
	{
		_lobby.ActInviteNoticeShow += SteamLobby_ActInviteNoticeShow;
		_lobby.ActInviteNoticeHide += SteamLobby_ActInviteNoticeHide;
		_showed = true;
	}

	private void OnDisable()
	{
		_lobby.ActInviteNoticeShow -= SteamLobby_ActInviteNoticeShow;
		_lobby.ActInviteNoticeHide -= SteamLobby_ActInviteNoticeHide;
		_showed = false;
	}

	private void SteamLobby_ActInviteNoticeHide()
	{
		_board.gameObject.SetActive(value: false);
		_playerInput.enabled = false;
		_rootPlayerInput.enabled = true;
	}

	private void SteamLobby_ActInviteNoticeShow(string friendName)
	{
		_invitorText.text = friendName + " invited you to a game.";
		_board.gameObject.SetActive(value: true);
		_buttons[0].Select();
		_rootPlayerInput.enabled = false;
		_playerInput.enabled = true;
	}

	public void SendInvitationFeedback(bool isAccept)
	{
		Debug.Log("Accept " + isAccept);
		ActInviteResponse(isAccept);
		_playerInput.enabled = false;
		_rootPlayerInput.enabled = true;
	}

	public void Nav(InputAction.CallbackContext value)
	{
		_fdir = value.ReadValue<Vector2>().x;
		if ((double)Mathf.Abs(_fdir) > 0.5)
		{
			_navMode = true;
			_navIndex += (int)Mathf.Sign(_fdir);
			if (_navIndex >= 0)
			{
				_navIndex %= 2;
			}
			else
			{
				_navIndex = 1;
			}
			EventSystem.current.SetSelectedGameObject(null);
			_buttons[_navIndex].Select();
			Debug.Log("Nav Sub " + _navIndex);
		}
	}

	public void ActionPress(InputAction.CallbackContext value)
	{
		if (_navMode)
		{
			Debug.Log("FIRE");
			_buttons[_navIndex].onClick.Invoke();
			_navMode = false;
		}
	}

	public void OnPointerMove(PointerEventData eventData)
	{
		if (_showed && _navMode)
		{
			_navMode = false;
			EventSystem.current.SetSelectedGameObject(null);
		}
	}
}

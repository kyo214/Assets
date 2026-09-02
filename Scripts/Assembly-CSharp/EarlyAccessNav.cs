using System.Collections;
using TMPro;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EarlyAccessNav : MonoBehaviour, IPointerMoveHandler, IEventSystemHandler
{
	[SerializeField]
	private Transform _buttonGroup;

	[SerializeField]
	private Transform _snsDescription;

	[SerializeField]
	private Transform _btnHighlight;

	[SerializeField]
	private Button _btnBack;

	private Button[] _snsButtons;

	private TextMeshProUGUI[] _tmpDescriptions;

	private int _navIndex;

	private bool _isBack;

	private Vector2 _navDir;

	private bool _navMode;

	[SerializeField]
	private PlayerInput _playerInput;

	private IEnumerator Start()
	{
		if ((bool)NetworkGameManager.Instance)
		{
			NetworkGameManager.Instance.ownPlayer.playerInput.enabled = false;
		}
		_playerInput.enabled = false;
		ClearHover();
		_snsButtons = new Button[_buttonGroup.childCount];
		for (int i = 0; i < _snsButtons.Length; i++)
		{
			_snsButtons[i] = _buttonGroup.GetChild(i).GetComponent<Button>();
		}
		_tmpDescriptions = new TextMeshProUGUI[_snsDescription.childCount];
		for (int j = 0; j < _tmpDescriptions.Length; j++)
		{
			_tmpDescriptions[j] = _snsDescription.GetChild(j).GetComponent<TextMeshProUGUI>();
		}
		_navDir = new Vector2(0f, 0f);
		yield return new WaitForSeconds(0.1f);
		_playerInput.enabled = true;
		if (GameModes.Instance.isEvent || GameModes.Instance.isDemo)
		{
			_btnBack.Select();
		}
	}

	public void Hover(int btdIdx)
	{
		_btnHighlight.position = _snsButtons[btdIdx].transform.position;
		_btnHighlight.gameObject.SetActive(value: true);
		for (int i = 0; i < _tmpDescriptions.Length; i++)
		{
			if (i == btdIdx)
			{
				_tmpDescriptions[i].gameObject.SetActive(value: true);
			}
			else
			{
				_tmpDescriptions[i].gameObject.SetActive(value: false);
			}
		}
	}

	public void ClearHover()
	{
		_btnHighlight.gameObject.SetActive(value: false);
	}

	public void OpenLink(string url)
	{
		Application.OpenURL(url);
	}

	public void OpenLinkOverlay(string url)
	{
		SteamApi.OpenWebOverlay(url);
	}

	public void ActionPress(InputAction.CallbackContext value)
	{
		if (_navMode)
		{
			_snsButtons[_navIndex].onClick.Invoke();
			_navMode = false;
		}
	}

	public void Nav(InputAction.CallbackContext value)
	{
		if (GameModes.Instance.isEvent || GameModes.Instance.isDemo)
		{
			return;
		}
		_navDir = value.ReadValue<Vector2>();
		if (Mathf.Abs(_navDir.x) > 0.5f)
		{
			if (!_isBack)
			{
				_navMode = true;
				ClearHover();
				_navIndex += (int)Mathf.Sign(_navDir.x);
				if (_navIndex < 0)
				{
					_navIndex = _snsButtons.Length - 1;
				}
				else
				{
					_navIndex %= _snsButtons.Length;
				}
				Hover(_navIndex);
			}
		}
		else if ((double)Mathf.Abs(_navDir.y) > 0.5)
		{
			_navMode = true;
			_isBack = !_isBack;
			ClearHover();
			if (_isBack)
			{
				_btnBack.Select();
				return;
			}
			EventSystem.current.SetSelectedGameObject(null);
			_navIndex = _snsButtons.Length - 1;
			Hover(_navIndex);
		}
	}

	public void OnPointerMove(PointerEventData eventData)
	{
		if (_navMode)
		{
			_navMode = false;
			ClearHover();
		}
	}
}

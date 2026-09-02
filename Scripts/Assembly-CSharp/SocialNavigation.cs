using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SocialNavigation : MonoBehaviour
{
	[SerializeField]
	private SocialButton[] _socialButtons;

	[SerializeField]
	private SocialButton[][] _btnIndexes;

	[SerializeField]
	private ButtonScene _backButtonScn;

	[SerializeField]
	private Button _retryBtn;

	private Sprite _retryNormal;

	[SerializeField]
	private Sprite _retryHl;

	[SerializeField]
	private Button _backBtn;

	private Sprite _backNormal;

	[SerializeField]
	private Sprite _backHl;

	private Vector2 prevDirection = Vector2.zero;

	private int _pRow;

	private int _pCol;

	private bool _isNav;

	private SocialButton _pSocialBtn;

	[SerializeField]
	private PlayerInput _playerInput;

	public IEnumerator Start()
	{
		NetworkGameManager.Instance.ownPlayer.playerInput.enabled = false;
		_playerInput.enabled = false;
		int num = 0;
		_btnIndexes = new SocialButton[3][];
		for (int i = 0; i < _btnIndexes.Length; i++)
		{
			_btnIndexes[i] = new SocialButton[2];
			for (int j = 0; j < _btnIndexes[i].Length; j++)
			{
				_btnIndexes[i][j] = _socialButtons[num];
				num++;
			}
		}
		_pRow = 0;
		_pCol = 0;
		_backNormal = _backBtn.image.sprite;
		_retryNormal = _backBtn.image.sprite;
		Highlight(0, 0);
		yield return new WaitForSeconds(0.1f);
		_playerInput.enabled = true;
	}

	private void Update()
	{
		if (_isNav && (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f))
		{
			_isNav = false;
			ClearAllHighlight();
		}
	}

	public void ActionPress(InputAction.CallbackContext value)
	{
		if (value.started && _isNav)
		{
			if (_pRow < 3)
			{
				_pSocialBtn.OpenLink();
			}
			else
			{
				_backButtonScn.BackToMainMenu(isRemovePlayer: true);
			}
		}
	}

	public void Nav(InputAction.CallbackContext value)
	{
		Vector2 vector = value.ReadValue<Vector2>();
		bool flag = false;
		if (prevDirection == Vector2.zero)
		{
			if (Mathf.Abs(vector.x) > 0.5f)
			{
				int num = _pCol + (int)Mathf.Sign(vector.x);
				num = ((num < 0) ? 1 : (num % 2));
				_pCol = num;
				flag = true;
			}
			else if (Mathf.Abs(vector.y) > 0.5f)
			{
				int num2 = _pRow + (int)Mathf.Sign(vector.y) * -1;
				num2 = ((num2 >= 0) ? (num2 % 4) : 2);
				_pRow = num2;
				flag = true;
			}
			if (flag)
			{
				_isNav = true;
				ClearAllHighlight();
				Highlight(_pRow, _pCol);
			}
		}
		prevDirection = vector;
	}

	private void Highlight(int pRow, int pCol)
	{
		if (pRow == 3)
		{
			_backBtn.image.sprite = _backHl;
			return;
		}
		_pSocialBtn = _btnIndexes[pRow][pCol];
		_pSocialBtn.Highlight();
	}

	private void ClearAllHighlight()
	{
		SocialButton[] socialButtons = _socialButtons;
		foreach (SocialButton obj in socialButtons)
		{
			_backBtn.image.sprite = _backNormal;
			_retryBtn.image.sprite = _retryNormal;
			obj.Normal();
		}
	}
}

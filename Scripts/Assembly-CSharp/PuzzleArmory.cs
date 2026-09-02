using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzleArmory : MonoBehaviour, IPuzzle, IPointerMoveHandler, IEventSystemHandler
{
	[Header("Setup Properties")]
	[SerializeField]
	private int _keyLength;

	[Header("Internal Component")]
	[SerializeField]
	private TextMeshProUGUI _inputDigitalText;

	[SerializeField]
	private Image _submitBtn;

	[SerializeField]
	private Transform _buttonGroup;

	[Header("External Component")]
	[SerializeField]
	private Sprite _sprSubmitNormal;

	[SerializeField]
	private Sprite _sprSubmitPressed;

	[SerializeField]
	private Sprite _sprSubmitAccepted;

	[SerializeField]
	private Sprite _sprSubmitDeclined;

	[SerializeField]
	private PuzzlePoliceComputer _puzzlePoliceComputer;

	[SerializeField]
	private PuzzlePoliceComputer _puzzlePoliceComputer2;

	private const string SFX_ACCEPTED = "sfx-armory-accepted";

	private const string SFX_PRESS = "sfx-armory-press";

	private const string SFX_DECLINED = "sfx-armory-decline";

	private const string SFX_UNLOCK = "sfx-armory-unlock";

	[SerializeField]
	private ItemInteractable _interactableTrigger;

	private string _key;

	private bool _locked;

	private bool _success;

	private Button[] _buttonCaches;

	private int _nav;

	private bool _isNavMode;

	private Color _white;

	private Color _dimmed;

	private void Start()
	{
		_buttonCaches = new Button[_buttonGroup.childCount];
		for (int i = 0; i < _buttonGroup.childCount; i++)
		{
			_buttonCaches[i] = _buttonGroup.GetChild(i).GetComponent<Button>();
		}
		_white = new Color(1f, 1f, 1f, 1f);
		_dimmed = new Color(0.5f, 0.5f, 0.5f, 1f);
		StartCoroutine(Generate());
	}

	private IEnumerator Generate()
	{
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		UnityEngine.Random.InitState(GlobalOptionsManager.Instance.GetSeedCombineWithMissionID());
		_key = "";
		for (int i = 0; i < _keyLength; i++)
		{
			_key += UnityEngine.Random.Range(0, 10);
		}
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
		ResetPuzzle();
		if (_puzzlePoliceComputer != null)
		{
			_puzzlePoliceComputer.SetClue(_key);
		}
		if (_puzzlePoliceComputer2 != null)
		{
			_puzzlePoliceComputer2.SetClue(_key);
		}
	}

	public void ResetPuzzle()
	{
		_inputDigitalText.text = "";
		_submitBtn.sprite = _sprSubmitNormal;
	}

	public void InputDigit(int digitNumber)
	{
		if (!_locked && _inputDigitalText.text.Length < _keyLength)
		{
			AudioManager.PlaySFX("sfx-armory-press");
			_inputDigitalText.text += digitNumber;
		}
	}

	public void Submit()
	{
		if (!_locked)
		{
			_locked = true;
			_submitBtn.sprite = _sprSubmitPressed;
			StartCoroutine(SubmitSequence());
		}
	}

	private IEnumerator SubmitSequence()
	{
		_locked = true;
		yield return new WaitForSeconds(0.5f);
		if (_inputDigitalText.text == _key.ToString())
		{
			for (int i = 0; i < 3; i++)
			{
				yield return new WaitForSeconds(0.1f);
				AudioManager.PlaySFX("sfx-armory-accepted");
				_submitBtn.sprite = _sprSubmitNormal;
				yield return new WaitForSeconds(0.1f);
				_submitBtn.sprite = _sprSubmitAccepted;
				StartCoroutine(PuzzleUnlocked());
			}
		}
		else
		{
			AudioManager.PlaySFX("sfx-armory-decline");
			_submitBtn.sprite = _sprSubmitDeclined;
			yield return new WaitForSeconds(0.5f);
			_submitBtn.sprite = _sprSubmitNormal;
		}
		ResetPuzzle();
		_locked = false;
	}

	public void Action1Press()
	{
		if (_isNavMode)
		{
			_buttonCaches[_nav].onClick.Invoke();
		}
	}

	public void Action1Release()
	{
	}

	public ItemInteractable GetInteractableObject()
	{
		return _interactableTrigger;
	}

	public void Hide()
	{
	}

	public void InitAnswer()
	{
	}

	public void Navigate(Vector2 direction)
	{
		if (Mathf.Abs(direction.x) > 0f || Mathf.Abs(direction.y) > 0f)
		{
			_isNavMode = true;
			int num = _nav / 3;
			int num2 = _nav % 3;
			if (Mathf.Abs(direction.x) > 0.5f)
			{
				num2 += (int)Mathf.Sign(direction.x);
				num2 = ((num2 >= 0) ? (num2 % 3) : 2);
			}
			else if (Mathf.Abs(direction.y) > 0.5f)
			{
				num -= (int)Mathf.Sign(direction.y);
				num = ((num >= 0) ? (num % 4) : 3);
			}
			_nav = num * 3 + num2;
			Highlight();
		}
	}

	public IEnumerator PuzzleUnlocked()
	{
		_success = true;
		AudioManager.PlaySFX("sfx-armory-unlock");
		yield return new WaitForSeconds(0.2f);
		UIGameManager.Instance.ShowUIInGame(_interactableTrigger.UIMenu);
		NetworkGameManager.Instance.ownPlayer.network.ExecInteractObject((short)_interactableTrigger.UniqueID);
		_interactableTrigger.DisableCollider();
		NetworkGameManager.Instance.ownPlayer.itemCollision = null;
		NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
	}

	public void SetInteractableObject(ItemInteractable intObject)
	{
		_interactableTrigger = intObject;
	}

	public void SetPassword(string pass)
	{
	}

	public void Show()
	{
		_locked = _success;
		ClearHighlight();
		_nav = 0;
		Debug.Log(_key.ToString());
		ResetPuzzle();
	}

	private void Highlight()
	{
		ClearHighlight();
		_buttonCaches[_nav].image.DOColor(_dimmed, 0.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
	}

	private void ClearHighlight()
	{
		for (int i = 0; i < _buttonCaches.Length; i++)
		{
			_buttonCaches[i].image.DOKill();
			_buttonCaches[i].image.color = _white;
		}
	}

	public void OnPointerMove(PointerEventData eventData)
	{
		_isNavMode = false;
		ClearHighlight();
	}

	public void PlaySoundButton()
	{
		if (!_locked)
		{
			AudioManager.PlaySFX("sfx-armory-press");
		}
	}
}

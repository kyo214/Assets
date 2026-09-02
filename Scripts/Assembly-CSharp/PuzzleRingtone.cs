using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzleRingtone : MonoBehaviour, IPuzzle, IPointerMoveHandler, IEventSystemHandler
{
	[Header("Transform References")]
	[SerializeField]
	private TextMeshProUGUI _textDigit;

	[SerializeField]
	private Image _led;

	[SerializeField]
	private BtNavRow[] _btNavs;

	[SerializeField]
	private Image[] _toneBars;

	[Header("Sprite Lib")]
	[SerializeField]
	private Sprite _ledOn;

	[Header("Data List")]
	[SerializeField]
	private List<string> _toneSequences;

	[Header("Test Values")]
	[SerializeField]
	private int _testSeed;

	private string _inputChar;

	private string _correctChar;

	private int _seed;

	private bool _success;

	private int _cNav;

	private int _rNav;

	private bool _sequencePlayed;

	private bool _sequencePlaying;

	private bool _isNav;

	private bool _shouldPlay;

	private ItemInteractable _interactableTrigger;

	private void Start()
	{
		StartCoroutine(GetSeed());
		GeneratePuzzle();
	}

	public void OnPointerMove(PointerEventData eventData)
	{
		_isNav = false;
	}

	private void ResetPuzzle()
	{
		Image[] toneBars = _toneBars;
		for (int i = 0; i < toneBars.Length; i++)
		{
			toneBars[i].gameObject.SetActive(value: false);
		}
		_sequencePlayed = false;
		EmptyField();
	}

	private void EmptyField()
	{
		_success = false;
		_inputChar = "";
		_textDigit.text = _inputChar;
	}

	private IEnumerator GetSeed()
	{
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		_seed = GlobalOptionsManager.Instance.GetSeedCombineWithMissionID();
	}

	private void GeneratePuzzle()
	{
		UnityEngine.Random.InitState(_seed);
		_inputChar = "";
		_correctChar = "";
		int index = UnityEngine.Random.Range(0, _toneSequences.Count);
		_correctChar = _toneSequences[index];
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
	}

	private void ActualInputValue(int tone)
	{
		if (!_success && _sequencePlayed)
		{
			if (tone == -1)
			{
				ClearInput();
				return;
			}
			if (_inputChar.Length < 5)
			{
				_inputChar += tone;
				_textDigit.text = _inputChar;
				StartCoroutine(VisualizeBeep(tone));
				AudioManager.PlaySFX("puzzle-ringtone-beep" + tone);
			}
		}
		if (_inputChar == _correctChar)
		{
			Invoke("PuzzleSuccess", 1f);
		}
		else if (_inputChar.Length >= 5)
		{
			Invoke("EmptyField", 0.75f);
		}
	}

	public void InputButton(int tone)
	{
		if (!_isNav)
		{
			ClearHighlight();
			ActualInputValue(tone);
		}
	}

	public void ClearInput()
	{
		_inputChar = "";
		EmptyField();
		_sequencePlayed = false;
		StartCoroutine(PlayToneSequence());
	}

	public void Action1Press()
	{
		if (_isNav)
		{
			ActualInputValue(_btNavs[_rNav].Tone[_cNav]);
		}
	}

	public void Action1Release()
	{
	}

	public ItemInteractable GetInteractableObject()
	{
		return _interactableTrigger;
	}

	public void InitAnswer()
	{
	}

	public void Navigate(Vector2 direction)
	{
		if (Mathf.Abs(direction.y) > 0.5f)
		{
			int num = _rNav - (int)Mathf.Sign(direction.y);
			if (num < 0)
			{
				_rNav = _btNavs.Length - 1;
			}
			else
			{
				_rNav = num % _btNavs.Length;
			}
			_isNav = true;
			Highlight();
		}
		else if (Mathf.Abs(direction.x) > 0.5f)
		{
			int num2 = _cNav + (int)Mathf.Sign(direction.x);
			if (num2 < 0)
			{
				_cNav = _btNavs[0].BtNav.Length - 1;
			}
			else
			{
				_cNav = num2 % _btNavs[0].BtNav.Length;
			}
			_isNav = true;
			Highlight();
		}
	}

	private void ClearHighlight()
	{
		BtNavRow[] btNavs = _btNavs;
		for (int i = 0; i < btNavs.Length; i++)
		{
			Image[] btNav = btNavs[i].BtNav;
			foreach (Image obj in btNav)
			{
				obj.DOKill();
				obj.color = Color.white;
			}
		}
	}

	private void Highlight()
	{
		Image target = _btNavs[_rNav].BtNav[_cNav];
		ClearHighlight();
		target.DOColor(Color.gray, 0.5f).SetLoops(-1, LoopType.Yoyo);
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
		ResetPuzzle();
		_shouldPlay = true;
		StartCoroutine(PlayToneSequence());
		Highlight();
		_led.DOKill();
		_led.DOColor(Color.gray, 0.75f).SetLoops(-1, LoopType.Yoyo);
	}

	private IEnumerator PlayToneSequence()
	{
		if (_sequencePlaying)
		{
			yield break;
		}
		_sequencePlaying = true;
		for (int i = 0; i < _correctChar.Length; i++)
		{
			yield return new WaitForSeconds(1f);
			if (!_shouldPlay)
			{
				break;
			}
			StartCoroutine(VisualizeBeep(int.Parse(_correctChar[i].ToString())));
			AudioManager.PlaySFX("puzzle-ringtone-beep" + _correctChar[i]);
		}
		_sequencePlaying = false;
		_sequencePlayed = true;
	}

	private IEnumerator VisualizeBeep(int length)
	{
		for (int i = 0; i < length; i++)
		{
			_toneBars[i].gameObject.SetActive(value: true);
		}
		yield return new WaitForSeconds(0.75f);
		Image[] toneBars = _toneBars;
		for (int j = 0; j < toneBars.Length; j++)
		{
			toneBars[j].gameObject.SetActive(value: false);
		}
	}

	private void PuzzleSuccess()
	{
		if (!_success)
		{
			_success = true;
			_led.sprite = _ledOn;
			StartCoroutine(PuzzleUnlocked());
		}
	}

	public IEnumerator PuzzleUnlocked()
	{
		AudioManager.StopSFX("lockpick_loop");
		yield return new WaitForSeconds(0.2f);
		UIGameManager.Instance.ShowUIInGame(_interactableTrigger.UIMenu);
		NetworkGameManager.Instance.ownPlayer.network.ExecInteractObject((short)_interactableTrigger.UniqueID);
		_interactableTrigger.DisableCollider();
		NetworkGameManager.Instance.ownPlayer.itemCollision = null;
		NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
	}

	public void Hide()
	{
		_shouldPlay = false;
	}
}

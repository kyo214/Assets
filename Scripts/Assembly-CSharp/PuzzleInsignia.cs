using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzleInsignia : MonoBehaviour, IPuzzle, IPointerMoveHandler, IEventSystemHandler
{
	[Header("Internal Components")]
	[SerializeField]
	private Image _imgInsignia;

	[SerializeField]
	private Image _indicatorGreen;

	[SerializeField]
	private Image _indicatorRed;

	[SerializeField]
	private Transform _btnPads;

	[SerializeField]
	private Image _btnSubmit;

	[SerializeField]
	private Transform _textCarvingGroup;

	[SerializeField]
	private Image _imgMetalScar;

	[Header("External Components")]
	[SerializeField]
	private CluePieceInsignia _matchingClue;

	[Header("Asset Reference")]
	[SerializeField]
	private Sprite _onButton;

	[SerializeField]
	private Sprite _offButton;

	[SerializeField]
	private Sprite[] _insigniaLibrary;

	[Header("Setup Properties")]
	[SerializeField]
	private int _insigniaIndex;

	[SerializeField]
	private Color _normalColor;

	[SerializeField]
	private Color _dimmedColor;

	[SerializeField]
	private NavDirections[] _navIndexes;

	[SerializeField]
	private string[] _charSetChunks;

	[SerializeField]
	private string[] _orderedCharSetChunks;

	[SerializeField]
	private bool _isGenerator;

	[SerializeField]
	private PuzzleInsignia _cloneTo;

	[Header("Sound Variables")]
	[SerializeField]
	private string _beepButtonWhite;

	[SerializeField]
	private string _sfxAccepted;

	[SerializeField]
	private string _sfxDeclined;

	private string _projectedClues;

	private string _thisClue;

	private string _charOrder;

	private int _chosenClueIndex;

	private List<Image> _ledImages;

	private List<Button> _buttonCaches;

	private TextMeshProUGUI[] _textDigitCaches;

	private int _missingIndex;

	private int _litCount = 5;

	private int _seed;

	private bool _success;

	private ItemInteractable _interactableTrigger;

	private bool _mode;

	private int _highlighIndex;

	private void Start()
	{
		_imgMetalScar.gameObject.SetActive(value: false);
		_textDigitCaches = new TextMeshProUGUI[_textCarvingGroup.childCount];
		for (int i = 0; i < _textDigitCaches.Length; i++)
		{
			_textDigitCaches[i] = _textCarvingGroup.GetChild(i).GetComponent<TextMeshProUGUI>();
		}
		_ledImages = new List<Image>();
		_buttonCaches = new List<Button>();
		for (int j = 0; j < _btnPads.childCount; j++)
		{
			_buttonCaches.Add(_btnPads.GetChild(j).GetComponent<Button>());
			_ledImages.Add(_btnPads.GetChild(j).GetComponent<Image>());
		}
		_buttonCaches.Add(_btnSubmit.GetComponent<Button>());
		_ledImages.Add(_btnSubmit);
		_mode = false;
		if (_isGenerator)
		{
			StartCoroutine(GeneratePuzzle(-1));
		}
	}

	private void ResetPuzzle()
	{
		_success = false;
		if (_insigniaIndex < _insigniaLibrary.Length)
		{
			_imgInsignia.sprite = _insigniaLibrary[_insigniaIndex];
		}
		_charOrder = "";
		for (int i = 0; i < _ledImages.Count - 1; i++)
		{
			_ledImages[i].sprite = _offButton;
		}
		RenderTextClue(_thisClue);
	}

	public void GetSeed()
	{
		_seed = GlobalOptionsManager.Instance.GetSeedCombineWithMissionID() + _insigniaIndex;
	}

	public void Action1Press()
	{
		if (_mode)
		{
			_buttonCaches[_highlighIndex].Select();
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
		if (Mathf.Abs(direction.x) > 0.5f)
		{
			_mode = true;
			if (direction.x > 0.5f)
			{
				_highlighIndex = _navIndexes[_highlighIndex].Right;
			}
			else if (direction.x < -0.5f)
			{
				_highlighIndex = _navIndexes[_highlighIndex].Left;
			}
			Highlight();
		}
		if (Mathf.Abs(direction.y) > 0.5f)
		{
			_mode = true;
			if (direction.y > 0.5f)
			{
				_highlighIndex = _navIndexes[_highlighIndex].Up;
			}
			else if (direction.y < -0.5f)
			{
				_highlighIndex = _navIndexes[_highlighIndex].Down;
			}
			Highlight();
		}
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
		_highlighIndex = 0;
		Highlight();
	}

	private void Highlight()
	{
		for (int i = 0; i < _ledImages.Count; i++)
		{
			_ledImages[i].DOKill();
			_ledImages[i].color = _normalColor;
		}
		if (_mode)
		{
			_ledImages[_highlighIndex].DOColor(_dimmedColor, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
		}
	}

	public void Hide()
	{
	}

	public IEnumerator GeneratePuzzle(int clonedIndex)
	{
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		GetSeed();
		UnityEngine.Random.InitState(_seed);
		if (clonedIndex >= 0)
		{
			_chosenClueIndex = clonedIndex;
		}
		else
		{
			int num = _insigniaIndex * _charSetChunks.Length / 2;
			int chosenClueIndex = UnityEngine.Random.Range(num, num + _charSetChunks.Length / 2);
			_chosenClueIndex = chosenClueIndex;
		}
		string source = _orderedCharSetChunks[_chosenClueIndex].Substring(0, 4);
		string source2 = _orderedCharSetChunks[_chosenClueIndex].Substring(4, 5);
		_projectedClues = _charSetChunks[_chosenClueIndex];
		string text = "";
		List<char> list = source.ToList();
		for (int i = 0; i < 3; i++)
		{
			int index = UnityEngine.Random.Range(0, list.Count());
			text += list[index];
			list.RemoveAt(index);
		}
		list = source2.ToList();
		for (int j = 0; j < 3; j++)
		{
			int index = UnityEngine.Random.Range(0, list.Count());
			text += list[index];
			list.RemoveAt(index);
		}
		list.Clear();
		list = text.ToList();
		_thisClue = "";
		for (int k = 0; k < text.Length; k++)
		{
			int index = UnityEngine.Random.Range(0, list.Count());
			_thisClue += list[index];
			list.RemoveAt(index);
		}
		if (_isGenerator)
		{
			Invoke("InvokeSetClue", 1f);
		}
		else
		{
			_missingIndex = UnityEngine.Random.Range(0, _thisClue.Length);
		}
		if (_cloneTo != null)
		{
			StartCoroutine(_cloneTo.GeneratePuzzle(_chosenClueIndex));
		}
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
	}

	private void RenderTextClue(string clue)
	{
		if (clue.Length != _textDigitCaches.Length)
		{
			return;
		}
		string text = "";
		if (!_isGenerator && _insigniaIndex == 0)
		{
			_imgMetalScar.gameObject.SetActive(value: true);
			_imgMetalScar.transform.position = new Vector3(_textDigitCaches[_missingIndex].transform.position.x, _textDigitCaches[_missingIndex].transform.position.y + 1f, _textDigitCaches[_missingIndex].transform.position.z);
			for (int i = 0; i < _thisClue.Length; i++)
			{
				text = ((i != _missingIndex) ? (text + _thisClue.Substring(i, 1)) : (text + " "));
			}
		}
		else
		{
			text = clue;
		}
		for (int j = 0; j < text.Length; j++)
		{
			_textDigitCaches[j].text = text.Substring(j, 1);
		}
	}

	private void InvokeSetClue()
	{
		if (_matchingClue != null)
		{
			_matchingClue.SetClue(_imgInsignia.sprite, _chosenClueIndex);
		}
	}

	public void DirectNavigate(int idx)
	{
		_mode = false;
		Highlight();
		_highlighIndex = idx;
		ToggleButton(_highlighIndex);
	}

	public void ToggleButton(int idx)
	{
		if (!_success && _ledImages[idx].sprite == _offButton)
		{
			AudioManager.PlaySFX(_beepButtonWhite);
			_ledImages[idx].sprite = _onButton;
			_charOrder += _projectedClues.Substring(idx, 1);
		}
	}

	public void CompareResult()
	{
		if (!CompareCheck())
		{
			ResetPuzzle();
			Invoke("InvokePlayDeclined", 0.1f);
		}
		else if (!_success)
		{
			PuzzleSuccess();
		}
	}

	private void InvokePlayDeclined()
	{
		AudioManager.PlaySFX(_sfxDeclined);
	}

	private bool CompareCheck()
	{
		if (_success)
		{
			return false;
		}
		if (_charOrder != _thisClue)
		{
			return false;
		}
		return true;
	}

	private void PuzzleSuccess()
	{
		_success = true;
		StartCoroutine(PuzzleUnlocked());
	}

	public IEnumerator PuzzleUnlocked()
	{
		EventSystem.current.SetSelectedGameObject(null);
		_success = true;
		yield return new WaitForSeconds(0.1f);
		AudioManager.PlaySFX(_sfxAccepted);
		_indicatorRed.gameObject.SetActive(value: false);
		_indicatorGreen.gameObject.SetActive(value: true);
		AudioManager.PlaySFX("puzzle-combilock-switch-on");
		yield return new WaitForSeconds(0.5f);
		_interactableTrigger.UIMenu.Hide();
		UIGameManager.Instance.ShowUIInGame(_interactableTrigger.UIMenu);
		NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: true);
		ChatSystem.Instance.ItemCommand.SetActive(value: false);
		UIGameManager.Instance.mapUI.SetActive(value: true);
		NetworkGameManager.Instance.ownPlayer.network.ExecInteractObject((short)_interactableTrigger.UniqueID);
		NetworkGameManager.Instance.ownPlayer.itemCollision = null;
		NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
	}

	public void OnPointerMove(PointerEventData eventData)
	{
		_mode = false;
	}
}

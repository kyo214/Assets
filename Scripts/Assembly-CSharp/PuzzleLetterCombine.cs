using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Toked;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleLetterCombine : MonoBehaviour, IPuzzle
{
	[Header("Object References")]
	[SerializeField]
	private List<Image> _letterWheels;

	[SerializeField]
	private List<Transform> _letterPlattingA;

	[SerializeField]
	private List<Transform> _letterPlattingB;

	[SerializeField]
	private List<Transform> _letterPlattingC;

	[SerializeField]
	private List<TextMeshProUGUI> _letters;

	[Header("Data Input")]
	[SerializeField]
	private string _charSet;

	[SerializeField]
	private IsoCode[] _isoCodes;

	[SerializeField]
	private Color _highlighted;

	[SerializeField]
	private Color _inactive;

	[Header("Randomizer Setup")]
	[SerializeField]
	private CluePuzzle _uiClue;

	private string _answerOverride;

	private int _seed;

	private List<char> _charSetList;

	private char[][] _charWheelSets;

	private List<Transform>[] _letterPlattings;

	private int _wheelCursor;

	private bool _isAnimating;

	private bool _setupComplete;

	private List<int> _shuffleData;

	private bool _success;

	private ItemInteractable _interactableTrigger;

	private void Start()
	{
		StartCoroutine(GeneratePuzzle());
	}

	private void ResetPuzzle()
	{
		_success = false;
		_wheelCursor = 0;
		_isAnimating = false;
		ShiftCursor(_letterWheels[_wheelCursor]);
	}

	public void Action1Press()
	{
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
		if (_isAnimating)
		{
			return;
		}
		if (direction.x > 0.5f || direction.x < -0.5f)
		{
			_isAnimating = true;
			if (direction.x > 0.5f)
			{
				direction.x = 1f;
			}
			else if (direction.x < -0.5f)
			{
				direction.x = -1f;
			}
			int num = _wheelCursor + (int)Mathf.Sign(direction.x);
			if (num < 0)
			{
				num = _letterPlattings.Length - 1;
			}
			_wheelCursor = num % _letterPlattings.Length;
			ShiftCursor(_letterWheels[_wheelCursor]);
			_isAnimating = false;
		}
		else if ((direction.y > 0.5f || direction.y < -0.5f) && !_success)
		{
			_isAnimating = true;
			if (direction.y > 0.5f)
			{
				direction.y = 1f;
			}
			else if (direction.y < -0.5f)
			{
				direction.y = -1f;
			}
			ShiftWheel(_letterPlattings[_wheelCursor], (int)Mathf.Sign(direction.y), isTween: true);
		}
	}

	public void ClickShift(int wheelIndex)
	{
		if (!_isAnimating && !_success)
		{
			_isAnimating = true;
			_wheelCursor = wheelIndex;
			ShiftCursor(_letterWheels[_wheelCursor]);
			ShiftWheel(_letterPlattings[_wheelCursor], -1, isTween: true);
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
	}

	public void Hide()
	{
	}

	private IEnumerator GeneratePuzzle()
	{
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		_seed = GlobalOptionsManager.Instance.GetSeedCombineWithMissionID();
		UnityEngine.Random.InitState(_seed);
		int num = UnityEngine.Random.Range(0, _isoCodes.Length);
		_answerOverride = _isoCodes[num].Code;
		if (_uiClue != null)
		{
			_uiClue._textObject[0].SetText(_isoCodes[num].Nation);
		}
		_charSetList = new List<char>();
		for (int i = 0; i < _charSet.Length; i++)
		{
			_charSetList.Add(_charSet[i]);
		}
		char[] array = new char[3];
		if (_answerOverride.Length >= 3)
		{
			for (int j = 0; j < array.Length; j++)
			{
				int index = _charSetList.IndexOf(_answerOverride[j]);
				array[j] = _charSetList[index];
				_charSetList.RemoveAt(index);
			}
		}
		_charWheelSets = new char[3][];
		int num2 = 0;
		for (int k = 0; k < _charWheelSets.Length; k++)
		{
			_charWheelSets[k] = new char[5];
			for (int l = 0; l < _charWheelSets[k].Length; l++)
			{
				int index2 = UnityEngine.Random.Range(0, _charSetList.Count);
				if (l == 0 && _answerOverride.Length >= 3)
				{
					_charWheelSets[k][l] = array[k];
				}
				else
				{
					_charWheelSets[k][l] = _charSetList[index2];
					_charSetList.RemoveAt(index2);
				}
				_letters[num2].text = _charWheelSets[k][l].ToString();
				num2++;
			}
		}
		_letterPlattings = new List<Transform>[3] { _letterPlattingA, _letterPlattingB, _letterPlattingC };
		List<Transform>[] letterPlattings = _letterPlattings;
		foreach (List<Transform> letterPlatting in letterPlattings)
		{
			LoopWheel(letterPlatting);
		}
		int num3 = 20;
		_shuffleData = new List<int>();
		for (int n = 0; n < num3; n++)
		{
			int item = UnityEngine.Random.Range(0, 3);
			_shuffleData.Add(item);
		}
		Shuffle();
		if (CompareCheck())
		{
			ShiftWheel(_letterPlattingA, 1, isTween: false);
			ShiftWheel(_letterPlattingB, -1, isTween: false);
			ShiftWheel(_letterPlattingC, 1, isTween: false);
		}
		_setupComplete = true;
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
	}

	private void Shuffle()
	{
		for (int i = 0; i < _shuffleData.Count; i++)
		{
			switch (_shuffleData[i])
			{
			case 0:
				ShiftWheel(_letterPlattingA, 1, isTween: false);
				break;
			case 1:
				ShiftWheel(_letterPlattingB, 1, isTween: false);
				break;
			case 2:
				ShiftWheel(_letterPlattingC, 1, isTween: false);
				break;
			}
		}
	}

	private void ShiftCursor(Image pointedWheel)
	{
		foreach (Image letterWheel in _letterWheels)
		{
			letterWheel.color = _inactive;
		}
		pointedWheel.color = _highlighted;
	}

	private void ShiftWheel(List<Transform> letterPlatting, int direction, bool isTween)
	{
		float num = Mathf.Sign(direction);
		foreach (Transform platting in letterPlatting)
		{
			Vector3 targetPosition = new Vector3(0f, platting.localPosition.y + 69f * num, 0f);
			if (isTween)
			{
				platting.transform.DOKill();
				platting.transform.DOLocalMove(targetPosition, 0.25f).OnComplete(() =>
				{
					SnapWhenComplete(letterPlatting, platting, targetPosition);
				});
			}
			else
			{
				platting.localPosition = targetPosition;
			}
		}
		if (_setupComplete)
		{
			AudioManager.PlaySFX("puzzle-combilock-letter-change");
		}
		if (!isTween)
		{
			LoopWheel(letterPlatting);
		}
	}

	private void SnapWhenComplete(List<Transform> letterPlatting, Transform platting, Vector3 position)
	{
		platting.localPosition = position;
		LoopWheel(letterPlatting);
		platting.transform.DOKill();
		if (_isAnimating)
		{
			_isAnimating = false;
		}
	}

	private void LoopWheel(List<Transform> letterPlatting)
	{
		foreach (Transform item in letterPlatting)
		{
			if (Mathf.Abs(item.localPosition.y) == 276f)
			{
				item.localPosition = new Vector3(0f, Mathf.Sign(item.localPosition.y) * 69f * -1f, 0f);
			}
			else if (Mathf.Abs(item.localPosition.y) == 207f)
			{
				item.localPosition = new Vector3(0f, Mathf.Sign(item.localPosition.y) * 138f * -1f, 0f);
			}
		}
		if (_setupComplete)
		{
			CompareResult();
		}
	}

	private void CompareResult()
	{
		if (CompareCheck() && !_success)
		{
			PuzzleSuccess();
		}
	}

	private bool CompareCheck()
	{
		for (int i = 0; i < _letterPlattings.Length; i++)
		{
			if (_letterPlattings[i][0].localPosition.y != 0f)
			{
				return false;
			}
		}
		return true;
	}

	private void PuzzleSuccess()
	{
		_success = true;
		_isAnimating = true;
		StartCoroutine(PuzzleUnlocked());
	}

	public IEnumerator PuzzleUnlocked()
	{
		_success = true;
		_isAnimating = true;
		yield return new WaitForSeconds(0.1f);
		AudioManager.PlaySFX("puzzle-combilock-switch-on");
		yield return new WaitForSeconds(0.5f);
		UIGameManager.Instance.ShowUIInGame(_interactableTrigger.UIMenu);
		NetworkGameManager.Instance.ownPlayer.network.ExecInteractObject((short)_interactableTrigger.UniqueID);
		NetworkGameManager.Instance.ownPlayer.itemCollision = null;
		NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
	}
}

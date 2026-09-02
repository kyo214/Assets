using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using Fusion.KCC;
using Toked;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleToolboxLock : MonoBehaviour, IPuzzle
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
	private Image _lockPlate;

	[SerializeField]
	private RectTransform[] _needles;

	[SerializeField]
	private Image _unlockImage;

	[Header("Data Input")]
	[SerializeField]
	private Color _highlighted;

	[SerializeField]
	private Color _inactive;

	[SerializeField]
	private Sprite[] _lockPlateFrames;

	[Header("Randomizer Setup")]
	[SerializeField]
	private CluePuzzle _uiClue;

	[Header("Sound Variables")]
	[SerializeField]
	private string _submitDeclined;

	[SerializeField]
	private string _metalUnlock;

	private int _seed;

	private List<Transform>[] _letterPlattings;

	private int _wheelCursor;

	private bool _isAnimating;

	private bool _setupComplete;

	private int[] _currentSymbolSet;

	private string _correctString;

	private string _currentString;

	private Button _unlockButton;

	private StringBuilder _stringBuilder;

	private List<int> _shuffleData;

	private bool _success;

	private ItemInteractable _interactableTrigger;

	private void Start()
	{
		_stringBuilder = new StringBuilder();
		_unlockButton = _unlockImage.GetComponent<Button>();
		StartCoroutine(GeneratePuzzle());
	}

	private void ResetPuzzle()
	{
		_wheelCursor = 0;
		_isAnimating = false;
		ShiftCursor(_letterWheels[_wheelCursor]);
	}

	public void GetSeed()
	{
		_seed = GlobalOptionsManager.Instance.GetSeedCombineWithMissionID();
	}

	public void Action1Press()
	{
		if (_wheelCursor == 3)
		{
			_unlockButton.Select();
			Submit();
		}
		else
		{
			_unlockButton.Select();
			Submit();
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
		if (_success || _isAnimating)
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
				num = 3;
			}
			_wheelCursor = num % 4;
			if (_wheelCursor == _letterPlattings.Length)
			{
				ShiftCursor(_unlockImage);
			}
			else
			{
				ShiftCursor(_letterWheels[_wheelCursor]);
			}
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
			if (_wheelCursor < _letterPlattings.Length)
			{
				ShiftWheel(_letterPlattings[_wheelCursor], (int)Mathf.Sign(direction.y), isTween: true);
			}
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
		GetSeed();
		_success = false;
		UnityEngine.Random.InitState(_seed);
		_currentSymbolSet = new int[3];
		int num = UnityEngine.Random.Range(1, 10);
		int num2 = UnityEngine.Random.Range(0, 12) * 5;
		_stringBuilder.Append(num);
		if (num2 < 10)
		{
			_stringBuilder.Append('0');
		}
		_stringBuilder.Append(num2);
		_correctString = _stringBuilder.ToString();
		float z = (float)num / 12f * 360f * -1f - (float)num2 / 60f * 30f;
		_needles[0].rotation = Quaternion.Euler(new Vector3(0f, 0f, z));
		_needles[1].rotation = Quaternion.Euler(new Vector3(0f, 0f, (float)num2 / 60f * 360f * -1f));
		_letterPlattings = new List<Transform>[3] { _letterPlattingA, _letterPlattingB, _letterPlattingC };
		List<Transform>[] letterPlattings = _letterPlattings;
		foreach (List<Transform> letterPlatting in letterPlattings)
		{
			LoopWheel(letterPlatting);
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
		_unlockImage.color = _inactive;
		pointedWheel.color = _highlighted;
	}

	private void ShiftWheel(List<Transform> letterPlatting, int direction, bool isTween)
	{
		float num = Mathf.Sign(direction);
		foreach (Transform platting in letterPlatting)
		{
			Vector3 targetPosition = new Vector3(0f, platting.localPosition.y + 48f * num, 0f);
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
		int num2 = KCCIListExtensions.IndexOf(_letterPlattings, letterPlatting);
		int num3 = _currentSymbolSet[num2] + direction;
		if (num3 < 0)
		{
			_currentSymbolSet[num2] = 9;
		}
		else
		{
			_currentSymbolSet[num2] = num3 % 10;
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
			if (item.localPosition.y >= 95f)
			{
				item.localPosition = new Vector3(0f, -384f, 0f);
			}
			else if (item.localPosition.y <= -431f)
			{
				item.localPosition = new Vector3(0f, 48f, 0f);
			}
		}
	}

	public void Submit()
	{
		if (!_success)
		{
			CompareResult();
		}
	}

	private void CompareResult()
	{
		if (!CompareCheck())
		{
			AudioManager.PlaySFX(_submitDeclined);
		}
		else if (!_success)
		{
			PuzzleSuccess();
		}
	}

	private bool CompareCheck()
	{
		_stringBuilder.Clear();
		for (int i = 0; i < _currentSymbolSet.Length; i++)
		{
			_stringBuilder.Append(_currentSymbolSet[i].ToString());
		}
		if (_stringBuilder.ToString() != _correctString)
		{
			return false;
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
		AudioManager.PlaySFX(_metalUnlock);
		for (int i = 0; i < _lockPlateFrames.Length; i++)
		{
			_lockPlate.sprite = _lockPlateFrames[i];
			yield return new WaitForSeconds(0.1f);
		}
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
}

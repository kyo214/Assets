using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Toked;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleLockpicks : MonoBehaviour, IPuzzle
{
	[Header("Object Reference")]
	[SerializeField]
	private Transform _lockPick;

	[SerializeField]
	private List<Image> _springs;

	[SerializeField]
	private List<Image> _pins;

	[SerializeField]
	private List<Image> _yellowPins;

	[SerializeField]
	private List<Image> _pinIndicator;

	[SerializeField]
	private Image _blinkingIndicator;

	[Header("Data Reference")]
	[SerializeField]
	private Material _flashMaterial;

	private ItemInteractable _interactableTrigger;

	[Header("Test Value")]
	[SerializeField]
	private int _testSeed;

	[SerializeField]
	private List<RectTransform> _rSprings;

	[SerializeField]
	private List<RectTransform> _rPins;

	[SerializeField]
	private List<RectTransform> _rYellowPins;

	[SerializeField]
	private List<RectTransform> _pinInit;

	private int _pinIndex;

	private int _controlState;

	[SerializeField]
	private RectTransform _pickLockInit;

	[Header("Mapper Setup")]
	private int _seed;

	[SerializeField]
	private int _offset;

	[SerializeField]
	private int _fixedPin = 2;

	[SerializeField]
	private int _fixedPinPhase2 = 1;

	[SerializeField]
	private int _lockpickingToolID;

	[SerializeField]
	private int _AdditionalFixedPinTool = 2;

	private void Start()
	{
		CacheComponents();
		GeneratePuzzle();
	}

	private void CacheComponents()
	{
		_rSprings = new List<RectTransform>();
		_rPins = new List<RectTransform>();
		_rYellowPins = new List<RectTransform>();
		for (int i = 0; i < _springs.Count; i++)
		{
			_rSprings.Add(_springs[i].GetComponent<RectTransform>());
		}
		for (int j = 0; j < _pins.Count; j++)
		{
			_rPins.Add(_pins[j].GetComponent<RectTransform>());
		}
		for (int k = 0; k < _yellowPins.Count; k++)
		{
			_rYellowPins.Add(_yellowPins[k].GetComponent<RectTransform>());
		}
	}

	private void GeneratePuzzle()
	{
		StartCoroutine(GetSeed());
		UnityEngine.Random.InitState(_seed + _offset);
		for (int i = 0; i < _pins.Count; i++)
		{
			int num = UnityEngine.Random.Range(4, 16);
			_rYellowPins[i].sizeDelta += Vector2.up * ((float)num * 4f);
			_rPins[i].localPosition += Vector3.up * _rYellowPins[i].sizeDelta.y;
			_pinInit[i].position = _rPins[i].position;
			_pinIndicator[i].gameObject.SetActive(value: false);
		}
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
	}

	private IEnumerator GetSeed()
	{
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		if (GameManagerPhoton.Instance.Phase > 0)
		{
			_fixedPin = _fixedPinPhase2;
		}
		_seed = GlobalOptionsManager.Instance.GetSeedCombineWithMissionID();
	}

	private void Update()
	{
		for (int i = 0; i < _rPins.Count; i++)
		{
			_rSprings[i].sizeDelta = new Vector2(_rSprings[i].sizeDelta.x, 218f - (_rPins[i].localPosition.y + (_rPins[i].sizeDelta.y - 90f)));
		}
	}

	public void Action1Press()
	{
	}

	public void Action1Release()
	{
		if (_controlState == 1)
		{
			PinPush();
		}
		else if (_controlState == 2)
		{
			LockPin();
		}
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
		_blinkingIndicator.gameObject.SetActive(value: false);
		AudioManager.PlaySFX("lockpick");
		if (NetworkGameManager.Instance.ownPlayer.data.FindInventory(_lockpickingToolID) != null)
		{
			StartCoroutine(UnlockPuzzleWithTool());
		}
		else
		{
			ResetPuzzle(_fixedPin);
		}
	}

	public void Hide()
	{
	}

	private void ResetPuzzle(int param_fixedPin)
	{
		_pinIndex = param_fixedPin;
		_lockPick.localPosition = _pickLockInit.localPosition;
		for (int i = 0; i < _pins.Count; i++)
		{
			_yellowPins[i].color = Color.yellow;
			_rPins[i].position = new Vector3(_rYellowPins[i].position.x, _pinInit[i].position.y, _rYellowPins[i].position.z);
			_rYellowPins[i].localPosition = new Vector3(_rYellowPins[i].localPosition.x, -3f, _rYellowPins[i].localPosition.z);
			if (i < param_fixedPin)
			{
				_rPins[i].localPosition = new Vector2(_rPins[i].localPosition.x, 98f);
				_rYellowPins[i].position = new Vector2(_rYellowPins[i].position.x, _pinInit[i].position.y);
			}
		}
		AlignPick();
	}

	private void AlignPick()
	{
		float duration = 0.5f;
		_lockPick.DOKill();
		_lockPick.DOMoveX(_rPins[_pinIndex].position.x, duration).SetEase(Ease.OutQuad).OnComplete(() =>
		{
			_controlState = 1;
		});
	}

	private void PinPush()
	{
		_controlState = 2;
		_lockPick.DOKill();
		_lockPick.position = new Vector2(_rPins[_pinIndex].position.x, _lockPick.position.y);
		_lockPick.DOLocalRotate(Vector3.forward * 3f, 0.1f).OnComplete(() =>
		{
			_lockPick.DOLocalRotate(Vector3.forward * -5f, 0.2f);
		});
		_blinkingIndicator.DOKill();
		_blinkingIndicator.color = Color.yellow * new Color(1f, 1f, 1f, 0.5f);
		Color endValue = Color.yellow * new Color(1f, 1f, 1f, 0.2f);
		_blinkingIndicator.DOColor(endValue, 0.2f).SetLoops(-1, LoopType.Yoyo);
		_blinkingIndicator.gameObject.SetActive(value: true);
		_rPins[_pinIndex].DOKill();
		_rPins[_pinIndex].DOLocalMoveY(215f, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
		{
			_rPins[_pinIndex].DOMoveY(_pinInit[_pinIndex].position.y, 0.5f).SetEase(Ease.InQuad).OnComplete(() =>
			{
				_controlState = 0;
				_pins[_pinIndex].DOKill();
				_pins[_pinIndex].color = Color.white;
				AudioManager.StopSFX("lockpick_loop");
				_blinkingIndicator.gameObject.SetActive(value: false);
				Invoke("InvokeResetControlState", 0.5f);
			});
		});
		AudioManager.PlaySFX("lockpick_loop");
	}

	private void LockPin()
	{
		_rPins[_pinIndex].DOKill();
		_controlState = 0;
		if (_rPins[_pinIndex].anchoredPosition.y > 45f && _rPins[_pinIndex].anchoredPosition.y < 130f)
		{
			AudioManager.StopSFX("lockpick_loop");
			AudioManager.PlaySFX("lockpick_done");
			BreakPin(_pinIndex, 0f);
			Invoke("NextPick", 0.5f);
			_blinkingIndicator.gameObject.SetActive(value: false);
			return;
		}
		AudioManager.StopSFX("lockpick_loop");
		_rPins[_pinIndex].DOMoveY(_pinInit[_pinIndex].position.y, 0.2f).SetEase(Ease.InQuad).SetDelay(0.5f)
			.OnComplete(() =>
			{
				_controlState = 1;
				_yellowPins[_pinIndex].color = Color.yellow;
			});
		_pinIndicator[_pinIndex].gameObject.SetActive(value: true);
		_pinIndicator[_pinIndex].DOKill();
		_pinIndicator[_pinIndex].color = Color.red;
		_pinIndicator[_pinIndex].DOColor(Color.red * new Color(1f, 1f, 1f, 0.5f), 0.2f).SetLoops(-1, LoopType.Yoyo);
		_blinkingIndicator.DOKill();
		_blinkingIndicator.color = Color.red;
		Color endValue = Color.red * new Color(1f, 1f, 1f, 0.2f);
		_blinkingIndicator.DOColor(endValue, 0.2f).SetLoops(-1, LoopType.Yoyo);
		Invoke("InvokeDisableIndicator", 0.5f);
	}

	private void BreakPin(int idx, float yOffset)
	{
		StartCoroutine(Flash(_pins[idx]));
		StartCoroutine(Flash(_yellowPins[idx]));
		_rPins[idx].localPosition = new Vector2(_rPins[idx].localPosition.x, 98f);
		_pins[idx].DOKill();
		_rYellowPins[idx].DOKill();
		_rYellowPins[idx].DOMoveY(_pinInit[idx].position.y + yOffset, 0.2f).SetEase(Ease.InQuad);
	}

	private void InvokeDisableIndicator()
	{
		foreach (Image item in _pinIndicator)
		{
			item.gameObject.SetActive(value: false);
		}
		_blinkingIndicator.gameObject.SetActive(value: false);
	}

	private IEnumerator Flash(Image pin)
	{
		pin.material = _flashMaterial;
		yield return new WaitForSeconds(0.05f);
		pin.material = null;
	}

	private void NextPick()
	{
		if (_pinIndex < 4)
		{
			_pinIndex++;
			AlignPick();
		}
		else
		{
			PuzzleSuccess();
		}
	}

	private void InvokeResetControlState()
	{
		_yellowPins[_pinIndex].color = Color.yellow;
		_controlState = 1;
	}

	private void PuzzleSuccess()
	{
		StartCoroutine(PuzzleUnlocked());
	}

	public IEnumerator UnlockPuzzleWithTool()
	{
		_blinkingIndicator.DOKill();
		_blinkingIndicator.color = Color.clear;
		for (int i = 0; i < _pins.Count; i++)
		{
			_rPins[i].localPosition = new Vector2(_rPins[i].localPosition.x, 98f);
			_pins[i].DOKill();
		}
		yield return new WaitForSeconds(0.5f);
		AudioManager.PlaySFX("lockpick_done");
		for (int j = 0; j < _pins.Count; j++)
		{
			BreakPin(j, 0f);
		}
		yield return new WaitForSeconds(3f);
		StartCoroutine(PuzzleUnlocked());
	}

	public IEnumerator PuzzleUnlocked()
	{
		_blinkingIndicator.DOKill();
		_interactableTrigger.isLocked = false;
		if (_interactableTrigger.animatorTrigger1 == null)
		{
			_interactableTrigger.isTriggered = false;
		}
		AudioManager.StopSFX("lockpick_loop");
		yield return new WaitForSeconds(0.2f);
		UIGameManager.Instance.ShowUIInGame(_interactableTrigger.UIMenu);
		NetworkGameManager.Instance.ownPlayer.network.ExecInteractObject((short)_interactableTrigger.UniqueID);
		NetworkGameManager.Instance.ownPlayer.itemCollision = null;
		NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
	}
}

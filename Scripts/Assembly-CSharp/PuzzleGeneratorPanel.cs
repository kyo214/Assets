using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzleGeneratorPanel : MonoBehaviour, IPuzzle, IPointerMoveHandler, IEventSystemHandler
{
	[Header("Component References")]
	[SerializeField]
	private RectTransform _avContainer;

	[SerializeField]
	private RectTransform _desiredAmpere;

	[SerializeField]
	private RectTransform _pointerAmpere;

	[SerializeField]
	private RectTransform _desiredVolt;

	[SerializeField]
	private RectTransform _pointerVolt;

	[SerializeField]
	private Image _indicatorLed;

	[SerializeField]
	private Sprite[] _ledStates;

	[SerializeField]
	private Image _barA;

	[SerializeField]
	private Sprite[] _barLightA;

	[SerializeField]
	private Image _barB;

	[SerializeField]
	private Sprite[] _barLightB;

	[SerializeField]
	private SwitchData _powerSwitch;

	[SerializeField]
	private SwitchData[] _switches;

	[SerializeField]
	private Color _highlighted;

	[SerializeField]
	private Color _inactive;

	[SerializeField]
	private Image[] _allSwitches;

	private float _containerWidth;

	private bool _switching;

	private int[] _switchState;

	private int[] _switchCorrectState;

	private int _isPower;

	private int _isSolved;

	private bool _isFirstPassed;

	private int _electricPowerState;

	private int _navIndex;

	private bool _navMode;

	private int _seed;

	[SerializeField]
	private ItemInteractable _interactableTrigger;

	private void Start()
	{
		_containerWidth = _avContainer.sizeDelta.x;
		StartCoroutine(GeneratePuzzle());
	}

	private IEnumerator GeneratePuzzle()
	{
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		UnityEngine.Random.InitState(GlobalOptionsManager.Instance.GetSeedCombineWithMissionID());
		UnityEngine.Random.InitState(UnityEngine.Random.Range(0, 6));
		_isFirstPassed = true;
		_isPower = 1;
		_isSolved = 0;
		int num = UnityEngine.Random.Range(1, 10);
		int num2 = UnityEngine.Random.Range(1, 10);
		_switchState = new int[_switches.Length];
		_switchCorrectState = new int[_switches.Length];
		_desiredAmpere.localPosition = new Vector2((float)num / 10f * _containerWidth, _desiredAmpere.localPosition.y);
		_desiredVolt.localPosition = new Vector2((float)num2 / 10f * _containerWidth, _desiredVolt.localPosition.y);
		for (int i = 0; i < _switches.Length; i++)
		{
			_switches[i].Ampere = UnityEngine.Random.Range(-2, 8);
			if (_switches[i].Ampere == 0)
			{
				_switches[i].Ampere = 1;
			}
			_switches[i].Volt = UnityEngine.Random.Range(-2, 8);
			if (_switches[i].Volt == 0)
			{
				_switches[i].Volt = -1;
			}
		}
		List<int> list = new List<int> { 0, 1, 2, 3 };
		for (int j = 0; j < 2; j++)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			int num3 = list[index];
			_switchCorrectState[num3] = 1;
			switch (j)
			{
			case 0:
				num -= _switches[num3].Ampere;
				num2 -= _switches[num3].Volt;
				break;
			case 1:
				_switches[num3].Ampere = num;
				_switches[num3].Volt = num2;
				break;
			}
			list.RemoveAt(index);
		}
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
		RestartPuzzle();
	}

	private void RestartPuzzle()
	{
		if (_isPower == 1 && _isSolved == 1)
		{
			_switchCorrectState.CopyTo(_switchState, 0);
		}
		else
		{
			_switchState = new int[_switches.Length];
		}
		for (int i = 0; i < _switches.Length; i++)
		{
			_switches[i].Switch.sprite = _switches[i].AnimationFrames[(_switches[i].AnimationFrames.Length - 1) * _switchCorrectState[i] * _isPower * _isSolved];
			_switches[i].State = _isSolved * _switchCorrectState[i];
		}
		_pointerAmpere.localPosition = new Vector2(_desiredAmpere.localPosition.x * (float)_isPower * (float)_isSolved, _pointerAmpere.localPosition.y);
		_pointerVolt.localPosition = new Vector2(_desiredVolt.localPosition.x * (float)_isPower * (float)_isSolved, _pointerVolt.localPosition.y);
		SetLed();
		_navIndex = 0;
		Highlight(noClear: true);
	}

	public void Action1Press()
	{
		if (_navMode && !_switching)
		{
			ClickSwitch(_navIndex - 1);
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
			_navMode = true;
			if (!_switching)
			{
				_navIndex += (int)Mathf.Sign(direction.x);
				if (_navIndex < 0)
				{
					_navIndex = 4;
				}
				_navIndex %= 5;
				Highlight(noClear: true);
			}
		}
		else if (Mathf.Abs(direction.y) > 0.5f)
		{
			_navMode = true;
			if (!_switching)
			{
				ClickSwitch(_navIndex - 1);
			}
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
		RestartPuzzle();
	}

	public void Hide()
	{
	}

	private void Highlight(bool noClear)
	{
		for (int i = 0; i < _allSwitches.Length; i++)
		{
			_allSwitches[i].DOKill();
			_allSwitches[i].color = Color.white;
		}
		if (noClear)
		{
			_allSwitches[_navIndex].DOColor(_inactive, 0.5f).SetLoops(-1, LoopType.Yoyo);
		}
	}

	public void JumpNav(int navindex)
	{
		_navIndex = navindex;
	}

	public void ClickSwitch(int idx)
	{
		if (idx >= 0)
		{
			StartCoroutine(ToggleSwitch(_switches[idx], power: false));
			if (_switches[idx] != null)
			{
				if (_switches[idx].State == 0)
				{
					AudioManager.PlaySFX("puzzle-generatorPanel-switch-down");
				}
				else
				{
					AudioManager.PlaySFX("puzzle-generatorPanel-switch-up");
				}
			}
			return;
		}
		StartCoroutine(ToggleSwitch(_powerSwitch, power: true));
		if (_powerSwitch != null)
		{
			if (_powerSwitch.State == 0)
			{
				AudioManager.PlaySFX("puzzle-generatorPanel-switch-on");
			}
			else
			{
				AudioManager.PlaySFX("puzzle-generatorPanel-switch-off");
			}
		}
	}

	private IEnumerator ToggleSwitch(SwitchData switchData, bool power)
	{
		bool flag = false;
		if (!_isFirstPassed && !power)
		{
			flag = true;
		}
		if (flag || _switching)
		{
			yield break;
		}
		_switching = true;
		switchData.State = (switchData.State + 1) % 2;
		Sprite[] animSet = new Sprite[switchData.AnimationFrames.Length];
		switchData.AnimationFrames.CopyTo(animSet, 0);
		if (switchData.State == 0)
		{
			Array.Reverse(animSet, 0, animSet.Length);
		}
		for (int i = 0; i < animSet.Length; i++)
		{
			switchData.Switch.sprite = animSet[i];
			yield return new WaitForSeconds(0.1f);
		}
		AnimatePointer();
		if (power && !CompareResult())
		{
			switchData.State = (switchData.State + 1) % 2;
			animSet = new Sprite[switchData.AnimationFrames.Length];
			switchData.AnimationFrames.CopyTo(animSet, 0);
			if (switchData.State == 0)
			{
				Array.Reverse(animSet, 0, animSet.Length);
			}
			for (int i = 0; i < animSet.Length; i++)
			{
				switchData.Switch.sprite = animSet[i];
				yield return new WaitForSeconds(0.1f);
			}
		}
		_switching = false;
	}

	private void AnimatePointer()
	{
		_pointerAmpere.transform.DOKill();
		_pointerVolt.transform.DOKill();
		float num = 0f;
		float num2 = 0f;
		if (_isPower == 1)
		{
			SwitchData[] switches = _switches;
			foreach (SwitchData switchData in switches)
			{
				num += (float)(switchData.Ampere * switchData.State);
				num2 += (float)(switchData.Volt * switchData.State);
			}
		}
		if (num > 10f)
		{
			num = 10f;
		}
		if (num < 0f)
		{
			num = 0f;
		}
		if (num2 > 10f)
		{
			num2 = 10f;
		}
		if (num2 < 0f)
		{
			num2 = 0f;
		}
		_pointerAmpere.transform.DOLocalMoveX(num / 10f * _containerWidth, 0.5f);
		_pointerVolt.transform.DOLocalMoveX(num2 / 10f * _containerWidth, 0.5f);
	}

	private void SetLed()
	{
		int num = (_isPower + _isSolved) % _ledStates.Length * _isPower;
		_indicatorLed.sprite = _ledStates[num];
		_barA.sprite = _barLightA[_isPower];
		_barB.sprite = _barLightB[_isPower];
		if (_electricPowerState == 0 && num == 2)
		{
			NotifyOn();
		}
		if (_electricPowerState == 1 && num != 2)
		{
			NotifyOff();
		}
		if (num == 2 && _isFirstPassed)
		{
			Success();
		}
	}

	private bool CompareResult()
	{
		bool result = true;
		_isSolved = 1;
		for (int i = 0; i < _switchCorrectState.Length; i++)
		{
			if (_switchCorrectState[i] != _switches[i].State)
			{
				_isSolved = 0;
				result = false;
				break;
			}
		}
		SetLed();
		return result;
	}

	private void Success()
	{
		StartCoroutine(PuzzleUnlocked());
	}

	public void NotifyOn()
	{
		_electricPowerState = 1;
	}

	public void NotifyOff()
	{
		_electricPowerState = 0;
		StartCoroutine(PuzzleUnlocked());
	}

	public void SyncState(int state)
	{
		switch (state)
		{
		case 0:
			_isFirstPassed = true;
			_isPower = 0;
			_isSolved = 0;
			break;
		case 1:
			_isFirstPassed = true;
			_isPower = 1;
			_isSolved = 0;
			break;
		case 2:
			_isFirstPassed = false;
			_isPower = 1;
			_isSolved = 1;
			break;
		}
		RestartPuzzle();
	}

	public IEnumerator PuzzleUnlocked()
	{
		yield return new WaitForSeconds(1f);
		UIGameManager.Instance.ShowUIInGame(_interactableTrigger.UIMenu);
		NetworkGameManager.Instance.ownPlayer.network.ExecInteractObject((short)_interactableTrigger.UniqueID);
		_interactableTrigger.DisableCollider();
		NetworkGameManager.Instance.ownPlayer.itemCollision = null;
		NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
		_isFirstPassed = false;
	}

	public void OnPointerMove(PointerEventData eventData)
	{
		_navMode = false;
		Highlight(noClear: false);
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleGeneratorSwitch : MonoBehaviour, IPuzzle
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
	private Image _powerSwitch;

	[SerializeField]
	private Image[] _switches;

	[Header("Animation Sprites")]
	[SerializeField]
	private Sprite _indicatorLedOn;

	[SerializeField]
	private Sprite _indicatorLedOff;

	[SerializeField]
	private Sprite _indicatorLedDead;

	[SerializeField]
	private Sprite _monitorOn;

	[SerializeField]
	private Sprite _monitorOff;

	[SerializeField]
	private List<Sprite> _switchTypeT;

	[SerializeField]
	private List<Sprite> _switchTypeA;

	[SerializeField]
	private List<Sprite> _switchTypeB;

	[SerializeField]
	private Color _highlighted;

	[Header("Test Value")]
	[SerializeField]
	private int _testSeed;

	private float _containerWidth;

	private bool _switching;

	private bool[] _switchAnimationSet;

	private bool[] _switchState;

	private int _navIndex;

	private bool _navAbsorbed;

	private int _randSeed;

	private Vector2Int[] _electricValue;

	private int[] _rightIndex;

	private int _desiredAmpereValue;

	private int _desiredVoltValue;

	private int _ampereValue;

	private int _voltValue;

	private bool _powerState;

	[SerializeField]
	private ItemInteractable interactableObject;

	private void Start()
	{
		GetSeed();
		_containerWidth = _avContainer.sizeDelta.x;
		GeneratePuzzle();
		_switchState = new bool[_switches.Length];
		_switchAnimationSet = new bool[_switches.Length];
		for (int i = 0; i < _switches.Length; i++)
		{
			if (i < 2)
			{
				_switchAnimationSet[i] = true;
			}
			else
			{
				_switchAnimationSet[i] = false;
			}
		}
	}

	private void GetSeed()
	{
		_randSeed = _testSeed;
	}

	public void Action1Press()
	{
		SwitchPress(_navIndex);
	}

	public void Action1Release()
	{
	}

	public ItemInteractable GetInteractableObject()
	{
		throw new NotImplementedException();
	}

	public void InitAnswer()
	{
	}

	public void Navigate(Vector2 direction)
	{
		if (Mathf.Abs(direction.x) > 0.5f && !_switching && !_navAbsorbed)
		{
			_navIndex += (int)Mathf.Sign(direction.x);
			if (_navIndex < 0)
			{
				_navIndex = 3;
			}
			_navIndex %= 4;
			Highlight();
		}
	}

	public void SetInteractableObject(ItemInteractable intObject)
	{
		interactableObject = intObject;
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

	private void GeneratePuzzle()
	{
		UnityEngine.Random.InitState(_randSeed);
		_electricValue = new Vector2Int[_switches.Length];
		for (int i = 0; i < _electricValue.Length; i++)
		{
			_electricValue[i].x = UnityEngine.Random.Range(-2, 8);
			if (_electricValue[i].x == 0)
			{
				_electricValue[i].x = 1;
			}
			_electricValue[i].y = UnityEngine.Random.Range(-2, 8);
			if (_electricValue[i].y == 0)
			{
				_electricValue[i].y = -1;
			}
		}
		_desiredAmpereValue = UnityEngine.Random.Range(1, 10);
		_desiredVoltValue = UnityEngine.Random.Range(1, 10);
		_desiredAmpere.localPosition = new Vector3((float)_desiredAmpereValue / 10f * _containerWidth, _desiredAmpere.localPosition.y, 1f);
		_desiredVolt.localPosition = new Vector3((float)_desiredVoltValue / 10f * _containerWidth, _desiredVolt.localPosition.y, 1f);
		int num = UnityEngine.Random.Range(2, 4);
		List<int> list = new List<int> { 0, 1, 2, 3 };
		int[] array = new int[num];
		for (int j = 0; j < array.Length; j++)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			array[j] = list[index];
			list.RemoveAt(index);
		}
		Vector2Int zero = Vector2Int.zero;
		for (int k = 0; k < array.Length - 1; k++)
		{
			zero.x += _electricValue[array[k]].x;
			zero.y += _electricValue[array[k]].y;
		}
		_electricValue[array[^1]].x = _desiredAmpereValue - zero.x;
		_electricValue[array[^1]].y = _desiredVoltValue - zero.y;
		_rightIndex = new int[_switches.Length];
		int[] array2 = array;
		foreach (int num2 in array2)
		{
			_rightIndex[num2] = 1;
		}
		list.Clear();
		Highlight();
		_powerState = true;
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
	}

	private void RestartPuzzle()
	{
		_ampereValue = 0;
		_voltValue = 0;
		if (_powerState)
		{
			_ampereValue = _desiredAmpereValue;
			_voltValue = _desiredVoltValue;
		}
		_pointerAmpere.transform.DOKill();
		_pointerVolt.transform.DOKill();
		_pointerAmpere.transform.localPosition = new Vector2((float)_ampereValue / 10f * _containerWidth, _pointerAmpere.transform.localPosition.y);
		_pointerVolt.transform.localPosition = new Vector2((float)_voltValue / 10f * _containerWidth, _pointerVolt.transform.localPosition.y);
		for (int i = 0; i < _switches.Length; i++)
		{
			if (i < 2)
			{
				_switches[i].sprite = _switchTypeB[_rightIndex[i] * (_switchTypeB.Count - 1)];
			}
			else
			{
				_switches[i].sprite = _switchTypeA[_rightIndex[i] * (_switchTypeA.Count - 1)];
			}
			_switchState[i] = false;
		}
	}

	private void Highlight()
	{
		Image[] switches = _switches;
		foreach (Image obj in switches)
		{
			obj.DOKill();
			obj.color = Color.white;
		}
		_switches[_navIndex].DOColor(_highlighted, 0.5f).SetLoops(-1, LoopType.Yoyo);
	}

	public void SwitchPress(int switchIndex)
	{
		if (!_switching)
		{
			_switching = true;
			StartCoroutine(AnimateSwitch(switchIndex));
		}
	}

	private IEnumerator AnimateSwitch(int switchIndex)
	{
		List<Sprite> switchType = ((!_switchAnimationSet[switchIndex]) ? _switchTypeA : _switchTypeB);
		if (_switchState[switchIndex])
		{
			for (int i = switchType.Count - 1; i >= 0; i--)
			{
				_switches[switchIndex].sprite = switchType[i];
				yield return new WaitForSeconds(0.05f);
			}
		}
		else
		{
			for (int i = 0; i < switchType.Count; i++)
			{
				_switches[switchIndex].sprite = switchType[i];
				yield return new WaitForSeconds(0.05f);
			}
		}
		_switchState[switchIndex] = !_switchState[switchIndex];
		_ampereValue = 0;
		_voltValue = 0;
		for (int j = 0; j < _switches.Length; j++)
		{
			if (_switchState[j])
			{
				_ampereValue += _electricValue[j].x;
				_voltValue += _electricValue[j].y;
			}
		}
		if (_ampereValue < 0)
		{
			_ampereValue = 0;
		}
		else if (_ampereValue > 10)
		{
			_ampereValue = 10;
		}
		if (_voltValue < 0)
		{
			_voltValue = 0;
		}
		else if (_voltValue > 10)
		{
			_voltValue = 10;
		}
		AnimatePointer();
	}

	private void AnimatePointer()
	{
		_pointerAmpere.transform.DOKill();
		_pointerVolt.transform.DOKill();
		_pointerAmpere.transform.DOLocalMoveX((float)_ampereValue / 10f * _containerWidth, 0.5f);
		_pointerVolt.transform.DOLocalMoveX((float)_voltValue / 10f * _containerWidth, 0.5f).OnComplete(() =>
		{
			CompareResult();
		});
		_switching = false;
	}

	private void CompareResult()
	{
		if (_ampereValue == _desiredAmpereValue && _voltValue == _desiredVoltValue)
		{
			Success();
		}
		_pointerAmpere.transform.DOKill();
		_pointerVolt.transform.DOKill();
	}

	private void Success()
	{
		_indicatorLed.sprite = _indicatorLedOn;
		StartCoroutine(PuzzleUnlocked());
	}

	public IEnumerator PuzzleUnlocked()
	{
		yield return new WaitForSeconds(1f);
		UIGameManager.Instance.ShowUIInGame(interactableObject.UIMenu);
		NetworkGameManager.Instance.ownPlayer.network.ExecInteractObject((short)interactableObject.UniqueID);
	}
}

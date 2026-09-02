using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleWaterPump : MonoBehaviour, IPuzzle
{
	[Serializable]
	public class LeverLight
	{
		public List<int> ListLight = new List<int>();
	}

	[SerializeField]
	private List<ItemInteractable> _listItemInteractable = new List<ItemInteractable>();

	[SerializeField]
	private List<Animator> _listLightAnimator = new List<Animator>();

	[SerializeField]
	private List<int> _listTempLeverSlot = new List<int>();

	[SerializeField]
	private List<LeverLight> _listLeverSlot = new List<LeverLight>();

	[SerializeField]
	private PuzzleNetworkBehaviour _puzzleNetworkBehaviour;

	[SerializeField]
	private ItemInteractable interactableObject;

	[SerializeField]
	private bool _initialized;

	[SerializeField]
	private int _solvedValue;

	[SerializeField]
	private int _currentValue;

	[SerializeField]
	private Animator _activateAnimatorAfterSolve;

	[SerializeField]
	private string _animationName;

	[SerializeField]
	private GameObject _objectToEnableOnSolve;

	private IEnumerator Start()
	{
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		OnInitialize();
	}

	private void OnEnable()
	{
		_puzzleNetworkBehaviour.OnIdxChange += OnValueChangeNetwork;
		_puzzleNetworkBehaviour.OnSpawned += OnInitialize;
	}

	private void OnDisable()
	{
		_puzzleNetworkBehaviour.OnIdxChange -= OnValueChangeNetwork;
		_puzzleNetworkBehaviour.OnSpawned -= OnInitialize;
	}

	private void OnInitialize()
	{
		if (_initialized || !_puzzleNetworkBehaviour.IsSpawned)
		{
			return;
		}
		if (_objectToEnableOnSolve != null)
		{
			_objectToEnableOnSolve.SetActive(value: false);
		}
		UnityEngine.Random.InitState(GlobalOptionsManager.Instance.GetSeedCombineWithMissionID());
		for (int i = 0; i < _listLightAnimator.Count; i++)
		{
			_listTempLeverSlot.Add(i);
		}
		for (int j = 0; j < _listItemInteractable.Count; j++)
		{
			LeverLight leverLight = new LeverLight();
			for (int k = 0; k < 2; k++)
			{
				int index = UnityEngine.Random.Range(0, _listTempLeverSlot.Count);
				leverLight.ListLight.Add(_listTempLeverSlot[index]);
				_listTempLeverSlot.RemoveAt(index);
				if (_listTempLeverSlot.Count == 0)
				{
					for (int l = 0; l < _listLightAnimator.Count; l++)
					{
						_listTempLeverSlot.Add(j);
					}
				}
			}
			_listLeverSlot.Add(leverLight);
		}
		List<int> list = new List<int>();
		list.Add(0);
		list.Add(1);
		list.Add(2);
		list.Add(3);
		for (int m = 0; m < 2; m++)
		{
			int index2 = UnityEngine.Random.Range(0, list.Count);
			_listItemInteractable[list[index2]].animatorTrigger1.Play(_listItemInteractable[m].animationName1[0]);
			_listItemInteractable[list[index2]].triggerOnReverse = true;
			list.RemoveAt(index2);
		}
		int num = 1;
		for (int n = 0; n < _listLightAnimator.Count; n++)
		{
			num = num * 10 + 1;
		}
		_solvedValue = num;
		_currentValue = num;
		while (_currentValue == _solvedValue)
		{
			list.Clear();
			list.Add(0);
			list.Add(1);
			list.Add(2);
			list.Add(3);
			for (int num2 = 0; num2 < 3; num2++)
			{
				int index3 = UnityEngine.Random.Range(0, list.Count);
				OnLeverInteraction(list[index3]);
				list.RemoveAt(index3);
			}
		}
		if (NetworkGameManager.Instance.isServer)
		{
			_puzzleNetworkBehaviour.currentIdx = _currentValue;
		}
		_initialized = true;
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
	}

	public void OnLeverInteraction(int indexLever)
	{
		int num = _puzzleNetworkBehaviour.currentIdx;
		if (!_initialized)
		{
			num = _currentValue;
		}
		List<int> list = new List<int>();
		for (int i = 0; i <= _listLightAnimator.Count; i++)
		{
			list.Add(num % 10);
			num /= 10;
		}
		list[_listLeverSlot[indexLever].ListLight[0]]++;
		if (list[_listLeverSlot[indexLever].ListLight[0]] > 1)
		{
			list[_listLeverSlot[indexLever].ListLight[0]] = 0;
		}
		if (_listLeverSlot[indexLever].ListLight[0] != _listLeverSlot[indexLever].ListLight[1])
		{
			list[_listLeverSlot[indexLever].ListLight[1]]++;
			if (list[_listLeverSlot[indexLever].ListLight[1]] > 1)
			{
				list[_listLeverSlot[indexLever].ListLight[1]] = 0;
			}
		}
		int num2 = 0;
		for (int num3 = list.Count - 1; num3 >= 0; num3--)
		{
			num2 = num2 * 10 + list[num3];
		}
		_currentValue = num2;
		if (_initialized)
		{
			_puzzleNetworkBehaviour.RPCChangeIdx(num2);
		}
	}

	public void OnValueChangeNetwork(int valueIdx)
	{
		TurnLightOnOff(valueIdx);
		if (valueIdx == _solvedValue && _initialized)
		{
			StartCoroutine(PuzzleUnlocked());
		}
	}

	private void TurnLightOnOff(int value)
	{
		List<int> list = new List<int>();
		for (int i = 0; i <= _listLightAnimator.Count; i++)
		{
			list.Add(value % 10);
			value /= 10;
		}
		for (int j = 0; j < _listLightAnimator.Count; j++)
		{
			if (list[j] == 1)
			{
				_listLightAnimator[j].Play("On");
			}
			else
			{
				_listLightAnimator[j].Play("Off");
			}
		}
	}

	public IEnumerator PuzzleUnlocked()
	{
		if (_objectToEnableOnSolve != null)
		{
			_objectToEnableOnSolve.SetActive(value: true);
		}
		for (int i = 0; i < _listItemInteractable.Count; i++)
		{
			_listItemInteractable[i].DisableCollider();
		}
		yield return new WaitForSeconds(0.2f);
		if (_activateAnimatorAfterSolve != null)
		{
			_activateAnimatorAfterSolve.Play(_animationName);
		}
	}

	public void Action1Press()
	{
	}

	public void Action1Release()
	{
	}

	public void SetInteractableObject(ItemInteractable intObject)
	{
		interactableObject = intObject;
	}

	public ItemInteractable GetInteractableObject()
	{
		return interactableObject;
	}

	public void InitAnswer()
	{
	}

	public void SetPassword(string pass)
	{
	}

	public void Show()
	{
	}

	public void Hide()
	{
	}

	public void Navigate(Vector2 direction)
	{
	}
}

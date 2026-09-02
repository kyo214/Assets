using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzleMemorySwitch : MonoBehaviour, IPuzzle, IPointerMoveHandler, IEventSystemHandler
{
	[Header("Internal")]
	[SerializeField]
	private Transform _ledsParent;

	[SerializeField]
	private Transform _switchesParent;

	[SerializeField]
	private RectTransform _switchesHighlight;

	[SerializeField]
	private Image[] _ledFuse;

	[Header("External")]
	[SerializeField]
	private Sprite[] _ledLib;

	[SerializeField]
	private Sprite[] _switchState = new Sprite[2];

	[SerializeField]
	private Sprite _switchStateInbetween;

	[SerializeField]
	private Sprite _ledFuseOn;

	private Image[] _leds;

	private Image[] _switches;

	private int[][] _ledStatus;

	private int _iMatch;

	private bool _isMatching;

	private bool _inputModeController;

	private int _navIndex;

	private bool _succeed;

	private ItemInteractable _interactableTrigger;

	private const string SFX_TOGGLE = "sfx-colorswitch-toggle";

	private const string SFX_ACTIVATE = "sfx-colorswitch-activate";

	private void Start()
	{
		_iMatch = -1;
		int childCount = _ledsParent.childCount;
		_ledStatus = new int[childCount][];
		for (int i = 0; i < childCount; i++)
		{
			_ledStatus[i] = new int[2];
		}
		_leds = new Image[childCount];
		_switches = new Image[childCount];
		for (int j = 0; j < childCount; j++)
		{
			_leds[j] = _ledsParent.GetChild(j).GetComponent<Image>();
			_switches[j] = _switchesParent.GetChild(j).GetComponent<Image>();
		}
		StartCoroutine(Generate());
	}

	private IEnumerator Generate()
	{
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		List<int> list = new List<int> { 1, 1, 2, 2, 3, 3, 4, 4 };
		List<int> list2 = new List<int>();
		UnityEngine.Random.InitState(GlobalOptionsManager.Instance.GetSeedCombineWithMissionID());
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			list2.Add(list[index]);
			list.RemoveAt(index);
		}
		for (int j = 0; j < list2.Count; j++)
		{
			_ledStatus[j][0] = list2[j];
		}
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
		ResetAll();
	}

	public void SwitchPressed(int idx)
	{
		Debug.Log("Pressed");
		if (_isMatching || _iMatch == idx)
		{
			return;
		}
		Debug.Log("1");
		if (_ledStatus[idx][1] == 1 || _succeed)
		{
			return;
		}
		Debug.Log("2");
		_switches[idx].sprite = _switchState[1];
		_leds[idx].sprite = _ledLib[_ledStatus[idx][0]];
		AudioManager.PlaySFX("sfx-colorswitch-toggle");
		if (_iMatch < 0)
		{
			_iMatch = idx;
			return;
		}
		_isMatching = true;
		if (_ledStatus[idx][0] == _ledStatus[_iMatch][0])
		{
			_ledStatus[_iMatch][1] = 1;
			_ledStatus[idx][1] = 1;
			CompareAll();
		}
		else
		{
			_ledStatus[_iMatch][1] = 0;
			_ledStatus[idx][1] = 0;
			UniTaskUtil.DelayedCall(this, 0.3f, () =>
			{
				ResetAll();
			}).Forget();
		}
		_iMatch = -1;
	}

	private void ResetUnmatch()
	{
		for (int i = 0; i < _ledStatus.Length; i++)
		{
			if (_ledStatus[i][1] == 0)
			{
				_switches[i].sprite = _switchState[0];
				_leds[i].sprite = _ledLib[0];
			}
		}
		_isMatching = false;
	}

	private void ResetAll(bool init = false)
	{
		for (int i = 0; i < _ledStatus.Length; i++)
		{
			if (_switches[i] != null)
			{
				_switches[i].sprite = _switchState[0];
				_leds[i].sprite = _ledLib[0];
				_ledStatus[i][1] = 0;
			}
		}
		if (!init)
		{
			AudioManager.PlaySFX("sfx-colorswitch-toggle");
		}
		_isMatching = false;
	}

	private void CompareAll()
	{
		for (int i = 0; i < _ledStatus.Length; i++)
		{
			if (_ledStatus[i][1] == 0)
			{
				_isMatching = false;
				return;
			}
		}
		_succeed = true;
		StartCoroutine(PuzzleUnlocked());
	}

	public void Action1Press()
	{
		if (_inputModeController)
		{
			SwitchPressed(_navIndex);
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
		if ((!(Mathf.Abs(direction.x) > 0.5f) && !(Mathf.Abs(direction.y) > 0.5f)) || _isMatching)
		{
			return;
		}
		if (_inputModeController)
		{
			if (Mathf.Abs(direction.x) > 0.5f)
			{
				_navIndex += (int)Mathf.Sign(direction.x);
				if (_navIndex < 0)
				{
					_navIndex = _switches.Length - 1;
				}
				else
				{
					_navIndex %= _switches.Length;
				}
				_switchesHighlight.position = _switches[_navIndex].transform.position;
			}
		}
		else
		{
			_inputModeController = true;
			_switchesHighlight.gameObject.SetActive(value: true);
		}
	}

	public IEnumerator PuzzleUnlocked()
	{
		yield return new WaitForSeconds(1f);
		for (int i = 0; i < _ledFuse.Length; i++)
		{
			_ledFuse[i].sprite = _ledFuseOn;
		}
		AudioManager.PlaySFX("sfx-colorswitch-activate");
		yield return new WaitForSeconds(1f);
		UIGameManager.Instance.ShowUIInGame(_interactableTrigger.UIMenu);
		NetworkGameManager.Instance.ownPlayer.network.ExecInteractObject((short)_interactableTrigger.UniqueID, triggerOnReverse: false, isForceInteract: true);
		_interactableTrigger.DisableCollider();
		NetworkGameManager.Instance.ownPlayer.itemCollision = null;
		NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
		yield return new WaitForSeconds(2f);
		if ((bool)_interactableTrigger.doorCollider)
		{
			Bounds bounds = new Bounds(_interactableTrigger.doorCollider.bounds.center, _interactableTrigger.doorCollider.bounds.size * 2f);
			_interactableTrigger.doorCollider.enabled = false;
			GameManager.Instance.AStarPath.UpdateGraphs(bounds);
			GameManager.Instance.AStarPath.FlushGraphUpdates();
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
		ResetAll();
		_switchesHighlight.gameObject.SetActive(_inputModeController);
		_navIndex = 0;
	}

	public void OnPointerMove(PointerEventData eventData)
	{
		_inputModeController = false;
		_switchesHighlight.gameObject.SetActive(value: false);
	}
}

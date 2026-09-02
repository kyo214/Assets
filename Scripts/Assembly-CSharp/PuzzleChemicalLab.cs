using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DarkTonic.MasterAudio;
using Fusion.KCC;
using Toked;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleChemicalLab : MonoBehaviour, IPuzzle
{
	[Header("Asset References")]
	[SerializeField]
	private Sprite[] _chemicalColors;

	[Header("Child Components")]
	[SerializeField]
	private Transform[] _columnsTop;

	[SerializeField]
	private Transform[] _columnsBot;

	[SerializeField]
	private Transform _chemicHolder;

	[SerializeField]
	private Image[] _indicatorLeds;

	[SerializeField]
	private RectTransform _fluidTop;

	[SerializeField]
	private RectTransform _fluidFill;

	[SerializeField]
	private List<RectTransform>[] _pinnedPos;

	[Header("Sound Variables")]
	[SerializeField]
	private string _digitBeep;

	[SerializeField]
	private string _ledColorBeep;

	[SerializeField]
	private string _liquidFill;

	[SerializeField]
	private string _beepDecline;

	private List<Image>[] _stackHolder;

	private List<Image> _carry;

	private int _seed;

	private bool _success;

	private bool _holding;

	private int _highlightIndex;

	private Dictionary<string, Transform> _ledDictionary;

	private const float OFFSET = 3.5f;

	private ItemInteractable _interactableTrigger;

	private IEnumerator Start()
	{
		_ledDictionary = new Dictionary<string, Transform>();
		for (int i = 0; i < _indicatorLeds.Length; i++)
		{
			_ledDictionary.Add(_chemicalColors[i].name, _indicatorLeds[i].transform);
		}
		_success = false;
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		_seed = GlobalOptionsManager.Instance.GetSeedCombineWithMissionID();
		ResetPuzzle();
	}

	private void Generate()
	{
		UnityEngine.Random.InitState(_seed);
		if (_stackHolder != null)
		{
			for (int i = 0; i < _stackHolder.Length; i++)
			{
				_stackHolder[i].Clear();
			}
			_stackHolder.Clear();
		}
		if (_carry != null)
		{
			_carry.Clear();
		}
		_chemicHolder.DestroyAllChildren();
		_stackHolder = new List<Image>[4];
		_carry = new List<Image>();
		for (int j = 0; j < 4; j++)
		{
			_stackHolder[j] = new List<Image>();
		}
		List<int> list = new List<int>
		{
			0, 0, 0, 0, 1, 1, 1, 1, 2, 2,
			2, 2
		};
		for (int k = 0; k < 3; k++)
		{
			for (int l = 0; l < 4; l++)
			{
				GameObject obj = new GameObject("Chemic_");
				Image image = obj.AddComponent<Image>();
				int index = UnityEngine.Random.Range(0, list.Count);
				image.sprite = _chemicalColors[list[index]];
				list.RemoveAt(index);
				obj.transform.parent = _chemicHolder;
				obj.transform.localPosition = new Vector3(_columnsBot[k].localPosition.x + 3.5f, _columnsBot[k].localPosition.y - 12f + (float)(9 * l), _columnsBot[k].localPosition.z);
				image.SetNativeSize();
				obj.transform.localScale = Vector3.one;
				_stackHolder[k].Add(image);
			}
		}
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
	}

	private void ResetPuzzle()
	{
		if (!_success)
		{
			Generate();
			for (int i = 0; i < _indicatorLeds.Length; i++)
			{
				_indicatorLeds[i].gameObject.SetActive(value: false);
			}
			_fluidTop.gameObject.SetActive(value: false);
			_fluidFill.gameObject.SetActive(value: false);
			_highlightIndex = 0;
			HighlightColumn();
		}
	}

	public void Show()
	{
		ResetPuzzle();
	}

	public void Hide()
	{
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
		if (_success)
		{
			return;
		}
		if (Mathf.Abs(direction.x) > 0.5f)
		{
			if (direction.x > 0.5f)
			{
				_highlightIndex = (_highlightIndex + 1) % _columnsBot.Length;
			}
			else if (direction.x < -0.5f)
			{
				if (_highlightIndex > 0)
				{
					_highlightIndex--;
				}
				else
				{
					_highlightIndex = _columnsBot.Length - 1;
				}
			}
			HighlightColumn();
			if (_holding)
			{
				MoveCarry();
			}
		}
		else if (_holding)
		{
			if (direction.y < -0.5f)
			{
				Transport();
			}
		}
		else if (direction.y > 0.5f && _stackHolder[_highlightIndex].Count > 0)
		{
			Transport();
		}
	}

	private void Transport()
	{
		if (_holding)
		{
			if (4 - _stackHolder[_highlightIndex].Count < _carry.Count)
			{
				AudioManager.PlaySFX(_beepDecline);
				return;
			}
			for (int i = 0; i < _carry.Count; i++)
			{
				_stackHolder[_highlightIndex].Add(_carry[i]);
				_carry[i].transform.localPosition = new Vector3(_columnsBot[_highlightIndex].localPosition.x + 3.5f, _columnsBot[_highlightIndex].localPosition.y - 12f + (float)(9 * (_stackHolder[_highlightIndex].Count - 1)), _columnsBot[_highlightIndex].localPosition.z);
			}
			AudioManager.PlaySFX(_digitBeep);
			_carry.Clear();
			_holding = false;
			HighlightColumn();
		}
		else
		{
			_carry.Clear();
			Sprite sprite = _stackHolder[_highlightIndex].Last().sprite;
			int num = _stackHolder[_highlightIndex].Count - 1;
			while (num >= 0 && _stackHolder[_highlightIndex][num].sprite == sprite)
			{
				_carry.Add(_stackHolder[_highlightIndex][num]);
				num--;
			}
			if (_carry.Count > 3)
			{
				_carry.Clear();
				return;
			}
			_stackHolder[_highlightIndex].RemoveRange(_stackHolder[_highlightIndex].Count - _carry.Count, _carry.Count);
			AudioManager.PlaySFX(_digitBeep);
			MoveCarry();
			_holding = true;
			HighlightColumn();
		}
		CheckFilled();
	}

	private void ChangeUpperBarState()
	{
	}

	private void MoveCarry()
	{
		for (int i = 0; i < _carry.Count; i++)
		{
			_carry[i].transform.localPosition = new Vector3(_columnsTop[_highlightIndex].localPosition.x + 1f, _columnsTop[_highlightIndex].localPosition.y - 9.5f + (float)(9 * i), _columnsTop[_highlightIndex].localPosition.z);
		}
	}

	private void HighlightColumn()
	{
		for (int i = 0; i < _columnsTop.Length; i++)
		{
			bool active = false;
			if (i == _highlightIndex && _holding)
			{
				active = true;
			}
			_columnsTop[i].GetChild(0).gameObject.SetActive(active);
			if (4 - _stackHolder[i].Count < _carry.Count)
			{
				_columnsTop[i].GetChild(1).gameObject.SetActive(value: true);
			}
			else
			{
				_columnsTop[i].GetChild(1).gameObject.SetActive(value: false);
			}
		}
		for (int j = 0; j < _columnsBot.Length; j++)
		{
			bool active2 = false;
			if (j == _highlightIndex && !_holding)
			{
				active2 = true;
			}
			_columnsBot[j].GetChild(0).gameObject.SetActive(active2);
		}
	}

	public void SetInteractableObject(ItemInteractable intObject)
	{
		_interactableTrigger = intObject;
	}

	public void SetPassword(string pass)
	{
	}

	private void CheckFilled()
	{
		Image[] indicatorLeds = _indicatorLeds;
		for (int i = 0; i < indicatorLeds.Length; i++)
		{
			indicatorLeds[i].gameObject.SetActive(value: false);
		}
		int num = 0;
		for (int j = 0; j < _stackHolder.Length; j++)
		{
			if (_stackHolder[j].Count <= 0)
			{
				continue;
			}
			Sprite sprite = _stackHolder[j][0].sprite;
			int num2 = 0;
			for (int k = 0; k < _stackHolder[j].Count; k++)
			{
				if (_stackHolder[j][k].sprite == sprite)
				{
					num2++;
				}
			}
			if (num2 >= 4)
			{
				AudioManager.PlaySFX(_ledColorBeep);
				_ledDictionary[sprite.name].gameObject.SetActive(value: true);
				num++;
			}
		}
		if (num == _indicatorLeds.Length)
		{
			_success = true;
			StartCoroutine(PuzzleUnlocked());
		}
	}

	public IEnumerator PuzzleUnlocked()
	{
		_success = true;
		yield return new WaitForSeconds(0.1f);
		AudioManager.PlaySFX("puzzle-combilock-switch-on");
		yield return new WaitForSeconds(0.5f);
		_fluidTop.gameObject.SetActive(value: true);
		_fluidFill.gameObject.SetActive(value: true);
		AudioManager.PlaySFX(_liquidFill);
		_fluidTop.DOLocalMoveY(14.5f, 2f);
		_fluidFill.DOLocalMoveY(0f, 2f);
		yield return new WaitForSeconds(2.5f);
		if ((bool)_interactableTrigger.UIMenu)
		{
			_interactableTrigger.UIMenu.Hide();
		}
		UIGameManager.Instance.ShowUIInGame(_interactableTrigger.UIMenu);
		NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: true);
		ChatSystem.Instance.ItemCommand.SetActive(value: false);
		UIGameManager.Instance.mapUI.SetActive(value: true);
		NetworkGameManager.Instance.ownPlayer.network.ExecInteractObject((short)_interactableTrigger.UniqueID);
		NetworkGameManager.Instance.ownPlayer.itemCollision = null;
		NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
	}
}

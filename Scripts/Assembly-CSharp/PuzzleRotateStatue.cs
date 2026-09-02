using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Toked;
using UnityEngine;

public class PuzzleRotateStatue : MonoBehaviour, IPuzzle
{
	private int _seed;

	private bool _success;

	[Header("Statue")]
	[SerializeField]
	private List<Transform> _listStatue = new List<Transform>();

	[SerializeField]
	private int[] _listRotationToUnlock = new int[4];

	[SerializeField]
	private List<Sprite> _listSpriteMoon = new List<Sprite>();

	[SerializeField]
	private ItemInteractable _interactableTrigger;

	[SerializeField]
	private PuzzleStatuePaper _puzzleStatuePaper;

	private void Start()
	{
		StartCoroutine(GeneratePuzzle());
	}

	public IEnumerator GeneratePuzzle()
	{
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		_seed = GlobalOptionsManager.Instance.GetSeedCombineWithMissionID();
		UnityEngine.Random.InitState(_seed);
		List<int> list = new List<int>();
		for (int i = 0; i < 8; i++)
		{
			list.Add(i * 45);
		}
		Sprite[] array = new Sprite[4];
		for (int j = 0; j < _listRotationToUnlock.Length; j++)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			_listRotationToUnlock[j] = list[index];
			list.RemoveAt(index);
			array[j] = _listSpriteMoon[_listRotationToUnlock[j] / 45];
		}
		_puzzleStatuePaper.SetMoon(array);
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
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
		AudioManager.PlaySFX("puzzle-statue-rotate");
		UniTaskUtil.DelayedCall(this, 1f, () =>
		{
			CheckPuzzle();
		}).Forget();
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
		_ = _success;
	}

	public void Hide()
	{
	}

	public void DebugSolvePuzzle()
	{
		_success = true;
		StartCoroutine(PuzzleUnlocked());
	}

	public void CheckPuzzle()
	{
		bool flag = true;
		for (int i = 0; i < _listRotationToUnlock.Length; i++)
		{
			if (Mathf.Abs(_listStatue[i].transform.localEulerAngles.y - (float)_listRotationToUnlock[i]) > 0.1f)
			{
				flag = false;
			}
		}
		if (flag)
		{
			_success = true;
			StartCoroutine(PuzzleUnlocked());
		}
	}

	public IEnumerator PuzzleUnlocked()
	{
		_success = true;
		if (NetworkGameManager.Instance.isServer)
		{
			NetworkGameManager.Instance.ownPlayer.network.ExecInteractObject((short)_interactableTrigger.UniqueID);
		}
		yield return new WaitForSeconds(0.1f);
	}
}

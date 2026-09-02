using System;
using System.Collections;
using DG.Tweening;
using Toked;
using UnityEngine;

public class PuzzleCrestDisc : MonoBehaviour, IPuzzle
{
	[SerializeField]
	private RectTransform[] _crestPieces;

	[SerializeField]
	private RectTransform _centerPiece;

	[SerializeField]
	private RectTransform[] _lockPieces;

	[SerializeField]
	private RectTransform _BoardPiece;

	[Header("Sound variables")]
	[SerializeField]
	private string _sfxDiscRotate;

	[SerializeField]
	private string _sfxInnerCrestRotate;

	[SerializeField]
	private string _sfxPadUnlock;

	[SerializeField]
	private string _sfxBoxUnlock;

	private Vector3[] _savedAngles;

	private bool _isAnimating;

	private int _activeCursor;

	private int _seed;

	private bool _success;

	private ItemInteractable _interactableTrigger;

	private void Start()
	{
		StartCoroutine(GetSeed());
		GeneratePuzzle();
	}

	private void ResetPuzzle()
	{
		_success = false;
		for (int i = 0; i < _crestPieces.Length; i++)
		{
			_crestPieces[i].eulerAngles = _savedAngles[i];
		}
		_activeCursor = 0;
		Highlight();
	}

	public IEnumerator GetSeed()
	{
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		_seed = GlobalOptionsManager.Instance.GetSeedCombineWithMissionID();
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
		if (_success || _isAnimating)
		{
			return;
		}
		if (direction.x > 0.5f || direction.x < -0.5f)
		{
			int dir = 0;
			if (direction.x > 0.5f)
			{
				dir = -1;
			}
			else if (direction.x < -0.5f)
			{
				dir = 1;
			}
			Rotate(_activeCursor, dir);
		}
		else if ((direction.y > 0.5f || direction.y < -0.5f) && !_success)
		{
			int num = 0;
			if (direction.y > 0.5f)
			{
				num = -1;
			}
			else if (direction.y < -0.5f)
			{
				num = 1;
			}
			int num2 = _activeCursor + num;
			if (num2 >= 0)
			{
				_activeCursor = num2 % _crestPieces.Length;
			}
			else
			{
				_activeCursor = _crestPieces.Length - 1;
			}
			Highlight();
		}
	}

	private void Highlight()
	{
		for (int i = 0; i < _crestPieces.Length; i++)
		{
			_crestPieces[i].GetChild(0).gameObject.SetActive(value: false);
		}
		_crestPieces[_activeCursor].GetChild(0).gameObject.SetActive(value: true);
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
		if (!_success)
		{
			ResetPuzzle();
		}
	}

	public void Hide()
	{
	}

	private void GeneratePuzzle()
	{
		UnityEngine.Random.InitState(_seed);
		int num = 30;
		for (int i = 0; i < num; i++)
		{
			int idx = UnityEngine.Random.Range(0, _crestPieces.Length);
			RotateInstant(idx);
		}
		_savedAngles = new Vector3[_crestPieces.Length];
		for (int j = 0; j < _savedAngles.Length; j++)
		{
			_savedAngles[j] = _crestPieces[j].eulerAngles;
		}
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
	}

	public void Rotate(int idx, int dir)
	{
		if (!_success && !_isAnimating)
		{
			_isAnimating = true;
			Vector3 nextRot = new Vector3(0f, 0f, _crestPieces[idx].eulerAngles.z + 15f * (float)dir);
			_crestPieces[idx].DORotate(nextRot, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
			{
				_crestPieces[idx].eulerAngles = nextRot;
				CompareResult();
				_isAnimating = false;
			});
			AudioManager.PlaySFX(_sfxDiscRotate);
		}
	}

	private void RotateInstant(int idx)
	{
		Vector3 eulerAngles = new Vector3(0f, 0f, _crestPieces[idx].eulerAngles.z + 45f);
		_crestPieces[idx].eulerAngles = eulerAngles;
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
		RectTransform[] crestPieces = _crestPieces;
		for (int i = 0; i < crestPieces.Length; i++)
		{
			crestPieces[i].GetChild(1).gameObject.SetActive(value: false);
		}
		for (int j = 0; j < _crestPieces.Length; j++)
		{
			if (_crestPieces[j].eulerAngles.z > 10f && _crestPieces[j].eulerAngles.z < 350f)
			{
				return false;
			}
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
		_success = true;
		yield return new WaitForSeconds(0.1f);
		AudioManager.PlaySFX(_sfxInnerCrestRotate);
		_centerPiece.DORotate(new Vector3(0f, 0f, -180f), 1.5f).SetEase(Ease.Linear);
		yield return new WaitForSeconds(0.25f);
		_lockPieces[0].DOMoveY(_lockPieces[0].position.y + 32f, 0.5f);
		AudioManager.PlaySFX(_sfxPadUnlock);
		yield return new WaitForSeconds(0.25f);
		_lockPieces[1].DOMoveX(_lockPieces[1].position.x + 32f, 0.5f);
		AudioManager.PlaySFX(_sfxPadUnlock);
		yield return new WaitForSeconds(0.25f);
		_lockPieces[2].DOMoveY(_lockPieces[2].position.y - 32f, 0.5f);
		AudioManager.PlaySFX(_sfxPadUnlock);
		yield return new WaitForSeconds(0.25f);
		_lockPieces[3].DOMoveX(_lockPieces[3].position.x - 32f, 0.5f);
		AudioManager.PlaySFX(_sfxPadUnlock);
		yield return new WaitForSeconds(0.7f);
		_BoardPiece.DOMoveY(_BoardPiece.position.y + _BoardPiece.sizeDelta.y * 4f, 0.5f);
		AudioManager.PlaySFX(_sfxBoxUnlock);
		yield return new WaitForSeconds(1f);
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

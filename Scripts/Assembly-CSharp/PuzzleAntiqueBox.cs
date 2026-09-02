using System;
using System.Collections;
using DG.Tweening;
using Doozy.Runtime.UIManager.Containers;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzleAntiqueBox : MonoBehaviour, IPuzzle, IPointerMoveHandler, IEventSystemHandler
{
	[Header("Transform Reference")]
	[SerializeField]
	private RectTransform _frontBoard;

	[SerializeField]
	private Transform[] _tokens;

	[Header("Puzzle Properties")]
	[SerializeField]
	private Color _normal;

	[SerializeField]
	private Color _highlighted;

	[Header("Data Reference")]
	[SerializeField]
	private int _spawnItemById;

	[Header("Link Object")]
	[SerializeField]
	private float _initRotOffset;

	[SerializeField]
	private Transform _wallClue;

	[SerializeField]
	private CluePuzzle _uiClue;

	private Transform[] _tokenLinks;

	private int[] _tokenRotation;

	private Image[] _tokenImages;

	private bool _rotating;

	private int _seed;

	private bool _success;

	private int _pointer;

	private bool _isNav;

	private int[] _overrides;

	private ItemInteractable _interactableTrigger;

	private void Start()
	{
		_tokenImages = new Image[_tokens.Length];
		for (int i = 0; i < _tokens.Length; i++)
		{
			_tokenImages[i] = _tokens[i].GetChild(0).GetComponent<Image>();
		}
		_overrides = new int[_tokens.Length];
		StartCoroutine(GeneratePuzzle());
	}

	public void OnPointerMove(PointerEventData eventData)
	{
		_isNav = false;
	}

	private IEnumerator GeneratePuzzle()
	{
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		_seed = GlobalOptionsManager.Instance.GetSeedCombineWithMissionID();
		Transform[] tokens = _tokens;
		for (int i = 0; i < tokens.Length; i++)
		{
			tokens[i].rotation = Quaternion.Euler(Vector3.zero);
		}
		_rotating = false;
		UnityEngine.Random.InitState(_seed);
		for (int j = 0; j < _tokens.Length; j++)
		{
			int num = UnityEngine.Random.Range(0, 4);
			_tokens[j].GetChild(0).localRotation = Quaternion.Euler((float)num * 90f * Vector3.forward);
			_overrides[j] = num;
		}
		if (_wallClue != null && _wallClue.GetChild(0).childCount == _overrides.Length)
		{
			for (int k = 0; k < _overrides.Length; k++)
			{
				_wallClue.GetChild(0).GetChild(k).localRotation = Quaternion.Euler(Vector3.forward * (_initRotOffset + -90f * (float)_overrides[k]));
			}
		}
		if (_uiClue != null && _wallClue.GetChild(0).childCount == _overrides.Length)
		{
			for (int l = 0; l < _overrides.Length; l++)
			{
				_uiClue._imageObject[l].transform.rotation = Quaternion.Euler(Vector3.forward * (90f * (float)_overrides[l]));
			}
		}
		_tokenLinks = new Transform[_tokens.Length];
		_tokenRotation = new int[_tokens.Length];
		for (int m = 0; m < _tokens.Length; m++)
		{
			int num2 = UnityEngine.Random.Range(0, _tokens.Length);
			if (_tokens[m] == _tokens[num2])
			{
				num2 = (num2 + 1) % _tokens.Length;
			}
			_tokenLinks[m] = _tokens[num2];
		}
		for (int n = 0; n < 10; n++)
		{
			RotateInstant(UnityEngine.Random.Range(0, _tokens.Length));
		}
		int num3 = 0;
		for (int num4 = 0; num4 < _tokens.Length; num4++)
		{
			if (_tokenRotation[num4] == 0)
			{
				num3++;
			}
		}
		if (num3 >= 3)
		{
			RotateInstant(UnityEngine.Random.Range(0, _tokens.Length));
		}
		Highlight(0);
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
	}

	private void Highlight(int id)
	{
		Image[] tokenImages = _tokenImages;
		foreach (Image obj in tokenImages)
		{
			obj.DOKill();
			obj.color = _normal;
		}
		_tokenImages[id].DOColor(_highlighted, 0.5f).SetLoops(-1, LoopType.Yoyo);
	}

	public void Interact(int id)
	{
		if (!_success && !_rotating)
		{
			RotateToken(id, 1);
			_pointer = id;
			Highlight(_pointer);
		}
	}

	private void RotateInstant(int id)
	{
		if (id < _tokens.Length)
		{
			_tokenRotation[id] = (_tokenRotation[id] + 1) % 4;
			_tokens[id].localRotation = Quaternion.Euler((float)_tokenRotation[id] * 90f * Vector3.forward);
			_tokenRotation[_tokenLinks[id].GetSiblingIndex()] = (_tokenRotation[_tokenLinks[id].GetSiblingIndex()] + 1) % 4;
			_tokenLinks[id].localRotation = Quaternion.Euler((float)_tokenRotation[_tokenLinks[id].GetSiblingIndex()] * 90f * Vector3.forward);
		}
	}

	private void RotateToken(int id, int clockWise)
	{
		if (clockWise != 0 && id < _tokens.Length)
		{
			_rotating = true;
			_tokens[id].DOKill();
			_tokenLinks[id].DOKill();
			_tokenRotation[id] = Shift(_tokenRotation[id], clockWise, 4);
			_tokens[id].DOLocalRotate((float)_tokenRotation[id] * 90f * Vector3.forward, 0.75f).SetEase(Ease.InQuad);
			_tokenRotation[_tokenLinks[id].GetSiblingIndex()] = Shift(_tokenRotation[_tokenLinks[id].GetSiblingIndex()], clockWise, 4);
			_tokenLinks[id].DOLocalRotate((float)_tokenRotation[_tokenLinks[id].GetSiblingIndex()] * 90f * Vector3.forward, 0.75f).SetEase(Ease.InQuad).OnComplete(() =>
			{
				CompareResult();
			});
			AudioManager.PlaySFX("puzzle-antiquebox-rotate");
		}
	}

	private int Shift(int number, int dir, int max)
	{
		int num = ((number + dir >= 0) ? (number + dir) : (max - 1));
		return num % max;
	}

	private void CompareResult()
	{
		for (int i = 0; i < _tokens.Length; i++)
		{
			if (_tokenRotation[i] != 0)
			{
				_rotating = false;
				return;
			}
		}
		Image[] tokenImages = _tokenImages;
		foreach (Image obj in tokenImages)
		{
			obj.DOKill();
			obj.color = _normal;
		}
		StartCoroutine(PuzzleUnlocked());
	}

	private void SlideFrontBoard()
	{
		float endValue = _frontBoard.position.x + _frontBoard.sizeDelta.x * 2f;
		_frontBoard.DOKill();
		_frontBoard.DOMoveX(endValue, 0.5f).SetEase(Ease.InQuad);
	}

	public IEnumerator PuzzleUnlocked()
	{
		_success = true;
		AudioManager.PlaySFX("puzzle-antiquebox-unlocked");
		yield return new WaitForSeconds(0.25f);
		SlideFrontBoard();
		AudioManager.PlaySFX("antique-box-slide");
		yield return new WaitForSeconds(1f);
		UIGameManager.Instance.ShowUIInGame(GetComponent<UIView>());
		NetworkGameManager.Instance.ownPlayer.network.SetDropItem(_spawnItemById, 1, 0, NetworkGameManager.Instance.ownPlayer.flashlight.transform.position, isSwapWeapon: false, isSpreading: false);
		InventoryObject inventoryObject = NetworkGameManager.Instance.ownPlayer.data.FindInventory(326);
		if (inventoryObject != null)
		{
			NetworkGameManager.Instance.ownPlayer.data.RemoveInventory(inventoryObject.IdxInventory);
		}
	}

	public void Action1Press()
	{
		if (_isNav && !_rotating)
		{
			RotateToken(_pointer, -1);
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
		if (!_success)
		{
			_isNav = true;
			if (Mathf.Abs(direction.x) > 0.5f)
			{
				int pointer = Shift(_pointer, (int)Mathf.Sign(direction.x), _tokens.Length);
				_pointer = pointer;
				Highlight(_pointer);
			}
			else if (Mathf.Abs(direction.y) > 0.5f && !_rotating)
			{
				RotateToken(_pointer, (int)Mathf.Sign(direction.y) * -1);
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
		ResetPuzzle();
		_isNav = true;
	}

	public void Hide()
	{
	}

	private void ResetPuzzle()
	{
		GeneratePuzzle();
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzleColorGemLock : MonoBehaviour, IPuzzle, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler, IPointerMoveHandler
{
	[Header("External Component")]
	[SerializeField]
	private GemPairing[] _gemPairingSetup;

	[SerializeField]
	private Sprite[] _interactiveGemsLib;

	[SerializeField]
	private Sprite _blankNotation;

	[Header("Internal Component")]
	[SerializeField]
	private Transform _valve;

	[SerializeField]
	private Transform _shardsParent;

	[SerializeField]
	private Transform _mutedGemsParent;

	[SerializeField]
	private Transform _inventorySlotParent;

	[SerializeField]
	private Transform _plateSlotParent;

	[SerializeField]
	private Transform _interactiveGemsParent;

	[Header("Setup Properties")]
	[SerializeField]
	private Color _initColor;

	[SerializeField]
	private Color _targetColor;

	[SerializeField]
	private Color _initHighlightColor;

	[SerializeField]
	private Color _targetHighlightColor;

	[Header("Audio String")]
	[SerializeField]
	private string _sfxPickGem;

	[SerializeField]
	private string _sfxPutGem;

	[SerializeField]
	private string _sfxRotateValve;

	private Image[] _lightShards;

	private Image[] _allMutedGems;

	private Image[] _mutedGems;

	private Image[] _inventorySlots;

	private Image[] _plateSlots;

	private Image[] _interactiveGems;

	private Transform[] _attachedSlots;

	private Canvas[] _interactiveCanvas;

	private Transform _pickingGem;

	private Transform _prevAttached;

	private bool _animating;

	private bool _succeed;

	private bool _isNavMode;

	private bool _navPicked;

	private int _navPickIndex;

	private int _navSlotSegment;

	private int _navSlotIndex;

	private Image _hlSlot;

	private ItemInteractable _interactableTrigger;

	private void Start()
	{
		_lightShards = new Image[_shardsParent.childCount];
		for (int i = 0; i < _lightShards.Length; i++)
		{
			_lightShards[i] = _shardsParent.GetChild(i).GetComponent<Image>();
		}
		_allMutedGems = new Image[_mutedGemsParent.childCount];
		for (int j = 0; j < _allMutedGems.Length; j++)
		{
			_allMutedGems[j] = _mutedGemsParent.GetChild(j).GetComponent<Image>();
		}
		_inventorySlots = new Image[_inventorySlotParent.childCount];
		for (int k = 0; k < _inventorySlotParent.childCount; k++)
		{
			_inventorySlots[k] = _inventorySlotParent.GetChild(k).GetComponent<Image>();
		}
		_plateSlots = new Image[_plateSlotParent.childCount];
		for (int l = 0; l < _plateSlotParent.childCount; l++)
		{
			_plateSlots[l] = _plateSlotParent.GetChild(l).GetComponent<Image>();
		}
		_interactiveGems = new Image[_interactiveGemsParent.childCount];
		_attachedSlots = new Transform[_interactiveGems.Length];
		_interactiveCanvas = new Canvas[_interactiveGems.Length];
		for (int m = 0; m < _interactiveGemsParent.childCount; m++)
		{
			_interactiveGems[m] = _interactiveGemsParent.GetChild(m).GetComponent<Image>();
			_attachedSlots[m] = _inventorySlots[m].transform;
			_interactiveCanvas[m] = _interactiveGems[m].GetComponent<Canvas>();
		}
		_mutedGems = new Image[_interactiveGems.Length];
		StartCoroutine(Generate());
	}

	private IEnumerator Generate()
	{
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		UnityEngine.Random.InitState(GlobalOptionsManager.Instance.GetSeedCombineWithMissionID());
		List<Sprite> list = _interactiveGemsLib.ToList();
		for (int i = 0; i < _interactiveGems.Length; i++)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			_interactiveGems[i].sprite = list[index];
			list.RemoveAt(index);
		}
		List<GemPairing> list2 = _gemPairingSetup.ToList();
		int num = _plateSlots.Length / 2;
		for (int j = 0; j < num; j++)
		{
			int index = UnityEngine.Random.Range(0, list2.Count);
			_plateSlots[j].sprite = list2[index].Head;
			_plateSlots[j + num].sprite = list2[index].Tail;
			list2.RemoveAt(index);
		}
		int num2 = 0;
		for (int k = num; k < _plateSlots.Length; k++)
		{
			for (int l = 0; l < _interactiveGemsLib.Length; l++)
			{
				if (_plateSlots[k].sprite == _interactiveGemsLib[l])
				{
					_plateSlots[k].gameObject.SetActive(value: false);
					_lightShards[k].gameObject.SetActive(value: false);
					_mutedGems[num2] = _allMutedGems[k];
					num2++;
					break;
				}
			}
		}
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
	}

	public void Action1Press()
	{
		if (!_animating && _isNavMode)
		{
			if (_navPicked)
			{
				SnapGem(_hlSlot.transform);
				_navPicked = false;
			}
			else if (_navPickIndex >= 0)
			{
				_navPicked = true;
				_pickingGem = _interactiveGems[_navPickIndex].transform;
				PickGem();
			}
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
		ClearHighlight();
	}

	public void InitAnswer()
	{
	}

	public void Navigate(Vector2 direction)
	{
		if ((!(Mathf.Abs(direction.x) > 0.5f) && !(Mathf.Abs(direction.y) > 0.5f)) || _animating)
		{
			return;
		}
		if (_isNavMode)
		{
			if ((double)Mathf.Abs(direction.x) > 0.5)
			{
				_navSlotSegment += (int)Mathf.Sign(direction.x) * -1;
				if (_navSlotSegment < 0)
				{
					_navSlotSegment = _inventorySlots.Length;
				}
				else
				{
					_navSlotSegment %= _inventorySlots.Length + 1;
				}
			}
			else if (Mathf.Abs(direction.y) > 0.5f && _navSlotSegment == 0)
			{
				_navSlotIndex += (int)Mathf.Sign(direction.y) * -1;
				if (_navSlotIndex < 0)
				{
					_navSlotIndex = _inventorySlots.Length - 1;
				}
				else
				{
					_navSlotIndex %= _inventorySlots.Length;
				}
			}
		}
		else
		{
			_isNavMode = true;
			_navSlotSegment = 0;
			_navSlotIndex = 0;
		}
		Highlight();
	}

	private void ClearHighlight()
	{
		Image[] mutedGems = _mutedGems;
		foreach (Image obj in mutedGems)
		{
			obj.DOKill();
			obj.color = _initColor;
		}
		mutedGems = _inventorySlots;
		foreach (Image obj2 in mutedGems)
		{
			obj2.DOKill();
			obj2.color = _initColor;
		}
		for (int j = 0; j < _interactiveGems.Length; j++)
		{
			_interactiveGems[j].DOKill();
			_interactiveGems[j].color = Color.white;
		}
	}

	private void Highlight()
	{
		if (_navSlotSegment == 0)
		{
			_hlSlot = _inventorySlots[_navSlotIndex];
		}
		else
		{
			_hlSlot = _mutedGems[_navSlotSegment - 1];
		}
		if (_navPicked)
		{
			_pickingGem.position = _hlSlot.transform.position;
			return;
		}
		_navPickIndex = -1;
		ClearHighlight();
		Color color = _initHighlightColor;
		Color endValue = _targetHighlightColor;
		for (int i = 0; i < _interactiveGems.Length; i++)
		{
			if (_attachedSlots[i].gameObject.name == _hlSlot.gameObject.name)
			{
				_navPickIndex = i;
				_hlSlot = _interactiveGems[i];
				color = Color.white;
				endValue = _targetColor;
				break;
			}
		}
		_hlSlot.color = color;
		_hlSlot.DOColor(endValue, 0.2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
	}

	public IEnumerator PuzzleUnlocked()
	{
		_valve.DOKill();
		yield return new WaitForSeconds(0.2f);
		UIGameManager.Instance.ShowUIInGame(_interactableTrigger.UIMenu);
		NetworkGameManager.Instance.ownPlayer.network.ExecInteractObject((short)_interactableTrigger.UniqueID, triggerOnReverse: false, isForceInteract: true);
		_interactableTrigger.DisableCollider();
		NetworkGameManager.Instance.ownPlayer.itemCollision = null;
		NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
		yield return new WaitForSeconds(2f);
		Bounds bounds = new Bounds(_interactableTrigger.doorCollider.bounds.center, _interactableTrigger.doorCollider.bounds.size * 2f);
		_interactableTrigger.doorCollider.enabled = false;
		GameManager.Instance.AStarPath.UpdateGraphs(bounds);
		GameManager.Instance.AStarPath.FlushGraphUpdates();
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
		UnPick();
		ClearHighlight();
		if (_isNavMode)
		{
			_navPicked = false;
			_navPickIndex = 0;
			_navSlotIndex = 0;
			Highlight();
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (!_animating && !_succeed)
		{
			GameObject gameObject = eventData.pointerCurrentRaycast.gameObject;
			if ((bool)gameObject && gameObject.transform.parent == _interactiveGemsParent)
			{
				_pickingGem = gameObject.transform;
				PickGem();
			}
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if ((bool)_pickingGem)
		{
			_pickingGem.position = eventData.position;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (_animating || !_pickingGem)
		{
			return;
		}
		for (int i = 0; i < _plateSlots.Length; i++)
		{
			if (!_plateSlots[i].gameObject.activeSelf && SnapGem(_plateSlots[i].transform))
			{
				return;
			}
		}
		for (int j = 0; j < _inventorySlots.Length; j++)
		{
			if (SnapGem(_inventorySlots[j].transform))
			{
				return;
			}
		}
		UnPick();
	}

	private void UnPick()
	{
		if (!(_pickingGem == null))
		{
			_pickingGem.position = _prevAttached.position;
			_animating = true;
			_pickingGem.DOScale(2f, 0.1f).OnComplete(() =>
			{
				_pickingGem.DOKill();
				_interactiveCanvas[_pickingGem.GetSiblingIndex()].sortingOrder = 2;
				_pickingGem = null;
				_prevAttached = null;
				_animating = false;
			});
			AudioManager.PlaySFX(_sfxPutGem);
		}
	}

	private void PickGem()
	{
		int siblingIndex = _pickingGem.GetSiblingIndex();
		_prevAttached = _attachedSlots[siblingIndex];
		_interactiveCanvas[siblingIndex].sortingOrder = 3;
		AudioManager.PlaySFX(_sfxPickGem);
		_pickingGem.DOKill();
		_animating = true;
		_pickingGem.DOScale(3f, 0.1f).OnComplete(() =>
		{
			_animating = false;
		});
	}

	private bool SnapGem(Transform target)
	{
		if (_pickingGem == null)
		{
			return false;
		}
		if (Vector3.Distance(_pickingGem.position, target.position) < 30f)
		{
			int siblingIndex = _pickingGem.GetSiblingIndex();
			target.GetSiblingIndex();
			_pickingGem.position = target.position;
			_attachedSlots[siblingIndex] = target;
			_animating = true;
			_pickingGem.DOScale(2f, 0.1f).OnComplete(() =>
			{
				_pickingGem.DOKill();
				_animating = false;
			});
			AudioManager.PlaySFX(_sfxPutGem);
			for (int num = 0; num < _interactiveGems.Length; num++)
			{
				if (_interactiveGems[num].transform != _pickingGem && _attachedSlots[num].gameObject.name == target.gameObject.name)
				{
					_interactiveGems[num].transform.position = _prevAttached.position;
					_attachedSlots[num] = _prevAttached;
					break;
				}
			}
			LightUpShards();
			_interactiveCanvas[_pickingGem.GetSiblingIndex()].sortingOrder = 2;
			_pickingGem = null;
			_prevAttached = null;
			return true;
		}
		return false;
	}

	private void LightUpShards()
	{
		for (int i = 0; i < _mutedGems.Length; i++)
		{
			int siblingIndex = _mutedGems[i].transform.GetSiblingIndex();
			_lightShards[siblingIndex].gameObject.SetActive(value: false);
		}
		int num = 0;
		int num2 = 0;
		for (int j = 0; j < _attachedSlots.Length; j++)
		{
			num = _attachedSlots[j].GetSiblingIndex();
			if (num >= _attachedSlots.Length && _interactiveGems[j].sprite == _plateSlots[num].sprite)
			{
				_lightShards[num].gameObject.SetActive(value: true);
				num2++;
			}
		}
		if (num2 >= 3)
		{
			_succeed = true;
			_valve.transform.DOKill();
			AudioManager.PlaySFX(_sfxRotateValve);
			_valve.DOLocalRotate(new Vector3(0f, 0f, 120f), 2f).SetEase(Ease.InCubic).OnComplete(() =>
			{
				StartCoroutine(PuzzleUnlocked());
			});
		}
	}

	public void OnPointerMove(PointerEventData eventData)
	{
		if (!_navPicked)
		{
			_isNavMode = false;
			ClearHighlight();
		}
	}
}

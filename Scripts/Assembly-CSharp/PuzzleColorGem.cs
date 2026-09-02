using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzleColorGem : MonoBehaviour, IPuzzle, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler, IPointerMoveHandler
{
	[SerializeField]
	private List<Sprite> _gemSpriteLib;

	[SerializeField]
	private Transform _valve;

	[SerializeField]
	private Transform _gemHLParent;

	[SerializeField]
	private GameObject _gemMuted;

	[SerializeField]
	private GemPair[] _gemPairs;

	private GemImage[] _gemImageCache;

	[SerializeField]
	private Transform _gemOwnedParent;

	[SerializeField]
	private Transform[] _gemOwned;

	private Image[] _gemOwnedImageCache;

	private Vector3[] _gemOwnedInitPos;

	private int[] _gemOwnedOccupy;

	[SerializeField]
	private Transform _allGemParent;

	private bool[] _allGemMissing;

	[SerializeField]
	private Transform _allPlateParent;

	[SerializeField]
	private List<Sprite> _removeGemSprite;

	private int _seed;

	private bool _succeed;

	private bool _isNav;

	private Transform _draggingGem;

	private Vector3 _draggingGemInitPos;

	private Color _transparent;

	private int _pSegment;

	private ItemInteractable _interactableTrigger;

	private bool _pointing;

	private int _gemNavPointer;

	private Image[] _gemArranged;

	private int[] _missingGemIndex;

	private int _slotNavPointer;

	private IEnumerator Start()
	{
		_transparent = new Color(1f, 1f, 1f, 0f);
		_gemImageCache = new GemImage[_gemPairs.Length];
		for (int i = 0; i < _gemPairs.Length; i++)
		{
			_gemImageCache[i].Head = _gemPairs[i].Head.GetComponent<Image>();
			_gemImageCache[i].Tail = _gemPairs[i].Tail.GetComponent<Image>();
		}
		_gemOwnedImageCache = new Image[_gemOwned.Length];
		_gemOwnedInitPos = new Vector3[_gemOwned.Length];
		_gemOwnedOccupy = new int[_gemOwned.Length];
		for (int j = 0; j < _gemOwned.Length; j++)
		{
			_gemOwnedImageCache[j] = _gemOwned[j].GetComponent<Image>();
			_gemOwnedInitPos[j] = _gemOwned[j].localPosition;
			_gemOwnedOccupy[j] = -1;
		}
		_allGemMissing = new bool[_allGemParent.childCount];
		_missingGemIndex = new int[_allGemParent.childCount];
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		_seed = GlobalOptionsManager.Instance.GetSeedCombineWithMissionID();
		GeneratePuzzle();
		int num = 0;
		for (int k = 0; k < _allGemMissing.Length; k++)
		{
			if (_allGemMissing[k])
			{
				_missingGemIndex[num] = k;
				num++;
			}
		}
	}

	public void OnPointerMove(PointerEventData eventData)
	{
		_isNav = false;
	}

	private void GeneratePuzzle()
	{
		UnityEngine.Random.InitState(_seed);
		Sprite[] array = new Sprite[_gemSpriteLib.Count];
		for (int i = 0; i < _gemOwned.Length; i++)
		{
			_gemOwnedOccupy[i] = -1;
		}
		ShuffleOrder(array);
		for (int j = 0; j < _gemImageCache.Length; j++)
		{
			_gemImageCache[j].Head.sprite = array[j];
		}
		SwapOrder(array);
		for (int k = 0; k < _gemImageCache.Length; k++)
		{
			_gemImageCache[k].Tail.sprite = array[k];
		}
		_pSegment = UnityEngine.Random.Range(0, 2);
		int num = 0;
		foreach (Sprite item in _removeGemSprite)
		{
			if (_pSegment > 0)
			{
				for (int l = 0; l < _gemImageCache.Length; l++)
				{
					if (_gemImageCache[l].Head.sprite == item)
					{
						_gemOwnedImageCache[num].sprite = item;
						_gemImageCache[l].Head.color = _transparent;
						_allGemMissing[l] = true;
						num++;
					}
				}
				continue;
			}
			for (int m = 0; m < _gemImageCache.Length; m++)
			{
				if (_gemImageCache[m].Tail.sprite == item)
				{
					_gemOwnedImageCache[num].sprite = item;
					GameObject gameObject = UnityEngine.Object.Instantiate(_gemMuted, _gemHLParent);
					gameObject.transform.position = _gemImageCache[m].Tail.transform.position;
					gameObject.GetComponent<Image>().DOColor(gameObject.GetComponent<Image>().color * new Color(1f, 1f, 1f, 0f), 0.5f).SetLoops(-1, LoopType.Yoyo)
						.SetEase(Ease.Linear);
					_gemImageCache[m].Tail.color = _transparent;
					_allGemMissing[m + 6] = true;
					num++;
				}
			}
		}
		for (int n = 0; n < _allPlateParent.childCount; n++)
		{
			if (_allGemMissing[n])
			{
				_allPlateParent.GetChild(n).GetComponent<Image>().color = _transparent;
			}
		}
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
	}

	private void ShuffleOrder(Sprite[] setOrder)
	{
		List<Sprite> list = new List<Sprite>(_gemSpriteLib);
		for (int i = 0; i < setOrder.Length; i++)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			setOrder[i] = list[index];
			list.RemoveAt(index);
		}
	}

	private void SwapOrder(Sprite[] setOrder)
	{
		for (int i = 0; i < setOrder.Length; i++)
		{
			if (i % 2 == 0)
			{
				Sprite sprite = setOrder[i];
				setOrder[i] = setOrder[i + 1];
				setOrder[i + 1] = sprite;
			}
		}
	}

	private void ResetPuzzle()
	{
		for (int i = 0; i < _gemOwned.Length; i++)
		{
			_gemOwned[i].transform.localPosition = _gemOwnedInitPos[i];
		}
		GeneratePuzzle();
	}

	public void Action1Press()
	{
		if (!_isNav)
		{
			return;
		}
		if (!_pointing)
		{
			_pointing = true;
			_draggingGem = _gemArranged[_gemNavPointer].transform;
			_draggingGemInitPos = _draggingGem.position;
			PointTo(_slotNavPointer);
			return;
		}
		bool flag = false;
		int num = -1;
		int num2 = -1;
		for (int i = 0; i < _gemOwned.Length; i++)
		{
			if (_draggingGem == _gemOwned[i])
			{
				num2 = _gemOwnedOccupy[i];
				num = i;
				break;
			}
		}
		Transform transform = null;
		int num3 = -1;
		for (int j = 0; j < _allGemParent.childCount; j++)
		{
			if (Vector3.Distance(_draggingGem.position, _allGemParent.GetChild(j).position) < 30f)
			{
				transform = _allGemParent.GetChild(j);
				num3 = j;
				break;
			}
		}
		for (int k = 0; k < _gemOwned.Length; k++)
		{
			if (num2 == -1 && _gemOwnedOccupy[k] == num3)
			{
				_draggingGem.position = _draggingGemInitPos;
				_draggingGem.DOScale(2f, 0.2f);
				_pointing = false;
				return;
			}
		}
		if (num3 == -1)
		{
			_draggingGem.position = _draggingGemInitPos;
			_draggingGem.DOScale(2f, 0.2f);
			_pointing = false;
			return;
		}
		if (!_allGemMissing[num3])
		{
			_draggingGem.position = _draggingGemInitPos;
			_draggingGem.DOScale(2f, 0.2f);
			_pointing = false;
			return;
		}
		flag = true;
		AudioManager.PlaySFX("puzzle-colorGem-putStone");
		if (num2 > -1)
		{
			for (int l = 0; l < _gemOwnedOccupy.Length; l++)
			{
				if (_gemOwnedOccupy[l] == num3)
				{
					_gemOwned[l].position = _allGemParent.GetChild(num2).position;
					_gemOwnedOccupy[l] = num2;
				}
			}
		}
		_draggingGem.position = transform.position;
		_gemOwnedOccupy[num] = num3;
		if (!flag)
		{
			_draggingGem.position = _draggingGemInitPos;
		}
		_draggingGem.DOScale(2f, 0.2f);
		DetermineResult();
		_pointing = false;
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
		int num = 0;
		if (Mathf.Abs(direction.y) > 0.5f)
		{
			num = (int)Mathf.Sign(direction.y) * -1;
		}
		else if (Mathf.Abs(direction.x) > 0.5f)
		{
			num = (int)Mathf.Sign(direction.x);
		}
		if (num != 0)
		{
			_isNav = true;
			if (!_pointing)
			{
				int num2 = _gemNavPointer + num;
				num2 = ((num2 >= 0) ? (num2 % _gemArranged.Length) : (_gemArranged.Length - 1));
				_gemNavPointer = num2;
				Highlight(_gemNavPointer);
			}
			else
			{
				int num3 = _slotNavPointer + num;
				num3 = ((num3 >= 0) ? (num3 % _gemArranged.Length) : (_gemArranged.Length - 1));
				_slotNavPointer = num3;
				PointTo(_slotNavPointer);
			}
		}
	}

	private void Highlight(int idx)
	{
		ClearHighlight();
		_gemArranged[idx].transform.SetAsLastSibling();
		_gemArranged[idx].DOColor(Color.gray, 0.5f).SetLoops(-1, LoopType.Yoyo);
	}

	private void ClearHighlight()
	{
		Image[] gemArranged = _gemArranged;
		foreach (Image obj in gemArranged)
		{
			obj.DOKill();
			obj.color = Color.white;
		}
	}

	private void PointTo(int idx)
	{
		_draggingGem.DOScale(3f, 0.2f);
		_gemArranged[_gemNavPointer].transform.position = _allGemParent.GetChild(_missingGemIndex[idx]).position;
	}

	private void XSort()
	{
		for (int i = 0; i < _gemArranged.Length; i++)
		{
			for (int j = 1; j < _gemArranged.Length; j++)
			{
				if (_gemArranged[j].transform.position.x > _gemArranged[j - 1].transform.position.x)
				{
					Image[] gemArranged = _gemArranged;
					int num = j - 1;
					Image[] gemArranged2 = _gemArranged;
					int num2 = j;
					Image image = _gemArranged[j];
					Image image2 = _gemArranged[j - 1];
					gemArranged[num] = image;
					gemArranged2[num2] = image2;
				}
			}
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		_isNav = false;
		if ((bool)eventData.pointerCurrentRaycast.gameObject && eventData.pointerCurrentRaycast.gameObject.transform.parent == _gemOwnedParent)
		{
			ClearHighlight();
			_draggingGem = eventData.pointerCurrentRaycast.gameObject.transform;
			_draggingGemInitPos = _draggingGem.position;
			_draggingGem.DOScale(3f, 0.2f);
			_draggingGem.SetAsLastSibling();
			AudioManager.PlaySFX("puzzle-colorGem-pickStone");
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if ((bool)_draggingGem != _succeed)
		{
			_draggingGem.position = eventData.position;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if ((bool)_draggingGem)
		{
			bool flag = false;
			int num = -1;
			int num2 = -1;
			for (int i = 0; i < _gemOwned.Length; i++)
			{
				if (_draggingGem == _gemOwned[i])
				{
					num2 = _gemOwnedOccupy[i];
					num = i;
					break;
				}
			}
			Transform transform = null;
			int num3 = -1;
			for (int j = 0; j < _allGemParent.childCount; j++)
			{
				if (Vector3.Distance(_draggingGem.position, _allGemParent.GetChild(j).position) < 30f)
				{
					transform = _allGemParent.GetChild(j);
					num3 = j;
					break;
				}
			}
			for (int k = 0; k < _gemOwned.Length; k++)
			{
				if (num2 == -1 && _gemOwnedOccupy[k] == num3)
				{
					_draggingGem.position = _draggingGemInitPos;
					_draggingGem.DOScale(2f, 0.2f);
					return;
				}
			}
			if (num3 == -1)
			{
				_draggingGem.position = _draggingGemInitPos;
				_draggingGem.DOScale(2f, 0.2f);
				return;
			}
			if (!_allGemMissing[num3])
			{
				_draggingGem.position = _draggingGemInitPos;
				_draggingGem.DOScale(2f, 0.2f);
				return;
			}
			flag = true;
			AudioManager.PlaySFX("puzzle-colorGem-putStone");
			if (num2 > -1)
			{
				for (int l = 0; l < _gemOwnedOccupy.Length; l++)
				{
					if (_gemOwnedOccupy[l] == num3)
					{
						_gemOwned[l].position = _allGemParent.GetChild(num2).position;
						_gemOwnedOccupy[l] = num2;
					}
				}
			}
			_draggingGem.position = transform.position;
			_gemOwnedOccupy[num] = num3;
			if (!flag)
			{
				_draggingGem.position = _draggingGemInitPos;
			}
			_draggingGem.DOScale(2f, 0.2f);
			DetermineResult();
		}
		_draggingGem = null;
	}

	private void DetermineResult()
	{
		int num = 0;
		bool flag = false;
		for (int i = 0; i < _allGemParent.childCount; i++)
		{
			int num2 = -1;
			for (int j = 0; j < _gemOwnedOccupy.Length; j++)
			{
				if (_gemOwnedOccupy[j] == i)
				{
					num2 = j;
					break;
				}
			}
			if (num2 > -1)
			{
				if (_allGemParent.GetChild(i).GetComponent<Image>().sprite == _gemOwnedImageCache[num2].sprite)
				{
					_allPlateParent.GetChild(i).GetComponent<Image>().color = Color.white;
					num++;
				}
				else
				{
					_allPlateParent.GetChild(i).GetComponent<Image>().color = _transparent;
				}
			}
			else if (_allGemMissing[i])
			{
				_allPlateParent.GetChild(i).GetComponent<Image>().color = _transparent;
			}
		}
		if (num >= _gemOwned.Length)
		{
			_succeed = true;
			_valve.transform.DOKill();
			AudioManager.PlaySFX("puzzle-colorGem-unlocked");
			_valve.DOLocalRotate(new Vector3(0f, 0f, 120f), 2f).SetEase(Ease.InCubic).OnComplete(() =>
			{
				PuzzleSuccess();
			});
		}
	}

	private void PuzzleSuccess()
	{
		_valve.DOKill();
		StartCoroutine(PuzzleUnlocked());
	}

	public IEnumerator PuzzleUnlocked()
	{
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
		ResetPuzzle();
		_gemArranged = new Image[_gemOwnedImageCache.Length];
		for (int i = 0; i < _gemArranged.Length; i++)
		{
			_gemArranged[i] = _gemOwnedImageCache[i];
		}
		Transform[] gemOwned = _gemOwned;
		foreach (Transform target in gemOwned)
		{
			target.DOKill();
			target.DOScale(2f, 0.05f);
		}
		XSort();
		Highlight(0);
	}

	public void Hide()
	{
	}
}

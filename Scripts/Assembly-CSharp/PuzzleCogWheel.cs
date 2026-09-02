using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzleCogWheel : MonoBehaviour, IPuzzle, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler, IPointerMoveHandler
{
	[Header("Setup")]
	[SerializeField]
	private bool _isTesting;

	[SerializeField]
	private bool _useForceIndex;

	[SerializeField]
	private int _forceIndex;

	[Header("Object Data")]
	[SerializeField]
	private Sprite[] _spriteData;

	[SerializeField]
	private float[] _collisionRData;

	[Header("Transform Reference")]
	[SerializeField]
	private Transform[] _permamnetCogParentList;

	[SerializeField]
	private Transform[] _cogHoleParentList;

	[SerializeField]
	private Transform _detachableCogParent;

	[Header("External")]
	[SerializeField]
	private ItemInteractable _interactableTrigger;

	private RectTransform[] _cogs;

	private float[] _cogsR;

	private int[] _powered;

	private RectTransform[] _cogHoles;

	private int[] _cogOccupy;

	private Image[] _cogYellowImages;

	private const float SCALE = 1f;

	private float _teeth;

	private Transform _draggingCog;

	private Vector2 _draggingCogPrevPos;

	private bool isShow;

	private bool isPause;

	private bool _inputModeNavigation;

	private int _holeIndex;

	private int _cogIndex;

	private RectTransform _cogGrab;

	private bool _succeed;

	private const string SFX_SNAP = "sfx-cogwheel-snapGear";

	private const string SFX_ROTATE = "sfx-cogwheel-rotatingGear";

	private const string SFX_UNLOCK = "sfx-cogwheel-unlock";

	private IEnumerator Start()
	{
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		UnityEngine.Random.InitState(GlobalOptionsManager.Instance.GetSeedCombineWithMissionID());
		int num = UnityEngine.Random.Range(0, 2);
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
		if (_useForceIndex)
		{
			num = _forceIndex;
		}
		for (int i = 0; i < _cogHoleParentList.Length; i++)
		{
			if (i != num)
			{
				_cogHoleParentList[i].gameObject.SetActive(value: false);
			}
			else
			{
				_cogHoleParentList[i].gameObject.SetActive(value: true);
			}
		}
		for (int j = 0; j < _permamnetCogParentList.Length; j++)
		{
			if (j != num)
			{
				_permamnetCogParentList[j].gameObject.SetActive(value: false);
			}
			else
			{
				_permamnetCogParentList[j].gameObject.SetActive(value: true);
			}
		}
		_cogHoles = new RectTransform[_cogHoleParentList[num].childCount];
		for (int k = 0; k < _cogHoles.Length; k++)
		{
			_cogHoles[k] = _cogHoleParentList[num].GetChild(k).GetComponent<RectTransform>();
		}
		int num2 = _permamnetCogParentList[num].childCount + _detachableCogParent.childCount;
		_cogs = new RectTransform[num2];
		int num3 = 0;
		for (int l = 0; l < _permamnetCogParentList[num].childCount; l++)
		{
			_cogs[num3] = _permamnetCogParentList[num].GetChild(l).GetComponent<RectTransform>();
			num3++;
		}
		_cogYellowImages = new Image[_detachableCogParent.childCount];
		_cogOccupy = new int[_cogYellowImages.Length];
		for (int m = 0; m < _detachableCogParent.childCount; m++)
		{
			_cogs[num3] = _detachableCogParent.GetChild(m).GetComponent<RectTransform>();
			num3++;
			_cogYellowImages[m] = _detachableCogParent.GetChild(m).GetComponent<Image>();
			_cogOccupy[m] = m;
		}
		_cogsR = new float[_cogs.Length];
		for (int n = 0; n < _cogsR.Length; n++)
		{
			_cogsR[n] = (_cogs[n].sizeDelta.x + _cogs[n].sizeDelta.y) * 0.25f * 1f;
		}
	}

	public void Action1Press()
	{
		if (_inputModeNavigation)
		{
			if (_cogGrab == null)
			{
				_cogGrab = _cogYellowImages[_cogIndex].GetComponent<RectTransform>();
				StopEngine();
			}
			else if (!OverlapCheck())
			{
				_cogGrab = null;
				AudioManager.PlaySFX("sfx-cogwheel-snapGear");
				PoweringGear();
				RestartEngine();
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

	public void InitAnswer()
	{
	}

	public void Navigate(Vector2 direction)
	{
		if ((!(Mathf.Abs(direction.x) > 0.5f) && !((double)Mathf.Abs(direction.y) > 0.5)) || _succeed)
		{
			return;
		}
		if (_inputModeNavigation)
		{
			int num = 0;
			if (_cogGrab != null)
			{
				if (Mathf.Abs(direction.x) > 0.5f)
				{
					num = (int)Mathf.Sign(direction.x);
					_holeIndex = (_holeIndex + num) % _cogHoles.Length;
				}
				else if (Mathf.Abs(direction.y) > 0.5f)
				{
					num = (int)Mathf.Sign(direction.y) * -1;
					_holeIndex = (_holeIndex + num) % _cogHoles.Length;
				}
				if (_holeIndex < 0)
				{
					_holeIndex = _cogHoles.Length - 1;
				}
				for (int i = 0; i < _cogOccupy.Length; i++)
				{
					if (i != _cogIndex)
					{
						if (_cogOccupy[i] == _holeIndex)
						{
							_holeIndex = (_holeIndex + num) % _cogHoles.Length;
						}
						if (_holeIndex < 0)
						{
							_holeIndex = _cogHoles.Length - 1;
						}
					}
				}
			}
			else
			{
				if (Mathf.Abs(direction.x) > 0.5f)
				{
					_cogIndex = (_cogIndex + (int)Mathf.Sign(direction.x)) % _cogYellowImages.Length;
				}
				else if (Mathf.Abs(direction.y) > 0.5f)
				{
					_cogIndex = (_cogIndex - (int)Mathf.Sign(direction.y)) % _cogYellowImages.Length;
				}
				if (_cogIndex < 0)
				{
					_cogIndex = _cogYellowImages.Length - 1;
				}
			}
			Highlight();
		}
		else
		{
			_inputModeNavigation = true;
			_cogIndex = 0;
			Highlight();
		}
	}

	private void Highlight()
	{
		if (_cogGrab == null)
		{
			ClearHighlight();
			_cogYellowImages[_cogIndex].DOColor(Color.gray, 0.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
		}
		else
		{
			_cogGrab.anchoredPosition = _cogHoles[_holeIndex].anchoredPosition;
			_cogOccupy[_cogIndex] = _holeIndex;
		}
	}

	private void ClearHighlight()
	{
		Image[] cogYellowImages = _cogYellowImages;
		foreach (Image obj in cogYellowImages)
		{
			obj.DOKill();
			obj.color = Color.white;
		}
		_cogYellowImages[_cogIndex].color = Color.white;
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
		isShow = true;
		_draggingCog = null;
		PoweringGear();
		RestartEngine();
	}

	public void Hide()
	{
		StopEngine();
		isShow = false;
	}

	private void PoweringGear()
	{
		ResetPower();
		Contagious();
		Contagious();
		Contagious();
	}

	private void Contagious()
	{
		_teeth = _cogs[0].sizeDelta.x * 0.25f;
		for (int i = 0; i < _cogs.Length; i++)
		{
			for (int j = 0; j < _cogs.Length; j++)
			{
				if (!(_cogs[i] != _cogs[j]))
				{
					continue;
				}
				float num = Vector2.Distance(_cogs[i].anchoredPosition, _cogs[j].anchoredPosition);
				if (num < _cogsR[i] + _cogsR[j] && num > _cogsR[i] + _cogsR[j] - _teeth)
				{
					if (_powered[i] != 0 && _powered[j] == 0)
					{
						_powered[j] = _powered[i] * -1;
					}
					else if (_powered[i] == 0 && _powered[j] != 0)
					{
						_powered[i] = _powered[j] * -1;
					}
				}
			}
		}
	}

	private bool OverlapCheck()
	{
		_teeth = _cogs[0].sizeDelta.x * 0.25f;
		for (int i = 0; i < _cogs.Length; i++)
		{
			for (int j = 0; j < _cogs.Length; j++)
			{
				if (_cogs[i] != _cogs[j] && Vector2.Distance(_cogs[i].anchoredPosition, _cogs[j].anchoredPosition) < _cogsR[i] + _cogsR[j] - _teeth)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void ResetPower()
	{
		_powered = new int[_cogs.Length];
		_powered[0] = 1;
	}

	private void RestartEngine()
	{
		StopEngine();
		bool flag = false;
		for (int i = 0; i < _cogs.Length; i++)
		{
			_cogs[i].DORotate(Vector3.zero, 0f);
			if (_powered[i] != 0)
			{
				_cogs[i].DORotate(Vector3.forward * ((float)_powered[i] * 360f), _cogsR[i] * 0.2f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
			}
			else
			{
				flag = true;
			}
		}
		if (!flag && !_isTesting)
		{
			_succeed = true;
			UniTaskUtil.DelayedCall(this, 1f, () =>
			{
				StartCoroutine(PuzzleUnlocked());
			}).Forget();
		}
		AudioManager.PlaySFX("sfx-cogwheel-rotatingGear");
	}

	private void StopEngine()
	{
		for (int i = 0; i < _cogs.Length; i++)
		{
			_cogs[i].DOKill();
		}
		AudioManager.StopSFX("sfx-cogwheel-rotatingGear");
	}

	public IEnumerator PuzzleUnlocked()
	{
		AudioManager.PlaySFX("sfx-cogwheel-unlock");
		yield return new WaitForSeconds(2f);
		UIGameManager.Instance.ShowUIInGame(_interactableTrigger.UIMenu);
		NetworkGameManager.Instance.ownPlayer.network.ExecInteractObject((short)_interactableTrigger.UniqueID, triggerOnReverse: false, isForceInteract: true);
		_interactableTrigger.DisableCollider();
		NetworkGameManager.Instance.ownPlayer.itemCollision = null;
		NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
		yield return new WaitForSeconds(2f);
		AudioManager.StopSFX("sfx-cogwheel-rotatingGear");
		if ((bool)_interactableTrigger.doorCollider)
		{
			Bounds bounds = new Bounds(_interactableTrigger.doorCollider.bounds.center, _interactableTrigger.doorCollider.bounds.size * 2f);
			_interactableTrigger.doorCollider.enabled = false;
			GameManager.Instance.AStarPath.UpdateGraphs(bounds);
			GameManager.Instance.AStarPath.FlushGraphUpdates();
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (_succeed)
		{
			return;
		}
		Image component = eventData.pointerCurrentRaycast.gameObject.GetComponent<Image>();
		if (!(component.transform.parent == _detachableCogParent))
		{
			return;
		}
		_draggingCog = component.transform;
		_draggingCogPrevPos = _draggingCog.position;
		for (int i = 0; i < _cogYellowImages.Length; i++)
		{
			if (component == _cogYellowImages[i])
			{
				_cogIndex = i;
				break;
			}
		}
		_draggingCog.SetAsLastSibling();
		isPause = true;
		StopEngine();
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (!(_draggingCog == null))
		{
			_draggingCog.position = eventData.position;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (_draggingCog == null)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < _cogHoles.Length; i++)
		{
			if (!(Vector3.Distance(_draggingCog.position, _cogHoles[i].position) < 30f))
			{
				continue;
			}
			for (int j = 0; j < _cogOccupy.Length; j++)
			{
				if (j != _cogIndex && _cogOccupy[j] == i)
				{
					flag2 = true;
				}
			}
			if (!flag2 && !OverlapCheck())
			{
				_cogOccupy[_cogIndex] = i;
				_draggingCog.position = _cogHoles[i].position;
				_draggingCog = null;
				flag = true;
			}
			break;
		}
		if (!flag)
		{
			_draggingCog.position = _draggingCogPrevPos;
		}
		AudioManager.PlaySFX("sfx-cogwheel-snapGear");
		PoweringGear();
		RestartEngine();
	}

	public void OnPointerMove(PointerEventData eventData)
	{
		_cogGrab = null;
		ClearHighlight();
		_inputModeNavigation = false;
	}
}

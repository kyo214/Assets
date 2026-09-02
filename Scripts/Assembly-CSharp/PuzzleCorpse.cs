using System;
using System.Collections;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PuzzleCorpse : MonoBehaviour, IPuzzle, IPointerDownHandler, IEventSystemHandler, IPointerMoveHandler
{
	[Header("Object Transform")]
	[SerializeField]
	private Transform _stitchParent;

	[SerializeField]
	private Transform _raycasterParent;

	[SerializeField]
	private Transform _woundParent;

	[SerializeField]
	private Image _imageEye;

	[SerializeField]
	private RectTransform _rectKnive;

	[Header("External")]
	[SerializeField]
	private Sprite[] _spriteOpenWound;

	[SerializeField]
	private Sprite[] _spriteEyeOpen;

	[SerializeField]
	private Sprite[] _spriteHighlight;

	[Header("Controller")]
	[SerializeField]
	private InputActionReference _directNavigate;

	[Header("Index Puzzle")]
	[SerializeField]
	private int _puzzleIndex;

	private RectTransform[] _rayStitches;

	private RectTransform[] _rectStitches;

	private Image[] _imageStitches;

	private Sprite[] _spriteOriginalStitches;

	private Image[] _imageWound;

	private int _hideIn;

	private bool _isTrap;

	private bool _inputModeController;

	private int _navIndex;

	private Vector2 _dirState;

	private bool _succeed;

	private bool _opening;

	private ItemInteractable _interactableTrigger;

	private const string SFX_SELECT = "sfx-autopsy-select";

	private const string SFX_OPEN = "sfx-autopsy-open";

	private const string SFX_AWAKE = "sfx-autopsy-awake";

	[SerializeField]
	private GameObject _activateObjectAfterEyeOpen;

	private void Awake()
	{
		_rectStitches = new RectTransform[_stitchParent.childCount];
		_imageStitches = new Image[_stitchParent.childCount];
		_spriteOriginalStitches = new Sprite[_stitchParent.childCount];
		for (int i = 0; i < _stitchParent.childCount; i++)
		{
			_rectStitches[i] = _stitchParent.GetChild(i).GetComponent<RectTransform>();
			_imageStitches[i] = _stitchParent.GetChild(i).GetComponent<Image>();
			_spriteOriginalStitches[i] = _imageStitches[i].sprite;
		}
		_rayStitches = new RectTransform[_raycasterParent.childCount];
		for (int j = 0; j < _raycasterParent.childCount; j++)
		{
			_rayStitches[j] = _raycasterParent.GetChild(j).GetComponent<RectTransform>();
		}
		_imageWound = new Image[_woundParent.childCount];
		for (int k = 0; k < _imageWound.Length; k++)
		{
			_imageWound[k] = _woundParent.GetChild(k).GetComponent<Image>();
		}
	}

	private void Start()
	{
		StartCoroutine(Generate());
	}

	private IEnumerator Generate()
	{
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		UnityEngine.Random.InitState(GlobalOptionsManager.Instance.GetSeedCombineWithMissionID() + _puzzleIndex);
		if (UnityEngine.Random.Range(0, 3) < 1)
		{
			_isTrap = true;
			_activateObjectAfterEyeOpen.SetActive(value: true);
		}
		else
		{
			_activateObjectAfterEyeOpen.SetActive(value: false);
		}
		_hideIn = UnityEngine.Random.Range(0, _rectStitches.Length);
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
	}

	public void Action1Press()
	{
		if (_inputModeController && _navIndex > -1)
		{
			_opening = true;
			StartCoroutine(LookInside(_navIndex));
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
		if (!_succeed && !_opening && !_inputModeController)
		{
			_inputModeController = true;
			_rectKnive.gameObject.SetActive(_inputModeController);
			_navIndex = -1;
		}
	}

	private void CheckHover()
	{
		for (int i = 0; i < _rayStitches.Length; i++)
		{
			if (RectTransformUtility.RectangleContainsScreenPoint(_rayStitches[i], _rectKnive.position))
			{
				_navIndex = i;
				if (_imageStitches[i].sprite != _spriteHighlight[i])
				{
					AudioManager.PlaySFX("sfx-autopsy-select");
				}
				_imageStitches[i].sprite = _spriteHighlight[i];
				return;
			}
			_imageStitches[i].sprite = _spriteOriginalStitches[i];
		}
		_navIndex = -1;
	}

	public IEnumerator PuzzleUnlocked()
	{
		_succeed = true;
		if (_isTrap)
		{
			AudioManager.PlaySFX("sfx-autopsy-awake");
			for (int i = 0; i < _spriteEyeOpen.Length; i++)
			{
				_imageEye.sprite = _spriteEyeOpen[i];
				yield return new WaitForSeconds(0.2f);
			}
			yield return new WaitForSeconds(1f);
		}
		_directNavigate.action.performed -= OnDirectNavigate;
		_directNavigate.action.canceled -= OnDirectNavigate;
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
		_directNavigate.action.performed += OnDirectNavigate;
		_directNavigate.action.canceled += OnDirectNavigate;
		_rectKnive.anchoredPosition = Vector2.zero;
		_rectKnive.gameObject.SetActive(_inputModeController);
	}

	private void OnDirectNavigate(InputAction.CallbackContext context)
	{
		_dirState = context.ReadValue<Vector2>().normalized * 4f;
	}

	private void FixedUpdate()
	{
		_rectKnive.anchoredPosition = new Vector2(_rectKnive.anchoredPosition.x + _dirState.x, _rectKnive.anchoredPosition.y + _dirState.y);
		CheckHover();
	}

	public void Hide()
	{
		Debug.Log("Close");
		_directNavigate.action.performed -= OnDirectNavigate;
		_directNavigate.action.canceled -= OnDirectNavigate;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!_succeed && !_opening)
		{
			_opening = true;
			RectTransform component = eventData.pointerCurrentRaycast.gameObject.GetComponent<RectTransform>();
			if (component.transform.parent == _raycasterParent)
			{
				int siblingIndex = component.transform.GetSiblingIndex();
				StartCoroutine(LookInside(siblingIndex));
			}
		}
	}

	private IEnumerator LookInside(int idx)
	{
		if (_imageWound[idx].sprite == _spriteOpenWound[idx * 2 + 1])
		{
			_imageWound[idx].sprite = _spriteOpenWound[idx * 2];
			_raycasterParent.GetChild(idx).gameObject.SetActive(value: false);
			StartCoroutine(PuzzleUnlocked());
			yield break;
		}
		AudioManager.PlaySFX("sfx-autopsy-open");
		yield return new WaitForSeconds(0.15f);
		_rectStitches[idx].gameObject.SetActive(value: false);
		_imageWound[idx].gameObject.SetActive(value: true);
		yield return new WaitForSeconds(0.15f);
		if (idx == _hideIn)
		{
			_imageWound[idx].sprite = _spriteOpenWound[idx * 2 + 1];
		}
		else
		{
			_raycasterParent.GetChild(idx).gameObject.SetActive(value: false);
			_imageWound[idx].sprite = _spriteOpenWound[idx * 2];
		}
		_opening = false;
	}

	public void OnPointerMove(PointerEventData eventData)
	{
		_inputModeController = false;
		_rectKnive.gameObject.SetActive(_inputModeController);
	}
}

using System;
using System.Collections;
using DG.Tweening;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PuzzleStatuePaper : MonoBehaviour, IPuzzle, IPointerMoveHandler, IEventSystemHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	[Header("Object Transform")]
	[SerializeField]
	private Transform[] _pieceSet;

	[SerializeField]
	private Transform _signParent;

	[SerializeField]
	private RectTransform _rectBookBinder;

	[SerializeField]
	private Image[] _imageRays;

	[SerializeField]
	private Image[] _imageSign;

	[Header("External")]
	[SerializeField]
	private Sprite[] _moonPhase;

	[Header("Controller")]
	[SerializeField]
	private InputActionReference _directNavigate;

	private RectTransform[] _rectPiece;

	private Image[] _imagePiece;

	private int _setIndex;

	private Vector2[] _correctPiecePlace;

	private int _grabIndex;

	private Vector2 _dirState;

	private Vector2 _offset;

	private Vector2 _clampPos;

	private bool _inputModeController;

	private int _navIndex;

	private bool _succeed;

	private ItemInteractable _interactableTrigger;

	private const string SFX_RUSTLE = "puzzle-paper-rustle";

	private const string SFX_SCAN = "puzzle-paper-scan";

	[SerializeField]
	private PuzzleNetworkBehaviour _puzzleNetworkBehaviour;

	private void Awake()
	{
		_imageSign = new Image[_signParent.childCount];
		for (int i = 0; i < _signParent.childCount; i++)
		{
			_imageSign[i] = _signParent.GetChild(i).GetComponent<Image>();
		}
	}

	private void Start()
	{
		_grabIndex = -1;
		StartCoroutine(Generate());
	}

	private IEnumerator Generate()
	{
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		UnityEngine.Random.InitState(GlobalOptionsManager.Instance.GetSeedCombineWithMissionID());
		_setIndex = UnityEngine.Random.Range(0, _pieceSet.Length);
		_rectPiece = new RectTransform[_pieceSet[_setIndex].childCount];
		_imagePiece = new Image[_pieceSet[_setIndex].childCount];
		for (int i = 0; i < _rectPiece.Length; i++)
		{
			_rectPiece[i] = _pieceSet[_setIndex].GetChild(i).GetComponent<RectTransform>();
			_imagePiece[i] = _pieceSet[_setIndex].GetChild(i).GetComponent<Image>();
		}
		for (int j = 0; j < _pieceSet.Length; j++)
		{
			if (_setIndex == j)
			{
				_pieceSet[j].gameObject.SetActive(value: true);
			}
			else
			{
				_pieceSet[j].gameObject.SetActive(value: false);
			}
		}
		float num = _rectBookBinder.rect.width * 0.55f;
		float num2 = _rectBookBinder.rect.height * 0.7f;
		_correctPiecePlace = new Vector2[_rectPiece.Length];
		for (int k = 0; k < _rectPiece.Length; k++)
		{
			_correctPiecePlace[k] = _rectPiece[k].anchoredPosition;
			if (k > 0)
			{
				_rectPiece[k].anchoredPosition = new Vector2(UnityEngine.Random.Range(num * -1f, num), UnityEngine.Random.Range(num2 * -1f, num2));
			}
		}
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
	}

	public void SetMoon(Sprite[] moonSet)
	{
		if (_imageSign.Length == moonSet.Length)
		{
			for (int i = 0; i < _imageSign.Length; i++)
			{
				_imageSign[i].sprite = moonSet[i];
			}
		}
	}

	private void TrySnapPiece()
	{
		if (!_succeed && _grabIndex >= 0)
		{
			if (Vector2.Distance(_rectPiece[_grabIndex].anchoredPosition, _correctPiecePlace[_grabIndex]) < 10f)
			{
				_rectPiece[_grabIndex].anchoredPosition = _correctPiecePlace[_grabIndex];
			}
			if (Compare())
			{
				StartCoroutine(PuzzleUnlocked());
			}
			AudioManager.PlaySFX("puzzle-paper-rustle");
			_grabIndex = -1;
		}
	}

	private bool Compare()
	{
		for (int i = 0; i < _rectPiece.Length; i++)
		{
			if (_rectPiece[i].anchoredPosition != _correctPiecePlace[i])
			{
				return false;
			}
		}
		RemoveHighlight();
		return true;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (_succeed || _inputModeController || eventData.pointerCurrentRaycast.gameObject == null)
		{
			return;
		}
		RectTransform component = eventData.pointerCurrentRaycast.gameObject.transform.parent.GetComponent<RectTransform>();
		if (!(component.parent == _pieceSet[_setIndex]))
		{
			return;
		}
		for (int i = 0; i < _rectPiece.Length; i++)
		{
			if (component == _rectPiece[i])
			{
				_grabIndex = i;
				break;
			}
		}
		_rectPiece[_grabIndex].transform.SetAsLastSibling();
		Vector2 vector = _rectPiece[_grabIndex].position;
		_offset = eventData.position - vector;
		AudioManager.PlaySFX("puzzle-paper-rustle");
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (_grabIndex >= 0)
		{
			_rectPiece[_grabIndex].position = eventData.position - _offset;
			_clampPos = _rectPiece[_grabIndex].position;
			_clampPos.x = Mathf.Clamp(_clampPos.x, 64f, (float)Screen.width - 64f);
			_clampPos.y = Mathf.Clamp(_clampPos.y, 64f, (float)Screen.height - 64f);
			_rectPiece[_grabIndex].position = _clampPos;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		TrySnapPiece();
	}

	public void Action1Press()
	{
		if (_inputModeController && !_succeed)
		{
			if (_grabIndex < 0)
			{
				_grabIndex = _navIndex;
				_rectPiece[_grabIndex].transform.SetAsLastSibling();
				AudioManager.PlaySFX("puzzle-paper-rustle");
			}
			else
			{
				TrySnapPiece();
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
		if (_grabIndex > 0 || (!(Mathf.Abs(direction.x) > 0.5f) && !(Mathf.Abs(direction.y) > 0.5f)) || _succeed)
		{
			return;
		}
		if (_inputModeController)
		{
			int num = 0;
			if ((double)Mathf.Abs(direction.x) > 0.5)
			{
				num = (int)Mathf.Sign(direction.x);
			}
			else if (Mathf.Abs(direction.y) > 0.5f)
			{
				num = (int)Mathf.Sign(direction.y);
			}
			_navIndex += num;
			if (_navIndex >= _imagePiece.Length)
			{
				_navIndex = 1;
			}
			if (_navIndex < 1)
			{
				_navIndex = _imagePiece.Length - 1;
			}
			Highlight();
		}
		else
		{
			_inputModeController = true;
			_navIndex = 1;
			Highlight();
		}
	}

	private void Highlight()
	{
		RemoveHighlight();
		for (int i = 0; i < _imagePiece.Length; i++)
		{
			if (i == _navIndex)
			{
				_imagePiece[i].color = Color.white;
			}
			else
			{
				_imagePiece[i].color = Color.gray;
			}
		}
	}

	private void RemoveHighlight()
	{
		Image[] imagePiece = _imagePiece;
		for (int i = 0; i < imagePiece.Length; i++)
		{
			imagePiece[i].color = Color.white;
		}
	}

	public void OnPointerMove(PointerEventData eventData)
	{
		if (_inputModeController)
		{
			_inputModeController = false;
			_grabIndex = -1;
			RemoveHighlight();
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
		_grabIndex = -1;
		if (_succeed || _puzzleNetworkBehaviour.isComplete)
		{
			for (int i = 0; i < _rectPiece.Length; i++)
			{
				_rectPiece[i].anchoredPosition = _correctPiecePlace[i];
			}
			RevealSigns(instantShow: true);
			return;
		}
		Color endValue = Color.white;
		if (!_succeed)
		{
			endValue = Color.clear;
		}
		for (int j = 0; j < _imageSign.Length; j++)
		{
			_imageSign[j].DOColor(endValue, 0f);
		}
		for (int k = 0; k < _imageRays.Length; k++)
		{
			_imageRays[k].DOKill();
			_imageRays[k].DOColor(endValue, 0f);
		}
	}

	private void OnDirectNavigate(InputAction.CallbackContext context)
	{
		_dirState = context.ReadValue<Vector2>().normalized * 4f;
	}

	private void FixedUpdate()
	{
		if (_inputModeController && _grabIndex >= 0)
		{
			_rectPiece[_grabIndex].position = new Vector2(_rectPiece[_grabIndex].position.x + _dirState.x, _rectPiece[_grabIndex].position.y + _dirState.y);
			_clampPos = _rectPiece[_grabIndex].position;
			_clampPos.x = Mathf.Clamp(_clampPos.x, 64f, (float)Screen.width - 64f);
			_clampPos.y = Mathf.Clamp(_clampPos.y, 64f, (float)Screen.height - 64f);
			_rectPiece[_grabIndex].position = _clampPos;
		}
	}

	public void Hide()
	{
		_directNavigate.action.performed -= OnDirectNavigate;
		_directNavigate.action.canceled -= OnDirectNavigate;
	}

	public void RevealSigns(bool instantShow)
	{
		RemoveHighlight();
		float duration = 1f;
		if (instantShow)
		{
			duration = 0f;
		}
		else
		{
			AudioManager.PlaySFX("puzzle-paper-scan");
		}
		for (int i = 0; i < _imageRays.Length; i++)
		{
			_imageRays[i].DOKill();
			_imageRays[i].DOColor(Color.white, duration);
		}
		for (int j = 0; j < _imageSign.Length; j++)
		{
			_imageSign[j].DOKill();
			_imageSign[j].DOColor(Color.white, duration);
		}
	}

	public IEnumerator PuzzleUnlocked()
	{
		_succeed = true;
		_puzzleNetworkBehaviour.RPCCompleteAndCheckMap((short)_interactableTrigger.UniqueID, NetworkGameManager.Instance.ownPlayer.network.GetIDX());
		yield return new WaitForSeconds(0.5f);
		RevealSigns(instantShow: false);
	}
}

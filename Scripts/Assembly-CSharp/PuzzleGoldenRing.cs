using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzleGoldenRing : MonoBehaviour, IPuzzle, IPointerMoveHandler, IEventSystemHandler
{
	[Header("Internal")]
	[SerializeField]
	private Transform _buttonParent;

	[SerializeField]
	private Transform _ringPlateParent;

	private Image[] _rotateButtons;

	private RectTransform[] _ringPlates;

	private const float ROTATION = 90f;

	private int[][] _plateSet;

	private bool _inputModeController;

	private int _navIndex;

	private bool _rotating;

	private bool _succeed;

	private ItemInteractable _interactableTrigger;

	private float[] _savedInit;

	private const string SFX_ROTATE = "sfx-goldenring-rotate";

	private const string SFX_PUSH = "sfx-goldenring-pushButton";

	private const string SFX_UNLOCK = "sfx-goldenring-unlock";

	private void Start()
	{
		_rotateButtons = new Image[_buttonParent.childCount];
		_ringPlates = new RectTransform[_ringPlateParent.childCount];
		for (int i = 0; i < _rotateButtons.Length; i++)
		{
			_rotateButtons[i] = _buttonParent.GetChild(i).GetComponent<Image>();
		}
		for (int j = 0; j < _ringPlates.Length; j++)
		{
			_ringPlates[j] = _ringPlateParent.GetChild(j).GetComponent<RectTransform>();
		}
		StartCoroutine(Generate());
	}

	private IEnumerator Generate()
	{
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		UnityEngine.Random.InitState(GlobalOptionsManager.Instance.GetSeedCombineWithMissionID());
		_plateSet = new int[4][];
		_plateSet[0] = new int[1];
		_plateSet[1] = new int[2] { 0, 1 };
		_plateSet[2] = new int[1] { 2 };
		_plateSet[3] = new int[2] { 2, 3 };
		for (int i = 0; i < 10; i++)
		{
			int idx = UnityEngine.Random.Range(0, _plateSet.Length);
			RotateInstant(idx);
		}
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
		_savedInit = new float[_ringPlates.Length];
		for (int j = 0; j < _ringPlates.Length; j++)
		{
			_savedInit[j] = _ringPlates[j].eulerAngles.z;
		}
	}

	public void RotateInstant(int idx)
	{
		float zAngle = _ringPlates[idx].eulerAngles.z + 90f;
		_ringPlates[idx].Rotate(0f, 0f, zAngle);
	}

	public void RotateButtonPress(int idx)
	{
		if (_succeed || _rotating)
		{
			return;
		}
		_rotating = true;
		for (int i = 0; i < _plateSet[idx].Length; i++)
		{
			_ringPlates[_plateSet[idx][i]].DOKill();
			float endVal = _ringPlates[_plateSet[idx][i]].localEulerAngles.z + 90f;
			_ringPlates[_plateSet[idx][i]].DOLocalRotate(new Vector3(0f, 0f, endVal), 0.75f).OnComplete(() =>
			{
				_ringPlates[_plateSet[idx][i]].Rotate(0f, 0f, endVal);
			});
		}
		AudioManager.PlaySFX("sfx-goldenring-rotate");
		UniTaskUtil.DelayedCall(this, 0.85f, () =>
		{
			for (int j = 0; j < _ringPlates.Length; j++)
			{
				if (_ringPlates[j].rotation.eulerAngles.z > 2f)
				{
					_rotating = false;
					return;
				}
			}
			StartCoroutine(PuzzleUnlocked());
		}).Forget();
	}

	public void Action1Press()
	{
		if (_inputModeController)
		{
			RotateButtonPress(_navIndex);
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
		if ((!(Mathf.Abs(direction.x) > 0.5f) && !(Mathf.Abs(direction.y) > 0.5f)) || _succeed || _rotating)
		{
			return;
		}
		if (_inputModeController)
		{
			if ((double)Mathf.Abs(direction.x) > 0.5)
			{
				switch (_navIndex)
				{
				case 0:
					_navIndex = 1;
					break;
				case 1:
					_navIndex = 0;
					break;
				case 2:
					_navIndex = 3;
					break;
				case 3:
					_navIndex = 2;
					break;
				}
			}
			else if (Mathf.Abs(direction.y) > 0.5f)
			{
				switch (_navIndex)
				{
				case 0:
					_navIndex = 2;
					break;
				case 1:
					_navIndex = 3;
					break;
				case 2:
					_navIndex = 0;
					break;
				case 3:
					_navIndex = 1;
					break;
				}
			}
			Highlight();
		}
		else
		{
			_inputModeController = true;
			Highlight();
		}
	}

	private void Highlight()
	{
		RemoveHighlight();
		_rotateButtons[_navIndex].DOColor(Color.grey, 0.3f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
		AudioManager.PlaySFX("sfx-goldenring-pushButton");
	}

	private void RemoveHighlight()
	{
		for (int i = 0; i < _rotateButtons.Length; i++)
		{
			_rotateButtons[i].DOKill();
			_rotateButtons[i].color = Color.white;
		}
	}

	public void OnPointerMove(PointerEventData eventData)
	{
		_inputModeController = false;
		RemoveHighlight();
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
		for (int i = 0; i < _ringPlates.Length; i++)
		{
			_ringPlates[i].transform.eulerAngles = new Vector3(0f, 0f, _savedInit[i]);
		}
	}

	public IEnumerator PuzzleUnlocked()
	{
		_succeed = true;
		yield return new WaitForSeconds(0.5f);
		AudioManager.PlaySFX("sfx-goldenring-unlock");
		yield return new WaitForSeconds(0.5f);
		Debug.Log("Puzzle Success");
		yield return new WaitForSeconds(0.2f);
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
}

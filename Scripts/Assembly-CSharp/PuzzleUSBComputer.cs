using System.Collections;
using DG.Tweening;
using Toked;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleUSBComputer : MonoBehaviour, IPuzzle
{
	[Header("Transform Reference")]
	[SerializeField]
	private Image _status;

	[SerializeField]
	private Image _alertWindow;

	[SerializeField]
	private RectTransform[] _windowPanels;

	[SerializeField]
	private RectTransform _progressBar;

	[Header("Data Reference")]
	[SerializeField]
	private Sprite[] _spriteStatus;

	[SerializeField]
	private Sprite[] _spriteAlert;

	private int _itemID;

	private InventoryObject _checkItem;

	private Vector3[] _initPos;

	private ItemInteractable _interactableTrigger;

	private void Start()
	{
		_progressBar.localScale = Vector3.up + Vector3.forward;
		_initPos = new Vector3[_windowPanels.Length];
		for (int i = 0; i < _windowPanels.Length; i++)
		{
			_initPos[i] = _windowPanels[i].localPosition;
		}
		_itemID = 318;
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
		PlayWindowSequence();
	}

	public void Hide()
	{
	}

	private void PlayWindowSequence()
	{
		AudioManager.PlaySFX("usbcomp-pc-keyboard-startup");
		float num = 0.2f;
		Vector3 localPosition = new Vector3(-120f, -100f, 1f);
		_alertWindow.gameObject.SetActive(value: false);
		_alertWindow.sprite = _spriteAlert[0];
		_status.sprite = _spriteStatus[0];
		_progressBar.gameObject.SetActive(value: true);
		_progressBar.DOKill();
		_progressBar.localScale = Vector3.up + Vector3.forward;
		for (int i = 0; i < _windowPanels.Length; i++)
		{
			_windowPanels[i].DOKill();
			_windowPanels[i].localScale = Vector3.zero;
			_windowPanels[i].localPosition = localPosition;
			_windowPanels[i].DOScale(1f, num).SetDelay(num * (float)i);
			_windowPanels[i].DOLocalMove(_initPos[i], num).SetDelay(num * (float)i);
		}
		StartCoroutine(ShowAlertWithDelay(num * (float)_windowPanels.Length));
	}

	private IEnumerator ShowAlertWithDelay(float delay)
	{
		yield return new WaitForSeconds(delay + 0.1f);
		_alertWindow.gameObject.SetActive(value: true);
		_progressBar.DOKill();
		_progressBar.DOScaleX(1f, 2f).SetEase(Ease.Linear).OnComplete(() =>
		{
			DetermineResult();
		});
	}

	private void DetermineResult()
	{
		StartCoroutine(ChangeSpriteAlert());
	}

	private IEnumerator ChangeSpriteAlert()
	{
		yield return new WaitForSeconds(0.2f);
		_progressBar.gameObject.SetActive(value: false);
		_checkItem = NetworkGameManager.Instance.ownPlayer.data.FindInventory(_itemID);
		int num = 1;
		if (_checkItem != null)
		{
			num = 2;
			_status.sprite = _spriteStatus[1];
			StartCoroutine(PuzzleUnlocked());
		}
		else
		{
			AudioManager.PlaySFX("usbcomp-denied");
		}
		_alertWindow.sprite = _spriteAlert[num];
	}

	public IEnumerator PuzzleUnlocked()
	{
		if (_checkItem != null)
		{
			NetworkGameManager.Instance.ownPlayer.data.RemoveInventory(_checkItem.IdxInventory);
		}
		AudioManager.PlaySFX("usbcomp-verified");
		yield return new WaitForSeconds(0.2f);
		UIGameManager.Instance.ShowUIInGame(_interactableTrigger.UIMenu);
		NetworkGameManager.Instance.ownPlayer.network.ExecInteractObject((short)_interactableTrigger.UniqueID, triggerOnReverse: false, isForceInteract: true);
		_interactableTrigger.DisableCollider();
		NetworkGameManager.Instance.ownPlayer.itemCollision = null;
		NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
	}
}

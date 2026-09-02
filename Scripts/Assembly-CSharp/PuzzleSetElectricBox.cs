using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzleSetElectricBox : MonoBehaviour, IPointerMoveHandler, IEventSystemHandler
{
	[SerializeField]
	private List<Button> _interactivePanels;

	[SerializeField]
	private int[] _switchVariant;

	[SerializeField]
	private bool[] _isFaker;

	[SerializeField]
	private List<Image> _swapable;

	[SerializeField]
	private LedRequirements[] _ledReq;

	[SerializeField]
	private List<Image> _missingPanel;

	[SerializeField]
	private List<Button> _missingInteractives;

	private PuzzleElectricBox _puzzleRoot;

	private int[] _correctValues;

	private int[] _initValues;

	private bool _panelInstalled;

	private int _highlightPointer;

	private Image[] _panelImg;

	private Transform[] _panelTransform;

	private bool _isNav;

	public void SetupInteractives(PuzzleElectricBox puzzleRoot, int seed)
	{
		UnityEngine.Random.InitState(seed);
		_puzzleRoot = puzzleRoot;
		_correctValues = new int[_interactivePanels.Count];
		_initValues = new int[_interactivePanels.Count];
		for (int i = 0; i < _interactivePanels.Count; i++)
		{
			_correctValues[i] = (int)_interactivePanels[i].transform.localRotation.eulerAngles.z / 90;
			_initValues[i] = UnityEngine.Random.Range(0, _switchVariant[i]);
			if (_initValues[i] == _correctValues[i])
			{
				_initValues[i] = (_initValues[i] + 1) % _switchVariant[i];
			}
			int indexing = i;
			_interactivePanels[i].onClick.AddListener(() =>
			{
				RotatePanel(_interactivePanels[indexing].transform, indexing);
			});
			_interactivePanels[i].transform.localRotation = Quaternion.Euler(0f, 0f, (float)_initValues[i] * 90f);
		}
		for (int num = 0; num < _ledReq.Length; num++)
		{
			for (int num2 = 0; num2 < _puzzleRoot.LedSpriteMap.Length; num2++)
			{
				if (_puzzleRoot.LedSpriteMap[num2].name == _ledReq[num].LedImage.sprite.name)
				{
					_ledReq[num].LedMap = num2;
					break;
				}
			}
		}
		_panelImg = new Image[_interactivePanels.Count];
		_panelTransform = new Transform[_interactivePanels.Count];
		for (int num3 = 0; num3 < _interactivePanels.Count; num3++)
		{
			_panelImg[num3] = _interactivePanels[num3].GetComponent<Image>();
			_panelTransform[num3] = _interactivePanels[num3].transform;
		}
		foreach (Button missingInteractive in _missingInteractives)
		{
			missingInteractive.gameObject.SetActive(value: false);
		}
		foreach (Image item in _missingPanel)
		{
			item.sprite = _puzzleRoot.MissingPanelSprite;
		}
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
	}

	public void RotatePanel(Transform panel, int idxPoint)
	{
		if (_puzzleRoot.GetFuseState() && !_puzzleRoot.GetSuccess())
		{
			Highlight(idxPoint);
			int siblingIndex = panel.GetSiblingIndex();
			_initValues[siblingIndex] = (_initValues[siblingIndex] + 1) % _switchVariant[siblingIndex];
			panel.DOLocalRotateQuaternion(Quaternion.Euler(0f, 0f, (float)_initValues[siblingIndex] * 90f), 0.25f);
			AudioManager.PlaySFX("puzzle-electricBox-rotate");
			CheckLedRequirement();
			CheckResult();
		}
	}

	private void CheckLedRequirement()
	{
		for (int i = 0; i < _ledReq.Length; i++)
		{
			bool flag = true;
			for (int j = 0; j < _ledReq[i].Interactives.Length; j++)
			{
				int siblingIndex = _ledReq[i].Interactives[i].GetSiblingIndex();
				if (_initValues[siblingIndex] != _correctValues[siblingIndex])
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				_ledReq[i].LedImage.sprite = _puzzleRoot.LedSpriteMap[_ledReq[i].LedMap + 1];
				AudioManager.PlaySFX("puzzle-electricBox-led-on");
			}
			else
			{
				_ledReq[i].LedImage.sprite = _puzzleRoot.LedSpriteMap[_ledReq[i].LedMap];
			}
		}
	}

	private void CheckResult()
	{
		if (_correctValues.Length != _initValues.Length)
		{
			return;
		}
		for (int i = 0; i < _correctValues.Length; i++)
		{
			if (!_isFaker[i] && _initValues[i] != _correctValues[i])
			{
				return;
			}
		}
		List<Image> swapable = _swapable;
		swapable[swapable.Count - 1].sprite = _puzzleRoot.LedSpriteMap[^1];
		_puzzleRoot.TriggerSuccess();
	}

	public void Show()
	{
		_isNav = true;
		if (!_panelInstalled)
		{
			_panelInstalled = true;
			for (int i = 0; i < _missingPanel.Count; i++)
			{
				_missingPanel[i].sprite = _puzzleRoot.SlotPanelSprite;
			}
			for (int j = 0; j < _missingPanel.Count; j++)
			{
				_missingInteractives[j].gameObject.SetActive(value: true);
			}
		}
		Highlight(0);
	}

	public void Navigate(Vector2 direction)
	{
		_isNav = true;
		int num = 0;
		if (direction.x > 0.5f)
		{
			num = 1;
		}
		else if (direction.x < -0.5f)
		{
			num = -1;
		}
		else if (direction.y > 0.5f)
		{
			num = 1;
		}
		else if (direction.y < -0.5f)
		{
			num = -1;
		}
		if (num != 0)
		{
			int num2 = _highlightPointer + num;
			num2 = ((num2 >= 0) ? (num2 % _interactivePanels.Count) : (_interactivePanels.Count - 1));
			_highlightPointer = num2;
			Highlight(_highlightPointer);
		}
	}

	public void OnPointerMove(PointerEventData eventData)
	{
		_isNav = false;
	}

	public void Action1Press()
	{
		if (_isNav)
		{
			RotatePanel(_panelTransform[_highlightPointer], _highlightPointer);
		}
	}

	private void Highlight(int idx)
	{
		_highlightPointer = idx;
		Image[] panelImg = _panelImg;
		foreach (Image obj in panelImg)
		{
			obj.DOKill();
			obj.color = Color.white;
		}
		_panelImg[idx].DOColor(Color.gray, 0.5f).SetLoops(-1, LoopType.Yoyo);
	}

	public IEnumerator PlaySFXWithDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		AudioManager.PlaySFX("electric-box-slot-panel");
	}
}

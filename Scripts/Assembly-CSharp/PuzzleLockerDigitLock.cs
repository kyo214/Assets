using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzleLockerDigitLock : MonoBehaviour, IPuzzle, IPointerMoveHandler, IEventSystemHandler
{
	[SerializeField]
	private List<UIButton> btnNumber = new List<UIButton>();

	[SerializeField]
	private List<Image> btnNumberList = new List<Image>();

	[SerializeField]
	private List<Image> lightList = new List<Image>();

	[SerializeField]
	private List<Sprite> lightRedGreen = new List<Sprite>();

	[SerializeField]
	private List<Sprite> answerRedGreen = new List<Sprite>();

	[SerializeField]
	private Image answerLight;

	[SerializeField]
	private int ctrCorrect;

	[SerializeField]
	private ItemInteractable interactableObject;

	[SerializeField]
	private bool enableTouch = true;

	private int _seed;

	[SerializeField]
	private string password;

	public UIView UIMenu;

	[SerializeField]
	private int inputIdxPass;

	[SerializeField]
	private Image[] _btnImages;

	[SerializeField]
	private Image[] _btnFGImages;

	private Image[][] _btnIndexes;

	private Image[][] _btnFGIndexes;

	private int[][] _btnCommands;

	private int _pRow;

	private int _pCol;

	private bool _isNav;

	private void Start()
	{
		StartCoroutine(GeneratePuzzle());
		int num = 0;
		_btnIndexes = new Image[4][];
		_btnFGIndexes = new Image[4][];
		_btnCommands = new int[4][];
		for (int i = 0; i < _btnIndexes.Length; i++)
		{
			_btnIndexes[i] = new Image[2];
			_btnFGIndexes[i] = new Image[2];
			_btnCommands[i] = new int[2];
			for (int j = 0; j < _btnIndexes[i].Length; j++)
			{
				_btnIndexes[i][j] = _btnImages[num];
				_btnFGIndexes[i][j] = _btnFGImages[num];
				_btnCommands[i][j] = num + 1;
				num++;
			}
		}
		_pRow = 0;
		_pCol = 0;
	}

	public void OnPointerMove(PointerEventData eventData)
	{
		_isNav = false;
		ClearAllHighlight();
	}

	private IEnumerator GeneratePuzzle()
	{
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		UnityEngine.Random.InitState(GlobalOptionsManager.Instance.GetSeedCombineWithMissionID());
		string text = "12345678";
		password = "";
		for (int i = 1; i <= 8; i++)
		{
			int startIndex = UnityEngine.Random.Range(0, text.Length);
			password += text.Substring(startIndex, 1);
			text = text.Remove(startIndex, 1);
		}
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
	}

	public void InputButton(int idx)
	{
		if (!enableTouch)
		{
			return;
		}
		AudioManager.PlaySFX("button-safebox");
		btnNumberList[idx - 1].enabled = true;
		btnNumber[idx - 1].interactable = false;
		int num = password.IndexOf(idx.ToString());
		bool flag;
		if (inputIdxPass == -1)
		{
			lightList[num].sprite = lightRedGreen[1];
			inputIdxPass = num;
			flag = true;
		}
		else
		{
			int num2 = Mathf.Abs(inputIdxPass - num);
			if ((num2 == 1 || num2 == 7) && num2 != 0)
			{
				lightList[num].sprite = lightRedGreen[1];
				inputIdxPass = num;
				flag = true;
			}
			else
			{
				lightList[num].sprite = lightRedGreen[0];
				flag = false;
			}
		}
		lightList[num].enabled = true;
		if (flag)
		{
			ctrCorrect++;
			if (ctrCorrect >= 8)
			{
				StartCoroutine(PuzzleUnlocked());
			}
		}
		else
		{
			ctrCorrect = 0;
			inputIdxPass = -1;
			StartCoroutine(InCorrect());
		}
	}

	private IEnumerator InCorrect()
	{
		enableTouch = false;
		AudioManager.PlaySFX("safebox-failed");
		answerLight.sprite = answerRedGreen[0];
		answerLight.enabled = true;
		for (int i = 0; i < 8; i++)
		{
			btnNumber[i].interactable = false;
		}
		yield return new WaitForSeconds(1f);
		for (int j = 0; j < 8; j++)
		{
			btnNumberList[j].enabled = false;
			lightList[j].enabled = false;
			btnNumber[j].interactable = true;
		}
		answerLight.enabled = false;
		enableTouch = true;
	}

	public void Show()
	{
		ClearAllHighlight();
		enableTouch = true;
		ctrCorrect = 0;
		inputIdxPass = -1;
		for (int i = 0; i < 8; i++)
		{
			btnNumberList[i].enabled = false;
			lightList[i].enabled = false;
			btnNumber[i].interactable = true;
		}
		answerLight.enabled = false;
		answerLight.sprite = answerRedGreen[0];
		_isNav = true;
		_pRow = 0;
		_pCol = 0;
		Highlight(0, 0);
	}

	public void Hide()
	{
	}

	public IEnumerator PuzzleUnlocked()
	{
		AudioManager.PlaySFX("safebox-success");
		answerLight.sprite = answerRedGreen[1];
		answerLight.enabled = true;
		yield return new WaitForSeconds(1f);
		interactableObject.isShowUI = false;
		NetworkGameManager.Instance.ownPlayer.network.ExecInteractObject((short)interactableObject.UniqueID);
		UIGameManager.Instance.ShowUIInGame(interactableObject.UIMenu);
		NetworkGameManager.Instance.ownPlayer.itemCollision = null;
		NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
		NetworkGameManager.Instance.ownPlayer.itemCollision = null;
		NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
	}

	public void InitAnswer()
	{
	}

	public void Navigate(Vector2 direction)
	{
		bool flag = false;
		if (Mathf.Abs(direction.x) > 0.5f)
		{
			int num = _pCol + (int)Mathf.Sign(direction.x);
			num = ((num < 0) ? 1 : (num % 2));
			_pCol = num;
			flag = true;
		}
		else if (Mathf.Abs(direction.y) > 0.5f)
		{
			int num2 = _pRow + (int)Mathf.Sign(direction.y) * -1;
			num2 = ((num2 >= 0) ? (num2 % 4) : 3);
			_pRow = num2;
			flag = true;
		}
		if (flag)
		{
			_isNav = true;
			ClearAllHighlight();
			Highlight(_pRow, _pCol);
		}
	}

	public void Action1Press()
	{
		if (_isNav)
		{
			InputButton(_btnCommands[_pRow][_pCol]);
		}
	}

	public void Action1Release()
	{
	}

	public void SetInteractableObject(ItemInteractable intObject)
	{
		interactableObject = intObject;
	}

	public ItemInteractable GetInteractableObject()
	{
		return interactableObject;
	}

	public void SetPassword(string pass)
	{
	}

	private void Highlight(int row, int col)
	{
		_btnIndexes[row][col].DOKill();
		_btnIndexes[row][col].color = Color.white;
		_btnIndexes[row][col].DOColor(Color.gray, 0.5f).SetLoops(-1, LoopType.Yoyo);
		_btnFGIndexes[row][col].DOKill();
		_btnFGIndexes[row][col].color = Color.white;
		_btnFGIndexes[row][col].DOColor(Color.gray, 0.5f).SetLoops(-1, LoopType.Yoyo);
	}

	private void ClearAllHighlight()
	{
		Image[] btnImages = _btnImages;
		foreach (Image obj in btnImages)
		{
			obj.DOKill();
			obj.color = Color.white;
		}
		btnImages = _btnFGImages;
		foreach (Image obj2 in btnImages)
		{
			obj2.DOKill();
			obj2.color = Color.white;
		}
	}
}

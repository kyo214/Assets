using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Doozy.Runtime.Reactor.Animators;
using Doozy.Runtime.UIManager.Containers;
using TMPro;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzleComputer : MonoBehaviour, IPuzzle, IPointerMoveHandler, IEventSystemHandler
{
	public List<TextMeshProUGUI> listNumber = new List<TextMeshProUGUI>();

	public List<UIAnimator> windows = new List<UIAnimator>();

	public string password;

	public bool passwordVerified;

	public UIView UIMenu;

	public GameObject correct;

	public GameObject incorrect;

	[SerializeField]
	private ItemInteractable interactableObject;

	[SerializeField]
	private CluePuzzle _uiClue;

	[SerializeField]
	private TextMeshProUGUI clueDate;

	[SerializeField]
	private Image[] _btnImages;

	[SerializeField]
	private string[] _inputCommands;

	private Image[][] _btnIndexes;

	private string[][] _arrCommands;

	private int _pRow;

	private int _pCol;

	private bool _isNav;

	private string[] _table = new string[10];

	private string[] _tableAnswer = new string[10];

	private IEnumerator Start()
	{
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		UnityEngine.Random.InitState(GlobalOptionsManager.Instance.GetSeedCombineWithMissionID());
		int num = 20 + UnityEngine.Random.Range(1, 9);
		int num2 = 97 + UnityEngine.Random.Range(-2, 1);
		string text = num + "09" + num2;
		if (_uiClue != null)
		{
			TextMeshProUGUI[] textObject = _uiClue._textObject;
			for (int i = 0; i < textObject.Length; i++)
			{
				_ = textObject[i];
				clueDate.SetText(num + "-09-" + num2);
			}
		}
		string text2 = "0123456789";
		for (int j = 0; j < 10; j++)
		{
			int startIndex = UnityEngine.Random.Range(0, text2.Length);
			_table[j] = text2.Substring(startIndex, 1);
			text2 = text2.Remove(startIndex, 1);
			_uiClue._textObject[j].SetText(_table[j]);
		}
		for (int k = 0; k < 10; k++)
		{
			if (k < 5)
			{
				_tableAnswer[k] = _table[k + 5];
			}
			else
			{
				_tableAnswer[k] = _table[k - 5];
			}
		}
		password = "";
		for (int l = 0; l < text.Length; l++)
		{
			for (int m = 0; m < 10; m++)
			{
				if (text.Substring(l, 1) == _table[m])
				{
					password += _tableAnswer[m];
					break;
				}
			}
		}
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
		int num3 = 0;
		_btnIndexes = new Image[4][];
		_arrCommands = new string[4][];
		for (int n = 0; n < _btnIndexes.Length; n++)
		{
			_btnIndexes[n] = new Image[3];
			_arrCommands[n] = new string[3];
			for (int num4 = 0; num4 < _btnIndexes[n].Length; num4++)
			{
				_btnIndexes[n][num4] = _btnImages[num3];
				_arrCommands[n][num4] = _inputCommands[num3];
				num3++;
			}
		}
		_pRow = 0;
		_pCol = 0;
	}

	public void InitAnswer()
	{
	}

	public void Show()
	{
		ClearAllHighlight();
		foreach (UIAnimator window in windows)
		{
			foreach (TextMeshProUGUI item in listNumber)
			{
				item.text = "_";
			}
			correct.SetActive(value: false);
			incorrect.SetActive(value: false);
			window.Play();
		}
		_isNav = true;
		_pRow = 0;
		_pCol = 0;
		Highlight(0, 0);
	}

	public void Hide()
	{
	}

	public void OnPointerMove(PointerEventData eventData)
	{
		_isNav = false;
		ClearAllHighlight();
	}

	public void InputCommand(string input)
	{
		AudioManager.PlaySFX("puzzle-keyboard-tick");
		if (input != "Del" && input != "Enter")
		{
			foreach (TextMeshProUGUI item in listNumber)
			{
				if (item.text == "_")
				{
					item.text = input;
					break;
				}
			}
			return;
		}
		if (input == "Del")
		{
			for (int num = listNumber.Count - 1; num >= 0; num--)
			{
				if (listNumber[num].text != "_")
				{
					listNumber[num].text = "_";
					break;
				}
			}
		}
		else
		{
			if (!(input == "Enter"))
			{
				return;
			}
			passwordVerified = true;
			for (int i = 0; i < listNumber.Count; i++)
			{
				if (listNumber[i].text != password.Substring(i, 1))
				{
					passwordVerified = false;
				}
			}
			if (passwordVerified)
			{
				interactableObject.transform.gameObject.SetActive(value: false);
				correct.SetActive(value: true);
				StartCoroutine(PuzzleUnlocked());
			}
			else
			{
				incorrect.SetActive(value: true);
				StartCoroutine(DelayInvisibleInCorrect());
			}
		}
	}

	private IEnumerator DelayInvisibleInCorrect()
	{
		yield return new WaitForSeconds(2f);
		incorrect.SetActive(value: false);
	}

	private void Highlight(int row, int col)
	{
		_btnIndexes[row][col].DOKill();
		_btnIndexes[row][col].color = Color.white;
		_btnIndexes[row][col].DOColor(Color.gray, 0.5f).SetLoops(-1, LoopType.Yoyo);
	}

	private void ClearAllHighlight()
	{
		Image[] btnImages = _btnImages;
		foreach (Image obj in btnImages)
		{
			obj.DOKill();
			obj.color = Color.white;
		}
	}

	public void Navigate(Vector2 direction)
	{
		bool flag = false;
		if (Mathf.Abs(direction.x) > 0.5f)
		{
			int num = _pCol + (int)Mathf.Sign(direction.x);
			num = ((num >= 0) ? (num % 3) : 2);
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
			InputCommand(_arrCommands[_pRow][_pCol]);
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

	public IEnumerator PuzzleUnlocked()
	{
		UIGameManager.Instance.UIProgressing = true;
		yield return new WaitForSeconds(2f);
		UIGameManager.Instance.UIProgressing = false;
		interactableObject.UIMenu.Hide();
		ChatSystem.Instance.ItemCommand.SetActive(value: false);
		interactableObject.IsSolved = true;
		if (!interactableObject.afterCompleteShowNote)
		{
			yield break;
		}
		if (interactableObject.note != null)
		{
			NetworkGameManager.Instance.ownPlayer.itemCollision = interactableObject.note.gameObject;
			NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = interactableObject.note.itemCollider;
			if (interactableObject.note.itemType == "Note")
			{
				interactableObject.note.ShowNote();
				interactableObject.note.itemCollider.enabled = true;
			}
		}
		else if (interactableObject.UInote != null)
		{
			NetworkGameManager.Instance.ownPlayer.itemCollision = null;
			NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
			UIGameManager.Instance.UIMenuPuzzle = interactableObject.UInote;
			interactableObject.UInote.Show();
			interactableObject.ObjectActiveAfterComplete.SetActive(value: true);
		}
		NetworkGameManager.Instance.ownPlayer.network.ExecInteractObject((short)interactableObject.UniqueID);
	}
}

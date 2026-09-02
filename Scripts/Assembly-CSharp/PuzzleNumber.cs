using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Doozy.Runtime.UIManager.Containers;
using TMPro;
using Toked;
using UnityEngine;

public class PuzzleNumber : MonoBehaviour, IPuzzle
{
	[SerializeField]
	private RectTransform cursor;

	[SerializeField]
	private List<RectTransform> posCursor = new List<RectTransform>();

	[SerializeField]
	private List<RectTransform> group = new List<RectTransform>();

	[SerializeField]
	private List<int> numberPick = new List<int>();

	[SerializeField]
	private List<TextMeshProUGUI> groupNumber0 = new List<TextMeshProUGUI>();

	[SerializeField]
	private List<TextMeshProUGUI> groupNumber1 = new List<TextMeshProUGUI>();

	[SerializeField]
	private List<TextMeshProUGUI> groupNumber2 = new List<TextMeshProUGUI>();

	[SerializeField]
	private List<TextMeshProUGUI> groupNumber3 = new List<TextMeshProUGUI>();

	[SerializeField]
	private TextMeshProUGUI _textOwner;

	[SerializeField]
	private Transform lockKey;

	[SerializeField]
	private int idxCursor;

	[SerializeField]
	private string password;

	[SerializeField]
	private string inputPassword;

	public UIView UIMenu;

	[SerializeField]
	private bool enableControl;

	[SerializeField]
	private ItemInteractable interactableObject;

	[Header("Randomizer Setup")]
	[SerializeField]
	private CluePuzzle _uiClue;

	[Header("Variant")]
	[SerializeField]
	private bool _isVariant;

	private void Awake()
	{
		if (_uiClue != null)
		{
			for (int i = 0; i < 3; i++)
			{
				_uiClue.ArrValue.Add(new ValueClue());
			}
		}
	}

	private IEnumerator Start()
	{
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		UnityEngine.Random.InitState(GlobalOptionsManager.Instance.GetSeedCombineWithMissionID());
		List<char> list = "0123456789".ToList();
		string text = "";
		for (int i = 0; i < 4; i++)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			text += list[index];
			list.RemoveAt(index);
		}
		password = text;
		int num = UnityEngine.Random.Range(1, 3);
		string[] array = new string[5] { "LASTRI", "MARNI", "SANTI", "MAYA", "NINA" };
		if (_isVariant)
		{
			array[0] = "JOKO";
			array[1] = "SUSILO";
			array[2] = "BASUKI";
			array[3] = "WAGIMAN";
			array[4] = "JUPRI";
		}
		for (int j = 0; j < array.Length; j++)
		{
			string text2 = array[j];
			int num2 = UnityEngine.Random.Range(j, array.Length);
			array[j] = array[num2];
			array[num2] = text2;
		}
		_textOwner.text = array[num];
		if (_uiClue != null)
		{
			for (int k = 0; k < 3; k++)
			{
				_uiClue.ArrValue[k].ValueText.Add(array[k]);
				_uiClue.ArrValue[k].ValueText.Add(UnityEngine.Random.Range(1000, 9999).ToString());
			}
			_uiClue.ArrValue[num].ValueText[0] = array[num];
			_uiClue.ArrValue[num].ValueText[1] = password;
		}
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
	}

	public void InitAnswer()
	{
	}

	public void Show()
	{
		idxCursor = 0;
		enableControl = true;
		cursor.anchoredPosition = new Vector2(posCursor[0].anchoredPosition.x, cursor.anchoredPosition.y);
		cursor.DOAnchorPosY(cursor.anchoredPosition.y - 3f, 0.5f).SetLoops(-1, LoopType.Yoyo);
		inputPassword = "0000";
		for (int i = 0; i < 4; i++)
		{
			numberPick[i] = 0;
		}
		GenerateNumber();
	}

	public void Hide()
	{
	}

	private IEnumerator InCorrect()
	{
		yield return new WaitForSeconds(1f);
	}

	public void Navigate(Vector2 direction)
	{
		if (!enableControl || !(inputPassword != password))
		{
			return;
		}
		if (direction.x <= -0.5f)
		{
			if (idxCursor > 0)
			{
				idxCursor--;
				cursor.DOAnchorPosX(posCursor[idxCursor].anchoredPosition.x, 0.2f);
			}
		}
		else if (direction.x >= 0.5f && idxCursor < 3)
		{
			idxCursor++;
			cursor.DOAnchorPosX(posCursor[idxCursor].anchoredPosition.x, 0.2f);
		}
		if (direction.y <= -0.5f)
		{
			AudioManager.PlaySFX("wooden-safebox-turn");
			group[idxCursor].DOAnchorPosY(20f, 0.2f).OnComplete(() =>
			{
				FinishRotate();
			});
			enableControl = false;
			if (numberPick[idxCursor] - 1 >= 0)
			{
				numberPick[idxCursor]--;
			}
			else
			{
				numberPick[idxCursor] = 9;
			}
		}
		else if (direction.y >= 0.5f)
		{
			AudioManager.PlaySFX("wooden-safebox-turn");
			group[idxCursor].DOAnchorPosY(143f, 0.2f).OnComplete(() =>
			{
				FinishRotate();
			});
			enableControl = false;
			if (numberPick[idxCursor] + 1 <= 9)
			{
				numberPick[idxCursor]++;
			}
			else
			{
				numberPick[idxCursor] = 0;
			}
		}
	}

	public IEnumerator PuzzleUnlocked()
	{
		AudioManager.PlaySFX("wooden-safebox-unlock");
		lockKey.DOLocalRotate(new Vector3(0f, 0f, 180f), 0.4f).SetEase(Ease.InQuad);
		yield return new WaitForSeconds(2f);
		UIGameManager.Instance.ShowUIInGame(interactableObject.UIMenu);
		NetworkGameManager.Instance.ownPlayer.network.ExecInteractObject((short)interactableObject.UniqueID);
		interactableObject.DisableCollider();
		NetworkGameManager.Instance.ownPlayer.itemCollision = null;
		NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
	}

	private void FinishRotate()
	{
		group[idxCursor].anchoredPosition = new Vector2(group[idxCursor].anchoredPosition.x, 83f);
		GenerateNumber();
		enableControl = true;
	}

	private void GenerateNumber()
	{
		for (int i = 0; i < 5; i++)
		{
			if (numberPick[0] + i - 2 < 0)
			{
				groupNumber0[i].text = (numberPick[0] + i - 2 + 10).ToString();
			}
			else if (numberPick[0] + i - 2 >= 10)
			{
				groupNumber0[i].text = (numberPick[0] + i - 2 - 10).ToString();
			}
			else
			{
				groupNumber0[i].text = (numberPick[0] + i - 2).ToString();
			}
			if (numberPick[1] + i - 2 < 0)
			{
				groupNumber1[i].text = (numberPick[1] + i - 2 + 10).ToString();
			}
			else if (numberPick[1] + i - 2 >= 10)
			{
				groupNumber1[i].text = (numberPick[1] + i - 2 - 10).ToString();
			}
			else
			{
				groupNumber1[i].text = (numberPick[1] + i - 2).ToString();
			}
			if (numberPick[2] + i - 2 < 0)
			{
				groupNumber2[i].text = (numberPick[2] + i - 2 + 10).ToString();
			}
			else if (numberPick[2] + i - 2 >= 10)
			{
				groupNumber2[i].text = (numberPick[2] + i - 2 - 10).ToString();
			}
			else
			{
				groupNumber2[i].text = (numberPick[2] + i - 2).ToString();
			}
			if (numberPick[3] + i - 2 < 0)
			{
				groupNumber3[i].text = (numberPick[3] + i - 2 + 10).ToString();
			}
			else if (numberPick[3] + i - 2 >= 10)
			{
				groupNumber3[i].text = (numberPick[3] + i - 2 - 10).ToString();
			}
			else
			{
				groupNumber3[i].text = (numberPick[3] + i - 2).ToString();
			}
		}
		inputPassword = groupNumber0[2].text + groupNumber1[2].text + groupNumber2[2].text + groupNumber3[2].text;
		if (inputPassword == password)
		{
			enableControl = false;
			StartCoroutine(PuzzleUnlocked());
		}
	}

	public void Action1Press()
	{
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
}

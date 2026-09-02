using System;
using System.Collections;
using DG.Tweening;
using Toked;
using UnityEngine;

public class PuzzleSafeBox : MonoBehaviour, IPuzzle
{
	[SerializeField]
	private RectTransform rotaryLock1;

	[SerializeField]
	private RectTransform rotaryLock2;

	[SerializeField]
	private string password;

	[SerializeField]
	private string inputPassword;

	[SerializeField]
	private int prevInput;

	[SerializeField]
	private ItemInteractable interactableObject;

	[SerializeField]
	private bool firstStepIsLeft;

	[SerializeField]
	private bool initial;

	[SerializeField]
	private CluePuzzle _uiClue;

	private IEnumerator Start()
	{
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		UnityEngine.Random.InitState(GlobalOptionsManager.Instance.GetSeedCombineWithMissionID());
		int num = UnityEngine.Random.Range(5, 10);
		int num2 = UnityEngine.Random.Range(1, 5);
		int num3 = UnityEngine.Random.Range(6, 10);
		int num4 = UnityEngine.Random.Range(1, 6);
		password = num.ToString() + num2 + num3 + num4;
		if (_uiClue != null)
		{
			_uiClue._textObject[0].SetText(num + "x");
			_uiClue._textObject[1].SetText(Mathf.RoundToInt(num - num2) + "x");
			_uiClue._textObject[2].SetText(Mathf.RoundToInt(num3 - num2) + "x");
			_uiClue._textObject[3].SetText(Mathf.RoundToInt(num3 - num4) + "x");
		}
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
	}

	public void InitAnswer()
	{
	}

	public void Show()
	{
		rotaryLock1.DOLocalRotate(new Vector3(0f, 0f, 0f), 0f);
		inputPassword = "0" + password;
		if (int.Parse(inputPassword.Substring(0, 1)) == 0)
		{
			inputPassword = inputPassword.Remove(0, 1);
			prevInput = 0;
			initial = true;
		}
	}

	public void Hide()
	{
	}

	public void Navigate(Vector2 direction)
	{
		if (inputPassword.Length == 0 || (!(direction.x <= -0.5f) && !(direction.x >= 0.5f)))
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		bool flag = false;
		if (direction.x <= -0.5f)
		{
			AudioManager.PlaySFX("puzzle_rotatelock");
			rotaryLock1.DOKill(complete: true);
			rotaryLock1.DOLocalRotate(new Vector3(0f, 0f, rotaryLock1.localEulerAngles.z + 36f), 0.2f);
			num = 10 - Mathf.RoundToInt(rotaryLock1.localEulerAngles.z + 36f) / 36;
			flag = true;
		}
		else if (direction.x >= 0.5f)
		{
			AudioManager.PlaySFX("puzzle_rotatelock");
			rotaryLock1.DOKill(complete: true);
			rotaryLock1.DOLocalRotate(new Vector3(0f, 0f, rotaryLock1.localEulerAngles.z - 36f), 0.2f);
			num = 10 - Mathf.RoundToInt(rotaryLock1.localEulerAngles.z - 36f) / 36;
			flag = false;
		}
		num2 = num;
		if (num > 9)
		{
			num -= 10;
		}
		string text = num.ToString();
		if (inputPassword.Length == password.Length + 1)
		{
			if (text == inputPassword.Substring(0, 1))
			{
				prevInput = num;
				inputPassword = inputPassword.Remove(0, 1);
			}
			return;
		}
		bool flag2 = false;
		int num3 = int.Parse(inputPassword.Substring(0, 1));
		Debug.Log("Prev" + prevInput + "    charInputPass" + num3);
		if (prevInput >= num3 && num2 <= prevInput)
		{
			if (text == inputPassword.Substring(0, 1))
			{
				prevInput = num;
				inputPassword = inputPassword.Remove(0, 1);
			}
		}
		else if (prevInput <= num3 && num2 >= prevInput)
		{
			if (text == inputPassword.Substring(0, 1))
			{
				prevInput = num;
				inputPassword = inputPassword.Remove(0, 1);
			}
		}
		else
		{
			prevInput = num;
			if (text == "0")
			{
				inputPassword = password;
			}
			else
			{
				inputPassword = "0" + password;
			}
			initial = true;
			flag2 = true;
		}
		if (initial && flag != firstStepIsLeft)
		{
			prevInput = num;
			if (text == "0")
			{
				inputPassword = password;
			}
			else
			{
				inputPassword = "0" + password;
			}
			initial = true;
		}
		else
		{
			prevInput = num;
			if (!flag2)
			{
				initial = false;
			}
		}
		if (inputPassword.Length == 0)
		{
			StartCoroutine(PuzzleUnlocked());
		}
	}

	public IEnumerator PuzzleUnlocked()
	{
		yield return new WaitForSeconds(0.2f);
		rotaryLock2.DOKill(complete: true);
		rotaryLock2.DOLocalRotate(new Vector3(0f, 0f, 90f), 0.4f).SetEase(Ease.OutQuad);
		AudioManager.PlaySFX("locker-open");
		yield return new WaitForSeconds(1f);
		UIGameManager.Instance.ShowUIInGame(interactableObject.UIMenu);
		NetworkGameManager.Instance.ownPlayer.network.ExecInteractObject((short)interactableObject.UniqueID);
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

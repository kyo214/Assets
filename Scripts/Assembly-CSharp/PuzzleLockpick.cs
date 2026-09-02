using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Toked;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleLockpick : MonoBehaviour, IPuzzle
{
	[SerializeField]
	private RectTransform lockpick;

	[SerializeField]
	private List<Transform> driverList = new List<Transform>();

	[SerializeField]
	private List<Transform> pinList = new List<Transform>();

	[SerializeField]
	private List<RectTransform> pinListVisible = new List<RectTransform>();

	[SerializeField]
	private List<RectTransform> driverListVisible = new List<RectTransform>();

	[SerializeField]
	private List<Image> pinImageListVisible = new List<Image>();

	[SerializeField]
	private List<Image> driverImageListVisible = new List<Image>();

	[SerializeField]
	private List<RectTransform> pinDriverList = new List<RectTransform>();

	[SerializeField]
	private List<Transform> springList = new List<Transform>();

	[SerializeField]
	private float springStrength;

	[SerializeField]
	private float strength;

	[SerializeField]
	private bool pressing;

	[SerializeField]
	private List<float> botBound = new List<float>();

	[SerializeField]
	private int pick;

	[SerializeField]
	private int ctrLockpick;

	[SerializeField]
	private bool enableControl;

	[SerializeField]
	private List<Sprite> spritePinComplete = new List<Sprite>();

	[SerializeField]
	private List<Sprite> spriteDriverComplete = new List<Sprite>();

	[SerializeField]
	private bool success;

	public string pinHeight;

	[SerializeField]
	private ItemInteractable interactableObject;

	[SerializeField]
	private string password;

	private void Awake()
	{
		InitAnswer();
	}

	private void FixedUpdate()
	{
		if (enableControl)
		{
			if (pressing)
			{
				if (lockpick.anchoredPosition.y < -31f)
				{
					float num = Random.Range(-0.2f, strength);
					pinDriverList[pick].anchoredPosition = new Vector2(pinDriverList[pick].anchoredPosition.x, pinDriverList[pick].anchoredPosition.y + num);
					lockpick.anchoredPosition = new Vector2(lockpick.anchoredPosition.x, lockpick.anchoredPosition.y + num);
				}
				else
				{
					pinDriverList[pick].anchoredPosition = new Vector2(pinDriverList[pick].anchoredPosition.x, pinDriverList[pick].anchoredPosition.y - springStrength);
					lockpick.anchoredPosition = new Vector2(lockpick.anchoredPosition.x, lockpick.anchoredPosition.y - springStrength);
				}
			}
			else if (pinDriverList[pick].anchoredPosition.y - springStrength > botBound[pick])
			{
				pinDriverList[pick].anchoredPosition = new Vector2(pinDriverList[pick].anchoredPosition.x, pinDriverList[pick].anchoredPosition.y - springStrength * 2f);
				lockpick.anchoredPosition = new Vector2(lockpick.anchoredPosition.x, lockpick.anchoredPosition.y - springStrength * 2f);
			}
			float num2 = Mathf.Abs(pinDriverList[pick].anchoredPosition.y);
			if (num2 <= 20f)
			{
				pinImageListVisible[pick].color = new Color(1f, 0.8f + num2 / 20f * 0.2f, num2 / 20f);
				driverImageListVisible[pick].color = new Color(1f, 0.8f + num2 / 20f * 0.2f, num2 / 20f);
			}
			else
			{
				pinImageListVisible[pick].color = new Color(1f, 1f, 1f);
				driverImageListVisible[pick].color = new Color(1f, 1f, 1f);
			}
			if (num2 <= 6f)
			{
				ctrLockpick++;
				if (ctrLockpick >= 60)
				{
					ctrLockpick = 0;
					pinList[pick].DOMoveY(485f, 0.5f);
					enableControl = false;
					if (pick >= 4)
					{
						AudioManager.PlaySFX("lockpick_done");
						pressing = false;
						StartCoroutine(PuzzleUnlocked());
						success = true;
					}
					else
					{
						if (pinImageListVisible[pick].name == "PinShort")
						{
							pinImageListVisible[pick].sprite = spritePinComplete[0];
						}
						else if (pinImageListVisible[pick].name == "PinMid")
						{
							pinImageListVisible[pick].sprite = spritePinComplete[1];
						}
						else
						{
							pinImageListVisible[pick].sprite = spritePinComplete[2];
						}
						if (driverImageListVisible[pick].name == "DriverShort")
						{
							driverImageListVisible[pick].sprite = spriteDriverComplete[0];
						}
						else if (driverImageListVisible[pick].name == "DriverMid")
						{
							driverImageListVisible[pick].sprite = spriteDriverComplete[1];
						}
						else
						{
							driverImageListVisible[pick].sprite = spriteDriverComplete[2];
						}
						pinImageListVisible[pick].material.SetFloat("_Brightness", 0.5f);
						pinImageListVisible[pick].material.DOFloat(0f, "_Brightness", 0.3f);
						driverImageListVisible[pick].material.SetFloat("_Brightness", 0.5f);
						driverImageListVisible[pick].material.DOFloat(0f, "_Brightness", 0.3f);
						pick++;
						lockpick.DOAnchorPos(new Vector2(pinDriverList[pick].anchoredPosition.x, -96f), 0.7f);
						AudioManager.PlaySFX("lockpick_done");
						if (!success)
						{
							AudioManager.StopSFX("lockpick_loop");
						}
						success = true;
						UniTaskUtil.DelayedCall(this, 0.75f, () =>
						{
							ChangePick();
						}).Forget();
					}
				}
			}
		}
		float num3 = (lockpick.anchoredPosition.y + 35f) / -60f * 10f;
		if (enableControl)
		{
			springList[pick].DOScaleY(1.4f + num3 / 10f * 0.6f, 0f);
		}
		float z = 0f - (num3 - 5f);
		lockpick.DOLocalRotate(new Vector3(0f, 0f, z), 0f);
	}

	public void InitAnswer()
	{
		pick = 0;
		springStrength = 1f;
		strength = 1f;
		pinHeight = pinHeight.Replace("-", "");
		for (int i = 0; i < pinList.Count; i++)
		{
			driverListVisible.Add(driverList[i].transform.GetChild(int.Parse(pinHeight.Substring(i * 2, 1))) as RectTransform);
			pinListVisible.Add(pinList[i].transform.GetChild(int.Parse(pinHeight.Substring(i * 2 + 1, 1))) as RectTransform);
			pinImageListVisible.Add(pinListVisible[i].GetComponent<Image>());
			driverImageListVisible.Add(driverListVisible[i].GetComponent<Image>());
			pinListVisible[i].gameObject.SetActive(value: true);
			driverListVisible[i].gameObject.SetActive(value: true);
			int num = 0;
			if (pinListVisible[i].name == "PinShort")
			{
				num = -25;
			}
			else if (pinListVisible[i].name == "PinMid")
			{
				num = -14;
			}
			botBound.Add(pinDriverList[i].anchoredPosition.y + (float)num - 40f);
			pinDriverList[i].anchoredPosition = new Vector2(pinDriverList[i].anchoredPosition.x, botBound[i]);
		}
	}

	public void Show()
	{
		lockpick.anchoredPosition = new Vector2(-65f, -97.7f);
		lockpick.DOAnchorPos(new Vector2(pinDriverList[pick].anchoredPosition.x, -96f), 0.7f);
		enableControl = true;
		success = false;
	}

	public void ChangePick()
	{
		success = false;
		enableControl = true;
		if (pressing)
		{
			AudioManager.PlaySFX("lockpick_loop");
		}
	}

	public void Navigate(Vector2 direction)
	{
	}

	public void Action1Press()
	{
		pressing = true;
		if (!success)
		{
			AudioManager.PlaySFX("lockpick_loop");
		}
	}

	public void Action1Release()
	{
		pressing = false;
		if (!success)
		{
			AudioManager.StopSFX("lockpick_loop");
		}
	}

	public IEnumerator PuzzleUnlocked()
	{
		AudioManager.StopSFX("lockpick_loop");
		yield return new WaitForSeconds(0.2f);
		UIGameManager.Instance.ShowUIInGame(interactableObject.UIMenu);
		NetworkGameManager.Instance.ownPlayer.network.ExecInteractObject((short)interactableObject.UniqueID);
		interactableObject.DisableCollider();
		NetworkGameManager.Instance.ownPlayer.itemCollision = null;
		NetworkGameManager.Instance.ownPlayer.itemCollisionCollider = null;
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
		password = pass;
	}

	public void Hide()
	{
	}
}

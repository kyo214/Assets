using System.Collections.Generic;
using DG.Tweening;
using Toked;
using UnityEngine;
using UnityEngine.UI;

public class ChatWheel : MonoBehaviour
{
	[SerializeField]
	private RectTransform _cursor;

	[SerializeField]
	private float angleTransform;

	[SerializeField]
	private int prevAngle;

	[SerializeField]
	private int angle;

	[SerializeField]
	private List<Image> listImageSelection = new List<Image>();

	[SerializeField]
	private float distCursor;

	public static ChatWheel Instance { get; private set; }

	public void MoveCursor(Vector2 inputPosition)
	{
		if (UIGameManager.Instance.uiChatWheel.isHidden)
		{
			return;
		}
		if (!GlobalOptionsManager.Instance.usingGamepad)
		{
			distCursor = MathFunc.Distance(inputPosition, (Vector2)_cursor.position);
			if (distCursor > 90f || GlobalOptionsManager.Instance.usingGamepad)
			{
				Vector2 vector = inputPosition - (Vector2)_cursor.position;
				angleTransform = Mathf.Atan2(vector.y, vector.x) * 57.29578f - 90f;
				if (angleTransform >= 360f)
				{
					angleTransform -= 360f;
				}
				else if (angleTransform < 0f)
				{
					angleTransform += 360f;
				}
				_cursor.rotation = Quaternion.Euler(0f, 0f, angleTransform);
				angle = Mathf.FloorToInt((360f - angleTransform + 22.5f) / 45f);
				if (angle >= 8)
				{
					angle = 0;
				}
				if (listImageSelection[angle].color.a == 0f)
				{
					if (prevAngle >= 0)
					{
						listImageSelection[prevAngle].DOKill();
						listImageSelection[prevAngle].DOFade(0f, 0.2f);
					}
					listImageSelection[angle].DOKill();
					listImageSelection[angle].DOFade(1f, 0.2f);
					prevAngle = angle;
				}
			}
			else if (prevAngle >= 0)
			{
				listImageSelection[prevAngle].DOKill();
				listImageSelection[prevAngle].DOFade(0f, 0.2f);
			}
			return;
		}
		distCursor = MathFunc.Distance(inputPosition, Vector2.zero);
		if (distCursor > 0f)
		{
			Vector2 vector2 = inputPosition;
			angleTransform = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f - 90f;
			if (angleTransform >= 360f)
			{
				angleTransform -= 360f;
			}
			else if (angleTransform < 0f)
			{
				angleTransform += 360f;
			}
			_cursor.rotation = Quaternion.Euler(0f, 0f, angleTransform);
			angle = Mathf.FloorToInt((360f - angleTransform + 22.5f) / 45f);
			if (angle >= 8)
			{
				angle = 0;
			}
			if (listImageSelection[angle].color.a == 0f)
			{
				if (prevAngle >= 0)
				{
					listImageSelection[prevAngle].DOKill();
					listImageSelection[prevAngle].DOFade(0f, 0.2f);
				}
				listImageSelection[angle].DOKill();
				listImageSelection[angle].DOFade(1f, 0.2f);
				prevAngle = angle;
			}
		}
		else if (prevAngle >= 0)
		{
			listImageSelection[prevAngle].DOKill();
			listImageSelection[prevAngle].DOFade(0f, 0.2f);
		}
	}

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	public void ShowChatWheel()
	{
		prevAngle = -1;
		angle = -1;
		UIGameManager.Instance.uiChatWheel.Show();
		foreach (Image item in listImageSelection)
		{
			item.DOFade(0f, 0f);
		}
	}

	public void HideChatWheel()
	{
		UIGameManager.Instance.uiChatWheel.Hide();
		if (((!GlobalOptionsManager.Instance.usingGamepad && distCursor > 90f) || (GlobalOptionsManager.Instance.usingGamepad && distCursor > 0f)) && angle >= 0)
		{
			int itemID = -1;
			if (NetworkGameManager.Instance.ownPlayer.weaponController.idWeaponRange > 0)
			{
				itemID = BGDatabase_Weapon.GetEntityByKeyid(NetworkGameManager.Instance.ownPlayer.weaponController.idBaseWeaponRange).AmmoTypeID;
			}
			NetworkGameManager.Instance.ownPlayer.network.ShowBaloonChat((ChatType)(angle + 20), itemID, -1, -1, -1, 10);
		}
	}
}

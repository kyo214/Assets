using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CluePuzzlePoliceID : MonoBehaviour, IPuzzle
{
	[Header("External Reference")]
	[SerializeField]
	private Sprite[] _policeIds;

	[Header("Internal Reference")]
	[SerializeField]
	private Image _policeIdImage;

	[SerializeField]
	private TextMeshProUGUI _memberId;

	[SerializeField]
	private TextMeshProUGUI _birthDate;

	[SerializeField]
	private Image[] _arrowButtons;

	private string[] _policeMemberIds;

	private string[] _policeBirthDates;

	private string[] _months;

	private int _activeSlide;

	private Color _fadeColor;

	public bool InitComplete;

	private void Start()
	{
		_months = new string[12]
		{
			"JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT",
			"NOV", "DEC"
		};
		_fadeColor = new Color(1f, 1f, 1f, 0.35f);
		InitComplete = true;
	}

	public void SetupPoliceData(string[] memberIds, string[] birthDates)
	{
		_policeMemberIds = memberIds;
		_policeBirthDates = birthDates;
		RenderSlide();
	}

	public void ChangeSlide(int dir)
	{
		_activeSlide = (_activeSlide + dir) % _policeIds.Length;
		if (_activeSlide < 0)
		{
			_activeSlide = _policeIds.Length - 1;
		}
		RenderSlide();
	}

	private void RenderSlide()
	{
		_policeIdImage.sprite = _policeIds[_activeSlide];
		if (_policeMemberIds != null && _policeMemberIds.Length != 0)
		{
			_memberId.text = _policeMemberIds[_activeSlide].Substring(0, 2) + ".";
			TextMeshProUGUI memberId = _memberId;
			memberId.text = memberId.text + _policeMemberIds[_activeSlide].Substring(2, 2) + ".";
			_memberId.text += _policeMemberIds[_activeSlide].Substring(4, 2);
			_birthDate.text = _policeBirthDates[_activeSlide].Substring(0, 2);
			int num = int.Parse(_policeBirthDates[_activeSlide].Substring(2, 2)) - 1;
			Debug.Log("Clue Police Computer MONTH INDEX : " + num);
			TextMeshProUGUI birthDate = _birthDate;
			birthDate.text = birthDate.text + "-" + _months[num];
			TextMeshProUGUI birthDate2 = _birthDate;
			birthDate2.text = birthDate2.text + "-19" + _policeBirthDates[_activeSlide].Substring(4, 2);
		}
	}

	public void Action1Press()
	{
	}

	public void Action1Release()
	{
	}

	public ItemInteractable GetInteractableObject()
	{
		throw new NotImplementedException();
	}

	public void Hide()
	{
		Image[] arrowButtons = _arrowButtons;
		foreach (Image obj in arrowButtons)
		{
			obj.DOKill();
			obj.color = Color.white;
		}
	}

	public void InitAnswer()
	{
	}

	public void Navigate(Vector2 direction)
	{
		if ((double)Mathf.Abs(direction.x) > 0.5)
		{
			ChangeSlide((int)Mathf.Sign(direction.x));
		}
	}

	public IEnumerator PuzzleUnlocked()
	{
		throw new NotImplementedException();
	}

	public void SetInteractableObject(ItemInteractable intObject)
	{
	}

	public void SetPassword(string pass)
	{
	}

	public void Show()
	{
		Image[] arrowButtons = _arrowButtons;
		foreach (Image obj in arrowButtons)
		{
			obj.DOKill();
			obj.color = Color.white;
			obj.DOColor(_fadeColor, 0.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
		}
		RenderSlide();
	}
}

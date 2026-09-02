using System;
using System.Collections;
using System.Text;
using DG.Tweening;
using TMPro;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzlePoliceComputer : MonoBehaviour, IPuzzle, IPointerMoveHandler, IEventSystemHandler
{
	[Header("Setup Properties")]
	[SerializeField]
	private string[] _officerNames;

	[Header("Internal Components")]
	[SerializeField]
	private Image _authPopup;

	[SerializeField]
	private RectTransform _rtInputWindowFrame;

	[SerializeField]
	private TextMeshProUGUI _tmOfficerName;

	[SerializeField]
	private TextMeshProUGUI[] _tmInputChars;

	[SerializeField]
	private RectTransform _rtProgressWindow;

	[SerializeField]
	private RectTransform _rtProgressBar;

	[SerializeField]
	private RectTransform _rtEmailWindow;

	[SerializeField]
	private TextMeshProUGUI _tmOfficerNameEmail;

	[SerializeField]
	private TextMeshProUGUI _tmEmailBody;

	[SerializeField]
	private Transform _buttonInputGroup;

	[SerializeField]
	private RectTransform _imgCursor;

	[Header("External Components")]
	[SerializeField]
	private CluePuzzlePoliceID _cluePoliceIdComputer;

	[SerializeField]
	private Sprite _sprAuthDeclined;

	[SerializeField]
	private Sprite _sprAuthAccepted;

	[SerializeField]
	private ItemInteractable interactableObject;

	private StringBuilder _stringBuilder;

	private string _key;

	private int _inputCursor;

	private int _activeOfficer;

	private bool _inputAvailable;

	private float _frameOriginalScale;

	private float _rtProgressBarOriginalWidth;

	private string _tArmoryClue;

	private StringBuilder _tBodyEmail;

	private RectTransform[] _buttonInputsRtCache;

	private Button[] _buttonInputsCache;

	private bool _isNavMode;

	private int _nav;

	private const string SFX_KEYBOARD_CLICK = "sfx-policecomp-keyboard-click";

	private const string SFX_PWD_ACCEPT = "sfx-policecomp-accept";

	private const string SFX_PWD_DECLINE = "sfx-policecomp-decline";

	private const string SFX_OLDCOMP_BOOT = "sfx-policecomp-boot";

	public bool IsSolved;

	private void Start()
	{
		_stringBuilder = new StringBuilder();
		_frameOriginalScale = _rtInputWindowFrame.localScale.x;
		_rtProgressBarOriginalWidth = _rtProgressBar.sizeDelta.x;
		_buttonInputsCache = new Button[_buttonInputGroup.childCount];
		_buttonInputsRtCache = new RectTransform[_buttonInputGroup.childCount];
		for (int i = 0; i < _buttonInputsCache.Length; i++)
		{
			_buttonInputsCache[i] = _buttonInputGroup.GetChild(i).GetComponent<Button>();
			_buttonInputsRtCache[i] = _buttonInputGroup.GetChild(i).GetComponent<RectTransform>();
		}
		StartCoroutine(GeneratePuzzle());
	}

	private IEnumerator GeneratePuzzle()
	{
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		yield return new WaitUntil(() => _tArmoryClue != null);
		UnityEngine.Random.InitState(GlobalOptionsManager.Instance.GetSeedCombineWithMissionID());
		Debug.Log("Police Computer SEED : " + GlobalOptionsManager.Instance.GetSeedCombineWithMissionID());
		if (_officerNames.Length == 5)
		{
			_activeOfficer = UnityEngine.Random.Range(0, _officerNames.Length);
			_tmOfficerName.text = _officerNames[_activeOfficer];
		}
		_stringBuilder = new StringBuilder(_tmEmailBody.text.ToString());
		_key = "";
		_key = "000000";
		int num = 5;
		string[] memberBirthDates = new string[num];
		string[] memberIds = new string[num];
		int num2 = UnityEngine.Random.Range(23, 30);
		int num3 = UnityEngine.Random.Range(45, 50);
		for (int num4 = 0; num4 < num; num4++)
		{
			int num5 = UnityEngine.Random.Range(1, 29);
			int num6 = UnityEngine.Random.Range(1, 13);
			int num7 = UnityEngine.Random.Range(50, 65);
			if (num5 < 10)
			{
				memberBirthDates[num4] += "0";
			}
			memberBirthDates[num4] += num5;
			if (num6 < 10)
			{
				memberBirthDates[num4] += "0";
			}
			memberBirthDates[num4] += num6;
			memberBirthDates[num4] += num7;
			int num8 = UnityEngine.Random.Range(10, 35);
			memberIds[num4] = num2.ToString() + (num3 + num4 / 2) + num8;
			int result = 0;
			int result2 = 0;
			int.TryParse(memberBirthDates[num4], out result);
			int.TryParse(memberIds[num4], out result2);
			result += result2;
			if (num4 == _activeOfficer)
			{
				_key = result.ToString();
			}
		}
		_tmOfficerNameEmail.text = _officerNames[_activeOfficer];
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
		if (_cluePoliceIdComputer != null)
		{
			yield return new WaitUntil(() => _cluePoliceIdComputer.InitComplete);
			_cluePoliceIdComputer.SetupPoliceData(memberIds, memberBirthDates);
		}
	}

	public void SetClue(string strClue)
	{
		_tArmoryClue = strClue;
	}

	public void ResetPuzzle()
	{
		for (int i = 0; i < _tmInputChars.Length; i++)
		{
			_tmInputChars[i].text = "";
		}
		_inputCursor = 0;
	}

	public void ClearInput()
	{
		AudioManager.PlaySFX("sfx-policecomp-keyboard-click");
		if (_inputAvailable)
		{
			ResetPuzzle();
		}
	}

	public void InputChar(int inputedNum)
	{
		AudioManager.PlaySFX("sfx-policecomp-keyboard-click");
		if (_inputAvailable && _inputCursor < _tmInputChars.Length)
		{
			_tmInputChars[_inputCursor].text = inputedNum.ToString();
			_inputCursor++;
		}
	}

	public void Submit()
	{
		AudioManager.PlaySFX("sfx-policecomp-keyboard-click");
		if (_inputAvailable)
		{
			string text = "";
			for (int i = 0; i < _tmInputChars.Length; i++)
			{
				text += _tmInputChars[i].text.ToString();
			}
			if (text == _key)
			{
				StartCoroutine(UnlockFiles(accept: true));
			}
			else
			{
				StartCoroutine(UnlockFiles(accept: false));
			}
		}
	}

	private IEnumerator UnlockFiles(bool accept)
	{
		_inputAvailable = false;
		yield return new WaitForSeconds(0f);
		_rtProgressBar.DOSizeDelta(new Vector2(0f, _rtProgressBar.sizeDelta.y), 0f);
		_rtProgressWindow.gameObject.SetActive(value: true);
		_rtProgressBar.DOSizeDelta(new Vector2(_rtProgressBarOriginalWidth, _rtProgressBar.sizeDelta.y), 1.5f).SetEase(Ease.Linear);
		yield return new WaitForSeconds(1.75f);
		_rtProgressWindow.gameObject.SetActive(value: false);
		if (!accept)
		{
			AudioManager.PlaySFX("sfx-policecomp-decline");
			_authPopup.sprite = _sprAuthDeclined;
			_authPopup.gameObject.SetActive(value: true);
			ResetPuzzle();
			yield return new WaitForSeconds(1.5f);
			_authPopup.gameObject.SetActive(value: false);
			_inputAvailable = true;
		}
		else
		{
			StartCoroutine(PuzzleUnlocked());
		}
	}

	public void Action1Press()
	{
		if (_isNavMode)
		{
			_buttonInputsCache[_nav].onClick.Invoke();
		}
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
	}

	public void InitAnswer()
	{
	}

	public void Navigate(Vector2 direction)
	{
		if (Mathf.Abs(direction.x) > 0f || Mathf.Abs(direction.y) > 0f)
		{
			_isNavMode = true;
			int num = _nav / 3;
			int num2 = _nav % 3;
			if (Mathf.Abs(direction.x) > 0.5f)
			{
				num2 += (int)Mathf.Sign(direction.x);
				num2 = ((num2 >= 0) ? (num2 % 3) : 2);
			}
			else if (Mathf.Abs(direction.y) > 0.5f)
			{
				num -= (int)Mathf.Sign(direction.y);
				num = ((num >= 0) ? (num % 4) : 3);
			}
			_nav = num * 3 + num2;
			Highlight();
		}
	}

	private void Highlight()
	{
		if (_nav < _buttonInputsRtCache.Length)
		{
			_imgCursor.gameObject.SetActive(value: true);
			_isNavMode = true;
			_imgCursor.position = _buttonInputsRtCache[_nav].position;
		}
	}

	public IEnumerator PuzzleUnlocked()
	{
		IsSolved = true;
		AudioManager.PlaySFX("sfx-policecomp-accept");
		_authPopup.sprite = _sprAuthAccepted;
		_authPopup.gameObject.SetActive(value: true);
		yield return new WaitForSeconds(1.5f);
		_rtEmailWindow.gameObject.SetActive(value: true);
		NetworkGameManager.Instance.ownPlayer.network.ExecInteractObject((short)interactableObject.UniqueID);
	}

	public void SetInteractableObject(ItemInteractable intObject)
	{
		interactableObject = intObject;
	}

	public void SetPassword(string pass)
	{
	}

	public void Show()
	{
		_tmOfficerNameEmail.text = _officerNames[_activeOfficer];
		_stringBuilder.Replace("[CODE]", _tArmoryClue);
		_tmEmailBody.text = _stringBuilder.ToString();
		if (IsSolved)
		{
			_rtEmailWindow.gameObject.SetActive(value: true);
			return;
		}
		_nav = 0;
		_rtEmailWindow.gameObject.SetActive(value: false);
		_inputAvailable = true;
		AudioManager.PlaySFX("sfx-policecomp-boot");
		ResetPuzzle();
		_authPopup.gameObject.SetActive(value: false);
		_rtProgressWindow.gameObject.SetActive(value: false);
		_rtProgressBar.DOSizeDelta(new Vector2(0f, _rtProgressBar.sizeDelta.y), 0f);
		_rtInputWindowFrame.localScale = new Vector3(0f, 0f, 1f);
		_rtInputWindowFrame.DOScaleX(_frameOriginalScale, 0.25f);
		_rtInputWindowFrame.DOScaleY(_frameOriginalScale, 0.25f);
		Debug.Log(_key);
	}

	public void OnPointerMove(PointerEventData eventData)
	{
		_isNavMode = false;
		_imgCursor.gameObject.SetActive(value: false);
	}
}

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Modules.Cutscene.Scripts;

public class AfterCreditCutsceneController : MonoBehaviour
{
	[SerializeField]
	private CutsceneManager _cutsceneManagerPrefab;

	private CinematicBlackBarController _blackBarController;

	[Header("Scroll Settings")]
	[SerializeField]
	private RectTransform _textRect;

	[SerializeField]
	private float _scrollSpeed = 200f;

	[SerializeField]
	private float _endYPosition = 1350f;

	[SerializeField]
	private float _fastMultiplier = 3f;

	[Header("Fade Settings")]
	[SerializeField]
	private Image _fadeImage;

	[SerializeField]
	private float _fadeDuration = 1.5f;

	[Header("Skip Hold Settings")]
	[SerializeField]
	private Image _skipCircle;

	[SerializeField]
	private float _holdDuration = 2f;

	[Header("Scene")]
	[SerializeField]
	private string _mainMenuSceneName = "MainMenu";

	[Header("Input")]
	[SerializeField]
	private KeyCode _fastKey = KeyCode.Space;

	[SerializeField]
	private KeyCode _skipKey = KeyCode.Escape;

	private float _currentSpeed;

	private float _holdTimer;

	private bool _isHolding;

	private bool _isFadingOut;

	private CinematicBlackBarController BlackBarController
	{
		get
		{
			if (_blackBarController == null)
			{
				_blackBarController = GenericSingleton<CutsceneManager>.InstanceNoCallback?.CinematicBlackBarController ?? Object.Instantiate(_cutsceneManagerPrefab).CinematicBlackBarController;
			}
			return _blackBarController;
		}
	}

	private void Start()
	{
		_currentSpeed = _scrollSpeed;
		_skipCircle.fillAmount = 0f;
		_fadeImage.color = new Color(0f, 0f, 0f, 1f);
		_fadeImage.DOFade(0f, _fadeDuration);
	}

	private void Update()
	{
		if (!_isFadingOut)
		{
			HandleScroll();
			HandleFastForward();
			HandleHoldSkip();
			CheckEndReached();
		}
	}

	private void HandleScroll()
	{
		Vector2 anchoredPosition = _textRect.anchoredPosition;
		anchoredPosition.y += _currentSpeed * Time.deltaTime;
		_textRect.anchoredPosition = anchoredPosition;
	}

	private void HandleFastForward()
	{
		if (Input.GetKey(_fastKey) || Input.GetButton("Submit") || Input.GetMouseButton(0))
		{
			_currentSpeed = _scrollSpeed * _fastMultiplier;
		}
		else
		{
			_currentSpeed = _scrollSpeed;
		}
	}

	private void HandleHoldSkip()
	{
		if (Input.GetKey(_skipKey) || Input.GetButton("Cancel"))
		{
			if (!_isHolding)
			{
				_isHolding = true;
				_holdTimer = 0f;
			}
			_holdTimer += Time.deltaTime;
			_skipCircle.fillAmount = _holdTimer / _holdDuration;
			if (_holdTimer >= _holdDuration)
			{
				StartFadeOut();
				Reset();
			}
		}
		else if (_isHolding)
		{
			Reset();
		}
		void Reset()
		{
			_holdTimer = 0f;
			_skipCircle.fillAmount = 0f;
			_isHolding = false;
		}
	}

	private void StartFadeOut()
	{
		if (!_isFadingOut)
		{
			_isFadingOut = true;
			OnCreditsFinished();
		}
	}

	private void OnCreditsFinished()
	{
		BlackBarController.HideBar(null);
		_fadeImage.DOFade(1f, _fadeDuration).OnComplete(() =>
		{
			GlobalUIManager.Instance.ClickGoToScene(_mainMenuSceneName);
		});
	}

	private void CheckEndReached()
	{
		if (_textRect.anchoredPosition.y >= _endYPosition && !_isFadingOut)
		{
			StartFadeOut();
		}
	}

	public void OnStart()
	{
		BlackBarController.ShowBar(null);
	}
}

using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Modules.Cutscene.Scripts;

public class CinematicBlackBarController : MonoBehaviour
{
	[SerializeField]
	private RectTransform _blackBarRectTransform;

	[SerializeField]
	private Image _inputBlockerImage;

	[SerializeField]
	private Image _topBlackBarImage;

	[SerializeField]
	private Image _bottomBlackBarImage;

	[SerializeField]
	private Image _fadeBlackImage;

	private Coroutine _showBlackBarCoroutine;

	private Coroutine _hideBlackBarCoroutine;

	public void ShowBar(Action onCompleteAction)
	{
		if (_showBlackBarCoroutine != null)
		{
			StopCoroutine(_showBlackBarCoroutine);
			_showBlackBarCoroutine = null;
		}
		_showBlackBarCoroutine = StartCoroutine(DoShowBarCoroutine(onCompleteAction));
	}

	public void HideBar(Action onCompleteAction)
	{
		if (_hideBlackBarCoroutine != null)
		{
			StopCoroutine(_hideBlackBarCoroutine);
			_hideBlackBarCoroutine = null;
		}
		_hideBlackBarCoroutine = StartCoroutine(DoHideBarCoroutine(onCompleteAction));
	}

	public void FadeBlack()
	{
		_fadeBlackImage.color = new Color(0f, 0f, 0f, 0f);
		_fadeBlackImage.gameObject.SetActive(value: true);
		_fadeBlackImage.DOFade(1f, 0.75f);
	}

	private IEnumerator DoShowBarCoroutine(Action onCompleteAction)
	{
		if (UIGameManager.Instance != null)
		{
			UIGameManager.Instance.SetUIVisibility(setActiveUI: false);
		}
		_inputBlockerImage.gameObject.SetActive(value: true);
		_fadeBlackImage.gameObject.SetActive(value: false);
		_blackBarRectTransform.gameObject.SetActive(value: false);
		_topBlackBarImage.DOComplete();
		_bottomBlackBarImage.DOComplete();
		_bottomBlackBarImage.rectTransform.anchoredPosition = new Vector2(0f, 0f - _bottomBlackBarImage.rectTransform.sizeDelta.y);
		_topBlackBarImage.rectTransform.anchoredPosition = new Vector2(0f, _topBlackBarImage.rectTransform.sizeDelta.y);
		yield return null;
		_blackBarRectTransform.gameObject.SetActive(value: true);
		_topBlackBarImage.rectTransform.DOAnchorPosY(0f, 0.3f).SetUpdate(isIndependentUpdate: true);
		yield return _bottomBlackBarImage.rectTransform.DOAnchorPosY(0f, 0.3f).SetUpdate(isIndependentUpdate: true).OnComplete(() =>
		{
			onCompleteAction?.Invoke();
		});
	}

	private IEnumerator DoHideBarCoroutine(Action onCompleteAction)
	{
		_topBlackBarImage.DOComplete();
		_bottomBlackBarImage.DOComplete();
		_topBlackBarImage.rectTransform.DOAnchorPosY(_topBlackBarImage.rectTransform.sizeDelta.y, 0.3f).SetUpdate(isIndependentUpdate: true);
		yield return _bottomBlackBarImage.rectTransform.DOAnchorPosY(0f - _bottomBlackBarImage.rectTransform.sizeDelta.y, 0.3f).SetUpdate(isIndependentUpdate: true).OnComplete(() =>
		{
			_inputBlockerImage.gameObject.SetActive(value: false);
			_fadeBlackImage.gameObject.SetActive(value: false);
			_blackBarRectTransform.gameObject.SetActive(value: false);
			UIGameManager.Instance?.SetUIVisibility(setActiveUI: true);
			onCompleteAction?.Invoke();
		});
	}
}

using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Disclaimer : MonoBehaviour
{
	[SerializeField]
	private Image _fading;

	[SerializeField]
	private GameObject _objPressKey;

	[SerializeField]
	private bool _isFadingComplete;

	[SerializeField]
	private bool _isShowPressKey;

	[SerializeField]
	private bool _isClicked;

	[SerializeField]
	private string _nexScene;

	private void Start()
	{
		_fading.DOFade(0f, 1.5f).OnComplete(() =>
		{
			FadeinComplete();
		});
	}

	private void FadeinComplete()
	{
		_isFadingComplete = true;
		UniTaskUtil.DelayedCall(this, 3f, () =>
		{
			ShowPressKey();
		}).Forget();
	}

	private void ShowPressKey()
	{
		_isShowPressKey = true;
		_objPressKey.SetActive(value: true);
	}

	private void Update()
	{
		if (_isFadingComplete && _isShowPressKey && Input.anyKey && !_isClicked && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
		{
			_isClicked = true;
			GlobalUIManager.Instance.ClickGoToScene(_nexScene);
		}
	}
}

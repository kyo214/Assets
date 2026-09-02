using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace _Modules.UIResult.Scripts;

public class ExtractionContainerAnimation : MonoBehaviour
{
	[SerializeField]
	private Image _image;

	[SerializeField]
	private Sprite _closeContainerSprite;

	[SerializeField]
	private Sprite _openContainerSprite;

	public void Init(Action onCompleteCallback)
	{
		StartCoroutine(DoAnimation(onCompleteCallback));
	}

	private IEnumerator DoAnimation(Action onCompleteCallback)
	{
		_image.gameObject.SetActive(value: false);
		_image.sprite = _closeContainerSprite;
		_image.gameObject.SetActive(value: true);
		yield return new WaitForSecondsRealtime(0.3f);
		_image.sprite = _openContainerSprite;
		yield return new WaitForSecondsRealtime(0.6f);
		_image.gameObject.SetActive(value: false);
		onCompleteCallback?.Invoke();
	}
}

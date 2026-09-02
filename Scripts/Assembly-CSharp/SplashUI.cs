using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashUI : MonoBehaviour
{
	[SerializeField]
	private Image _togeLogo;

	[SerializeField]
	private AudioSource _bgmSource;

	[SerializeField]
	private string _nextScene;

	private Color _transparent;

	private void Start()
	{
		_transparent = new Color(1f, 1f, 1f, 0f);
		_togeLogo.color = _transparent;
		Invoke("LogoTweenIn", 1f);
	}

	private void LogoTweenIn()
	{
		PlaySplashBGM();
		_togeLogo.DOKill();
		_togeLogo.DOColor(Color.white, 1f).OnComplete(() =>
		{
			Invoke("LogoTweenOut", 4f);
		});
	}

	private void LogoTweenOut()
	{
		_togeLogo.DOKill();
		_togeLogo.DOColor(_transparent, 1f).OnComplete(() =>
		{
			Invoke("NextScene", 1f);
		});
	}

	private void PlaySplashBGM()
	{
		_bgmSource.Play();
	}

	private void NextScene()
	{
		SceneManager.LoadScene(_nextScene);
	}
}

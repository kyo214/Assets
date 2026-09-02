using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasySubtitles;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayingScript : MonoBehaviour
{
	private bool _exit;

	[SerializeField]
	private SubtitlePlayer _subtitlePlayer;

	[SerializeField]
	private Image _imageFadeBlack;

	[SerializeField]
	private VideoPlayer _videoPlayer;

	[SerializeField]
	private bool _isFadingComplete;

	private void Start()
	{
		if ((bool)_videoPlayer.clip)
		{
			UniTaskUtil.DelayedCall(this, 1.5f, () =>
			{
				_isFadingComplete = true;
			}).Forget();
			_imageFadeBlack.DOFade(0f, 3f).SetDelay(1f);
			LoadSubtitle();
		}
		else if (GameModes.Instance.isInitDemo)
		{
			SceneManager.LoadSceneAsync("MainMenuFriendPass");
		}
		else
		{
			SceneManager.LoadSceneAsync("MainMenu");
		}
	}

	private void OnEnable()
	{
		_videoPlayer.loopPointReached += OnVideoComplete;
	}

	private void OnDisable()
	{
		_videoPlayer.loopPointReached -= OnVideoComplete;
	}

	private void OnVideoComplete(VideoPlayer source)
	{
		_exit = true;
		if (GameModes.Instance.isInitDemo)
		{
			_imageFadeBlack.DOFade(1f, 0.5f).OnComplete(() =>
			{
				SceneManager.LoadSceneAsync("MainMenuFriendPass");
			});
		}
		else
		{
			_imageFadeBlack.DOFade(1f, 0.5f).OnComplete(() =>
			{
				SceneManager.LoadSceneAsync("MainMenu");
			});
		}
	}

	private void FixedUpdate()
	{
		if (!_isFadingComplete)
		{
			return;
		}
		InputSystem.onAnyButtonPress.CallOnce((InputControl ctrl) =>
		{
			if (!_exit)
			{
				_exit = true;
				if (GameModes.Instance.isInitDemo)
				{
					_imageFadeBlack.DOFade(1f, 0.5f).OnComplete(() =>
					{
						SceneManager.LoadSceneAsync("MainMenuFriendPass");
					});
				}
				else
				{
					_imageFadeBlack.DOFade(1f, 0.5f).OnComplete(() =>
					{
						SceneManager.LoadSceneAsync("MainMenu");
					});
				}
			}
		});
	}

	private void LoadSubtitle()
	{
		_subtitlePlayer.Play();
	}
}

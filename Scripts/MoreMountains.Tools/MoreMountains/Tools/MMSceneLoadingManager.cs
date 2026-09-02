using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MoreMountains.Tools;

public class MMSceneLoadingManager : MonoBehaviour
{
	public enum LoadingStatus
	{
		LoadStarted = 0,
		BeforeEntryFade = 1,
		EntryFade = 2,
		AfterEntryFade = 3,
		UnloadOriginScene = 4,
		LoadDestinationScene = 5,
		LoadProgressComplete = 6,
		InterpolatedLoadProgressComplete = 7,
		BeforeExitFade = 8,
		ExitFade = 9,
		DestinationSceneActivation = 10,
		UnloadSceneLoader = 11,
		LoadTransitionComplete = 12
	}

	public struct LoadingSceneEvent(string sceneName, LoadingStatus status)
	{
		public LoadingStatus Status = status;

		public string SceneName = sceneName;

		private static LoadingSceneEvent e;

		public static void Trigger(string sceneName, LoadingStatus status)
		{
			e.Status = status;
			e.SceneName = sceneName;
			MMEventManager.TriggerEvent(e);
		}
	}

	[Header("Binding")]
	public static string LoadingScreenSceneName = "LoadingScreen";

	[Header("GameObjects")]
	public Text LoadingText;

	public CanvasGroup LoadingProgressBar;

	public CanvasGroup LoadingAnimation;

	public CanvasGroup LoadingCompleteAnimation;

	[Header("Time")]
	public float StartFadeDuration = 0.2f;

	public float ProgressBarSpeed = 2f;

	public float ExitFadeDuration = 0.2f;

	public float LoadCompleteDelay = 0.5f;

	protected AsyncOperation _asyncOperation;

	protected static string _sceneToLoad = "";

	protected float _fadeDuration = 0.5f;

	protected float _fillTarget;

	protected string _loadingTextValue;

	protected Image _progressBarImage;

	protected static MMTweenType _tween;

	public static void LoadScene(string sceneToLoad)
	{
		_sceneToLoad = sceneToLoad;
		Application.backgroundLoadingPriority = ThreadPriority.High;
		if (LoadingScreenSceneName != null)
		{
			LoadingSceneEvent.Trigger(sceneToLoad, LoadingStatus.LoadStarted);
			SceneManager.LoadScene(LoadingScreenSceneName);
		}
	}

	public static void LoadScene(string sceneToLoad, string loadingSceneName)
	{
		_sceneToLoad = sceneToLoad;
		Application.backgroundLoadingPriority = ThreadPriority.High;
		SceneManager.LoadScene(loadingSceneName);
	}

	protected virtual void Start()
	{
		_tween = new MMTweenType(MMTween.MMTweenCurve.EaseOutCubic);
		_progressBarImage = LoadingProgressBar.GetComponent<Image>();
		_loadingTextValue = LoadingText.text;
		if (!string.IsNullOrEmpty(_sceneToLoad))
		{
			StartCoroutine(LoadAsynchronously());
		}
	}

	protected virtual void Update()
	{
		Time.timeScale = 1f;
		_progressBarImage.fillAmount = MMMaths.Approach(_progressBarImage.fillAmount, _fillTarget, Time.deltaTime * ProgressBarSpeed);
	}

	protected virtual IEnumerator LoadAsynchronously()
	{
		LoadingSetup();
		MMFadeOutEvent.Trigger(StartFadeDuration, _tween);
		yield return new WaitForSeconds(StartFadeDuration);
		_asyncOperation = SceneManager.LoadSceneAsync(_sceneToLoad, LoadSceneMode.Single);
		_asyncOperation.allowSceneActivation = false;
		while (_asyncOperation.progress < 0.9f)
		{
			_fillTarget = _asyncOperation.progress;
			yield return null;
		}
		_fillTarget = 1f;
		while (_progressBarImage.fillAmount != _fillTarget)
		{
			yield return null;
		}
		LoadingComplete();
		yield return new WaitForSeconds(LoadCompleteDelay);
		MMFadeInEvent.Trigger(ExitFadeDuration, _tween);
		yield return new WaitForSeconds(ExitFadeDuration);
		_asyncOperation.allowSceneActivation = true;
		LoadingSceneEvent.Trigger(_sceneToLoad, LoadingStatus.LoadTransitionComplete);
	}

	protected virtual void LoadingSetup()
	{
		LoadingCompleteAnimation.alpha = 0f;
		_progressBarImage.fillAmount = 0f;
		LoadingText.text = _loadingTextValue;
	}

	protected virtual void LoadingComplete()
	{
		LoadingSceneEvent.Trigger(_sceneToLoad, LoadingStatus.InterpolatedLoadProgressComplete);
		LoadingCompleteAnimation.gameObject.SetActive(value: true);
		StartCoroutine(MMFade.FadeCanvasGroup(LoadingProgressBar, 0.1f, 0f));
		StartCoroutine(MMFade.FadeCanvasGroup(LoadingAnimation, 0.1f, 0f));
		StartCoroutine(MMFade.FadeCanvasGroup(LoadingCompleteAnimation, 0.1f, 1f));
	}
}

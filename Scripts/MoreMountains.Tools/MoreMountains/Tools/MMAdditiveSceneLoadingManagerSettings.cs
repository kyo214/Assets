using System;
using UnityEngine;

namespace MoreMountains.Tools;

[Serializable]
public class MMAdditiveSceneLoadingManagerSettings
{
	public enum UnloadMethods
	{
		None = 0,
		ActiveScene = 1,
		AllScenes = 2
	}

	[Tooltip("the name of the MMSceneLoadingManager scene you want to use when in additive mode")]
	public string LoadingSceneName = "MMAdditiveLoadingScreen";

	[Tooltip("when in additive loading mode, the thread priority to apply to the loading")]
	public ThreadPriority ThreadPriority = ThreadPriority.High;

	[Tooltip("whether or not to make additional sanity checks (better leave this to true)")]
	public bool SecureLoad = true;

	[Tooltip("when in additive loading mode, whether or not to interpolate the progress bar's progress")]
	public bool InterpolateProgress = true;

	[Tooltip("when in additive loading mode, when in additive loading mode, the duration (in seconds) of the delay before the entry fade")]
	public float BeforeEntryFadeDelay;

	[Tooltip("when in additive loading mode, the duration (in seconds) of the entry fade")]
	public float EntryFadeDuration = 0.25f;

	[Tooltip("when in additive loading mode, the duration (in seconds) of the delay before the entry fade")]
	public float AfterEntryFadeDelay = 0.1f;

	[Tooltip("when in additive loading mode, the duration (in seconds) of the delay before the exit fade")]
	public float BeforeExitFadeDelay = 0.25f;

	[Tooltip("when in additive loading mode, the duration (in seconds) of the exit fade")]
	public float ExitFadeDuration = 0.2f;

	[Tooltip("when in additive loading mode, when in additive loading mode, the tween to use to fade on entry")]
	public MMTweenType EntryFadeTween;

	[Tooltip("when in additive loading mode, the tween to use to fade on exit")]
	public MMTweenType ExitFadeTween;

	[Tooltip("when in additive loading mode, the speed at which the loader's progress bar should move")]
	public float ProgressBarSpeed = 5f;

	[Tooltip("when in additive loading mode, the selective additive fade mode")]
	public MMAdditiveSceneLoadingManager.FadeModes FadeMode;

	[Tooltip("the chosen way to unload scenes (none, only the active scene, all loaded scenes)")]
	public UnloadMethods UnloadMethod = UnloadMethods.AllScenes;
}

using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback lets you unload a scene by name or build index")]
[FeedbackPath("Scene/Unload Scene")]
public class MMF_UnloadScene : MMF_Feedback
{
	public enum ColorModes
	{
		Instant = 0,
		Gradient = 1,
		Interpolate = 2
	}

	public enum Methods
	{
		BuildIndex = 0,
		SceneName = 1
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("Unload Scene")]
	[Tooltip("whether to unload a scene by build index or by name")]
	public Methods Method = Methods.SceneName;

	[Tooltip("the build ID of the scene to unload, find it in your Build Settings")]
	[MMFEnumCondition("Method", new int[] { 0 })]
	public int BuildIndex;

	[Tooltip("the name of the scene to unload")]
	[MMFEnumCondition("Method", new int[] { 1 })]
	public string SceneName = "";

	[Tooltip("whether or not to output warnings if the scene doesn't exist or can't be loaded")]
	public bool OutputWarningsIfNeeded = true;

	protected Scene _sceneToUnload;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			if (Method == Methods.BuildIndex)
			{
				_sceneToUnload = SceneManager.GetSceneByBuildIndex(BuildIndex);
			}
			else
			{
				_sceneToUnload = SceneManager.GetSceneByName(SceneName);
			}
			_ = _sceneToUnload;
			if (_sceneToUnload.isLoaded)
			{
				SceneManager.UnloadSceneAsync(_sceneToUnload);
			}
			else if (OutputWarningsIfNeeded)
			{
				Debug.LogWarning("Unload Scene Feedback : you're trying to unload a scene that hasn't been loaded.");
			}
		}
	}
}

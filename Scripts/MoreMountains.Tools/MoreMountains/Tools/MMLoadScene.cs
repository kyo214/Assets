using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreMountains.Tools;

public class MMLoadScene : MonoBehaviour
{
	public enum LoadingSceneModes
	{
		UnityNative = 0,
		MMSceneLoadingManager = 1,
		MMAdditiveSceneLoadingManager = 2
	}

	[Tooltip("the name of the scene that needs to be loaded when LoadScene gets called")]
	public string SceneName;

	[Tooltip("defines whether the scene will be loaded using Unity's native API or MoreMountains' way")]
	public LoadingSceneModes LoadingSceneMode;

	public virtual void LoadScene()
	{
		switch (LoadingSceneMode)
		{
		case LoadingSceneModes.UnityNative:
			SceneManager.LoadScene(SceneName);
			break;
		case LoadingSceneModes.MMSceneLoadingManager:
			MMSceneLoadingManager.LoadScene(SceneName);
			break;
		case LoadingSceneModes.MMAdditiveSceneLoadingManager:
			MMAdditiveSceneLoadingManager.LoadScene(SceneName);
			break;
		}
	}
}

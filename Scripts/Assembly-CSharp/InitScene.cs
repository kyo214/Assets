using Toked;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitScene : MonoBehaviour
{
	public bool isBackToMainMenu = true;

	public bool isBackToSplashScreen = true;

	public static InitScene Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
		if (isBackToMainMenu && SceneManager.GetActiveScene().name != "MainMenu" && GlobalUIManager.Instance == null)
		{
			if (GameModes.Instance.isDemo)
			{
				GenericSingleton<LoadSceneManager>.Instance.LoadSceneAsync("MainMenuFriendPass");
			}
			else
			{
				GenericSingleton<LoadSceneManager>.Instance.LoadSceneAsync("MainMenu");
			}
		}
	}
}

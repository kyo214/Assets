using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace MoreMountains.Tools;

[AddComponentMenu("More Mountains/Tools/Utilities/MMSceneRestarter")]
public class MMSceneRestarter : MonoBehaviour
{
	public enum RestartModes
	{
		ActiveScene = 0,
		SpecificScene = 1
	}

	[Header("Settings")]
	public RestartModes RestartMode;

	[MMEnumCondition("RestartMode", new int[] { 1 })]
	public string SceneName;

	public LoadSceneMode LoadMode;

	[Header("Input")]
	public Key RestarterKey = Key.Backspace;

	protected string _newSceneName;

	protected virtual void Update()
	{
		HandleInput();
	}

	protected virtual void HandleInput()
	{
		if (Keyboard.current[RestarterKey].wasPressedThisFrame)
		{
			RestartScene();
		}
	}

	public virtual void RestartScene()
	{
		Debug.Log("Scene restarted by MMSceneRestarter");
		switch (RestartMode)
		{
		case RestartModes.ActiveScene:
			_newSceneName = SceneManager.GetActiveScene().name;
			break;
		case RestartModes.SpecificScene:
			_newSceneName = SceneName;
			break;
		}
		SceneManager.LoadScene(_newSceneName, LoadMode);
	}
}

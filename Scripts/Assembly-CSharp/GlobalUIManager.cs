using System.Collections;
using Toked;
using UnityEngine;

public class GlobalUIManager : MonoBehaviour
{
	[SerializeField]
	private Canvas _saveIconCanvas;

	private Coroutine _saveIconCoroutine;

	public static GlobalUIManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	public void ClickGoToScene(string nameScene, bool PlaySFX = true)
	{
		Debug.Log("!DBG LOAD SCENE = " + nameScene);
		if (nameScene == "MainMenu" && GameModes.Instance.isInitDemo)
		{
			nameScene = "MainMenuFriendPass";
		}
		GenericSingleton<LoadSceneManager>.Instance.LoadSceneAsync(nameScene);
	}

	public void ShowSaveIcon()
	{
		if (_saveIconCoroutine != null)
		{
			StopCoroutine(_saveIconCoroutine);
			_saveIconCoroutine = null;
		}
		_saveIconCoroutine = StartCoroutine(DoShowSaveIcon());
	}

	private IEnumerator DoShowSaveIcon()
	{
		_saveIconCanvas.gameObject.SetActive(value: true);
		yield return new WaitForSecondsRealtime(3f);
		_saveIconCanvas.gameObject.SetActive(value: false);
	}
}

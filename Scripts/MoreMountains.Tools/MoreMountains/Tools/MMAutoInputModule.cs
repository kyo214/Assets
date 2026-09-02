using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace MoreMountains.Tools;

public class MMAutoInputModule : MonoBehaviour
{
	protected InputSystemUIInputModule _module;

	protected GameObject _eventSystemGameObject;

	protected virtual void Awake()
	{
		StartCoroutine(InitializeInputModule());
	}

	protected virtual IEnumerator InitializeInputModule()
	{
		EventSystem eventSystem = Object.FindObjectOfType<EventSystem>();
		if (!(eventSystem == null))
		{
			_eventSystemGameObject = eventSystem.gameObject;
			_module = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
			yield return null;
			_module.enabled = false;
			yield return null;
			_module.enabled = true;
			yield return null;
		}
	}
}

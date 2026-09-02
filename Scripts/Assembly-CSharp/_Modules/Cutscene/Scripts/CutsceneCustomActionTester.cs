using UnityEngine;

namespace _Modules.Cutscene.Scripts;

[RequireComponent(typeof(CustomActionTimelineContainer))]
public class CutsceneCustomActionTester : MonoBehaviour
{
	[SerializeField]
	private KeyCode _keyCodeAction1 = KeyCode.Comma;

	[SerializeField]
	private KeyCode _keyCodeAction2 = KeyCode.Period;

	[SerializeField]
	private CustomActionTimelineContainer _customActionTimelineContainer;

	private void Update()
	{
		if (Input.GetKeyUp(_keyCodeAction1))
		{
			_customActionTimelineContainer?.OnStart();
		}
		else if (Input.GetKeyUp(_keyCodeAction2))
		{
			_customActionTimelineContainer?.OnComplete();
		}
	}
}

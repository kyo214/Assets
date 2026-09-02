using System.Collections.Generic;
using UnityEngine;

namespace _Modules.Cutscene.Scripts;

public class CustomActionTimelineContainer : MonoBehaviour
{
	[SerializeField]
	private List<CustomCutsceneAction> _onStartCustomCutsceneActions;

	[SerializeField]
	private List<CustomCutsceneAction> _onCompleteCustomCutsceneActions;

	public void OnStart()
	{
		Execute(_onStartCustomCutsceneActions);
	}

	public void OnComplete()
	{
		Execute(_onCompleteCustomCutsceneActions);
	}

	private void Execute(List<CustomCutsceneAction> actions)
	{
		foreach (CustomCutsceneAction action in actions)
		{
			action.Invoke();
		}
	}
}

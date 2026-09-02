using Toked;
using UnityEngine;

namespace _Modules.Cutscene.Scripts;

[CreateAssetMenu(fileName = "SetActiveInputCutsceneAction", menuName = "WMO/Cutscene/Cutscene Action/SetActiveInputCutsceneAction", order = 0)]
public class SetActiveInputCutsceneAction : CustomCutsceneAction
{
	[SerializeField]
	private bool _enableInput;

	public override void Invoke()
	{
		if (_enableInput)
		{
			InputManager.EnableInput();
		}
		else
		{
			InputManager.DisableInput();
		}
	}
}

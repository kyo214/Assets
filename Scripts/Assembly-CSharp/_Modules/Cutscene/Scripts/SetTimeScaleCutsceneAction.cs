using UnityEngine;

namespace _Modules.Cutscene.Scripts;

[CreateAssetMenu(fileName = "CustomCutsceneAction", menuName = "WMO/Cutscene/Cutscene Action/PauseGameCutsceneAction", order = 0)]
public class SetTimeScaleCutsceneAction : CustomCutsceneAction
{
	[SerializeField]
	private float timeScale;

	public override void Invoke()
	{
		Time.timeScale = timeScale;
	}
}

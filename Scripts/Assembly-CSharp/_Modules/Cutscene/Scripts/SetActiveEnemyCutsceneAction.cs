using UnityEngine;

namespace _Modules.Cutscene.Scripts;

[CreateAssetMenu(fileName = "SetActiveEnemyCutsceneAction", menuName = "WMO/Cutscene/Cutscene Action/SetActiveEnemyCutsceneAction", order = 0)]
public class SetActiveEnemyCutsceneAction : CustomCutsceneAction
{
	[SerializeField]
	private bool _enableEnemy;

	public override void Invoke()
	{
		GameManager.Instance?.SetActiveEnemy(_enableEnemy);
	}
}

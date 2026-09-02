using UnityEngine;
using _Modules.Cutscene.Scripts;

namespace Toked.Item;

public class SetCutsceneTriggerCustomFunction : ItemInteractableCustomFunction
{
	[SerializeField]
	private string _cutsceneTriggerId;

	[SerializeField]
	private bool _setCutscene = true;

	public override void Execute(PlayerController playerController = null)
	{
		if (string.IsNullOrWhiteSpace(_cutsceneTriggerId))
		{
			return;
		}
		CutsceneTrigger cutsceneTrigger = CutsceneManager.GetCutsceneTrigger(_cutsceneTriggerId);
		if ((bool)cutsceneTrigger)
		{
			CutsceneColliderTrigger cutsceneColliderTrigger = cutsceneTrigger as CutsceneColliderTrigger;
			if ((bool)cutsceneColliderTrigger)
			{
				cutsceneColliderTrigger.SetActiveCollider(_setCutscene);
			}
			else
			{
				cutsceneTrigger.gameObject.SetActive(_setCutscene);
			}
		}
	}
}

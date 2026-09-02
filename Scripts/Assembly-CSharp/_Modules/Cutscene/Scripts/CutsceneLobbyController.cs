using UnityEngine;

namespace _Modules.Cutscene.Scripts;

public class CutsceneLobbyController : MonoBehaviour
{
	[SerializeField]
	private CutsceneTrigger _cutsceneTrigger;

	public void PlayCutscene()
	{
		CutsceneTrigger cutsceneTrigger = _cutsceneTrigger;
		SO_MissionMap currentMission = GameManagerPhoton.Instance.CurrentMission;
		if (currentMission.customStartCutsceneLobby && (bool)currentMission.cutsceneScriptableObject)
		{
			cutsceneTrigger = CutsceneManager.GetCutsceneTrigger(currentMission.cutsceneScriptableObject.PlayableDirectorId) ?? _cutsceneTrigger;
		}
		cutsceneTrigger.PlayCutscene();
	}
}

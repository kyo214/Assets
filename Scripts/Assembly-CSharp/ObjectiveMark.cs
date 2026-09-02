using System.Collections;
using UnityEngine;

public class ObjectiveMark : MonoBehaviour
{
	private IEnumerator Start()
	{
		while (GameManagerPhoton.Instance == null || GameManagerPhoton.Instance.CurrentMission == null)
		{
			yield return null;
		}
		if (LobbyManager.Instance == null && (GameManagerPhoton.Instance.CurrentMission.MissionObjective.IsCarRepairingOnStart || GameManagerPhoton.Instance.CurrentMission.MissionObjective.MinTargetDestroy > 0))
		{
			base.gameObject.SetActive(value: false);
		}
	}
}

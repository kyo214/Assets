using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateItemMissionObjective : MonoBehaviour
{
	[SerializeField]
	private List<SO_MissionObjective> ListMissionObjectiveToDeactivateObj = new List<SO_MissionObjective>();

	[SerializeField]
	private bool _isCheckingBycode = true;

	private IEnumerator Start()
	{
		while (GameManagerPhoton.Instance == null || GameManagerPhoton.Instance.CurrentMission == null)
		{
			yield return null;
		}
		if (!(LobbyManager.Instance == null))
		{
			yield break;
		}
		foreach (SO_MissionObjective item in ListMissionObjectiveToDeactivateObj)
		{
			if (_isCheckingBycode)
			{
				if (GameManagerPhoton.Instance.CurrentMission.MissionObjective.Code == item.Code)
				{
					base.gameObject.SetActive(value: false);
					break;
				}
			}
			else if (GameManagerPhoton.Instance.CurrentMission.MissionObjective.ID == item.ID)
			{
				base.gameObject.SetActive(value: false);
				break;
			}
		}
	}
}

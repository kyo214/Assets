using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class EggSpotlight : MonoBehaviour
{
	public List<DestructibleObject> ListDestructible = new List<DestructibleObject>();

	public Light lightSpotlight;

	public SO_MissionObjective missionObjective;

	[SerializeField]
	private List<GameObject> ObjectActivateEggMission = new List<GameObject>();

	[SerializeField]
	private List<GameObject> ObjectDeactivateAfterEggDestroyed = new List<GameObject>();

	[SerializeField]
	private ItemInteractable _itemInteractable;

	[SerializeField]
	private bool _eggDestroyed;

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(4f);
		if (missionObjective != null && (bool)GameManagerPhoton.Instance && (bool)GameManagerPhoton.Instance.CurrentMission && missionObjective.Code != GameManagerPhoton.Instance.CurrentMission.MissionObjective.Code)
		{
			foreach (GameObject item in ObjectActivateEggMission)
			{
				item?.SetActive(value: false);
			}
			lightSpotlight.gameObject.SetActive(value: false);
			lightSpotlight.enabled = false;
			_itemInteractable.IsSolved = true;
			_eggDestroyed = true;
		}
		ListDestructible = GetComponentsInChildren<DestructibleObject>().ToList();
	}

	public void OnObjectDestroyed(DestructibleObject destructObj)
	{
		ListDestructible.Remove(destructObj);
		if (ListDestructible.Count != 0)
		{
			return;
		}
		_eggDestroyed = true;
		foreach (GameObject item in ObjectDeactivateAfterEggDestroyed)
		{
			item?.SetActive(value: false);
		}
		lightSpotlight.DOIntensity(0f, 1f).OnComplete(() =>
		{
			lightSpotlight.enabled = false;
			lightSpotlight.gameObject.SetActive(value: false);
		});
		_itemInteractable.IsSolved = true;
		foreach (PlayerController item2 in NetworkGameManager.Instance.arrPlayerController)
		{
			RoomCollider roomCollider = GameManager.Instance.GetRoomCollider(item2.RoomName);
			if ((bool)roomCollider)
			{
				roomCollider.CheckMap(item2);
			}
		}
	}
}

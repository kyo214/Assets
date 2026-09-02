using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Modules.Map.Scripts;

public class PlayerSpawnController : MonoBehaviour
{
	[SerializeField]
	private List<PlayerSpawn> _arrPlayerSpawnList = new List<PlayerSpawn>();

	[SerializeField]
	private List<PlayerSpawn> _arrMapDefenseSpawnList = new List<PlayerSpawn>();

	private bool initialized;

	public void InitSpawnObject(int index)
	{
		if (!initialized)
		{
			if ((bool)GameManagerPhoton.Instance && (bool)GameManagerPhoton.Instance.CurrentMission && (bool)GameManagerPhoton.Instance.CurrentMission.MissionObjective && GameManagerPhoton.Instance.CurrentMission.MissionObjective.IsSpawnEndlessHordeFromBeginning && _arrMapDefenseSpawnList.Count > 0)
			{
				for (int i = 0; i < _arrPlayerSpawnList.Count; i++)
				{
					_arrPlayerSpawnList[i].SetActiveSpawnObject(setActive: false);
				}
				_arrPlayerSpawnList = _arrMapDefenseSpawnList.ToList();
				initialized = true;
			}
			else
			{
				for (int j = 0; j < _arrMapDefenseSpawnList.Count; j++)
				{
					_arrMapDefenseSpawnList[j].SetActiveSpawnObject(setActive: false);
				}
			}
			initialized = true;
		}
		for (int k = 0; k < _arrPlayerSpawnList.Count; k++)
		{
			if (k != index)
			{
				_arrPlayerSpawnList[k].SetActiveSpawnObject(setActive: false);
			}
		}
	}

	public PlayerSpawn GetPlayerSpawn(int index)
	{
		if (index >= _arrPlayerSpawnList.Count)
		{
			return null;
		}
		return _arrPlayerSpawnList[index];
	}

	public int GetTotalSpawn()
	{
		return _arrPlayerSpawnList.Count;
	}

	public Vector3 GetPlayerSpawn(int index, int playerIndex)
	{
		if (index < _arrPlayerSpawnList.Count)
		{
			return _arrPlayerSpawnList[index].GetPlayerSpawn(playerIndex);
		}
		return NetworkGameManager.Instance.GetPlayer(playerIndex).transform.position;
	}
}

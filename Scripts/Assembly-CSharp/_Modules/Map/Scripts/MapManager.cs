using System.Collections;
using UnityEngine;

namespace _Modules.Map.Scripts;

public class MapManager : MonoBehaviour
{
	[SerializeField]
	private PlayerSpawnController _playerSpawnController;

	private int _currentSpawnPosIdx;

	public IEnumerator InitMap()
	{
		while (GameManagerPhoton.Instance == null)
		{
			yield return null;
		}
		bool flag = false;
		if ((bool)LobbyManager.Instance)
		{
			_currentSpawnPosIdx = 1;
			flag = true;
		}
		else if ((bool)GameManagerPhoton.Instance && (bool)GameManagerPhoton.Instance.CurrentMission)
		{
			_currentSpawnPosIdx = GameManagerPhoton.Instance.CurrentMission.PlayerSpawningIdx;
			flag = true;
		}
		if (NetworkGameManager.Instance.isServer)
		{
			GameManagerPhoton.Instance.SpawnIdx = _currentSpawnPosIdx;
		}
		else if (!flag)
		{
			_currentSpawnPosIdx = GameManagerPhoton.Instance.SpawnIdx;
		}
		_playerSpawnController.InitSpawnObject(_currentSpawnPosIdx);
	}

	public Vector3 GetSpawnPosition(int spawnIndex, int playerIndex)
	{
		Vector3 playerSpawn = _playerSpawnController.GetPlayerSpawn(spawnIndex, playerIndex);
		return new Vector3(playerSpawn.x, 0f, playerSpawn.z);
	}

	public Vector3 GetSpawnPosition(int playerIndex)
	{
		Vector3 spawnPosition = GetSpawnPosition(_currentSpawnPosIdx, playerIndex);
		return new Vector3(spawnPosition.x, 0f, spawnPosition.z);
	}

	public void CheckWinCondition()
	{
		PlayerSpawn playerSpawn = _playerSpawnController.GetPlayerSpawn(_currentSpawnPosIdx);
		if ((bool)playerSpawn)
		{
			playerSpawn.CheckWinCondition();
		}
		else
		{
			NetworkGameManager.Instance.StartCoroutine(NetworkGameManager.Instance.WinLevel());
		}
	}
}

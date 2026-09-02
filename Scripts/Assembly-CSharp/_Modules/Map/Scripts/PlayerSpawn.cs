using System.Collections.Generic;
using UnityEngine;

namespace _Modules.Map.Scripts;

public class PlayerSpawn : MonoBehaviour
{
	[SerializeField]
	private List<Transform> _arrPosPlayer = new List<Transform>();

	[SerializeField]
	private GameObject _spawnLocationObject;

	[SerializeField]
	private WinArea _winArea;

	public void SetActiveSpawnObject(bool setActive)
	{
		if (_spawnLocationObject != null)
		{
			_spawnLocationObject.SetActive(setActive);
		}
	}

	public void CheckWinCondition()
	{
		if ((bool)_winArea)
		{
			_winArea.OnCompleteLevel();
		}
		else
		{
			NetworkGameManager.Instance.StartCoroutine(NetworkGameManager.Instance.WinLevel());
		}
	}

	public Vector3 GetPlayerSpawn(int index)
	{
		return _arrPosPlayer[index].position;
	}
}

using UnityEngine;

namespace _Modules.Data.Scripts;

public class SetGameClearedSaveData : MonoBehaviour
{
	private void Start()
	{
		GlobalSaveData instance = GlobalSaveData.instance;
		if (instance != null)
		{
			instance.gameData?.SetGameCompleted();
			instance.SaveCurrentGameData();
		}
	}
}

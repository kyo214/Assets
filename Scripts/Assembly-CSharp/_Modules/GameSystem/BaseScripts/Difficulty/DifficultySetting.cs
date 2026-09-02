using UnityEngine;

namespace _Modules.GameSystem.BaseScripts.Difficulty;

public class DifficultySetting : MonoBehaviour
{
	public enum Difficulty
	{
		Easy = 0,
		Normal = 1,
		Hard = 2,
		VeryHard = 3
	}

	[SerializeField]
	private DifficultyScriptableObjectLibrary _difficultyScriptableObjectLibrary;

	[SerializeField]
	private DifficultyData _difficultyData = new DifficultyData();

	public DifficultyData GetDifficultyData()
	{
		return _difficultyData;
	}

	public void SetDifficulty(Difficulty difficulty)
	{
		DifficultyScriptableObject data = _difficultyScriptableObjectLibrary.GetData(difficulty);
		if (!(data == null))
		{
			_difficultyData.SetData(data.GetDifficultyData());
		}
	}

	public void SetDifficultyNetwork(Difficulty difficulty)
	{
		SetDifficulty(difficulty);
		if ((bool)NetworkGameManager.Instance && NetworkGameManager.Instance.isServer && (bool)GameManagerPhoton.Instance)
		{
			GameManagerPhoton.Instance.Difficulty = (int)_difficultyData.DifficultySetting;
			GameManagerPhoton.Instance.RpcSetDifficulty();
		}
	}
}

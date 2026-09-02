using UnityEngine;

[CreateAssetMenu(menuName = "WMO/Score Config")]
public class ScoreConfig : ScriptableObject
{
	[Header("Kill Zombie")]
	public int ScorePerKillZombie = 1;

	[Header("Kill Elite")]
	public int ScorePerKillElite = 15;

	[Header("Puzzle")]
	public int ScorePerPuzzle = 20;

	[Header("Death")]
	public int DeathPenalty = -10;

	[Header("Life")]
	public int Life = 300;

	[Header("ReviveOtherPlayer")]
	public int ReviveOtherPlayer = 20;

	[Header("Multiply Difficulty")]
	public float AdditionMultiplyDifficulty = 0.1f;

	[Header("Time Reward")]
	public float MaxTimeRewardBonus = 1000f;

	[Header("Time Reward")]
	public float MaxTimeAllMapBySecond = 5400f;
}

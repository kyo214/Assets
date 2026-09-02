using System;

[Serializable]
public class WavePerEnemy
{
	public int EnemyKey;

	public int Total;

	public WavePerEnemy(int newKey, int newTotal)
	{
		EnemyKey = newKey;
		Total = newTotal;
	}
}

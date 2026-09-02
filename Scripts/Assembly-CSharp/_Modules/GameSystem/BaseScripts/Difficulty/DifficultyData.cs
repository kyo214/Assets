using System;
using UnityEngine;

namespace _Modules.GameSystem.BaseScripts.Difficulty;

[Serializable]
public class DifficultyData
{
	[SerializeField]
	private DifficultySetting.Difficulty _difficultySetting = _Modules.GameSystem.BaseScripts.Difficulty.DifficultySetting.Difficulty.Normal;

	[SerializeField]
	private float _enemyHpMultiplier = 1f;

	[SerializeField]
	private float _enemyEliteHpMultiplier = 1f;

	[SerializeField]
	private float _enemyDamageMultiplier;

	[SerializeField]
	private int _hordeAdditionalTime;

	[HideInInspector]
	[SerializeField]
	private int _waveAdditionalTime;

	[SerializeField]
	private float _enemyHordeMultiplier;

	[SerializeField]
	private float _delayEnemySpawn;

	[SerializeField]
	private int _startIndexHorde;

	public DifficultySetting.Difficulty DifficultySetting
	{
		get
		{
			return _difficultySetting;
		}
		set
		{
			_difficultySetting = value;
		}
	}

	public float EnemyHpMultiplier
	{
		get
		{
			return _enemyHpMultiplier;
		}
		set
		{
			_enemyHpMultiplier = value;
		}
	}

	public float EnemyEliteHpMultiplier
	{
		get
		{
			return _enemyEliteHpMultiplier;
		}
		set
		{
			_enemyEliteHpMultiplier = value;
		}
	}

	public int WaveAdditionalTime
	{
		get
		{
			return _waveAdditionalTime;
		}
		set
		{
			_waveAdditionalTime = value;
		}
	}

	public int HordeAdditionalTime
	{
		get
		{
			return _hordeAdditionalTime;
		}
		set
		{
			_hordeAdditionalTime = value;
		}
	}

	public float EnemyHordeMultiplier
	{
		get
		{
			return _enemyHordeMultiplier;
		}
		set
		{
			_enemyHordeMultiplier = value;
		}
	}

	public float DelayEnemySpawn
	{
		get
		{
			return _delayEnemySpawn;
		}
		set
		{
			_delayEnemySpawn = value;
		}
	}

	public int StartIndexHorde
	{
		get
		{
			return _startIndexHorde;
		}
		set
		{
			_startIndexHorde = value;
		}
	}

	public float EnemyDamageMultiplier
	{
		get
		{
			return _enemyDamageMultiplier;
		}
		set
		{
			_enemyDamageMultiplier = value;
		}
	}

	public void SetData(DifficultyData difficultyData)
	{
		_difficultySetting = difficultyData.DifficultySetting;
		_enemyHpMultiplier = difficultyData.EnemyHpMultiplier;
		_waveAdditionalTime = difficultyData.WaveAdditionalTime;
		_enemyEliteHpMultiplier = difficultyData.EnemyEliteHpMultiplier;
		_hordeAdditionalTime = difficultyData.HordeAdditionalTime;
		_enemyHordeMultiplier = difficultyData.EnemyHordeMultiplier;
		_delayEnemySpawn = difficultyData.DelayEnemySpawn;
		_startIndexHorde = difficultyData.StartIndexHorde;
		_enemyDamageMultiplier = difficultyData.EnemyDamageMultiplier;
	}
}

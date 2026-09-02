using System;
using UnityEngine;

namespace _Modules.Achievement.Scripts;

[Serializable]
public class GameStatisticSaveData
{
	[SerializeField]
	public string _gameStatisticType;

	[SerializeField]
	public string _targetAdditionalVarKey;

	[SerializeField]
	public float _value;

	public string GameStatisticType
	{
		get
		{
			return _gameStatisticType;
		}
		set
		{
			_gameStatisticType = value;
		}
	}

	public string TargetAdditionalVarKey
	{
		get
		{
			return _targetAdditionalVarKey;
		}
		set
		{
			_targetAdditionalVarKey = value;
		}
	}

	public float Value
	{
		get
		{
			return _value;
		}
		set
		{
			_value = value;
		}
	}

	public GameStatisticSaveData(string gameStatisticType, string targetAdditionalVarKey, float value)
	{
		_gameStatisticType = gameStatisticType;
		_targetAdditionalVarKey = targetAdditionalVarKey;
		_value = value;
	}

	public GameStatisticSaveData(string key, float value)
	{
		string[] array = key.Split(GameStatisticData.SPARATOR, 2);
		_gameStatisticType = array[0];
		_targetAdditionalVarKey = ((array.Length == 2) ? array[1] : "");
		_value = value;
	}

	public GameStatisticSaveData()
	{
	}
}

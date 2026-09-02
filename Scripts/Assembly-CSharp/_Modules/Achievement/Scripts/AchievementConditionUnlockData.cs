using System;
using UnityEngine;

namespace _Modules.Achievement.Scripts;

[Serializable]
public class AchievementConditionUnlockData
{
	public enum ActivationType
	{
		NOT_EQUAL = 0,
		LESS = 1,
		LESS_EQUAL = 2,
		EQUAL = 3,
		MORE_EQUAL = 4,
		MORE = 5
	}

	private const string TAG_ADDITIONAL_KEY = "[ADDITIONAL_KEY]";

	private const string TAG_VALUE = "[VAL]";

	[SerializeField]
	public GameStatisticData gameStatisticData;

	[SerializeField]
	private bool _isOnOneRun;

	[SerializeField]
	private string _targetVarValue;

	[SerializeField]
	private ActivationType _activationType;

	public string TargetVarValue => _targetVarValue;

	public bool IsOnOneRun => _isOnOneRun;

	public string MissionKey => gameStatisticData.GameStatisticKey;

	public ActivationType MissionActivationType => _activationType;

	public AchievementConditionUnlockData()
		: this(GameStatisticType.COMPLETE_GAME, "", isOnOneLevel: true, "", ActivationType.EQUAL)
	{
	}

	public AchievementConditionUnlockData(GameStatisticType gameStatisticType, string targetAdditionalVarKey, bool isOnOneLevel, string targetVarValue, ActivationType activationType)
	{
		SetUnlockCondition(gameStatisticType, targetAdditionalVarKey, isOnOneLevel, targetVarValue, activationType);
	}

	public AchievementConditionUnlockData(AchievementConditionUnlockData missionData)
	{
		SetUnlockCondition(missionData.gameStatisticData.GameStatisticType, missionData.gameStatisticData.TargetAdditionalVarKey, missionData._isOnOneRun, missionData._targetVarValue, missionData._activationType);
	}

	public void SetUnlockCondition(GameStatisticType missionType, string targetVarAdditionalKey, bool isOnOneLevel, string targetVarValue, ActivationType activationType)
	{
		gameStatisticData = new GameStatisticData(missionType, targetVarAdditionalKey);
		_isOnOneRun = isOnOneLevel;
		_targetVarValue = targetVarValue;
		_activationType = activationType;
	}

	public bool CheckActivation(string checkValue)
	{
		if (float.TryParse(_targetVarValue, out var result) && float.TryParse(checkValue, out var result2))
		{
			switch (_activationType)
			{
			case ActivationType.NOT_EQUAL:
				if (!Mathf.Approximately(result2, result))
				{
					return true;
				}
				break;
			case ActivationType.LESS:
				if (result2 < result)
				{
					return true;
				}
				break;
			case ActivationType.LESS_EQUAL:
				if (result2 <= result)
				{
					return true;
				}
				break;
			case ActivationType.EQUAL:
				if (Mathf.Approximately(result2, result))
				{
					return true;
				}
				break;
			case ActivationType.MORE_EQUAL:
				if (result2 >= result)
				{
					return true;
				}
				break;
			case ActivationType.MORE:
				if (result2 > result)
				{
					return true;
				}
				break;
			}
		}
		else if (_activationType == ActivationType.NOT_EQUAL)
		{
			if (checkValue != _targetVarValue)
			{
				return true;
			}
		}
		else if (_activationType == ActivationType.EQUAL && checkValue == _targetVarValue)
		{
			return true;
		}
		return false;
	}

	private bool CheckActivationNumber(float val)
	{
		if (float.TryParse(_targetVarValue, out var result))
		{
			switch (_activationType)
			{
			case ActivationType.NOT_EQUAL:
				if (!Mathf.Approximately(val, result))
				{
					return true;
				}
				break;
			case ActivationType.LESS:
				if (val < result)
				{
					return true;
				}
				break;
			case ActivationType.LESS_EQUAL:
				if (val <= result)
				{
					return true;
				}
				break;
			case ActivationType.EQUAL:
				if (Mathf.Approximately(val, result))
				{
					return true;
				}
				break;
			case ActivationType.MORE_EQUAL:
				if (val >= result)
				{
					return true;
				}
				break;
			case ActivationType.MORE:
				if (val > result)
				{
					return true;
				}
				break;
			}
		}
		return false;
	}
}

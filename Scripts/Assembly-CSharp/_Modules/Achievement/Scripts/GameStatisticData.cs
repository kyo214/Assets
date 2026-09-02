using System;
using UnityEngine;

namespace _Modules.Achievement.Scripts;

[Serializable]
public class GameStatisticData
{
	public static string SPARATOR = "|";

	[SerializeField]
	private GameStatisticType gameStatisticType;

	[SerializeField]
	private string targetAdditionalVarKey;

	[SerializeField]
	[HideInInspector]
	private string gameStatisticKey;

	public string TargetAdditionalVarKey
	{
		get
		{
			return targetAdditionalVarKey;
		}
		set
		{
			targetAdditionalVarKey = value;
			UpdateGameStatisticKey();
		}
	}

	public GameStatisticType GameStatisticType
	{
		get
		{
			return gameStatisticType;
		}
		set
		{
			gameStatisticType = value;
			UpdateGameStatisticKey();
		}
	}

	public string GameStatisticKey => gameStatisticKey;

	public GameStatisticData()
		: this(GameStatisticType.COMPLETE_GAME, "")
	{
	}

	public GameStatisticData(GameStatisticType gameStatisticType, string targetAdditionalVarKey)
	{
		this.gameStatisticType = gameStatisticType;
		this.targetAdditionalVarKey = targetAdditionalVarKey;
		UpdateGameStatisticKey();
	}

	public void UpdateGameStatisticKey()
	{
		gameStatisticKey = GetGameStatisticKey();
	}

	public string GetGameStatisticKey()
	{
		return ConvertToKey(gameStatisticType, targetAdditionalVarKey);
	}

	public static string ConvertToKey(GameStatisticType type, string additionalKey = "")
	{
		return type.ToString() + (string.IsNullOrWhiteSpace(additionalKey) ? "" : (SPARATOR + additionalKey));
	}
}

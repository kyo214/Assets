using System.Collections.Generic;
using ES3Internal;
using UnityEngine.Scripting;
using _Modules.Data.Scripts;

namespace ES3Types;

[Preserve]
[ES3Properties(new string[]
{
	"FirstSaveDateTime", "GameVersion", "CurrentMission", "ArrMapCleared", "ArrMapLocked", "PlayerSaveData", "LastSaveDateTime", "LastRoomSessionType", "ScenarioId", "Difficulty",
	"Life", "ResetData", "Seed", "SessionName", "PlayerList", "ItemLobbyList", "MaxMission", "IsCompleted", "TotalMissionTime"
})]
public class ES3UserType_GameData : ES3ObjectType
{
	public static ES3Type Instance;

	public ES3UserType_GameData()
		: base(typeof(GameData))
	{
		Instance = this;
		priority = 1;
	}

	protected override void WriteObject(object obj, ES3Writer writer)
	{
		GameData gameData = (GameData)obj;
		writer.WriteProperty("FirstSaveDateTime", gameData.FirstSaveDateTime, ES3Type_long.Instance);
		writer.WriteProperty("GameVersion", gameData.GameVersion, ES3Type_string.Instance);
		writer.WriteProperty("CurrentMission", gameData.CurrentMission, ES3Type_int.Instance);
		writer.WriteProperty("ArrMapCleared", gameData.ArrMapCleared, ES3Type_boolArray.Instance);
		writer.WriteProperty("ArrMapLocked", gameData.ArrMapLocked, ES3Type_boolArray.Instance);
		writer.WriteProperty("PlayerSaveData", gameData.PlayerSaveData, ES3UserType_PlayerSaveData.Instance);
		writer.WriteProperty("LastSaveDateTime", gameData.LastSaveDateTime, ES3Type_long.Instance);
		writer.WriteProperty("LastRoomSessionType", gameData.LastRoomSessionType, ES3Type_int.Instance);
		writer.WriteProperty("ScenarioId", gameData.ScenarioId, ES3Type_string.Instance);
		writer.WriteProperty("Difficulty", gameData.Difficulty, ES3Type_int.Instance);
		writer.WriteProperty("Life", gameData.Life, ES3Type_int.Instance);
		writer.WriteProperty("ResetData", gameData.ResetData, ES3Type_bool.Instance);
		writer.WriteProperty("Seed", gameData.Seed, ES3Type_int.Instance);
		writer.WriteProperty("SessionName", gameData.SessionName, ES3Type_string.Instance);
		writer.WriteProperty("PlayerList", gameData.PlayerList, ES3TypeMgr.GetOrCreateES3Type(typeof(List<string>)));
		writer.WriteProperty("ItemLobbyList", gameData.ItemLobbyList, ES3TypeMgr.GetOrCreateES3Type(typeof(List<int>)));
		writer.WriteProperty("MaxMission", gameData.MaxMission, ES3Type_int.Instance);
		writer.WriteProperty("IsCompleted", gameData.IsCompleted, ES3Type_bool.Instance);
		writer.WriteProperty("TotalMissionTime", gameData.TotalMissionTime, ES3Type_float.Instance);
	}

	protected override void ReadObject<T>(ES3Reader reader, object obj)
	{
		GameData gameData = (GameData)obj;
		foreach (string property in reader.Properties)
		{
			switch (property)
			{
			case "FirstSaveDateTime":
				gameData.FirstSaveDateTime = reader.Read<long>(ES3Type_long.Instance);
				break;
			case "GameVersion":
				gameData.GameVersion = reader.Read<string>(ES3Type_string.Instance);
				break;
			case "CurrentMission":
				gameData.CurrentMission = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "ArrMapCleared":
				gameData.ArrMapCleared = reader.Read<bool[]>(ES3Type_boolArray.Instance);
				break;
			case "ArrMapLocked":
				gameData.ArrMapLocked = reader.Read<bool[]>(ES3Type_boolArray.Instance);
				break;
			case "PlayerSaveData":
				gameData.PlayerSaveData = reader.Read<PlayerSaveData>(ES3UserType_PlayerSaveData.Instance);
				break;
			case "LastSaveDateTime":
				gameData.LastSaveDateTime = reader.Read<long>(ES3Type_long.Instance);
				break;
			case "LastRoomSessionType":
				gameData.LastRoomSessionType = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "ScenarioId":
				gameData.ScenarioId = reader.Read<string>(ES3Type_string.Instance);
				break;
			case "Difficulty":
				gameData.Difficulty = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "Life":
				gameData.Life = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "ResetData":
				gameData.ResetData = reader.Read<bool>(ES3Type_bool.Instance);
				break;
			case "Seed":
				gameData.Seed = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "SessionName":
				gameData.SessionName = reader.Read<string>(ES3Type_string.Instance);
				break;
			case "PlayerList":
				gameData.PlayerList = reader.Read<List<string>>();
				break;
			case "ItemLobbyList":
				gameData.ItemLobbyList = reader.Read<List<int>>();
				break;
			case "MaxMission":
				gameData.MaxMission = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "IsCompleted":
				gameData.IsCompleted = reader.Read<bool>(ES3Type_bool.Instance);
				break;
			case "TotalMissionTime":
				gameData.TotalMissionTime = reader.Read<float>(ES3Type_float.Instance);
				break;
			default:
				reader.Skip();
				break;
			}
		}
	}

	protected override object ReadObject<T>(ES3Reader reader)
	{
		GameData gameData = new GameData();
		ReadObject<T>(reader, gameData);
		return gameData;
	}
}

using UnityEngine.Scripting;
using _Modules.Achievement.Scripts;

namespace ES3Types;

[Preserve]
[ES3Properties(new string[] { "GameStatisticType", "TargetAdditionalVarKey", "Value" })]
public class ES3UserType_GameStatisticSaveData : ES3ObjectType
{
	public static ES3Type Instance;

	public ES3UserType_GameStatisticSaveData()
		: base(typeof(GameStatisticSaveData))
	{
		Instance = this;
		priority = 1;
	}

	protected override void WriteObject(object obj, ES3Writer writer)
	{
		GameStatisticSaveData gameStatisticSaveData = (GameStatisticSaveData)obj;
		writer.WriteProperty("GameStatisticType", gameStatisticSaveData.GameStatisticType, ES3Type_string.Instance);
		writer.WriteProperty("TargetAdditionalVarKey", gameStatisticSaveData.TargetAdditionalVarKey, ES3Type_string.Instance);
		writer.WriteProperty("Value", gameStatisticSaveData.Value, ES3Type_float.Instance);
	}

	protected override void ReadObject<T>(ES3Reader reader, object obj)
	{
		GameStatisticSaveData gameStatisticSaveData = (GameStatisticSaveData)obj;
		foreach (string property in reader.Properties)
		{
			switch (property)
			{
			case "GameStatisticType":
				gameStatisticSaveData.GameStatisticType = reader.Read<string>(ES3Type_string.Instance);
				break;
			case "TargetAdditionalVarKey":
				gameStatisticSaveData.TargetAdditionalVarKey = reader.Read<string>(ES3Type_string.Instance);
				break;
			case "Value":
				gameStatisticSaveData.Value = reader.Read<float>(ES3Type_float.Instance);
				break;
			default:
				reader.Skip();
				break;
			}
		}
	}

	protected override object ReadObject<T>(ES3Reader reader)
	{
		GameStatisticSaveData gameStatisticSaveData = new GameStatisticSaveData();
		ReadObject<T>(reader, gameStatisticSaveData);
		return gameStatisticSaveData;
	}
}

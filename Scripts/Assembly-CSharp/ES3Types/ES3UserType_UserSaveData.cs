using System.Collections.Generic;
using ES3Internal;
using UnityEngine.Scripting;
using _Modules.Achievement.Scripts;
using _Modules.Data.Scripts;

namespace ES3Types;

[Preserve]
[ES3Properties(new string[] { "GameStatisticSaveDataDictionary", "AchievementSaveDataDictionary", "UserUniqueId", "UserName" })]
public class ES3UserType_UserSaveData : ES3ObjectType
{
	public static ES3Type Instance;

	public ES3UserType_UserSaveData()
		: base(typeof(UserSaveData))
	{
		Instance = this;
		priority = 1;
	}

	protected override void WriteObject(object obj, ES3Writer writer)
	{
		UserSaveData userSaveData = (UserSaveData)obj;
		writer.WriteProperty("GameStatisticSaveDataDictionary", userSaveData.GameStatisticSaveDataDictionary, ES3TypeMgr.GetOrCreateES3Type(typeof(Dictionary<string, GameStatisticSaveData>)));
		writer.WriteProperty("AchievementSaveDataDictionary", userSaveData.AchievementSaveDataDictionary, ES3TypeMgr.GetOrCreateES3Type(typeof(Dictionary<string, AchievementSaveData>)));
		writer.WriteProperty("UserUniqueId", userSaveData.UserUniqueId, ES3Type_string.Instance);
		writer.WriteProperty("UserName", userSaveData.UserName, ES3Type_string.Instance);
	}

	protected override void ReadObject<T>(ES3Reader reader, object obj)
	{
		UserSaveData userSaveData = (UserSaveData)obj;
		foreach (string property in reader.Properties)
		{
			switch (property)
			{
			case "GameStatisticSaveDataDictionary":
				userSaveData.GameStatisticSaveDataDictionary = reader.Read<Dictionary<string, GameStatisticSaveData>>();
				break;
			case "AchievementSaveDataDictionary":
				userSaveData.AchievementSaveDataDictionary = reader.Read<Dictionary<string, AchievementSaveData>>();
				break;
			case "UserUniqueId":
				userSaveData.UserUniqueId = reader.Read<string>(ES3Type_string.Instance);
				break;
			case "UserName":
				userSaveData.UserName = reader.Read<string>(ES3Type_string.Instance);
				break;
			default:
				reader.Skip();
				break;
			}
		}
	}

	protected override object ReadObject<T>(ES3Reader reader)
	{
		UserSaveData userSaveData = new UserSaveData();
		ReadObject<T>(reader, userSaveData);
		return userSaveData;
	}
}

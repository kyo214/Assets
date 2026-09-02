using UnityEngine.Scripting;
using _Modules.Achievement.Scripts;

namespace ES3Types;

[Preserve]
[ES3Properties(new string[] { "Id", "ProgressList", "Completed", "IsClaimed" })]
public class ES3UserType_AchievementSaveData : ES3ObjectType
{
	public static ES3Type Instance;

	public ES3UserType_AchievementSaveData()
		: base(typeof(AchievementSaveData))
	{
		Instance = this;
		priority = 1;
	}

	protected override void WriteObject(object obj, ES3Writer writer)
	{
		AchievementSaveData achievementSaveData = (AchievementSaveData)obj;
		writer.WriteProperty("Id", achievementSaveData.Id, ES3Type_string.Instance);
		writer.WriteProperty("ProgressList", achievementSaveData.ProgressList, ES3Type_floatArray.Instance);
		writer.WriteProperty("Completed", achievementSaveData.Completed, ES3Type_bool.Instance);
		writer.WriteProperty("IsClaimed", achievementSaveData.IsClaimed, ES3Type_bool.Instance);
	}

	protected override void ReadObject<T>(ES3Reader reader, object obj)
	{
		AchievementSaveData achievementSaveData = (AchievementSaveData)obj;
		foreach (string property in reader.Properties)
		{
			switch (property)
			{
			case "Id":
				achievementSaveData.Id = reader.Read<string>(ES3Type_string.Instance);
				break;
			case "ProgressList":
				achievementSaveData.ProgressList = reader.Read<float[]>(ES3Type_floatArray.Instance);
				break;
			case "Completed":
				achievementSaveData.Completed = reader.Read<bool>(ES3Type_bool.Instance);
				break;
			case "IsClaimed":
				achievementSaveData.IsClaimed = reader.Read<bool>(ES3Type_bool.Instance);
				break;
			default:
				reader.Skip();
				break;
			}
		}
	}

	protected override object ReadObject<T>(ES3Reader reader)
	{
		AchievementSaveData achievementSaveData = new AchievementSaveData();
		ReadObject<T>(reader, achievementSaveData);
		return achievementSaveData;
	}
}

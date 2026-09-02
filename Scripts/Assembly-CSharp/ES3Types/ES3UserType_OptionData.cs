using UnityEngine.Scripting;

namespace ES3Types;

[Preserve]
[ES3Properties(new string[]
{
	"gameVer", "lang", "shakeLevel", "volMaster", "volMusic", "volSFX", "volAmbient", "resHeight", "resWidth", "graphic",
	"autoMinimap", "fullscreen", "showFpsRtt", "chatLog", "username", "region", "alreadyShowDisclaimerAnalytic", "windowMode", "volVoice", "vsync",
	"limitFPS", "lastRoomCode", "lastSeed", "voiceChatMode", "timerCountdown", "sprintModeToggle", "hintShowed", "IsFirstTimeControlShowed", "SkipIntroControl", "SkipIntroDialogue",
	"EnableTutorial", "IsTutorialMoveCleared", "IsTutorialSprintCleared", "IsTutorialDashCleared", "IsTutorialMeleeCleared", "IsTutorialShootCleared"
})]
public class ES3UserType_OptionData : ES3ObjectType
{
	public static ES3Type Instance;

	public ES3UserType_OptionData()
		: base(typeof(GlobalSaveData.OptionData))
	{
		Instance = this;
		priority = 1;
	}

	protected override void WriteObject(object obj, ES3Writer writer)
	{
		GlobalSaveData.OptionData optionData = (GlobalSaveData.OptionData)obj;
		writer.WriteProperty("gameVer", optionData.gameVer, ES3Type_int.Instance);
		writer.WriteProperty("lang", optionData.lang, ES3Type_string.Instance);
		writer.WriteProperty("shakeLevel", optionData.shakeLevel, ES3Type_int.Instance);
		writer.WriteProperty("volMaster", optionData.volMaster, ES3Type_int.Instance);
		writer.WriteProperty("volMusic", optionData.volMusic, ES3Type_int.Instance);
		writer.WriteProperty("volSFX", optionData.volSFX, ES3Type_int.Instance);
		writer.WriteProperty("volAmbient", optionData.volAmbient, ES3Type_int.Instance);
		writer.WriteProperty("resHeight", optionData.resHeight, ES3Type_int.Instance);
		writer.WriteProperty("resWidth", optionData.resWidth, ES3Type_int.Instance);
		writer.WriteProperty("graphic", optionData.graphic, ES3Type_int.Instance);
		writer.WriteProperty("autoMinimap", optionData.autoMinimap, ES3Type_int.Instance);
		writer.WriteProperty("fullscreen", optionData.fullscreen, ES3Type_bool.Instance);
		writer.WriteProperty("showFpsRtt", optionData.showFpsRtt, ES3Type_bool.Instance);
		writer.WriteProperty("chatLog", optionData.chatLog, ES3Type_bool.Instance);
		writer.WriteProperty("region", optionData.region, ES3Type_string.Instance);
		writer.WriteProperty("lastRegion", optionData.lastRegion, ES3Type_string.Instance);
		writer.WriteProperty("alreadyShowDisclaimerAnalytic", optionData.alreadyShowDisclaimerAnalytic, ES3Type_bool.Instance);
		writer.WriteProperty("windowMode", optionData.windowMode, ES3Type_int.Instance);
		writer.WriteProperty("volVoice", optionData.volVoice, ES3Type_int.Instance);
		writer.WriteProperty("vsync", optionData.vsync, ES3Type_bool.Instance);
		writer.WriteProperty("limitFPS", optionData.limitFPS, ES3Type_int.Instance);
		writer.WriteProperty("lastRoomCode", optionData.lastRoomCode, ES3Type_string.Instance);
		writer.WriteProperty("lastSeed", optionData.lastSeed, ES3Type_int.Instance);
		writer.WriteProperty("voiceChatMode", optionData.voiceChatMode, ES3Type_int.Instance);
		writer.WriteProperty("timerCountdown", optionData.timerCountdown, ES3Type_int.Instance);
		writer.WriteProperty("sprintModeToggle", optionData.sprintModeToggle, ES3Type_bool.Instance);
		writer.WriteProperty("hintShowed", optionData.hintShowed, ES3Type_bool.Instance);
		writer.WriteProperty("IsFirstTimeControlShowed", optionData.IsFirstTimeControlShowed, ES3Type_bool.Instance);
		writer.WriteProperty("SkipIntroControl", optionData.SkipIntroControl, ES3Type_bool.Instance);
		writer.WriteProperty("SkipIntroDialogue", optionData.SkipIntroDialogue, ES3Type_bool.Instance);
		writer.WriteProperty("EnableTutorial", optionData.EnableTutorial, ES3Type_bool.Instance);
		writer.WriteProperty("IsTutorialMoveCleared", optionData.IsTutorialMoveCleared, ES3Type_bool.Instance);
		writer.WriteProperty("IsTutorialSprintCleared", optionData.IsTutorialSprintCleared, ES3Type_bool.Instance);
		writer.WriteProperty("IsTutorialDashCleared", optionData.IsTutorialDashCleared, ES3Type_bool.Instance);
		writer.WriteProperty("IsTutorialMeleeCleared", optionData.IsTutorialMeleeCleared, ES3Type_bool.Instance);
		writer.WriteProperty("IsTutorialShootCleared", optionData.IsTutorialShootCleared, ES3Type_bool.Instance);
	}

	protected override void ReadObject<T>(ES3Reader reader, object obj)
	{
		GlobalSaveData.OptionData optionData = (GlobalSaveData.OptionData)obj;
		foreach (string property in reader.Properties)
		{
			switch (property)
			{
			case "gameVer":
				optionData.gameVer = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "lang":
				optionData.lang = reader.Read<string>(ES3Type_string.Instance);
				break;
			case "shakeLevel":
				optionData.shakeLevel = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "volMaster":
				optionData.volMaster = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "volMusic":
				optionData.volMusic = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "volSFX":
				optionData.volSFX = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "volAmbient":
				optionData.volAmbient = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "resHeight":
				optionData.resHeight = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "resWidth":
				optionData.resWidth = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "graphic":
				optionData.graphic = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "autoMinimap":
				optionData.autoMinimap = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "fullscreen":
				optionData.fullscreen = reader.Read<bool>(ES3Type_bool.Instance);
				break;
			case "showFpsRtt":
				optionData.showFpsRtt = reader.Read<bool>(ES3Type_bool.Instance);
				break;
			case "chatLog":
				optionData.chatLog = reader.Read<bool>(ES3Type_bool.Instance);
				break;
			case "region":
				optionData.region = reader.Read<string>(ES3Type_string.Instance);
				break;
			case "lastRegion":
				optionData.lastRegion = reader.Read<string>(ES3Type_string.Instance);
				break;
			case "alreadyShowDisclaimerAnalytic":
				optionData.alreadyShowDisclaimerAnalytic = reader.Read<bool>(ES3Type_bool.Instance);
				break;
			case "windowMode":
				optionData.windowMode = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "volVoice":
				optionData.volVoice = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "vsync":
				optionData.vsync = reader.Read<bool>(ES3Type_bool.Instance);
				break;
			case "limitFPS":
				optionData.limitFPS = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "lastRoomCode":
				optionData.lastRoomCode = reader.Read<string>(ES3Type_string.Instance);
				break;
			case "lastSeed":
				optionData.lastSeed = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "voiceChatMode":
				optionData.voiceChatMode = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "timerCountdown":
				optionData.timerCountdown = reader.Read<int>(ES3Type_int.Instance);
				break;
			case "sprintModeToggle":
				optionData.sprintModeToggle = reader.Read<bool>(ES3Type_bool.Instance);
				break;
			case "hintShowed":
				optionData.hintShowed = reader.Read<bool>(ES3Type_bool.Instance);
				break;
			case "IsFirstTimeControlShowed":
				optionData.IsFirstTimeControlShowed = reader.Read<bool>(ES3Type_bool.Instance);
				break;
			case "SkipIntroControl":
				optionData.SkipIntroControl = reader.Read<bool>(ES3Type_bool.Instance);
				break;
			case "SkipIntroDialogue":
				optionData.SkipIntroDialogue = reader.Read<bool>(ES3Type_bool.Instance);
				break;
			case "EnableTutorial":
				optionData.EnableTutorial = reader.Read<bool>(ES3Type_bool.Instance);
				break;
			case "IsTutorialMoveCleared":
				optionData.IsTutorialMoveCleared = reader.Read<bool>(ES3Type_bool.Instance);
				break;
			case "IsTutorialSprintCleared":
				optionData.IsTutorialSprintCleared = reader.Read<bool>(ES3Type_bool.Instance);
				break;
			case "IsTutorialDashCleared":
				optionData.IsTutorialDashCleared = reader.Read<bool>(ES3Type_bool.Instance);
				break;
			case "IsTutorialMeleeCleared":
				optionData.IsTutorialMeleeCleared = reader.Read<bool>(ES3Type_bool.Instance);
				break;
			case "IsTutorialShootCleared":
				optionData.IsTutorialShootCleared = reader.Read<bool>(ES3Type_bool.Instance);
				break;
			default:
				reader.Skip();
				break;
			}
		}
	}

	protected override object ReadObject<T>(ES3Reader reader)
	{
		GlobalSaveData.OptionData optionData = new GlobalSaveData.OptionData();
		ReadObject<T>(reader, optionData);
		return optionData;
	}
}

using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreMountains.Tools;

public class MMDebugMenuCommands : MonoBehaviour
{
	[MMDebugLogCommand]
	public static void Now()
	{
		MMDebug.DebugLogTime("Time.time is " + Time.time);
	}

	[MMDebugLogCommand]
	public static void Clear()
	{
		MMDebug.DebugLogClear();
	}

	[MMDebugLogCommand]
	public static void Restart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
	}

	[MMDebugLogCommand]
	public static void Reload()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
	}

	[MMDebugLogCommand]
	public static void Sysinfo()
	{
		MMDebug.DebugLogTime(MMDebug.GetSystemInfo());
	}

	[MMDebugLogCommand]
	public static void Quit()
	{
		InternalQuit();
	}

	[MMDebugLogCommand]
	public static void Exit()
	{
		InternalQuit();
	}

	[MMDebugLogCommand]
	public static void Help()
	{
		string text = "LIST OF COMMANDS";
		foreach (MethodInfo item in MMDebug.Commands.OrderBy((MethodInfo m) => m.Name))
		{
			text = text + "\n- <color=#FFFFFF>" + item.Name + "</color>";
		}
		MMDebug.DebugLogTime(text, "#FFC400");
	}

	private static void InternalQuit()
	{
		Application.Quit();
	}

	[MMDebugLogCommandArgumentCount(1)]
	[MMDebugLogCommand]
	public static void Vsync(string[] args)
	{
		if (int.TryParse(args[1], out var result))
		{
			QualitySettings.vSyncCount = result;
			MMDebug.DebugLogTime("VSyncCount set to " + result, "#FFC400");
		}
	}

	[MMDebugLogCommandArgumentCount(1)]
	[MMDebugLogCommand]
	public static void Framerate(string[] args)
	{
		if (int.TryParse(args[1], out var result))
		{
			Application.targetFrameRate = result;
			MMDebug.DebugLogTime("Framerate set to " + result, "#FFC400");
		}
	}

	[MMDebugLogCommandArgumentCount(1)]
	[MMDebugLogCommand]
	public static void Timescale(string[] args)
	{
		if (float.TryParse(args[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
		{
			Time.timeScale = result;
			MMDebug.DebugLogTime("Timescale set to " + result, "#FFC400");
		}
	}

	[MMDebugLogCommandArgumentCount(2)]
	[MMDebugLogCommand]
	public static void Biggest(string[] args)
	{
		if (int.TryParse(args[1], out var result) && int.TryParse(args[2], out var result2))
		{
			MMDebug.DebugLogTime(((result >= result2) ? result : result2) + " is the biggest number", "#FFC400");
		}
	}
}

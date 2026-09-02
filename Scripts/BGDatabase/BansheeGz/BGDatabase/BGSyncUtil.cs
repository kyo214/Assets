using System;
using System.IO;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public static class BGSyncUtil
{
	public static void ReadFile(BGLogger logger, string path, Action<byte[]> action)
	{
		logger.AppendLine("Trying to read file at ($)..", path);
		byte[] array;
		using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			array = new byte[fileStream.Length];
			fileStream.Read(array, 0, array.Length);
		}
		if (!logger.AppendLine(array.Length == 0, "Content of file is empty"))
		{
			logger.AppendLine("File is read successfully. ($) bytes", array.Length);
			action(array);
		}
	}

	public static byte[] ReadFile(BGLogger logger, string path)
	{
		logger.AppendLine("Trying to read file at ($)..", path);
		byte[] array;
		using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			array = new byte[fileStream.Length];
			fileStream.Read(array, 0, array.Length);
		}
		if (logger.AppendLine(array.Length == 0, "Content of file is empty"))
		{
			return null;
		}
		logger.AppendLine("File is read successfully. $ bytes", array.Length);
		return array;
	}

	public static bool AppendWarning(BGLogger logger, bool print, bool condition, string message, params object[] parameters)
	{
		if (condition)
		{
			AppendWarning(logger, print, message, parameters);
		}
		return condition;
	}

	public static void AppendWarning(BGLogger logger, bool print, string message, params object[] parameters)
	{
		try
		{
			if (print)
			{
				Debug.Log("[BGDatabase] WARNING: " + BGUtil.Format(message, parameters));
			}
			logger?.AppendWarning(message, parameters);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}
}

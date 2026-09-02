using System;
using System.IO;
using UnityEngine;

namespace MoreMountains.Tools;

public static class MMSaveLoadManager
{
	public static IMMSaveLoadManagerMethod saveLoadMethod = new MMSaveLoadManagerMethodBinary();

	private const string _baseFolderName = "/MMData/";

	private const string _defaultFolderName = "MMSaveLoadManager";

	private static string DetermineSavePath(string folderName = "MMSaveLoadManager")
	{
		string text = ((Application.platform != RuntimePlatform.IPhonePlayer) ? (Application.persistentDataPath + "/MMData/") : (Application.persistentDataPath + "/MMData/"));
		return text + folderName + "/";
	}

	private static string DetermineSaveFileName(string fileName)
	{
		return fileName;
	}

	public static void Save(object saveObject, string fileName, string foldername = "MMSaveLoadManager")
	{
		string text = DetermineSavePath(foldername);
		string text2 = DetermineSaveFileName(fileName);
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		FileStream fileStream = File.Create(text + text2);
		saveLoadMethod.Save(saveObject, fileStream);
		fileStream.Close();
	}

	public static object Load(Type objectType, string fileName, string foldername = "MMSaveLoadManager")
	{
		string text = DetermineSavePath(foldername);
		string path = text + DetermineSaveFileName(fileName);
		if (!Directory.Exists(text) || !File.Exists(path))
		{
			return null;
		}
		FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
		object result = saveLoadMethod.Load(objectType, fileStream);
		fileStream.Close();
		return result;
	}

	public static void DeleteSave(string fileName, string folderName = "MMSaveLoadManager")
	{
		string text = DetermineSavePath(folderName);
		string text2 = DetermineSaveFileName(fileName);
		if (File.Exists(text + text2))
		{
			File.Delete(text + text2);
		}
	}

	public static void DeleteSaveFolder(string folderName = "MMSaveLoadManager")
	{
		string text = DetermineSavePath(folderName);
		if (Directory.Exists(text))
		{
			DeleteDirectory(text);
		}
	}

	public static void DeleteDirectory(string target_dir)
	{
		string[] files = Directory.GetFiles(target_dir);
		string[] directories = Directory.GetDirectories(target_dir);
		string[] array = files;
		foreach (string path in array)
		{
			File.SetAttributes(path, FileAttributes.Normal);
			File.Delete(path);
		}
		array = directories;
		for (int i = 0; i < array.Length; i++)
		{
			DeleteDirectory(array[i]);
		}
		Directory.Delete(target_dir, recursive: false);
	}
}

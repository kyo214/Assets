using System.Collections.Generic;
using System.IO;
using Doozy.Runtime.Common.Extensions;
using UnityEngine;

namespace Doozy.Runtime.Common.Utils;

public static class PathUtils
{
	public static void CreatePath(string path)
	{
		Directory.CreateDirectory(ToAbsolutePath(path));
	}

	public static string CleanPath(string path)
	{
		string text = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
		string oldValue = $"{Path.AltDirectorySeparatorChar}{Path.AltDirectorySeparatorChar}";
		char altDirectorySeparatorChar = Path.AltDirectorySeparatorChar;
		return text.Replace(oldValue, altDirectorySeparatorChar.ToString());
	}

	public static string ToAbsolutePath(string path)
	{
		return CleanPath(path.Contains(Application.dataPath) ? path : path.RemoveFirst("Assets".Length).AppendPrefixIfMissing(Application.dataPath));
	}

	public static bool IsAbsolutePath(string path)
	{
		return path.StartsWith(Application.dataPath);
	}

	public static string ToRelativePath(string path)
	{
		return CleanPath(IsAbsolutePath(path) ? ("Assets" + path.Substring(Application.dataPath.Length)) : path);
	}

	public static bool IsRelativePath(string path)
	{
		return !IsAbsolutePath(path);
	}

	public static string[] GetResourcesDirectories()
	{
		List<string> list = new List<string>();
		Stack<string> stack = new Stack<string>();
		stack.Push(Application.dataPath);
		while (stack.Count > 0)
		{
			string text = stack.Pop();
			try
			{
				string[] directories = Directory.GetDirectories(text);
				foreach (string text2 in directories)
				{
					if (Path.GetFileName(text2).Equals("Resources"))
					{
						list.Add(text2);
					}
					stack.Push(text2);
				}
			}
			catch
			{
				Debug.LogError("Directory " + text + " couldn't be read from");
			}
		}
		return list.ToArray();
	}

	public static bool PathIsDirectory(string path)
	{
		return (File.GetAttributes(ToAbsolutePath(path)) & FileAttributes.Directory) == FileAttributes.Directory;
	}

	public static string GetDirectoryName(string path)
	{
		return CleanPath(Path.GetDirectoryName(path));
	}

	public static string GetFileName(string path)
	{
		return CleanPath(Path.GetFileName(path));
	}

	public static string GetFileNameWithoutExtension(string path)
	{
		return CleanPath(Path.GetFileNameWithoutExtension(path));
	}

	public static string GetExtension(string path)
	{
		return CleanPath(Path.GetExtension(path));
	}

	public static bool HasExtension(string path)
	{
		return Path.HasExtension(CleanPath(path));
	}
}

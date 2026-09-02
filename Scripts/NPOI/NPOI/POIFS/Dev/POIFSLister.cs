using System;
using System.Collections;
using System.IO;
using NPOI.POIFS.FileSystem;

namespace NPOI.POIFS.Dev;

public class POIFSLister
{
	public static void ViewFile(string filename)
	{
		using Stream stream = new FileStream(filename, FileMode.Open);
		DisplayDirectory(new POIFSFileSystem(stream).Root, "");
	}

	public static void DisplayDirectory(DirectoryNode dir, string indent)
	{
		Console.WriteLine(indent + dir.Name + " -");
		string text = indent + "  ";
		IEnumerator entries = dir.Entries;
		while (entries.MoveNext())
		{
			object current = entries.Current;
			if (current is DirectoryNode)
			{
				DisplayDirectory((DirectoryNode)current, text);
				continue;
			}
			string text2 = ((DocumentNode)current).Name;
			if (text2[0] < '\n')
			{
				string text3 = "(0x0" + (int)text2[0] + ")" + text2.Substring(1);
				text2 = text2.Substring(1) + " <" + text3 + ">";
			}
			Console.WriteLine(text + text2);
		}
	}
}

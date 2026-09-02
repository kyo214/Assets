using System;
using System.IO;
using System.Threading;

namespace NPOI.Util;

public class TempFile
{
	private static string dir;

	public static FileInfo CreateTempFile(string prefix, string suffix)
	{
		if (dir == null)
		{
			dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "poifiles")).FullName;
		}
		string text = Path.Combine(dir, prefix + Guid.NewGuid().ToString() + suffix);
		while (File.Exists(text))
		{
			text = Path.Combine(dir, prefix + Guid.NewGuid().ToString() + suffix);
			Thread.Sleep(1);
		}
		new FileStream(text, FileMode.CreateNew, FileAccess.ReadWrite).Close();
		return new FileInfo(text);
	}

	public static string GetTempFilePath(string prefix, string suffix)
	{
		if (dir == null)
		{
			dir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "poifiles")).FullName;
		}
		Random random = new Random(DateTime.Now.Millisecond);
		Thread.Sleep(10);
		return Path.Combine(dir, prefix + random.Next() + suffix);
	}
}

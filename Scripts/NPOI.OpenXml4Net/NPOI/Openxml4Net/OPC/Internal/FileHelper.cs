using System.IO;

namespace NPOI.OpenXml4Net.OPC.Internal;

public class FileHelper
{
	public static string GetDirectory(string filepath)
	{
		return Path.GetDirectoryName(filepath).Replace("\\", "/");
	}

	public static void CopyFile(string inpath, string outpath)
	{
		File.Copy(inpath, outpath, overwrite: true);
	}

	public static void CopyFile(FileInfo inpath, FileInfo outpath)
	{
		File.Copy(inpath.FullName, outpath.FullName, overwrite: true);
	}

	public static string GetFilename(string filepath)
	{
		int length = filepath.Length;
		int num = length;
		while (--num >= 0)
		{
			if (filepath[num] == '\\')
			{
				return filepath.Substring(num + 1, length);
			}
		}
		return "";
	}
}

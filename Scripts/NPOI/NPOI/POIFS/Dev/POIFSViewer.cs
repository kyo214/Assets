using System;
using System.Collections;
using System.IO;
using System.Text;
using NPOI.POIFS.FileSystem;

namespace NPOI.POIFS.Dev;

public class POIFSViewer
{
	public static void ViewFile(string filename, bool printName)
	{
		if (printName)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(".");
			for (int i = 0; i < filename.Length; i++)
			{
				stringBuilder.Append("-");
			}
			stringBuilder.Append(".");
			Console.WriteLine(stringBuilder);
			Console.WriteLine("|" + filename + "|");
			Console.WriteLine(stringBuilder);
		}
		try
		{
			using Stream stream = File.OpenRead(filename);
			IEnumerator enumerator = POIFSViewEngine.InspectViewable(new NPOIFSFileSystem(stream), drilldown: true, 0, "  ").GetEnumerator();
			while (enumerator.MoveNext())
			{
				Console.Write(enumerator.Current);
			}
		}
		catch (IOException ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}

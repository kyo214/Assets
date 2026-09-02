using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NPOI.POIFS.Macros;

public class VBAMacroExtractor
{
	public static void main(string[] args)
	{
		if (args.Length == 0)
		{
			Console.WriteLine("Use:");
			Console.WriteLine("   VBAMacroExtractor <office.doc> [output]");
			Console.WriteLine("");
			Console.WriteLine("If an output directory is given, macros are written there");
			Console.WriteLine("Otherwise they are output to the screen");
		}
		FileInfo input = new FileInfo(args[0]);
		DirectoryInfo outputDir = null;
		if (args.Length > 1)
		{
			outputDir = new DirectoryInfo(args[1]);
		}
		new VBAMacroExtractor().Extract(input, outputDir);
	}

	public void Extract(FileInfo input, DirectoryInfo outputDir, string extension)
	{
		if (!input.Exists)
		{
			throw new FileNotFoundException(input.ToString());
		}
		if (outputDir != null)
		{
			if (!outputDir.Exists)
			{
				outputDir.Create();
			}
			Console.WriteLine(outputDir);
		}
		else
		{
			Console.WriteLine("STDOUT");
		}
		VBAMacroReader vBAMacroReader = new VBAMacroReader(input);
		Dictionary<string, string> dictionary = vBAMacroReader.ReadMacros();
		vBAMacroReader.Close();
		string value = "---------------------------------------";
		foreach (KeyValuePair<string, string> item in dictionary)
		{
			string key = item.Key;
			string value2 = item.Value;
			if (outputDir == null)
			{
				Console.WriteLine(value);
				Console.WriteLine(key);
				Console.WriteLine("");
				Console.WriteLine(value2);
			}
			else
			{
				FileInfo fileInfo = new FileInfo(Path.Combine(outputDir.FullName, key + extension));
				FileStream fileStream = fileInfo.Create();
				StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
				streamWriter.Write(value2);
				streamWriter.Close();
				fileStream.Close();
				Console.WriteLine("Extracted " + fileInfo);
			}
		}
	}

	public void Extract(FileInfo input, DirectoryInfo outputDir)
	{
		Extract(input, outputDir, ".vba");
	}
}

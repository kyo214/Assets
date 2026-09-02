using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BansheeGz.BGDatabase;

public class CsvFileWriter : CsvFileCommon, IDisposable
{
	private StreamWriter Writer;

	private string OneQuote;

	private string TwoQuotes;

	private string QuotedFormat;

	public CsvFileWriter(StreamWriter writer)
	{
		Writer = writer;
	}

	public CsvFileWriter(Stream stream)
	{
		Writer = new StreamWriter(stream);
	}

	public CsvFileWriter(string path)
	{
		Writer = new StreamWriter(path);
	}

	public static void WriteAll(List<List<string>> dataGrid, string path, Encoding encoding)
	{
		using StreamWriter writer = new StreamWriter(path, append: false, encoding);
		CsvFileWriter csvFileWriter = new CsvFileWriter(writer);
		foreach (List<string> item in dataGrid)
		{
			csvFileWriter.WriteRow(item);
		}
	}

	public void WriteAll(List<List<string>> dataGrid)
	{
		foreach (List<string> item in dataGrid)
		{
			WriteRow(item);
		}
	}

	public void WriteRow(List<string> columns)
	{
		if (columns == null)
		{
			throw new ArgumentNullException("columns");
		}
		if (OneQuote == null || OneQuote[0] != base.Quote)
		{
			OneQuote = $"{base.Quote}";
			TwoQuotes = string.Format("{0}{0}", base.Quote);
			QuotedFormat = string.Format("{0}{{0}}{0}", base.Quote);
		}
		for (int i = 0; i < columns.Count; i++)
		{
			if (i > 0)
			{
				Writer.Write(base.Delimiter);
			}
			if (columns[i].IndexOfAny(SpecialChars) == -1)
			{
				Writer.Write(columns[i]);
			}
			else
			{
				Writer.Write(QuotedFormat, columns[i].Replace(OneQuote, TwoQuotes));
			}
		}
		Writer.Write("\r\n");
	}

	public void Dispose()
	{
		Writer.Dispose();
	}
}

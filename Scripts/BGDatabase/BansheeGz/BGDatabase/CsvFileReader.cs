using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BansheeGz.BGDatabase;

public class CsvFileReader : CsvFileCommon, IDisposable
{
	private StreamReader Reader;

	private string CurrLine;

	private int CurrPos;

	private EmptyLineBehavior EmptyLineBehavior;

	public CsvFileReader(StreamReader reader, EmptyLineBehavior emptyLineBehavior = EmptyLineBehavior.NoColumns)
	{
		Reader = reader;
		EmptyLineBehavior = emptyLineBehavior;
	}

	public CsvFileReader(Stream stream, EmptyLineBehavior emptyLineBehavior = EmptyLineBehavior.NoColumns)
	{
		Reader = new StreamReader(stream);
		EmptyLineBehavior = emptyLineBehavior;
	}

	public CsvFileReader(string path, EmptyLineBehavior emptyLineBehavior = EmptyLineBehavior.NoColumns)
	{
		Reader = new StreamReader(path);
		EmptyLineBehavior = emptyLineBehavior;
	}

	public static List<List<string>> ReadAll(string path, Encoding encoding)
	{
		using (StreamReader reader = new StreamReader(path, encoding))
		{
			CsvFileReader csvFileReader = new CsvFileReader(reader);
			List<List<string>> list = new List<List<string>>();
			if (csvFileReader.ReadAll(list))
			{
				return list;
			}
		}
		return null;
	}

	public bool ReadAll(List<List<string>> dataGrid)
	{
		if (dataGrid == null)
		{
			throw new ArgumentNullException("dataGrid");
		}
		List<string> list = new List<string>();
		while (ReadRow(list))
		{
			dataGrid.Add(new List<string>(list));
		}
		return true;
	}

	public bool ReadRow(List<string> columns)
	{
		if (columns == null)
		{
			throw new ArgumentNullException("columns");
		}
		while (true)
		{
			CurrLine = Reader.ReadLine();
			CurrPos = 0;
			if (CurrLine == null)
			{
				return false;
			}
			if (CurrLine.Length == 0)
			{
				switch (EmptyLineBehavior)
				{
				case EmptyLineBehavior.Ignore:
					continue;
				case EmptyLineBehavior.NoColumns:
					columns.Clear();
					return true;
				case EmptyLineBehavior.EndOfFile:
					return false;
				}
			}
			break;
		}
		int num = 0;
		while (true)
		{
			string text = ((CurrPos >= CurrLine.Length || CurrLine[CurrPos] != base.Quote) ? ReadUnquotedColumn() : ReadQuotedColumn());
			if (num < columns.Count)
			{
				columns[num] = text;
			}
			else
			{
				columns.Add(text);
			}
			num++;
			if (CurrLine == null || CurrPos == CurrLine.Length)
			{
				break;
			}
			CurrPos++;
		}
		if (num < columns.Count)
		{
			columns.RemoveRange(num, columns.Count - num);
		}
		return true;
	}

	private string ReadQuotedColumn()
	{
		CurrPos++;
		StringBuilder stringBuilder = new StringBuilder();
		while (true)
		{
			if (CurrPos == CurrLine.Length)
			{
				CurrLine = Reader.ReadLine();
				CurrPos = 0;
				if (CurrLine == null)
				{
					return stringBuilder.ToString();
				}
				stringBuilder.Append(Environment.NewLine);
				continue;
			}
			if (CurrLine[CurrPos] == base.Quote)
			{
				int num = CurrPos + 1;
				if (num >= CurrLine.Length || CurrLine[num] != base.Quote)
				{
					break;
				}
				CurrPos++;
			}
			stringBuilder.Append(CurrLine[CurrPos++]);
		}
		if (CurrPos < CurrLine.Length)
		{
			CurrPos++;
			stringBuilder.Append(ReadUnquotedColumn());
		}
		return stringBuilder.ToString();
	}

	private string ReadUnquotedColumn()
	{
		int currPos = CurrPos;
		CurrPos = CurrLine.IndexOf(base.Delimiter, CurrPos);
		if (CurrPos == -1)
		{
			CurrPos = CurrLine.Length;
		}
		if (CurrPos > currPos)
		{
			return CurrLine.Substring(currPos, CurrPos - currPos);
		}
		return string.Empty;
	}

	public void Dispose()
	{
		Reader.Dispose();
	}
}

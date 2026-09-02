using System;
using System.Globalization;
using System.IO;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;
using NPOI.XSSF.Model;
using NPOI.XSSF.UserModel;

namespace NPOI.XSSF.Streaming;

public class SheetDataWriter
{
	private static POILogger logger = POILogFactory.GetLogger(typeof(SheetDataWriter));

	public int NumberLastFlushedRow = -1;

	private SharedStringsTable _sharedStringSource;

	private StreamWriter _outputWriter;

	protected FileInfo TemporaryFileInfo { get; set; }

	protected Stream OutputStream { get; private set; }

	private int RowNum { get; set; }

	public int NumberOfFlushedRows { get; set; }

	public int LowestIndexOfFlushedRows { get; set; } = -1;

	public int NumberOfCellsOfLastFlushedRow { get; set; }

	public FileInfo TempFileInfo => TemporaryFileInfo;

	public SheetDataWriter()
	{
		TemporaryFileInfo = CreateTempFile();
		OutputStream = CreateWriter(TemporaryFileInfo);
		_outputWriter = new StreamWriter(OutputStream, Encoding.UTF8);
	}

	public SheetDataWriter(SharedStringsTable sharedStringsTable)
		: this()
	{
		_sharedStringSource = sharedStringsTable;
	}

	public virtual FileInfo CreateTempFile()
	{
		return TempFile.CreateTempFile("poi-sxssf-sheet", ".xml");
	}

	public virtual Stream CreateWriter(FileInfo fd)
	{
		FileStream fileStream = new FileStream(fd.FullName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		Stream stream = null;
		try
		{
			return DecorateOutputStream(fileStream);
		}
		catch (Exception)
		{
			fileStream.Close();
			throw;
		}
	}

	protected virtual Stream DecorateOutputStream(Stream fos)
	{
		return fos;
	}

	public void Close()
	{
		try
		{
			_outputWriter.Flush();
			OutputStream.Flush();
		}
		catch (Exception)
		{
		}
		try
		{
			OutputStream.Close();
		}
		catch (Exception)
		{
		}
	}

	public Stream GetWorksheetXmlInputStream()
	{
		Stream stream = new FileStream(TemporaryFileInfo.FullName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		try
		{
			return DecorateInputStream(stream);
		}
		catch (IOException)
		{
			stream.Close();
			throw;
		}
	}

	protected virtual Stream DecorateInputStream(Stream fis)
	{
		return fis;
	}

	protected void FinalizeWriter()
	{
		TemporaryFileInfo.Delete();
		if (File.Exists(TemporaryFileInfo.FullName))
		{
			logger.Log(7, "Can't delete temporary encryption file: " + TemporaryFileInfo);
		}
	}

	public void WriteRow(int rownum, SXSSFRow row)
	{
		BeginRow(rownum, row);
		using (SXSSFRow.CellIterator cellIterator = row.AllCellsIterator())
		{
			int num = 0;
			while (cellIterator.MoveNext())
			{
				WriteCell(num++, cellIterator.Current);
			}
			EndRow();
		}
		if (LowestIndexOfFlushedRows == -1 || LowestIndexOfFlushedRows > rownum)
		{
			LowestIndexOfFlushedRows = rownum;
			NumberOfFlushedRows++;
		}
	}

	public void FlushRows(int rowCount, int lastRowNum, int lastRowCellsCount)
	{
		NumberLastFlushedRow = Math.Max(lastRowNum, NumberLastFlushedRow);
		NumberOfCellsOfLastFlushedRow = lastRowCellsCount;
		_outputWriter.Flush();
		OutputStream.Flush();
	}

	private void BeginRow(int rownum, SXSSFRow row)
	{
		WriteAsBytes("<row r=\"");
		WriteAsBytes(rownum + 1);
		WriteAsBytes("\"");
		if (row.HasCustomHeight())
		{
			WriteAsBytes(" customHeight=\"true\"  ht=\"");
			WriteAsBytes(row.HeightInPoints);
			WriteAsBytes("\"");
		}
		if (row.ZeroHeight)
		{
			WriteAsBytes(" hidden=\"true\"");
		}
		if (row.IsFormatted)
		{
			WriteAsBytes(" s=\"");
			WriteAsBytes(row.RowStyle.Index);
			WriteAsBytes("\"");
			WriteAsBytes(" customFormat=\"1\"");
		}
		if (row.OutlineLevel != 0)
		{
			WriteAsBytes(" outlineLevel=\"");
			WriteAsBytes(row.OutlineLevel);
			WriteAsBytes("\"");
		}
		if (row.Hidden.HasValue)
		{
			WriteAsBytes(" hidden=\"");
			WriteAsBytes(row.Hidden.Value ? "1" : "0");
			WriteAsBytes("\"");
		}
		if (row.Collapsed.HasValue)
		{
			WriteAsBytes(" collapsed=\"");
			WriteAsBytes(row.Collapsed.Value ? "1" : "0");
			WriteAsBytes("\"");
		}
		WriteAsBytes(">\n");
		RowNum = rownum;
	}

	private void EndRow()
	{
		WriteAsBytes("</row>\n");
	}

	public void WriteCell(int columnIndex, ICell cell)
	{
		if (cell == null)
		{
			return;
		}
		string text = new CellReference(RowNum, columnIndex).FormatAsString();
		WriteAsBytes("<c r=\"");
		WriteAsBytes(text);
		WriteAsBytes("\"");
		if (cell.CellStyle.Index != 0)
		{
			WriteAsBytes(" s=\"");
			WriteAsBytes(cell.CellStyle.Index & 0xFFFF);
			WriteAsBytes("\"");
		}
		switch (cell.CellType)
		{
		case CellType.Blank:
			WriteAsBytes(">");
			break;
		case CellType.Formula:
			WriteAsBytes(">");
			WriteAsBytes("<f>");
			OutputQuotedString(cell.CellFormula);
			WriteAsBytes("</f>");
			if (cell.GetCachedFormulaResultTypeEnum() == CellType.Numeric)
			{
				double numericCellValue = cell.NumericCellValue;
				if (!double.IsNaN(numericCellValue))
				{
					WriteAsBytes("<v>");
					WriteAsBytes(numericCellValue);
					WriteAsBytes("</v>");
				}
			}
			break;
		case CellType.String:
			if (_sharedStringSource != null)
			{
				XSSFRichTextString xSSFRichTextString = new XSSFRichTextString(cell.StringCellValue);
				int value = _sharedStringSource.AddEntry(xSSFRichTextString.GetCTRst());
				WriteAsBytes(" t=\"");
				WriteAsBytes("s");
				WriteAsBytes("\">");
				WriteAsBytes("<v>");
				WriteAsBytes(value);
				WriteAsBytes("</v>");
			}
			else
			{
				WriteAsBytes(" t=\"inlineStr\">");
				WriteAsBytes("<is><t");
				if (HasLeadingTrailingSpaces(cell.StringCellValue))
				{
					WriteAsBytes(" xml:space=\"preserve\"");
				}
				WriteAsBytes(">");
				OutputQuotedString(cell.StringCellValue);
				WriteAsBytes("</t></is>");
			}
			break;
		case CellType.Numeric:
			WriteAsBytes(" t=\"n\">");
			WriteAsBytes("<v>");
			WriteAsBytes(cell.NumericCellValue);
			WriteAsBytes("</v>");
			break;
		case CellType.Boolean:
			WriteAsBytes(" t=\"b\">");
			WriteAsBytes("<v>");
			WriteAsBytes(cell.BooleanCellValue ? "1" : "0");
			WriteAsBytes("</v>");
			break;
		case CellType.Error:
		{
			FormulaError formulaError = FormulaError.ForInt(cell.ErrorCellValue);
			WriteAsBytes(" t=\"e\">");
			WriteAsBytes("<v>");
			WriteAsBytes(formulaError.String);
			WriteAsBytes("</v>");
			break;
		}
		default:
			throw new InvalidOperationException("Invalid cell type: " + cell.CellType);
		}
		WriteAsBytes("</c>");
	}

	private void WriteAsBytes(string text)
	{
		_outputWriter.Write(text);
	}

	private void WriteAsBytes(ArraySegment<char> chars)
	{
		_outputWriter.Write(chars.Array, chars.Offset, chars.Count);
	}

	private void WriteAsBytes(int value)
	{
		_outputWriter.Write(value);
	}

	private void WriteAsBytes(float value)
	{
		_outputWriter.Write(value.ToString(CultureInfo.InvariantCulture));
	}

	private void WriteAsBytes(double value)
	{
		_outputWriter.Write(value.ToString(CultureInfo.InvariantCulture));
	}

	private bool HasLeadingTrailingSpaces(string str)
	{
		if (!string.IsNullOrEmpty(str))
		{
			char src = str[0];
			char src2 = str[str.Length - 1];
			if (!Character.isWhitespace(src))
			{
				return Character.isWhitespace(src2);
			}
			return true;
		}
		return false;
	}

	protected void OutputQuotedString(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return;
		}
		char[] array = s.ToCharArray();
		int num = 0;
		int length = s.Length;
		for (int i = 0; i < length; i++)
		{
			char c = array[i];
			switch (c)
			{
			case '<':
				if (i > num)
				{
					WriteAsBytes(GetSubArray(array, num, i - num));
				}
				num = i + 1;
				WriteAsBytes("&lt;");
				continue;
			case '>':
				if (i > num)
				{
					WriteAsBytes(GetSubArray(array, num, i - num));
				}
				num = i + 1;
				WriteAsBytes("&gt;");
				continue;
			case '&':
				if (i > num)
				{
					WriteAsBytes(GetSubArray(array, num, i - num));
				}
				num = i + 1;
				WriteAsBytes("&amp;");
				continue;
			case '"':
				if (i > num)
				{
					WriteAsBytes(GetSubArray(array, num, i - num));
				}
				num = i + 1;
				WriteAsBytes("&quot;");
				continue;
			case '\n':
			case '\r':
				if (i > num)
				{
					WriteAsBytes(GetSubArray(array, num, i - num));
				}
				WriteAsBytes("&#xa;");
				num = i + 1;
				continue;
			case '\t':
				if (i > num)
				{
					WriteAsBytes(GetSubArray(array, num, i - num));
				}
				WriteAsBytes("&#x9;");
				num = i + 1;
				continue;
			case '\u00a0':
				if (i > num)
				{
					WriteAsBytes(GetSubArray(array, num, i - num));
				}
				WriteAsBytes("&#xa0;");
				num = i + 1;
				continue;
			}
			if (c < ' ' || char.IsLowSurrogate(c) || char.IsHighSurrogate(c) || '\ufffe' <= c)
			{
				if (i > num)
				{
					WriteAsBytes(GetSubArray(array, num, i - num));
				}
				WriteAsBytes("?");
				num = i + 1;
			}
			else if (c > '\u007f')
			{
				if (i > num)
				{
					WriteAsBytes(GetSubArray(array, num, i - num));
				}
				num = i + 1;
				WriteAsBytes("&#");
				WriteAsBytes(c);
				WriteAsBytes(";");
			}
		}
		if (num < length)
		{
			WriteAsBytes(GetSubArray(array, num, length - num));
		}
	}

	private static ArraySegment<char> GetSubArray(char[] oldArray, int skip, int take)
	{
		return new ArraySegment<char>(oldArray, skip, take);
	}

	public bool Dispose()
	{
		bool result;
		try
		{
			OutputStream.Close();
		}
		finally
		{
			TemporaryFileInfo.Delete();
			result = !File.Exists(TemporaryFileInfo.FullName);
			TemporaryFileInfo.Refresh();
		}
		return result;
	}

	public string TemporaryFilePath()
	{
		if (TemporaryFileInfo != null)
		{
			return TemporaryFileInfo.FullName;
		}
		return string.Empty;
	}
}

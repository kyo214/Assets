using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using NPOI.SS;
using NPOI.SS.Formula.UDF;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;
using NPOI.XSSF.Model;
using NPOI.XSSF.UserModel;

namespace NPOI.XSSF.Streaming;

public class SXSSFWorkbook : IWorkbook, ICloseable
{
	private class SheetEnumerator<T> : IEnumerator<T>, IDisposable, IEnumerator where T : class, ISheet
	{
		private XSSFWorkbook _wb;

		private SXSSFWorkbook _xwb;

		private IEnumerator<ISheet> it;

		T IEnumerator<T>.Current
		{
			get
			{
				XSSFSheet sheet = (XSSFSheet)it.Current;
				return _xwb.GetSXSSFSheet(sheet) as T;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				XSSFSheet sheet = (XSSFSheet)it.Current;
				return _xwb.GetSXSSFSheet(sheet);
			}
		}

		public SheetEnumerator(XSSFWorkbook wb, SXSSFWorkbook xwb)
		{
			_wb = wb;
			_xwb = xwb;
			it = wb.GetEnumerator();
		}

		public void Dispose()
		{
			it.Dispose();
		}

		public bool MoveNext()
		{
			return it.MoveNext();
		}

		public void Reset()
		{
			it.Reset();
		}
	}

	private static readonly POILogger logger = POILogFactory.GetLogger(typeof(SXSSFWorkbook));

	public const int DEFAULT_WINDOW_SIZE = 100;

	private XSSFWorkbook _wb;

	private Dictionary<SXSSFSheet, XSSFSheet> _sxFromXHash = new Dictionary<SXSSFSheet, XSSFSheet>();

	private Dictionary<XSSFSheet, SXSSFSheet> _xFromSxHash = new Dictionary<XSSFSheet, SXSSFSheet>();

	private int _randomAccessWindowSize = 100;

	private bool _compressTmpFiles;

	private SharedStringsTable _sharedStringSource;

	public XSSFWorkbook XssfWorkbook => _wb;

	public int RandomAccessWindowSize
	{
		get
		{
			return _randomAccessWindowSize;
		}
		set
		{
			if (value <= 0)
			{
				throw new ArgumentException("rowAccessWindowSize must be greater than 0 or -1");
			}
			_randomAccessWindowSize = value;
		}
	}

	public int ActiveSheetIndex => XssfWorkbook.ActiveSheetIndex;

	public int FirstVisibleTab
	{
		get
		{
			return XssfWorkbook.FirstVisibleTab;
		}
		set
		{
			XssfWorkbook.FirstVisibleTab = value;
		}
	}

	public int NumberOfSheets => XssfWorkbook.NumberOfSheets;

	public short NumberOfFonts => XssfWorkbook.NumberOfFonts;

	public int NumCellStyles => XssfWorkbook.NumCellStyles;

	public int NumberOfNames => XssfWorkbook.NumberOfNames;

	public MissingCellPolicy MissingCellPolicy
	{
		get
		{
			return XssfWorkbook.MissingCellPolicy;
		}
		set
		{
			XssfWorkbook.MissingCellPolicy = value;
		}
	}

	public bool IsHidden
	{
		get
		{
			return XssfWorkbook.IsHidden;
		}
		set
		{
			XssfWorkbook.IsHidden = value;
		}
	}

	public bool CompressTempFiles
	{
		get
		{
			return _compressTmpFiles;
		}
		set
		{
			_compressTmpFiles = value;
		}
	}

	public SpreadsheetVersion SpreadsheetVersion => SpreadsheetVersion.EXCEL2007;

	public SXSSFWorkbook(int rowAccessWindowSize)
		: this(null, rowAccessWindowSize)
	{
	}

	public SXSSFWorkbook()
		: this(null)
	{
	}

	public SXSSFWorkbook(XSSFWorkbook workbook)
		: this(workbook, 100)
	{
	}

	public SXSSFWorkbook(XSSFWorkbook workbook, int rowAccessWindowSize)
		: this(workbook, rowAccessWindowSize, compressTmpFiles: false)
	{
	}

	public SXSSFWorkbook(XSSFWorkbook workbook, int rowAccessWindowSize, bool compressTmpFiles)
		: this(workbook, rowAccessWindowSize, compressTmpFiles, useSharedStringsTable: false)
	{
	}

	public SXSSFWorkbook(XSSFWorkbook workbook, int rowAccessWindowSize, bool compressTmpFiles, bool useSharedStringsTable)
	{
		RandomAccessWindowSize = rowAccessWindowSize;
		_compressTmpFiles = compressTmpFiles;
		if (workbook == null)
		{
			_wb = new XSSFWorkbook();
			_sharedStringSource = (useSharedStringsTable ? XssfWorkbook.GetSharedStringSource() : null);
			return;
		}
		_wb = workbook;
		_sharedStringSource = (useSharedStringsTable ? XssfWorkbook.GetSharedStringSource() : null);
		int numberOfSheets = XssfWorkbook.NumberOfSheets;
		for (int i = 0; i < numberOfSheets; i++)
		{
			XSSFSheet xSheet = (XSSFSheet)XssfWorkbook.GetSheetAt(i);
			CreateAndRegisterSXSSFSheet(xSheet);
		}
	}

	private SXSSFSheet CreateAndRegisterSXSSFSheet(ISheet xSheet)
	{
		SXSSFSheet sXSSFSheet;
		try
		{
			sXSSFSheet = new SXSSFSheet(this, (XSSFSheet)xSheet);
		}
		catch (IOException e)
		{
			throw new RuntimeException(e);
		}
		RegisterSheetMapping(sXSSFSheet, (XSSFSheet)xSheet);
		return sXSSFSheet;
	}

	private void RegisterSheetMapping(SXSSFSheet sxSheet, XSSFSheet xSheet)
	{
		_sxFromXHash.Add(sxSheet, xSheet);
		_xFromSxHash.Add(xSheet, sxSheet);
	}

	private void DeregisterSheetMapping(XSSFSheet xSheet)
	{
		SXSSFSheet sXSSFSheet = GetSXSSFSheet(xSheet);
		try
		{
			sXSSFSheet.SheetDataWriter.Close();
		}
		catch (IOException)
		{
		}
		_sxFromXHash.Remove(sXSSFSheet);
		_xFromSxHash.Remove(xSheet);
	}

	private XSSFSheet GetXSSFSheet(SXSSFSheet sheet)
	{
		if (sheet != null && _sxFromXHash.ContainsKey(sheet))
		{
			return _sxFromXHash[sheet];
		}
		return null;
	}

	private SXSSFSheet GetSXSSFSheet(XSSFSheet sheet)
	{
		if (sheet != null && _xFromSxHash.ContainsKey(sheet))
		{
			return _xFromSxHash[sheet];
		}
		return null;
	}

	public SheetDataWriter CreateSheetDataWriter()
	{
		if (_compressTmpFiles)
		{
			return new GZIPSheetDataWriter(_sharedStringSource);
		}
		return new SheetDataWriter(_sharedStringSource);
	}

	private XSSFSheet GetSheetFromZipEntryName(string sheetRef)
	{
		foreach (XSSFSheet value in _sxFromXHash.Values)
		{
			if (sheetRef.Equals(value.GetPackagePart().PartName.Name.Substring(1)))
			{
				return value;
			}
		}
		return null;
	}

	private void InjectData(FileInfo zipfile, Stream outStream)
	{
		ZipFile zipFile = new ZipFile(zipfile.FullName);
		try
		{
			ZipOutputStream zipOutputStream = new ZipOutputStream(outStream);
			try
			{
				IEnumerator enumerator = zipFile.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ZipEntry zipEntry = (ZipEntry)enumerator.Current;
					zipOutputStream.PutNextEntry(new ZipEntry(zipEntry.Name));
					Stream inputStream = zipFile.GetInputStream(zipEntry);
					XSSFSheet sheetFromZipEntryName = GetSheetFromZipEntryName(zipEntry.Name);
					if (sheetFromZipEntryName != null)
					{
						Stream worksheetXMLInputStream = GetSXSSFSheet(sheetFromZipEntryName).GetWorksheetXMLInputStream();
						try
						{
							CopyStreamAndInjectWorksheet(inputStream, zipOutputStream, worksheetXMLInputStream);
						}
						finally
						{
							worksheetXMLInputStream.Close();
						}
					}
					else
					{
						inputStream.CopyTo(zipOutputStream);
					}
					inputStream.Close();
				}
			}
			finally
			{
				zipOutputStream.Close();
			}
		}
		finally
		{
			zipFile.Close();
		}
	}

	private static void CopyStreamAndInjectWorksheet(Stream inputStream, Stream outputStream, Stream worksheetData)
	{
		StreamReader streamReader = new StreamReader(inputStream, Encoding.UTF8);
		StreamWriter streamWriter = new StreamWriter(outputStream, Encoding.UTF8);
		bool flag = true;
		int num = 0;
		string text = "<sheetData";
		StringBuilder stringBuilder = new StringBuilder();
		int length = text.Length;
		int num2;
		while ((num2 = streamReader.Read()) != -1)
		{
			if ((ushort)num2 == text[num])
			{
				num++;
				if (num != length)
				{
					continue;
				}
				if (!"<sheetData".Equals(text))
				{
					break;
				}
				num2 = streamReader.Read();
				if (num2 == -1)
				{
					streamWriter.Write(text);
					stringBuilder.Append(text);
					break;
				}
				if ((ushort)num2 == 62)
				{
					streamWriter.Write(text);
					stringBuilder.Append(text);
					streamWriter.Write((char)num2);
					stringBuilder.Append((char)num2);
					text = "</sheetData>";
					length = text.Length;
					num = 0;
					flag = false;
				}
				else if ((ushort)num2 == 47)
				{
					num2 = streamReader.Read();
					if (num2 == -1)
					{
						streamWriter.Write(text);
						stringBuilder.Append(text);
						break;
					}
					if ((ushort)num2 == 62)
					{
						break;
					}
					streamWriter.Write(text);
					stringBuilder.Append(text);
					streamWriter.Write('/');
					stringBuilder.Append('/');
					streamWriter.Write((char)num2);
					stringBuilder.Append((char)num2);
					num = 0;
				}
				else
				{
					streamWriter.Write(text);
					stringBuilder.Append(text);
					streamWriter.Write('/');
					stringBuilder.Append('/');
					streamWriter.Write((char)num2);
					stringBuilder.Append((char)num2);
					num = 0;
				}
			}
			else
			{
				if (num > 0)
				{
					streamWriter.Write(text.Substring(0, num));
					stringBuilder.Append(text, 0, num);
				}
				if (num2 == text[0])
				{
					num = 1;
					continue;
				}
				streamWriter.Write((char)num2);
				stringBuilder.Append((char)num2);
				num = 0;
			}
		}
		streamWriter.Flush();
		if (flag)
		{
			streamWriter.Write("<sheetData>\n");
			stringBuilder.Append("<sheetData>\n");
			streamWriter.Flush();
		}
		worksheetData.CopyTo(outputStream);
		streamWriter.Write("</sheetData>");
		streamWriter.Flush();
		while ((num2 = streamReader.Read()) != -1)
		{
			streamWriter.Write((char)num2);
			stringBuilder.Append((char)num2);
		}
		streamWriter.Flush();
	}

	public void SetSheetOrder(string sheetname, int pos)
	{
		XssfWorkbook.SetSheetOrder(sheetname, pos);
	}

	public void SetSelectedTab(int index)
	{
		XssfWorkbook.SetSelectedTab(index);
	}

	public void SetActiveSheet(int sheetIndex)
	{
		XssfWorkbook.SetActiveSheet(sheetIndex);
	}

	public string GetSheetName(int sheet)
	{
		return XssfWorkbook.GetSheetName(sheet);
	}

	public void SetSheetName(int sheet, string name)
	{
		XssfWorkbook.SetSheetName(sheet, name);
	}

	public int GetSheetIndex(string name)
	{
		return XssfWorkbook.GetSheetIndex(name);
	}

	public int GetSheetIndex(ISheet sheet)
	{
		return XssfWorkbook.GetSheetIndex(GetXSSFSheet((SXSSFSheet)sheet));
	}

	public ISheet CreateSheet()
	{
		return CreateAndRegisterSXSSFSheet(XssfWorkbook.CreateSheet());
	}

	public ISheet CreateSheet(string sheetname)
	{
		WorkbookUtil.ValidateSheetName(sheetname);
		return CreateAndRegisterSXSSFSheet(XssfWorkbook.CreateSheet(sheetname));
	}

	public ISheet CloneSheet(int sheetNum)
	{
		throw new RuntimeException("NotImplemented");
	}

	public ISheet GetSheetAt(int index)
	{
		return GetSXSSFSheet((XSSFSheet)XssfWorkbook.GetSheetAt(index));
	}

	public ISheet GetSheet(string name)
	{
		return GetSXSSFSheet((XSSFSheet)XssfWorkbook.GetSheet(name));
	}

	public void RemoveSheetAt(int index)
	{
		XSSFSheet xSSFSheet = (XSSFSheet)XssfWorkbook.GetSheetAt(index);
		SXSSFSheet sXSSFSheet = GetSXSSFSheet(xSSFSheet);
		XssfWorkbook.RemoveSheetAt(index);
		DeregisterSheetMapping(xSSFSheet);
		try
		{
			sXSSFSheet.Dispose();
		}
		catch (IOException exception)
		{
			logger.Log(5, exception);
		}
	}

	public IEnumerator<ISheet> GetEnumerator()
	{
		return new SheetEnumerator<SXSSFSheet>(XssfWorkbook, this);
	}

	public IFont CreateFont()
	{
		return XssfWorkbook.CreateFont();
	}

	[Obsolete("deprecated in poi 3.16")]
	public IFont FindFont(short boldWeight, short color, short fontHeight, string name, bool italic, bool strikeout, FontSuperScript typeOffset, FontUnderlineType underline)
	{
		return XssfWorkbook.FindFont(boldWeight, color, fontHeight, name, italic, strikeout, typeOffset, underline);
	}

	public IFont FindFont(bool bold, short color, short fontHeight, string name, bool italic, bool strikeout, FontSuperScript typeOffset, FontUnderlineType underline)
	{
		return XssfWorkbook.FindFont(bold, color, fontHeight, name, italic, strikeout, typeOffset, underline);
	}

	public IFont GetFontAt(short idx)
	{
		return XssfWorkbook.GetFontAt(idx);
	}

	public ICellStyle CreateCellStyle()
	{
		return XssfWorkbook.CreateCellStyle();
	}

	public ICellStyle GetCellStyleAt(int idx)
	{
		return XssfWorkbook.GetCellStyleAt(idx);
	}

	public void Close()
	{
		foreach (SXSSFSheet value in _xFromSxHash.Values)
		{
			try
			{
				value.SheetDataWriter.Close();
			}
			catch (IOException exception)
			{
				logger.Log(5, "An exception occurred while closing sheet data writer for sheet " + value.SheetName + ".", exception);
			}
		}
		XssfWorkbook.Close();
	}

	public void Write(Stream stream)
	{
		FlushSheets();
		FileInfo fileInfo = TempFile.CreateTempFile("poi-sxssf-template", ".xlsx");
		try
		{
			FileStream fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.ReadWrite);
			try
			{
				XssfWorkbook.Write(fileStream);
			}
			finally
			{
				fileStream.Close();
			}
			InjectData(fileInfo, stream);
		}
		finally
		{
			fileInfo.Delete();
			if (File.Exists(fileInfo.FullName))
			{
				throw new IOException("Could not delete temporary file after processing: " + fileInfo);
			}
		}
	}

	private void FlushSheets()
	{
		foreach (SXSSFSheet value in _xFromSxHash.Values)
		{
			value.FlushRows();
		}
	}

	public bool Dispose()
	{
		bool flag = true;
		foreach (SXSSFSheet key in _sxFromXHash.Keys)
		{
			try
			{
				flag = key.Dispose() & flag;
			}
			catch (IOException exception)
			{
				logger.Log(5, exception);
				flag = false;
			}
		}
		return flag;
	}

	public IName GetName(string name)
	{
		return XssfWorkbook.GetName(name);
	}

	public IList<IName> GetNames(string name)
	{
		return XssfWorkbook.GetNames(name);
	}

	public IList<IName> GetAllNames()
	{
		return _wb.GetAllNames();
	}

	[Obsolete("Deprecated 3.16, New projects should avoid accessing named ranges by index.")]
	public IName GetNameAt(int nameIndex)
	{
		return XssfWorkbook.GetNameAt(nameIndex);
	}

	public IName CreateName()
	{
		return XssfWorkbook.CreateName();
	}

	[Obsolete("deprecated in 3.16 New projects should avoid accessing named ranges by index. GetName(String)} instead.")]
	public int GetNameIndex(string name)
	{
		return XssfWorkbook.GetNameIndex(name);
	}

	[Obsolete("deprecated in 3.16 New projects should use RemoveName(Name)")]
	public void RemoveName(int index)
	{
		XssfWorkbook.RemoveName(index);
	}

	[Obsolete("deprecated in 3.16 New projects should use RemoveName(IName Name)")]
	public void RemoveName(string name)
	{
		XssfWorkbook.RemoveName(name);
	}

	public void RemoveName(IName name)
	{
		_wb.RemoveName(name);
	}

	public int LinkExternalWorkbook(string name, IWorkbook workbook)
	{
		throw new NotImplementedException();
	}

	public void SetPrintArea(int sheetIndex, string reference)
	{
		XssfWorkbook.SetPrintArea(sheetIndex, reference);
	}

	public void SetPrintArea(int sheetIndex, int startColumn, int endColumn, int startRow, int endRow)
	{
		XssfWorkbook.SetPrintArea(sheetIndex, startColumn, endColumn, startRow, endRow);
	}

	public string GetPrintArea(int sheetIndex)
	{
		return XssfWorkbook.GetPrintArea(sheetIndex);
	}

	public void RemovePrintArea(int sheetIndex)
	{
		XssfWorkbook.RemovePrintArea(sheetIndex);
	}

	public IDataFormat CreateDataFormat()
	{
		return XssfWorkbook.CreateDataFormat();
	}

	public int AddPicture(byte[] pictureData, PictureType format)
	{
		return XssfWorkbook.AddPicture(pictureData, format);
	}

	public IList GetAllPictures()
	{
		return XssfWorkbook.GetAllPictures();
	}

	public ICreationHelper GetCreationHelper()
	{
		return new SXSSFCreationHelper(this);
	}

	public bool IsSheetHidden(int sheetIx)
	{
		return XssfWorkbook.IsSheetHidden(sheetIx);
	}

	public bool IsSheetVeryHidden(int sheetIx)
	{
		return XssfWorkbook.IsSheetVeryHidden(sheetIx);
	}

	public void SetSheetHidden(int sheetIx, SheetState hidden)
	{
		XssfWorkbook.SetSheetHidden(sheetIx, hidden);
	}

	public void SetSheetHidden(int sheetIx, int hidden)
	{
		XssfWorkbook.SetSheetHidden(sheetIx, hidden);
	}

	public void AddToolPack(UDFFinder toopack)
	{
		XssfWorkbook.AddToolPack(toopack);
	}

	public bool IsDate1904()
	{
		return XssfWorkbook.IsDate1904();
	}
}

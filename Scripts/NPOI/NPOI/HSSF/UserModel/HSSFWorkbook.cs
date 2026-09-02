using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using NPOI.DDF;
using NPOI.HSSF.Model;
using NPOI.HSSF.Record;
using NPOI.HSSF.Record.Aggregates;
using NPOI.POIFS.Crypt;
using NPOI.POIFS.FileSystem;
using NPOI.SS;
using NPOI.SS.Formula;
using NPOI.SS.Formula.UDF;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.HSSF.UserModel;

[Serializable]
public class HSSFWorkbook : POIDocument, IWorkbook, ICloseable
{
	private class SheetRecordCollector : RecordVisitor, IDisposable
	{
		private ArrayList _list;

		private int _totalSize;

		public int TotalSize => _totalSize;

		public SheetRecordCollector()
		{
			_totalSize = 0;
			_list = new ArrayList(128);
		}

		public void VisitRecord(NPOI.HSSF.Record.Record r)
		{
			_list.Add(r);
			_totalSize += r.RecordSize;
		}

		public int Serialize(int offset, byte[] data)
		{
			int num = 0;
			_ = _list.Count;
			foreach (NPOI.HSSF.Record.Record item in _list)
			{
				num += item.Serialize(offset + num, data);
			}
			return num;
		}

		public void Dispose()
		{
		}
	}

	private const int MAX_STYLES = 4030;

	public const int INITIAL_CAPACITY = 3;

	private InternalWorkbook workbook;

	protected List<HSSFSheet> _sheets;

	private List<HSSFName> names;

	private bool preserveNodes;

	private HSSFDataFormat formatter;

	private Dictionary<short, HSSFFont> fonts;

	[NonSerialized]
	private MissingCellPolicy missingCellPolicy = MissingCellPolicy.RETURN_NULL_AND_BLANK;

	public const byte ENCODING_COMPRESSED_UNICODE = 0;

	public const byte ENCODING_UTF_16 = 1;

	[NonSerialized]
	private UDFFinder _udfFinder = new IndexedUDFFinder(UDFFinder.GetDefault());

	public MissingCellPolicy MissingCellPolicy
	{
		get
		{
			return missingCellPolicy;
		}
		set
		{
			missingCellPolicy = value;
		}
	}

	public int ActiveSheetIndex => workbook.WindowOne.ActiveSheetIndex;

	public int FirstVisibleTab
	{
		get
		{
			return workbook.WindowOne.FirstVisibleTab;
		}
		set
		{
			workbook.WindowOne.FirstVisibleTab = value;
		}
	}

	public int NumberOfSheets => _sheets.Count;

	public bool BackupFlag
	{
		get
		{
			return workbook.BackupRecord.Backup != 0;
		}
		set
		{
			workbook.BackupRecord.Backup = (short)(value ? 1 : 0);
		}
	}

	public short NumberOfFonts => (short)workbook.NumberOfFontRecords;

	public bool IsHidden
	{
		get
		{
			return workbook.WindowOne.Hidden;
		}
		set
		{
			workbook.WindowOne.Hidden = value;
		}
	}

	public int NumCellStyles => workbook.NumExFormats;

	public InternalWorkbook Workbook => workbook;

	public int NumberOfNames => names.Count;

	public bool IsWriteProtected => workbook.IsWriteProtected;

	public byte[] NewUID => new byte[16];

	public bool ForceFormulaRecalculation
	{
		get
		{
			RecalcIdRecord recalcIdRecord = (RecalcIdRecord)Workbook.FindFirstRecordBySid(449);
			if (recalcIdRecord != null)
			{
				return recalcIdRecord.EngineId != 0;
			}
			return false;
		}
		set
		{
			Workbook.RecalcId.EngineId = 0;
		}
	}

	public InternalWorkbook InternalWorkbook => workbook;

	public SpreadsheetVersion SpreadsheetVersion => SpreadsheetVersion.EXCEL97;

	public DirectoryNode RootDirectory => directory;

	public ISheet this[int index]
	{
		get
		{
			return GetSheetAt(index);
		}
		set
		{
			if (_sheets[index] != null)
			{
				_sheets[index] = (HSSFSheet)value;
			}
			else
			{
				_sheets.Insert(index, (HSSFSheet)value);
			}
		}
	}

	public int Count => NumberOfSheets;

	public bool IsReadOnly
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public ICreationHelper GetCreationHelper()
	{
		return new HSSFCreationHelper(this);
	}

	public static HSSFWorkbook Create(InternalWorkbook book)
	{
		return new HSSFWorkbook(book);
	}

	public HSSFWorkbook()
		: this(InternalWorkbook.CreateWorkbook())
	{
	}

	public HSSFWorkbook(InternalWorkbook book)
		: base((DirectoryNode)null)
	{
		workbook = book;
		_sheets = new List<HSSFSheet>(3);
		names = new List<HSSFName>(3);
	}

	public HSSFWorkbook(POIFSFileSystem fs)
		: this(fs, preserveNodes: true)
	{
	}

	public HSSFWorkbook(NPOIFSFileSystem fs)
		: this(fs.Root, preserveNodes: true)
	{
	}

	public HSSFWorkbook(POIFSFileSystem fs, bool preserveNodes)
		: this(fs.Root, fs, preserveNodes)
	{
	}

	private static string GetWorkbookDirEntryName(DirectoryNode directory)
	{
		string[] wORKBOOK_DIR_ENTRY_NAMES = InternalWorkbook.WORKBOOK_DIR_ENTRY_NAMES;
		foreach (string text in wORKBOOK_DIR_ENTRY_NAMES)
		{
			try
			{
				directory.GetEntry(text);
				return text;
			}
			catch (FileNotFoundException)
			{
			}
		}
		try
		{
			directory.GetEntry(Decryptor.DEFAULT_POIFS_ENTRY);
			throw new EncryptedDocumentException("The supplied spreadsheet seems to be an Encrypted .xlsx file. It must be decrypted before use by XSSF, it cannot be used by HSSF");
		}
		catch (FileNotFoundException)
		{
		}
		try
		{
			directory.GetEntry(InternalWorkbook.OLD_WORKBOOK_DIR_ENTRY_NAME);
			throw new OldExcelFormatException("The supplied spreadsheet seems to be Excel 5.0/7.0 (BIFF5) format. POI only supports BIFF8 format (from Excel versions 97/2000/XP/2003)");
		}
		catch (FileNotFoundException)
		{
		}
		throw new ArgumentException("The supplied POIFSFileSystem does not contain a BIFF8 'Workbook' entry. Is it really an excel file?");
	}

	public HSSFWorkbook(DirectoryNode directory, POIFSFileSystem fs, bool preserveNodes)
		: this(directory, preserveNodes)
	{
	}

	public HSSFWorkbook(DirectoryNode directory, bool preserveNodes)
		: base(directory)
	{
		string workbookDirEntryName = GetWorkbookDirEntryName(directory);
		this.preserveNodes = preserveNodes;
		if (!preserveNodes)
		{
			base.directory = null;
		}
		_sheets = new List<HSSFSheet>(3);
		names = new List<HSSFName>(3);
		List<NPOI.HSSF.Record.Record> list = RecordFactory.CreateRecords(directory.CreatePOIFSDocumentReader(workbookDirEntryName));
		workbook = InternalWorkbook.CreateWorkbook(list);
		SetPropertiesFromWorkbook(workbook);
		int numRecords = workbook.NumRecords;
		ConvertLabelRecords(list, numRecords);
		RecordStream recordStream = new RecordStream(list, numRecords);
		while (recordStream.HasNext())
		{
			try
			{
				InternalSheet sheet = InternalSheet.CreateSheet(recordStream);
				_sheets.Add(new HSSFSheet(this, sheet));
			}
			catch (UnsupportedBOFType unsupportedBOFType)
			{
				Console.WriteLine("Unsupported BOF found of type " + unsupportedBOFType.Type);
			}
		}
		for (int i = 0; i < workbook.NumNames; i++)
		{
			NameRecord nameRecord = workbook.GetNameRecord(i);
			HSSFName item = new HSSFName(this, workbook.GetNameRecord(i), workbook.GetNameCommentRecord(nameRecord));
			names.Add(item);
		}
	}

	public HSSFWorkbook(Stream s)
		: this(s, preserveNodes: true)
	{
	}

	public HSSFWorkbook(Stream s, bool preserveNodes)
		: this(new POIFSFileSystem(s), preserveNodes)
	{
	}

	private void SetPropertiesFromWorkbook(InternalWorkbook book)
	{
		workbook = book;
	}

	private void ConvertLabelRecords(IList records, int offset)
	{
		for (int i = offset; i < records.Count; i++)
		{
			NPOI.HSSF.Record.Record record = (NPOI.HSSF.Record.Record)records[i];
			if (record.Sid == 516)
			{
				LabelRecord labelRecord = (LabelRecord)record;
				records.RemoveAt(i);
				LabelSSTRecord labelSSTRecord = new LabelSSTRecord();
				int sSTIndex = workbook.AddSSTString(new UnicodeString(labelRecord.Value));
				labelSSTRecord.Row = labelRecord.Row;
				labelSSTRecord.Column = labelRecord.Column;
				labelSSTRecord.XFIndex = labelRecord.XFIndex;
				labelSSTRecord.SSTIndex = sSTIndex;
				records.Insert(i, labelSSTRecord);
			}
		}
	}

	public void SetSheetOrder(string sheetname, int pos)
	{
		int sheetIndex = GetSheetIndex(sheetname);
		HSSFSheet item = (HSSFSheet)GetSheet(sheetname);
		_sheets.RemoveAt(sheetIndex);
		_sheets.Insert(pos, item);
		workbook.SetSheetOrder(sheetname, pos);
		FormulaShifter shifter = FormulaShifter.CreateForSheetShift(sheetIndex, pos);
		foreach (HSSFSheet sheet in _sheets)
		{
			sheet.Sheet.UpdateFormulasAfterCellShift(shifter, -1);
		}
		workbook.UpdateNamesAfterCellShift(shifter);
		int activeSheetIndex = ActiveSheetIndex;
		if (activeSheetIndex == sheetIndex)
		{
			SetActiveSheet(pos);
		}
		else if ((activeSheetIndex >= sheetIndex || activeSheetIndex >= pos) && (activeSheetIndex <= sheetIndex || activeSheetIndex <= pos))
		{
			if (pos > sheetIndex)
			{
				SetActiveSheet(activeSheetIndex - 1);
			}
			else
			{
				SetActiveSheet(activeSheetIndex + 1);
			}
		}
	}

	private void ValidateSheetIndex(int index)
	{
		int num = _sheets.Count - 1;
		if (index < 0 || index > num)
		{
			string text = "(0.." + num + ")";
			if (num == -1)
			{
				text = "(no sheets)";
			}
			throw new ArgumentException("Sheet index (" + index + ") is out of range " + text);
		}
	}

	public void InsertChartRecord()
	{
		int index = workbook.FindFirstRecordLocBySid(252);
		byte[] data = new byte[90]
		{
			15, 0, 0, 240, 82, 0, 0, 0, 0, 0,
			6, 240, 24, 0, 0, 0, 1, 8, 0, 0,
			2, 0, 0, 0, 2, 0, 0, 0, 1, 0,
			0, 0, 1, 0, 0, 0, 3, 0, 0, 0,
			51, 0, 11, 240, 18, 0, 0, 0, 191, 0,
			8, 0, 8, 0, 129, 1, 9, 0, 0, 8,
			192, 1, 64, 0, 0, 8, 64, 0, 30, 241,
			16, 0, 0, 0, 13, 0, 0, 8, 12, 0,
			0, 8, 23, 0, 0, 8, 247, 0, 0, 16
		};
		UnknownRecord value = new UnknownRecord(235, data);
		workbook.Records.Insert(index, value);
	}

	public void SetSelectedTab(int index)
	{
		ValidateSheetIndex(index);
		int count = _sheets.Count;
		for (int i = 0; i < count; i++)
		{
			GetSheetAt(i).IsSelected = i == index;
		}
		workbook.WindowOne.NumSelectedTabs = 1;
	}

	public void SetSelectedTabs(int[] indexes)
	{
		IList<int> selectedTabs = new List<int>(indexes);
		SetSelectedTabs(selectedTabs);
	}

	public void SetSelectedTabs(IList<int> indexes)
	{
		foreach (int index in indexes)
		{
			ValidateSheetIndex(index);
		}
		ISet<int> set = new HashSet<int>(indexes);
		int count = _sheets.Count;
		for (int i = 0; i < count; i++)
		{
			bool isSelected = set.Contains(i);
			GetSheetAt(i).IsSelected = isSelected;
		}
		workbook.WindowOne.NumSelectedTabs = (short)indexes.Count;
	}

	public IList<int> GetSelectedTabs()
	{
		List<int> list = new List<int>();
		int count = _sheets.Count;
		for (int i = 0; i < count; i++)
		{
			if ((GetSheetAt(i) as HSSFSheet).IsSelected)
			{
				list.Add(i);
			}
		}
		return list.AsReadOnly();
	}

	public void SetActiveSheet(int index)
	{
		ValidateSheetIndex(index);
		int count = _sheets.Count;
		for (int i = 0; i < count; i++)
		{
			GetSheetAt(i).SetActive(i == index);
		}
		workbook.WindowOne.ActiveSheetIndex = index;
	}

	public void SetSheetName(int sheetIx, string name)
	{
		if (name == null)
		{
			throw new ArgumentException("sheetName must not be null");
		}
		if (workbook.ContainsSheetName(name, sheetIx))
		{
			throw new ArgumentException("The workbook already contains a sheet named '" + name + "'");
		}
		ValidateSheetIndex(sheetIx);
		workbook.SetSheetName(sheetIx, name);
	}

	public string GetSheetName(int sheetIx)
	{
		ValidateSheetIndex(sheetIx);
		return workbook.GetSheetName(sheetIx);
	}

	public bool IsSheetHidden(int sheetIx)
	{
		ValidateSheetIndex(sheetIx);
		return workbook.IsSheetHidden(sheetIx);
	}

	public bool IsSheetVeryHidden(int sheetIx)
	{
		ValidateSheetIndex(sheetIx);
		return workbook.IsSheetVeryHidden(sheetIx);
	}

	public void SetSheetHidden(int sheetIx, SheetState hidden)
	{
		ValidateSheetIndex(sheetIx);
		WorkbookUtil.ValidateSheetState(hidden);
		workbook.SetSheetHidden(sheetIx, (int)hidden);
	}

	public void SetSheetHidden(int sheetIx, int hidden)
	{
		ValidateSheetIndex(sheetIx);
		workbook.SetSheetHidden(sheetIx, hidden);
	}

	public void SetSheetHidden(int sheetIx, bool hidden)
	{
		ValidateSheetIndex(sheetIx);
		workbook.SetSheetHidden(sheetIx, hidden);
	}

	public int GetSheetIndex(string name)
	{
		return workbook.GetSheetIndex(name);
	}

	public int GetSheetIndex(ISheet sheet)
	{
		for (int i = 0; i < _sheets.Count; i++)
		{
			if (_sheets[i] == sheet)
			{
				return i;
			}
		}
		return -1;
	}

	public ISheet CreateSheet()
	{
		HSSFSheet hSSFSheet = new HSSFSheet(this);
		_sheets.Add(hSSFSheet);
		workbook.SetSheetName(_sheets.Count - 1, "Sheet" + (_sheets.Count - 1));
		bool isActive = (hSSFSheet.IsSelected = _sheets.Count == 1);
		hSSFSheet.IsActive = isActive;
		return hSSFSheet;
	}

	public ISheet CloneSheet(int sheetIndex)
	{
		ValidateSheetIndex(sheetIndex);
		HSSFSheet hSSFSheet = _sheets[sheetIndex];
		string sheetName = workbook.GetSheetName(sheetIndex);
		ISheet sheet = hSSFSheet.CloneSheet(this);
		sheet.IsSelected = false;
		sheet.IsActive = false;
		string uniqueSheetName = GetUniqueSheetName(sheetName);
		int count = _sheets.Count;
		_sheets.Add((HSSFSheet)sheet);
		workbook.SetSheetName(count, uniqueSheetName);
		int num = FindExistingBuiltinNameRecordIdx(sheetIndex, 13);
		if (num != -1)
		{
			NameRecord name = workbook.CloneFilter(num, count);
			HSSFName item = new HSSFName(this, name);
			names.Add(item);
		}
		return sheet;
	}

	private string GetUniqueSheetName(string srcName)
	{
		int num = 2;
		string text = srcName;
		int num2 = srcName.LastIndexOf('(');
		if (num2 > 0 && srcName.EndsWith(")", StringComparison.Ordinal))
		{
			string text2 = srcName.Substring(num2 + 1, srcName.Length - num2 - 2);
			try
			{
				num = int.Parse(text2.Trim(), CultureInfo.InvariantCulture);
				num++;
				text = srcName.Substring(0, num2).Trim();
			}
			catch (FormatException)
			{
			}
		}
		string text4;
		do
		{
			string text3 = num++.ToString(CultureInfo.CurrentCulture);
			text4 = ((text.Length + text3.Length + 2 >= 31) ? (text.Substring(0, 31 - text3.Length - 2) + "(" + text3 + ")") : (text + " (" + text3 + ")"));
		}
		while (workbook.GetSheetIndex(text4) != -1);
		return text4;
	}

	public ISheet CreateSheet(string sheetname)
	{
		if (sheetname == null)
		{
			throw new ArgumentException("sheetName must not be null");
		}
		if (workbook.ContainsSheetName(sheetname, _sheets.Count))
		{
			throw new ArgumentException("The workbook already contains a sheet named '" + sheetname + "'");
		}
		WorkbookUtil.ValidateSheetName(sheetname);
		HSSFSheet hSSFSheet = new HSSFSheet(this);
		workbook.SetSheetName(_sheets.Count, sheetname);
		_sheets.Add(hSSFSheet);
		bool isActive = (hSSFSheet.IsSelected = _sheets.Count == 1);
		hSSFSheet.IsActive = isActive;
		return hSSFSheet;
	}

	private List<HSSFSheet> GetSheets()
	{
		return _sheets;
	}

	public ISheet GetSheetAt(int index)
	{
		return _sheets[index];
	}

	public ISheet GetSheet(string name)
	{
		HSSFSheet result = null;
		for (int i = 0; i < _sheets.Count; i++)
		{
			if (workbook.GetSheetName(i).Equals(name, StringComparison.OrdinalIgnoreCase))
			{
				result = _sheets[i];
				break;
			}
		}
		return result;
	}

	public void RemoveSheetAt(int index)
	{
		ValidateSheetIndex(index);
		bool isSelected = GetSheetAt(index).IsSelected;
		_sheets.RemoveAt(index);
		workbook.RemoveSheet(index);
		int count = _sheets.Count;
		if (count < 1)
		{
			return;
		}
		int num = index;
		if (num >= count)
		{
			num = count - 1;
		}
		if (isSelected)
		{
			bool flag = false;
			for (int i = 0; i < count; i++)
			{
				if (GetSheetAt(i).IsSelected)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				SetSelectedTab(num);
			}
		}
		int activeSheetIndex = ActiveSheetIndex;
		if (activeSheetIndex == index)
		{
			SetActiveSheet(num);
		}
		else if (activeSheetIndex > index)
		{
			SetActiveSheet(activeSheetIndex - 1);
		}
	}

	internal int FindExistingBuiltinNameRecordIdx(int sheetIndex, byte builtinCode)
	{
		for (int i = 0; i < names.Count; i++)
		{
			NameRecord nameRecord = workbook.GetNameRecord(i);
			if (nameRecord == null)
			{
				throw new InvalidOperationException("Unable to find all defined names to iterate over");
			}
			if (nameRecord.IsBuiltInName && nameRecord.BuiltInName == builtinCode && nameRecord.SheetNumber - 1 == sheetIndex)
			{
				return i;
			}
		}
		return -1;
	}

	internal HSSFName CreateBuiltInName(byte builtinCode, int sheetIndex)
	{
		NameRecord name = workbook.CreateBuiltInName(builtinCode, sheetIndex + 1);
		HSSFName hSSFName = new HSSFName(this, name, null);
		names.Add(hSSFName);
		return hSSFName;
	}

	internal HSSFName GetBuiltInName(byte builtinCode, int sheetIndex)
	{
		int num = FindExistingBuiltinNameRecordIdx(sheetIndex, builtinCode);
		if (num < 0)
		{
			return null;
		}
		return names[num];
	}

	private bool IsRowColHeaderRecord(NameRecord r)
	{
		if (r.OptionFlag == 32)
		{
			return "\a".Equals(r.NameText);
		}
		return false;
	}

	public IFont CreateFont()
	{
		workbook.CreateNewFont();
		short num = (short)(NumberOfFonts - 1);
		if (num > 3)
		{
			num++;
		}
		if (num == short.MaxValue)
		{
			throw new ArgumentException("Maximum number of fonts was exceeded");
		}
		return GetFontAt(num);
	}

	[Obsolete("deprecated 3.15 beta 2. Use {@link #findFont(boolean, short, short, String, boolean, boolean, short, byte)} instead.")]
	public IFont FindFont(short boldWeight, short color, short fontHeight, string name, bool italic, bool strikeout, FontSuperScript typeOffset, FontUnderlineType underline)
	{
		short numberOfFonts = NumberOfFonts;
		for (short num = 0; num <= numberOfFonts; num++)
		{
			if (num != 4)
			{
				IFont fontAt = GetFontAt(num);
				if (fontAt.Boldweight == boldWeight && fontAt.Color == color && fontAt.FontHeight == (double)fontHeight && fontAt.FontName.Equals(name) && fontAt.IsItalic == italic && fontAt.IsStrikeout == strikeout && fontAt.TypeOffset == typeOffset && fontAt.Underline == underline)
				{
					return fontAt;
				}
			}
		}
		return null;
	}

	public IFont FindFont(bool bold, short color, short fontHeight, string name, bool italic, bool strikeout, FontSuperScript typeOffset, FontUnderlineType underline)
	{
		short numberOfFonts = NumberOfFonts;
		for (short num = 0; num <= numberOfFonts; num++)
		{
			if (num != 4)
			{
				HSSFFont hSSFFont = GetFontAt(num) as HSSFFont;
				if (hSSFFont.IsBold == bold && hSSFFont.Color == color && hSSFFont.FontHeight == (double)fontHeight && hSSFFont.FontName.Equals(name) && hSSFFont.IsItalic == italic && hSSFFont.IsStrikeout == strikeout && hSSFFont.TypeOffset == typeOffset && hSSFFont.Underline == underline)
				{
					return hSSFFont;
				}
			}
		}
		return null;
	}

	public IFont GetFontAt(short idx)
	{
		if (fonts == null)
		{
			fonts = new Dictionary<short, HSSFFont>();
		}
		if (fonts.ContainsKey(idx))
		{
			return fonts[idx];
		}
		FontRecord fontRecordAt = workbook.GetFontRecordAt(idx);
		HSSFFont hSSFFont = new HSSFFont(idx, fontRecordAt);
		fonts[idx] = hSSFFont;
		return hSSFFont;
	}

	public void ResetFontCache()
	{
		fonts = new Dictionary<short, HSSFFont>();
	}

	public ICellStyle CreateCellStyle()
	{
		if (workbook.NumExFormats == 4030)
		{
			throw new InvalidOperationException("The maximum number of cell styles was exceeded. You can define up to 4000 styles in a .xls workbook");
		}
		ExtendedFormatRecord rec = workbook.CreateCellXF();
		return new HSSFCellStyle((short)(NumCellStyles - 1), rec, this);
	}

	public ICellStyle GetCellStyleAt(int idx)
	{
		ExtendedFormatRecord exFormatAt = workbook.GetExFormatAt(idx);
		return new HSSFCellStyle((short)idx, exFormatAt, this);
	}

	public override void Close()
	{
		base.Close();
	}

	public override void Write()
	{
		ValidateInPlaceWritePossible();
		new NPOIFSDocument((DocumentNode)directory.GetEntry(GetWorkbookDirEntryName(directory))).ReplaceContents(new ByteArrayInputStream(GetBytes()));
		WriteProperties();
		directory.FileSystem.WriteFileSystem();
	}

	public override void Write(FileInfo newFile)
	{
		POIFSFileSystem pOIFSFileSystem = POIFSFileSystem.Create(newFile);
		try
		{
			Write(pOIFSFileSystem);
			pOIFSFileSystem.WriteFileSystem();
		}
		finally
		{
			pOIFSFileSystem.Close();
		}
	}

	public override void Write(Stream stream)
	{
		NPOIFSFileSystem nPOIFSFileSystem = new NPOIFSFileSystem();
		try
		{
			Write(nPOIFSFileSystem);
			nPOIFSFileSystem.WriteFileSystem(stream);
		}
		finally
		{
			nPOIFSFileSystem.Close();
		}
	}

	private void Write(NPOIFSFileSystem fs)
	{
		List<string> list = new List<string>(1);
		using MemoryStream stream = new MemoryStream(GetBytes());
		fs.CreateDocument(stream, "Workbook");
		WriteProperties(fs, list);
		if (preserveNodes)
		{
			list.AddRange(InternalWorkbook.WORKBOOK_DIR_ENTRY_NAMES);
			EntryUtils.CopyNodes(new FilteringDirectoryNode(directory, list), new FilteringDirectoryNode(fs.Root, list));
			fs.Root.StorageClsid = directory.StorageClsid;
		}
	}

	public byte[] GetBytes()
	{
		List<HSSFSheet> sheets = GetSheets();
		int count = sheets.Count;
		workbook.PreSerialize();
		foreach (HSSFSheet item in sheets)
		{
			item.Sheet.Preserialize();
			item.PreSerialize();
		}
		int num = workbook.Size;
		SheetRecordCollector[] array = new SheetRecordCollector[count];
		for (int i = 0; i < count; i++)
		{
			workbook.SetSheetBof(i, num);
			using SheetRecordCollector sheetRecordCollector = new SheetRecordCollector();
			sheets[i].Sheet.VisitContainedRecords(sheetRecordCollector, num);
			num += sheetRecordCollector.TotalSize;
			array[i] = sheetRecordCollector;
		}
		byte[] array2 = new byte[num];
		int num2 = workbook.Serialize(0, array2);
		for (int j = 0; j < count; j++)
		{
			SheetRecordCollector sheetRecordCollector2 = array[j];
			int num3 = sheetRecordCollector2.Serialize(num2, array2);
			if (num3 != sheetRecordCollector2.TotalSize)
			{
				throw new InvalidOperationException("Actual serialized sheet size (" + num3 + ") differs from pre-calculated size (" + sheetRecordCollector2.TotalSize + ") for sheet (" + j + ")");
			}
			num2 += num3;
			sheetRecordCollector2.Dispose();
		}
		return array2;
	}

	public void AddToolPack(UDFFinder toopack)
	{
		((AggregatingUDFFinder)_udfFinder).Add(toopack);
	}

	internal UDFFinder GetUDFFinder()
	{
		return _udfFinder;
	}

	public IName GetName(string name)
	{
		int nameIndex = GetNameIndex(name);
		if (nameIndex < 0)
		{
			return null;
		}
		return names[nameIndex];
	}

	public IList<IName> GetNames(string name)
	{
		List<IName> list = new List<IName>();
		foreach (HSSFName name2 in names)
		{
			if (name2.NameName.Equals(name))
			{
				list.Add(name2);
			}
		}
		return list;
	}

	public IName GetNameAt(int nameIndex)
	{
		int count = names.Count;
		if (count < 1)
		{
			throw new InvalidOperationException("There are no defined names in this workbook");
		}
		if (nameIndex < 0 || nameIndex > count)
		{
			throw new ArgumentOutOfRangeException("Specified name index " + nameIndex + " is outside the allowable range (0.." + (count - 1) + ").");
		}
		return names[nameIndex];
	}

	public IList<IName> GetAllNames()
	{
		List<IName> list = new List<IName>();
		list.AddRange(names);
		return list.AsReadOnly();
	}

	public string GetNameName(int index)
	{
		return GetNameAt(index).NameName;
	}

	public NameRecord GetNameRecord(int nameIndex)
	{
		return Workbook.GetNameRecord(nameIndex);
	}

	public void SetPrintArea(int sheetIndex, string reference)
	{
		NameRecord nameRecord = workbook.GetSpecificBuiltinRecord(6, sheetIndex + 1);
		if (nameRecord == null)
		{
			nameRecord = workbook.CreateBuiltInName(6, sheetIndex + 1);
		}
		string[] array = reference.Split(new char[1] { ',' });
		StringBuilder stringBuilder = new StringBuilder(32);
		for (int i = 0; i < array.Length; i++)
		{
			if (i > 0)
			{
				stringBuilder.Append(",");
			}
			SheetNameFormatter.AppendFormat(stringBuilder, GetSheetName(sheetIndex));
			stringBuilder.Append("!");
			stringBuilder.Append(array[i]);
		}
		nameRecord.NameDefinition = HSSFFormulaParser.Parse(stringBuilder.ToString(), this, FormulaType.NamedRange, sheetIndex);
	}

	public void SetPrintArea(int sheetIndex, int startColumn, int endColumn, int startRow, int endRow)
	{
		CellReference cellReference = new CellReference(startRow, startColumn, pAbsRow: true, pAbsCol: true);
		string text = cellReference.FormatAsString();
		cellReference = new CellReference(endRow, endColumn, pAbsRow: true, pAbsCol: true);
		text = text + ":" + cellReference.FormatAsString();
		SetPrintArea(sheetIndex, text);
	}

	public string GetPrintArea(int sheetIndex)
	{
		NameRecord specificBuiltinRecord = workbook.GetSpecificBuiltinRecord(6, sheetIndex + 1);
		if (specificBuiltinRecord == null)
		{
			return null;
		}
		return HSSFFormulaParser.ToFormulaString(this, specificBuiltinRecord.NameDefinition);
	}

	public void RemovePrintArea(int sheetIndex)
	{
		Workbook.RemoveBuiltinRecord(6, sheetIndex + 1);
	}

	public IName CreateName()
	{
		NameRecord name = workbook.CreateName();
		HSSFName hSSFName = new HSSFName(this, name);
		names.Add(hSSFName);
		return hSSFName;
	}

	public int GetNameIndex(string name)
	{
		int result = -1;
		for (int i = 0; i < names.Count; i++)
		{
			if (GetNameName(i).Equals(name, StringComparison.OrdinalIgnoreCase))
			{
				result = i;
				break;
			}
		}
		return result;
	}

	public int GetNameIndex(HSSFName name)
	{
		for (int i = 0; i < names.Count; i++)
		{
			if (name == names[i])
			{
				return i;
			}
		}
		return -1;
	}

	public void RemoveName(int index)
	{
		names.RemoveAt(index);
		workbook.RemoveName(index);
	}

	public IDataFormat CreateDataFormat()
	{
		if (formatter == null)
		{
			formatter = new HSSFDataFormat(workbook);
		}
		return formatter;
	}

	public void RemoveName(string name)
	{
		int nameIndex = GetNameIndex(name);
		RemoveName(nameIndex);
	}

	public void RemoveName(IName name)
	{
		int nameIndex = GetNameIndex((HSSFName)name);
		RemoveName(nameIndex);
	}

	public HSSFPalette GetCustomPalette()
	{
		return new HSSFPalette(workbook.CustomPalette);
	}

	public void DumpDrawingGroupRecords(bool fat)
	{
		DrawingGroupRecord obj = (DrawingGroupRecord)workbook.FindFirstRecordBySid(235);
		obj.Decode();
		foreach (EscherRecord item in (IEnumerable)obj.EscherRecords)
		{
			if (fat)
			{
				Console.WriteLine(item.ToString());
			}
			else
			{
				item.Display(0);
			}
		}
	}

	internal void InitDrawings()
	{
		if (workbook.FindDrawingGroup() != null)
		{
			foreach (HSSFSheet sheet in _sheets)
			{
				_ = sheet.DrawingPatriarch;
			}
			return;
		}
		workbook.CreateDrawingGroup();
	}

	public int AddPicture(byte[] pictureData, PictureType format)
	{
		InitDrawings();
		byte[] uID;
		using (MD5 mD = MD5.Create())
		{
			uID = mD.ComputeHash(pictureData);
		}
		EscherBlipRecord escherBlipRecord2;
		int size;
		short tag;
		switch (format)
		{
		case PictureType.WMF:
		{
			if (LittleEndian.GetInt(pictureData) == -1698247209)
			{
				byte[] array = new byte[pictureData.Length - 22];
				Array.Copy(pictureData, 22, array, 0, pictureData.Length - 22);
				pictureData = array;
			}
			EscherBlipRecord escherBlipRecord4 = (escherBlipRecord2 = new EscherMetafileBlip());
			((EscherMetafileBlip)escherBlipRecord4).UID = uID;
			((EscherMetafileBlip)escherBlipRecord4).SetPictureData(pictureData);
			((EscherMetafileBlip)escherBlipRecord4).Filter = 254;
			size = ((EscherMetafileBlip)escherBlipRecord4).CompressedSize + 58;
			tag = 0;
			break;
		}
		case PictureType.EMF:
		{
			EscherBlipRecord escherBlipRecord3 = (escherBlipRecord2 = new EscherMetafileBlip());
			((EscherMetafileBlip)escherBlipRecord3).UID = uID;
			((EscherMetafileBlip)escherBlipRecord3).SetPictureData(pictureData);
			((EscherMetafileBlip)escherBlipRecord3).Filter = 254;
			size = ((EscherMetafileBlip)escherBlipRecord3).CompressedSize + 58;
			tag = 0;
			break;
		}
		default:
		{
			EscherBlipRecord escherBlipRecord = (escherBlipRecord2 = new EscherBitmapBlip());
			((EscherBitmapBlip)escherBlipRecord).UID = uID;
			((EscherBitmapBlip)escherBlipRecord).Marker = byte.MaxValue;
			escherBlipRecord.PictureData = pictureData;
			size = pictureData.Length + 25;
			tag = 255;
			break;
		}
		}
		escherBlipRecord2.RecordId = (short)(-4072 + format);
		switch (format)
		{
		case PictureType.EMF:
			escherBlipRecord2.Options = 15680;
			break;
		case PictureType.WMF:
			escherBlipRecord2.Options = 8544;
			break;
		case PictureType.PICT:
			escherBlipRecord2.Options = 21536;
			break;
		case PictureType.PNG:
			escherBlipRecord2.Options = 28160;
			break;
		case PictureType.JPEG:
			escherBlipRecord2.Options = 18080;
			break;
		case PictureType.DIB:
			escherBlipRecord2.Options = 31360;
			break;
		default:
			throw new InvalidOperationException("Unexpected picture format: " + format);
		}
		EscherBSERecord escherBSERecord = new EscherBSERecord();
		escherBSERecord.RecordId = -4089;
		escherBSERecord.Options = (short)(2 | ((int)format << 4));
		escherBSERecord.BlipTypeMacOS = (byte)format;
		escherBSERecord.BlipTypeWin32 = (byte)format;
		escherBSERecord.UID = uID;
		escherBSERecord.Tag = tag;
		escherBSERecord.Size = size;
		escherBSERecord.Ref = 0;
		escherBSERecord.Offset = 0;
		escherBSERecord.BlipRecord = escherBlipRecord2;
		return workbook.AddBSERecord(escherBSERecord);
	}

	public IList GetAllPictures()
	{
		List<HSSFPictureData> list = new List<HSSFPictureData>();
		foreach (NPOI.HSSF.Record.Record record in workbook.Records)
		{
			if (record is AbstractEscherHolderRecord)
			{
				((AbstractEscherHolderRecord)record).Decode();
				IList escherRecords = ((AbstractEscherHolderRecord)record).EscherRecords;
				SearchForPictures(escherRecords, list);
			}
		}
		return list;
	}

	private void SearchForPictures(IList escherRecords, List<HSSFPictureData> pictures)
	{
		IEnumerator enumerator = escherRecords.GetEnumerator();
		while (enumerator.MoveNext())
		{
			object current = enumerator.Current;
			if (!(current is EscherRecord))
			{
				continue;
			}
			EscherRecord escherRecord = (EscherRecord)current;
			if (escherRecord is EscherBSERecord)
			{
				EscherBlipRecord blipRecord = ((EscherBSERecord)escherRecord).BlipRecord;
				if (blipRecord != null)
				{
					pictures.Add(new HSSFPictureData(blipRecord));
				}
			}
			SearchForPictures(escherRecord.ChildRecords, pictures);
		}
	}

	protected static Dictionary<string, ClassID> GetOleMap()
	{
		Dictionary<string, ClassID> dictionary = new Dictionary<string, ClassID>();
		dictionary.Add("PowerPoint Document", ClassID.PPT_SHOW);
		string[] wORKBOOK_DIR_ENTRY_NAMES = InternalWorkbook.WORKBOOK_DIR_ENTRY_NAMES;
		foreach (string key in wORKBOOK_DIR_ENTRY_NAMES)
		{
			dictionary.Add(key, ClassID.XLS_WORKBOOK);
		}
		return dictionary;
	}

	public int AddOlePackage(POIFSFileSystem poiData, string label, string fileName, string command)
	{
		DirectoryNode root = poiData.Root;
		foreach (KeyValuePair<string, ClassID> item in GetOleMap())
		{
			if (root.HasEntry(item.Key))
			{
				root.StorageClsid = item.Value;
				break;
			}
		}
		MemoryStream memoryStream = new MemoryStream();
		poiData.WriteFileSystem(memoryStream);
		return AddOlePackage(memoryStream.ToArray(), label, fileName, command);
	}

	public int AddOlePackage(byte[] oleData, string label, string fileName, string command)
	{
		if (directory == null)
		{
			directory = new POIFSFileSystem().Root;
			preserveNodes = true;
		}
		int num = 0;
		DirectoryEntry directoryEntry = null;
		do
		{
			string name = "MBD" + HexDump.ToHex(++num);
			if (!directory.HasEntry(name))
			{
				directoryEntry = directory.CreateDirectory(name);
				directoryEntry.StorageClsid = ClassID.OLE10_PACKAGE;
			}
		}
		while (directoryEntry == null);
		byte[] buffer = new byte[20]
		{
			1, 0, 0, 2, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0
		};
		directoryEntry.CreateDocument("\u0001Ole", new MemoryStream(buffer));
		Ole10Native ole10Native = new Ole10Native(label, fileName, command, oleData);
		MemoryStream memoryStream = new MemoryStream();
		ole10Native.WriteOut(memoryStream);
		directoryEntry.CreateDocument(Ole10Native.OLE10_NATIVE, new MemoryStream(memoryStream.ToArray()));
		return num;
	}

	public int LinkExternalWorkbook(string name, IWorkbook workbook)
	{
		return this.workbook.LinkExternalWorkbook(name, workbook);
	}

	public void WriteProtectWorkbook(string password, string username)
	{
		workbook.WriteProtectWorkbook(password, username);
	}

	public void UnwriteProtectWorkbook()
	{
		workbook.UnwriteProtectWorkbook();
	}

	public IList<HSSFObjectData> GetAllEmbeddedObjects()
	{
		List<HSSFObjectData> list = new List<HSSFObjectData>();
		foreach (HSSFSheet sheet in _sheets)
		{
			GetAllEmbeddedObjects(sheet, list);
		}
		return list;
	}

	private void GetAllEmbeddedObjects(HSSFSheet sheet, List<HSSFObjectData> objects)
	{
		if (sheet.DrawingPatriarch is HSSFPatriarch parent)
		{
			GetAllEmbeddedObjects(parent, objects);
		}
	}

	private void GetAllEmbeddedObjects(HSSFShapeContainer parent, List<HSSFObjectData> objects)
	{
		foreach (HSSFShape child in parent.Children)
		{
			if (child is HSSFObjectData)
			{
				objects.Add((HSSFObjectData)child);
			}
			else if (child is HSSFShapeContainer)
			{
				GetAllEmbeddedObjects((HSSFShapeContainer)child, objects);
			}
		}
	}

	public IEnumerator<ISheet> GetEnumerator()
	{
		return _sheets.GetEnumerator();
	}

	public bool ChangeExternalReference(string oldUrl, string newUrl)
	{
		return workbook.ChangeExternalReference(oldUrl, newUrl);
	}

	public int IndexOf(ISheet item)
	{
		throw new NotImplementedException();
	}

	public void Insert(int index, ISheet item)
	{
		_sheets.Insert(index, (HSSFSheet)item);
	}

	public void RemoveAt(int index)
	{
		_sheets.RemoveAt(index);
	}

	public void Add(ISheet item)
	{
		_sheets.Add((HSSFSheet)item);
	}

	public void Clear()
	{
		_sheets.Clear();
	}

	public bool Contains(ISheet item)
	{
		throw new NotImplementedException();
	}

	public void CopyTo(ISheet[] array, int arrayIndex)
	{
		throw new NotImplementedException();
	}

	public bool Remove(ISheet item)
	{
		return _sheets.Remove((HSSFSheet)item);
	}

	public bool IsDate1904()
	{
		return Workbook.IsUsing1904DateWindowing;
	}
}

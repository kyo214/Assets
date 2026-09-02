using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Threading;
using NPOI.DDF;
using NPOI.HSSF.Record;
using NPOI.POIFS.Crypt;
using NPOI.SS.Formula;
using NPOI.SS.Formula.PTG;
using NPOI.SS.Formula.UDF;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.Model;

[Serializable]
public class InternalWorkbook
{
	private const int MAX_SENSITIVE_SHEET_NAME_LEN = 31;

	public static readonly string[] WORKBOOK_DIR_ENTRY_NAMES = new string[3] { "Workbook", "WORKBOOK", "BOOK" };

	public static string OLD_WORKBOOK_DIR_ENTRY_NAME = "Book";

	private const short CODEPAGE = 1200;

	[NonSerialized]
	protected WorkbookRecordList records = new WorkbookRecordList();

	[NonSerialized]
	protected SSTRecord sst;

	[NonSerialized]
	private LinkTable linkTable;

	protected List<BoundSheetRecord> boundsheets;

	protected List<FormatRecord> formats;

	protected List<HyperlinkRecord> hyperlinks;

	protected int numxfs;

	protected int numfonts;

	private int maxformatid = -1;

	private bool uses1904datewindowing;

	[NonSerialized]
	private DrawingManager2 drawingManager;

	private IList escherBSERecords;

	[NonSerialized]
	private WindowOneRecord windowOne;

	[NonSerialized]
	private FileSharingRecord fileShare;

	[NonSerialized]
	private WriteAccessRecord writeAccess;

	[NonSerialized]
	private WriteProtectRecord writeProtect;

	private Dictionary<string, NameCommentRecord> commentRecords;

	public int NumRecords => records.Count;

	public int NumberOfFontRecords => numfonts;

	public BackupRecord BackupRecord => (BackupRecord)records[records.Backuppos];

	public int NumSheets => boundsheets.Count;

	public int NumExFormats => numxfs;

	public int Size
	{
		get
		{
			int num = 0;
			SSTRecord sSTRecord = null;
			for (int i = 0; i < records.Count; i++)
			{
				NPOI.HSSF.Record.Record record = records[i];
				if (record.Sid != 449 || ((RecalcIdRecord)record).IsNeeded)
				{
					if (record is SSTRecord)
					{
						sSTRecord = (SSTRecord)record;
					}
					num = ((record.Sid != 255 || sSTRecord == null) ? (num + record.RecordSize) : (num + sSTRecord.CalcExtSSTRecordSize()));
				}
			}
			return num;
		}
	}

	private LinkTable OrCreateLinkTable => GetOrCreateLinkTable();

	public int NumNames
	{
		get
		{
			if (linkTable == null)
			{
				return 0;
			}
			return linkTable.NumNames;
		}
	}

	public List<FormatRecord> Formats => formats;

	public IList Hyperlinks => hyperlinks;

	public IList Records => records.Records;

	public bool IsUsing1904DateWindowing => uses1904datewindowing;

	public PaletteRecord CustomPalette
	{
		get
		{
			int palettepos = records.Palettepos;
			PaletteRecord paletteRecord;
			if (palettepos != -1)
			{
				NPOI.HSSF.Record.Record record = records[palettepos];
				if (!(record is PaletteRecord))
				{
					throw new Exception("InternalError: Expected PaletteRecord but got a '" + record?.ToString() + "'");
				}
				paletteRecord = (PaletteRecord)record;
			}
			else
			{
				paletteRecord = CreatePalette();
				records.Add(1, paletteRecord);
				records.Palettepos = 1;
			}
			return paletteRecord;
		}
	}

	public WindowOneRecord WindowOne => windowOne;

	public DrawingManager2 DrawingManager => drawingManager;

	public WriteProtectRecord WriteProtect
	{
		get
		{
			if (writeProtect == null)
			{
				writeProtect = new WriteProtectRecord();
				int num = 0;
				for (num = 0; num < records.Count && !(records[num] is BOFRecord); num++)
				{
				}
				records.Add(num + 1, writeProtect);
			}
			return writeProtect;
		}
	}

	public WriteAccessRecord WriteAccess
	{
		get
		{
			if (writeAccess == null)
			{
				writeAccess = (WriteAccessRecord)CreateWriteAccess();
				int num = 0;
				for (num = 0; num < records.Count && !(records[num] is InterfaceEndRecord); num++)
				{
				}
				records.Add(num + 1, writeAccess);
			}
			return writeAccess;
		}
	}

	public FileSharingRecord FileSharing
	{
		get
		{
			if (fileShare == null)
			{
				fileShare = new FileSharingRecord();
				int num = 0;
				for (num = 0; num < records.Count && !(records[num] is WriteAccessRecord); num++)
				{
				}
				records.Add(num + 1, fileShare);
			}
			return fileShare;
		}
	}

	public bool IsWriteProtected
	{
		get
		{
			if (fileShare == null)
			{
				return false;
			}
			return FileSharing.ReadOnly == 1;
		}
	}

	public RecalcIdRecord RecalcId
	{
		get
		{
			RecalcIdRecord recalcIdRecord = (RecalcIdRecord)FindFirstRecordBySid(449);
			if (recalcIdRecord == null)
			{
				recalcIdRecord = new RecalcIdRecord();
				int num = FindFirstRecordLocBySid(140);
				records.Add(num + 1, recalcIdRecord);
			}
			return recalcIdRecord;
		}
	}

	public InternalWorkbook()
	{
		records = new WorkbookRecordList();
		boundsheets = new List<BoundSheetRecord>();
		formats = new List<FormatRecord>();
		hyperlinks = new List<HyperlinkRecord>();
		numxfs = 0;
		numfonts = 0;
		maxformatid = -1;
		uses1904datewindowing = false;
		escherBSERecords = new List<EscherBSERecord>();
		commentRecords = new Dictionary<string, NameCommentRecord>();
	}

	public static InternalWorkbook CreateWorkbook(List<NPOI.HSSF.Record.Record> recs)
	{
		InternalWorkbook internalWorkbook = new InternalWorkbook();
		List<NPOI.HSSF.Record.Record> list = new List<NPOI.HSSF.Record.Record>(recs.Count / 3);
		internalWorkbook.records.Records = list;
		int i;
		for (i = 0; i < recs.Count; i++)
		{
			NPOI.HSSF.Record.Record record = recs[i];
			if (record.Sid == 10)
			{
				list.Add(record);
				break;
			}
			switch (record.Sid)
			{
			case 133:
				internalWorkbook.boundsheets.Add((BoundSheetRecord)record);
				internalWorkbook.records.Bspos = i;
				break;
			case 252:
				internalWorkbook.sst = (SSTRecord)record;
				break;
			case 49:
				internalWorkbook.records.Fontpos = i;
				internalWorkbook.numfonts++;
				break;
			case 224:
				internalWorkbook.records.Xfpos = i;
				internalWorkbook.numxfs++;
				break;
			case 317:
				internalWorkbook.records.Tabpos = i;
				break;
			case 18:
				internalWorkbook.records.Protpos = i;
				break;
			case 64:
				internalWorkbook.records.Backuppos = i;
				break;
			case 23:
				throw new Exception("Extern sheet is part of LinkTable");
			case 24:
			case 430:
				internalWorkbook.linkTable = new LinkTable(recs, i, internalWorkbook.records, internalWorkbook.commentRecords);
				i += internalWorkbook.linkTable.RecordCount - 1;
				continue;
			case 1054:
				internalWorkbook.formats.Add((FormatRecord)record);
				internalWorkbook.maxformatid = ((internalWorkbook.maxformatid >= ((FormatRecord)record).IndexCode) ? internalWorkbook.maxformatid : ((FormatRecord)record).IndexCode);
				break;
			case 34:
				internalWorkbook.uses1904datewindowing = ((DateWindow1904Record)record).Windowing == 1;
				break;
			case 146:
				internalWorkbook.records.Palettepos = i;
				break;
			case 61:
				internalWorkbook.windowOne = (WindowOneRecord)record;
				break;
			case 92:
				internalWorkbook.writeAccess = (WriteAccessRecord)record;
				break;
			case 134:
				internalWorkbook.writeProtect = (WriteProtectRecord)record;
				break;
			case 91:
				internalWorkbook.fileShare = (FileSharingRecord)record;
				break;
			case 2196:
			{
				NameCommentRecord nameCommentRecord = (NameCommentRecord)record;
				internalWorkbook.commentRecords[nameCommentRecord.NameText] = nameCommentRecord;
				break;
			}
			}
			list.Add(record);
		}
		for (; i < recs.Count; i++)
		{
			NPOI.HSSF.Record.Record record2 = recs[i];
			if (record2.Sid == 440)
			{
				internalWorkbook.hyperlinks.Add((HyperlinkRecord)record2);
			}
		}
		if (internalWorkbook.windowOne == null)
		{
			internalWorkbook.windowOne = (WindowOneRecord)CreateWindowOne();
		}
		return internalWorkbook;
	}

	public NameCommentRecord GetNameCommentRecord(NameRecord nameRecord)
	{
		if (commentRecords.ContainsKey(nameRecord.NameText))
		{
			return commentRecords[nameRecord.NameText];
		}
		return null;
	}

	public static InternalWorkbook CreateWorkbook()
	{
		InternalWorkbook internalWorkbook = new InternalWorkbook();
		List<NPOI.HSSF.Record.Record> list = new List<NPOI.HSSF.Record.Record>(30);
		internalWorkbook.records.Records = list;
		List<FormatRecord> list2 = new List<FormatRecord>(8);
		list.Add(CreateBOF());
		list.Add(new InterfaceHdrRecord(1200));
		list.Add(CreateMMS());
		list.Add(InterfaceEndRecord.Instance);
		list.Add(CreateWriteAccess());
		list.Add(CreateCodepage());
		list.Add(CreateDSF());
		list.Add(CreateTabId());
		internalWorkbook.records.Tabpos = list.Count - 1;
		list.Add(CreateFnGroupCount());
		list.Add(CreateWindowProtect());
		list.Add(CreateProtect());
		internalWorkbook.records.Protpos = list.Count - 1;
		list.Add(CreatePassword());
		list.Add(CreateProtectionRev4());
		list.Add(CreatePasswordRev4());
		internalWorkbook.windowOne = (WindowOneRecord)CreateWindowOne();
		list.Add(internalWorkbook.windowOne);
		list.Add(CreateBackup());
		internalWorkbook.records.Backuppos = list.Count - 1;
		list.Add(CreateHideObj());
		list.Add(CreateDateWindow1904());
		list.Add(CreatePrecision());
		list.Add(CreateRefreshAll());
		list.Add(CreateBookBool());
		list.Add(CreateFont());
		list.Add(CreateFont());
		list.Add(CreateFont());
		list.Add(CreateFont());
		internalWorkbook.records.Fontpos = list.Count - 1;
		internalWorkbook.numfonts = 4;
		for (int i = 0; i <= 7; i++)
		{
			NPOI.HSSF.Record.Record record = CreateFormat(i);
			internalWorkbook.maxformatid = ((internalWorkbook.maxformatid >= ((FormatRecord)record).IndexCode) ? internalWorkbook.maxformatid : ((FormatRecord)record).IndexCode);
			list2.Add((FormatRecord)record);
			list.Add(record);
		}
		internalWorkbook.formats = list2;
		for (int j = 0; j < 21; j++)
		{
			list.Add(CreateExtendedFormat(j));
			internalWorkbook.numxfs++;
		}
		internalWorkbook.records.Xfpos = list.Count - 1;
		for (int k = 0; k < 6; k++)
		{
			list.Add(CreateStyle(k));
		}
		list.Add(CreateUseSelFS());
		int num = 1;
		for (int l = 0; l < num; l++)
		{
			BoundSheetRecord item = (BoundSheetRecord)CreateBoundSheet(l);
			list.Add(item);
			internalWorkbook.boundsheets.Add(item);
			internalWorkbook.records.Bspos = list.Count - 1;
		}
		list.Add(CreateCountry());
		for (int m = 0; m < num; m++)
		{
			internalWorkbook.OrCreateLinkTable.CheckExternSheet(m);
		}
		internalWorkbook.sst = new SSTRecord();
		list.Add(internalWorkbook.sst);
		list.Add(CreateExtendedSST());
		list.Add(EOFRecord.instance);
		return internalWorkbook;
	}

	public NameRecord GetSpecificBuiltinRecord(byte name, int sheetIndex)
	{
		return OrCreateLinkTable.GetSpecificBuiltinRecord(name, sheetIndex);
	}

	public ExternalName GetExternalName(int externSheetIndex, int externNameIndex)
	{
		string text = linkTable.ResolveNameXText(externSheetIndex, externNameIndex, this);
		if (text == null)
		{
			return null;
		}
		int ix = linkTable.ResolveNameXIx(externSheetIndex, externNameIndex);
		return new ExternalName(text, externNameIndex, ix);
	}

	public void RemoveBuiltinRecord(byte name, int sheetIndex)
	{
		linkTable.RemoveBuiltinRecord(name, sheetIndex);
	}

	public FontRecord GetFontRecordAt(int idx)
	{
		int num = idx;
		if (num > 4)
		{
			num--;
		}
		if (num > numfonts - 1)
		{
			throw new IndexOutOfRangeException("There are only " + numfonts + " font records, you asked for " + idx);
		}
		return (FontRecord)records[records.Fontpos - (numfonts - 1) + num];
	}

	public FontRecord CreateNewFont()
	{
		FontRecord fontRecord = (FontRecord)CreateFont();
		records.Add(records.Fontpos + 1, fontRecord);
		records.Fontpos += 1;
		numfonts++;
		return fontRecord;
	}

	public void CloneDrawings(InternalSheet sheet)
	{
		FindDrawingGroup();
		if (drawingManager == null || sheet.AggregateDrawingRecords(drawingManager, CreateIfMissing: false) == -1)
		{
			return;
		}
		EscherContainerRecord escherContainer = ((EscherAggregate)sheet.FindFirstRecordBySid(9876)).GetEscherContainer();
		if (escherContainer == null)
		{
			return;
		}
		EscherDggRecord dgg = drawingManager.GetDgg();
		int num = drawingManager.FindNewDrawingGroupId();
		dgg.AddCluster(num, 0);
		dgg.DrawingsSaved++;
		EscherDgRecord escherDgRecord = null;
		IEnumerator enumerator = escherContainer.ChildRecords.GetEnumerator();
		while (enumerator.MoveNext())
		{
			object current = enumerator.Current;
			if (current is EscherDgRecord)
			{
				escherDgRecord = (EscherDgRecord)current;
				escherDgRecord.Options = (short)(num << 4);
			}
			else
			{
				if (!(current is EscherContainerRecord))
				{
					continue;
				}
				new ArrayList();
				IEnumerator enumerator2 = ((EscherContainerRecord)current).ChildRecords.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					foreach (EscherRecord childRecord in ((EscherContainerRecord)enumerator2.Current).ChildRecords)
					{
						switch ((int)childRecord.RecordId)
						{
						case -4086:
						{
							EscherSpRecord obj = (EscherSpRecord)childRecord;
							int shapeId = drawingManager.AllocateShapeId((short)num, escherDgRecord);
							escherDgRecord.NumShapes--;
							obj.ShapeId = shapeId;
							break;
						}
						case -4085:
						{
							EscherSimpleProperty escherSimpleProperty = (EscherSimpleProperty)((EscherOptRecord)childRecord).Lookup(260);
							if (escherSimpleProperty != null)
							{
								int propertyValue = escherSimpleProperty.PropertyValue;
								GetBSERecord(propertyValue).Ref++;
							}
							break;
						}
						}
					}
				}
			}
		}
	}

	public void SetSheetBof(int sheetIndex, int pos)
	{
		CheckSheets(sheetIndex);
		GetBoundSheetRec(sheetIndex).PositionOfBof = pos;
	}

	public void SetSheetName(int sheetnum, string sheetname)
	{
		CheckSheets(sheetnum);
		boundsheets[sheetnum].Sheetname = sheetname;
	}

	private BoundSheetRecord GetBoundSheetRec(int sheetIndex)
	{
		return boundsheets[sheetIndex];
	}

	public bool ContainsSheetName(string name, int excludeSheetIdx)
	{
		string text = name;
		if (text.Length > 31)
		{
			text = text.Substring(0, 31);
		}
		for (int i = 0; i < boundsheets.Count; i++)
		{
			BoundSheetRecord boundSheetRec = GetBoundSheetRec(i);
			if (excludeSheetIdx != i)
			{
				string text2 = boundSheetRec.Sheetname;
				if (text2.Length > 31)
				{
					text2 = text2.Substring(0, 31);
				}
				if (text.Equals(text2, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void SetSheetName(int sheetnum, string sheetname, short encoding)
	{
		CheckSheets(sheetnum);
		boundsheets[sheetnum].Sheetname = sheetname;
	}

	public void SetSheetOrder(string sheetname, int pos)
	{
		int sheetIndex = GetSheetIndex(sheetname);
		BoundSheetRecord item = boundsheets[sheetIndex];
		boundsheets.RemoveAt(sheetIndex);
		boundsheets.Insert(pos, item);
		int bspos = records.Bspos;
		int num = records.Bspos - (boundsheets.Count - 1);
		NPOI.HSSF.Record.Record r = records[num + sheetIndex];
		records.Remove(num + sheetIndex);
		records.Add(num + pos, r);
		records.Bspos = bspos;
	}

	public string GetSheetName(int sheetIndex)
	{
		return GetBoundSheetRec(sheetIndex).Sheetname;
	}

	public bool IsSheetHidden(int sheetnum)
	{
		return GetBoundSheetRec(sheetnum).IsHidden;
	}

	public bool IsSheetVeryHidden(int sheetnum)
	{
		return GetBoundSheetRec(sheetnum).IsVeryHidden;
	}

	public void SetSheetHidden(int sheetnum, bool hidden)
	{
		boundsheets[sheetnum].IsHidden = hidden;
	}

	public void SetSheetHidden(int sheetnum, int hidden)
	{
		BoundSheetRecord boundSheetRec = GetBoundSheetRec(sheetnum);
		bool isHidden = false;
		bool isVeryHidden = false;
		switch (hidden)
		{
		case 1:
			isHidden = true;
			break;
		case 2:
			isVeryHidden = true;
			break;
		default:
			throw new ArgumentException("Invalid hidden flag " + hidden + " given, must be 0, 1 or 2");
		case 0:
			break;
		}
		boundSheetRec.IsHidden = isHidden;
		boundSheetRec.IsVeryHidden = isVeryHidden;
	}

	public int GetSheetIndex(string name)
	{
		int result = -1;
		for (int i = 0; i < boundsheets.Count; i++)
		{
			if (GetSheetName(i).Equals(name, StringComparison.OrdinalIgnoreCase))
			{
				result = i;
				break;
			}
		}
		return result;
	}

	private void CheckSheets(int sheetnum)
	{
		if (boundsheets.Count <= sheetnum)
		{
			if (boundsheets.Count + 1 <= sheetnum)
			{
				throw new Exception("Sheet number out of bounds!");
			}
			BoundSheetRecord boundSheetRecord = (BoundSheetRecord)CreateBoundSheet(sheetnum);
			records.Add(records.Bspos + 1, boundSheetRecord);
			records.Bspos += 1;
			boundsheets.Add(boundSheetRecord);
			OrCreateLinkTable.CheckExternSheet(sheetnum);
			FixTabIdRecord();
		}
	}

	public void RemoveSheet(int sheetIndex)
	{
		if (boundsheets.Count > sheetIndex)
		{
			records.Remove(records.Bspos - (boundsheets.Count - 1) + sheetIndex);
			boundsheets.RemoveAt(sheetIndex);
			FixTabIdRecord();
		}
		int num = sheetIndex + 1;
		for (int i = 0; i < NumNames; i++)
		{
			NameRecord nameRecord = GetNameRecord(i);
			if (nameRecord.SheetNumber == num)
			{
				nameRecord.SheetNumber = 0;
			}
			else if (nameRecord.SheetNumber > num)
			{
				nameRecord.SheetNumber--;
			}
		}
		if (linkTable != null)
		{
			linkTable.RemoveSheet(sheetIndex);
		}
	}

	private void FixTabIdRecord()
	{
		if (records.Tabpos > 0)
		{
			TabIdRecord tabIdRecord = (TabIdRecord)records[records.Tabpos];
			_ = tabIdRecord.RecordSize;
			short[] array = new short[boundsheets.Count];
			for (short num = 0; num < array.Length; num++)
			{
				array[num] = num;
			}
			tabIdRecord.SetTabIdArray(array);
		}
	}

	public int GetFontIndex(FontRecord font)
	{
		for (int i = 0; i <= numfonts; i++)
		{
			if ((FontRecord)records[records.Fontpos - (numfonts - 1) + i] == font)
			{
				if (i > 3)
				{
					return i + 1;
				}
				return i;
			}
		}
		throw new ArgumentException("Could not find that font!");
	}

	public StyleRecord GetStyleRecord(int xfIndex)
	{
		bool flag = false;
		for (int i = records.Xfpos; i < records.Count; i++)
		{
			if (flag)
			{
				break;
			}
			NPOI.HSSF.Record.Record record = records[i];
			if (record is ExtendedFormatRecord)
			{
				continue;
			}
			if (record is StyleRecord)
			{
				StyleRecord styleRecord = (StyleRecord)record;
				if (styleRecord.XFIndex == xfIndex)
				{
					return styleRecord;
				}
			}
			else
			{
				flag = true;
			}
		}
		return null;
	}

	public ExtendedFormatRecord GetExFormatAt(int index)
	{
		int num = records.Xfpos - (numxfs - 1);
		num += index;
		return (ExtendedFormatRecord)records[num];
	}

	public ExtendedFormatRecord CreateCellXF()
	{
		ExtendedFormatRecord extendedFormatRecord = CreateExtendedFormat();
		records.Add(records.Xfpos + 1, extendedFormatRecord);
		records.Xfpos += 1;
		numxfs++;
		return extendedFormatRecord;
	}

	public int AddSSTString(UnicodeString str)
	{
		if (sst == null)
		{
			InsertSST();
		}
		return sst.AddString(str);
	}

	public UnicodeString GetSSTString(int str)
	{
		if (sst == null)
		{
			InsertSST();
		}
		return sst.GetString(str);
	}

	public void InsertSST()
	{
		sst = new SSTRecord();
		records.Add(records.Count - 1, CreateExtendedSST());
		records.Add(records.Count - 2, sst);
	}

	public int Serialize(int offset, byte[] data)
	{
		int num = 0;
		SSTRecord sSTRecord = null;
		int num2 = 0;
		bool flag = false;
		for (int i = 0; i < records.Count; i++)
		{
			NPOI.HSSF.Record.Record record = records[i];
			if (record.Sid == 449 && !((RecalcIdRecord)record).IsNeeded)
			{
				continue;
			}
			int num3 = 0;
			if (record is SSTRecord)
			{
				sSTRecord = (SSTRecord)record;
				num2 = num;
			}
			if (record.Sid == 255 && sSTRecord != null)
			{
				record = sSTRecord.CreateExtSSTRecord(num2 + offset);
			}
			if (record is BoundSheetRecord)
			{
				if (!flag)
				{
					for (int j = 0; j < boundsheets.Count; j++)
					{
						num3 += boundsheets[j].Serialize(num + offset + num3, data);
					}
					flag = true;
				}
			}
			else
			{
				num3 = record.Serialize(num + offset, data);
			}
			num += num3;
		}
		return num;
	}

	public void PreSerialize()
	{
		if (records.Tabpos > 0 && ((TabIdRecord)records[records.Tabpos])._tabids.Length < boundsheets.Count)
		{
			FixTabIdRecord();
		}
	}

	private static NPOI.HSSF.Record.Record CreateBOF()
	{
		return new BOFRecord
		{
			Version = 1536,
			Type = BOFRecordType.Workbook,
			Build = 4307,
			BuildYear = 1996,
			HistoryBitMask = 65,
			RequiredVersion = 6
		};
	}

	[Obsolete]
	protected NPOI.HSSF.Record.Record CreateInterfaceHdr()
	{
		return null;
	}

	private static NPOI.HSSF.Record.Record CreateMMS()
	{
		return new MMSRecord
		{
			AddMenuCount = 0,
			DelMenuCount = 0
		};
	}

	[Obsolete]
	protected NPOI.HSSF.Record.Record CreateInterfaceEnd()
	{
		return null;
	}

	private static NPOI.HSSF.Record.Record CreateWriteAccess()
	{
		WriteAccessRecord writeAccessRecord = new WriteAccessRecord();
		string text = "NPOI";
		try
		{
			string text2 = Environment.UserName;
			if (string.IsNullOrEmpty(text2))
			{
				text2 = text;
			}
			writeAccessRecord.Username = text2;
		}
		catch (SecurityException)
		{
			writeAccessRecord.Username = text;
		}
		return writeAccessRecord;
	}

	private static NPOI.HSSF.Record.Record CreateCodepage()
	{
		return new CodepageRecord
		{
			Codepage = 1200
		};
	}

	private static NPOI.HSSF.Record.Record CreateDSF()
	{
		return new DSFRecord(isBiff5BookStreamPresent: false);
	}

	private static NPOI.HSSF.Record.Record CreateTabId()
	{
		TabIdRecord tabIdRecord = new TabIdRecord();
		short[] tabIdArray = new short[1];
		tabIdRecord.SetTabIdArray(tabIdArray);
		return tabIdRecord;
	}

	private static NPOI.HSSF.Record.Record CreateFnGroupCount()
	{
		return new FnGroupCountRecord
		{
			Count = 14
		};
	}

	private static NPOI.HSSF.Record.Record CreateWindowProtect()
	{
		return new WindowProtectRecord(protect: false);
	}

	private static ProtectRecord CreateProtect()
	{
		return new ProtectRecord(isProtected: false);
	}

	private static NPOI.HSSF.Record.Record CreatePassword()
	{
		return new PasswordRecord(0);
	}

	private static ProtectionRev4Record CreateProtectionRev4()
	{
		return new ProtectionRev4Record(protect: false);
	}

	private static NPOI.HSSF.Record.Record CreatePasswordRev4()
	{
		return new PasswordRev4Record(0);
	}

	private static NPOI.HSSF.Record.Record CreateWindowOne()
	{
		return new WindowOneRecord
		{
			HorizontalHold = 360,
			VerticalHold = 270,
			Width = 14940,
			Height = 9150,
			Options = 56,
			ActiveSheetIndex = 0,
			FirstVisibleTab = 0,
			NumSelectedTabs = 1,
			TabWidthRatio = 600
		};
	}

	private static NPOI.HSSF.Record.Record CreateBackup()
	{
		return new BackupRecord
		{
			Backup = 0
		};
	}

	private static NPOI.HSSF.Record.Record CreateHideObj()
	{
		HideObjRecord hideObjRecord = new HideObjRecord();
		hideObjRecord.SetHideObj(0);
		return hideObjRecord;
	}

	private static NPOI.HSSF.Record.Record CreateDateWindow1904()
	{
		return new DateWindow1904Record
		{
			Windowing = 0
		};
	}

	private static NPOI.HSSF.Record.Record CreatePrecision()
	{
		return new PrecisionRecord
		{
			FullPrecision = true
		};
	}

	private static NPOI.HSSF.Record.Record CreateRefreshAll()
	{
		return new RefreshAllRecord(refreshAll: false);
	}

	private static NPOI.HSSF.Record.Record CreateBookBool()
	{
		return new BookBoolRecord
		{
			SaveLinkValues = 0
		};
	}

	private static NPOI.HSSF.Record.Record CreateFont()
	{
		return new FontRecord
		{
			FontHeight = 200,
			Attributes = 0,
			ColorPaletteIndex = short.MaxValue,
			BoldWeight = 400,
			FontName = "Arial"
		};
	}

	private static NPOI.HSSF.Record.Record CreateExtendedFormat(int id)
	{
		ExtendedFormatRecord extendedFormatRecord = new ExtendedFormatRecord();
		switch (id)
		{
		case 0:
			extendedFormatRecord.FontIndex = 0;
			extendedFormatRecord.FormatIndex = 0;
			extendedFormatRecord.CellOptions = -11;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = 0;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		case 1:
			extendedFormatRecord.FontIndex = 1;
			extendedFormatRecord.FormatIndex = 0;
			extendedFormatRecord.CellOptions = -11;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = -3072;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		case 2:
			extendedFormatRecord.FontIndex = 1;
			extendedFormatRecord.FormatIndex = 0;
			extendedFormatRecord.CellOptions = -11;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = -3072;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		case 3:
			extendedFormatRecord.FontIndex = 2;
			extendedFormatRecord.FormatIndex = 0;
			extendedFormatRecord.CellOptions = -11;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = -3072;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		case 4:
			extendedFormatRecord.FontIndex = 2;
			extendedFormatRecord.FormatIndex = 0;
			extendedFormatRecord.CellOptions = -11;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = -3072;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		case 5:
			extendedFormatRecord.FontIndex = 0;
			extendedFormatRecord.FormatIndex = 0;
			extendedFormatRecord.CellOptions = -11;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = -3072;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		case 6:
			extendedFormatRecord.FontIndex = 0;
			extendedFormatRecord.FormatIndex = 0;
			extendedFormatRecord.CellOptions = -11;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = -3072;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		case 7:
			extendedFormatRecord.FontIndex = 0;
			extendedFormatRecord.FormatIndex = 0;
			extendedFormatRecord.CellOptions = -11;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = -3072;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		case 8:
			extendedFormatRecord.FontIndex = 0;
			extendedFormatRecord.FormatIndex = 0;
			extendedFormatRecord.CellOptions = -11;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = -3072;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		case 9:
			extendedFormatRecord.FontIndex = 0;
			extendedFormatRecord.FormatIndex = 0;
			extendedFormatRecord.CellOptions = -11;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = -3072;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		case 10:
			extendedFormatRecord.FontIndex = 0;
			extendedFormatRecord.FormatIndex = 0;
			extendedFormatRecord.CellOptions = -11;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = -3072;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		case 11:
			extendedFormatRecord.FontIndex = 0;
			extendedFormatRecord.FormatIndex = 0;
			extendedFormatRecord.CellOptions = -11;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = -3072;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		case 12:
			extendedFormatRecord.FontIndex = 0;
			extendedFormatRecord.FormatIndex = 0;
			extendedFormatRecord.CellOptions = -11;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = -3072;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		case 13:
			extendedFormatRecord.FontIndex = 0;
			extendedFormatRecord.FormatIndex = 0;
			extendedFormatRecord.CellOptions = -11;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = -3072;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		case 14:
			extendedFormatRecord.FontIndex = 0;
			extendedFormatRecord.FormatIndex = 0;
			extendedFormatRecord.CellOptions = -11;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = -3072;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		case 15:
			extendedFormatRecord.FontIndex = 0;
			extendedFormatRecord.FormatIndex = 0;
			extendedFormatRecord.CellOptions = 1;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = 0;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		case 16:
			extendedFormatRecord.FontIndex = 1;
			extendedFormatRecord.FormatIndex = 43;
			extendedFormatRecord.CellOptions = -11;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = -2048;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		case 17:
			extendedFormatRecord.FontIndex = 1;
			extendedFormatRecord.FormatIndex = 41;
			extendedFormatRecord.CellOptions = -11;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = -2048;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		case 18:
			extendedFormatRecord.FontIndex = 1;
			extendedFormatRecord.FormatIndex = 44;
			extendedFormatRecord.CellOptions = -11;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = -2048;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		case 19:
			extendedFormatRecord.FontIndex = 1;
			extendedFormatRecord.FormatIndex = 42;
			extendedFormatRecord.CellOptions = -11;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = -2048;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		case 20:
			extendedFormatRecord.FontIndex = 1;
			extendedFormatRecord.FormatIndex = 9;
			extendedFormatRecord.CellOptions = -11;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = -2048;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		case 21:
			extendedFormatRecord.FontIndex = 5;
			extendedFormatRecord.FormatIndex = 0;
			extendedFormatRecord.CellOptions = 1;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = 2048;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		case 22:
			extendedFormatRecord.FontIndex = 6;
			extendedFormatRecord.FormatIndex = 0;
			extendedFormatRecord.CellOptions = 1;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = 23552;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		case 23:
			extendedFormatRecord.FontIndex = 0;
			extendedFormatRecord.FormatIndex = 49;
			extendedFormatRecord.CellOptions = 1;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = 23552;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		case 24:
			extendedFormatRecord.FontIndex = 0;
			extendedFormatRecord.FormatIndex = 8;
			extendedFormatRecord.CellOptions = 1;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = 23552;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		case 25:
			extendedFormatRecord.FontIndex = 6;
			extendedFormatRecord.FormatIndex = 8;
			extendedFormatRecord.CellOptions = 1;
			extendedFormatRecord.AlignmentOptions = 32;
			extendedFormatRecord.IndentionOptions = 23552;
			extendedFormatRecord.BorderOptions = 0;
			extendedFormatRecord.PaletteOptions = 0;
			extendedFormatRecord.AdtlPaletteOptions = 0;
			extendedFormatRecord.FillPaletteOptions = 8384;
			break;
		default:
			throw new InvalidOperationException("Unrecognized format id: " + id);
		}
		return extendedFormatRecord;
	}

	private static ExtendedFormatRecord CreateExtendedFormat()
	{
		return new ExtendedFormatRecord
		{
			FontIndex = 0,
			FormatIndex = 0,
			CellOptions = 1,
			AlignmentOptions = 32,
			IndentionOptions = 0,
			BorderOptions = 0,
			PaletteOptions = 0,
			AdtlPaletteOptions = 0,
			FillPaletteOptions = 8384,
			TopBorderPaletteIdx = 8,
			BottomBorderPaletteIdx = 8,
			LeftBorderPaletteIdx = 8,
			RightBorderPaletteIdx = 8
		};
	}

	public StyleRecord CreateStyleRecord(int xfIndex)
	{
		StyleRecord styleRecord = new StyleRecord();
		styleRecord.XFIndex = (short)xfIndex;
		int num = -1;
		for (int i = records.Xfpos; i < records.Count; i++)
		{
			if (num != -1)
			{
				break;
			}
			NPOI.HSSF.Record.Record record = records[i];
			if (!(record is ExtendedFormatRecord) && !(record is StyleRecord))
			{
				num = i;
			}
		}
		if (num == -1)
		{
			throw new InvalidOperationException("No XF Records found!");
		}
		records.Add(num, styleRecord);
		return styleRecord;
	}

	private static NPOI.HSSF.Record.Record CreateStyle(int id)
	{
		StyleRecord styleRecord = new StyleRecord();
		switch (id)
		{
		case 0:
			styleRecord.XFIndex = -32752;
			styleRecord.SetBuiltinStyle(3);
			styleRecord.OutlineStyleLevel = 255;
			break;
		case 1:
			styleRecord.XFIndex = -32751;
			styleRecord.SetBuiltinStyle(6);
			styleRecord.OutlineStyleLevel = 255;
			break;
		case 2:
			styleRecord.XFIndex = -32750;
			styleRecord.SetBuiltinStyle(4);
			styleRecord.OutlineStyleLevel = 255;
			break;
		case 3:
			styleRecord.XFIndex = -32749;
			styleRecord.SetBuiltinStyle(7);
			styleRecord.OutlineStyleLevel = 255;
			break;
		case 4:
			styleRecord.XFIndex = short.MinValue;
			styleRecord.SetBuiltinStyle(0);
			styleRecord.OutlineStyleLevel = 255;
			break;
		case 5:
			styleRecord.XFIndex = -32748;
			styleRecord.SetBuiltinStyle(5);
			styleRecord.OutlineStyleLevel = 255;
			break;
		default:
			throw new InvalidOperationException("Unrecognized style id: " + id);
		}
		return styleRecord;
	}

	private static PaletteRecord CreatePalette()
	{
		return new PaletteRecord();
	}

	private static UseSelFSRecord CreateUseSelFS()
	{
		return new UseSelFSRecord(b: false);
	}

	private static NPOI.HSSF.Record.Record CreateBoundSheet(int id)
	{
		return new BoundSheetRecord("Sheet" + (id + 1));
	}

	private static NPOI.HSSF.Record.Record CreateCountry()
	{
		CountryRecord countryRecord = new CountryRecord();
		countryRecord.DefaultCountry = 1;
		if (Thread.CurrentThread.CurrentCulture.Name.Equals("ru_RU"))
		{
			countryRecord.CurrentCountry = 7;
		}
		else
		{
			countryRecord.CurrentCountry = 1;
		}
		return countryRecord;
	}

	private static NPOI.HSSF.Record.Record CreateExtendedSST()
	{
		return new ExtSSTRecord
		{
			NumStringsPerBucket = 8
		};
	}

	private LinkTable GetOrCreateLinkTable()
	{
		if (linkTable == null)
		{
			linkTable = new LinkTable((short)NumSheets, records);
		}
		return linkTable;
	}

	public int LinkExternalWorkbook(string name, IWorkbook externalWorkbook)
	{
		return GetOrCreateLinkTable().LinkExternalWorkbook(name, externalWorkbook);
	}

	public string FindSheetFirstNameFromExternSheet(int externSheetIndex)
	{
		int firstInternalSheetIndexForExtIndex = linkTable.GetFirstInternalSheetIndexForExtIndex(externSheetIndex);
		return FindSheetNameFromIndex(firstInternalSheetIndexForExtIndex);
	}

	public string FindSheetLastNameFromExternSheet(int externSheetIndex)
	{
		int lastInternalSheetIndexForExtIndex = linkTable.GetLastInternalSheetIndexForExtIndex(externSheetIndex);
		return FindSheetNameFromIndex(lastInternalSheetIndexForExtIndex);
	}

	private string FindSheetNameFromIndex(int internalSheetIndex)
	{
		if (internalSheetIndex < 0)
		{
			return "";
		}
		if (internalSheetIndex >= boundsheets.Count)
		{
			return "";
		}
		return GetSheetName(internalSheetIndex);
	}

	public ExternalSheet GetExternalSheet(int externSheetIndex)
	{
		string[] externalBookAndSheetName = linkTable.GetExternalBookAndSheetName(externSheetIndex);
		if (externalBookAndSheetName == null)
		{
			return null;
		}
		if (externalBookAndSheetName.Length == 2)
		{
			return new ExternalSheet(externalBookAndSheetName[0], externalBookAndSheetName[1]);
		}
		return new ExternalSheetRange(externalBookAndSheetName[0], externalBookAndSheetName[1], externalBookAndSheetName[2]);
	}

	public int GetFirstSheetIndexFromExternSheetIndex(int externSheetNumber)
	{
		return linkTable.GetFirstInternalSheetIndexForExtIndex(externSheetNumber);
	}

	public int GetLastSheetIndexFromExternSheetIndex(int externSheetNumber)
	{
		return linkTable.GetLastInternalSheetIndexForExtIndex(externSheetNumber);
	}

	public int CheckExternSheet(int sheetNumber)
	{
		return OrCreateLinkTable.CheckExternSheet(sheetNumber);
	}

	public short checkExternSheet(int firstSheetNumber, int lastSheetNumber)
	{
		return (short)OrCreateLinkTable.CheckExternSheet(firstSheetNumber, lastSheetNumber);
	}

	public int GetExternalSheetIndex(string workbookName, string sheetName)
	{
		return OrCreateLinkTable.GetExternalSheetIndex(workbookName, sheetName, sheetName);
	}

	public int GetExternalSheetIndex(string workbookName, string firstSheetName, string lastSheetName)
	{
		return OrCreateLinkTable.GetExternalSheetIndex(workbookName, firstSheetName, lastSheetName);
	}

	public NameXPtg GetNameXPtg(string name, int sheetRefIndex, UDFFinder udf)
	{
		LinkTable orCreateLinkTable = OrCreateLinkTable;
		NameXPtg nameXPtg = orCreateLinkTable.GetNameXPtg(name, sheetRefIndex);
		if (nameXPtg == null && udf.FindFunction(name) != null)
		{
			nameXPtg = orCreateLinkTable.AddNameXPtg(name);
		}
		return nameXPtg;
	}

	public NameXPtg GetNameXPtg(string name, UDFFinder udf)
	{
		return GetNameXPtg(name, -1, udf);
	}

	public NameRecord GetNameRecord(int index)
	{
		return linkTable.GetNameRecord(index);
	}

	public NameRecord CreateName()
	{
		return AddName(new NameRecord());
	}

	public NameRecord AddName(NameRecord name)
	{
		OrCreateLinkTable.AddName(name);
		return name;
	}

	public NameRecord CreateBuiltInName(byte builtInName, int index)
	{
		if (index == -1 || index + 1 > 32767)
		{
			throw new ArgumentException("Index is not valid [" + index + "]");
		}
		NameRecord nameRecord = new NameRecord(builtInName, (short)index);
		AddName(nameRecord);
		return nameRecord;
	}

	public void RemoveName(int namenum)
	{
		if (linkTable.NumNames > namenum)
		{
			int num = FindFirstRecordLocBySid(24);
			records.Remove(num + namenum);
			linkTable.RemoveName(namenum);
		}
	}

	public void UpdateNameCommentRecordCache(NameCommentRecord commentRecord)
	{
		if (commentRecords.ContainsValue(commentRecord))
		{
			foreach (KeyValuePair<string, NameCommentRecord> commentRecord2 in commentRecords)
			{
				if (commentRecord2.Value.Equals(commentRecord))
				{
					commentRecords.Remove(commentRecord2.Key);
					break;
				}
			}
		}
		commentRecords[commentRecord.NameText] = commentRecord;
	}

	public short GetFormat(string format, bool CreateIfNotFound)
	{
		IEnumerator enumerator = formats.GetEnumerator();
		while (enumerator.MoveNext())
		{
			FormatRecord formatRecord = (FormatRecord)enumerator.Current;
			if (formatRecord.FormatString.Equals(format))
			{
				return (short)formatRecord.IndexCode;
			}
		}
		if (CreateIfNotFound)
		{
			return (short)CreateFormat(format);
		}
		return -1;
	}

	public int CreateFormat(string formatString)
	{
		maxformatid = ((maxformatid >= 164) ? ((short)(maxformatid + 1)) : 164);
		FormatRecord formatRecord = new FormatRecord(maxformatid, formatString);
		int i;
		for (i = 0; i < records.Count && records[i].Sid != 1054; i++)
		{
		}
		i += formats.Count;
		formats.Add(formatRecord);
		records.Add(i, formatRecord);
		return maxformatid;
	}

	private static FormatRecord CreateFormat(int id)
	{
		return id switch
		{
			0 => new FormatRecord(5, BuiltinFormats.GetBuiltinFormat(5)), 
			1 => new FormatRecord(6, BuiltinFormats.GetBuiltinFormat(6)), 
			2 => new FormatRecord(7, BuiltinFormats.GetBuiltinFormat(7)), 
			3 => new FormatRecord(8, BuiltinFormats.GetBuiltinFormat(8)), 
			4 => new FormatRecord(42, BuiltinFormats.GetBuiltinFormat(42)), 
			5 => new FormatRecord(41, BuiltinFormats.GetBuiltinFormat(41)), 
			6 => new FormatRecord(44, BuiltinFormats.GetBuiltinFormat(44)), 
			7 => new FormatRecord(43, BuiltinFormats.GetBuiltinFormat(43)), 
			_ => throw new ArgumentException("Unexpected id " + id), 
		};
	}

	public NPOI.HSSF.Record.Record FindFirstRecordBySid(short sid)
	{
		IEnumerator enumerator = records.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NPOI.HSSF.Record.Record record = (NPOI.HSSF.Record.Record)enumerator.Current;
			if (record.Sid == sid)
			{
				return record;
			}
		}
		return null;
	}

	public int FindFirstRecordLocBySid(short sid)
	{
		int num = 0;
		IEnumerator enumerator = records.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (((NPOI.HSSF.Record.Record)enumerator.Current).Sid == sid)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public NPOI.HSSF.Record.Record FindNextRecordBySid(short sid, int pos)
	{
		int num = 0;
		IEnumerator enumerator = records.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NPOI.HSSF.Record.Record record = (NPOI.HSSF.Record.Record)enumerator.Current;
			if (record.Sid == sid && num++ == pos)
			{
				return record;
			}
		}
		return null;
	}

	public DrawingManager2 FindDrawingGroup()
	{
		if (drawingManager != null)
		{
			return drawingManager;
		}
		IEnumerator enumerator = records.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NPOI.HSSF.Record.Record record = (NPOI.HSSF.Record.Record)enumerator.Current;
			if (!(record is DrawingGroupRecord))
			{
				continue;
			}
			DrawingGroupRecord obj = (DrawingGroupRecord)record;
			obj.ProcessChildRecords();
			EscherContainerRecord escherContainer = obj.GetEscherContainer();
			if (escherContainer == null)
			{
				continue;
			}
			EscherDggRecord escherDggRecord = null;
			EscherContainerRecord escherContainerRecord = null;
			IEnumerator enumerator2 = escherContainer.ChildRecords.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				EscherRecord escherRecord = (EscherRecord)enumerator2.Current;
				if (escherRecord is EscherDggRecord)
				{
					escherDggRecord = (EscherDggRecord)escherRecord;
				}
				else if (escherRecord.RecordId == -4095)
				{
					escherContainerRecord = (EscherContainerRecord)escherRecord;
				}
			}
			if (escherDggRecord == null)
			{
				continue;
			}
			drawingManager = new DrawingManager2(escherDggRecord);
			if (escherContainerRecord != null)
			{
				foreach (EscherRecord childRecord in escherContainerRecord.ChildRecords)
				{
					if (childRecord is EscherBSERecord)
					{
						escherBSERecords.Add((EscherBSERecord)childRecord);
					}
				}
			}
			return drawingManager;
		}
		int num = FindFirstRecordLocBySid(235);
		if (num != -1)
		{
			DrawingGroupRecord obj2 = (DrawingGroupRecord)records[num];
			EscherDggRecord escherDggRecord2 = null;
			EscherContainerRecord escherContainerRecord2 = null;
			IEnumerator enumerator4 = obj2.EscherRecords.GetEnumerator();
			while (enumerator4.MoveNext())
			{
				EscherRecord escherRecord2 = (EscherRecord)enumerator4.Current;
				if (escherRecord2 is EscherDggRecord)
				{
					escherDggRecord2 = (EscherDggRecord)escherRecord2;
				}
				else if (escherRecord2.RecordId == -4095)
				{
					escherContainerRecord2 = (EscherContainerRecord)escherRecord2;
				}
			}
			if (escherDggRecord2 != null)
			{
				drawingManager = new DrawingManager2(escherDggRecord2);
				if (escherContainerRecord2 != null)
				{
					foreach (EscherRecord childRecord2 in escherContainerRecord2.ChildRecords)
					{
						if (childRecord2 is EscherBSERecord)
						{
							escherBSERecords.Add((EscherBSERecord)childRecord2);
						}
					}
				}
			}
		}
		return drawingManager;
	}

	public void CreateDrawingGroup()
	{
		if (drawingManager != null)
		{
			return;
		}
		EscherContainerRecord escherContainerRecord = new EscherContainerRecord();
		EscherDggRecord escherDggRecord = new EscherDggRecord();
		EscherOptRecord escherOptRecord = new EscherOptRecord();
		EscherSplitMenuColorsRecord escherSplitMenuColorsRecord = new EscherSplitMenuColorsRecord();
		escherContainerRecord.RecordId = -4096;
		escherContainerRecord.Options = 15;
		escherDggRecord.RecordId = -4090;
		escherDggRecord.Options = 0;
		escherDggRecord.ShapeIdMax = 1024;
		escherDggRecord.NumShapesSaved = 0;
		escherDggRecord.DrawingsSaved = 0;
		escherDggRecord.FileIdClusters = new EscherDggRecord.FileIdCluster[0];
		drawingManager = new DrawingManager2(escherDggRecord);
		EscherContainerRecord escherContainerRecord2 = null;
		if (escherBSERecords.Count > 0)
		{
			escherContainerRecord2 = new EscherContainerRecord();
			escherContainerRecord2.RecordId = -4095;
			escherContainerRecord2.Options = (short)((escherBSERecords.Count << 4) | 0xF);
			IEnumerator enumerator = escherBSERecords.GetEnumerator();
			while (enumerator.MoveNext())
			{
				EscherRecord record = (EscherRecord)enumerator.Current;
				escherContainerRecord2.AddChildRecord(record);
			}
		}
		escherOptRecord.RecordId = -4085;
		escherOptRecord.Options = 51;
		escherOptRecord.AddEscherProperty(new EscherBoolProperty(191, 524296));
		escherOptRecord.AddEscherProperty(new EscherRGBProperty(385, 134217793));
		escherOptRecord.AddEscherProperty(new EscherRGBProperty(448, 134217792));
		escherSplitMenuColorsRecord.RecordId = -3810;
		escherSplitMenuColorsRecord.Options = 64;
		escherSplitMenuColorsRecord.Color1 = 134217741;
		escherSplitMenuColorsRecord.Color2 = 134217740;
		escherSplitMenuColorsRecord.Color3 = 134217751;
		escherSplitMenuColorsRecord.Color4 = 268435703;
		escherContainerRecord.AddChildRecord(escherDggRecord);
		if (escherContainerRecord2 != null)
		{
			escherContainerRecord.AddChildRecord(escherContainerRecord2);
		}
		escherContainerRecord.AddChildRecord(escherOptRecord);
		escherContainerRecord.AddChildRecord(escherSplitMenuColorsRecord);
		int num = FindFirstRecordLocBySid(235);
		if (num == -1)
		{
			DrawingGroupRecord drawingGroupRecord = new DrawingGroupRecord();
			drawingGroupRecord.AddEscherRecord(escherContainerRecord);
			int num2 = FindFirstRecordLocBySid(140);
			Records.Insert(num2 + 1, drawingGroupRecord);
		}
		else
		{
			DrawingGroupRecord drawingGroupRecord2 = new DrawingGroupRecord();
			drawingGroupRecord2.AddEscherRecord(escherContainerRecord);
			Records[num] = drawingGroupRecord2;
		}
	}

	public void RemoveFontRecord(FontRecord rec)
	{
		records.Remove(rec);
		numfonts--;
	}

	public void RemoveExFormatRecord(ExtendedFormatRecord rec)
	{
		records.Remove(rec);
		numxfs--;
	}

	public void RemoveExFormatRecord(int index)
	{
		int pos = records.Xfpos - (numxfs - 1) + index;
		records.Remove(pos);
		numxfs--;
	}

	public EscherBSERecord GetBSERecord(int pictureIndex)
	{
		return (EscherBSERecord)escherBSERecords[pictureIndex - 1];
	}

	public int AddBSERecord(EscherBSERecord e)
	{
		CreateDrawingGroup();
		escherBSERecords.Add(e);
		int index = FindFirstRecordLocBySid(235);
		EscherContainerRecord escherContainerRecord = (EscherContainerRecord)((DrawingGroupRecord)Records[index]).GetEscherRecord(0);
		EscherContainerRecord escherContainerRecord2;
		if (escherContainerRecord.GetChild(1).RecordId == -4095)
		{
			escherContainerRecord2 = (EscherContainerRecord)escherContainerRecord.GetChild(1);
		}
		else
		{
			escherContainerRecord2 = new EscherContainerRecord();
			escherContainerRecord2.RecordId = -4095;
			List<EscherRecord> childRecords = escherContainerRecord.ChildRecords;
			childRecords.Insert(1, escherContainerRecord2);
			escherContainerRecord.ChildRecords = childRecords;
		}
		escherContainerRecord2.Options = (short)((escherBSERecords.Count << 4) | 0xF);
		escherContainerRecord2.AddChildRecord(e);
		return escherBSERecords.Count;
	}

	public void WriteProtectWorkbook(string password, string username)
	{
		FileSharingRecord fileSharing = FileSharing;
		WriteAccessRecord writeAccessRecord = WriteAccess;
		fileSharing.ReadOnly = 1;
		fileSharing.Password = (short)CryptoFunctions.CreateXorVerifier1(password);
		fileSharing.Username = username;
		writeAccessRecord.Username = username;
	}

	public void UnwriteProtectWorkbook()
	{
		records.Remove(fileShare);
		records.Remove(WriteProtect);
		fileShare = null;
		writeProtect = null;
	}

	public string ResolveNameXText(int reFindex, int definedNameIndex)
	{
		return linkTable.ResolveNameXText(reFindex, definedNameIndex, this);
	}

	public NameRecord CloneFilter(int filterDbNameIndex, int newSheetIndex)
	{
		NameRecord nameRecord = GetNameRecord(filterDbNameIndex);
		int externSheetIndex = CheckExternSheet(newSheetIndex);
		Ptg[] nameDefinition = nameRecord.NameDefinition;
		for (int i = 0; i < nameDefinition.Length; i++)
		{
			Ptg ptg = nameDefinition[i];
			if (ptg is Area3DPtg)
			{
				Area3DPtg area3DPtg = (Area3DPtg)((OperandPtg)ptg).Copy();
				area3DPtg.ExternSheetIndex = externSheetIndex;
				nameDefinition[i] = area3DPtg;
			}
			else if (ptg is Ref3DPtg)
			{
				Ref3DPtg ref3DPtg = (Ref3DPtg)((OperandPtg)ptg).Copy();
				ref3DPtg.ExternSheetIndex = externSheetIndex;
				nameDefinition[i] = ref3DPtg;
			}
		}
		NameRecord nameRecord2 = CreateBuiltInName(13, newSheetIndex + 1);
		nameRecord2.NameDefinition = nameDefinition;
		nameRecord2.IsHiddenName = true;
		return nameRecord2;
	}

	public void UpdateNamesAfterCellShift(FormulaShifter shifter)
	{
		for (int i = 0; i < NumNames; i++)
		{
			NameRecord nameRecord = GetNameRecord(i);
			Ptg[] nameDefinition = nameRecord.NameDefinition;
			if (shifter.AdjustFormula(nameDefinition, nameRecord.SheetNumber))
			{
				nameRecord.NameDefinition = nameDefinition;
			}
		}
	}

	public bool ChangeExternalReference(string oldUrl, string newUrl)
	{
		return linkTable.ChangeExternalReference(oldUrl, newUrl);
	}
}

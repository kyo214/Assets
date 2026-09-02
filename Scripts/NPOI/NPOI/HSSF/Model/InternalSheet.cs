using System;
using System.Collections;
using System.Collections.Generic;
using NPOI.HSSF.Record;
using NPOI.HSSF.Record.Aggregates;
using NPOI.SS.Formula;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.HSSF.Model;

[Serializable]
public class InternalSheet
{
	private class RecordCloner : RecordVisitor
	{
		private IList<NPOI.HSSF.Record.Record> _destList;

		public RecordCloner(IList<NPOI.HSSF.Record.Record> destList)
		{
			_destList = destList;
		}

		public void VisitRecord(NPOI.HSSF.Record.Record r)
		{
			try
			{
				_destList.Add((NPOI.HSSF.Record.Record)r.Clone());
			}
			catch (NotSupportedException ex)
			{
				throw new RecordFormatException(ex);
			}
		}
	}

	private class RecordVisitor1 : RecordVisitor
	{
		private List<RecordBase> _records;

		public RecordVisitor1(List<RecordBase> recs)
		{
			_records = recs;
		}

		public void VisitRecord(NPOI.HSSF.Record.Record r)
		{
			_records.Add(r);
		}
	}

	private static POILogger log = POILogFactory.GetLogger(typeof(InternalSheet));

	private int preoffset;

	protected int dimsloc = -1;

	[NonSerialized]
	protected DimensionsRecord dims;

	[NonSerialized]
	protected DefaultColWidthRecord defaultcolwidth = new DefaultColWidthRecord();

	[NonSerialized]
	protected DefaultRowHeightRecord defaultrowheight = new DefaultRowHeightRecord();

	[NonSerialized]
	protected GridsetRecord gridset;

	[NonSerialized]
	protected PrintSetupRecord printSetup;

	[NonSerialized]
	protected HeaderRecord header;

	[NonSerialized]
	protected FooterRecord footer;

	[NonSerialized]
	protected PrintGridlinesRecord printGridlines;

	[NonSerialized]
	protected PrintHeadersRecord printHeaders;

	[NonSerialized]
	protected WindowTwoRecord windowTwo;

	[NonSerialized]
	protected MergeCellsRecord merged;

	[NonSerialized]
	private MergedCellsTable _mergedCellsTable;

	[NonSerialized]
	protected RowRecordsAggregate _rowsAggregate;

	[NonSerialized]
	private PageSettingsBlock _psBlock;

	protected IMargin[] margins;

	[NonSerialized]
	protected SelectionRecord _selection;

	[NonSerialized]
	internal ColumnInfoRecordsAggregate _columnInfos;

	[NonSerialized]
	private DimensionsRecord _dimensions;

	[NonSerialized]
	private DataValidityTable _dataValidityTable;

	private IEnumerator rowRecEnumerator;

	protected int eofLoc;

	[NonSerialized]
	private GutsRecord _gutsRecord;

	[NonSerialized]
	protected PageBreakRecord rowBreaks;

	[NonSerialized]
	protected PageBreakRecord colBreaks;

	[NonSerialized]
	protected ConditionalFormattingTable condFormatting;

	[NonSerialized]
	protected SheetExtRecord sheetext;

	protected List<RecordBase> records;

	protected bool _isUncalced;

	[NonSerialized]
	private WorksheetProtectionBlock _protectionBlock = new WorksheetProtectionBlock();

	public WindowTwoRecord WindowTwo => windowTwo;

	public ColumnInfoRecordsAggregate ColumnInfos => _columnInfos;

	internal MergedCellsTable MergedRecords => _mergedCellsTable;

	public int NumMergedRegions => MergedRecords.NumberOfMergedRegions;

	public int NumConditionalFormattings => condFormatting.Count;

	public int PreOffset
	{
		get
		{
			return preoffset;
		}
		set
		{
			preoffset = value;
		}
	}

	public RowRecord NextRow
	{
		get
		{
			if (rowRecEnumerator == null)
			{
				rowRecEnumerator = _rowsAggregate.GetEnumerator();
			}
			if (!rowRecEnumerator.MoveNext())
			{
				return null;
			}
			return (RowRecord)rowRecEnumerator.Current;
		}
	}

	public PageSettingsBlock PageSettings
	{
		get
		{
			if (_psBlock == null)
			{
				_psBlock = new PageSettingsBlock();
				RecordOrderer.AddNewSheetRecord(records, _psBlock);
			}
			return _psBlock;
		}
	}

	public int DefaultColumnWidth
	{
		get
		{
			return defaultcolwidth.ColWidth;
		}
		set
		{
			defaultcolwidth.ColWidth = (short)value;
		}
	}

	public short DefaultRowHeight
	{
		get
		{
			return defaultrowheight.RowHeight;
		}
		set
		{
			defaultrowheight.RowHeight = value;
			defaultrowheight.OptionFlags = 1;
		}
	}

	public short TopRow
	{
		get
		{
			if (windowTwo != null)
			{
				return windowTwo.TopRow;
			}
			return 0;
		}
		set
		{
			if (windowTwo != null)
			{
				windowTwo.TopRow = value;
			}
		}
	}

	public short LeftCol
	{
		get
		{
			if (windowTwo != null)
			{
				return windowTwo.LeftCol;
			}
			return 0;
		}
		set
		{
			if (windowTwo != null)
			{
				windowTwo.LeftCol = value;
			}
		}
	}

	public int ActiveCellRow
	{
		get
		{
			if (_selection == null)
			{
				return 0;
			}
			return _selection.ActiveCellRow;
		}
		set
		{
			if (_selection != null)
			{
				_selection.ActiveCellRow = value;
			}
		}
	}

	public int ActiveCellCol
	{
		get
		{
			if (_selection == null)
			{
				return 0;
			}
			return _selection.ActiveCellCol;
		}
		set
		{
			if (_selection != null)
			{
				_selection.ActiveCellCol = value;
			}
		}
	}

	public List<RecordBase> Records => records;

	public GridsetRecord GridsetRecord => gridset;

	private GutsRecord GutsRecord
	{
		get
		{
			if (_gutsRecord == null)
			{
				GutsRecord gutsRecord = CreateGuts();
				RecordOrderer.AddNewSheetRecord(records, gutsRecord);
				_gutsRecord = gutsRecord;
			}
			return _gutsRecord;
		}
	}

	public HeaderRecord Header
	{
		get
		{
			return header;
		}
		set
		{
			header = value;
		}
	}

	public bool IsAutoTabColor
	{
		get
		{
			return sheetext.IsAutoColor;
		}
		set
		{
			sheetext.IsAutoColor = value;
		}
	}

	public short TabColorIndex
	{
		get
		{
			return sheetext.TabColorIndex;
		}
		set
		{
			if ((value <= 8 || value >= 63) && value != 127)
			{
				throw new ArgumentException("invalid color index");
			}
			sheetext.TabColorIndex = value;
		}
	}

	public FooterRecord Footer
	{
		get
		{
			return footer;
		}
		set
		{
			footer = value;
		}
	}

	public PrintSetupRecord PrintSetup
	{
		get
		{
			return printSetup;
		}
		set
		{
			printSetup = value;
		}
	}

	public bool IsGridsPrinted
	{
		get
		{
			if (gridset == null)
			{
				gridset = CreateGridset();
				int index = FindFirstRecordLocBySid(10);
				records.Insert(index, gridset);
			}
			return !gridset.Gridset;
		}
		set
		{
			gridset.Gridset = !value;
		}
	}

	public PrintGridlinesRecord PrintGridlines
	{
		get
		{
			return printGridlines;
		}
		set
		{
			printGridlines = value;
		}
	}

	public PrintHeadersRecord PrintHeaders
	{
		get
		{
			return printHeaders;
		}
		set
		{
			printHeaders = value;
		}
	}

	public PaneInformation PaneInformation
	{
		get
		{
			PaneRecord paneRecord = (PaneRecord)FindFirstRecordBySid(65);
			if (paneRecord == null)
			{
				return null;
			}
			return new PaneInformation(paneRecord.X, paneRecord.Y, paneRecord.TopRow, paneRecord.LeftColumn, (byte)paneRecord.ActivePane, windowTwo.FreezePanes);
		}
	}

	public SelectionRecord Selection
	{
		get
		{
			return _selection;
		}
		set
		{
			_selection = value;
		}
	}

	public WorksheetProtectionBlock ProtectionBlock => _protectionBlock;

	public bool DisplayGridlines
	{
		get
		{
			return windowTwo.DisplayGridlines;
		}
		set
		{
			windowTwo.DisplayGridlines = value;
		}
	}

	public bool DisplayFormulas
	{
		get
		{
			return windowTwo.DisplayFormulas;
		}
		set
		{
			windowTwo.DisplayFormulas = value;
		}
	}

	public bool DisplayRowColHeadings
	{
		get
		{
			return windowTwo.DisplayRowColHeadings;
		}
		set
		{
			windowTwo.DisplayRowColHeadings = value;
		}
	}

	public bool IsPrintRowColHeadings
	{
		get
		{
			return windowTwo.DisplayRowColHeadings;
		}
		set
		{
			windowTwo.DisplayRowColHeadings = value;
		}
	}

	public bool IsUncalced
	{
		get
		{
			return _isUncalced;
		}
		set
		{
			_isUncalced = value;
		}
	}

	public RowRecordsAggregate RowsAggregate => _rowsAggregate;

	public ConditionalFormattingTable ConditionalFormattingTable
	{
		get
		{
			if (condFormatting == null)
			{
				condFormatting = new ConditionalFormattingTable();
				RecordOrderer.AddNewSheetRecord(records, condFormatting);
			}
			return condFormatting;
		}
	}

	public InternalSheet CloneSheet()
	{
		List<NPOI.HSSF.Record.Record> list = new List<NPOI.HSSF.Record.Record>(records.Count);
		for (int i = 0; i < records.Count; i++)
		{
			RecordBase recordBase = records[i];
			if (recordBase is RecordAggregate)
			{
				((RecordAggregate)recordBase).VisitContainedRecords(new RecordCloner(list));
				continue;
			}
			if (recordBase is EscherAggregate)
			{
				recordBase = new DrawingRecord();
			}
			try
			{
				NPOI.HSSF.Record.Record item = (NPOI.HSSF.Record.Record)((NPOI.HSSF.Record.Record)recordBase).Clone();
				list.Add(item);
			}
			catch (NotSupportedException ex)
			{
				throw new RecordFormatException(ex);
			}
		}
		return CreateSheet(new RecordStream(list, 0));
	}

	public static InternalSheet CreateSheet(RecordStream rs)
	{
		return new InternalSheet(rs);
	}

	private InternalSheet(RecordStream rs)
	{
		_mergedCellsTable = new MergedCellsTable();
		RowRecordsAggregate rowRecordsAggregate = null;
		records = new List<RecordBase>(128);
		int num = -1;
		if (rs.PeekNextSid() != 2057)
		{
			throw new RecordFormatException("BOF record expected");
		}
		BOFRecord bOFRecord = (BOFRecord)rs.GetNext();
		if (bOFRecord.Type != BOFRecordType.Worksheet && bOFRecord.Type != BOFRecordType.Chart && bOFRecord.Type != BOFRecordType.Excel4Macro)
		{
			while (rs.HasNext() && !(rs.GetNext() is EOFRecord))
			{
			}
			throw new UnsupportedBOFType(bOFRecord.Type);
		}
		records.Add(bOFRecord);
		while (rs.HasNext())
		{
			int num2 = rs.PeekNextSid();
			if (num2 == CFHeaderRecord.sid || num2 == CFHeader12Record.sid)
			{
				condFormatting = new ConditionalFormattingTable(rs);
				records.Add(condFormatting);
				continue;
			}
			switch (num2)
			{
			case 125:
				_columnInfos = new ColumnInfoRecordsAggregate(rs);
				records.Add(_columnInfos);
				continue;
			case 434:
				_dataValidityTable = new DataValidityTable(rs);
				records.Add(_dataValidityTable);
				continue;
			}
			if (RecordOrderer.IsRowBlockRecord(num2))
			{
				if (rowRecordsAggregate != null)
				{
					throw new InvalidOperationException("row/cell records found in the wrong place");
				}
				RowBlocksReader rowBlocksReader = new RowBlocksReader(rs);
				_mergedCellsTable.AddRecords(rowBlocksReader.LooseMergedCells);
				rowRecordsAggregate = new RowRecordsAggregate(rowBlocksReader.PlainRecordStream, rowBlocksReader.SharedFormulaManager);
				records.Add(rowRecordsAggregate);
				continue;
			}
			if (CustomViewSettingsRecordAggregate.IsBeginRecord(num2))
			{
				records.Add(new CustomViewSettingsRecordAggregate(rs));
				continue;
			}
			if (PageSettingsBlock.IsComponentRecord(num2))
			{
				if (_psBlock == null)
				{
					_psBlock = new PageSettingsBlock(rs);
					records.Add(_psBlock);
				}
				else
				{
					_psBlock.AddLateRecords(rs);
				}
				_psBlock.PositionRecords(records);
				continue;
			}
			if (WorksheetProtectionBlock.IsComponentRecord(num2))
			{
				_protectionBlock.AddRecords(rs);
				continue;
			}
			switch (num2)
			{
			case 229:
				_mergedCellsTable.Read(rs);
				continue;
			case 2057:
				SpillAggregate(new ChartSubstreamRecordAggregate(rs), records);
				continue;
			}
			NPOI.HSSF.Record.Record next = rs.GetNext();
			if (num2 == 523)
			{
				continue;
			}
			if (num2 == 94)
			{
				_isUncalced = true;
				continue;
			}
			if (num2 == 2152 || num2 == 2151)
			{
				records.Add(next);
				continue;
			}
			if (num2 == 10)
			{
				records.Add(next);
				break;
			}
			switch (num2)
			{
			case 512:
				if (_columnInfos == null)
				{
					_columnInfos = new ColumnInfoRecordsAggregate();
					records.Add(_columnInfos);
				}
				_dimensions = (DimensionsRecord)next;
				num = records.Count;
				break;
			case 85:
				defaultcolwidth = (DefaultColWidthRecord)next;
				break;
			case 549:
				defaultrowheight = (DefaultRowHeightRecord)next;
				break;
			case 43:
				printGridlines = (PrintGridlinesRecord)next;
				break;
			case 42:
				printHeaders = (PrintHeadersRecord)next;
				break;
			case 130:
				gridset = (GridsetRecord)next;
				break;
			case 29:
				_selection = (SelectionRecord)next;
				break;
			case 574:
				windowTwo = (WindowTwoRecord)next;
				break;
			case 2146:
				sheetext = (SheetExtRecord)next;
				break;
			case 128:
				_gutsRecord = (GutsRecord)next;
				break;
			}
			records.Add(next);
		}
		if (windowTwo == null)
		{
			throw new RecordFormatException("WINDOW2 was not found");
		}
		if (_dimensions == null)
		{
			if (rowRecordsAggregate == null)
			{
				rowRecordsAggregate = new RowRecordsAggregate();
			}
			num = FindFirstRecordLocBySid(574);
			_dimensions = rowRecordsAggregate.CreateDimensions();
			records.Insert(num, _dimensions);
		}
		if (rowRecordsAggregate == null)
		{
			rowRecordsAggregate = new RowRecordsAggregate();
			records.Insert(num + 1, rowRecordsAggregate);
		}
		_rowsAggregate = rowRecordsAggregate;
		RecordOrderer.AddNewSheetRecord(records, _mergedCellsTable);
		RecordOrderer.AddNewSheetRecord(records, _protectionBlock);
	}

	private static void SpillAggregate(RecordAggregate ra, List<RecordBase> recs)
	{
		ra.VisitContainedRecords(new RecordVisitor1(recs));
	}

	public static InternalSheet CreateSheet()
	{
		return new InternalSheet();
	}

	private InternalSheet()
	{
		_mergedCellsTable = new MergedCellsTable();
		records = new List<RecordBase>(32);
		records.Add(CreateBOF());
		records.Add(CreateCalcMode());
		records.Add(CreateCalcCount());
		records.Add(CreateRefMode());
		records.Add(CreateIteration());
		records.Add(CreateDelta());
		records.Add(CreateSaveRecalc());
		printHeaders = CreatePrintHeaders();
		records.Add(printHeaders);
		printGridlines = CreatePrintGridlines();
		records.Add(printGridlines);
		gridset = CreateGridset();
		records.Add(gridset);
		_gutsRecord = CreateGuts();
		records.Add(_gutsRecord);
		defaultrowheight = CreateDefaultRowHeight();
		records.Add(defaultrowheight);
		records.Add(CreateWSBool());
		_psBlock = new PageSettingsBlock();
		records.Add(_psBlock);
		records.Add(_protectionBlock);
		defaultcolwidth = CreateDefaultColWidth();
		records.Add(defaultcolwidth);
		ColumnInfoRecordsAggregate columnInfoRecordsAggregate = new ColumnInfoRecordsAggregate();
		records.Add(columnInfoRecordsAggregate);
		_columnInfos = columnInfoRecordsAggregate;
		_dimensions = CreateDimensions();
		records.Add(_dimensions);
		_rowsAggregate = new RowRecordsAggregate();
		records.Add(_rowsAggregate);
		records.Add(windowTwo = CreateWindowTwo());
		_selection = CreateSelection();
		records.Add(_selection);
		records.Add(_mergedCellsTable);
		sheetext = new SheetExtRecord();
		records.Add(sheetext);
		records.Add(EOFRecord.instance);
	}

	public int AddMergedRegion(int rowFrom, int colFrom, int rowTo, int colTo)
	{
		if (rowTo < rowFrom)
		{
			throw new ArgumentException("The 'to' row (" + rowTo + ") must not be less than the 'from' row (" + rowFrom + ")");
		}
		if (colTo < colFrom)
		{
			throw new ArgumentException("The 'to' col (" + colTo + ") must not be less than the 'from' col (" + colFrom + ")");
		}
		MergedCellsTable mergedRecords = MergedRecords;
		mergedRecords.AddArea(rowFrom, colFrom, rowTo, colTo);
		return mergedRecords.NumberOfMergedRegions - 1;
	}

	public void RemoveMergedRegion(int index)
	{
		MergedCellsTable mergedRecords = MergedRecords;
		if (index < mergedRecords.NumberOfMergedRegions)
		{
			mergedRecords.Remove(index);
		}
	}

	public CellRangeAddress GetMergedRegionAt(int index)
	{
		MergedCellsTable mergedRecords = MergedRecords;
		if (index >= mergedRecords.NumberOfMergedRegions)
		{
			return null;
		}
		return mergedRecords.Get(index);
	}

	public void SetDimensions(int firstrow, short firstcol, int lastrow, short lastcol)
	{
		dims.FirstCol = firstcol;
		dims.FirstRow = firstrow;
		dims.LastCol = lastcol;
		dims.LastRow = lastrow;
	}

	public RowRecord CreateRow(int row)
	{
		return RowRecordsAggregate.CreateRow(row);
	}

	public LabelSSTRecord CreateLabelSST(int row, short col, int index)
	{
		return new LabelSSTRecord
		{
			Row = row,
			Column = col,
			SSTIndex = index,
			XFIndex = 15
		};
	}

	public NumberRecord CreateNumber(int row, short col, double value)
	{
		return new NumberRecord
		{
			Row = row,
			Column = col,
			Value = value,
			XFIndex = 15
		};
	}

	public BlankRecord CreateBlank(int row, short col)
	{
		return new BlankRecord
		{
			Row = row,
			Column = col,
			XFIndex = 15
		};
	}

	public void AddValueRecord(int row, CellValueRecordInterface col)
	{
		DimensionsRecord dimensions = _dimensions;
		if (col.Column >= dimensions.LastCol)
		{
			dimensions.LastCol = (short)(col.Column + 1);
		}
		if (col.Column < dimensions.FirstCol)
		{
			dimensions.FirstCol = col.Column;
		}
		_rowsAggregate.InsertCell(col);
	}

	public void RemoveValueRecord(int row, CellValueRecordInterface col)
	{
		log.Log(1, "Remove value record row " + row);
		_rowsAggregate.RemoveCell(col);
	}

	public void ReplaceValueRecord(CellValueRecordInterface newval)
	{
		_rowsAggregate.RemoveCell(newval);
		_rowsAggregate.InsertCell(newval);
	}

	public void AddRow(RowRecord row)
	{
		DimensionsRecord dimensions = _dimensions;
		if (row.RowNumber >= dimensions.LastRow)
		{
			dimensions.LastRow = row.RowNumber + 1;
		}
		if (row.RowNumber < dimensions.FirstRow)
		{
			dimensions.FirstRow = row.RowNumber;
		}
		RowRecord row2 = _rowsAggregate.GetRow(row.RowNumber);
		if (row2 != null)
		{
			_rowsAggregate.RemoveRow(row2);
		}
		_rowsAggregate.InsertRow(row);
	}

	public void RemoveRow(RowRecord row)
	{
		_rowsAggregate.RemoveRow(row);
	}

	public IEnumerator<CellValueRecordInterface> GetCellValueIterator()
	{
		return _rowsAggregate.GetCellValueEnumerator();
	}

	public RowRecord GetRow(int rownum)
	{
		return _rowsAggregate.GetRow(rownum);
	}

	public static BOFRecord CreateBOF()
	{
		return new BOFRecord
		{
			Version = 1536,
			Type = BOFRecordType.Worksheet,
			Build = 3515,
			BuildYear = 1996,
			HistoryBitMask = 193,
			RequiredVersion = 6
		};
	}

	private static IndexRecord CreateIndex()
	{
		return new IndexRecord
		{
			FirstRow = 0,
			LastRowAdd1 = 0
		};
	}

	private static CalcModeRecord CreateCalcMode()
	{
		CalcModeRecord calcModeRecord = new CalcModeRecord();
		calcModeRecord.SetCalcMode(1);
		return calcModeRecord;
	}

	private static CalcCountRecord CreateCalcCount()
	{
		return new CalcCountRecord
		{
			Iterations = 100
		};
	}

	private static RefModeRecord CreateRefMode()
	{
		return new RefModeRecord
		{
			Mode = 1
		};
	}

	private static IterationRecord CreateIteration()
	{
		return new IterationRecord(iterateOn: false);
	}

	private static DeltaRecord CreateDelta()
	{
		return new DeltaRecord(0.001);
	}

	private static SaveRecalcRecord CreateSaveRecalc()
	{
		return new SaveRecalcRecord
		{
			Recalc = true
		};
	}

	private static PrintHeadersRecord CreatePrintHeaders()
	{
		return new PrintHeadersRecord
		{
			PrintHeaders = false
		};
	}

	private static PrintGridlinesRecord CreatePrintGridlines()
	{
		return new PrintGridlinesRecord
		{
			PrintGridlines = false
		};
	}

	private static GridsetRecord CreateGridset()
	{
		return new GridsetRecord
		{
			Gridset = true
		};
	}

	private static GutsRecord CreateGuts()
	{
		return new GutsRecord
		{
			LeftRowGutter = 0,
			TopColGutter = 0,
			RowLevelMax = 0,
			ColLevelMax = 0
		};
	}

	private static DefaultRowHeightRecord CreateDefaultRowHeight()
	{
		return new DefaultRowHeightRecord
		{
			OptionFlags = 0,
			RowHeight = 255
		};
	}

	private static WSBoolRecord CreateWSBool()
	{
		return new WSBoolRecord
		{
			WSBool1 = 4,
			WSBool2 = 1
		};
	}

	private static HCenterRecord CreateHCenter()
	{
		return new HCenterRecord
		{
			HCenter = false
		};
	}

	private static VCenterRecord CreateVCenter()
	{
		return new VCenterRecord
		{
			VCenter = false
		};
	}

	private static PrintSetupRecord CreatePrintSetup()
	{
		return new PrintSetupRecord
		{
			PaperSize = 1,
			Scale = 100,
			PageStart = 1,
			FitWidth = 1,
			FitHeight = 1,
			Options = 2,
			HResolution = 300,
			VResolution = 300,
			HeaderMargin = 0.5,
			FooterMargin = 0.5,
			Copies = 0
		};
	}

	private static DefaultColWidthRecord CreateDefaultColWidth()
	{
		return new DefaultColWidthRecord
		{
			ColWidth = 8
		};
	}

	public int GetColumnWidth(int columnIndex)
	{
		return _columnInfos.FindColumnInfo(columnIndex)?.ColumnWidth ?? (256 * defaultcolwidth.ColWidth);
	}

	public short GetXFIndexForColAt(short columnIndex)
	{
		ColumnInfoRecord columnInfoRecord = _columnInfos.FindColumnInfo(columnIndex);
		if (columnInfoRecord != null)
		{
			return (short)columnInfoRecord.XFIndex;
		}
		return 15;
	}

	public void SetColumnWidth(int column, int width)
	{
		if (width > 65280)
		{
			throw new ArgumentException("The maximum column width for an individual cell is 255 characters.");
		}
		SetColumn(column, null, width, null, null, null);
	}

	public bool IsColumnHidden(int columnIndex)
	{
		return _columnInfos.FindColumnInfo(columnIndex)?.IsHidden ?? false;
	}

	public void SetColumnHidden(int column, bool hidden)
	{
		SetColumn(column, null, null, null, hidden, null);
	}

	public void SetDefaultColumnStyle(int column, int styleIndex)
	{
		SetColumn(column, (short)styleIndex, null, null, null, null);
	}

	public void SetColumn(int column, int width, int level, bool hidden, bool collapsed)
	{
		_columnInfos.SetColumn(column, 0, width, level, hidden, collapsed);
	}

	public void SetColumn(int column, short? xfStyle, int? width, int? level, bool? hidden, bool? collapsed)
	{
		_columnInfos.SetColumn(column, xfStyle, width, level, hidden, collapsed);
	}

	private GutsRecord GetGutsRecord()
	{
		if (_gutsRecord == null)
		{
			GutsRecord gutsRecord = CreateGuts();
			RecordOrderer.AddNewSheetRecord(records, gutsRecord);
			_gutsRecord = gutsRecord;
		}
		return _gutsRecord;
	}

	public void GroupColumnRange(int fromColumn, int toColumn, bool indent)
	{
		_columnInfos.GroupColumnRange(fromColumn, toColumn, indent);
		int maxOutlineLevel = _columnInfos.MaxOutlineLevel;
		GutsRecord gutsRecord = GetGutsRecord();
		gutsRecord.ColLevelMax = (short)(maxOutlineLevel + 1);
		if (maxOutlineLevel == 0)
		{
			gutsRecord.TopColGutter = 0;
		}
		else
		{
			gutsRecord.TopColGutter = (short)(29 + 12 * (maxOutlineLevel - 1));
		}
	}

	private static DimensionsRecord CreateDimensions()
	{
		return new DimensionsRecord
		{
			FirstCol = 0,
			LastRow = 1,
			FirstRow = 0,
			LastCol = 1
		};
	}

	private static WindowTwoRecord CreateWindowTwo()
	{
		return new WindowTwoRecord
		{
			Options = 1718,
			TopRow = 0,
			LeftCol = 0,
			HeaderColor = 64,
			PageBreakZoom = 0,
			NormalZoom = 0
		};
	}

	private static SelectionRecord CreateSelection()
	{
		return new SelectionRecord(0, 0);
	}

	public void SetActiveCell(int row, int column)
	{
		SetActiveCellRange(row, row, column, column);
	}

	public void SetActiveCellRange(int firstRow, int lastRow, int firstColumn, int lastColumn)
	{
		List<CellRangeAddress8Bit> list = new List<CellRangeAddress8Bit>();
		list.Add(new CellRangeAddress8Bit(firstRow, lastRow, firstColumn, lastColumn));
		SetActiveCellRange(list, 0, firstRow, firstColumn);
	}

	public void SetActiveCellRange(List<CellRangeAddress8Bit> cellranges, int activeRange, int activeRow, int activeColumn)
	{
		_selection.ActiveCellCol = activeColumn;
		_selection.ActiveCellRow = activeRow;
		_selection.ActiveCellRef = activeRange;
		_selection.CellReferences = cellranges.ToArray();
	}

	protected NPOI.HSSF.Record.Record CreateEOF()
	{
		return new EOFRecord();
	}

	public NPOI.HSSF.Record.Record FindFirstRecordBySid(short sid)
	{
		int num = FindFirstRecordLocBySid(sid);
		if (num < 0)
		{
			return null;
		}
		return (NPOI.HSSF.Record.Record)records[num];
	}

	public void SetSCLRecord(SCLRecord sclRecord)
	{
		int num = FindFirstRecordLocBySid(160);
		if (num == -1)
		{
			int num2 = FindFirstRecordLocBySid(574);
			records.Insert(num2 + 1, sclRecord);
		}
		else
		{
			records[num] = sclRecord;
		}
	}

	public int FindFirstRecordLocBySid(short sid)
	{
		int count = records.Count;
		for (int i = 0; i < count; i++)
		{
			object obj = records[i];
			if (obj is NPOI.HSSF.Record.Record && ((NPOI.HSSF.Record.Record)obj).Sid == sid)
			{
				return i;
			}
		}
		return -1;
	}

	public WindowTwoRecord GetWindowTwo()
	{
		return windowTwo;
	}

	public void SetSelected(bool sel)
	{
		windowTwo.IsSelected = sel;
	}

	public void CreateFreezePane(int colSplit, int rowSplit, int topRow, int leftmostColumn)
	{
		int num = FindFirstRecordLocBySid(65);
		if (num != -1)
		{
			records.RemoveAt(num);
		}
		if (colSplit == 0 && rowSplit == 0)
		{
			windowTwo.FreezePanes = false;
			windowTwo.FreezePanesNoSplit = false;
			((SelectionRecord)FindFirstRecordBySid(29)).Pane = 3;
			return;
		}
		int num2 = FindFirstRecordLocBySid(574);
		PaneRecord paneRecord = new PaneRecord();
		paneRecord.X = (short)colSplit;
		paneRecord.Y = (short)rowSplit;
		paneRecord.TopRow = (short)topRow;
		paneRecord.LeftColumn = (short)leftmostColumn;
		if (rowSplit == 0)
		{
			paneRecord.TopRow = 0;
			paneRecord.ActivePane = 1;
		}
		else if (colSplit == 0)
		{
			paneRecord.LeftColumn = 0;
			paneRecord.ActivePane = 2;
		}
		else
		{
			paneRecord.ActivePane = 0;
		}
		records.Insert(num2 + 1, paneRecord);
		windowTwo.FreezePanes = true;
		windowTwo.FreezePanesNoSplit = true;
		((SelectionRecord)FindFirstRecordBySid(29)).Pane = (byte)paneRecord.ActivePane;
	}

	public void CreateSplitPane(int xSplitPos, int ySplitPos, int topRow, int leftmostColumn, PanePosition activePane)
	{
		int num = FindFirstRecordLocBySid(65);
		if (num != -1)
		{
			records.RemoveAt(num);
		}
		int num2 = FindFirstRecordLocBySid(574);
		PaneRecord paneRecord = new PaneRecord();
		paneRecord.X = (short)xSplitPos;
		paneRecord.Y = (short)ySplitPos;
		paneRecord.TopRow = (short)topRow;
		paneRecord.LeftColumn = (short)leftmostColumn;
		paneRecord.ActivePane = (short)activePane;
		records.Insert(num2 + 1, paneRecord);
		windowTwo.FreezePanes = false;
		windowTwo.FreezePanesNoSplit = false;
		((SelectionRecord)FindFirstRecordBySid(29)).Pane = 0;
	}

	protected static PasswordRecord CreatePassword()
	{
		return new PasswordRecord(0);
	}

	protected ProtectRecord CreateProtect()
	{
		return new ProtectRecord(isProtected: false);
	}

	protected ObjectProtectRecord CreateObjectProtect()
	{
		return new ObjectProtectRecord
		{
			Protect = false
		};
	}

	protected ScenarioProtectRecord CreateScenarioProtect()
	{
		return new ScenarioProtectRecord
		{
			Protect = false
		};
	}

	public int AggregateDrawingRecords(DrawingManager2 drawingManager, bool CreateIfMissing)
	{
		int num = FindFirstRecordLocBySid(236);
		if (num == -1)
		{
			if (!CreateIfMissing)
			{
				return -1;
			}
			EscherAggregate item = new EscherAggregate(createDefaultTree: true);
			num = FindFirstRecordLocBySid(9876);
			if (num == -1)
			{
				num = FindFirstRecordLocBySid(574);
			}
			else
			{
				Records.RemoveAt(num);
			}
			Records.Insert(num, item);
			return num;
		}
		EscherAggregate.CreateAggregate(records, num);
		return num;
	}

	public void Preserialize()
	{
		IEnumerator enumerator = Records.GetEnumerator();
		while (enumerator.MoveNext())
		{
			RecordBase recordBase = (RecordBase)enumerator.Current;
			if (recordBase is EscherAggregate)
			{
				_ = recordBase.RecordSize;
			}
		}
	}

	public void ShiftBreaks(PageBreakRecord breaks, short start, short stop, int count)
	{
		if (rowBreaks == null)
		{
			return;
		}
		IEnumerator breaksEnumerator = breaks.GetBreaksEnumerator();
		IList list = new ArrayList();
		while (breaksEnumerator.MoveNext())
		{
			PageBreakRecord.Break obj = (PageBreakRecord.Break)breaksEnumerator.Current;
			int main = obj.main;
			bool flag = main >= start;
			bool flag2 = main <= stop;
			if (flag & flag2)
			{
				list.Add(obj);
			}
		}
		breaksEnumerator = list.GetEnumerator();
		while (breaksEnumerator.MoveNext())
		{
			PageBreakRecord.Break obj2 = (PageBreakRecord.Break)breaksEnumerator.Current;
			breaks.RemoveBreak(obj2.main);
			breaks.AddBreak(obj2.main + count, obj2.subFrom, obj2.subTo);
		}
	}

	public void ShiftRowBreaks(int startingRow, int endingRow, int count)
	{
		ShiftBreaks(rowBreaks, (short)startingRow, (short)endingRow, (short)count);
	}

	public void ShiftColumnBreaks(short startingCol, short endingCol, short count)
	{
		ShiftBreaks(colBreaks, startingCol, endingCol, count);
	}

	public void SetColumnGroupCollapsed(int columnNumber, bool collapsed)
	{
		if (collapsed)
		{
			_columnInfos.CollapseColumn(columnNumber);
		}
		else
		{
			_columnInfos.ExpandColumn(columnNumber);
		}
	}

	public void UpdateFormulasAfterCellShift(FormulaShifter shifter, int externSheetIndex)
	{
		RowsAggregate.UpdateFormulasAfterRowShift(shifter, externSheetIndex);
		if (condFormatting != null)
		{
			ConditionalFormattingTable.UpdateFormulasAfterCellShift(shifter, externSheetIndex);
		}
	}

	public void VisitContainedRecords(RecordVisitor rv, int offset)
	{
		PositionTrackingVisitor positionTrackingVisitor = new PositionTrackingVisitor(rv, offset);
		bool flag = false;
		int num = offset;
		for (int i = 0; i < records.Count; i++)
		{
			RecordBase recordBase = records[i];
			if (recordBase is RecordAggregate)
			{
				RecordAggregate recordAggregate = (RecordAggregate)recordBase;
				recordAggregate.VisitContainedRecords(positionTrackingVisitor);
				num += recordAggregate.RecordSize;
			}
			else
			{
				if (recordBase is DefaultColWidthRecord)
				{
					((DefaultColWidthRecord)recordBase).offsetForFilePointer = num;
				}
				positionTrackingVisitor.VisitRecord((NPOI.HSSF.Record.Record)recordBase);
				num += recordBase.RecordSize;
			}
			if (recordBase is BOFRecord && !flag)
			{
				flag = true;
				if (_isUncalced)
				{
					UncalcedRecord uncalcedRecord = new UncalcedRecord();
					positionTrackingVisitor.VisitRecord(uncalcedRecord);
					num += uncalcedRecord.RecordSize;
				}
				if (_rowsAggregate != null)
				{
					int sizeOfInitialSheetRecords = GetSizeOfInitialSheetRecords(i);
					int position = positionTrackingVisitor.Position;
					IndexRecord indexRecord = _rowsAggregate.CreateIndexRecord(position, sizeOfInitialSheetRecords, 0);
					positionTrackingVisitor.VisitRecord(indexRecord);
					num += indexRecord.RecordSize;
				}
			}
		}
	}

	private int GetSizeOfInitialSheetRecords(int bofRecordIndex)
	{
		int num = 0;
		for (int i = bofRecordIndex + 1; i < records.Count; i++)
		{
			RecordBase recordBase = records[i];
			if (recordBase is RowRecordsAggregate)
			{
				break;
			}
			num += recordBase.RecordSize;
		}
		if (_isUncalced)
		{
			num += UncalcedRecord.StaticRecordSize;
		}
		return num;
	}

	public void GroupRowRange(int fromRow, int toRow, bool indent)
	{
		for (int i = fromRow; i <= toRow; i++)
		{
			RowRecord rowRecord = GetRow(i);
			if (rowRecord == null)
			{
				rowRecord = CreateRow(i);
				AddRow(rowRecord);
			}
			int outlineLevel = rowRecord.OutlineLevel;
			outlineLevel = ((!indent) ? (outlineLevel - 1) : (outlineLevel + 1));
			outlineLevel = Math.Max(0, outlineLevel);
			outlineLevel = Math.Min(7, outlineLevel);
			rowRecord.OutlineLevel = (short)outlineLevel;
		}
		RecalcRowGutter();
	}

	private void RecalcRowGutter()
	{
		int num = 0;
		IEnumerator enumerator = _rowsAggregate.GetEnumerator();
		while (enumerator.MoveNext())
		{
			num = Math.Max(((RowRecord)enumerator.Current).OutlineLevel, num);
		}
		GutsRecord gutsRecord = GetGutsRecord();
		if (gutsRecord == null)
		{
			gutsRecord = new GutsRecord();
			records.Add(gutsRecord);
		}
		gutsRecord.RowLevelMax = (short)(num + 1);
		gutsRecord.LeftRowGutter = (short)(29 + 12 * num);
	}

	public DataValidityTable GetOrCreateDataValidityTable()
	{
		if (_dataValidityTable == null)
		{
			_dataValidityTable = new DataValidityTable();
			RecordOrderer.AddNewSheetRecord(records, _dataValidityTable);
		}
		return _dataValidityTable;
	}

	public NoteRecord[] GetNoteRecords()
	{
		List<NoteRecord> list = new List<NoteRecord>();
		for (int num = records.Count - 1; num >= 0; num--)
		{
			RecordBase recordBase = records[num];
			if (recordBase is NoteRecord)
			{
				list.Add((NoteRecord)recordBase);
			}
		}
		if (list.Count < 1)
		{
			return NoteRecord.EMPTY_ARRAY;
		}
		_ = new NoteRecord[list.Count];
		return list.ToArray();
	}

	public int GetColumnOutlineLevel(int columnIndex)
	{
		return _columnInfos.GetOutlineLevel(columnIndex);
	}
}

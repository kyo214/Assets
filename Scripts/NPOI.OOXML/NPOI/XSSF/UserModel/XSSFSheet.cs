using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using NPOI.OpenXml4Net.Exceptions;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats;
using NPOI.OpenXmlFormats.Dml.Spreadsheet;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.POIFS.Crypt;
using NPOI.SS;
using NPOI.SS.Formula;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;
using NPOI.XSSF.Model;
using NPOI.XSSF.UserModel.Helpers;

namespace NPOI.XSSF.UserModel;

public class XSSFSheet : POIXMLDocumentPart, ISheet
{
	private class ShiftCommentComparator : IComparer<XSSFComment>
	{
		private int shiftDir;

		public ShiftCommentComparator(int shiftDir)
		{
			this.shiftDir = shiftDir;
		}

		public int Compare(XSSFComment o1, XSSFComment o2)
		{
			int row = o1.Row;
			int row2 = o2.Row;
			if (row == row2)
			{
				return o1.GetHashCode() - o2.GetHashCode();
			}
			if (shiftDir > 0)
			{
				if (row >= row2)
				{
					return -1;
				}
				return 1;
			}
			if (row <= row2)
			{
				return -1;
			}
			return 1;
		}
	}

	public class PivotTableReferenceConfigurator1 : XSSFPivotTable.IPivotTableReferenceConfigurator
	{
		private AreaReference source;

		public PivotTableReferenceConfigurator1(AreaReference source)
		{
			this.source = source;
		}

		public void ConfigureReference(CT_WorksheetSource wsSource)
		{
			string[] cellRefParts = source.FirstCell.CellRefParts;
			string text = cellRefParts[1];
			string text2 = cellRefParts[2];
			string[] cellRefParts2 = source.LastCell.CellRefParts;
			string text3 = cellRefParts2[1];
			string text4 = cellRefParts2[2];
			string text5 = text2 + text + ":" + text4 + text3;
			wsSource.@ref = text5;
		}
	}

	public class PivotTableReferenceConfigurator2 : XSSFPivotTable.IPivotTableReferenceConfigurator
	{
		private IName source;

		public PivotTableReferenceConfigurator2(IName source)
		{
			this.source = source;
		}

		public void ConfigureReference(CT_WorksheetSource wsSource)
		{
			wsSource.name = source.NameName;
		}
	}

	public class PivotTableReferenceConfigurator3 : XSSFPivotTable.IPivotTableReferenceConfigurator
	{
		private ITable source;

		public PivotTableReferenceConfigurator3(ITable source)
		{
			this.source = source;
		}

		public void ConfigureReference(CT_WorksheetSource wsSource)
		{
			wsSource.name = source.Name;
		}
	}

	private static POILogger logger = POILogFactory.GetLogger(typeof(XSSFSheet));

	private static double DEFAULT_ROW_HEIGHT = 15.0;

	private static double DEFAULT_MARGIN_HEADER = 0.3;

	private static double DEFAULT_MARGIN_FOOTER = 0.3;

	private static double DEFAULT_MARGIN_TOP = 0.75;

	private static double DEFAULT_MARGIN_BOTTOM = 0.75;

	private static double DEFAULT_MARGIN_LEFT = 0.7;

	private static double DEFAULT_MARGIN_RIGHT = 0.7;

	public static int TWIPS_PER_POINT = 20;

	internal CT_Sheet sheet;

	internal CT_Worksheet worksheet;

	private SortedList<int, XSSFRow> _rows = new SortedList<int, XSSFRow>();

	private List<XSSFHyperlink> hyperlinks;

	private ColumnHelper columnHelper;

	private CommentsTable sheetComments;

	private Dictionary<int, CT_CellFormula> sharedFormulas;

	private Dictionary<string, XSSFTable> tables;

	private List<CellRangeAddress> arrayFormulas;

	private XSSFDataValidationHelper dataValidationHelper;

	private XSSFDrawing drawing;

	public IWorkbook Workbook => (XSSFWorkbook)GetParent();

	public string SheetName => sheet.name;

	public int[] ColumnBreaks
	{
		get
		{
			if (!worksheet.IsSetColBreaks() || worksheet.colBreaks.sizeOfBrkArray() == 0)
			{
				return new int[0];
			}
			List<CT_Break> brk = worksheet.colBreaks.brk;
			int[] array = new int[brk.Count];
			for (int i = 0; i < brk.Count; i++)
			{
				CT_Break cT_Break = brk[i];
				array[i] = (int)(cT_Break.id - 1);
			}
			return array;
		}
	}

	public int DefaultColumnWidth
	{
		get
		{
			return (int)(worksheet.sheetFormatPr?.baseColWidth ?? 8);
		}
		set
		{
			GetSheetTypeSheetFormatPr().baseColWidth = (uint)value;
		}
	}

	public short DefaultRowHeight
	{
		get
		{
			return (short)((decimal)DefaultRowHeightInPoints * (decimal)TWIPS_PER_POINT);
		}
		set
		{
			DefaultRowHeightInPoints = (float)value / (float)TWIPS_PER_POINT;
		}
	}

	public float DefaultRowHeightInPoints
	{
		get
		{
			return (float)(worksheet.sheetFormatPr?.defaultRowHeight ?? 0.0);
		}
		set
		{
			CT_SheetFormatPr sheetTypeSheetFormatPr = GetSheetTypeSheetFormatPr();
			sheetTypeSheetFormatPr.defaultRowHeight = value;
			sheetTypeSheetFormatPr.customHeight = true;
		}
	}

	public bool RightToLeft
	{
		get
		{
			return GetDefaultSheetView()?.rightToLeft ?? false;
		}
		set
		{
			GetDefaultSheetView().rightToLeft = value;
		}
	}

	public bool DisplayGuts
	{
		get
		{
			CT_SheetPr sheetTypeSheetPr = GetSheetTypeSheetPr();
			return ((sheetTypeSheetPr.outlinePr == null) ? new CT_OutlinePr() : sheetTypeSheetPr.outlinePr).showOutlineSymbols;
		}
		set
		{
			CT_SheetPr sheetTypeSheetPr = GetSheetTypeSheetPr();
			((sheetTypeSheetPr.outlinePr == null) ? sheetTypeSheetPr.AddNewOutlinePr() : sheetTypeSheetPr.outlinePr).showOutlineSymbols = value;
		}
	}

	public bool DisplayZeros
	{
		get
		{
			return GetDefaultSheetView()?.showZeros ?? true;
		}
		set
		{
			GetSheetTypeSheetView().showZeros = value;
		}
	}

	public int FirstRowNum
	{
		get
		{
			if (_rows.Count == 0)
			{
				return 0;
			}
			using (IEnumerator<int> enumerator = _rows.Keys.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current;
				}
			}
			throw new ArgumentOutOfRangeException();
		}
	}

	public bool FitToPage
	{
		get
		{
			CT_SheetPr sheetTypeSheetPr = GetSheetTypeSheetPr();
			return ((sheetTypeSheetPr == null || !sheetTypeSheetPr.IsSetPageSetUpPr()) ? new CT_PageSetUpPr() : sheetTypeSheetPr.pageSetUpPr).fitToPage;
		}
		set
		{
			GetSheetTypePageSetUpPr().fitToPage = value;
		}
	}

	public IFooter Footer => OddFooter;

	public IHeader Header => OddHeader;

	public IFooter OddFooter => new XSSFOddFooter(GetSheetTypeHeaderFooter());

	public IFooter EvenFooter => new XSSFEvenFooter(GetSheetTypeHeaderFooter());

	public IFooter FirstFooter => new XSSFFirstFooter(GetSheetTypeHeaderFooter());

	public IHeader OddHeader => new XSSFOddHeader(GetSheetTypeHeaderFooter());

	public IHeader EvenHeader => new XSSFEvenHeader(GetSheetTypeHeaderFooter());

	public IHeader FirstHeader => new XSSFFirstHeader(GetSheetTypeHeaderFooter());

	public bool HorizontallyCenter
	{
		get
		{
			return worksheet.printOptions?.horizontalCentered ?? false;
		}
		set
		{
			(worksheet.IsSetPrintOptions() ? worksheet.printOptions : worksheet.AddNewPrintOptions()).horizontalCentered = value;
		}
	}

	public int LastRowNum
	{
		get
		{
			if (_rows.Count != 0)
			{
				return GetLastKey(_rows.Keys);
			}
			return 0;
		}
	}

	public List<CellRangeAddress> MergedRegions
	{
		get
		{
			List<CellRangeAddress> list = new List<CellRangeAddress>();
			CT_MergeCells mergeCells = worksheet.mergeCells;
			if (mergeCells == null)
			{
				return list;
			}
			foreach (CT_MergeCell item in mergeCells.mergeCell)
			{
				string reference = item.@ref;
				list.Add(CellRangeAddress.ValueOf(reference));
			}
			return list;
		}
	}

	public int NumMergedRegions => worksheet.mergeCells?.sizeOfMergeCellArray() ?? 0;

	public int NumHyperlinks => hyperlinks.Count;

	public PaneInformation PaneInformation
	{
		get
		{
			CT_Pane pane = GetDefaultSheetView().pane;
			if (pane == null)
			{
				return null;
			}
			CellReference cellReference = (pane.IsSetTopLeftCell() ? new CellReference(pane.topLeftCell) : null);
			return new PaneInformation((short)pane.xSplit, (short)pane.ySplit, (short)((cellReference != null) ? ((short)cellReference.Row) : 0), cellReference?.Col ?? 0, (byte)pane.activePane, pane.state == ST_PaneState.frozen);
		}
	}

	public int PhysicalNumberOfRows => _rows.Count;

	public IPrintSetup PrintSetup => new XSSFPrintSetup(worksheet);

	public bool Protect => IsSheetLocked;

	public int[] RowBreaks
	{
		get
		{
			if (!worksheet.IsSetRowBreaks() || worksheet.rowBreaks.sizeOfBrkArray() == 0)
			{
				return new int[0];
			}
			List<CT_Break> brk = worksheet.rowBreaks.brk;
			int[] array = new int[brk.Count];
			for (int i = 0; i < brk.Count; i++)
			{
				CT_Break cT_Break = brk[i];
				array[i] = (int)(cT_Break.id - 1);
			}
			return array;
		}
	}

	public bool RowSumsBelow
	{
		get
		{
			CT_SheetPr sheetPr = worksheet.sheetPr;
			return ((sheetPr != null && sheetPr.IsSetOutlinePr()) ? sheetPr.outlinePr : null)?.summaryBelow ?? true;
		}
		set
		{
			ensureOutlinePr().summaryBelow = value;
		}
	}

	public bool RowSumsRight
	{
		get
		{
			CT_SheetPr sheetPr = worksheet.sheetPr;
			return ((sheetPr != null && sheetPr.IsSetOutlinePr()) ? sheetPr.outlinePr : new CT_OutlinePr()).summaryRight;
		}
		set
		{
			ensureOutlinePr().summaryRight = value;
		}
	}

	public bool ScenarioProtect
	{
		get
		{
			if (worksheet.IsSetSheetProtection())
			{
				return worksheet.sheetProtection.scenarios;
			}
			return false;
		}
	}

	public short LeftCol
	{
		get
		{
			string topLeftCell = GetPane().topLeftCell;
			if (topLeftCell == null)
			{
				return 0;
			}
			return new CellReference(topLeftCell).Col;
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public short TopRow
	{
		get
		{
			string topLeftCell = GetSheetTypeSheetView().topLeftCell;
			if (topLeftCell == null)
			{
				return 0;
			}
			return (short)new CellReference(topLeftCell).Row;
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public bool VerticallyCenter
	{
		get
		{
			return worksheet.printOptions?.verticalCentered ?? false;
		}
		set
		{
			(worksheet.IsSetPrintOptions() ? worksheet.printOptions : worksheet.AddNewPrintOptions()).verticalCentered = value;
		}
	}

	public bool DisplayFormulas
	{
		get
		{
			return GetSheetTypeSheetView().showFormulas;
		}
		set
		{
			GetSheetTypeSheetView().showFormulas = value;
		}
	}

	public bool DisplayGridlines
	{
		get
		{
			return GetSheetTypeSheetView().showGridLines;
		}
		set
		{
			GetSheetTypeSheetView().showGridLines = value;
		}
	}

	public bool DisplayRowColHeadings
	{
		get
		{
			return GetSheetTypeSheetView().showRowColHeaders;
		}
		set
		{
			GetSheetTypeSheetView().showRowColHeaders = value;
		}
	}

	public bool IsPrintGridlines
	{
		get
		{
			return worksheet.printOptions?.gridLines ?? false;
		}
		set
		{
			(worksheet.IsSetPrintOptions() ? worksheet.printOptions : worksheet.AddNewPrintOptions()).gridLines = value;
		}
	}

	public bool IsPrintRowAndColumnHeadings
	{
		get
		{
			return worksheet.printOptions?.headings ?? false;
		}
		set
		{
			(worksheet.IsSetPrintOptions() ? worksheet.printOptions : worksheet.AddNewPrintOptions()).headings = value;
		}
	}

	public bool ForceFormulaRecalculation
	{
		get
		{
			if (worksheet.IsSetSheetCalcPr())
			{
				return worksheet.sheetCalcPr.fullCalcOnLoad;
			}
			return false;
		}
		set
		{
			CT_CalcPr calcPr = (Workbook as XSSFWorkbook).GetCTWorkbook().calcPr;
			if (worksheet.IsSetSheetCalcPr())
			{
				worksheet.sheetCalcPr.fullCalcOnLoad = value;
			}
			else if (value)
			{
				worksheet.AddNewSheetCalcPr().fullCalcOnLoad = value;
			}
			if (value && calcPr != null && calcPr.calcMode == ST_CalcMode.manual)
			{
				calcPr.calcMode = ST_CalcMode.auto;
			}
		}
	}

	public bool Autobreaks
	{
		get
		{
			CT_SheetPr sheetTypeSheetPr = GetSheetTypeSheetPr();
			return ((sheetTypeSheetPr == null || !sheetTypeSheetPr.IsSetPageSetUpPr()) ? new CT_PageSetUpPr() : sheetTypeSheetPr.pageSetUpPr).autoPageBreaks;
		}
		set
		{
			CT_SheetPr sheetTypeSheetPr = GetSheetTypeSheetPr();
			(sheetTypeSheetPr.IsSetPageSetUpPr() ? sheetTypeSheetPr.pageSetUpPr : sheetTypeSheetPr.AddNewPageSetUpPr()).autoPageBreaks = value;
		}
	}

	public bool IsSelected
	{
		get
		{
			return GetDefaultSheetView()?.tabSelected ?? false;
		}
		set
		{
			foreach (CT_SheetView item in GetSheetTypeSheetViews().sheetView)
			{
				item.tabSelected = value;
			}
		}
	}

	public CellAddress ActiveCell
	{
		get
		{
			string activeCell = GetSheetTypeSelection().activeCell;
			if (activeCell == null)
			{
				return null;
			}
			return new CellAddress(activeCell);
		}
		set
		{
			string text = value.FormatAsString();
			CT_Selection sheetTypeSelection = GetSheetTypeSelection();
			sheetTypeSelection.activeCell = text;
			sheetTypeSelection.SetSqref(new string[1] { text });
		}
	}

	public bool HasComments
	{
		get
		{
			if (sheetComments == null)
			{
				return false;
			}
			return sheetComments.GetNumberOfComments() > 0;
		}
	}

	internal int NumberOfComments
	{
		get
		{
			if (sheetComments == null)
			{
				return 0;
			}
			return sheetComments.GetNumberOfComments();
		}
	}

	public bool IsAutoFilterLocked
	{
		get
		{
			if (IsSheetLocked)
			{
				return SafeGetProtectionField().autoFilter;
			}
			return false;
		}
	}

	public bool IsDeleteColumnsLocked
	{
		get
		{
			if (IsSheetLocked)
			{
				return SafeGetProtectionField().deleteColumns;
			}
			return false;
		}
	}

	public bool IsDeleteRowsLocked
	{
		get
		{
			if (IsSheetLocked)
			{
				return SafeGetProtectionField().deleteRows;
			}
			return false;
		}
	}

	public bool IsFormatCellsLocked
	{
		get
		{
			if (IsSheetLocked)
			{
				return SafeGetProtectionField().formatCells;
			}
			return false;
		}
	}

	public bool IsFormatColumnsLocked
	{
		get
		{
			if (IsSheetLocked)
			{
				return SafeGetProtectionField().formatColumns;
			}
			return false;
		}
	}

	public bool IsFormatRowsLocked
	{
		get
		{
			if (IsSheetLocked)
			{
				return SafeGetProtectionField().formatRows;
			}
			return false;
		}
	}

	public bool IsInsertColumnsLocked
	{
		get
		{
			if (IsSheetLocked)
			{
				return SafeGetProtectionField().insertColumns;
			}
			return false;
		}
	}

	public bool IsInsertHyperlinksLocked
	{
		get
		{
			if (IsSheetLocked)
			{
				return SafeGetProtectionField().insertHyperlinks;
			}
			return false;
		}
	}

	public bool IsInsertRowsLocked
	{
		get
		{
			if (IsSheetLocked)
			{
				return SafeGetProtectionField().insertRows;
			}
			return false;
		}
	}

	public bool IsPivotTablesLocked
	{
		get
		{
			if (IsSheetLocked)
			{
				return SafeGetProtectionField().pivotTables;
			}
			return false;
		}
	}

	public bool IsSortLocked
	{
		get
		{
			if (IsSheetLocked)
			{
				return SafeGetProtectionField().sort;
			}
			return false;
		}
	}

	public bool IsObjectsLocked
	{
		get
		{
			if (IsSheetLocked)
			{
				return SafeGetProtectionField().objects;
			}
			return false;
		}
	}

	public bool IsScenariosLocked
	{
		get
		{
			if (IsSheetLocked)
			{
				return SafeGetProtectionField().scenarios;
			}
			return false;
		}
	}

	public bool IsSelectLockedCellsLocked
	{
		get
		{
			if (IsSheetLocked)
			{
				return SafeGetProtectionField().selectLockedCells;
			}
			return false;
		}
	}

	public bool IsSelectUnlockedCellsLocked
	{
		get
		{
			if (IsSheetLocked)
			{
				return SafeGetProtectionField().selectUnlockedCells;
			}
			return false;
		}
	}

	public bool IsSheetLocked
	{
		get
		{
			if (worksheet.IsSetSheetProtection())
			{
				return SafeGetProtectionField().sheet;
			}
			return false;
		}
	}

	public ISheetConditionalFormatting SheetConditionalFormatting => new XSSFSheetConditionalFormatting(this);

	public XSSFColor TabColor
	{
		get
		{
			CT_SheetPr cT_SheetPr = worksheet.sheetPr;
			if (cT_SheetPr == null)
			{
				cT_SheetPr = worksheet.AddNewSheetPr();
			}
			if (!cT_SheetPr.IsSetTabColor())
			{
				return null;
			}
			return new XSSFColor(cT_SheetPr.tabColor);
		}
		set
		{
			CT_SheetPr cT_SheetPr = worksheet.sheetPr;
			if (cT_SheetPr == null)
			{
				cT_SheetPr = worksheet.AddNewSheetPr();
			}
			cT_SheetPr.tabColor = value.GetCTColor();
		}
	}

	public IDrawing DrawingPatriarch
	{
		get
		{
			if (drawing == null)
			{
				NPOI.OpenXmlFormats.Spreadsheet.CT_Drawing cTDrawing = GetCTDrawing();
				if (cTDrawing == null)
				{
					return null;
				}
				foreach (RelationPart relationPart in base.RelationParts)
				{
					POIXMLDocumentPart documentPart = relationPart.DocumentPart;
					if (documentPart is XSSFDrawing)
					{
						XSSFDrawing xSSFDrawing = (XSSFDrawing)documentPart;
						if (relationPart.Relationship.Id.Equals(cTDrawing.id))
						{
							drawing = xSSFDrawing;
						}
						break;
					}
				}
			}
			return drawing;
		}
	}

	public bool IsActive
	{
		get
		{
			return IsSelected;
		}
		set
		{
			IsSelected = value;
		}
	}

	public short TabColorIndex
	{
		get
		{
			throw new NotImplementedException("Use XSSFSheet.TabColor instead");
		}
		set
		{
			throw new NotImplementedException("Use XSSFSheet.TabColor instead");
		}
	}

	public bool IsRightToLeft
	{
		get
		{
			return GetDefaultSheetView()?.rightToLeft ?? false;
		}
		set
		{
			GetDefaultSheetView().rightToLeft = value;
		}
	}

	public CellRangeAddress RepeatingRows
	{
		get
		{
			return GetRepeatingRowsOrColums(rows: true);
		}
		set
		{
			CellRangeAddress repeatingColumns = RepeatingColumns;
			SetRepeatingRowsAndColumns(value, repeatingColumns);
		}
	}

	public CellRangeAddress RepeatingColumns
	{
		get
		{
			return GetRepeatingRowsOrColums(rows: false);
		}
		set
		{
			CellRangeAddress repeatingRows = RepeatingRows;
			SetRepeatingRowsAndColumns(repeatingRows, value);
		}
	}

	private CT_Pane Pane
	{
		get
		{
			if (GetDefaultSheetView().pane == null)
			{
				GetDefaultSheetView().AddNewPane();
			}
			return GetDefaultSheetView().pane;
		}
	}

	public XSSFSheet()
	{
		dataValidationHelper = new XSSFDataValidationHelper(this);
		OnDocumentCreate();
	}

	protected internal XSSFSheet(PackagePart part)
		: base(part)
	{
		dataValidationHelper = new XSSFDataValidationHelper(this);
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	internal XSSFSheet(PackagePart part, PackageRelationship rel)
		: this(part)
	{
	}

	internal override void OnDocumentRead()
	{
		try
		{
			Read(GetPackagePart().GetInputStream());
		}
		catch (IOException ex)
		{
			throw new POIXMLException(ex);
		}
	}

	internal virtual void Read(Stream is1)
	{
		try
		{
			XmlDocument xmldoc = POIXMLDocumentPart.ConvertStreamToXml(is1);
			worksheet = WorksheetDocument.Parse(xmldoc, POIXMLDocumentPart.NamespaceManager).GetWorksheet();
		}
		catch (XmlException ex)
		{
			throw new POIXMLException(ex);
		}
		InitRows(worksheet);
		columnHelper = new ColumnHelper(worksheet);
		foreach (RelationPart relationPart in base.RelationParts)
		{
			POIXMLDocumentPart documentPart = relationPart.DocumentPart;
			if (documentPart is CommentsTable)
			{
				sheetComments = (CommentsTable)documentPart;
			}
			if (documentPart is XSSFTable)
			{
				tables[relationPart.Relationship.Id] = (XSSFTable)documentPart;
			}
			if (documentPart is XSSFPivotTable)
			{
				GetWorkbook().PivotTables.Add((XSSFPivotTable)documentPart);
			}
		}
		InitHyperlinks();
	}

	internal override void OnDocumentCreate()
	{
		worksheet = NewSheet();
		InitRows(worksheet);
		columnHelper = new ColumnHelper(worksheet);
		hyperlinks = new List<XSSFHyperlink>();
	}

	private void InitRows(CT_Worksheet worksheetParam)
	{
		_rows.Clear();
		tables = new Dictionary<string, XSSFTable>();
		sharedFormulas = new Dictionary<int, CT_CellFormula>();
		arrayFormulas = new List<CellRangeAddress>();
		if (0 >= worksheetParam.sheetData.SizeOfRowArray())
		{
			return;
		}
		foreach (CT_Row item in worksheetParam.sheetData.row)
		{
			XSSFRow xSSFRow = new XSSFRow(item, this);
			if (!_rows.ContainsKey(xSSFRow.RowNum))
			{
				_rows.Add(xSSFRow.RowNum, xSSFRow);
			}
		}
	}

	private void InitHyperlinks()
	{
		hyperlinks = new List<XSSFHyperlink>();
		if (!worksheet.IsSetHyperlinks())
		{
			return;
		}
		try
		{
			PackageRelationshipCollection relationshipsByType = GetPackagePart().GetRelationshipsByType(XSSFRelation.SHEET_HYPERLINKS.Relation);
			foreach (CT_Hyperlink item in worksheet.hyperlinks.hyperlink)
			{
				PackageRelationship hyperlinkRel = null;
				if (item.id != null)
				{
					hyperlinkRel = relationshipsByType.GetRelationshipByID(item.id);
				}
				hyperlinks.Add(new XSSFHyperlink(item, hyperlinkRel));
			}
		}
		catch (InvalidFormatException ex)
		{
			throw new POIXMLException(ex);
		}
	}

	private static CT_Worksheet NewSheet()
	{
		CT_Worksheet cT_Worksheet = new CT_Worksheet();
		cT_Worksheet.AddNewSheetFormatPr().defaultRowHeight = DEFAULT_ROW_HEIGHT;
		cT_Worksheet.AddNewSheetViews().AddNewSheetView().workbookViewId = 0u;
		cT_Worksheet.AddNewDimension().@ref = "A1";
		cT_Worksheet.AddNewSheetData();
		CT_PageMargins cT_PageMargins = cT_Worksheet.AddNewPageMargins();
		cT_PageMargins.bottom = DEFAULT_MARGIN_BOTTOM;
		cT_PageMargins.footer = DEFAULT_MARGIN_FOOTER;
		cT_PageMargins.header = DEFAULT_MARGIN_HEADER;
		cT_PageMargins.left = DEFAULT_MARGIN_LEFT;
		cT_PageMargins.right = DEFAULT_MARGIN_RIGHT;
		cT_PageMargins.top = DEFAULT_MARGIN_TOP;
		return cT_Worksheet;
	}

	public CT_Worksheet GetCTWorksheet()
	{
		return worksheet;
	}

	public ColumnHelper GetColumnHelper()
	{
		return columnHelper;
	}

	public int AddMergedRegion(CellRangeAddress region)
	{
		return AddMergedRegion(region, validate: true);
	}

	public int AddMergedRegionUnsafe(CellRangeAddress region)
	{
		return AddMergedRegion(region, validate: false);
	}

	private int AddMergedRegion(CellRangeAddress region, bool validate)
	{
		if (region.NumberOfCells < 2)
		{
			throw new ArgumentException("Merged region " + region.FormatAsString() + " must contain 2 or more cells");
		}
		region.Validate(SpreadsheetVersion.EXCEL2007);
		if (validate)
		{
			ValidateArrayFormulas(region);
			ValidateMergedRegions(region);
		}
		CT_MergeCells obj = (worksheet.IsSetMergeCells() ? worksheet.mergeCells : worksheet.AddNewMergeCells());
		obj.AddNewMergeCell().@ref = region.FormatAsString();
		return obj.sizeOfMergeCellArray();
	}

	private void ValidateArrayFormulas(CellRangeAddress region)
	{
		int firstRow = region.FirstRow;
		int firstColumn = region.FirstColumn;
		int lastRow = region.LastRow;
		int lastColumn = region.LastColumn;
		for (int i = firstRow; i <= lastRow; i++)
		{
			IRow row = GetRow(i);
			if (row == null)
			{
				continue;
			}
			for (int j = firstColumn; j <= lastColumn; j++)
			{
				ICell cell = row.GetCell(j);
				if (cell != null && cell.IsPartOfArrayFormulaGroup)
				{
					CellRangeAddress arrayFormulaRange = cell.ArrayFormulaRange;
					if (arrayFormulaRange.NumberOfCells > 1 && region.Intersects(arrayFormulaRange))
					{
						throw new InvalidOperationException("The range " + region.FormatAsString() + " intersects with a multi-cell array formula. You cannot merge cells of an array.");
					}
				}
			}
		}
	}

	private void CheckForMergedRegionsIntersectingArrayFormulas()
	{
		foreach (CellRangeAddress mergedRegion in MergedRegions)
		{
			ValidateArrayFormulas(mergedRegion);
		}
	}

	private void ValidateMergedRegions(CellRangeAddress candidateRegion)
	{
		foreach (CellRangeAddress mergedRegion in MergedRegions)
		{
			if (mergedRegion.Intersects(candidateRegion))
			{
				throw new InvalidOperationException("Cannot add merged region " + candidateRegion.FormatAsString() + " to sheet because it overlaps with an existing merged region (" + mergedRegion.FormatAsString() + ").");
			}
		}
	}

	private void CheckForIntersectingMergedRegions()
	{
		List<CellRangeAddress> mergedRegions = MergedRegions;
		int count = mergedRegions.Count;
		for (int i = 0; i < count; i++)
		{
			CellRangeAddress cellRangeAddress = mergedRegions[i];
			foreach (CellRangeAddress item in mergedRegions.Skip(i + 1))
			{
				if (cellRangeAddress.Intersects(item))
				{
					throw new InvalidOperationException("The range " + cellRangeAddress.FormatAsString() + " intersects with another merged region " + item.FormatAsString() + " in this sheet");
				}
			}
		}
	}

	public void ValidateMergedRegions()
	{
		CheckForMergedRegionsIntersectingArrayFormulas();
		CheckForIntersectingMergedRegions();
	}

	public void AutoSizeColumn(int column)
	{
		AutoSizeColumn(column, useMergedCells: false);
	}

	public void AutoSizeColumn(int column, bool useMergedCells)
	{
		double columnWidth = SheetUtil.GetColumnWidth(this, column, useMergedCells);
		if (columnWidth != -1.0)
		{
			columnWidth *= 256.0;
			int num = 65280;
			if (columnWidth > (double)num)
			{
				columnWidth = num;
			}
			SetColumnWidth(column, (int)columnWidth);
			columnHelper.SetColBestFit(column, bestFit: true);
		}
	}

	public XSSFDrawing GetDrawingPatriarch()
	{
		NPOI.OpenXmlFormats.Spreadsheet.CT_Drawing cTDrawing = GetCTDrawing();
		if (cTDrawing != null)
		{
			foreach (RelationPart relationPart in base.RelationParts)
			{
				POIXMLDocumentPart documentPart = relationPart.DocumentPart;
				if (documentPart is XSSFDrawing)
				{
					XSSFDrawing result = (XSSFDrawing)documentPart;
					if (relationPart.Relationship.Id.Equals(cTDrawing.id))
					{
						return result;
					}
					break;
				}
			}
			logger.Log(7, "Can't find Drawing with id=" + cTDrawing.id + " in the list of the sheet's relationships");
		}
		return null;
	}

	public IDrawing CreateDrawingPatriarch()
	{
		if (GetCTDrawing() != null)
		{
			return GetDrawingPatriarch();
		}
		int idx = GetPackagePart().Package.GetPartsByContentType(XSSFRelation.DRAWINGS.ContentType).Count + 1;
		RelationPart relationPart = CreateRelationship(XSSFRelation.DRAWINGS, XSSFFactory.GetInstance(), idx, noRelation: false);
		XSSFDrawing result = relationPart.DocumentPart as XSSFDrawing;
		string id = relationPart.Relationship.Id;
		worksheet.AddNewDrawing().id = id;
		return result;
	}

	internal XSSFVMLDrawing GetVMLDrawing(bool autoCreate)
	{
		XSSFVMLDrawing xSSFVMLDrawing = null;
		NPOI.OpenXmlFormats.Spreadsheet.CT_LegacyDrawing cTLegacyDrawing = GetCTLegacyDrawing();
		if (cTLegacyDrawing == null)
		{
			if (autoCreate)
			{
				int idx = GetPackagePart().Package.GetPartsByContentType(XSSFRelation.VML_DRAWINGS.ContentType).Count + 1;
				RelationPart relationPart = CreateRelationship(XSSFRelation.VML_DRAWINGS, XSSFFactory.GetInstance(), idx, noRelation: false);
				xSSFVMLDrawing = relationPart.DocumentPart as XSSFVMLDrawing;
				string id = relationPart.Relationship.Id;
				cTLegacyDrawing = worksheet.AddNewLegacyDrawing();
				cTLegacyDrawing.id = id;
			}
		}
		else
		{
			string id2 = cTLegacyDrawing.id;
			foreach (RelationPart relationPart2 in base.RelationParts)
			{
				POIXMLDocumentPart documentPart = relationPart2.DocumentPart;
				if (documentPart is XSSFVMLDrawing)
				{
					XSSFVMLDrawing xSSFVMLDrawing2 = (XSSFVMLDrawing)documentPart;
					if (relationPart2.Relationship.Id.Equals(id2))
					{
						xSSFVMLDrawing = xSSFVMLDrawing2;
						break;
					}
				}
			}
			if (xSSFVMLDrawing == null)
			{
				logger.Log(7, "Can't find VML drawing with id=" + id2 + " in the list of the sheet's relationships");
			}
		}
		return xSSFVMLDrawing;
	}

	protected virtual NPOI.OpenXmlFormats.Spreadsheet.CT_Drawing GetCTDrawing()
	{
		return worksheet.drawing;
	}

	protected virtual NPOI.OpenXmlFormats.Spreadsheet.CT_LegacyDrawing GetCTLegacyDrawing()
	{
		return worksheet.legacyDrawing;
	}

	public void CreateFreezePane(int colSplit, int rowSplit)
	{
		CreateFreezePane(colSplit, rowSplit, colSplit, rowSplit);
	}

	public void CreateFreezePane(int colSplit, int rowSplit, int leftmostColumn, int topRow)
	{
		CT_SheetView defaultSheetView = GetDefaultSheetView();
		if (colSplit == 0 && rowSplit == 0)
		{
			if (defaultSheetView.IsSetPane())
			{
				defaultSheetView.UnsetPane();
			}
			defaultSheetView.SetSelectionArray(null);
			return;
		}
		if (!defaultSheetView.IsSetPane())
		{
			defaultSheetView.AddNewPane();
		}
		CT_Pane pane = defaultSheetView.pane;
		if (colSplit > 0)
		{
			pane.xSplit = colSplit;
		}
		else if (pane.IsSetXSplit())
		{
			pane.UnsetXSplit();
		}
		if (rowSplit > 0)
		{
			pane.ySplit = rowSplit;
		}
		else if (pane.IsSetYSplit())
		{
			pane.UnsetYSplit();
		}
		pane.state = ST_PaneState.frozen;
		if (rowSplit == 0)
		{
			pane.topLeftCell = new CellReference(0, leftmostColumn).FormatAsString();
			pane.activePane = ST_Pane.topRight;
		}
		else if (colSplit == 0)
		{
			pane.topLeftCell = new CellReference(topRow, 0).FormatAsString();
			pane.activePane = ST_Pane.bottomLeft;
		}
		else
		{
			pane.topLeftCell = new CellReference(topRow, leftmostColumn).FormatAsString();
			pane.activePane = ST_Pane.bottomRight;
		}
		defaultSheetView.selection = null;
		defaultSheetView.AddNewSelection().pane = pane.activePane;
	}

	private int GetLastKey(IList<int> keys)
	{
		_ = keys.Count;
		return keys[keys.Count - 1];
	}

	private int HeadMapCount(SortedList<int, XSSFRow> rows, int rownum)
	{
		int num = 0;
		using (IEnumerator<int> enumerator = rows.Keys.GetEnumerator())
		{
			while (enumerator.MoveNext() && enumerator.Current < rownum)
			{
				num++;
			}
		}
		return num;
	}

	public virtual IRow CreateRow(int rownum)
	{
		XSSFRow xSSFRow = (_rows.ContainsKey(rownum) ? _rows[rownum] : null);
		CT_Row cT_Row;
		if (xSSFRow != null)
		{
			while (xSSFRow.FirstCellNum != -1)
			{
				xSSFRow.RemoveCell(xSSFRow.GetCell(xSSFRow.FirstCellNum));
			}
			cT_Row = xSSFRow.GetCTRow();
			cT_Row.Set(new CT_Row());
		}
		else if (_rows.Count == 0 || rownum > GetLastKey(_rows.Keys))
		{
			cT_Row = worksheet.sheetData.AddNewRow();
		}
		else
		{
			int index = HeadMapCount(_rows, rownum);
			cT_Row = worksheet.sheetData.InsertNewRow(index);
		}
		XSSFRow xSSFRow2 = new XSSFRow(cT_Row, this);
		xSSFRow2.RowNum = rownum;
		_rows[rownum] = xSSFRow2;
		return xSSFRow2;
	}

	public void CreateSplitPane(int xSplitPos, int ySplitPos, int leftmostColumn, int topRow, PanePosition activePane)
	{
		CreateFreezePane(xSplitPos, ySplitPos, leftmostColumn, topRow);
		GetPane().state = ST_PaneState.split;
		GetPane().activePane = (ST_Pane)activePane;
	}

	[Obsolete("deprecated as of 2015-11-23 (circa POI 3.14beta1). Use {@link #getCellComment(CellAddress)} instead.")]
	public IComment GetCellComment(int row, int column)
	{
		return GetCellComment(new CellAddress(row, column));
	}

	public IComment GetCellComment(CellAddress address)
	{
		if (sheetComments == null)
		{
			return null;
		}
		int row = address.Row;
		int column = address.Column;
		CellAddress cellRef = new CellAddress(row, column);
		CT_Comment cTComment = sheetComments.GetCTComment(cellRef);
		if (cTComment == null)
		{
			return null;
		}
		XSSFVMLDrawing vMLDrawing = GetVMLDrawing(autoCreate: false);
		return new XSSFComment(sheetComments, cTComment, vMLDrawing?.FindCommentShape(row, column));
	}

	public Dictionary<CellAddress, IComment> GetCellComments()
	{
		if (sheetComments == null)
		{
			return new Dictionary<CellAddress, IComment>();
		}
		return sheetComments.GetCellComments();
	}

	public IHyperlink GetHyperlink(int row, int column)
	{
		return GetHyperlink(new CellAddress(row, column));
	}

	public IHyperlink GetHyperlink(CellAddress addr)
	{
		string value = addr.FormatAsString();
		foreach (XSSFHyperlink hyperlink in hyperlinks)
		{
			if (hyperlink.CellRef.Equals(value))
			{
				return hyperlink;
			}
		}
		return null;
	}

	public List<IHyperlink> GetHyperlinkList()
	{
		return ((IEnumerable<IHyperlink>)hyperlinks).ToList();
	}

	public int GetColumnWidth(int columnIndex)
	{
		CT_Col column = columnHelper.GetColumn(columnIndex, splitColumns: false);
		return (int)(((column == null || !column.IsSetWidth()) ? ((double)DefaultColumnWidth) : column.width) * 256.0);
	}

	public float GetColumnWidthInPixels(int columnIndex)
	{
		return (float)((double)(float)GetColumnWidth(columnIndex) / 256.0 * (double)XSSFWorkbook.DEFAULT_CHARACTER_WIDTH);
	}

	private CT_SheetFormatPr GetSheetTypeSheetFormatPr()
	{
		if (!worksheet.IsSetSheetFormatPr())
		{
			return worksheet.AddNewSheetFormatPr();
		}
		return worksheet.sheetFormatPr;
	}

	public ICellStyle GetColumnStyle(int column)
	{
		int colDefaultStyle = columnHelper.GetColDefaultStyle(column);
		return Workbook.GetCellStyleAt((colDefaultStyle != -1) ? colDefaultStyle : 0);
	}

	private CT_SheetPr GetSheetTypeSheetPr()
	{
		if (worksheet.sheetPr == null)
		{
			worksheet.sheetPr = new CT_SheetPr();
		}
		return worksheet.sheetPr;
	}

	private CT_HeaderFooter GetSheetTypeHeaderFooter()
	{
		if (worksheet.headerFooter == null)
		{
			worksheet.headerFooter = new CT_HeaderFooter();
		}
		return worksheet.headerFooter;
	}

	public double GetMargin(MarginType margin)
	{
		if (!worksheet.IsSetPageMargins())
		{
			return 0.0;
		}
		CT_PageMargins pageMargins = worksheet.pageMargins;
		return margin switch
		{
			MarginType.LeftMargin => pageMargins.left, 
			MarginType.RightMargin => pageMargins.right, 
			MarginType.TopMargin => pageMargins.top, 
			MarginType.BottomMargin => pageMargins.bottom, 
			MarginType.HeaderMargin => pageMargins.header, 
			MarginType.FooterMargin => pageMargins.footer, 
			_ => throw new ArgumentException("Unknown margin constant:  " + margin), 
		};
	}

	public void SetMargin(MarginType margin, double size)
	{
		CT_PageMargins cT_PageMargins = (worksheet.IsSetPageMargins() ? worksheet.pageMargins : worksheet.AddNewPageMargins());
		switch (margin)
		{
		case MarginType.LeftMargin:
			cT_PageMargins.left = size;
			break;
		case MarginType.RightMargin:
			cT_PageMargins.right = size;
			break;
		case MarginType.TopMargin:
			cT_PageMargins.top = size;
			break;
		case MarginType.BottomMargin:
			cT_PageMargins.bottom = size;
			break;
		case MarginType.HeaderMargin:
			cT_PageMargins.header = size;
			break;
		case MarginType.FooterMargin:
			cT_PageMargins.footer = size;
			break;
		default:
			throw new InvalidOperationException("Unknown margin constant:  " + margin);
		}
	}

	public CellRangeAddress GetMergedRegion(int index)
	{
		CT_MergeCell mergeCellArray = (worksheet.mergeCells ?? throw new InvalidOperationException("This worksheet does not contain merged regions")).GetMergeCellArray(index);
		if (mergeCellArray == null)
		{
			return null;
		}
		return CellRangeAddress.ValueOf(mergeCellArray.@ref);
	}

	public CellRangeAddress GetMergedRegion(CellRangeAddress mergedRegion)
	{
		if (worksheet.mergeCells == null || worksheet.mergeCells.mergeCell == null)
		{
			return null;
		}
		foreach (CT_MergeCell item in worksheet.mergeCells.mergeCell)
		{
			if (item != null && !string.IsNullOrEmpty(item.@ref))
			{
				CellRangeAddress cellRangeAddress = CellRangeAddress.ValueOf(item.@ref);
				if (cellRangeAddress.FirstColumn <= mergedRegion.FirstColumn && cellRangeAddress.LastColumn >= mergedRegion.LastColumn && cellRangeAddress.FirstRow <= mergedRegion.FirstRow && cellRangeAddress.LastRow >= mergedRegion.LastRow)
				{
					return cellRangeAddress;
				}
			}
		}
		return null;
	}

	public void ProtectSheet(string password)
	{
		if (password != null)
		{
			CT_SheetProtection cT_SheetProtection = worksheet.AddNewSheetProtection();
			SetSheetPassword(password, null);
			cT_SheetProtection.sheet = true;
			cT_SheetProtection.scenarios = true;
			cT_SheetProtection.objects = true;
		}
		else
		{
			worksheet.UnsetSheetProtection();
		}
	}

	public void SetSheetPassword(string password, HashAlgorithm hashAlgo)
	{
		if (password != null || IsSheetProtectionEnabled())
		{
			XSSFPasswordHelper.SetPassword(SafeGetProtectionField(), password, hashAlgo, null);
		}
	}

	public bool ValidateSheetPassword(string password)
	{
		if (!IsSheetProtectionEnabled())
		{
			return password == null;
		}
		return XSSFPasswordHelper.ValidatePassword(SafeGetProtectionField(), password, null);
	}

	public IRow GetRow(int rownum)
	{
		if (_rows.ContainsKey(rownum))
		{
			return _rows[rownum];
		}
		return null;
	}

	private List<XSSFRow> GetRows(int startRowNum, int endRowNum, bool createRowIfMissing)
	{
		if (startRowNum > endRowNum)
		{
			throw new ArgumentException("getRows: startRowNum must be less than or equal to endRowNum");
		}
		List<XSSFRow> list = new List<XSSFRow>();
		if (createRowIfMissing)
		{
			for (int i = startRowNum; i <= endRowNum; i++)
			{
				XSSFRow xSSFRow = GetRow(i) as XSSFRow;
				if (xSSFRow == null)
				{
					xSSFRow = CreateRow(i) as XSSFRow;
				}
				list.Add(xSSFRow);
			}
		}
		else
		{
			list.AddRange(from x in _rows.SkipWhile((KeyValuePair<int, XSSFRow> x) => x.Key < startRowNum).TakeWhile((KeyValuePair<int, XSSFRow> x) => x.Key < endRowNum + 1)
				select x.Value);
		}
		return list;
	}

	private CT_OutlinePr ensureOutlinePr()
	{
		CT_SheetPr cT_SheetPr = (worksheet.IsSetSheetPr() ? worksheet.sheetPr : worksheet.AddNewSheetPr());
		if (!cT_SheetPr.IsSetOutlinePr())
		{
			return cT_SheetPr.AddNewOutlinePr();
		}
		return cT_SheetPr.outlinePr;
	}

	public void GroupColumn(int fromColumn, int toColumn)
	{
		GroupColumn1Based(fromColumn + 1, toColumn + 1);
	}

	private void GroupColumn1Based(int fromColumn, int toColumn)
	{
		CT_Cols colsArray = worksheet.GetColsArray(0);
		CT_Col cT_Col = new CT_Col();
		CT_Col cT_Col2 = columnHelper.GetColumn1Based(toColumn, splitColumns: false);
		if (cT_Col2 != null)
		{
			cT_Col2 = cT_Col2.Copy();
		}
		cT_Col.min = (uint)fromColumn;
		cT_Col.max = (uint)toColumn;
		columnHelper.AddCleanColIntoCols(colsArray, cT_Col);
		CT_Col column1Based = columnHelper.GetColumn1Based(toColumn, splitColumns: false);
		if (cT_Col2 != null && column1Based != null)
		{
			columnHelper.SetColumnAttributes(cT_Col2, column1Based);
		}
		int num;
		for (num = fromColumn; num <= toColumn; num++)
		{
			CT_Col column1Based2 = columnHelper.GetColumn1Based(num, splitColumns: false);
			column1Based2.outlineLevel++;
			num = (int)column1Based2.max;
		}
		worksheet.SetColsArray(0, colsArray);
		SetSheetFormatPrOutlineLevelCol();
	}

	private void SetColWidthAttribute(CT_Cols ctCols)
	{
		foreach (CT_Col col in ctCols.GetColList())
		{
			if (!col.IsSetWidth())
			{
				col.width = DefaultColumnWidth;
				col.customWidth = false;
			}
		}
	}

	public void GroupRow(int fromRow, int toRow)
	{
		for (int i = fromRow; i <= toRow; i++)
		{
			XSSFRow xSSFRow = (XSSFRow)GetRow(i);
			if (xSSFRow == null)
			{
				xSSFRow = (XSSFRow)CreateRow(i);
			}
			xSSFRow.GetCTRow().outlineLevel++;
		}
		SetSheetFormatPrOutlineLevelRow();
	}

	private short GetMaxOutlineLevelRows()
	{
		short num = 0;
		foreach (XSSFRow value in _rows.Values)
		{
			num = ((value.GetCTRow().outlineLevel > num) ? value.GetCTRow().outlineLevel : num);
		}
		return num;
	}

	[Obsolete]
	private short GetMaxOutlineLevelCols()
	{
		CT_Cols colsArray = worksheet.GetColsArray(0);
		short num = 0;
		foreach (CT_Col col in colsArray.GetColList())
		{
			num = ((col.outlineLevel > num) ? col.outlineLevel : num);
		}
		return num;
	}

	public bool IsColumnBroken(int column)
	{
		int[] columnBreaks = ColumnBreaks;
		for (int i = 0; i < columnBreaks.Length; i++)
		{
			if (columnBreaks[i] == column)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsColumnHidden(int columnIndex)
	{
		return columnHelper.GetColumn(columnIndex, splitColumns: false)?.hidden ?? false;
	}

	public bool IsRowBroken(int row)
	{
		int[] rowBreaks = RowBreaks;
		for (int i = 0; i < rowBreaks.Length; i++)
		{
			if (rowBreaks[i] == row)
			{
				return true;
			}
		}
		return false;
	}

	public void SetRowBreak(int row)
	{
		CT_PageBreak cT_PageBreak = (worksheet.IsSetRowBreaks() ? worksheet.rowBreaks : worksheet.AddNewRowBreaks());
		if (!IsRowBroken(row))
		{
			CT_Break cT_Break = cT_PageBreak.AddNewBrk();
			cT_Break.id = (uint)(row + 1);
			cT_Break.man = true;
			cT_Break.max = (uint)SpreadsheetVersion.EXCEL2007.LastColumnIndex;
			cT_PageBreak.count = (uint)cT_PageBreak.sizeOfBrkArray();
			cT_PageBreak.manualBreakCount = (uint)cT_PageBreak.sizeOfBrkArray();
		}
	}

	public void RemoveColumnBreak(int column)
	{
		if (!worksheet.IsSetColBreaks())
		{
			return;
		}
		CT_PageBreak colBreaks = worksheet.colBreaks;
		List<CT_Break> brk = colBreaks.brk;
		for (int i = 0; i < brk.Count; i++)
		{
			if (brk[i].id == column + 1)
			{
				colBreaks.RemoveBrk(i);
			}
		}
	}

	public void RemoveMergedRegion(int index)
	{
		CT_MergeCells mergeCells = worksheet.mergeCells;
		int num = mergeCells.sizeOfMergeCellArray();
		CT_MergeCell[] array = new CT_MergeCell[num - 1];
		for (int i = 0; i < num; i++)
		{
			if (i < index)
			{
				array[i] = mergeCells.GetMergeCellArray(i);
			}
			else if (i > index)
			{
				array[i - 1] = mergeCells.GetMergeCellArray(i);
			}
		}
		if (array.Length != 0)
		{
			mergeCells.SetMergeCellArray(array);
		}
		else
		{
			worksheet.UnsetMergeCells();
		}
	}

	public void RemoveMergedRegions(IList<int> indices)
	{
		if (!worksheet.IsSetMergeCells())
		{
			return;
		}
		CT_MergeCells mergeCells = worksheet.mergeCells;
		int num = mergeCells.sizeOfMergeCellArray();
		List<CT_MergeCell> list = new List<CT_MergeCell>(mergeCells.sizeOfMergeCellArray());
		int i = 0;
		int num2 = 0;
		for (; i < num; i++)
		{
			if (!indices.Contains(i))
			{
				list.Add(mergeCells.GetMergeCellArray(i));
				num2++;
			}
		}
		if (ListIsEmpty(list))
		{
			worksheet.UnsetMergeCells();
		}
		else
		{
			mergeCells.SetMergeCellArray(list.ToArray());
		}
	}

	private bool ListIsEmpty(List<CT_MergeCell> list)
	{
		foreach (CT_MergeCell item in list)
		{
			if (item != null)
			{
				return false;
			}
		}
		return true;
	}

	public void RemoveRow(IRow row)
	{
		if (row.Sheet != this)
		{
			throw new ArgumentException("Specified row does not belong to this sheet");
		}
		List<XSSFCell> list = new List<XSSFCell>();
		foreach (ICell item in row)
		{
			list.Add((XSSFCell)item);
		}
		foreach (XSSFCell item2 in list)
		{
			row.RemoveCell(item2);
		}
		int num = _rows.Count((KeyValuePair<int, XSSFRow> p) => p.Key < row.RowNum);
		_rows.Remove(row.RowNum);
		worksheet.sheetData.RemoveRow(row.RowNum + 1);
		if (sheetComments == null)
		{
			return;
		}
		foreach (CellAddress key in GetCellComments().Keys)
		{
			if (key.Row == num)
			{
				sheetComments.RemoveComment(key);
			}
		}
	}

	public void RemoveRowBreak(int row)
	{
		if (!worksheet.IsSetRowBreaks())
		{
			return;
		}
		CT_PageBreak rowBreaks = worksheet.rowBreaks;
		List<CT_Break> brk = rowBreaks.brk;
		for (int i = 0; i < brk.Count; i++)
		{
			if (brk[i].id == row + 1)
			{
				rowBreaks.RemoveBrk(i);
			}
		}
	}

	public void SetColumnBreak(int column)
	{
		if (!IsColumnBroken(column))
		{
			CT_PageBreak obj = (worksheet.IsSetColBreaks() ? worksheet.colBreaks : worksheet.AddNewColBreaks());
			CT_Break cT_Break = obj.AddNewBrk();
			cT_Break.id = (uint)(column + 1);
			cT_Break.man = true;
			cT_Break.max = (uint)SpreadsheetVersion.EXCEL2007.LastRowIndex;
			obj.count = (uint)obj.sizeOfBrkArray();
			obj.manualBreakCount = (uint)obj.sizeOfBrkArray();
		}
	}

	public void SetColumnGroupCollapsed(int columnNumber, bool collapsed)
	{
		if (collapsed)
		{
			CollapseColumn(columnNumber);
		}
		else
		{
			ExpandColumn(columnNumber);
		}
	}

	private void CollapseColumn(int columnNumber)
	{
		CT_Cols colsArray = worksheet.GetColsArray(0);
		CT_Col column = columnHelper.GetColumn(columnNumber, splitColumns: false);
		int indexOfColumn = columnHelper.GetIndexOfColumn(colsArray, column);
		if (indexOfColumn != -1)
		{
			int num = FindStartOfColumnOutlineGroup(indexOfColumn);
			CT_Col colArray = colsArray.GetColArray(num);
			int num2 = SetGroupHidden(num, colArray.outlineLevel, hidden: true);
			SetColumn(num2 + 1, null, 0, null, null, true);
		}
	}

	private void SetColumn(int targetColumnIx, short? xfIndex, int? style, int? level, bool? hidden, bool? collapsed)
	{
		CT_Cols colsArray = worksheet.GetColsArray(0);
		CT_Col cT_Col = null;
		int num = 0;
		for (num = 0; num < colsArray.sizeOfColArray(); num++)
		{
			CT_Col colArray = colsArray.GetColArray(num);
			if (colArray.min >= targetColumnIx && colArray.max <= targetColumnIx)
			{
				cT_Col = colArray;
				break;
			}
			if (colArray.min > targetColumnIx)
			{
				break;
			}
		}
		if (cT_Col == null)
		{
			CT_Col cT_Col2 = new CT_Col();
			cT_Col2.min = (uint)targetColumnIx;
			cT_Col2.max = (uint)targetColumnIx;
			UnsetCollapsed(collapsed.Value, cT_Col2);
			columnHelper.AddCleanColIntoCols(colsArray, cT_Col2);
			return;
		}
		bool flag = style.HasValue && cT_Col.style != style;
		bool num2 = level.HasValue && cT_Col.outlineLevel != level;
		bool flag2 = hidden.HasValue && cT_Col.hidden != hidden;
		bool flag3 = collapsed.HasValue && cT_Col.collapsed != collapsed;
		if (!(num2 | flag2 | flag3 | flag))
		{
			return;
		}
		if (cT_Col.min == targetColumnIx && cT_Col.max == targetColumnIx)
		{
			UnsetCollapsed(collapsed.Value, cT_Col);
		}
		else if (cT_Col.min == targetColumnIx || cT_Col.max == targetColumnIx)
		{
			if (cT_Col.min == targetColumnIx)
			{
				cT_Col.min = (uint)(targetColumnIx + 1);
			}
			else
			{
				cT_Col.max = (uint)(targetColumnIx - 1);
				num++;
			}
			CT_Col cT_Col3 = columnHelper.CloneCol(colsArray, cT_Col);
			cT_Col3.min = (uint)targetColumnIx;
			UnsetCollapsed(collapsed.Value, cT_Col3);
			columnHelper.AddCleanColIntoCols(colsArray, cT_Col3);
		}
		else
		{
			CT_Col cT_Col4 = cT_Col;
			CT_Col cT_Col5 = columnHelper.CloneCol(colsArray, cT_Col);
			CT_Col cT_Col6 = columnHelper.CloneCol(colsArray, cT_Col);
			int max = (int)cT_Col.max;
			cT_Col4.max = (uint)(targetColumnIx - 1);
			cT_Col5.min = (uint)targetColumnIx;
			cT_Col5.max = (uint)targetColumnIx;
			UnsetCollapsed(collapsed.Value, cT_Col5);
			columnHelper.AddCleanColIntoCols(colsArray, cT_Col5);
			cT_Col6.min = (uint)(targetColumnIx + 1);
			cT_Col6.max = (uint)max;
			columnHelper.AddCleanColIntoCols(colsArray, cT_Col6);
		}
	}

	private void UnsetCollapsed(bool collapsed, CT_Col ci)
	{
		if (collapsed)
		{
			ci.collapsed = collapsed;
		}
		else
		{
			ci.UnsetCollapsed();
		}
	}

	private int SetGroupHidden(int pIdx, int level, bool hidden)
	{
		CT_Cols colsArray = worksheet.GetColsArray(0);
		int i = pIdx;
		CT_Col cT_Col = colsArray.GetColArray(i);
		for (; i < colsArray.sizeOfColArray(); i++)
		{
			cT_Col.hidden = hidden;
			if (i + 1 < colsArray.sizeOfColArray())
			{
				CT_Col colArray = colsArray.GetColArray(i + 1);
				if (!IsAdjacentBefore(cT_Col, colArray) || colArray.outlineLevel < level)
				{
					break;
				}
				cT_Col = colArray;
			}
		}
		return (int)cT_Col.max;
	}

	private bool IsAdjacentBefore(CT_Col col, CT_Col other_col)
	{
		return col.max == other_col.min - 1;
	}

	private int FindStartOfColumnOutlineGroup(int pIdx)
	{
		CT_Cols colsArray = worksheet.GetColsArray(0);
		CT_Col cT_Col = colsArray.GetColArray(pIdx);
		int outlineLevel = cT_Col.outlineLevel;
		int num = pIdx;
		while (num != 0)
		{
			CT_Col colArray = colsArray.GetColArray(num - 1);
			if (!IsAdjacentBefore(colArray, cT_Col) || colArray.outlineLevel < outlineLevel)
			{
				break;
			}
			num--;
			cT_Col = colArray;
		}
		return num;
	}

	private int FindEndOfColumnOutlineGroup(int colInfoIndex)
	{
		CT_Cols colsArray = worksheet.GetColsArray(0);
		CT_Col cT_Col = colsArray.GetColArray(colInfoIndex);
		int outlineLevel = cT_Col.outlineLevel;
		int num = colInfoIndex;
		while (num < colsArray.sizeOfColArray() - 1)
		{
			CT_Col colArray = colsArray.GetColArray(num + 1);
			if (!IsAdjacentBefore(cT_Col, colArray) || colArray.outlineLevel < outlineLevel)
			{
				break;
			}
			num++;
			cT_Col = colArray;
		}
		return num;
	}

	private void ExpandColumn(int columnIndex)
	{
		CT_Cols colsArray = worksheet.GetColsArray(0);
		CT_Col column = columnHelper.GetColumn(columnIndex, splitColumns: false);
		int indexOfColumn = columnHelper.GetIndexOfColumn(colsArray, column);
		int num = FindColInfoIdx((int)column.max, indexOfColumn);
		if (num == -1 || !IsColumnGroupCollapsed(num))
		{
			return;
		}
		int num2 = FindStartOfColumnOutlineGroup(num);
		int num3 = FindEndOfColumnOutlineGroup(num);
		CT_Col colArray = colsArray.GetColArray(num3);
		if (!IsColumnGroupHiddenByParent(num))
		{
			int outlineLevel = colArray.outlineLevel;
			bool flag = false;
			for (int i = num2; i <= num3; i++)
			{
				CT_Col colArray2 = colsArray.GetColArray(i);
				if (outlineLevel == colArray2.outlineLevel)
				{
					colArray2.UnsetHidden();
					if (flag)
					{
						flag = false;
						colArray2.collapsed = true;
					}
				}
				else
				{
					flag = true;
				}
			}
		}
		SetColumn((int)(colArray.max + 1), null, null, null, false, false);
	}

	private bool IsColumnGroupHiddenByParent(int idx)
	{
		CT_Cols colsArray = worksheet.GetColsArray(0);
		int num = 0;
		bool result = false;
		int num2 = FindEndOfColumnOutlineGroup(idx);
		if (num2 < colsArray.sizeOfColArray())
		{
			CT_Col colArray = colsArray.GetColArray(num2 + 1);
			if (IsAdjacentBefore(colsArray.GetColArray(num2), colArray))
			{
				num = colArray.outlineLevel;
				result = colArray.hidden;
			}
		}
		int num3 = 0;
		bool result2 = false;
		int num4 = FindStartOfColumnOutlineGroup(idx);
		if (num4 > 0)
		{
			CT_Col colArray2 = colsArray.GetColArray(num4 - 1);
			if (IsAdjacentBefore(colArray2, colsArray.GetColArray(num4)))
			{
				num3 = colArray2.outlineLevel;
				result2 = colArray2.hidden;
			}
		}
		if (num > num3)
		{
			return result;
		}
		return result2;
	}

	private int FindColInfoIdx(int columnValue, int fromColInfoIdx)
	{
		CT_Cols colsArray = worksheet.GetColsArray(0);
		if (columnValue < 0)
		{
			throw new ArgumentException("column parameter out of range: " + columnValue);
		}
		if (fromColInfoIdx < 0)
		{
			throw new ArgumentException("fromIdx parameter out of range: " + fromColInfoIdx);
		}
		for (int i = fromColInfoIdx; i < colsArray.sizeOfColArray(); i++)
		{
			CT_Col colArray = colsArray.GetColArray(i);
			if (ContainsColumn(colArray, columnValue))
			{
				return i;
			}
			if (colArray.min > fromColInfoIdx)
			{
				break;
			}
		}
		return -1;
	}

	private bool ContainsColumn(CT_Col col, int columnIndex)
	{
		if (col.min <= columnIndex)
		{
			return columnIndex <= col.max;
		}
		return false;
	}

	private bool IsColumnGroupCollapsed(int idx)
	{
		CT_Cols colsArray = worksheet.GetColsArray(0);
		int num = FindEndOfColumnOutlineGroup(idx);
		int num2 = num + 1;
		if (num2 >= colsArray.sizeOfColArray())
		{
			return false;
		}
		CT_Col colArray = colsArray.GetColArray(num2);
		CT_Col colArray2 = colsArray.GetColArray(num);
		if (!IsAdjacentBefore(colArray2, colArray))
		{
			return false;
		}
		return colArray.collapsed;
	}

	public void SetColumnHidden(int columnIndex, bool hidden)
	{
		columnHelper.SetColHidden(columnIndex, hidden);
	}

	public void SetColumnWidth(int columnIndex, int width)
	{
		if (width > 65280)
		{
			throw new ArgumentException("The maximum column width for an individual cell is 255 characters.");
		}
		columnHelper.SetColWidth(columnIndex, (double)width / 256.0);
		columnHelper.SetCustomWidth(columnIndex, width: true);
	}

	public void SetDefaultColumnStyle(int column, ICellStyle style)
	{
		columnHelper.SetColDefaultStyle(column, style);
	}

	private CT_SheetView GetSheetTypeSheetView()
	{
		if (GetDefaultSheetView() == null)
		{
			GetSheetTypeSheetViews().SetSheetViewArray(0, new CT_SheetView());
		}
		return GetDefaultSheetView();
	}

	public void SetRowGroupCollapsed(int rowIndex, bool collapse)
	{
		if (collapse)
		{
			CollapseRow(rowIndex);
		}
		else
		{
			ExpandRow(rowIndex);
		}
	}

	private void CollapseRow(int rowIndex)
	{
		XSSFRow xSSFRow = (XSSFRow)GetRow(rowIndex);
		if (xSSFRow != null)
		{
			int rowIndex2 = FindStartOfRowOutlineGroup(rowIndex);
			int rownum = WriteHidden(xSSFRow, rowIndex2, hidden: true);
			if (GetRow(rownum) != null)
			{
				((XSSFRow)GetRow(rownum)).GetCTRow().collapsed = true;
			}
			else
			{
				((XSSFRow)CreateRow(rownum)).GetCTRow().collapsed = true;
			}
		}
	}

	private int FindStartOfRowOutlineGroup(int rowIndex)
	{
		int outlineLevel = ((XSSFRow)GetRow(rowIndex)).GetCTRow().outlineLevel;
		int num = rowIndex;
		while (GetRow(num) != null)
		{
			if (((XSSFRow)GetRow(num)).GetCTRow().outlineLevel < outlineLevel)
			{
				return num + 1;
			}
			num--;
		}
		return num;
	}

	private int WriteHidden(XSSFRow xRow, int rowIndex, bool hidden)
	{
		int outlineLevel = xRow.GetCTRow().outlineLevel;
		IEnumerator rowEnumerator = GetRowEnumerator();
		while (rowEnumerator.MoveNext())
		{
			xRow = (XSSFRow)rowEnumerator.Current;
			if (xRow.GetCTRow().outlineLevel >= outlineLevel)
			{
				xRow.GetCTRow().hidden = hidden;
				rowIndex++;
			}
		}
		return rowIndex;
	}

	private void ExpandRow(int rowNumber)
	{
		if (rowNumber == -1)
		{
			return;
		}
		XSSFRow xSSFRow = (XSSFRow)GetRow(rowNumber);
		if (!xSSFRow.GetCTRow().IsSetHidden())
		{
			return;
		}
		int num = FindStartOfRowOutlineGroup(rowNumber);
		int num2 = FindEndOfRowOutlineGroup(rowNumber);
		if (!IsRowGroupHiddenByParent(rowNumber))
		{
			for (int i = num; i < num2; i++)
			{
				if (xSSFRow.GetCTRow().outlineLevel == ((XSSFRow)GetRow(i)).GetCTRow().outlineLevel)
				{
					((XSSFRow)GetRow(i)).GetCTRow().UnsetHidden();
				}
				else if (!IsRowGroupCollapsed(i))
				{
					((XSSFRow)GetRow(i)).GetCTRow().UnsetHidden();
				}
			}
		}
		if (GetRow(num2) is XSSFRow xSSFRow2)
		{
			CT_Row cTRow = xSSFRow2.GetCTRow();
			if (cTRow.collapsed)
			{
				cTRow.UnsetCollapsed();
			}
		}
	}

	public int FindEndOfRowOutlineGroup(int row)
	{
		int outlineLevel = ((XSSFRow)GetRow(row)).GetCTRow().outlineLevel;
		int lastRowNum = LastRowNum;
		int i;
		for (i = row; i < lastRowNum && GetRow(i) != null && ((XSSFRow)GetRow(i)).GetCTRow().outlineLevel >= outlineLevel; i++)
		{
		}
		return i;
	}

	private bool IsRowGroupHiddenByParent(int row)
	{
		int rownum = FindEndOfRowOutlineGroup(row);
		int num;
		bool result;
		if (GetRow(rownum) == null)
		{
			num = 0;
			result = false;
		}
		else
		{
			num = ((XSSFRow)GetRow(rownum)).GetCTRow().outlineLevel;
			result = ((XSSFRow)GetRow(rownum)).GetCTRow().hidden;
		}
		int num2 = FindStartOfRowOutlineGroup(row);
		int num3;
		bool result2;
		if (num2 < 0 || GetRow(num2) == null)
		{
			num3 = 0;
			result2 = false;
		}
		else
		{
			num3 = ((XSSFRow)GetRow(num2)).GetCTRow().outlineLevel;
			result2 = ((XSSFRow)GetRow(num2)).GetCTRow().hidden;
		}
		if (num > num3)
		{
			return result;
		}
		return result2;
	}

	private bool IsRowGroupCollapsed(int row)
	{
		int rownum = FindEndOfRowOutlineGroup(row) + 1;
		if (GetRow(rownum) == null)
		{
			return false;
		}
		return ((XSSFRow)GetRow(rownum)).GetCTRow().collapsed;
	}

	[Obsolete("deprecated 2015-11-23 (circa POI 3.14beta1). Use {@link #setZoom(int)} instead.")]
	public void SetZoom(int numerator, int denominator)
	{
		int zoom = 100 * numerator / denominator;
		SetZoom(zoom);
	}

	public void SetZoom(int scale)
	{
		if (scale < 10 || scale > 400)
		{
			throw new ArgumentException("Valid scale values range from 10 to 400");
		}
		GetSheetTypeSheetView().zoomScale = (uint)scale;
	}

	public void CopyRows(List<XSSFRow> srcRows, int destStartRow, CellCopyPolicy policy)
	{
		if (srcRows == null || srcRows.Count == 0)
		{
			throw new ArgumentException("No rows to copy");
		}
		IRow row = srcRows[0];
		XSSFRow xSSFRow = srcRows[srcRows.Count - 1];
		if (row == null)
		{
			throw new ArgumentException("copyRows: First row cannot be null");
		}
		int rowNum = row.RowNum;
		int rowNum2 = ((IRow)xSSFRow).RowNum;
		int count = srcRows.Count;
		for (int i = 1; i < count; i++)
		{
			IRow row2 = srcRows[i];
			if (row2 == null)
			{
				throw new ArgumentException("srcRows may not contain null rows. Found null row at index " + i + ".");
			}
			if (row.Sheet.Workbook != row2.Sheet.Workbook)
			{
				throw new ArgumentException("All rows in srcRows must belong to the same sheet in the same workbook.Expected all rows from same workbook (" + row.Sheet.Workbook?.ToString() + "). Got srcRows[" + i + "] from different workbook (" + row2.Sheet.Workbook?.ToString() + ").");
			}
			if (row.Sheet != row2.Sheet)
			{
				throw new ArgumentException("All rows in srcRows must belong to the same sheet. Expected all rows from " + row.Sheet.SheetName + ". Got srcRows[" + i + "] from " + row2.Sheet.SheetName);
			}
		}
		CellCopyPolicy cellCopyPolicy = new CellCopyPolicy(policy);
		cellCopyPolicy.IsCopyMergedRegions = false;
		int num = destStartRow;
		foreach (XSSFRow srcRow in srcRows)
		{
			int rownum;
			if (policy.IsCondenseRows)
			{
				rownum = num++;
			}
			else
			{
				int num2 = ((IRow)srcRow).RowNum - rowNum;
				rownum = destStartRow + num2;
			}
			(CreateRow(rownum) as XSSFRow).CopyRowFrom(srcRow, cellCopyPolicy);
		}
		if (!policy.IsCopyMergedRegions)
		{
			return;
		}
		int num3 = destStartRow - rowNum;
		foreach (CellRangeAddress mergedRegion in row.Sheet.MergedRegions)
		{
			if (rowNum <= mergedRegion.FirstRow && mergedRegion.LastRow <= rowNum2)
			{
				CellRangeAddress cellRangeAddress = mergedRegion.Copy();
				cellRangeAddress.FirstRow += num3;
				cellRangeAddress.LastRow += num3;
				AddMergedRegion(cellRangeAddress);
			}
		}
	}

	public void CopyRows(int srcStartRow, int srcEndRow, int destStartRow, CellCopyPolicy cellCopyPolicy)
	{
		List<XSSFRow> rows = GetRows(srcStartRow, srcEndRow, createRowIfMissing: false);
		CopyRows(rows, destStartRow, cellCopyPolicy);
	}

	public void ShiftRows(int startRow, int endRow, int n)
	{
		ShiftRows(startRow, endRow, n, copyRowHeight: false, resetOriginalRowHeight: false);
	}

	public void ShiftRows(int startRow, int endRow, int n, bool copyRowHeight, bool resetOriginalRowHeight)
	{
		XSSFVMLDrawing vMLDrawing = GetVMLDrawing(autoCreate: false);
		List<int> list = new List<int>();
		List<CellAddress> list2 = new List<CellAddress>();
		List<CT_Row> list3 = new List<CT_Row>();
		foreach (KeyValuePair<int, XSSFRow> row in _rows)
		{
			XSSFRow value = row.Value;
			int rowNum = value.RowNum;
			if (!ShouldRemoveRow(startRow, endRow, n, rowNum))
			{
				continue;
			}
			int index = _rows.IndexOfValue(value);
			list3.Add(worksheet.sheetData.GetRowArray(index));
			list.Add(row.Key);
			list2.Clear();
			if (sheetComments != null)
			{
				foreach (CT_Comment item in sheetComments.GetCTComments().commentList.comment)
				{
					CellAddress cellAddress = new CellAddress(item.@ref);
					if (cellAddress.Row == rowNum)
					{
						list2.Add(cellAddress);
					}
				}
			}
			foreach (CellAddress item2 in list2)
			{
				sheetComments.RemoveComment(item2);
				vMLDrawing.RemoveCommentShape(item2.Row, item2.Column);
			}
			if (hyperlinks == null)
			{
				continue;
			}
			foreach (XSSFHyperlink item3 in new List<XSSFHyperlink>(hyperlinks))
			{
				if (new CellReference(item3.CellRef).Row == rowNum)
				{
					hyperlinks.Remove(item3);
				}
			}
		}
		foreach (int item4 in list)
		{
			_rows.Remove(item4);
		}
		worksheet.sheetData.RemoveRows(list3);
		SortedDictionary<XSSFComment, int> sortedDictionary = new SortedDictionary<XSSFComment, int>(new ShiftCommentComparator(n));
		foreach (KeyValuePair<int, XSSFRow> row2 in _rows)
		{
			XSSFRow value2 = row2.Value;
			int rowNum2 = value2.RowNum;
			if (sheetComments != null)
			{
				int num = ShiftedRowNum(startRow, endRow, n, rowNum2);
				if (num != rowNum2)
				{
					foreach (CT_Comment item5 in sheetComments.GetCTComments().commentList.comment)
					{
						CellReference cellReference = new CellReference(item5.@ref);
						if (cellReference.Row == rowNum2)
						{
							XSSFComment key = new XSSFComment(sheetComments, item5, vMLDrawing?.FindCommentShape(rowNum2, cellReference.Col));
							if (sortedDictionary.ContainsKey(key))
							{
								sortedDictionary[key] = num;
							}
							else
							{
								sortedDictionary.Add(key, num);
							}
						}
					}
				}
			}
			if (rowNum2 >= startRow && rowNum2 <= endRow)
			{
				if (!copyRowHeight)
				{
					value2.Height = -1;
				}
				value2.Shift(n);
			}
		}
		foreach (KeyValuePair<XSSFComment, int> item6 in sortedDictionary)
		{
			item6.Key.Row = item6.Value;
		}
		XSSFRowShifter xSSFRowShifter = new XSSFRowShifter(this);
		int sheetIndex = Workbook.GetSheetIndex(this);
		string sheetName = Workbook.GetSheetName(sheetIndex);
		FormulaShifter shifter = FormulaShifter.CreateForRowShift(sheetIndex, sheetName, startRow, endRow, n, SpreadsheetVersion.EXCEL2007);
		xSSFRowShifter.UpdateNamedRanges(shifter);
		xSSFRowShifter.UpdateFormulas(shifter);
		xSSFRowShifter.ShiftMergedRegions(startRow, endRow, n);
		xSSFRowShifter.UpdateConditionalFormatting(shifter);
		xSSFRowShifter.UpdateHyperlinks(shifter);
		Dictionary<int, XSSFRow> dictionary = new Dictionary<int, XSSFRow>();
		foreach (XSSFRow value3 in _rows.Values)
		{
			dictionary.Add(value3.RowNum, value3);
		}
		_rows.Clear();
		foreach (KeyValuePair<int, XSSFRow> item7 in dictionary)
		{
			_rows.Add(item7.Key, item7.Value);
		}
		if (worksheet.sheetData.row != null)
		{
			worksheet.sheetData.row.Sort((CT_Row row1, CT_Row row2) => row1.r.CompareTo(row2.r));
		}
	}

	private int ShiftedRowNum(int startRow, int endRow, int n, int rownum)
	{
		if (rownum < startRow && (n > 0 || startRow - rownum > n))
		{
			return rownum;
		}
		if (rownum > endRow && (n < 0 || rownum - endRow > n))
		{
			return rownum;
		}
		if (rownum < startRow)
		{
			return rownum + (endRow - startRow);
		}
		if (rownum > endRow)
		{
			return rownum - (endRow - startRow);
		}
		return rownum + n;
	}

	public void UngroupColumn(int fromColumn, int toColumn)
	{
		CT_Cols colsArray = worksheet.GetColsArray(0);
		for (int i = fromColumn; i <= toColumn; i++)
		{
			CT_Col column = columnHelper.GetColumn(i, splitColumns: false);
			if (column != null)
			{
				column.outlineLevel--;
				i = (int)column.max;
				if (column.outlineLevel <= 0)
				{
					int indexOfColumn = columnHelper.GetIndexOfColumn(colsArray, column);
					worksheet.GetColsArray(0).RemoveCol(indexOfColumn);
				}
			}
		}
		worksheet.SetColsArray(0, colsArray);
		SetSheetFormatPrOutlineLevelCol();
	}

	public void UngroupRow(int fromRow, int toRow)
	{
		for (int i = fromRow; i <= toRow; i++)
		{
			XSSFRow xSSFRow = (XSSFRow)GetRow(i);
			if (xSSFRow != null)
			{
				CT_Row cTRow = xSSFRow.GetCTRow();
				cTRow.outlineLevel--;
				if (cTRow.outlineLevel == 0 && xSSFRow.FirstCellNum == -1)
				{
					RemoveRow(xSSFRow);
				}
			}
		}
		SetSheetFormatPrOutlineLevelRow();
	}

	private void SetSheetFormatPrOutlineLevelRow()
	{
		short maxOutlineLevelRows = GetMaxOutlineLevelRows();
		GetSheetTypeSheetFormatPr().outlineLevelRow = (byte)maxOutlineLevelRows;
	}

	private void SetSheetFormatPrOutlineLevelCol()
	{
		short maxOutlineLevelCols = GetMaxOutlineLevelCols();
		GetSheetTypeSheetFormatPr().outlineLevelCol = (byte)maxOutlineLevelCols;
	}

	private CT_SheetViews GetSheetTypeSheetViews()
	{
		if (worksheet.sheetViews == null)
		{
			worksheet.sheetViews = new CT_SheetViews();
			worksheet.sheetViews.AddNewSheetView();
		}
		return worksheet.sheetViews;
	}

	public void AddHyperlink(XSSFHyperlink hyperlink)
	{
		hyperlinks.Add(hyperlink);
	}

	public void RemoveHyperlink(int row, int column)
	{
		string value = new CellReference(row, column).FormatAsString();
		for (int i = 0; i < hyperlinks.Count; i++)
		{
			if (hyperlinks[i].CellRef.Equals(value))
			{
				hyperlinks.RemoveAt(i);
				break;
			}
		}
	}

	[Obsolete("deprecated 3.14beta2 (circa 2015-12-05). Use {@link #setActiveCell(CellAddress)} instead.")]
	public void SetActiveCell(string cellref)
	{
		CT_Selection sheetTypeSelection = GetSheetTypeSelection();
		sheetTypeSelection.activeCell = cellref;
		sheetTypeSelection.SetSqref(new string[1] { cellref });
	}

	private CT_Selection GetSheetTypeSelection()
	{
		if (GetSheetTypeSheetView().SizeOfSelectionArray() == 0)
		{
			GetSheetTypeSheetView().InsertNewSelection(0);
		}
		return GetSheetTypeSheetView().GetSelectionArray(0);
	}

	private CT_SheetView GetDefaultSheetView()
	{
		CT_SheetViews sheetTypeSheetViews = GetSheetTypeSheetViews();
		int num = sheetTypeSheetViews?.sizeOfSheetViewArray() ?? 0;
		if (num == 0)
		{
			return null;
		}
		return sheetTypeSheetViews.GetSheetViewArray(num - 1);
	}

	protected internal CommentsTable GetCommentsTable(bool create)
	{
		if ((sheetComments == null) & create)
		{
			try
			{
				sheetComments = (CommentsTable)CreateRelationship(XSSFRelation.SHEET_COMMENTS, XSSFFactory.GetInstance(), (int)sheet.sheetId);
			}
			catch (PartAlreadyExistsException)
			{
				sheetComments = (CommentsTable)CreateRelationship(XSSFRelation.SHEET_COMMENTS, XSSFFactory.GetInstance(), -1);
			}
		}
		return sheetComments;
	}

	private CT_PageSetUpPr GetSheetTypePageSetUpPr()
	{
		CT_SheetPr sheetTypeSheetPr = GetSheetTypeSheetPr();
		if (!sheetTypeSheetPr.IsSetPageSetUpPr())
		{
			return sheetTypeSheetPr.AddNewPageSetUpPr();
		}
		return sheetTypeSheetPr.pageSetUpPr;
	}

	private static bool ShouldRemoveRow(int startRow, int endRow, int n, int rownum)
	{
		if (rownum >= startRow + n && rownum <= endRow + n)
		{
			if (n > 0 && rownum > endRow)
			{
				return true;
			}
			if (n < 0 && rownum < startRow)
			{
				return true;
			}
		}
		return false;
	}

	private CT_Pane GetPane()
	{
		if (GetDefaultSheetView().pane == null)
		{
			GetDefaultSheetView().AddNewPane();
		}
		return GetDefaultSheetView().pane;
	}

	internal CT_CellFormula GetSharedFormula(int sid)
	{
		return sharedFormulas[sid];
	}

	internal void OnReadCell(XSSFCell cell)
	{
		CT_CellFormula f = cell.GetCTCell().f;
		if (f != null && f.t == ST_CellFormulaType.shared && f.isSetRef() && f.Value != null)
		{
			CT_CellFormula cT_CellFormula = f.Copy();
			CellRangeAddress cellRangeAddress = CellRangeAddress.ValueOf(cT_CellFormula.@ref);
			CellReference cellReference = new CellReference(cell);
			if (cellReference.Col > cellRangeAddress.FirstColumn || cellReference.Row > cellRangeAddress.FirstRow)
			{
				string text = new CellRangeAddress(Math.Max(cellReference.Row, cellRangeAddress.FirstRow), cellRangeAddress.LastRow, Math.Max(cellReference.Col, cellRangeAddress.FirstColumn), cellRangeAddress.LastColumn).FormatAsString();
				cT_CellFormula.@ref = text;
			}
			sharedFormulas[(int)f.si] = cT_CellFormula;
		}
		if (f != null && f.t == ST_CellFormulaType.array && f.@ref != null)
		{
			arrayFormulas.Add(CellRangeAddress.ValueOf(f.@ref));
		}
	}

	protected internal override void Commit()
	{
		Stream outputStream = GetPackagePart().GetOutputStream();
		Write(outputStream);
		outputStream.Close();
	}

	internal virtual void Write(Stream stream)
	{
		bool flag = false;
		if (worksheet.sizeOfColsArray() == 1)
		{
			CT_Cols colsArray = worksheet.GetColsArray(0);
			if (colsArray.sizeOfColArray() == 0)
			{
				flag = true;
				worksheet.SetColsArray(null);
			}
			else
			{
				SetColWidthAttribute(colsArray);
			}
		}
		if (hyperlinks.Count > 0)
		{
			if (worksheet.hyperlinks == null)
			{
				worksheet.AddNewHyperlinks();
			}
			CT_Hyperlink[] array = new CT_Hyperlink[hyperlinks.Count];
			for (int i = 0; i < array.Length; i++)
			{
				XSSFHyperlink xSSFHyperlink = hyperlinks[i];
				xSSFHyperlink.GenerateRelationIfNeeded(GetPackagePart());
				array[i] = xSSFHyperlink.GetCTHyperlink();
			}
			worksheet.hyperlinks.SetHyperlinkArray(array);
		}
		foreach (XSSFRow value in _rows.Values)
		{
			value.OnDocumentWrite();
		}
		new Dictionary<string, string>()[ST_RelationshipId.NamespaceURI] = "r";
		new WorksheetDocument(worksheet).Save(stream);
		if (flag)
		{
			worksheet.AddNewCols();
		}
	}

	public void EnableLocking()
	{
		SafeGetProtectionField().sheet = true;
	}

	public void DisableLocking()
	{
		SafeGetProtectionField().sheet = false;
	}

	public void LockAutoFilter(bool enabled)
	{
		SafeGetProtectionField().autoFilter = enabled;
	}

	public void LockDeleteColumns(bool enabled)
	{
		SafeGetProtectionField().deleteColumns = enabled;
	}

	public void LockDeleteRows(bool enabled)
	{
		SafeGetProtectionField().deleteRows = enabled;
	}

	public void LockFormatCells(bool enabled)
	{
		SafeGetProtectionField().formatCells = enabled;
	}

	public void LockFormatColumns(bool enabled)
	{
		SafeGetProtectionField().formatColumns = enabled;
	}

	public void LockFormatRows(bool enabled)
	{
		SafeGetProtectionField().formatRows = enabled;
	}

	public void LockInsertColumns(bool enabled)
	{
		SafeGetProtectionField().insertColumns = enabled;
	}

	public void LockInsertHyperlinks(bool enabled)
	{
		SafeGetProtectionField().insertHyperlinks = enabled;
	}

	public void LockInsertRows(bool enabled)
	{
		SafeGetProtectionField().insertRows = enabled;
	}

	public void LockPivotTables(bool enabled)
	{
		SafeGetProtectionField().pivotTables = enabled;
	}

	public void LockSort(bool enabled)
	{
		SafeGetProtectionField().sort = enabled;
	}

	public void LockObjects(bool enabled)
	{
		SafeGetProtectionField().objects = enabled;
	}

	public void LockScenarios(bool enabled)
	{
		SafeGetProtectionField().scenarios = enabled;
	}

	public void LockSelectLockedCells(bool enabled)
	{
		SafeGetProtectionField().selectLockedCells = enabled;
	}

	public void LockSelectUnlockedCells(bool enabled)
	{
		SafeGetProtectionField().selectUnlockedCells = enabled;
	}

	private CT_SheetProtection SafeGetProtectionField()
	{
		if (!IsSheetProtectionEnabled())
		{
			return worksheet.AddNewSheetProtection();
		}
		return worksheet.sheetProtection;
	}

	private bool IsSheetProtectionEnabled()
	{
		return worksheet.IsSetSheetProtection();
	}

	internal bool IsCellInArrayFormulaContext(ICell cell)
	{
		foreach (CellRangeAddress arrayFormula in arrayFormulas)
		{
			if (arrayFormula.IsInRange(cell.RowIndex, cell.ColumnIndex))
			{
				return true;
			}
		}
		return false;
	}

	internal XSSFCell GetFirstCellInArrayFormula(ICell cell)
	{
		foreach (CellRangeAddress arrayFormula in arrayFormulas)
		{
			if (arrayFormula.IsInRange(cell.RowIndex, cell.ColumnIndex))
			{
				return (XSSFCell)GetRow(arrayFormula.FirstRow).GetCell(arrayFormula.FirstColumn);
			}
		}
		return null;
	}

	private ICellRange<ICell> GetCellRange(CellRangeAddress range)
	{
		int firstRow = range.FirstRow;
		int firstColumn = range.FirstColumn;
		int lastRow = range.LastRow;
		int lastColumn = range.LastColumn;
		int num = lastRow - firstRow + 1;
		int num2 = lastColumn - firstColumn + 1;
		List<ICell> list = new List<ICell>(num * num2);
		for (int i = firstRow; i <= lastRow; i++)
		{
			for (int j = firstColumn; j <= lastColumn; j++)
			{
				IRow row = GetRow(i);
				if (row == null)
				{
					row = CreateRow(i);
				}
				ICell cell = row.GetCell(j);
				if (cell == null)
				{
					cell = row.CreateCell(j);
				}
				list.Add(cell);
			}
		}
		return SSCellRange<ICell>.Create(firstRow, firstColumn, num, num2, list, typeof(ICell));
	}

	public ICellRange<ICell> SetArrayFormula(string formula, CellRangeAddress range)
	{
		ICellRange<ICell> cellRange = GetCellRange(range);
		((XSSFCell)cellRange.TopLeftCell).SetCellArrayFormula(formula, range);
		arrayFormulas.Add(range);
		return cellRange;
	}

	public ICellRange<ICell> RemoveArrayFormula(ICell cell)
	{
		if (cell.Sheet != this)
		{
			throw new ArgumentException("Specified cell does not belong to this sheet.");
		}
		foreach (CellRangeAddress arrayFormula in arrayFormulas)
		{
			if (!arrayFormula.IsInRange(cell.RowIndex, cell.ColumnIndex))
			{
				continue;
			}
			arrayFormulas.Remove(arrayFormula);
			ICellRange<ICell> cellRange = GetCellRange(arrayFormula);
			foreach (ICell item in cellRange)
			{
				item.SetCellType(CellType.Blank);
			}
			return cellRange;
		}
		string r = ((XSSFCell)cell).GetCTCell().r;
		throw new ArgumentException("Cell " + r + " is not part of an array formula.");
	}

	public IDataValidationHelper GetDataValidationHelper()
	{
		return dataValidationHelper;
	}

	public List<IDataValidation> GetDataValidations()
	{
		List<IDataValidation> list = new List<IDataValidation>();
		CT_DataValidations dataValidations = worksheet.dataValidations;
		if (dataValidations != null && dataValidations.count != 0)
		{
			foreach (CT_DataValidation item2 in dataValidations.dataValidation)
			{
				CellRangeAddressList cellRangeAddressList = new CellRangeAddressList();
				string[] array = item2.sqref.Split(new char[1] { ' ' });
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Length != 0)
					{
						string[] array2 = array[i].Split(new char[1] { ':' });
						CellReference cellReference = new CellReference(array2[0]);
						CellReference cellReference2 = ((array2.Length > 1) ? new CellReference(array2[1]) : cellReference);
						CellRangeAddress cra = new CellRangeAddress(cellReference.Row, cellReference2.Row, cellReference.Col, cellReference2.Col);
						cellRangeAddressList.AddCellRangeAddress(cra);
					}
				}
				XSSFDataValidation item = new XSSFDataValidation(cellRangeAddressList, item2);
				list.Add(item);
			}
		}
		return list;
	}

	public void AddValidationData(IDataValidation dataValidation)
	{
		XSSFDataValidation xSSFDataValidation = (XSSFDataValidation)dataValidation;
		CT_DataValidations cT_DataValidations = worksheet.dataValidations;
		if (cT_DataValidations == null)
		{
			cT_DataValidations = worksheet.AddNewDataValidations();
		}
		int num = cT_DataValidations.sizeOfDataValidationArray();
		cT_DataValidations.AddNewDataValidation().Set(xSSFDataValidation.GetCTDataValidation());
		cT_DataValidations.count = (uint)(num + 1);
	}

	public IAutoFilter SetAutoFilter(CellRangeAddress range)
	{
		CT_AutoFilter cT_AutoFilter = worksheet.autoFilter;
		if (cT_AutoFilter == null)
		{
			cT_AutoFilter = worksheet.AddNewAutoFilter();
		}
		string text = new CellRangeAddress(range.FirstRow, range.LastRow, range.FirstColumn, range.LastColumn).FormatAsString();
		cT_AutoFilter.@ref = text;
		XSSFWorkbook xSSFWorkbook = (XSSFWorkbook)Workbook;
		int sheetIndex = Workbook.GetSheetIndex(this);
		XSSFName xSSFName = xSSFWorkbook.GetBuiltInName(XSSFName.BUILTIN_FILTER_DB, sheetIndex);
		if (xSSFName == null)
		{
			xSSFName = xSSFWorkbook.CreateBuiltInName(XSSFName.BUILTIN_FILTER_DB, sheetIndex);
		}
		xSSFName.GetCTName().hidden = true;
		CellReference cellReference = new CellReference(SheetName, range.FirstRow, range.FirstColumn, pAbsRow: true, pAbsCol: true);
		string refersToFormula = string.Concat(str2: new CellReference(null, range.LastRow, range.LastColumn, pAbsRow: true, pAbsCol: true).FormatAsString(), str0: cellReference.FormatAsString(), str1: ":");
		xSSFName.RefersToFormula = refersToFormula;
		return new XSSFAutoFilter(this);
	}

	public XSSFTable CreateTable()
	{
		if (!worksheet.IsSetTableParts())
		{
			worksheet.AddNewTableParts();
		}
		CT_TablePart cT_TablePart = worksheet.tableParts.AddNewTablePart();
		int idx = GetPackagePart().Package.GetPartsByContentType(XSSFRelation.TABLE.ContentType).Count + 1;
		RelationPart relationPart = CreateRelationship(XSSFRelation.TABLE, XSSFFactory.GetInstance(), idx, noRelation: false);
		XSSFTable xSSFTable = relationPart.DocumentPart as XSSFTable;
		cT_TablePart.id = relationPart.Relationship.Id;
		tables[cT_TablePart.id] = xSSFTable;
		return xSSFTable;
	}

	public List<XSSFTable> GetTables()
	{
		return new List<XSSFTable>(tables.Values);
	}

	[Obsolete("deprecated 3.15-beta2. Removed in 3.17. Use {@link #setTabColor(XSSFColor)}.")]
	public void SetTabColor(int colorIndex)
	{
		CT_SheetPr cT_SheetPr = worksheet.sheetPr;
		if (cT_SheetPr == null)
		{
			cT_SheetPr = worksheet.AddNewSheetPr();
		}
		CT_Color cT_Color = new CT_Color();
		cT_Color.indexed = (uint)colorIndex;
		cT_SheetPr.tabColor = cT_Color;
	}

	public IEnumerator GetEnumerator()
	{
		return _rows.Values.GetEnumerator();
	}

	public IEnumerator GetRowEnumerator()
	{
		return GetEnumerator();
	}

	public bool IsMergedRegion(CellRangeAddress mergedRegion)
	{
		if (worksheet.mergeCells == null || worksheet.mergeCells.mergeCell == null)
		{
			return false;
		}
		foreach (CT_MergeCell item in worksheet.mergeCells.mergeCell)
		{
			if (!string.IsNullOrEmpty(item.@ref))
			{
				CellRangeAddress cellRangeAddress = CellRangeAddress.ValueOf(item.@ref);
				if (cellRangeAddress.FirstColumn <= mergedRegion.FirstColumn && cellRangeAddress.LastColumn >= mergedRegion.LastColumn && cellRangeAddress.FirstRow <= mergedRegion.FirstRow && cellRangeAddress.LastRow >= mergedRegion.LastRow)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void SetActive(bool value)
	{
		IsSelected = value;
	}

	public void SetActiveCellRange(List<CellRangeAddress8Bit> cellranges, int activeRange, int activeRow, int activeColumn)
	{
		throw new NotImplementedException();
	}

	public void SetActiveCellRange(int firstRow, int lastRow, int firstColumn, int lastColumn)
	{
		throw new NotImplementedException();
	}

	public IRow CopyRow(int sourceIndex, int targetIndex)
	{
		return SheetUtil.CopyRow(this, sourceIndex, targetIndex);
	}

	public void ShowInPane(int toprow, int leftcol)
	{
		string topLeftCell = new CellReference(toprow, leftcol).FormatAsString();
		Pane.topLeftCell = topLeftCell;
	}

	private void SetRepeatingRowsAndColumns(CellRangeAddress rowDef, CellRangeAddress colDef)
	{
		int num = -1;
		int num2 = -1;
		int num3 = -1;
		int num4 = -1;
		if (rowDef != null)
		{
			num3 = rowDef.FirstRow;
			num4 = rowDef.LastRow;
			if ((num3 == -1 && num4 != -1) || num3 < -1 || num4 < -1 || num3 > num4)
			{
				throw new ArgumentException("Invalid row range specification");
			}
		}
		if (colDef != null)
		{
			num = colDef.FirstColumn;
			num2 = colDef.LastColumn;
			if ((num == -1 && num2 != -1) || num < -1 || num2 < -1 || num > num2)
			{
				throw new ArgumentException("Invalid column range specification");
			}
		}
		int sheetIndex = Workbook.GetSheetIndex(this);
		bool num5 = rowDef == null && colDef == null;
		if (!(Workbook is XSSFWorkbook xSSFWorkbook))
		{
			throw new RuntimeException("Workbook should not be null");
		}
		XSSFName xSSFName = xSSFWorkbook.GetBuiltInName(XSSFName.BUILTIN_PRINT_TITLE, sheetIndex);
		if (num5)
		{
			if (xSSFName != null)
			{
				xSSFWorkbook.RemoveName(xSSFName);
			}
			return;
		}
		if (xSSFName == null)
		{
			xSSFName = xSSFWorkbook.CreateBuiltInName(XSSFName.BUILTIN_PRINT_TITLE, sheetIndex);
		}
		string referenceBuiltInRecord = GetReferenceBuiltInRecord(xSSFName.SheetName, num, num2, num3, num4);
		xSSFName.RefersToFormula = referenceBuiltInRecord;
		if (!worksheet.IsSetPageSetup() || !worksheet.IsSetPageMargins())
		{
			PrintSetup.ValidSettings = false;
		}
	}

	private static string GetReferenceBuiltInRecord(string sheetName, int startC, int endC, int startR, int endR)
	{
		CellReference cellReference = new CellReference(sheetName, 0, startC, pAbsRow: true, pAbsCol: true);
		CellReference cellReference2 = new CellReference(sheetName, 0, endC, pAbsRow: true, pAbsCol: true);
		CellReference cellReference3 = new CellReference(sheetName, startR, 0, pAbsRow: true, pAbsCol: true);
		CellReference cellReference4 = new CellReference(sheetName, endR, 0, pAbsRow: true, pAbsCol: true);
		string text = SheetNameFormatter.Format(sheetName);
		string value = "";
		string text2 = "";
		if (startC != -1 || endC != -1)
		{
			string text3 = cellReference.CellRefParts[2];
			string text4 = cellReference2.CellRefParts[2];
			value = text + "!$" + text3 + ":$" + text4;
		}
		if (startR != -1 || endR != -1)
		{
			string text5 = cellReference3.CellRefParts[1];
			string text6 = cellReference4.CellRefParts[1];
			if (!text5.Equals("0") && !text6.Equals("0"))
			{
				text2 = text + "!$" + text5 + ":$" + text6;
			}
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(value);
		if (stringBuilder.Length > 0 && text2.Length > 0)
		{
			stringBuilder.Append(',');
		}
		stringBuilder.Append(text2);
		return stringBuilder.ToString();
	}

	private CellRangeAddress GetRepeatingRowsOrColums(bool rows)
	{
		int sheetIndex = Workbook.GetSheetIndex(this);
		XSSFName builtInName = ((Workbook as XSSFWorkbook) ?? throw new RuntimeException("Workbook should not be null")).GetBuiltInName(XSSFName.BUILTIN_PRINT_TITLE, sheetIndex);
		if (builtInName == null)
		{
			return null;
		}
		string refersToFormula = builtInName.RefersToFormula;
		if (refersToFormula == null)
		{
			return null;
		}
		string[] array = refersToFormula.Split(",".ToCharArray());
		int lastRowIndex = SpreadsheetVersion.EXCEL2007.LastRowIndex;
		int lastColumnIndex = SpreadsheetVersion.EXCEL2007.LastColumnIndex;
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			CellRangeAddress cellRangeAddress = CellRangeAddress.ValueOf(array2[i]);
			if ((cellRangeAddress.FirstColumn == 0 && cellRangeAddress.LastColumn == lastColumnIndex) || (cellRangeAddress.FirstColumn == -1 && cellRangeAddress.LastColumn == -1))
			{
				if (rows)
				{
					return cellRangeAddress;
				}
			}
			else if (((cellRangeAddress.FirstRow == 0 && cellRangeAddress.LastRow == lastRowIndex) || (cellRangeAddress.FirstRow == -1 && cellRangeAddress.LastRow == -1)) && !rows)
			{
				return cellRangeAddress;
			}
		}
		return null;
	}

	public ISheet CopySheet(string Name)
	{
		return CopySheet(Name, copyStyle: true);
	}

	public ISheet CopySheet(string name, bool copyStyle)
	{
		string uniqueSheetName = SheetUtil.GetUniqueSheetName(Workbook, name);
		XSSFSheet xSSFSheet = (XSSFSheet)Workbook.CreateSheet(uniqueSheetName);
		try
		{
			using MemoryStream memoryStream = new MemoryStream();
			Write(memoryStream);
			xSSFSheet.Read(new MemoryStream(memoryStream.ToArray()));
		}
		catch (IOException ex)
		{
			throw new POIXMLException("Failed to clone sheet", ex);
		}
		CT_Worksheet cTWorksheet = xSSFSheet.GetCTWorksheet();
		if (cTWorksheet.IsSetLegacyDrawing())
		{
			logger.Log(5, "Cloning sheets with comments is not yet supported.");
			cTWorksheet.UnsetLegacyDrawing();
		}
		xSSFSheet.IsSelected = false;
		List<POIXMLDocumentPart> list = GetRelations();
		XSSFDrawing xSSFDrawing = null;
		foreach (POIXMLDocumentPart item in list)
		{
			if (item is XSSFDrawing)
			{
				xSSFDrawing = (XSSFDrawing)item;
			}
			else if (!item.GetPackagePart().PartName.Name.StartsWith("/xl/printerSettings/printerSettings"))
			{
				PackageRelationship packageRelationship = item.GetPackageRelationship();
				xSSFSheet.GetPackagePart().AddRelationship(packageRelationship.TargetUri, packageRelationship.TargetMode.Value, packageRelationship.RelationshipType);
				xSSFSheet.AddRelation(packageRelationship.Id, item);
			}
		}
		xSSFSheet.hyperlinks = new List<XSSFHyperlink>(hyperlinks);
		if (xSSFDrawing != null)
		{
			if (cTWorksheet.IsSetDrawing())
			{
				cTWorksheet.UnsetDrawing();
			}
			XSSFDrawing xSSFDrawing2 = xSSFSheet.CreateDrawingPatriarch() as XSSFDrawing;
			xSSFDrawing2.GetCTDrawing().Set(xSSFDrawing.GetCTDrawing());
			xSSFDrawing2 = xSSFSheet.CreateDrawingPatriarch() as XSSFDrawing;
			foreach (POIXMLDocumentPart relation in xSSFDrawing.GetRelations())
			{
				PackageRelationship packageRelationship2 = relation.GetPackageRelationship();
				xSSFDrawing2.AddRelation(packageRelationship2.Id, relation);
				xSSFDrawing2.GetPackagePart().AddRelationship(packageRelationship2.TargetUri, packageRelationship2.TargetMode.Value, packageRelationship2.RelationshipType, packageRelationship2.Id);
			}
		}
		return xSSFSheet;
	}

	private void CopySheetImages(XSSFWorkbook destWorkbook, XSSFSheet destSheet)
	{
		XSSFDrawing drawingPatriarch = GetDrawingPatriarch();
		if (drawingPatriarch == null)
		{
			return;
		}
		IDrawing drawing = destSheet.CreateDrawingPatriarch();
		List<POIXMLDocumentPart> sheetPictures = drawingPatriarch.GetRelations();
		Dictionary<string, uint> dictionary = new Dictionary<string, uint>();
		foreach (IEG_Anchor cellAnchor in drawingPatriarch.GetCTDrawing().CellAnchors)
		{
			if (!(cellAnchor is CT_TwoCellAnchor cT_TwoCellAnchor))
			{
				continue;
			}
			XSSFClientAnchor xSSFClientAnchor = new XSSFClientAnchor((int)cT_TwoCellAnchor.from.colOff, (int)cT_TwoCellAnchor.from.rowOff, (int)cT_TwoCellAnchor.to.colOff, (int)cT_TwoCellAnchor.to.rowOff, cT_TwoCellAnchor.from.col, cT_TwoCellAnchor.from.row, cT_TwoCellAnchor.to.col, cT_TwoCellAnchor.to.row);
			if (cT_TwoCellAnchor.editAsSpecified)
			{
				switch (cT_TwoCellAnchor.editAs)
				{
				case ST_EditAs.twoCell:
					xSSFClientAnchor.AnchorType = AnchorType.MoveAndResize;
					break;
				case ST_EditAs.oneCell:
					xSSFClientAnchor.AnchorType = AnchorType.MoveDontResize;
					break;
				default:
					xSSFClientAnchor.AnchorType = AnchorType.DontMoveAndResize;
					break;
				}
			}
			string text = cellAnchor.picture?.blipFill?.blip.embed;
			if (text == null)
			{
				continue;
			}
			if (!dictionary.ContainsKey(text))
			{
				XSSFPictureData xSSFPictureData = FindPicture(sheetPictures, text);
				if (xSSFPictureData == null || xSSFPictureData.PictureType == PictureType.None)
				{
					continue;
				}
				dictionary.Add(text, (uint)destWorkbook.AddPicture(xSSFPictureData.Data, xSSFPictureData.PictureType));
			}
			drawing.CreatePicture(xSSFClientAnchor, (int)dictionary[text]);
		}
	}

	private XSSFPictureData FindPicture(List<POIXMLDocumentPart> sheetPictures, string id)
	{
		foreach (POIXMLDocumentPart sheetPicture in sheetPictures)
		{
			if (sheetPicture.GetPackageRelationship().Id == id)
			{
				return sheetPicture as XSSFPictureData;
			}
		}
		return null;
	}

	public void CopyTo(IWorkbook dest, string name, bool copyStyle, bool keepFormulas)
	{
		StylesTable stylesSource = ((XSSFWorkbook)dest).GetStylesSource();
		if (copyStyle && Workbook.NumberOfFonts > 0)
		{
			foreach (XSSFFont font in ((XSSFWorkbook)Workbook).GetStylesSource().GetFonts())
			{
				stylesSource.PutFont(font);
			}
		}
		XSSFSheet xSSFSheet = (XSSFSheet)dest.CreateSheet(name);
		xSSFSheet.sheet.state = sheet.state;
		IDictionary<int, ICellStyle> styleMap = (copyStyle ? new Dictionary<int, ICellStyle>() : null);
		for (int i = FirstRowNum; i <= LastRowNum; i++)
		{
			XSSFRow xSSFRow = (XSSFRow)GetRow(i);
			XSSFRow destRow = (XSSFRow)xSSFSheet.CreateRow(i);
			if (xSSFRow != null)
			{
				CopyRow(this, xSSFSheet, xSSFRow, destRow, styleMap, keepFormulas);
			}
		}
		List<CT_Cols> colsList = worksheet.GetColsList();
		List<CT_Cols> colsList2 = xSSFSheet.worksheet.GetColsList();
		colsList2.Clear();
		foreach (CT_Cols item in colsList)
		{
			CT_Cols cT_Cols = new CT_Cols();
			foreach (CT_Col item2 in item.col)
			{
				cT_Cols.col.Add(item2.Copy());
			}
			colsList2.Add(cT_Cols);
		}
		xSSFSheet.ForceFormulaRecalculation = true;
		xSSFSheet.PrintSetup.Landscape = PrintSetup.Landscape;
		xSSFSheet.PrintSetup.HResolution = PrintSetup.HResolution;
		xSSFSheet.PrintSetup.VResolution = PrintSetup.VResolution;
		xSSFSheet.SetMargin(MarginType.LeftMargin, GetMargin(MarginType.LeftMargin));
		xSSFSheet.SetMargin(MarginType.RightMargin, GetMargin(MarginType.RightMargin));
		xSSFSheet.SetMargin(MarginType.TopMargin, GetMargin(MarginType.TopMargin));
		xSSFSheet.SetMargin(MarginType.BottomMargin, GetMargin(MarginType.BottomMargin));
		xSSFSheet.PrintSetup.HeaderMargin = PrintSetup.HeaderMargin;
		xSSFSheet.PrintSetup.FooterMargin = PrintSetup.FooterMargin;
		xSSFSheet.Header.Left = Header.Left;
		xSSFSheet.Header.Center = Header.Center;
		xSSFSheet.Header.Right = Header.Right;
		xSSFSheet.Footer.Left = Footer.Left;
		xSSFSheet.Footer.Center = Footer.Center;
		xSSFSheet.Footer.Right = Footer.Right;
		xSSFSheet.PrintSetup.Scale = PrintSetup.Scale;
		xSSFSheet.PrintSetup.FitHeight = PrintSetup.FitHeight;
		xSSFSheet.PrintSetup.FitWidth = PrintSetup.FitWidth;
		xSSFSheet.DisplayGridlines = DisplayGridlines;
		if (worksheet.IsSetSheetPr())
		{
			xSSFSheet.worksheet.sheetPr = worksheet.sheetPr.Clone();
		}
		if (GetDefaultSheetView().pane != null)
		{
			CT_Pane pane = GetDefaultSheetView().pane;
			CT_Pane pane2 = xSSFSheet.GetPane();
			pane2.activePane = pane.activePane;
			pane2.state = pane.state;
			pane2.topLeftCell = pane.topLeftCell;
			pane2.xSplit = pane.xSplit;
			pane2.ySplit = pane.ySplit;
		}
		CopySheetImages(dest as XSSFWorkbook, xSSFSheet);
	}

	private static void CopyRow(XSSFSheet srcSheet, XSSFSheet destSheet, XSSFRow srcRow, XSSFRow destRow, IDictionary<int, ICellStyle> styleMap, bool keepFormulas)
	{
		destRow.Height = srcRow.Height;
		if (!srcRow.GetCTRow().IsSetCustomHeight())
		{
			destRow.GetCTRow().UnsetCustomHeight();
		}
		destRow.Hidden = srcRow.Hidden;
		destRow.Collapsed = srcRow.Collapsed;
		destRow.OutlineLevel = srcRow.OutlineLevel;
		if (srcRow.FirstCellNum < 0)
		{
			return;
		}
		for (int i = srcRow.FirstCellNum; i <= srcRow.LastCellNum; i++)
		{
			XSSFCell xSSFCell = (XSSFCell)srcRow.GetCell(i);
			XSSFCell xSSFCell2 = (XSSFCell)destRow.GetCell(i);
			if (srcSheet.Workbook == destSheet.Workbook)
			{
				xSSFCell2 = (XSSFCell)destRow.GetCell(i);
			}
			if (xSSFCell == null)
			{
				continue;
			}
			if (xSSFCell2 == null)
			{
				xSSFCell2 = (XSSFCell)destRow.CreateCell(i);
			}
			CopyCell(xSSFCell, xSSFCell2, styleMap, keepFormulas);
			CellRangeAddress mergedRegion = srcSheet.GetMergedRegion(new CellRangeAddress(srcRow.RowNum, srcRow.RowNum, (short)xSSFCell.ColumnIndex, (short)xSSFCell.ColumnIndex));
			if (mergedRegion != null)
			{
				CellRangeAddress cellRangeAddress = new CellRangeAddress(mergedRegion.FirstRow, mergedRegion.LastRow, mergedRegion.FirstColumn, mergedRegion.LastColumn);
				if (!destSheet.IsMergedRegion(cellRangeAddress))
				{
					destSheet.AddMergedRegion(cellRangeAddress);
				}
			}
		}
	}

	private static void CopyCell(ICell oldCell, ICell newCell, IDictionary<int, ICellStyle> styleMap, bool keepFormulas)
	{
		if (styleMap != null)
		{
			if (oldCell.CellStyle != null)
			{
				if (oldCell.Sheet.Workbook == newCell.Sheet.Workbook)
				{
					newCell.CellStyle = oldCell.CellStyle;
				}
				else
				{
					int hashCode = oldCell.CellStyle.GetHashCode();
					if (styleMap.ContainsKey(hashCode))
					{
						newCell.CellStyle = styleMap[hashCode];
					}
					else
					{
						ICellStyle cellStyle = newCell.Sheet.Workbook.CreateCellStyle();
						cellStyle.CloneStyleFrom(oldCell.CellStyle);
						newCell.CellStyle = cellStyle;
						styleMap.Add(hashCode, cellStyle);
					}
				}
			}
			else
			{
				newCell.CellStyle = null;
			}
		}
		switch (oldCell.CellType)
		{
		case CellType.String:
		{
			XSSFRichTextString xSSFRichTextString = oldCell.RichStringCellValue as XSSFRichTextString;
			newCell.SetCellValue(xSSFRichTextString);
			if (xSSFRichTextString != null)
			{
				for (int i = 0; i < xSSFRichTextString.NumFormattingRuns; i++)
				{
					int indexOfFormattingRun = xSSFRichTextString.GetIndexOfFormattingRun(i);
					int num = 0;
					num = ((i + 1 != xSSFRichTextString.NumFormattingRuns) ? xSSFRichTextString.GetIndexOfFormattingRun(i + 1) : xSSFRichTextString.Length);
					IFont font = newCell.Sheet.Workbook.CreateFont();
					font.CloneStyleFrom(xSSFRichTextString.GetFontOfFormattingRun(i));
					newCell.RichStringCellValue.ApplyFont(indexOfFormattingRun, num, font);
				}
			}
			break;
		}
		case CellType.Numeric:
			newCell.SetCellValue(oldCell.NumericCellValue);
			break;
		case CellType.Blank:
			newCell.SetCellType(CellType.Blank);
			break;
		case CellType.Boolean:
			newCell.SetCellValue(oldCell.BooleanCellValue);
			break;
		case CellType.Error:
			newCell.SetCellValue((int)oldCell.ErrorCellValue);
			break;
		case CellType.Formula:
			if (keepFormulas)
			{
				newCell.SetCellType(CellType.Formula);
				newCell.CellFormula = oldCell.CellFormula;
				break;
			}
			try
			{
				newCell.SetCellType(CellType.Numeric);
				newCell.SetCellValue(oldCell.NumericCellValue);
				break;
			}
			catch (Exception)
			{
				newCell.SetCellType(CellType.String);
				newCell.SetCellValue(oldCell.ToString());
				break;
			}
		}
	}

	public XSSFWorkbook GetWorkbook()
	{
		return (XSSFWorkbook)GetParent();
	}

	private XSSFPivotTable CreatePivotTable()
	{
		XSSFWorkbook workbook = GetWorkbook();
		List<XSSFPivotTable> pivotTables = workbook.PivotTables;
		int idx = GetWorkbook().PivotTables.Count + 1;
		XSSFPivotTable xSSFPivotTable = (XSSFPivotTable)CreateRelationship(XSSFRelation.PIVOT_TABLE, XSSFFactory.GetInstance(), idx);
		xSSFPivotTable.SetParentSheet(this);
		pivotTables.Add(xSSFPivotTable);
		XSSFWorkbook workbook2 = GetWorkbook();
		XSSFPivotCacheDefinition xSSFPivotCacheDefinition = (XSSFPivotCacheDefinition)workbook2.CreateRelationship(XSSFRelation.PIVOT_CACHE_DEFINITION, XSSFFactory.GetInstance(), idx);
		string relationId = workbook2.GetRelationId(xSSFPivotCacheDefinition);
		xSSFPivotTable.GetPackagePart().AddRelationship(xSSFPivotCacheDefinition.GetPackagePart().PartName, TargetMode.Internal, XSSFRelation.PIVOT_CACHE_DEFINITION.Relation);
		xSSFPivotTable.SetPivotCacheDefinition(xSSFPivotCacheDefinition);
		xSSFPivotTable.SetPivotCache(new XSSFPivotCache(workbook2.AddPivotCache(relationId)));
		XSSFPivotCacheRecords part = (XSSFPivotCacheRecords)xSSFPivotCacheDefinition.CreateRelationship(XSSFRelation.PIVOT_CACHE_RECORDS, XSSFFactory.GetInstance(), idx);
		xSSFPivotTable.GetPivotCacheDefinition().GetCTPivotCacheDefinition().id = xSSFPivotCacheDefinition.GetRelationId(part);
		workbook.PivotTables = pivotTables;
		return xSSFPivotTable;
	}

	public XSSFPivotTable CreatePivotTable(AreaReference source, CellReference position, ISheet sourceSheet)
	{
		string sheetName = source.FirstCell.SheetName;
		if (sheetName != null && !sheetName.Equals(sourceSheet.SheetName, StringComparison.InvariantCultureIgnoreCase))
		{
			throw new ArgumentException("The area is referenced in another sheet than the defined source sheet " + sourceSheet.SheetName + ".");
		}
		XSSFPivotTable.IPivotTableReferenceConfigurator refConfig = new PivotTableReferenceConfigurator1(source);
		return CreatePivotTable(position, sourceSheet, refConfig);
	}

	private XSSFPivotTable CreatePivotTable(CellReference position, ISheet sourceSheet, XSSFPivotTable.IPivotTableReferenceConfigurator refConfig)
	{
		XSSFPivotTable xSSFPivotTable = CreatePivotTable();
		xSSFPivotTable.SetDefaultPivotTableDefinition();
		xSSFPivotTable.CreateSourceReferences(position, sourceSheet, refConfig);
		xSSFPivotTable.GetPivotCacheDefinition().CreateCacheFields(sourceSheet);
		xSSFPivotTable.CreateDefaultDataColumns();
		return xSSFPivotTable;
	}

	public XSSFPivotTable CreatePivotTable(AreaReference source, CellReference position)
	{
		string sheetName = source.FirstCell.SheetName;
		if (sheetName != null && !sheetName.Equals(SheetName, StringComparison.InvariantCultureIgnoreCase))
		{
			XSSFSheet sourceSheet = Workbook.GetSheet(sheetName) as XSSFSheet;
			return CreatePivotTable(source, position, sourceSheet);
		}
		return CreatePivotTable(source, position, this);
	}

	public XSSFPivotTable CreatePivotTable(IName source, CellReference position, ISheet sourceSheet)
	{
		if (source.SheetName != null && !source.SheetName.Equals(sourceSheet.SheetName))
		{
			throw new ArgumentException("The named range references another sheet than the defined source sheet " + sourceSheet.SheetName + ".");
		}
		return CreatePivotTable(position, sourceSheet, new PivotTableReferenceConfigurator2(source));
	}

	public XSSFPivotTable CreatePivotTable(IName source, CellReference position)
	{
		return CreatePivotTable(source, position, GetWorkbook().GetSheet(source.SheetName));
	}

	public XSSFPivotTable CreatePivotTable(ITable source, CellReference position)
	{
		return CreatePivotTable(position, GetWorkbook().GetSheet(source.SheetName), new PivotTableReferenceConfigurator3(source));
	}

	public List<XSSFPivotTable> GetPivotTables()
	{
		List<XSSFPivotTable> list = new List<XSSFPivotTable>();
		foreach (XSSFPivotTable pivotTable in GetWorkbook().PivotTables)
		{
			if (pivotTable.GetParent() == this)
			{
				list.Add(pivotTable);
			}
		}
		return list;
	}

	public int GetColumnOutlineLevel(int columnIndex)
	{
		return columnHelper.GetColumn(columnIndex, splitColumns: false)?.outlineLevel ?? 0;
	}

	public bool IsDate1904()
	{
		throw new NotImplementedException();
	}

	public void AddIgnoredErrors(CellReference cell, params IgnoredErrorType[] ignoredErrorTypes)
	{
		AddIgnoredErrors(cell.FormatAsString(), ignoredErrorTypes);
	}

	public void AddIgnoredErrors(CellRangeAddress region, params IgnoredErrorType[] ignoredErrorTypes)
	{
		region.Validate(SpreadsheetVersion.EXCEL2007);
		AddIgnoredErrors(region.FormatAsString(), ignoredErrorTypes);
	}

	public Dictionary<IgnoredErrorType, ISet<CellRangeAddress>> GetIgnoredErrors()
	{
		Dictionary<IgnoredErrorType, ISet<CellRangeAddress>> dictionary = new Dictionary<IgnoredErrorType, ISet<CellRangeAddress>>();
		if (worksheet.IsSetIgnoredErrors())
		{
			foreach (CT_IgnoredError item in worksheet.ignoredErrors.ignoredError)
			{
				foreach (IgnoredErrorType errorType in GetErrorTypes(item))
				{
					if (!dictionary.ContainsKey(errorType))
					{
						dictionary.Add(errorType, new HashSet<CellRangeAddress>());
					}
					foreach (string item2 in item.sqref)
					{
						dictionary[errorType].Add(CellRangeAddress.ValueOf(item2.ToString()));
					}
				}
			}
		}
		return dictionary;
	}

	private void AddIgnoredErrors(string ref1, params IgnoredErrorType[] ignoredErrorTypes)
	{
		XSSFIgnoredErrorHelper.AddIgnoredErrors((worksheet.IsSetIgnoredErrors() ? worksheet.ignoredErrors : worksheet.AddNewIgnoredErrors()).AddNewIgnoredError(), ref1, ignoredErrorTypes);
	}

	private ISet<IgnoredErrorType> GetErrorTypes(CT_IgnoredError err)
	{
		ISet<IgnoredErrorType> set = new HashSet<IgnoredErrorType>();
		IgnoredErrorType[] values = IgnoredErrorTypeValues.Values;
		foreach (IgnoredErrorType ignoredErrorType in values)
		{
			if (XSSFIgnoredErrorHelper.IsSet(ignoredErrorType, err))
			{
				set.Add(ignoredErrorType);
			}
		}
		return set;
	}
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using NPOI.HSSF.Model;
using NPOI.HSSF.Record;
using NPOI.HSSF.Record.Aggregates;
using NPOI.SS;
using NPOI.SS.Formula;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.PTG;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace NPOI.HSSF.UserModel;

[Serializable]
public class HSSFCell : ICell
{
	public const short ENCODING_UNCHANGED = -1;

	public const short ENCODING_COMPRESSED_UNICODE = 0;

	public const short ENCODING_UTF_16 = 1;

	private CellType cellType;

	private HSSFRichTextString stringValue;

	private HSSFWorkbook book;

	private HSSFSheet _sheet;

	private CellValueRecordInterface _record;

	private IComment comment;

	private const string FILE_FORMAT_NAME = "BIFF8";

	public static readonly int LAST_COLUMN_NUMBER = SpreadsheetVersion.EXCEL97.LastColumnIndex;

	private static readonly string LAST_COLUMN_NAME = SpreadsheetVersion.EXCEL97.LastColumnName;

	public InternalWorkbook BoundWorkbook => book.Workbook;

	public ISheet Sheet => _sheet;

	public IRow Row
	{
		get
		{
			int rowIndex = RowIndex;
			return _sheet.GetRow(rowIndex);
		}
	}

	public CellType CellType => cellType;

	public string CellFormula
	{
		get
		{
			if (!(_record is FormulaRecordAggregate))
			{
				throw TypeMismatch(CellType.Formula, cellType, isFormulaCell: true);
			}
			return HSSFFormulaParser.ToFormulaString(book, ((FormulaRecordAggregate)_record).FormulaTokens);
		}
		set
		{
			SetCellFormula(value);
		}
	}

	public double NumericCellValue
	{
		get
		{
			switch (cellType)
			{
			case CellType.Blank:
				return 0.0;
			case CellType.Numeric:
				return ((NumberRecord)_record).Value;
			default:
				throw TypeMismatch(CellType.Numeric, cellType, isFormulaCell: false);
			case CellType.Formula:
			{
				FormulaRecord formulaRecord = ((FormulaRecordAggregate)_record).FormulaRecord;
				CheckFormulaCachedValueType(CellType.Numeric, formulaRecord);
				return formulaRecord.Value;
			}
			}
		}
	}

	public DateTime DateCellValue
	{
		get
		{
			if (cellType == CellType.Blank)
			{
				return DateTime.MaxValue;
			}
			if (cellType == CellType.String)
			{
				throw new InvalidDataException("You cannot get a date value from a String based cell");
			}
			if (cellType == CellType.Boolean)
			{
				throw new InvalidDataException("You cannot get a date value from a bool cell");
			}
			if (cellType == CellType.Error)
			{
				throw new InvalidDataException("You cannot get a date value from an error cell");
			}
			double numericCellValue = NumericCellValue;
			if (book.IsDate1904())
			{
				return DateUtil.GetJavaDate(numericCellValue, use1904windowing: true);
			}
			return DateUtil.GetJavaDate(numericCellValue, use1904windowing: false);
		}
	}

	public string StringCellValue => RichStringCellValue.String;

	public IRichTextString RichStringCellValue
	{
		get
		{
			switch (cellType)
			{
			case CellType.Blank:
				return new HSSFRichTextString("");
			case CellType.String:
				return stringValue;
			default:
				throw TypeMismatch(CellType.String, cellType, isFormulaCell: false);
			case CellType.Formula:
			{
				FormulaRecordAggregate formulaRecordAggregate = (FormulaRecordAggregate)_record;
				CheckFormulaCachedValueType(CellType.String, formulaRecordAggregate.FormulaRecord);
				string text = formulaRecordAggregate.StringValue;
				return new HSSFRichTextString((text == null) ? "" : text);
			}
			}
		}
	}

	public bool BooleanCellValue
	{
		get
		{
			switch (cellType)
			{
			case CellType.Blank:
				return false;
			case CellType.Boolean:
				return ((BoolErrRecord)_record).BooleanValue;
			default:
				throw TypeMismatch(CellType.Boolean, cellType, isFormulaCell: false);
			case CellType.Formula:
			{
				FormulaRecord formulaRecord = ((FormulaRecordAggregate)_record).FormulaRecord;
				CheckFormulaCachedValueType(CellType.Boolean, formulaRecord);
				return formulaRecord.CachedBooleanValue;
			}
			}
		}
	}

	public byte ErrorCellValue
	{
		get
		{
			switch (cellType)
			{
			case CellType.Error:
				return ((BoolErrRecord)_record).ErrorValue;
			default:
				throw TypeMismatch(CellType.Error, cellType, isFormulaCell: false);
			case CellType.Formula:
			{
				FormulaRecord formulaRecord = ((FormulaRecordAggregate)_record).FormulaRecord;
				CheckFormulaCachedValueType(CellType.Error, formulaRecord);
				return (byte)formulaRecord.CachedErrorValue;
			}
			}
		}
	}

	public ICellStyle CellStyle
	{
		get
		{
			short xFIndex = _record.XFIndex;
			ExtendedFormatRecord exFormatAt = book.Workbook.GetExFormatAt(xFIndex);
			return new HSSFCellStyle(xFIndex, exFormatAt, book);
		}
		set
		{
			if (value == null)
			{
				_record.XFIndex = 15;
				return;
			}
			((HSSFCellStyle)value).VerifyBelongsToWorkbook(book);
			short xFIndex = ((((HSSFCellStyle)value).UserStyleName == null) ? value.Index : ApplyUserCellStyle((HSSFCellStyle)value));
			_record.XFIndex = xFIndex;
		}
	}

	public CellValueRecordInterface CellValueRecord => _record;

	public IComment CellComment
	{
		get
		{
			if (comment == null)
			{
				comment = _sheet.FindCellComment(_record.Row, _record.Column);
			}
			return comment;
		}
		set
		{
			if (value == null)
			{
				RemoveCellComment();
				return;
			}
			value.Row = _record.Row;
			value.Column = _record.Column;
			comment = value;
		}
	}

	public int ColumnIndex => _record.Column & 0xFFFF;

	public CellAddress Address => new CellAddress(this);

	public int RowIndex => _record.Row;

	public IHyperlink Hyperlink
	{
		get
		{
			return _sheet.GetHyperlink(_record.Row, _record.Column);
		}
		set
		{
			if (value == null)
			{
				RemoveHyperlink();
				return;
			}
			HSSFHyperlink hSSFHyperlink = (HSSFHyperlink)value;
			value.FirstRow = _record.Row;
			value.LastRow = _record.Row;
			value.FirstColumn = _record.Column;
			value.LastColumn = _record.Column;
			switch (hSSFHyperlink.Type)
			{
			case HyperlinkType.Url:
			case HyperlinkType.Email:
				value.Label = "url";
				break;
			case HyperlinkType.File:
				value.Label = "file";
				break;
			case HyperlinkType.Document:
				value.Label = "place";
				break;
			}
			int index = _sheet.Sheet.FindFirstRecordLocBySid(10);
			_sheet.Sheet.Records.Insert(index, hSSFHyperlink.record);
		}
	}

	public CellType CachedFormulaResultType
	{
		get
		{
			if (cellType != CellType.Formula)
			{
				throw new InvalidOperationException("Only formula cells have cached results");
			}
			return ((FormulaRecordAggregate)_record).FormulaRecord.CachedResultType;
		}
	}

	public bool IsPartOfArrayFormulaGroup
	{
		get
		{
			if (cellType != CellType.Formula)
			{
				return false;
			}
			return ((FormulaRecordAggregate)_record).IsPartOfArrayFormula;
		}
	}

	public CellRangeAddress ArrayFormulaRange
	{
		get
		{
			if (cellType != CellType.Formula)
			{
				string text = new CellReference(this).FormatAsString();
				throw new InvalidOperationException("Cell " + text + " is not part of an array formula.");
			}
			return ((FormulaRecordAggregate)_record).GetArrayFormulaRange();
		}
	}

	public bool IsMergedCell
	{
		get
		{
			foreach (CellRangeAddress mergedRegion in _sheet.Sheet.MergedRecords.MergedRegions)
			{
				if (mergedRegion.FirstColumn <= ColumnIndex && mergedRegion.LastColumn >= ColumnIndex && mergedRegion.FirstRow <= RowIndex && mergedRegion.LastRow >= RowIndex)
				{
					return true;
				}
			}
			return false;
		}
	}

	public HSSFCell(HSSFWorkbook book, HSSFSheet sheet, int row, short col)
		: this(book, sheet, row, col, CellType.Blank)
	{
	}

	public HSSFCell(HSSFWorkbook book, HSSFSheet sheet, int row, short col, CellType type)
	{
		CheckBounds(col);
		cellType = CellType.Unknown;
		stringValue = null;
		this.book = book;
		_sheet = sheet;
		short xFIndexForColAt = sheet.Sheet.GetXFIndexForColAt(col);
		SetCellType(type, setValue: false, row, col, xFIndexForColAt);
	}

	public HSSFCell(HSSFWorkbook book, HSSFSheet sheet, CellValueRecordInterface cval)
	{
		_record = cval;
		cellType = DetermineType(cval);
		stringValue = null;
		this.book = book;
		_sheet = sheet;
		switch (cellType)
		{
		case CellType.String:
			stringValue = new HSSFRichTextString(book.Workbook, (LabelSSTRecord)cval);
			break;
		case CellType.Formula:
			stringValue = new HSSFRichTextString(((FormulaRecordAggregate)cval).StringValue);
			break;
		case CellType.Blank:
			break;
		}
	}

	private HSSFCell()
	{
	}

	private CellType DetermineType(CellValueRecordInterface cval)
	{
		if (cval is FormulaRecordAggregate)
		{
			return CellType.Formula;
		}
		NPOI.HSSF.Record.Record record = (NPOI.HSSF.Record.Record)cval;
		switch ((int)record.Sid)
		{
		case 515:
			return CellType.Numeric;
		case 513:
			return CellType.Blank;
		case 253:
			return CellType.String;
		case -2000:
			return CellType.Formula;
		case 517:
			if (!((BoolErrRecord)record).IsBoolean)
			{
				return CellType.Error;
			}
			return CellType.Boolean;
		default:
			throw new Exception("Bad cell value rec (" + cval.GetType().Name + ")");
		}
	}

	public void SetCellType(CellType cellType)
	{
		NotifyFormulaChanging();
		if (IsPartOfArrayFormulaGroup)
		{
			NotifyArrayFormulaChanging();
		}
		int row = _record.Row;
		int column = _record.Column;
		short xFIndex = _record.XFIndex;
		SetCellType(cellType, setValue: true, row, column, xFIndex);
	}

	private void SetCellType(CellType cellType, bool setValue, int row, int col, short styleIndex)
	{
		if (cellType > CellType.Error)
		{
			throw new Exception("I have no idea what type that Is!");
		}
		switch (cellType)
		{
		case CellType.Formula:
		{
			FormulaRecordAggregate formulaRecordAggregate = null;
			formulaRecordAggregate = ((cellType == this.cellType) ? ((FormulaRecordAggregate)_record) : _sheet.Sheet.RowsAggregate.CreateFormula(row, col));
			formulaRecordAggregate.Column = col;
			if (setValue)
			{
				formulaRecordAggregate.FormulaRecord.Value = NumericCellValue;
			}
			formulaRecordAggregate.XFIndex = styleIndex;
			formulaRecordAggregate.Row = row;
			_record = formulaRecordAggregate;
			break;
		}
		case CellType.Numeric:
		{
			NumberRecord numberRecord = null;
			numberRecord = ((cellType == this.cellType) ? ((NumberRecord)_record) : new NumberRecord());
			numberRecord.Column = col;
			if (setValue)
			{
				numberRecord.Value = NumericCellValue;
			}
			numberRecord.XFIndex = styleIndex;
			numberRecord.Row = row;
			_record = numberRecord;
			break;
		}
		case CellType.String:
		{
			LabelSSTRecord labelSSTRecord = null;
			labelSSTRecord = ((cellType == this.cellType) ? ((LabelSSTRecord)_record) : new LabelSSTRecord());
			labelSSTRecord.Column = col;
			labelSSTRecord.Row = row;
			labelSSTRecord.XFIndex = styleIndex;
			if (setValue)
			{
				string text = ConvertCellValueToString();
				if (text == null)
				{
					SetCellType(CellType.Blank, setValue: false, row, col, styleIndex);
					return;
				}
				int str = (labelSSTRecord.SSTIndex = book.Workbook.AddSSTString(new UnicodeString(text)));
				UnicodeString sSTString = book.Workbook.GetSSTString(str);
				stringValue = new HSSFRichTextString();
				stringValue.UnicodeString = sSTString;
			}
			_record = labelSSTRecord;
			break;
		}
		case CellType.Blank:
		{
			BlankRecord blankRecord = null;
			blankRecord = ((cellType == this.cellType) ? ((BlankRecord)_record) : new BlankRecord());
			blankRecord.Column = col;
			blankRecord.XFIndex = styleIndex;
			blankRecord.Row = row;
			_record = blankRecord;
			break;
		}
		case CellType.Boolean:
		{
			BoolErrRecord boolErrRecord2 = null;
			boolErrRecord2 = ((cellType == this.cellType) ? ((BoolErrRecord)_record) : new BoolErrRecord());
			boolErrRecord2.Column = col;
			if (setValue)
			{
				boolErrRecord2.SetValue(ConvertCellValueToBoolean());
			}
			boolErrRecord2.XFIndex = styleIndex;
			boolErrRecord2.Row = row;
			_record = boolErrRecord2;
			break;
		}
		case CellType.Error:
		{
			BoolErrRecord boolErrRecord = null;
			boolErrRecord = ((cellType == this.cellType) ? ((BoolErrRecord)_record) : new BoolErrRecord());
			boolErrRecord.Column = col;
			if (setValue)
			{
				boolErrRecord.SetValue(FormulaError.VALUE.Code);
			}
			boolErrRecord.XFIndex = styleIndex;
			boolErrRecord.Row = row;
			_record = boolErrRecord;
			break;
		}
		default:
			throw new InvalidOperationException("Invalid cell type: " + cellType);
		}
		if (cellType != this.cellType && this.cellType != CellType.Unknown)
		{
			_sheet.Sheet.ReplaceValueRecord(_record);
		}
		this.cellType = cellType;
	}

	private string ConvertCellValueToString()
	{
		switch (cellType)
		{
		case CellType.Blank:
			return "";
		case CellType.Boolean:
			if (!((BoolErrRecord)_record).BooleanValue)
			{
				return "FALSE";
			}
			return "TRUE";
		case CellType.String:
		{
			int sSTIndex = ((LabelSSTRecord)_record).SSTIndex;
			return book.Workbook.GetSSTString(sSTIndex).String;
		}
		case CellType.Numeric:
			return NumberToTextConverter.ToText(((NumberRecord)_record).Value);
		case CellType.Error:
			return FormulaError.ForInt(((BoolErrRecord)_record).ErrorValue).String;
		default:
			throw new InvalidDataException("Unexpected cell type (" + cellType.ToString() + ")");
		case CellType.Formula:
		{
			FormulaRecordAggregate formulaRecordAggregate = (FormulaRecordAggregate)_record;
			FormulaRecord formulaRecord = formulaRecordAggregate.FormulaRecord;
			switch (formulaRecord.CachedResultType)
			{
			case CellType.Boolean:
				if (!formulaRecord.CachedBooleanValue)
				{
					return "FALSE";
				}
				return "TRUE";
			case CellType.String:
				return formulaRecordAggregate.StringValue;
			case CellType.Numeric:
				return NumberToTextConverter.ToText(formulaRecord.Value);
			case CellType.Error:
				return FormulaError.ForInt(formulaRecord.CachedErrorValue).String;
			default:
				throw new InvalidDataException("Unexpected formula result type (" + cellType.ToString() + ")");
			}
		}
		}
	}

	public void SetCellValue(double value)
	{
		if (double.IsInfinity(value))
		{
			SetCellErrorValue(FormulaError.DIV0.Code);
			return;
		}
		if (double.IsNaN(value))
		{
			SetCellErrorValue(FormulaError.NUM.Code);
			return;
		}
		int row = _record.Row;
		int column = _record.Column;
		short xFIndex = _record.XFIndex;
		switch (cellType)
		{
		case CellType.Numeric:
			((NumberRecord)_record).Value = value;
			break;
		case CellType.Formula:
			((FormulaRecordAggregate)_record).SetCachedDoubleResult(value);
			break;
		default:
			SetCellType(CellType.Numeric, setValue: false, row, column, xFIndex);
			((NumberRecord)_record).Value = value;
			break;
		}
	}

	public void SetCellValue(DateTime value)
	{
		SetCellValue(DateUtil.GetExcelDate(value, book.IsDate1904()));
	}

	public void SetCellValue(string value)
	{
		HSSFRichTextString cellValue = ((value == null) ? null : new HSSFRichTextString(value));
		SetCellValue(cellValue);
	}

	[Obsolete("deprecated 3.15 beta 2. Use {@link #setCellErrorValue(FormulaError)} instead.")]
	public void SetCellErrorValue(byte errorCode)
	{
		FormulaError cellErrorValue = FormulaError.ForInt(errorCode);
		SetCellErrorValue(cellErrorValue);
	}

	public void SetCellErrorValue(FormulaError error)
	{
		int row = _record.Row;
		int column = _record.Column;
		short xFIndex = _record.XFIndex;
		switch (cellType)
		{
		case CellType.Error:
			((BoolErrRecord)_record).SetValue(error);
			break;
		case CellType.Formula:
			((FormulaRecordAggregate)_record).SetCachedErrorResult(error);
			break;
		default:
			SetCellType(CellType.Error, setValue: false, row, column, xFIndex);
			((BoolErrRecord)_record).SetValue(error);
			break;
		}
	}

	public void SetCellValue(IRichTextString value)
	{
		int row = _record.Row;
		int column = _record.Column;
		short xFIndex = _record.XFIndex;
		if (value == null)
		{
			NotifyFormulaChanging();
			SetCellType(CellType.Blank, setValue: false, row, column, xFIndex);
			return;
		}
		if (value.Length > SpreadsheetVersion.EXCEL97.MaxTextLength)
		{
			throw new ArgumentException("The maximum length of cell contents (text) is 32,767 characters");
		}
		if (cellType == CellType.Formula)
		{
			((FormulaRecordAggregate)_record).SetCachedStringResult(value.String);
			stringValue = new HSSFRichTextString(value.String);
			return;
		}
		if (cellType != CellType.String)
		{
			SetCellType(CellType.String, setValue: false, row, column, xFIndex);
		}
		int num = 0;
		HSSFRichTextString hSSFRichTextString = (HSSFRichTextString)value;
		UnicodeString unicodeString = hSSFRichTextString.UnicodeString;
		num = book.Workbook.AddSSTString(unicodeString);
		((LabelSSTRecord)_record).SSTIndex = num;
		stringValue = hSSFRichTextString;
		stringValue.SetWorkbookReferences(book.Workbook, (LabelSSTRecord)_record);
		stringValue.UnicodeString = book.Workbook.GetSSTString(num);
	}

	private void NotifyFormulaChanging()
	{
		if (_record is FormulaRecordAggregate)
		{
			((FormulaRecordAggregate)_record).NotifyFormulaChanging();
		}
	}

	public void SetCellFormula(string formula)
	{
		if (IsPartOfArrayFormulaGroup)
		{
			NotifyArrayFormulaChanging();
		}
		int row = _record.Row;
		int column = _record.Column;
		short xFIndex = _record.XFIndex;
		if (string.IsNullOrEmpty(formula))
		{
			NotifyFormulaChanging();
			SetCellType(CellType.Blank, setValue: false, row, column, xFIndex);
			return;
		}
		int sheetIndex = book.GetSheetIndex(_sheet);
		Ptg[] parsedExpression = HSSFFormulaParser.Parse(formula, book, FormulaType.Cell, sheetIndex);
		SetCellType(CellType.Formula, setValue: false, row, column, xFIndex);
		FormulaRecordAggregate formulaRecordAggregate = (FormulaRecordAggregate)_record;
		FormulaRecord formulaRecord = formulaRecordAggregate.FormulaRecord;
		formulaRecord.Options = 2;
		formulaRecord.Value = 0.0;
		if (formulaRecordAggregate.XFIndex == 0)
		{
			formulaRecordAggregate.XFIndex = 15;
		}
		formulaRecordAggregate.SetParsedExpression(parsedExpression);
	}

	private string GetCellTypeName(CellType cellTypeCode)
	{
		return cellTypeCode switch
		{
			CellType.Blank => "blank", 
			CellType.String => "text", 
			CellType.Boolean => "boolean", 
			CellType.Error => "error", 
			CellType.Numeric => "numeric", 
			CellType.Formula => "formula", 
			_ => "#unknown cell type (" + cellTypeCode.ToString() + ")#", 
		};
	}

	private Exception TypeMismatch(CellType expectedTypeCode, CellType actualTypeCode, bool isFormulaCell)
	{
		return new InvalidOperationException("Cannot get a " + GetCellTypeName(expectedTypeCode) + " value from a " + GetCellTypeName(actualTypeCode) + " " + (isFormulaCell ? "formula " : "") + "cell");
	}

	private void CheckFormulaCachedValueType(CellType expectedTypeCode, FormulaRecord fr)
	{
		CellType cachedResultType = fr.CachedResultType;
		if (cachedResultType != expectedTypeCode)
		{
			throw TypeMismatch(expectedTypeCode, cachedResultType, isFormulaCell: true);
		}
	}

	public void SetCellValue(bool value)
	{
		int row = _record.Row;
		int column = _record.Column;
		short xFIndex = _record.XFIndex;
		switch (cellType)
		{
		case CellType.Boolean:
			((BoolErrRecord)_record).SetValue(value);
			break;
		case CellType.Formula:
			((FormulaRecordAggregate)_record).SetCachedBooleanResult(value);
			break;
		default:
			SetCellType(CellType.Boolean, setValue: false, row, column, xFIndex);
			((BoolErrRecord)_record).SetValue(value);
			break;
		}
	}

	private bool ConvertCellValueToBoolean()
	{
		switch (cellType)
		{
		case CellType.Boolean:
			return ((BoolErrRecord)_record).BooleanValue;
		case CellType.String:
		{
			int sSTIndex = ((LabelSSTRecord)_record).SSTIndex;
			return Convert.ToBoolean(book.Workbook.GetSSTString(sSTIndex).String, CultureInfo.CurrentCulture);
		}
		case CellType.Numeric:
			return ((NumberRecord)_record).Value != 0.0;
		case CellType.Formula:
		{
			FormulaRecord formulaRecord = ((FormulaRecordAggregate)_record).FormulaRecord;
			CheckFormulaCachedValueType(CellType.Boolean, formulaRecord);
			return formulaRecord.CachedBooleanValue;
		}
		case CellType.Blank:
		case CellType.Error:
			return false;
		default:
			throw new Exception("Unexpected cell type (" + cellType.ToString() + ")");
		}
	}

	private short ApplyUserCellStyle(HSSFCellStyle style)
	{
		if (style.UserStyleName == null)
		{
			throw new ArgumentException("Expected user-defined style");
		}
		InternalWorkbook workbook = book.Workbook;
		short num = -1;
		int numExFormats = workbook.NumExFormats;
		for (short num2 = 0; num2 < numExFormats; num2++)
		{
			ExtendedFormatRecord exFormatAt = workbook.GetExFormatAt(num2);
			if (exFormatAt.XFType == 0 && exFormatAt.ParentIndex == style.Index)
			{
				num = num2;
				break;
			}
		}
		if (num == -1)
		{
			ExtendedFormatRecord extendedFormatRecord = workbook.CreateCellXF();
			extendedFormatRecord.CloneStyleFrom(workbook.GetExFormatAt(style.Index));
			extendedFormatRecord.IndentionOptions = 0;
			extendedFormatRecord.XFType = 0;
			extendedFormatRecord.ParentIndex = style.Index;
			return (short)numExFormats;
		}
		return num;
	}

	private void CheckBounds(int cellIndex)
	{
		if (cellIndex < 0 || cellIndex > LAST_COLUMN_NUMBER)
		{
			string[] obj = new string[7]
			{
				"Invalid column index (",
				cellIndex.ToString(),
				").  Allowable column range for BIFF8 is (0..",
				null,
				null,
				null,
				null
			};
			int lAST_COLUMN_NUMBER = LAST_COLUMN_NUMBER;
			obj[3] = lAST_COLUMN_NUMBER.ToString();
			obj[4] = ") or ('A'..'";
			obj[5] = LAST_COLUMN_NAME;
			obj[6] = "')";
			throw new ArgumentException(string.Concat(obj));
		}
	}

	public void SetAsActiveCell()
	{
		int row = _record.Row;
		int column = _record.Column;
		_sheet.Sheet.SetActiveCell(row, column);
	}

	public override string ToString()
	{
		switch (CellType)
		{
		case CellType.Blank:
			return "";
		case CellType.Boolean:
			if (!BooleanCellValue)
			{
				return "FALSE";
			}
			return "TRUE";
		case CellType.Error:
			return ErrorEval.GetText(((BoolErrRecord)_record).ErrorValue);
		case CellType.Formula:
			return CellFormula;
		case CellType.Numeric:
			CellStyle.GetDataFormatString();
			return new DataFormatter().FormatCellValue(this);
		case CellType.String:
			return StringCellValue;
		default:
			return "Unknown Cell Type: " + CellType;
		}
	}

	public void RemoveCellComment()
	{
		HSSFComment hSSFComment = _sheet.FindCellComment(_record.Row, _record.Column);
		comment = null;
		if (hSSFComment != null)
		{
			(_sheet.DrawingPatriarch as HSSFPatriarch).RemoveShape(hSSFComment);
		}
	}

	internal void UpdateCellNum(int num)
	{
		_record.Column = num;
	}

	public void RemoveHyperlink()
	{
		RecordBase recordBase = null;
		IEnumerator<RecordBase> enumerator = _sheet.Sheet.Records.GetEnumerator();
		while (enumerator.MoveNext())
		{
			RecordBase current = enumerator.Current;
			if (current is HyperlinkRecord)
			{
				HyperlinkRecord hyperlinkRecord = (HyperlinkRecord)current;
				if (hyperlinkRecord.FirstColumn == _record.Column && hyperlinkRecord.FirstRow == _record.Row)
				{
					recordBase = current;
					break;
				}
			}
		}
		if (recordBase != null)
		{
			_sheet.Sheet.Records.Remove(recordBase);
		}
	}

	internal void SetCellArrayFormula(CellRangeAddress range)
	{
		int row = _record.Row;
		int column = _record.Column;
		short xFIndex = _record.XFIndex;
		SetCellType(CellType.Formula, setValue: false, row, column, xFIndex);
		Ptg[] parsedExpression = new Ptg[1]
		{
			new ExpPtg(range.FirstRow, range.FirstColumn)
		};
		((FormulaRecordAggregate)_record).SetParsedExpression(parsedExpression);
	}

	public ICell CopyCellTo(int targetIndex)
	{
		return Row.CopyCell(ColumnIndex, targetIndex);
	}

	internal void NotifyArrayFormulaChanging(string msg)
	{
		if (ArrayFormulaRange.NumberOfCells > 1)
		{
			throw new InvalidOperationException(msg);
		}
		Row.Sheet.RemoveArrayFormula(this);
	}

	internal void NotifyArrayFormulaChanging()
	{
		CellReference cellReference = new CellReference(this);
		string msg = "Cell " + cellReference.FormatAsString() + " is part of a multi-cell array formula. You cannot change part of an array.";
		NotifyArrayFormulaChanging(msg);
	}

	public CellType GetCachedFormulaResultTypeEnum()
	{
		throw new NotImplementedException();
	}

	public void SetBlank()
	{
		SetCellType(CellType.Blank);
	}
}

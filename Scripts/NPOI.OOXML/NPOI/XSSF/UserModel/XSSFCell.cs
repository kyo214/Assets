using System;
using System.Globalization;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS;
using NPOI.SS.Formula;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.PTG;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;
using NPOI.XSSF.Model;

namespace NPOI.XSSF.UserModel;

public class XSSFCell : ICell
{
	private static string FALSE_AS_STRING = "0";

	private static string TRUE_AS_STRING = "1";

	private CT_Cell _cell;

	private XSSFRow _row;

	private int _cellNum;

	private SharedStringsTable _sharedStringSource;

	private StylesTable _stylesSource;

	public ISheet Sheet => _row.Sheet;

	public IRow Row => _row;

	public bool BooleanCellValue
	{
		get
		{
			CellType cellType = CellType;
			switch (cellType)
			{
			case CellType.Blank:
				return false;
			case CellType.Boolean:
				if (_cell.IsSetV())
				{
					return TRUE_AS_STRING.Equals(_cell.v);
				}
				return false;
			case CellType.Formula:
				if (_cell.IsSetV())
				{
					return TRUE_AS_STRING.Equals(_cell.v);
				}
				return false;
			default:
				throw TypeMismatch(CellType.Boolean, cellType, isFormulaCell: false);
			}
		}
	}

	public double NumericCellValue
	{
		get
		{
			CellType cellType = CellType;
			switch (cellType)
			{
			case CellType.Blank:
				return 0.0;
			case CellType.Numeric:
			case CellType.Formula:
				if (_cell.IsSetV())
				{
					if (string.IsNullOrEmpty(_cell.v))
					{
						return 0.0;
					}
					try
					{
						return double.Parse(_cell.v, CultureInfo.InvariantCulture);
					}
					catch (FormatException)
					{
						throw TypeMismatch(CellType.Numeric, CellType.String, isFormulaCell: false);
					}
				}
				return 0.0;
			default:
				throw TypeMismatch(CellType.Numeric, cellType, isFormulaCell: false);
			}
		}
	}

	public string StringCellValue => RichStringCellValue.String;

	public IRichTextString RichStringCellValue
	{
		get
		{
			CellType cellType = CellType;
			XSSFRichTextString xSSFRichTextString;
			switch (cellType)
			{
			case CellType.Blank:
				xSSFRichTextString = new XSSFRichTextString("");
				break;
			case CellType.String:
				if (_cell.t == ST_CellType.inlineStr)
				{
					xSSFRichTextString = ((!_cell.IsSetIs()) ? ((!_cell.IsSetV()) ? new XSSFRichTextString("") : new XSSFRichTextString(_cell.v)) : new XSSFRichTextString(_cell.@is));
				}
				else if (_cell.t == ST_CellType.str)
				{
					xSSFRichTextString = new XSSFRichTextString(_cell.IsSetV() ? _cell.v : "");
				}
				else if (_cell.IsSetV())
				{
					int idx = int.Parse(_cell.v);
					xSSFRichTextString = new XSSFRichTextString(_sharedStringSource.GetEntryAt(idx));
				}
				else
				{
					xSSFRichTextString = new XSSFRichTextString("");
				}
				break;
			case CellType.Formula:
				CheckFormulaCachedValueType(CellType.String, GetBaseCellType(blankCells: false));
				xSSFRichTextString = new XSSFRichTextString(_cell.IsSetV() ? _cell.v : "");
				break;
			default:
				throw TypeMismatch(CellType.String, cellType, isFormulaCell: false);
			}
			xSSFRichTextString.SetStylesTableReference(_stylesSource);
			return xSSFRichTextString;
		}
	}

	public string CellFormula
	{
		get
		{
			return GetCellFormula(null);
		}
		set
		{
			SetCellFormula(value);
		}
	}

	public int ColumnIndex => _cellNum;

	public int RowIndex => _row.RowNum;

	public CellAddress Address => new CellAddress(this);

	public ICellStyle CellStyle
	{
		get
		{
			XSSFCellStyle result = null;
			if (_stylesSource != null && _stylesSource.NumCellStyles > 0)
			{
				long num = (_cell.IsSetS() ? _cell.s : 0);
				result = _stylesSource.GetStyleAt((int)num);
			}
			return result;
		}
		set
		{
			if (value == null)
			{
				if (_cell.IsSetS())
				{
					_cell.unsetS();
				}
			}
			else
			{
				XSSFCellStyle xSSFCellStyle = (XSSFCellStyle)value;
				xSSFCellStyle.VerifyBelongsToStylesSource(_stylesSource);
				long num = _stylesSource.PutStyle(xSSFCellStyle);
				_cell.s = (uint)num;
			}
		}
	}

	private bool IsFormulaCell
	{
		get
		{
			if (_cell.f != null || ((XSSFSheet)Sheet).IsCellInArrayFormulaContext(this))
			{
				return true;
			}
			return false;
		}
	}

	public CellType CellType
	{
		get
		{
			if (IsFormulaCell)
			{
				return CellType.Formula;
			}
			return GetBaseCellType(blankCells: true);
		}
	}

	public CellType CachedFormulaResultType
	{
		get
		{
			if (!IsFormulaCell)
			{
				throw new InvalidOperationException("Only formula cells have cached results");
			}
			return GetBaseCellType(blankCells: false);
		}
	}

	public DateTime DateCellValue
	{
		get
		{
			if (CellType == CellType.Blank)
			{
				return DateTime.MinValue;
			}
			double numericCellValue = NumericCellValue;
			bool use1904windowing = Sheet.Workbook.IsDate1904();
			return DateUtil.GetJavaDate(numericCellValue, use1904windowing);
		}
	}

	public string ErrorCellString
	{
		get
		{
			CellType baseCellType = GetBaseCellType(blankCells: true);
			if (baseCellType != CellType.Error)
			{
				throw TypeMismatch(CellType.Error, baseCellType, isFormulaCell: false);
			}
			return _cell.v;
		}
	}

	public byte ErrorCellValue
	{
		get
		{
			string errorCellString = ErrorCellString;
			if (errorCellString == null)
			{
				return 0;
			}
			return FormulaError.ForString(errorCellString).Code;
		}
	}

	public IComment CellComment
	{
		get
		{
			return Sheet.GetCellComment(new CellAddress(this));
		}
		set
		{
			if (value == null)
			{
				RemoveCellComment();
			}
			else
			{
				value.SetAddress(RowIndex, ColumnIndex);
			}
		}
	}

	public IHyperlink Hyperlink
	{
		get
		{
			return ((XSSFSheet)Sheet).GetHyperlink(_row.RowNum, _cellNum);
		}
		set
		{
			if (value == null)
			{
				RemoveHyperlink();
				return;
			}
			XSSFHyperlink xSSFHyperlink = (XSSFHyperlink)value;
			xSSFHyperlink.SetCellReference(new CellReference(_row.RowNum, _cellNum).FormatAsString());
			((XSSFSheet)Sheet).AddHyperlink(xSSFHyperlink);
		}
	}

	public CellRangeAddress ArrayFormulaRange => CellRangeAddress.ValueOf((((XSSFSheet)Sheet).GetFirstCellInArrayFormula(this) ?? throw new InvalidOperationException("Cell " + GetReference() + " is not part of an array formula."))._cell.f.@ref);

	public bool IsPartOfArrayFormulaGroup => ((XSSFSheet)Sheet).IsCellInArrayFormulaContext(this);

	public bool IsMergedCell => Sheet.IsMergedRegion(new CellRangeAddress(RowIndex, RowIndex, ColumnIndex, ColumnIndex));

	public XSSFCell(XSSFRow row, CT_Cell cell)
	{
		_cell = cell;
		_row = row;
		if (cell.r != null)
		{
			_cellNum = new CellReference(cell.r).Col;
		}
		else
		{
			int lastCellNum = row.LastCellNum;
			if (lastCellNum != -1)
			{
				_cellNum = row.GetCell(lastCellNum - 1, MissingCellPolicy.RETURN_NULL_AND_BLANK).ColumnIndex + 1;
			}
		}
		_sharedStringSource = ((XSSFWorkbook)row.Sheet.Workbook).GetSharedStringSource();
		_stylesSource = ((XSSFWorkbook)row.Sheet.Workbook).GetStylesSource();
	}

	public void CopyCellFrom(ICell srcCell, CellCopyPolicy policy)
	{
		if (policy.IsCopyCellValue)
		{
			if (srcCell != null)
			{
				CellType cellType = srcCell.CellType;
				if (cellType == CellType.Formula && !policy.IsCopyCellFormula)
				{
					cellType = srcCell.CachedFormulaResultType;
				}
				switch (cellType)
				{
				case CellType.Boolean:
					SetCellValue(srcCell.BooleanCellValue);
					break;
				case CellType.Error:
					SetCellErrorValue(srcCell.ErrorCellValue);
					break;
				case CellType.Formula:
					SetCellFormula(srcCell.CellFormula);
					break;
				case CellType.Numeric:
					if (DateUtil.IsCellDateFormatted(srcCell))
					{
						SetCellValue(srcCell.DateCellValue);
					}
					else
					{
						SetCellValue(srcCell.NumericCellValue);
					}
					break;
				case CellType.String:
					SetCellValue(srcCell.StringCellValue);
					break;
				case CellType.Blank:
					SetBlankInternal();
					break;
				default:
					throw new ArgumentException("Invalid cell type " + srcCell.CellType);
				}
			}
			else
			{
				SetBlankInternal();
			}
		}
		if (policy.IsCopyCellStyle)
		{
			if (srcCell != null)
			{
				CellStyle = srcCell.CellStyle;
			}
			else
			{
				CellStyle = null;
			}
		}
		if (policy.IsMergeHyperlink)
		{
			IHyperlink hyperlink = srcCell.Hyperlink;
			if (hyperlink != null)
			{
				Hyperlink = new XSSFHyperlink(hyperlink);
			}
		}
		else if (policy.IsCopyHyperlink)
		{
			IHyperlink hyperlink2 = srcCell.Hyperlink;
			if (hyperlink2 == null)
			{
				Hyperlink = null;
			}
			else
			{
				Hyperlink = new XSSFHyperlink(hyperlink2);
			}
		}
	}

	protected SharedStringsTable GetSharedStringSource()
	{
		return _sharedStringSource;
	}

	protected StylesTable GetStylesSource()
	{
		return _stylesSource;
	}

	public void SetCellValue(bool value)
	{
		_cell.t = ST_CellType.b;
		_cell.v = (value ? TRUE_AS_STRING : FALSE_AS_STRING);
	}

	public void SetCellValue(double value)
	{
		if (double.IsInfinity(value))
		{
			_cell.t = ST_CellType.e;
			_cell.v = FormulaError.DIV0.String;
		}
		else if (double.IsNaN(value))
		{
			_cell.t = ST_CellType.e;
			_cell.v = FormulaError.NUM.String;
		}
		else
		{
			_cell.t = ST_CellType.n;
			_cell.v = value.ToString(CultureInfo.InvariantCulture);
		}
	}

	private static void CheckFormulaCachedValueType(CellType expectedTypeCode, CellType cachedValueType)
	{
		if (cachedValueType != expectedTypeCode)
		{
			throw TypeMismatch(expectedTypeCode, cachedValueType, isFormulaCell: true);
		}
	}

	public void SetCellValue(string str)
	{
		SetCellValue((str == null) ? null : new XSSFRichTextString(str));
	}

	public void SetCellValue(IRichTextString str)
	{
		if (str == null || str.String == null)
		{
			SetCellType(CellType.Blank);
			return;
		}
		if (str.Length > SpreadsheetVersion.EXCEL2007.MaxTextLength)
		{
			throw new ArgumentException("The maximum length of cell contents (text) is 32,767 characters");
		}
		if (CellType == CellType.Formula)
		{
			_cell.v = str.String;
			_cell.t = ST_CellType.str;
			return;
		}
		if (_cell.t == ST_CellType.inlineStr)
		{
			_cell.v = str.String;
			return;
		}
		_cell.t = ST_CellType.s;
		XSSFRichTextString xSSFRichTextString = (XSSFRichTextString)str;
		xSSFRichTextString.SetStylesTableReference(_stylesSource);
		int num = _sharedStringSource.AddEntry(xSSFRichTextString.GetCTRst());
		_cell.v = num.ToString();
	}

	protected internal string GetCellFormula(XSSFEvaluationWorkbook fpb)
	{
		CellType cellType = CellType;
		if (cellType != CellType.Formula)
		{
			throw TypeMismatch(CellType.Formula, cellType, isFormulaCell: false);
		}
		CT_CellFormula f = _cell.f;
		if (IsPartOfArrayFormulaGroup && f == null)
		{
			return ((XSSFSheet)Sheet).GetFirstCellInArrayFormula(this).GetCellFormula(fpb);
		}
		if (f.t == ST_CellFormulaType.shared)
		{
			return ConvertSharedFormula((int)f.si, (fpb == null) ? XSSFEvaluationWorkbook.Create(Sheet.Workbook) : fpb);
		}
		return f.Value;
	}

	private string ConvertSharedFormula(int si, XSSFEvaluationWorkbook fpb)
	{
		XSSFSheet xSSFSheet = (XSSFSheet)Sheet;
		CT_CellFormula obj = xSSFSheet.GetSharedFormula(si) ?? throw new InvalidOperationException("Master cell of a shared formula with sid=" + si + " was not found");
		string value = obj.Value;
		CellRangeAddress cellRangeAddress = CellRangeAddress.ValueOf(obj.@ref);
		int sheetIndex = xSSFSheet.Workbook.GetSheetIndex(xSSFSheet);
		SharedFormula sharedFormula = new SharedFormula(SpreadsheetVersion.EXCEL2007);
		Ptg[] ptgs = FormulaParser.Parse(value, fpb, FormulaType.Cell, sheetIndex, RowIndex);
		Ptg[] ptgs2 = sharedFormula.ConvertSharedFormulas(ptgs, RowIndex - cellRangeAddress.FirstRow, ColumnIndex - cellRangeAddress.FirstColumn);
		return FormulaRenderer.ToFormulaString(fpb, ptgs2);
	}

	public void SetCellFormula(string formula)
	{
		if (IsPartOfArrayFormulaGroup)
		{
			NotifyArrayFormulaChanging();
		}
		SetFormula(formula, FormulaType.Cell);
	}

	internal void SetCellArrayFormula(string formula, CellRangeAddress range)
	{
		SetFormula(formula, FormulaType.Array);
		CT_CellFormula f = _cell.f;
		f.t = ST_CellFormulaType.array;
		f.@ref = range.FormatAsString();
	}

	private void SetFormula(string formula, FormulaType formulaType)
	{
		IWorkbook workbook = _row.Sheet.Workbook;
		if (formula == null)
		{
			((XSSFWorkbook)workbook).OnDeleteFormula(this);
			if (_cell.IsSetF())
			{
				_cell.unsetF();
			}
			return;
		}
		IFormulaParsingWorkbook workbook2 = XSSFEvaluationWorkbook.Create(workbook);
		FormulaParser.Parse(formula, workbook2, formulaType, workbook.GetSheetIndex(Sheet), RowIndex);
		CT_CellFormula cT_CellFormula = new CT_CellFormula();
		cT_CellFormula.Value = formula;
		_cell.f = cT_CellFormula;
		if (_cell.IsSetV())
		{
			_cell.unsetV();
		}
	}

	public string GetReference()
	{
		string r = _cell.r;
		if (r == null)
		{
			return new CellAddress(this).FormatAsString();
		}
		return r;
	}

	private CellType GetBaseCellType(bool blankCells)
	{
		switch (_cell.t)
		{
		case ST_CellType.b:
			return CellType.Boolean;
		case ST_CellType.n:
			if (!_cell.IsSetV() & blankCells)
			{
				return CellType.Blank;
			}
			return CellType.Numeric;
		case ST_CellType.e:
			return CellType.Error;
		case ST_CellType.s:
		case ST_CellType.str:
		case ST_CellType.inlineStr:
			return CellType.String;
		default:
			throw new InvalidOperationException("Illegal cell type: " + _cell.t);
		}
	}

	public void SetCellValue(DateTime? value)
	{
		if (!value.HasValue)
		{
			SetCellType(CellType.Blank);
		}
		else
		{
			SetCellValue(value.Value);
		}
	}

	public void SetCellValue(DateTime value)
	{
		bool use1904windowing = Sheet.Workbook.IsDate1904();
		SetCellValue(DateUtil.GetExcelDate(value, use1904windowing));
	}

	public void SetCellErrorValue(byte errorCode)
	{
		FormulaError cellErrorValue = FormulaError.ForInt(errorCode);
		SetCellErrorValue(cellErrorValue);
	}

	public void SetCellErrorValue(FormulaError error)
	{
		_cell.t = ST_CellType.e;
		_cell.v = error.String;
	}

	public void SetAsActiveCell()
	{
		Sheet.ActiveCell = Address;
	}

	private void SetBlankInternal()
	{
		CT_Cell cT_Cell = new CT_Cell();
		cT_Cell.r = _cell.r;
		if (_cell.IsSetS())
		{
			cT_Cell.s = _cell.s;
		}
		_cell.Set(cT_Cell);
	}

	public void SetBlank()
	{
		SetCellType(CellType.Blank);
	}

	internal void SetCellNum(int num)
	{
		CheckBounds(num);
		_cellNum = num;
		string r = new CellReference(RowIndex, ColumnIndex).FormatAsString();
		_cell.r = r;
	}

	public void SetCellType(CellType cellType)
	{
		CellType cellType2 = CellType;
		if (IsPartOfArrayFormulaGroup)
		{
			NotifyArrayFormulaChanging();
		}
		if (cellType2 == CellType.Formula && cellType != CellType.Formula)
		{
			((XSSFWorkbook)Sheet.Workbook).OnDeleteFormula(this);
		}
		switch (cellType)
		{
		case CellType.Blank:
			SetBlankInternal();
			break;
		case CellType.Boolean:
		{
			string v = (ConvertCellValueToBoolean() ? TRUE_AS_STRING : FALSE_AS_STRING);
			_cell.t = ST_CellType.b;
			_cell.v = v;
			break;
		}
		case CellType.Numeric:
			_cell.t = ST_CellType.n;
			break;
		case CellType.Error:
			_cell.t = ST_CellType.e;
			break;
		case CellType.String:
			if (cellType2 != CellType.String)
			{
				XSSFRichTextString xSSFRichTextString = new XSSFRichTextString(ConvertCellValueToString());
				xSSFRichTextString.SetStylesTableReference(_stylesSource);
				int num = _sharedStringSource.AddEntry(xSSFRichTextString.GetCTRst());
				_cell.v = num.ToString();
			}
			_cell.t = ST_CellType.s;
			break;
		case CellType.Formula:
			if (!_cell.IsSetF())
			{
				CT_CellFormula cT_CellFormula = new CT_CellFormula();
				cT_CellFormula.Value = "0";
				_cell.f = cT_CellFormula;
				if (_cell.IsSetT())
				{
					_cell.unsetT();
				}
			}
			break;
		default:
			throw new ArgumentException("Illegal cell type: " + cellType);
		}
		if (cellType != CellType.Formula && _cell.IsSetF())
		{
			_cell.unsetF();
		}
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
			return ErrorEval.GetText(ErrorCellValue);
		case CellType.Formula:
			return CellFormula;
		case CellType.Numeric:
			if (DateUtil.IsCellDateFormatted(this))
			{
				return new SimpleDateFormat("dd-MMM-yyyy").Format(DateCellValue, CultureInfo.CurrentCulture);
			}
			return NumericCellValue.ToString();
		case CellType.String:
			return RichStringCellValue.ToString();
		default:
			return "Unknown Cell Type: " + CellType;
		}
	}

	public string GetRawValue()
	{
		return _cell.v;
	}

	private static string GetCellTypeName(CellType cellTypeCode)
	{
		return cellTypeCode switch
		{
			CellType.Blank => "blank", 
			CellType.String => "text", 
			CellType.Boolean => "bool", 
			CellType.Error => "error", 
			CellType.Numeric => "numeric", 
			CellType.Formula => "formula", 
			_ => "#unknown cell type (" + cellTypeCode.ToString() + ")#", 
		};
	}

	private static Exception TypeMismatch(CellType expectedTypeCode, CellType actualTypeCode, bool isFormulaCell)
	{
		return new InvalidOperationException("Cannot get a " + GetCellTypeName(expectedTypeCode) + " value from a " + GetCellTypeName(actualTypeCode) + " " + (isFormulaCell ? "formula " : "") + "cell");
	}

	private static void CheckBounds(int cellIndex)
	{
		SpreadsheetVersion eXCEL = SpreadsheetVersion.EXCEL2007;
		int lastColumnIndex = SpreadsheetVersion.EXCEL2007.LastColumnIndex;
		if (cellIndex < 0 || cellIndex > lastColumnIndex)
		{
			throw new ArgumentException("Invalid column index (" + cellIndex + ").  Allowable column range for " + eXCEL.ToString() + " is (0.." + lastColumnIndex + ") or ('A'..'" + eXCEL.LastColumnName + "')");
		}
	}

	public void RemoveCellComment()
	{
		if (CellComment != null)
		{
			CellAddress cellRef = new CellAddress(GetReference());
			XSSFSheet obj = (XSSFSheet)Sheet;
			obj.GetCommentsTable(create: false).RemoveComment(cellRef);
			obj.GetVMLDrawing(autoCreate: false).RemoveCommentShape(RowIndex, ColumnIndex);
		}
	}

	public void RemoveHyperlink()
	{
		((XSSFSheet)Sheet).RemoveHyperlink(_row.RowNum, _cellNum);
	}

	internal CT_Cell GetCTCell()
	{
		return _cell;
	}

	private bool ConvertCellValueToBoolean()
	{
		CellType cellType = CellType;
		if (cellType == CellType.Formula)
		{
			cellType = GetBaseCellType(blankCells: false);
		}
		switch (cellType)
		{
		case CellType.Boolean:
			return TRUE_AS_STRING.Equals(_cell.v);
		case CellType.String:
		{
			int idx = int.Parse(_cell.v);
			return bool.Parse(new XSSFRichTextString(_sharedStringSource.GetEntryAt(idx)).String);
		}
		case CellType.Numeric:
			return double.Parse(_cell.v, CultureInfo.InvariantCulture) != 0.0;
		case CellType.Blank:
		case CellType.Error:
			return false;
		default:
			throw new RuntimeException("Unexpected cell type (" + cellType.ToString() + ")");
		}
	}

	private string ConvertCellValueToString()
	{
		CellType cellType = CellType;
		switch (cellType)
		{
		case CellType.Blank:
			return "";
		case CellType.Boolean:
			if (!TRUE_AS_STRING.Equals(_cell.v))
			{
				return "FALSE";
			}
			return "TRUE";
		case CellType.String:
		{
			int idx = int.Parse(_cell.v);
			return new XSSFRichTextString(_sharedStringSource.GetEntryAt(idx)).String;
		}
		case CellType.Numeric:
		case CellType.Error:
			return _cell.v;
		default:
			throw new InvalidOperationException("Unexpected cell type (" + cellType.ToString() + ")");
		case CellType.Formula:
		{
			cellType = GetBaseCellType(blankCells: false);
			string v = _cell.v;
			switch (cellType)
			{
			case CellType.Boolean:
				if (TRUE_AS_STRING.Equals(v))
				{
					return "TRUE";
				}
				if (FALSE_AS_STRING.Equals(v))
				{
					return "FALSE";
				}
				throw new InvalidOperationException("Unexpected bool cached formula value '" + v + "'.");
			case CellType.Numeric:
			case CellType.String:
			case CellType.Error:
				return v;
			default:
				throw new InvalidOperationException("Unexpected formula result type (" + cellType.ToString() + ")");
			}
		}
		}
	}

	internal void NotifyArrayFormulaChanging(string msg)
	{
		if (IsPartOfArrayFormulaGroup)
		{
			if (ArrayFormulaRange.NumberOfCells > 1)
			{
				throw new InvalidOperationException(msg);
			}
			Row.Sheet.RemoveArrayFormula(this);
		}
	}

	internal void NotifyArrayFormulaChanging()
	{
		CellReference cellReference = new CellReference(this);
		string msg = "Cell " + cellReference.FormatAsString() + " is part of a multi-cell array formula. You cannot change part of an array.";
		NotifyArrayFormulaChanging(msg);
	}

	public ICell CopyCellTo(int targetIndex)
	{
		return CellUtil.CopyCell(Row, ColumnIndex, targetIndex);
	}

	public CellType GetCachedFormulaResultTypeEnum()
	{
		throw new NotImplementedException();
	}
}

using System;
using NPOI.SS;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;
using NPOI.XSSF.Streaming.Properties;
using NPOI.XSSF.Streaming.Values;
using NPOI.XSSF.UserModel;

namespace NPOI.XSSF.Streaming;

public class SXSSFCell : ICell
{
	private static POILogger logger = POILogFactory.GetLogger(typeof(SXSSFCell));

	private SXSSFRow _row;

	private Value _value;

	private ICellStyle _style;

	private Property _firstProperty;

	public CellRangeAddress ArrayFormulaRange
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public bool BooleanCellValue
	{
		get
		{
			CellType type = _value.GetType();
			switch (type)
			{
			case CellType.Blank:
				return false;
			case CellType.Formula:
				if (((FormulaValue)_value).GetFormulaType() != CellType.Boolean)
				{
					throw typeMismatch(CellType.Boolean, CellType.Formula, isFormulaCell: false);
				}
				return ((BooleanFormulaValue)_value).PreEvaluatedValue;
			case CellType.Boolean:
				return ((BooleanValue)_value).Value;
			default:
				throw typeMismatch(CellType.Boolean, type, isFormulaCell: false);
			}
		}
	}

	public CellType CachedFormulaResultType => GetCachedFormulaResultTypeEnum();

	public IComment CellComment
	{
		get
		{
			return (IComment)GetPropertyValue(1);
		}
		set
		{
			SetProperty(1, value);
		}
	}

	public string CellFormula
	{
		get
		{
			if (_value.GetType() != CellType.Formula)
			{
				throw typeMismatch(CellType.Formula, _value.GetType(), isFormulaCell: false);
			}
			return ((FormulaValue)_value).Value;
		}
		set
		{
			if (value == null)
			{
				SetType(CellType.Blank);
				return;
			}
			EnsureFormulaType(ComputeTypeFromFormula(value));
			((FormulaValue)_value).Value = value;
		}
	}

	public ICellStyle CellStyle
	{
		get
		{
			if (_style == null)
			{
				return ((SXSSFWorkbook)Row.Sheet.Workbook).GetCellStyleAt(0);
			}
			return _style;
		}
		set
		{
			_style = value;
		}
	}

	public CellType CellType => _value.GetType();

	public int ColumnIndex => _row.GetCellIndex(this);

	public DateTime DateCellValue
	{
		get
		{
			if (_value.GetType() == CellType.Blank)
			{
				return default;
			}
			double numericCellValue = NumericCellValue;
			bool use1904windowing = Sheet.Workbook.IsDate1904();
			return DateUtil.GetJavaDate(numericCellValue, use1904windowing);
		}
	}

	public byte ErrorCellValue
	{
		get
		{
			CellType type = _value.GetType();
			switch (type)
			{
			case CellType.Blank:
				return 0;
			case CellType.Formula:
				if (((FormulaValue)_value).GetFormulaType() != CellType.Error)
				{
					throw typeMismatch(CellType.Error, CellType.Formula, isFormulaCell: false);
				}
				return ((ErrorFormulaValue)_value).PreEvaluatedValue;
			case CellType.Error:
				return ((ErrorValue)_value).Value;
			default:
				throw typeMismatch(CellType.Error, type, isFormulaCell: false);
			}
		}
	}

	public IHyperlink Hyperlink
	{
		get
		{
			return (IHyperlink)GetPropertyValue(2);
		}
		set
		{
			if (value == null)
			{
				RemoveHyperlink();
				return;
			}
			SetProperty(2, value);
			XSSFHyperlink xSSFHyperlink = (XSSFHyperlink)value;
			CellReference cellReference = new CellReference(RowIndex, ColumnIndex);
			xSSFHyperlink.GetCTHyperlink().@ref = cellReference.FormatAsString();
			((SXSSFSheet)Sheet)._sh.AddHyperlink(xSSFHyperlink);
		}
	}

	public bool IsMergedCell
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public bool IsPartOfArrayFormulaGroup => false;

	public double NumericCellValue
	{
		get
		{
			CellType type = _value.GetType();
			switch (type)
			{
			case CellType.Blank:
				return 0.0;
			case CellType.Formula:
				if (((FormulaValue)_value).GetFormulaType() != CellType.Numeric)
				{
					throw typeMismatch(CellType.Numeric, CellType.Formula, isFormulaCell: false);
				}
				return ((NumericFormulaValue)_value).PreEvaluatedValue;
			case CellType.Numeric:
				return ((NumericValue)_value).Value;
			default:
				throw typeMismatch(CellType.Numeric, type, isFormulaCell: false);
			}
		}
	}

	public IRichTextString RichStringCellValue
	{
		get
		{
			CellType type = _value.GetType();
			if (type != CellType.String)
			{
				throw typeMismatch(CellType.String, type, isFormulaCell: false);
			}
			if (((StringValue)_value).IsRichText())
			{
				return ((RichTextValue)_value).Value;
			}
			string stringCellValue = StringCellValue;
			return Sheet.Workbook.GetCreationHelper().CreateRichTextString(stringCellValue);
		}
	}

	public IRow Row => _row;

	public int RowIndex => _row.RowNum;

	public CellAddress Address => new CellAddress(this);

	public ISheet Sheet => _row.Sheet;

	public string StringCellValue
	{
		get
		{
			CellType type = _value.GetType();
			switch (type)
			{
			case CellType.Blank:
				return "";
			case CellType.Formula:
				if (((FormulaValue)_value).GetFormulaType() != CellType.String)
				{
					throw typeMismatch(CellType.String, CellType.Formula, isFormulaCell: false);
				}
				return ((StringFormulaValue)_value).PreEvaluatedValue;
			case CellType.String:
				if (((StringValue)_value).IsRichText())
				{
					return ((RichTextValue)_value).Value.String;
				}
				return ((PlainStringValue)_value).Value;
			default:
				throw typeMismatch(CellType.String, type, isFormulaCell: false);
			}
		}
	}

	public SXSSFCell(SXSSFRow row, CellType cellType)
	{
		_row = row;
		SetType(cellType);
	}

	public CellType GetCachedFormulaResultTypeEnum()
	{
		if (_value.GetType() != CellType.Formula)
		{
			throw new InvalidOperationException("Only formula cells have cached results");
		}
		return ((FormulaValue)_value).GetFormulaType();
	}

	public ICell CopyCellTo(int targetIndex)
	{
		throw new NotImplementedException();
	}

	public void RemoveCellComment()
	{
		RemoveProperty(1);
	}

	public void RemoveHyperlink()
	{
		RemoveProperty(2);
		((SXSSFSheet)Sheet)._sh.RemoveHyperlink(RowIndex, ColumnIndex);
	}

	public void SetAsActiveCell()
	{
		Sheet.ActiveCell = Address;
	}

	public void SetCellErrorValue(byte value)
	{
		EnsureType(CellType.Error);
		if (_value.GetType() == CellType.Formula)
		{
			((ErrorFormulaValue)_value).PreEvaluatedValue = value;
		}
		else
		{
			((ErrorValue)_value).Value = value;
		}
	}

	public void SetCellFormula(string formula)
	{
		if (formula == null)
		{
			SetType(CellType.Blank);
			return;
		}
		EnsureFormulaType(ComputeTypeFromFormula(formula));
		((FormulaValue)_value).Value = formula;
	}

	public void SetCellType(CellType cellType)
	{
		EnsureType(cellType);
	}

	public void SetCellValue(string value)
	{
		if (value != null)
		{
			EnsureTypeOrFormulaType(CellType.String);
			if (value.Length > SpreadsheetVersion.EXCEL2007.MaxTextLength)
			{
				throw new ArgumentException("The maximum length of cell contents (text) is 32,767 characters");
			}
			if (_value.GetType() == CellType.Formula)
			{
				if (_value is NumericFormulaValue)
				{
					((NumericFormulaValue)_value).PreEvaluatedValue = double.Parse(value);
				}
				else
				{
					((StringFormulaValue)_value).PreEvaluatedValue = value;
				}
			}
			else
			{
				((PlainStringValue)_value).Value = value;
			}
		}
		else
		{
			SetCellType(CellType.Blank);
		}
	}

	public void SetCellValue(bool value)
	{
		EnsureTypeOrFormulaType(CellType.Boolean);
		if (_value.GetType() == CellType.Formula)
		{
			((BooleanFormulaValue)_value).PreEvaluatedValue = value;
		}
		else
		{
			((BooleanValue)_value).Value = value;
		}
	}

	public void SetCellValue(IRichTextString value)
	{
		XSSFRichTextString xSSFRichTextString = (XSSFRichTextString)value;
		if (xSSFRichTextString != null && xSSFRichTextString.String != null)
		{
			EnsureRichTextStringType();
			if (xSSFRichTextString.Length > SpreadsheetVersion.EXCEL2007.MaxTextLength)
			{
				throw new InvalidOperationException("The maximum length of cell contents (text) is 32,767 characters");
			}
			if (xSSFRichTextString.HasFormatting())
			{
				logger.Log(5, "SXSSF doesn't support Shared Strings, rich text formatting information has be lost");
			}
			((RichTextValue)_value).Value = xSSFRichTextString;
		}
		else
		{
			SetCellType(CellType.Blank);
		}
	}

	public void SetCellValue(DateTime? value)
	{
		if (!value.HasValue)
		{
			SetCellType(CellType.Blank);
			return;
		}
		bool use1904windowing = ((SXSSFWorkbook)Sheet.Workbook).XssfWorkbook.IsDate1904();
		SetCellValue(DateUtil.GetExcelDate(value.Value, use1904windowing));
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
		EnsureTypeOrFormulaType(CellType.Numeric);
		if (_value.GetType() == CellType.Formula)
		{
			((NumericFormulaValue)_value).PreEvaluatedValue = value;
		}
		else
		{
			((NumericValue)_value).Value = value;
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
				return new SimpleDateFormat("dd-MMM-yyyy").Format(DateCellValue);
			}
			return NumericCellValue.ToString() ?? "";
		case CellType.String:
			return RichStringCellValue.ToString();
		default:
			return "Unknown Cell Type: " + CellType;
		}
	}

	private void RemoveProperty(int type)
	{
		Property property = _firstProperty;
		Property property2 = null;
		while (property != null && property.GetType() != type)
		{
			property2 = property;
			property = property._next;
		}
		if (property != null)
		{
			if (property2 != null)
			{
				property2._next = property._next;
			}
			else
			{
				_firstProperty = property._next;
			}
		}
	}

	private void SetProperty(int type, object value)
	{
		Property property = _firstProperty;
		Property property2 = null;
		while (property != null && property.GetType() != type)
		{
			property2 = property;
			property = property._next;
		}
		if (property != null)
		{
			property._value = value;
			return;
		}
		property = type switch
		{
			1 => new CommentProperty(value), 
			2 => new HyperlinkProperty(value), 
			_ => throw new ArgumentException("Invalid type: " + type), 
		};
		if (property2 != null)
		{
			property2._next = property;
		}
		else
		{
			_firstProperty = property;
		}
	}

	private object GetPropertyValue(int type)
	{
		return GetPropertyValue(type, null);
	}

	private object GetPropertyValue(int type, string defaultValue)
	{
		Property property = _firstProperty;
		while (property != null && property.GetType() != type)
		{
			property = property._next;
		}
		if (property != null)
		{
			return property._value;
		}
		return defaultValue;
	}

	private void EnsurePlainStringType()
	{
		if (_value.GetType() != CellType.String || ((StringValue)_value).IsRichText())
		{
			_value = new PlainStringValue();
		}
	}

	private void EnsureRichTextStringType()
	{
		if (_value.GetType() != CellType.String || !((StringValue)_value).IsRichText())
		{
			_value = new RichTextValue();
		}
	}

	private void EnsureType(CellType type)
	{
		if (_value.GetType() != type)
		{
			SetType(type);
		}
	}

	private void EnsureFormulaType(CellType type)
	{
		if (_value.GetType() != CellType.Formula || ((FormulaValue)_value).GetFormulaType() != type)
		{
			setFormulaType(type);
		}
	}

	private void EnsureTypeOrFormulaType(CellType type)
	{
		if (_value.GetType() == type)
		{
			if (type == CellType.String && ((StringValue)_value).IsRichText())
			{
				SetType(CellType.String);
			}
		}
		else if (_value.GetType() == CellType.Formula)
		{
			if (((FormulaValue)_value).GetFormulaType() != type)
			{
				setFormulaType(type);
			}
		}
		else
		{
			SetType(type);
		}
	}

	private void SetType(CellType type)
	{
		switch (type)
		{
		case CellType.Numeric:
			_value = new NumericValue();
			break;
		case CellType.String:
		{
			PlainStringValue plainStringValue = new PlainStringValue();
			if (_value != null)
			{
				string value2 = ConvertCellValueToString();
				plainStringValue.Value = value2;
			}
			_value = plainStringValue;
			break;
		}
		case CellType.Formula:
			_value = new NumericFormulaValue();
			break;
		case CellType.Blank:
			_value = new BlankValue();
			break;
		case CellType.Boolean:
		{
			BooleanValue booleanValue = new BooleanValue();
			if (_value != null)
			{
				bool value = convertCellValueToBoolean();
				booleanValue.Value = value;
			}
			_value = booleanValue;
			break;
		}
		case CellType.Error:
			_value = new ErrorValue();
			break;
		default:
			throw new ArgumentException("Illegal type " + type);
		}
	}

	private void setFormulaType(CellType type)
	{
		Value value = _value;
		switch (type)
		{
		case CellType.Numeric:
			_value = new NumericFormulaValue();
			break;
		case CellType.String:
			_value = new StringFormulaValue();
			break;
		case CellType.Boolean:
			_value = new BooleanFormulaValue();
			break;
		case CellType.Error:
			_value = new ErrorFormulaValue();
			break;
		default:
			throw new ArgumentException("Illegal type " + type);
		}
		if (value is FormulaValue)
		{
			((FormulaValue)_value).Value = ((FormulaValue)value).Value;
		}
	}

	private CellType ComputeTypeFromFormula(string formula)
	{
		return CellType.Numeric;
	}

	private static InvalidOperationException typeMismatch(CellType expectedTypeCode, CellType actualTypeCode, bool isFormulaCell)
	{
		return new InvalidOperationException("Cannot get a " + expectedTypeCode.ToString() + " value from a " + actualTypeCode.ToString() + " " + (isFormulaCell ? "formula " : "") + "cell");
	}

	private bool convertCellValueToBoolean()
	{
		CellType cellType = _value.GetType();
		if (cellType == CellType.Formula)
		{
			cellType = GetCachedFormulaResultTypeEnum();
		}
		switch (cellType)
		{
		case CellType.Boolean:
			return BooleanCellValue;
		case CellType.String:
			return bool.Parse(StringCellValue);
		case CellType.Numeric:
			return NumericCellValue != 0.0;
		case CellType.Blank:
		case CellType.Error:
			return false;
		default:
			throw new RuntimeException("Unexpected cell type (" + cellType.ToString() + ")");
		}
	}

	private string ConvertCellValueToString()
	{
		CellType type = _value.GetType();
		return ConvertCellValueToString(type);
	}

	private string ConvertCellValueToString(CellType cellType)
	{
		switch (cellType)
		{
		case CellType.Blank:
			return "";
		case CellType.Boolean:
			if (!BooleanCellValue)
			{
				return "FALSE";
			}
			return "TRUE";
		case CellType.String:
			return StringCellValue;
		case CellType.Numeric:
			return NumericCellValue.ToString();
		case CellType.Error:
			return FormulaError.ForInt(ErrorCellValue).String;
		case CellType.Formula:
			if (_value != null)
			{
				FormulaValue formulaValue = (FormulaValue)_value;
				if (formulaValue.GetFormulaType() != CellType.Formula)
				{
					return ConvertCellValueToString(formulaValue.GetFormulaType());
				}
			}
			return "";
		default:
			throw new InvalidOperationException("Unexpected cell type (" + cellType.ToString() + ")");
		}
	}

	public void SetCellValue(DateTime value)
	{
		SetCellValue((DateTime?)value);
	}

	public void SetBlank()
	{
		SetCellType(CellType.Blank);
	}
}

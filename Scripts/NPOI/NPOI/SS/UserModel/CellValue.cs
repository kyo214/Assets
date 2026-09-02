using System.Globalization;
using System.Text;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.UserModel;

public class CellValue
{
	public static readonly CellValue TRUE = new CellValue(CellType.Boolean, 0.0, boolValue: true, null, 0);

	public static readonly CellValue FALSE = new CellValue(CellType.Boolean, 0.0, boolValue: false, null, 0);

	private CellType _cellType;

	private double _numberValue;

	private bool _boolValue;

	private string _textValue;

	private int _errorCode;

	public bool BooleanValue => _boolValue;

	public double NumberValue => _numberValue;

	public string StringValue => _textValue;

	public CellType CellType => _cellType;

	public sbyte ErrorValue => (sbyte)_errorCode;

	private CellValue(CellType cellType, double numberValue, bool boolValue, string textValue, int errorCode)
	{
		_cellType = cellType;
		_numberValue = numberValue;
		_boolValue = boolValue;
		_textValue = textValue;
		_errorCode = errorCode;
	}

	public CellValue(double numberValue)
		: this(CellType.Numeric, numberValue, boolValue: false, null, 0)
	{
	}

	public static CellValue ValueOf(bool boolValue)
	{
		if (!boolValue)
		{
			return FALSE;
		}
		return TRUE;
	}

	public CellValue(string stringValue)
		: this(CellType.String, 0.0, boolValue: false, stringValue, 0)
	{
	}

	public static CellValue GetError(int errorCode)
	{
		return new CellValue(CellType.Error, 0.0, boolValue: false, null, errorCode);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(64);
		stringBuilder.Append(GetType().Name).Append(" [");
		stringBuilder.Append(FormatAsString());
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}

	public string FormatAsString()
	{
		switch (_cellType)
		{
		case CellType.Numeric:
			return _numberValue.ToString(CultureInfo.InvariantCulture);
		case CellType.String:
			return "\"" + _textValue + "\"";
		case CellType.Boolean:
			if (!_boolValue)
			{
				return "FALSE";
			}
			return "TRUE";
		case CellType.Error:
			return ErrorEval.GetText(_errorCode);
		default:
			return "<error unexpected cell type " + _cellType.ToString() + ">";
		}
	}
}

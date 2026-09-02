using NPOI.SS.Util;

namespace NPOI.SS;

public class SpreadsheetVersion
{
	public static SpreadsheetVersion EXCEL97 = new SpreadsheetVersion("xls", 65536, 256, 30, 3, 4000, 32767, "EXCEL97");

	public static SpreadsheetVersion EXCEL2007 = new SpreadsheetVersion("xlsx", 1048576, 16384, 255, int.MaxValue, 64000, 32767, "EXCEL2007");

	private string _defaultExtension;

	private int _maxRows;

	private int _maxColumns;

	private int _maxFunctionArgs;

	private int _maxCondFormats;

	private int _maxCellStyles;

	private int _maxTextLength;

	private string _name;

	public string Name => _name;

	public string DefaultExtension => _defaultExtension;

	public int MaxRows => _maxRows;

	public int LastRowIndex => _maxRows - 1;

	public int MaxColumns => _maxColumns;

	public int LastColumnIndex => _maxColumns - 1;

	public int MaxFunctionArgs => _maxFunctionArgs;

	public int MaxConditionalFormats => _maxCondFormats;

	public string LastColumnName => CellReference.ConvertNumToColString(LastColumnIndex);

	public int MaxCellStyles => _maxCellStyles;

	public int MaxTextLength => _maxTextLength;

	private SpreadsheetVersion(string defaultExtension, int maxRows, int maxColumns, int maxFunctionArgs, int maxCondFormats, int maxCellStyles, int maxText, string name)
	{
		_defaultExtension = defaultExtension;
		_maxRows = maxRows;
		_maxColumns = maxColumns;
		_maxFunctionArgs = maxFunctionArgs;
		_maxCondFormats = maxCondFormats;
		_maxCellStyles = maxCellStyles;
		_maxTextLength = maxText;
		_name = name;
	}
}

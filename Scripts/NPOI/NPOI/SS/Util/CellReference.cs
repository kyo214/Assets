using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using NPOI.SS.Formula;
using NPOI.SS.UserModel;

namespace NPOI.SS.Util;

public class CellReference
{
	internal class CellRefPartsInner
	{
		public string sheetName;

		public string rowRef;

		public string colRef;

		public CellRefPartsInner(string sheetName, string rowRef, string colRef)
		{
			this.sheetName = sheetName;
			this.rowRef = rowRef ?? "";
			this.colRef = colRef ?? "";
		}
	}

	private const char ABSOLUTE_REFERENCE_MARKER = '$';

	private const char SHEET_NAME_DELIMITER = '!';

	private const char SPECIAL_NAME_DELIMITER = '\'';

	private static Regex CELL_REF_PATTERN = new Regex("(\\$?[A-Z]+)?(\\$?[0-9]+)?", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

	private static Regex STRICTLY_CELL_REF_PATTERN = new Regex("^\\$?([A-Z]+)\\$?([0-9]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

	private static Regex COLUMN_REF_PATTERN = new Regex("^\\$?([A-Za-z]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

	private static Regex ROW_REF_PATTERN = new Regex("^\\$?([0-9]+)$");

	private static Regex NAMED_RANGE_NAME_PATTERN = new Regex("^[_A-Za-z][_.A-Za-z0-9]*$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

	private string _sheetName;

	private int _rowIndex;

	private int _colIndex;

	private bool _isRowAbs;

	private bool _isColAbs;

	public int Row => _rowIndex;

	public short Col => (short)_colIndex;

	public bool IsRowAbsolute => _isRowAbs;

	public bool IsColAbsolute => _isColAbs;

	public string SheetName => _sheetName;

	public string[] CellRefParts => new string[3]
	{
		_sheetName,
		(_rowIndex + 1).ToString(CultureInfo.InvariantCulture),
		ConvertNumToColString(_colIndex)
	};

	public CellReference(string cellRef)
	{
		if (cellRef.EndsWith("#REF!", StringComparison.InvariantCulture))
		{
			throw new ArgumentException("Cell reference invalid: " + cellRef);
		}
		CellRefPartsInner cellRefPartsInner = SeparateRefParts(cellRef);
		_sheetName = cellRefPartsInner.sheetName;
		string text = cellRefPartsInner.colRef;
		_isColAbs = text.Length > 0 && text[0] == '$';
		if (_isColAbs)
		{
			text = text.Substring(1);
		}
		if (text.Length == 0)
		{
			_colIndex = -1;
		}
		else
		{
			_colIndex = ConvertColStringToIndex(text);
		}
		string text2 = cellRefPartsInner.rowRef;
		_isRowAbs = text2.Length > 0 && text2[0] == '$';
		if (_isRowAbs)
		{
			text2 = text2.Substring(1);
		}
		if (text2.Length == 0)
		{
			_rowIndex = -1;
		}
		else
		{
			_rowIndex = int.Parse(text2, CultureInfo.InvariantCulture) - 1;
		}
	}

	public CellReference(ICell cell)
		: this(cell.RowIndex, cell.ColumnIndex, pAbsRow: false, pAbsCol: false)
	{
	}

	public CellReference(int pRow, int pCol)
		: this(pRow, pCol, pAbsRow: false, pAbsCol: false)
	{
	}

	public CellReference(int pRow, short pCol)
		: this(pRow, pCol & 0xFFFF, pAbsRow: false, pAbsCol: false)
	{
	}

	public CellReference(int pRow, int pCol, bool pAbsRow, bool pAbsCol)
		: this(null, pRow, pCol, pAbsRow, pAbsCol)
	{
	}

	public CellReference(string pSheetName, int pRow, int pCol, bool pAbsRow, bool pAbsCol)
	{
		if (pRow < -1)
		{
			throw new ArgumentException("row index may not be negative, but had " + pRow);
		}
		if (pCol < -1)
		{
			throw new ArgumentException("column index may not be negative, but had " + pCol);
		}
		_sheetName = pSheetName;
		_rowIndex = pRow;
		_colIndex = pCol;
		_isRowAbs = pAbsRow;
		_isColAbs = pAbsCol;
	}

	public static int ConvertColStringToIndex(string ref1)
	{
		int num = 0;
		char[] array = ref1.ToUpper().ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			char c = array[i];
			if (c == '$')
			{
				if (i != 0)
				{
					throw new ArgumentException("Bad col ref format '" + ref1 + "'");
				}
			}
			else
			{
				num = num * 26 + (c - 65 + 1);
			}
		}
		return num - 1;
	}

	public static bool IsPartAbsolute(string part)
	{
		return part[0] == '$';
	}

	public static NameType ClassifyCellReference(string str, SpreadsheetVersion ssVersion)
	{
		int length = str.Length;
		if (length < 1)
		{
			throw new ArgumentException("Empty string not allowed");
		}
		char c = str[0];
		if (c != '$' && c != '.' && c != '_' && !char.IsLetter(c) && !char.IsDigit(c))
		{
			throw new ArgumentException("Invalid first char (" + c + ") of cell reference or named range.  Letter expected");
		}
		if (!char.IsDigit(str[length - 1]))
		{
			return ValidateNamedRangeName(str, ssVersion);
		}
		Regex sTRICTLY_CELL_REF_PATTERN = STRICTLY_CELL_REF_PATTERN;
		if (!sTRICTLY_CELL_REF_PATTERN.IsMatch(str))
		{
			return ValidateNamedRangeName(str, ssVersion);
		}
		MatchCollection matchCollection = sTRICTLY_CELL_REF_PATTERN.Matches(str);
		string value = matchCollection[0].Groups[1].Value;
		string value2 = matchCollection[0].Groups[2].Value;
		if (CellReferenceIsWithinRange(value, value2, ssVersion))
		{
			return NameType.Cell;
		}
		if (str.IndexOf('$') >= 0)
		{
			return NameType.BadCellOrNamedRange;
		}
		return NameType.NamedRange;
	}

	private static NameType ValidateNamedRangeName(string str, SpreadsheetVersion ssVersion)
	{
		Regex cOLUMN_REF_PATTERN = COLUMN_REF_PATTERN;
		if (cOLUMN_REF_PATTERN.IsMatch(str) && IsColumnWithinRange(cOLUMN_REF_PATTERN.Matches(str)[0].Groups[1].Value, ssVersion))
		{
			return NameType.Column;
		}
		Regex rOW_REF_PATTERN = ROW_REF_PATTERN;
		if (rOW_REF_PATTERN.IsMatch(str) && IsRowWithinRange(rOW_REF_PATTERN.Matches(str)[0].Groups[1].Value, ssVersion))
		{
			return NameType.Row;
		}
		if (!NAMED_RANGE_NAME_PATTERN.IsMatch(str))
		{
			return NameType.BadCellOrNamedRange;
		}
		return NameType.NamedRange;
	}

	public static string ConvertNumToColString(int col)
	{
		int num = col + 1;
		StringBuilder stringBuilder = new StringBuilder(2);
		int num2 = num;
		while (num2 > 0)
		{
			int num3 = num2 % 26;
			if (num3 == 0)
			{
				num3 = 26;
			}
			num2 = (num2 - num3) / 26;
			char value = (char)(num3 + 64);
			stringBuilder.Insert(0, value);
		}
		return stringBuilder.ToString();
	}

	private static CellRefPartsInner SeparateRefParts(string reference)
	{
		int num = reference.LastIndexOf('!');
		string sheetName = ParseSheetName(reference, num);
		string input = reference.Substring(num + 1).ToUpper(CultureInfo.InvariantCulture);
		Match match = CELL_REF_PATTERN.Match(input);
		if (!match.Success)
		{
			throw new ArgumentException("Invalid CellReference: " + reference);
		}
		string value = match.Groups[1].Value;
		string value2 = match.Groups[2].Value;
		return new CellRefPartsInner(sheetName, value2, value);
	}

	private static string ParseSheetName(string reference, int indexOfSheetNameDelimiter)
	{
		if (indexOfSheetNameDelimiter < 0)
		{
			return null;
		}
		if (reference[0] != '\'')
		{
			if (reference.IndexOf(' ') == -1)
			{
				return reference.Substring(0, indexOfSheetNameDelimiter);
			}
			throw new ArgumentException("Sheet names containing spaces must be quoted: (" + reference + ")");
		}
		int num = indexOfSheetNameDelimiter - 1;
		if (reference[num] != '\'')
		{
			throw new ArgumentException("Mismatched quotes: (" + reference + ")");
		}
		StringBuilder stringBuilder = new StringBuilder(indexOfSheetNameDelimiter);
		for (int i = 1; i < num; i++)
		{
			char c = reference[i];
			if (c != '\'')
			{
				stringBuilder.Append(c);
				continue;
			}
			if (i < num && reference[i + 1] == '\'')
			{
				i++;
				stringBuilder.Append(c);
				continue;
			}
			throw new ArgumentException("Bad sheet name quote escaping: (" + reference + ")");
		}
		return stringBuilder.ToString();
	}

	public string FormatAsString()
	{
		StringBuilder stringBuilder = new StringBuilder(32);
		if (_sheetName != null)
		{
			SheetNameFormatter.AppendFormat(stringBuilder, _sheetName);
			stringBuilder.Append('!');
		}
		AppendCellReference(stringBuilder);
		return stringBuilder.ToString();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(64);
		stringBuilder.Append(GetType().Name).Append(" [");
		stringBuilder.Append(FormatAsString());
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}

	public void AppendCellReference(StringBuilder sb)
	{
		if (_colIndex != -1)
		{
			if (_isColAbs)
			{
				sb.Append('$');
			}
			sb.Append(ConvertNumToColString(_colIndex));
		}
		if (_rowIndex != -1)
		{
			if (_isRowAbs)
			{
				sb.Append('$');
			}
			sb.Append(_rowIndex + 1);
		}
	}

	public static bool CellReferenceIsWithinRange(string colStr, string rowStr, SpreadsheetVersion ssVersion)
	{
		if (!IsColumnWithinRange(colStr, ssVersion))
		{
			return false;
		}
		return IsRowWithinRange(rowStr, ssVersion);
	}

	[Obsolete("deprecated 3.15 beta 2. Use {@link #isColumnWithinRange}.")]
	public static bool IsColumnWithnRange(string colStr, SpreadsheetVersion ssVersion)
	{
		return IsColumnWithinRange(colStr, ssVersion);
	}

	public static bool IsRowWithinRange(string rowStr, SpreadsheetVersion ssVersion)
	{
		int num = int.Parse(rowStr) - 1;
		if (0 <= num)
		{
			return num <= ssVersion.LastRowIndex;
		}
		return false;
	}

	[Obsolete("deprecated 3.15 beta 2. Use {@link #isRowWithinRange}")]
	public static bool isRowWithnRange(string rowStr, SpreadsheetVersion ssVersion)
	{
		return IsRowWithinRange(rowStr, ssVersion);
	}

	public static bool IsColumnWithinRange(string colStr, SpreadsheetVersion ssVersion)
	{
		string lastColumnName = ssVersion.LastColumnName;
		int length = lastColumnName.Length;
		int length2 = colStr.Length;
		if (length2 > length)
		{
			return false;
		}
		if (length2 == length && string.Compare(colStr.ToUpper(), lastColumnName, StringComparison.Ordinal) > 0)
		{
			return false;
		}
		return true;
	}

	public override bool Equals(object o)
	{
		if (this == o)
		{
			return true;
		}
		if (!(o is CellReference))
		{
			return false;
		}
		CellReference cellReference = (CellReference)o;
		if (_rowIndex == cellReference._rowIndex && _colIndex == cellReference._colIndex && _isRowAbs == cellReference._isRowAbs && _isColAbs == cellReference._isColAbs)
		{
			if (_sheetName != null)
			{
				return _sheetName.Equals(cellReference._sheetName);
			}
			return cellReference._sheetName == null;
		}
		return false;
	}

	public override int GetHashCode()
	{
		int num = 17;
		num = 31 * num + _rowIndex;
		num = 31 * num + _colIndex;
		num = 31 * num + (_isRowAbs ? 1 : 0);
		num = 31 * num + (_isColAbs ? 1 : 0);
		return 31 * num + ((_sheetName != null) ? _sheetName.GetHashCode() : 0);
	}
}

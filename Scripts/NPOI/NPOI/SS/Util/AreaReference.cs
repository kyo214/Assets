using System;
using System.Collections;
using System.Text;

namespace NPOI.SS.Util;

public class AreaReference
{
	private const char SHEET_NAME_DELIMITER = '!';

	private const char CELL_DELIMITER = ':';

	private const char SPECIAL_NAME_DELIMITER = '\'';

	private static SpreadsheetVersion DEFAULT_SPREADSHEET_VERSION = SpreadsheetVersion.EXCEL97;

	private CellReference _firstCell;

	private CellReference _lastCell;

	private bool _isSingleCell;

	private SpreadsheetVersion _version;

	public bool IsSingleCell => _isSingleCell;

	public CellReference FirstCell => _firstCell;

	public CellReference LastCell => _lastCell;

	[Obsolete("deprecated since NPOI 2.5.1 Prefer supplying a spreadsheet version.")]
	public AreaReference(string reference)
		: this(reference, DEFAULT_SPREADSHEET_VERSION)
	{
	}

	public AreaReference(string reference, SpreadsheetVersion version)
	{
		_version = ((version != null) ? version : DEFAULT_SPREADSHEET_VERSION);
		if (!IsContiguous(reference))
		{
			throw new ArgumentException("References passed to the AreaReference must be contiguous, use generateContiguous(ref) if you have non-contiguous references");
		}
		string[] array = SeparateAreaRefs(reference);
		string text = array[0];
		if (array.Length == 1)
		{
			_firstCell = new CellReference(text);
			_lastCell = _firstCell;
			_isSingleCell = true;
			return;
		}
		if (array.Length != 2)
		{
			throw new ArgumentException("Bad area ref '" + reference + "'");
		}
		string text2 = array[1];
		if (IsPlainColumn(text))
		{
			if (!IsPlainColumn(text2))
			{
				throw new Exception("Bad area ref '" + reference + "'");
			}
			bool pAbsCol = CellReference.IsPartAbsolute(text);
			bool pAbsCol2 = CellReference.IsPartAbsolute(text2);
			int pCol = CellReference.ConvertColStringToIndex(text);
			int pCol2 = CellReference.ConvertColStringToIndex(text2);
			_firstCell = new CellReference(0, pCol, pAbsRow: true, pAbsCol);
			_lastCell = new CellReference(65535, pCol2, pAbsRow: true, pAbsCol2);
			_isSingleCell = false;
		}
		else
		{
			_firstCell = new CellReference(text);
			_lastCell = new CellReference(text2);
			_isSingleCell = text.Equals(text2);
		}
	}

	private static bool IsPlainColumn(string refPart)
	{
		for (int num = refPart.Length - 1; num >= 0; num--)
		{
			int num2 = refPart[num];
			if ((num2 != 36 || num != 0) && (num2 < 65 || num2 > 90))
			{
				return false;
			}
		}
		return true;
	}

	public static AreaReference GetWholeRow(SpreadsheetVersion version, string start, string end)
	{
		if (version == null)
		{
			version = DEFAULT_SPREADSHEET_VERSION;
		}
		return new AreaReference("$A" + start + ":$" + version.LastColumnName + end, version);
	}

	public static AreaReference GetWholeColumn(SpreadsheetVersion version, string start, string end)
	{
		if (version == null)
		{
			version = DEFAULT_SPREADSHEET_VERSION;
		}
		return new AreaReference(start + "$1:" + end + "$" + version.MaxRows, version);
	}

	public AreaReference(CellReference topLeft, CellReference botRight)
	{
		_version = DEFAULT_SPREADSHEET_VERSION;
		bool flag = topLeft.Row > botRight.Row;
		bool flag2 = topLeft.Col > botRight.Col;
		if (flag | flag2)
		{
			int row;
			bool isRowAbsolute;
			int row2;
			bool isRowAbsolute2;
			if (flag)
			{
				row = botRight.Row;
				isRowAbsolute = botRight.IsRowAbsolute;
				row2 = topLeft.Row;
				isRowAbsolute2 = topLeft.IsRowAbsolute;
			}
			else
			{
				row = topLeft.Row;
				isRowAbsolute = topLeft.IsRowAbsolute;
				row2 = botRight.Row;
				isRowAbsolute2 = botRight.IsRowAbsolute;
			}
			int col;
			bool isColAbsolute;
			int col2;
			bool isColAbsolute2;
			if (flag2)
			{
				col = botRight.Col;
				isColAbsolute = botRight.IsColAbsolute;
				col2 = topLeft.Col;
				isColAbsolute2 = topLeft.IsColAbsolute;
			}
			else
			{
				col = topLeft.Col;
				isColAbsolute = topLeft.IsColAbsolute;
				col2 = botRight.Col;
				isColAbsolute2 = botRight.IsColAbsolute;
			}
			_firstCell = new CellReference(row, col, isRowAbsolute, isColAbsolute);
			_lastCell = new CellReference(row2, col2, isRowAbsolute2, isColAbsolute2);
		}
		else
		{
			_firstCell = topLeft;
			_lastCell = botRight;
		}
		_isSingleCell = false;
	}

	public static bool IsContiguous(string reference)
	{
		int num = reference.IndexOf('!');
		if (num != -1)
		{
			reference = reference.Substring(num);
		}
		if (reference.IndexOf(',') == -1)
		{
			return true;
		}
		return false;
	}

	public static bool IsWholeColumnReference(SpreadsheetVersion version, CellReference topLeft, CellReference botRight)
	{
		if (version == null)
		{
			version = SpreadsheetVersion.EXCEL97;
		}
		if (topLeft.Row == 0 && topLeft.IsRowAbsolute && botRight.Row == version.LastRowIndex && botRight.IsRowAbsolute)
		{
			return true;
		}
		return false;
	}

	public bool IsWholeColumnReference()
	{
		return IsWholeColumnReference(_version, _firstCell, _lastCell);
	}

	public static AreaReference[] GenerateContiguous(string reference)
	{
		ArrayList arrayList = new ArrayList();
		string[] array = reference.Split(new char[1] { ',' });
		foreach (string reference2 in array)
		{
			arrayList.Add(new AreaReference(reference2));
		}
		return (AreaReference[])arrayList.ToArray(typeof(AreaReference));
	}

	public CellReference[] GetAllReferencedCells()
	{
		if (_isSingleCell)
		{
			return new CellReference[1] { _firstCell };
		}
		int num = Math.Min(_firstCell.Row, _lastCell.Row);
		int num2 = Math.Max(_firstCell.Row, _lastCell.Row);
		int num3 = Math.Min(_firstCell.Col, _lastCell.Col);
		int num4 = Math.Max(_firstCell.Col, _lastCell.Col);
		string sheetName = _firstCell.SheetName;
		ArrayList arrayList = new ArrayList();
		for (int i = num; i <= num2; i++)
		{
			for (int j = num3; j <= num4; j++)
			{
				CellReference value = new CellReference(sheetName, i, j, _firstCell.IsRowAbsolute, _firstCell.IsColAbsolute);
				arrayList.Add(value);
			}
		}
		return (CellReference[])arrayList.ToArray(typeof(CellReference));
	}

	public string FormatAsString()
	{
		if (IsWholeColumnReference())
		{
			return CellReference.ConvertNumToColString(_firstCell.Col) + ":" + CellReference.ConvertNumToColString(_lastCell.Col);
		}
		StringBuilder stringBuilder = new StringBuilder(32);
		stringBuilder.Append(_firstCell.FormatAsString());
		if (!_isSingleCell)
		{
			stringBuilder.Append(':');
			if (_lastCell.SheetName == null)
			{
				stringBuilder.Append(_lastCell.FormatAsString());
			}
			else
			{
				_lastCell.AppendCellReference(stringBuilder);
			}
		}
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

	private static string[] SeparateAreaRefs(string reference)
	{
		int length = reference.Length;
		int num = -1;
		bool flag = false;
		for (int i = 0; i < length; i++)
		{
			switch (reference[i])
			{
			case ':':
				if (!flag)
				{
					if (num >= 0)
					{
						throw new ArgumentException("More than one cell delimiter ':' appears in area reference '" + reference + "'");
					}
					num = i;
				}
				break;
			case '\'':
				if (!flag)
				{
					flag = true;
					break;
				}
				if (i >= length - 1)
				{
					throw new ArgumentException("Area reference '" + reference + "' ends with special name delimiter '''");
				}
				if (reference[i + 1] == '\'')
				{
					i++;
				}
				else
				{
					flag = false;
				}
				break;
			}
		}
		if (num >= 0)
		{
			string text = reference.Substring(0, num);
			string text2 = reference.Substring(num + 1);
			if (text2.IndexOf('!') >= 0)
			{
				throw new Exception("Unexpected ! in second cell reference of '" + reference + "'");
			}
			int num2 = text.LastIndexOf('!');
			if (num2 >= 0)
			{
				string text3 = text.Substring(0, num2 + 1);
				return new string[2]
				{
					text,
					text3 + text2
				};
			}
			return new string[2] { text, text2 };
		}
		return new string[1] { reference };
	}
}

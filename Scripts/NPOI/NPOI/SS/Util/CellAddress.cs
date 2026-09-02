using System;
using NPOI.SS.UserModel;

namespace NPOI.SS.Util;

public class CellAddress : IComparable<CellAddress>
{
	public static CellAddress A1 = new CellAddress(0, 0);

	private int _row;

	private int _col;

	public int Row => _row;

	public int Column => _col;

	public CellAddress(int row, int column)
	{
		_row = row;
		_col = column;
	}

	public CellAddress(string address)
	{
		int length = address.Length;
		int i;
		for (i = 0; i < length && !char.IsDigit(address[i]); i++)
		{
		}
		string @ref = address.Substring(0, i).ToUpper();
		string s = address.Substring(i);
		_row = int.Parse(s) - 1;
		_col = CellReference.ConvertColStringToIndex(@ref);
	}

	public CellAddress(CellReference reference)
		: this(reference.Row, reference.Col)
	{
	}

	public CellAddress(ICell cell)
		: this(cell.RowIndex, cell.ColumnIndex)
	{
	}

	public int CompareTo(CellAddress other)
	{
		int num = _row - other._row;
		if (num != 0)
		{
			return num;
		}
		num = _col - other._col;
		if (num != 0)
		{
			return num;
		}
		return 0;
	}

	public override bool Equals(object o)
	{
		if (this == o)
		{
			return true;
		}
		if (!(o is CellAddress))
		{
			return false;
		}
		CellAddress cellAddress = (CellAddress)o;
		if (_row == cellAddress._row)
		{
			return _col == cellAddress._col;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return _row + _col << 16;
	}

	public override string ToString()
	{
		return FormatAsString();
	}

	public string FormatAsString()
	{
		return CellReference.ConvertNumToColString(_col) + (_row + 1);
	}
}

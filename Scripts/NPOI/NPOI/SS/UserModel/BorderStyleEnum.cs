using System;

namespace NPOI.SS.UserModel;

public class BorderStyleEnum
{
	private static BorderStyle[] _table;

	public static BorderStyle[] Values()
	{
		return _table;
	}

	static BorderStyleEnum()
	{
		_table = new BorderStyle[14];
		foreach (BorderStyle value in Enum.GetValues(typeof(BorderStyle)))
		{
			_table[(int)value] = value;
		}
	}

	public static BorderStyle ValueOf(short code)
	{
		return _table[code];
	}
}

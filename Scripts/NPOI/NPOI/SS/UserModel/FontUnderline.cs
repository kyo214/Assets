namespace NPOI.SS.UserModel;

public class FontUnderline
{
	public static readonly FontUnderline SINGLE;

	public static readonly FontUnderline DOUBLE;

	public static readonly FontUnderline SINGLE_ACCOUNTING;

	public static readonly FontUnderline DOUBLE_ACCOUNTING;

	public static readonly FontUnderline NONE;

	private int value;

	private static FontUnderline[] _table;

	public int Value => value;

	public byte ByteValue
	{
		get
		{
			if (this == DOUBLE)
			{
				return 2;
			}
			if (this == DOUBLE_ACCOUNTING)
			{
				return 34;
			}
			if (this == SINGLE_ACCOUNTING)
			{
				return 33;
			}
			if (this == NONE)
			{
				return 0;
			}
			_ = SINGLE;
			return 1;
		}
	}

	private FontUnderline(int val)
	{
		value = val;
	}

	static FontUnderline()
	{
		SINGLE = new FontUnderline(1);
		DOUBLE = new FontUnderline(2);
		SINGLE_ACCOUNTING = new FontUnderline(3);
		DOUBLE_ACCOUNTING = new FontUnderline(4);
		NONE = new FontUnderline(0);
		_table = null;
		if (_table == null)
		{
			_table = new FontUnderline[5];
			_table[0] = NONE;
			_table[1] = SINGLE;
			_table[2] = DOUBLE;
			_table[3] = SINGLE_ACCOUNTING;
			_table[4] = DOUBLE_ACCOUNTING;
		}
	}

	public static FontUnderline ValueOf(int value)
	{
		return _table[value];
	}

	public static FontUnderline ValueOf(FontUnderlineType value)
	{
		return value switch
		{
			FontUnderlineType.Double => DOUBLE, 
			FontUnderlineType.DoubleAccounting => DOUBLE_ACCOUNTING, 
			FontUnderlineType.SingleAccounting => SINGLE_ACCOUNTING, 
			FontUnderlineType.Single => SINGLE, 
			_ => NONE, 
		};
	}
}

namespace NPOI.SS.UserModel;

public class PrintCellComments
{
	public static PrintCellComments NONE;

	public static PrintCellComments AS_DISPLAYED;

	public static PrintCellComments AT_END;

	private int comments;

	private static PrintCellComments[] _table;

	public int Value => comments;

	static PrintCellComments()
	{
		_table = new PrintCellComments[4];
		NONE = new PrintCellComments(1);
		AS_DISPLAYED = new PrintCellComments(2);
		AT_END = new PrintCellComments(3);
	}

	private PrintCellComments(int comments)
	{
		this.comments = comments;
		_table[Value] = this;
	}

	public static PrintCellComments ValueOf(int value)
	{
		return _table[value];
	}
}

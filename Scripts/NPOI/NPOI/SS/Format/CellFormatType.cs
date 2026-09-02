namespace NPOI.SS.Format;

public abstract class CellFormatType
{
	public static readonly CellFormatType GENERAL = new GeneralCellFormatType();

	public static readonly CellFormatType NUMBER = new NumberCellFormatType();

	public static readonly CellFormatType DATE = new DateCellFormatType();

	public static readonly CellFormatType ELAPSED = new ElapsedCellFormatType();

	public static readonly CellFormatType TEXT = new TextCellFormatType();

	public abstract bool IsSpecial(char ch);

	public abstract CellFormatter Formatter(string pattern);
}

namespace NPOI.SS.Util;

public class PaneInformation
{
	public const byte PANE_LOWER_RIGHT = 0;

	public const byte PANE_UPPER_RIGHT = 1;

	public const byte PANE_LOWER_LEFT = 2;

	public const byte PANE_UPPER_LEFT = 3;

	private short x;

	private short y;

	private short topRow;

	private short leftColumn;

	private byte activePane;

	private bool frozen;

	public short VerticalSplitPosition => x;

	public short HorizontalSplitPosition => y;

	public short HorizontalSplitTopRow => topRow;

	public short VerticalSplitLeftColumn => leftColumn;

	public byte ActivePane => activePane;

	public PaneInformation(short x, short y, short top, short left, byte active, bool frozen)
	{
		this.x = x;
		this.y = y;
		topRow = top;
		leftColumn = left;
		activePane = active;
		this.frozen = frozen;
	}

	public bool IsFreezePane()
	{
		return frozen;
	}
}

namespace NPOI.HSSF.EventUserModel.DummyRecord;

public class MissingCellDummyRecord : DummyRecordBase
{
	private int row;

	private int column;

	public int Row => row;

	public int Column => column;

	public MissingCellDummyRecord(int row, int column)
	{
		this.row = row;
		this.column = column;
	}
}

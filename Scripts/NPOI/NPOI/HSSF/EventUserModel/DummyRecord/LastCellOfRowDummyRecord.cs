namespace NPOI.HSSF.EventUserModel.DummyRecord;

public class LastCellOfRowDummyRecord : DummyRecordBase
{
	private int row;

	private int lastColumnNumber;

	public int Row => row;

	public int LastColumnNumber => lastColumnNumber;

	public LastCellOfRowDummyRecord(int row, int lastColumnNumber)
	{
		this.row = row;
		this.lastColumnNumber = lastColumnNumber;
	}
}

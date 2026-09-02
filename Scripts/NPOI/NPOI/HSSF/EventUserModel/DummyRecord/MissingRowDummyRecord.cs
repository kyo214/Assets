namespace NPOI.HSSF.EventUserModel.DummyRecord;

public class MissingRowDummyRecord : DummyRecordBase
{
	private int rowNumber;

	public int RowNumber => rowNumber;

	public MissingRowDummyRecord(int rowNumber)
	{
		this.rowNumber = rowNumber;
	}
}

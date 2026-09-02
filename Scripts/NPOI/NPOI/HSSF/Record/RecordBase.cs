namespace NPOI.HSSF.Record;

public abstract class RecordBase
{
	public abstract int RecordSize { get; }

	public abstract int Serialize(int offset, byte[] data);
}

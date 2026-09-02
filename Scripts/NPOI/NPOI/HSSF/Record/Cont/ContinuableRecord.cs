using NPOI.Util;

namespace NPOI.HSSF.Record.Cont;

public abstract class ContinuableRecord : Record
{
	public override int RecordSize
	{
		get
		{
			ContinuableRecordOutput continuableRecordOutput = ContinuableRecordOutput.CreateForCountingOnly();
			Serialize(continuableRecordOutput);
			continuableRecordOutput.Terminate();
			return continuableRecordOutput.TotalSize;
		}
	}

	protected abstract void Serialize(ContinuableRecordOutput out1);

	public override int Serialize(int offset, byte[] data)
	{
		ContinuableRecordOutput continuableRecordOutput = new ContinuableRecordOutput(new LittleEndianByteArrayOutputStream(data, offset), Sid);
		Serialize(continuableRecordOutput);
		continuableRecordOutput.Terminate();
		return continuableRecordOutput.TotalSize;
	}
}

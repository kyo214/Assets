using NPOI.Util;

namespace NPOI.HSSF.Record;

public class InterfaceEndRecord : StandardRecord
{
	public const short sid = 226;

	public static InterfaceEndRecord Instance = new InterfaceEndRecord();

	protected override int DataSize => 0;

	public override short Sid => 226;

	private InterfaceEndRecord()
	{
	}

	public static Record Create(RecordInputStream in1)
	{
		return in1.Remaining switch
		{
			0 => Instance, 
			2 => new InterfaceHdrRecord(in1), 
			_ => throw new RecordFormatException("Invalid record data size: " + in1.Remaining), 
		};
	}

	public override string ToString()
	{
		return "[INTERFACEEND/]\n";
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
	}

	public int GetDataSize()
	{
		return DataSize;
	}
}

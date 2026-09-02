using System.IO;

namespace NPOI.HSSF.Record;

public class DrawingRecordForBiffViewer : AbstractEscherHolderRecord
{
	public const short sid = 236;

	protected override string RecordName => "MSODRAWING";

	public override short Sid => 236;

	public DrawingRecordForBiffViewer()
	{
	}

	public DrawingRecordForBiffViewer(RecordInputStream in1)
		: base(in1)
	{
	}

	public DrawingRecordForBiffViewer(DrawingRecord r)
		: base(ConvertToInputStream(r))
	{
		ConvertRawBytesToEscherRecords();
	}

	private static RecordInputStream ConvertToInputStream(DrawingRecord r)
	{
		using MemoryStream @in = new MemoryStream(r.Serialize());
		RecordInputStream recordInputStream = new RecordInputStream(@in);
		recordInputStream.NextRecord();
		return recordInputStream;
	}
}

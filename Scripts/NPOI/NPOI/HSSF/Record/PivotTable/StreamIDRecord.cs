using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.PivotTable;

public class StreamIDRecord : StandardRecord
{
	public const short sid = 213;

	private int idstm;

	protected override int DataSize => 2;

	public override short Sid => 213;

	public StreamIDRecord(RecordInputStream in1)
	{
		idstm = in1.ReadShort();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(idstm);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[SXIDSTM]\n");
		stringBuilder.Append("    .idstm      =").Append(HexDump.ShortToHex(idstm)).Append('\n');
		stringBuilder.Append("[/SXIDSTM]\n");
		return stringBuilder.ToString();
	}
}

using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.PivotTable;

public class ViewSourceRecord : StandardRecord
{
	public const short sid = 227;

	private int vs;

	protected override int DataSize => 2;

	public override short Sid => 227;

	public ViewSourceRecord(RecordInputStream in1)
	{
		vs = in1.ReadShort();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(vs);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[SXVS]\n");
		stringBuilder.Append("    .vs      =").Append(HexDump.ShortToHex(vs)).Append('\n');
		stringBuilder.Append("[/SXVS]\n");
		return stringBuilder.ToString();
	}
}

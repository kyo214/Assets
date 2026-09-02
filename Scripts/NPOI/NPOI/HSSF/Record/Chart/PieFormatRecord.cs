using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class PieFormatRecord : StandardRecord
{
	public const short sid = 4107;

	private short field_1_pcExplode;

	protected override int DataSize => 2;

	public override short Sid => 4107;

	public int Explode
	{
		get
		{
			return field_1_pcExplode;
		}
		set
		{
			field_1_pcExplode = (short)value;
		}
	}

	public PieFormatRecord()
	{
	}

	public PieFormatRecord(RecordInputStream ris)
	{
		field_1_pcExplode = ris.ReadShort();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_pcExplode);
	}

	public override object Clone()
	{
		return new PieFormatRecord
		{
			Explode = Explode
		};
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[PIEFORMAT]").AppendLine().Append("   .pcExplode = ")
			.Append(HexDump.ToHex(field_1_pcExplode))
			.Append("(")
			.Append(field_1_pcExplode)
			.AppendLine(")")
			.AppendLine("[/PIEFORMAT]");
		return stringBuilder.ToString();
	}
}

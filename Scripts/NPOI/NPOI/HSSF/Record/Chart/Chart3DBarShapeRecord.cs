using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class Chart3DBarShapeRecord : StandardRecord
{
	public const short sid = 4191;

	private byte field_1_riser;

	private byte field_2_taper;

	protected override int DataSize => 2;

	public override short Sid => 4191;

	public byte Riser
	{
		get
		{
			return field_1_riser;
		}
		set
		{
			field_1_riser = value;
		}
	}

	public byte Taper
	{
		get
		{
			return field_2_taper;
		}
		set
		{
			field_2_taper = value;
		}
	}

	public Chart3DBarShapeRecord()
	{
	}

	public Chart3DBarShapeRecord(RecordInputStream in1)
	{
		field_1_riser = (byte)in1.ReadByte();
		field_2_taper = (byte)in1.ReadByte();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[Chart3DBarShape]\n");
		stringBuilder.Append("    .axisType             = ").Append("0x").Append(HexDump.ToHex(Riser))
			.Append(" (")
			.Append(Riser)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .x                    = ").Append("0x").Append(HexDump.ToHex(Taper))
			.Append(" (")
			.Append(Taper)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("[/Chart3DBarShape]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteByte(field_1_riser);
		out1.WriteByte(field_2_taper);
	}

	public override object Clone()
	{
		return new Chart3DBarShapeRecord
		{
			Riser = Riser,
			Taper = Taper
		};
	}
}

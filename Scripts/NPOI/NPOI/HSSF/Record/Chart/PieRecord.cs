using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class PieRecord : StandardRecord
{
	public const short sid = 4121;

	private short field_1_anStart;

	private short field_2_pcDonut;

	private short field_3_option;

	private BitField fHasShadow = BitFieldFactory.GetInstance(1);

	private BitField fShowLdrLines = BitFieldFactory.GetInstance(2);

	protected override int DataSize => 6;

	public override short Sid => 4121;

	public int Start
	{
		get
		{
			return field_1_anStart;
		}
		set
		{
			field_1_anStart = (short)value;
		}
	}

	public int Dount
	{
		get
		{
			return field_2_pcDonut;
		}
		set
		{
			field_2_pcDonut = (short)value;
		}
	}

	public bool HasShadow
	{
		get
		{
			return fHasShadow.IsSet(field_3_option);
		}
		set
		{
			field_3_option = fHasShadow.SetShortBoolean(field_3_option, value);
		}
	}

	public bool ShowLdrLines
	{
		get
		{
			return fShowLdrLines.IsSet(field_3_option);
		}
		set
		{
			field_3_option = fShowLdrLines.SetShortBoolean(field_3_option, value);
		}
	}

	public PieRecord()
	{
	}

	public PieRecord(RecordInputStream ris)
	{
		field_1_anStart = ris.ReadShort();
		field_2_pcDonut = ris.ReadShort();
		field_3_option = ris.ReadShort();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_anStart);
		out1.WriteShort(field_2_pcDonut);
		out1.WriteShort(field_3_option);
	}

	public override object Clone()
	{
		return new PieRecord
		{
			Dount = Dount,
			HasShadow = HasShadow,
			ShowLdrLines = ShowLdrLines,
			Start = Start
		};
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[PIE]").AppendLine().Append("   .anStart             =")
			.Append(HexDump.ToHex(field_1_anStart))
			.Append("(")
			.Append(field_1_anStart)
			.AppendLine(")")
			.Append("   .anDount             =")
			.Append(HexDump.ToHex(field_2_pcDonut))
			.Append("(")
			.Append(field_2_pcDonut)
			.AppendLine(")")
			.Append("   .option              =")
			.Append(HexDump.ToHex(field_3_option))
			.Append("(")
			.Append(field_3_option)
			.AppendLine(")")
			.Append("       .fHasShadow         =")
			.Append(HasShadow)
			.AppendLine()
			.Append("       .fShowLdrLines      =")
			.Append(ShowLdrLines)
			.AppendLine()
			.AppendLine("[/PIE]");
		return stringBuilder.ToString();
	}
}

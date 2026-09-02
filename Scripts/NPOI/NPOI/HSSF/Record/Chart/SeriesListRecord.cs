using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class SeriesListRecord : StandardRecord
{
	public const short sid = 4118;

	private short[] field_1_seriesNumbers;

	protected override int DataSize => field_1_seriesNumbers.Length * 2 + 2;

	public override short Sid => 4118;

	public short[] SeriesNumbers
	{
		get
		{
			return field_1_seriesNumbers;
		}
		set
		{
			field_1_seriesNumbers = value;
		}
	}

	public SeriesListRecord(short[] seriesNumbers)
	{
		field_1_seriesNumbers = ((seriesNumbers == null) ? null : ((short[])seriesNumbers.Clone()));
	}

	public SeriesListRecord(RecordInputStream in1)
	{
		int num = in1.ReadUShort();
		short[] array = new short[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = in1.ReadShort();
		}
		field_1_seriesNumbers = array;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[SERIESLIST]\n");
		stringBuilder.Append("    .seriesNumbers        = ").Append(" (").Append(SeriesNumbers)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("[/SERIESLIST]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		int num = field_1_seriesNumbers.Length;
		out1.WriteShort(num);
		for (int i = 0; i < num; i++)
		{
			out1.WriteShort(field_1_seriesNumbers[i]);
		}
	}

	public override object Clone()
	{
		return new SeriesListRecord(field_1_seriesNumbers);
	}
}

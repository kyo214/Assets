using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class DataFormatRecord : StandardRecord, ICloneable
{
	public const short sid = 4102;

	private short field_1_pointNumber;

	private short field_2_seriesIndex;

	private short field_3_seriesNumber;

	private short field_4_formatFlags;

	private BitField useExcel4Colors = BitFieldFactory.GetInstance(1);

	protected override int DataSize => 8;

	public override short Sid => 4102;

	public short PointNumber
	{
		get
		{
			return field_1_pointNumber;
		}
		set
		{
			field_1_pointNumber = value;
		}
	}

	public short SeriesIndex
	{
		get
		{
			return field_2_seriesIndex;
		}
		set
		{
			field_2_seriesIndex = value;
		}
	}

	public short SeriesNumber
	{
		get
		{
			return field_3_seriesNumber;
		}
		set
		{
			field_3_seriesNumber = value;
		}
	}

	public short FormatFlags
	{
		get
		{
			return field_4_formatFlags;
		}
		set
		{
			field_4_formatFlags = value;
		}
	}

	public bool UseExcel4Colors
	{
		get
		{
			return useExcel4Colors.IsSet(field_4_formatFlags);
		}
		set
		{
			field_4_formatFlags = useExcel4Colors.SetShortBoolean(field_4_formatFlags, value);
		}
	}

	public DataFormatRecord()
	{
	}

	public DataFormatRecord(RecordInputStream in1)
	{
		field_1_pointNumber = in1.ReadShort();
		field_2_seriesIndex = in1.ReadShort();
		field_3_seriesNumber = in1.ReadShort();
		field_4_formatFlags = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[DATAFORMAT]\n");
		stringBuilder.Append("    .pointNumber          = ").Append("0x").Append(HexDump.ToHex(PointNumber))
			.Append(" (")
			.Append(PointNumber)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .seriesIndex          = ").Append("0x").Append(HexDump.ToHex(SeriesIndex))
			.Append(" (")
			.Append(SeriesIndex)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .seriesNumber         = ").Append("0x").Append(HexDump.ToHex(SeriesNumber))
			.Append(" (")
			.Append(SeriesNumber)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .formatFlags          = ").Append("0x").Append(HexDump.ToHex(FormatFlags))
			.Append(" (")
			.Append(FormatFlags)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("         .useExcel4Colors          = ").Append(UseExcel4Colors).Append('\n');
		stringBuilder.Append("[/DATAFORMAT]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_pointNumber);
		out1.WriteShort(field_2_seriesIndex);
		out1.WriteShort(field_3_seriesNumber);
		out1.WriteShort(field_4_formatFlags);
	}

	public override object Clone()
	{
		return new DataFormatRecord
		{
			field_1_pointNumber = field_1_pointNumber,
			field_2_seriesIndex = field_2_seriesIndex,
			field_3_seriesNumber = field_3_seriesNumber,
			field_4_formatFlags = field_4_formatFlags
		};
	}
}

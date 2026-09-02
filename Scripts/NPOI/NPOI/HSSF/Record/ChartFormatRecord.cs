using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class ChartFormatRecord : StandardRecord
{
	public const short sid = 4116;

	private int field1_x_position;

	private int field2_y_position;

	private int field3_width;

	private int field4_height;

	private short field5_grbit;

	private BitField varyDisplayPattern = BitFieldFactory.GetInstance(1);

	private short field6_icrt;

	protected override int DataSize => 20;

	public override short Sid => 4116;

	public int XPosition
	{
		get
		{
			return field1_x_position;
		}
		set
		{
			field1_x_position = value;
		}
	}

	public int YPosition
	{
		get
		{
			return field2_y_position;
		}
		set
		{
			field2_y_position = value;
		}
	}

	public int Width
	{
		get
		{
			return field3_width;
		}
		set
		{
			field3_width = value;
		}
	}

	public int Height
	{
		get
		{
			return field4_height;
		}
		set
		{
			field4_height = value;
		}
	}

	public short Icrt
	{
		get
		{
			return field6_icrt;
		}
		set
		{
			field6_icrt = value;
		}
	}

	public bool VaryDisplayPattern
	{
		get
		{
			return varyDisplayPattern.IsSet(field5_grbit);
		}
		set
		{
			field5_grbit = varyDisplayPattern.SetShortBoolean(field5_grbit, value);
		}
	}

	public ChartFormatRecord()
	{
	}

	public ChartFormatRecord(RecordInputStream in1)
	{
		field1_x_position = in1.ReadInt();
		field2_y_position = in1.ReadInt();
		field3_width = in1.ReadInt();
		field4_height = in1.ReadInt();
		field5_grbit = in1.ReadShort();
		field6_icrt = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[CHARTFORMAT]\n");
		stringBuilder.Append("    .xPosition       = ").Append(XPosition).Append("\n");
		stringBuilder.Append("    .yPosition       = ").Append(YPosition).Append("\n");
		stringBuilder.Append("    .width           = ").Append(Width).Append("\n");
		stringBuilder.Append("    .height          = ").Append(Height).Append("\n");
		stringBuilder.Append("    .grBit           = ").Append(StringUtil.ToHexString(field5_grbit)).Append("\n");
		stringBuilder.Append("[/CHARTFORMAT]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteInt(XPosition);
		out1.WriteInt(YPosition);
		out1.WriteInt(Width);
		out1.WriteInt(Height);
		out1.WriteShort(field5_grbit);
		out1.WriteShort(field6_icrt);
	}

	public override object Clone()
	{
		return new ChartFormatRecord
		{
			Height = Height,
			Icrt = Icrt,
			VaryDisplayPattern = VaryDisplayPattern,
			Width = Width,
			XPosition = XPosition,
			YPosition = YPosition
		};
	}
}

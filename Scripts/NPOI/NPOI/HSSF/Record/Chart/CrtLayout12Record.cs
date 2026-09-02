using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class CrtLayout12Record : StandardRecord
{
	public const short sid = 2205;

	private short field_1_frtHeader_rt;

	private short field_2_frtHeader_grbitFrt;

	private int field_5_dwCheckSum;

	private short field_6_option;

	private short field_7_wXMode;

	private short field_8_wYMode;

	private short field_9_wWidthMode;

	private short field_10_wHeightMode;

	private double field_11_x;

	private double field_12_y;

	private double field_13_dx;

	private double field_14_dy;

	public static int AutoLayoutType_Bottom = 0;

	public static int AutoLayoutType_TopRightCorner = 1;

	public static int AutoLayoutType_Top = 2;

	public static int AutoLayoutType_Right = 3;

	public static int AutoLayoutType_Left = 4;

	private BitField autolayouttype = BitFieldFactory.GetInstance(30);

	protected override int DataSize => 60;

	public override short Sid => 2205;

	public int AutoLayoutType
	{
		get
		{
			return autolayouttype.GetValue(field_6_option);
		}
		set
		{
			field_6_option = autolayouttype.SetShortValue(field_6_option, (short)value);
		}
	}

	public int CheckSum
	{
		get
		{
			return field_5_dwCheckSum;
		}
		set
		{
			field_5_dwCheckSum = value;
		}
	}

	public CrtLayout12Mode XMode
	{
		get
		{
			return (CrtLayout12Mode)field_7_wXMode;
		}
		set
		{
			field_7_wXMode = (short)value;
		}
	}

	public CrtLayout12Mode YMode
	{
		get
		{
			return (CrtLayout12Mode)field_8_wYMode;
		}
		set
		{
			field_8_wYMode = (short)value;
		}
	}

	public CrtLayout12Mode WidthMode
	{
		get
		{
			return (CrtLayout12Mode)field_9_wWidthMode;
		}
		set
		{
			field_9_wWidthMode = (short)value;
		}
	}

	public CrtLayout12Mode HeightMode
	{
		get
		{
			return (CrtLayout12Mode)field_10_wHeightMode;
		}
		set
		{
			field_10_wHeightMode = (short)value;
		}
	}

	public double X
	{
		get
		{
			return field_11_x;
		}
		set
		{
			field_11_x = value;
		}
	}

	public double Y
	{
		get
		{
			return field_12_y;
		}
		set
		{
			field_12_y = value;
		}
	}

	public double DX
	{
		get
		{
			return field_13_dx;
		}
		set
		{
			field_13_dx = value;
		}
	}

	public double DY
	{
		get
		{
			return field_14_dy;
		}
		set
		{
			field_14_dy = value;
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[CRTLAYOUT12]").AppendLine().Append("   .rt               =")
			.Append(HexDump.ToHex(field_1_frtHeader_rt))
			.Append("(")
			.Append(field_1_frtHeader_rt)
			.AppendLine(")")
			.Append("   .grbit            =")
			.Append(HexDump.ToHex(field_2_frtHeader_grbitFrt))
			.Append("(")
			.Append(field_2_frtHeader_grbitFrt)
			.AppendLine(")")
			.Append("   .reserved         =")
			.Append(HexDump.ToHex(0))
			.Append("(")
			.Append(0)
			.AppendLine(")")
			.Append("   .dwCheckSum       =")
			.Append(HexDump.ToHex(field_5_dwCheckSum))
			.Append("(")
			.Append(field_5_dwCheckSum)
			.AppendLine(")")
			.Append("   .option           =")
			.Append(HexDump.ToHex(field_6_option))
			.Append("(")
			.Append(field_6_option)
			.AppendLine(")")
			.Append("       .autolayouttype =")
			.Append(autolayouttype.GetValue(field_6_option))
			.AppendLine()
			.Append("   .wXMode           =")
			.Append(HexDump.ToHex(field_7_wXMode))
			.Append("(")
			.Append(field_7_wXMode)
			.AppendLine(")")
			.Append("   .wYMode           =")
			.Append(HexDump.ToHex(field_8_wYMode))
			.Append("(")
			.Append(field_8_wYMode)
			.AppendLine(")")
			.Append("   .wWidthMode       =")
			.Append(HexDump.ToHex(field_9_wWidthMode))
			.Append("(")
			.Append(field_9_wWidthMode)
			.AppendLine(")")
			.Append("   .wHeightMode      =")
			.Append(HexDump.ToHex(field_10_wHeightMode))
			.Append("(")
			.Append(field_10_wHeightMode)
			.AppendLine(")")
			.Append("   .x                =")
			.Append(HexDump.ToHex(BitConverter.DoubleToInt64Bits(field_11_x)))
			.Append("(")
			.Append(field_11_x)
			.AppendLine(")")
			.Append("   .y                =")
			.Append(HexDump.ToHex(BitConverter.DoubleToInt64Bits(field_12_y)))
			.Append("(")
			.Append(field_12_y)
			.AppendLine(")")
			.Append("   .dx               =")
			.Append(HexDump.ToHex(BitConverter.DoubleToInt64Bits(field_13_dx)))
			.Append("(")
			.Append(field_13_dx)
			.AppendLine(")")
			.Append("   .dy               =")
			.Append(HexDump.ToHex(BitConverter.DoubleToInt64Bits(field_14_dy)))
			.Append("(")
			.Append(field_14_dy)
			.AppendLine(")")
			.AppendLine("[/CRTLAYOUT12]");
		return stringBuilder.ToString();
	}

	public CrtLayout12Record()
	{
		field_1_frtHeader_rt = 2205;
		field_2_frtHeader_grbitFrt = 0;
	}

	public CrtLayout12Record(RecordInputStream ris)
	{
		field_1_frtHeader_rt = ris.ReadShort();
		field_2_frtHeader_grbitFrt = ris.ReadShort();
		ris.ReadLong();
		field_5_dwCheckSum = ris.ReadInt();
		field_6_option = ris.ReadShort();
		field_7_wXMode = ris.ReadShort();
		field_8_wYMode = ris.ReadShort();
		field_9_wWidthMode = ris.ReadShort();
		field_10_wHeightMode = ris.ReadShort();
		field_11_x = ris.ReadDouble();
		field_12_y = ris.ReadDouble();
		field_13_dx = ris.ReadDouble();
		field_14_dy = ris.ReadDouble();
		ris.ReadShort();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_frtHeader_rt);
		out1.WriteShort(field_2_frtHeader_grbitFrt);
		out1.WriteInt(0);
		out1.WriteInt(0);
		out1.WriteInt(field_5_dwCheckSum);
		out1.WriteShort(field_6_option);
		out1.WriteShort(field_7_wXMode);
		out1.WriteShort(field_8_wYMode);
		out1.WriteShort(field_9_wWidthMode);
		out1.WriteShort(field_10_wHeightMode);
		out1.WriteDouble(field_11_x);
		out1.WriteDouble(field_12_y);
		out1.WriteDouble(field_13_dx);
		out1.WriteDouble(field_14_dy);
		out1.WriteShort(0);
	}

	public override object Clone()
	{
		return new CrtLayout12Record
		{
			AutoLayoutType = AutoLayoutType,
			CheckSum = CheckSum,
			DX = DX,
			DY = DY,
			HeightMode = HeightMode,
			WidthMode = WidthMode,
			X = X,
			XMode = XMode,
			Y = Y,
			YMode = YMode
		};
	}
}

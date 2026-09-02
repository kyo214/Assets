using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class CrtLayout12ARecord : StandardRecord
{
	public const short sid = 2215;

	private FrtHeader frtHeader;

	private int field_1_dwCheckSum;

	private short field_2_option;

	private short field_3_xTL;

	private short field_4_yTL;

	private short field_5_xBR;

	private short field_6_yBR;

	private short field_7_wXMode;

	private short field_8_wYMode;

	private short field_9_wWidthMode;

	private short field_10_wHeightMode;

	private double field_11_x;

	private double field_12_y;

	private double field_13_dx;

	private double field_14_dy;

	private short reserved2;

	private BitField fLayoutTargetInner = BitFieldFactory.GetInstance(1);

	protected override int DataSize => 68;

	public override short Sid => 2215;

	public bool IsLayoutTargetInner
	{
		get
		{
			return fLayoutTargetInner.IsSet(field_2_option);
		}
		set
		{
			field_2_option = fLayoutTargetInner.SetShortBoolean(field_2_option, value);
		}
	}

	public int CheckSum
	{
		get
		{
			return field_1_dwCheckSum;
		}
		set
		{
			field_1_dwCheckSum = value;
		}
	}

	public short XTL
	{
		get
		{
			return field_3_xTL;
		}
		set
		{
			field_3_xTL = value;
		}
	}

	public short YTL
	{
		get
		{
			return field_4_yTL;
		}
		set
		{
			field_4_yTL = value;
		}
	}

	public short XBR
	{
		get
		{
			return field_5_xBR;
		}
		set
		{
			field_5_xBR = value;
		}
	}

	public short YBR
	{
		get
		{
			return field_6_yBR;
		}
		set
		{
			field_6_yBR = value;
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

	public CrtLayout12ARecord()
	{
		frtHeader.rt = 2215;
		frtHeader.grbitFrt = 0;
	}

	public override object Clone()
	{
		return new CrtLayout12ARecord
		{
			IsLayoutTargetInner = IsLayoutTargetInner,
			CheckSum = CheckSum,
			DX = DX,
			DY = DY,
			HeightMode = HeightMode,
			WidthMode = WidthMode,
			X = X,
			XMode = XMode,
			Y = Y,
			YMode = YMode,
			XTL = XTL,
			YTL = YTL,
			XBR = XBR,
			YBR = YBR
		};
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[CRTLAYOUT12A]").AppendLine().Append("   .rt               =")
			.Append(HexDump.ToHex(frtHeader.rt))
			.Append("(")
			.Append(frtHeader.rt)
			.AppendLine(")")
			.Append("   .grbit            =")
			.Append(HexDump.ToHex(frtHeader.grbitFrt))
			.Append("(")
			.Append(frtHeader.grbitFrt)
			.AppendLine(")")
			.Append("   .reserved         =")
			.Append(HexDump.ToHex(0))
			.Append("(")
			.Append(0)
			.AppendLine(")")
			.Append("   .dwCheckSum       =")
			.Append(HexDump.ToHex(field_1_dwCheckSum))
			.Append("(")
			.Append(field_1_dwCheckSum)
			.AppendLine(")")
			.Append("   .option           =")
			.Append(HexDump.ToHex(field_2_option))
			.Append("(")
			.Append(field_2_option)
			.AppendLine(")")
			.Append("       .fLayoutTargetInner =")
			.Append(IsLayoutTargetInner)
			.AppendLine()
			.Append("   .xTL              =")
			.Append(HexDump.ToHex(field_3_xTL))
			.Append("(")
			.Append(field_3_xTL)
			.AppendLine(")")
			.Append("   .yTL              =")
			.Append(HexDump.ToHex(field_4_yTL))
			.Append("(")
			.Append(field_4_yTL)
			.AppendLine(")")
			.Append("   .xBR              =")
			.Append(HexDump.ToHex(field_5_xBR))
			.Append("(")
			.Append(field_5_xBR)
			.AppendLine(")")
			.Append("   .yBR              =")
			.Append(HexDump.ToHex(field_6_yBR))
			.Append("(")
			.Append(field_6_yBR)
			.AppendLine(")")
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
			.AppendLine("[/CRTLAYOUT12A]");
		return stringBuilder.ToString();
	}

	public CrtLayout12ARecord(RecordInputStream ris)
	{
		frtHeader.rt = (ushort)ris.ReadUShort();
		frtHeader.grbitFrt = (ushort)ris.ReadUShort();
		ris.ReadLong();
		field_1_dwCheckSum = ris.ReadInt();
		field_2_option = ris.ReadShort();
		field_3_xTL = ris.ReadShort();
		field_4_yTL = ris.ReadShort();
		field_5_xBR = ris.ReadShort();
		field_6_yBR = ris.ReadShort();
		field_7_wXMode = ris.ReadShort();
		field_8_wYMode = ris.ReadShort();
		field_9_wWidthMode = ris.ReadShort();
		field_10_wHeightMode = ris.ReadShort();
		field_11_x = ris.ReadDouble();
		field_12_y = ris.ReadDouble();
		field_13_dx = ris.ReadDouble();
		field_14_dy = ris.ReadDouble();
		reserved2 = ris.ReadShort();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(frtHeader.rt);
		out1.WriteShort(frtHeader.grbitFrt);
		out1.WriteLong(frtHeader.reserved);
		out1.WriteInt(field_1_dwCheckSum);
		out1.WriteShort(field_2_option);
		out1.WriteShort(field_3_xTL);
		out1.WriteShort(field_4_yTL);
		out1.WriteShort(field_5_xBR);
		out1.WriteShort(field_6_yBR);
		out1.WriteShort(field_7_wXMode);
		out1.WriteShort(field_8_wYMode);
		out1.WriteShort(field_9_wWidthMode);
		out1.WriteShort(field_10_wHeightMode);
		out1.WriteDouble(field_11_x);
		out1.WriteDouble(field_12_y);
		out1.WriteDouble(field_13_dx);
		out1.WriteDouble(field_14_dy);
		out1.WriteShort(reserved2);
	}
}

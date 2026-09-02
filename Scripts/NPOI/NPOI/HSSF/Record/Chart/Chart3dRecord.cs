using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class Chart3dRecord : StandardRecord
{
	public const short sid = 4154;

	private short field_1_anRot;

	private short field_2_anElev;

	private short field_3_pcDist;

	private short field_4_pcHeight;

	private short field_5_pcDepth;

	private short field_6_pcGap;

	private short field_7_option;

	private BitField fPerspective = BitFieldFactory.GetInstance(1);

	private BitField fCluster = BitFieldFactory.GetInstance(2);

	private BitField f3DScaling = BitFieldFactory.GetInstance(4);

	private BitField reserved1 = BitFieldFactory.GetInstance(8);

	private BitField fNotPieChart = BitFieldFactory.GetInstance(16);

	private BitField fWalls2D = BitFieldFactory.GetInstance(32);

	protected override int DataSize => 14;

	public override short Sid => 4154;

	public int Rotation
	{
		get
		{
			return field_1_anRot;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			if (value > 360)
			{
				value = 360;
			}
			field_1_anRot = (short)value;
		}
	}

	public int Elev
	{
		get
		{
			return field_2_anElev;
		}
		set
		{
			if (value < -90)
			{
				value = -90;
			}
			if (value > 90)
			{
				value = 90;
			}
			field_2_anElev = (short)value;
		}
	}

	public int Dist
	{
		get
		{
			return field_3_pcDist;
		}
		set
		{
			if (value < 0)
			{
				value = 0;
			}
			if (value > 200)
			{
				value = 200;
			}
			field_3_pcDist = (short)value;
		}
	}

	public int Height
	{
		get
		{
			return field_4_pcHeight;
		}
		set
		{
			field_4_pcHeight = (short)value;
		}
	}

	public int Depth
	{
		get
		{
			return field_5_pcDepth;
		}
		set
		{
			field_5_pcDepth = (short)value;
		}
	}

	public int Gap
	{
		get
		{
			return field_6_pcGap;
		}
		set
		{
			field_6_pcGap = (short)value;
		}
	}

	public bool IsPerspective
	{
		get
		{
			return fPerspective.IsSet(field_7_option);
		}
		set
		{
			field_7_option = fPerspective.SetShortBoolean(field_7_option, value);
		}
	}

	public bool IsCluster
	{
		get
		{
			return fCluster.IsSet(field_7_option);
		}
		set
		{
			field_7_option = fCluster.SetShortBoolean(field_7_option, value);
		}
	}

	public bool Is3DScaling
	{
		get
		{
			return f3DScaling.IsSet(field_7_option);
		}
		set
		{
			field_7_option = f3DScaling.SetShortBoolean(field_7_option, value);
		}
	}

	public bool IsNotPieChart
	{
		get
		{
			return fNotPieChart.IsSet(field_7_option);
		}
		set
		{
			field_7_option = fNotPieChart.SetShortBoolean(field_7_option, value);
		}
	}

	public bool IsWalls2D
	{
		get
		{
			return fWalls2D.IsSet(field_7_option);
		}
		set
		{
			field_7_option = fWalls2D.SetShortBoolean(field_7_option, value);
		}
	}

	public Chart3dRecord()
	{
	}

	public Chart3dRecord(RecordInputStream in1)
	{
		field_1_anRot = in1.ReadShort();
		field_2_anElev = in1.ReadShort();
		field_3_pcDist = in1.ReadShort();
		field_4_pcHeight = in1.ReadShort();
		field_5_pcDepth = in1.ReadShort();
		field_6_pcGap = in1.ReadShort();
		field_7_option = in1.ReadShort();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_anRot);
		out1.WriteShort(field_2_anElev);
		out1.WriteShort(field_3_pcDist);
		out1.WriteShort(field_4_pcHeight);
		out1.WriteShort(field_5_pcDepth);
		out1.WriteShort(field_6_pcGap);
		out1.WriteShort(field_7_option);
	}

	public override object Clone()
	{
		return new Chart3dRecord
		{
			Depth = Depth,
			Dist = Dist,
			Elev = Elev,
			Height = Height,
			Gap = Gap,
			Is3DScaling = Is3DScaling,
			IsCluster = IsCluster,
			IsNotPieChart = IsNotPieChart,
			IsPerspective = IsPerspective,
			IsWalls2D = IsWalls2D,
			Rotation = Rotation
		};
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[CHART3D]").AppendLine().Append("   .anRot              =")
			.Append(HexDump.ToHex(field_1_anRot))
			.Append("(")
			.Append(field_1_anRot)
			.AppendLine(")")
			.Append("   .anElev             =")
			.Append(HexDump.ToHex(field_2_anElev))
			.Append("(")
			.Append(field_2_anElev)
			.AppendLine(")")
			.Append("   .pcDist             =")
			.Append(HexDump.ToHex(field_3_pcDist))
			.Append("(")
			.Append(field_3_pcDist)
			.AppendLine(")")
			.Append("   .pcHeight           =")
			.Append(HexDump.ToHex(field_4_pcHeight))
			.Append("(")
			.Append(field_4_pcHeight)
			.AppendLine(")")
			.Append("   .pcDepth            =")
			.Append(HexDump.ToHex(field_5_pcDepth))
			.Append("(")
			.Append(field_5_pcDepth)
			.AppendLine(")")
			.Append("   .pcGap              =")
			.Append(HexDump.ToHex(field_6_pcGap))
			.Append("(")
			.Append(field_6_pcGap)
			.AppendLine(")")
			.Append("   .option             =")
			.Append(HexDump.ToHex(field_7_option))
			.Append("(")
			.Append(field_7_option)
			.AppendLine(")")
			.Append("       .fPerspective       =")
			.Append(IsPerspective)
			.AppendLine()
			.Append("       .fCluster           =")
			.Append(IsCluster)
			.AppendLine()
			.Append("       .f3DScaling         =")
			.Append(Is3DScaling)
			.AppendLine()
			.Append("       .fNotPieChart       =")
			.Append(IsNotPieChart)
			.AppendLine()
			.Append("       .fWalls2D           =")
			.Append(IsWalls2D)
			.AppendLine()
			.AppendLine("[/CHART3D]");
		return stringBuilder.ToString();
	}
}

using System;
using System.Text;
using NPOI.HSSF.Record.Common;
using NPOI.Util;

namespace NPOI.HSSF.Record.CF;

public class DataBarFormatting : ICloneable
{
	private byte options;

	private byte percentMin;

	private byte percentMax;

	private ExtendedColor color;

	private DataBarThreshold thresholdMin;

	private DataBarThreshold thresholdMax;

	private static BitField iconOnly = BitFieldFactory.GetInstance(1);

	private static BitField reversed = BitFieldFactory.GetInstance(4);

	public bool IsIconOnly
	{
		get
		{
			return GetOptionFlag(iconOnly);
		}
		set
		{
			SetOptionFlag(value, iconOnly);
		}
	}

	public bool IsReversed
	{
		get
		{
			return GetOptionFlag(reversed);
		}
		set
		{
			SetOptionFlag(value, reversed);
		}
	}

	public byte PercentMin
	{
		get
		{
			return percentMin;
		}
		set
		{
			percentMin = value;
		}
	}

	public byte PercentMax
	{
		get
		{
			return percentMax;
		}
		set
		{
			percentMax = value;
		}
	}

	public ExtendedColor Color
	{
		get
		{
			return color;
		}
		set
		{
			color = value;
		}
	}

	public DataBarThreshold ThresholdMin
	{
		get
		{
			return thresholdMin;
		}
		set
		{
			thresholdMin = value;
		}
	}

	public DataBarThreshold ThresholdMax
	{
		get
		{
			return thresholdMax;
		}
		set
		{
			thresholdMax = value;
		}
	}

	public int DataLength => 6 + color.DataLength + thresholdMin.DataLength + thresholdMax.DataLength;

	public DataBarFormatting()
	{
		options = 2;
	}

	public DataBarFormatting(ILittleEndianInput in1)
	{
		in1.ReadShort();
		in1.ReadByte();
		options = (byte)in1.ReadByte();
		percentMin = (byte)in1.ReadByte();
		percentMax = (byte)in1.ReadByte();
		if (percentMin < 0 || percentMin > 100)
		{
			Console.WriteLine("Inconsistent Minimum Percentage found " + percentMin);
		}
		if (percentMax < 0 || percentMax > 100)
		{
			Console.WriteLine("Inconsistent Minimum Percentage found " + percentMin);
		}
		color = new ExtendedColor(in1);
		thresholdMin = new DataBarThreshold(in1);
		thresholdMax = new DataBarThreshold(in1);
	}

	private bool GetOptionFlag(BitField field)
	{
		if (field.GetValue(options) != 0)
		{
			return true;
		}
		return false;
	}

	private void SetOptionFlag(bool option, BitField field)
	{
		options = field.SetByteBoolean(options, option);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("    [Data Bar Formatting]\n");
		stringBuilder.Append("          .icon_only= ").Append(IsIconOnly).Append("\n");
		stringBuilder.Append("          .reversed = ").Append(IsReversed).Append("\n");
		stringBuilder.Append(color);
		stringBuilder.Append(thresholdMin);
		stringBuilder.Append(thresholdMax);
		stringBuilder.Append("    [/Data Bar Formatting]\n");
		return stringBuilder.ToString();
	}

	public object Clone()
	{
		return new DataBarFormatting
		{
			options = options,
			percentMin = percentMin,
			percentMax = percentMax,
			color = (ExtendedColor)color.Clone(),
			thresholdMin = (DataBarThreshold)thresholdMin.Clone(),
			thresholdMax = (DataBarThreshold)thresholdMax.Clone()
		};
	}

	public void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(0);
		out1.WriteByte(0);
		out1.WriteByte(options);
		out1.WriteByte(percentMin);
		out1.WriteByte(percentMax);
		color.Serialize(out1);
		thresholdMin.Serialize(out1);
		thresholdMax.Serialize(out1);
	}
}

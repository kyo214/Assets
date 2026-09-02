using System;
using NPOI.Util;

namespace NPOI.HSSF.Record.CF;

public class ColorGradientThreshold : Threshold, ICloneable
{
	private double position;

	public double Position
	{
		get
		{
			return position;
		}
		set
		{
			position = value;
		}
	}

	public override int DataLength => base.DataLength + 8;

	public ColorGradientThreshold()
	{
		position = 0.0;
	}

	public ColorGradientThreshold(ILittleEndianInput in1)
		: base(in1)
	{
		position = in1.ReadDouble();
	}

	public object Clone()
	{
		ColorGradientThreshold colorGradientThreshold = new ColorGradientThreshold();
		CopyTo(colorGradientThreshold);
		colorGradientThreshold.position = position;
		return colorGradientThreshold;
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		base.Serialize(out1);
		out1.WriteDouble(position);
	}
}

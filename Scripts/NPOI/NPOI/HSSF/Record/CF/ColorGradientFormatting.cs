using System;
using System.Text;
using NPOI.HSSF.Record.Common;
using NPOI.Util;

namespace NPOI.HSSF.Record.CF;

public class ColorGradientFormatting : ICloneable
{
	private byte options;

	private ColorGradientThreshold[] thresholds;

	private ExtendedColor[] colors;

	private static BitField clamp = BitFieldFactory.GetInstance(1);

	private static BitField background = BitFieldFactory.GetInstance(2);

	public int NumControlPoints
	{
		get
		{
			return thresholds.Length;
		}
		set
		{
			if (value != thresholds.Length)
			{
				ColorGradientThreshold[] destinationArray = new ColorGradientThreshold[value];
				ExtendedColor[] destinationArray2 = new ExtendedColor[value];
				int length = Math.Min(thresholds.Length, value);
				Array.Copy(thresholds, 0, destinationArray, 0, length);
				Array.Copy(colors, 0, destinationArray2, 0, length);
				thresholds = destinationArray;
				colors = destinationArray2;
				updateThresholdPositions();
			}
		}
	}

	public ColorGradientThreshold[] Thresholds
	{
		get
		{
			return thresholds;
		}
		set
		{
			thresholds = ((value == null) ? null : ((ColorGradientThreshold[])value.Clone()));
			updateThresholdPositions();
		}
	}

	public ExtendedColor[] Colors
	{
		get
		{
			return colors;
		}
		set
		{
			colors = ((value == null) ? null : ((ExtendedColor[])value.Clone()));
		}
	}

	public bool IsClampToCurve => GetOptionFlag(clamp);

	public bool IsAppliesToBackground => GetOptionFlag(background);

	public int DataLength
	{
		get
		{
			int num = 6;
			ColorGradientThreshold[] array = thresholds;
			foreach (Threshold threshold in array)
			{
				num += threshold.DataLength;
			}
			ExtendedColor[] array2 = colors;
			foreach (ExtendedColor extendedColor in array2)
			{
				num += extendedColor.DataLength;
				num += 8;
			}
			return num;
		}
	}

	public ColorGradientFormatting()
	{
		options = 3;
		thresholds = new ColorGradientThreshold[3];
		colors = new ExtendedColor[3];
	}

	public ColorGradientFormatting(ILittleEndianInput in1)
	{
		in1.ReadShort();
		in1.ReadByte();
		int num = in1.ReadByte();
		int num2 = in1.ReadByte();
		options = (byte)in1.ReadByte();
		thresholds = new ColorGradientThreshold[num];
		for (int i = 0; i < thresholds.Length; i++)
		{
			thresholds[i] = new ColorGradientThreshold(in1);
		}
		colors = new ExtendedColor[num2];
		for (int j = 0; j < colors.Length; j++)
		{
			in1.ReadDouble();
			colors[j] = new ExtendedColor(in1);
		}
	}

	private bool GetOptionFlag(BitField field)
	{
		if (field.GetValue(options) != 0)
		{
			return true;
		}
		return false;
	}

	private void updateThresholdPositions()
	{
		double num = 1.0 / (double)(thresholds.Length - 1);
		for (int i = 0; i < thresholds.Length; i++)
		{
			thresholds[i].Position = num * (double)i;
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("    [Color Gradient Formatting]\n");
		stringBuilder.Append("          .clamp     = ").Append(IsClampToCurve).Append("\n");
		stringBuilder.Append("          .background= ").Append(IsAppliesToBackground).Append("\n");
		ColorGradientThreshold[] array = thresholds;
		foreach (Threshold threshold in array)
		{
			stringBuilder.Append(threshold.ToString());
		}
		ExtendedColor[] array2 = colors;
		foreach (ExtendedColor extendedColor in array2)
		{
			stringBuilder.Append(extendedColor.ToString());
		}
		stringBuilder.Append("    [/Color Gradient Formatting]\n");
		return stringBuilder.ToString();
	}

	public object Clone()
	{
		ColorGradientFormatting colorGradientFormatting = new ColorGradientFormatting();
		colorGradientFormatting.options = options;
		colorGradientFormatting.thresholds = new ColorGradientThreshold[thresholds.Length];
		colorGradientFormatting.colors = new ExtendedColor[colors.Length];
		Array.Copy(thresholds, 0, colorGradientFormatting.thresholds, 0, thresholds.Length);
		Array.Copy(colors, 0, colorGradientFormatting.colors, 0, colors.Length);
		return colorGradientFormatting;
	}

	public void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(0);
		out1.WriteByte(0);
		out1.WriteByte(thresholds.Length);
		out1.WriteByte(thresholds.Length);
		out1.WriteByte(options);
		ColorGradientThreshold[] array = thresholds;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Serialize(out1);
		}
		double num = 1.0 / (double)(colors.Length - 1);
		for (int j = 0; j < colors.Length; j++)
		{
			out1.WriteDouble((double)j * num);
			colors[j].Serialize(out1);
		}
	}
}

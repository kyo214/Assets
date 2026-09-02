using NPOI.HSSF.Record;
using NPOI.HSSF.Record.CF;
using NPOI.HSSF.Record.Common;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

public class HSSFColorScaleFormatting : IColorScaleFormatting
{
	private HSSFSheet sheet;

	private CFRule12Record cfRule12Record;

	private ColorGradientFormatting colorFormatting;

	public int NumControlPoints
	{
		get
		{
			return colorFormatting.NumControlPoints;
		}
		set
		{
			colorFormatting.NumControlPoints = value;
		}
	}

	public IColor[] Colors
	{
		get
		{
			NPOI.HSSF.Record.Common.ExtendedColor[] colors = colorFormatting.Colors;
			HSSFExtendedColor[] array = new HSSFExtendedColor[colors.Length];
			for (int i = 0; i < colors.Length; i++)
			{
				array[i] = new HSSFExtendedColor(colors[i]);
			}
			return array;
		}
		set
		{
			NPOI.HSSF.Record.Common.ExtendedColor[] array = new NPOI.HSSF.Record.Common.ExtendedColor[value.Length];
			for (int i = 0; i < value.Length; i++)
			{
				array[i] = ((HSSFExtendedColor)value[i]).ExtendedColor;
			}
			colorFormatting.Colors = array;
		}
	}

	public IConditionalFormattingThreshold[] Thresholds
	{
		get
		{
			Threshold[] thresholds = colorFormatting.Thresholds;
			Threshold[] array = thresholds;
			HSSFConditionalFormattingThreshold[] array2 = new HSSFConditionalFormattingThreshold[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = new HSSFConditionalFormattingThreshold(array[i], sheet);
			}
			return array2;
		}
		set
		{
			ColorGradientThreshold[] array = new ColorGradientThreshold[value.Length];
			for (int i = 0; i < array.Length; i++)
			{
				HSSFConditionalFormattingThreshold hSSFConditionalFormattingThreshold = (HSSFConditionalFormattingThreshold)value[i];
				array[i] = (ColorGradientThreshold)hSSFConditionalFormattingThreshold.Threshold;
			}
			colorFormatting.Thresholds = array;
		}
	}

	protected internal HSSFColorScaleFormatting(CFRule12Record cfRule12Record, HSSFSheet sheet)
	{
		this.sheet = sheet;
		this.cfRule12Record = cfRule12Record;
		colorFormatting = this.cfRule12Record.ColorGradientFormatting;
	}

	public IConditionalFormattingThreshold CreateThreshold()
	{
		return new HSSFConditionalFormattingThreshold(new ColorGradientThreshold(), sheet);
	}
}

using NPOI.HSSF.Record;
using NPOI.HSSF.Record.CF;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

public class HSSFIconMultiStateFormatting : IIconMultiStateFormatting
{
	private HSSFSheet sheet;

	private CFRule12Record cfRule12Record;

	private IconMultiStateFormatting iconFormatting;

	public IconSet IconSet
	{
		get
		{
			return iconFormatting.IconSet;
		}
		set
		{
			iconFormatting.IconSet = value;
		}
	}

	public bool IsIconOnly
	{
		get
		{
			return iconFormatting.IsIconOnly;
		}
		set
		{
			iconFormatting.IsIconOnly = value;
		}
	}

	public bool IsReversed
	{
		get
		{
			return iconFormatting.IsReversed;
		}
		set
		{
			iconFormatting.IsReversed = value;
		}
	}

	public IConditionalFormattingThreshold[] Thresholds
	{
		get
		{
			Threshold[] thresholds = iconFormatting.Thresholds;
			HSSFConditionalFormattingThreshold[] array = new HSSFConditionalFormattingThreshold[thresholds.Length];
			for (int i = 0; i < thresholds.Length; i++)
			{
				array[i] = new HSSFConditionalFormattingThreshold(thresholds[i], sheet);
			}
			return array;
		}
		set
		{
			Threshold[] array = new Threshold[value.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = ((HSSFConditionalFormattingThreshold)value[i]).Threshold;
			}
			iconFormatting.Thresholds = array;
		}
	}

	protected internal HSSFIconMultiStateFormatting(CFRule12Record cfRule12Record, HSSFSheet sheet)
	{
		this.sheet = sheet;
		this.cfRule12Record = cfRule12Record;
		iconFormatting = this.cfRule12Record.MultiStateFormatting;
	}

	public IConditionalFormattingThreshold CreateThreshold()
	{
		return new HSSFConditionalFormattingThreshold(new IconMultiStateThreshold(), sheet);
	}
}

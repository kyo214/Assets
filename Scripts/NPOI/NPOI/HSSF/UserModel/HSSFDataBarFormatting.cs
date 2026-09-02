using NPOI.HSSF.Record;
using NPOI.HSSF.Record.CF;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

public class HSSFDataBarFormatting : IDataBarFormatting
{
	private HSSFSheet sheet;

	private CFRule12Record cfRule12Record;

	private DataBarFormatting databarFormatting;

	public bool IsLeftToRight
	{
		get
		{
			return !databarFormatting.IsReversed;
		}
		set
		{
			databarFormatting.IsReversed = value;
		}
	}

	public int WidthMin
	{
		get
		{
			return databarFormatting.PercentMin;
		}
		set
		{
			databarFormatting.PercentMin = (byte)value;
		}
	}

	public int WidthMax
	{
		get
		{
			return databarFormatting.PercentMax;
		}
		set
		{
			databarFormatting.PercentMax = (byte)value;
		}
	}

	public IColor Color
	{
		get
		{
			return new HSSFExtendedColor(databarFormatting.Color);
		}
		set
		{
			HSSFExtendedColor hSSFExtendedColor = (HSSFExtendedColor)value;
			databarFormatting.Color = hSSFExtendedColor.ExtendedColor;
		}
	}

	public IConditionalFormattingThreshold MinThreshold => new HSSFConditionalFormattingThreshold(databarFormatting.ThresholdMin, sheet);

	public IConditionalFormattingThreshold MaxThreshold => new HSSFConditionalFormattingThreshold(databarFormatting.ThresholdMax, sheet);

	public bool IsIconOnly
	{
		get
		{
			return databarFormatting.IsIconOnly;
		}
		set
		{
			databarFormatting.IsIconOnly = value;
		}
	}

	protected internal HSSFDataBarFormatting(CFRule12Record cfRule12Record, HSSFSheet sheet)
	{
		this.sheet = sheet;
		this.cfRule12Record = cfRule12Record;
		databarFormatting = this.cfRule12Record.DataBarFormatting;
	}

	public HSSFConditionalFormattingThreshold CreateThreshold()
	{
		return new HSSFConditionalFormattingThreshold(new DataBarThreshold(), sheet);
	}
}

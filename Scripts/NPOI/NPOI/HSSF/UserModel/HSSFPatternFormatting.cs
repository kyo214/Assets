using NPOI.HSSF.Record;
using NPOI.HSSF.Record.CF;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

public class HSSFPatternFormatting : IPatternFormatting
{
	private CFRuleBase cfRuleRecord;

	private HSSFWorkbook workbook;

	private PatternFormatting patternFormatting;

	public PatternFormatting PatternFormattingBlock => patternFormatting;

	public IColor FillBackgroundColorColor
	{
		get
		{
			return workbook.GetCustomPalette().GetColor(FillBackgroundColor);
		}
		set
		{
			HSSFColor hSSFColor = HSSFColor.ToHSSFColor(value);
			if (hSSFColor == null)
			{
				FillBackgroundColor = 0;
			}
			else
			{
				FillBackgroundColor = hSSFColor.Indexed;
			}
		}
	}

	public IColor FillForegroundColorColor
	{
		get
		{
			return workbook.GetCustomPalette().GetColor(FillForegroundColor);
		}
		set
		{
			HSSFColor hSSFColor = HSSFColor.ToHSSFColor(value);
			if (hSSFColor == null)
			{
				FillForegroundColor = 0;
			}
			else
			{
				FillForegroundColor = hSSFColor.Indexed;
			}
		}
	}

	public short FillBackgroundColor
	{
		get
		{
			return patternFormatting.FillBackgroundColor;
		}
		set
		{
			patternFormatting.FillBackgroundColor = value;
			if (value != 0)
			{
				cfRuleRecord.IsPatternBackgroundColorModified = true;
			}
		}
	}

	public short FillForegroundColor
	{
		get
		{
			return patternFormatting.FillForegroundColor;
		}
		set
		{
			patternFormatting.FillForegroundColor = value;
			if (value != 0)
			{
				cfRuleRecord.IsPatternColorModified = true;
			}
		}
	}

	public FillPattern FillPattern
	{
		get
		{
			return patternFormatting.FillPattern;
		}
		set
		{
			patternFormatting.FillPattern = value;
			if (value != FillPattern.NoFill)
			{
				cfRuleRecord.IsPatternStyleModified = true;
			}
		}
	}

	public HSSFPatternFormatting(CFRuleBase cfRuleRecord, HSSFWorkbook workbook)
	{
		this.workbook = workbook;
		this.cfRuleRecord = cfRuleRecord;
		patternFormatting = cfRuleRecord.PatternFormatting;
	}
}

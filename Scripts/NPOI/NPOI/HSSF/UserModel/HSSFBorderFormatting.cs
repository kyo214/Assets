using NPOI.HSSF.Record;
using NPOI.HSSF.Record.CF;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

public class HSSFBorderFormatting : IBorderFormatting
{
	private HSSFWorkbook workbook;

	private CFRuleBase cfRuleRecord;

	private BorderFormatting borderFormatting;

	public BorderStyle BorderBottom
	{
		get
		{
			return borderFormatting.BorderBottom;
		}
		set
		{
			borderFormatting.BorderBottom = value;
			if (value != BorderStyle.None)
			{
				cfRuleRecord.IsBottomBorderModified = true;
			}
			else
			{
				cfRuleRecord.IsBottomBorderModified = false;
			}
		}
	}

	public BorderStyle BorderDiagonal
	{
		get
		{
			return borderFormatting.BorderDiagonal;
		}
		set
		{
			borderFormatting.BorderDiagonal = value;
			if (value != BorderStyle.None)
			{
				cfRuleRecord.IsBottomLeftTopRightBorderModified = true;
				cfRuleRecord.IsTopLeftBottomRightBorderModified = true;
			}
			else
			{
				cfRuleRecord.IsBottomLeftTopRightBorderModified = false;
				cfRuleRecord.IsTopLeftBottomRightBorderModified = false;
			}
		}
	}

	public BorderStyle BorderLeft
	{
		get
		{
			return borderFormatting.BorderLeft;
		}
		set
		{
			borderFormatting.BorderLeft = value;
			if (value != BorderStyle.None)
			{
				cfRuleRecord.IsLeftBorderModified = true;
			}
			else
			{
				cfRuleRecord.IsLeftBorderModified = false;
			}
		}
	}

	public BorderStyle BorderRight
	{
		get
		{
			return borderFormatting.BorderRight;
		}
		set
		{
			borderFormatting.BorderRight = value;
			if (value != BorderStyle.None)
			{
				cfRuleRecord.IsRightBorderModified = true;
			}
			else
			{
				cfRuleRecord.IsRightBorderModified = false;
			}
		}
	}

	public BorderStyle BorderTop
	{
		get
		{
			return borderFormatting.BorderTop;
		}
		set
		{
			borderFormatting.BorderTop = value;
			if (value != BorderStyle.None)
			{
				cfRuleRecord.IsTopBorderModified = true;
			}
			else
			{
				cfRuleRecord.IsTopBorderModified = false;
			}
		}
	}

	public short BottomBorderColor
	{
		get
		{
			return borderFormatting.BottomBorderColor;
		}
		set
		{
			borderFormatting.BottomBorderColor = value;
			if (value != 0)
			{
				cfRuleRecord.IsBottomBorderModified = true;
			}
			else
			{
				cfRuleRecord.IsBottomBorderModified = false;
			}
		}
	}

	public IColor BottomBorderColorColor
	{
		get
		{
			return workbook.GetCustomPalette().GetColor(borderFormatting.BottomBorderColor);
		}
		set
		{
			HSSFColor hSSFColor = HSSFColor.ToHSSFColor(value);
			if (hSSFColor == null)
			{
				BottomBorderColor = 0;
			}
			else
			{
				BottomBorderColor = hSSFColor.Indexed;
			}
		}
	}

	public short DiagonalBorderColor
	{
		get
		{
			return borderFormatting.DiagonalBorderColor;
		}
		set
		{
			borderFormatting.DiagonalBorderColor = value;
			if (value != 0)
			{
				cfRuleRecord.IsBottomLeftTopRightBorderModified = true;
				cfRuleRecord.IsTopLeftBottomRightBorderModified = true;
			}
			else
			{
				cfRuleRecord.IsBottomLeftTopRightBorderModified = false;
				cfRuleRecord.IsTopLeftBottomRightBorderModified = false;
			}
		}
	}

	public IColor DiagonalBorderColorColor
	{
		get
		{
			return workbook.GetCustomPalette().GetColor(borderFormatting.DiagonalBorderColor);
		}
		set
		{
			HSSFColor hSSFColor = HSSFColor.ToHSSFColor(value);
			if (hSSFColor == null)
			{
				DiagonalBorderColor = 0;
			}
			else
			{
				DiagonalBorderColor = hSSFColor.Indexed;
			}
		}
	}

	public short LeftBorderColor
	{
		get
		{
			return borderFormatting.LeftBorderColor;
		}
		set
		{
			borderFormatting.LeftBorderColor = value;
			if (value != 0)
			{
				cfRuleRecord.IsLeftBorderModified = true;
			}
			else
			{
				cfRuleRecord.IsLeftBorderModified = false;
			}
		}
	}

	public IColor LeftBorderColorColor
	{
		get
		{
			return workbook.GetCustomPalette().GetColor(borderFormatting.LeftBorderColor);
		}
		set
		{
			HSSFColor hSSFColor = HSSFColor.ToHSSFColor(value);
			if (hSSFColor == null)
			{
				LeftBorderColor = 0;
			}
			else
			{
				LeftBorderColor = hSSFColor.Indexed;
			}
		}
	}

	public short RightBorderColor
	{
		get
		{
			return borderFormatting.RightBorderColor;
		}
		set
		{
			borderFormatting.RightBorderColor = value;
			if (value != 0)
			{
				cfRuleRecord.IsRightBorderModified = true;
			}
			else
			{
				cfRuleRecord.IsRightBorderModified = false;
			}
		}
	}

	public IColor RightBorderColorColor
	{
		get
		{
			return workbook.GetCustomPalette().GetColor(borderFormatting.RightBorderColor);
		}
		set
		{
			HSSFColor hSSFColor = HSSFColor.ToHSSFColor(value);
			if (hSSFColor == null)
			{
				RightBorderColor = 0;
			}
			else
			{
				RightBorderColor = hSSFColor.Indexed;
			}
		}
	}

	public short TopBorderColor
	{
		get
		{
			return borderFormatting.TopBorderColor;
		}
		set
		{
			borderFormatting.TopBorderColor = value;
			if (value != 0)
			{
				cfRuleRecord.IsTopBorderModified = true;
			}
			else
			{
				cfRuleRecord.IsTopBorderModified = false;
			}
		}
	}

	public IColor TopBorderColorColor
	{
		get
		{
			return workbook.GetCustomPalette().GetColor(borderFormatting.TopBorderColor);
		}
		set
		{
			HSSFColor hSSFColor = HSSFColor.ToHSSFColor(value);
			if (hSSFColor == null)
			{
				TopBorderColor = 0;
			}
			else
			{
				TopBorderColor = hSSFColor.Indexed;
			}
		}
	}

	public bool IsBackwardDiagonalOn
	{
		get
		{
			return borderFormatting.IsBackwardDiagonalOn;
		}
		set
		{
			borderFormatting.IsBackwardDiagonalOn = value;
			if (value)
			{
				cfRuleRecord.IsTopLeftBottomRightBorderModified = value;
			}
		}
	}

	public bool IsForwardDiagonalOn
	{
		get
		{
			return borderFormatting.IsForwardDiagonalOn;
		}
		set
		{
			borderFormatting.IsForwardDiagonalOn = value;
			if (value)
			{
				cfRuleRecord.IsBottomLeftTopRightBorderModified = value;
			}
		}
	}

	public HSSFBorderFormatting(CFRuleBase cfRuleRecord, HSSFWorkbook workbook)
	{
		this.workbook = workbook;
		this.cfRuleRecord = cfRuleRecord;
		borderFormatting = cfRuleRecord.BorderFormatting;
	}

	public BorderFormatting GetBorderFormattingBlock()
	{
		return borderFormatting;
	}
}

using NPOI.HSSF.Record;
using NPOI.HSSF.Record.CF;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

public class HSSFFontFormatting : IFontFormatting
{
	private FontFormatting fontFormatting;

	private HSSFWorkbook workbook;

	public FontSuperScript EscapementType
	{
		get
		{
			return fontFormatting.EscapementType;
		}
		set
		{
			switch (value)
			{
			case FontSuperScript.Super:
			case FontSuperScript.Sub:
				fontFormatting.EscapementType = value;
				fontFormatting.IsEscapementTypeModified = true;
				break;
			case FontSuperScript.None:
				fontFormatting.EscapementType = value;
				fontFormatting.IsEscapementTypeModified = false;
				break;
			}
		}
	}

	public short FontColorIndex
	{
		get
		{
			return fontFormatting.FontColorIndex;
		}
		set
		{
			fontFormatting.FontColorIndex = value;
		}
	}

	public IColor FontColor
	{
		get
		{
			return workbook.GetCustomPalette().GetColor(FontColorIndex);
		}
		set
		{
			HSSFColor hSSFColor = HSSFColor.ToHSSFColor(value);
			if (hSSFColor == null)
			{
				fontFormatting.FontColorIndex = 0;
			}
			else
			{
				fontFormatting.FontColorIndex = hSSFColor.Indexed;
			}
		}
	}

	public int FontHeight
	{
		get
		{
			return fontFormatting.FontHeight;
		}
		set
		{
			fontFormatting.FontHeight = value;
		}
	}

	public short FontWeight => fontFormatting.FontWeight;

	public FontUnderlineType UnderlineType
	{
		get
		{
			return fontFormatting.UnderlineType;
		}
		set
		{
			switch (value)
			{
			case FontUnderlineType.Single:
			case FontUnderlineType.Double:
			case FontUnderlineType.SingleAccounting:
			case FontUnderlineType.DoubleAccounting:
				fontFormatting.UnderlineType = value;
				IsUnderlineTypeModified = true;
				break;
			case FontUnderlineType.None:
				fontFormatting.UnderlineType = value;
				IsUnderlineTypeModified = false;
				break;
			}
		}
	}

	public bool IsBold
	{
		get
		{
			if (fontFormatting.IsFontWeightModified)
			{
				return fontFormatting.IsBold;
			}
			return false;
		}
	}

	public bool IsEscapementTypeModified
	{
		get
		{
			return fontFormatting.IsEscapementTypeModified;
		}
		set
		{
			fontFormatting.IsEscapementTypeModified = value;
		}
	}

	public bool IsFontCancellationModified
	{
		get
		{
			return fontFormatting.IsFontCancellationModified;
		}
		set
		{
			fontFormatting.IsFontCancellationModified = value;
		}
	}

	public bool IsFontOutlineModified
	{
		get
		{
			return fontFormatting.IsFontOutlineModified;
		}
		set
		{
			fontFormatting.IsFontOutlineModified = value;
		}
	}

	public bool IsFontShadowModified
	{
		get
		{
			return fontFormatting.IsFontShadowModified;
		}
		set
		{
			fontFormatting.IsFontShadowModified = value;
		}
	}

	public bool IsFontStyleModified
	{
		get
		{
			return fontFormatting.IsFontStyleModified;
		}
		set
		{
			fontFormatting.IsFontStyleModified = value;
		}
	}

	public bool IsItalic
	{
		get
		{
			if (fontFormatting.IsFontStyleModified)
			{
				return fontFormatting.IsItalic;
			}
			return false;
		}
	}

	public bool IsOutlineOn
	{
		get
		{
			if (fontFormatting.IsFontOutlineModified)
			{
				return fontFormatting.IsOutlineOn;
			}
			return false;
		}
		set
		{
			fontFormatting.IsOutlineOn = value;
			fontFormatting.IsFontOutlineModified = value;
		}
	}

	public bool IsShadowOn
	{
		get
		{
			if (fontFormatting.IsFontOutlineModified)
			{
				return fontFormatting.IsShadowOn;
			}
			return false;
		}
		set
		{
			fontFormatting.IsShadowOn = value;
			fontFormatting.IsFontShadowModified = value;
		}
	}

	public bool IsStrikeout
	{
		get
		{
			if (fontFormatting.IsFontCancellationModified)
			{
				return fontFormatting.IsStruckout;
			}
			return false;
		}
		set
		{
			fontFormatting.IsStruckout = value;
			fontFormatting.IsFontCancellationModified = value;
		}
	}

	public bool IsUnderlineTypeModified
	{
		get
		{
			return fontFormatting.IsUnderlineTypeModified;
		}
		set
		{
			fontFormatting.IsUnderlineTypeModified = value;
		}
	}

	public bool IsFontWeightModified => fontFormatting.IsFontWeightModified;

	public HSSFFontFormatting(CFRuleBase cfRuleRecord, HSSFWorkbook workbook)
	{
		fontFormatting = cfRuleRecord.FontFormatting;
		this.workbook = workbook;
	}

	protected FontFormatting GetFontFormattingBlock()
	{
		return fontFormatting;
	}

	protected byte[] GetRawRecord()
	{
		return fontFormatting.RawRecord;
	}

	public void SetFontStyle(bool italic, bool bold)
	{
		bool flag = italic | bold;
		fontFormatting.IsItalic = italic;
		fontFormatting.IsBold = bold;
		fontFormatting.IsFontStyleModified = flag;
		fontFormatting.IsFontWeightModified = flag;
	}

	public void ResetFontStyle()
	{
		SetFontStyle(italic: false, bold: false);
	}
}

using System;
using NPOI.HSSF.Record;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

public class HSSFFont : IFont
{
	public const string FONT_ARIAL = "Arial";

	private FontRecord font;

	private short index;

	public string FontName
	{
		get
		{
			return font.FontName;
		}
		set
		{
			font.FontName = value;
		}
	}

	public short Index => index;

	public double FontHeight
	{
		get
		{
			return font.FontHeight;
		}
		set
		{
			font.FontHeight = (short)value;
		}
	}

	public double FontHeightInPoints
	{
		get
		{
			return (double)font.FontHeight / 20.0;
		}
		set
		{
			font.FontHeight = (short)(value * 20.0);
		}
	}

	public bool IsItalic
	{
		get
		{
			return font.IsItalic;
		}
		set
		{
			font.IsItalic = value;
		}
	}

	public bool IsStrikeout
	{
		get
		{
			return font.IsStrikeout;
		}
		set
		{
			font.IsStrikeout = value;
		}
	}

	public short Color
	{
		get
		{
			return font.ColorPaletteIndex;
		}
		set
		{
			font.ColorPaletteIndex = value;
		}
	}

	[Obsolete("deprecated POI 3.15 beta 2. Use IsBold instead.")]
	public short Boldweight
	{
		get
		{
			return font.BoldWeight;
		}
		set
		{
			font.BoldWeight = value;
		}
	}

	public bool IsBold
	{
		get
		{
			return font.BoldWeight == 700;
		}
		set
		{
			if (value)
			{
				font.BoldWeight = 700;
			}
			else
			{
				font.BoldWeight = 400;
			}
		}
	}

	public FontSuperScript TypeOffset
	{
		get
		{
			return font.SuperSubScript;
		}
		set
		{
			font.SuperSubScript = value;
		}
	}

	public FontUnderlineType Underline
	{
		get
		{
			return font.Underline;
		}
		set
		{
			font.Underline = value;
		}
	}

	public short Charset
	{
		get
		{
			return font.Charset;
		}
		set
		{
			font.Charset = (byte)value;
		}
	}

	public HSSFFont(short index, FontRecord rec)
	{
		font = rec;
		this.index = index;
	}

	public HSSFColor GetHSSFColor(HSSFWorkbook wb)
	{
		return wb.GetCustomPalette().GetColor(Color);
	}

	public override string ToString()
	{
		return "NPOI.HSSF.UserModel.HSSFFont{" + font?.ToString() + "}";
	}

	public override int GetHashCode()
	{
		int num = 1;
		num = 31 * num + ((font != null) ? font.GetHashCode() : 0);
		return 31 * num + index;
	}

	public override bool Equals(object obj)
	{
		if (this == obj)
		{
			return true;
		}
		if (obj == null)
		{
			return false;
		}
		if (obj is HSSFFont)
		{
			HSSFFont hSSFFont = (HSSFFont)obj;
			if (font == null)
			{
				if (hSSFFont.font != null)
				{
					return false;
				}
			}
			else if (!font.Equals(hSSFFont.font))
			{
				return false;
			}
			if (index != hSSFFont.index)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public void CloneStyleFrom(IFont src)
	{
		FontName = src.FontName;
		FontHeight = src.FontHeight;
		IsBold = src.IsBold;
		Boldweight = src.Boldweight;
		IsItalic = src.IsItalic;
		IsStrikeout = src.IsStrikeout;
		Color = src.Color;
		Underline = src.Underline;
		Charset = src.Charset;
		TypeOffset = src.TypeOffset;
	}
}

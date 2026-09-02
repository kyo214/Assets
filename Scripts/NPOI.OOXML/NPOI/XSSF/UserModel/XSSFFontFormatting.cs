using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;

namespace NPOI.XSSF.UserModel;

public class XSSFFontFormatting : IFontFormatting
{
	private CT_Font _font;

	public FontSuperScript EscapementType
	{
		get
		{
			if (_font.sizeOfVertAlignArray() == 0)
			{
				return FontSuperScript.None;
			}
			return (FontSuperScript)(_font.GetVertAlignArray(0).val - 1);
		}
		set
		{
			_font.SetVertAlignArray(null);
			if (value != FontSuperScript.None)
			{
				_font.AddNewVertAlign().val = (ST_VerticalAlignRun)(value + 1);
			}
		}
	}

	public short FontColorIndex
	{
		get
		{
			if (_font.sizeOfColorArray() == 0)
			{
				return -1;
			}
			int num = 0;
			CT_Color colorArray = _font.GetColorArray(0);
			if (colorArray.IsSetIndexed())
			{
				num = (int)colorArray.indexed;
			}
			return (short)num;
		}
		set
		{
			_font.SetColorArray(null);
			if (value != -1)
			{
				CT_Color cT_Color = _font.AddNewColor();
				cT_Color.indexed = (uint)value;
				cT_Color.indexedSpecified = true;
			}
		}
	}

	public IColor FontColor
	{
		get
		{
			if (_font.sizeOfColorArray() == 0)
			{
				return null;
			}
			return new XSSFColor(_font.GetColorArray(0));
		}
		set
		{
			XSSFColor xSSFColor = XSSFColor.ToXSSFColor(value);
			if (xSSFColor == null)
			{
				_font.color.Clear();
			}
			else
			{
				_font.SetColorArray(0, xSSFColor.GetCTColor());
			}
		}
	}

	public int FontHeight
	{
		get
		{
			if (_font.sizeOfSzArray() == 0)
			{
				return -1;
			}
			CT_FontSize szArray = _font.GetSzArray(0);
			return (short)(20.0 * szArray.val);
		}
		set
		{
			_font.SetSzArray(null);
			if (value != -1)
			{
				_font.AddNewSz().val = (double)value / 20.0;
			}
		}
	}

	public FontUnderlineType UnderlineType
	{
		get
		{
			if (_font.sizeOfUArray() == 0)
			{
				return FontUnderlineType.None;
			}
			return _font.GetUArray(0).val switch
			{
				ST_UnderlineValues.single => FontUnderlineType.Single, 
				ST_UnderlineValues.@double => FontUnderlineType.Double, 
				ST_UnderlineValues.singleAccounting => FontUnderlineType.SingleAccounting, 
				ST_UnderlineValues.doubleAccounting => FontUnderlineType.DoubleAccounting, 
				_ => FontUnderlineType.None, 
			};
		}
		set
		{
			_font.SetUArray(null);
			if (value != FontUnderlineType.None)
			{
				ST_UnderlineValues value2 = (ST_UnderlineValues)FontUnderline.ValueOf(value).Value;
				_font.AddNewU().val = value2;
			}
		}
	}

	public bool IsBold
	{
		get
		{
			if (_font.SizeOfBArray() == 1)
			{
				return _font.GetBArray(0).val;
			}
			return false;
		}
	}

	public bool IsItalic
	{
		get
		{
			if (_font.sizeOfIArray() == 1)
			{
				return _font.GetIArray(0).val;
			}
			return false;
		}
	}

	internal XSSFFontFormatting(CT_Font font)
	{
		_font = font;
	}

	public void SetFontStyle(bool italic, bool bold)
	{
		_font.SetIArray(null);
		_font.SetBArray(null);
		if (italic)
		{
			_font.AddNewI().val = true;
		}
		if (bold)
		{
			_font.AddNewB().val = true;
		}
	}

	public void ResetFontStyle()
	{
		_font = new CT_Font();
	}
}

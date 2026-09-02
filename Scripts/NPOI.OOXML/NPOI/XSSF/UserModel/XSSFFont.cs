using System;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.XSSF.Model;

namespace NPOI.XSSF.UserModel;

public class XSSFFont : IFont
{
	public const string DEFAULT_FONT_NAME = "Calibri";

	public const short DEFAULT_FONT_SIZE = 11;

	public static short DEFAULT_FONT_COLOR = IndexedColors.Black.Index;

	private ThemesTable _themes;

	private CT_Font _ctFont;

	private short _index;

	public bool IsBold
	{
		get
		{
			return ((_ctFont.SizeOfBArray() == 0) ? null : _ctFont.GetBArray(0))?.val ?? false;
		}
		set
		{
			if (value)
			{
				((_ctFont.SizeOfBArray() == 0) ? _ctFont.AddNewB() : _ctFont.GetBArray(0)).val = value;
			}
			else
			{
				_ctFont.SetBArray(null);
			}
		}
	}

	public short Charset
	{
		get
		{
			CT_IntProperty cT_IntProperty = ((_ctFont.sizeOfCharsetArray() == 0) ? null : _ctFont.GetCharsetArray(0));
			return (short)((cT_IntProperty == null) ? FontCharset.ANSI.Value : FontCharset.ValueOf(cT_IntProperty.val).Value);
		}
		set
		{
		}
	}

	public short Color
	{
		get
		{
			CT_Color cT_Color = ((_ctFont.sizeOfColorArray() == 0) ? null : _ctFont.GetColorArray(0));
			if (cT_Color == null)
			{
				return IndexedColors.Black.Index;
			}
			long num = cT_Color.indexed;
			if (num == DEFAULT_FONT_COLOR)
			{
				return IndexedColors.Black.Index;
			}
			if (num == IndexedColors.Red.Index)
			{
				return IndexedColors.Red.Index;
			}
			return (short)num;
		}
		set
		{
			CT_Color cT_Color = ((_ctFont.sizeOfColorArray() == 0) ? _ctFont.AddNewColor() : _ctFont.GetColorArray(0));
			switch (value)
			{
			case short.MaxValue:
				cT_Color.indexed = (uint)DEFAULT_FONT_COLOR;
				cT_Color.indexedSpecified = true;
				break;
			case 10:
				cT_Color.indexed = (uint)IndexedColors.Red.Index;
				cT_Color.indexedSpecified = true;
				break;
			default:
				cT_Color.indexed = (uint)value;
				cT_Color.indexedSpecified = true;
				break;
			}
		}
	}

	public double FontHeight
	{
		get
		{
			return FontHeightRaw * 20.0;
		}
		set
		{
			FontHeightRaw = value / 20.0;
		}
	}

	public double FontHeightInPoints
	{
		get
		{
			return FontHeightRaw;
		}
		set
		{
			FontHeightRaw = value;
		}
	}

	private double FontHeightRaw
	{
		get
		{
			return ((_ctFont.sizeOfSzArray() == 0) ? null : _ctFont.GetSzArray(0))?.val ?? 11.0;
		}
		set
		{
			((_ctFont.sizeOfSzArray() == 0) ? _ctFont.AddNewSz() : _ctFont.GetSzArray(0)).val = value;
		}
	}

	public string FontName
	{
		get
		{
			CT_FontName name = _ctFont.name;
			if (name != null)
			{
				return name.val;
			}
			return "Calibri";
		}
		set
		{
			((_ctFont.name == null) ? _ctFont.AddNewName() : _ctFont.name).val = ((value == null) ? "Calibri" : value);
		}
	}

	public bool IsItalic
	{
		get
		{
			return ((_ctFont.sizeOfIArray() == 0) ? null : _ctFont.GetIArray(0))?.val ?? false;
		}
		set
		{
			if (value)
			{
				((_ctFont.sizeOfIArray() == 0) ? _ctFont.AddNewI() : _ctFont.GetIArray(0)).val = value;
			}
			else
			{
				_ctFont.SetIArray(null);
			}
		}
	}

	public bool IsStrikeout
	{
		get
		{
			return ((_ctFont.sizeOfStrikeArray() == 0) ? null : _ctFont.GetStrikeArray(0))?.val ?? false;
		}
		set
		{
			if (!value)
			{
				_ctFont.SetStrikeArray(null);
			}
			else
			{
				((_ctFont.sizeOfStrikeArray() == 0) ? _ctFont.AddNewStrike() : _ctFont.GetStrikeArray(0)).val = value;
			}
		}
	}

	public FontSuperScript TypeOffset
	{
		get
		{
			CT_VerticalAlignFontProperty cT_VerticalAlignFontProperty = ((_ctFont.sizeOfVertAlignArray() == 0) ? null : _ctFont.GetVertAlignArray(0));
			if (cT_VerticalAlignFontProperty == null)
			{
				return FontSuperScript.None;
			}
			ST_VerticalAlignRun val = cT_VerticalAlignFontProperty.val;
			return val switch
			{
				ST_VerticalAlignRun.baseline => FontSuperScript.None, 
				ST_VerticalAlignRun.subscript => FontSuperScript.Sub, 
				ST_VerticalAlignRun.superscript => FontSuperScript.Super, 
				_ => throw new POIXMLException("Wrong offset value " + val), 
			};
		}
		set
		{
			if (value == FontSuperScript.None)
			{
				_ctFont.SetVertAlignArray(null);
				return;
			}
			CT_VerticalAlignFontProperty cT_VerticalAlignFontProperty = ((_ctFont.sizeOfVertAlignArray() == 0) ? _ctFont.AddNewVertAlign() : _ctFont.GetVertAlignArray(0));
			switch (value)
			{
			case FontSuperScript.None:
				cT_VerticalAlignFontProperty.val = ST_VerticalAlignRun.baseline;
				break;
			case FontSuperScript.Sub:
				cT_VerticalAlignFontProperty.val = ST_VerticalAlignRun.subscript;
				break;
			case FontSuperScript.Super:
				cT_VerticalAlignFontProperty.val = ST_VerticalAlignRun.superscript;
				break;
			default:
				throw new InvalidOperationException("Invalid type offset: " + value);
			}
		}
	}

	public FontUnderlineType Underline
	{
		get
		{
			CT_UnderlineProperty cT_UnderlineProperty = ((_ctFont.sizeOfUArray() == 0) ? null : _ctFont.GetUArray(0));
			if (cT_UnderlineProperty != null)
			{
				return (FontUnderlineType)FontUnderline.ValueOf((int)cT_UnderlineProperty.val).ByteValue;
			}
			return (FontUnderlineType)FontUnderline.NONE.ByteValue;
		}
		set
		{
			SetUnderline(value);
		}
	}

	[Obsolete("deprecated POI 3.15 beta 2. Use IsBold instead.")]
	public short Boldweight
	{
		get
		{
			if (!IsBold)
			{
				return 400;
			}
			return 700;
		}
		set
		{
			IsBold = value == 700;
		}
	}

	public int Family
	{
		get
		{
			CT_IntProperty cT_IntProperty = ((_ctFont.sizeOfFamilyArray() == 0) ? _ctFont.AddNewFamily() : _ctFont.GetFamilyArray(0));
			if (cT_IntProperty != null)
			{
				return FontFamily.ValueOf(cT_IntProperty.val).Value;
			}
			return FontFamily.NOT_APPLICABLE.Value;
		}
		set
		{
			((_ctFont.sizeOfFamilyArray() == 0) ? _ctFont.AddNewFamily() : _ctFont.GetFamilyArray(0)).val = value;
		}
	}

	public short Index => _index;

	public XSSFFont(CT_Font font)
	{
		_ctFont = font;
		_index = 0;
	}

	public XSSFFont(CT_Font font, int index)
	{
		_ctFont = font;
		_index = (short)index;
	}

	public XSSFFont()
	{
		_ctFont = new CT_Font();
		FontName = "Calibri";
		FontHeightInPoints = 11.0;
	}

	public CT_Font GetCTFont()
	{
		return _ctFont;
	}

	public XSSFColor GetXSSFColor()
	{
		CT_Color cT_Color = ((_ctFont.sizeOfColorArray() == 0) ? null : _ctFont.GetColorArray(0));
		if (cT_Color != null)
		{
			XSSFColor xSSFColor = new XSSFColor(cT_Color);
			if (_themes != null)
			{
				_themes.InheritFromThemeAsRequired(xSSFColor);
			}
			return xSSFColor;
		}
		return null;
	}

	public short GetThemeColor()
	{
		CT_Color cT_Color = ((_ctFont.sizeOfColorArray() == 0) ? null : _ctFont.GetColorArray(0));
		return (short)((cT_Color != null && cT_Color.themeSpecified) ? cT_Color.theme : 0);
	}

	public void SetCharSet(byte charset)
	{
		int charSet = charset & 0xFF;
		SetCharSet(charSet);
	}

	public void SetCharSet(int charset)
	{
		FontCharset fontCharset = FontCharset.ValueOf(charset);
		if (fontCharset != null)
		{
			SetCharSet(fontCharset);
			return;
		}
		throw new POIXMLException("Attention: An attempt was made to set an unknown character set");
	}

	public void SetCharSet(FontCharset charset)
	{
		CT_IntProperty cT_IntProperty = ((_ctFont.sizeOfCharsetArray() != 0) ? _ctFont.GetCharsetArray(0) : _ctFont.AddNewCharset());
		cT_IntProperty.val = charset.Value;
	}

	public void SetColor(XSSFColor color)
	{
		if (color == null)
		{
			_ctFont.SetColorArray(null);
			return;
		}
		CT_Color cT_Color = ((_ctFont.sizeOfColorArray() == 0) ? _ctFont.AddNewColor() : _ctFont.GetColorArray(0));
		if (cT_Color.IsSetIndexed())
		{
			cT_Color.UnsetIndexed();
		}
		cT_Color.SetRgb(color.RGB);
	}

	public void SetThemeColor(short theme)
	{
		((_ctFont.sizeOfColorArray() == 0) ? _ctFont.AddNewColor() : _ctFont.GetColorArray(0)).theme = (uint)theme;
	}

	internal void SetUnderline(FontUnderlineType underline)
	{
		if (underline == FontUnderlineType.None)
		{
			_ctFont.SetUArray(null);
			return;
		}
		CT_UnderlineProperty obj = ((_ctFont.sizeOfUArray() == 0) ? _ctFont.AddNewU() : _ctFont.GetUArray(0));
		ST_UnderlineValues value = (ST_UnderlineValues)FontUnderline.ValueOf(underline).Value;
		obj.val = value;
	}

	public override string ToString()
	{
		return _ctFont.ToString();
	}

	public long RegisterTo(StylesTable styles)
	{
		_themes = styles.GetTheme();
		return _index = (short)styles.PutFont(this, forceRegistration: true);
	}

	public void SetThemesTable(ThemesTable themes)
	{
		_themes = themes;
	}

	public FontScheme GetScheme()
	{
		CT_FontScheme cT_FontScheme = ((_ctFont.sizeOfSchemeArray() == 0) ? null : _ctFont.GetSchemeArray(0));
		if (cT_FontScheme != null)
		{
			return FontScheme.ValueOf((int)cT_FontScheme.val);
		}
		return FontScheme.NONE;
	}

	public void SetScheme(FontScheme scheme)
	{
		CT_FontScheme obj = ((_ctFont.sizeOfSchemeArray() == 0) ? _ctFont.AddNewScheme() : _ctFont.GetSchemeArray(0));
		ST_FontScheme value = (ST_FontScheme)scheme.Value;
		obj.val = value;
	}

	public void SetFamily(FontFamily family)
	{
		Family = family.Value;
	}

	public override int GetHashCode()
	{
		return _ctFont.ToString().GetHashCode();
	}

	public override bool Equals(object o)
	{
		if (!(o is XSSFFont))
		{
			return false;
		}
		XSSFFont xSSFFont = (XSSFFont)o;
		return _ctFont.ToString().Equals(xSSFFont.GetCTFont().ToString());
	}

	public void CloneStyleFrom(IFont src)
	{
		if (src != null)
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
}

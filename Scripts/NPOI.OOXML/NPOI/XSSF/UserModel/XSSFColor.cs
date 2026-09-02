using System;
using System.Drawing;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.Util;

namespace NPOI.XSSF.UserModel;

public class XSSFColor : ExtendedColor
{
	private CT_Color ctColor;

	public override bool IsAuto
	{
		get
		{
			return ctColor.auto;
		}
		set
		{
			ctColor.auto = value;
			ctColor.autoSpecified = true;
		}
	}

	public override bool IsIndexed => ctColor.IsSetIndexed();

	public override bool IsRGB => ctColor.IsSetRgb();

	public override bool IsThemed => ctColor.IsSetTheme();

	public bool HasAlpha
	{
		get
		{
			if (!ctColor.IsSetRgb())
			{
				return false;
			}
			return ctColor.rgb.Length == 4;
		}
	}

	public bool HasTint
	{
		get
		{
			if (!ctColor.IsSetTint())
			{
				return false;
			}
			return ctColor.tint != 0.0;
		}
	}

	public override short Index
	{
		get
		{
			if (!ctColor.indexedSpecified)
			{
				return 0;
			}
			return (short)ctColor.indexed;
		}
	}

	public short Indexed
	{
		get
		{
			return Index;
		}
		set
		{
			ctColor.indexed = (uint)value;
			ctColor.indexedSpecified = true;
		}
	}

	protected override byte[] StoredRBG => ctColor.rgb;

	public override byte[] RGB
	{
		get
		{
			byte[] rGBOrARGB = GetRGBOrARGB();
			if (rGBOrARGB == null)
			{
				return null;
			}
			if (rGBOrARGB.Length == 4)
			{
				byte[] array = new byte[3];
				Array.Copy(rGBOrARGB, 1, array, 0, 3);
				return array;
			}
			return rGBOrARGB;
		}
		set
		{
			ctColor.SetRgb(value);
		}
	}

	public override byte[] ARGB
	{
		get
		{
			byte[] rGBOrARGB = GetRGBOrARGB();
			if (rGBOrARGB == null)
			{
				return null;
			}
			if (rGBOrARGB.Length == 3)
			{
				byte[] array = new byte[4] { 255, 0, 0, 0 };
				Array.Copy(rGBOrARGB, 0, array, 1, 3);
				return array;
			}
			return rGBOrARGB;
		}
	}

	public override int Theme
	{
		get
		{
			if (!ctColor.themeSpecified)
			{
				return 0;
			}
			return (int)ctColor.theme;
		}
		set
		{
			ctColor.theme = (uint)value;
		}
	}

	public override double Tint
	{
		get
		{
			return ctColor.tint;
		}
		set
		{
			ctColor.tint = value;
			ctColor.tintSpecified = true;
		}
	}

	public XSSFColor(CT_Color color)
	{
		ctColor = color;
	}

	public XSSFColor()
	{
		ctColor = new CT_Color();
	}

	public XSSFColor(Color clr)
		: this()
	{
		ctColor.SetRgb(clr.R, clr.G, clr.B);
	}

	public XSSFColor(byte[] rgb)
		: this()
	{
		ctColor.SetRgb(rgb);
	}

	public XSSFColor(IndexedColors indexedColor)
		: this()
	{
		ctColor.indexed = (uint)indexedColor.Index;
	}

	[Obsolete("use property RGB")]
	public byte[] GetRgb()
	{
		return RGB;
	}

	[Obsolete("use property ARGB")]
	public byte[] GetARgb()
	{
		return ARGB;
	}

	public byte[] GetRgbWithTint()
	{
		byte[] array = ctColor.GetRgb();
		if (array != null)
		{
			if (array.Length == 4)
			{
				byte[] array2 = new byte[3];
				Array.Copy(array, 1, array2, 0, 3);
				array = array2;
			}
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = ApplyTint(array[i] & 0xFF, ctColor.tint);
			}
		}
		return array;
	}

	private static byte ApplyTint(int lum, double tint)
	{
		if (tint > 0.0)
		{
			return (byte)((double)lum * (1.0 - tint) + (255.0 - 255.0 * (1.0 - tint)));
		}
		if (tint < 0.0)
		{
			return (byte)((double)lum * (1.0 + tint));
		}
		return (byte)lum;
	}

	public void SetRgb(byte[] rgb)
	{
		ctColor.SetRgb(rgb);
	}

	internal CT_Color GetCTColor()
	{
		return ctColor;
	}

	public static XSSFColor ToXSSFColor(IColor color)
	{
		if (color != null && !(color is XSSFColor))
		{
			throw new ArgumentException("Only XSSFColor objects are supported");
		}
		return (XSSFColor)color;
	}

	private bool SameIndexed(XSSFColor other)
	{
		if (IsIndexed == other.IsIndexed)
		{
			if (IsIndexed)
			{
				return Indexed == other.Indexed;
			}
			return true;
		}
		return false;
	}

	private bool SameARGB(XSSFColor other)
	{
		if (IsRGB == other.IsRGB)
		{
			if (IsRGB)
			{
				return Arrays.Equals(ARGB, other.ARGB);
			}
			return true;
		}
		return false;
	}

	private bool SameTheme(XSSFColor other)
	{
		if (IsThemed == other.IsThemed)
		{
			if (IsThemed)
			{
				return Theme == other.Theme;
			}
			return true;
		}
		return false;
	}

	private bool SameTint(XSSFColor other)
	{
		if (HasTint == other.HasTint)
		{
			if (HasTint)
			{
				return Tint == other.Tint;
			}
			return true;
		}
		return false;
	}

	private bool SameAuto(XSSFColor other)
	{
		return IsAuto == other.IsAuto;
	}

	public override int GetHashCode()
	{
		return ctColor.ToString().GetHashCode();
	}

	public override bool Equals(object o)
	{
		if (o == null || !(o is XSSFColor))
		{
			return false;
		}
		XSSFColor other = (XSSFColor)o;
		if (SameARGB(other) && SameTheme(other) && SameIndexed(other) && SameTint(other))
		{
			return SameAuto(other);
		}
		return false;
	}
}

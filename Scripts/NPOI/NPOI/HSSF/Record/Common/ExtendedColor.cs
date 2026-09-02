using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Common;

public class ExtendedColor : ICloneable
{
	public static int TYPE_AUTO = 0;

	public static int TYPE_INDEXED = 1;

	public static int TYPE_RGB = 2;

	public static int TYPE_THEMED = 3;

	public static int TYPE_UNSET = 4;

	public static int THEME_DARK_1 = 0;

	public static int THEME_LIGHT_1 = 1;

	public static int THEME_DARK_2 = 2;

	public static int THEME_LIGHT_2 = 3;

	public static int THEME_ACCENT_1 = 4;

	public static int THEME_ACCENT_2 = 5;

	public static int THEME_ACCENT_3 = 6;

	public static int THEME_ACCENT_4 = 7;

	public static int THEME_ACCENT_5 = 8;

	public static int THEME_ACCENT_6 = 9;

	public static int THEME_HYPERLINK = 10;

	public static int THEME_FOLLOWED_HYPERLINK = 11;

	public int type;

	public int colorIndex;

	public byte[] rgba;

	public int themeIndex;

	private double tint;

	public int Type
	{
		get
		{
			return type;
		}
		set
		{
			type = value;
		}
	}

	public int ColorIndex
	{
		get
		{
			return colorIndex;
		}
		set
		{
			colorIndex = value;
		}
	}

	public byte[] RGBA
	{
		get
		{
			return rgba;
		}
		set
		{
			rgba = ((value == null) ? null : ((byte[])value.Clone()));
		}
	}

	public int ThemeIndex
	{
		get
		{
			return themeIndex;
		}
		set
		{
			themeIndex = value;
		}
	}

	public double Tint
	{
		get
		{
			return tint;
		}
		set
		{
			if (tint < -1.0 || tint > 1.0)
			{
				throw new ArgumentException("Tint/Shade must be between -1 and +1");
			}
			tint = value;
		}
	}

	public int DataLength => 16;

	public ExtendedColor()
	{
		type = TYPE_INDEXED;
		colorIndex = 0;
		tint = 0.0;
	}

	public ExtendedColor(ILittleEndianInput in1)
	{
		type = in1.ReadInt();
		if (type == TYPE_INDEXED)
		{
			colorIndex = in1.ReadInt();
		}
		else if (type == TYPE_RGB)
		{
			rgba = new byte[4];
			in1.ReadFully(rgba);
		}
		else if (type == TYPE_THEMED)
		{
			themeIndex = in1.ReadInt();
		}
		else
		{
			in1.ReadInt();
		}
		tint = in1.ReadDouble();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("    [Extended Color]\n");
		stringBuilder.Append("          .type  = ").Append(type).Append("\n");
		stringBuilder.Append("          .tint  = ").Append(tint).Append("\n");
		stringBuilder.Append("          .c_idx = ").Append(colorIndex).Append("\n");
		stringBuilder.Append("          .rgba  = ").Append(HexDump.ToHex(rgba)).Append("\n");
		stringBuilder.Append("          .t_idx = ").Append(themeIndex).Append("\n");
		stringBuilder.Append("    [/Extended Color]\n");
		return stringBuilder.ToString();
	}

	public object Clone()
	{
		ExtendedColor extendedColor = new ExtendedColor();
		extendedColor.type = type;
		extendedColor.tint = tint;
		if (type == TYPE_INDEXED)
		{
			extendedColor.colorIndex = colorIndex;
		}
		else if (type == TYPE_RGB)
		{
			extendedColor.rgba = new byte[4];
			Array.Copy(rgba, 0, extendedColor.rgba, 0, 4);
		}
		else if (type == TYPE_THEMED)
		{
			extendedColor.themeIndex = themeIndex;
		}
		return extendedColor;
	}

	public void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteInt(type);
		if (type == TYPE_INDEXED)
		{
			out1.WriteInt(colorIndex);
		}
		else if (type == TYPE_RGB)
		{
			out1.Write(rgba);
		}
		else if (type == TYPE_THEMED)
		{
			out1.WriteInt(themeIndex);
		}
		else
		{
			out1.WriteInt(0);
		}
		out1.WriteDouble(tint);
	}
}

using System;
using NPOI.HSSF.Record.Common;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

public class HSSFExtendedColor : NPOI.SS.UserModel.ExtendedColor
{
	private NPOI.HSSF.Record.Common.ExtendedColor color;

	public NPOI.HSSF.Record.Common.ExtendedColor ExtendedColor => color;

	public override bool IsAuto
	{
		get
		{
			return color.type == NPOI.HSSF.Record.Common.ExtendedColor.TYPE_AUTO;
		}
		set
		{
			if (value)
			{
				color.type = NPOI.HSSF.Record.Common.ExtendedColor.TYPE_AUTO;
			}
			else
			{
				color.type = NPOI.HSSF.Record.Common.ExtendedColor.TYPE_UNSET;
			}
		}
	}

	public override bool IsIndexed => color.type == NPOI.HSSF.Record.Common.ExtendedColor.TYPE_INDEXED;

	public override bool IsRGB => color.type == NPOI.HSSF.Record.Common.ExtendedColor.TYPE_RGB;

	public override bool IsThemed => color.type == NPOI.HSSF.Record.Common.ExtendedColor.TYPE_THEMED;

	public override short Index => (short)color.ColorIndex;

	public override int Theme
	{
		get
		{
			return color.ThemeIndex;
		}
		set
		{
			color.ThemeIndex = value;
		}
	}

	public override byte[] RGB
	{
		get
		{
			byte[] array = new byte[3];
			byte[] rGBA = color.RGBA;
			if (rGBA == null)
			{
				return null;
			}
			Array.Copy(rGBA, 0, array, 0, 3);
			return array;
		}
		set
		{
			if (value.Length == 3)
			{
				byte[] array = new byte[4];
				Array.Copy(value, 0, array, 0, 3);
				array[3] = byte.MaxValue;
			}
			else
			{
				byte b = value[0];
				value[0] = value[1];
				value[1] = value[2];
				value[2] = value[3];
				value[3] = b;
				color.RGBA = value;
			}
			color.Type = NPOI.HSSF.Record.Common.ExtendedColor.TYPE_RGB;
		}
	}

	public override byte[] ARGB
	{
		get
		{
			byte[] array = new byte[4];
			byte[] rGBA = color.RGBA;
			if (rGBA == null)
			{
				return null;
			}
			Array.Copy(rGBA, 0, array, 1, 3);
			array[0] = rGBA[3];
			return array;
		}
	}

	protected override byte[] StoredRBG => ARGB;

	public override double Tint
	{
		get
		{
			return color.Tint;
		}
		set
		{
			color.Tint = value;
		}
	}

	public HSSFExtendedColor(NPOI.HSSF.Record.Common.ExtendedColor color)
	{
		this.color = color;
	}

	protected NPOI.HSSF.Record.Common.ExtendedColor GetExtendedColor()
	{
		return color;
	}

	public void SetRGB(byte[] rgb)
	{
	}
}

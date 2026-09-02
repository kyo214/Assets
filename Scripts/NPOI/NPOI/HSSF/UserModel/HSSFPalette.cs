using System;
using System.Text;
using NPOI.HSSF.Record;
using NPOI.HSSF.Util;
using NPOI.Util;

namespace NPOI.HSSF.UserModel;

public class HSSFPalette
{
	private class CustomColor : HSSFColor
	{
		private short byteOffset;

		private byte red;

		private byte green;

		private byte blue;

		public override short Indexed => byteOffset;

		public CustomColor(short byteOffset, byte[] colors)
			: this(byteOffset, colors[0], colors[1], colors[2])
		{
		}

		public CustomColor(short byteOffset, byte red, byte green, byte blue)
		{
			this.byteOffset = byteOffset;
			this.red = red;
			this.green = green;
			this.blue = blue;
		}

		public override byte[] GetTriplet()
		{
			return new byte[3]
			{
				(byte)(red & 0xFF),
				(byte)(green & 0xFF),
				(byte)(blue & 0xFF)
			};
		}

		public override string GetHexString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GetGnumericPart(red));
			stringBuilder.Append(':');
			stringBuilder.Append(GetGnumericPart(green));
			stringBuilder.Append(':');
			stringBuilder.Append(GetGnumericPart(blue));
			return stringBuilder.ToString();
		}

		private string GetGnumericPart(byte color)
		{
			string text;
			if (color == 0)
			{
				text = "0";
			}
			else
			{
				int num = color & 0xFF;
				num = (num << 8) | num;
				text = StringUtil.ToHexString(num).ToUpper();
				while (text.Length < 4)
				{
					text = "0" + text;
				}
			}
			return text;
		}
	}

	private PaletteRecord palette;

	public HSSFPalette(PaletteRecord palette)
	{
		this.palette = palette;
	}

	public HSSFColor GetColor(short index)
	{
		if (index == 64)
		{
			return HSSFColor.Automatic.GetInstance();
		}
		byte[] color = palette.GetColor(index);
		if (color != null)
		{
			return new CustomColor(index, color);
		}
		return null;
	}

	public HSSFColor FindColor(byte red, byte green, byte blue)
	{
		byte[] color = palette.GetColor(8);
		short num = 8;
		while (color != null)
		{
			if (color[0] == red && color[1] == green && color[2] == blue)
			{
				return new CustomColor(num, color);
			}
			color = palette.GetColor(++num);
		}
		return null;
	}

	public HSSFColor FindSimilarColor(byte red, byte green, byte blue)
	{
		HSSFColor result = null;
		int num = int.MaxValue;
		byte[] color = palette.GetColor(8);
		short num2 = 8;
		while (color != null)
		{
			int num3 = Math.Abs(red - color[0]) + Math.Abs(green - color[1]) + Math.Abs(blue - color[2]);
			if (num3 < num)
			{
				num = num3;
				result = GetColor(num2);
			}
			color = palette.GetColor(++num2);
		}
		return result;
	}

	public void SetColorAtIndex(short index, byte red, byte green, byte blue)
	{
		palette.SetColor(index, red, green, blue);
	}

	public HSSFColor AddColor(byte red, byte green, byte blue)
	{
		byte[] color = palette.GetColor(8);
		short num = 8;
		while (num < 64)
		{
			if (color == null)
			{
				SetColorAtIndex(num, red, green, blue);
				return GetColor(num);
			}
			color = palette.GetColor(++num);
		}
		throw new Exception("Could not Find free color index");
	}
}

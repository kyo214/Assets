using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using NPOI.HSSF.Util;

namespace NPOI.SS.UserModel;

public abstract class ExtendedColor : IColor
{
	public abstract bool IsAuto { get; set; }

	public abstract bool IsIndexed { get; }

	public abstract bool IsRGB { get; }

	public abstract bool IsThemed { get; }

	public abstract short Index { get; }

	public abstract int Theme { get; set; }

	public abstract byte[] RGB { get; set; }

	public abstract byte[] ARGB { get; }

	protected abstract byte[] StoredRBG { get; }

	public byte[] RGBWithTint
	{
		get
		{
			byte[] array = StoredRBG;
			if (array != null)
			{
				if (array.Length == 4)
				{
					byte[] array2 = new byte[3];
					Array.Copy(array, 1, array2, 0, 3);
					array = array2;
				}
				double tint = Tint;
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = ApplyTint(array[i] & 0xFF, tint);
				}
			}
			return array;
		}
	}

	public string ARGBHex
	{
		get
		{
			byte[] aRGB = ARGB;
			if (aRGB == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			byte[] array = aRGB;
			for (int i = 0; i < array.Length; i++)
			{
				int num = array[i] & 0xFF;
				string text = $"{num:x}";
				if (text.Length == 1)
				{
					stringBuilder.Append('0');
				}
				stringBuilder.Append(text);
			}
			return stringBuilder.ToString().ToUpper();
		}
		set
		{
			if (value.Length == 6 || value.Length == 8)
			{
				byte[] array = new byte[value.Length / 2];
				for (int i = 0; i < array.Length; i++)
				{
					string s = value.Substring(i * 2, (i + 1) * 2 - i * 2);
					array[i] = (byte)int.Parse(s, NumberStyles.HexNumber);
				}
				RGB = array;
				return;
			}
			throw new ArgumentException("Must be of the form 112233 or FFEEDDCC");
		}
	}

	public abstract double Tint { get; set; }

	protected void SetColor(Color clr)
	{
		RGB = new byte[3] { clr.R, clr.G, clr.B };
	}

	protected byte[] GetRGBOrARGB()
	{
		if (IsIndexed && Index > 0)
		{
			int index = Index;
			Dictionary<int, HSSFColor> indexHash = HSSFColor.GetIndexHash();
			HSSFColor hSSFColor = null;
			if (indexHash.ContainsKey(index))
			{
				hSSFColor = indexHash[index];
			}
			if (hSSFColor != null)
			{
				return new byte[3]
				{
					hSSFColor.GetTriplet()[0],
					hSSFColor.GetTriplet()[1],
					hSSFColor.GetTriplet()[2]
				};
			}
		}
		return StoredRBG;
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
}

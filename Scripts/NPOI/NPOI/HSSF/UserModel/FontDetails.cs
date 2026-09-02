using System;
using System.Collections;
using System.Globalization;
using NPOI.Util.Collections;

namespace NPOI.HSSF.UserModel;

public class FontDetails
{
	private string fontName;

	private int height;

	private Hashtable charWidths = new Hashtable();

	public FontDetails(string fontName, int height)
	{
		this.fontName = fontName;
		this.height = height;
	}

	public string GetFontName()
	{
		return fontName;
	}

	public int GetHeight()
	{
		return height;
	}

	public void AddChar(char c, int width)
	{
		charWidths[c] = width;
	}

	public int GetCharWidth(char c)
	{
		object obj = charWidths[c];
		if (obj == null)
		{
			if ('W' != c)
			{
				return GetCharWidth('W');
			}
			return 0;
		}
		return (int)obj;
	}

	public void AddChars(char[] Chars, int[] widths)
	{
		for (int i = 0; i < Chars.Length; i++)
		{
			if (Chars[i] != ' ')
			{
				charWidths[Chars[i]] = widths[i];
			}
		}
	}

	public static string BuildFontHeightProperty(string fontName)
	{
		return "font." + fontName + ".height";
	}

	public static string BuildFontWidthsProperty(string fontName)
	{
		return "font." + fontName + ".widths";
	}

	public static string BuildFontCharsProperty(string fontName)
	{
		return "font." + fontName + ".characters";
	}

	public static FontDetails Create(string fontName, Properties fontMetricsProps)
	{
		string text = fontMetricsProps[BuildFontHeightProperty(fontName)];
		string text2 = fontMetricsProps[BuildFontWidthsProperty(fontName)];
		string text3 = fontMetricsProps[BuildFontCharsProperty(fontName)];
		if (text == null || text2 == null || text3 == null)
		{
			throw new ArgumentException("The supplied FontMetrics doesn't know about the font '" + fontName + "', so we can't use it. Please Add it to your font metrics file (see StaticFontMetrics.GetFontDetails");
		}
		int num = int.Parse(text, CultureInfo.InvariantCulture);
		FontDetails fontDetails = new FontDetails(fontName, num);
		string[] array = Split(text3, ",", -1);
		string[] array2 = Split(text2, ",", -1);
		if (array.Length != array2.Length)
		{
			throw new Exception("Number of Chars does not number of widths for font " + fontName);
		}
		for (int i = 0; i < array2.Length; i++)
		{
			if (array[i].Trim().Length != 0)
			{
				fontDetails.AddChar(array[i].Trim()[0], int.Parse(array2[i], CultureInfo.InvariantCulture));
			}
		}
		return fontDetails;
	}

	public int GetStringWidth(string str)
	{
		int num = 0;
		for (int i = 0; i < str.Length; i++)
		{
			num += GetCharWidth(str[i]);
		}
		return num;
	}

	private static string[] Split(string text, string separator, int max)
	{
		return text.Split(separator.ToCharArray());
	}
}

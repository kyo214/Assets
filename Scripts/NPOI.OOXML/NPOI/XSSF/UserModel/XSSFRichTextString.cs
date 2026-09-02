using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.XSSF.Model;

namespace NPOI.XSSF.UserModel;

public class XSSFRichTextString : IRichTextString
{
	private static Regex utfPtrn = new Regex("_x([0-9A-F]{4})_", RegexOptions.Compiled);

	private CT_Rst st;

	private StylesTable styles;

	public string String
	{
		get
		{
			if (st.sizeOfRArray() == 0)
			{
				return UtfDecode(st.t);
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (CT_RElt item in st.r)
			{
				stringBuilder.Append(item.t);
			}
			return UtfDecode(stringBuilder.ToString());
		}
		set
		{
			ClearFormatting();
			st.t = value;
			PreserveSpaces(st.t);
		}
	}

	public int Length => String.Length;

	public int NumFormattingRuns => st.sizeOfRArray();

	public XSSFRichTextString(string str)
	{
		st = new CT_Rst();
		st.t = str;
		PreserveSpaces(st.t);
	}

	public void SetStylesTableReference(StylesTable stylestable)
	{
		styles = stylestable;
		if (st.sizeOfRArray() <= 0)
		{
			return;
		}
		foreach (CT_RElt item in st.r)
		{
			CT_RPrElt rPr = item.rPr;
			if (rPr != null && rPr.SizeOfRFontArray() > 0)
			{
				string val = rPr.GetRFontArray(0).val;
				if (val.StartsWith("#"))
				{
					int idx = int.Parse(val.Substring(1));
					XSSFFont fontAt = styles.GetFontAt(idx);
					rPr.rFont = null;
					SetRunAttributes(fontAt.GetCTFont(), rPr);
				}
			}
		}
	}

	public XSSFRichTextString()
	{
		st = new CT_Rst();
	}

	public XSSFRichTextString(CT_Rst st)
	{
		this.st = st;
	}

	public void ApplyFont(int startIndex, int endIndex, short fontIndex)
	{
		XSSFFont xSSFFont;
		if (styles == null)
		{
			xSSFFont = new XSSFFont();
			xSSFFont.FontName = "#" + fontIndex;
		}
		else
		{
			xSSFFont = styles.GetFontAt(fontIndex);
		}
		ApplyFont(startIndex, endIndex, xSSFFont);
	}

	internal void ApplyFont(SortedDictionary<int, CT_RPrElt> formats, int startIndex, int endIndex, CT_RPrElt fmt)
	{
		List<int> list = new List<int>();
		SortedDictionary<int, CT_RPrElt>.KeyCollection.Enumerator enumerator = formats.Keys.GetEnumerator();
		while (enumerator.MoveNext())
		{
			int current = enumerator.Current;
			if (current >= startIndex && current < endIndex)
			{
				list.Add(current);
			}
		}
		foreach (int item in list)
		{
			formats.Remove(item);
		}
		if (startIndex > 0 && !formats.ContainsKey(startIndex))
		{
			foreach (KeyValuePair<int, CT_RPrElt> format in formats)
			{
				if (format.Key > startIndex)
				{
					formats[startIndex] = format.Value;
					break;
				}
			}
		}
		formats[endIndex] = fmt;
	}

	public void ApplyFont(int startIndex, int endIndex, IFont font)
	{
		if (startIndex > endIndex)
		{
			throw new ArgumentException("Start index must be less than end index, but had " + startIndex + " and " + endIndex);
		}
		if (startIndex < 0 || endIndex > Length)
		{
			throw new ArgumentException("Start and end index not in range, but had " + startIndex + " and " + endIndex);
		}
		if (startIndex != endIndex)
		{
			if (st.sizeOfRArray() == 0 && st.IsSetT())
			{
				st.AddNewR().t = st.t;
				st.unsetT();
			}
			string text = String;
			XSSFFont xSSFFont = (XSSFFont)font;
			SortedDictionary<int, CT_RPrElt> formatMap = GetFormatMap(st);
			CT_RPrElt cT_RPrElt = new CT_RPrElt();
			SetRunAttributes(xSSFFont.GetCTFont(), cT_RPrElt);
			ApplyFont(formatMap, startIndex, endIndex, cT_RPrElt);
			CT_Rst o = BuildCTRst(text, formatMap);
			st.Set(o);
		}
	}

	internal SortedDictionary<int, CT_RPrElt> GetFormatMap(CT_Rst entry)
	{
		int num = 0;
		SortedDictionary<int, CT_RPrElt> sortedDictionary = new SortedDictionary<int, CT_RPrElt>();
		foreach (CT_RElt item in entry.r)
		{
			string t = item.t;
			CT_RPrElt rPr = item.rPr;
			num += t.Length;
			sortedDictionary[num] = rPr;
		}
		return sortedDictionary;
	}

	public void ApplyFont(IFont font)
	{
		string text = String;
		ApplyFont(0, text.Length, font);
	}

	public void ApplyFont(short fontIndex)
	{
		XSSFFont xSSFFont;
		if (styles == null)
		{
			xSSFFont = new XSSFFont();
			xSSFFont.FontName = "#" + fontIndex;
		}
		else
		{
			xSSFFont = styles.GetFontAt(fontIndex);
		}
		string text = String;
		ApplyFont(0, text.Length, xSSFFont);
	}

	public void Append(string text, XSSFFont font)
	{
		if (st.sizeOfRArray() == 0 && st.IsSetT())
		{
			CT_RElt cT_RElt = st.AddNewR();
			cT_RElt.t = st.t;
			PreserveSpaces(cT_RElt.t);
			st.unsetT();
		}
		CT_RElt cT_RElt2 = st.AddNewR();
		cT_RElt2.t = text;
		PreserveSpaces(cT_RElt2.t);
		if (font != null)
		{
			CT_RPrElt pr = cT_RElt2.AddNewRPr();
			SetRunAttributes(font.GetCTFont(), pr);
		}
	}

	public void Append(string text)
	{
		Append(text, null);
	}

	private void SetRunAttributes(CT_Font ctFont, CT_RPrElt pr)
	{
		if (ctFont.SizeOfBArray() > 0)
		{
			pr.AddNewB().val = ctFont.GetBArray(0).val;
		}
		if (ctFont.sizeOfUArray() > 0)
		{
			pr.AddNewU().val = ctFont.GetUArray(0).val;
		}
		if (ctFont.sizeOfIArray() > 0)
		{
			pr.AddNewI().val = ctFont.GetIArray(0).val;
		}
		if (ctFont.sizeOfColorArray() > 0)
		{
			CT_Color colorArray = ctFont.GetColorArray(0);
			CT_Color cT_Color = pr.AddNewColor();
			if (colorArray.IsSetAuto())
			{
				cT_Color.auto = colorArray.auto;
				cT_Color.autoSpecified = true;
			}
			if (colorArray.IsSetIndexed())
			{
				cT_Color.indexed = colorArray.indexed;
				cT_Color.indexedSpecified = true;
			}
			if (colorArray.IsSetRgb())
			{
				cT_Color.SetRgb(colorArray.rgb);
				cT_Color.rgbSpecified = true;
			}
			if (colorArray.IsSetTheme())
			{
				cT_Color.theme = colorArray.theme;
				cT_Color.themeSpecified = true;
			}
			if (colorArray.IsSetTint())
			{
				cT_Color.tint = colorArray.tint;
				cT_Color.tintSpecified = true;
			}
		}
		if (ctFont.sizeOfSzArray() > 0)
		{
			pr.AddNewSz().val = ctFont.GetSzArray(0).val;
		}
		if (ctFont.sizeOfNameArray() > 0)
		{
			pr.AddNewRFont().val = ctFont.name.val;
		}
		if (ctFont.sizeOfFamilyArray() > 0)
		{
			pr.AddNewFamily().val = ctFont.GetFamilyArray(0).val;
		}
		if (ctFont.sizeOfSchemeArray() > 0)
		{
			pr.AddNewScheme().val = ctFont.GetSchemeArray(0).val;
		}
		if (ctFont.sizeOfCharsetArray() > 0)
		{
			pr.AddNewCharset().val = ctFont.GetCharsetArray(0).val;
		}
		if (ctFont.sizeOfCondenseArray() > 0)
		{
			pr.AddNewCondense().val = ctFont.GetCondenseArray(0).val;
		}
		if (ctFont.sizeOfExtendArray() > 0)
		{
			pr.AddNewExtend().val = ctFont.GetExtendArray(0).val;
		}
		if (ctFont.sizeOfVertAlignArray() > 0)
		{
			pr.AddNewVertAlign().val = ctFont.GetVertAlignArray(0).val;
		}
		if (ctFont.sizeOfOutlineArray() > 0)
		{
			pr.AddNewOutline().val = ctFont.GetOutlineArray(0).val;
		}
		if (ctFont.sizeOfShadowArray() > 0)
		{
			pr.AddNewShadow().val = ctFont.GetShadowArray(0).val;
		}
		if (ctFont.sizeOfStrikeArray() > 0)
		{
			pr.AddNewStrike().val = ctFont.GetStrikeArray(0).val;
		}
	}

	public bool HasFormatting()
	{
		List<CT_RElt> r = st.r;
		if (r == null || r.Count == 0)
		{
			return false;
		}
		foreach (CT_RElt item in r)
		{
			if (item.isSetRPr())
			{
				return true;
			}
		}
		return false;
	}

	public void ClearFormatting()
	{
		string t = String;
		st.r = null;
		st.t = t;
	}

	public int GetIndexOfFormattingRun(int index)
	{
		if (st.sizeOfRArray() == 0)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < st.sizeOfRArray(); i++)
		{
			CT_RElt rArray = st.GetRArray(i);
			if (i == index)
			{
				return num;
			}
			num += rArray.t.Length;
		}
		return -1;
	}

	public int GetLengthOfFormattingRun(int index)
	{
		if (st.sizeOfRArray() == 0 || index >= st.sizeOfRArray())
		{
			return -1;
		}
		return st.GetRArray(index).t.Length;
	}

	public override string ToString()
	{
		return String;
	}

	public IFont GetFontOfFormattingRun(int index)
	{
		if (st.sizeOfRArray() == 0 || index >= st.sizeOfRArray())
		{
			return null;
		}
		CT_RElt rArray = st.GetRArray(index);
		if (rArray.rPr != null)
		{
			XSSFFont xSSFFont = new XSSFFont(ToCTFont(rArray.rPr));
			xSSFFont.SetThemesTable(GetThemesTable());
			return xSSFFont;
		}
		return null;
	}

	public XSSFFont GetFontAtIndex(int index)
	{
		ThemesTable themesTable = GetThemesTable();
		int num = 0;
		if (st.r == null)
		{
			return null;
		}
		foreach (CT_RElt item in st.r)
		{
			int length = item.t.Length;
			if (index >= num && index < num + length)
			{
				XSSFFont xSSFFont = new XSSFFont(ToCTFont(item.rPr));
				xSSFFont.SetThemesTable(themesTable);
				return xSSFFont;
			}
			num += length;
		}
		return null;
	}

	public CT_Rst GetCTRst()
	{
		return st;
	}

	protected static CT_Font ToCTFont(CT_RPrElt pr)
	{
		CT_Font cT_Font = new CT_Font();
		if (pr == null)
		{
			return cT_Font;
		}
		if (pr.SizeOfBArray() > 0)
		{
			cT_Font.AddNewB().val = pr.GetBArray(0).val;
		}
		if (pr.SizeOfUArray() > 0)
		{
			cT_Font.AddNewU().val = pr.GetUArray(0).val;
		}
		if (pr.SizeOfIArray() > 0)
		{
			cT_Font.AddNewI().val = pr.GetIArray(0).val;
		}
		if (pr.SizeOfColorArray() > 0)
		{
			CT_Color colorArray = pr.GetColorArray(0);
			CT_Color cT_Color = cT_Font.AddNewColor();
			if (colorArray.IsSetAuto())
			{
				cT_Color.auto = colorArray.auto;
				cT_Color.autoSpecified = true;
			}
			if (colorArray.IsSetIndexed())
			{
				cT_Color.indexed = colorArray.indexed;
				cT_Color.indexedSpecified = true;
			}
			if (colorArray.IsSetRgb())
			{
				cT_Color.SetRgb(colorArray.GetRgb());
				cT_Color.rgbSpecified = true;
			}
			if (colorArray.IsSetTheme())
			{
				cT_Color.theme = colorArray.theme;
				cT_Color.themeSpecified = true;
			}
			if (colorArray.IsSetTint())
			{
				cT_Color.tint = colorArray.tint;
				cT_Color.tintSpecified = true;
			}
		}
		if (pr.SizeOfSzArray() > 0)
		{
			cT_Font.AddNewSz().val = pr.GetSzArray(0).val;
		}
		if (pr.SizeOfRFontArray() > 0)
		{
			cT_Font.AddNewName().val = pr.GetRFontArray(0).val;
		}
		if (pr.SizeOfFamilyArray() > 0)
		{
			cT_Font.AddNewFamily().val = pr.GetFamilyArray(0).val;
		}
		if (pr.sizeOfSchemeArray() > 0)
		{
			cT_Font.AddNewScheme().val = pr.GetSchemeArray(0).val;
		}
		if (pr.sizeOfCharsetArray() > 0)
		{
			cT_Font.AddNewCharset().val = pr.GetCharsetArray(0).val;
		}
		if (pr.sizeOfCondenseArray() > 0)
		{
			cT_Font.AddNewCondense().val = pr.GetCondenseArray(0).val;
		}
		if (pr.sizeOfExtendArray() > 0)
		{
			cT_Font.AddNewExtend().val = pr.GetExtendArray(0).val;
		}
		if (pr.sizeOfVertAlignArray() > 0)
		{
			cT_Font.AddNewVertAlign().val = pr.GetVertAlignArray(0).val;
		}
		if (pr.sizeOfOutlineArray() > 0)
		{
			cT_Font.AddNewOutline().val = pr.GetOutlineArray(0).val;
		}
		if (pr.sizeOfShadowArray() > 0)
		{
			cT_Font.AddNewShadow().val = pr.GetShadowArray(0).val;
		}
		if (pr.sizeOfStrikeArray() > 0)
		{
			cT_Font.AddNewStrike().val = pr.GetStrikeArray(0).val;
		}
		return cT_Font;
	}

	protected static void PreserveSpaces(string xs)
	{
		if (xs != null && xs.Length > 0)
		{
			char c = xs[0];
			char c2 = xs[xs.Length - 1];
			if (!char.IsWhiteSpace(c))
			{
				char.IsWhiteSpace(c2);
			}
		}
	}

	private static string UtfDecode(string value)
	{
		if (value == null)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		MatchCollection matchCollection = utfPtrn.Matches(value);
		int num = 0;
		for (int i = 0; i < matchCollection.Count; i++)
		{
			int index = matchCollection[i].Index;
			if (index > num)
			{
				stringBuilder.Append(value.Substring(num, index - num));
			}
			int num2 = int.Parse(matchCollection[i].Groups[1].Value, NumberStyles.AllowHexSpecifier);
			stringBuilder.Append((char)num2);
			num = matchCollection[i].Index + matchCollection[i].Length;
		}
		if (num == 0)
		{
			return value;
		}
		stringBuilder.Append(value.Substring(num));
		return stringBuilder.ToString();
	}

	public int GetLastKey(SortedDictionary<int, CT_RPrElt>.KeyCollection keys)
	{
		int num = 0;
		foreach (int key in keys)
		{
			if (num == keys.Count - 1)
			{
				return key;
			}
			num++;
		}
		throw new ArgumentOutOfRangeException("GetLastKey failed");
	}

	private CT_Rst BuildCTRst(string text, SortedDictionary<int, CT_RPrElt> formats)
	{
		if (text.Length != GetLastKey(formats.Keys))
		{
			throw new ArgumentException("Text length was " + text.Length + " but the last format index was " + GetLastKey(formats.Keys));
		}
		CT_Rst cT_Rst = new CT_Rst();
		int num = 0;
		foreach (KeyValuePair<int, CT_RPrElt> format in formats)
		{
			int key = format.Key;
			CT_RElt cT_RElt = cT_Rst.AddNewR();
			string t = text.Substring(num, key - num);
			cT_RElt.t = t;
			PreserveSpaces(cT_RElt.t);
			CT_RPrElt value = format.Value;
			if (value != null)
			{
				cT_RElt.rPr = value;
			}
			num = key;
		}
		return cT_Rst;
	}

	private ThemesTable GetThemesTable()
	{
		if (styles == null)
		{
			return null;
		}
		return styles.GetTheme();
	}
}

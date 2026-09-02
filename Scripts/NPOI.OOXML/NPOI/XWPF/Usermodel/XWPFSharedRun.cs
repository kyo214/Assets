using System;
using System.Text;
using NPOI.OpenXmlFormats.Shared;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.WP.UserModel;
using NPOI.XWPF.UserModel;

namespace NPOI.XWPF.Usermodel;

public class XWPFSharedRun : ICharacterRun
{
	private NPOI.OpenXmlFormats.Shared.CT_R run;

	private IRunBody parent;

	public bool IsBold { get; set; }

	public bool IsItalic
	{
		get
		{
			CT_RPr rPr = run.rPr1;
			if (rPr == null || !rPr.IsSetI())
			{
				return false;
			}
			return IsCTOnOff(rPr.i);
		}
		set
		{
			CT_RPr cT_RPr = (run.IsSetRPr1() ? run.rPr1 : run.AddNewRPr1());
			(cT_RPr.IsSetI() ? cT_RPr.i : cT_RPr.AddNewI()).val = value;
		}
	}

	public bool IsSmallCaps { get; set; }

	public bool IsCapitalized { get; set; }

	public bool IsStrikeThrough { get; set; }

	public bool IsDoubleStrikeThrough { get; set; }

	public bool IsShadowed { get; set; }

	public bool IsEmbossed { get; set; }

	public bool IsImprinted { get; set; }

	public int CharacterSpacing { get; set; }

	public int Kerning { get; set; }

	public bool IsHighlighted { get; set; }

	public string FontName => FontFamily;

	public string FontFamily
	{
		get
		{
			return GetFontFamily(FontCharRange.None);
		}
		set
		{
			SetFontFamily(value, FontCharRange.None);
		}
	}

	public double FontSize { get; set; }

	public string Text
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < run.Items.Count; i++)
			{
				object obj = run.Items[i];
				if (obj is CT_Text1)
				{
					stringBuilder.Append(((CT_Text1)obj).Value);
				}
			}
			return stringBuilder.ToString();
		}
	}

	public XWPFSharedRun(NPOI.OpenXmlFormats.Shared.CT_R ctR, IRunBody p)
	{
		run = ctR;
		parent = p;
		SetFontFamily("Cambria Math", FontCharRange.None);
	}

	private bool IsCTOnOff(NPOI.OpenXmlFormats.Wordprocessing.CT_OnOff onoff)
	{
		if (!onoff.IsSetVal())
		{
			return true;
		}
		return onoff.val;
	}

	public void SetFontFamily(string fontFamily, FontCharRange fcr)
	{
		CT_RPr cT_RPr = (run.IsSetRPr1() ? run.rPr1 : run.AddNewRPr1());
		CT_Fonts cT_Fonts = (cT_RPr.IsSetRFonts() ? cT_RPr.rFonts : cT_RPr.AddNewRFonts());
		switch (fcr)
		{
		case FontCharRange.None:
			cT_Fonts.ascii = fontFamily;
			if (!cT_Fonts.IsSetHAnsi())
			{
				cT_Fonts.hAnsi = fontFamily;
			}
			if (!cT_Fonts.IsSetCs())
			{
				cT_Fonts.cs = fontFamily;
			}
			if (!cT_Fonts.IsSetEastAsia())
			{
				cT_Fonts.eastAsia = fontFamily;
			}
			break;
		case FontCharRange.Ascii:
			cT_Fonts.ascii = fontFamily;
			break;
		case FontCharRange.CS:
			cT_Fonts.cs = fontFamily;
			break;
		case FontCharRange.EastAsia:
			cT_Fonts.eastAsia = fontFamily;
			break;
		case FontCharRange.HAnsi:
			cT_Fonts.hAnsi = fontFamily;
			break;
		}
	}

	public string GetFontFamily(FontCharRange fcr)
	{
		CT_RPr rPr = run.rPr1;
		if (rPr == null || !rPr.IsSetRFonts())
		{
			return null;
		}
		CT_Fonts rFonts = rPr.rFonts;
		return ((fcr == FontCharRange.None) ? FontCharRange.Ascii : fcr) switch
		{
			FontCharRange.CS => rFonts.cs, 
			FontCharRange.EastAsia => rFonts.eastAsia, 
			FontCharRange.HAnsi => rFonts.hAnsi, 
			_ => rFonts.ascii, 
		};
	}

	public XWPFSharedRun SetText(string value)
	{
		SetText(value, 0);
		return this;
	}

	private XWPFSharedRun SetText(string value, int pos)
	{
		int num = run.SizeOfTArray();
		if (pos > num)
		{
			throw new IndexOutOfRangeException("Value too large for the parameter position");
		}
		CT_Text1 obj = ((pos < num && pos >= 0) ? run.GetTArray(pos) : run.AddNewT());
		obj.Value = value;
		preserveSpaces(obj);
		return this;
	}

	private static void preserveSpaces(CT_Text1 xs)
	{
		string value = xs.Value;
		if (value != null && (value.StartsWith(" ") || value.EndsWith(" ")))
		{
			xs.space = "preserve";
		}
	}
}

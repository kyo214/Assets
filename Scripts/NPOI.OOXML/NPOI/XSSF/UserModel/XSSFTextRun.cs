using System;
using System.Drawing;
using NPOI.OpenXmlFormats.Dml;
using NPOI.Util;

namespace NPOI.XSSF.UserModel;

public class XSSFTextRun
{
	private CT_RegularTextRun _r;

	private XSSFTextParagraph _p;

	public XSSFTextParagraph ParentParagraph => _p;

	public string Text
	{
		get
		{
			return _r.t;
		}
		set
		{
			_r.t = value;
		}
	}

	public Color FontColor
	{
		get
		{
			CT_TextCharacterProperties rPr = GetRPr();
			if (rPr.IsSetSolidFill())
			{
				CT_SolidColorFillProperties solidFill = rPr.solidFill;
				if (solidFill.IsSetSrgbClr())
				{
					byte[] val = solidFill.srgbClr.val;
					return Color.FromArgb(0xFF & val[0], 0xFF & val[1], 0xFF & val[2]);
				}
			}
			return Color.FromArgb(0, 0, 0);
		}
		set
		{
			CT_TextCharacterProperties rPr = GetRPr();
			CT_SolidColorFillProperties cT_SolidColorFillProperties = (rPr.IsSetSolidFill() ? rPr.solidFill : rPr.AddNewSolidFill());
			(cT_SolidColorFillProperties.IsSetSrgbClr() ? cT_SolidColorFillProperties.srgbClr : cT_SolidColorFillProperties.AddNewSrgbClr()).val = new byte[3] { value.R, value.G, value.B };
			if (cT_SolidColorFillProperties.IsSetHslClr())
			{
				cT_SolidColorFillProperties.UnsetHslClr();
			}
			if (cT_SolidColorFillProperties.IsSetPrstClr())
			{
				cT_SolidColorFillProperties.UnsetPrstClr();
			}
			if (cT_SolidColorFillProperties.IsSetSchemeClr())
			{
				cT_SolidColorFillProperties.UnsetSchemeClr();
			}
			if (cT_SolidColorFillProperties.IsSetScrgbClr())
			{
				cT_SolidColorFillProperties.UnsetScrgbClr();
			}
			if (cT_SolidColorFillProperties.IsSetSysClr())
			{
				cT_SolidColorFillProperties.UnsetSysClr();
			}
		}
	}

	public double FontSize
	{
		get
		{
			double num = 1.0;
			double num2 = 11.0;
			CT_TextNormalAutofit normAutofit = ParentParagraph.ParentShape.txBody.bodyPr.normAutofit;
			if (normAutofit != null)
			{
				num = (double)normAutofit.fontScale / 100000.0;
			}
			CT_TextCharacterProperties rPr = GetRPr();
			if (rPr.IsSetSz())
			{
				num2 = (double)rPr.sz * 0.01;
			}
			return num2 * num;
		}
		set
		{
			CT_TextCharacterProperties rPr = GetRPr();
			if (value == -1.0)
			{
				if (rPr.IsSetSz())
				{
					rPr.UnsetSz();
				}
				return;
			}
			if (value < 1.0)
			{
				throw new ArgumentException("Minimum font size is 1pt but was " + value);
			}
			rPr.sz = (int)(100.0 * value);
		}
	}

	public double CharacterSpacing
	{
		get
		{
			CT_TextCharacterProperties rPr = GetRPr();
			if (rPr.IsSetSpc())
			{
				return (double)rPr.spc * 0.01;
			}
			return 0.0;
		}
		set
		{
			CT_TextCharacterProperties rPr = GetRPr();
			if (value == 0.0)
			{
				if (rPr.IsSetSpc())
				{
					rPr.UnsetSpc();
				}
			}
			else
			{
				rPr.spc = (int)(100.0 * value);
			}
		}
	}

	public string FontFamily
	{
		get
		{
			CT_TextFont latin = GetRPr().latin;
			if (latin != null)
			{
				return latin.typeface;
			}
			return "Calibri";
		}
	}

	public byte PitchAndFamily
	{
		get
		{
			CT_TextFont latin = GetRPr().latin;
			if (latin != null)
			{
				return (byte)latin.pitchFamily;
			}
			return 0;
		}
	}

	public bool IsStrikethrough
	{
		get
		{
			CT_TextCharacterProperties rPr = GetRPr();
			if (rPr.IsSetStrike())
			{
				return rPr.strike != ST_TextStrikeType.noStrike;
			}
			return false;
		}
		set
		{
			GetRPr().strike = (value ? ST_TextStrikeType.sngStrike : ST_TextStrikeType.noStrike);
		}
	}

	public bool IsSuperscript
	{
		get
		{
			CT_TextCharacterProperties rPr = GetRPr();
			if (rPr.IsSetBaseline())
			{
				return rPr.baseline > 0;
			}
			return false;
		}
		set
		{
			SetBaselineOffset(value ? 30.0 : 0.0);
		}
	}

	public bool IsSubscript
	{
		get
		{
			CT_TextCharacterProperties rPr = GetRPr();
			if (rPr.IsSetBaseline())
			{
				return rPr.baseline < 0;
			}
			return false;
		}
		set
		{
			SetBaselineOffset(value ? (-25.0) : 0.0);
		}
	}

	public TextCap TextCap
	{
		get
		{
			CT_TextCharacterProperties rPr = GetRPr();
			if (rPr.IsSetCap())
			{
				return EnumConverter.ValueOf<TextCap, ST_TextCapsType>(rPr.cap);
			}
			return TextCap.NONE;
		}
	}

	public bool IsBold
	{
		get
		{
			CT_TextCharacterProperties rPr = GetRPr();
			if (rPr.IsSetB())
			{
				return rPr.b;
			}
			return false;
		}
		set
		{
			GetRPr().b = value;
		}
	}

	public bool IsItalic
	{
		get
		{
			CT_TextCharacterProperties rPr = GetRPr();
			if (rPr.IsSetI())
			{
				return rPr.i;
			}
			return false;
		}
		set
		{
			GetRPr().i = value;
		}
	}

	public bool IsUnderline
	{
		get
		{
			CT_TextCharacterProperties rPr = GetRPr();
			if (rPr.IsSetU())
			{
				return rPr.u != ST_TextUnderlineType.none;
			}
			return false;
		}
		set
		{
			GetRPr().u = (value ? ST_TextUnderlineType.sng : ST_TextUnderlineType.none);
		}
	}

	public XSSFTextRun(CT_RegularTextRun r, XSSFTextParagraph p)
	{
		_r = r;
		_p = p;
	}

	public CT_RegularTextRun GetXmlObject()
	{
		return _r;
	}

	public void SetFont(string typeface)
	{
		SetFontFamily(typeface, byte.MaxValue, byte.MaxValue, isSymbol: false);
	}

	public void SetFontFamily(string typeface, byte charset, byte pictAndFamily, bool isSymbol)
	{
		CT_TextCharacterProperties rPr = GetRPr();
		if (typeface == null)
		{
			if (rPr.IsSetLatin())
			{
				rPr.UnsetLatin();
			}
			if (rPr.IsSetCs())
			{
				rPr.UnsetCs();
			}
			if (rPr.IsSetSym())
			{
				rPr.UnsetSym();
			}
		}
		else if (isSymbol)
		{
			(rPr.IsSetSym() ? rPr.sym : rPr.AddNewSym()).typeface = typeface;
		}
		else
		{
			CT_TextFont cT_TextFont = (rPr.IsSetLatin() ? rPr.latin : rPr.AddNewLatin());
			cT_TextFont.typeface = typeface;
			if ((sbyte)charset != -1)
			{
				cT_TextFont.charset = (sbyte)charset;
			}
			if ((sbyte)pictAndFamily != -1)
			{
				cT_TextFont.pitchFamily = (sbyte)pictAndFamily;
			}
		}
	}

	public void SetBaselineOffset(double baselineOffset)
	{
		GetRPr().baseline = (int)baselineOffset * 1000;
	}

	internal CT_TextCharacterProperties GetRPr()
	{
		if (!_r.IsSetRPr())
		{
			return _r.AddNewRPr();
		}
		return _r.rPr;
	}

	public override string ToString()
	{
		return "[" + GetType().ToString() + "]" + Text;
	}
}

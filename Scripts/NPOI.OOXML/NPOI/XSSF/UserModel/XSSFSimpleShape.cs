using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NPOI.HSSF.Util;
using NPOI.OpenXmlFormats.Dml;
using NPOI.OpenXmlFormats.Dml.Spreadsheet;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.Util;

namespace NPOI.XSSF.UserModel;

public class XSSFSimpleShape : XSSFShape, IEnumerable<XSSFTextParagraph>, IEnumerable
{
	private List<XSSFTextParagraph> _paragraphs;

	private static CT_Shape prototype = null;

	private CT_Shape ctShape;

	private static string[] _romanChars = new string[13]
	{
		"M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX",
		"V", "IV", "I"
	};

	private static int[] _romanAlphaValues = new int[13]
	{
		1000, 900, 500, 400, 100, 90, 50, 40, 10, 9,
		5, 4, 1
	};

	public string Text
	{
		get
		{
			int num = 9;
			StringBuilder stringBuilder = new StringBuilder();
			List<int> list = new List<int>(num);
			XSSFTextParagraph xSSFTextParagraph = null;
			for (int i = 0; i < num; i++)
			{
				list.Add(0);
			}
			for (int j = 0; j < _paragraphs.Count; j++)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append('\n');
				}
				xSSFTextParagraph = _paragraphs[j];
				if (xSSFTextParagraph.IsBullet && xSSFTextParagraph.Text.Length > 0)
				{
					int num2 = Math.Min(xSSFTextParagraph.Level, num - 1);
					if (xSSFTextParagraph.IsBulletAutoNumber)
					{
						j = ProcessAutoNumGroup(j, num2, list, stringBuilder);
						continue;
					}
					for (int k = 0; k < num2; k++)
					{
						stringBuilder.Append('\t');
					}
					string bulletCharacter = xSSFTextParagraph.BulletCharacter;
					stringBuilder.Append((bulletCharacter.Length > 0) ? (bulletCharacter + " ") : "- ");
					stringBuilder.Append(xSSFTextParagraph.Text);
				}
				else
				{
					stringBuilder.Append(xSSFTextParagraph.Text);
					for (int l = 0; l < num; l++)
					{
						list[l] = 0;
					}
				}
			}
			return stringBuilder.ToString();
		}
	}

	public List<XSSFTextParagraph> TextParagraphs => _paragraphs;

	public TextHorizontalOverflow TextHorizontalOverflow
	{
		get
		{
			CT_TextBodyProperties bodyPr = ctShape.txBody.bodyPr;
			if (bodyPr != null && bodyPr.IsSetHorzOverflow())
			{
				return (TextHorizontalOverflow)(bodyPr.horzOverflow - 1);
			}
			return TextHorizontalOverflow.OVERFLOW;
		}
		set
		{
			CT_TextBodyProperties bodyPr = ctShape.txBody.bodyPr;
			if (bodyPr == null)
			{
				return;
			}
			if (value == TextHorizontalOverflow.None)
			{
				if (bodyPr.IsSetHorzOverflow())
				{
					bodyPr.UnsetHorzOverflow();
				}
			}
			else
			{
				bodyPr.horzOverflow = (ST_TextHorzOverflowType)(value + 1);
			}
		}
	}

	public TextVerticalOverflow TextVerticalOverflow
	{
		get
		{
			CT_TextBodyProperties bodyPr = ctShape.txBody.bodyPr;
			if (bodyPr != null && bodyPr.IsSetVertOverflow())
			{
				return (TextVerticalOverflow)(bodyPr.vertOverflow - 1);
			}
			return TextVerticalOverflow.OVERFLOW;
		}
		set
		{
			CT_TextBodyProperties bodyPr = ctShape.txBody.bodyPr;
			if (bodyPr == null)
			{
				return;
			}
			if (value == TextVerticalOverflow.None)
			{
				if (bodyPr.IsSetVertOverflow())
				{
					bodyPr.UnsetVertOverflow();
				}
			}
			else
			{
				bodyPr.vertOverflow = (ST_TextVertOverflowType)(value + 1);
			}
		}
	}

	public VerticalAlignment VerticalAlignment
	{
		get
		{
			CT_TextBodyProperties bodyPr = ctShape.txBody.bodyPr;
			if (bodyPr != null && bodyPr.IsSetAnchor())
			{
				return (VerticalAlignment)bodyPr.anchor;
			}
			return VerticalAlignment.Top;
		}
		set
		{
			CT_TextBodyProperties bodyPr = ctShape.txBody.bodyPr;
			if (bodyPr == null)
			{
				return;
			}
			if (value == VerticalAlignment.None)
			{
				if (bodyPr.IsSetAnchor())
				{
					bodyPr.UnsetAnchor();
				}
			}
			else
			{
				bodyPr.anchor = (ST_TextAnchoringType)value;
			}
		}
	}

	public TextDirection TextDirection
	{
		get
		{
			CT_TextBodyProperties bodyPr = ctShape.txBody.bodyPr;
			if (bodyPr != null)
			{
				ST_TextVerticalType vert = bodyPr.vert;
				if (vert != ST_TextVerticalType.horz)
				{
					return (TextDirection)(vert - 1);
				}
			}
			return TextDirection.HORIZONTAL;
		}
		set
		{
			CT_TextBodyProperties bodyPr = ctShape.txBody.bodyPr;
			if (bodyPr == null)
			{
				return;
			}
			if (value == TextDirection.None)
			{
				if (bodyPr.IsSetVert())
				{
					bodyPr.UnsetVert();
				}
			}
			else
			{
				bodyPr.vert = (ST_TextVerticalType)(value + 1);
			}
		}
	}

	public double BottomInset
	{
		get
		{
			CT_TextBodyProperties bodyPr = ctShape.txBody.bodyPr;
			if (bodyPr != null && bodyPr.IsSetBIns())
			{
				return Units.ToPoints(bodyPr.bIns);
			}
			return 3.6;
		}
		set
		{
			CT_TextBodyProperties bodyPr = ctShape.txBody.bodyPr;
			if (bodyPr == null)
			{
				return;
			}
			if (value == -1.0)
			{
				if (bodyPr.IsSetBIns())
				{
					bodyPr.UnsetBIns();
				}
			}
			else
			{
				bodyPr.bIns = Units.ToEMU(value);
			}
		}
	}

	public double LeftInset
	{
		get
		{
			CT_TextBodyProperties bodyPr = ctShape.txBody.bodyPr;
			if (bodyPr != null && bodyPr.IsSetLIns())
			{
				return Units.ToPoints(bodyPr.lIns);
			}
			return 3.6;
		}
		set
		{
			CT_TextBodyProperties bodyPr = ctShape.txBody.bodyPr;
			if (bodyPr == null)
			{
				return;
			}
			if (value == -1.0)
			{
				if (bodyPr.IsSetLIns())
				{
					bodyPr.UnsetLIns();
				}
			}
			else
			{
				bodyPr.lIns = Units.ToEMU(value);
			}
		}
	}

	public double RightInset
	{
		get
		{
			CT_TextBodyProperties bodyPr = ctShape.txBody.bodyPr;
			if (bodyPr != null && bodyPr.IsSetRIns())
			{
				return Units.ToPoints(bodyPr.rIns);
			}
			return 3.6;
		}
		set
		{
			CT_TextBodyProperties bodyPr = ctShape.txBody.bodyPr;
			if (bodyPr == null)
			{
				return;
			}
			if (value == -1.0)
			{
				if (bodyPr.IsSetRIns())
				{
					bodyPr.UnsetRIns();
				}
			}
			else
			{
				bodyPr.rIns = Units.ToEMU(value);
			}
		}
	}

	public double TopInset
	{
		get
		{
			CT_TextBodyProperties bodyPr = ctShape.txBody.bodyPr;
			if (bodyPr != null && bodyPr.IsSetTIns())
			{
				return Units.ToPoints(bodyPr.tIns);
			}
			return 3.6;
		}
		set
		{
			CT_TextBodyProperties bodyPr = ctShape.txBody.bodyPr;
			if (bodyPr == null)
			{
				return;
			}
			if (value == -1.0)
			{
				if (bodyPr.IsSetTIns())
				{
					bodyPr.UnsetTIns();
				}
			}
			else
			{
				bodyPr.tIns = Units.ToEMU(value);
			}
		}
	}

	public bool WordWrap
	{
		get
		{
			CT_TextBodyProperties bodyPr = ctShape.txBody.bodyPr;
			if (bodyPr != null && bodyPr.IsSetWrap())
			{
				return bodyPr.wrap == ST_TextWrappingType.square;
			}
			return true;
		}
		set
		{
			CT_TextBodyProperties bodyPr = ctShape.txBody.bodyPr;
			if (bodyPr != null)
			{
				bodyPr.wrap = (value ? ST_TextWrappingType.square : ST_TextWrappingType.none);
			}
		}
	}

	public TextAutofit TextAutofit
	{
		get
		{
			CT_TextBodyProperties bodyPr = ctShape.txBody.bodyPr;
			if (bodyPr != null)
			{
				if (bodyPr.IsSetNoAutofit())
				{
					return TextAutofit.NONE;
				}
				if (bodyPr.IsSetNormAutofit())
				{
					return TextAutofit.NORMAL;
				}
				if (bodyPr.IsSetSpAutoFit())
				{
					return TextAutofit.SHAPE;
				}
			}
			return TextAutofit.NORMAL;
		}
		set
		{
			CT_TextBodyProperties bodyPr = ctShape.txBody.bodyPr;
			if (bodyPr != null)
			{
				if (bodyPr.IsSetSpAutoFit())
				{
					bodyPr.UnsetSpAutoFit();
				}
				if (bodyPr.IsSetNoAutofit())
				{
					bodyPr.UnsetNoAutofit();
				}
				if (bodyPr.IsSetNormAutofit())
				{
					bodyPr.UnsetNormAutofit();
				}
				switch (value)
				{
				case TextAutofit.NONE:
					bodyPr.AddNewNoAutofit();
					break;
				case TextAutofit.NORMAL:
					bodyPr.AddNewNormAutofit();
					break;
				case TextAutofit.SHAPE:
					bodyPr.AddNewSpAutoFit();
					break;
				}
			}
		}
	}

	public int ShapeType
	{
		get
		{
			return (int)ctShape.spPr.prstGeom.prst;
		}
		set
		{
			ctShape.spPr.prstGeom.prst = (ST_ShapeType)value;
		}
	}

	protected internal XSSFSimpleShape(XSSFDrawing Drawing, CT_Shape ctShape)
	{
		drawing = Drawing;
		this.ctShape = ctShape;
		_paragraphs = new List<XSSFTextParagraph>();
		NPOI.OpenXmlFormats.Dml.Spreadsheet.CT_TextBody txBody = ctShape.txBody;
		if (txBody != null)
		{
			for (int i = 0; i < txBody.SizeOfPArray(); i++)
			{
				_paragraphs.Add(new XSSFTextParagraph(txBody.GetPArray(i), ctShape));
			}
		}
	}

	protected internal static CT_Shape Prototype()
	{
		CT_Shape cT_Shape = new CT_Shape();
		CT_ShapeNonVisual cT_ShapeNonVisual = cT_Shape.AddNewNvSpPr();
		NPOI.OpenXmlFormats.Dml.Spreadsheet.CT_NonVisualDrawingProps cT_NonVisualDrawingProps = cT_ShapeNonVisual.AddNewCNvPr();
		cT_NonVisualDrawingProps.id = 1u;
		cT_NonVisualDrawingProps.name = "Shape 1";
		cT_ShapeNonVisual.AddNewCNvSpPr();
		NPOI.OpenXmlFormats.Dml.Spreadsheet.CT_ShapeProperties cT_ShapeProperties = cT_Shape.AddNewSpPr();
		CT_Transform2D cT_Transform2D = cT_ShapeProperties.AddNewXfrm();
		CT_PositiveSize2D cT_PositiveSize2D = cT_Transform2D.AddNewExt();
		cT_PositiveSize2D.cx = 0L;
		cT_PositiveSize2D.cy = 0L;
		CT_Point2D cT_Point2D = cT_Transform2D.AddNewOff();
		cT_Point2D.x = 0L;
		cT_Point2D.y = 0L;
		CT_PresetGeometry2D cT_PresetGeometry2D = cT_ShapeProperties.AddNewPrstGeom();
		cT_PresetGeometry2D.prst = ST_ShapeType.rect;
		cT_PresetGeometry2D.AddNewAvLst();
		NPOI.OpenXmlFormats.Dml.Spreadsheet.CT_TextBody cT_TextBody = cT_Shape.AddNewTxBody();
		CT_TextBodyProperties cT_TextBodyProperties = cT_TextBody.AddNewBodyPr();
		cT_TextBodyProperties.anchor = ST_TextAnchoringType.t;
		cT_TextBodyProperties.rtlCol = false;
		CT_TextParagraph cT_TextParagraph = cT_TextBody.AddNewP();
		cT_TextParagraph.AddNewPPr().algn = ST_TextAlignType.l;
		CT_TextCharacterProperties cT_TextCharacterProperties = cT_TextParagraph.AddNewEndParaRPr();
		cT_TextCharacterProperties.lang = "en-US";
		cT_TextCharacterProperties.sz = 1100;
		cT_TextCharacterProperties.AddNewSolidFill().AddNewSrgbClr().val = new byte[3];
		cT_TextBody.AddNewLstStyle();
		prototype = cT_Shape;
		return prototype;
	}

	public CT_Shape GetCTShape()
	{
		return ctShape;
	}

	public IEnumerator<XSSFTextParagraph> GetEnumerator()
	{
		return _paragraphs.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		throw new NotImplementedException();
	}

	private int ProcessAutoNumGroup(int index, int level, List<int> levelCount, StringBuilder out1)
	{
		XSSFTextParagraph xSSFTextParagraph = null;
		XSSFTextParagraph xSSFTextParagraph2 = null;
		xSSFTextParagraph = _paragraphs[index];
		int bulletAutoNumberStart = xSSFTextParagraph.BulletAutoNumberStart;
		ListAutoNumber bulletAutoNumberScheme = xSSFTextParagraph.BulletAutoNumberScheme;
		if (levelCount[level] == 0)
		{
			levelCount[level] = ((bulletAutoNumberStart == 0) ? 1 : bulletAutoNumberStart);
		}
		for (int i = 0; i < level; i++)
		{
			out1.Append('\t');
		}
		if (xSSFTextParagraph.Text.Length > 0)
		{
			out1.Append(GetBulletPrefix(bulletAutoNumberScheme, levelCount[level]));
			out1.Append(xSSFTextParagraph.Text);
		}
		while (true)
		{
			xSSFTextParagraph2 = ((index + 1 == _paragraphs.Count) ? null : _paragraphs[index + 1]);
			if (xSSFTextParagraph2 == null || !xSSFTextParagraph2.IsBullet || !xSSFTextParagraph.IsBulletAutoNumber)
			{
				break;
			}
			if (xSSFTextParagraph2.Level > level)
			{
				if (out1.Length > 0)
				{
					out1.Append('\n');
				}
				index = ProcessAutoNumGroup(index + 1, xSSFTextParagraph2.Level, levelCount, out1);
				continue;
			}
			if (xSSFTextParagraph2.Level < level)
			{
				break;
			}
			ListAutoNumber bulletAutoNumberScheme2 = xSSFTextParagraph2.BulletAutoNumberScheme;
			int bulletAutoNumberStart2 = xSSFTextParagraph2.BulletAutoNumberStart;
			if (bulletAutoNumberScheme2 != bulletAutoNumberScheme || bulletAutoNumberStart2 != bulletAutoNumberStart)
			{
				break;
			}
			index++;
			if (out1.Length > 0)
			{
				out1.Append('\n');
			}
			for (int j = 0; j < level; j++)
			{
				out1.Append('\t');
			}
			if (xSSFTextParagraph2.Text.Length > 0)
			{
				levelCount[level]++;
				out1.Append(GetBulletPrefix(bulletAutoNumberScheme2, levelCount[level]));
				out1.Append(xSSFTextParagraph2.Text);
			}
		}
		levelCount[level] = 0;
		return index;
	}

	private string GetBulletPrefix(ListAutoNumber scheme, int value)
	{
		StringBuilder stringBuilder = new StringBuilder();
		switch (scheme)
		{
		case ListAutoNumber.ALPHA_LC_PARENT_BOTH:
			stringBuilder.Append('(');
			goto case ListAutoNumber.ALPHA_LC_PARENT_R;
		case ListAutoNumber.ALPHA_LC_PARENT_R:
			stringBuilder.Append(valueToAlpha(value).ToLower());
			stringBuilder.Append(')');
			break;
		case ListAutoNumber.ALPHA_UC_PARENT_BOTH:
			stringBuilder.Append('(');
			goto case ListAutoNumber.ALPHA_UC_PARENT_R;
		case ListAutoNumber.ALPHA_UC_PARENT_R:
			stringBuilder.Append(valueToAlpha(value));
			stringBuilder.Append(')');
			break;
		case ListAutoNumber.ALPHA_LC_PERIOD:
			stringBuilder.Append(valueToAlpha(value).ToLower());
			stringBuilder.Append('.');
			break;
		case ListAutoNumber.ALPHA_UC_PERIOD:
			stringBuilder.Append(valueToAlpha(value));
			stringBuilder.Append('.');
			break;
		case ListAutoNumber.ARABIC_PARENT_BOTH:
			stringBuilder.Append('(');
			goto case ListAutoNumber.ARABIC_PARENT_R;
		case ListAutoNumber.ARABIC_PARENT_R:
			stringBuilder.Append(value);
			stringBuilder.Append(')');
			break;
		case ListAutoNumber.ARABIC_PERIOD:
			stringBuilder.Append(value);
			stringBuilder.Append('.');
			break;
		case ListAutoNumber.ARABIC_PLAIN:
			stringBuilder.Append(value);
			break;
		case ListAutoNumber.ROMAN_LC_PARENT_BOTH:
			stringBuilder.Append('(');
			goto case ListAutoNumber.ROMAN_LC_PARENT_R;
		case ListAutoNumber.ROMAN_LC_PARENT_R:
			stringBuilder.Append(valueToRoman(value).ToLower());
			stringBuilder.Append(')');
			break;
		case ListAutoNumber.ROMAN_UC_PARENT_BOTH:
			stringBuilder.Append('(');
			goto case ListAutoNumber.ROMAN_UC_PARENT_R;
		case ListAutoNumber.ROMAN_UC_PARENT_R:
			stringBuilder.Append(valueToRoman(value));
			stringBuilder.Append(')');
			break;
		case ListAutoNumber.ROMAN_LC_PERIOD:
			stringBuilder.Append(valueToRoman(value).ToLower());
			stringBuilder.Append('.');
			break;
		case ListAutoNumber.ROMAN_UC_PERIOD:
			stringBuilder.Append(valueToRoman(value));
			stringBuilder.Append('.');
			break;
		default:
			stringBuilder.Append('•');
			break;
		}
		stringBuilder.Append(" ");
		return stringBuilder.ToString();
	}

	private string valueToAlpha(int value)
	{
		string text = "";
		while (value > 0)
		{
			int num = (value - 1) % 26;
			text = (char)(65 + num) + text;
			value = (value - num) / 26;
		}
		return text;
	}

	private string valueToRoman(int value)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		while (value > 0 && num < _romanChars.Length)
		{
			while (_romanAlphaValues[num] <= value)
			{
				stringBuilder.Append(_romanChars[num]);
				value -= _romanAlphaValues[num];
			}
			num++;
		}
		return stringBuilder.ToString();
	}

	public void ClearText()
	{
		_paragraphs.Clear();
		ctShape.txBody.SetPArray(null);
	}

	public void SetText(string text)
	{
		ClearText();
		AddNewTextParagraph().AddNewTextRun().Text = text;
	}

	public void SetText(XSSFRichTextString str)
	{
		XSSFWorkbook xSSFWorkbook = (XSSFWorkbook)GetDrawing().GetParent().GetParent();
		str.SetStylesTableReference(xSSFWorkbook.GetStylesSource());
		CT_TextParagraph cT_TextParagraph = new CT_TextParagraph();
		if (str.NumFormattingRuns == 0)
		{
			CT_RegularTextRun cT_RegularTextRun = cT_TextParagraph.AddNewR();
			CT_TextCharacterProperties cT_TextCharacterProperties = cT_RegularTextRun.AddNewRPr();
			cT_TextCharacterProperties.lang = "en-US";
			cT_TextCharacterProperties.sz = 1100;
			cT_RegularTextRun.t = str.String;
		}
		else
		{
			for (int i = 0; i < str.GetCTRst().SizeOfRArray(); i++)
			{
				CT_RElt rArray = str.GetCTRst().GetRArray(i);
				CT_RPrElt cT_RPrElt = rArray.rPr;
				if (cT_RPrElt == null)
				{
					cT_RPrElt = rArray.AddNewRPr();
				}
				CT_RegularTextRun cT_RegularTextRun2 = cT_TextParagraph.AddNewR();
				CT_TextCharacterProperties cT_TextCharacterProperties2 = cT_RegularTextRun2.AddNewRPr();
				cT_TextCharacterProperties2.lang = "en-US";
				ApplyAttributes(cT_RPrElt, cT_TextCharacterProperties2);
				cT_RegularTextRun2.t = rArray.t;
			}
		}
		ClearText();
		ctShape.txBody.SetPArray(new CT_TextParagraph[1] { cT_TextParagraph });
		_paragraphs.Add(new XSSFTextParagraph(ctShape.txBody.GetPArray(0), ctShape));
	}

	public XSSFTextParagraph AddNewTextParagraph()
	{
		XSSFTextParagraph xSSFTextParagraph = new XSSFTextParagraph(ctShape.txBody.AddNewP(), ctShape);
		_paragraphs.Add(xSSFTextParagraph);
		return xSSFTextParagraph;
	}

	public XSSFTextParagraph AddNewTextParagraph(string text)
	{
		XSSFTextParagraph xSSFTextParagraph = AddNewTextParagraph();
		xSSFTextParagraph.AddNewTextRun().Text = text;
		return xSSFTextParagraph;
	}

	public XSSFTextParagraph AddNewTextParagraph(XSSFRichTextString str)
	{
		CT_TextParagraph cT_TextParagraph = ctShape.txBody.AddNewP();
		if (str.NumFormattingRuns == 0)
		{
			CT_RegularTextRun cT_RegularTextRun = cT_TextParagraph.AddNewR();
			CT_TextCharacterProperties cT_TextCharacterProperties = cT_RegularTextRun.AddNewRPr();
			cT_TextCharacterProperties.lang = "en-US";
			cT_TextCharacterProperties.sz = 1100;
			cT_RegularTextRun.t = str.String;
		}
		else
		{
			for (int i = 0; i < str.GetCTRst().SizeOfRArray(); i++)
			{
				CT_RElt rArray = str.GetCTRst().GetRArray(i);
				CT_RPrElt cT_RPrElt = rArray.rPr;
				if (cT_RPrElt == null)
				{
					cT_RPrElt = rArray.AddNewRPr();
				}
				CT_RegularTextRun cT_RegularTextRun2 = cT_TextParagraph.AddNewR();
				CT_TextCharacterProperties cT_TextCharacterProperties2 = cT_RegularTextRun2.AddNewRPr();
				cT_TextCharacterProperties2.lang = "en-US";
				ApplyAttributes(cT_RPrElt, cT_TextCharacterProperties2);
				cT_RegularTextRun2.t = rArray.t;
			}
		}
		XSSFTextParagraph xSSFTextParagraph = new XSSFTextParagraph(cT_TextParagraph, ctShape);
		_paragraphs.Add(xSSFTextParagraph);
		return xSSFTextParagraph;
	}

	protected internal override NPOI.OpenXmlFormats.Dml.Spreadsheet.CT_ShapeProperties GetShapeProperties()
	{
		return ctShape.spPr;
	}

	private static void ApplyAttributes(CT_RPrElt pr, CT_TextCharacterProperties rPr)
	{
		if (pr.SizeOfBArray() > 0)
		{
			rPr.b = pr.GetBArray(0).val;
		}
		if (pr.SizeOfUArray() > 0)
		{
			switch (pr.GetUArray(0).val)
			{
			case ST_UnderlineValues.single:
				rPr.u = ST_TextUnderlineType.sng;
				break;
			case ST_UnderlineValues.@double:
				rPr.u = ST_TextUnderlineType.dbl;
				break;
			case ST_UnderlineValues.none:
				rPr.u = ST_TextUnderlineType.none;
				break;
			}
		}
		if (pr.SizeOfIArray() > 0)
		{
			rPr.i = pr.GetIArray(0).val;
		}
		if (pr.SizeOfRFontArray() > 0)
		{
			(rPr.IsSetLatin() ? rPr.latin : rPr.AddNewLatin()).typeface = pr.GetRFontArray(0).val;
		}
		if (pr.SizeOfSzArray() > 0)
		{
			int sz = (int)(pr.GetSzArray(0).val * 100.0);
			rPr.sz = sz;
		}
		if (pr.SizeOfColorArray() <= 0)
		{
			return;
		}
		CT_SolidColorFillProperties cT_SolidColorFillProperties = (rPr.IsSetSolidFill() ? rPr.solidFill : rPr.AddNewSolidFill());
		NPOI.OpenXmlFormats.Spreadsheet.CT_Color colorArray = pr.GetColorArray(0);
		if (colorArray.IsSetRgb())
		{
			(cT_SolidColorFillProperties.IsSetSrgbClr() ? cT_SolidColorFillProperties.srgbClr : cT_SolidColorFillProperties.AddNewSrgbClr()).val = colorArray.rgb;
		}
		else if (colorArray.IsSetIndexed())
		{
			HSSFColor hSSFColor = HSSFColor.GetIndexHash()[(int)colorArray.indexed];
			if (hSSFColor != null)
			{
				byte[] val = new byte[3]
				{
					hSSFColor.GetTriplet()[0],
					hSSFColor.GetTriplet()[1],
					hSSFColor.GetTriplet()[2]
				};
				(cT_SolidColorFillProperties.IsSetSrgbClr() ? cT_SolidColorFillProperties.srgbClr : cT_SolidColorFillProperties.AddNewSrgbClr()).val = val;
			}
		}
	}
}

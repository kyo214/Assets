using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using NPOI.OpenXmlFormats.Dml;
using NPOI.OpenXmlFormats.Dml.Spreadsheet;
using NPOI.Util;
using NPOI.XSSF.Model;

namespace NPOI.XSSF.UserModel;

public class XSSFTextParagraph : IEnumerator<XSSFTextRun>, IDisposable, IEnumerator, IEnumerable<XSSFTextRun>, IEnumerable
{
	private class ParagraphPropertyTextAlignFetcher : ParagraphPropertyFetcher<TextAlign?>
	{
		public ParagraphPropertyTextAlignFetcher(int level)
			: base(level)
		{
		}

		public override bool Fetch(CT_TextParagraphProperties props)
		{
			if (props.IsSetAlgn())
			{
				TextAlign value = (TextAlign)(props.algn + 1);
				SetValue(value);
				return true;
			}
			return false;
		}
	}

	private class ParagraphPropertyFetcherTextFontAlign : ParagraphPropertyFetcher<TextFontAlign?>
	{
		public ParagraphPropertyFetcherTextFontAlign(int level)
			: base(level)
		{
		}

		public override bool Fetch(CT_TextParagraphProperties props)
		{
			if (props.IsSetFontAlgn())
			{
				TextFontAlign value = (TextFontAlign)(props.fontAlgn + 1);
				SetValue(value);
				return true;
			}
			return false;
		}
	}

	private class ParagraphPropertyFetcherBulletFont : ParagraphPropertyFetcher<string>
	{
		public ParagraphPropertyFetcherBulletFont(int level)
			: base(level)
		{
		}

		public override bool Fetch(CT_TextParagraphProperties props)
		{
			if (props.IsSetBuFont())
			{
				SetValue(props.buFont.typeface);
				return true;
			}
			return false;
		}
	}

	private class ParagraphPropertyFetcherBulletCharacter : ParagraphPropertyFetcher<string>
	{
		public ParagraphPropertyFetcherBulletCharacter(int level)
			: base(level)
		{
		}

		public override bool Fetch(CT_TextParagraphProperties props)
		{
			if (props.IsSetBuChar())
			{
				SetValue(props.buChar.@char);
				return true;
			}
			return false;
		}
	}

	private class ParagraphPropertyFetcherBulletFontColor : ParagraphPropertyFetcher<Color>
	{
		public ParagraphPropertyFetcherBulletFontColor(int level)
			: base(level)
		{
		}

		public override bool Fetch(CT_TextParagraphProperties props)
		{
			if (props.IsSetBuClr() && props.buClr.IsSetSrgbClr())
			{
				byte[] val = props.buClr.srgbClr.val;
				SetValue(Color.FromArgb(0xFF & val[0], 0xFF & val[1], 0xFF & val[2]));
				return true;
			}
			return false;
		}
	}

	private class ParagraphPropertyFetcherBulletFontSize : ParagraphPropertyFetcher<double?>
	{
		public ParagraphPropertyFetcherBulletFontSize(int level)
			: base(level)
		{
		}

		public override bool Fetch(CT_TextParagraphProperties props)
		{
			if (props.IsSetBuSzPct())
			{
				SetValue((double)props.buSzPct.val * 0.001);
				return true;
			}
			if (props.IsSetBuSzPts())
			{
				SetValue((double)(-props.buSzPts.val) * 0.01);
				return true;
			}
			return false;
		}
	}

	private class ParagraphPropertyFetcherIndent : ParagraphPropertyFetcher<double>
	{
		public ParagraphPropertyFetcherIndent(int level)
			: base(level)
		{
		}

		public override bool Fetch(CT_TextParagraphProperties props)
		{
			if (props.IsSetIndent())
			{
				SetValue(Units.ToPoints(props.indent));
				return true;
			}
			return false;
		}
	}

	private class ParagraphPropertyFetcherLeftMargin : ParagraphPropertyFetcher<double>
	{
		public ParagraphPropertyFetcherLeftMargin(int level)
			: base(level)
		{
		}

		public override bool Fetch(CT_TextParagraphProperties props)
		{
			if (props.IsSetMarL())
			{
				double value = Units.ToPoints(props.marL);
				SetValue(value);
				return true;
			}
			return false;
		}
	}

	private class ParagraphPropertyFetcherRightMargin : ParagraphPropertyFetcher<double>
	{
		public ParagraphPropertyFetcherRightMargin(int level)
			: base(level)
		{
		}

		public override bool Fetch(CT_TextParagraphProperties props)
		{
			if (props.IsSetMarR())
			{
				double value = Units.ToPoints(props.marR);
				SetValue(value);
				return true;
			}
			return false;
		}
	}

	private class ParagraphPropertyFetcherDefaultTabSize : ParagraphPropertyFetcher<double>
	{
		public ParagraphPropertyFetcherDefaultTabSize(int level)
			: base(level)
		{
		}

		public override bool Fetch(CT_TextParagraphProperties props)
		{
			if (props.IsSetDefTabSz())
			{
				double value = Units.ToPoints(props.defTabSz);
				SetValue(value);
				return true;
			}
			return false;
		}
	}

	private class ParagraphPropertyFetcherTabStop : ParagraphPropertyFetcher<double>
	{
		private int idx;

		public ParagraphPropertyFetcherTabStop(int level, int idx)
			: base(level)
		{
			this.idx = idx;
		}

		public override bool Fetch(CT_TextParagraphProperties props)
		{
			if (props.IsSetTabLst())
			{
				CT_TextTabStopList tabLst = props.tabLst;
				if (idx < tabLst.SizeOfTabArray())
				{
					double value = Units.ToPoints(tabLst.GetTabArray(idx).pos);
					SetValue(value);
					return true;
				}
			}
			return false;
		}
	}

	private class ParagraphPropertyFetcherLineSpacing : ParagraphPropertyFetcher<double?>
	{
		public ParagraphPropertyFetcherLineSpacing(int level)
			: base(level)
		{
		}

		public override bool Fetch(CT_TextParagraphProperties props)
		{
			if (props.IsSetLnSpc())
			{
				CT_TextSpacing lnSpc = props.lnSpc;
				if (lnSpc.IsSetSpcPct())
				{
					SetValue((double)lnSpc.spcPct.val * 0.001);
				}
				else if (lnSpc.IsSetSpcPts())
				{
					SetValue((double)(-lnSpc.spcPts.val) * 0.01);
				}
				return true;
			}
			return false;
		}
	}

	private class ParagraphPropertyFetcherSpaceBefore : ParagraphPropertyFetcher<double>
	{
		public ParagraphPropertyFetcherSpaceBefore(int level)
			: base(level)
		{
		}

		public override bool Fetch(CT_TextParagraphProperties props)
		{
			if (props.IsSetSpcBef())
			{
				CT_TextSpacing spcBef = props.spcBef;
				if (spcBef.IsSetSpcPct())
				{
					SetValue((double)spcBef.spcPct.val * 0.001);
				}
				else if (spcBef.IsSetSpcPts())
				{
					SetValue((double)(-spcBef.spcPts.val) * 0.01);
				}
				return true;
			}
			return false;
		}
	}

	private class ParagraphPropertyFetcherSpaceAfter : ParagraphPropertyFetcher<double>
	{
		public ParagraphPropertyFetcherSpaceAfter(int level)
			: base(level)
		{
		}

		public override bool Fetch(CT_TextParagraphProperties props)
		{
			if (props.IsSetSpcAft())
			{
				CT_TextSpacing spcAft = props.spcAft;
				if (spcAft.IsSetSpcPct())
				{
					SetValue((double)spcAft.spcPct.val * 0.001);
				}
				else if (spcAft.IsSetSpcPts())
				{
					SetValue((double)(-spcAft.spcPts.val) * 0.01);
				}
				return true;
			}
			return false;
		}
	}

	private class ParagraphPropertyFetcherBullet : ParagraphPropertyFetcher<bool?>
	{
		public ParagraphPropertyFetcherBullet(int level)
			: base(level)
		{
		}

		public override bool Fetch(CT_TextParagraphProperties props)
		{
			if (props.IsSetBuNone())
			{
				SetValue(false);
				return true;
			}
			if (props.IsSetBuFont() && (props.IsSetBuChar() || props.IsSetBuAutoNum()))
			{
				SetValue(true);
				return true;
			}
			return false;
		}
	}

	private class ParagraphPropertyFetcherIsBulletAutoNumber : ParagraphPropertyFetcher<bool>
	{
		public ParagraphPropertyFetcherIsBulletAutoNumber(int level)
			: base(level)
		{
		}

		public override bool Fetch(CT_TextParagraphProperties props)
		{
			if (props.IsSetBuAutoNum())
			{
				SetValue(val: true);
				return true;
			}
			return false;
		}
	}

	private class ParagraphPropertyFetcherBulletAutoNumberStart : ParagraphPropertyFetcher<int>
	{
		public ParagraphPropertyFetcherBulletAutoNumberStart(int level)
			: base(level)
		{
		}

		public override bool Fetch(CT_TextParagraphProperties props)
		{
			if (props.IsSetBuAutoNum() && props.buAutoNum.IsSetStartAt())
			{
				SetValue(props.buAutoNum.startAt);
				return true;
			}
			return false;
		}
	}

	private class ParagraphPropertyFetcherBulletAutoNumberScheme : ParagraphPropertyFetcher<ListAutoNumber?>
	{
		public ParagraphPropertyFetcherBulletAutoNumberScheme(int level)
			: base(level)
		{
		}

		public override bool Fetch(CT_TextParagraphProperties props)
		{
			if (props.IsSetBuAutoNum())
			{
				SetValue((ListAutoNumber)props.buAutoNum.type);
				return true;
			}
			return false;
		}
	}

	private CT_TextParagraph _p;

	private CT_Shape _shape;

	private List<XSSFTextRun> _Runs;

	public string Text
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (XSSFTextRun run in _Runs)
			{
				stringBuilder.Append(run.Text);
			}
			return stringBuilder.ToString();
		}
	}

	public CT_Shape ParentShape => _shape;

	public List<XSSFTextRun> TextRuns => _Runs;

	public TextAlign TextAlign
	{
		get
		{
			ParagraphPropertyTextAlignFetcher paragraphPropertyTextAlignFetcher = new ParagraphPropertyTextAlignFetcher(Level);
			fetchParagraphProperty(paragraphPropertyTextAlignFetcher);
			if (paragraphPropertyTextAlignFetcher.GetValue().HasValue)
			{
				return paragraphPropertyTextAlignFetcher.GetValue().Value;
			}
			return TextAlign.LEFT;
		}
		set
		{
			CT_TextParagraphProperties cT_TextParagraphProperties = (_p.IsSetPPr() ? _p.pPr : _p.AddNewPPr());
			if (value == TextAlign.None)
			{
				if (cT_TextParagraphProperties.IsSetAlgn())
				{
					cT_TextParagraphProperties.UnsetAlgn();
				}
			}
			else
			{
				cT_TextParagraphProperties.algn = (ST_TextAlignType)(value - 1);
			}
		}
	}

	public TextFontAlign TextFontAlign
	{
		get
		{
			ParagraphPropertyFetcherTextFontAlign paragraphPropertyFetcherTextFontAlign = new ParagraphPropertyFetcherTextFontAlign(Level);
			fetchParagraphProperty(paragraphPropertyFetcherTextFontAlign);
			if (paragraphPropertyFetcherTextFontAlign.GetValue().HasValue)
			{
				return paragraphPropertyFetcherTextFontAlign.GetValue().Value;
			}
			return TextFontAlign.BASELINE;
		}
		set
		{
			CT_TextParagraphProperties cT_TextParagraphProperties = (_p.IsSetPPr() ? _p.pPr : _p.AddNewPPr());
			if (value == TextFontAlign.None)
			{
				if (cT_TextParagraphProperties.IsSetFontAlgn())
				{
					cT_TextParagraphProperties.UnsetFontAlgn();
				}
			}
			else
			{
				cT_TextParagraphProperties.fontAlgn = (ST_TextFontAlignType)(value - 1);
			}
		}
	}

	public string BulletFont
	{
		get
		{
			ParagraphPropertyFetcherBulletFont paragraphPropertyFetcherBulletFont = new ParagraphPropertyFetcherBulletFont(Level);
			fetchParagraphProperty(paragraphPropertyFetcherBulletFont);
			return paragraphPropertyFetcherBulletFont.GetValue();
		}
		set
		{
			CT_TextParagraphProperties cT_TextParagraphProperties = (_p.IsSetPPr() ? _p.pPr : _p.AddNewPPr());
			(cT_TextParagraphProperties.IsSetBuFont() ? cT_TextParagraphProperties.buFont : cT_TextParagraphProperties.AddNewBuFont()).typeface = value;
		}
	}

	public string BulletCharacter
	{
		get
		{
			ParagraphPropertyFetcherBulletCharacter paragraphPropertyFetcherBulletCharacter = new ParagraphPropertyFetcherBulletCharacter(Level);
			fetchParagraphProperty(paragraphPropertyFetcherBulletCharacter);
			return paragraphPropertyFetcherBulletCharacter.GetValue();
		}
		set
		{
			CT_TextParagraphProperties cT_TextParagraphProperties = (_p.IsSetPPr() ? _p.pPr : _p.AddNewPPr());
			(cT_TextParagraphProperties.IsSetBuChar() ? cT_TextParagraphProperties.buChar : cT_TextParagraphProperties.AddNewBuChar()).@char = value;
		}
	}

	public Color BulletFontColor
	{
		get
		{
			ParagraphPropertyFetcherBulletFontColor paragraphPropertyFetcherBulletFontColor = new ParagraphPropertyFetcherBulletFontColor(Level);
			fetchParagraphProperty(paragraphPropertyFetcherBulletFontColor);
			return paragraphPropertyFetcherBulletFontColor.GetValue();
		}
		set
		{
			CT_TextParagraphProperties cT_TextParagraphProperties = (_p.IsSetPPr() ? _p.pPr : _p.AddNewPPr());
			CT_Color cT_Color = (cT_TextParagraphProperties.IsSetBuClr() ? cT_TextParagraphProperties.buClr : cT_TextParagraphProperties.AddNewBuClr());
			(cT_Color.IsSetSrgbClr() ? cT_Color.srgbClr : cT_Color.AddNewSrgbClr()).val = new byte[3] { value.R, value.G, value.B };
		}
	}

	public double BulletFontSize
	{
		get
		{
			ParagraphPropertyFetcherBulletFontSize paragraphPropertyFetcherBulletFontSize = new ParagraphPropertyFetcherBulletFontSize(Level);
			fetchParagraphProperty(paragraphPropertyFetcherBulletFontSize);
			if (paragraphPropertyFetcherBulletFontSize.GetValue().HasValue)
			{
				return paragraphPropertyFetcherBulletFontSize.GetValue().Value;
			}
			return 100.0;
		}
		set
		{
			CT_TextParagraphProperties cT_TextParagraphProperties = (_p.IsSetPPr() ? _p.pPr : _p.AddNewPPr());
			if (value >= 0.0)
			{
				(cT_TextParagraphProperties.IsSetBuSzPct() ? cT_TextParagraphProperties.buSzPct : cT_TextParagraphProperties.AddNewBuSzPct()).val = (int)(value * 1000.0);
				if (cT_TextParagraphProperties.IsSetBuSzPts())
				{
					cT_TextParagraphProperties.UnsetBuSzPts();
				}
			}
			else
			{
				(cT_TextParagraphProperties.IsSetBuSzPts() ? cT_TextParagraphProperties.buSzPts : cT_TextParagraphProperties.AddNewBuSzPts()).val = (int)((0.0 - value) * 100.0);
				if (cT_TextParagraphProperties.IsSetBuSzPct())
				{
					cT_TextParagraphProperties.UnsetBuSzPct();
				}
			}
		}
	}

	public double Indent
	{
		get
		{
			ParagraphPropertyFetcherIndent paragraphPropertyFetcherIndent = new ParagraphPropertyFetcherIndent(Level);
			fetchParagraphProperty(paragraphPropertyFetcherIndent);
			return paragraphPropertyFetcherIndent.GetValue();
		}
		set
		{
			CT_TextParagraphProperties cT_TextParagraphProperties = (_p.IsSetPPr() ? _p.pPr : _p.AddNewPPr());
			if (value == -1.0)
			{
				if (cT_TextParagraphProperties.IsSetIndent())
				{
					cT_TextParagraphProperties.UnsetIndent();
				}
			}
			else
			{
				cT_TextParagraphProperties.indent = Units.ToEMU(value);
			}
		}
	}

	public double LeftMargin
	{
		get
		{
			ParagraphPropertyFetcherLeftMargin paragraphPropertyFetcherLeftMargin = new ParagraphPropertyFetcherLeftMargin(Level);
			fetchParagraphProperty(paragraphPropertyFetcherLeftMargin);
			return paragraphPropertyFetcherLeftMargin.GetValue();
		}
		set
		{
			CT_TextParagraphProperties cT_TextParagraphProperties = (_p.IsSetPPr() ? _p.pPr : _p.AddNewPPr());
			if (value == -1.0)
			{
				if (cT_TextParagraphProperties.IsSetMarL())
				{
					cT_TextParagraphProperties.UnsetMarL();
				}
			}
			else
			{
				cT_TextParagraphProperties.marL = Units.ToEMU(value);
			}
		}
	}

	public double RightMargin
	{
		get
		{
			ParagraphPropertyFetcherRightMargin paragraphPropertyFetcherRightMargin = new ParagraphPropertyFetcherRightMargin(Level);
			fetchParagraphProperty(paragraphPropertyFetcherRightMargin);
			return paragraphPropertyFetcherRightMargin.GetValue();
		}
		set
		{
			CT_TextParagraphProperties cT_TextParagraphProperties = (_p.IsSetPPr() ? _p.pPr : _p.AddNewPPr());
			if (value == -1.0)
			{
				if (cT_TextParagraphProperties.IsSetMarR())
				{
					cT_TextParagraphProperties.UnsetMarR();
				}
			}
			else
			{
				cT_TextParagraphProperties.marR = Units.ToEMU(value);
			}
		}
	}

	public double DefaultTabSize
	{
		get
		{
			ParagraphPropertyFetcherDefaultTabSize paragraphPropertyFetcherDefaultTabSize = new ParagraphPropertyFetcherDefaultTabSize(Level);
			fetchParagraphProperty(paragraphPropertyFetcherDefaultTabSize);
			return paragraphPropertyFetcherDefaultTabSize.GetValue();
		}
	}

	public double LineSpacing
	{
		get
		{
			ParagraphPropertyFetcherLineSpacing paragraphPropertyFetcherLineSpacing = new ParagraphPropertyFetcherLineSpacing(Level);
			fetchParagraphProperty(paragraphPropertyFetcherLineSpacing);
			double num = ((!paragraphPropertyFetcherLineSpacing.GetValue().HasValue) ? 100.0 : paragraphPropertyFetcherLineSpacing.GetValue().Value);
			if (num > 0.0)
			{
				CT_TextNormalAutofit normAutofit = _shape.txBody.bodyPr.normAutofit;
				if (normAutofit != null)
				{
					double num2 = 1.0 - (double)normAutofit.lnSpcReduction / 100000.0;
					num *= num2;
				}
			}
			return num;
		}
		set
		{
			CT_TextParagraphProperties obj = (_p.IsSetPPr() ? _p.pPr : _p.AddNewPPr());
			CT_TextSpacing cT_TextSpacing = new CT_TextSpacing();
			if (value >= 0.0)
			{
				cT_TextSpacing.AddNewSpcPct().val = (int)(value * 1000.0);
			}
			else
			{
				cT_TextSpacing.AddNewSpcPts().val = (int)((0.0 - value) * 100.0);
			}
			obj.lnSpc = cT_TextSpacing;
		}
	}

	public double SpaceBefore
	{
		get
		{
			ParagraphPropertyFetcherSpaceBefore paragraphPropertyFetcherSpaceBefore = new ParagraphPropertyFetcherSpaceBefore(Level);
			fetchParagraphProperty(paragraphPropertyFetcherSpaceBefore);
			return paragraphPropertyFetcherSpaceBefore.GetValue();
		}
		set
		{
			CT_TextParagraphProperties obj = (_p.IsSetPPr() ? _p.pPr : _p.AddNewPPr());
			CT_TextSpacing cT_TextSpacing = new CT_TextSpacing();
			if (value >= 0.0)
			{
				cT_TextSpacing.AddNewSpcPct().val = (int)(value * 1000.0);
			}
			else
			{
				cT_TextSpacing.AddNewSpcPts().val = (int)((0.0 - value) * 100.0);
			}
			obj.spcBef = cT_TextSpacing;
		}
	}

	public double SpaceAfter
	{
		get
		{
			ParagraphPropertyFetcherSpaceAfter paragraphPropertyFetcherSpaceAfter = new ParagraphPropertyFetcherSpaceAfter(Level);
			fetchParagraphProperty(paragraphPropertyFetcherSpaceAfter);
			return paragraphPropertyFetcherSpaceAfter.GetValue();
		}
		set
		{
			CT_TextParagraphProperties obj = (_p.IsSetPPr() ? _p.pPr : _p.AddNewPPr());
			CT_TextSpacing cT_TextSpacing = new CT_TextSpacing();
			if (value >= 0.0)
			{
				cT_TextSpacing.AddNewSpcPct().val = (int)(value * 1000.0);
			}
			else
			{
				cT_TextSpacing.AddNewSpcPts().val = (int)((0.0 - value) * 100.0);
			}
			obj.spcAft = cT_TextSpacing;
		}
	}

	public int Level
	{
		get
		{
			return _p.pPr?.lvl ?? 0;
		}
		set
		{
			(_p.IsSetPPr() ? _p.pPr : _p.AddNewPPr()).lvl = value;
		}
	}

	public bool IsBullet
	{
		get
		{
			ParagraphPropertyFetcherBullet paragraphPropertyFetcherBullet = new ParagraphPropertyFetcherBullet(Level);
			fetchParagraphProperty(paragraphPropertyFetcherBullet);
			if (paragraphPropertyFetcherBullet.GetValue().HasValue)
			{
				return paragraphPropertyFetcherBullet.GetValue().Value;
			}
			return false;
		}
		set
		{
			if (IsBullet == value)
			{
				return;
			}
			CT_TextParagraphProperties cT_TextParagraphProperties = (_p.IsSetPPr() ? _p.pPr : _p.AddNewPPr());
			if (!value)
			{
				cT_TextParagraphProperties.AddNewBuNone();
				if (cT_TextParagraphProperties.IsSetBuAutoNum())
				{
					cT_TextParagraphProperties.UnsetBuAutoNum();
				}
				if (cT_TextParagraphProperties.IsSetBuBlip())
				{
					cT_TextParagraphProperties.UnsetBuBlip();
				}
				if (cT_TextParagraphProperties.IsSetBuChar())
				{
					cT_TextParagraphProperties.UnsetBuChar();
				}
				if (cT_TextParagraphProperties.IsSetBuClr())
				{
					cT_TextParagraphProperties.UnsetBuClr();
				}
				if (cT_TextParagraphProperties.IsSetBuClrTx())
				{
					cT_TextParagraphProperties.UnsetBuClrTx();
				}
				if (cT_TextParagraphProperties.IsSetBuFont())
				{
					cT_TextParagraphProperties.UnsetBuFont();
				}
				if (cT_TextParagraphProperties.IsSetBuFontTx())
				{
					cT_TextParagraphProperties.UnsetBuFontTx();
				}
				if (cT_TextParagraphProperties.IsSetBuSzPct())
				{
					cT_TextParagraphProperties.UnsetBuSzPct();
				}
				if (cT_TextParagraphProperties.IsSetBuSzPts())
				{
					cT_TextParagraphProperties.UnsetBuSzPts();
				}
				if (cT_TextParagraphProperties.IsSetBuSzTx())
				{
					cT_TextParagraphProperties.UnsetBuSzTx();
				}
			}
			else
			{
				if (cT_TextParagraphProperties.IsSetBuNone())
				{
					cT_TextParagraphProperties.UnsetBuNone();
				}
				if (!cT_TextParagraphProperties.IsSetBuFont())
				{
					cT_TextParagraphProperties.AddNewBuFont().typeface = "Arial";
				}
				if (!cT_TextParagraphProperties.IsSetBuAutoNum())
				{
					cT_TextParagraphProperties.AddNewBuChar().@char = "•";
				}
			}
		}
	}

	public bool IsBulletAutoNumber
	{
		get
		{
			ParagraphPropertyFetcherIsBulletAutoNumber paragraphPropertyFetcherIsBulletAutoNumber = new ParagraphPropertyFetcherIsBulletAutoNumber(Level);
			fetchParagraphProperty(paragraphPropertyFetcherIsBulletAutoNumber);
			return paragraphPropertyFetcherIsBulletAutoNumber.GetValue();
		}
	}

	public int BulletAutoNumberStart
	{
		get
		{
			ParagraphPropertyFetcherBulletAutoNumberStart paragraphPropertyFetcherBulletAutoNumberStart = new ParagraphPropertyFetcherBulletAutoNumberStart(Level);
			fetchParagraphProperty(paragraphPropertyFetcherBulletAutoNumberStart);
			return paragraphPropertyFetcherBulletAutoNumberStart.GetValue();
		}
	}

	public ListAutoNumber BulletAutoNumberScheme
	{
		get
		{
			ParagraphPropertyFetcherBulletAutoNumberScheme paragraphPropertyFetcherBulletAutoNumberScheme = new ParagraphPropertyFetcherBulletAutoNumberScheme(Level);
			fetchParagraphProperty(paragraphPropertyFetcherBulletAutoNumberScheme);
			if (paragraphPropertyFetcherBulletAutoNumberScheme.GetValue().HasValue)
			{
				return paragraphPropertyFetcherBulletAutoNumberScheme.GetValue().Value;
			}
			return ListAutoNumber.ARABIC_PLAIN;
		}
	}

	public XSSFTextRun Current
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	object IEnumerator.Current
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public XSSFTextParagraph(CT_TextParagraph p, CT_Shape ctShape)
	{
		_p = p;
		_shape = ctShape;
		_Runs = new List<XSSFTextRun>();
		foreach (CT_RegularTextRun item in _p.r)
		{
			if (item is CT_RegularTextRun)
			{
				CT_RegularTextRun r = item;
				_Runs.Add(new XSSFTextRun(r, this));
			}
			else if (item is CT_TextLineBreak)
			{
				CT_TextLineBreak cT_TextLineBreak = (CT_TextLineBreak)(object)item;
				CT_RegularTextRun r2 = new CT_RegularTextRun
				{
					rPr = cT_TextLineBreak.rPr,
					t = "\n"
				};
				_Runs.Add(new XSSFTextRun(r2, this));
			}
			else if (item is CT_TextField)
			{
				CT_TextField cT_TextField = (CT_TextField)(object)item;
				CT_RegularTextRun r3 = new CT_RegularTextRun
				{
					rPr = cT_TextField.rPr,
					t = cT_TextField.t
				};
				_Runs.Add(new XSSFTextRun(r3, this));
			}
		}
	}

	public CT_TextParagraph GetXmlObject()
	{
		return _p;
	}

	public XSSFTextRun AddNewTextRun()
	{
		CT_RegularTextRun cT_RegularTextRun = _p.AddNewR();
		cT_RegularTextRun.AddNewRPr().lang = "en-US";
		XSSFTextRun xSSFTextRun = new XSSFTextRun(cT_RegularTextRun, this);
		_Runs.Add(xSSFTextRun);
		return xSSFTextRun;
	}

	public XSSFTextRun AddLineBreak()
	{
		CT_TextCharacterProperties cT_TextCharacterProperties = _p.AddNewBr().AddNewRPr();
		if (_Runs.Count > 0)
		{
			cT_TextCharacterProperties = _Runs[_Runs.Count - 1].GetRPr();
		}
		XSSFTextRun xSSFTextRun = new XSSFLineBreak(new CT_RegularTextRun
		{
			rPr = cT_TextCharacterProperties,
			t = "\n"
		}, this, cT_TextCharacterProperties);
		_Runs.Add(xSSFTextRun);
		return xSSFTextRun;
	}

	public double GetTabStop(int idx)
	{
		ParagraphPropertyFetcherTabStop paragraphPropertyFetcherTabStop = new ParagraphPropertyFetcherTabStop(Level, idx);
		fetchParagraphProperty(paragraphPropertyFetcherTabStop);
		return paragraphPropertyFetcherTabStop.GetValue();
	}

	public void AddTabStop(double value)
	{
		CT_TextParagraphProperties cT_TextParagraphProperties = (_p.IsSetPPr() ? _p.pPr : _p.AddNewPPr());
		(cT_TextParagraphProperties.IsSetTabLst() ? cT_TextParagraphProperties.tabLst : cT_TextParagraphProperties.AddNewTabLst()).AddNewTab().pos = Units.ToEMU(value);
	}

	public void SetBullet(bool flag)
	{
		if (IsBullet == flag)
		{
			return;
		}
		CT_TextParagraphProperties cT_TextParagraphProperties = (_p.IsSetPPr() ? _p.pPr : _p.AddNewPPr());
		if (!flag)
		{
			cT_TextParagraphProperties.AddNewBuNone();
			if (cT_TextParagraphProperties.IsSetBuAutoNum())
			{
				cT_TextParagraphProperties.UnsetBuAutoNum();
			}
			if (cT_TextParagraphProperties.IsSetBuBlip())
			{
				cT_TextParagraphProperties.UnsetBuBlip();
			}
			if (cT_TextParagraphProperties.IsSetBuChar())
			{
				cT_TextParagraphProperties.UnsetBuChar();
			}
			if (cT_TextParagraphProperties.IsSetBuClr())
			{
				cT_TextParagraphProperties.UnsetBuClr();
			}
			if (cT_TextParagraphProperties.IsSetBuClrTx())
			{
				cT_TextParagraphProperties.UnsetBuClrTx();
			}
			if (cT_TextParagraphProperties.IsSetBuFont())
			{
				cT_TextParagraphProperties.UnsetBuFont();
			}
			if (cT_TextParagraphProperties.IsSetBuFontTx())
			{
				cT_TextParagraphProperties.UnsetBuFontTx();
			}
			if (cT_TextParagraphProperties.IsSetBuSzPct())
			{
				cT_TextParagraphProperties.UnsetBuSzPct();
			}
			if (cT_TextParagraphProperties.IsSetBuSzPts())
			{
				cT_TextParagraphProperties.UnsetBuSzPts();
			}
			if (cT_TextParagraphProperties.IsSetBuSzTx())
			{
				cT_TextParagraphProperties.UnsetBuSzTx();
			}
		}
		else
		{
			if (cT_TextParagraphProperties.IsSetBuNone())
			{
				cT_TextParagraphProperties.UnsetBuNone();
			}
			if (!cT_TextParagraphProperties.IsSetBuFont())
			{
				cT_TextParagraphProperties.AddNewBuFont().typeface = "Arial";
			}
			if (!cT_TextParagraphProperties.IsSetBuAutoNum())
			{
				cT_TextParagraphProperties.AddNewBuChar().@char = "•";
			}
		}
	}

	public void SetBullet(ListAutoNumber scheme, int startAt)
	{
		if (startAt < 1)
		{
			throw new ArgumentException("Start Number must be greater or equal that 1");
		}
		CT_TextParagraphProperties cT_TextParagraphProperties = (_p.IsSetPPr() ? _p.pPr : _p.AddNewPPr());
		CT_TextAutonumberBullet obj = (cT_TextParagraphProperties.IsSetBuAutoNum() ? cT_TextParagraphProperties.buAutoNum : cT_TextParagraphProperties.AddNewBuAutoNum());
		obj.type = (ST_TextAutonumberScheme)scheme;
		obj.startAt = startAt;
		if (!cT_TextParagraphProperties.IsSetBuFont())
		{
			cT_TextParagraphProperties.AddNewBuFont().typeface = "Arial";
		}
		if (cT_TextParagraphProperties.IsSetBuNone())
		{
			cT_TextParagraphProperties.UnsetBuNone();
		}
		if (cT_TextParagraphProperties.IsSetBuBlip())
		{
			cT_TextParagraphProperties.UnsetBuBlip();
		}
		if (cT_TextParagraphProperties.IsSetBuChar())
		{
			cT_TextParagraphProperties.UnsetBuChar();
		}
	}

	public void SetBullet(ListAutoNumber scheme)
	{
		CT_TextParagraphProperties cT_TextParagraphProperties = (_p.IsSetPPr() ? _p.pPr : _p.AddNewPPr());
		(cT_TextParagraphProperties.IsSetBuAutoNum() ? cT_TextParagraphProperties.buAutoNum : cT_TextParagraphProperties.AddNewBuAutoNum()).type = (ST_TextAutonumberScheme)scheme;
		if (!cT_TextParagraphProperties.IsSetBuFont())
		{
			cT_TextParagraphProperties.AddNewBuFont().typeface = "Arial";
		}
		if (cT_TextParagraphProperties.IsSetBuNone())
		{
			cT_TextParagraphProperties.UnsetBuNone();
		}
		if (cT_TextParagraphProperties.IsSetBuBlip())
		{
			cT_TextParagraphProperties.UnsetBuBlip();
		}
		if (cT_TextParagraphProperties.IsSetBuChar())
		{
			cT_TextParagraphProperties.UnsetBuChar();
		}
	}

	private bool fetchParagraphProperty(ParagraphPropertyFetcher visitor)
	{
		bool flag = false;
		if (_p.IsSetPPr())
		{
			flag = visitor.Fetch(_p.pPr);
		}
		if (!flag)
		{
			flag = visitor.Fetch(_shape);
		}
		return flag;
	}

	public override string ToString()
	{
		return "[" + GetType().ToString() + "]" + Text;
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}

	public bool MoveNext()
	{
		throw new NotImplementedException();
	}

	public void Reset()
	{
		throw new NotImplementedException();
	}

	public IEnumerator<XSSFTextRun> GetEnumerator()
	{
		return _Runs.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _Runs.GetEnumerator();
	}
}

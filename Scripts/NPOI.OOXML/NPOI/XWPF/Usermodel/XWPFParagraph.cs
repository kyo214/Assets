using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NPOI.OpenXmlFormats.Shared;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.Util;
using NPOI.WP.UserModel;

namespace NPOI.XWPF.UserModel;

public class XWPFParagraph : IBodyElement, IRunBody, ISDTContents, IParagraph
{
	private CT_P paragraph;

	protected IBody part;

	protected XWPFDocument document;

	protected List<XWPFRun> runs;

	protected List<IRunElement> iRuns;

	protected List<XWPFOMath> oMaths;

	private StringBuilder footnoteText = new StringBuilder();

	public IList<XWPFRun> Runs => runs.AsReadOnly();

	public List<IRunElement> IRuns => iRuns;

	public IList<XWPFOMath> OMaths => oMaths.AsReadOnly();

	public bool IsEmpty
	{
		get
		{
			if (paragraph.Items.Count == 0)
			{
				if (paragraph.pPr != null)
				{
					return paragraph.pPr.IsEmpty;
				}
				return true;
			}
			return false;
		}
	}

	public XWPFDocument Document => document;

	public string Text
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (IRunElement iRun in iRuns)
			{
				if (iRun is XWPFRun)
				{
					XWPFRun xWPFRun = (XWPFRun)iRun;
					if (xWPFRun.GetCTR().GetDelTextList().Count == 0)
					{
						stringBuilder.Append(xWPFRun.ToString());
					}
				}
				else if (iRun is XWPFSDT)
				{
					stringBuilder.Append(((XWPFSDT)iRun).Content.Text);
				}
				else
				{
					stringBuilder.Append(iRun.ToString());
				}
			}
			stringBuilder.Append((object)footnoteText);
			return stringBuilder.ToString();
		}
	}

	public string StyleID
	{
		get
		{
			if (paragraph.pPr != null && paragraph.pPr.pStyle != null && paragraph.pPr.pStyle.val != null)
			{
				return paragraph.pPr.pStyle.val;
			}
			return null;
		}
	}

	public string NumLevelText
	{
		get
		{
			string numID = GetNumID();
			XWPFNumbering xWPFNumbering = document.CreateNumbering();
			if (numID != null && xWPFNumbering != null)
			{
				XWPFNum num = xWPFNumbering.GetNum(numID);
				if (num != null)
				{
					string numIlvl = GetNumIlvl();
					CT_Num cTNum = num.GetCTNum();
					if (cTNum == null)
					{
						return null;
					}
					CT_DecimalNumber abstractNumId = cTNum.abstractNumId;
					if (abstractNumId == null)
					{
						return null;
					}
					string val = abstractNumId.val;
					if (val == null)
					{
						return null;
					}
					XWPFAbstractNum abstractNum = xWPFNumbering.GetAbstractNum(val);
					if (abstractNum == null)
					{
						return null;
					}
					CT_AbstractNum cTAbstractNum = abstractNum.GetCTAbstractNum();
					if (cTAbstractNum == null)
					{
						return null;
					}
					CT_Lvl cT_Lvl = null;
					for (int i = 0; i < cTAbstractNum.SizeOfLvlArray(); i++)
					{
						CT_Lvl lvlArray = cTAbstractNum.GetLvlArray(i);
						if (lvlArray != null && lvlArray.ilvl != null && lvlArray.ilvl.Equals(numIlvl))
						{
							cT_Lvl = lvlArray;
							break;
						}
					}
					if (cT_Lvl != null && cT_Lvl.lvlText != null && cT_Lvl.lvlText.val != null)
					{
						return cT_Lvl.lvlText.val.ToString();
					}
				}
			}
			return null;
		}
	}

	public string ParagraphText
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (XWPFRun run in runs)
			{
				stringBuilder.Append(run.ToString());
			}
			return stringBuilder.ToString();
		}
	}

	public string PictureText
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (XWPFRun run in runs)
			{
				stringBuilder.Append(run.PictureText);
			}
			return stringBuilder.ToString();
		}
	}

	public string FootnoteText => footnoteText.ToString();

	public ParagraphAlignment Alignment
	{
		get
		{
			CT_PPr cTPPr = GetCTPPr();
			if (cTPPr != null && cTPPr.IsSetJc())
			{
				return EnumConverter.ValueOf<ParagraphAlignment, NPOI.OpenXmlFormats.Wordprocessing.ST_Jc>(cTPPr.jc.val);
			}
			return ParagraphAlignment.LEFT;
		}
		set
		{
			CT_PPr cTPPr = GetCTPPr();
			(cTPPr.IsSetJc() ? cTPPr.jc : cTPPr.AddNewJc()).val = EnumConverter.ValueOf<NPOI.OpenXmlFormats.Wordprocessing.ST_Jc, ParagraphAlignment>(value);
		}
	}

	public int FontAlignment
	{
		get
		{
			return (int)Alignment;
		}
		set
		{
			Alignment = (ParagraphAlignment)value;
		}
	}

	public TextAlignment VerticalAlignment
	{
		get
		{
			CT_PPr cTPPr = GetCTPPr();
			if (cTPPr != null && cTPPr.IsSetTextAlignment())
			{
				return EnumConverter.ValueOf<TextAlignment, ST_TextAlignment>(cTPPr.textAlignment.val);
			}
			return TextAlignment.AUTO;
		}
		set
		{
			CT_PPr cTPPr = GetCTPPr();
			(cTPPr.IsSetTextAlignment() ? cTPPr.textAlignment : cTPPr.AddNewTextAlignment()).val = EnumConverter.ValueOf<ST_TextAlignment, TextAlignment>(value);
		}
	}

	public Borders BorderTop
	{
		get
		{
			CT_PBdr cTPBrd = GetCTPBrd(create: false);
			CT_Border cT_Border = null;
			if (cTPBrd != null)
			{
				cT_Border = cTPBrd.top;
			}
			return EnumConverter.ValueOf<Borders, ST_Border>(cT_Border?.val ?? ST_Border.none);
		}
		set
		{
			CT_PBdr cTPBrd = GetCTPBrd(create: true);
			if (cTPBrd == null)
			{
				throw new RuntimeException("invalid paragraph state");
			}
			CT_Border cT_Border = (cTPBrd.IsSetTop() ? cTPBrd.top : cTPBrd.AddNewTop());
			if (value == Borders.None)
			{
				cTPBrd.UnsetTop();
			}
			else
			{
				cT_Border.val = EnumConverter.ValueOf<ST_Border, Borders>(value);
			}
		}
	}

	public Borders BorderBottom
	{
		get
		{
			CT_PBdr cTPBrd = GetCTPBrd(create: false);
			CT_Border cT_Border = null;
			if (cTPBrd != null)
			{
				cT_Border = cTPBrd.bottom;
			}
			return EnumConverter.ValueOf<Borders, ST_Border>(cT_Border?.val ?? ST_Border.none);
		}
		set
		{
			CT_PBdr cTPBrd = GetCTPBrd(create: true);
			CT_Border cT_Border = (cTPBrd.IsSetBottom() ? cTPBrd.bottom : cTPBrd.AddNewBottom());
			if (value == Borders.None)
			{
				cTPBrd.UnsetBottom();
			}
			else
			{
				cT_Border.val = EnumConverter.ValueOf<ST_Border, Borders>(value);
			}
		}
	}

	public Borders BorderLeft
	{
		get
		{
			CT_PBdr cTPBrd = GetCTPBrd(create: false);
			CT_Border cT_Border = null;
			if (cTPBrd != null)
			{
				cT_Border = cTPBrd.left;
			}
			return EnumConverter.ValueOf<Borders, ST_Border>(cT_Border?.val ?? ST_Border.none);
		}
		set
		{
			CT_PBdr cTPBrd = GetCTPBrd(create: true);
			CT_Border cT_Border = (cTPBrd.IsSetLeft() ? cTPBrd.left : cTPBrd.AddNewLeft());
			if (value == Borders.None)
			{
				cTPBrd.UnsetLeft();
			}
			else
			{
				cT_Border.val = EnumConverter.ValueOf<ST_Border, Borders>(value);
			}
		}
	}

	public Borders BorderRight
	{
		get
		{
			CT_PBdr cTPBrd = GetCTPBrd(create: false);
			CT_Border cT_Border = null;
			if (cTPBrd != null)
			{
				cT_Border = cTPBrd.right;
			}
			return EnumConverter.ValueOf<Borders, ST_Border>(cT_Border?.val ?? ST_Border.none);
		}
		set
		{
			CT_PBdr cTPBrd = GetCTPBrd(create: true);
			CT_Border cT_Border = (cTPBrd.IsSetRight() ? cTPBrd.right : cTPBrd.AddNewRight());
			if (value == Borders.None)
			{
				cTPBrd.UnsetRight();
			}
			else
			{
				cT_Border.val = EnumConverter.ValueOf<ST_Border, Borders>(value);
			}
		}
	}

	public ST_Shd FillPattern
	{
		get
		{
			if (!GetCTPPr().IsSetShd())
			{
				return ST_Shd.nil;
			}
			return GetCTPPr().shd.val;
		}
		set
		{
			CT_Shd cT_Shd = null;
			cT_Shd = (GetCTPPr().IsSetShd() ? GetCTPPr().shd : GetCTPPr().AddNewShd());
			cT_Shd.val = value;
		}
	}

	public string FillBackgroundColor
	{
		get
		{
			if (!GetCTPPr().IsSetShd())
			{
				return null;
			}
			return GetCTPPr().shd.fill;
		}
		set
		{
			CT_Shd cT_Shd = null;
			cT_Shd = (GetCTPPr().IsSetShd() ? GetCTPPr().shd : GetCTPPr().AddNewShd());
			cT_Shd.color = "auto";
			cT_Shd.fill = value;
		}
	}

	public Borders BorderBetween
	{
		get
		{
			CT_PBdr cTPBrd = GetCTPBrd(create: false);
			CT_Border cT_Border = null;
			if (cTPBrd != null)
			{
				cT_Border = cTPBrd.between;
			}
			return EnumConverter.ValueOf<Borders, ST_Border>(cT_Border?.val ?? ST_Border.none);
		}
		set
		{
			CT_PBdr cTPBrd = GetCTPBrd(create: true);
			CT_Border cT_Border = (cTPBrd.IsSetBetween() ? cTPBrd.between : cTPBrd.AddNewBetween());
			if (value == Borders.None)
			{
				cTPBrd.UnsetBetween();
			}
			else
			{
				cT_Border.val = EnumConverter.ValueOf<ST_Border, Borders>(value);
			}
		}
	}

	public bool IsPageBreak
	{
		get
		{
			CT_PPr cTPPr = GetCTPPr();
			NPOI.OpenXmlFormats.Wordprocessing.CT_OnOff cT_OnOff = (cTPPr.IsSetPageBreakBefore() ? cTPPr.pageBreakBefore : null);
			if (cT_OnOff != null && cT_OnOff.val)
			{
				return true;
			}
			return false;
		}
		set
		{
			CT_PPr cTPPr = GetCTPPr();
			(cTPPr.IsSetPageBreakBefore() ? cTPPr.pageBreakBefore : cTPPr.AddNewPageBreakBefore()).val = value;
		}
	}

	public int SpacingAfter
	{
		get
		{
			CT_Spacing cTSpacing = GetCTSpacing(create: false);
			if (cTSpacing == null || !cTSpacing.IsSetAfter())
			{
				return -1;
			}
			return (int)cTSpacing.after.Value;
		}
		set
		{
			CT_Spacing cTSpacing = GetCTSpacing(create: true);
			if (cTSpacing != null)
			{
				cTSpacing.after = (ulong)value;
			}
		}
	}

	public int SpacingAfterLines
	{
		get
		{
			CT_Spacing cTSpacing = GetCTSpacing(create: false);
			if (cTSpacing == null || !cTSpacing.IsSetAfterLines())
			{
				return -1;
			}
			return int.Parse(cTSpacing.afterLines);
		}
		set
		{
			GetCTSpacing(create: true).afterLines = value.ToString();
		}
	}

	public int SpacingBefore
	{
		get
		{
			CT_Spacing cTSpacing = GetCTSpacing(create: false);
			if (cTSpacing == null || !cTSpacing.IsSetBefore())
			{
				return -1;
			}
			return (int)cTSpacing.before.Value;
		}
		set
		{
			GetCTSpacing(create: true).before = (ulong)value;
		}
	}

	public int SpacingBeforeLines
	{
		get
		{
			CT_Spacing cTSpacing = GetCTSpacing(create: false);
			if (cTSpacing == null || !cTSpacing.IsSetBeforeLines())
			{
				return -1;
			}
			return int.Parse(cTSpacing.beforeLines);
		}
		set
		{
			GetCTSpacing(create: true).beforeLines = value.ToString();
		}
	}

	public LineSpacingRule SpacingLineRule
	{
		get
		{
			CT_Spacing cTSpacing = GetCTSpacing(create: false);
			if (cTSpacing == null || !cTSpacing.IsSetLineRule())
			{
				return LineSpacingRule.AUTO;
			}
			return EnumConverter.ValueOf<LineSpacingRule, ST_LineSpacingRule>(cTSpacing.lineRule);
		}
		set
		{
			GetCTSpacing(create: true).lineRule = EnumConverter.ValueOf<ST_LineSpacingRule, LineSpacingRule>(value);
		}
	}

	public double SpacingBetween
	{
		set
		{
			setSpacingBetween(value, LineSpacingRule.AUTO);
		}
	}

	public int IndentationLeft
	{
		get
		{
			CT_Ind cTInd = GetCTInd(create: false);
			if (cTInd == null || !cTInd.IsSetLeft())
			{
				return -1;
			}
			return int.Parse(cTInd.left);
		}
		set
		{
			GetCTInd(create: true).left = value.ToString();
		}
	}

	public int IndentationRight
	{
		get
		{
			CT_Ind cTInd = GetCTInd(create: false);
			if (cTInd == null || !cTInd.IsSetRight())
			{
				return -1;
			}
			return int.Parse(cTInd.right);
		}
		set
		{
			GetCTInd(create: true).right = value.ToString();
		}
	}

	public int IndentationHanging
	{
		get
		{
			CT_Ind cTInd = GetCTInd(create: false);
			if (cTInd == null || !cTInd.IsSetHanging())
			{
				return -1;
			}
			return (int)cTInd.hanging;
		}
		set
		{
			GetCTInd(create: true).hanging = (ulong)value;
		}
	}

	public int IndentationFirstLine
	{
		get
		{
			CT_Ind cTInd = GetCTInd(create: false);
			if (cTInd == null || !cTInd.IsSetFirstLine())
			{
				return -1;
			}
			return (int)cTInd.firstLine;
		}
		set
		{
			GetCTInd(create: true).firstLine = value;
		}
	}

	public int IndentFromLeft
	{
		get
		{
			return IndentationLeft;
		}
		set
		{
			IndentationLeft = value;
		}
	}

	public int IndentFromRight
	{
		get
		{
			return IndentationRight;
		}
		set
		{
			IndentationRight = value;
		}
	}

	public int FirstLineIndent
	{
		get
		{
			return IndentationFirstLine;
		}
		set
		{
			IndentationFirstLine = value;
		}
	}

	public bool IsWordWrapped
	{
		get
		{
			return (GetCTPPr().IsSetWordWrap() ? GetCTPPr().wordWrap : null)?.val ?? false;
		}
		set
		{
			NPOI.OpenXmlFormats.Wordprocessing.CT_OnOff cT_OnOff = (GetCTPPr().IsSetWordWrap() ? GetCTPPr().wordWrap : GetCTPPr().AddNewWordWrap());
			if (value)
			{
				cT_OnOff.val = true;
			}
			else
			{
				cT_OnOff.UnSetVal();
			}
		}
	}

	[Obsolete]
	public bool IsWordWrap
	{
		get
		{
			return IsWordWrapped;
		}
		set
		{
			IsWordWrapped = value;
		}
	}

	public string Style
	{
		get
		{
			CT_PPr cTPPr = GetCTPPr();
			return (cTPPr.IsSetPStyle() ? cTPPr.pStyle : null)?.val;
		}
		set
		{
			CT_PPr cTPPr = GetCTPPr();
			((cTPPr.pStyle != null) ? cTPPr.pStyle : cTPPr.AddNewPStyle()).val = value;
		}
	}

	public BodyElementType ElementType => BodyElementType.PARAGRAPH;

	public IBody Body => part;

	public POIXMLDocumentPart Part
	{
		get
		{
			if (part != null)
			{
				return part.Part;
			}
			return null;
		}
	}

	public BodyType PartType => part.PartType;

	public XWPFParagraph(CT_P prgrph, IBody part)
	{
		paragraph = prgrph;
		this.part = part;
		document = part.GetXWPFDocument();
		if (document == null)
		{
			throw new NullReferenceException();
		}
		runs = new List<XWPFRun>();
		iRuns = new List<IRunElement>();
		BuildRunsInOrderFromXml(paragraph.Items);
		oMaths = new List<XWPFOMath>();
		BuildOMathsInOrderFromXml(paragraph.Items);
		foreach (XWPFRun run in runs)
		{
			NPOI.OpenXmlFormats.Wordprocessing.CT_R cTR = run.GetCTR();
			if (document == null)
			{
				continue;
			}
			for (int i = 0; i < cTR.Items.Count; i++)
			{
				object obj = cTR.Items[i];
				if (!(obj is CT_FtnEdnRef))
				{
					continue;
				}
				CT_FtnEdnRef cT_FtnEdnRef = (CT_FtnEdnRef)obj;
				footnoteText.Append("[").Append(cT_FtnEdnRef.id).Append(": ");
				XWPFFootnote xWPFFootnote = null;
				if (cTR.ItemsElementName.Count > i && cTR.ItemsElementName[i] == RunItemsChoiceType.endnoteReference)
				{
					xWPFFootnote = document.GetEndnoteByID(int.Parse(cT_FtnEdnRef.id));
					if (xWPFFootnote == null)
					{
						xWPFFootnote = document.GetFootnoteByID(int.Parse(cT_FtnEdnRef.id));
					}
				}
				else
				{
					xWPFFootnote = document.GetFootnoteByID(int.Parse(cT_FtnEdnRef.id));
					if (xWPFFootnote == null)
					{
						xWPFFootnote = document.GetEndnoteByID(int.Parse(cT_FtnEdnRef.id));
					}
				}
				if (xWPFFootnote != null)
				{
					bool flag = true;
					foreach (XWPFParagraph paragraph in xWPFFootnote.Paragraphs)
					{
						if (!flag)
						{
							footnoteText.Append("\n");
							flag = false;
						}
						footnoteText.Append(paragraph.Text);
					}
				}
				footnoteText.Append("]");
			}
		}
	}

	private void BuildRunsInOrderFromXml(ArrayList items)
	{
		foreach (object item8 in items)
		{
			if (item8 is NPOI.OpenXmlFormats.Wordprocessing.CT_R)
			{
				XWPFRun item = new XWPFRun((NPOI.OpenXmlFormats.Wordprocessing.CT_R)item8, (IRunBody)this);
				runs.Add(item);
				iRuns.Add(item);
			}
			if (item8 is CT_Hyperlink1)
			{
				CT_Hyperlink1 cT_Hyperlink = (CT_Hyperlink1)item8;
				foreach (NPOI.OpenXmlFormats.Wordprocessing.CT_R r in cT_Hyperlink.GetRList())
				{
					XWPFHyperlinkRun item2 = new XWPFHyperlinkRun(cT_Hyperlink, r, this);
					runs.Add(item2);
					iRuns.Add(item2);
				}
			}
			if (item8 is CT_SimpleField)
			{
				CT_SimpleField cT_SimpleField = (CT_SimpleField)item8;
				foreach (NPOI.OpenXmlFormats.Wordprocessing.CT_R r2 in cT_SimpleField.GetRList())
				{
					XWPFFieldRun item3 = new XWPFFieldRun(cT_SimpleField, r2, this);
					runs.Add(item3);
					iRuns.Add(item3);
				}
			}
			if (item8 is CT_SdtBlock)
			{
				XWPFSDT item4 = new XWPFSDT((CT_SdtBlock)item8, part);
				iRuns.Add(item4);
			}
			if (item8 is CT_SdtRun)
			{
				XWPFSDT item5 = new XWPFSDT((CT_SdtRun)item8, part);
				iRuns.Add(item5);
			}
			if (item8 is CT_RunTrackChange)
			{
				foreach (NPOI.OpenXmlFormats.Wordprocessing.CT_R r3 in ((CT_RunTrackChange)item8).GetRList())
				{
					XWPFRun item6 = new XWPFRun(r3, (IRunBody)this);
					runs.Add(item6);
					iRuns.Add(item6);
				}
			}
			if (item8 is CT_SmartTagRun)
			{
				BuildRunsInOrderFromXml((item8 as CT_SmartTagRun).Items);
			}
			if (!(item8 is CT_RunTrackChange))
			{
				continue;
			}
			foreach (CT_RunTrackChange ins in ((CT_RunTrackChange)item8).GetInsList())
			{
				foreach (NPOI.OpenXmlFormats.Wordprocessing.CT_R r4 in ins.GetRList())
				{
					XWPFRun item7 = new XWPFRun(r4, (IRunBody)this);
					runs.Add(item7);
					iRuns.Add(item7);
				}
			}
		}
	}

	private void BuildOMathsInOrderFromXml(ArrayList items)
	{
		foreach (object item in items)
		{
			if (item is CT_OMath)
			{
				oMaths.Add(new XWPFOMath(item as CT_OMath, this));
			}
		}
	}

	public CT_P GetCTP()
	{
		return paragraph;
	}

	public string GetNumID()
	{
		if (paragraph.pPr != null && paragraph.pPr.numPr != null && paragraph.pPr.numPr.numId != null)
		{
			return paragraph.pPr.numPr.numId.val;
		}
		return null;
	}

	public void SetNumILvl(string iLvl)
	{
		if (paragraph.pPr == null)
		{
			paragraph.AddNewPPr();
		}
		if (paragraph.pPr.numPr == null)
		{
			paragraph.pPr.AddNewNumPr();
		}
		if (paragraph.pPr.numPr.ilvl == null)
		{
			paragraph.pPr.numPr.AddNewIlvl();
		}
		paragraph.pPr.numPr.ilvl.val = iLvl;
	}

	public string GetNumIlvl()
	{
		if (paragraph.pPr != null && paragraph.pPr.numPr != null && paragraph.pPr.numPr.ilvl != null)
		{
			return paragraph.pPr.numPr.ilvl.val;
		}
		return null;
	}

	public string GetNumFmt()
	{
		string numID = GetNumID();
		XWPFNumbering numbering = document.GetNumbering();
		if (numID != null && numbering != null)
		{
			XWPFNum num = numbering.GetNum(numID);
			if (num != null)
			{
				string numIlvl = GetNumIlvl();
				string val = num.GetCTNum().abstractNumId.val;
				CT_AbstractNum abstractNum = numbering.GetAbstractNum(val).GetAbstractNum();
				CT_Lvl cT_Lvl = null;
				for (int i = 0; i < abstractNum.lvl.Count; i++)
				{
					CT_Lvl cT_Lvl2 = abstractNum.lvl[i];
					if (cT_Lvl2.ilvl.Equals(numIlvl))
					{
						cT_Lvl = cT_Lvl2;
						break;
					}
				}
				if (cT_Lvl != null && cT_Lvl.numFmt != null)
				{
					return cT_Lvl.numFmt.val.ToString();
				}
			}
		}
		return null;
	}

	public string GetNumStartOverride()
	{
		string numID = GetNumID();
		XWPFNumbering xWPFNumbering = document.CreateNumbering();
		if (numID != null && xWPFNumbering != null)
		{
			XWPFNum num = xWPFNumbering.GetNum(numID);
			if (num != null)
			{
				CT_Num cTNum = num.GetCTNum();
				if (cTNum == null)
				{
					return null;
				}
				string numIlvl = GetNumIlvl();
				CT_NumLvl cT_NumLvl = null;
				for (int i = 0; i < cTNum.SizeOfLvlOverrideArray(); i++)
				{
					CT_NumLvl lvlOverrideArray = cTNum.GetLvlOverrideArray(i);
					if (lvlOverrideArray != null && lvlOverrideArray.ilvl != null && lvlOverrideArray.ilvl.Equals(numIlvl))
					{
						cT_NumLvl = lvlOverrideArray;
						break;
					}
				}
				if (cT_NumLvl != null && cT_NumLvl.startOverride != null)
				{
					return cT_NumLvl.startOverride.val;
				}
			}
		}
		return null;
	}

	public void SetNumID(string numId)
	{
		if (paragraph.pPr == null)
		{
			paragraph.AddNewPPr();
		}
		if (paragraph.pPr.numPr == null)
		{
			paragraph.pPr.AddNewNumPr();
		}
		if (paragraph.pPr.numPr.numId == null)
		{
			paragraph.pPr.numPr.AddNewNumId();
		}
		paragraph.pPr.numPr.ilvl = new CT_DecimalNumber();
		paragraph.pPr.numPr.ilvl.val = "0";
		paragraph.pPr.numPr.numId.val = numId;
	}

	public void SetNumID(string numId, string ilvl)
	{
		if (paragraph.pPr == null)
		{
			paragraph.AddNewPPr();
		}
		if (paragraph.pPr.numPr == null)
		{
			paragraph.pPr.AddNewNumPr();
		}
		if (paragraph.pPr.numPr.numId == null)
		{
			paragraph.pPr.numPr.AddNewNumId();
		}
		paragraph.pPr.numPr.ilvl = new CT_DecimalNumber();
		paragraph.pPr.numPr.ilvl.val = ilvl;
		paragraph.pPr.numPr.numId.val = numId;
	}

	public void setSpacingBetween(double spacing, LineSpacingRule rule)
	{
		CT_Spacing cTSpacing = GetCTSpacing(create: true);
		if (rule == LineSpacingRule.AUTO)
		{
			cTSpacing.line = Math.Round(spacing * 240.0).ToString();
		}
		else
		{
			cTSpacing.line = Math.Round(spacing * 20.0).ToString();
		}
		cTSpacing.lineRule = EnumConverter.ValueOf<ST_LineSpacingRule, LineSpacingRule>(rule);
	}

	private CT_PBdr GetCTPBrd(bool create)
	{
		CT_PPr cTPPr = GetCTPPr();
		CT_PBdr cT_PBdr = (cTPPr.IsSetPBdr() ? cTPPr.pBdr : null);
		if (create && cT_PBdr == null)
		{
			cT_PBdr = cTPPr.AddNewPBdr();
		}
		return cT_PBdr;
	}

	private CT_Spacing GetCTSpacing(bool create)
	{
		CT_PPr cTPPr = GetCTPPr();
		CT_Spacing cT_Spacing = ((cTPPr.spacing == null) ? null : cTPPr.spacing);
		if (create && cT_Spacing == null)
		{
			cT_Spacing = cTPPr.AddNewSpacing();
		}
		return cT_Spacing;
	}

	private CT_Ind GetCTInd(bool create)
	{
		CT_PPr cTPPr = GetCTPPr();
		CT_Ind cT_Ind = ((cTPPr.ind == null) ? null : cTPPr.ind);
		if (create && cT_Ind == null)
		{
			cT_Ind = cTPPr.AddNewInd();
		}
		return cT_Ind;
	}

	internal CT_PPr GetCTPPr()
	{
		if (paragraph.pPr != null)
		{
			return paragraph.pPr;
		}
		return paragraph.AddNewPPr();
	}

	protected internal void AddRun(NPOI.OpenXmlFormats.Wordprocessing.CT_R run)
	{
		int count = paragraph.GetRList().Count;
		paragraph.AddNewR();
		paragraph.SetRArray(count, run);
	}

	public void ReplaceText(string oldText, string newText)
	{
		if (string.IsNullOrEmpty(oldText))
		{
			throw new ArgumentNullException("oldText should not be null");
		}
		TextSegment textSegment = SearchText(oldText, new PositionInParagraph
		{
			Run = 0
		});
		if (textSegment == null)
		{
			return;
		}
		if (textSegment.BeginRun == textSegment.EndRun)
		{
			runs[textSegment.BeginRun].ReplaceText(oldText, newText);
			return;
		}
		runs[textSegment.BeginRun].ReplaceText(runs[textSegment.BeginRun].Text.Substring(textSegment.BeginChar), newText);
		runs[textSegment.EndRun].ReplaceText(runs[textSegment.EndRun].Text.Substring(0, textSegment.EndChar + 1), "");
		for (int num = textSegment.EndRun - 1; num > textSegment.BeginRun; num--)
		{
			RemoveRun(num);
		}
	}

	public TextSegment SearchText(string searched, PositionInParagraph startPos)
	{
		int run = startPos.Run;
		int text = startPos.Text;
		int num = startPos.Char;
		int beginRun = 0;
		int beginText = 0;
		int beginChar = 0;
		int num2 = 0;
		bool flag = false;
		for (int i = run; i < paragraph.GetRList().Count; i++)
		{
			int num3 = 0;
			int num4 = 0;
			foreach (object item in paragraph.GetRList()[i].Items)
			{
				if (item is CT_Text)
				{
					if (num3 >= text)
					{
						string value = ((CT_Text)item).Value;
						for (num4 = ((i == run) ? num : 0); num4 < value.Length; num4++)
						{
							if (value[num4] == searched[0] && num2 == 0)
							{
								beginText = num3;
								beginChar = num4;
								beginRun = i;
								flag = true;
							}
							if (value[num4] == searched[num2])
							{
								if (num2 + 1 < searched.Length)
								{
									num2++;
								}
								else if (flag)
								{
									return new TextSegment
									{
										BeginRun = beginRun,
										BeginText = beginText,
										BeginChar = beginChar,
										EndRun = i,
										EndText = num3,
										EndChar = num4
									};
								}
							}
							else
							{
								num2 = 0;
							}
						}
					}
					num3++;
				}
				else if (!(item is CT_ProofErr) && !(item is CT_RPr))
				{
					num2 = 0;
				}
			}
		}
		return null;
	}

	public XWPFRun CreateRun()
	{
		XWPFRun xWPFRun = new XWPFRun(paragraph.AddNewR(), (IRunBody)this);
		runs.Add(xWPFRun);
		iRuns.Add(xWPFRun);
		return xWPFRun;
	}

	public XWPFOMath CreateOMath()
	{
		XWPFOMath xWPFOMath = new XWPFOMath(paragraph.AddNewOMath(), this);
		oMaths.Add(xWPFOMath);
		return xWPFOMath;
	}

	public XWPFRun InsertNewRun(int pos)
	{
		if (pos >= 0 && pos <= runs.Count)
		{
			int num = 0;
			for (int i = 0; i < pos; i++)
			{
				XWPFRun xWPFRun = runs[i];
				if (!(xWPFRun is XWPFHyperlinkRun) && !(xWPFRun is XWPFFieldRun))
				{
					num++;
				}
			}
			XWPFRun xWPFRun2 = new XWPFRun(paragraph.InsertNewR(num), (IRunBody)this);
			int index = iRuns.Count;
			if (pos < runs.Count)
			{
				XWPFRun item = runs[pos];
				int num2 = iRuns.IndexOf(item);
				if (num2 != -1)
				{
					index = num2;
				}
			}
			iRuns.Insert(index, xWPFRun2);
			runs.Insert(pos, xWPFRun2);
			return xWPFRun2;
		}
		return null;
	}

	public string GetText(TextSegment segment)
	{
		int beginRun = segment.BeginRun;
		int beginText = segment.BeginText;
		int beginChar = segment.BeginChar;
		int endRun = segment.EndRun;
		int endText = segment.EndText;
		int endChar = segment.EndChar;
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = beginRun; i <= endRun; i++)
		{
			int num = 0;
			int num2 = paragraph.GetRList()[i].GetTList().Count - 1;
			if (i == beginRun)
			{
				num = beginText;
			}
			if (i == endRun)
			{
				num2 = endText;
			}
			for (int j = num; j <= num2; j++)
			{
				string value = paragraph.GetRList()[i].GetTArray(j).Value;
				int num3 = 0;
				int num4 = value.Length - 1;
				if (j == beginText && i == beginRun)
				{
					num3 = beginChar;
				}
				if (j == endText && i == endRun)
				{
					num4 = endChar;
				}
				stringBuilder.Append(value.Substring(num3, num4 - num3 + 1));
			}
		}
		return stringBuilder.ToString();
	}

	public bool RemoveRun(int pos)
	{
		if (pos >= 0 && pos < runs.Count)
		{
			XWPFRun xWPFRun = runs[pos];
			if (xWPFRun is XWPFHyperlinkRun || xWPFRun is XWPFFieldRun)
			{
				throw new ArgumentException("Removing Field or Hyperlink runs not yet supported");
			}
			runs.RemoveAt(pos);
			iRuns.Remove(xWPFRun);
			int num = 0;
			for (int i = 0; i < pos; i++)
			{
				XWPFRun xWPFRun2 = runs[i];
				if (!(xWPFRun2 is XWPFHyperlinkRun) && !(xWPFRun2 is XWPFFieldRun))
				{
					num++;
				}
			}
			GetCTP().RemoveR(pos);
			return true;
		}
		return false;
	}

	public void AddRun(XWPFRun r)
	{
		if (!runs.Contains(r))
		{
			runs.Add(r);
		}
	}

	public XWPFRun GetRun(NPOI.OpenXmlFormats.Wordprocessing.CT_R r)
	{
		for (int i = 0; i < runs.Count; i++)
		{
			if (runs[i].GetCTR() == r)
			{
				return runs[i];
			}
		}
		return null;
	}

	public XWPFHyperlinkRun CreateHyperlinkRun(string rId)
	{
		NPOI.OpenXmlFormats.Wordprocessing.CT_R cT_R = new NPOI.OpenXmlFormats.Wordprocessing.CT_R();
		cT_R.AddNewRPr().rStyle = new NPOI.OpenXmlFormats.Wordprocessing.CT_String
		{
			val = "Hyperlink"
		};
		CT_Hyperlink1 cT_Hyperlink = paragraph.AddNewHyperlink();
		cT_Hyperlink.history = NPOI.OpenXmlFormats.Wordprocessing.ST_OnOff.on;
		cT_Hyperlink.id = rId;
		cT_Hyperlink.Items.Add(cT_R);
		XWPFHyperlinkRun xWPFHyperlinkRun = new XWPFHyperlinkRun(cT_Hyperlink, cT_R, this);
		runs.Add(xWPFHyperlinkRun);
		iRuns.Add(xWPFHyperlinkRun);
		return xWPFHyperlinkRun;
	}
}

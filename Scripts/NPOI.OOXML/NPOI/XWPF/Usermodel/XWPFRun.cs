using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXmlFormats.Dml;
using NPOI.OpenXmlFormats.Dml.Picture;
using NPOI.OpenXmlFormats.Dml.WordProcessing;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.Util;
using NPOI.WP.UserModel;

namespace NPOI.XWPF.UserModel;

public class XWPFRun : ISDTContents, IRunElement, ICharacterRun
{
	private CT_R run;

	private string pictureText;

	private IRunBody parent;

	private List<XWPFPicture> pictures;

	public IRunBody Parent => parent;

	public XWPFParagraph Paragraph
	{
		get
		{
			if (parent is XWPFParagraph)
			{
				return (XWPFParagraph)parent;
			}
			return null;
		}
	}

	public XWPFDocument Document
	{
		get
		{
			if (parent != null)
			{
				return parent.Document;
			}
			return null;
		}
	}

	public bool IsBold
	{
		get
		{
			CT_RPr rPr = run.rPr;
			if (rPr == null || !rPr.IsSetB())
			{
				return false;
			}
			return IsCTOnOff(rPr.b);
		}
		set
		{
			CT_RPr cT_RPr = (run.IsSetRPr() ? run.rPr : run.AddNewRPr());
			(cT_RPr.IsSetB() ? cT_RPr.b : cT_RPr.AddNewB()).val = value;
		}
	}

	public string PictureText => pictureText;

	public bool IsItalic
	{
		get
		{
			CT_RPr rPr = run.rPr;
			if (rPr == null || !rPr.IsSetI())
			{
				return false;
			}
			return IsCTOnOff(rPr.i);
		}
		set
		{
			CT_RPr cT_RPr = (run.IsSetRPr() ? run.rPr : run.AddNewRPr());
			(cT_RPr.IsSetI() ? cT_RPr.i : cT_RPr.AddNewI()).val = value;
		}
	}

	public UnderlinePatterns Underline
	{
		get
		{
			CT_RPr rPr = run.rPr;
			if (rPr != null && rPr.IsSetU())
			{
				_ = rPr.u.val;
				return EnumConverter.ValueOf<UnderlinePatterns, ST_Underline>(rPr.u.val);
			}
			return UnderlinePatterns.None;
		}
	}

	public string Text
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < run.Items.Count; i++)
			{
				object obj = run.Items[i];
				if (obj is CT_Text && run.ItemsElementName[i] != RunItemsChoiceType.instrText)
				{
					stringBuilder.Append(((CT_Text)obj).Value);
				}
				if (obj is CT_FldChar)
				{
					CT_FldChar cT_FldChar = (CT_FldChar)obj;
					if (cT_FldChar.fldCharType == ST_FldCharType.begin && cT_FldChar.ffData != null)
					{
						foreach (CT_FFCheckBox checkBox in cT_FldChar.ffData.GetCheckBoxList())
						{
							if (checkBox.@default.val)
							{
								stringBuilder.Append("|X|");
							}
							else
							{
								stringBuilder.Append("|_|");
							}
						}
					}
				}
				if (obj is CT_PTab)
				{
					stringBuilder.Append("\t");
				}
				if (obj is CT_Br)
				{
					stringBuilder.Append("\n");
				}
				if (obj is CT_Empty)
				{
					if (run.ItemsElementName[i] == RunItemsChoiceType.tab)
					{
						stringBuilder.Append("\t");
					}
					if (run.ItemsElementName[i] == RunItemsChoiceType.br)
					{
						stringBuilder.Append("\n");
					}
					if (run.ItemsElementName[i] == RunItemsChoiceType.cr)
					{
						stringBuilder.Append("\n");
					}
				}
				if (obj is CT_FtnEdnRef)
				{
					CT_FtnEdnRef cT_FtnEdnRef = (CT_FtnEdnRef)obj;
					string value = (cT_FtnEdnRef.DomNode.LocalName.Equals("footnoteReference") ? ("[footnoteRef:" + cT_FtnEdnRef.id + "]") : ("[endnoteRef:" + cT_FtnEdnRef.id + "]"));
					stringBuilder.Append(value);
				}
			}
			if (pictureText != null && pictureText.Length > 0)
			{
				stringBuilder.Append("\n").Append(pictureText);
			}
			return stringBuilder.ToString();
		}
	}

	public bool IsStrikeThrough
	{
		get
		{
			CT_RPr rPr = run.rPr;
			if (rPr == null || !rPr.IsSetStrike())
			{
				return false;
			}
			return IsCTOnOff(rPr.strike);
		}
		set
		{
			CT_RPr cT_RPr = (run.IsSetRPr() ? run.rPr : run.AddNewRPr());
			(cT_RPr.IsSetStrike() ? cT_RPr.strike : cT_RPr.AddNewStrike()).val = value;
		}
	}

	[Obsolete]
	public bool IsStrike
	{
		get
		{
			return IsStrikeThrough;
		}
		set
		{
			IsStrikeThrough = value;
		}
	}

	public bool IsDoubleStrikeThrough
	{
		get
		{
			CT_RPr rPr = run.rPr;
			if (rPr == null || !rPr.IsSetDstrike())
			{
				return false;
			}
			return IsCTOnOff(rPr.dstrike);
		}
		set
		{
			CT_RPr cT_RPr = (run.IsSetRPr() ? run.rPr : run.AddNewRPr());
			(cT_RPr.IsSetDstrike() ? cT_RPr.dstrike : cT_RPr.AddNewDstrike()).val = value;
		}
	}

	public bool IsSmallCaps
	{
		get
		{
			CT_RPr rPr = run.rPr;
			if (rPr == null || !rPr.IsSetSmallCaps())
			{
				return false;
			}
			return IsCTOnOff(rPr.smallCaps);
		}
		set
		{
			CT_RPr cT_RPr = (run.IsSetRPr() ? run.rPr : run.AddNewRPr());
			(cT_RPr.IsSetSmallCaps() ? cT_RPr.smallCaps : cT_RPr.AddNewSmallCaps()).val = value;
		}
	}

	public bool IsCapitalized
	{
		get
		{
			CT_RPr rPr = run.rPr;
			if (rPr == null || !rPr.IsSetCaps())
			{
				return false;
			}
			return IsCTOnOff(rPr.caps);
		}
		set
		{
			CT_RPr cT_RPr = (run.IsSetRPr() ? run.rPr : run.AddNewRPr());
			(cT_RPr.IsSetCaps() ? cT_RPr.caps : cT_RPr.AddNewCaps()).val = value;
		}
	}

	public bool IsShadowed
	{
		get
		{
			CT_RPr rPr = run.rPr;
			if (rPr == null || !rPr.IsSetShadow())
			{
				return false;
			}
			return IsCTOnOff(rPr.shadow);
		}
		set
		{
			CT_RPr cT_RPr = (run.IsSetRPr() ? run.rPr : run.AddNewRPr());
			(cT_RPr.IsSetShadow() ? cT_RPr.shadow : cT_RPr.AddNewShadow()).val = value;
		}
	}

	public bool IsImprinted
	{
		get
		{
			CT_RPr rPr = run.rPr;
			if (rPr == null || !rPr.IsSetImprint())
			{
				return false;
			}
			return IsCTOnOff(rPr.imprint);
		}
		set
		{
			CT_RPr cT_RPr = (run.IsSetRPr() ? run.rPr : run.AddNewRPr());
			(cT_RPr.IsSetImprint() ? cT_RPr.imprint : cT_RPr.AddNewImprint()).val = value;
		}
	}

	public bool IsEmbossed
	{
		get
		{
			CT_RPr rPr = run.rPr;
			if (rPr == null || !rPr.IsSetEmboss())
			{
				return false;
			}
			return IsCTOnOff(rPr.emboss);
		}
		set
		{
			CT_RPr cT_RPr = (run.IsSetRPr() ? run.rPr : run.AddNewRPr());
			(cT_RPr.IsSetEmboss() ? cT_RPr.emboss : cT_RPr.AddNewEmboss()).val = value;
		}
	}

	public VerticalAlign Subscript
	{
		get
		{
			CT_RPr rPr = run.rPr;
			if (rPr == null || !rPr.IsSetVertAlign())
			{
				return VerticalAlign.BASELINE;
			}
			return EnumConverter.ValueOf<VerticalAlign, ST_VerticalAlignRun>(rPr.vertAlign.val);
		}
		set
		{
			CT_RPr cT_RPr = (run.IsSetRPr() ? run.rPr : run.AddNewRPr());
			(cT_RPr.IsSetVertAlign() ? cT_RPr.vertAlign : cT_RPr.AddNewVertAlign()).val = EnumConverter.ValueOf<ST_VerticalAlignRun, VerticalAlign>(value);
		}
	}

	public int Kerning
	{
		get
		{
			CT_RPr rPr = run.rPr;
			if (rPr == null || !rPr.IsSetKern())
			{
				return 0;
			}
			return (int)rPr.kern.val;
		}
		set
		{
			CT_RPr cT_RPr = (run.IsSetRPr() ? run.rPr : run.AddNewRPr());
			(cT_RPr.IsSetKern() ? cT_RPr.kern : cT_RPr.AddNewKern()).val = (ulong)value;
		}
	}

	public bool IsHighlighted
	{
		get
		{
			CT_RPr rPr = run.rPr;
			if (rPr == null || !rPr.IsSetHighlight())
			{
				return false;
			}
			if (rPr.highlight.val == ST_HighlightColor.none)
			{
				return false;
			}
			return true;
		}
	}

	public int CharacterSpacing
	{
		get
		{
			CT_RPr rPr = run.rPr;
			if (rPr == null || !rPr.IsSetSpacing())
			{
				return 0;
			}
			return int.Parse(rPr.spacing.val);
		}
		set
		{
			CT_RPr cT_RPr = (run.IsSetRPr() ? run.rPr : run.AddNewRPr());
			(cT_RPr.IsSetSpacing() ? cT_RPr.spacing : cT_RPr.AddNewSpacing()).val = value.ToString();
		}
	}

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

	public string FontName => FontFamily;

	public double FontSize
	{
		get
		{
			CT_RPr rPr = run.rPr;
			if (rPr == null || !rPr.IsSetSz())
			{
				return -1.0;
			}
			return (double)rPr.sz.val / 2.0;
		}
		set
		{
			CT_RPr cT_RPr = (run.IsSetRPr() ? run.rPr : run.AddNewRPr());
			(cT_RPr.IsSetSz() ? cT_RPr.sz : cT_RPr.AddNewSz()).val = (ulong)(value * 2.0);
		}
	}

	public int TextPosition
	{
		get
		{
			CT_RPr rPr = run.rPr;
			if (rPr == null || !rPr.IsSetPosition())
			{
				return -1;
			}
			return int.Parse(rPr.position.val);
		}
		set
		{
			CT_RPr cT_RPr = (run.IsSetRPr() ? run.rPr : run.AddNewRPr());
			(cT_RPr.IsSetPosition() ? cT_RPr.position : cT_RPr.AddNewPosition()).val = value.ToString();
		}
	}

	public XWPFRun(CT_R r, IRunBody p)
	{
		run = r;
		parent = p;
		IList<CT_Drawing> drawingList = r.GetDrawingList();
		foreach (CT_Drawing item2 in drawingList)
		{
			foreach (CT_Anchor anchor in item2.GetAnchorList())
			{
				if (anchor.docPr != null)
				{
					Document.DrawingIdManager.Reserve(anchor.docPr.id);
				}
			}
			foreach (CT_Inline inline in item2.GetInlineList())
			{
				if (inline.docPr != null)
				{
					Document.DrawingIdManager.Reserve(inline.docPr.id);
				}
			}
		}
		StringBuilder stringBuilder = new StringBuilder();
		List<object> list = new List<object>();
		foreach (NPOI.OpenXmlFormats.Wordprocessing.CT_Picture pict in r.GetPictList())
		{
			list.Add(pict);
		}
		foreach (CT_Drawing item3 in drawingList)
		{
			list.Add(item3);
		}
		pictureText = stringBuilder.ToString();
		pictures = new List<XWPFPicture>();
		foreach (object item4 in list)
		{
			foreach (NPOI.OpenXmlFormats.Dml.Picture.CT_Picture cTPicture in GetCTPictures(item4))
			{
				XWPFPicture item = new XWPFPicture(cTPicture, this);
				pictures.Add(item);
			}
		}
	}

	[Obsolete("Use XWPFRun(CTR, IRunBody)")]
	public XWPFRun(CT_R r, XWPFParagraph p)
		: this(r, (IRunBody)p)
	{
	}

	private List<NPOI.OpenXmlFormats.Dml.Picture.CT_Picture> GetCTPictures(object o)
	{
		List<NPOI.OpenXmlFormats.Dml.Picture.CT_Picture> result = new List<NPOI.OpenXmlFormats.Dml.Picture.CT_Picture>();
		if (o is CT_Drawing)
		{
			CT_Drawing cT_Drawing = o as CT_Drawing;
			if (cT_Drawing.inline != null)
			{
				foreach (CT_Inline item in cT_Drawing.inline)
				{
					GetPictures(item.graphic.graphicData, result);
				}
			}
		}
		else if (o is CT_GraphicalObjectData)
		{
			GetPictures(o as CT_GraphicalObjectData, result);
		}
		return result;
	}

	private void GetPictures(CT_GraphicalObjectData god, List<NPOI.OpenXmlFormats.Dml.Picture.CT_Picture> pictures)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(NPOI.OpenXmlFormats.Dml.Picture.CT_Picture));
		foreach (string item2 in god.Any)
		{
			if (item2.IndexOf("pic:pic") >= 0)
			{
				StringReader input = new StringReader(item2);
				NPOI.OpenXmlFormats.Dml.Picture.CT_Picture item = xmlSerializer.Deserialize(XmlReader.Create(input)) as NPOI.OpenXmlFormats.Dml.Picture.CT_Picture;
				pictures.Add(item);
			}
		}
	}

	public CT_R GetCTR()
	{
		return run;
	}

	private bool IsCTOnOff(CT_OnOff onoff)
	{
		if (!onoff.IsSetVal())
		{
			return true;
		}
		return onoff.val;
	}

	public string GetColor()
	{
		string result = null;
		if (run.IsSetRPr())
		{
			CT_RPr rPr = run.rPr;
			if (rPr.IsSetColor())
			{
				result = rPr.color.val;
			}
		}
		return result;
	}

	public void SetColor(string rgbStr)
	{
		CT_RPr cT_RPr = (run.IsSetRPr() ? run.rPr : run.AddNewRPr());
		(cT_RPr.IsSetColor() ? cT_RPr.color : cT_RPr.AddNewColor()).val = rgbStr;
	}

	public string GetText(int pos)
	{
		if (run.SizeOfTArray() != 0)
		{
			return run.GetTArray(pos).Value;
		}
		return null;
	}

	public void ReplaceText(string oldText, string newText)
	{
		string text = Text.Replace(oldText, newText);
		SetText(text);
	}

	public void SetText(string value)
	{
		SetText(value, 0);
	}

	public void AppendText(string value)
	{
		SetText(value, run.GetTList().Count);
	}

	public void SetText(string value, int pos)
	{
		int num = run.SizeOfTArray();
		if (pos > num)
		{
			throw new IndexOutOfRangeException("Value too large for the parameter position");
		}
		CT_Text obj = ((pos < num && pos >= 0) ? run.GetTArray(pos) : run.AddNewT());
		obj.Value = value;
		preserveSpaces(obj);
	}

	internal void InsertText(CT_Text text, int textIndex)
	{
		run.GetTList().Insert(textIndex, text);
	}

	public void InsertText(string text, int startIndex)
	{
		List<CT_Text> tList = run.GetTList();
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < tList.Count; i++)
		{
			num2 = num;
			num += tList[i].Value.Length;
			if (num > startIndex)
			{
				tList[i].Value = tList[i].Value.Insert(startIndex - num2, text);
				break;
			}
		}
	}

	public void SetUnderline(UnderlinePatterns value)
	{
		CT_RPr cT_RPr = (run.IsSetRPr() ? run.rPr : run.AddNewRPr());
		((cT_RPr.u == null) ? cT_RPr.AddNewU() : cT_RPr.u).val = EnumConverter.ValueOf<ST_Underline, UnderlinePatterns>(value);
	}

	[Obsolete]
	public void SetStrike(bool value)
	{
		CT_RPr cT_RPr = (run.IsSetRPr() ? run.rPr : run.AddNewRPr());
		(cT_RPr.IsSetStrike() ? cT_RPr.strike : cT_RPr.AddNewStrike()).val = value;
	}

	public string GetFontFamily(FontCharRange fcr)
	{
		CT_RPr rPr = run.rPr;
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

	public void SetFontFamily(string fontFamily, FontCharRange fcr)
	{
		CT_RPr cT_RPr = (run.IsSetRPr() ? run.rPr : run.AddNewRPr());
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

	public void RemoveBreak()
	{
	}

	public void AddBreak()
	{
		run.AddNewBr();
	}

	public void AddBreak(BreakType type)
	{
		run.AddNewBr().type = EnumConverter.ValueOf<ST_BrType, BreakType>(type);
	}

	public void AddBreak(BreakClear Clear)
	{
		CT_Br cT_Br = run.AddNewBr();
		cT_Br.type = EnumConverter.ValueOf<ST_BrType, BreakType>(BreakType.TEXTWRAPPING);
		cT_Br.clear = EnumConverter.ValueOf<ST_BrClear, BreakClear>(Clear);
	}

	public void AddTab()
	{
		run.AddNewTab();
	}

	public void RemoveTab()
	{
	}

	public void AddCarriageReturn()
	{
		run.AddNewCr();
	}

	public void RemoveCarriageReturn(int i)
	{
		throw new NotImplementedException();
	}

	private XWPFPicture AddPicture(Stream pictureData, int pictureType, string filename, int width, int height, Action<XWPFDocument, CT_Blip> extAct)
	{
		XWPFDocument xWPFDocument = null;
		XWPFPictureData xWPFPictureData;
		if (parent.Part is XWPFHeaderFooter)
		{
			XWPFHeaderFooter obj = (XWPFHeaderFooter)parent.Part;
			string id = obj.AddPictureData(pictureData, pictureType);
			xWPFPictureData = (XWPFPictureData)obj.GetRelationById(id);
		}
		else
		{
			xWPFDocument = parent.Document;
			string id = xWPFDocument.AddPictureData(pictureData, pictureType);
			xWPFPictureData = (XWPFPictureData)xWPFDocument.GetRelationById(id);
		}
		try
		{
			CT_Inline cT_Inline = run.AddNewDrawing().AddNewInline();
			cT_Inline.graphic = new CT_GraphicalObject();
			cT_Inline.graphic.graphicData = new CT_GraphicalObjectData();
			cT_Inline.graphic.graphicData.uri = "http://schemas.openxmlformats.org/drawingml/2006/picture";
			cT_Inline.distT = 0u;
			cT_Inline.distR = 0u;
			cT_Inline.distB = 0u;
			cT_Inline.distL = 0u;
			NPOI.OpenXmlFormats.Dml.WordProcessing.CT_NonVisualDrawingProps cT_NonVisualDrawingProps = cT_Inline.AddNewDocPr();
			long num = parent.Document.DrawingIdManager.ReserveNew();
			cT_NonVisualDrawingProps.id = (uint)num;
			cT_NonVisualDrawingProps.name = "Drawing " + num;
			cT_NonVisualDrawingProps.descr = filename;
			NPOI.OpenXmlFormats.Dml.WordProcessing.CT_PositiveSize2D cT_PositiveSize2D = cT_Inline.AddNewExtent();
			cT_PositiveSize2D.cx = width;
			cT_PositiveSize2D.cy = height;
			NPOI.OpenXmlFormats.Dml.Picture.CT_Picture cT_Picture = new NPOI.OpenXmlFormats.Dml.Picture.CT_Picture();
			CT_PictureNonVisual cT_PictureNonVisual = cT_Picture.AddNewNvPicPr();
			NPOI.OpenXmlFormats.Dml.CT_NonVisualDrawingProps cT_NonVisualDrawingProps2 = cT_PictureNonVisual.AddNewCNvPr();
			cT_NonVisualDrawingProps2.id = 0u;
			cT_NonVisualDrawingProps2.name = "Picture " + num;
			cT_NonVisualDrawingProps2.descr = filename;
			cT_PictureNonVisual.AddNewCNvPicPr().AddNewPicLocks().noChangeAspect = true;
			CT_BlipFillProperties cT_BlipFillProperties = cT_Picture.AddNewBlipFill();
			CT_Blip cT_Blip = cT_BlipFillProperties.AddNewBlip();
			cT_Blip.embed = xWPFPictureData.GetPackageRelationship().Id;
			if (xWPFDocument != null)
			{
				extAct(xWPFDocument, cT_Blip);
			}
			cT_BlipFillProperties.AddNewStretch().AddNewFillRect();
			CT_ShapeProperties cT_ShapeProperties = cT_Picture.AddNewSpPr();
			CT_Transform2D cT_Transform2D = cT_ShapeProperties.AddNewXfrm();
			CT_Point2D cT_Point2D = cT_Transform2D.AddNewOff();
			cT_Point2D.x = 0L;
			cT_Point2D.y = 0L;
			NPOI.OpenXmlFormats.Dml.CT_PositiveSize2D cT_PositiveSize2D2 = cT_Transform2D.AddNewExt();
			cT_PositiveSize2D2.cx = width;
			cT_PositiveSize2D2.cy = height;
			CT_PresetGeometry2D cT_PresetGeometry2D = cT_ShapeProperties.AddNewPrstGeom();
			cT_PresetGeometry2D.prst = ST_ShapeType.rect;
			cT_PresetGeometry2D.AddNewAvLst();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				StreamWriter streamWriter = new StreamWriter(memoryStream);
				cT_Picture.Write(streamWriter, "pic:pic");
				streamWriter.Flush();
				memoryStream.Position = 0L;
				string el = new StreamReader(memoryStream).ReadToEnd();
				cT_Inline.graphic.graphicData.AddPicElement(el);
			}
			XWPFPicture xWPFPicture = new XWPFPicture(cT_Picture, this);
			pictures.Add(xWPFPicture);
			return xWPFPicture;
		}
		catch (XmlException innerException)
		{
			throw new InvalidOperationException("XWPFRun.Addpicture error", innerException);
		}
	}

	public XWPFPicture AddSvg(Stream svgData, Stream altPictureData, int altPictureType, string filename, int width, int height)
	{
		return AddPicture(altPictureData, altPictureType, filename, width, height, (XWPFDocument doc, CT_Blip blip) =>
		{
			string id = doc.AddPictureData(svgData, 13);
			XWPFPictureData xWPFPictureData = (XWPFPictureData)doc.GetRelationById(id);
			CT_OfficeArtExtensionList cT_OfficeArtExtensionList = new CT_OfficeArtExtensionList();
			CT_OfficeArtExtension item = new CT_OfficeArtExtension
			{
				uri = "{96DAC541-7B7A-43D3-8B79-37D633B846F1}",
				Any = "<asvg:svgBlip xmlns:asvg=\"http://schemas.microsoft.com/office/drawing/2016/SVG/main\" r:embed=\"" + xWPFPictureData.GetPackageRelationship().Id + "\"/>"
			};
			cT_OfficeArtExtensionList.ext.Add(item);
			blip.extLst = cT_OfficeArtExtensionList;
		});
	}

	public XWPFPicture AddPicture(Stream pictureData, int pictureType, string filename, int width, int height)
	{
		return AddPicture(pictureData, pictureType, filename, width, height, (XWPFDocument doc, CT_Blip blip) =>
		{
		});
	}

	public List<XWPFPicture> GetEmbeddedPictures()
	{
		return pictures;
	}

	public void SetStyle(string styleId)
	{
		CT_RPr cT_RPr = GetCTR().rPr;
		if (cT_RPr == null)
		{
			cT_RPr = GetCTR().AddNewRPr();
		}
		((cT_RPr.rStyle != null) ? cT_RPr.rStyle : cT_RPr.AddNewRStyle()).val = styleId;
	}

	public string GetStyle()
	{
		CT_RPr rPr = GetCTR().rPr;
		if (rPr == null)
		{
			return "";
		}
		CT_String rStyle = rPr.rStyle;
		if (rStyle == null)
		{
			return "";
		}
		return rStyle.val;
	}

	private static void preserveSpaces(CT_Text xs)
	{
		string value = xs.Value;
		if (value != null && value.Length >= 1 && (value.StartsWith(" ") || value.EndsWith(" ")))
		{
			xs.space = "preserve";
		}
	}

	public override string ToString()
	{
		return Text;
	}
}

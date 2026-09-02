using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace NPOI.XWPF.UserModel;

public class XWPFStyles : POIXMLDocumentPart
{
	private CT_Styles ctStyles;

	private List<XWPFStyle> listStyle = new List<XWPFStyle>();

	private XWPFLatentStyles latentStyles;

	private XWPFDefaultRunStyle defaultRunStyle;

	private XWPFDefaultParagraphStyle defaultParaStyle;

	public int NumberOfStyles => listStyle.Count;

	public XWPFDefaultRunStyle DefaultRunStyle
	{
		get
		{
			EnsureDocDefaults();
			return defaultRunStyle;
		}
	}

	public XWPFDefaultParagraphStyle DefaultParagraphStyle
	{
		get
		{
			EnsureDocDefaults();
			return defaultParaStyle;
		}
	}

	public XWPFLatentStyles LatentStyles => latentStyles;

	public XWPFStyles(PackagePart part)
		: base(part)
	{
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	public XWPFStyles(PackagePart part, PackageRelationship rel)
		: this(part)
	{
	}

	public XWPFStyles()
	{
	}

	internal override void OnDocumentRead()
	{
		Stream inputStream = GetPackagePart().GetInputStream();
		try
		{
			StylesDocument stylesDocument = StylesDocument.Parse(POIXMLDocumentPart.ConvertStreamToXml(inputStream), POIXMLDocumentPart.NamespaceManager);
			SetStyles(stylesDocument.Styles);
			latentStyles = new XWPFLatentStyles(ctStyles.latentStyles, this);
		}
		catch (XmlException ex)
		{
			throw new POIXMLException("Unable to read styles", ex);
		}
		finally
		{
			inputStream.Close();
		}
	}

	protected internal override void Commit()
	{
		if (ctStyles == null)
		{
			throw new InvalidOperationException("Unable to write out styles that were never read in!");
		}
		using Stream stream = GetPackagePart().GetOutputStream();
		new StylesDocument(ctStyles).Save(stream);
	}

	protected void EnsureDocDefaults()
	{
		if (!ctStyles.IsSetDocDefaults())
		{
			ctStyles.AddNewDocDefaults();
		}
		CT_DocDefaults docDefaults = ctStyles.docDefaults;
		if (!docDefaults.IsSetPPrDefault())
		{
			docDefaults.AddNewPPrDefault();
		}
		if (!docDefaults.IsSetRPrDefault())
		{
			docDefaults.AddNewRPrDefault();
		}
		CT_PPrDefault pPrDefault = docDefaults.pPrDefault;
		CT_RPrDefault rPrDefault = docDefaults.rPrDefault;
		if (!pPrDefault.IsSetPPr())
		{
			pPrDefault.AddNewPPr();
		}
		if (!rPrDefault.IsSetRPr())
		{
			rPrDefault.AddNewRPr();
		}
		defaultRunStyle = new XWPFDefaultRunStyle(rPrDefault.rPr);
		defaultParaStyle = new XWPFDefaultParagraphStyle(pPrDefault.pPr);
	}

	public void SetStyles(CT_Styles styles)
	{
		ctStyles = styles;
		foreach (CT_Style style in ctStyles.GetStyleList())
		{
			listStyle.Add(new XWPFStyle(style, this));
		}
		if (ctStyles.IsSetDocDefaults())
		{
			CT_DocDefaults docDefaults = ctStyles.docDefaults;
			if (docDefaults.IsSetRPrDefault() && docDefaults.rPrDefault.IsSetRPr())
			{
				defaultRunStyle = new XWPFDefaultRunStyle(docDefaults.rPrDefault.rPr);
			}
			if (docDefaults.IsSetPPrDefault() && docDefaults.pPrDefault.IsSetPPr())
			{
				defaultParaStyle = new XWPFDefaultParagraphStyle(docDefaults.pPrDefault.pPr);
			}
		}
	}

	public bool StyleExist(string styleID)
	{
		foreach (XWPFStyle item in listStyle)
		{
			if (item.StyleId.Equals(styleID))
			{
				return true;
			}
		}
		return false;
	}

	public void AddStyle(XWPFStyle style)
	{
		listStyle.Add(style);
		ctStyles.AddNewStyle();
		int pos = ctStyles.GetStyleList().Count - 1;
		ctStyles.SetStyleArray(pos, style.GetCTStyle());
	}

	public XWPFStyle GetStyle(string styleID)
	{
		foreach (XWPFStyle item in listStyle)
		{
			if (item.StyleId.Equals(styleID))
			{
				return item;
			}
		}
		return null;
	}

	public XWPFStyle GetStyleWithName(string styleName)
	{
		foreach (XWPFStyle item in listStyle)
		{
			if (item.Name == styleName)
			{
				return item;
			}
		}
		return null;
	}

	public List<XWPFStyle> GetUsedStyleList(XWPFStyle style)
	{
		List<XWPFStyle> list = new List<XWPFStyle>();
		list.Add(style);
		return GetUsedStyleList(style, list);
	}

	private List<XWPFStyle> GetUsedStyleList(XWPFStyle style, List<XWPFStyle> usedStyleList)
	{
		string basisStyleID = style.BasisStyleID;
		XWPFStyle style2 = GetStyle(basisStyleID);
		if (style2 != null && !usedStyleList.Contains(style2))
		{
			usedStyleList.Add(style2);
			GetUsedStyleList(style2, usedStyleList);
		}
		string linkStyleID = style.LinkStyleID;
		XWPFStyle style3 = GetStyle(linkStyleID);
		if (style3 != null && !usedStyleList.Contains(style3))
		{
			usedStyleList.Add(style3);
			GetUsedStyleList(style3, usedStyleList);
		}
		string nextStyleID = style.NextStyleID;
		XWPFStyle style4 = GetStyle(nextStyleID);
		if (style4 != null && !usedStyleList.Contains(style4))
		{
			usedStyleList.Add(style3);
			GetUsedStyleList(style3, usedStyleList);
		}
		return usedStyleList;
	}

	protected CT_Language GetCTLanguage()
	{
		EnsureDocDefaults();
		CT_Language cT_Language = null;
		if (defaultRunStyle.GetRPr().IsSetLang())
		{
			return defaultRunStyle.GetRPr().lang;
		}
		return defaultRunStyle.GetRPr().AddNewLang();
	}

	public void SetSpellingLanguage(string strSpellingLanguage)
	{
		CT_Language cTLanguage = GetCTLanguage();
		cTLanguage.val = strSpellingLanguage;
		cTLanguage.bidi = strSpellingLanguage;
	}

	public void SetEastAsia(string strEastAsia)
	{
		GetCTLanguage().eastAsia = strEastAsia;
	}

	public void SetDefaultFonts(CT_Fonts fonts)
	{
		EnsureDocDefaults();
		defaultRunStyle.GetRPr().rFonts = fonts;
	}

	public XWPFStyle GetStyleWithSameName(XWPFStyle style)
	{
		foreach (XWPFStyle item in listStyle)
		{
			if (item.HasSameName(style))
			{
				return item;
			}
		}
		return null;
	}
}

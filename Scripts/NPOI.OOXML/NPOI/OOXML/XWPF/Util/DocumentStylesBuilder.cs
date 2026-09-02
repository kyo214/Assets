using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;

namespace NPOI.OOXML.XWPF.Util;

public class DocumentStylesBuilder
{
	private XWPFStyles documentStyles;

	private CT_Styles ctStyles;

	public DocumentStylesBuilder()
	{
		documentStyles = new XWPFStyles();
		ctStyles = new CT_Styles();
	}

	public DocumentStylesBuilder(XWPFDocument docxDocument)
	{
		documentStyles = docxDocument.CreateStyles();
		ctStyles = new CT_Styles();
	}

	public void AddDefaultStyle()
	{
		CT_RPr cT_RPr = ctStyles.AddNewDocDefaults().AddNewRPrDefault().AddNewRPr();
		cT_RPr.AddNewSz().val = 24uL;
		cT_RPr.AddNewSzCs().val = 24uL;
		CT_Fonts cT_Fonts = cT_RPr.AddNewRFonts();
		cT_Fonts.asciiTheme = ST_Theme.minorAscii;
		cT_Fonts.cstheme = ST_Theme.minorBidi;
		cT_Fonts.eastAsiaTheme = ST_Theme.minorHAnsi;
		cT_Fonts.hAnsiTheme = ST_Theme.minorHAnsi;
	}

	public void AddCustomHeadingStyle(string name, int headingLevel, int outlineLevel, int ptSize = 12)
	{
		CT_Style cT_Style = ctStyles.AddNewStyle();
		cT_Style.styleId = name;
		cT_Style.name = new CT_String
		{
			val = name
		};
		cT_Style.uiPriority = new CT_DecimalNumber
		{
			val = headingLevel.ToString()
		};
		CT_OnOff qFormat = (cT_Style.unhideWhenUsed = new CT_OnOff());
		cT_Style.qFormat = qFormat;
		cT_Style.pPr = new CT_PPr
		{
			outlineLvl = new CT_DecimalNumber
			{
				val = outlineLevel.ToString()
			}
		};
		CT_RPr cT_RPr = new CT_RPr();
		cT_RPr.AddNewSz().val = (ulong)ptSize * 2uL;
		cT_Style.rPr = cT_RPr;
	}

	public CT_Styles Build()
	{
		documentStyles.SetStyles(ctStyles);
		return ctStyles;
	}

	public static CT_Styles BuildStylesForTOC(XWPFDocument doc = null)
	{
		DocumentStylesBuilder documentStylesBuilder = ((doc == null) ? new DocumentStylesBuilder() : new DocumentStylesBuilder(doc));
		documentStylesBuilder.AddDefaultStyle();
		documentStylesBuilder.AddCustomHeadingStyle("TOCHeading", 1, 9);
		documentStylesBuilder.AddCustomHeadingStyle("TOC1", 2, 0);
		documentStylesBuilder.AddCustomHeadingStyle("TOC2", 3, 0);
		documentStylesBuilder.AddCustomHeadingStyle("Heading1", 4, 0);
		documentStylesBuilder.AddCustomHeadingStyle("Heading2", 5, 1);
		return documentStylesBuilder.Build();
	}
}

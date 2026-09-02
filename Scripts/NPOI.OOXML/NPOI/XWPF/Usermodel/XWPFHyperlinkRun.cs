using NPOI.OpenXmlFormats.Wordprocessing;

namespace NPOI.XWPF.UserModel;

public class XWPFHyperlinkRun : XWPFRun
{
	private CT_Hyperlink1 hyperlink;

	public string Anchor => hyperlink.anchor;

	public string HyperlinkId
	{
		get
		{
			return hyperlink.id;
		}
		set
		{
			hyperlink.id = value;
		}
	}

	public XWPFHyperlinkRun(CT_Hyperlink1 hyperlink, CT_R Run, IRunBody p)
		: base(Run, p)
	{
		this.hyperlink = hyperlink;
	}

	public CT_Hyperlink1 GetCTHyperlink()
	{
		return hyperlink;
	}

	public XWPFHyperlink GetHyperlink(XWPFDocument document)
	{
		string hyperlinkId = HyperlinkId;
		if (hyperlinkId == null)
		{
			return null;
		}
		return document.GetHyperlinkByID(hyperlinkId);
	}
}

using NPOI.OpenXmlFormats.Wordprocessing;

namespace NPOI.XWPF.UserModel;

public class XWPFDefaultParagraphStyle
{
	private CT_PPr ppr;

	public int SpacingAfter
	{
		get
		{
			if (ppr.IsSetSpacing())
			{
				return (int)ppr.spacing.after.Value;
			}
			return -1;
		}
	}

	public XWPFDefaultParagraphStyle(CT_PPr ppr)
	{
		this.ppr = ppr;
	}

	protected internal CT_PPr GetPPr()
	{
		return ppr;
	}
}

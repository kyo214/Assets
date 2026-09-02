using NPOI.OpenXmlFormats.Wordprocessing;

namespace NPOI.XWPF.UserModel;

public class XWPFLatentStyles
{
	private CT_LatentStyles latentStyles;

	protected XWPFStyles styles;

	public int NumberOfStyles => latentStyles.SizeOfLsdExceptionArray();

	protected XWPFLatentStyles()
	{
	}

	public XWPFLatentStyles(CT_LatentStyles latentStyles)
		: this(latentStyles, null)
	{
	}

	public XWPFLatentStyles(CT_LatentStyles latentStyles, XWPFStyles styles)
	{
		this.latentStyles = latentStyles;
		this.styles = styles;
	}

	public bool IsLatentStyle(string latentStyleID)
	{
		foreach (CT_LsdException item in latentStyles.lsdException)
		{
			if (item.name.Equals(latentStyleID))
			{
				return true;
			}
		}
		return false;
	}
}

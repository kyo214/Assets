using NPOI.OpenXmlFormats.Wordprocessing;

namespace NPOI.XWPF.UserModel;

public class XWPFDefaultRunStyle
{
	private CT_RPr rpr;

	public int FontSize
	{
		get
		{
			if (rpr.IsSetSz())
			{
				return (int)rpr.sz.val / 2;
			}
			return -1;
		}
	}

	public XWPFDefaultRunStyle(CT_RPr rpr)
	{
		this.rpr = rpr;
	}

	protected internal CT_RPr GetRPr()
	{
		return rpr;
	}
}
